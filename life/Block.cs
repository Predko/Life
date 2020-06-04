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
        private int minX, maxX;
        private int minY, maxY;

        /// <summary>
        /// Размер блока.
        /// </summary>
        public Size Size => new Size(maxX - minX + 1, maxY - minY + 1);

        /// <summary>
        /// Прямоугольник, ограничивающий блок/
        /// </summary>
        public Rectangle Rectangle
        {
            get => new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
            set
            {
                minX = value.X;
                minY = value.Y;
                maxX = value.X + value.Width;
                maxY = value.Y + value.Height;
            }
        }

        /// <summary>
        /// Возвращает начальную точку прямоугольника, ограничивающего блок.
        /// </summary>
        public Point Location => new Point(minX, minY);

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
            if (cell.Location.X < minX)
                minX = cell.Location.X;
            else
            if (cell.Location.X > maxX)
                maxX = cell.Location.X;

            if (cell.Location.Y < minY)
                minY = cell.Location.Y;
            else
            if (cell.Location.Y > maxY)
                maxY = cell.Location.Y;
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

            minX = minY = maxX = maxY = 0;
        }

        public Block(Field field)
        {
            cells.AddRange(field.GetCells());
        }

        /// <summary>
        /// Смещаем блок в указанную точку.
        /// </summary>
        /// <param name="start">Новая начальная точка блока.</param>
        public void SetStartPoint(Point start)
        {
            // Вычисляем смещение координат.
            int dx = start.X - minX;
            int dy = start.Y - minY;

            foreach (Cell cell in cells)
            {
                cell.Location = new CellLocation(cell.Location.X + dx, cell.Location.Y + dy);
            }

            minX = start.X;
            minY = start.Y;
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
                minX = maxX = cell.Location.X;
                minY = maxY = cell.Location.Y;
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
                Add(cell);
            }
        }

        /// <summary>
        /// Очистка содержимого блока.
        /// </summary>
        public void Clear()
        {
            cells.Clear();

            Rectangle = Rectangle.Empty;
        }

        /// <summary>
        /// Читает блок игрового поля из файла.
        /// </summary>
        /// <param name="filename">Имя файла.</param>
        /// <returns>Размер прочитанного поля.</returns>
        public Size LoadFromFile(string filename)
        {
            if (filename != null)
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    try
                    {
                        cells.Clear();

                        int dx = ReadInt(reader);
                        int dy = ReadInt(reader);

                        int count = ReadInt(reader);

                        for (int i = 0; i < count; i++)
                        {
                            Add(ReadCell(reader));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\npublic void Block::LoadFromFile(string filename)");
                    }
                }
            }
            else
            {
                Rectangle = Rectangle.Empty;
            }

            return Size;
        }

        /// <summary>
        /// Запись блока игрового поля в указанный файл.
        /// Формат текстового файла:
        /// {width},{height},{Count}:{x},{y},{isStaticCell = 'n'/'s'};...{xn},{yn}{'n'\'s'};
        /// </summary>
        /// <param name="filename">Имя файла.</param>
        public void SaveToFile(string filename)
        {
            if (cells.Count != 0 && filename != null)
            {

                try
                {
                    using (StreamWriter writer = new StreamWriter(filename))
                    {
                        writer.Write($"{Size.Width},{Size.Height},{cells.Count}:");

                        foreach (var cell in cells)
                        {
                            char isStaticCell = (cell.IsStatic) ? 's' : 'n';
                            writer.Write($"{cell.Location.X},{cell.Location.Y},{isStaticCell};");
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + $"public void {GetType().Name}::SaveToFile(string filename)");
                }
            }
        }

        /// <summary>
        /// Отрисовывает блок на указанном игровом поле.
        /// </summary>
        /// <param name="field">Игровое поле для отрисовки блока.</param>
        /// <param name="g">Экземпляр Graphics для отрисовки.</param>
        /// <param name="bitmaps">Хранилище изображений клеток для отрисовки.</param>
        /// <param name="x">Смещение блока по X от начала поля.</param>
        /// <param name="y">Смещение блока по Y от начала поля.</param>
        public void Draw(Field field, Graphics g, BitmapCellsStorage bitmaps, int x, int y)
        {
            foreach (Cell cell in cells)
            {
                int newX = cell.Location.X + x;
                int newY = cell.Location.Y + y;

                if (newX < 0 || newX >= field.width || newY < 0 || newY >= field.height)
                {
                    return; // Клетка блока находится вне поля, отрисовке не подлежит
                }

                cell.Location = new CellLocation(newX, newY);

                cell.Draw(g, bitmaps);
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
