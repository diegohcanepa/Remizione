using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Creature
    /// </summary>
    public class Creature : Actor
    {
        private readonly FloatTween breathTween;
        private readonly FloatTween creepTween = new();
        private Vector2 scaleFactor;

        // Constructor
        public Creature(GameSession session, string name)
            : base(session, name)
        {
            Faction = Faction.Creature;
            Verb = Verb.Examine;
            breathTween = FloatTween.Create(TweenStyle.CubicInOut, 0, .05f, 600, -1);
            creepTween = FloatTween.Create(TweenStyle.CubicInOut, 0, -.1f, 200, -1);
        }

        #region Private members

        // RefreshCreepTween
        private void RefreshCreepTween()
        {
            if (CreepEffectSize == 0 || CreepEffectInterval == 0)
            {
                creepTween.Stop();
                return;
            }
            creepTween.Start(TweenStyle.CubicInOut, 0, CreepEffectSize, CreepEffectInterval, -1);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Sprite.ScaleFactor += scaleFactor;
            base.OnDraw(gameTime);
            Sprite.ScaleFactor -= scaleFactor;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            breathTween.Update(gameTime);
            creepTween.Update(gameTime);

            scaleFactor.X = IsMoving && creepTween.IsRunning ? creepTween.CurrentValue : 0;
            scaleFactor.Y = !IsMoving && BreathEffect ? breathTween.CurrentValue : 0;
        }

        #endregion

        // BreathEffect
        [ScriptProperty]
        public bool BreathEffect { get; set; }

        // CreepEffectSize
        [ScriptProperty]
        public float CreepEffectSize
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    RefreshCreepTween();
                }
            }
        }

        // CreepEffectInterval
        [ScriptProperty]
        public int CreepEffectInterval
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    RefreshCreepTween();
                }
            }
        }
    }
}
