using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// TransitionManager
    /// </summary>
    public static class TransitionManager
    {
        private static readonly Stack<Transition> transitions = new();

        // CurrentTransition
        public static Transition CurrentTransition => transitions.Count == 0 ? DefaultTransition : transitions.Peek();

        // DefaultTransition
        public static Transition DefaultTransition { get; set; } = new Transition() { TweenStyle = TweenStyle.CubicIn };

        // PopTransition
        public static Transition PopTransition()
        {
            return transitions.Pop();
        }

        // PushTransition
        public static void PushTransition(Transition transition)
        {
            transitions.Push(transition);
        }
    }
}
