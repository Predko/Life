using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace life
{
    public class LogOfSteps
    {
        private readonly Field currentField;

        private readonly LinkedList<string> steps;
        private LinkedListNode<string> current;

        public int Count => steps.Count;

        /// <summary>
        /// Конструктор класса логирования шагов игры.
        /// </summary>
        /// <param name="filename">Имя файла для записи шагов.</param>
        /// <param name="f">Логируемое игровое поле.</param>
        public LogOfSteps(Field f)
        {
            steps = new LinkedList<string>();

            current = steps.Last;

            currentField = f;
        }

        public void Clear()
        {
            steps.Clear();
            current = steps.Last;
        }

        public void Add(string s) => current = steps.AddLast(s);

        public bool IsLogEmpty() => (current == null);

        /// <summary>
        /// Восстанавливает состояние клеток на предыдущем ходу.
        /// </summary>
        /// <param name="newListCells">Список живых и статичных клеток текущего хода.</param>
        /// <returns>false - если не удалось выполнить операцию.</returns>
        public bool Previous(List<Cell> newListCells)
        {
            // Проверяем не пуст ли лог
            if (current == null)
            {
                return true;   // Лог пуст
            }

            // извлекаем список изменений клеток из журнала
            List<Cell> cellsFromLog = GetStep();

            if (cellsFromLog == null)
            {
                MessageBox.Show("Не удалось извлечь данные из истории.");

                Clear();

                return false;
            }

            // Заносим в список newListCells все живые клетки,
            // за исключением тех, которые появились на этом ходе
            // и добавляем те, которые исчезли
            foreach (var cell in cellsFromLog)
            {
                // Если клетка была добавлена на предыдущем ходу
                // удаляем её с поля и из текущего списка активных клеток при ходе назад
                if (cell.NewStatus == StatusCell.Yes)
                {
                    int hashCode = cell.GetHashCode();

                    Cell foundCell = newListCells.Find(c => c.GetHashCode() == hashCode);

                    if (foundCell == null)
                    {
                        MessageBox.Show("Не соответствие истории и текущих клеток.");

                        Clear();

                        return false;
                    }

                    foundCell.NewStatus = StatusCell.No;
                    foundCell.Status = StatusCell.No;

                    foundCell.active = false; // make inactive

                    // удаляем клетку с поля и из списка активных
                    // добавляем в список для отрисовки
                    newListCells.Remove(foundCell);

                    currentField.RemoveCell(foundCell);

                    currentField.AddToDraw(foundCell);
                }
                else
                // Если клетка исчезла на предыдущем ходе
                // добавим в список активных и на игровое поле
                // и для отрисовки
                if (cell.NewStatus == StatusCell.No)
                {
                    cell.active = true;
                    cell.Status = StatusCell.Yes;
                    cell.NewStatus = StatusCell.Yes;

                    newListCells.Add(cell);

                    currentField.AddCell(cell);

                    currentField.AddToDraw(cell);
                }
            }

            var delNode = current;

            current = current.Previous;

            steps.Remove(delNode);

            return true;
        }

        /// <summary>
        /// Сохраняет изменения в клетках на игровом поле из списка cells в список steps
        /// Формат:
        ///    "Число клеток":"'-' если удалена""координата X","Координата Y";"Координата X","Координата Y"; ...
        ///    Разделителями являются только ':' ',' и ';'. Например: "2:10,5;-1,2" - две клетки, первая добавлена, вторая удалена
        /// </summary>
        /// <param name="cells">Список видимых клеток на игровом поле</param>
        public void SetStep(List<Cell> cells)
        {
            StringBuilder step = new StringBuilder(10 + cells.Count * 15);

            int count = 0;

            foreach (Cell cell in cells)
            {
                // Клетка изменит своё состояние?
                if (cell.IsChangeStatus)
                {
                    int x = cell.Location.X;

                    if (cell.NewStatus == StatusCell.No)
                    {
                        x *= -1;    // клетка удалена
                    }

                    step.Append($"{x},{cell.Location.Y};");

                    count++;
                }
            }

            if (step.Length != 0)
            {
                step.Remove(step.Length - 1, 1); // удаляем последний символ ";"
            }

            step.Insert(0, $"{count}:");

            steps.AddLast(step.ToString());

            current = steps.Last;
        }

        /// <summary>
        /// Возвращает список клеток cells изменённых на предыдущем ходу.
        /// Если не было изменений, возвращает пустой список.
        /// null - если была ошибка
        /// </summary>
        /// <returns>Список клеток.</returns>
        private List<Cell> GetStep()
        {
            string[] step = steps.Last.Value.Split(":,;".ToCharArray());

            List<Cell> cells;

            try
            {
                cells = new List<Cell>();

                int count = int.Parse(step[0]);
                if (count == 0)
                {
                    return cells;
                }

                for (int i = 1; (i - 1) / 2 < count; i += 2)
                {
                    int x = int.Parse(step[i]);
                    int y = int.Parse(step[i + 1]);

                    StatusCell sc;

                    // если x отрицательна, клетка была удалена
                    if (x < 0)
                    {
                        sc = StatusCell.No;
                        x *= -1;
                    }
                    else
                    {
                        sc = StatusCell.Yes;
                    }

                    Cell cell = new Cell(new CellLocation(x, y))
                    {
                        NewStatus = sc
                    };

                    cells.Add(cell);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nprivate List<Cell> GetStep()");
                return null;
            }


            return cells;
        }
    }
}
