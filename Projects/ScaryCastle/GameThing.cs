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
        private const int contactCooldown = 500;
        private int contactTimer;
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
        private int statusEffectTimer;

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
            this.CombatBehavior = CombatBehavior.Behaviors.Find(DeclaredName);
            this.ContactIntent = CombatBehavior?.Intents.Find(EffectContext.Contact.ToString());
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
                DisplaySentence = DisplayName;
        }

        // UpdateStatusEffect
        private void UpdateStatusEffect(GameTime gameTime)
        {
            if (StatusEffect == StatusEffectType.None)
                return;

            if (statusEffectTimer > 0)
            {
                statusEffectTimer -= gameTime.ElapsedGameTime.Milliseconds;
                if (statusEffectTimer <= 0)
                {
                    if (StatusEffectAmount > 0)
                    {
                        StatusEffectAmount -= 1;
                        HP -= 1;
                        statusEffectTimer = GameSettings.StatusEffectCooldown;
                        Sound.Play(SoundNames.StatusEffectDamage);
                        ShowComicText(ComicTextKind.AghGreen);
                    }
                }
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
            if (!Session.LootGenerator.TryDropLoot(this))
                Session.LootGenerator.TryDropCoins(this);
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
            contactTimer = 0;
        }

        // OnApplyStatusEffect
        protected virtual void OnApplyStatusEffect(StatusEffectType statusEffect, int amount)
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

        // OnHPChanged
        protected virtual void OnHPChanged()
        {
        }

        // OnIgnoreAttachedLightChanged
        protected virtual void OnIgnoreAttachedLightChanged()
        {
        }

        // OnIsAttackableChanged
        protected virtual void OnIsAttackableChanged()
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

        // OnStopMoving
        protected override void OnStopMoving()
        {
            base.OnStopMoving();
            contactTimer = contactCooldown;
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

            if (ContactIntent != null)
            {
                if (contactTimer > 0)
                {
                    contactTimer -= gameTime.ElapsedGameTime.Milliseconds;
                }
                else if (Session.Player != null && IsMoving)
                {
                    if (RuntimeCollider.Contains(Session.Player.Position))
                    {
                        TryInflictContactDamage(Session.Player);
                        contactTimer = 500;
                        return;
                    }
                }
            }

            UpdateStatusEffect(gameTime);
        }

        // OnUpdateEmittingSound
        protected override void OnUpdateEmittingSound(SoundInstance instance, float masterVolume)
        {
            Utils.ApplySoundEmitter(this, instance, masterVolume);
        }

        // TryInflictContactDamage
        protected virtual void TryInflictContactDamage(GameThing target)
        {
            if (ContactIntent != null)
            {
                EffectDescriptor.Apply(ContactIntent.EffectDescriptors, this, target, EffectContext.Contact);
            }
        }

        #endregion

        // AffectsPathfinding
        [ScriptProperty]
        public bool AffectsPathfinding { get; set; } = true;

        // AllowInteraction
        [ScriptProperty]
        public bool AllowInteraction { get; set; } = true;

        // ApplyStatusEffect
        public void ApplyStatusEffect(StatusEffectType statusEffect, int amount, ComicTextKind comicTextKind)
        {
            // 1. Clear status effect
            if (statusEffect == StatusEffectType.None)
            {
                ClearStatusEffect();
                return;
            }

            // 2. Si el efecto entrante es Maldición: PISA el veneno o SE SUMA a una maldición previa.
            if (statusEffect == StatusEffectType.Curse)
            {
                if (StatusEffect != StatusEffectType.Curse)
                {
                    StatusEffect = StatusEffectType.Curse;
                    StatusEffectAmount = amount;
                    statusEffectTimer = GameSettings.StatusEffectCooldown;
                }
                else
                {
                    StatusEffectAmount += amount; // Ya estaba maldito, se acumula.
                    if (StatusEffectAmount > HP)
                        HP -= 1;
                }

                if (Session.Player == this)
                {
                    Session.HUD.Message.Show(MessageKind.Cursed, 2000);
                    Session.ObjectPools.FloatingTexts.Get()?.ShowAmount(this, ColorPalette.Text.Purple, amount);
                }
            }

            // 3. Si el efecto entrante es Veneno: Solo importa si no estás maldito.
            if (statusEffect == StatusEffectType.Poison && StatusEffect != StatusEffectType.Curse)
            {
                if (StatusEffect != StatusEffectType.Poison)
                {
                    StatusEffect = StatusEffectType.Poison;
                    StatusEffectAmount = amount;
                    statusEffectTimer = GameSettings.StatusEffectCooldown;
                }
                else
                {
                    StatusEffectAmount += amount; // Ya estaba envenenado, se acumula.
                    if (StatusEffectAmount > HP)
                        HP -= 1;
                }

                if (Session.Player == this)
                {
                    Session.HUD.Message.Show(MessageKind.Poisoned, 2000);
                    Session.ObjectPools.FloatingTexts.Get()?.ShowAmount(this, ColorPalette.Text.Green, amount);
                }
            }

            // ComicText si hubo daño real
            if (comicTextKind != ComicTextKind.None)
            {
                if (!IsDead || DeathWord == ComicTextKind.None)
                    ShowComicText(comicTextKind);
            }

            OnApplyStatusEffect(statusEffect, amount);
        }

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
        [ScriptProperty]
        public bool CanBeHit { get; set; } = true;

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

        // CombatBehavior
        public CombatBehavior? CombatBehavior { get; }

        // ContactIntent
        public CombatIntent? ContactIntent { get; }

        // ClearStatusEffect
        public void ClearStatusEffect()
        {
            this.StatusEffect = StatusEffectType.None;
            this.StatusEffectAmount = 0;
        }

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

        // DropMode
        [ScriptProperty]
        public LootDropMode DropMode { get; set; } = LootDropMode.Standard;

        // DropCoinChanceBonus
        public Ratio DropCoinChanceBonus { get; set; }

        // DropSackChanceBonus
        public Ratio DropSackChanceBonus { get; set; }

        // FaceTo
        public void FaceTo(GameThing target)
        {
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

        // GetFootstepSound
        public Sound? GetFootstepSound(Vector2 position)
        {
            return RuntimeCollider?.Contains(position) == true ? TerrainSound : null;
        }

        // GetMouseCursor
        public virtual MouseCursorState? GetMouseCursor()
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
                return this.GetAnchoredPosition(OverheadOrigin);
        }

        // GetResistanceModifier
        public float GetResistanceModifier(DamageType damageType)
        {
            if (ResistanceTable.Find(ResistanceTableName) is ResistanceTable table)
                return table.GetModifier(damageType);

            return 1;
        }

        // Heal
        public void Heal(int amount)
        {
            var current = HP;
            HP += amount;
            Session.ObjectPools.FloatingTexts.Get()?.ShowHealingAmount(this, Math.Abs(current - HP));
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
                    field = Math.Min(value, MaxHP);

                    if (field > 0)
                        dieCalled = false;

                    OnHPChanged();
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

        // IgnoreWalkArea
        [ScriptProperty]
        public bool IgnoreWalkArea { get; set; } = true;

        // IsAttackable
        [ScriptProperty]
        public bool IsAttackable
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    OnIsAttackableChanged();
                }
            }
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

        // IsBlinking
        public bool IsBlinking => blinker.IsRunning && blinker.CurrentValue;

        // IsCornered
        public bool IsCornered(GameThing target)
        {
            if (Room?.WalkArea == null)
                return false;

            var destX = Direction == FacingDirection.Left ? int.MaxValue : int.MinValue;
            var destination = Room.WalkArea.ClampInside(new(destX, Y));

            var distanceToTarget = float.MaxValue;
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

        // StatusEffect
        public StatusEffectType StatusEffect { get; private set; }

        // StatusEffectAmount
        [ScriptProperty]
        public int StatusEffectAmount
        {
            get;
            private set
            {
                if (value != field)
                {
                    field = value;
                    if (field < 0)
                        field = 0;
                }
            }
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
            if (knockbackForce != Vector2.Zero)
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