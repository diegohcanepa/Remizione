using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Adberration.Scripting.Core
{
    /// <summary>
    /// ScriptRegistry
    /// </summary>
    public class ScriptRegistry
    {
        private readonly List<EntityInfo> entities = [];
        private readonly List<StatementInfo> statements = [];

        // Static constructor
        internal ScriptRegistry()
        {
            RegisterCoreTypes();
        }

        #region Private members

        // RegisterCoreTypes
        private void RegisterCoreTypes()
        {
            RegisterStatement(ScriptSyntax.ProperyReference, typeof(SetPropertyCommand));
            RegisterStatement("animate-opacity", typeof(AnimateOpacityCommand), CodingContext.Execution);
            RegisterStatement("animation", typeof(AnimationCommand), CodingContext.EntityDeclaration);
            RegisterStatement("await-animation", typeof(AwaitAnimationCommand), CodingContext.Execution);
            RegisterStatement("await-camera", typeof(AwaitCameraCommand), CodingContext.Execution);
            RegisterStatement("await", typeof(AwaitCommand), CodingContext.Execution);
            RegisterStatement("await-enter-room", typeof(AwaitEnterRoomCommand), CodingContext.Execution);
            RegisterStatement("await-move", typeof(AwaitMoveCommand), CodingContext.Execution);
            RegisterStatement("await-music", typeof(AwaitMusicCommand), CodingContext.Execution);
            RegisterStatement("await-opacity-tween", typeof(AwaitOpacityTweenCommand), CodingContext.Execution);
            RegisterStatement("await-outcome", typeof(AwaitOutcomeCommand), CodingContext.Execution);
            RegisterStatement("await-outcome-completion", typeof(AwaitOutcomeCompletionCommand), CodingContext.Execution);
            RegisterStatement("await-routine", typeof(AwaitRoutineCommand), CodingContext.Execution);
            RegisterStatement("await-script", typeof(AwaitScriptCommand), CodingContext.Execution);
            RegisterStatement("await-session-scene", typeof(AwaitSessionSceneCommand), CodingContext.Execution);
            RegisterStatement("await-shake", typeof(AwaitShakeCommand), CodingContext.Execution);
            RegisterStatement("await-sound", typeof(AwaitSoundCommand), CodingContext.Execution);
            RegisterStatement("await-sync-scripts", typeof(AwaitSyncScriptsCommand), CodingContext.Execution);
            RegisterStatement("await-this-routine", typeof(AwaitThisRountineCommand), CodingContext.Execution);
            RegisterStatement("await-transition", typeof(AwaitTransitionCommand), CodingContext.Execution);
            RegisterStatement("change-sound-settings", typeof(ChangeSoundSettingsCommand));
            RegisterStatement("color-tween", typeof(ColorTweenCommand));
            RegisterStatement("const", typeof(ConstCommand), CodingContext.Declaration);
            RegisterStatement(ScriptSyntax.MethodReference, typeof(CallMethodCommand));
            RegisterStatement("else", typeof(ElseStatement), CodingContext.Execution);
            RegisterStatement("endif", typeof(EndifStatement), CodingContext.Execution);
            RegisterStatement("flip", typeof(FlipCommand), CodingContext.Any);
            RegisterStatement("goto-animation-frame", typeof(GoToAnimationFrameCommand));
            RegisterStatement("if-counter", typeof(IfCounterStatement), CodingContext.Execution);
            RegisterStatement("if-entity", typeof(IfEntityStatement));
            RegisterStatement("if-entity-type", typeof(IfEntityTypeStatement));
            RegisterStatement("if-flag", typeof(IfFlagStatement), CodingContext.Execution);
            RegisterStatement("if-not-null", typeof(IfNotNullStatement));
            RegisterStatement("if-parent", typeof(IfParentStatement));
            RegisterStatement("if-random-number", typeof(IfRandomNumberStatement));
            RegisterStatement("if-roll", typeof(IfRollStatement));
            RegisterStatement("if-routine-running", typeof(IfRoutineRunningStatement));
            RegisterStatement("if", typeof(IfCoreStatement), CodingContext.Execution);
            RegisterStatement("restart", typeof(RestartStatement));
            RegisterStatement("return", typeof(ReturnStatement));
            RegisterStatement(ScriptSyntax.CloneKeyword, typeof(CloneCommand), CodingContext.Instantiation);
            RegisterStatement("counter", typeof(CounterCommand), CodingContext.Declaration);
            RegisterStatement("decrement-counter", typeof(DecrementCounterCommand));
            RegisterStatement("export-localizable-texts", typeof(ExportLocalizableTextsCommand));
            RegisterStatement("fade-sound", typeof(FadeSoundCommand));
            RegisterStatement("flag", typeof(FlagCommand), CodingContext.Declaration);
            RegisterStatement("focus", typeof(FocusCommand), CodingContext.Execution);
            RegisterStatement("focus-xy", typeof(FocusXYCommand), CodingContext.Execution);
            RegisterStatement("follow", typeof(FollowCommand), CodingContext.Execution);
            RegisterStatement("frame", typeof(FrameCommand), CodingContext.EntityDeclaration);
            RegisterStatement("generate-random-number", typeof(GenerateRandomNumberCommand), CodingContext.Any);
            RegisterStatement("increment-counter", typeof(IncrementCounterCommand));
            RegisterStatement("move", typeof(MoveCommand), CodingContext.Execution);
            RegisterStatement("opacity-tween", typeof(OpacityTweenCommand));
            RegisterStatement("pause-routine", typeof(PauseRoutineCommand), CodingContext.Execution);
            RegisterStatement("play-animation", typeof(PlayAnimationCommand));
            RegisterStatement("play-music", typeof(PlayMusicCommand));
            RegisterStatement("play-sound", typeof(PlaySoundCommand));
            RegisterStatement("play-music-tag", typeof(PlayMusicTagCommand));
            RegisterStatement("pop-scene", typeof(PopSceneCommand), CodingContext.Execution);
            RegisterStatement("position-tween", typeof(PositionTweenCommand));
            RegisterStatement("position-x-tween", typeof(XTweenCommand));
            RegisterStatement("position-y-tween", typeof(YTweenCommand));
            RegisterStatement("put", typeof(PutCommand));
            RegisterStatement("random-position", typeof(RandomPositionCommand));
            RegisterStatement("reload", typeof(ReloadCommand), CodingContext.Execution);
            RegisterStatement("reset-camera", typeof(ResetCameraCommand), CodingContext.Execution);
            RegisterStatement("reset-tweens", typeof(ResetTweensCommand));
            RegisterStatement("resume-routine", typeof(ResumeRoutineCommand), CodingContext.Execution);
            RegisterStatement("rotation-tween", typeof(RotationTweenCommand));
            RegisterStatement("save-game", typeof(SaveGameCommand), CodingContext.Execution);
            RegisterStatement("scale-tween", typeof(ScaleTweenCommand));
            RegisterStatement("set-achievement", typeof(SetAchievementCommand), CodingContext.Execution);
            RegisterStatement("set-counter", typeof(SetCounterCommand));
            RegisterStatement("set-flag", typeof(SetFlagCommand));
            RegisterStatement("set-music-tag", typeof(SetMusicTagCommand));
            RegisterStatement("shake", typeof(ShakeCommand), CodingContext.Execution);
            RegisterStatement("shake-horizontally", typeof(ShakeHorizontallyCommand), CodingContext.Execution);
            RegisterStatement("shake-vertically", typeof(ShakeVerticallyCommand), CodingContext.Execution);
            RegisterStatement("start-routine", typeof(StartRoutineCommand), CodingContext.Execution);
            RegisterStatement("stop-animation", typeof(StopAnimationCommand));
            RegisterStatement("stop-following", typeof(StopFollowingCommand), CodingContext.Execution);
            RegisterStatement("stop-moving", typeof(StopMovingCommand));
            RegisterStatement("stop-music", typeof(StopMusicCommand));
            RegisterStatement("stop-routine", typeof(StopRoutineCommand), CodingContext.Execution);
            RegisterStatement("stop-shaking", typeof(StopShakingCommand), CodingContext.Execution);
            RegisterStatement("stop-sound", typeof(StopSoundCommand), CodingContext.Execution);
            RegisterStatement("stop-vibration", typeof(StopVibrationCommand), CodingContext.Execution);
            RegisterStatement("suspend-input", typeof(SuspendInputCommand), CodingContext.Execution);
            RegisterStatement("suspend-vibration", typeof(SuspendVibrationCommand), CodingContext.Execution);
            RegisterStatement("toggle-flag", typeof(ToggleFlagCommand));
            RegisterStatement("transition", typeof(TransitionCommand));
            RegisterStatement("unparent", typeof(UnparentCommand));
            RegisterStatement("zoom", typeof(ZoomCommand));
        }

        #endregion

        // Entities
        public IEnumerable<EntityInfo> Entities => entities;

        // RegisterEntity
        public void RegisterEntity([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
        {
            // Check if type is an entity
            if (!typeof(Entity).IsAssignableFrom(type))
                throw new ArgumentException($"[{type.Name}] Type is not an entity.", nameof(type));

            entities.Add(new EntityInfo(type.Name, type));
        }

        // RegisterStatement
        public void RegisterStatement(string name, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, CodingContext context = CodingContext.Any)
        {
            // Check if type is an entity
            if (!typeof(Statement).IsAssignableFrom(type))
                throw new ArgumentException($"[{type.Name}]Type is not a statement.", nameof(type));

            statements.Add(new StatementInfo(name, type, context));
        }

        // Statements
        public IEnumerable<StatementInfo> Statements => statements;

        /// <summary>
        /// EntityInfo
        /// </summary>
        public sealed class EntityInfo
        {
            // Constructor
            internal EntityInfo(string name, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
            {
                this.Type = type;
                this.Name = name;
            }

            // Name
            internal string Name { get; }

            // Type
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
            internal Type Type { get; }
        }

        /// <summary>
        /// StatementInfo
        /// </summary>
        public sealed class StatementInfo
        {
            // Constructor
            internal StatementInfo(string name, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, CodingContext context)
            {
                this.Type = type;
                this.Name = name;
                this.Context = context;
            }

            // Context
            internal CodingContext Context { get; }

            // Name
            internal string Name { get; }

            // Type
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
            internal Type Type { get; }
        }
    }
}
