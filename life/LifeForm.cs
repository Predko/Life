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
        private IContainer components = null;

        private Button btnStartStop;
        private const string name_btnStartStop = "btnStartStop";

        private Button btnMakeAStep;
        private const string name_btnMakeAStep = "btnDoStep";

        private const string text_start = "Start";
        private const string text_stop = "Stop";

        Label lbCount;

        private Font font;


        private int count;      // счётчик ходов

        private Field field;            // игровое поле

        private int fieldX = 80;        // Размер поля 
        private int fieldY = 50;        // в ячейках

        private const int x0 = 0;
        private int y0;                 // координата Y начала игрового поля

        private Timer timer;            // 

        public int Count
        {
            get
            {
                return count;
            }

            set
            {
                count = value;
                lbCount.Text = $"Step: {count}";
            }
        }

        public LifeForm():base()
        {
            InitializeComponent();

            InitialazeLifeForm();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
                        
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            field.DrawAll(e.Graphics);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();

            AutoSize = true;

            btnStartStop = new Button
            {
                Location = new Point(2, 2),
                Text = text_start,
                AutoSize = true,
                Name = name_btnStartStop,
                TabIndex = 1
            };

            btnStartStop.Click += Btn_Click;

            btnMakeAStep = new Button
            {
                Location = new Point(btnStartStop.Location.X + btnStartStop.Size.Width + 10, 2),
                Text = "Make a step",
                AutoSize = true,
                Name = name_btnMakeAStep,
                TabIndex = 2
            };

            btnMakeAStep.Click += Btn_Click;

            lbCount = new Label()
            {
                Location = new Point(btnMakeAStep.Location.X + btnMakeAStep.Size.Width + 10, 2),
                Text = "Step: ",
                AutoSize = true,
                Name = nameof(lbCount)
            };

            y0 = btnStartStop.Location.Y + btnStartStop.Size.Height + 2;
            
            ClientSize = new Size(800, 450);
            Text = "Life";
            BackColor = SystemColors.Window;
            ResizeRedraw = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;

            Controls.Add(btnStartStop);
            Controls.Add(btnMakeAStep);
            Controls.Add(lbCount);

            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode = AutoScaleMode.Font;

            // Корректируем положение lbCount по Y - по оси кнопок
            lbCount.Location = new Point(lbCount.Location.X, btnMakeAStep.Location.Y + btnMakeAStep.Size.Height / 2 - lbCount.Size.Height / 2);

            ResumeLayout(false);

            timer = new Timer
            {
                Interval = 50
            };

            timer.Tick += Timer_Tick;

            KeyUp += LifeForm_KeyUp;
        }

        private void InitialazeLifeForm()
        {
            Count = 0;

            SetSizeFormAndField();

            InitField();

            field.DrawAll();
        }

        private void InitField()
        {
            field.EnterCells();
            field.FieldInit();
        }

        // Корректируем размер рабочей области формы кратно размерам поля
        private void SetSizeFormAndField()
        {
            int cellSize = Math.Min((int)Math.Ceiling((decimal)(ClientSize.Width - x0) / fieldX),
                               (int)Math.Ceiling((decimal)(ClientSize.Height - y0) / fieldY));

            ClientSize = new Size()
            {
                Width = x0 + cellSize * fieldX,
                Height = y0 + cellSize * fieldY
            };

            field = new Field(fieldX, fieldY)
            {
                TopLeftCorner = new Point(x0, y0),

                CellSize = new Size(cellSize, cellSize),   // размер ячейки

                brushCellYes = Brushes.DarkGreen,
                brushCellNo = Brushes.LightGray
            };
        }

    }
}
