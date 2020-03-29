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
                            btnPreviousStep.Enabled = false;
                            btnSaveField.Enabled = false;
                            btnLoadField.Enabled = false;
                        }
                        else
                        {
                            timer.Stop();

                            btnStartStop.Text = text_start;
                            btnMakeAStep.Enabled = true;
                            btnPreviousStep.Enabled = true;
                            btnSaveField.Enabled = true;
                            btnLoadField.Enabled = true;
                        }

                        break;

                    case name_btnMakeAStep:

                        field.CalcNextStep();

                        Count++;

                        btnPreviousStep.Enabled = true;

                        Invalidate();
                        break;

                    case name_btnPreviousStep:

                        if (field.IsLogEmpty())
                        {
                            break;
                        }
                        
                        if (!field.PreviousStep()) // если лог пуст, делаем неактивной кнопку 
                        {
                            btnPreviousStep.Enabled = false;
                        }

                        Count--;

                        Invalidate();
                        break;

                    case name_btnSaveField:

                        field.Save();
                        break;

                    case name_btnLoadField:

                        field.Load();

                        field.Draw();

                        btnPreviousStep.Enabled = false;

                        Count = 0;

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
