using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace life
{
    public partial class LifeForm : Form
    {
        /// <summary>
        /// Последняя координата в момент нажатия левой клавиши мыши.
        /// </summary>
        private Point LastMouseDownCoordinate;

        /// <summary>
        /// Проверяет, находится ли точка внутри прямоугольника, 
        /// ограниченного точками startSelection и endSelection.
        /// </summary>
        /// <param name="p">Проверяемая точка.</param>
        /// <returns>true - если точка принадлежит прямоугольнику.</returns>
        private bool IsInsideSelection(Point p)
        {
            return Selection.Contains(p);
        }

        /// <summary>
        /// Блок выбран.
        /// </summary>
        private bool IsSelected
        {
            get => isSelected;
            set
            {
                if (value)
                {
                    // Отключаем редактирование клетки.
                    IsSetCellMode = false;
                }

                isSelected = value;
            }
        }

        private bool isSelected;

        /// <summary>
        /// Режим перемещения блока.
        /// </summary>
        private bool isMoveMode = false;

        /// <summary>
        /// Стартовая точка в режиме перемещения.
        /// </summary>
        private Point startMoveLocation;

        /// <summary>
        /// Режим редактирования клетки игрового поля.
        /// </summary>
        private bool IsSetCellMode { get; set; }

        /// <summary>
        /// Указывает, находится ли программа в режиме выбора блока.
        /// </summary>
        private bool IsSelectionMode { get; set; }

        /// <summary>
        /// Минимальное смещение мыши с нажатой левой кнопкой,
        /// чтобы перейти в режим выбора.
        /// </summary>
        private const int minOffsetMouseForSelect = 3;

        /// <summary>
        /// Тип клетки, который будет размещён на поле
        /// </summary>
        private StatusCell currentCellStatus = StatusCell.Yes;

        /// <summary>
        /// Переходит в режим выбора блока, если возможно.
        /// Условие перехода - если любая координата мыши изменилась более чем на minOffsetMouseForSelect
        /// при нажатой левой клавише.
        /// </summary>
        /// <param name="currentPosition">Текущие координаты мыши.</param>
        /// <returns>true - если установлен режим выбора блока,
        /// false - если нет.</returns>
        private bool IsSetSelectionMode(Point currentPosition)
        {
            Point p = LastMouseDownCoordinate;

            p.Offset(-currentPosition.X, -currentPosition.Y);

            return (Math.Abs(p.X) > minOffsetMouseForSelect || Math.Abs(p.Y) > minOffsetMouseForSelect);
        }

        /// <summary>
        /// Устанавливает или убирает клетку в точку currentMousePosition.
        /// Тип клетки записан в переменной currentCellStatus.
        /// </summary>
        /// <param name="currentMousePosition">Точка для установки клетки.</param>
        private void SetCellToField(Point currentMousePosition)
        {
            // Обнуляем лог и счётчик шагов.
            ClearLogOfSteps();

            Rectangle selection = GetSelection(currentMousePosition, currentMousePosition);

            Rectangle fieldRectangle = new Rectangle()
            {
                Location = new Point(selection.X / cellSize, selection.Y / cellSize),
                Width = selection.Width / cellSize,
                Height = selection.Height / cellSize
            };

            Point currentCellLocation = fieldRectangle.Location;

            Cell cell = field.GetCell(currentCellLocation);

            if (cell == null || cell.IsNoCell)
            {
                cell = new Cell(currentCellLocation)
                {
                    Status = currentCellStatus,
                    NewStatus = StatusCell.Yes,
                    active = true
                };

                field.AddAndPrepareCell(cell);
            }
            else
            if (cell.Status != currentCellStatus)
            {
                cell.Status = currentCellStatus;

                field.AddAndPrepareCell(cell);
            }
            else
            {
                field.RemoveAndPrepareCell(cell);
            }

            field.Draw(bitmapGraphics, BitmapCells, fieldRectangle);

            panelField.Invalidate(selection);
        }

        /// <summary>
        /// Очищает историю и счётчик шагов, а также отключает кнопку "Шаг назад"
        /// </summary>
        private void ClearLogOfSteps()
        {
            if (steps != null)
            {
                steps.Clear();

                MoveCounter = 0;

                btnPreviousStep.Enabled = false;
            }
        }

        /// <summary>
        /// Обработчик события нажатия клавиши мыши.
        /// Обрабатывает нажатие левой клавиши мыши в режиме, когда блок уже выбран,
        ///  и подготавливает переменные к режиму выделения или режиму редактирования клетки.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelField_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsSelected)
                {
                    // Блок выбран.
                    if (IsInsideSelection(e.Location))
                    {
                        // мышь находится внутри выделенного блока - переходим в режим перемещения блока.
                        isMoveMode = true;

                        startMoveLocation = e.Location;
                    }
                    else
                    {
                        // Блок выбран, но кнопка мыши нажата вне выделенного блока.
                        IsSelected = false;

                        selectedCells.Clear();

                        // Выходим из режима выделения.
                        ExitSelectionMode();
                    }
                }
                else
                {
                    // Запоминаем координату мыши, для возможного входа в режим выбора блока.
                    LastMouseDownCoordinate = e.Location;

                    // Включаем режим редактирования клетки.
                    IsSetCellMode = true;
                }
            }
        }

        /// <summary>
        /// Обработчик события отпускания клавиши мыши.
        /// Завершает режим выделения, режим перемещения(копирования) блока 
        /// и включает редактирование клетки игрового поля.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelField_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsSelectionMode)
                {
                    // Создаём блок клеток из выделенной области.
                    IsSelectionMode = false;

                    SelectBlock();

                    IsSelected = true;
                }
                else
                if (isMoveMode)
                {
                    // Копируем выбранный блок в в новую позицию. 
                    CopySelected(e.Location);
                }
                else
                if (IsSetCellMode)
                {
                    // Редактируем клетку в текущей позиции мыши.
                    SetCellToField(e.Location);
                }
            }
        }

        /// <summary>
        /// Обработчик события перемещения мыши при нажатой левой клавише.
        /// В режиме выделения - отрисовывает прямоугольник выделения.
        /// В режиме перемещения блока - отрисовывает блок с выделением.
        /// В режиме редактирования клетки игрового поля - проверяет возможность 
        /// перехода в режим выбора блока.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelField_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.Left)
            {
                if (IsSelectionMode)
                {
                    // Мы в режиме выделения.
                    // Отрисовываем прямоугольник выделения.
                    DrawSelectionRectangle(e.Location);
                }
                else
                if (isMoveMode)
                {
                    // Режим перемещения блока.
                    // Отрисовываем блок в новой позиции.
                    DrawSelectedBlock(e.Location);
                }
                else
                if (IsSetCellMode && IsSetSelectionMode(e.Location))
                {
                    // Переходим к режиму выбора блока.
                    IsSelectionMode = true;

                    StartSelection(e.Location);
                }
            }
        }

        /// <summary>
        /// Устанавливает обработчики событий мыши для редактирования игрового поля,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelField_MouseEnter(object sender, EventArgs e)
        {
            // Проверяем, можно ли включить режим редактирования игрового поля.
            // Если таймер включён, значит запущены игровые ходы по таймеру.
            if (timer.Enabled)
            {
                return;
            }

            panelField.Focus();

            MouseEditingGameFieldMode(turnOn);
        }

        /// <summary>
        /// Выходит из режима редактирования игрового поля при потере фокуса панелью.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelField_LostFocus(object sender, EventArgs e)
        {
            ExitSelectionMode();

            MouseEditingGameFieldMode(turnOff);
        }

        /// <summary>
        /// Отрисовывает панель игрового поля.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelField_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawImage(bitmap, Point.Empty);
        }


        /// <summary>
        /// Указывает, вкючён ли режим редактирования.
        /// </summary>
        private bool mouseSelectionModeOn = false;
        
        /// <summary>
        /// Включить режим редактирования.
        /// </summary>
        private const bool turnOn = true;
        
        /// <summary>
        /// Выключить режим редактирования.
        /// </summary>
        private const bool turnOff = false;

        /// <summary>
        /// Включает или выключает режим редактирования игрового поля мышью.
        /// </summary>
        /// <param name="mode">turnOn = true - включить режим, turnOff = false - выключить.</param>
        public void MouseEditingGameFieldMode(bool mode)
        {
            if (mode == turnOn && mouseSelectionModeOn == turnOff)
            {
                PanelField_AddMouseEvent();

                mouseSelectionModeOn = turnOn;
            }
            else
            if (mode == turnOff && mouseSelectionModeOn == turnOn)
            {
                PanelField_RemoveMouseEvent();

                mouseSelectionModeOn = turnOff;
            }
        }

        /// <summary>
        /// Удаляет обработчики событий мыши панели игрового поля.
        /// </summary>
        private void PanelField_RemoveMouseEvent()
        {
            panelField.MouseDown -= PanelField_MouseDown;
            panelField.MouseMove -= PanelField_MouseMove;
            panelField.MouseUp -= PanelField_MouseUp;
            panelField.LostFocus -= PanelField_LostFocus;
        }

        /// <summary>
        /// Устанавливает обработчики событий мыши панели игрового поля.
        /// </summary>
        private void PanelField_AddMouseEvent()
        {
            panelField.MouseDown += PanelField_MouseDown;
            panelField.MouseMove += PanelField_MouseMove;
            panelField.MouseUp += PanelField_MouseUp;
            panelField.LostFocus += PanelField_LostFocus;
        }
    }
}

