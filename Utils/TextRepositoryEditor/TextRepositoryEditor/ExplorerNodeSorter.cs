using System.Collections;

namespace TextRepositoryEditor
{
    /// <summary>
    /// ExplorerNodeSorter
    /// </summary>
    internal class ExplorerNodeSorter : IComparer, IComparer<TreeNode>
    {
        // Compare the length of the strings, or the strings
        // themselves, if they are the same length.
        public int Compare(TreeNode? x, TreeNode? y)
        {
            if (x == null || x is LanguagePackageFolderNode)
                return -1;

            if (y is LanguagePackageFolderNode)
                return 1;

            if (y == null)
                return 1;

            if (x is FolderNode folderNodeX && folderNodeX.IsPredefinedFolder)
                return -1;

            if (y is FolderNode folderNodeY && folderNodeY.IsPredefinedFolder)
                return 1;

            if (x is FolderNode && y is TextNode)
                return -1;

            else if (x is TextNode && y is FolderNode)
                return 1;

            else
                return string.Compare(x.Text, y.Text);
        }

        // Compare
        int IComparer.Compare(object? x, object? y) => Compare(x as TreeNode, y as TreeNode);
    }
}
