using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace life
{
    public partial class LifeForm : Form
    {
        private int count;      // счётчик ходов

        private Field field;            // игровое поле

        private int fieldX = 100;        // Размер поля 
        private int fieldY = 80;        // в ячейках


        public LifeForm()
        {
            InitializeComponent();

            InitialazeLifeForm();
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Text = "Life";
            BackColor = SystemColors.Window;
            ResizeRedraw = true;
        }


        private void InitialazeLifeForm()
        {
            count = 0;

            field = new Field(100, 80);

            // Корректируем размер рабочей области формы кратно размерам поля
            ClientSize = new Size()
            {
                Width  = (int)Math.Ceiling((decimal)ClientSize.Width / fieldX) * fieldX,    //  + (Size.Width - ClientSize.Width) 
                Height = (int)Math.Ceiling((decimal)ClientSize.Height / fieldY) * fieldY    //  + (Size.Height - ClientSize.Height)
            };

            // размер ячейки
            field.CellSize = new Size() 
                            {
                                Width  = ClientSize.Width / fieldX, 
                                Height = ClientSize.Height / fieldY 
                            };

            field.brushCellYes = Brushes.BlueViolet;
            field.brushCellNo = new SolidBrush( this.BackColor);

            field.EnterCells();
            field.FieldInit();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            field.Draw(e.Graphics);
        }
    }
}
