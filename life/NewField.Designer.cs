namespace life
{
    partial class NewField
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblSizeField = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblWidth = new System.Windows.Forms.Label();
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.lblHeight = new System.Windows.Forms.Label();
            this.nudHeight = new System.Windows.Forms.NumericUpDown();
            this.cbStaticCells = new System.Windows.Forms.ComboBox();
            this.lblColorCell = new System.Windows.Forms.Label();
            this.lblColorStaticCell = new System.Windows.Forms.Label();
            this.cbCells = new System.Windows.Forms.ComboBox();
            this.btnSettingOk = new System.Windows.Forms.Button();
            this.btnSettingCancel = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cbBorder = new System.Windows.Forms.CheckBox();
            this.labelDensity = new System.Windows.Forms.Label();
            this.hsbDensity = new System.Windows.Forms.HScrollBar();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.lblSizeField, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbStaticCells, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblColorCell, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblColorStaticCell, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbCells, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnSettingOk, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnSettingCancel, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1062, 117);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblSizeField
            // 
            this.lblSizeField.AutoSize = true;
            this.lblSizeField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSizeField.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblSizeField.Location = new System.Drawing.Point(4, 0);
            this.lblSizeField.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSizeField.Name = "lblSizeField";
            this.lblSizeField.Size = new System.Drawing.Size(431, 25);
            this.lblSizeField.TabIndex = 0;
            this.lblSizeField.Text = "Размер игрового поля";
            this.lblSizeField.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.lblWidth);
            this.flowLayoutPanel1.Controls.Add(this.nudWidth);
            this.flowLayoutPanel1.Controls.Add(this.lblHeight);
            this.flowLayoutPanel1.Controls.Add(this.nudHeight);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(4, 29);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(431, 36);
            this.flowLayoutPanel1.TabIndex = 1;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // lblWidth
            // 
            this.lblWidth.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblWidth.AutoSize = true;
            this.lblWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblWidth.Location = new System.Drawing.Point(4, 5);
            this.lblWidth.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(82, 25);
            this.lblWidth.TabIndex = 0;
            this.lblWidth.Text = "Ширина";
            // 
            // nudWidth
            // 
            this.nudWidth.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudWidth.Location = new System.Drawing.Point(93, 3);
            this.nudWidth.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudWidth.Name = "nudWidth";
            this.nudWidth.Size = new System.Drawing.Size(120, 30);
            this.nudWidth.TabIndex = 3;
            // 
            // lblHeight
            // 
            this.lblHeight.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblHeight.AutoSize = true;
            this.lblHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblHeight.Location = new System.Drawing.Point(220, 5);
            this.lblHeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(81, 25);
            this.lblHeight.TabIndex = 2;
            this.lblHeight.Text = "Высота";
            // 
            // nudHeight
            // 
            this.nudHeight.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudHeight.Location = new System.Drawing.Point(308, 3);
            this.nudHeight.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudHeight.Name = "nudHeight";
            this.nudHeight.Size = new System.Drawing.Size(120, 30);
            this.nudHeight.TabIndex = 4;
            // 
            // cbStaticCells
            // 
            this.cbStaticCells.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.cbStaticCells.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.cbStaticCells.FormattingEnabled = true;
            this.cbStaticCells.Location = new System.Drawing.Point(775, 29);
            this.cbStaticCells.Margin = new System.Windows.Forms.Padding(4);
            this.cbStaticCells.Name = "cbStaticCells";
            this.cbStaticCells.Size = new System.Drawing.Size(87, 37);
            this.cbStaticCells.TabIndex = 5;
            // 
            // lblColorCell
            // 
            this.lblColorCell.AutoSize = true;
            this.lblColorCell.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblColorCell.Location = new System.Drawing.Point(442, 0);
            this.lblColorCell.Name = "lblColorCell";
            this.lblColorCell.Size = new System.Drawing.Size(130, 25);
            this.lblColorCell.TabIndex = 6;
            this.lblColorCell.Text = "Цвет клетки";
            this.lblColorCell.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblColorStaticCell
            // 
            this.lblColorStaticCell.AutoSize = true;
            this.lblColorStaticCell.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblColorStaticCell.Location = new System.Drawing.Point(578, 0);
            this.lblColorStaticCell.Name = "lblColorStaticCell";
            this.lblColorStaticCell.Size = new System.Drawing.Size(481, 25);
            this.lblColorStaticCell.TabIndex = 7;
            this.lblColorStaticCell.Text = "Цвет статичных клеток";
            this.lblColorStaticCell.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbCells
            // 
            this.cbCells.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.cbCells.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.cbCells.FormattingEnabled = true;
            this.cbCells.Location = new System.Drawing.Point(460, 29);
            this.cbCells.Margin = new System.Windows.Forms.Padding(4);
            this.cbCells.Name = "cbCells";
            this.cbCells.Size = new System.Drawing.Size(94, 37);
            this.cbCells.TabIndex = 2;
            // 
            // btnSettingOk
            // 
            this.btnSettingOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.btnSettingOk.AutoSize = true;
            this.btnSettingOk.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSettingOk.Location = new System.Drawing.Point(456, 73);
            this.btnSettingOk.Name = "btnSettingOk";
            this.btnSettingOk.Size = new System.Drawing.Size(102, 41);
            this.btnSettingOk.TabIndex = 8;
            this.btnSettingOk.Text = "Принять";
            this.btnSettingOk.UseVisualStyleBackColor = true;
            this.btnSettingOk.Click += new System.EventHandler(this.BtnSettingOk_Click);
            // 
            // btnSettingCancel
            // 
            this.btnSettingCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.btnSettingCancel.AutoSize = true;
            this.btnSettingCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSettingCancel.Location = new System.Drawing.Point(769, 73);
            this.btnSettingCancel.Name = "btnSettingCancel";
            this.btnSettingCancel.Size = new System.Drawing.Size(98, 41);
            this.btnSettingCancel.TabIndex = 9;
            this.btnSettingCancel.Text = "Отмена";
            this.btnSettingCancel.UseVisualStyleBackColor = true;
            this.btnSettingCancel.Click += new System.EventHandler(this.BtnSettingCancel_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 70);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cbBorder);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.labelDensity);
            this.splitContainer1.Panel2.Controls.Add(this.hsbDensity);
            this.splitContainer1.Size = new System.Drawing.Size(439, 47);
            this.splitContainer1.SplitterDistance = 207;
            this.splitContainer1.TabIndex = 10;
            // 
            // cbBorder
            // 
            this.cbBorder.AutoSize = true;
            this.cbBorder.Checked = true;
            this.cbBorder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBorder.Location = new System.Drawing.Point(22, 8);
            this.cbBorder.Name = "cbBorder";
            this.cbBorder.Size = new System.Drawing.Size(165, 29);
            this.cbBorder.TabIndex = 0;
            this.cbBorder.Text = "Граница поля";
            this.cbBorder.UseVisualStyleBackColor = true;
            // 
            // labelDensity
            // 
            this.labelDensity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.labelDensity.AutoSize = true;
            this.labelDensity.Location = new System.Drawing.Point(0, 9);
            this.labelDensity.Margin = new System.Windows.Forms.Padding(0);
            this.labelDensity.Name = "labelDensity";
            this.labelDensity.Size = new System.Drawing.Size(68, 25);
            this.labelDensity.TabIndex = 1;
            this.labelDensity.Text = "100 %";
            this.labelDensity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hsbDensity
            // 
            this.hsbDensity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hsbDensity.Location = new System.Drawing.Point(68, 8);
            this.hsbDensity.Maximum = 109;
            this.hsbDensity.Name = "hsbDensity";
            this.hsbDensity.Size = new System.Drawing.Size(155, 26);
            this.hsbDensity.TabIndex = 0;
            this.hsbDensity.Value = 30;
            this.hsbDensity.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HsbDensity_Scroll);
            // 
            // SettingField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1062, 117);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingField";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка игрового поля";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblSizeField;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.ComboBox cbStaticCells;
        private System.Windows.Forms.Label lblColorCell;
        private System.Windows.Forms.Label lblColorStaticCell;
        private System.Windows.Forms.ComboBox cbCells;
        private System.Windows.Forms.NumericUpDown nudWidth;
        private System.Windows.Forms.NumericUpDown nudHeight;
        private System.Windows.Forms.Button btnSettingOk;
        private System.Windows.Forms.Button btnSettingCancel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox cbBorder;
        private System.Windows.Forms.Label labelDensity;
        private System.Windows.Forms.HScrollBar hsbDensity;
    }
}