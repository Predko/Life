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
        private CellsComboBox cellsComboBox;

        private CellsComboBox staticCellsComboBox;

        public void InitCellsComboBox(int cellSize)
        {
            ResourceManager temp = new ResourceManager("life.Properties.Resources", typeof(Resources).Assembly);

            ResourceSet rs = temp.GetResourceSet(CultureInfo.CurrentCulture, true, true);

            Size szbm = new Size(cellSize * 2, cellSize);

            cellsComboBox = new CellsComboBox(new Rectangle(0, 0, szbm.Width, szbm.Height))
            {
                Location = new Point(lbCount.Location.X + lbCount.Size.Width + 10, 2),
                Name = "CellsComboBox",
                ItemHeight = szbm.Height + 4,
                Width = szbm.Width + 27,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DropDownHeight = (szbm.Height + 4) * 10,
                DrawMode = DrawMode.OwnerDrawFixed
            };

            staticCellsComboBox = new CellsComboBox(new Rectangle(0, 0, szbm.Width, szbm.Height))
            {
                Location = new Point(cellsComboBox.Location.X + cellsComboBox.Size.Width + 10, 2),
                Name = "StaticCellsComboBox",
                ItemHeight = szbm.Height + 4,
                Width = szbm.Width + 27,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DropDownHeight = (szbm.Height + 4) * 10,
                DrawMode = DrawMode.OwnerDrawFixed
            };

            foreach (DictionaryEntry r in rs)
            {
                if (r.Value.GetType() == typeof(Bitmap))
                {
                    var bmp = new Bitmap((Bitmap)r.Value, szbm);

                    bmp.MakeTransparent(Color.Transparent);

                    cellsComboBox.Items.Add(bmp);

                    staticCellsComboBox.Items.Add(bmp);
                }
            }

            if (cellsComboBox.Items.Count != 0)
            {
                Controls.Add(cellsComboBox);

                cellsComboBox.SelectedIndex = 5;
                
                Controls.Add(staticCellsComboBox);

                staticCellsComboBox.SelectedIndex = 2;
            }

            cellsComboBox.SelectedIndexChanged += CellsComboBox_SelectedIndexChanged;

            staticCellsComboBox.SelectedIndexChanged += StaticCellsComboBox_SelectedIndexChanged;
        }

        private void StaticCellsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            field.BitmapStaticCellChanged((Bitmap)comboBox.SelectedItem);

            field.DrawAll();

            field.Draw();

            Invalidate();
        }

        private void CellsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            field.BitmapCellChanged((Bitmap)comboBox.SelectedItem);

            field.DrawAll();

            field.Draw();

            Invalidate();
        }
    }


    public class CellsComboBox : ComboBox
    {
        private Rectangle rectBitmap;
        public Bitmap arrows { get; set; }

        private int focusIndex = 0;
        private ComboBox cb;

        public CellsComboBox(Rectangle rbm) : base()
        {
            rectBitmap = rbm;
            cb = this;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index != -1)
            {
                e.Graphics.DrawImage((Bitmap)Items[e.Index], e.Bounds.Left + 2, e.Bounds.Top + 2, rectBitmap, GraphicsUnit.Pixel);

            //    if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
            //    {
            //        e.Graphics.DrawImage(arrows, e.Bounds.Left + 2, e.Bounds.Top + 2, rectBitmap, GraphicsUnit.Pixel);
            //        //cb.Invalidate();
            //    }
            }

            e.DrawFocusRectangle();

            base.OnDrawItem(e);
        }
    }
}
