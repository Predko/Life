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
        private PanelField panelField;

        public void InitPanelfield()
        {
            panelField = new PanelField();

            tlPanel.Controls.Add(panelField, 0, 1);

            panelField.Dock = DockStyle.Fill;
            tlPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelField.Name = "panelField";
            panelField.TabIndex = 3;

            panelField.Paint += PanelField_Paint;
        }

        private void PanelField_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            field.Redraw(e.Graphics);
        }
    }

    class PanelField : Panel
    {
        public PanelField()
        { 
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true); 
        }
    }
}
