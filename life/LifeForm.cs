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

        private Field field;            // игровое поле

        private int fieldX;        // Размер поля 
        private int fieldY;        // в ячейках

        private int cellSize = 25;      // размер клетки в пикселах

        private const int x0 = 0;
        private int y0;                 // координата Y начала игрового поля

        private Timer timer;            // 
        
        private TableLayoutPanel tlPanel;
        
        private FlowLayoutPanel flpGameCantrols;
        
        private Button btnStartStop;
        private Button btnMakeAStep;
        private Button btnPreviousStep;
        private Button btnSaveField;
        private Button btnLoadField;
        private ComboBox cellsComboBox;
        private ComboBox staticCellsComboBox;
        
        private Label lbCount;
        
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
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
                lbCount.Text = $"Step: {count,6:d}";
            }
        }

        public LifeForm():base()
        {
            InitializeComponent();

            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode = AutoScaleMode.Font;

            InitPanelfield();
            
            InitCellsComboBox(cellSize);

            InitButtons();

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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tlPanel = new System.Windows.Forms.TableLayoutPanel();
            this.flpGameCantrols = new System.Windows.Forms.FlowLayoutPanel();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.btnMakeAStep = new System.Windows.Forms.Button();
            this.btnPreviousStep = new System.Windows.Forms.Button();
            this.btnSaveField = new System.Windows.Forms.Button();
            this.btnLoadField = new System.Windows.Forms.Button();
            this.lbCount = new System.Windows.Forms.Label();
            this.cellsComboBox = new System.Windows.Forms.ComboBox();
            this.staticCellsComboBox = new System.Windows.Forms.ComboBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.tlPanel.SuspendLayout();
            this.flpGameCantrols.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlPanel
            // 
            this.tlPanel.AutoSize = true;
            this.tlPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlPanel.ColumnCount = 1;
            this.tlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlPanel.Controls.Add(this.flpGameCantrols, 0, 0);
            this.tlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tlPanel.Location = new System.Drawing.Point(0, 0);
            this.tlPanel.Name = "tlPanel";
            this.tlPanel.RowCount = 2;
            this.tlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlPanel.Size = new System.Drawing.Size(1618, 717);
            this.tlPanel.TabIndex = 1;
            // 
            // flpGameCantrols
            // 
            this.flpGameCantrols.AutoSize = true;
            this.flpGameCantrols.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpGameCantrols.Controls.Add(this.btnStartStop);
            this.flpGameCantrols.Controls.Add(this.btnMakeAStep);
            this.flpGameCantrols.Controls.Add(this.btnPreviousStep);
            this.flpGameCantrols.Controls.Add(this.btnSaveField);
            this.flpGameCantrols.Controls.Add(this.btnLoadField);
            this.flpGameCantrols.Controls.Add(this.lbCount);
            this.flpGameCantrols.Controls.Add(this.cellsComboBox);
            this.flpGameCantrols.Controls.Add(this.staticCellsComboBox);
            this.flpGameCantrols.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpGameCantrols.Location = new System.Drawing.Point(3, 3);
            this.flpGameCantrols.Name = "flpGameCantrols";
            this.flpGameCantrols.Size = new System.Drawing.Size(1612, 66);
            this.flpGameCantrols.TabIndex = 1;
            // 
            // btnStartStop
            // 
            this.btnStartStop.AutoSize = true;
            this.btnStartStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnStartStop.Location = new System.Drawing.Point(3, 3);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(111, 60);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            // 
            // btnMakeAStep
            // 
            this.btnMakeAStep.AutoSize = true;
            this.btnMakeAStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnMakeAStep.Location = new System.Drawing.Point(120, 3);
            this.btnMakeAStep.Name = "btnMakeAStep";
            this.btnMakeAStep.Size = new System.Drawing.Size(141, 60);
            this.btnMakeAStep.TabIndex = 1;
            this.btnMakeAStep.Text = "Make a step";
            this.btnMakeAStep.UseVisualStyleBackColor = true;
            // 
            // btnPreviousStep
            // 
            this.btnPreviousStep.AutoSize = true;
            this.btnPreviousStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnPreviousStep.Location = new System.Drawing.Point(267, 3);
            this.btnPreviousStep.Name = "btnPreviousStep";
            this.btnPreviousStep.Size = new System.Drawing.Size(160, 60);
            this.btnPreviousStep.TabIndex = 2;
            this.btnPreviousStep.Text = "Previous step";
            this.btnPreviousStep.UseVisualStyleBackColor = true;
            // 
            // btnSaveField
            // 
            this.btnSaveField.AutoSize = true;
            this.btnSaveField.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSaveField.Location = new System.Drawing.Point(433, 3);
            this.btnSaveField.Name = "btnSaveField";
            this.btnSaveField.Size = new System.Drawing.Size(128, 60);
            this.btnSaveField.TabIndex = 3;
            this.btnSaveField.Text = "Save Field";
            this.btnSaveField.UseVisualStyleBackColor = true;
            // 
            // btnLoadField
            // 
            this.btnLoadField.AutoSize = true;
            this.btnLoadField.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnLoadField.Location = new System.Drawing.Point(567, 3);
            this.btnLoadField.Name = "btnLoadField";
            this.btnLoadField.Size = new System.Drawing.Size(122, 60);
            this.btnLoadField.TabIndex = 4;
            this.btnLoadField.Text = "Load Field";
            this.btnLoadField.UseVisualStyleBackColor = true;
            // 
            // lbCount
            // 
            this.lbCount.AutoSize = true;
            this.lbCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbCount.Location = new System.Drawing.Point(695, 3);
            this.lbCount.Margin = new System.Windows.Forms.Padding(3);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(119, 60);
            this.lbCount.TabIndex = 7;
            this.lbCount.Text = "Step: 00000";
            this.lbCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cellsComboBox
            // 
            this.cellsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cellsComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.cellsComboBox.FormattingEnabled = true;
            this.cellsComboBox.ItemHeight = 29;
            this.cellsComboBox.Location = new System.Drawing.Point(820, 14);
            this.cellsComboBox.Name = "cellsComboBox";
            this.cellsComboBox.Size = new System.Drawing.Size(150, 37);
            this.cellsComboBox.TabIndex = 5;
            // 
            // staticCellsComboBox
            // 
            this.staticCellsComboBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.staticCellsComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.staticCellsComboBox.FormattingEnabled = true;
            this.staticCellsComboBox.ItemHeight = 29;
            this.staticCellsComboBox.Location = new System.Drawing.Point(976, 14);
            this.staticCellsComboBox.Name = "staticCellsComboBox";
            this.staticCellsComboBox.Size = new System.Drawing.Size(150, 37);
            this.staticCellsComboBox.TabIndex = 6;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "\"*.life|*.life|*.save|*.save\"";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "\"*.life|*.life|*.save|*.save\"";
            // 
            // LifeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1618, 717);
            this.Controls.Add(this.tlPanel);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.Name = "LifeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Life";
            this.tlPanel.ResumeLayout(false);
            this.tlPanel.PerformLayout();
            this.flpGameCantrols.ResumeLayout(false);
            this.flpGameCantrols.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void InitialazeLifeForm()
        {
            Count = 0;

            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();

            saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();

            flpGameCantrols.DoubleBuffered(true);

            timer = new Timer
            {
                Interval = 200
            };

            timer.Tick += Timer_Tick;

            KeyUp += LifeForm_KeyUp;

            InitField();
        }

        /// <summary>
        /// Инициализация игрового поля
        /// </summary>
        private void InitField()
        {
            SetMaxFormSize();
            
            fieldY = (int)Math.Floor((decimal)(panelField.ClientSize.Height - y0) / cellSize);

            fieldX = (int)Math.Floor((decimal)(panelField.ClientSize.Width - x0) / cellSize);

            field = new Field(fieldX, fieldY, new CellArray(fieldX, fieldY), (Bitmap)cellsComboBox.SelectedItem, (Bitmap)staticCellsComboBox.SelectedItem);

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

            Size oldSize = panelField.ClientSize;

            panelField.ClientSize = new Size()
            {
                Width = x0 + fieldRectangle.Width,
                Height = y0 + fieldRectangle.Height
            };

            ClientSize = new Size()
            {
                Width = ClientSize.Width + panelField.ClientSize.Width - oldSize.Width,
                Height = ClientSize.Height + panelField.ClientSize.Height - oldSize.Height
            };

            StartPosition = FormStartPosition.CenterScreen;

            field.Bounds = fieldRectangle;

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
            
            dx = (int)Math.Floor((decimal)(panelField.ClientSize.Width - x0) / fieldX);

            dy = (int)Math.Floor((decimal)(panelField.ClientSize.Height - y0) / fieldY);

            cellSize = (dx >  dy) ? dy : dx;

            field.DisposeBitmaps((Bitmap)cellsComboBox.SelectedItem, (Bitmap)staticCellsComboBox.SelectedItem);

            SetSizeFormAndField();
           
            field.Draw();

            btnPreviousStep.Enabled = false;

            Count = 0;
        }
    }
}
