using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace life
{
    public partial class LifeForm : Form
    {
        private int count;      // счётчик ходов

        private Field field;            // игровое поле

        private int fieldX = 80;        // Размер поля 
        private int fieldY = 60;        // в ячейках

        private Timer timer;            // 


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
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
        }


        private void InitialazeLifeForm()
        {
            count = 0;

            field = new Field(fieldX, fieldY);

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

            field.brushCellYes = Brushes.DarkGreen;
            field.brushCellNo = Brushes.LightGray;

            field.EnterCells();
            field.FieldInit();
            field.DrawAll();

            timer = new Timer
            {
                Interval = 100
            };
            
            timer.Tick += Timer_Tick;
            timer.Start();

            KeyUp += LifeForm_KeyUp;

           
        }

        private void LifeForm_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Space)
            {
                if (timer.Enabled)
                {
                    timer.Enabled = false;
                }
                else
                {
                    timer.Enabled = true;
                }
            }
            else
            if(e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            field.CalcNextStep();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            
            field.DrawAll(e.Graphics);
        }
    }
}
