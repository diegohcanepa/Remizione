using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Engendro
{
    /// <summary>
    /// TreeNodeExtensions
    /// </summary>
    internal static class TreeNodeExtensions
    {
        // CollectChildNodes
        private static void CollectChildNodes(TreeNode node, List<TreeNode> list)
        {
            foreach (var childNode in node.Nodes.OfType<TreeNode>())
            {
                list.Add(childNode);
                CollectChildNodes(childNode, list);
            }
        }

        // GetChildNodesRecursively
        internal static TreeNode[] GetChildNodesRecursively(this TreeNode node)
        {
            List<TreeNode> nodeList = [];
            CollectChildNodes(node, nodeList);
            return nodeList.ToArray();
        }

        // IsParentOf
        internal static bool IsParentOf(this TreeView tree, TreeNode node1, TreeNode node2)
        {
            // Same node
            if (node1 == node2 || node2.Parent == null)
            {
                return false;
            }

            if (node2.Parent == node1)
            {
                return true;
            }

            // If the parent node is not null or equal to the first node,   
            // call the ContainsNode method recursively using the parent of   
            // the second node.  
            return IsParentOf(tree, node1, node2.Parent);
        }

        // SortTree
        internal static void SortTree(this TreeView tree)
        {
            var currentNode = tree.SelectedNode;
            tree.BeginUpdate();
            tree.Sort();
            tree.SelectedNode = currentNode;
            tree.EndUpdate();
        }
    }
}
