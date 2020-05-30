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
        private Button btnNewGame;
        private HScrollBar hsbTimer;
        private TableLayoutPanel tableLayoutPanel1;
        private Label lblSpeedGame;
        private FlowLayoutPanel flpSettingField;

        public LifeForm() : base()
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

                BitmapCells.Dispose();

                bitmap.Dispose();

                bitmapGraphics.Dispose();
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
            this.flpSettingField = new System.Windows.Forms.FlowLayoutPanel();
            this.staticCellsComboBox = new System.Windows.Forms.ComboBox();
            this.cellsComboBox = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblSpeedGame = new System.Windows.Forms.Label();
            this.hsbTimer = new System.Windows.Forms.HScrollBar();
            this.flpGameCantrols = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNewGame = new System.Windows.Forms.Button();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.btnMakeAStep = new System.Windows.Forms.Button();
            this.btnPreviousStep = new System.Windows.Forms.Button();
            this.btnSaveField = new System.Windows.Forms.Button();
            this.btnLoadField = new System.Windows.Forms.Button();
            this.lbCount = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.tlPanel.SuspendLayout();
            this.flpSettingField.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flpGameCantrols.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlPanel
            // 
            this.tlPanel.AutoSize = true;
            this.tlPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlPanel.ColumnCount = 2;
            this.tlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlPanel.Controls.Add(this.flpSettingField, 1, 0);
            this.tlPanel.Controls.Add(this.flpGameCantrols, 0, 0);
            this.tlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tlPanel.Location = new System.Drawing.Point(0, 0);
            this.tlPanel.Margin = new System.Windows.Forms.Padding(0);
            this.tlPanel.Name = "tlPanel";
            this.tlPanel.RowCount = 2;
            this.tlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlPanel.Size = new System.Drawing.Size(1534, 717);
            this.tlPanel.TabIndex = 1;
            // 
            // flpSettingField
            // 
            this.flpSettingField.AutoSize = true;
            this.flpSettingField.Controls.Add(this.staticCellsComboBox);
            this.flpSettingField.Controls.Add(this.cellsComboBox);
            this.flpSettingField.Controls.Add(this.tableLayoutPanel1);
            this.flpSettingField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpSettingField.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpSettingField.Location = new System.Drawing.Point(1033, 0);
            this.flpSettingField.Margin = new System.Windows.Forms.Padding(0);
            this.flpSettingField.Name = "flpSettingField";
            this.flpSettingField.Size = new System.Drawing.Size(501, 66);
            this.flpSettingField.TabIndex = 2;
            this.flpSettingField.WrapContents = false;
            // 
            // staticCellsComboBox
            // 
            this.staticCellsComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.staticCellsComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.staticCellsComboBox.FormattingEnabled = true;
            this.staticCellsComboBox.ItemHeight = 29;
            this.staticCellsComboBox.Location = new System.Drawing.Point(348, 3);
            this.staticCellsComboBox.Name = "staticCellsComboBox";
            this.staticCellsComboBox.Size = new System.Drawing.Size(150, 37);
            this.staticCellsComboBox.TabIndex = 6;
            // 
            // cellsComboBox
            // 
            this.cellsComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cellsComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.cellsComboBox.FormattingEnabled = true;
            this.cellsComboBox.ItemHeight = 29;
            this.cellsComboBox.Location = new System.Drawing.Point(192, 3);
            this.cellsComboBox.Name = "cellsComboBox";
            this.cellsComboBox.Size = new System.Drawing.Size(150, 37);
            this.cellsComboBox.TabIndex = 5;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.lblSpeedGame, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.hsbTimer, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(183, 50);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // lblSpeedGame
            // 
            this.lblSpeedGame.AutoSize = true;
            this.lblSpeedGame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSpeedGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblSpeedGame.Location = new System.Drawing.Point(3, 0);
            this.lblSpeedGame.Name = "lblSpeedGame";
            this.lblSpeedGame.Size = new System.Drawing.Size(177, 25);
            this.lblSpeedGame.TabIndex = 0;
            this.lblSpeedGame.Text = "Скорость";
            this.lblSpeedGame.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hsbTimer
            // 
            this.hsbTimer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hsbTimer.LargeChange = 50;
            this.hsbTimer.Location = new System.Drawing.Point(0, 25);
            this.hsbTimer.Maximum = 549;
            this.hsbTimer.Minimum = 10;
            this.hsbTimer.Name = "hsbTimer";
            this.hsbTimer.Size = new System.Drawing.Size(183, 25);
            this.hsbTimer.SmallChange = 10;
            this.hsbTimer.TabIndex = 9;
            this.hsbTimer.Value = 110;
            this.hsbTimer.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HsbTimer_Scroll);
            // 
            // flpGameCantrols
            // 
            this.flpGameCantrols.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.flpGameCantrols.AutoSize = true;
            this.flpGameCantrols.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpGameCantrols.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.flpGameCantrols.Controls.Add(this.btnNewGame);
            this.flpGameCantrols.Controls.Add(this.btnStartStop);
            this.flpGameCantrols.Controls.Add(this.btnMakeAStep);
            this.flpGameCantrols.Controls.Add(this.btnPreviousStep);
            this.flpGameCantrols.Controls.Add(this.btnSaveField);
            this.flpGameCantrols.Controls.Add(this.btnLoadField);
            this.flpGameCantrols.Controls.Add(this.lbCount);
            this.flpGameCantrols.Location = new System.Drawing.Point(0, 0);
            this.flpGameCantrols.Margin = new System.Windows.Forms.Padding(0);
            this.flpGameCantrols.Name = "flpGameCantrols";
            this.flpGameCantrols.Size = new System.Drawing.Size(1033, 66);
            this.flpGameCantrols.TabIndex = 1;
            this.flpGameCantrols.WrapContents = false;
            // 
            // btnNewGame
            // 
            this.btnNewGame.AutoSize = true;
            this.btnNewGame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNewGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnNewGame.Location = new System.Drawing.Point(3, 3);
            this.btnNewGame.Name = "btnNewGame";
            this.btnNewGame.Size = new System.Drawing.Size(61, 60);
            this.btnNewGame.TabIndex = 8;
            this.btnNewGame.Text = "New";
            this.btnNewGame.UseVisualStyleBackColor = true;
            // 
            // btnStartStop
            // 
            this.btnStartStop.AutoSize = true;
            this.btnStartStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStartStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnStartStop.Location = new System.Drawing.Point(70, 3);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(111, 60);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            // 
            // btnMakeAStep
            // 
            this.btnMakeAStep.AutoSize = true;
            this.btnMakeAStep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMakeAStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnMakeAStep.Location = new System.Drawing.Point(187, 3);
            this.btnMakeAStep.Name = "btnMakeAStep";
            this.btnMakeAStep.Size = new System.Drawing.Size(141, 60);
            this.btnMakeAStep.TabIndex = 1;
            this.btnMakeAStep.Text = "Make a step";
            this.btnMakeAStep.UseVisualStyleBackColor = true;
            this.btnMakeAStep.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnMakeAStep_MouseDown);
            this.btnMakeAStep.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BtnMakeAStep_MouseUp);
            // 
            // btnPreviousStep
            // 
            this.btnPreviousStep.AutoSize = true;
            this.btnPreviousStep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPreviousStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnPreviousStep.Location = new System.Drawing.Point(334, 3);
            this.btnPreviousStep.Name = "btnPreviousStep";
            this.btnPreviousStep.Size = new System.Drawing.Size(160, 60);
            this.btnPreviousStep.TabIndex = 2;
            this.btnPreviousStep.Text = "Previous step";
            this.btnPreviousStep.UseVisualStyleBackColor = true;
            this.btnPreviousStep.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnPreviousStep_MouseDown);
            this.btnPreviousStep.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BtnPreviousStep_MouseUp);
            // 
            // btnSaveField
            // 
            this.btnSaveField.AutoSize = true;
            this.btnSaveField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSaveField.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSaveField.Location = new System.Drawing.Point(500, 3);
            this.btnSaveField.Name = "btnSaveField";
            this.btnSaveField.Size = new System.Drawing.Size(128, 60);
            this.btnSaveField.TabIndex = 3;
            this.btnSaveField.Text = "Save Field";
            this.btnSaveField.UseVisualStyleBackColor = true;
            // 
            // btnLoadField
            // 
            this.btnLoadField.AutoSize = true;
            this.btnLoadField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLoadField.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnLoadField.Location = new System.Drawing.Point(634, 3);
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
            this.lbCount.Location = new System.Drawing.Point(762, 3);
            this.lbCount.Margin = new System.Windows.Forms.Padding(3);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(119, 60);
            this.lbCount.TabIndex = 7;
            this.lbCount.Text = "Step: 00000";
            this.lbCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "*.life|*.life|*.save|*.save";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "*.life|*.life|*.save|*.save";
            // 
            // LifeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1534, 717);
            this.Controls.Add(this.tlPanel);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.Name = "LifeForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Life";
            this.tlPanel.ResumeLayout(false);
            this.tlPanel.PerformLayout();
            this.flpSettingField.ResumeLayout(false);
            this.flpSettingField.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flpGameCantrols.ResumeLayout(false);
            this.flpGameCantrols.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
