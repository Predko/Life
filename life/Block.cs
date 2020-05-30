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
    internal class Block : IEnumerable<Cell>
    {
        /// <summary>
        /// Размер блока.
        /// </summary>
        public Size Size { get; private set; }

        /// <summary>
        /// Список ячеек игрового поля.
        /// </summary>
        private readonly List<Cell> cells;

        public Block()
        {
            cells = new List<Cell>();
        }

        public Block(Field field)
        {
            cells = field.GetCells();

            int minX, maxX;
            int minY, maxY;

            minX = maxX = cells.First().Location.X;
            minY = maxY = cells.First().Location.Y;

            // Находим минимальные и максимальные значения координат x и y
            foreach (Cell cell in cells)
            {
                int x = cell.Location.X;
                int y = cell.Location.Y;

                if (x < minX)
                    minX = x;
                else
                if (x > maxX)
                    maxX = x;

                if (y < minY)
                    minY = y;
                else
                if (y > maxY)
                    maxY = y;
            }

            Size = new Size(maxX - minX + 1, maxY - minY + 1);
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
                            cells.Add(ReadCell(reader));
                        }

                        Size = new Size(dx, dy);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\npublic void Block::LoadFromFile(string filename)");
                    }
                }
            }
            else
            {
                Size = Size.Empty;
            }

            return Size;
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
        /// Читает данные одной ячейки и создаёт её.
        /// </summary>
        /// <param name="sr">Поток для чтения.</param>
        /// <returns>возвращает созданную ячейку.</returns>
        private Cell ReadCell(StreamReader sr)
        {
            int x = ReadInt(sr);
            int y = ReadInt(sr);

            Cell cell = new Cell(x, y)
            {
                Status = (sr.Read() == 's') ? StatusCell.Static : StatusCell.Yes,
                NewStatus = StatusCell.Yes,
                active = true   //,isStaticCell = (sr.Read() == 's') ? true : false
            };

            sr.Read();  // разделитель ';'

            return cell;
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
                            char isStaticCell = (cell.IsStatic()) ? 's' : 'n';
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
        /// <param name="bitmaps">Хранилище изображений ячеек для отрисовки.</param>
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
                    return; // Ячейка блока находится вне поля, отрисовке не подлежит
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
