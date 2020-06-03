using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Resources;
using life.Properties;

namespace life
{
    internal enum ExitCodePreviousStep { Ok, NoSteps, Error }

    /// <summary>
    /// Игровое поле
    /// </summary>
    public class Field
    {
        public int width;
        public int height;

        /// <summary>
        /// граница поля есть/нет
        /// </summary>
        public bool isBorder;

        /// <summary>
        /// плотность генерируемого поля
        /// </summary>
        public float density;

        public Rectangle Rectangle { get => new Rectangle(0, 0, width, height); }

        public int Count { get => field.Count; }

        /// <summary>
        /// Отступ от края игрового поля, где не будут располагаться клетки, кроме статичных клеток.
        /// </summary>
        private const int padding = 2;

        /// <summary>
        /// массив координат ячеек вокруг данной
        /// </summary>
        private readonly CellLocation[] nearestCellsLocation = new CellLocation[8];

        /// <summary>
        /// хранилище ячеек игрового поля
        /// </summary>
        private readonly ICellArray field;

        /// <summary>
        /// Список активных клеток текущего хода
        /// </summary>
        private readonly List<Cell> CurrentListCells;

        /// <summary>
        /// Список активных клеток для следующего хода
        /// </summary>
        private readonly List<Cell> NewListCells;

        /// <summary>
        /// Список клеток для отрисовки
        /// </summary>
        private readonly List<Cell> ListCellsForDraw;

        /// <summary>
        /// Запись истории сделанных ходов.
        /// </summary>
        public LogOfSteps steps;

        public Field(int width, int height, ICellArray cellArray)
        {
            this.height = height;
            this.width = width;

            isBorder = true;
            density = 0.3f;

            nearestCellsLocation[0].X = -1; nearestCellsLocation[0].Y = -1;
            nearestCellsLocation[1].X = -1; nearestCellsLocation[1].Y = 0;
            nearestCellsLocation[2].X = -1; nearestCellsLocation[2].Y = 1;
            nearestCellsLocation[3].X = 0; nearestCellsLocation[3].Y = -1;
            nearestCellsLocation[4].X = 0; nearestCellsLocation[4].Y = 1;
            nearestCellsLocation[5].X = 1; nearestCellsLocation[5].Y = -1;
            nearestCellsLocation[6].X = 1; nearestCellsLocation[6].Y = 0;
            nearestCellsLocation[7].X = 1; nearestCellsLocation[7].Y = 1;

            CurrentListCells = new List<Cell>();
            NewListCells = new List<Cell>();

            ListCellsForDraw = new List<Cell>();

            field = cellArray ?? new CellArray(width, height);
        }

        internal void SetLog(LogOfSteps log) => steps = log;

        /// <summary>
        /// Возвращает клетку игрового поля с данными координатами.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Cell GetCell(CellLocation location) => field[location.X, location.Y];

        /// <summary>
        /// Отрисовывает изменившиеся клетки игрового поля.
        /// </summary>
        internal void DrawChangedCells(Graphics bitmapGraphics, BitmapCellsStorage bitmapCells)
        {
            foreach (Cell cell in ListCellsForDraw)
            {
                cell.Draw(bitmapGraphics, bitmapCells);
            }
        }

        /// <summary>
        /// Добавляет ячейку в список для следующего хода.
        /// </summary>
        /// <param name="cell">Добавляемая ячейка.</param>
        internal void AddCellForNextStep(Cell cell) => NewListCells.Add(cell);

        /// <summary>
        /// Очистка игрового поля - очистка хранилища ячеек, очистка истории ходов
        /// </summary>
        private void Clear()
        {
            field.Clear();

            if (steps != null)
            {
                steps.Clear();
            }
        }

        /// <summary>
        /// Добавляет клетку в список для отрисовки на следующем ходу.
        /// </summary>
        /// <param name="cell"></param>
        internal void AddToDraw(Cell cell)
        {
            ListCellsForDraw.Add(cell);
        }

        /// <summary>
        /// Перерисовывает всё игровое поле.
        /// </summary>
        /// <param name="bitmapGraphics"></param>
        /// <param name="bitmapCells"></param>
        public void DrawAll(Graphics bitmapGraphics, BitmapCellsStorage bitmapCells)
        {
            Draw(bitmapGraphics, bitmapCells, Rectangle);
        }

        /// <summary>
        /// Отрисовывает активные клетки игрового поля.
        /// </summary>
        public void Draw(Graphics bitmapGraphics, BitmapCellsStorage bitmapCells)
        {
            foreach (Cell cell in field)
            {
                cell.Draw(bitmapGraphics, bitmapCells);
            }
        }

        /// <summary>
        /// Перерисовывает все ячейки игрового поля(любого типа).
        /// </summary>
        /// <param name="bitmapGraphics"></param>
        /// <param name="bitmapCells"></param>
        /// <param name="rect">Ограничивающий прямоугольник в кординатах игрового поля.</param>
        public void Draw(Graphics bitmapGraphics, BitmapCellsStorage bitmapCells, Rectangle rect)
        {
            Bitmap bmNoCell = bitmapCells.GetBitmap(StatusCell.No);

            for (int x = rect.X; x < (rect.X + width); x++)
            {
                for (int y = rect.Y; y < (rect.Y + height); y++)
                {
                    Cell cell = field[x, y];

                    if (cell != null)
                    {
                        cell.Draw(bitmapGraphics, bitmapCells);
                    }
                    else
                    {
                        bitmapGraphics.DrawImage(bmNoCell, x * bmNoCell.Width, y * bmNoCell.Height);
                    }
                }
            }
        }

        /// <summary>
        /// Добавляет клетки на игровое поле.
        /// </summary>
        /// <param name="cell"></param>
        public void AddCell(Cell cell) => field.Add(cell);

        /// <summary>
        /// Удаляет клетку с игрового поля.
        /// </summary>
        /// <param name="cell"></param>
        public void RemoveCell(Cell cell) => field.Remove(cell);

        /// <summary>
        /// Удаляет с игрового поля клетки, указанные в перечислении.
        /// </summary>
        /// <param name="cells">Список клеток для удаления.</param>
        public void RemoveCells(IEnumerable<Cell> cells)
        {
            foreach (Cell cell in cells)
            {
                field.Remove(cell);
            }
        }

        /// <summary>
        /// Добавляет клетки, близлежащие к клетке с данными координатами, в список активных клеток.
        /// </summary>
        /// <param name="x">Координата x клетки.</param>
        /// <param name="y">Координата y клетки.</param>
        private void AddNearestCells(int x, int y)
        {
            foreach (CellLocation i in nearestCellsLocation)
            {
                Cell currentCell = field[x + i.X, y + i.Y];

                if (currentCell == null)
                {
                    currentCell = new Cell(new CellLocation(x + i.X, y + i.Y)) { active = true };

                    field[x + i.X, y + i.Y] = currentCell;

                    CurrentListCells.Add(currentCell);
                }
                else
                if (currentCell.IsStatic())
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Добавляет клетки, близлежащие к данной, в список активных клеток.
        /// </summary>
        /// <param name="cell">Текущая клетка.</param>
        private void AddNearestCells(Cell cell) => AddNearestCells(cell.Location.X, cell.Location.Y);

        /// <summary>
        /// Подсчёт живых ячеек вокруг данной.
        /// </summary>
        /// <param name="x">Координата x клетки.</param>
        /// <param name="y">Координата y клетки.</param>
        /// <returns>Число живых клеток, примыкающих к данной.</returns>
        internal int NumberLiveCells(int x, int y)
        {

            int count = 0;   // счётчик живых ячеек вокруг данной

            foreach (CellLocation loc in nearestCellsLocation)
            {
                Cell currentcell = field[x + loc.X, y + loc.Y];

                if (currentcell == null)
                    continue;                   // клетки нет

                if (currentcell.IsStatic()) // Статичная клетка(стенка)
                {
                    return 5;   // клетки рядом с ней должны погибнуть
                }

                if (currentcell.IsLive())     // если найденная клетка живая - увеличиваем счётчик
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Ход назад.
        /// </summary>
        /// <returns>ExitCodePreviousStep.Ok - если ещё есть записи в истории ходов.
        /// ExitCodePreviousStep.NoSteps - если история пуста.
        /// ExitCodePreviousStep.Error - если произошла ошибка.</returns>
        internal ExitCodePreviousStep PreviousStep()
        {
            if (steps == null || steps.IsLogEmpty())
            {
                return ExitCodePreviousStep.NoSteps;
            }

            ListCellsForDraw.Clear();

            // удаляем все не живые клетки с поля(активные клетки, вокруг живых)  
            foreach (Cell cell in CurrentListCells)
            {
                if (!cell.IsLive() && !cell.IsStatic())
                {
                    RemoveCell(cell);
                }
            }

            // восстанавливаем состояние поля на состояние предыдущего хода
            if (!steps.Previous(NewListCells))
            {
                // возврат на предыдущий ход завершился с ошибкой
                // состояние поля - на начальное
                Clear();

                SettingCells();

                PrepareField();

                return ExitCodePreviousStep.Error;
            }

            CurrentListCells.Clear();
            CurrentListCells.AddRange(NewListCells);

            foreach (Cell cell in NewListCells)
            {
                AddNearestCells(cell);
            }

            return ExitCodePreviousStep.Ok;
        }

        /// <summary>
        /// Ход вперёд.
        /// </summary>
        internal void NextStep()
        {
            // очищаем списки для клеток следующего хода и отрисовки
            NewListCells.Clear();
            ListCellsForDraw.Clear();

            // рассчитываем состояние клеток для следующего шага
            // Заносим клетки в список клеток следующего шага - NewListCells
            foreach (Cell cell in CurrentListCells)
            {
                cell.AnalysisNextStep(this);
            }

            // Сохраняем изменения клеток на текущем ходе
            if (steps != null)
            {
                steps.SetStep(CurrentListCells);
            }

            // фиксируем изменения клеток, рассчитанных при анализе
            foreach (Cell cell in CurrentListCells)
            {
                cell.ChangeStatus(this);
            }

            CurrentListCells.Clear();
            CurrentListCells.AddRange(NewListCells);

            // Подготавливаем список текущих клеток, добавляя в него клетки вокруг живых
            foreach (Cell cell in NewListCells)
            {
                AddNearestCells(cell);
            }
        }

        /// <summary>
        /// Создание нового игрового поля из блока ячеек.
        /// </summary>
        /// <param name="block">Блок ячеек.</param>
        internal void SetField(Block block)
        {
            Clear();

            IfNeededToMakeResizing(block.Size.Width, block.Size.Height);

            PlaceBlock(block, CellLocation.Empty);

            PrepareField();
        }

        /// <summary>
        /// Помещает указанный блок ячеек на игровое поле, в заданные координаты.
        /// Ячейки, чьи координаты выходят за пределы поля, игнорируются.
        /// </summary>
        /// <param name="block">Блок ячеек.</param>
        /// <param name="begin">Позиция на поле для размещения.</param>
        public void PlaceBlock(Block block, CellLocation begin)
        {
            foreach (Cell cell in block)
            {
                cell.Offset(begin);
                field.Add(cell);
            }
        }

        /// <summary>
        /// Возвращает список ячеек, находящихся в заданной прямоугольной области.
        /// </summary>
        /// <param name="fieldRect">Ограничивающий прямоугольник.</param>
        /// <returns>Список ячеек.</returns>
        internal List<Cell> GetCells(Rectangle fieldRect)
        {
            List<Cell> listCells = new List<Cell>();

            foreach (Cell cell in field)
            {
                if ((cell.IsLive() || cell.IsStatic()) &&
                    fieldRect.Contains(cell.Location.X, cell.Location.Y))
                {
                    listCells.Add(cell);
                }
            }

            return listCells;
        }

        /// <summary>
        /// Возвращает список всех ячеек игрового поля.
        /// </summary>
        /// <returns>Список ячеек.</returns>
        internal List<Cell> GetCells() => GetCells(Rectangle);

        private void IfNeededToMakeResizing(int dx, int dy)
        {
            if (dx != width || dy != height)
            {
                Clear();

                field.Resize(dx, dy);

                width = dx;
                height = dy;
            }
        }

        /// <summary>
        /// Подготавливает список текущих клеток для начала игры.
        /// </summary>
        internal void PrepareField()
        {
            NewListCells.Clear();

            foreach (Cell currentCell in field)
            {
                if (currentCell.IsLive())
                {
                    NewListCells.Add(currentCell);
                }
            }

            CurrentListCells.Clear();
            CurrentListCells.AddRange(NewListCells);

            foreach (Cell cell in NewListCells)
            {
                AddNearestCells(cell);
            }
        }

        public void AddAndPrepareCell(Cell cell)
        {
            Cell c = field[cell.Location.X, cell.Location.Y];

            if (c != null && c.IsNoCell())
            {
                field.Remove(c);
            }

            field.Add(cell);

            NewListCells.Add(cell);

            CurrentListCells.Add(cell);

            AddNearestCells(cell);
        }

        public void RemoveAndPrepareCell(Cell cell)
        {
            field.Remove(cell);

            NewListCells.Remove(cell);

            CurrentListCells.Remove(cell);
        }

        public void GospersGliderGun(short x0, short y0)
        {
            long[] bitmap = new long[] {
                36,
                0b000000000000000000000000100000000000,
                0b000000000000000000000010100000000000,
                0b000000000000110000001100000000000011,
                0b000000000001000100001100000000000011,
                0b110000000010000010001100000000000000,
                0b110000000010001011000010100000000000,
                0b000000000010000010000000100000000000,
                0b000000000001000100000000000000000000,
                0b000000000000110000000000000000000000
            };

            PlaceLifeObject(x0, y0, bitmap);
        }

        public void DiagonalSpaceShip(int x0, int y0)
        {
            long[] bitmap = new long[] {
                22,
                0b0000000000110000000000,
                0b0000000001001000000000,
                0b0000000011000000000000,
                0b0000000001011000000000,
                0b0000000000101110000000,
                0b0000000000110111000000,
                0b0000000000001000011000,
                0b0000000000001110000110,
                0b0010000000001010000000,
                0b0111000000001001000000,
                0b1000110000000000000000,
                0b1001010000000110100100,
                0b0101101111000100011110,
                0b0000110100011000000010,
                0b0000110110010000000001,
                0b0000010001000000001011,
                0b0000000000010000000100,
                0b0000001000001000000100,
                0b0000001000001001000000,
                0b0000000100011000110000,
                0b0000000100001101000000,
                0b0000000000000011000000
            };

            PlaceLifeObject(x0, y0, bitmap);
        }

        private void PlaceLifeObject(int x0, int y0, long[] bitmap)
        {
            int x1 = x0 + (int)bitmap[0] - 1;
            int y1 = y0 + bitmap.Length - 1;
            int i = 1;

            for (int y = y0; y < y1; y++, i++)
            {
                long current = bitmap[i];
                int x = x1;

                for (int j = (int)bitmap[0]; j > 0; j--, x--)
                {
                    if ((current & 1) != 0)
                    {
                        Cell cell = new Cell(x, y) { active = true, Status = StatusCell.Yes };

                        field[x, y] = cell;
                    }

                    current >>= 1;
                }
            }
        }

        /// <summary>
        /// Заполнение поля клетками 
        /// </summary>
        internal void SettingCells()
        {
            SetRandomCells();   // случайным образом

            if (isBorder)
            {
                SetStaticBorderCells();   // Граница игрового поля
            }
        }

        internal void SettingCells(int dx, int dy, float density, bool isBorderCells)
        {
            Clear();

            IfNeededToMakeResizing(dx, dy);

            this.density = density;
            isBorder = isBorderCells;

            SetRandomCells();   // случайным образом

            if (isBorderCells)
            {
                SetStaticBorderCells();   // Граница игрового поля
            }
        }

        private void SetStaticBorderCells()
        {
            for (int x = 0; x < width; x++)
            {
                field[x, 0] = new Cell(x, 0) { Status = StatusCell.Static };
                field[x, height - 1] = new Cell(x, height - 1) { Status = StatusCell.Static };
            }

            for (int y = 1; y < height - 1; y++)
            {
                field[0, y] = new Cell(0, y) { Status = StatusCell.Static };
                field[width - 1, y] = new Cell(width - 1, y) { Status = StatusCell.Static };
            }
        }

        private void SetRandomCells()
        {
            Random rnd = new Random();
            Random rndX = new Random(rnd.Next());
            Random rndY = new Random(rnd.Next());

            int needNumberOfCells = (int)((width - padding * 2) * (height - padding * 2) * density);

            while (needNumberOfCells > 0)
            {
                int x;
                int y;

                do
                {
                    x = rndX.Next(padding, width - padding);
                    y = rndY.Next(padding, height - padding);
                }
                while (field[x, y] != null);

                field[x, y] = new Cell(x, y) { Status = StatusCell.Yes, active = true };

                needNumberOfCells--;
            }
        }
    }
}
