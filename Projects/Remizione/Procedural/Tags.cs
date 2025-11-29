using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Remizione
{
    /// <summary>
    /// Tags
    /// </summary>
    public sealed class Tags : ReadOnlyCollection<string>
    {
        private static readonly HashSet<string> allowedTags = new()
        {
            "ceiling",
            "flip",
            "floor",
            "pottery",
            "wall",
        };

        // Constructor
        public Tags(IList<string> tags)
            : base(tags)
        {
            // Validate tags
            foreach (var tag in tags)
            {
                if (!allowedTags.Contains(tag))
                    throw new ArgumentException($"'{tag}' is not a valid tag.");
            }
        }
    }
}
