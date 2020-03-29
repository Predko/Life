using System;
using System.Windows.Forms;

namespace life
{
    public partial class LifeForm : Form
    {
        private void BtnStartStop_Click(object sender, EventArgs e)
        {
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
        }

        private void BtnMakeAStep_Click(object sender, EventArgs e)
        {
            field.CalcNextStep();

            Count++;

            btnPreviousStep.Enabled = true;

            Invalidate();
        }

        private void BtnPreviousStep_Click(object sender, EventArgs e)
        {
            if (field.IsLogEmpty())
            {
                return;
            }

            if (!field.PreviousStep()) // если лог пуст, делаем неактивной кнопку 
            {
                btnPreviousStep.Enabled = false;
            }

            Count--;

            Invalidate();
        }

        private void BtnSaveField_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            field.Save(saveFileDialog.FileName);
        }

        private void BtnLoadField_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            field.Load(openFileDialog.FileName);

            field.Draw();

            btnPreviousStep.Enabled = false;

            Count = 0;

            Invalidate();
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
