namespace ScaryCastle.Menus
{
    /// <summary>
    /// BooleanOption
    /// </summary>
    public class BooleanOption : Option<bool>
    {
        // Constructor
        public BooleanOption(ScaryCastleGame game, string displayName, bool value)
            : base(game, displayName)
        {
            AddValue("@Misc.No", false);
            AddValue("@Misc.Yes", true);

            this.Value = value;
        }
    }
}
