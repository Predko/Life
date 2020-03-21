using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace life
{
    public enum SideNode { Left, Right, Parent }
    
    public class BinaryTreeCellsNode: IComparable<Cell>, IEquatable<BinaryTreeCellsNode>
    {
        public BinaryTreeCellsNode left;
        public BinaryTreeCellsNode right;
        public BinaryTreeCellsNode parent;

        private readonly Cell cell;

        public BinaryTreeCellsNode(Cell cell)
        {
            this.cell = cell;
        }

        public SideNode Side() => (this == parent.left)  ? SideNode.Left : 
                                  (this == parent.right) ? SideNode.Right : 
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

        public static bool operator ==(BinaryTreeCellsNode leftOp, BinaryTreeCellsNode rightOp) => leftOp.cell.Location == rightOp.cell.Location;

        public static bool operator ==(BinaryTreeCellsNode leftOp, Cell cell) => leftOp.cell.Location == cell.Location;

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

    public class BinaryTreeCells
    {
        BinaryTreeCellsNode rootNode;

        public BinaryTreeCells()
        {
            rootNode = null;
        }



        public void Add(BinaryTreeCellsNode node, BinaryTreeCellsNode current = null)
        {
            if ( rootNode == (BinaryTreeCellsNode)null)
            {
                rootNode = node;
                node.parent = null;
                return;
            }

            if (current == (BinaryTreeCellsNode)null)
            {
                current = rootNode;
            }
            
            while (node == current)  // эта ячейка уже есть
            { 
                if (node < current)
                {
                    if (current.left == (BinaryTreeCellsNode)null)
                    {
                        node.parent = current;
                        current.left = node;
                        return;
                    }

                    current = current.left;
                    continue;
                }

                if (current.right == (BinaryTreeCellsNode)null)
                {
                    node.parent = current;
                    current.right = node;
                    return;
                }

                current = current.right;
            }
        }

        public void Remove(Cell cell)
        {
            BinaryTreeCellsNode current = rootNode;
            BinaryTreeCellsNode node = Find(cell);

            if (node != (BinaryTreeCellsNode)null)
            {
                if (node.parent == (BinaryTreeCellsNode)null) // Это корневой узел
                {
                    if (node.right == (BinaryTreeCellsNode)null)
                    {
                        if (node.left == (BinaryTreeCellsNode)null)  // нет потомков
                        {
                            rootNode = null;
                            return;
                        }
                        else
                        {
                            node.left.parent = null;
                            rootNode = node.left;
                            return;
                        }
                    }
                    else 
                    {
                        node.right.parent = null;
                        rootNode = node.right;

                        if (node.left != (BinaryTreeCellsNode)null)
                        {
                            Add(node.left, node.right);
                        }

                        return;
                    }
                }

                if (node.right == (BinaryTreeCellsNode)null)
                {
                    if (node.left == (BinaryTreeCellsNode)null)  // нет потомков
                    {
                        node.SetParentLeftOrRight(null);
                    }
                    else
                    {
                        node.SetParentLeftOrRight(node.left);
                    }
                }
                else
                {
                    node.SetParentLeftOrRight(node.right);

                    if (node.left != (BinaryTreeCellsNode)null)
                    {
                        Add(node.left, node.right);
                    }
                }
                
                node.parent = null;
            }
        }

        private BinaryTreeCellsNode Find(Cell cell)
        {
            BinaryTreeCellsNode current = rootNode;
        
            while (current != cell || current != (BinaryTreeCellsNode)null)
            {
                if (current > cell)
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
    }
}
