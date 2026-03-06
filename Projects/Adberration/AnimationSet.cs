using Engendro;
using System.Collections.Generic;

namespace Adberration
{
    /// <summary>
    /// AnimationSet
    /// </summary>
    internal sealed class AnimationSet : INamedObject
    {
        private readonly Dictionary<string, SpriteAnimation> animations = [];

        // Constructor
        public AnimationSet(string archetype)
        {
            CodeContract.ValidName(archetype, nameof(archetype));
            Name = archetype;
        }

        // AddAnimation
        public void AddAnimation(SpriteAnimation animation)
        {
            if (animations.ContainsKey(animation.Name))
                CodeContract.ThrowDuplicatedNameException(animation.Name);

            animations.Add(animation.Name, animation);
        }

        // GetAnimation
        public SpriteAnimation? GetAnimation(string name)
        {
            animations.TryGetValue(name, out var animation);
            return animation;
        }

        // Name
        public string Name { get; }
    }
}