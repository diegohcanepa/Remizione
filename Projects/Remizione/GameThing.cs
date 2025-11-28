using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Engendro.PathFinding;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Remizione
{
    /// <summary> 
    /// GameThing 
    /// </summary>
    public abstract class GameThing : Thing, IHoleArea, ILightSource, ILootContext
    {
        #region Private fields

        private readonly Blinker<bool> blinker = new(false, true);
        private readonly Polygon holePoly = new();
        private Vector2Tween? hurtShakeTween;
        private FloatTween? hurtTween;
        private FloatTween? floatingTween;
        private bool isCollisionDirty;
        private bool isHotspotDirty = true;
        private readonly Vector2Tween knockbackTween = new();
        private readonly List<LootTag> lootTags = [];
        private PathNode[]? pathNodes;
        private int renderLayerDepth;
        private bool shouldClampToWalkablePosition;

        #endregion

        #region Constructor

        // Constructor
        protected GameThing(GameSession session, string name)
            : base(session, name)
        {
            this.RenderLayer = RenderLayer.Default;
            this.Session = session;
            this.LootTags = new(lootTags);
            this.LootTableName = StaticName;
            this.ResistanceTableName = StaticName;
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
                    if (thing.IsDead || !thing.CollisionDetection)
                        continue;

                    if (Altitude > thing.CollisionHeight)
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

        // GetImpactWordPosition
        private Vector2? GetImpactWordPosition()
        {
            if (HitTestPolygon == TestPolygon.Collider && !Collider.IsEmpty)
            {
                return this.GetAbsolutePoint(Collider.BoundingRectangleF.GetPoint(RectanglePoint.Top));
            }
            else if (HitTestPolygon == TestPolygon.Hotspot && RuntimeHotspot != null)
            {
                return RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.Top);
            }
            else
            {
                return null;
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
            if (!string.IsNullOrWhiteSpace(WalkAreaName))
                WalkArea = Room?.WalkAreas.Find(WalkAreaName);
            else
                WalkArea = null;

            shouldClampToWalkablePosition = true;
            isCollisionDirty = true;
        }

        #endregion

        #region Protected members

        // CanCheckCollisions
        protected virtual bool CanCheckCollisions()
        {
            return CollisionDetection;
        }

        // CanInteractCore
        protected virtual bool CanInteractCore(Actor requester)
        {
            if (requester == this || !AllowInteraction)
                return false;

            if (IsDead || string.IsNullOrWhiteSpace(LocalizedDisplayName))
                return false;

            return true;
        }

        // DropLoot
        protected void DropLoot()
        {
            if (Session.Room is not ProceduralRoom room)
                return;

            //Loot.TryDropLoot(room, Position, LootTableName, out _);

            if (Random.Shared.NextDouble() < TicketRewardChance)
            {
                var tickets = TicketReward.RandomValue();
                if (tickets > 0 && Session.Player != null)
                {
                    for (var i = 0; i < tickets; i++)
                    {
                        Session.ObjectPools.Tickets.Get()?.Drop(room, Position);
                    }
                }
            }
        }

        // GetShakeOffset
        protected Vector2 GetShakeOffset()
        {
            if (hurtShakeTween?.IsRunning == true)
                return hurtShakeTween.CurrentValue;
            else
                return Vector2.Zero;
        }

        // OnCollision
        protected virtual void OnCollision(GameThing thing)
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
        }

        // OnHPChanged
        protected virtual void OnHPChanged()
        {
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            blinker.Stop();
            isCollisionDirty = true;
            InvalidateCollisionPolygons();
            InvalidateWalkArea();
        }

        // OnTakeDamage
        protected virtual void OnTakeDamage(GameThing attacker, int damage, DamageType damageType, Vector2 knockback)
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

            if (knockbackTween.IsRunning)
            {
                knockbackTween.Update(gameTime);
                Position = knockbackTween.CurrentValue;
                if (!knockbackTween.IsRunning && IsDead)
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

        // OnUpdateEmittingSound (re-implementada)
        protected override void OnUpdateEmittingSound(SoundInstance instance, float masterVolume)
        {
            const int margin = 80; // margen en píxeles fuera del VisibleBox donde el volumen cae linealmente a 0

            RectangleF visible = Session.Camera.VisibleBox;
            float spriteX = Position.X;

            // límites extendidos (visible box + margen a ambos lados)
            float leftLimit = visible.Left - margin;
            float rightLimit = visible.Right + margin;

            // Denominador para normalizar pan respecto al centro (ancho/2 + margin).
            // Protegemos contra ancho 0.
            float halfRange = (visible.Width * 0.5f) + margin;
            if (halfRange <= 0.0001f) halfRange = 1f; // fallback seguro

            // Pan: -1 en leftLimit, 0 en el centro de la cámara, +1 en rightLimit
            float centerX = visible.Center.X;
            float pan = MathHelper.Clamp((spriteX - centerX) / halfRange, -1f, 1f);

            // Volumen:
            // - Si está dentro del VisibleBox => 1
            // - Si está fuera del leftLimit/rightLimit => 0
            // - Si está entre VisibleBox y límite extendido => interpolación lineal 1 -> 0
            float volume;
            if (visible.Contains(Position))
            {
                volume = 1f;
            }
            else if (spriteX <= leftLimit || spriteX >= rightLimit)
            {
                // completamente fuera del rango extendido
                volume = 0f;
            }
            else
            {
                // Está fuera del VisibleBox pero dentro del margen extendido.
                if (spriteX < visible.Left)
                {
                    // se encuentra a la izquierda del VisibleBox
                    float t = (visible.Left - spriteX) / margin; // 0..1
                    volume = MathHelper.Clamp(1f - t, 0f, 1f);
                }
                else // spriteX > visible.Right
                {
                    float t = (spriteX - visible.Right) / margin; // 0..1
                    volume = MathHelper.Clamp(1f - t, 0f, 1f);
                }
            }

            // Aplicar valores al SoundInstance
            instance.Pan = pan;
            instance.Volume.Current = volume * masterVolume;
        }

        #endregion

        // AllowInteraction
        [ScriptProperty]
        public bool AllowInteraction { get; set; } = true;

        // ApproachPosition
        [ScriptProperty]
        public Vector2 ApproachPosition { get; set; }

        // AttachedLight
        public Light? AttachedLight { get; set; }

        // AttachedLightPosition
        public Vector2 AttachedLightPosition { get; set; }

        // AttackRange
        [ScriptProperty]
        public int AttackRange { get; set; } = -1;

        // CanInteract
        public bool CanInteract(Actor requester)
        {
            if (!CanInteractCore(requester))
                return false;

            if (InteractionPolygon == TestPolygon.Collider)
                return RuntimeCollider.BoundingRectangleF.Intersects(requester.GetAbsoluteBounds(requester.HotspotDetectorArea));
            else
                return RuntimeHotspot.BoundingRectangleF.Intersects(requester.GetAbsoluteBounds(requester.HotspotDetectorArea));
        }

        // CanInteractWithKeyItem
        [ScriptProperty]
        public bool CanInteractWithKeyItem
        {
            get
            {
                if (Session.Player == null)
                    return false;

                var friendlyItems = Session.GetFriendlyItems(StaticName);
                for (var i = 0; i < friendlyItems.Length; i++)
                {
                    if (Session.Inventory.Find(friendlyItems[i].Name) != null)
                        return true;
                }

                return false;
            }
        }

        // CanTakeDamage
        public bool CanTakeDamage(GameThing attacker)
        {
            if (IsDead || attacker.Faction == Faction)
                return false;

            if (blinker.IsRunning)
                return false;

            return true;
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

        // ContactDamagePolygon
        public TestPolygon ContactDamagePolygon { get; set; } = TestPolygon.Collider;

        // ContactDamageType
        public DamageType ContactDamageType { get; set; }

        // Die
        [ScriptMethod]
        public void Die()
        {
            HP = int.MinValue;

            if (DeathSound != null)
                PlaySound(DeathSound);

            OnDie();
            Room?.RecountEnemies();
            DropLoot();
        }

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

        // GridMargin
        [ScriptProperty]
        public Vector2 GridMargin { get; set; }

        // GridMeasureType
        [ScriptProperty]
        public GridMeasureType GridMeasureType { get; set; }

        // GetGridPixelArea
        public RectangleF GetGridPixelArea()
        {
            var result = GridMeasureType switch
            {
                GridMeasureType.BoundingBox => BoundingBox,
                GridMeasureType.Collider => Collider.BoundingRectangleF,
                GridMeasureType.Hotspot => Hotspot.BoundingRectangleF,
                _ => RectangleF.Empty,
            };

            result.Inflate(GridMargin);

            return result;
        }

        // HitEffect
        [ScriptProperty]
        public HitEffect HitEffect { get; set; }

        // InvulnerabilityPeriod
        public bool InvulnerabilityPeriod => blinker.IsRunning;

        // IsBlinking
        public bool IsBlinking => blinker.IsRunning && blinker.CurrentValue;

        // IsEnemy
        public bool IsEnemy(GameThing target)
        {
            if (Faction == target.Faction)
                return false;

            if (target.Faction == Faction.Neutral)
                return false;

            return true;
        }

        // DeathSound
        [ScriptProperty]
        public Sound? DeathSound { get; set; }

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
                    LocalizedDisplayName = TextRepository.GetValue(DisplayNameKey);
                }
            }
        } = string.Empty;

        // DrawLights
        public void DrawLights(GameTime gameTime)
        {
            if (!IsEmittingLight)
                return;

            if (AttachedLight != null)
            {
                if (AttachedLightPosition != Vector2.Zero)
                    AttachedLight.Position = this.GetAbsolutePoint(AttachedLightPosition);
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
        public void FaceTo(GameThing thing)
        {
            if (X < thing.X)
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
                if (value != field)
                {
                    field = value;
                    Room?.RecountEnemies();
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
        public Vector2 GetFloatingTextPosition(Vector2 knockback)
        {
            return GetFloatingTextPosition(knockback, 0, 0);
        }

        // GetFloatingTextPosition
        public Vector2 GetFloatingTextPosition(Vector2 knockback, int xOffset, int yOffset)
        {
            var result = GetOverheadPosition();

            if (Direction == FacingDirection.Right)
                result.X -= Math.Abs(knockback.X);
            else
                result.X += Math.Abs(knockback.X);

            result.X += xOffset;
            result.Y += yOffset;

            return result;
        }

        // GetFootstepSound
        public Sound? GetFootstepSound(Vector2 position)
        {
            if (RuntimeCollider?.Contains(position) == true)
                return TerrainSound;

            return null;
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
        public Vector2 GetOverheadPosition()
        {
            return GetOverheadPosition(0, 0);
        }

        // GetOverheadPosition
        public Vector2 GetOverheadPosition(int xOffset, int yOffset)
        {
            // Origin
            if (OverheadOrigin == Vector2.Zero)
                return BoundingBox.GetPoint(RectanglePoint.Top, xOffset, yOffset);
            else
                return this.GetAbsolutePoint(OverheadOrigin, xOffset, yOffset);
        }

        // GetResistanceModifier
        public float GetResistanceModifier(DamageType damageType)
        {
            if (ResistanceTable.Find(ResistanceTableName) is ResistanceTable table)
                return table.GetModifier(damageType);

            return 1;
        }

        // GetThrowableSpawnPosition
        public Vector2 GetThrowableSpawnPosition()
        {
            return this.GetAbsolutePoint(ThrowableSpawnPosition);
        }

        // HighlightInteraction
        [ScriptProperty]
        public bool HighlightInteraction { get; set; } = true;

        // HitTest
        public bool HitTest(Vector2 value)
        {
            if (HitTestPolygon == TestPolygon.Hotspot)
                return RuntimeHotspot.Contains(value);
            else
                return (this as IHoleArea).Contains(value);
        }

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
                    field = Math.Min(value, MaxHP);
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

        // IgnoreAttachedLight
        [ScriptProperty]
        public bool IgnoreAttachedLight { get; set; }

        // IgnoreThrowables
        [ScriptProperty]
        public bool IgnoreThrowables { get; set; }

        // IgnoreWalkArea
        [ScriptProperty]
        public bool IgnoreWalkArea { get; set; } = true;

        // InteractionPolygon
        [ScriptProperty]
        public TestPolygon InteractionPolygon { get; set; } = TestPolygon.Hotspot;

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
        public bool IsDead => (HP <= 0 && MaxHP > 0) || (HP == int.MinValue);

        // IsEmittingLight
        public virtual bool IsEmittingLight => AttachedLight != null && !IgnoreAttachedLight && AttachedLight.IsEmitting;

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

        // LightDamageResistance
        [ScriptProperty]
        public bool LightDamageResistance { get; set; }

        // LocalizedDisplayName
        public string LocalizedDisplayName { get; private set; } = string.Empty;

        // LootTableName
        [ScriptProperty]
        public string LootTableName { get; set; }

        // LootTags
        public ReadOnlyCollection<LootTag> LootTags { get; }

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
                    HP = value;
                }
            }
        }

        // OverheadOrigin
        [ScriptProperty]
        public Vector2 OverheadOrigin { get; set; }

        // PowerBonus
        [ScriptProperty]
        public int PowerBonus { get; set; }

        // PreventKnockback
        [ScriptProperty]
        public bool PreventKnockback { get; set; }

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

        // ScoreValue
        [ScriptProperty]
        public int ScoreValue { get; set; }

        // Session
        public new GameSession Session { get; }

        // TakeDamage
        public void TakeDamage(GameThing attacker, int amount, DamageType damageType, bool critical, Vector2 knockback, ImpactWordName impactWord)
        {
            if (amount <= 0 || !CanTakeDamage(attacker))
                return;

            knockback = PreventKnockback ? Vector2.Zero : knockback;

            if (HurtSound != null)
                PlaySound(HurtSound);

            if (HurtImpactSound != null)
                PlaySound(HurtImpactSound);

            if (HitEffect == HitEffect.Shake)
            {
                hurtShakeTween ??= new();
                hurtShakeTween.Start(TweenStyle.Linear, Vector2.Zero, HurtShake, 40, 4);
            }

            // Impact word
            if (MaxHP > 0 && impactWord != ImpactWordName.None && GetImpactWordPosition() is Vector2 wordPos)
                Session.ImpactWordPool.Get()?.Show(impactWord, wordPos);

            if (MaxHP == 0)
                return;

            amount = (int)(amount * GetResistanceModifier(damageType));

            if (amount > HP)
                amount = HP;

            HP -= amount;

            if ((knockback == Vector2.Zero || Session.Player != this) && HP <= 0)
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

                if (knockback != Vector2.Zero)
                {
                    if (bottomDistance < topDistance)
                        destination.Y += knockback.Y;
                    else
                        destination.Y -= knockback.Y;

                    knockbackTween.Start(TweenStyle.CubicOut, Position, destination, 400, 0);
                }

                hurtTween ??= new();
                hurtTween.Start(TweenStyle.Linear, 0, 1, 150, 2);

                if (HitEffect == HitEffect.Blink)
                    blinker.Start(40, Session.Player == this ? 15 : 4);
                else
                    blinker.Stop();

                if (amount > 0)
                    Session.ObjectPools.FloatingTexts.Get()?.Show(GetFloatingTextPosition(knockback), amount.ToString(CultureInfo.InvariantCulture), critical);

                if (Session.Player == attacker)
                    Session.HUD.TargetMeter.Target = this;

                else if (Session.Player == this)
                    Session.HUD.TargetMeter.Target = attacker;

                if (amount > 0)
                    OnTakeDamage(attacker, amount, damageType, knockback);
            }
        }

        // TerrainParticleColor
        [ScriptProperty]
        public Color TerrainParticleColor { get; set; }

        // TerrainSound
        [ScriptProperty]
        public Sound? TerrainSound { get; set; }

        // TicketReward
        [ScriptProperty]
        public Int32Range TicketReward { get; set; }

        // TicketRewardChance
        [ScriptProperty]
        public float TicketRewardChance { get; set; } = .5f;

        // ThrowableSpawnPosition
        [ScriptProperty]
        public Vector2 ThrowableSpawnPosition { get; set; }

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