using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Drawing;

namespace life
{
    public partial class LifeForm : Form
    {
        private Button btnStartStop;

        private Button btnMakeAStep;

        private Button btnPreviousStep;

        private Button btnSaveField;

        private Button btnLoadField;

        private const string text_start = "Start";
        private const string text_stop = "Stop";

        protected void InitButtons()
        {

            btnStartStop = new Button
            {
                Location = new Point(2, 2),
                Text = text_start,
                AutoSize = true,
                Name = "btnStartStop",
                TabIndex = 1
            };

            btnStartStop.Click += BtnStartStop_Click;

            btnMakeAStep = new Button
            {
                Location = new Point(btnStartStop.Location.X + btnStartStop.Size.Width + 10, 2),
                Text = "Make a step",
                AutoSize = true,
                Name = "MakeAStep",
                TabIndex = 2
            };

            btnMakeAStep.Click += BtnMakeAStep_Click;

            btnPreviousStep = new Button
            {
                Location = new Point(btnMakeAStep.Location.X + btnMakeAStep.Size.Width + 10, 2),
                Text = "Previous step",
                AutoSize = true,
                Name = "PreviousStep",
                TabIndex = 3,
                Enabled = false
            };

            btnPreviousStep.Click += BtnPreviousStep_Click;

            btnSaveField = new Button
            {
                Location = new Point(btnPreviousStep.Location.X + btnPreviousStep.Size.Width + 10, 2),
                Text = "Save Field",
                AutoSize = true,
                Name = "SaveField",
                TabIndex = 4
            };

            btnSaveField.Click += BtnSaveField_Click;

            btnLoadField = new Button
            {
                Location = new Point(btnSaveField.Location.X + btnSaveField.Size.Width + 10, 2),
                Text = "Load Field",
                AutoSize = true,
                Name = "LoadField",
                TabIndex = 5
            };

            btnLoadField.Click += BtnLoadField_Click;

            Controls.Add(btnStartStop);
            Controls.Add(btnMakeAStep);
            Controls.Add(btnPreviousStep);
            Controls.Add(btnSaveField);
            Controls.Add(btnLoadField);
        }
    }
}
