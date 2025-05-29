using Engendro;
using Engendro.Audio;
using Engendro.PathFinding;
using EngendroAdventure;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        private PathNode[]? holeNodes;
        private readonly Polygon holePoly = new();
        private RectangleF hotspotBox;
        private PlacementMode hotspotPlacement = PlacementMode.Relative;
        private int hp;
        private RectangleF hurtBox;
        private FloatTween? hurtTween;
        private float floatingForce;
        private FloatTween? floatingTween;
        private HitType hitType;
        private bool isHoleAreaDirty;
        private bool isHotspotDirty = true;
        private bool isHurtBoxDirty = true;
        private Vector2 knockback;
        private readonly Vector2Tween knockbackTween = new();
        private string localizedDisplayName = string.Empty;
        private int maxHP;
        private readonly List<PlacementCondition> placementConditions = [];
        private RenderLayer renderLayer;
        private int renderLayerDepth;
        private bool shouldClampToWalkablePosition;
        private List<Verb>? verbList;
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
            this.Inventory = new ItemContainer(this, ItemContainerCategory.Inventory, Localization.GetValue(InGameMenuOptionName.Inventory));
        }

        #endregion

        #region IHoleArea explicit implementation

        // ClampOutside
        Vector2 IHoleArea.ClampOutside(Vector2 position)
        {
            if (CollisionPolygon != null)
            {
                InvalidateHoleArea();
                if (holePoly.IsPointInside(position))
                    position = holeInflatedPoly.GetClosestPointOnEdge(position);
            }

            return position;
        }

        // CollectNodes
        void IHoleArea.CollectNodes(IList<PathNode> targetList)
        {
            InvalidateHoleArea();

            for (int i = 0; i < holeInflatedPoly.Vertices.Count; i++)
            {
                // Is point concave?
                if (holeInflatedPoly.IsVertexConcave(i))
                    continue;

                // Is point ourside walk area
                if (WalkArea != null && !WalkArea.IsInside(holeInflatedPoly.Vertices[i]))
                    continue;

                if (holeNodes != null)
                    targetList.Add(holeNodes[i]);
            }
        }

        // Contains
        bool IHoleArea.Contains(Vector2 point)
        {
            InvalidateHoleArea();
            return holePoly.IsPointInside(point);
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
                    if (thing.CollisionPolygon != null)
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
                Session.Player.Ashes += Ashes;

            if (LootTable.Find(StaticName) is LootTable lootTable)
            {
                if (Session.ObjectPools.LootBags.Get() is LootBag lootBag)
                {
                    var itemName = lootTable.GetLoot();
                    lootBag.Drop(Position, itemName);
                }
            }
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
            if (!isHoleAreaDirty || CollisionPolygon == null)
                return;

            int vertexCount = CollisionPolygon.Vertices.Count;
            var vertices = new Vector2[vertexCount];

            var offset = new Vector2(X - BoundingBox.Width / 2, Y - BoundingBox.Height);
            CollisionPolygon.GetVertices(vertices, offset);
            holePoly.SetVertices(vertices);
            holeInflatedPoly.SetVertices(vertices, .05f);

            if (holeNodes == null || holeNodes.Length != vertices.Length)
            {
                holeNodes = new PathNode[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    holeNodes[i] = new PathNode(holeInflatedPoly.Vertices[i]);
                }
            }

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
            {
                floatingTween.Update(gameTime);
                Altitude += floatingTween.CurrentValue;
            }

            if (hurtTween != null && hurtTween.IsRunning)
            {
                hurtTween.Update(gameTime);
                Altitude += hurtTween.CurrentValue;
            }

            base.OnDraw(gameTime);

            if (floatingTween != null && floatingTween.IsRunning)
                Altitude -= floatingTween.CurrentValue;

            if (hurtTween != null && hurtTween.IsRunning)
                Altitude -= hurtTween.CurrentValue;
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

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);

            isHotspotDirty = true;
            isHurtBoxDirty = true;
            shouldClampToWalkablePosition = true;

            if (change != TransformChange.Altitude)
                MarkHoleAreaDirty();
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();

            ResetApplyDamageValues();

            OpacityFactor = 1;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            hurtTween?.Update(gameTime);

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

            if (shouldClampToWalkablePosition)
            {
                ClampToWalkablePosition();
                shouldClampToWalkablePosition = false;
            }
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

        #endregion

        // AddPlacementCondition
        public void AddPlacementCondition(PlacementCondition condition)
        {
            placementConditions.Add(condition);
        }

        // AddVerbs
        public void AddVerbs(params Verb[] verbs)
        {
            verbList ??= [];

            for (var i = 0; i < verbs.Length; i++)
            {
                if (!verbList.Contains(verbs[i]))
                    verbList.Add(verbs[i]);
            }

            verbList.Sort();
        }

        // ApplyDamage
        public void ApplyDamage(GameThing attacker)
        {
            if (!applyDamagePending || IsDead || CumulativeDamage <= 0)
            {
                ResetApplyDamageValues();
                return;
            }

            HP -= (int)CumulativeDamage;

            if (HurtSound != null)
                PlaySound(HurtSound);

            if (HurtImpactSound != null)
                PlaySound(HurtImpactSound);

            OnDamageReaction(attacker);

            var damageTextColor = hitType == HitType.Critical ? ColorPalette.TextDepracated.Dark : ColorPalette.Text.Default;
            var damageText = $"{(int)CumulativeDamage}";
            if (hitType == HitType.Critical)
                damageText += " " + TextRepository.GetValue("HitType.Critical");

            Session.ObjectPools.FloatingTexts.Get()?.Show(GetFloatingTextPosition(knockback), damageText, damageTextColor);

            damageMeterCooldown = 1500;
            if (damageMeter == null)
            {
                damageMeter = new(Game, ColorPalette.HPMeter.Back, ColorPalette.HPMeter.Fore) { MaximumValue = 10 };
                InvalidateDamageMeter();
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

        // Ashes
        [ScriptProperty]
        public int Ashes { get; set; }

        // CanBeTargeted
        public bool CanBeTargeted => !IsMoving && MaxHP > 0 && !IsDead;

        // CanInteract
        public bool CanInteract(Actor requester)
        {
            if (requester == this)
                return false;

            if (string.IsNullOrWhiteSpace(DisplayName))
                return false;

            return HotspotBox.Contains(requester.GetAbsolutePoint(requester.HotspotDetectorPosition));
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
            if (WalkArea != null && WalkArea.Holes.Count > 0)
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

        // CollisionPolygon
        [ScriptProperty]
        public Polygon? CollisionPolygon { get; set; }

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
            if (ShowHotspotBoxes)
                DrawBox(Game, HotspotBox, Color.Purple * .2f);

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

            var box = HotspotBox;
            if (box.IsEmpty)
                box = BoundingBox;

            var requesterBox = requester.HotspotBox;
            if (requesterBox.IsEmpty)
                requesterBox = requester.BoundingBox;

            Vector2 result;

            if (inFront)
            {
                if (Direction == FacingDirection.Left)
                    result = box.GetPoint(RectanglePoint.LeftBottom, -requesterBox.Width / 2, 0);
                else
                    result = box.GetPoint(RectanglePoint.RightBottom, requesterBox.Width / 2, 0);
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

        // GetItemContainerSize
        public virtual int GetItemContainerSize(ItemContainerCategory category) => 8;

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

            if (CollisionPolygon == null)
                bbox = BoundingBox;
            else
                bbox = CollisionPolygon.BoundingRectangleF;

            int width = (int)Math.Ceiling(bbox.Width / cellSize) + CellMargin * 2;
            int height = (int)Math.Ceiling(bbox.Height / cellSize) + CellMargin * 2;

            return new Size(width, height);
        }

        // GetThrowableSpawnPosition
        public Vector2 GetThrowableSpawnPosition() => this.GetAbsolutePoint(ThrowableSpawnPosition);

        // GetVerbs
        public Verb[]? GetVerbs() => verbList?.ToArray();

        // HotspotArea
        [ScriptProperty]
        public Rectangle HotspotArea { get; set; }

        // HotspotBox
        public virtual RectangleF HotspotBox
        {
            get
            {
                if (isHotspotDirty)
                {
                    if (HotspotArea.IsEmpty)
                        hotspotBox = RectangleF.Empty;

                    if (HotspotPlacement == PlacementMode.Relative)
                        hotspotBox = Sprite.GetAbsoluteBounds(HotspotArea);
                    else
                        hotspotBox = HotspotArea;

                    isHotspotDirty = false;
                }

                return hotspotBox;
            }
        }

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

        // IgnoreWalkArea
        [ScriptProperty]
        public bool IgnoreWalkArea { get; set; } = true;

        // InstancesPerBlock
        public Int32Range InstancesPerBlock { get; set; } = new Int32Range(1);

        // Inventory
        public ItemContainer Inventory { get; }

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

        // IsWalkAreaHole
        [ScriptProperty]
        public virtual bool IsWalkAreaHole => CollisionPolygon != null;

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

        // Session
        public new GameSession Session { get; }

        // TakeDamage
        public void TakeDamage(GameThing attacker, int amount, HitType hitType, Vector2 knockback)
        {
            if (IsDead)
                return;

            this.hitType = hitType;
            applyDamagePending = true;
            CumulativeDamage += amount;

            if (knockback != Vector2.Zero)
                this.knockback = knockback;
        }

        // ThrowableSpawnPosition
        [ScriptProperty]
        public Vector2 ThrowableSpawnPosition { get; set; }

        // Verb
        [ScriptProperty]
        public Verb Verb { get; set; }

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

        // WorldVersion
        [ScriptProperty]
        public int WorldVersion { get; set; } = 1;
    }
}