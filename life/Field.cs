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
    /// <summary>
    /// Игровое поле
    /// </summary>
    public class Field : IDisposable
    {
        public int width;
        public int height;

        /// <summary>
        /// Отступ от края игрового поля, где не будут располагаться клетки, кроме статичных клеток.
        /// </summary>
        private const int padding = 2;

        public int CellSize { get; set; }
        /// <summary>
        /// Отображаемый размер игрового поля в пикселах
        /// </summary>
        public Rectangle Bounds { get; set; }

        Graphics bitmapGraphics;

        public Bitmap bitmap;
        public Bitmap NormalCellBitmap;
        public Bitmap StaticCellBitmap;

        public Rectangle rectCellYes;
        public Rectangle rectCellNo;

        // массив координат ячеек вокруг данной
        private readonly CellLocation[] Dxy = new CellLocation[8];

        /// <summary>
        /// хранилище ячеек игрового поля
        /// </summary>
        private readonly ICellArray field;

        /// <summary>
        /// граница поля есть/нет
        /// </summary>
        public bool isBorder;

        /// <summary>
        /// плотность генерируемого поля
        /// </summary>
        public float density;

        /// <summary>
        /// Список активных клеток текущего хода
        /// </summary>
        private readonly List<Cell> CurrentListCells;
        
        /// <summary>
        /// Список активных клеток для следующего хода
        /// </summary>
        public List<Cell> NewListCells;

        /// <summary>
        /// Список клеток для отрисовки
        /// </summary>
        private readonly List<Cell> ListCellsForDraw;

        /// <summary>
        /// Запись сделанных ходов
        /// </summary>
        private readonly LogOfSteps steps;

        public Field(int width, int height, ICellArray cellArray, Bitmap normalCell, Bitmap staticCell, int cellSize = 15)
        {
            this.height = height;
            this.width = width;

            isBorder = true;
            density = 0.3f;

            Dxy[0].X = -1; Dxy[0].Y = -1; Dxy[1].X = -1; Dxy[1].Y = 0; Dxy[2].X = -1; Dxy[2].Y = 1;
            Dxy[3].X = 0; Dxy[3].Y = -1; Dxy[4].X = 0; Dxy[4].Y = 1;
            Dxy[5].X = 1; Dxy[5].Y = -1; Dxy[6].X = 1; Dxy[6].Y = 0; Dxy[7].X = 1; Dxy[7].Y = 1;

            CurrentListCells = new List<Cell>();
            NewListCells = new List<Cell>();

            ListCellsForDraw = new List<Cell>();

            field = cellArray ?? new BinaryTreeCells();

            steps = new LogOfSteps(this);

            NormalCellBitmap = normalCell;
            StaticCellBitmap = staticCell;

            CellSize = cellSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="normalCell"></param>
        /// <param name="staticCell"></param>
        internal void ChangeBitmapCells(Bitmap normalCell, Bitmap staticCell)
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
            }

            NormalCellBitmap.Dispose();

            NormalCellBitmap = normalCell;

            StaticCellBitmap.Dispose();

            StaticCellBitmap = staticCell;
        }

        internal void BitmapCellChanged(Bitmap bmcell)
        {
            NormalCellBitmap.Dispose();

            NormalCellBitmap = new Bitmap(bmcell, new Size(CellSize * 2, CellSize));
        }

        internal void AddToDraw(Cell cell)
        {
            ListCellsForDraw.Add(cell);
        }

        internal void BitmapStaticCellChanged(Bitmap bmcell)
        {
            StaticCellBitmap.Dispose();

            StaticCellBitmap = new Bitmap(bmcell, new Size(CellSize * 2, CellSize));
        }

        internal void InitBitmap()
        {

            bitmap = new Bitmap(Bounds.Width, Bounds.Height);

            if (bitmapGraphics != null)
            {
                bitmapGraphics.Dispose();
            }

            bitmapGraphics = Graphics.FromImage(bitmap);

            FirstDrawFieldToBitmap();
        }

        /// <summary>
        /// Очистка игрового поля - очистка хранилища ячеек, очистка лога шагов
        /// </summary>
        public void Clear()
        {
            field.Clear();

            steps.Clear();
        }

        public bool IsLogEmpty() => steps.IsBegin();

        public virtual void FirstDrawFieldToBitmap()
        {
            Size szbm = new Size(CellSize * 2, CellSize);

            NormalCellBitmap = new Bitmap(NormalCellBitmap, szbm);
            StaticCellBitmap = new Bitmap(StaticCellBitmap, szbm);

            rectCellYes = new Rectangle(0, 0, NormalCellBitmap.Width / 2, NormalCellBitmap.Height);
            rectCellNo = new Rectangle(NormalCellBitmap.Width / 2, 0, NormalCellBitmap.Width / 2, NormalCellBitmap.Height);

            DrawAll();
        }

        public void DrawAll()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bitmapGraphics.DrawImage(NormalCellBitmap, x * CellSize, y * CellSize, rectCellNo, GraphicsUnit.Pixel);
                }
            }
        }

        /// <summary>
        /// Отрисовка подготовленной битовой карты на форме
        /// </summary>
        /// <param name="g"></param>
        public void Redraw(Graphics g)
        {
            //Rectangle rectsrc = new Rectangle(0, 0, Bounds.Width, Bounds.Height);

            g.DrawImage(bitmap, Bounds);
        }

        /// <summary>
        /// Отрисовка изменившихся клеток игрового поля
        /// </summary>
        public void Draw()
        {
            foreach (Cell cell in field)
            {
                cell.Draw(this, bitmapGraphics);
            }
        }

        /// <summary>
        /// Добавление клетки на игровое поле
        /// </summary>
        /// <param name="cell"></param>
        public void AddCell(Cell cell) => field.Add(cell);

        /// <summary>
        /// Удаление клетки с игрового поля(кроме статичных клеток)
        /// </summary>
        /// <param name="cell"></param>
        public void RemoveCell(Cell cell)
        {
            if (!cell.IsStatic())   // не удаляем из списка статичные клетки
            {
                field.Remove(cell);
            }
        }

        /// <summary>
        /// Добавляем близлежащие клетки к данной в список активных клеток
        /// </summary>
        /// <param name="x">Координата x клетки</param>
        /// <param name="y">Координата y клетки</param>
        public void AddNearestCells(int x, int y)
        {
            foreach (CellLocation i in Dxy)
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

        // - подсчёт живых ячеек вокруг данной. status - состояние вызывающей ячейки - клетка : true - есть, false - нет
        /// <summary>
        /// Подсчёт живых ячеек вокруг данной.
        /// </summary>
        /// <param name="x">Координата x клетки</param>
        /// <param name="y">Координата y клетки</param>
        /// <returns>Число живых клеток, примыкающих к данной</returns>
        public int NumberLiveCells(int x, int y)
        {

            int count = 0;   // счётчик живых ячеек вокруг данной

            foreach (CellLocation loc in Dxy)
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
        /// Восстанавливает состояние клеток на состояние предыдущего хода
        /// </summary>
        /// <returns>true  если ещё есть записи в логе</returns>
        public bool PreviousStep()
        {
            ListCellsForDraw.Clear();

            // удаляем все не живые клетки с поля(активные клетки, вокруг живых)  
            foreach (Cell cell in CurrentListCells)
            {
                if (!cell.IsLive())
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

                DrawAll();

                return false;
            }

            // Отрисовка изменившихся ячеек
            foreach (Cell cell in ListCellsForDraw)
            {
                cell.Draw(this, bitmapGraphics);
            }

            CurrentListCells.Clear();
            CurrentListCells.AddRange(NewListCells);

            foreach (Cell cell in NewListCells)
            {
                AddNearestCells(cell.Location.X, cell.Location.Y);
            }

            return !steps.IsBegin();
        }

        /// <summary>
        /// Расчёт следующего хода.
        /// </summary>
        public void CalcNextStep()
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

            // Отрисовка изменившихся ячеек
            foreach (Cell cell in ListCellsForDraw)
            {
                cell.Draw(this, bitmapGraphics);
            }

            CurrentListCells.Clear();
            CurrentListCells.AddRange(NewListCells);

            // Подготавливаем список текущих клеток, добавляя в него клетки вокруг живых
            foreach (Cell cell in NewListCells)
            {
                AddNearestCells(cell.Location.X, cell.Location.Y);
            }
        }

        /// <summary>
        /// Сохраняет текущее поле в файл
        /// Формат текстового файла:
        /// {width},{height},{Count}:{x},{y},{isStaticCell = 'n'/'s'};...{xn},{yn}{'n'\'s'};
        /// </summary>
        /// <param name="fileName">Имя файла для записи</param>
        public void Save(string fileName = null)
        {
            if (fileName == null)
            {
                fileName = "Life.save";
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    int count = 0;

                    foreach (var cell in field)
                    {
                        if (cell.IsLive() || cell.IsStatic())
                        {
                            count++;
                        }
                    }

                    writer.Write($"{width},{height},{count}:");

                    foreach (var cell in field)
                    {
                        if (cell.IsLive() || cell.IsStatic())
                        {
                            char isStaticCell = (cell.IsStatic()) ? 's' : 'n';
                            writer.Write($"{cell.Location.X},{cell.Location.Y},{isStaticCell};");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "public void Field::Save(string fileName)");
            }
        }

        internal void SetField(Block block)
        {
            Clear();

            IfNeededToMakeResizing(block.size.Width, block.size.Height);

            PlaceBlock(block, CellLocation.Empty);

            PrepareField();

            FirstDrawFieldToBitmap();
        }

        private void PlaceBlock(Block block, CellLocation begin)
        {
            foreach (Cell cell in block.cells)
            {
                cell.Offset(begin);
                field.Add(cell);
            }
        }

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
        /// Подготавливаем список текущих клеток для начала игры
        /// </summary>
        public void PrepareField()
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
                AddNearestCells(cell.Location.X, cell.Location.Y);
            }
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
        public void SettingCells()
        {
            SetRandomCells();   // случайным образом

            if (isBorder)
            {
                SetStaticBorderCells();   // Граница игрового поля
            }
        }

        public void SettingCells(int dx, int dy, float density, bool isBorderCells)
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

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов
        private int numberCells;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    bitmapGraphics.Dispose();
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~Field()
        // {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
