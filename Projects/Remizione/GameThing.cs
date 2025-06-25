using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Engendro.PathFinding;
using EngendroAdventure;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace Remizione
{
    /// <summary>
    /// GameThing
    /// </summary>
    public abstract class GameThing : Thing, IHoleArea, ILightSource
    {
        #region Private fields

        private bool applyDamagePending;
        private Meter? damageMeter;
        private int damageMeterCooldown;
        private string displayName = string.Empty;
        private readonly Polygon holeInflatedPoly = new();
        private readonly Polygon holePoly = new();
        private Polygon hotspotPoly = new();
        private PlacementMode hotspotPlacement = PlacementMode.Relative;
        private int hp;
        private RectangleF hurtBox;
        private Vector2Tween? hurtShakeTween;
        private FloatTween? hurtTween;
        private float floatingForce;
        private FloatTween? floatingTween;
        private HitType hitType;
        private ImpactWord? impactWord;
        private ImpactWordKind impactWordKind;
        private bool isHoleAreaDirty;
        private bool isHotspotDirty = true;
        private bool isHurtBoxDirty = true;
        private Vector2 knockback;
        private readonly Vector2Tween knockbackTween = new();
        private string localizedDisplayName = string.Empty;
        private int maxHP;
        private PathNode[]? pathNodes;
        private readonly List<PlacementCondition> placementConditions = [];
        private RenderLayer renderLayer;
        private int renderLayerDepth;
        private bool shouldClampToWalkablePosition;
        private WalkArea? walkArea;
        private string walkAreaName = string.Empty;

        #endregion

        #region Constructor

        // Constructor
        protected GameThing(GameSession session, string name)
            : base(session, name)
        {
            this.RenderLayer = RenderLayer.Default;
            this.Session = session;
            this.PlacementConditions = new(placementConditions);
            this.Inventory = new ItemContainer(this, Localization.GetValue(InGameMenuOptionName.Inventory));
        }

        #endregion

        #region IHoleArea explicit implementation

        // ClampOutside
        Vector2 IHoleArea.ClampOutside(Vector2 position)
        {
            if (Collider != null)
            {
                InvalidateHoleArea();
                if (holePoly.Contains(position))
                    position = holeInflatedPoly.GetClosestPointOnEdge(position);
            }

            return position;
        }

        // CollectPathNodes
        void IHoleArea.CollectPathNodes(IList<PathNode> targetList)
        {
            InvalidateHoleArea();

            if (Collider == null || Collider.Vertices.Count == 0)
                return;

            if (pathNodes == null || pathNodes.Length != Collider.Vertices.Count)
                pathNodes = new PathNode[Collider.Vertices.Count];

            for (int i = 0; i < holeInflatedPoly.Vertices.Count; i++)
            {
                // Is point concave?
                if (holeInflatedPoly.IsVertexConcave(i))
                    continue;

                // Is point outside walk area
                if (WalkArea != null && !WalkArea.Contains(holeInflatedPoly.Vertices[i]))
                    continue;

                if (pathNodes[i] == null)
                    pathNodes[i] = new(holeInflatedPoly.Vertices[i]);
                else
                    pathNodes[i].Position = holeInflatedPoly.Vertices[i];

                targetList.Add(pathNodes[i]);
            }
        }

        // Contains
        bool IHoleArea.Contains(Vector2 point)
        {
            InvalidateHoleArea();
            return holePoly.Contains(point);
        }

        // InLineOfSight
        bool IHoleArea.InLineOfSight(Vector2 start, Vector2 end)
        {
            InvalidateHoleArea();
            return holePoly.InLineOfSight(start, end);
        }

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
                if (Room.CulledThings[i] == this || Room.CulledThings[i].IsMoving)
                    continue;

                if (Room.CulledThings[i] is GameThing thing)
                {
                    if (thing.IsDead)
                        continue;

                    // If thing is an obstacle (walk area hole)
                    if (thing.Collider != null)
                    {
                        if (thing is IHoleArea holeArea && holeArea.Contains(Position))
                            Position = holeArea.ClampOutside(Position);
                    }
                }
            }
        }

        // Die
        private void Die()
        {
            damageMeterCooldown = 0;

            if (DeathSound != null)
                PlaySound(DeathSound);

            OnDeath();

            if (Session.Player != null && Session.Player != this)
                Session.Player.Grace += Grace;

            if (LootTable.Find(StaticName) is LootTable lootTable)
            {
                if (Session.ObjectPools.LootBags.Get() is LootBag lootBag)
                {
                    var itemName = lootTable.GetLoot();
                    lootBag.Drop(Position, itemName);
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

        // InvalidateDamageMeter
        private void InvalidateDamageMeter()
        {
            if (damageMeter != null)
                damageMeter.Value = HP * 100 / MaxHP / damageMeter.MaximumValue;
        }

        // InvalidateHoleArea
        private void InvalidateHoleArea()
        {
            if (!isHoleAreaDirty || Collider == null)
                return;

            int vertexCount = Collider.Vertices.Count;
            var vertices = new Vector2[vertexCount];

            var offset = GetPivotBasedPolyOffset();
            Collider.GetVertices(vertices, offset);
            holePoly.SetVertices(vertices);
            holeInflatedPoly.SetVertices(vertices, .05f);

            isHoleAreaDirty = false;
        }

        // InvalidateWalkArea
        private void InvalidateWalkArea()
        {
            if (!string.IsNullOrWhiteSpace(walkAreaName))
                walkArea = Room?.WalkAreas.Find(walkAreaName);
            else
                walkArea = null;

            shouldClampToWalkablePosition = true;

            MarkHoleAreaDirty();
        }

        // MarkHoleAreaDirty
        private void MarkHoleAreaDirty()
        {
            if (IsWalkAreaHole)
                isHoleAreaDirty = true;
        }

        // ResetApplyDamageValues
        private void ResetApplyDamageValues()
        {
            applyDamagePending = false;
            CumulativeDamage = 0;
            impactWordKind = ImpactWordKind.None;
            knockback = Vector2.Zero;
        }

        #endregion

        #region Protected members

        // CanCheckCollisions
        protected virtual bool CanCheckCollisions() => CollisionDetection;

        // OnDamageReaction
        protected virtual void OnDamageReaction(GameThing attacker)
        {
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

            if (hurtTween != null && hurtTween.IsRunning)
                Altitude += hurtTween.CurrentValue;

            if (hurtShakeTween != null && hurtShakeTween.IsRunning)
                Position += hurtShakeTween.CurrentValue;

            base.OnDraw(gameTime);

            if (floatingTween != null && floatingTween.IsRunning)
                Altitude -= floatingTween.CurrentValue;

            if (hurtTween != null && hurtTween.IsRunning)
                Altitude -= hurtTween.CurrentValue;

            if (hurtShakeTween != null && hurtShakeTween.IsRunning)
                Position -= hurtShakeTween.CurrentValue;
        }

        // OnDrawReflection
        protected virtual void OnDrawReflection(GameTime gameTime)
        {
        }

        // OnDrawLights
        protected virtual void OnDrawLights(GameTime gameTime, List<Light> renderedLights)
        {
        }

        // OnDrawShadow
        protected virtual void OnDrawShadow(GameTime gameTime)
        {
        }

        // OnHPChanged
        protected virtual void OnHPChanged()
        {
        }

        // OnHurt
        protected virtual void OnHurt(GameThing attacker)
        {
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            InvalidateHoleArea();
            InvalidateWalkArea();
        }

        // OnParentChanged
        protected override void OnParentChanged(Entity? previousParent)
        {
            if (!Session.IsInitializing && WorldBlockOrigin != null)
                StateID = -1;
        }

        // OnRead
        protected override void OnRead(XmlAttributeCollection attributes)
        {
            // Inventory
            if (attributes[nameof(Inventory)]?.Value is string inventoryData)
                Inventory.SetSerializationData(inventoryData);
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);

            if (hurtShakeTween == null || !hurtShakeTween.IsRunning)
            {
                isHotspotDirty = true;
                isHurtBoxDirty = true;
                shouldClampToWalkablePosition = true;

                if (change != TransformChange.Altitude)
                    MarkHoleAreaDirty();
            }
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();

            ResetApplyDamageValues();

            if (impactWord != null)
            {
                Session.ImpactWordPool.Return(impactWord);
                impactWord = null;
            }

            OpacityFactor = 1;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            floatingTween?.Update(gameTime);
            hurtTween?.Update(gameTime);
            hurtShakeTween?.Update(gameTime);

            if (knockbackTween.IsRunning)
            {
                knockbackTween.Update(gameTime);
                Position = knockbackTween.CurrentValue;
                if (!knockbackTween.IsRunning && IsDead)
                    Die();
            }

            if (damageMeterCooldown > 0)
            {
                damageMeterCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                damageMeter?.Update(gameTime);
            }

            base.OnUpdate(gameTime);

            if (impactWord != null)
            {
                impactWord.Update(gameTime);
                if (!impactWord.IsActive)
                {
                    Session.ImpactWordPool.Return(impactWord);
                    impactWord = null;
                }
            }

            if (shouldClampToWalkablePosition)
            {
                ClampToWalkablePosition();
                shouldClampToWalkablePosition = false;
            }

            Light?.Update(gameTime);
        }

        // OnUpdateEmittingSound
        protected override void OnUpdateEmittingSound(SoundInstance instance, float masterVolume)
        {
            const int margin = 50;

            RectangleF visible = Session.Camera.VisibleBox;
            //var centerX = visible.Center.X;

            // Expandimos los límites visibles con el margen extra
            float leftLimit = visible.Left - margin;
            float rightLimit = visible.Right + margin;

            // Cálculo del Pan (-1 izquierda, 0 centro, 1 derecha)
            float pan = MathHelper.Clamp(MathHelper.Lerp(-1f, 1f, (X - leftLimit) / (rightLimit - leftLimit)), -1f, 1f);

            // Cálculo del Volumen (1 dentro del VisibleBox, 0 fuera del margen extendido)
            float volume;
            if (visible.Contains(Position))
            {
                volume = 1f;
            }
            else if (X < leftLimit || X > rightLimit)
            {
                volume = 0f;
            }
            else
            {
                float distanceToEdge = Math.Min(Math.Abs(X - visible.Left), Math.Abs(X - visible.Right));
                volume = MathHelper.Clamp(distanceToEdge / margin, 0f, 1f);
            }

            instance.Pan = pan;
            instance.Volume.Current = volume * masterVolume;
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            output.WriteAttributeString(nameof(Inventory), Inventory.GetSerializationData());
        }

        #endregion

        // AddPlacementCondition
        public void AddPlacementCondition(PlacementCondition condition)
        {
            placementConditions.Add(condition);
        }

        // ApplyDamage
        public void ApplyDamage(GameThing attacker)
        {
            if (!applyDamagePending || IsDead || CumulativeDamage <= 0)
            {
                ResetApplyDamageValues();
                return;
            }

            if (HurtSound != null)
                PlaySound(HurtSound);

            if (HurtImpactSound != null)
                PlaySound(HurtImpactSound);

            OnDamageReaction(attacker);

            // Impact word
            if (HurtImpactSound != null && impactWordKind != ImpactWordKind.None && Collider != null)
            {
                impactWord ??= Session.ImpactWordPool.Get();
                impactWord.Show(impactWordKind, this.GetAbsolutePoint(Collider.BoundingRectangleF.GetPoint(RectanglePoint.Top)));
            }

            if (maxHP == 0 && (HurtSound != null || HurtImpactSound != null))
            {
                hurtShakeTween ??= new();
                hurtShakeTween.Start(TweenStyle.Linear, Vector2.Zero, HurtShake, 40, 4);
                return;
            }

            HP -= (int)CumulativeDamage;

            var damageTextColor = hitType == HitType.Critical ? ColorPalette.TextDepracated.Dark : ColorPalette.Text.Default;
            var damageText = $"{(int)CumulativeDamage}";
            if (hitType == HitType.Critical)
                damageText += " " + TextRepository.GetValue("HitType.Critical");

            Session.ObjectPools.FloatingTexts.Get()?.ShowAsDamage(GetFloatingTextPosition(knockback), damageText, damageTextColor);

            if (MaxHP > 0)
            {
                damageMeterCooldown = 1500;
                if (damageMeter == null)
                {
                    damageMeter = new(Game, ColorPalette.HPMeter.Back, ColorPalette.HPMeter.Fore) { MaximumValue = 10 };
                    InvalidateDamageMeter();
                }
            }

            if (knockback == Vector2.Zero && HP <= 0)
            {
                Die();
            }
            else if (MaxHP > 0)
            {
                var destination = Position;
                destination.Y += knockback.Y;

                if (X > attacker.X)
                    destination.X += knockback.X;
                else
                    destination.X -= knockback.X;

                knockbackTween.Start(TweenStyle.CubicOut, Position, destination, 400, 0);

                hurtTween ??= new();
                hurtTween.Start(TweenStyle.Linear, 0, 1, 150, 2);

                OnHurt(attacker);
            }

            ResetApplyDamageValues();
        }

        // ApproachPosition
        [ScriptProperty]
        public Vector2 ApproachPosition { get; set; }

        // AreHurtBoxesVisuallyOverlapping
        public bool AreHurtBoxesVisuallyOverlapping(GameThing otherThing)
        {
            float diff = Math.Abs(Y - otherThing.Y);
            if (diff > 3)
                return false;

            return HurtBox.Intersects(otherThing.HurtBox);
        }

        // CanBeTargeted
        public bool CanBeTargeted => !IsMoving && MaxHP > 0 && !IsDead;

        // CanInteract
        public bool CanInteract(Actor requester)
        {
            if (requester == this)
                return false;

            if (string.IsNullOrWhiteSpace(DisplayName))
                return false;

            return RuntimeHotspot.Contains(requester.GetAbsolutePoint(requester.HotspotDetectorPosition));
        }

        // CellMargin
        [ScriptProperty]
        public int CellMargin { get; set; }

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

        // Collider
        [ScriptProperty]
        public Polygon? Collider { get; set; }

        // CumulativeDamage
        public float CumulativeDamage { get; set; }

#if DEBUG
        // DrawBox
        private static void DrawBox(EngendroGame game, RectangleF bounds, Color color)
        {
            game.Shapes.DrawRectangle(bounds, color);
        }

        // DrawDebugBoxes
        public void DrawDebugBoxes()
        {
            //if (ShowHotspotBoxes)
              //  DrawBox(Game, HotspotBox, Color.Purple * .2f);

            if (ShowHurtBoxes)
                DrawBox(Game, HurtBox, Color.Red * .2f);
        }

        // ShowHotspotBoxes
        public static bool ShowHotspotBoxes { get; set; }

        // ShowHurtBoxes
        public static bool ShowHurtBoxes { get; set; }
#endif

        // DeathSound
        [ScriptProperty]
        public Sound? DeathSound { get; set; }

        // DisplayName
        [ScriptProperty]
        public string DisplayName
        {
            get => displayName;
            set
            {
                if (value != displayName)
                {
                    displayName = value;
                    localizedDisplayName = TextRepository.GetValue(DisplayName);
                }
            }
        }

        // DistributionStrategy
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public PlacementDistributionStrategy DistributionStrategy { get; set; }

        // DrawDamagerMeter
        public void DrawDamagerMeter(GameTime gameTime)
        {
            if (damageMeterCooldown > 0 && damageMeter != null)
            {
                damageMeter.Position = GetOverheadPosition(-5, -3);
                damageMeter.Draw(gameTime);
            }
        }

        // DrawImpactWord
        public void DrawImpactWord(GameTime gameTime) => impactWord?.Draw(gameTime);

        // DrawLights
        public void DrawLights(GameTime gameTime, List<Light> renderedLights)
        {
            if (Light != null)
            {
                Light.Position = this.GetAbsolutePoint(LightPosition);
                Light.Draw(gameTime);
                renderedLights.Add(Light);
            }

            OnDrawLights(gameTime, renderedLights);
        }

        // DrawReflection
        public void DrawReflection(GameTime gameTime)
        {
            OnDrawReflection(gameTime);
        }

        // DrawShadow
        public void DrawShadow(GameTime gameTime)
        {
            if (!IsDead)
                OnDrawShadow(gameTime);
        }

        // FaceTo
        public void FaceTo(GameThing thing)
        {
            if (X < thing.X)
                Direction = FacingDirection.Right;
            else
                Direction = FacingDirection.Left;
        }

        // FloatingForce
        [ScriptProperty]
        public float FloatingForce
        {
            get => floatingForce;
            set
            {
                if (value != floatingForce)
                {
                    floatingForce = value;

                    if (floatingForce > 0)
                    {
                        floatingTween ??= new FloatTween();
                        floatingTween.Start(TweenStyle.CubicInOut, 0, floatingForce, 200, -1);
                    }
                    else
                        floatingTween?.Stop();
                }
            }
        }

        // GetApproachPosition
        public Vector2 GetApproachPosition(GameThing requester, bool inFront)
        {
            if (ApproachPosition != Vector2.Zero)
                return this.GetAbsolutePoint(ApproachPosition);

            var box = RuntimeHotspot.BoundingRectangleF;
            if (box.IsEmpty)
                box = BoundingBox;

            var requesterBox = requester.RuntimeHotspot.BoundingRectangleF;
            if (requesterBox.IsEmpty)
                requesterBox = requester.BoundingBox;

            Vector2 result;

            if (inFront)
            {
                var offset = this is Actor ? requesterBox.Width + 3 : requesterBox.Width / 2;
                if (Direction == FacingDirection.Left)
                    result = box.GetPoint(RectanglePoint.LeftBottom, -offset, 0);
                else
                    result = box.GetPoint(RectanglePoint.RightBottom, offset, 0);
            }
            else
            {
                // Doesn't matter the enemy facing direction
                if (requester.X <= X)
                    result = box.GetPoint(RectanglePoint.LeftBottom, -requesterBox.Width / 2, 0);
                else
                    result = box.GetPoint(RectanglePoint.RightBottom, requesterBox.Width / 2, 0);
            }

            result.Y = BoundingBox.Bottom + Altitude;

            return result;
        }

        // GetFloatingTextPosition
        public Vector2 GetFloatingTextPosition(Vector2 knockback) => GetFloatingTextPosition(knockback, 0, 0);

        // GetFloatingTextPosition
        public Vector2 GetFloatingTextPosition(Vector2 knockback, int xOffset, int yOffset)
        {
            var result = GetOverheadPosition(0, -3);

            if (Direction == FacingDirection.Right)
                result.X -= Math.Abs(knockback.X);
            else
                result.X += Math.Abs(knockback.X);

            result.X += xOffset;
            result.Y += yOffset;

            return result;
        }

        // GetFrameSubArea
        public RectangleF GetFrameSubArea()
        {
            if (AnimationPlayer.Frame != null)
                return this.GetAbsoluteBounds(AnimationPlayer.Frame.SubArea);
            else
                return RectangleF.Empty;
        }

        // GetOverheadPosition
        public Vector2 GetOverheadPosition() => GetOverheadPosition(0, 0);

        // GetOverheadPosition
        public Vector2 GetOverheadPosition(int xOffset, int yOffset)
        {
            // Origin
            if (OverheadOrigin == Vector2.Zero)
                return BoundingBox.GetPoint(RectanglePoint.Top, xOffset, yOffset);
            else
                return this.GetAbsolutePoint(OverheadOrigin, xOffset, yOffset);
        }

        // GetRequiredGridSpace
        public Size GetRequiredGridSpace(int cellSize)
        {
            RectangleF bbox;

            if (Collider == null)
                bbox = BoundingBox;
            else
                bbox = Collider.BoundingRectangleF;

            int width = (int)Math.Ceiling(bbox.Width / cellSize) + CellMargin * 2;
            int height = (int)Math.Ceiling(bbox.Height / cellSize) + CellMargin * 2;

            return new Size(width, height);
        }

        // GetThrowableSpawnPosition
        public Vector2 GetThrowableSpawnPosition() => this.GetAbsolutePoint(ThrowableSpawnPosition);

        // Grace
        [ScriptProperty]
        public int Grace { get; set; }

        // Highlight
        [ScriptProperty]
        public bool Highlight { get; set; } = true;

        // Hotspot
        [ScriptProperty]
        public Polygon Hotspot { get; set; } = new();

        // HotspotPlacement
        [ScriptProperty]
        public PlacementMode HotspotPlacement
        {
            get => hotspotPlacement;
            set
            {
                if (value != hotspotPlacement)
                {
                    hotspotPlacement = value;
                    isHotspotDirty = true;
                }
            }
        }

        // HP
        [ScriptProperty]
        public int HP
        {
            get => hp;
            set
            {
                if (value != hp)
                {
                    hp = Math.Min(value, MaxHP);
                    InvalidateDamageMeter();
                    OnHPChanged();
                }
            }
        }

        // HurtArea
        [ScriptProperty]
        public Rectangle HurtArea { get; set; }

        // HurtBox
        public RectangleF HurtBox
        {
            get
            {
                if (isHurtBoxDirty)
                {
                    hurtBox = HurtArea.IsEmpty ? RectangleF.Empty : this.GetAbsoluteBounds(HurtArea);
                    isHurtBoxDirty = false;
                }

                return hurtBox;
            }
        }

        // HurtShake
        [ScriptProperty]
        public Vector2 HurtShake { get; set; } = new Vector2(.5f, 0);

        // HurtImpactSound
        [ScriptProperty]
        public Sound? HurtImpactSound { get; set; }

        // HurtSound
        [ScriptProperty]
        public Sound? HurtSound { get; set; }

        // IgnoreThrowables
        [ScriptProperty]
        public bool IgnoreThrowables { get; set; }

        // IgnoreWalkArea
        [ScriptProperty]
        public bool IgnoreWalkArea { get; set; } = true;

        // InstancesPerBlock
        public Int32Range InstancesPerBlock { get; set; } = new Int32Range(1);

        // Inventory
        public ItemContainer Inventory { get; }

        // InventorySize
        public int InventorySize => 8;

        // IsAvailable
        public bool IsAvailable(WorldBlock worldBlock, Random random)
        {
            for (int i = 0; i < placementConditions.Count; i++)
            {
                if (!placementConditions[i].IsAvailable(this, worldBlock, random))
                    return false;
            }

            return true;
        }

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

        // IsDead
        public bool IsDead => HP <= 0 && MaxHP > 0;

        // IsEmittingLight
        public virtual bool IsEmittingLight => Light != null && Light.IsEmitting;

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

        // IsWalkAreaHole
        [ScriptProperty]
        public virtual bool IsWalkAreaHole => Collider != null;

        // Light
        public Light? Light { get; set; }

        // LightPosition
        public Vector2 LightPosition { get; set; }

        // LocalizedDisplayName
        public string LocalizedDisplayName => localizedDisplayName;

        // MaxHP
        [ScriptProperty]
        public int MaxHP
        {
            get => maxHP;
            set
            {
                if (value != maxHP)
                {
                    maxHP = value;
                    HP = value;
                }
            }
        }

        // OverheadOrigin
        [ScriptProperty]
        public Vector2 OverheadOrigin { get; set; }

        // PlacementConditions
        public ReadOnlyCollection<PlacementCondition> PlacementConditions { get; }

        // PlacementPhase
        public PlacementPhase PlacementPhase { get; set; }

        // RenderLayer
        [ScriptProperty]
        public RenderLayer RenderLayer
        {
            get => renderLayer;
            set
            {
                if (value != renderLayer)
                {
                    renderLayer = value;
                    renderLayerDepth = (int)renderLayer;
                }
            }
        }

        // RenderLayerDepth
        public override int RenderLayerDepth => renderLayerDepth;

        // Replenish
        [ScriptMethod]
        public virtual void Replenish() => HP = MaxHP;

        // Room
        public new GameRoom? Room => Parent as GameRoom;

        // RuntimeHotspot
        public Polygon RuntimeHotspot
        {
            get
            {
                if (isHotspotDirty)
                {
                    if (Hotspot.IsEmpty)
                        hotspotPoly.Clear();

                    else if (HotspotPlacement == PlacementMode.Relative)
                    {
                        var offset = GetPivotBasedPolyOffset();
                        var vertices = new Vector2[Hotspot.Vertices.Count];
                        Hotspot.GetVertices(vertices, offset);
                        hotspotPoly.SetVertices(vertices);
                    }
                    else
                    {
                        isHotspotDirty = false;
                        return Hotspot;
                    }

                    isHotspotDirty = false;
                }

                return hotspotPoly;
            }
        }

        // Session
        public new GameSession Session { get; }

        // StateID
        public int StateID { get; set; }

        // TakeDamage
        public void TakeDamage(GameThing attacker, int amount, HitType hitType, Vector2 knockback)
        {
            if (IsDead)
                return;

            this.hitType = hitType;
            this.knockback = knockback;
            applyDamagePending = true;
            CumulativeDamage += amount;
            impactWordKind = ImpactWordKind.Kapow;
        }

        // ThrowableSpawnPosition
        [ScriptProperty]
        public Vector2 ThrowableSpawnPosition { get; set; }

        // ViewAngle
        public float ViewAngle { get; set; } = 90;

        // ViewDistance
        public float ViewDistance { get; set; }

        // WalkArea
        public WalkArea? WalkArea => walkArea ?? Room?.WalkArea;

        // WalkAreaName
        [ScriptProperty]
        public string WalkAreaName
        {
            get => walkAreaName;
            set
            {
                if (value != walkAreaName)
                {
                    walkAreaName = value;
                    InvalidateWalkArea();
                }
            }
        }

        // WorldBlockOrigin
        public WorldBlock? WorldBlockOrigin { get; set; }

        // WorldVersion
        [ScriptProperty]
        public int WorldVersion { get; set; } = 1;
    }
}