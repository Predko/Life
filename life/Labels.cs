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
        Label lbCount;

        protected void InitLabels()
        {
            lbCount = new Label()
            {
                //Location = new Point(btnLoadField.Location.X + btnLoadField.Size.Width + 10, 2),
                Text = "Step: 00000",
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true,
                Name = nameof(lbCount)
            };

            Controls.Add(lbCount);

            // Корректируем положение lbCount по Y - по оси кнопок
            lbCount.Location = new Point
            {
                X = btnLoadField.Location.X + btnLoadField.Size.Width + 10,
                Y = btnLoadField.Location.Y + btnLoadField.Size.Height / 2 - lbCount.Size.Height / 2
            };
        }
    }
}
