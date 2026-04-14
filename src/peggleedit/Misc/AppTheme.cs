using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using IntelOrca.PeggleEdit.Tools.Levels;

namespace IntelOrca.PeggleEdit.Designer
{
    internal static class AppTheme
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

        public static readonly Color DarkBackColor = Color.FromArgb(32, 34, 37);
        public static readonly Color DarkControlColor = Color.FromArgb(43, 45, 49);
        public static readonly Color DarkInputColor = Color.FromArgb(24, 25, 28);
        public static readonly Color DarkForeColor = Color.FromArgb(230, 232, 235);

        public static void Apply(Control control)
        {
            if (control == null)
                return;

            if (Settings.Default.DarkMode)
                ApplyDark(control);
            else
                ApplyLight(control);

            Level.WorkspaceBackColor = Settings.Default.DarkMode ? Color.FromArgb(18, 19, 22) : SystemColors.AppWorkspace;
        }

        public static RibbonRenderer CreateRibbonRenderer()
        {
            var renderer = new RibbonProfessionalRenderer();
            if (Settings.Default.DarkMode)
                renderer.ColorTable = new DarkRibbonColorTable();
            return renderer;
        }

        private static void ApplyDark(Control control)
        {
            switch (control)
            {
                case TextBoxBase textBox:
                    textBox.BackColor = DarkInputColor;
                    textBox.ForeColor = DarkForeColor;
                    break;
                case ListBox listBox:
                    listBox.BackColor = DarkInputColor;
                    listBox.ForeColor = DarkForeColor;
                    break;
                case TreeView treeView:
                    treeView.BackColor = DarkInputColor;
                    treeView.ForeColor = DarkForeColor;
                    break;
                case PropertyGrid propertyGrid:
                    propertyGrid.BackColor = DarkControlColor;
                    propertyGrid.ForeColor = DarkForeColor;
                    propertyGrid.CategoryForeColor = DarkForeColor;
                    propertyGrid.CommandsBackColor = DarkControlColor;
                    propertyGrid.CommandsForeColor = DarkForeColor;
                    propertyGrid.HelpBackColor = DarkControlColor;
                    propertyGrid.HelpForeColor = DarkForeColor;
                    propertyGrid.LineColor = Color.FromArgb(70, 74, 80);
                    propertyGrid.ViewBackColor = DarkInputColor;
                    propertyGrid.ViewForeColor = DarkForeColor;
                    break;
                case ButtonBase button:
                    button.BackColor = DarkControlColor;
                    button.ForeColor = DarkForeColor;
                    button.UseVisualStyleBackColor = false;
                    break;
                case TabPage tabPage:
                    tabPage.BackColor = DarkBackColor;
                    tabPage.ForeColor = DarkForeColor;
                    break;
                default:
                    control.BackColor = DarkBackColor;
                    control.ForeColor = DarkForeColor;
                    break;
            }
            TrySetWindowTheme(control, "DarkMode_Explorer");

            foreach (Control child in control.Controls)
                ApplyDark(child);
        }

        private static void ApplyLight(Control control)
        {
            switch (control)
            {
                case PropertyGrid propertyGrid:
                    propertyGrid.BackColor = SystemColors.Control;
                    propertyGrid.ForeColor = SystemColors.ControlText;
                    propertyGrid.CategoryForeColor = SystemColors.ControlText;
                    propertyGrid.CommandsBackColor = SystemColors.Control;
                    propertyGrid.CommandsForeColor = SystemColors.ControlText;
                    propertyGrid.HelpBackColor = SystemColors.Control;
                    propertyGrid.HelpForeColor = SystemColors.ControlText;
                    propertyGrid.LineColor = SystemColors.InactiveBorder;
                    propertyGrid.ViewBackColor = SystemColors.Window;
                    propertyGrid.ViewForeColor = SystemColors.WindowText;
                    break;
                case TextBoxBase textBox:
                    textBox.BackColor = SystemColors.Window;
                    textBox.ForeColor = SystemColors.WindowText;
                    break;
                case ListBox listBox:
                    listBox.BackColor = SystemColors.Window;
                    listBox.ForeColor = SystemColors.WindowText;
                    break;
                case TreeView treeView:
                    treeView.BackColor = SystemColors.Window;
                    treeView.ForeColor = SystemColors.WindowText;
                    break;
                case ButtonBase button:
                    button.BackColor = SystemColors.Control;
                    button.ForeColor = SystemColors.ControlText;
                    button.UseVisualStyleBackColor = true;
                    break;
                case TabPage tabPage:
                    tabPage.BackColor = SystemColors.Control;
                    tabPage.ForeColor = SystemColors.ControlText;
                    break;
                default:
                    control.BackColor = SystemColors.Control;
                    control.ForeColor = SystemColors.ControlText;
                    break;
            }
            TrySetWindowTheme(control, "Explorer");

            foreach (Control child in control.Controls)
                ApplyLight(child);
        }

        private static void TrySetWindowTheme(Control control, string theme)
        {
            if (!control.IsHandleCreated)
                return;

            try
            {
                SetWindowTheme(control.Handle, theme, null);
            }
            catch
            {
            }
        }

        private sealed class DarkRibbonColorTable : RibbonProfesionalRendererColorTable
        {
            public DarkRibbonColorTable()
            {
                var background = Color.FromArgb(28, 30, 34);
                var panel = Color.FromArgb(39, 42, 48);
                var panelLight = Color.FromArgb(50, 54, 61);
                var border = Color.FromArgb(70, 74, 84);
                var hover = Color.FromArgb(63, 69, 78);
                var pressed = Color.FromArgb(80, 86, 96);
                var text = Color.FromArgb(232, 235, 238);
                var mutedText = Color.FromArgb(185, 190, 198);

                FormBorder = Color.FromArgb(18, 20, 23);

                Caption1 = background;
                Caption2 = background;
                Caption3 = background;
                Caption4 = background;
                Caption5 = background;
                Caption6 = background;
                Caption7 = Color.FromArgb(18, 20, 23);

                QuickAccessBorderDark = border;
                QuickAccessBorderLight = Color.FromArgb(85, 90, 100);
                QuickAccessUpper = panel;
                QuickAccessLower = background;

                OrbDropDownDarkBorder = border;
                OrbDropDownLightBorder = border;
                OrbDropDownBack = background;
                OrbDropDownNorthA = panelLight;
                OrbDropDownNorthB = panel;
                OrbDropDownNorthC = panel;
                OrbDropDownNorthD = background;
                OrbDropDownSouthC = background;
                OrbDropDownSouthD = panel;
                OrbDropDownContentbg = panel;
                OrbDropDownContentbglight = panelLight;
                OrbDropDownSeparatorlight = border;
                OrbDropDownSeparatordark = Color.FromArgb(18, 20, 23);

                OrbOptionBorder = border;
                OrbOptionBackground = hover;
                OrbOptionShine = panelLight;

                Arrow = text;
                ArrowLight = Color.FromArgb(245, 247, 249);
                ArrowDisabled = Color.FromArgb(120, 125, 133);
                Text = text;

                OrbBackgroundDark = Color.FromArgb(45, 49, 57);
                OrbBackgroundMedium = Color.FromArgb(61, 66, 75);
                OrbBackgroundLight = Color.FromArgb(76, 82, 92);
                OrbLight = Color.FromArgb(112, 120, 132);
                OrbSelectedBackgroundDark = Color.FromArgb(73, 78, 88);
                OrbSelectedBackgroundMedium = Color.FromArgb(88, 95, 108);
                OrbSelectedBackgroundLight = Color.FromArgb(106, 113, 126);
                OrbSelectedLight = Color.FromArgb(136, 146, 160);
                OrbPressedBackgroundDark = Color.FromArgb(22, 24, 27);
                OrbPressedBackgroundMedium = Color.FromArgb(40, 43, 49);
                OrbPressedBackgroundLight = Color.FromArgb(59, 64, 72);
                OrbPressedLight = Color.FromArgb(94, 101, 112);
                OrbBorderAero = border;

                RibbonBackground = background;
                TabBorder = border;
                TabNorth = panelLight;
                TabSouth = panel;
                TabGlow = Color.FromArgb(95, 105, 116);
                TabText = text;
                TabActiveText = text;
                TabContentNorth = panel;
                TabContentSouth = background;
                TabSelectedGlow = Color.FromArgb(84, 91, 102);

                PanelDarkBorder = border;
                PanelLightBorder = Color.FromArgb(60, 65, 74);
                PanelTextBackground = Color.FromArgb(33, 36, 41);
                PanelTextBackgroundSelected = Color.FromArgb(45, 49, 56);
                PanelText = mutedText;
                PanelBackgroundSelected = hover;
                PanelOverflowBackground = panelLight;
                PanelOverflowBackgroundPressed = pressed;
                PanelOverflowBackgroundSelectedNorth = hover;
                PanelOverflowBackgroundSelectedSouth = panel;

                ButtonBgOut = panel;
                ButtonBgCenter = panelLight;
                ButtonBorderOut = border;
                ButtonBorderIn = Color.FromArgb(82, 88, 100);
                ButtonGlossyNorth = Color.FromArgb(60, 65, 73);
                ButtonGlossySouth = panel;

                ButtonDisabledBgOut = Color.FromArgb(34, 36, 40);
                ButtonDisabledBgCenter = Color.FromArgb(38, 40, 44);
                ButtonDisabledBorderOut = Color.FromArgb(50, 53, 58);
                ButtonDisabledBorderIn = Color.FromArgb(44, 47, 52);
                ButtonDisabledGlossyNorth = Color.FromArgb(42, 45, 50);
                ButtonDisabledGlossySouth = Color.FromArgb(36, 38, 42);

                ButtonSelectedBgOut = hover;
                ButtonSelectedBgCenter = Color.FromArgb(77, 84, 96);
                ButtonSelectedBorderOut = Color.FromArgb(105, 113, 128);
                ButtonSelectedBorderIn = Color.FromArgb(88, 96, 110);
                ButtonSelectedGlossyNorth = Color.FromArgb(91, 99, 113);
                ButtonSelectedGlossySouth = hover;

                ButtonPressedBgOut = Color.FromArgb(70, 77, 89);
                ButtonPressedBgCenter = pressed;
                ButtonPressedBorderOut = Color.FromArgb(124, 134, 152);
                ButtonPressedBorderIn = Color.FromArgb(104, 113, 130);
                ButtonPressedGlossyNorth = Color.FromArgb(96, 105, 121);
                ButtonPressedGlossySouth = Color.FromArgb(69, 75, 87);

                ButtonCheckedBgOut = Color.FromArgb(64, 71, 83);
                ButtonCheckedBgCenter = Color.FromArgb(82, 91, 105);
                ButtonCheckedBorderOut = Color.FromArgb(122, 132, 150);
                ButtonCheckedBorderIn = Color.FromArgb(101, 111, 128);
                ButtonCheckedGlossyNorth = Color.FromArgb(96, 105, 121);
                ButtonCheckedGlossySouth = Color.FromArgb(72, 79, 92);

                ItemGroupOuterBorder = border;
                ItemGroupInnerBorder = Color.FromArgb(45, 49, 56);
                ItemGroupSeparatorLight = Color.FromArgb(68, 73, 82);
                ItemGroupSeparatorDark = Color.FromArgb(28, 30, 34);
                ItemGroupBgNorth = Color.FromArgb(44, 48, 55);
                ItemGroupBgSouth = panel;
                ItemGroupBgGlossy = Color.FromArgb(54, 59, 67);

                ButtonListBorder = border;
                ButtonListBg = panel;
                ButtonListBgSelected = hover;

                DropDownBg = panel;
                DropDownImageBg = Color.FromArgb(33, 36, 41);
                DropDownImageSeparator = border;
                DropDownBorder = border;
                DropDownGripNorth = panelLight;
                DropDownGripSouth = background;
                DropDownGripBorder = border;
                DropDownGripDark = Color.FromArgb(18, 20, 23);
                DropDownGripLight = Color.FromArgb(80, 86, 96);

                SeparatorLight = Color.FromArgb(68, 73, 82);
                SeparatorDark = Color.FromArgb(18, 20, 23);
                SeparatorBg = background;
                SeparatorLine = border;

                TextBoxUnselectedBg = DarkInputColor;
                TextBoxBorder = border;
            }
        }
    }
}
