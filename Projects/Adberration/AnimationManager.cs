using Engendro;
using System.Collections.Generic;

namespace Adberration
{
    /// <summary>
    /// AnimationManager
    /// </summary>
    internal class AnimationManager
    {
        private readonly Dictionary<string, AnimationSet> animationSets = [];

        // Clear
        public void Clear()
        {
            animationSets.Clear();
        }

        // Find
        public AnimationSet? Find(string name)
        {
            return animationSets.TryGetValue(name, out var set) ? set : null;
        }

        // Get
        public AnimationSet Get(string name)
        {
            var result = Find(name) ?? throw new KeyNotFoundException($"The animation set '{name}' is not registered.");
            return result;
        }

        // Register
        public void Register(AnimationSet animationSet)
        {
            if (animationSets.ContainsKey(animationSet.Name))
                CodeContract.ThrowDuplicatedNameException(animationSet.Name);

            animationSets.Add(animationSet.Name, animationSet);
        }
    }
}