using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace life
{
    /// <summary>
    /// Блок игрового поля.
    /// </summary>
    public class Block : IEnumerable<Cell>
    {
        /// <summary>
        /// Блок - игровое поле целиком.
        /// </summary>
        public const bool GameField = false;

        /// <summary>
        /// Блок - простой блок клеток.
        /// </summary>
        public const bool GameBlock = true;

        /// <summary>
        /// Тип блока:
        /// 'B' - простой блок клеток,
        /// 'F' - игровое поле целиком.
        /// </summary>
        private char blockType;

        /// <summary>
        /// Указывает, является ли данный блок, простым блоком клеток.
        /// </summary>
        public bool IsGameBlock => (blockType == 'B');

        /// <summary>
        /// Указывает, является ли данный блок, игровым полем.
        /// </summary>
        public bool IsGameField => (blockType == 'F');

        /// <summary>
        /// Координата X блока в клетках игрового поля.
        /// </summary>
        public int X { get => rectangle.X; private set => rectangle.X = value; }

        /// <summary>
        /// Координата Y блока в клетках игрового поля.
        /// </summary>
        public int Y { get => rectangle.Y; private set => rectangle.Y = value; }

        /// <summary>
        /// Ширина блока в клетках игрового поля.
        /// </summary>
        public int Width { get => rectangle.Width; private set => rectangle.Width = value; }

        /// <summary>
        /// Высота блока в клетках игрового поля.
        /// </summary>
        public int Height { get => rectangle.Height; private set => rectangle.Height = value; }

        /// <summary>
        /// Размер блока в клетках игрового поля.
        /// </summary>
        public Size Size => rectangle.Size;

        /// <summary>
        /// Прямоугольник, ограничивающий блок в клетках игрового поля.
        /// </summary>
        public Rectangle Rectangle => rectangle;

        /// <summary>
        /// Возвращает начальную точку прямоугольника, ограничивающего блок в клетках игрового поля.
        /// </summary>
        public Point Location => rectangle.Location;


        /// <summary>
        /// Прямоугольник, ограничивающий блок в клетках игрового поля.
        /// </summary>
        private Rectangle rectangle;

        /// <summary>
        /// Список клеток игрового поля.
        /// </summary>
        private readonly List<Cell> cells;

        /// <summary>
        /// Корректируем координаты прямоугольника блока.
        /// </summary>
        /// <param name="cell">Добавляемая клетка.</param>
        private void AdjustRectangle(Cell cell)
        {
            if (cell.Location.X < X)
            {
                Width += X - cell.Location.X;
                X = cell.Location.X;
            }
            else
            if (cell.Location.X >= X + Width)
                Width = cell.Location.X - X + 1;

            if (cell.Location.Y < Y)
            {
                Height += Y - cell.Location.Y;
                Y = cell.Location.Y;
            }
            else
            if (cell.Location.Y >= Y + Height)
                Height = cell.Location.Y - Y + 1;
        }

        /// <summary>
        /// Читает целое число, ограниченное разделителями, из потока.
        /// </summary>
        /// <param name="sr">Поток для чтения.</param>
        /// <returns>Прочитанное значение.</returns>
        private int ReadInt(StreamReader sr)
        {
            StringBuilder s = new StringBuilder(10);

            while (sr.Peek() > -1)
            {
                char c = (char)sr.Read();

                if (c == ',' || c == ';' || c == ':')
                {
                    break;
                }

                s.Append(c);
            }

            return int.Parse(s.ToString());
        }

        /// <summary>
        /// Читает данные одной клетки и создаёт её.
        /// </summary>
        /// <param name="sr">Поток для чтения.</param>
        /// <returns>возвращает созданную клетку.</returns>
        private Cell ReadCell(StreamReader sr)
        {
            int x = ReadInt(sr);
            int y = ReadInt(sr);

            Cell cell = new Cell(x, y)
            {
                Status = (sr.Read() == 's') ? StatusCell.Static : StatusCell.Yes,
                NewStatus = StatusCell.Yes,
                active = true
            };

            sr.Read();  // разделитель ';'

            return cell;
        }


        public Block()
        {
            cells = new List<Cell>();

            rectangle = Rectangle.Empty;
        }

        /// <summary>
        /// Создаёт новый блок из клеток указанного игрового поля.
        /// </summary>
        /// <param name="field">Игровое поле.</param>
        public Block(Field field)
        {
            cells = new List<Cell>();

            rectangle = Rectangle.Empty;

            cells.AddRange(field.GetCells());
        }

        /// <summary>
        /// Смещаем блок в указанную точку.
        /// </summary>
        /// <param name="start">Новая начальная точка блока в клетках игрового поля.</param>
        public void SetStartPoint(Point start)
        {
            // Вычисляем смещение координат.
            start.Offset(-X, -Y);

            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].Offset(start);
            }

            // Корректируем границы блока после изменения стартовой точки.
            rectangle.Offset(start);
        }

        /// <summary>
        /// Добавляем клетку в блок
        /// </summary>
        /// <param name="cell"></param>
        public void Add(Cell cell)
        {
            // Проверяем, нужна ли первичная инициализация
            // минимальных и максимальных значений координат. 
            if (cells.Count == 0)
            {
                X = cell.Location.X;
                Y = cell.Location.Y;

                Width = Height = 1;
            }
            else
            {
                AdjustRectangle(cell);
            }

            cells.Add(cell);
        }

        /// <summary>
        /// Добавить список клеток.
        /// </summary>
        /// <param name="rangeCells"></param>
        public void AddRange(IEnumerable<Cell> rangeCells)
        {
            foreach (Cell cell in rangeCells)
            {
                Add(new Cell(cell));
            }
        }

        /// <summary>
        /// Очистка содержимого блока.
        /// </summary>
        public void Clear()
        {
            cells.Clear();

            rectangle = Rectangle.Empty;
        }

        /// <summary>
        /// Читает блок игрового поля из файла.
        /// </summary>
        /// <param name="filename">Имя файла.</param>
        /// <returns>true - если загрузка успешна, false - если ошибка.</returns>
        public bool LoadFromFile(string filename)
        {
            if (filename != null)
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    try
                    {
                        Load(reader);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\npublic void Block::LoadFromFile(string filename)");

                        return false;
                    }
                }
            }
            else
            {
                rectangle = Rectangle.Empty;
            }

            return true;
        }

        /// <summary>
        /// Читает блок клеток из потока.
        /// </summary>
        /// <param name="reader">Поток для чтения.</param>
        public void Load(StreamReader reader)
        {
            Clear();

            {
                blockType = (char)reader.Read();
                reader.Read();
            }

            Width = ReadInt(reader);
            Height = ReadInt(reader);

            int count = ReadInt(reader);

            for (int i = 0; i < count; i++)
            {
                Add(ReadCell(reader));
            }
        }

        /// <summary>
        /// Запись блока клеток в указанный файл.
        /// Формат текстового файла:
        /// {width},{height},{Count}:{x},{y},{isStaticCell = 'n'/'s'};...{xn},{yn}{'n'\'s'};
        /// </summary>
        /// <param name="filename">Имя файла.</param>
        /// <param name="typeBlock">Тип блока: true - простой блок, false - игровое поле</param>
        public void SaveToFile(string filename, bool typeBlock = GameField)
        {
            if (cells.Count != 0 && filename != null)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(filename))
                    {
                        Save(writer, typeBlock);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + $"public void {GetType().Name}::SaveToFile(string filename)");
                }
            }
        }

        /// <summary>
        /// Сохраняет блок клеток в потоке.
        /// </summary>
        /// <param name="writer">Поток для сохранения.</param>
        /// <param name="typeBlock">Тип блока: true - простой блок, false - игровое поле</param>
        private void Save(StreamWriter writer, bool typeBlock)
        {
            writer.Write("{0},", (typeBlock) ? 'B' : 'F');

            writer.Write($"{Size.Width},{Size.Height},{cells.Count}:");

            foreach (var cell in cells)
            {
                char isStaticCell = (cell.IsStatic) ? 's' : 'n';
                writer.Write($"{cell.Location.X},{cell.Location.Y},{isStaticCell};");
            }
        }

        /// <summary>
        /// Отрисовывает блок на указанном игровом поле, в указанной точке.
        /// </summary>
        /// <param name="field">Игровое поле для отрисовки блока.</param>
        /// <param name="g">Экземпляр Graphics для отрисовки.</param>
        /// <param name="bitmaps">Хранилище изображений клеток для отрисовки.</param>
        /// <param name="p">Координаты.</param>
        public void Draw(Field field, Graphics g, BitmapCellsStorage bitmaps, Point p) => Draw(field, g, bitmaps, p.X, p.Y);

        /// <summary>
        /// Отрисовывает блок на указанном игровом поле в указанные координаты x и y.
        /// Координаты в клетках игрового поля.
        /// </summary>
        /// <param name="field">Игровое поле для отрисовки блока.</param>
        /// <param name="g">Экземпляр Graphics для отрисовки.</param>
        /// <param name="bitmaps">Хранилище изображений клеток для отрисовки.</param>
        /// <param name="x">Кордината x.</param>
        /// <param name="y">Координата y.</param>
        public void Draw(Field field, Graphics g, BitmapCellsStorage bitmaps, int x, int y)
        {
            // Смещение для определения координат клеток.
            int dx = x - X;
            int dy = y - Y;

            foreach (Cell cell in cells)
            {
                Point p = cell.Location;

                p.Offset(dx, dy);

                if (field.Contains(p))
                {
                    return; // Клетка блока находится вне поля, отрисовке не подлежит
                }

                cell.Draw(g, bitmaps, p);
            }
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            return cells.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
