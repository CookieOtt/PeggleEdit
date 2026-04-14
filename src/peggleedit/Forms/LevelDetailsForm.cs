// This file is part of PeggleEdit.
// Copyright Ted John 2010 - 2011. http://tedtycoon.co.uk
//
// PeggleEdit is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// PeggleEdit is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with PeggleEdit. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Windows.Forms;
using IntelOrca.PeggleEdit.Tools.Levels;
using IntelOrca.PeggleEdit.Tools.Pack;

namespace IntelOrca.PeggleEdit.Designer
{
    partial class LevelDetailsForm : Form
    {
        private Level mLevel;
        private LevelPack mPack;

        public LevelDetailsForm(Level level)
            : this(level, null)
        {
        }

        public LevelDetailsForm(Level level, LevelPack pack)
        {
            mLevel = level;
            mPack = pack;

            InitializeComponent();

            txtFilename.Text = mLevel.Info.Filename;
            txtName.Text = mLevel.Info.Name;
            txtAceScore.Text = mLevel.Info.AceScore.ToString();
            txtMinStage.Text = mLevel.Info.MinStage.ToString();

            pnlThumnail.BackgroundImage = mLevel.GetThumbnail();
            pnlThumnail.BackgroundImageLayout = ImageLayout.Center;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var filename = txtFilename.Text.Trim();
            var name = txtName.Text.Trim();

            if (filename.Length == 0)
            {
                MessageBox.Show("No filename specified.", "Properties", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (name.Length == 0)
            {
                MessageBox.Show("No display name specified.", "Properties", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (mPack != null && mPack.IsLevelFilenameInUse(filename, mLevel))
            {
                MessageBox.Show("Another level already uses this filename. Choose a unique filename before saving the level properties.", "Properties", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            int aceScore;
            if (!Int32.TryParse(txtAceScore.Text, out aceScore))
            {
                MessageBox.Show("Invalid ace score.", "Properties", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            int minStage;
            if (!Int32.TryParse(txtMinStage.Text, out minStage))
            {
                MessageBox.Show("Invalid minimum stage.", "Properties", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            LevelInfo info = new LevelInfo();
            info.Filename = filename;
            info.Name = name;
            info.AceScore = aceScore;
            info.MinStage = minStage;
            mLevel.Info = info;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public Level Level
        {
            get
            {
                return mLevel;
            }
        }
    }
}
