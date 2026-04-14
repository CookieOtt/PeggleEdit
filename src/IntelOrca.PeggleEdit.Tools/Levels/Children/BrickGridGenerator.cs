using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace IntelOrca.PeggleEdit.Tools.Levels.Children
{
    public class BrickGridGenerator : LevelEntry, ICloneable, IEntryFunction
    {
        private int mColumns = 8;
        private int mRows = 5;
        private float mColumnSpacing = 38.0f;
        private float mRowSpacing = 30.0f;
        private float mAlternateRowOffset = 0.0f;
        private float mBrickLength = 30.0f;
        private float mBrickWidth = 20.0f;
        private float mBrickRotation = 90.0f;

        public BrickGridGenerator(Level level)
            : base(level)
        {
        }

        public void Execute()
        {
            foreach (var point in GetPoints())
            {
                var brick = CreateBrick(point);
                Level.Entries.Add(brick);
            }

            Level.Entries.Remove(this);
        }

        public override void ReadData(BinaryReader br, int version)
        {
            byte hl = br.ReadByte();
            if (hl > 0)
            {
                X = br.ReadSingle();
                Y = br.ReadSingle();
            }

            mColumns = br.ReadInt32();
            mRows = br.ReadInt32();
            mColumnSpacing = br.ReadSingle();
            mRowSpacing = br.ReadSingle();
            mAlternateRowOffset = br.ReadSingle();
            mBrickLength = br.ReadSingle();
            mBrickWidth = br.ReadSingle();
            mBrickRotation = br.ReadSingle();
        }

        public override void WriteData(BinaryWriter bw, int version)
        {
            if (!HasMovementInfo)
            {
                bw.Write((byte)1);
                bw.Write(X);
                bw.Write(Y);
            }
            else
            {
                bw.Write((byte)0);
            }

            bw.Write(mColumns);
            bw.Write(mRows);
            bw.Write(mColumnSpacing);
            bw.Write(mRowSpacing);
            bw.Write(mAlternateRowOffset);
            bw.Write(mBrickLength);
            bw.Write(mBrickWidth);
            bw.Write(mBrickRotation);
        }

        public override void Draw(Graphics g)
        {
            if (Level.ShowCollision)
                return;

            base.Draw(g);

            using (var brush = new SolidBrush(Color.FromArgb(160, 234, 140, 22)))
            using (var innerBrush = new SolidBrush(Color.FromArgb(160, 80, 0, 0)))
            using (var pen = new Pen(Color.Black))
            {
                pen.DashStyle = DashStyle.Custom;
                pen.DashPattern = new float[] { 2, 4 };

                g.DrawRectangle(pen, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
                foreach (var point in GetPoints())
                {
                    DrawBrickPreview(g, brush, innerBrush, pen, point);
                }
            }
        }

        public override object Clone()
        {
            var copy = new BrickGridGenerator(Level);
            base.CloneTo(copy);
            copy.mColumns = mColumns;
            copy.mRows = mRows;
            copy.mColumnSpacing = mColumnSpacing;
            copy.mRowSpacing = mRowSpacing;
            copy.mAlternateRowOffset = mAlternateRowOffset;
            copy.mBrickLength = mBrickLength;
            copy.mBrickWidth = mBrickWidth;
            copy.mBrickRotation = mBrickRotation;
            return copy;
        }

        private Brick CreateBrick(PointF point)
        {
            var brick = new Brick(Level);
            brick.PegInfo = new PegInfo(brick, true, false);
            brick.Location = point.Round();
            brick.Length = mBrickLength;
            brick.Width = mBrickWidth;
            brick.Rotation = mBrickRotation;
            brick.Curved = false;
            return brick;
        }

        private PointF[] GetPoints()
        {
            var result = new PointF[Math.Max(0, mColumns) * Math.Max(0, mRows)];
            var index = 0;
            var origin = GetGridOrigin();
            for (var row = 0; row < mRows; row++)
            {
                var rowOffset = IsOffsetRow(row) ? mAlternateRowOffset : 0.0f;
                for (var column = 0; column < mColumns; column++)
                {
                    result[index++] = new PointF(
                        origin.X + (column * mColumnSpacing) + rowOffset,
                        origin.Y + (row * mRowSpacing));
                }
            }
            return result;
        }

        private PointF GetGridOrigin()
        {
            return new PointF(
                DrawX - ((Math.Max(0, mColumns - 1) * mColumnSpacing) / 2.0f),
                DrawY - ((Math.Max(0, mRows - 1) * mRowSpacing) / 2.0f));
        }

        private void DrawBrickPreview(Graphics g, Brush brush, Brush innerBrush, Pen pen, PointF point)
        {
            var oldTransform = g.Transform;
            var matrix = g.Transform;
            matrix.RotateAt(-mBrickRotation + 90.0f, point);
            g.Transform = matrix;

            var rect = new RectangleF(point.X - (mBrickLength / 2.0f), point.Y - (mBrickWidth / 2.0f), mBrickLength, mBrickWidth);
            g.FillRectangle(brush, rect);
            rect.Inflate(-2.0f, -5.0f);
            g.FillRectangle(innerBrush, rect);
            rect.Inflate(2.0f, 5.0f);
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

            g.Transform = oldTransform;
        }

        private static bool IsOffsetRow(int row) => row % 2 == 1;

        [Category("Grid")]
        [DefaultValue(8)]
        public int Columns
        {
            get => mColumns;
            set => mColumns = Math.Max(1, value);
        }

        [Category("Grid")]
        [DefaultValue(5)]
        public int Rows
        {
            get => mRows;
            set => mRows = Math.Max(1, value);
        }

        [DisplayName("Column Spacing")]
        [Category("Grid")]
        [DefaultValue(38.0f)]
        public float ColumnSpacing
        {
            get => mColumnSpacing;
            set => mColumnSpacing = Math.Max(1.0f, value);
        }

        [DisplayName("Row Spacing")]
        [Category("Grid")]
        [DefaultValue(30.0f)]
        public float RowSpacing
        {
            get => mRowSpacing;
            set => mRowSpacing = Math.Max(1.0f, value);
        }

        [DisplayName("Alternate Row Offset")]
        [Description("Horizontal offset applied to every other row.")]
        [Category("Grid")]
        [DefaultValue(0.0f)]
        public float AlternateRowOffset
        {
            get => mAlternateRowOffset;
            set => mAlternateRowOffset = value;
        }

        [DisplayName("Brick Length")]
        [Category("Bricks")]
        [DefaultValue(30.0f)]
        public float BrickLength
        {
            get => mBrickLength;
            set => mBrickLength = Math.Max(1.0f, value);
        }

        [DisplayName("Brick Width")]
        [Category("Bricks")]
        [DefaultValue(20.0f)]
        public float BrickWidth
        {
            get => mBrickWidth;
            set => mBrickWidth = Math.Max(1.0f, value);
        }

        [DisplayName("Brick Rotation")]
        [Category("Bricks")]
        [DefaultValue(90.0f)]
        public float BrickRotation
        {
            get => mBrickRotation;
            set => mBrickRotation = value;
        }

        public override int Type => LevelEntryTypes.BrickGridGenerator;

        public override RectangleF Bounds
        {
            get
            {
                var inflate = Math.Max(mBrickLength, mBrickWidth) / 2.0f;
                var minX = float.MaxValue;
                var minY = float.MaxValue;
                var maxX = float.MinValue;
                var maxY = float.MinValue;
                foreach (var point in GetPoints())
                {
                    minX = Math.Min(minX, point.X - inflate);
                    minY = Math.Min(minY, point.Y - inflate);
                    maxX = Math.Max(maxX, point.X + inflate);
                    maxY = Math.Max(maxY, point.Y + inflate);
                }
                return RectangleF.FromLTRB(minX, minY, maxX, maxY);
            }
        }
    }
}
