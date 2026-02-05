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
        private Vector2Tween? hurtShakeTween;
        private FloatTween? hurtTween;
        private bool isCollisionDirty;
        private bool isHotspotDirty = true;
        private Vector2 _knockbackVelocity;
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

            Definition = ThingDefinition.Find(DeclaredName);
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

        // DropLoot
        protected void DropLoot()
        {
            if (Room is not ProceduralRoom)
                return;

            if (Definition == null)
                return;

            Ratio lootChance = Definition.Difficulty switch
            {
                Difficulty.Easy => .05f,   // 5%
                Difficulty.Normal => .15f, // 15%
                Difficulty.Hard => .30f,   // 30%
                _ => .02f
            };

            lootChance += Session.Inventory.GetLuckFactor();

            if (!lootChance.Roll())
                return;
        }

        // DropCoins
        protected void DropCoins()
        {
            if (Definition == null)
                return;

            if (Session.Room is not ProceduralRoom room)
                return;

            var coins = Session.LootGenerator.RollCoins(room.Definition, Definition);

            if (coins > 0)
            {
                for (var i = 0; i < coins; i++)
                {
                    if (room.CreateThingClone("Coin") is Coin coin)
                    {
                        coin.Position = Position;
                        room.Children.Add(coin);
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

        // OnCollisioning
        protected virtual void OnCollisioning(GameThing thing, out bool handled)
        {
            handled = false;
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
            shadowSpot.Draw(gameTime);
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
        protected virtual void OnTakeDamage(GameThing attacker, int amount, DamageType damageType, Vector2 knockback)
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

            if (_knockbackVelocity != Vector2.Zero)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                // 1. Aplicar movimiento
                Position += _knockbackVelocity * dt;

                // 2. Aplicar fricción (decaimiento)
                _knockbackVelocity *= KnockbackFriction;

                // 3. Limpiar valores residuales muy chicos
                if (_knockbackVelocity.LengthSquared() < 100f) // Ajustá según tu escala de píxeles
                    _knockbackVelocity = Vector2.Zero;

                // Si murió por el golpe, chequear acá si paró para llamar a Die() visualmente
                if (_knockbackVelocity == Vector2.Zero && IsDead)
                    Die();
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

        #endregion

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

        // CanInteract
        public virtual bool CanInteract()
        {
            if (!AllowInteraction)
                return false;

            if (IsMoving || IsDead || string.IsNullOrWhiteSpace(LocalizedDisplayName))
                return false;

            if (Session.InteractionContext.HeldItem != null && !CanInteractWithItem())
                return false;

            return true;
        }

        // CanInteractWithItem
        public virtual bool CanInteractWithItem()
        {
            return true;
        }

        // CanTakeDamage
        public bool CanTakeDamage()
        {
            return !IsDead && !blinker.IsRunning;
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

        // Definition
        public ThingDefinition? Definition { get; }

        // Die
        [ScriptMethod]
        public void Die()
        {
            dieCalled = true;

            HP = int.MinValue;

            if (DeathSound?.PopInstance() is SoundInstance deathSoundInstance)
            {
                Utils.ApplySoundEmitter(this, deathSoundInstance, deathSoundInstance.GetEffectiveVolume());
                deathSoundInstance.Play();
            }

            if (Session.Player == this)
                ShowImpactWord(ImpactWordName.PlopRed);

            OnDie();
            DropLoot();
            DropCoins();
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

        // HitEffect
        [ScriptProperty]
        public HitEffect HitEffect { get; set; }

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
        public Faction Faction { get; set; }

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
                    return this.GetAbsolutePoint(ApproachPosition);
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
            float spacing = (this is Actor) ? requesterBox.Width + 3 : requesterBox.Width / 2;

            float targetX = X; // Default

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
            }

            // Mantenemos la Y en la base del objeto (los pies)
            return new Vector2(targetX, BoundingBox.Bottom + Altitude);
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

        // GetInteractPrompt
        public virtual string? GetInteractPrompt()
        {
            return null;
        }

        // GetMouseCursorState
        public virtual MouseCursorState? GetMouseCursorState()
        {
            return null;
        }

        // GetOverheadPosition
        public Vector2 GetOverheadPosition()
        {
            // Origin
            if (OverheadOrigin == Vector2.Zero)
                return BoundingBox.GetPoint(RectanglePoint.Top);
            else
                return this.GetAbsolutePoint(OverheadOrigin);
        }

        // GetResistanceModifier
        public float GetResistanceModifier(DamageType damageType)
        {
            if (ResistanceTable.Find(ResistanceTableName) is ResistanceTable table)
                return table.GetModifier(damageType);

            return 1;
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

                    if (field > 0)
                        dieCalled = false;

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

        // LocalizedDisplayName
        public string LocalizedDisplayName { get; private set; } = string.Empty;

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

        // Session
        public new GameSession Session { get; }

        // ShadowOffset
        [ScriptProperty]
        public Vector2 ShadowOffset
        {
            get => shadowSpot.Offset;
            set => shadowSpot.Offset = value;
        }

        // ShadowSpotSize
        [ScriptProperty]
        public int ShadowSpotSize
        {
            get => shadowSpot.Size;
            set => shadowSpot.Size = value;
        }

        // ShowFloatingText
        public void ShowFloatingText(string text, Color color, int duration = 1000)
        {
            if (Session.ObjectPools.FloatingTexts.Get() is FloatingText floatingText)
                floatingText.Show(GetOverheadPosition(), text, color, duration);
        }

        // ShowImpactWord
        public void ShowImpactWord(ImpactWordName impactWordName)
        {
            if (GetOverheadPosition() is Vector2 wordPos)
                Session.ImpactWordPool.Get()?.Show(impactWordName, wordPos + new Vector2(0, 5));
        }

        // TakeDamage
        public void TakeDamage(GameThing attacker, DamageType damageType, int amount, ImpactWordName impactWordName, Vector2 knockbackForce)
        {
            // ---------------------------------------------------------
            // 1. FILTROS DE SALIDA (Gatekeepers)
            // ---------------------------------------------------------

            // Si la cantidad es 0 o negativa, no hay interacción de daño.
            if (amount <= 0)
                return;

            // Si ya está muerto o está en frames de invencibilidad, ignoramos todo.
            if (!CanTakeDamage())
                return;

            // ---------------------------------------------------------
            // 2. FEEDBACK INICIAL (Juice)
            // ---------------------------------------------------------
            // Esto ocurre SIEMPRE que hay un impacto válido, aunque sea indestructible.

            // Sonido de dolor o impacto
            if (HurtSound != null)
                PlaySound(HurtSound);

            if (HurtImpactSound != null)
                PlaySound(HurtImpactSound);

            // Shake: El objeto tiembla por el golpe (incluso una pared dura puede vibrar)
            if (HitEffect == HitEffect.Shake)
            {
                hurtShakeTween ??= new();
                hurtShakeTween.Start(TweenStyle.Linear, Vector2.Zero, HurtShake, 40, 4);
            }

            // ---------------------------------------------------------
            // 3. REACCIÓN DE EFECTOS (Espinas / Rebote) - CRÍTICO
            // ---------------------------------------------------------
            // Hacemos esto ANTES de calcular si muere o recibe daño real. 
            // Si golpeo una vasija igual quiero que mis efectos se activen.
            if (Session.CombatManager == null)
            {
                if (Definition?.EffectDescriptors != null)
                    EffectDescriptor.Apply(Definition.EffectDescriptors, this, attacker);
            }

            // ---------------------------------------------------------
            // 4. LÓGICA DE SALUD (Solo si es Destructible)
            // ---------------------------------------------------------

            // Aquí es donde manejamos el MaxHP == 0
            if (MaxHP > 0)
            {
                // Aplicar resistencias
                int finalDamage = (int)(amount * GetResistanceModifier(damageType));

                // Clamp para no restar más de lo que tiene
                if (finalDamage > HP) finalDamage = HP;

                // Si después de la resistencia el daño es 0, salimos de la lógica de HP
                if (finalDamage > 0)
                {
                    HP -= finalDamage;

                    // Feedback de Daño Real (Parpadeo Rojo / Blanco)
                    // Solo parpadeamos si realmente perdimos vida
                    hurtTween ??= new();
                    hurtTween.Start(TweenStyle.Linear, 0, 1, 150, 2);

                    if (HitEffect == HitEffect.Blink)
                        blinker.Start(40, 15); // Invulnerabilidad post-daño
                    else
                        blinker.Stop();

                    // Evento específico para lógicas custom
                    OnTakeDamage(attacker, finalDamage, damageType, Vector2.Zero);

                    // Impact Word (Solo mostramos "Pow!" si hubo daño real)
                    if (impactWordName != ImpactWordName.None)
                        ShowImpactWord(impactWordName);
                }
            }
            else
            {
                // Lógica para Indestructibles (MaxHP == 0)
                // Opcional: Sonido de "Metal/Rebote" o palabra "BLOCK"
                // ShowImpactWord(ImpactWordName.Clink); 
            }


            // ---------------------------------------------------------
            // 5. FÍSICAS (Knockback)
            // ---------------------------------------------------------
            // El empuje se aplica independientemente de la vida. 
            // Una caja de metal indestructible (MaxHP=0) debería poder ser empujada.

            if (knockbackForce != Vector2.Zero)
            {
                Vector2 pushDirection = Position - attacker.Position;
                if (pushDirection != Vector2.Zero) pushDirection.Normalize();
                else pushDirection = new Vector2(1, 0);

                // Aplicamos la fuerza
                _knockbackVelocity = pushDirection * knockbackForce.Length() * 5f;
            }
        }

        // TerrainParticleColor
        [ScriptProperty]
        public Color TerrainParticleColor { get; set; }

        // TerrainSound
        [ScriptProperty]
        public Sound? TerrainSound { get; set; }

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