using Adberration.Scripting;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// DialogBlock
    /// </summary>
    public sealed class DialogBlock : Collection<DialogOption>
    {
        private readonly List<DialogOption> availableOptions = [];

        #region Constructor

        // Constructor
        public DialogBlock(Script? script, bool allowQuit)
        {
            this.Script = script;
            this.AllowQuit = allowQuit;
            this.AvailableOptions = new ReadOnlyCollection<DialogOption>(availableOptions);

            Invalidate();
        }

        #endregion

        #region Protected members

        // ClearItems
        protected override void ClearItems()
        {
            base.ClearItems();
            Invalidate();
        }

        // InsertItem
        protected override void InsertItem(int index, DialogOption item)
        {
            base.InsertItem(index, item);
            Invalidate();
        }

        // RemoveItem
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            Invalidate();
        }

        // SetItem
        protected override void SetItem(int index, DialogOption item)
        {
            base.SetItem(index, item);
            Invalidate();
        }

        #endregion

        // Add
        public DialogOption Add(int id, string text, FlagCondition? condition, int[] requiredOptions)
        {
            var result = new DialogOption(this, id, text, condition, requiredOptions);
            Add(result);
            return result;
        }

        // AllowQuit
        public bool AllowQuit { get; }

        // AvailableOptions
        public ReadOnlyCollection<DialogOption> AvailableOptions { get; private set; }

        // FindOption
        public DialogOption? FindOption(int id)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Id == id)
                    return this[i];
            }

            return null;
        }

        // Instance
        public static DialogBlock? Instance { get; set; }

        // IndexOf
        public int IndexOf(int id)
        {
            var option = FindOption(id);
            return option == null ? -1 : IndexOf(option);
        }

        // Insert
        public DialogOption Insert(int index, int id, string text, FlagCondition? condition, int[] requiredOptions)
        {
            var result = new DialogOption(this, id, text, condition, requiredOptions);
            Insert(index, result);
            return result;
        }

        // Invalidate
        public void Invalidate()
        {
            availableOptions.Clear();
            for (int i = 0; i < Count; i++)
            {
                if (this[i].IsAvailable)
                    availableOptions.Add(this[i]);
            }
        }

        // IsEmpty
        public bool IsEmpty => AvailableOptions.Count == 0;

        // Remove
        public bool Remove(int id)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Id == id)
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        // Script
        public Script? Script { get; }
    }
}
