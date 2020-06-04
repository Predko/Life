using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace life
{
    public enum SideNode { Left, Right, Parent }

    public class BinaryTreeCellsNode : IComparable<Cell>, IEquatable<BinaryTreeCellsNode>
    {
        public BinaryTreeCellsNode left;
        public BinaryTreeCellsNode right;
        public BinaryTreeCellsNode parent;

        public readonly Cell cell;

        public BinaryTreeCellsNode(Cell cell)
        {
            this.cell = cell;
        }

        public SideNode Side() => (this == (object)parent.left) ? SideNode.Left :
                                  (this == (object)parent.right) ? SideNode.Right :
                                                           SideNode.Parent;

        public void SetParentLeftOrRight(BinaryTreeCellsNode node)
        {
            switch (Side())
            {
                case SideNode.Left:
                    parent.left = node;
                    break;

                case SideNode.Right:
                    parent.right = node;
                    break;
            }
        }

        public int CompareTo(Cell other) => cell.CompareTo(other);

        public int CompareTo(BinaryTreeCellsNode other) => cell.CompareTo(other.cell);

        public static bool operator ==(BinaryTreeCellsNode leftOp, BinaryTreeCellsNode rightOp)
        {
            return leftOp.cell.Location == rightOp.cell.Location;
        }

        public static bool operator ==(BinaryTreeCellsNode leftOp, Cell cell)
        {
            return leftOp.cell.Location == cell.Location;
        }

        public static bool operator !=(BinaryTreeCellsNode leftOp, BinaryTreeCellsNode rightOp) => leftOp.cell.Location != rightOp.cell.Location;

        public static bool operator !=(BinaryTreeCellsNode leftOp, Cell cell) => leftOp.cell.Location != cell.Location;

        public static bool operator <(BinaryTreeCellsNode leftOp, BinaryTreeCellsNode rightOp) => leftOp.CompareTo(rightOp) < 0;

        public static bool operator <(BinaryTreeCellsNode leftOp, Cell cell) => leftOp.CompareTo(cell) < 0;

        public static bool operator <=(BinaryTreeCellsNode leftOp, BinaryTreeCellsNode rightOp) => leftOp.CompareTo(rightOp) <= 0;

        public static bool operator >(BinaryTreeCellsNode leftOp, BinaryTreeCellsNode rightOp) => leftOp.CompareTo(rightOp) > 0;

        public static bool operator >(BinaryTreeCellsNode leftOp, Cell cell) => leftOp.CompareTo(cell) > 0;

        public static bool operator >=(BinaryTreeCellsNode leftOp, BinaryTreeCellsNode rightOp) => leftOp.CompareTo(rightOp) >= 0;


        public bool Equals(BinaryTreeCellsNode other)
        {
            return cell.Equals(other.cell);
        }
    }

    public class BinaryTreeCellsEnumerator : IEnumerator<Cell>
    {
        BinaryTreeCellsNode current;
        readonly BinaryTreeCells cells;

        Stack<BinaryTreeCellsNode> RightNode;

        
        
        public BinaryTreeCellsEnumerator(BinaryTreeCells cells)
        {
            this.cells = cells;

            Reset();

            RightNode = new Stack<BinaryTreeCellsNode>();
        }

        public Cell Current => current.cell;

        object IEnumerator.Current => Current;

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                current = null;
                RightNode = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public bool MoveNext()
        {
            BinaryTreeCellsNode oldCurrentRight = current.right;

            current = current.left;
            
            if (current == (Object)null)
            {
                if (oldCurrentRight != (Object)null)
                {
                    current = oldCurrentRight;
                }
                else
                {
                    if (RightNode.Count == 0)
                    {
                        return false;
                    }
                        
                    current = RightNode.Pop(); // В стеке ещё есть праый узел, Переходим на него
                }
            }
            else
            if (oldCurrentRight != (Object)null)
            {
                RightNode.Push(oldCurrentRight);
            }

            return true;
        }

        public void Reset()
        {
            current = new BinaryTreeCellsNode(null)
            {
                left = cells.First(),
                right = null,
                parent = null
            };
        }
    }

    public class BinaryTreeCells : ICellArray
    {
        BinaryTreeCellsNode rootNode;
        private int count;

        public int Count { get => count; set => count = value; }

        public BinaryTreeCells()
        {
            rootNode = null;
            count = 0;
        }

        public void Clear()
        {
            rootNode = null;
            count = 0;
        }

        public BinaryTreeCellsNode First() => rootNode;

        public void Add(Cell cell) => Add(new BinaryTreeCellsNode(cell));

        public void Add(BinaryTreeCellsNode node, BinaryTreeCellsNode current = null)
        {
            if (rootNode == (Object)null)
            {
                rootNode = node;
                node.parent = null;
                count++;
                return;
            }

            if (current == (Object)null)
            {
                current = rootNode;
            }

            while (node != current)  // если эта клетка уже есть - выходим
            {
                if (node < current)
                {
                    if (current.left == (Object)null)
                    {
                        node.parent = current;
                        current.left = node;
                        count++;
                        return;
                    }

                    current = current.left;
                    continue;
                }

                if (current.right == (Object)null)
                {
                    node.parent = current;
                    current.right = node;
                    count++;
                    return;
                }

                current = current.right;
            }
        }

        public void Remove(Cell cell)
        {
            BinaryTreeCellsNode node = Find(cell);

            if (node != (Object)null)
            {
                if (node.parent == (Object)null) // Это корневой узел
                {
                    if (node.right == (Object)null)
                    {
                        if (node.left == (Object)null)  // нет потомков
                        {
                            rootNode = null;
                            count--;
                            return;
                        }
                        else
                        {
                            node.left.parent = null;
                            rootNode = node.left;
                            count--;
                            return;
                        }
                    }
                    else
                    {
                        node.right.parent = null;
                        rootNode = node.right;
                        count--;

                        if (node.left != (Object)null)
                        {
                            count--;                    // узел перемещается, компенсируем увеличение счётчика в Add
                            Add(node.left);
                        }

                        return;
                    }
                }

                if (node.right == (Object)null)
                {
                    node.SetParentLeftOrRight(node.left);
                    if (node.left != (object)null)
                    {
                        node.left.parent = node.parent;
                    }
                }
                else
                {
                    node.SetParentLeftOrRight(node.right);
                    node.right.parent = node.parent;

                    if (node.left != (Object)null)
                    {
                        count--;                    // узел перемещается, компенсируем увеличение счётчика в Add
                        Add(node.left, node.right);
                    }
                }

                count--;
                node.parent = null;
            }
        }

        private BinaryTreeCellsNode Find(Cell cell)
        {
            BinaryTreeCellsNode current = rootNode;

            while (current != (object)null)
            {
                if (current == cell)    // клетка найдена
                {
                    return current;
                }

                if (current < cell) // cell > current.cell
                {
                    current = current.right;
                }
                else
                {
                    current = current.left;
                }
            }

            return current;
        }

        public Cell this[int x, int y]
        {
            get
            {
                return Find(new Cell(x, y))?.cell;
            }

            set
            {
                BinaryTreeCellsNode node = Find(new Cell(x, y));

                if (node != (Object)null)
                {
                    return; // такая клетка уже есть
                }

                Add(new BinaryTreeCellsNode(value));
            }
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            return new BinaryTreeCellsEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Resize(int dx, int dy)
        {
            rootNode = null;
            count = 0;
        }
    }
}
