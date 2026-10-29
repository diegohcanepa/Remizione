using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Engendro.PathFinding;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Remizione
{
    /// <summary> 
    /// GameThing 
    /// </summary>
    public abstract class GameThing : Thing, IHoleArea, ILightSource
    {
        #region Private fields

        private Vector2[] collisionVerticesBuffer = [];
        private bool dieCalled;
        private FloatTween? floatingTween;
        private readonly Polygon holePoly = new();
        private Vector2[] hotspotVerticesBuffer = [];
        private FlatMeter? hpMeter;
        private static readonly Vector2 hurtShakeForce = new(1.5f, 0);
        private Vector2Tween? hurtShakeTween;
        private FloatTween? hurtTween;
        private bool isCollisionDirty;
        private bool isHotspotDirty = true;
        private bool killedByPlayer;
        private Vector2 knockbackVelocity;
        private const float KnockbackFriction = 0.90f; // Ajustá este valor (0.8 - 0.95)
        private Vector2 lastCheckedPosition;
        private PathNode[]? pathNodes;
        private int renderLayerDepth;
        private readonly ShadowSpot shadowSpot;
        private bool shouldClampToWalkablePosition;
        private StatusSet? statuses;

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

        // BlocksLineOfSight
        bool IHoleArea.BlocksLineOfSight => BlocksLineOfSight;

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
        bool IHoleArea.IsActive => !Collider.IsEmpty && AffectsPathfinding && IsInCurrentRoom;

        // Polygon
        IPolygon IHoleArea.Polygon => holePoly;

        #endregion

        #region ILightSource implementation

        // DrawLights
        void ILightSource.DrawLights(GameTime gameTime, LightBlendMode blendMode)
        {
            if (AttachedLight?.BlendMode == blendMode)
            {
                if (AttachedLightPosition != Vector2.Zero)
                    AttachedLight.Position = this.GetAnchoredPosition(AttachedLightPosition);
                AttachedLight.Draw(gameTime);
            }
        }

        // IsEmittingLight
        bool ILightSource.IsEmittingLight => AttachedLight?.IsEmitting == true && !IgnoreAttachedLight;

        #endregion

        #region Private members

        // ApplySoftSeparation
        private void ApplySoftSeparation()
        {
            // Si YO tengo Collider rígido o estoy en knockback, no aplico separación suave
            if (IsDead || !Collider.IsEmpty || knockbackVelocity != Vector2.Zero || Room == null)
                return;

            // Leemos la masa de esta entidad
            float myMass = Mass;

            for (int i = 0; i < Room.CulledThings.Count; i++)
            {
                if (Room.CulledThings[i] is not GameThing other || other == this)
                    continue;

                // Solo nos separamos de cosas dinámicas sin Collider rígido
                if (other.IsDead || !other.Collider.IsEmpty || other.knockbackVelocity != Vector2.Zero)
                    continue;

                // Leemos la masa de la otra entidad
                float otherMass = other.Mass;

                // Semiejes
                float myRadiusX = CollisionSize.X * 0.5f;
                float myRadiusY = CollisionSize.Y * 0.5f;
                float aspectY = myRadiusX / MathF.Max(0.1f, myRadiusY);

                // Escalamos delta Y
                Vector2 diff = Position - other.Position;
                diff.Y *= aspectY;

                float distSq = diff.LengthSquared();
                float minDistX = myRadiusX + (other.CollisionSize.X * 0.5f);

                if (distSq > 0 && distSq < (minDistX * minDistX))
                {
                    float dist = MathF.Sqrt(distSq);

                    Vector2 pushDir = diff / dist;
                    pushDir.Y /= aspectY;

                    float overlap = minDistX - dist;

                    // --- CÁLCULO DE MASA APLICADO ---
                    float totalMass = myMass + otherMass;

                    // Cuanto más masa tiene el otro respecto al total, más me empuja a mí.
                    float myMoveRatio = totalMass > 0f ? (otherMass / totalMass) : 0.5f;

                    // Aplicamos separación (multiplicador base de ~0.4f para un tacto elástico)
                    Position += pushDir * (overlap * myMoveRatio * 0.4f);
                }
            }
        }

        // GetPassiveStatusModifier
        private int GetPassiveStatusModifier(StatusType statusType)
        {
            return 0;
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

            if (collisionVerticesBuffer.Length != vertexCount)
                collisionVerticesBuffer = new Vector2[vertexCount];

            var offset = ColliderPlacement == PlacementMode.Relative ? GetPivotBasedPolyOffset() : Vector2.Zero;
            Vector2 currentScale = ColliderPlacement == PlacementMode.Relative ? Scale : Vector2.One;

            // Usamos el buffer reutilizado
            Collider.GetVertices(collisionVerticesBuffer, offset, currentScale);

            holePoly.SetVertices(collisionVerticesBuffer);
            RuntimeCollider.SetVertices(collisionVerticesBuffer, .05f);

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

        // ProcessDamageFlyOff
        private void ProcessDamageFlyOff(int amount, DamageType damageType)
        {
            var color = Colors.Palette.Text;
            var damageAmount = Math.Abs(amount);

            if (IsPlayer)
            {
                for (var i = 0; i < damageAmount; i++)
                {
                    ShowFlyOff(Atlases.UI.HeartIcon, ScaleInfo.UIElement.Tiny.X);
                }
            }
            else
            {
                //ShowFlyOff(damageAmount.ToString(CultureInfo.InvariantCulture), color, ScaleInfo.Text.Tiny.X);
            }
        }

        // ResolveStaticCollisions
        private void ResolveStaticCollisions()
        {
            if (Room == null || !CollisionDetection)
                return;

            float radiusX = CollisionSize.X * .5f;
            float radiusY = CollisionSize.Y * .5f;
            float aspectY = radiusX / MathF.Max(.1f, radiusY);
            float radiusSq = radiusX * radiusX;

            for (int i = 0; i < Room.CulledThings.Count; i++)
            {
                if (Room.CulledThings[i] is not GameThing thing || thing == this)
                    continue;

                if (!thing.CollisionDetection || thing.Collider.IsEmpty)
                    continue;

                if (Altitude > thing.CollisionHeight)
                    continue;

                var poly = thing.RuntimeCollider;

                // --- BROAD-PHASE: Descarte AABB $O(1)$ ---
                // Ampliamos el BoundingBox del polígono por el radio de la entidad
                var bounds = poly.BoundingRectangleF;
                if (Position.X < bounds.Left - radiusX || Position.X > bounds.Right + radiusX ||
                    Position.Y < bounds.Top - radiusY || Position.Y > bounds.Bottom + radiusY)
                {
                    continue; // Está fuera del área de influencia: omitimos Raycasts y pases de aristas
                }

                int vertexCount = poly.Vertices.Count;
                if (vertexCount < 2)
                    continue;

                Vector2 initialScaledPos = new(Position.X, Position.Y * aspectY);
                Vector2 scaledPos = initialScaledPos;

                // 1. Failsafe: Solo se ejecuta si el AABB confirmó proximidad
                if (poly.Contains(Position))
                {
                    Vector2 closestReal = poly.GetClosestPointOnEdge(Position);
                    Vector2 scaledClosest = new(closestReal.X, closestReal.Y * aspectY);

                    Vector2 escapeDir = scaledClosest - scaledPos;
                    float dist = escapeDir.Length();

                    if (dist > 0.0001f)
                    {
                        escapeDir /= dist;
                        scaledPos = scaledClosest + (escapeDir * radiusX);
                    }
                    else
                    {
                        scaledPos.Y -= radiusX;
                    }
                }

                // 2. Resolución de aristas (Narrow-Phase)
                for (int iter = 0; iter < 2; iter++)
                {
                    bool movedThisIter = false;

                    for (int v = 0; v < vertexCount; v++)
                    {
                        Vector2 p1 = poly.Vertices[v];
                        Vector2 p2 = poly.Vertices[(v + 1) % vertexCount];

                        Vector2 sp1 = new(p1.X, p1.Y * aspectY);
                        Vector2 sp2 = new(p2.X, p2.Y * aspectY);

                        Vector2 closestOnSegment = Geometry.GetClosestPointOnSegment(scaledPos, sp1, sp2);
                        Vector2 delta = scaledPos - closestOnSegment;
                        float dSq = delta.LengthSquared();

                        if (dSq > 0f && dSq < radiusSq)
                        {
                            float dist = MathF.Sqrt(dSq);
                            Vector2 pushDir = delta / dist;
                            float overlap = radiusX - dist;

                            scaledPos += pushDir * overlap;
                            movedThisIter = true;
                        }
                    }

                    if (!movedThisIter)
                        break;
                }

                // 3. Aplicación final de posición
                if (scaledPos != initialScaledPos)
                {
                    Position = new Vector2(scaledPos.X, scaledPos.Y / aspectY);
                    OnCollision(thing);
                }
            }
        }

        // SyncHPMeter
        private void SyncHPMeter()
        {
            if (MaxHP < 2)
                return;

            hpMeter ??= CreateHPMeter();
            hpMeter.MaximumValue = MaxHP;
            hpMeter.Value = HP;
        }

        // UpdateKnockback
        private void UpdateKnockback(GameTime gameTime)
        {
            if (knockbackVelocity == Vector2.Zero)
                return;

            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += knockbackVelocity * dt;
            knockbackVelocity *= MathF.Pow(KnockbackFriction, dt * 60f);
            if (knockbackVelocity.LengthSquared() < 100f)
            {
                knockbackVelocity = Vector2.Zero;
                if (IsDead)
                    Die();
            }
        }

        #endregion

        #region Protected members

        // BlocksLineOfSight
        protected virtual bool BlocksLineOfSight => false;

        // CanCheckCollisions
        protected virtual bool CanCheckCollisions()
        {
            return CollisionDetection;
        }

        // CreateHPMeter
        protected virtual FlatMeter CreateHPMeter()
        {
            var back = Colors.HPMeterActor.Back;
            var border = Colors.HPMeterActor.Border;
            var fore = Colors.HPMeterActor.Fore;
            var diff = Colors.HPMeterActor.Diff;

            return new(border, back, fore, diff, new(10, 3), .75f);
        }

        // DropLoot
        protected void DropLoot()
        {
            if (Session.Room is not GameRoom room || Definition == null)
                return;

            ItemReward ??= Session.LootGenerator.RollForLoot(Definition);

            if (ItemReward != null)
                ItemOrb.Drop(ItemReward, 1, room, Position);

            ItemReward = null;
        }

        // GetHurtShakeOffset
        protected Vector2 GetHurtShakeOffset()
        {
            return hurtShakeTween?.IsRunning == true ? hurtShakeTween.CurrentValue : Vector2.Zero;
        }

        // GetKnockbackMultiplier
        protected virtual float GetKnockbackMultiplier(GameThing target)
        {
            return 1;
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

            if (hurtShakeTween != null && hurtShakeTween.IsRunning)
                Position += hurtShakeTween.CurrentValue;

            base.OnDraw(gameTime);

            if (floatingTween != null && floatingTween.IsRunning)
                Altitude -= floatingTween.CurrentValue;

            if (hurtShakeTween != null && hurtShakeTween.IsRunning)
                Position -= hurtShakeTween.CurrentValue;
        }

        // OnDrawShadow
        protected virtual void OnDrawShadow(GameTime gameTime)
        {
            if (!IsDead)
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

        // OnItemRewardChanged
        protected virtual void OnItemRewardChanged()
        {
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            if (shadowSpot.Size > 0 && shadowSpot.AnchorPosition == Vector2.Zero && Sprite.RenderImage != null)
                shadowSpot.AnchorPosition = new Vector2(Sprite.Width / 2, Sprite.Height - .5f);

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

            if (hpMeter != null)
            {
                hpMeter.Update(gameTime);
                hpMeter.Position = GetOverheadPosition(new(0, -3));
            }

            // 1. Físicas de knockback y estados de muerte (fuerzas mayores)
            if (knockbackVelocity != Vector2.Zero)
            {
                UpdateKnockback(gameTime);
            }
            else if (IsDead && !dieCalled)
            {
                Die();
            }

            base.OnUpdate(gameTime);

            // 2. Lógica de Colisiones y Navegación (Solo para entidades dinámicas)
            if (!IsStatic)
            {
                if (shouldClampToWalkablePosition || Position != lastCheckedPosition)
                {
                    ClampToWalkablePosition(); // WalkArea boundaries
                    shouldClampToWalkablePosition = false;
                }

                // 3. Resolución de escenario estático (Círculo vs Polígono)
                if (CanCheckCollisions())
                    ResolveStaticCollisions();

                // 4. Separación de entidades (Círculo vs Círculo)
                ApplySoftSeparation();

                lastCheckedPosition = Position;
            }

            AttachedLight?.Update(gameTime);
        }

        // OnUpdateEmittingSound
        protected override void OnUpdateEmittingSound(SoundInstance instance, float masterVolume)
        {
            Utils.ApplySoundEmitter(this, instance, masterVolume);
        }

        #endregion

        // AffectsPathfinding
        [ScriptProperty]
        public bool AffectsPathfinding { get; set; } = true;

        // AllowInteraction
        [ScriptProperty]
        public bool AllowInteraction { get; set; } = true;

        // ApplyStatus
        public void ApplyStatus(StatusType statusType, int amount)
        {
            if (IsDead)
                return;

            var passiveMod = GetPassiveStatusModifier(statusType);
            var finalAmount = amount - passiveMod;

            if (finalAmount <= 0)
                return;

            statuses ??= new StatusSet();

            StatusVersion++;

            if (statuses.ApplyAmount(statusType, finalAmount))
            {
                TakeDamage(this, DamageType.Status, 1, Vector2.Zero);
                ShowFlyOff(Localization.GetValue(statusType), Colors.Palette.Red, ScaleInfo.Text.Medium.X);
            }
            else if (Atlases.UI.FindImage($"{statusType}Icon") is { } statusIcon)
            {
                for (int i = 0; i < amount; i++)
                {
                    ShowFlyOff(statusIcon, ScaleInfo.UIElement.Tiny.X);
                }
            }
        }

        // ApproachBehavior
        [ScriptProperty]
        public ApproachBehavior ApproachBehavior { get; set; }

        // AttachedLight
        public Light? AttachedLight { get; set; }

        // AttachedLightPosition
        public Vector2 AttachedLightPosition { get; set; }

        // BodySize
        [ScriptProperty]
        public BodySize BodySize
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    this.Mass = field switch
                    {
                        BodySize.Small => 10,
                        BodySize.Medium => 30,
                        BodySize.Large => 100,
                        _ => 30
                    };
                }
            }
        } = BodySize.Medium;

        // CanBeHit
        public virtual bool CanBeHit()
        {
            return IsHittable || (MaxHP > 0 && !IsDead);
        }

        // CanInteract
        public virtual bool CanInteract()
        {
            if (Verb == Verb.None || Session.Player == null)
                return false;

            if (!AllowInteraction)
                return false;

            if (IsDead || string.IsNullOrWhiteSpace(Label))
                return false;

            if (Session.Player.MaxHP == 0)
                return false;

            return InteractCondition == null || InteractCondition.Evaluate();
        }

        // CanTakeDamage
        public virtual bool CanTakeDamage()
        {
            return !IsDead;
        }

        // ClampToWalkablePosition
        public void ClampToWalkablePosition()
        {
            if (IgnoreWalkArea)
                return;

            if (WalkArea != null)
            {
                Position = WalkArea.ClampInside(Position, out _);

                for (int i = 0; i < WalkArea.Holes.Count; i++)
                {
                    if (WalkArea.Holes[i].Test())
                        Position = WalkArea.Holes[i].ClampOutside(Position);
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

        // CollisionSize
        [ScriptProperty]
        public Vector2 CollisionSize { get; set; } = new(4, 2);

        // CollisionDetection
        [ScriptProperty]
        public bool CollisionDetection { get; set; } = true;

        // CollisionHeight
        [ScriptProperty]
        public int CollisionHeight { get; set; }

        // Definition
        public virtual ThingDefinition? Definition { get; }

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

            if (!IsPlayer)
            {
                if (killedByPlayer)
                {
                    if (Definition?.GraceReward > 0)
                    {
                        Sound.Play(SoundNames.GraceGain);
                        Session.PlayerData.Grace += Definition.GraceReward;
                        var text = $"+{Definition.GraceReward} {Localization.GetValue(PlayerStat.Grace)}";
                        Session.ObjectPools.FlyOffs.Get()?.ShowText(GetOverheadPosition(), text, Colors.Palette.Grace, ScaleInfo.Text.Medium.X, 1000);
                    }

                    if (Definition?.MaterialReward != null)
                        Session.PlayerData.Materials.Add(Definition.MaterialReward.Value, 1);
                }

                DropLoot();
            }
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
                Game.Shapes.DrawFrame(BoundingBox, Color.Green * .3f, .25f);
        }

        // ShowBoundingBoxes
        public static bool ShowBoundingBoxes { get; set; }

        // ShowColliders
        public static bool ShowColliders { get; set; }

        // ShowHotspots
        public static bool ShowHotspots { get; set; }
#endif

        // DeathSound
        [ScriptProperty]
        public Sound? DeathSound { get; set; }

        // DistanceToTarget
        public float DistanceToTarget(GameThing target)
        {
            if (target.X < X)
                return Vector2.Distance(target.RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.RightBottom), RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.LeftBottom));
            else
                return Vector2.Distance(target.RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.LeftBottom), RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.RightBottom));
        }

        // DrawHPMeter
        public void DrawHPMeter(GameTime gameTime)
        {
            if (hpMeter == null || (HideHPMeter && !hpMeter.IsAnimating))
                return;

            if (!IsPlayer && !IsDead)
            {
                if (Session.InteractionContext.Target == this || hpMeter.IsAnimating)
                    hpMeter.Draw(gameTime);
            }
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
            {
                if (target.Sprite.RenderImage == null)
                    FaceTo(target.RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.Bottom));
                else
                    FaceTo(target.Position);
            }
        }

        // FaceTo
        public void FaceTo(Vector2 position)
        {
            Direction = X < position.X ? FacingDirection.Right : FacingDirection.Left;
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
                        floatingTween.Start(TweenStyle.CubicInOut, 0, field, 1000, -1);
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

            if (behavior == ApproachBehavior.None)
                return Vector2.Zero;

            // 1. Override Manual (Prioridad absoluta del editor)
            if (StandPoint != Vector2.Zero)
            {
                if (HotspotPlacement == PlacementMode.Absolute || behavior is ApproachBehavior.StandPoint)
                    return StandPoint;
                else
                    return this.GetAnchoredPosition(StandPoint);
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
                case ApproachBehavior.Behind:
                    // "Párate a mi espalda"
                    if (Direction == FacingDirection.Left)
                        targetX = myBox.Right + spacing;  // Estoy mirando izq -> mi espalda está a la derecha
                    else
                        targetX = myBox.Left - spacing;   // Estoy mirando der -> mi espalda está a la izquierda
                    break;

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
            return GetOverheadPosition(Vector2.Zero);
        }

        // GetOverheadPosition
        public Vector2 GetOverheadPosition(Vector2 offset)
        {
            // Origin
            if (OverheadOrigin == Vector2.Zero)
                return BoundingBox.GetPoint(RectanglePoint.Top, offset);
            else
                return this.GetAnchoredPosition(OverheadOrigin + offset);
        }

        /// <summary>
        /// Calcula la posición navegable más cercana para realizar un ataque de rango/lanzamiento,
        /// respetando la tolerancia vertical en Y y asegurando que el personaje quede dentro del 
        /// rango permitido en X (ni demasiado lejos ni demasiado cerca).
        /// </summary>
        public Vector2? GetRangedApproachPosition(Actor actor, float maxDistanceX, float minDistanceX, float yTolerance)
        {
            Vector2 targetPos = this.Position;
            Vector2 playerPos = actor.Position;

            // 1. Determinar el lado (izquierda o derecha) donde está el jugador respecto al target
            float directionX = playerPos.X >= targetPos.X ? 1f : -1f;

            // 2. Definir la Y objetivo:
            // Si el jugador ya está dentro de la franja de tolerancia Y, mantiene su Y actual.
            // Si está fuera, busca la frontera más cercana de esa franja.
            float targetY = playerPos.Y;
            float deltaY = Math.Abs(playerPos.Y - targetPos.Y);

            if (deltaY > yTolerance)
            {
                targetY = playerPos.Y > targetPos.Y
                    ? targetPos.Y + yTolerance
                    : targetPos.Y - yTolerance;
            }

            // 3. Definir la X objetivo:
            float deltaX = Math.Abs(playerPos.X - targetPos.X);
            float targetX = playerPos.X;

            // Si la distancia X actual es mayor al rango máximo, se acerca al límite exterior.
            if (deltaX > maxDistanceX)
            {
                targetX = targetPos.X + (directionX * maxDistanceX);
            }
            // Si está demasiado cerca del target (menos que la distancia mínima), retrocede hasta la distancia mínima.
            else if (deltaX < minDistanceX)
            {
                targetX = targetPos.X + (directionX * minDistanceX);
            }

            var candidatePosition = new Vector2(targetX, targetY);

            // 4. Validar si la posición candidata es navegable en el mapa.
            var gameRoom = actor.Room;
            var walkArea = gameRoom?.WalkArea;

            if (walkArea == null)
                return null;

            if (walkArea.IsWalkableAt(candidatePosition))
                return candidatePosition;

            // Fallback: si el punto exacto no es caminable (ej. hay una pared detrás al retroceder), busca el punto libre más próximo.
            return walkArea.GetWalkablePoint(candidatePosition);
        }

        // GetStatusValue
        public int GetStatusValue(StatusType statusType)
        {
            return statuses?.GetValue(statusType) ?? 0;
        }

        // GraceCost
        [ScriptProperty]
        public int GraceCost { get; set; }

        // HasLineOfSightTo
        public bool HasLineOfSightTo(GameThing target)
        {
            return Room?.WalkArea == null || Room.WalkArea.HasLineOfSight(Position, target.Position, RaycastContext.LineOfSight, this, out _);
        }

        // HideHPMeter
        [ScriptProperty]
        public bool HideHPMeter { get; set; }

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

        // HomePosition
        public Vector2 HomePosition { get; set; }

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
                    SyncHPMeter();
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

        // InteractCondition
        [ScriptProperty]
        public FlagCondition? InteractCondition { get; set; }

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
        [ScriptProperty]
        public bool IsDead => (HP <= 0 && MaxHP > 0) || (HP == int.MinValue);

        // IsDealingContactDamage
        public bool IsDealingContactDamage { get; set; }

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
        public bool IsHittable { get; set; }

        // IsKnockbackInProgress
        public bool IsKnockbackInProgress => knockbackVelocity != Vector2.Zero;

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

        // IsPlayer
        [ScriptProperty]
        public bool IsPlayer => Session.Player == this;

        // IsStatic
        [ScriptProperty]
        public bool IsStatic { get; set; }

        // ItemReward
        public ItemDefinition? ItemReward
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    ItemRewardAmount = field == null ? 0 : 1;
                    OnItemRewardChanged();
                }
            }
        }

        // ItemRewardAmount
        public int ItemRewardAmount
        {
            get;
            set
            {
                if (value != field)
                    field = Math.Max(0, value);
            }
        }

        // ItemRewardName
        [ScriptProperty]
        public string ItemRewardName
        {
            get => ItemReward?.Name ?? string.Empty;
            set
            {
                if (value != ItemRewardName)
                {
                    ItemReward = GameData.Items.Find(value);
                }
            }
        }

        // Label
        public virtual string Label { get; private set; } = string.Empty;

        // LabelKey
        [ScriptProperty]
        public string LabelKey
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Label = TextRepository.GetValue(LabelKey);
                }
            }
        } = string.Empty;

        // Mass
        public float Mass { get; private set; } = 30f;

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
                        HP = field;
                    }
                    else if (HP > field)
                    {
                        HP = field;
                    }

                    SyncHPMeter();
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

                        int vertexCount = Hotspot.Vertices.Count;

                        // Validación y reúso del buffer
                        if (hotspotVerticesBuffer.Length != vertexCount)
                        {
                            hotspotVerticesBuffer = new Vector2[vertexCount];
                        }

                        Hotspot.GetVertices(hotspotVerticesBuffer, offset, Scale);
                        field.SetVertices(hotspotVerticesBuffer);

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

        // ShowFlyOff
        public FlyOff? ShowFlyOff(AtlasImage image, float scale = 1)
        {
            if (Session.ObjectPools.FlyOffs.Get() is FlyOff flyOff)
            {
                flyOff.ShowIcon(GetOverheadPosition(), image, scale);
                return flyOff;
            }

            return null;
        }

        // ShowFlyOff
        public FlyOff? ShowFlyOff(string text, Color color)
        {
            return ShowFlyOff(text, color, ScaleInfo.Text.Small.X);
        }

        // ShowFlyOff
        public FlyOff? ShowFlyOff(string text, Color color, float scale)
        {
            if (Session.ObjectPools.FlyOffs.Get() is FlyOff flyOff)
            {
                flyOff.ShowText(GetOverheadPosition(new(0, -3)), text, color, scale, 2500);
                return flyOff;
            }

            return null;
        }

        // StandPoint
        [ScriptProperty]
        public Vector2 StandPoint { get; set; }

        // StatusVersion
        public int StatusVersion { get; set; }

        // TakeDamage
        public int TakeDamage(GameThing attacker, DamageType damageType, int amount, Vector2 knockbackForce)
        {
            // Si la cantidad es 0 o negativa, no hay interacción de daño.
            if (!CanTakeDamage())
                return 0;

            StopMoving();

            // ---------------------------------------------------------
            // 1. FEEDBACK INICIAL
            // ---------------------------------------------------------
            // Esto ocurre SIEMPRE que hay un impacto válido, aunque sea indestructible.

            // Sonido de dolor o impacto
            if (HurtSound != null)
                PlaySound(HurtSound);

            // Shake: El objeto tiembla por el golpe (incluso una pared dura puede vibrar)
            hurtShakeTween ??= new();
            hurtShakeTween.Start(TweenStyle.Linear, Vector2.Zero, hurtShakeForce, 40, 4);

            // ---------------------------------------------------------
            // 2. LÓGICA DE SALUD (Solo si es Destructible)
            // ---------------------------------------------------------

            // Aquí es donde manejamos el MaxHP == 0
            if (MaxHP > 0)
            {
                // Aplicar resistencias
                //amount = (int)(amount * GetResistanceModifier(damageType));

                // Clamp para no restar más de lo que tiene
                if (amount > HP)
                    amount = HP;

                // Si después de la resistencia el daño es 0, salimos de la lógica de HP
                //if (amount > 0)
                {
                    HP -= amount;

                    if (IsDead)
                    {
                        killedByPlayer = attacker.IsPlayer;
                    }
                    else
                    {
                        hurtTween ??= new();
                        hurtTween.Start(TweenStyle.Linear, 0, 1, 150, 2);
                    }

                    OnTakeDamage(attacker, amount, damageType);

                    if (amount > 0 && MaxHP > 1 && !IsDead && damageType != DamageType.Status)
                        ProcessDamageFlyOff(amount, damageType);
                }
            }
            else
            {
                // Lógica para Indestructibles (MaxHP == 0)
                // Opcional: Sonido de "Metal/Rebote" o palabra "BLOCK"
            }

            // ---------------------------------------------------------
            // 3. Knockback
            // ---------------------------------------------------------
            if (MaxHP > 0 && knockbackForce != Vector2.Zero && !IgnoreKnockback)
            {
                knockbackForce *= attacker.GetKnockbackMultiplier(this);

                Vector2 pushDirection = Position - attacker.Position;
                pushDirection = pushDirection != Vector2.Zero
                    ? Vector2.Normalize(pushDirection)
                    : new Vector2(1, 0);

                float calculatedSpeed = knockbackForce.Length() * 5f;

                // Descartar impulsos insignificantes (deadzone) en lugar de inflarlos
                if (calculatedSpeed >= 2)
                    knockbackVelocity = pushDirection * calculatedSpeed;
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
        public Verb Verb { get; set; }

        // WalkArea
        public WalkArea? WalkArea
        {
            get => field ?? Room?.WalkArea;
            private set;
        }

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