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
        private Point newMouseDownCoordinate;

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
            Point p = newMouseDownCoordinate;

            p.Offset(-currentPosition.X, -currentPosition.Y);
            
            if (Math.Abs(p.X) > minOffsetMouseForSelect || Math.Abs(p.Y) > minOffsetMouseForSelect)
            {
                StartSelection(currentPosition);

                return true;
            }

            return false;
        }

        private void PanelField_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Rectangle selection = GetSelection(e.Location, e.Location);

                Rectangle fieldRect = new Rectangle()
                {
                    Location = new Point(selection.X / cellSize, selection.Y / cellSize),
                    Width = selection.Width / cellSize,
                    Height = selection.Height / cellSize
                };

                CellLocation cellLocation = new CellLocation(fieldRect.X, fieldRect.Y);

                Cell cell = field.GetCell(cellLocation);

                if (cell == null || cell.IsNoCell)
                {
                    cell = new Cell(cellLocation)
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

                field.Draw(bitmapGraphics, BitmapCells, fieldRect);

                panelField.Invalidate(selection);
            }
        }

        private void PanelField_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                if (IsInsideSelection(e.Location))
                {
                    // мышь находится внутри выделенного блока - переходим в режим перемещения блока.

                    StartMoveSelected();
                }
                else
                {
                    newMouseDownCoordinate = e.Location;
                }
            }
        }

        private void PanelField_MouseUp(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Минимальное смещение мыши с нажатой левой кнопкой,
        /// чтобы перейти в режим выбора.
        /// </summary>
        private const int minOffsetMouseForSelect = 3;

        private void PanelField_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.Left)
            {
                // Проверяем, надо ли включить выбора блока.
                if (IsSetSelectionMode(e.Location))
                {
                    return;
                }
            }
        }

        private void PanelField_MouseEnter(object sender, EventArgs e)
        {
            // Проверяем, можно ли включить режим редактирования игрового поля.
            if (timer.Enabled)
            {
                return;
            }

            panelField.Focus();

            MouseEditingGameFieldMode(turnOn);
        }

        private void PanelField_LostFocus(object sender, EventArgs e)
        {
            MouseEditingGameFieldMode(turnOff);
        }

        private void PanelField_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawImage(bitmap, Point.Empty);
        }


        /// <summary>
        /// Указывает, вкючён ли режим редактирования.
        /// </summary>
        private bool mouseSelectionModeOn = false;
        
        private const bool turnOn = true;
        private const bool turnOff = false;

        /// <summary>
        /// Включает или выключает режим редактирования игрового поля мышью.
        /// </summary>
        /// <param name="mode">turnOn = true - включить режим, turnOff = false - выключить.</param>
        public void MouseEditingGameFieldMode(bool mode)
        {
            if (mode == turnOn && mouseSelectionModeOn == false)
            {
                PanelField_AddMouseEvent();

                mouseSelectionModeOn = true;
            }
            else
            if (mode == turnOff && mouseSelectionModeOn)
            {
                PanelField_RemoveMouseEvent();

                mouseSelectionModeOn = false;
            }
        }

        /// <summary>
        /// Удаляет обработчики событий мыши панели игрового поля.
        /// </summary>
        private void PanelField_RemoveMouseEvent()
        {
            panelField.MouseClick -= PanelField_MouseClick;
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
            panelField.MouseClick += PanelField_MouseClick;
            panelField.MouseDown += PanelField_MouseDown;
            panelField.MouseMove += PanelField_MouseMove;
            panelField.MouseUp += PanelField_MouseUp;
            panelField.LostFocus += PanelField_LostFocus;
        }
    }
}

