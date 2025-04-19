namespace TextRepositoryEditor
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

        // FindChild
        internal static T? FindChild<T>(this TreeNode node, string name)
            where T : TreeNode
        {
            foreach (T childNode in node.Nodes.OfType<T>())
            {
                if (childNode.Text == name)
                    return childNode;
            }

            return null;
        }

        // GetChildNodesRecursively
        internal static TreeNode[] GetChildNodesRecursively(this TreeNode node)
        {
            var nodeList = new List<TreeNode>();
            CollectChildNodes(node, nodeList);
            return nodeList.ToArray();
        }
    }
}
