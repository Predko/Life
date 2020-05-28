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
            field.NextStep();

            MoveCounter++;

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

                MoveCounter = 0;
            }
            else
            {
                MoveCounter--;
            }

            panelField.Invalidate();
        }

        private void BtnSaveField_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            Block cells = new Block(field);

            cells.SaveToFile(saveFileDialog.FileName);
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
                field.NextStep();

                MoveCounter++;

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
            field.NextStep();

            MoveCounter++;

            lbCount.Text = $"Step: {MoveCounter,5:d}";

            panelField.Invalidate();
        }

        /// <summary>
        /// Отслеживает изменения позиции ползунка и изменяет значение TimerInterval
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HsbTimer_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.ThumbTrack || e.Type == ScrollEventType.EndScroll)
            {
                SetSpeedGame(e.NewValue);
            }
        }

    }
}
