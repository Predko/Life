using life.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Windows.Forms;
using System.Collections;

namespace life
{
    public partial class LifeForm : Form
    {
        private readonly IContainer components = null;

        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;

        private Field field;            // игровое поле

        private int fieldX;        // Размер поля 
        private int fieldY;        // в ячейках

        private int cellSize = 25;      // размер клетки в пикселах

        private const int x0 = 0;
        private int y0;                 // координата Y начала игрового поля

        private Timer timer;            // 

        private int count;      // счётчик ходов

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

            openFileDialog = new OpenFileDialog()
            {
                Filter = "*.life|*.life|*.save|*.save",
                InitialDirectory = Directory.GetCurrentDirectory()
            };

            saveFileDialog = new SaveFileDialog()
            {
                Filter = "*.life|*.life|*.save|*.save",
                OverwritePrompt = true,
                InitialDirectory = Directory.GetCurrentDirectory()
            };

            InitButtons();

            InitLabels();

            InitCellsComboBox(25);

            //cellsComboBox.Size = new Size(szbm.Width + 10, szbm.Height);

            y0 = 2 + cellsComboBox.ItemHeight + 10;

            Text = "Life";
            BackColor = SystemColors.Window;
            ResizeRedraw = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;

            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode = AutoScaleMode.Font;

            ResumeLayout(false);

            timer = new Timer
            {
                Interval = 200
            };

            timer.Tick += Timer_Tick;

            KeyUp += LifeForm_KeyUp;
        }

        private void InitialazeLifeForm()
        {
            Count = 0;

            InitField();
        }

        /// <summary>
        /// Инициализация игрового поля
        /// </summary>
        private void InitField()
        {
            SetMaxFormSize();
            
            fieldY = (int)Math.Floor((decimal)(ClientSize.Height - y0) / cellSize);

            fieldX = (int)Math.Floor((decimal)(ClientSize.Width - x0) / cellSize);

            field = new Field(fieldX, fieldY, new CellArray(fieldX, fieldY), (Bitmap)cellsComboBox.SelectedItem, (Bitmap)staticCellsComboBox.SelectedItem);
            //Resources.Cell2_08_6, Resources.Cell2_08_1);

            field.EnterCells();

            SetSizeFormAndField();

            field.FieldInit();

            field.Draw();
        }

        /// <summary>
        /// Устанавливает максимальные размеры формы
        /// </summary>
        private void SetMaxFormSize()
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

            Size = new Size(workingArea.Width - 10, workingArea.Height - 10);
        }

        /// <summary>
        /// Корректируем размер рабочей области формы кратно размерам поля
        /// </summary>
        private void SetSizeFormAndField()
        {
            Rectangle fieldRectangle = new Rectangle(x0, y0, cellSize * fieldX, cellSize * fieldY);

            ClientSize = new Size()
            {
                Width = x0 + fieldRectangle.Width,
                Height = y0 + fieldRectangle.Height
            };

            StartPosition = FormStartPosition.CenterScreen;

            field.rectangle = fieldRectangle;

            field.CellSize = cellSize;   // размер ячейки

            field.InitBitmap();
        }

        /// <summary>
        /// Загрузка игрового поля с изменением размера ячейки
        /// </summary>
        private void LoadField()
        {
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            var (dx, dy) = field.Load(openFileDialog.FileName);

            fieldX = dx;
            fieldY = dy;

            SetMaxFormSize();
            
            dx = (int)Math.Floor((decimal)(ClientSize.Width - x0) / fieldX);

            dy = (int)Math.Floor((decimal)(ClientSize.Height - y0) / fieldY);

            cellSize = (dx >  dy) ? dy : dx;

            field.DisposeBitmaps((Bitmap)cellsComboBox.SelectedItem, (Bitmap)staticCellsComboBox.SelectedItem);

            SetSizeFormAndField();
           
            field.Draw();

            btnPreviousStep.Enabled = false;

            Count = 0;
        }
    }
}
