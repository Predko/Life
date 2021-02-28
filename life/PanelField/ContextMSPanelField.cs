using System;
using System.Collections.Generic;
using System.Drawing;
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
                selectedCells = block;

                isSelected = true;

                isMoveMode = true;

                // Размещаем блок за пределами игрового поля + cellSize.
                startSelection = endSelection = new Point(panelField.Right + cellSize, panelField.Top + cellSize);

                endSelection.Offset(block.Width * cellSize, block.Height * cellSize);

                StartMoveBlock(startSelection);

                // Перемещаем в позицию мыши
                
                //To Do 
                //1. Вычислить координаты расположения блока, чтобы он не выходил за пределы игрового поля.
                //2. Если блок не помещается в пределах игрового поля:
                //   - отказать во вставке,
                //   - или обрезать блок, после запроса пользователю,
                //   - или изменить размеры игрового поля, после запроса пользователю.

                DrawSelectedBlock(panelField.Location);
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
