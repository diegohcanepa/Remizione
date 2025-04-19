using Engendro.Nodes;
using System;
using System.Collections;

namespace Engendro
{
    /// <summary>
    /// DocumentNodeSorter
    /// </summary>
    public sealed class DocumentNodeSorter : IComparer
    {
        // Compare
        public int Compare(object? x, object? y)
        {
            DocumentNode? nodeX = x as DocumentNode;
            DocumentNode? nodeY = y as DocumentNode;

            if (nodeX == null || nodeY == null)
            {
                return 0;
            }

            // Primero compara por la propiedad Checked
            if (nodeX.IsPinned && !nodeY.IsPinned)
            {
                return -1;
            }
            else if (!nodeX.IsPinned && nodeY.IsPinned)
            {
                return 1;
            }
            else
            {
                return string.Compare(nodeX.Text, nodeY.Text, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
