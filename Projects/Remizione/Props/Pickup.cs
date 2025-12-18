using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// Pickup
    /// </summary>
    public sealed class Pickup : Prop
    {
        #region Private fields

        private BouncingIcon? bouncingIcon;
        private int delayCoolDown;
        private bool isCoin;
        private MetaItem? metaItem;
        private readonly BounceScaleEffect bounceScaleEffect = new();
        private bool showItemIcon;

        #endregion

        // Constructor
        public Pickup(GameSession session, string name)
            : base(session, name)
        {
            this.CollisionDetection = false;
            this.DepthOffset = -1;
            this.HighlightInteraction = false;
            this.Hotspot = new("7,0;7,7;0,7;0,0");
            this.IgnoreWalkArea = false;
        }

        #region Private members

        // DropCore
        private void DropCore(Room room, Vector2 position, MetaItem metaItem, bool showItemIcon, int delay)
        {
            room.Children.Add(this);
            this.Position = position;
            this.metaItem = metaItem;
            this.showItemIcon = showItemIcon;
            this.delayCoolDown = delay;

            Sprite.ClearAnimations();
            this.Atlas = showItemIcon ? Atlases.UI : Atlases.Environment;
            this.DefaultImageName = showItemIcon ? metaItem.Name : nameof(Atlases.Environment.Sack);
            this.DisplayNameKey = $"Item.{metaItem.Name}.Name";
            this.isCoin = metaItem.Name == MetaItem.CoinItemName;
            this.Scale = Vector2.Zero;
        }

        // Pop
        private void Pop()
        {
            float scale = showItemIcon ? ScaleInfo.UIElement.Tiny.X : 1;
            bounceScaleEffect.Play(scale, .75f);

            if (Sound.Find(isCoin ? SoundNames.LootCoin : SoundNames.LootSack)?.PopInstance() is SoundInstance soundInstance)
                soundInstance.PlayDelayed(300);
        }

        #endregion

        #region Protected members

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            this.metaItem = null;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (metaItem == null)
                return;

            if (delayCoolDown >= 0)
            {
                delayCoolDown -= gameTime.ElapsedGameTime.Milliseconds;

                if (delayCoolDown < 0)
                    Pop();

                return;
            }

            if (bouncingIcon?.IsSettled == false)
            {
                bouncingIcon.Update(gameTime);
                Position = bouncingIcon.Position;
                if (bouncingIcon.IsSettled)
                    AllowInteraction = true;
            }
            
            base.OnUpdate(gameTime);

            if (bounceScaleEffect.IsPlaying)
            {
                bounceScaleEffect.Update(gameTime);
                Scale = new(bounceScaleEffect.Value);
            }
        }

        #endregion

        // Collect
        [ScriptMethod]
        public void Collect()
        {
            if (metaItem != null)
            {
                if (Room is ProceduralRoom procRoom)
                {
                    /*
                    if (isCoin)
                        procRoom.RoomGraph.HasCoin = false;
                    else
                        procRoom.RoomGraph.SackCount--;
                    */
                }

                Session.Player?.Animate(AnimationNames.PickUp);
                Session.Inventory.Add(metaItem, 1);
                Session.HUD.Log.Show(LogVerb.PickedUp, metaItem);
                Unparent();
                Session.ObjectPools.Pickups.Return(this);
            }
        }

        // DropCoin
        public void Drop(Room room, Vector2 origin)
        {
            DropCore(room, origin, MetaItem.FindNotNull(MetaItem.CoinItemName), true, 0);
        }

        // DropSack
        public void Drop(Room room, Vector2 origin, MetaItem metaItem)
        {
            DropCore(room, origin, metaItem, false, 0);
        }

        // DropItem
        public void Drop(Room room, Vector2 origin, MetaItem metaItem, float floorY, int delay)
        {
            DropCore(room, origin, metaItem, true, delay);
            Scale = ScaleInfo.UIElement.Tiny;
            bouncingIcon = new(origin, floorY);
        }

        /// <summary>
        /// BouncingIcon
        /// </summary>
        private sealed class BouncingIcon
        {
            public Vector2 Position;
            Vector2 _velocity;

            public bool IsSettled { get; private set; }

            float _floorY;
            int _bouncesLeft = 3;

            const float Gravity = 900f;
            const float BounceDamping = 0.55f;
            const float Friction = 0.65f;
            const float MinYVelocity = 25f;

            public BouncingIcon(Vector2 startPos, float floorY)
            {
                Position = startPos;
                _floorY = floorY;

                _velocity = new Vector2(
                    Random.Shared.NextSingle() * 80f - 40f, // arco corto
                    -120f                                  // impulso inicial
                );
            }

            public void Update(GameTime gameTime)
            {
                if (IsSettled)
                    return;

                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                _velocity.Y += Gravity * dt;
                Position += _velocity * dt;

                if (Position.Y >= _floorY)
                {
                    Position.Y = _floorY;

                    if (_bouncesLeft > 0 && MathF.Abs(_velocity.Y) > MinYVelocity)
                    {
                        _velocity.Y = -_velocity.Y * BounceDamping;
                        _velocity.X *= Friction;
                        _bouncesLeft--;
                    }
                    else
                    {
                        _velocity = Vector2.Zero;
                        IsSettled = true;
                    }
                }
            }
        }


    }
}
