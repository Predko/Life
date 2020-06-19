using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace life
{
    public partial class LifeForm : Form
    {
        private void InitContextMenuStrip()
        {
            panelField.ContextMenuStrip = contextMenuStripPanelField;

            saveBlockToolStripMenuItem.Click += SaveBlockToolStripMenuItem_Click;

            loadBlockToolStripMenuItem.Click += LoadBlockToolStripMenuItem_Click;

            copyToolStripMenuItem.Click += CopyToolStripMenuItem_Click;

            deleteToolStripMenuItem.Click += DeleteToolStripMenuItem_Click;

            PasteToolStripMenuItem.Click += PasteToolStripMenuItem_Click;
        }

        private void SaveBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedCells != null && selectedCells.Count() != 0)
            {
                if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }

                selectedCells.SaveToFile(saveFileDialog.FileName, Block.GameBlock);
            }
        }

        private void LoadBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Block block = LoadBlock();

            if (block == null)
            {
                return;
            }

            if (block.IsGameBlock)
            {

            }

        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
