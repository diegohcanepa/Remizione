using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Engendro.PathFinding;
using Adberration;
using Adberration.Scripting;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// GameThing 
    /// </summary>
    public abstract class GameThing : Thing, IHoleArea, ILightSource
    {
        #region Private fields

        private bool applyDamagePending;
        private readonly Blinker<bool> blinker = new(false, true);
        private PlacementMode colliderPlacement = PlacementMode.Relative;
        private string displayName = string.Empty;
        private readonly Polygon holePolyInflated = new();
        private readonly Polygon holePoly = new();
        private readonly Polygon hotspotPoly = new();
        private PlacementMode hotspotPlacement = PlacementMode.Relative;
        private int hp;
        private Vector2Tween? hurtShakeTween;
        private FloatTween? hurtTween;
        private float floatingForce;
        private FloatTween? floatingTween;
        private ImpactWord? impactWord;
        private ImpactWordKind impactWordKind;
        private bool isCollisionDirty;
        private bool isHotspotDirty = true;
        private Vector2 knockback;
        private readonly Vector2Tween knockbackTween = new();
        private int maxHP;
        private PathNode[]? pathNodes;
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
            this.LootTableName = StaticName;
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
                    position = holePolyInflated.GetClosestPointOnEdge(position);
            }

            return position;
        }

        // CollectPathNodes
        void IHoleArea.CollectPathNodes(IList<PathNode> targetList)
        {
            InvalidateCollisionPolygons();

            if (Collider.IsEmpty)
                return;

            if (pathNodes == null || pathNodes.Length != Collider.Vertices.Count)
                pathNodes = new PathNode[Collider.Vertices.Count];

            for (int i = 0; i < holePolyInflated.Vertices.Count; i++)
            {
                // Is point concave?
                if (holePolyInflated.IsVertexConcave(i))
                    continue;

                // Is point outside walk area
                if (WalkArea != null && !WalkArea.Contains(holePolyInflated.Vertices[i]))
                    continue;

                if (pathNodes[i] == null)
                    pathNodes[i] = new(holePolyInflated.Vertices[i]);
                else
                    pathNodes[i].Position = holePolyInflated.Vertices[i];

                targetList.Add(pathNodes[i]);
            }
        }

        // Contains
        bool IHoleArea.Contains(Vector2 point)
        {
            InvalidateCollisionPolygons();
            return holePoly.Contains(point);
        }

        // InLineOfSight
        bool IHoleArea.InLineOfSight(Vector2 start, Vector2 end)
        {
            InvalidateCollisionPolygons();
            return holePoly.InLineOfSight(start, end);
        }

        // IsActive
        bool IHoleArea.IsActive => IsWalkAreaHole;

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
                    if (thing.IsDead)
                        continue;

                    // If thing is an obstacle (walk area hole)
                    if (thing.RuntimeCollider.Contains(Position))
                    {
                        if (thing is IHoleArea holeArea && holeArea.Contains(Position))
                        {
                            Position = holeArea.ClampOutside(Position);
                            OnCollision(thing);
                        }
                    }
                }
            }
        }

        // Die
        private void Die()
        {
            if (DeathSound != null)
                PlaySound(DeathSound);

            OnDie();

            if (Session.Player != null && Session.Player != this)
            {
                Session.Player.RedTickets += RedTickets;
                Session.Player.GoldenTickets += GoldenTickets;
                Session.Player.WhiteTickets += WhiteTickets;
            }

            DropLootBag();
        }

        // GetImpactWordPosition
        private Vector2? GetImpactWordPosition()
        {
            if (HitTestSource == HitTestSource.Collider && !Collider.IsEmpty)
            {
                return this.GetAbsolutePoint(Collider.BoundingRectangleF.GetPoint(RectanglePoint.Top));
            }
            else if (HitTestSource == HitTestSource.Hotspot && RuntimeHotspot != null)
            {
                return RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.Top);
            }
            else
                return null;
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

            var offset = GetPivotBasedPolyOffset();
            Collider.GetVertices(vertices, offset);
            
            holePoly.SetVertices(vertices);
            holePolyInflated.SetVertices(vertices, .05f);

            if (IsFlippedHorizontally)
            {
                holePoly.FlipHorizontally(X);
                holePolyInflated.FlipHorizontally(X);
            }

            isCollisionDirty = false;
        }

        // InvalidateWalkArea
        private void InvalidateWalkArea()
        {
            if (!string.IsNullOrWhiteSpace(walkAreaName))
                walkArea = Room?.WalkAreas.Find(walkAreaName);
            else
                walkArea = null;

            shouldClampToWalkablePosition = true;
            isCollisionDirty = true;
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

        // CanInteractCore
        protected virtual bool CanInteractCore(Actor requester)
        {
            if (requester == this || !AllowInteraction)
                return false;

            if (string.IsNullOrWhiteSpace(DisplayName))
                return false;

            return true;
        }

        // DropLootBag
        protected bool DropLootBag()
        {
            if (Session.Room is ProceduralRoom room && GetLoot() is MetaItem metaItem)
            {
                if (room.CreateRuntimeClone(nameof(LootBag)) is LootBag lootBag)
                {
                    lootBag.Drop(room, Position, metaItem);
                    return true;
                }
            }

            return false;
        }

        // GetPixelAreaForGrid
        protected virtual RectangleF GetPixelAreaForGrid()
        {
            if (Collider.IsEmpty)
                return BoundingBox;
            else
                return Collider.BoundingRectangleF;
        }

        // OnCollision
        protected virtual void OnCollision(GameThing thing)
        {
        }

        // OnDamageReaction
        protected virtual void OnDamageReaction(GameThing attacker)
        {
        }

        // OnDie
        protected virtual void OnDie()
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

        // OnDrawLights
        protected virtual void OnDrawLights(GameTime gameTime)
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
        protected virtual void OnHurt(GameThing attacker, int damage, Vector2 knockback)
        {
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            isCollisionDirty = true;
            InvalidateCollisionPolygons();
            InvalidateWalkArea();
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

            base.OnUpdate(gameTime);

            impactWord?.Update(gameTime);

            if (shouldClampToWalkablePosition || Session.Player == this)
            {
                ClampToWalkablePosition();
                shouldClampToWalkablePosition = false;
            }

            Light?.Update(gameTime);

            if (blinker.IsRunning)
            {
                blinker.Update(gameTime);

                if (!blinker.IsRunning)
                    OpacityFactor = 1;

                else if (blinker.CurrentValue)
                    OpacityFactor = .8f;

                else
                    OpacityFactor = .5f;
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

        // AllowInteraction
        [ScriptProperty]
        public bool AllowInteraction { get; set; } = true;

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

            if (HurtSound != null || HurtImpactSound != null)
            {
                hurtShakeTween ??= new();
                hurtShakeTween.Start(TweenStyle.Linear, Vector2.Zero, HurtShake, 40, 4);

                // Impact word
                if (impactWordKind != ImpactWordKind.None && GetImpactWordPosition() is Vector2 wordPos)
                {
                    impactWord ??= Session.ImpactWordPool.Get();
                    impactWord.Show(impactWordKind, wordPos);
                }
            }

            if (MaxHP == 0)
                return;

            if (CumulativeDamage > 0 && !PreventBlink)
                blinker.Start(20, 4);

            if (CumulativeDamage > HP)
                CumulativeDamage = HP;

            HP -= (int)CumulativeDamage;

            if (knockback == Vector2.Zero && HP <= 0)
            {
                Die();
            }
            else
            {
                var destination = Position;

                if (X > attacker.X)
                    destination.X += knockback.X;
                else
                    destination.X -= knockback.X;

                var bottomDistance = Math.Abs(Y - attacker.Y);
                var topDistance = Math.Abs(Y - attacker.BoundingBox.Top);

                if (bottomDistance < topDistance) 
                    destination.Y += knockback.Y;
                else
                    destination.Y -= knockback.Y;

                knockbackTween.Start(TweenStyle.CubicOut, Position, destination, 400, 0);

                hurtTween ??= new();
                hurtTween.Start(TweenStyle.Linear, 0, 1, 150, 2);

                OnHurt(attacker, (int)CumulativeDamage, knockback);
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

            return RuntimeHotspot.BoundingRectangleF.Intersects(otherThing.RuntimeHotspot.BoundingRectangleF);
        }

        // CanBeTargeted
        public bool CanBeTargeted => !IsMoving && MaxHP > 0 && !IsDead;

        // CanInteract
        public bool CanInteract(Actor requester)
        {
            if (!CanInteractCore(requester))
                return false;

            if (holePolyInflated.IsEmpty)
                return BoundingBox.Intersects(requester.GetAbsoluteBounds(requester.HotspotDetectorArea));
            else
                return holePolyInflated.BoundingRectangleF.Intersects(requester.GetAbsoluteBounds(requester.HotspotDetectorArea));
        }

        // CanInteract
        public bool CanInteract(Actor requester, Vector2 mousePos)
        {
            if (!CanInteractCore(requester))
                return false;

            return RuntimeHotspot.Contains(mousePos);
        }

        // CellMargin
        [ScriptProperty]
        public int CellMargin { get; set; } = 1;

        // ColliderPlacement
        [ScriptProperty]
        public PlacementMode ColliderPlacement
        {
            get => colliderPlacement;
            set
            {
                if (value != colliderPlacement)
                {
                    colliderPlacement = value;
                    isCollisionDirty = true;
                }
            }
        }

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

        // CollisionDamage
        [ScriptProperty]
        public DamageKind CollisionDamage { get; set; }

        // CollisionDetection
        [ScriptProperty]
        public bool CollisionDetection { get; set; } = true;

        // Collider
        [ScriptProperty]
        public Polygon Collider { get; set; } = new();

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
            if (ShowColliders)
                DrawBox(Game, holePoly.BoundingRectangleF, Color.Red * .2f);

            if (ShowHotspots)
                DrawBox(Game, RuntimeHotspot.BoundingRectangleF, Color.Purple * .2f);
        }

        // ShowColliders
        public static bool ShowColliders { get; set; }

        // ShowHotspots
        public static bool ShowHotspots { get; set; }
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
                    LocalizedDisplayName = TextRepository.GetValue(DisplayName);
                }
            }
        }

        // DrawImpactWord
        public void DrawImpactWord(GameTime gameTime) => impactWord?.Draw(gameTime);

        // DrawLights
        public void DrawLights(GameTime gameTime)
        {
            if (!IsEmittingLight)
                return;

            if (Light != null)
            {
                Light.Position = this.GetAbsolutePoint(LightPosition);
                Light.Draw(gameTime);
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

        // GetLoot
        public MetaItem? GetLoot()
        {
            if (LootTable.Find(LootTableName) is LootTable lootTable && lootTable.GetLoot() is MetaItem metaItem)
                return metaItem;

            return null;
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
            var bbox = GetPixelAreaForGrid();
            int width = (int)Math.Ceiling(bbox.Width / cellSize) + CellMargin * 2;
            int height = (int)Math.Ceiling(bbox.Height / cellSize) + CellMargin * 2;

            return new Size(width, height);
        }

        // GetFootstepSound
        public Sound? GetFootstepSound(Vector2 position)
        {
            if (RuntimeHotspot?.Contains(position) == true)
                return TerrainSound;

            return null;
        }

        // GetThrowableSpawnPosition
        public Vector2 GetThrowableSpawnPosition() => this.GetAbsolutePoint(ThrowableSpawnPosition);

        // GoldenTickets
        [ScriptProperty]
        public int GoldenTickets { get; set; }

        // HasFriendlyItems
        [ScriptProperty]
        public bool HasFriendlyItems => Session.HasFriendlyItems(StaticName);

        // Highlight
        [ScriptProperty]
        public bool Highlight { get; set; } = true;

        // HitTest
        public bool HitTest(Vector2 value)
        {
            if (HitTestSource == HitTestSource.Hotspot)
                return RuntimeHotspot.Contains(value);
            else
                return (this as IHoleArea).Contains(value);
        }

        // HitTestSource
        public HitTestSource HitTestSource { get; init; }

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
                    OnHPChanged();
                }
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

        // IsBlinkingDamage
        public bool IsBlinkingDamage => blinker.IsRunning && blinker.CurrentValue;

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
        public virtual bool IsWalkAreaHole => !Collider.IsEmpty;

        // Light
        public Light? Light { get; set; }

        // LightPosition
        public Vector2 LightPosition { get; set; }

        // LocalizedDisplayName
        public string LocalizedDisplayName { get; protected set; } = string.Empty;

        // LootTableName
        [ScriptProperty]
        public string LootTableName { get; set; }

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

        // PlacementPhase
        [ScriptProperty]
        public PlacementPhase PlacementPhase { get; set; }

        // PreventBlink
        [ScriptProperty]
        public bool PreventBlink { get; set; }

        // PreventKnockback
        [ScriptProperty]
        public bool PreventKnockback { get; set; }

        // RedTickets
        [ScriptProperty]
        public int RedTickets { get; set; }

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

        // RuntimeCollider
        public Polygon RuntimeCollider => holePolyInflated;

        // RuntimeHotspot
        public Polygon RuntimeHotspot
        {
            get
            {
                if (isHotspotDirty)
                {
                    if (Hotspot.IsEmpty)
                    {
                        hotspotPoly.Clear();
                    }
                    else if (HotspotPlacement == PlacementMode.Relative)
                    {
                        var offset = GetPivotBasedPolyOffset();
                        offset.Y -= Altitude;

                        var vertices = new Vector2[Hotspot.Vertices.Count];
                        Hotspot.GetVertices(vertices, offset);
                        hotspotPoly.SetVertices(vertices);
                        if (IsFlippedHorizontally)
                            hotspotPoly.FlipHorizontally(X);
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

        // TakeDamage
        public void TakeDamage(GameThing attacker, int amount, Vector2 knockback, ImpactWordKind impactWordKind)
        {
            if (IsDead)
                return;

            this.knockback = PreventKnockback ? Vector2.Zero : knockback;
            this.applyDamagePending = true;
            this.CumulativeDamage += amount;
            this.impactWordKind = impactWordKind;
        }

        // TerrainParticleColor
        [ScriptProperty]
        public Color TerrainParticleColor { get; set; }

        // TerrainSound
        [ScriptProperty]
        public Sound? TerrainSound { get; set; }

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

        // WhiteTickets
        [ScriptProperty]
        public int WhiteTickets { get; set; }

        // WorldVersion
        [ScriptProperty]
        public int WorldVersion { get; set; } = 1;
    }
}