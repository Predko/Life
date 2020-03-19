using System;
using System.Windows.Forms;

namespace life
{
    public partial class LifeForm : Form
    {

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                Button btn = sender as Button;

                switch (btn.Name)
                {
                    case name_btnStartStop:

                        if (btnStartStop.Text == text_start)
                        {
                            timer.Start();

                            btnStartStop.Text = text_stop;
                            btnMakeAStep.Enabled = false;
                        }
                        else
                        {
                            timer.Stop();

                            btnStartStop.Text = text_start;
                            btnMakeAStep.Enabled = true;
                        }

                        break;

                    case name_btnMakeAStep:

                        field.CalcNextStep();

                        Count++;

                        Invalidate();
                        break;
                }

            }
        }


        private void BtnMakeAStep_Click(object sender, EventArgs e)
        {
            field.CalcNextStep();

            Count++;

            Invalidate();
        }

        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            if (btnStartStop.Text == text_start)
            {
                timer.Start();

                btnStartStop.Text = text_stop;
                btnMakeAStep.Enabled = false;
            }
            else
            {
                timer.Stop();

                btnStartStop.Text = text_start;
                btnMakeAStep.Enabled = true;
            }
        }

        private void LifeForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                if (timer.Enabled)
                {
                    timer.Enabled = false;
                }
                else
                {
                    timer.Enabled = true;
                }
                field.CalcNextStep();

                Count++;

                Invalidate();
            }
            else
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            field.CalcNextStep();

            Count++;

            lbCount.Text = $"Step: {Count}";

            Invalidate();
        }
    }
}
