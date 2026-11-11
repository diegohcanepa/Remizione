using System.Collections.Generic;

namespace Adberration
{
    /// <summary>
    /// EntityDepthComparer
    /// </summary>
    public sealed class EntityDepthComparer : Comparer<Entity>
    {
        // Compare
        public override int Compare(Entity? x, Entity? y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            // Compare against layer depth
            var layerDepthComparison = x.RenderLayerDepth.CompareTo(y.RenderLayerDepth);
            if (layerDepthComparison != 0)
            {
                return layerDepthComparison;
            }

            if (x.Depth == y.Depth)
            {
                return x.EntityId.CompareTo(y.EntityId);
            }
            else
            {
                return x.Depth.CompareTo(y.Depth);
            }
        }

        // Instance
        public static EntityDepthComparer Instance { get; } = new EntityDepthComparer();
    }
}
