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
    /// <summary>
    /// Форма для настройки параметров игрового поляю
    /// </summary>
    public partial class SettingField : Form
    {
        /// <summary>
        /// Список обычных клеток.
        /// </summary>
        private readonly ComboBox Cells;

        /// <summary>
        /// Список статичных клеток.
        /// </summary>
        private readonly ComboBox StaticCells;

        /// <summary>
        /// Индекс обычной клетки в ComboBox при загрузке данной формы.
        /// </summary>
        private int indexCell;
        /// <summary>
        /// Индекс статичной клетки в ComboBox при загрузке данной формы.
        /// </summary>
        private int indexStaticCell;

        /// <summary>
        /// Настраиваемое игровое поле.
        /// </summary>
        private readonly Field field;

        /// <summary>
        /// Указывает, требуется ли граница поля.
        /// </summary>
        public bool IsBorder
        {
            get => cbBorder.Checked;
        }

        /// <summary>
        /// Плотность заполнения клетками игрового поля.
        /// </summary>
        public float Density => (float)hsbDensity.Value / 100;

        /// <summary>
        /// Размер игрового поля.
        /// </summary>
        public Size SizeField
        {
            get => new Size((int)nudWidth.Value, (int)nudHeight.Value);
            set
            {
                nudWidth.Value = value.Width;
                nudHeight.Value = value.Height;
            }
        }

        /// <summary>
        /// Конструктор формы настройки игрового поля.
        /// </summary>
        /// <param name="f">Игровое поле.</param>
        /// <param name="cbNormal">Ссылка на ComboBox, содержащий обычные ячейки.</param>
        /// <param name="cbStatic">Ссылка на ComboBox, содержащий статичные ячейки.</param>
        /// <param name="cellsSelectedIndexChanged">Обработчик изменения индекса обычных клеток в ComboBox.</param>
        /// <param name="StaticCellsSelectedIndexChanged">Обработчик изменения индекса статичных клеток в ComboBox.</param>
        /// <param name="CellsComboBox_DrawItem">Обработчик перерисовки пункта ComboBox.</param>
        public SettingField(Field f, ComboBox cbNormal, ComboBox cbStatic, 
                            EventHandler cellsSelectedIndexChanged, 
                            EventHandler StaticCellsSelectedIndexChanged, 
                            DrawItemEventHandler CellsComboBox_DrawItem)
        {
            InitializeComponent();

            field = f;
            
            foreach(var item in cbNormal.Items)
            {
                cbCells.Items.Add(item);
            }

            Cells = cbNormal;
            StaticCells = cbStatic;
            
            cbCells.DropDownStyle = cbNormal.DropDownStyle;
            cbCells.DropDownHeight = cbNormal.DropDownHeight;
            cbCells.DrawMode = cbNormal.DrawMode;

            foreach (var item in cbStatic.Items)
            {
                cbStaticCells.Items.Add(item);
            }

            cbStaticCells.DropDownStyle = cbStatic.DropDownStyle;
            cbStaticCells.DropDownHeight = cbStatic.DropDownHeight;
            cbStaticCells.DrawMode = cbStatic.DrawMode;

            cbCells.SelectedIndexChanged += cellsSelectedIndexChanged;

            cbCells.DrawItem += CellsComboBox_DrawItem;

            cbStaticCells.SelectedIndexChanged += StaticCellsSelectedIndexChanged;

            cbStaticCells.DrawItem += CellsComboBox_DrawItem;

            Load += SettingField_Load;
        }

        /// <summary>
        /// Настройка формы при начальной загрузке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingField_Load(object sender, EventArgs e)
        {
            indexCell = Cells.SelectedIndex;
            indexStaticCell = StaticCells.SelectedIndex;

            cbCells.SelectedIndex = indexCell;
            cbStaticCells.SelectedIndex = indexStaticCell;

            nudWidth.Value = field.width;
            nudHeight.Value = field.height;

            cbBorder.Checked = field.isBorder;
            hsbDensity.Value = (int)(field.density * 100);
            labelDensity.Text = $"{hsbDensity.Value}";
        }

        private void BtnSettingOk_Click(object sender, EventArgs e)
        {
            Cells.SelectedIndex = cbCells.SelectedIndex;
            StaticCells.SelectedIndex = cbStaticCells.SelectedIndex;

            DialogResult = DialogResult.OK;

            Close();
        }

        private void BtnSettingCancel_Click(object sender, EventArgs e)
        {
            cbCells.SelectedIndex = indexCell;
            cbStaticCells.SelectedIndex = indexStaticCell;

            Close();
        }

        private void HsbDensity_Scroll(object sender, ScrollEventArgs e)
        {
            labelDensity.Text = $"{hsbDensity.Value}";
        }
    }
}
