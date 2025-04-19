using System.Collections.Generic;
using System.Windows.Forms;

namespace Engendro
{
    /// <summary>
    /// TreeNodeTextComparer
    /// </summary>
    internal sealed class TreeNodeTextComparer : IComparer<TreeNode>
    {
        // Compare
        public int Compare(TreeNode? x, TreeNode? y)
        {
            if (x == null || y == null)
            {
                return -1;
            }

            return string.Compare(x.Text, y.Text, System.StringComparison.Ordinal);
        }
    }
}