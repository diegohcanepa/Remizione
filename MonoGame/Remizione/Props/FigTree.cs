using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;

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
            StandPoint = new(35, 60);
            Verb = Verb.LookAt;

            AttachedLight ??= new("Light", LightKind.Default)
            {
                BlendMode = LightBlendMode.Max,
                PivotOrigin = RectanglePoint.Center,
                Scale = new(3,2)
            };

            AttachedLightPosition = new(40,32);

            AttachedLight.Unlit(true);
        }

        #endregion

        #region Protected members

        // OnCustomStateChanged
        protected override void OnCustomStateChanged()
        {
            base.OnCustomStateChanged();
            
            if (CustomState == 1)
                Scale = new(.75f);

            else if (CustomState == 2)
                TimeUntilNextReward = GameData.WorldSettings.FigTreeRewardInterval;
        }

        // OnItemRewardChanged
        protected override void OnItemRewardChanged()
        {
            base.OnItemRewardChanged();

            if (HasItemReward)
            {
                Verb = Verb.Use;
                AttachedLight?.Lit(4000);
            }
            else
            {
                TimeUntilNextReward = GameData.WorldSettings.FigTreeRewardInterval;
                Verb = Verb.LookAt;
                AttachedLight?.Unlit(true);
            }
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            if (ItemReward != null)
                AttachedLight?.Lit(true);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            
            if (!HasItemReward && CustomState == 2 && GameData.WorldSettings.FigTreeRewardInterval > 0)
            {
                TimeUntilNextReward -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (TimeUntilNextReward <= 0)
                    ItemRewardName = ItemNames.Fig;
            }
        }

        #endregion

        // TimeUntilNextReward
        [ScriptProperty]
        public float TimeUntilNextReward { get; set; } = GameData.WorldSettings.FigTreeRewardInterval;
    }
}
