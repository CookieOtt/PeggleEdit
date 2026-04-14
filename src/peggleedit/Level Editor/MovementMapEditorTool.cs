using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using IntelOrca.PeggleEdit.Tools.Levels;
using IntelOrca.PeggleEdit.Tools.Levels.Children;

namespace IntelOrca.PeggleEdit.Designer.Level_Editor
{
    internal class MovementMapEditorTool : EditorTool
    {
        private readonly List<PointF> _points = new List<PointF>();
        private PointF _previewPoint;
        private bool _hasPreviewPoint;

        public override void Activate()
        {
            MainMDIForm.Instance.SetStatus("Map Path: click points. 2 points = line, 3+ = box. Shift snaps, Enter/right-click applies, Backspace removes, Esc cancels.");
            Editor.UpdateRedraw();
        }

        public override void Deactivate()
        {
            ClearPath();
            Editor.UpdateRedraw();
        }

        public override void MouseDown(MouseButtons button, Point location, Keys modifierKeys)
        {
            if (button == MouseButtons.Right)
            {
                ApplyPath();
                return;
            }

            if (button != MouseButtons.Left)
                return;

            if (Editor.SelectedEntries.Count == 0)
            {
                MainMDIForm.Instance.SetStatus("Select one or more objects before mapping movement.");
                return;
            }

            var point = GetMappedLocation(location, modifierKeys);
            _points.Add(point);
            _previewPoint = point;
            _hasPreviewPoint = true;

            if (_points.Count == 1)
            {
                MainMDIForm.Instance.SetStatus("First path point set. Add another for a line; add more for a box. Shift snaps, Esc cancels.");
            }
            else if (_points.Count == 2)
            {
                MainMDIForm.Instance.SetStatus("Second path point set. Enter/right-click applies a line; add more points for a box. Backspace removes.");
            }
            else
            {
                MainMDIForm.Instance.SetStatus("Box path points set. Enter/right-click applies, Backspace removes the last point, Esc cancels.");
            }

            Editor.UpdateRedraw();
        }

        public override void MouseMove(MouseButtons button, Point location, Keys modifierKeys)
        {
            if (_points.Count == 0)
                return;

            _previewPoint = GetMappedLocation(location, modifierKeys);
            _hasPreviewPoint = true;
            Editor.UpdateRedraw();
        }

        public override void KeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    ApplyPath();
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    ClearPath();
                    MainMDIForm.Instance.SetStatus("Movement path cancelled.");
                    Editor.UpdateRedraw();
                    e.Handled = true;
                    break;
                case Keys.Back:
                    RemoveLastPoint();
                    e.Handled = true;
                    break;
            }
        }

        public override void Draw(Graphics g)
        {
            if (_points.Count == 0)
                return;

            var origin = Editor.Level.GetActualXY(0, 0);
            var oldTransform = g.Transform;
            g.TranslateTransform(origin.X, origin.Y);

            using (var pen = new Pen(Color.Lime, 2))
            {
                pen.DashPattern = new float[] { 6.0f, 4.0f };
                foreach (LevelEntry entry in Editor.GetSelectedObjects())
                {
                    DrawMappedPath(g, pen, entry);
                }
            }

            g.Transform = oldTransform;
        }

        public override object Clone()
        {
            var tool = new MovementMapEditorTool();
            CloneTo(tool);
            return tool;
        }

        private PointF GetMappedLocation(Point location, Keys modifierKeys)
        {
            var result = (PointF)Editor.Level.GetVirtualXY(location);
            if ((modifierKeys & Keys.Shift) != 0 && _points.Count != 0)
            {
                var previousPoint = _points[_points.Count - 1];
                var offset = new PointF(result.X - previousPoint.X, result.Y - previousPoint.Y);
                var distance = Math.Sqrt((offset.X * offset.X) + (offset.Y * offset.Y));
                if (distance > 0)
                {
                    var angle = Math.Atan2(offset.Y, offset.X);
                    var snapAngle = Math.Round(angle / (Math.PI / 4)) * (Math.PI / 4);
                    result = new PointF(
                        previousPoint.X + (float)(Math.Cos(snapAngle) * distance),
                        previousPoint.Y + (float)(Math.Sin(snapAngle) * distance));
                }
            }
            return result;
        }

        private void ApplyPath()
        {
            if (Editor.SelectedEntries.Count == 0)
            {
                MainMDIForm.Instance.SetStatus("Select one or more objects before mapping movement.");
                return;
            }

            if (_points.Count < 2)
            {
                MainMDIForm.Instance.SetStatus("Set at least two path points before applying movement.");
                return;
            }

            Editor.CreateUndoPoint();
            foreach (LevelEntry entry in Editor.GetSelectedObjects())
            {
                if (_points.Count == 2)
                {
                    var offset = new PointF(_points[1].X - _points[0].X, _points[1].Y - _points[0].Y);
                    ApplyLineMovement(entry, offset);
                }
                else
                {
                    ApplyBoxMovement(entry);
                }
            }

            ClearPath();
            Editor.UpdateRedraw();
            MainMDIForm.Instance.UpdateProperties(Editor.GetSelectedObjects().ToArray());
            MainMDIForm.Instance.SetStatus("Movement path mapped. Two points makes a line; three or more makes a box. Turn on Preview to test it.");
        }

        private void ApplyLineMovement(LevelEntry entry, PointF offset)
        {
            var distance = Math.Sqrt((offset.X * offset.X) + (offset.Y * offset.Y));
            var radius = (short)Math.Max(1, Math.Min(short.MaxValue, Math.Round(distance / 2.0)));
            var angle = (float)(-Math.Atan2(offset.Y, offset.X) * 180.0 / Math.PI);
            var anchor = new PointF(entry.X + (offset.X / 2.0f), entry.Y + (offset.Y / 2.0f));

            var movement = entry.MovementLink?.OwnsMovement == true ?
                entry.MovementLink.Movement :
                new Movement(Editor.Level);

            movement.Location = anchor;
            movement.Type = MovementType.HorizontalCycle;
            movement.TimePeriod = 400;
            movement.Radius1 = radius;
            movement.Radius2 = 0;
            movement.MovementAngle = angle;

            if (entry.MovementLink?.OwnsMovement != true)
            {
                entry.MovementLink = new MovementLink(Editor.Level)
                {
                    Movement = movement
                };
            }
        }

        private void ApplyBoxMovement(LevelEntry entry)
        {
            var bounds = GetPointBounds();
            var radiusX = ClampToMovementRadius(bounds.Width / 2.0);
            var radiusY = ClampToMovementRadius(bounds.Height / 2.0);
            var topLeft = new PointF(bounds.Left - _points[0].X, bounds.Top - _points[0].Y);
            var boxStart = new PointF(entry.X + topLeft.X, entry.Y + topLeft.Y);
            var sideDuration = (short)100;
            var cycleMoveTime = (short)(sideDuration * 2);

            var movement = entry.MovementLink?.OwnsMovement == true ?
                entry.MovementLink.Movement :
                new Movement(Editor.Level);
            var verticalMovement = movement.MovementLink?.OwnsMovement == true ?
                movement.MovementLink.Movement :
                new Movement(Editor.Level);

            movement.Location = new PointF(boxStart.X + radiusX, boxStart.Y);
            movement.Type = MovementType.HorizontalCycle;
            movement.TimePeriod = cycleMoveTime;
            movement.Radius1 = radiusX;
            movement.Radius2 = 0;
            movement.MovementAngle = 0;
            movement.Phase = 0;
            movement.Phase1 = 50;
            movement.Pause1 = sideDuration;
            movement.Phase2 = 100;
            movement.Pause2 = sideDuration;

            verticalMovement.Location = new PointF(0, radiusY);
            verticalMovement.Type = MovementType.VerticalCycle;
            verticalMovement.TimePeriod = cycleMoveTime;
            verticalMovement.Radius1 = radiusY;
            verticalMovement.Radius2 = 0;
            verticalMovement.MovementAngle = 0;
            verticalMovement.Phase = 0.75f;
            verticalMovement.Phase1 = 0;
            verticalMovement.Pause1 = sideDuration;
            verticalMovement.Phase2 = 50;
            verticalMovement.Pause2 = sideDuration;

            if (movement.MovementLink?.OwnsMovement != true)
            {
                movement.MovementLink = new MovementLink(Editor.Level)
                {
                    Movement = verticalMovement
                };
            }

            if (entry.MovementLink?.OwnsMovement != true)
            {
                entry.MovementLink = new MovementLink(Editor.Level)
                {
                    Movement = movement
                };
            }

            AddSubMovementHelper(verticalMovement);
        }

        private RectangleF GetPointBounds()
        {
            return GetPointBounds(_points, false);
        }

        private RectangleF GetPreviewPointBounds()
        {
            return GetPointBounds(_points, true);
        }

        private RectangleF GetPointBounds(List<PointF> points, bool includePreview)
        {
            var minX = points[0].X;
            var minY = points[0].Y;
            var maxX = points[0].X;
            var maxY = points[0].Y;

            foreach (var point in points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
            }
            if (includePreview && _hasPreviewPoint)
            {
                minX = Math.Min(minX, _previewPoint.X);
                minY = Math.Min(minY, _previewPoint.Y);
                maxX = Math.Max(maxX, _previewPoint.X);
                maxY = Math.Max(maxY, _previewPoint.Y);
            }

            return RectangleF.FromLTRB(minX, minY, maxX, maxY);
        }

        private void DrawMappedPath(Graphics g, Pen pen, LevelEntry entry)
        {
            var previous = MapPointToEntry(entry, _points[0]);
            DrawHandle(g, previous);

            for (int i = 1; i < _points.Count; i++)
            {
                var next = MapPointToEntry(entry, _points[i]);
                g.DrawLine(pen, previous, next);
                DrawHandle(g, next);
                previous = next;
            }

            if (_hasPreviewPoint)
            {
                var preview = MapPointToEntry(entry, _previewPoint);
                g.DrawLine(pen, previous, preview);
                DrawHandle(g, preview);
            }

            if (_points.Count >= 3 || (_points.Count >= 2 && _hasPreviewPoint))
            {
                var bounds = MapBoundsToEntry(entry, GetPreviewPointBounds());
                if (bounds.Width > 0 && bounds.Height > 0)
                    g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width, bounds.Height);
            }
        }

        private PointF MapPointToEntry(LevelEntry entry, PointF point)
        {
            return new PointF(entry.X + point.X - _points[0].X, entry.Y + point.Y - _points[0].Y);
        }

        private RectangleF MapBoundsToEntry(LevelEntry entry, RectangleF bounds)
        {
            return new RectangleF(
                entry.X + bounds.X - _points[0].X,
                entry.Y + bounds.Y - _points[0].Y,
                bounds.Width,
                bounds.Height);
        }

        private void RemoveLastPoint()
        {
            if (_points.Count == 0)
                return;

            _points.RemoveAt(_points.Count - 1);
            _hasPreviewPoint = false;
            MainMDIForm.Instance.SetStatus(_points.Count == 0 ?
                "Movement path cleared." :
                "Last movement path point removed.");
            Editor.UpdateRedraw();
        }

        private void ClearPath()
        {
            _points.Clear();
            _hasPreviewPoint = false;
        }

        private void AddSubMovementHelper(Movement movement)
        {
            if (movement.HasEntryParent())
                return;

            var helper = new Circle(Editor.Level)
            {
                X = 0,
                Y = 0,
                Radius = 1,
                Visible = false,
                Collision = false,
                CanMove = true,
                MovementLink = new MovementLink(Editor.Level)
                {
                    InternalLinkId = movement.MUID,
                    InternalMovement = movement
                }
            };
            Editor.Level.Entries.Add(helper);
        }

        private static short ClampToMovementRadius(double value)
        {
            return (short)Math.Max(1, Math.Min(short.MaxValue, Math.Round(value)));
        }

        private static void DrawHandle(Graphics g, PointF point)
        {
            var rect = new RectangleF(point.X - 4, point.Y - 4, 8, 8);
            g.FillEllipse(Brushes.Lime, rect);
            g.DrawEllipse(Pens.Black, rect);
        }
    }
}
