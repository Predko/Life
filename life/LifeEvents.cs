using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace life
{
    public partial class LifeForm : Form
    {
        /// <summary>
        /// Используется для определения, была ли отпущена клавиша мыши, после нажатия и удержания.
        /// </summary>
        private bool isMouseKeyUp;

        /// <summary>
        /// время удержания нажатой клавиши мыши
        /// </summary>
        private readonly int pauseKeyDown = 500;

        /// <summary>
        /// Указывает текущую установку направления таймера. true - вперёд, false - назад(предыдущий ход).
        /// </summary>
        private bool isTimerNext = true;

        /// <summary>
        /// Указывает текущую установку направления таймера. false - вперёд, true - назад(предыдущий ход).
        /// </summary>
        private bool IsTimerPrevious
        {
            get => (isTimerNext == false);
            set => isTimerNext = !value;
        }

        /// <summary>
        /// Создание нового игрового поля.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNewGame_Click(object sender, EventArgs e)
        {
            if (settingField.ShowDialog() == DialogResult.OK)
            {
                NewField(settingField.SizeField.Width, settingField.SizeField.Height, settingField.Density, settingField.IsBorder);

                MoveCounter = 0;

                panelField.Invalidate();
            }
        }

        /// <summary>
        /// Запускает или останавливает игру.
        /// </summary>
        private void PlayStartStop()
        {
            if (btnStartStop.Text == text_start)
            {
                SetTimerNext();

                timer.Start();

                btnNewGame.Enabled = false;
                btnStartStop.Text = text_stop;
                btnMakeAStep.Enabled = false;
                btnPreviousStep.Enabled = false;
                btnSaveField.Enabled = false;
                btnLoadField.Enabled = false;
            }
            else
            {
                timer.Stop();

                btnNewGame.Enabled = true;
                btnStartStop.Text = text_start;
                btnMakeAStep.Enabled = true;
                if ((steps?.IsLogEmpty() ?? true) == false)
                {
                    btnPreviousStep.Enabled = true;
                }
                btnSaveField.Enabled = true;
                btnLoadField.Enabled = true;
            }
        }

        /// <summary>
        /// Запуск игры - остановка игры.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            PlayStartStop();
        }

        /// <summary>
        /// Сделать один шаг.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMakeAStep_Click(object sender, EventArgs e)
        {
            field.NextStep();

            field.DrawChangedCells(bitmapGraphics, BitmapCells);

            MoveCounter++;

            if ((steps?.IsLogEmpty() ?? true) == false)
            {
                btnPreviousStep.Enabled = true;
            }

            panelField.Invalidate();
        }

        /// <summary>
        /// Восстанавливает нормальный обработчик btnMakeAStep.MouseDown
        /// </summary>
        private void HandlerEnableMakeAStep()
        {
            // Восстанавливаем обработчик.
            btnMakeAStep.MouseDown -= BtnMouseDownBlankHandler;
            btnMakeAStep.MouseDown += BtnMakeAStep_MouseDown;
        }

        /// <summary>
        /// Если кнопка мыши была нажата в течении более чем pauseKeyDown миллисек.,
        /// запускает игру с помощью setTimer.
        /// </summary>
        /// <param name="setTimer">Метод запуска таймера игры.</param>
        /// <param name="handlerEnable">Метод, восстанавливающий обработчик событий кнопки, если запуск не произошёл.</param>
        private async void TurnOnMoves(Action setTimer, Action handlerEnable)
        {
            await Task.Run(() => Thread.Sleep(pauseKeyDown));

            // Проверяем, была ли отпущена клавиша мыши.
            if (isMouseKeyUp == false)
            {
                setTimer();

                timer.Start();
            }
            else
            {
                handlerEnable();
            }
        }

        /// <summary>
        /// При удержании кнопки нажатой pauseKeyDown миллисекунд, переходит режим непрерывных ходов вперёд.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMakeAStep_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Переключаем обработчик.
                btnMakeAStep.MouseDown -= BtnMakeAStep_MouseDown;
                btnMakeAStep.MouseDown += BtnMouseDownBlankHandler;

                isMouseKeyUp = false;

                TurnOnMoves(SetTimerNext, HandlerEnableMakeAStep);
            }
        }

        /// <summary>
        /// Обработчик-заглушка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMouseDownBlankHandler(object sender, MouseEventArgs e)
        {
            // Если было повторное нажатие клавиши мыши за время паузы - указываем на это.
            isMouseKeyUp = true;
        }

        /// <summary>
        /// Выход их режима непрерывных ходов вперёд.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMakeAStep_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseKeyUp = true;

                if (timer.Enabled)
                {
                    // Восстанавливаем обработчик.
                    HandlerEnableMakeAStep();

                    timer.Stop();
                }
            }
        }

        /// <summary>
        /// Возвращает состояние игрового поля на шаг назад.
        /// </summary>
        private void PreviousStep()
        {
            ExitCodePreviousStep exitCode = field.PreviousStep();

            if (exitCode == ExitCodePreviousStep.Error) // если лог пуст или была ошибка, делаем неактивной кнопку 
            {
                btnPreviousStep.Enabled = false;

                // Произошла ошибка, перерисовываем поле
                field.DrawAll(bitmapGraphics, BitmapCells);

                MoveCounter = 0;
            }
            else
            if (exitCode == ExitCodePreviousStep.Ok)
            {
                field.DrawChangedCells(bitmapGraphics, BitmapCells);

                MoveCounter--;

                if ((steps?.IsLogEmpty() ?? true))
                {
                    btnPreviousStep.Enabled = false;
                }
            }
            else
            {
                return;
            }

            panelField.Invalidate();
        }

        /// <summary>
        /// Сделать шаг назад.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPreviousStep_Click(object sender, EventArgs e) => PreviousStep();

        /// <summary>
        /// Восстанавливает нормальный обработчик btnPreviousStep.MouseDown.
        /// </summary>
        private void HandlerEnablePreviousStep()
        {
            // Восстанавливаем обработчик.
            btnPreviousStep.MouseDown -= BtnMouseDownBlankHandler;
            btnPreviousStep.MouseDown += BtnPreviousStep_MouseDown;
        }

        /// <summary>
        /// При удержании кнопки нажатой, переходит режим непрерывных ходов назад.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPreviousStep_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Переключаем обработчик.
                btnPreviousStep.MouseDown -= BtnPreviousStep_MouseDown;
                btnPreviousStep.MouseDown += BtnMouseDownBlankHandler;

                isMouseKeyUp = false;

                TurnOnMoves(SetTimerPrevious, HandlerEnablePreviousStep);
            }
        }

        /// <summary>
        /// Выход их режима непрерывных ходов назад.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPreviousStep_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseKeyUp = true;

                if (timer.Enabled)
                {
                    // Восстанавливаем обработчик.
                    HandlerEnablePreviousStep();

                    timer.Stop();

                    SetTimerNext();
                }
            }
        }

        /// <summary>
        /// Устанавливает событие таймера для ходов вперёд.
        /// </summary>
        private void SetTimerNext()
        {
            if (isTimerNext)
            {
                return;
            }

            timer.Tick -= TimerPrevious_Tick;

            timer.Tick += TimerNext_Tick;

            isTimerNext = true;
        }

        /// <summary>
        /// Устанавливает событие таймера для ходов назад.
        /// </summary>
        private void SetTimerPrevious()
        {
            if (IsTimerPrevious)
            {
                return;
            }

            timer.Tick -= TimerNext_Tick;

            timer.Tick += TimerPrevious_Tick;

            IsTimerPrevious = true;
        }

        /// <summary>
        /// Сохранить игровое поле в файл.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSaveField_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            Block cells = new Block(field);

            cells.SaveToFile(saveFileDialog.FileName);
        }

        /// <summary>
        /// Загрузить игровое поле из файла.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLoadField_Click(object sender, EventArgs e)
        {
            LoadField();

            MoveCounter = 0;

            panelField.Invalidate();
        }

        /// <summary>
        /// Обработчик событий с клавиатуры.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LifeForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                PlayStartStop();
            }
            else
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Обработчик события таймера для хода вперёд.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerNext_Tick(object sender, EventArgs e)
        {
            field.NextStep();

            field.DrawChangedCells(bitmapGraphics, BitmapCells);

            MoveCounter++;

            panelField.Invalidate();
        }

        /// <summary>
        /// Обработчик события таймера для хода назад.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerPrevious_Tick(object sender, EventArgs e)
        {
            PreviousStep();

            if ((steps?.IsLogEmpty() ?? true))
            {
                // Проверяем, если таймер был вкючён по зажатой клавише мыши - восстанавливаем обработчик мыши
                if (isMouseKeyUp == false)
                {
                    // Восстанавливаем обработчик.
                    HandlerEnablePreviousStep();
                }

                timer.Stop();

                SetTimerNext();
            }
        }

        /// <summary>
        /// Отслеживает изменения позиции ползунка и изменяет значение TimerInterval(пауза между ходами).
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
