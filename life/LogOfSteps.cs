using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace life
{
    public class LogOfSteps
    {
        private string fileName;

        Field currentField;

        private readonly LinkedList<string> steps;
        private LinkedListNode<string> current;


        /// <summary>
        /// Конструктор класса логирования шагов игры
        /// </summary>
        /// <param name="filename">Имя файла для записи шагов</param>
        /// <param name="field">Текущее игровое поле</param>
        public LogOfSteps(string filename, Field field)
        {
            fileName = filename;

            currentField = field;

            steps = new LinkedList<string>();

            current = steps.Last;
        }

        public void Add(string s) => current = steps.AddLast(s);

        public List<Cell> Next(List<Cell> cells) 
        {
            if(current.Next == null)   // достигнут конец лога
            {
                return null;
            }

            current = current.Next;




            return cells;
        }

        /// <summary>
        /// Восстанавливает состояние ячеек на предыдущем ходу
        /// cells - состояние ячеек на текущем ходу
        /// Возвращает изменённый список
        /// </summary>
        /// <param name="currentCells"></param>
        /// <returns></returns>
        public bool Previous(List<Cell> currentCells, List<Cell> newListCells)
        {
            List<Cell> listCopyOfCells = new List<Cell>();
            List<Cell> newCells = new List<Cell>();     // список клеток для добавления

            // делаем копию текущего списка ячеек и их точного состояния
            foreach (var cell in  currentCells)
            {
                listCopyOfCells.Add(new Cell(cell));
            }

            // Проверяем не пуст ли лог
            if (current == null)
            {
                current = steps.Last;

                if (current == null)
                {
                    return false;   // Лог пуст
                }
            }
            
            // извлекаем список изменений клеток из журнала
            List<Cell> cellsFromLog = GetStep();

            if (cellsFromLog == null)
            {
                MessageBox.Show("Не удалось извлечь данные из истории");
                return false;
            }

            // Заносим в список newListCells все живые клетки,
            // за исключением тех, которые появились на этом ходе
            // и добавляем те, которые исчезли
            foreach (var cell in cellsFromLog)
            {
                // Если клетка была добавлена на предыдущем ходе
                // помечаем её для удаления при ходе назад
                if (cell.NewStatus == StatusCell.Yes) 
                {
                    int hashCode = cell.GetHashCode();

                    Cell foundCell = currentCells.Find(c => c.GetHashCode() == hashCode);
                    
                    if (foundCell == null)
                    {
                        MessageBox.Show("Не соответствие истории и текущих ячеек");

                        // восстанавливаем состояние клеток
                        currentCells.Clear();
                        currentCells.AddRange(listCopyOfCells);

                        return false;
                    }

                    foundCell.NewStatus = StatusCell.No;
                    
                    foundCell.active = false; // make inactive
                }
                else
                // Если клетка исчезла на предыдущем ходе
                // добавим в список для изменения статуса и добавления её при ходе назад
                if (cell.NewStatus == StatusCell.No)
                {                                    
                    cell.field = currentField;
                    cell.active = true;
                    cell.Status = StatusCell.No;
                    cell.NewStatus = StatusCell.Yes;

                    newCells.Add(cell);
                }
            }

            currentCells.AddRange(newCells);

            // формируем новый список следующего хода
            foreach (var cell in currentCells)
            {
                if ((cell.IsLive() && cell.NewStatus != StatusCell.No) ||
                    cell.NewStatus == StatusCell.Yes)
                {
                    newListCells.Add(cell);
                }
            }

            current = current.Previous;
            return true;
        }

        /// <summary>
        /// Сохраняет изменения в ячейках на игровом поле из списка cells в список steps
        /// Формат:
        ///    "Число ячеек":"'-' если удалена""координата X","Координата Y";"'-' если удалена""Координата X","Координата Y"; ...
        ///    Разделителями являются только ':' ',' и ';'. Например: "2:10,5;-1,2" - две ячейки, первая добавлена, вторая удалена
        /// </summary>
        /// <param name="cells">Список видимых ячеек на игровом поле</param>
        public void SetStep(List<Cell> cells)
        {
            StringBuilder step = new StringBuilder (10 + cells.Count * 15);

            int count = 0;

            foreach(Cell cell in cells)
            {
                // Ячейка изменит своё состояние?
                if (cell.IsChangeStatus())
                {
                    int x = cell.Location.X;

                    if (cell.NewStatus == StatusCell.No)
                    {
                        x *= -1;    // ячейка удалена
                    }
                
                    step.Append ($"{x},{cell.Location.Y};");

                    count++;
                }
            }

            step.Remove(step.Length - 1, 1); // удаляем последний символ ";"

            step.Insert(0, $"{count}:");

            if (current == steps.Last)
            {
                steps.AddLast(step.ToString());

                current = steps.Last;
            }
        }

        /// <summary>
        /// Возвращает список ячеек cells изменённых в current.Value
        /// cell.NewStatus - изменения иячейки
        /// </summary>
        /// <returns></returns>
        private List<Cell> GetStep()
        {
            string[] step = steps.Last.Value.Split(":,;".ToCharArray());

            List<Cell> cells = null;

            try
            {
                int count = int.Parse(step[0]);
                if (count == 0)
                {
                    return null;
                }

                cells = new List<Cell>();

                for (int i = 1; (i - 1) / 2 < count; i += 2)
                {
                    int x = int.Parse(step[i]);
                    int y = int.Parse(step[i + 1]);

                    StatusCell sc;

                    // если x отрицательна, ячейка была удалена
                    if (x < 0)
                    {
                        sc = StatusCell.No;
                        x *= -1; 
                    }
                    else
                    {
                        sc = StatusCell.Yes;
                    }

                    Cell cell = new Cell(null, new FieldLocation(x, y))
                    {
                        NewStatus = sc
                    };

                    cells.Add(cell);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nprivate List<Cell> GetStep()");
            }


            return cells;
        }

        /// <summary>
        /// Сохранение истории в файл
        /// </summary>
        public void SaveFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    foreach (var stepString in steps)
                    {
                        writer.WriteLine(stepString);
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message + "public void SaveFile() in LogOfSteps.cs");
            }
        }

        /// <summary>
        /// Загрузка истории из файла
        /// </summary>
        public void LoadFile()
        {
            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string stepString;

                    while ((stepString = reader.ReadLine()) != null)
                    {
                        steps.AddLast(stepString);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "public void SaveFile() in LogOfSteps.cs");
            }
        }
    }
}
