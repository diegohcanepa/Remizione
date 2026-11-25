using Adberration.Scripting;
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
            LabelKey = "Prop.FigTree";
            ShadowOffset = new(16, -2);
            StandPoint = new(35, 60);
        }

        #endregion

        #region Protected members

        // OnCustomStateChanged
        protected override void OnCustomStateChanged()
        {
            base.OnCustomStateChanged();
            Scale = CustomState == 1 ? new(.75f) : Vector2.One;
        }

        // OnItemRewardChanged
        protected override void OnItemRewardChanged()
        {
            base.OnItemRewardChanged();

            if (!HasItemReward)
                TimeUntilNextReward = GameData.WorldSettings.FigTreeRewardInterval;

            OnCustomStateChanged();
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            OnCustomStateChanged();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (Session.CycleManager.CycleCount >= CycleThreshold)
            {
                if (!HasItemReward && CustomState == 3 && GameData.WorldSettings.FigTreeRewardInterval > 0)
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
            CycleThreshold = Session.CycleManager.CycleCount + 3;
            TimeUntilNextReward = GameData.WorldSettings.FigTreeRewardInterval;

            if (Room != null && Orb.Drop(ItemNames.Fig, Room, Position + new Vector2(-6, 4)) is { } orb)
                orb.ExpirationTimer = 0;
        }

        // TimeUntilNextReward
        [ScriptProperty]
        public float TimeUntilNextReward { get; set; }
    }
}
