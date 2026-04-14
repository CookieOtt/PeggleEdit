using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using IntelOrca.PeggleEdit.Designer.Properties;
using IntelOrca.PeggleEdit.Tools.Levels;

namespace IntelOrca.PeggleEdit.Designer
{
    internal class BackgroundImportForm : Form
    {
        private const int OutputWidth = 800;
        private const int OutputHeight = 600;

        private readonly Image _sourceImage;
        private readonly PreviewPanel _previewPanel;
        private readonly NumericUpDown _scaleInput;
        private readonly Button _fitButton;
        private readonly Button _fillButton;
        private readonly Button _centerButton;
        private readonly CheckBox _interfaceCheckBox;

        private float _scale;
        private PointF _offset;
        private bool _dragging;
        private Point _lastMousePosition;

        public BackgroundImportForm(Image sourceImage)
        {
            _sourceImage = (Image)sourceImage.Clone();

            Icon = Icon.FromHandle(Resources.image_16.GetHicon());
            Text = "Set Background";
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(760, 620);
            ClientSize = new Size(900, 700);

            _previewPanel = new PreviewPanel();
            _previewPanel.Dock = DockStyle.Fill;
            _previewPanel.BackColor = Color.FromArgb(32, 32, 32);
            _previewPanel.Paint += previewPanel_Paint;
            _previewPanel.MouseDown += previewPanel_MouseDown;
            _previewPanel.MouseMove += previewPanel_MouseMove;
            _previewPanel.MouseUp += previewPanel_MouseUp;
            _previewPanel.MouseWheel += previewPanel_MouseWheel;

            _scaleInput = new NumericUpDown();
            _scaleInput.DecimalPlaces = 1;
            _scaleInput.Minimum = 1;
            _scaleInput.Maximum = 800;
            _scaleInput.Increment = 5;
            _scaleInput.Width = 70;
            _scaleInput.ValueChanged += scaleInput_ValueChanged;

            _fitButton = new Button { Text = "Fit", Width = 70 };
            _fitButton.Click += (sender, e) => SetScaleAndCenter(GetFitScale());

            _fillButton = new Button { Text = "Fill", Width = 70 };
            _fillButton.Click += (sender, e) => SetScaleAndCenter(GetFillScale());

            _centerButton = new Button { Text = "Center", Width = 70 };
            _centerButton.Click += (sender, e) => CenterImage();

            _interfaceCheckBox = new CheckBox { Text = "Show UI overlay", Checked = true, AutoSize = true, Padding = new Padding(12, 5, 0, 0) };
            _interfaceCheckBox.CheckedChanged += (sender, e) => _previewPanel.Invalidate();

            var okButton = new Button { Text = "OK", Width = 80, DialogResult = DialogResult.OK };
            var cancelButton = new Button { Text = "Cancel", Width = 80, DialogResult = DialogResult.Cancel };

            var controlsPanel = new FlowLayoutPanel();
            controlsPanel.Dock = DockStyle.Bottom;
            controlsPanel.Height = 44;
            controlsPanel.Padding = new Padding(8);
            controlsPanel.FlowDirection = FlowDirection.LeftToRight;
            controlsPanel.Controls.Add(new Label { Text = "Scale", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 6, 0, 0) });
            controlsPanel.Controls.Add(_scaleInput);
            controlsPanel.Controls.Add(_fitButton);
            controlsPanel.Controls.Add(_fillButton);
            controlsPanel.Controls.Add(_centerButton);
            controlsPanel.Controls.Add(_interfaceCheckBox);
            controlsPanel.Controls.Add(new Label { Text = "Drag the image. Mouse wheel resizes.", AutoSize = true, Padding = new Padding(12, 6, 0, 0) });

            var buttonsPanel = new FlowLayoutPanel();
            buttonsPanel.Dock = DockStyle.Bottom;
            buttonsPanel.Height = 44;
            buttonsPanel.Padding = new Padding(8);
            buttonsPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonsPanel.Controls.Add(cancelButton);
            buttonsPanel.Controls.Add(okButton);

            Controls.Add(_previewPanel);
            Controls.Add(controlsPanel);
            Controls.Add(buttonsPanel);

            AcceptButton = okButton;
            CancelButton = cancelButton;

            SetScaleAndCenter(GetFillScale());
            AppTheme.Apply(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _sourceImage.Dispose();

            base.Dispose(disposing);
        }

        public Bitmap CreateBackground()
        {
            var result = new Bitmap(OutputWidth, OutputHeight);
            using (var g = Graphics.FromImage(result))
            {
                g.Clear(Color.Black);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(_sourceImage, GetImageRectangle(), new Rectangle(0, 0, _sourceImage.Width, _sourceImage.Height), GraphicsUnit.Pixel);
            }
            return result;
        }

        private void previewPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.Clear(_previewPanel.BackColor);

            var viewport = GetViewportRectangle();
            e.Graphics.FillRectangle(Brushes.Black, viewport);

            var imageRect = GetPreviewImageRectangle(viewport);
            var state = e.Graphics.Save();
            e.Graphics.SetClip(viewport);
            e.Graphics.DrawImage(_sourceImage, imageRect);
            if (_interfaceCheckBox.Checked)
                e.Graphics.DrawImage(Level.InterfaceImage, viewport);
            e.Graphics.Restore(state);
            e.Graphics.DrawRectangle(Pens.White, viewport);
        }

        private void previewPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            _dragging = true;
            _lastMousePosition = e.Location;
            _previewPanel.Focus();
        }

        private void previewPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging)
                return;

            var viewport = GetViewportRectangle();
            var canvasScale = viewport.Width / (float)OutputWidth;
            _offset.X += (e.X - _lastMousePosition.X) / canvasScale;
            _offset.Y += (e.Y - _lastMousePosition.Y) / canvasScale;
            _lastMousePosition = e.Location;
            _previewPanel.Invalidate();
        }

        private void previewPanel_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void previewPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            var factor = e.Delta > 0 ? 1.05f : 0.95f;
            SetScale(_scale * factor);
        }

        private void scaleInput_ValueChanged(object sender, EventArgs e)
        {
            SetScale((float)_scaleInput.Value / 100.0f, false);
        }

        private void SetScale(float scale, bool updateInput = true)
        {
            _scale = Math.Max(0.01f, Math.Min(8.0f, scale));
            if (updateInput)
            {
                _scaleInput.ValueChanged -= scaleInput_ValueChanged;
                _scaleInput.Value = (decimal)Math.Max((float)_scaleInput.Minimum, Math.Min((float)_scaleInput.Maximum, _scale * 100.0f));
                _scaleInput.ValueChanged += scaleInput_ValueChanged;
            }
            _previewPanel.Invalidate();
        }

        private void CenterImage()
        {
            var imageWidth = _sourceImage.Width * _scale;
            var imageHeight = _sourceImage.Height * _scale;
            _offset = new PointF((OutputWidth - imageWidth) / 2.0f, (OutputHeight - imageHeight) / 2.0f);
            _previewPanel.Invalidate();
        }

        private void SetScaleAndCenter(float scale)
        {
            SetScale(scale);
            CenterImage();
        }

        private float GetFitScale()
        {
            return Math.Min(OutputWidth / (float)_sourceImage.Width, OutputHeight / (float)_sourceImage.Height);
        }

        private float GetFillScale()
        {
            return Math.Max(OutputWidth / (float)_sourceImage.Width, OutputHeight / (float)_sourceImage.Height);
        }

        private RectangleF GetImageRectangle()
        {
            return new RectangleF(_offset.X, _offset.Y, _sourceImage.Width * _scale, _sourceImage.Height * _scale);
        }

        private Rectangle GetViewportRectangle()
        {
            var margin = 16;
            var maxWidth = Math.Max(1, _previewPanel.ClientSize.Width - (margin * 2));
            var maxHeight = Math.Max(1, _previewPanel.ClientSize.Height - (margin * 2));
            var previewScale = Math.Min(maxWidth / (float)OutputWidth, maxHeight / (float)OutputHeight);
            var width = (int)(OutputWidth * previewScale);
            var height = (int)(OutputHeight * previewScale);
            return new Rectangle((_previewPanel.ClientSize.Width - width) / 2, (_previewPanel.ClientSize.Height - height) / 2, width, height);
        }

        private RectangleF GetPreviewImageRectangle(Rectangle viewport)
        {
            var imageRect = GetImageRectangle();
            var canvasScale = viewport.Width / (float)OutputWidth;
            return new RectangleF(
                viewport.X + (imageRect.X * canvasScale),
                viewport.Y + (imageRect.Y * canvasScale),
                imageRect.Width * canvasScale,
                imageRect.Height * canvasScale);
        }

        private sealed class PreviewPanel : Panel
        {
            public PreviewPanel()
            {
                DoubleBuffered = true;
                TabStop = true;
            }
        }
    }
}
