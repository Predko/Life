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
            return Selection.Contains(p);
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
        /// Стартовая точка в режиме перемещения.
        /// </summary>
        private Point startMoveLocation;

        /// <summary>
        /// Начало процесса выбора прямоугольного блока игрового поля.
        /// </summary>
        /// <param name="current"></param>
        private void StartSelection(Point current)
        {
            SetMouseEventForSelectionMode();

            isLeaveSelectionMode = false;

            startSelection = endSelection = current;

            oldSelection = Rectangle.Empty;

            // Сохраняем текущее изображение поля(bitmap).
            SavedBitmapField = new Bitmap(bitmap);

            isSelected = false;

            isMoveMode = false;
        }

        /// <summary>
        /// Создаёт блок из выбранных клеток игрового поля
        /// </summary>
        private void SelectBlock()
        {
            CorrectStartEndPoint();

            selectedCells = new Block();

            Rectangle selection = Selection;

            Rectangle seletedRectangle = new Rectangle()
            {
                X = selection.X / cellSize,
                Y = selection.Y / cellSize,
                Width = selection.Width / cellSize,
                Height = selection.Height / cellSize
            };

            selectedCells.AddRange(field.GetCells(seletedRectangle));
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

                startSelection.X = endSelection.X;

                endSelection.X = temp;
            }

            if (startSelection.Y > endSelection.Y)
            {
                temp = startSelection.Y;

                startSelection.Y = endSelection.Y;

                endSelection.Y = temp;
            }
        }

        private void MoveSelected(Point current)
        {
            // Прямоугольник выделения на месте установки блока.
            Rectangle selection = OffsetSelection(current);

            // Меняем стартовые и конечные значения прямоугольника выделения
            startSelection = endSelection = selection.Location;

            endSelection.Offset(selection.Width - 1, selection.Height - 1);

            //startMoveLocation = startSelection;

            // Прямоугольник выделения в координатах игрового поля.
            Rectangle fieldSselection = new Rectangle()
            {
                X = selection.X / cellSize,
                Y = selection.Y / cellSize,
                Width = selection.Width / cellSize,
                Height = selection.Height / cellSize
            };

            // Точка установки блока.
            Point pointToPlace = fieldSselection.Location;

            // Перерисовываем игровое поле в месте выделения блока.
            field.Draw(bitmapGraphics, BitmapCells, selectedCells.Rectangle);

            // Устанавливаем блок на новом месте.
            field.PlaceBlock(selectedCells, pointToPlace);

            // Подготавливаем игровое поле для нормальной работы игры.
            {
                field.RemoveNoLivesCells();

                field.PrepareField();
            }

            // Отрисовываем блок в новом месте.
            field.Draw(bitmapGraphics, BitmapCells, fieldSselection);

            // Сохраняем новое состояние игрового поля.
            {
                SavedBitmapField.Dispose();

                SavedBitmapField = new Bitmap(bitmap);

                selectedCells.Clear();
                selectedCells.AddRange(field.GetCells(fieldSselection));

                // Отрисовываем прямоугольник выделения.
                bitmapGraphics.FillRectangle(OpacityBrush, selection);

                oldSelection = selection;
            }

            panelField.Invalidate();
        }

        private void DrawSelectedBlock(Point current)
        {
            Rectangle selection = OffsetSelection(current);

            RedrawOldSelection();

            selectedCells.Draw(field, bitmapGraphics, BitmapCells, selection.X / cellSize, selection.Y / cellSize);

            bitmapGraphics.FillRectangle(OpacityBrush, selection);

            panelField.Invalidate(Rectangle.Union(oldSelection, selection));

            oldSelection = selection;
        }

        /// <summary>
        /// Вычисляет координаты прямоугольника выделения, смещённого в указанную точку.
        /// </summary>
        /// <param name="current">Последние координаты мыши.</param>
        /// <returns></returns>
        private Rectangle OffsetSelection(Point current)
        {
            current = TruncatePoint(current, cellSize);

            startMoveLocation = TruncatePoint(startMoveLocation, cellSize);

            current.Offset(-startMoveLocation.X, -startMoveLocation.Y);

            Rectangle selection = Selection;

            selection.Offset(current);

            return selection;
        }

        /// <summary>
        /// Отрисовка прямоугольника выделения.
        /// </summary>
        /// <param name="current"></param>
        private void DrawSelectionRectangle(Point current)
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

        private void PanelField_MouseClick_InSelectionMode(object sender, MouseEventArgs e)
        {
        }

        private void PanelField_MouseDown_InSelectionMode(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isSelected)
                {
                    if (IsInsideSelection(e.Location))
                    {
                        // мышь находится внутри выделенного блока - переходим в режим перемещения блока.
                        isMoveMode = true;

                        startMoveLocation = e.Location;
                    }
                    else
                    {
                        // Блок выбран, но кнопка мыши нажата вне выделенного блока - сбрасываем выделение.
                        LeaveSelectionMode();
                    }
                }
            }
        }

        private void PanelField_MouseUp_InSelectionMode(object sender, MouseEventArgs e)
        {
            if (isSelected == false)
            {
                isSelected = true;

                SelectBlock();
            }
            else
            if (isMoveMode)
            {
                MoveSelected(e.Location);
            }
        }

        private void PanelField_MouseMove_InSelectionMode(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isSelected == false)
                {
                    DrawSelectionRectangle(e.Location);
                }
                else
                if (isMoveMode)
                {
                    DrawSelectedBlock(e.Location);
                }
            }
        }

        private void PanelField_LostFocus_InSelectionMode(object sender, EventArgs e)
        {
            LeaveSelectionMode();

            MouseEditingGameFieldMode(turnOff);
        }

        /// <summary>
        /// Устанавливает обработчики событий мыши для режима выбора блока.
        /// </summary>
        private void SetMouseEventForSelectionMode()
        {
            MouseEditingGameFieldMode(turnOff);

            panelField.MouseClick += PanelField_MouseClick_InSelectionMode;
            panelField.MouseDown += PanelField_MouseDown_InSelectionMode;
            panelField.MouseUp += PanelField_MouseUp_InSelectionMode;
            panelField.MouseMove += PanelField_MouseMove_InSelectionMode;
            panelField.LostFocus += PanelField_LostFocus_InSelectionMode;
        }

        /// <summary>
        /// Восстанавливает обработчики событий мыши.
        /// </summary>
        private void RemoveMouseEventForSelectionMode()
        {
            panelField.MouseClick -= PanelField_MouseClick_InSelectionMode;
            panelField.MouseDown -= PanelField_MouseDown_InSelectionMode;
            panelField.MouseUp -= PanelField_MouseUp_InSelectionMode;
            panelField.MouseMove -= PanelField_MouseMove_InSelectionMode;
            panelField.LostFocus -= PanelField_LostFocus_InSelectionMode;

            MouseEditingGameFieldMode(turnOn);
        }


        /// <summary>
        /// Если уже был произведён выход из режима выбора, равно true.
        /// </summary>
        private bool isLeaveSelectionMode = false;

        /// <summary>
        /// Выход из режима выбора блока игрового поля.
        /// </summary>
        private void LeaveSelectionMode()
        {
            if (isLeaveSelectionMode)
            {
                return;
            }

            isLeaveSelectionMode = true;

            isMoveMode = false;
            
            panelField.Enabled = false;

            // Восстанавливаем поле после предыдущего выбора.
            {
                RedrawOldSelection();

                panelField.Invalidate(oldSelection);

                SavedBitmapField.Dispose();

                SavedBitmapField = null;
            }

            if (selectedCells != null)
            {
                selectedCells.Clear();

                selectedCells = null;
            }

            // Выходим из режима выделения - восстанавливаем обработчики событий мыши.
            RemoveMouseEventForSelectionMode();

            panelField.Enabled = true;
        }
    }
}

