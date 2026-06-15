using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Engendro.PathFinding;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary> 
    /// GameThing 
    /// </summary>
    public abstract class GameThing : Thing, IHoleArea, ILightSource
    {
        #region Private fields

        private readonly Blinker<bool> blinker = new(false, true);
        private bool dieCalled;
        private FloatTween? floatingTween;
        private readonly Polygon holePoly = new();
        private static readonly Vector2 hurtShakeForce = new(1.5f, 0);
        private Vector2Tween? hurtShakeTween;
        private FloatTween? hurtTween;
        private bool isCollisionDirty;
        private bool isHotspotDirty = true;
        private Vector2 knockbackVelocity;
        private const float KnockbackFriction = 0.90f; // Ajustá este valor (0.8 - 0.95)
        private PathNode[]? pathNodes;
        private int renderLayerDepth;
        private readonly ShadowSpot shadowSpot;
        private bool shouldClampToWalkablePosition;

        #endregion

        #region Constructor

        // Constructor
        protected GameThing(GameSession session, string name)
            : base(session, name)
        {
            this.RenderLayer = RenderLayer.Default;
            this.Session = session;
            this.ResistanceTableName = DeclaredName;
            this.shadowSpot = new ShadowSpot(this);
        }

        #endregion

        #region IHoleArea explicit implementation

        // ClampOutside
        Vector2 IHoleArea.ClampOutside(Vector2 position)
        {
            if (!Collider.IsEmpty)
            {
                InvalidateCollisionPolygons();
                if (holePoly.Contains(position))
                    position = RuntimeCollider.GetClosestPointOnEdge(position);
            }

            return position;
        }

        // CollectPathNodes
        void IHoleArea.CollectPathNodes(IList<PathNode> list)
        {
            InvalidateCollisionPolygons();

            if (Collider.IsEmpty)
                return;

            if (pathNodes == null || pathNodes.Length != Collider.Vertices.Count)
                pathNodes = new PathNode[Collider.Vertices.Count];

            for (int i = 0; i < RuntimeCollider.Vertices.Count; i++)
            {
                // Is point concave?
                if (RuntimeCollider.IsVertexConcave(i))
                    continue;

                // Is point outside walk area
                if (WalkArea != null && !WalkArea.Contains(RuntimeCollider.Vertices[i]))
                    continue;

                if (pathNodes[i] == null)
                    pathNodes[i] = new(RuntimeCollider.Vertices[i]);
                else
                    pathNodes[i].Position = RuntimeCollider.Vertices[i];

                list.Add(pathNodes[i]);
            }
        }

        // Contains
        bool IHoleArea.Contains(Vector2 point)
        {
            InvalidateCollisionPolygons();
            return holePoly.Contains(point);
        }

        // InLineOfSight
        bool IHoleArea.InLineOfSight(Vector2 origin, Vector2 destination)
        {
            InvalidateCollisionPolygons();
            return holePoly.InLineOfSight(origin, destination);
        }

        // IsActive
        bool IHoleArea.IsActive => !Collider.IsEmpty && AffectsPathfinding;

        // Polygon
        ReadOnlyPolygon IHoleArea.Polygon => holePoly;

        #endregion

        #region Private members

        // CheckCollisions
        private void CheckCollisions()
        {
            if (Room == null)
                return;

            for (int i = 0; i < Room.CulledThings.Count; i++)
            {
                // Skip if it is the same thing
                if (Room.CulledThings[i] == this)
                    continue;

                if (Room.CulledThings[i] is GameThing thing)
                {
                    if (thing.IsDead || !thing.CollisionDetection)
                        continue;

                    if (Altitude > thing.CollisionHeight)
                        continue;

                    // If thing is an obstacle (walk area hole)
                    if (thing.RuntimeCollider.Contains(Position))
                    {
                        if (thing is IHoleArea holeArea && holeArea.Contains(Position))
                        {
                            OnCollisioning(thing, out var handled);
                            if (!handled)
                            {
                                Position = holeArea.ClampOutside(Position);
                                OnCollision(thing);
                            }
                        }
                    }
                }
            }
        }

        // GetPivotBasedPolyOffset
        private Vector2 GetPivotBasedPolyOffset()
        {
            var result = new Vector2(X, Y);

            if (Sprite.Pivot.AtMiddleX)
                result.X -= BoundingBox.Width / 2;
            else if (Sprite.Pivot.AtRight)
                result.X -= BoundingBox.Width;

            if (Sprite.Pivot.AtMiddleY)
                result.Y -= BoundingBox.Height / 2;
            else if (Sprite.Pivot.AtBottom)
                result.Y -= BoundingBox.Height;

            return result;
        }

        // InvalidateCollisionPolygons
        private void InvalidateCollisionPolygons()
        {
            if (Collider.IsEmpty || !isCollisionDirty)
                return;

            int vertexCount = Collider.Vertices.Count;
            var vertices = new Vector2[vertexCount];

            var offset = ColliderPlacement == PlacementMode.Relative ? GetPivotBasedPolyOffset() : Vector2.Zero;
            Collider.GetVertices(vertices, offset);

            holePoly.SetVertices(vertices);
            RuntimeCollider.SetVertices(vertices, .05f);

            if (ColliderPlacement == PlacementMode.Relative && IsFlippedHorizontally)
            {
                holePoly.FlipHorizontally(X);
                RuntimeCollider.FlipHorizontally(X);
            }

            isCollisionDirty = false;
        }

        // InvalidateWalkArea
        private void InvalidateWalkArea()
        {
            WalkArea = !string.IsNullOrWhiteSpace(WalkAreaName) ? (Room?.WalkAreas.Find(WalkAreaName)) : null;

            shouldClampToWalkablePosition = true;
            isCollisionDirty = true;
        }

        // RefreshDisplayInfo
        private void RefreshDisplayInfo()
        {
            DisplayInfo = GetDisplayInfo();
        }

        // RefreshDisplayName
        private void RefreshDisplayName()
        {
            const string ellipsis = "...";

            DisplayName = GetDisplayName();

            if (Verb == Verb.Ellipsis)
            {
                DisplayName += ellipsis;
                DisplaySentence = DisplayName;
            }
            else if (Verb != Verb.None)
            {
                DisplaySentence = Localization.GetValue(Verb) + " " + DisplayName;
            }
            else
            {
                DisplaySentence = DisplayName;
            }
        }

        #endregion

        #region Protected members

        // CanCheckCollisions
        protected virtual bool CanCheckCollisions()
        {
            return CollisionDetection;
        }

        // DropLoot
        protected void DropLoot()
        {
            if (!PrecalculateLoot)
                PrepareLoot();

            if (Session.Room is not ProceduralRoom room)
                return;

            if (ItemReward != null)
            {
                Prop? loot;
                if (AotTypeRegistry.Find(ItemReward.Name) is AotTypeEntry entry && typeof(PickableLoot).IsAssignableFrom(entry.Type))
                {
                    loot = room.CreateThingClone<Prop>(ItemReward.Name);
                }
                else
                {
                    loot = room.CreateThingClone<Prop>(nameof(Sack));
                }

                if (loot != null)
                {
                    (loot as ILootContainer<ItemDefinition>)?.Loot = ItemReward;
                    loot.Position = Position;
                    room.Children.Add(loot);
                }
            }

            else if (CoinReward > 0)
            {
                for (int i = 0; i < CoinReward; i++)
                {
                    if (room.CreateThingClone<Coin>("Coin") is Coin coin)
                    {
                        coin.Position = Position;

                        // Offset aleatorio para que no caigan apiladas exactamente en el mismo píxel
                        coin.Position += new Vector2(
                            Session.Random.Next(-6, 7),
                            Session.Random.Next(-6, 7)
                        );
                        room.Children.Add(coin);
                    }
                }
            }
        }

        // GetDisplayInfo
        protected virtual string GetDisplayInfo()
        {
            return string.Empty;
        }

        // GetDisplayName
        protected virtual string GetDisplayName()
        {
            return TextRepository.GetValue(DisplayNameKey);
        }

        // GetKnockbackMultiplier
        protected virtual float GetKnockbackMultiplier(GameThing target)
        {
            return 1;
        }

        // GetShakeOffset
        protected Vector2 GetShakeOffset()
        {
            if (hurtShakeTween?.IsRunning == true)
                return hurtShakeTween.CurrentValue;
            else
                return Vector2.Zero;
        }

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();

            if (PrecalculateLoot && Session.Player != this)
            {
                if (ItemReward == null && CoinReward == 0)
                {
                    PrepareLoot();
                    RefreshDisplayName();
                }
            }
        }

        // OnApplyCondition
        protected virtual void OnApplyCondition(ConditionType condition, int amount)
        {
        }

        // OnCollision
        protected virtual void OnCollision(GameThing thing)
        {
        }

        // OnCollisioning
        protected virtual void OnCollisioning(GameThing thing, out bool handled)
        {
            handled = false;
        }

        // OnDeath
        protected virtual void OnDeath()
        {
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (floatingTween != null && floatingTween.IsRunning)
                Altitude += floatingTween.CurrentValue;

            if (HitEffect == HitEffect.Blink && hurtTween != null && hurtTween.IsRunning)
                Altitude += hurtTween.CurrentValue;

            if (hurtShakeTween != null && hurtShakeTween.IsRunning)
                Position += hurtShakeTween.CurrentValue;

            base.OnDraw(gameTime);

            if (floatingTween != null && floatingTween.IsRunning)
                Altitude -= floatingTween.CurrentValue;

            if (HitEffect == HitEffect.Blink && hurtTween != null && hurtTween.IsRunning)
                Altitude -= hurtTween.CurrentValue;

            if (hurtShakeTween != null && hurtShakeTween.IsRunning)
                Position -= hurtShakeTween.CurrentValue;
        }

        // OnDrawLights
        protected virtual void OnDrawLights(GameTime gameTime)
        {
        }

        // OnDrawShadow
        protected virtual void OnDrawShadow(GameTime gameTime)
        {
            shadowSpot.Draw(gameTime);
        }

        // OnFactionChanged
        protected virtual void OnFactionChanged()
        {
        }

        // OnHPChanged
        protected virtual void OnHPChanged(int previousValue)
        {
        }

        // OnIgnoreAttachedLightChanged
        protected virtual void OnIgnoreAttachedLightChanged()
        {
        }

        // OnKnockbackCompleted
        protected virtual void OnKnockbackCompleted()
        {
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            if (shadowSpot.Size > 0 && shadowSpot.AnchorPosition == Vector2.Zero && Sprite.RenderImage != null)
                shadowSpot.AnchorPosition = new Vector2(Sprite.Width / 2, Sprite.Height - .5f);

            blinker.Stop();
            isCollisionDirty = true;
            InvalidateCollisionPolygons();
            InvalidateWalkArea();
        }

        // OnTakeDamage
        protected virtual void OnTakeDamage(GameThing attacker, int amount, DamageType damageType)
        {
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);

            isHotspotDirty = true;
            shouldClampToWalkablePosition = true;

            if (change != TransformChange.Altitude)
            {
                isCollisionDirty = true;
                InvalidateCollisionPolygons();
            }
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            OpacityFactor = 1;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            floatingTween?.Update(gameTime);
            hurtTween?.Update(gameTime);
            hurtShakeTween?.Update(gameTime);
            shadowSpot.Update(gameTime);

            if (knockbackVelocity != Vector2.Zero)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                // 1. Aplicar movimiento
                Position += knockbackVelocity * dt;

                // 2. Aplicar fricción (decaimiento)
                knockbackVelocity *= KnockbackFriction;

                // 3. Limpiar valores residuales muy chicos
                if (knockbackVelocity.LengthSquared() < 100) // Ajustá según tu escala de píxeles
                    knockbackVelocity = Vector2.Zero;

                // Si murió por el golpe, chequear acá si paró para llamar a Die() visualmente
                if (knockbackVelocity == Vector2.Zero)
                {
                    if (IsDead)
                        Die();
                    else
                        OnKnockbackCompleted();
                }
            }
            else if (IsDead && !dieCalled)
            {
                Die();
            }

            base.OnUpdate(gameTime);

            if (shouldClampToWalkablePosition || Session.Player == this)
            {
                ClampToWalkablePosition();
                shouldClampToWalkablePosition = false;
            }

            AttachedLight?.Update(gameTime);

            if (blinker.IsRunning)
                blinker.Update(gameTime);
        }

        // OnUpdateEmittingSound
        protected override void OnUpdateEmittingSound(SoundInstance instance, float masterVolume)
        {
            Utils.ApplySoundEmitter(this, instance, masterVolume);
        }

        // PrepareLoot
        protected void PrepareLoot()
        {
            ItemReward = Session.LootGenerator.RollForLoot(this);
            if (ItemReward == null)
                CoinReward = Session.LootGenerator.RollForCoin(this);
        }

        #endregion

        // AffectsPathfinding
        [ScriptProperty]
        public bool AffectsPathfinding { get; set; } = true;

        // AllowInteraction
        [ScriptProperty]
        public bool AllowInteraction { get; set; } = true;

        // ApproachBehavior
        [ScriptProperty]
        public ApproachBehavior ApproachBehavior { get; set; }

        // ApproachPosition
        [ScriptProperty]
        public Vector2 ApproachPosition { get; set; }

        // AttachedLight
        public Light? AttachedLight { get; set; }

        // AttachedLightPosition
        public Vector2 AttachedLightPosition { get; set; }

        // CanBeHit
        public virtual bool CanBeHit()
        {
            return IsHittable && !IsDead;
        }

        // CanInteract
        public virtual bool CanInteract()
        {
            if (!AllowInteraction)
                return false;

            if (IsDead || string.IsNullOrWhiteSpace(DisplayName))
                return false;

            return true;
        }

        // CanTakeDamage
        public virtual bool CanTakeDamage()
        {
            if (IsDead)
                return false;

            if (Session.Player == this && blinker.IsRunning)
                return false;

            return true;
        }

        // CoinReward
        public int CoinReward
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    RefreshDisplayInfo();
                }
            }
        }

        // Collider
        [ScriptProperty]
        public Polygon Collider
        {
            get;
            set
            {
                field = value;
                isCollisionDirty = true;
                InvalidateCollisionPolygons();
            }
        } = new();

        // ColliderPlacement
        [ScriptProperty]
        public PlacementMode ColliderPlacement
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    isCollisionDirty = true;
                }
            }
        } = PlacementMode.Relative;

        // ClampToWalkablePosition
        public void ClampToWalkablePosition()
        {
            if (IgnoreWalkArea)
                return;

            // Clamp to a walkable position
            if (WalkArea != null)
            {
                Position = WalkArea.ClampInside(Position, out _);

                for (int i = 0; i < WalkArea.Holes.Count; i++)
                {
                    if (WalkArea.Holes[i].Test())
                        Position = WalkArea.Holes[i].ClampOutside(Position);
                }
            }

            if (CanCheckCollisions())
                CheckCollisions();
        }

        // CollisionDetection
        [ScriptProperty]
        public bool CollisionDetection { get; set; } = true;

        // CollisionHeight
        [ScriptProperty]
        public int CollisionHeight { get; set; }

        // Cursor
        [ScriptProperty]
        public MouseCursorState Cursor { get; set; } = MouseCursorState.Cross;

        // CustomDropName
        [ScriptProperty]
        public string CustomDropName { get; set; } = string.Empty;

        // Die
        [ScriptMethod]
        public void Die()
        {
            if (dieCalled)
                return;

            dieCalled = true;

            HP = int.MinValue;

            if (DeathSound?.PopInstance() is SoundInstance deathSoundInstance)
            {
                Utils.ApplySoundEmitter(this, deathSoundInstance, deathSoundInstance.GetEffectiveVolume());
                deathSoundInstance.Play();
            }

            OnDeath();

            if (FaithReward > 0)
                Session.ObjectPools.FloatingTexts.Get()?.Show(GetOverheadPosition(), $"+{FaithReward} {Localization.GetValue(StatName.Faith)}", ColorPalette.Text.Highlight, ScaleInfo.Text.VeryLarge.X, 2000);

            DropLoot();
        }

#if DEBUG
        // DrawBox
        private static void DrawBox(RectangleF bounds, Color color)
        {
            EngendroGame.Instance.Shapes.DrawRectangle(bounds, color);
        }

        // DrawDebugBoxes
        public void DrawDebugBoxes()
        {
            if (ShowColliders)
                DrawBox(holePoly.BoundingRectangleF, Color.Red * .2f);

            if (ShowHotspots)
                DrawBox(RuntimeHotspot.BoundingRectangleF, Color.Purple * .2f);

            if (ShowBoundingBoxes)
                Game.Shapes.DrawFrame(BoundingBox, Color.Yellow * .2f, .2f);

            if (RoomEditor.SelectedThing == this)
                Game.Shapes.DrawFrame(BoundingBox, Color.Green * .6f, .5f);
        }

        // ShowBoundingBoxes
        public static bool ShowBoundingBoxes { get; set; }

        // ShowColliders
        public static bool ShowColliders { get; set; }

        // ShowHotspots
        public static bool ShowHotspots { get; set; }
#endif

        // DeathWord
        [ScriptProperty]
        public ComicTextKind DeathWord { get; set; }

        // DeathSound
        [ScriptProperty]
        public Sound? DeathSound { get; set; }

        // DisplayInfo
        public string DisplayInfo { get; private set; } = string.Empty;

        // DisplayName
        public string DisplayName { get; private set; } = string.Empty;

        // DisplayNameKey
        [ScriptProperty]
        public string DisplayNameKey
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    RefreshDisplayName();
                }
            }
        } = string.Empty;

        // DisplaySentence
        public string DisplaySentence { get; private set; } = string.Empty;

        // DrawLights
        public void DrawLights(GameTime gameTime)
        {
            if (!IsEmittingLight)
                return;

            if (AttachedLight != null)
            {
                if (AttachedLightPosition != Vector2.Zero)
                    AttachedLight.Position = this.GetAnchoredPosition(AttachedLightPosition);
                AttachedLight.Draw(gameTime);
            }

            OnDrawLights(gameTime);
        }

        // DrawShadow
        public void DrawShadow(GameTime gameTime)
        {
            if (!IsDead)
                OnDrawShadow(gameTime);
        }

        // FaceTo
        public void FaceTo(GameThing target)
        {
            if (target != this)
                FaceTo(target.Position);
        }

        // FaceTo
        public void FaceTo(Vector2 position)
        {
            if (X < position.X)
                Direction = FacingDirection.Right;
            else
                Direction = FacingDirection.Left;
        }

        // Faction
        [ScriptProperty]
        public Faction Faction
        {
            get;
            set
            {
                field = value;
                if (field == Faction.Evil)
                    IsHostile = true;
            }
        }

        // FaithReward
        [ScriptProperty]
        public int FaithReward
        {
            get;
            set
            {
                if (value != field)
                {
                    field = Math.Max(0, value);
                    RefreshDisplayInfo();
                }
            }
        }

        // FloatingForce
        [ScriptProperty]
        public float FloatingForce
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    if (field > 0)
                    {
                        floatingTween ??= new FloatTween();
                        floatingTween.Start(TweenStyle.CubicInOut, 0, field, 200, -1);
                    }
                    else
                    {
                        floatingTween?.Stop();
                    }
                }
            }
        }

        // GetApproachPosition
        public Vector2 GetApproachPosition(GameThing requester, ApproachBehavior? behavior = null)
        {
            behavior ??= this.ApproachBehavior;

            // 1. Override Manual (Prioridad absoluta del editor)
            if (ApproachPosition != Vector2.Zero)
            {
                if (HotspotPlacement == PlacementMode.Absolute)
                    return ApproachPosition;
                else
                    return this.GetAnchoredPosition(ApproachPosition);
            }

            // Datos básicos
            var myBox = RuntimeHotspot.BoundingRectangleF;
            if (myBox.IsEmpty)
                myBox = BoundingBox;

            var requesterBox = requester.RuntimeHotspot.BoundingRectangleF;
            if (requesterBox.IsEmpty)
                requesterBox = requester.BoundingBox;

            // Distancia de "respeto" (Spacing)
            // Si soy un Actor (NPC), dejo espacio para conversar. Si soy un objeto, te puedes pegar más.
            float spacing = (this is Actor) ? requesterBox.Width : requesterBox.Width / 2;

            float targetX = X;
            float targetY = BoundingBox.Bottom + Altitude;

            switch (behavior)
            {
                case ApproachBehavior.FaceToFace:
                    // "Párate frente a mi cara"
                    if (Direction == FacingDirection.Left)
                        targetX = myBox.Left - spacing;  // Estoy mirando izq -> ven a mi izq
                    else
                        targetX = myBox.Right + spacing; // Estoy mirando der -> ven a mi der
                    break;

                case ApproachBehavior.ClosestSide:
                    // "Párate en mi flanco más cercano a ti"
                    if (requester.X < X)
                        targetX = myBox.Left - (requesterBox.Width / 2);
                    else
                        targetX = myBox.Right + (requesterBox.Width / 2);
                    break;

                case ApproachBehavior.InFront:
                    // "Párate en mi centro X"
                    targetX = myBox.Center.X;
                    break;

                case ApproachBehavior.Over:
                    targetX = myBox.Center.X;
                    targetY = myBox.Center.Y;
                    break;
            }

            // Mantenemos la Y en la base del objeto (los pies)
            return new Vector2(targetX, targetY);
        }

        // GetFootstepSound
        public Sound? GetFootstepSound(Vector2 position)
        {
            return RuntimeCollider?.Contains(position) == true ? TerrainSound : null;
        }

        // GetOverheadPosition
        public Vector2 GetOverheadPosition()
        {
            // Origin
            if (OverheadOrigin == Vector2.Zero)
                return BoundingBox.GetPoint(RectanglePoint.Top);
            else
                return this.GetAnchoredPosition(OverheadOrigin);
        }

        // GetResistanceModifier
        public float GetResistanceModifier(DamageType damageType)
        {
            if (ResistanceTable.Find(ResistanceTableName) is ResistanceTable table)
                return table.GetModifier(damageType);

            return 1;
        }

        // HitTest
        public bool HitTest(Vector2 value)
        {
            if (HitTestPolygon == TestPolygon.Hotspot)
                return RuntimeHotspot.Contains(value);
            else
                return (this as IHoleArea).Contains(value);
        }

        // HitEffect
        [ScriptProperty]
        public HitEffect HitEffect { get; init; } = HitEffect.Shake;

        // HitTestPolygon
        [ScriptProperty]
        public TestPolygon HitTestPolygon { get; set; }

        // Hotspot
        [ScriptProperty]
        public Polygon Hotspot { get; set; } = new();

        // HotspotPlacement
        [ScriptProperty]
        public PlacementMode HotspotPlacement
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    isHotspotDirty = true;
                }
            }
        } = PlacementMode.Relative;

        // HP
        [ScriptProperty]
        public int HP
        {
            get;
            set
            {
                if (value != field)
                {
                    var previousValue = field;

                    field = Math.Min(value, MaxHP);

                    if (field > 0)
                        dieCalled = false;

                    OnHPChanged(previousValue);
                }
            }
        }

        // HPRatio
        [ScriptProperty]
        public float HPRatio => (float)HP / MaxHP;

        // HurtSound
        [ScriptProperty]
        public Sound? HurtSound { get; set; }

        // IgnoreAttachedLight
        [ScriptProperty]
        public bool IgnoreAttachedLight
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnIgnoreAttachedLightChanged();
                }
            }
        }

        // IgnoreKnockback
        [ScriptProperty]
        public bool IgnoreKnockback { get; set; }

        // IgnoreWalkArea
        [ScriptProperty]
        public bool IgnoreWalkArea { get; set; } = true;

        // IsBehind
        public bool IsBehind(GameThing thing)
        {
            if (thing.Direction == FacingDirection.Right && X < thing.X)
                return true;

            else if (thing.Direction == FacingDirection.Left && X > thing.X)
                return true;

            else
                return false;
        }

        // IsBlinking
        public bool IsBlinking => blinker.IsRunning && blinker.CurrentValue;

        // IsCornered
        public bool IsCornered(GameThing target)
        {
            if (Room?.WalkArea == null)
                return false;

            var destX = Direction == FacingDirection.Left ? int.MaxValue : int.MinValue;
            var destination = Room.WalkArea.ClampInside(new(destX, Y));

            float distanceToTarget;
            if (target.X < X)
            {
                distanceToTarget = Vector2.Distance(target.RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.RightBottom), RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.LeftBottom));
            }
            else
            {
                distanceToTarget = Vector2.Distance(target.RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.LeftBottom), RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.RightBottom));
            }

            float distanceToWall;
            if (Direction == FacingDirection.Left)
                distanceToWall = Vector2.Distance(RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.RightBottom), destination);
            else
                distanceToWall = Vector2.Distance(RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.LeftBottom), destination);

            return distanceToWall < 40 && distanceToTarget < 20;
        }

        // IsDead
        public bool IsDead => (HP <= 0 && MaxHP > 0) || (HP == int.MinValue);

        // IsEmittingLight
        public virtual bool IsEmittingLight => AttachedLight?.IsEmitting == true && !IgnoreAttachedLight;

        // IsFacingTarget
        public bool IsFacingTarget(GameThing target)
        {
            float directionToTarget = target.Position.X - this.Position.X;
            if (directionToTarget == 0)
                return true;

            return (directionToTarget > 0 && this.Direction == FacingDirection.Right) ||
                   (directionToTarget < 0 && this.Direction == FacingDirection.Left);
        }

        // IsHittable
        [ScriptProperty]
        public bool IsHittable { get; set; } = true;

        // IsHostile
        [ScriptProperty]
        public bool IsHostile { get; set; }

        // IsMouseOver
        public bool IsMouseOver()
        {
            if (InputManager.DefaultPlayer.LastInputMethod != InputMethod.Mouse)
                return false;

            if (RuntimeHotspot.IsEmpty)
                return BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.WorldPosition(Session.Camera));
            else
                return RuntimeHotspot.Contains(InputManager.DefaultPlayer.Mouse.WorldPosition(Session.Camera));
        }

        // IsKnockbackInProgress
        public bool IsKnockbackInProgress => knockbackVelocity != Vector2.Zero;

        // MaxHP
        [ScriptProperty]
        public int MaxHP
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    if (HP == 0)
                    {
                        HP = value;
                    }
                    else if (HP > field)
                    {
                        HP = field;
                    }
                }
            }
        }

        // OverheadOrigin
        [ScriptProperty]
        public Vector2 OverheadOrigin { get; set; }

        // PrecalculateLoot
        [ScriptProperty]
        public bool PrecalculateLoot { get; set; }

        // Reheal
        [ScriptMethod]
        public virtual void Reheal()
        {
            HP = MaxHP;
        }

        // RenderLayer
        [ScriptProperty]
        public RenderLayer RenderLayer
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    renderLayerDepth = (int)field;
                }
            }
        }

        // RenderLayerDepth
        public override int RenderLayerDepth => renderLayerDepth;

        // ResistanceTableName
        [ScriptProperty]
        public string ResistanceTableName { get; set; }

        // Room
        public new GameRoom? Room => Parent as GameRoom;

        // RuntimeCollider
        public Polygon RuntimeCollider { get; } = new();

        // RuntimeHotspot
        public Polygon RuntimeHotspot
        {
            get
            {
                if (HotspotPlacement == PlacementMode.Absolute)
                {
                    isHotspotDirty = false;
                    return Hotspot;
                }

                if (isHotspotDirty)
                {
                    if (Hotspot.IsEmpty)
                    {
                        field.Clear();
                    }
                    else if (HotspotPlacement == PlacementMode.Relative)
                    {
                        var offset = GetPivotBasedPolyOffset();
                        offset.Y -= Altitude;

                        var vertices = new Vector2[Hotspot.Vertices.Count];
                        Hotspot.GetVertices(vertices, offset);
                        field.SetVertices(vertices);
                        if (IsFlippedHorizontally)
                            field.FlipHorizontally(X);
                    }

                    isHotspotDirty = false;
                }

                return field;
            }
        } = new();

        // ItemReward
        public ItemDefinition? ItemReward
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    RefreshDisplayInfo();
                }
            }
        }

        // Session
        public new GameSession Session { get; }

        // ShadowSpotSize
        [ScriptProperty]
        public int ShadowSpotSize
        {
            get => shadowSpot.Size;
            set => shadowSpot.Size = value;
        }

        // ShowComicText
        public void ShowComicText(ComicTextKind kind)
        {
            var pos = RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.Top, 0, 3);
            Session.ComicTextPool.Get()?.Show(kind, pos);
        }

        // ShowFloatingText
        public void ShowFloatingText(string text, Color color, int duration = 1000)
        {
            if (Session.ObjectPools.FloatingTexts.Get() is FloatingText floatingText)
                floatingText.Show(GetOverheadPosition(), text, color, duration);
        }

        // TakeDamage
        public int TakeDamage(GameThing attacker, DamageType damageType, int amount, ComicTextKind comicTextKind, Vector2 knockbackForce)
        {
            // Si la cantidad es 0 o negativa, no hay interacción de daño.
            if (!CanTakeDamage())
                return 0;

            // ---------------------------------------------------------
            // 1. FEEDBACK INICIAL
            // ---------------------------------------------------------
            // Esto ocurre SIEMPRE que hay un impacto válido, aunque sea indestructible.

            // Sonido de dolor o impacto
            if (HurtSound != null)
                PlaySound(HurtSound);

            // Shake: El objeto tiembla por el golpe (incluso una pared dura puede vibrar)
            if (HitEffect == HitEffect.Shake)
            {
                hurtShakeTween ??= new();
                hurtShakeTween.Start(TweenStyle.Linear, Vector2.Zero, hurtShakeForce, 40, 4);
            }

            // ---------------------------------------------------------
            // 2. LÓGICA DE SALUD (Solo si es Destructible)
            // ---------------------------------------------------------

            // Aquí es donde manejamos el MaxHP == 0
            if (MaxHP > 0)
            {
                // Aplicar resistencias
                amount = (int)(amount * GetResistanceModifier(damageType));

                // Clamp para no restar más de lo que tiene
                if (amount > HP)
                    amount = HP;

                // Si después de la resistencia el daño es 0, salimos de la lógica de HP
                //if (amount > 0)
                {
                    HP -= amount;

                    if (IsDead)
                    {
                        knockbackForce = Vector2.Zero;
                    }
                    else
                    {
                        hurtTween ??= new();
                        hurtTween.Start(TweenStyle.Linear, 0, 1, 150, 2);

                        // Invulnerabilidad post-daño
                        if (HitEffect == HitEffect.Blink)
                            blinker.Start(40, 15);
                        else
                            blinker.Stop();
                    }

                    OnTakeDamage(attacker, amount, damageType);

                    if (!IsDead)
                        Session.ObjectPools.FloatingTexts.Get()?.ShowAmount(this, ColorPalette.Text.Highlight, amount);

                    // ComicText si hubo daño real
                    if (comicTextKind != ComicTextKind.None)
                    {
                        if (!IsDead || DeathWord == ComicTextKind.None)
                            ShowComicText(comicTextKind);
                    }
                }
            }
            else
            {
                // Lógica para Indestructibles (MaxHP == 0)
                // Opcional: Sonido de "Metal/Rebote" o palabra "BLOCK"
            }

            // ---------------------------------------------------------
            // 3. FÍSICAS (Knockback)
            // ---------------------------------------------------------
            // El empuje se aplica independientemente de la vida. 
            // Una caja de metal indestructible (MaxHP=0) debería poder ser empujada.
            if (MaxHP > 0 && knockbackForce != Vector2.Zero && !IgnoreKnockback)
            {
                knockbackForce *= attacker.GetKnockbackMultiplier(this);

                Vector2 pushDirection = Position - attacker.Position;
                if (pushDirection != Vector2.Zero)
                {
                    pushDirection.Normalize();
                }
                else
                {
                    pushDirection = new Vector2(1, 0);
                }

                // Aplicamos la fuerza
                knockbackVelocity = pushDirection * knockbackForce.Length() * 5f;
            }

            return amount;
        }

        // TerrainParticleColor
        [ScriptProperty]
        public Color TerrainParticleColor { get; set; }

        // TerrainSound
        [ScriptProperty]
        public Sound? TerrainSound { get; set; }

        // Verb
        [ScriptProperty]
        public Verb Verb
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    RefreshDisplayName();
                }
            }
        }

        // WalkArea
        public WalkArea? WalkArea { get => field ?? Room?.WalkArea; private set; }

        // WalkAreaName
        [ScriptProperty]
        public string WalkAreaName
        {
            get; set
            {
                if (value != field)
                {
                    field = value;
                    InvalidateWalkArea();
                }
            }
        } = string.Empty;
    }
}