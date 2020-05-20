using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Drawing;

namespace life
{
    public partial class LifeForm : Form
    {
        private const string text_start = "Start";
        private const string text_stop = "Stop";

        protected void InitButtons()
        {
            btnNewGame.Click += BtnNewGame_Click;
            
            btnStartStop.Click += BtnStartStop_Click;

            btnMakeAStep.Click += BtnMakeAStep_Click;

            btnPreviousStep.Click += BtnPreviousStep_Click;

            btnLoadField.Click += BtnLoadField_Click;

            btnSaveField.Click += BtnSaveField_Click;
        }
    }
}
