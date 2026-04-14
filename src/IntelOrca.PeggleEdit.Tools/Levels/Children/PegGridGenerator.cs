using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace IntelOrca.PeggleEdit.Tools.Levels.Children
{
    public class PegGridGenerator : LevelEntry, ICloneable, IEntryFunction
    {
        private int mColumns = 8;
        private int mRows = 5;
        private float mColumnSpacing = 30.0f;
        private float mRowSpacing = 30.0f;
        private float mAlternateRowOffset = 0.0f;

        public PegGridGenerator(Level level)
            : base(level)
        {
        }

        public void Execute()
        {
            foreach (var point in GetPoints())
            {
                var peg = new Circle(Level);
                peg.PegInfo = new PegInfo(peg, true, false);
                peg.Location = point.Round();
                Level.Entries.Add(peg);
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
        }

        public override void Draw(Graphics g)
        {
            if (Level.ShowCollision)
                return;

            base.Draw(g);

            using (var pegBrush = new SolidBrush(Color.FromArgb(128, Color.Orange)))
            using (var pen = new Pen(Color.Black))
            {
                pen.DashStyle = DashStyle.Custom;
                pen.DashPattern = new float[] { 2, 4 };

                g.DrawRectangle(pen, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
                foreach (var point in GetPoints())
                {
                    g.FillEllipse(pegBrush, point.X - 10.0f, point.Y - 10.0f, 20.0f, 20.0f);
                    g.DrawEllipse(pen, point.X - 10.0f, point.Y - 10.0f, 20.0f, 20.0f);
                }
            }
        }

        public override object Clone()
        {
            var copy = new PegGridGenerator(Level);
            base.CloneTo(copy);
            copy.mColumns = mColumns;
            copy.mRows = mRows;
            copy.mColumnSpacing = mColumnSpacing;
            copy.mRowSpacing = mRowSpacing;
            copy.mAlternateRowOffset = mAlternateRowOffset;
            return copy;
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
        [DefaultValue(30.0f)]
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
        [Description("Horizontal offset applied to every other row. Use half the column spacing for a honeycomb-style grid.")]
        [Category("Grid")]
        [DefaultValue(0.0f)]
        public float AlternateRowOffset
        {
            get => mAlternateRowOffset;
            set => mAlternateRowOffset = value;
        }

        public override int Type => LevelEntryTypes.PegGridGenerator;

        public override RectangleF Bounds
        {
            get
            {
                var minX = float.MaxValue;
                var minY = float.MaxValue;
                var maxX = float.MinValue;
                var maxY = float.MinValue;
                foreach (var point in GetPoints())
                {
                    minX = Math.Min(minX, point.X - 10.0f);
                    minY = Math.Min(minY, point.Y - 10.0f);
                    maxX = Math.Max(maxX, point.X + 10.0f);
                    maxY = Math.Max(maxY, point.Y + 10.0f);
                }
                return RectangleF.FromLTRB(minX, minY, maxX, maxY);
            }
        }
    }
}
