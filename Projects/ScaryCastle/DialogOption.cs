using Adberration.Scripting;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// DialogOption
    /// </summary>
    public sealed class DialogOption
    {
        private readonly FlagCondition? condition;

        #region Constructor

        // Constructor
        public DialogOption(DialogBlock dialog, int id, string text, FlagCondition? condition, int[] requiredOptions)
        {
            this.Dialog = dialog;
            this.Id = id;
            this.Text = text;
            this.RequiredOptions = new ReadOnlyCollection<int>(requiredOptions ?? []);
            this.condition = condition;
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

                if (RequiredOptions.Count > 0)
                {
                    for (int i = 0; i < RequiredOptions.Count; i++)
                    {
                        if (Dialog.FindOption(RequiredOptions[i]) != null)
                            return false;
                    }
                }

                return true;
            }
        }

        // ReadKey
        public string ReadKey => string.IsNullOrWhiteSpace(Dialog.ReadKeyPrefix) ? string.Empty : Dialog.ReadKeyPrefix + "_" + Id;

        // RequiredOptions
        public ReadOnlyCollection<int> RequiredOptions { get; }

        // Text
        public string Text { get; }
    }
}
