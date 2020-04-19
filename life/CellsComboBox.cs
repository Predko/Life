using life.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace life
{
    public partial class LifeForm : Form
    {
        Rectangle rectBitmap;

        public void InitCellsComboBox(int cellSize)
        {
            ResourceManager temp = new ResourceManager("life.Properties.Resources", typeof(Resources).Assembly);

            ResourceSet rs = temp.GetResourceSet(CultureInfo.CurrentCulture, true, true);

            rectBitmap = new Rectangle(0, 0, cellSize * 2, cellSize);

            cellsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            cellsComboBox.DropDownHeight = (rectBitmap.Height + 4) * 10;
            cellsComboBox.DrawMode = DrawMode.OwnerDrawFixed;

            staticCellsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            staticCellsComboBox.DropDownHeight = (rectBitmap.Height + 4) * 10;
            staticCellsComboBox.DrawMode = DrawMode.OwnerDrawFixed;

            foreach (DictionaryEntry r in rs)
            {
                if (r.Value.GetType() == typeof(Bitmap))
                {
                    var bmp = new Bitmap((Bitmap)r.Value, rectBitmap.Size);

                    bmp.MakeTransparent(Color.Transparent);

                    cellsComboBox.Items.Add(bmp);

                    staticCellsComboBox.Items.Add(bmp);
                }
            }

            cellsComboBox.SelectedIndex = 5;
                
            staticCellsComboBox.SelectedIndex = 2;

            cellsComboBox.SelectedIndexChanged += CellsComboBox_SelectedIndexChanged;

            cellsComboBox.DrawItem += CellsComboBox_DrawItem;
            
            staticCellsComboBox.SelectedIndexChanged += StaticCellsComboBox_SelectedIndexChanged;

            staticCellsComboBox.DrawItem += CellsComboBox_DrawItem;
        }

        private void CellsComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index != -1)
            {
                e.Graphics.DrawImage((Bitmap)((ComboBox)sender).Items[e.Index], e.Bounds.Left + 2, e.Bounds.Top + 2, rectBitmap, GraphicsUnit.Pixel);
            }

            e.DrawFocusRectangle();
        }

        private void StaticCellsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            field.BitmapStaticCellChanged((Bitmap)comboBox.SelectedItem);

            field.DrawAll();

            field.Draw();

            panelField.Invalidate();
        }

        private void CellsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            field.BitmapCellChanged((Bitmap)comboBox.SelectedItem);

            field.DrawAll();

            field.Draw();

            panelField.Invalidate();
        }
    }
}
