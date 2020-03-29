﻿using System;
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
        private readonly IContainer components = null;

        private Button btnStartStop;
        private const string name_btnStartStop = "btnStartStop";

        private Button btnMakeAStep;
        private const string name_btnMakeAStep = "btnDoStep";

        private Button btnPreviousStep;
        private const string name_btnPreviousStep = "btnPreviousStep";

        private Button btnSaveField;
        private const string name_btnSaveField = "btnSaveField";

        private Button btnLoadField;
        private const string name_btnLoadField = "btnLoadField";

        private const string text_start = "Start";
        private const string text_stop = "Stop";

        Label lbCount;

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

            field.Redraw(e.Graphics);
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

            btnPreviousStep = new Button
            {
                Location = new Point(btnMakeAStep.Location.X + btnMakeAStep.Size.Width + 10, 2),
                Text = "Previous step",
                AutoSize = true,
                Name = name_btnPreviousStep,
                TabIndex = 3,
                Enabled = false
            };

            btnPreviousStep.Click += Btn_Click;

            btnSaveField = new Button
            {
                Location = new Point(btnPreviousStep.Location.X + btnPreviousStep.Size.Width + 10, 2),
                Text = "Save Field",
                AutoSize = true,
                Name = name_btnSaveField,
                TabIndex = 4
            };

            btnSaveField.Click += Btn_Click;

            btnLoadField = new Button
            {
                Location = new Point(btnSaveField.Location.X + btnSaveField.Size.Width + 10, 2),
                Text = "Load Field",
                AutoSize = true,
                Name = name_btnLoadField,
                TabIndex = 5
            };

            btnLoadField.Click += Btn_Click;

            lbCount = new Label()
            {
                Location = new Point(btnLoadField.Location.X + btnLoadField.Size.Width + 10, 2),
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
            Controls.Add(btnPreviousStep);
            Controls.Add(btnSaveField);
            Controls.Add(btnLoadField);
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

            InitField();
        }

        private void InitField()
        {
            field = new Field(fieldX, fieldY, new CellArray(fieldX, fieldY));

            field.EnterCells();

            SetSizeFormAndField();

            field.FieldInit();

            field.Draw();
        }

        // Корректируем размер рабочей области формы кратно размерам поля
        private void SetSizeFormAndField()
        {
            int cellSize = Math.Min((int)Math.Ceiling((decimal)(ClientSize.Width - x0) / fieldX),
                               (int)Math.Ceiling((decimal)(ClientSize.Height - y0) / fieldY));

            Rectangle fieldRectangle = new Rectangle(x0, y0, cellSize * fieldX, cellSize * fieldY);

            ClientSize = new Size()
            {
                Width = x0 + fieldRectangle.Width,
                Height = y0 + fieldRectangle.Height
            };

            field.rectangle = fieldRectangle;

            field.CellSize = new Size(cellSize, cellSize);   // размер ячейки

            field.brushCellYes = Brushes.DarkGreen;
            field.brushCellNo = Brushes.LightGray;

            field.InitBitmap();
        }

    }
}
