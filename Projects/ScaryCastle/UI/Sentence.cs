using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// Sentence
    /// </summary>
    public sealed class Sentence
    {
        private readonly GameSession session;
        private GameThing? lastKnownTarget;
        private readonly string useVerb;
        private readonly string withPreposition;

        // Constructor
        public Sentence(GameSession session)
        {
            this.session = session;
            this.useVerb = Localization.GetValue(Verb.Use);
            this.withPreposition = TextRepository.GetValue("Misc.WithPreposition");
        }

        // ForceRefresh
        public bool ForceRefresh { get;set; }

        // Refresh
        public void Refresh()
        {
            if (session.IsCurrentScene && session.Player?.InteractiveTarget is GameThing currentTarget)
            {
                if (currentTarget != lastKnownTarget || ForceRefresh)
                {
                    lastKnownTarget = currentTarget;
                    var sentence = currentTarget.GetInteractPrompt() ?? currentTarget.LocalizedDisplayName;

                    if (session.Inventory.HeldItem == null)
                    {
                        MouseCursor.Highlight = false;
                        MouseCursor.Text = sentence;
                    }
                    else
                    {
                        MouseCursor.Highlight = true;
                        MouseCursor.Text = $"{useVerb} {session.Inventory.HeldItem.Definition.LocalizedDisplayName} {withPreposition} {sentence}";
                    }

                    ForceRefresh = false;
                }
            }
            else
            {
                MouseCursor.Highlight = false;
                MouseCursor.Text = null;
                lastKnownTarget = null;
            }
        }
    }
}
