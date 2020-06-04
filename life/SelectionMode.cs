using System;
using System.Drawing;
using System.Windows.Forms;

namespace life
{
    public partial class LifeForm
    {
        /// <summary>
        /// Начальная точка прямоугольника выбора.
        /// </summary>
        private Point startSelection;

        /// <summary>
        /// Конечная точка прямоугольника выбора.
        /// </summary>
        private Point endSelection;

        #region Округление координат и размера.

        /// <summary>
        /// Округляет число вниз, кратно указанной величине.
        /// </summary>
        /// <param name="i">Число для округления.</param>
        /// <param name="multiplicity">Кратность.</param>
        /// <returns></returns>
        private int Truncate(int i, int multiplicity) => (i / multiplicity) * multiplicity;

        /// <summary>
        /// Обрезает координаты точки кратно cellSize.
        /// Например: multiplicity = 25, p = (30,55), результат = (25, 50)
        /// </summary>
        /// <param name="p">Точка для округления.</param>
        /// <param name="multiplicity">Кратность округления.</param>
        /// <returns></returns>
        private Point TruncatePoint(Point p, int multiplicity) => new Point()
        {
            X = Truncate(p.X, multiplicity),
            Y = Truncate(p.Y, multiplicity)
        };

        private Point TruncatePoint(int x, int y, int multiplicity) => new Point()
        {
            X = Truncate(x, multiplicity),
            Y = Truncate(y, multiplicity)
        };


        /// <summary>
        /// Округляет число вверх, кратно указанной величине.
        /// </summary>
        /// <param name="i">Округляемое число.</param>
        /// <param name="multiplicity">Кратность округления.</param>
        /// <returns></returns>
        private int Ceiling(int i, int multiplicity) => (i / multiplicity + 1) * multiplicity;

        /// <summary>
        /// Округляет размер вверх, кратно указанному числу.
        /// </summary>
        /// <param name="size">Округляемый размер.</param>
        /// <param name="multiplicity">Кратность округления.</param>
        /// <returns></returns>
        private Size CeilingSize(Size size, int multiplicity) => new Size()
        {
            Width = Ceiling(size.Width, multiplicity),
            Height = Ceiling(size.Height, multiplicity)
        };

        private Size CeilingSize(int width, int height, int multiplicity) => new Size()
        {
            Width = Ceiling(width, multiplicity),
            Height = Ceiling(height, multiplicity)
        };

        #endregion

        /// <summary>
        /// Возвращает прямоугольник между точками startSelection и endSelection,
        /// кратный размеру клетки.
        /// </summary>
        private Rectangle Selection => GetSelection(startSelection, endSelection);

        /// <summary>
        /// Возвращает прямоугольник между точками start и end,
        /// кратный размеру клетки cellSize.
        /// </summary>
        private Rectangle GetSelection(Point start, Point end)
        {
            CalcMinCoordinateAndSize(out int minX, out int maxX, start.X, end.X);

            CalcMinCoordinateAndSize(out int minY, out int maxY, start.Y, end.Y);

            Rectangle r = new Rectangle()
            {
                Location = new Point(minX, minY),
                Size = new Size(maxX - minX, maxY - minY)
            };

            return r;
        }

        private Rectangle oldSelection = Rectangle.Empty;

        private Bitmap SavedBitmapField;

        /// <summary>
        /// Определяет минимальную и максимальную координату из (startCoord,endCoord),
        /// с учётом округления кратно размеру клетки.
        /// </summary>
        /// <param name="minCoord">Вычисленное минимальное значение.</param>
        /// <param name="maxCoord">Вычисленное максимальное значение.</param>
        /// <param name="startCoord">Начальная координата.</param>
        /// <param name="endCoord">Конечная координата.</param>
        private void CalcMinCoordinateAndSize(out int minCoord, out int maxCoord, int startCoord, int endCoord)
        {
            if (startCoord > endCoord)
            {
                int temp = startCoord;

                startCoord = endCoord;

                endCoord = temp;
            }

            minCoord = Truncate(startCoord, cellSize);

            maxCoord = Ceiling(endCoord, cellSize);
        }

        /// <summary>
        /// Проверяет, находится ли точка внутри прямоугольника(вкючая границы), 
        /// ограниченного точками startSelection и endSelection.
        /// </summary>
        /// <param name="p">Проверяемая точка.</param>
        /// <returns>true - если точка принадлежит прямоугольнику.</returns>
        private bool IsInsideSelection(Point p)
        {
            return (p.X >= startSelection.X && p.X <= endSelection.X &&
                    p.Y >= startSelection.Y && p.Y <= endSelection.Y);
        }

        /// <summary>
        /// Блок выбран.
        /// </summary>
        private bool isSelected = false;

        /// <summary>
        /// Режим перемещения блока.
        /// </summary>
        private bool isMoveMode = false;

        /// <summary>
        /// Выбранный блок клеток.
        /// </summary>
        private Block selectedCells;

        /// <summary>
        /// Начало процесса выбора прямоугольного блока игрового поля.
        /// </summary>
        /// <param name="current"></param>
        private void StartSelection(Point current)
        {
            SetMouseEventForSelectionMode();

            startSelection = endSelection = current;

            oldSelection = Rectangle.Empty;

            // Сохраняем текущее изображение поля(bitmap).
            SavedBitmapField = new Bitmap(bitmap);

            isSelected = false;

            isMoveMode = false;
        }

        /// <summary>
        /// Устанавливает обработчики событий мыши для режима выбора блока.
        /// </summary>
        private void SetMouseEventForSelectionMode()
        {
            PanelField_RemoveMouseEvent();

            panelField.MouseClick += PanelField_MouseClickInSelectionMode;
            panelField.MouseDown += PanelField_MouseDownInSelectionMode;
            panelField.MouseUp += PanelField_MouseUpInSelectionMode;
            panelField.MouseMove += PanelField_MouseMoveInSelectionMode;
            panelField.LostFocus += PanelField_LostFocusInSelectionMode;
        }

        /// <summary>
        /// Восстанавливает обработчики событий мыши.
        /// </summary>
        private void RemoveMouseEventForSelectionMode()
        {
            panelField.MouseClick -= PanelField_MouseClickInSelectionMode;
            panelField.MouseDown -= PanelField_MouseDownInSelectionMode;
            panelField.MouseUp -= PanelField_MouseUpInSelectionMode;
            panelField.MouseMove -= PanelField_MouseMoveInSelectionMode;
            panelField.LostFocus -= PanelField_LostFocusInSelectionMode;

            PanelField_AddMouseEvent();
        }

        private void PanelField_MouseClickInSelectionMode(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Активизирует режим перемещения блока.
        /// </summary>
        /// <param name="current">Точка, из которой начинается перемещение.</param>
        private void StartMoveSelected()
        {
            isMoveMode = true;
        }

        /// <summary>
        /// Создаёт блок из выбранных клеток игрового поля
        /// </summary>
        private void SelectBlock()
        {
            if (startSelection != endSelection)
            {
                CorrectStartEndPoint();

                isSelected = true;

                selectedCells = new Block();

                Rectangle r = Selection;

                Rectangle seletedRectangle = new Rectangle()
                {
                    Location = new Point(r.X / cellSize, r.Y / cellSize),
                    Width = r.Width / cellSize,
                    Height = r.Height / cellSize
                };

                selectedCells.AddRange(field.GetCells(seletedRectangle));
            }
        }

        /// <summary>
        /// Корректируем координаты начальной и конечной точки прямоугольника.
        /// </summary>
        private void CorrectStartEndPoint()
        {
            int temp;

            if (startSelection.X > endSelection.X)
            {
                temp = startSelection.X;

                endSelection.X = startSelection.X;

                startSelection.X = temp;
            }

            if (startSelection.Y > endSelection.Y)
            {
                temp = startSelection.Y;

                endSelection.Y = startSelection.Y;

                startSelection.Y = temp;
            }
        }

        private void MoveSelected()
        {
            field.RemoveCells(selectedCells);

            Point p = selectedCells.Location;

            CellLocation pointToPlace = new CellLocation()
            {
                X = p.X + (endSelection.X / cellSize - startSelection.X / cellSize),
                Y = p.Y + (endSelection.Y / cellSize - startSelection.Y / cellSize),
            };

            Rectangle newRectangle = new Rectangle(pointToPlace.X, pointToPlace.Y, selectedCells.Size.Width, selectedCells.Size.Height);

            field.PlaceBlock(selectedCells, pointToPlace);

            field.Draw(bitmapGraphics, BitmapCells, Rectangle.Union(selectedCells.Rectangle, newRectangle));
        }

        /// <summary>
        /// Отрисовка прямоугольника выделения.
        /// </summary>
        /// <param name="current"></param>
        private void DrawSelection(Point current)
        {
            endSelection = current;

            Rectangle selection = Selection;

            RedrawOldSelection();

            bitmapGraphics.FillRectangle(OpacityBrush, selection);

            panelField.Invalidate(Rectangle.Union(oldSelection, selection));

            oldSelection = selection;
        }

        /// <summary>
        /// Восстанавливает исходное изображение в последнем выбранном прямоугольнике
        /// </summary>
        private void RedrawOldSelection() => bitmapGraphics.DrawImage(SavedBitmapField,
                                                                      oldSelection.X,
                                                                      oldSelection.Y,
                                                                      oldSelection,
                                                                      GraphicsUnit.Pixel);

        private void LeaveSelectionMode()
        {
            if (selectedCells != null)
            {
                // Восстанавливаем поле после предыдущего выбора.
                {
                    RedrawOldSelection();

                    panelField.Invalidate(oldSelection);

                    SavedBitmapField.Dispose();

                    SavedBitmapField = null;
                }

                selectedCells.Clear();

                selectedCells = null;

                // Выходим из режима выделения - восстанавливаем обработчики событий мыши.
                RemoveMouseEventForSelectionMode();
            }
        }

        private void PanelField_MouseDownInSelectionMode(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                if (IsInsideSelection(e.Location))
                {
                    // мышь находится внутри выделенного блока - переходим в режим перемещения блока.

                    StartMoveSelected();
                }
                else
                if (isSelected)
                {
                    // Блок выбран, но кнопка мыши нажата вне выделенного блока - сбрасываем выделение.
                    LeaveSelectionMode();
                }
            }
        }

        private void PanelField_MouseUpInSelectionMode(object sender, MouseEventArgs e)
        {
            if (isSelected == false)
            {
                SelectBlock();
            }
            else
            if (isMoveMode)
            {


            }
        }

        private void PanelField_MouseMoveInSelectionMode(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isSelected == false)
                {
                    DrawSelection(e.Location);
                }
                else
                if (isMoveMode)
                {
                }
            }
        }

        private void PanelField_LostFocusInSelectionMode(object sender, EventArgs e)
        {
            LeaveSelectionMode();

            MouseEditingGameFieldMode(turnOff);
        }



    }
}

