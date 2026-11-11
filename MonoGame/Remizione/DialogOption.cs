using Adberration.Scripting;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// DialogOption
    /// </summary>
    public sealed class DialogOption
    {
        private readonly FlagCondition? condition;

        #region Constructor

        // Constructor
        public DialogOption(DialogBlock dialog, int id, string text, FlagCondition? condition, int[] requiresRead)
        {
            this.Dialog = dialog;
            this.Id = id;
            this.Text = text;
            this.condition = condition;
            this.RequiresRead = new ReadOnlyCollection<int>(requiresRead ?? []);
        }

        #endregion

        // Dialog
        public DialogBlock Dialog { get; }

        // Id
        public int Id { get; }

        // IsAvailable
        public bool IsAvailable
        {
            get
            {
                if (condition != null && !condition.Evaluate())
                    return false;

                if (RequiresRead.Count > 0)
                {
                    for (int i = 0; i < RequiresRead.Count; i++)
                    {
                        var requiredOption = Dialog.FindOption(RequiresRead[i]);
                        if (requiredOption == null || !requiredOption.IsRead)
                            return false;
                    }
                }

                return true;
            }
        }

        // IsRead
        public bool IsRead { get; set; }

        // RequiresRead
        public ReadOnlyCollection<int> RequiresRead { get; }

        // Text
        public string Text { get; }
    }
}
