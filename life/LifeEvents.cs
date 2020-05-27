using System;
using System.Windows.Forms;

namespace life
{
    public partial class LifeForm : Form
    {
        private void BtnNewGame_Click(object sender, EventArgs e)
        {
            if (settingField.ShowDialog() == DialogResult.OK)
            {
                field.density = settingField.Density;
                field.isBorder = settingField.IsBorder;
                
                NewField(settingField.SizeField.Width, settingField.SizeField.Height, settingField.Density, settingField.IsBorder);

                panelField.Invalidate();
            }
        }

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

            panelField.Invalidate();
        }

        private void BtnPreviousStep_Click(object sender, EventArgs e)
        {
            if (field.IsLogEmpty())
            {
                return;
            }

            if (!field.PreviousStep()) // если лог пуст или была ошибка, делаем неактивной кнопку 
            {
                btnPreviousStep.Enabled = false;

                Count = 0;
            }
            else
            {
                Count--;
            }

            panelField.Invalidate();
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
            LoadField();

            panelField.Invalidate();
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

                panelField.Invalidate();
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

            lbCount.Text = $"Step: {Count,5:d}";

            panelField.Invalidate();
        }

        /// <summary>
        /// Отслеживаем изменения позиции ползунка и изменяем значение TimerInterval
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HsbTimer_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.ThumbTrack || e.Type == ScrollEventType.EndScroll)
            {
                SetTimerInterval(e.NewValue);
            }
        }

    }
}
