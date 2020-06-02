using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace life
{
    public partial class LifeForm : Form
    {
        /// <summary>
        /// Панель для отображения игрового поля.
        /// </summary>
        private PanelField panelField;

        public void InitPanelfield()
        {
            panelField = new PanelField();

            tlPanel.Controls.Add(panelField, 0, 1);
            tlPanel.SetColumnSpan(panelField, 2);
            tlPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            panelField.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            panelField.Name = "panelField";
            panelField.TabIndex = 3;

            panelField.Margin = Padding.Empty;

            panelField.Paint += PanelField_Paint;

            panelField.LostFocus += PanelField_LostFocus;

            panelField.MouseEnter += PanelField_MouseEnter;

            isSelectionMode = false;
        }
    }

    class PanelField : Panel
    {
        public PanelField():base()
        { 
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true); 
        }
    }
}
