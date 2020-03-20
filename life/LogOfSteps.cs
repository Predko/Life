using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace life
{
    class LogOfSteps
    {
        private string fileName;

        private readonly LinkedList<string> steps;
        private LinkedListNode<string> current;


        /// <summary>
        /// Конструктор класса логирования шагов игры
        /// </summary>
        /// <param name="filename">Имя файла для записи шагов</param>
        public LogOfSteps(string filename)
        {
            fileName = filename;
            steps = new LinkedList<string>();
        }

        public void Add(string s) => current = steps.AddLast(s);

        public bool Next(List<Cell> cells) 
        {
            if(current.Next == null)   // достигнут конец лога
            {
                return false;
            }

            current = current.Next;




            return false;
        }




        public void SaveStep(List<Cell> cells)
        {
            // массив символов step[] - строки, содержащие данные о изменениях в состоянии ячеек поля
            string[] step = new string[1 + cells.Count];

            int i = 0;
            
            step[i++] = $"{cells.Count}:";

            foreach(Cell cell in cells)
            {
                step[i++] = $"+({cell.Location.X},{cell.Location.Y});";
            }


        }

        public void LoadStep(List<Cell> cells)
        {

        }




    }
}
