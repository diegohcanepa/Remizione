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

            AttachedLight ??= new("Light", LightKind.Default)
            {
                BlendMode = LightBlendMode.Max,
                PivotOrigin = RectanglePoint.Center,
                Opacity = .7f,
                Scale = new(4)
            };

            AttachedLightPosition = new(27, 30);

            AttachedLight.Unlit(true);
        }

        #endregion

        #region Protected members

        // OnCustomStateChanged
        protected override void OnCustomStateChanged()
        {
            base.OnCustomStateChanged();
            Scale = CustomState == 1 ? new(.75f) : Vector2.One;

            if (CustomState > 1)
                Verb = Verb.Use;
            else
                Verb = Verb.LookAt;
        }

        // OnItemRewardChanged
        protected override void OnItemRewardChanged()
        {
            base.OnItemRewardChanged();

            if (HasItemReward)
            {
                AttachedLight?.Lit(2000);
            }
            else
            {
                TimeUntilNextReward = GameData.WorldSettings.FigTreeRewardInterval;
                AttachedLight?.Unlit(3000);
            }

            OnCustomStateChanged();
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            if (ItemReward != null)
                AttachedLight?.Lit(true);
            OnCustomStateChanged();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            
            if (Session.CycleManager.CycleCount >= CycleThreshold)
            {
                if (!HasItemReward && CustomState == 2 && GameData.WorldSettings.FigTreeRewardInterval > 0)
                {
                    TimeUntilNextReward -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (TimeUntilNextReward <= 0)
                        Reward();
                }
            }
        }

        #endregion

        // CycleThreshold
        [ScriptProperty]
        public int CycleThreshold { get; set; }

        // Reward
        [ScriptMethod]
        public void Reward()
        {
            ItemRewardName = ItemNames.Fig;
            CycleThreshold = Session.CycleManager.CycleCount + 3;
            TimeUntilNextReward = GameData.WorldSettings.FigTreeRewardInterval;
        }

        // TimeUntilNextReward
        [ScriptProperty]
        public float TimeUntilNextReward { get; set; }
    }
}
