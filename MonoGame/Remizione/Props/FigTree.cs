namespace Remizione
{
    /// <summary>
    /// FigTree
    /// </summary>
    public class FigTree : Prop
    {
        #region Constructor

        // Constructor
        public FigTree(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            DepthOffset = -2;
            IsStatic = true;
            LabelKey = "Prop.FigTree";
            ShadowOffset = new(16, -2);
            StandPoint = new(20, 33);
            Verb = Verb.LookAt;
        }

        #endregion
    }
}
