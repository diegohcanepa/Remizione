using Engendro;
using System;

namespace EngendroAdventure.Scripting
{
    /// <summary>
    /// Statement
    /// </summary>
    public abstract class Statement
    {
        #region Argument names

        protected static readonly string AbsoluteArg = "#absolute";
        protected static readonly string ActionArg = "#action";
        protected static readonly string AlignmentArg = "#alignment";
        protected static readonly string AmountArg = "#amount";
        protected static readonly string AnchorArg = "#anchor";
        protected static readonly string AngerArg = "#anger";
        protected static readonly string AnimationArg = "#animation";
        protected static readonly string AnimationNameArg = "#animation-name";
        protected static readonly string AllArg = "#all";
        protected static readonly string AreaArg = "#area";
        protected static readonly string AtArg = "#at";
        protected static readonly string AtlasFolderArg = "#atlas-folder";
        protected static readonly string AudioArg = "#audio";
        protected static readonly string AwaitArg = "#await";
        protected static readonly string BackgroundArg = "#background";
        protected static readonly string BehaviorArg = "#behavior";
        protected static readonly string BlockTagArg = "#block-tag";
        protected static readonly string BonusArg = "#bonus";
        protected static readonly string BottomArg = "#bottom";
        protected static readonly string BounceDelayArg = "#bounce-delay";
        protected static readonly string BouncesArg = "#bounces";
        protected static readonly string BoxArg = "#box";
        protected static readonly string CategoryArg = "#category";
        protected static readonly string ChanceArg = "#chance";
        protected static readonly string ClearArg = "#clear";
        protected static readonly string ClearTagArg = "#clear-tag";
        protected static readonly string CodeArg = "#code";
        protected static readonly string ColorArg = "#color";
        protected static readonly string ConditionArg = "#condition";
        protected static readonly string ConstraintArg = "#constraint";
        protected static readonly string CoordinatesArg = "#coordinates";
        protected static readonly string CostArg = "#cost";
        protected static readonly string CountArg = "#count";
        protected static readonly string CurrentArg = "#current";
        protected static readonly string CyclesArg = "#cycles";
        protected static readonly string DamageArg = "#damage";
        protected static readonly string DecimalsArg = "#decimals";
        protected static readonly string DefaultArg = "#default";
        protected static readonly string DeferredArg = "#deferred";
        protected static readonly string DelayArg = "#delay";
        protected static readonly string DepthOffsetArg = "#depth-offset";
        protected static readonly string DirectionArg = "#direction";
        protected static readonly string DirectionConstraintArg = "#direction-constraint";
        protected static readonly string DisableArg = "#disable";
        protected static readonly string DisabledArg = "#disabled";
        protected static readonly string DurationArg = "#duration";
        protected static readonly string DrawOwnerArg = "#draw-owner";
        protected static readonly string EnforceArg = "#enforce";
        protected static readonly string EntryPointArg = "#entry-point";
        protected static readonly string ExactArg = "#exact";
        protected static readonly string ExtendedArg = "#extended";
        protected static readonly string EmitterArg = "#emitter";
        protected static readonly string EnabledArg = "#enabled";
        protected static readonly string ExcludeArg = "#exclude";
        protected static readonly string ExitArg = "#exit";
        protected static readonly string FaceArg = "#face";
        protected static readonly string FadeArg = "#fade";
        protected static readonly string FadeModeArg = "#fade-mode";
        protected static readonly string FastArg = "#fast";
        protected static readonly string FileArg = "#file";
        protected static readonly string FlipArg = "#flip";
        protected static readonly string FocusArg = "#focus";
        protected static readonly string FolderNamesArg = "#folder-names";
        protected static readonly string FollowArg = "#follow";
        protected static readonly string FaithArg = "#faith";
        protected static readonly string FramesArg = "#frames";
        protected static readonly string FreeArg = "#free";
        protected static readonly string GlobalArg = "#global";
        protected static readonly string HitAreaArg = "#hit-area";
        protected static readonly string HPArg = "#hp";
        protected static readonly string IdArg = "#id";
        protected static readonly string ImageArg = "#image";
        protected static readonly string ImageOffsetArg = "#image-offset";
        protected static readonly string ImageScaleArg = "#image-scale";
        protected static readonly string ImmediateArg = "#immediate";
        protected static readonly string ImpactWordArg = "#impact-word";
        protected static readonly string IndexArg = "#index";
        protected static readonly string InstancesArg = "#instances";
        protected static readonly string IntensityArg = "#intensity";
        protected static readonly string IntervalArg = "#interval";
        protected static readonly string KeepArg = "#keep";
        protected static readonly string KindArg = "#kind";
        protected static readonly string KnockbackArg = "#knockback";
        protected static readonly string LabelArg = "#label";
        protected static readonly string LeftArg = "#left";
        protected static readonly string LeftTriggerArg = "#left-trigger";
        protected static readonly string LiteralArg = "#literal";
        protected static readonly string LocalizationIdArg = "#lid";
        protected static readonly string LockArg = "#lock";
        protected static readonly string LoopedArg = "#looped";
        protected static readonly string MasterVolumeArg = "#master-volume";
        protected static readonly string MaximumArg = "#maximum";
        protected static readonly string MaximumLevelArg = "#maximum-level";
        protected static readonly string MaximumWidthArg = "#maximum-width";
        protected static readonly string MusicArg = "#music";
        protected static readonly string MuteArg = "#mute";
        protected static readonly string NameArg = "#name";
        protected static readonly string NewArg = "#new";
        protected static readonly string NextArg = "#next";
        protected static readonly string NoAwaitArg = "#no-await";
        protected static readonly string NoCaptionArg = "#no-caption";
        protected static readonly string OffArg = "#off";
        protected static readonly string OffsetArg = "#offset";
        protected static readonly string OnceArg = "#once";
        protected static readonly string OnCompleteArg = "#on-complete";
        protected static readonly string OnExitArg = "#on-exit";
        protected static readonly string OnFailureArg = "#on-failure";
        protected static readonly string OnSuccessArg = "#on-success";
        protected static readonly string OpacityArg = "#opacity";
        protected static readonly string OverlayArg = "#overlay";
        protected static readonly string OwnerArg = "#owner";
        protected static readonly string PanArg = "#pan";
        protected static readonly string ParentArg = "#parent";
        protected static readonly string PauseAwareArg = "#pause-aware";
        protected static readonly string PassesArg = "#passes";
        protected static readonly string PercentageArg = "#percentage";
        protected static readonly string PersistentArg = "#persistent";
        protected static readonly string PitchArg = "#pitch";
        protected static readonly string PivotArg = "#pivot";
        protected static readonly string PlayerLevelArg = "#player-level";
        protected static readonly string PositionArg = "#position";
        protected static readonly string PrefixArg = "#prefix";
        protected static readonly string PreserveArg = "#preserve";
        protected static readonly string QuitArg = "#quit";
        protected static readonly string RadiansArg = "#radians";
        protected static readonly string RandomFrameArg = "#random-frame";
        protected static readonly string RangeArg = "#range";
        protected static readonly string RecurseArg = "#recurse";
        protected static readonly string RelativeArg = "#relative";
        protected static readonly string RelativeHotspotArg = "#relative-hotspot";
        protected static readonly string RelativePositionArg = "#relative-position";
        protected static readonly string RepeatArg = "#repeat";
        protected static readonly string ReplaceArg = "#replace";
        protected static readonly string ReverseArg = "#reverse";
        protected static readonly string RightArg = "#right";
        protected static readonly string RightTriggerArg = "#right-trigger";
        protected static readonly string RoomArg = "#room";
        protected static readonly string RunArg = "#run";
        protected static readonly string ScaleArg = "#scale";
        protected static readonly string ScopeArg = "#scope";
        protected static readonly string SessionArg = "#session";
        protected static readonly string SilentArg = "#silent";
        protected static readonly string SlotArg = "#slot";
        protected static readonly string SlowArg = "#slow";
        protected static readonly string SoundArg = "#sound";
        protected static readonly string SoundNameArg = "#sound-name";
        protected static readonly string SoundsArg = "#sounds";
        protected static readonly string SpeedFactorArg = "#speed-factor";
        protected static readonly string StartDelayArg = "#start-delay";
        protected static readonly string StateArg = "#state";
        protected static readonly string StepsArg = "#steps";
        protected static readonly string StrongArg = "#strong";
        protected static readonly string StyleArg = "#style";
        protected static readonly string SubtractArg = "#subtract";
        protected static readonly string TargetArg = "#target";
        protected static readonly string TempArg = "#temp";
        protected static readonly string ThingArg = "#thing";
        protected static readonly string TimesArg = "#times";
        protected static readonly string TitleArg = "#title";
        protected static readonly string TransientArg = "#transient";
        protected static readonly string TransitionAwareArg = "#transition-aware";
        protected static readonly string TypingArg = "#typing";
        protected static readonly string TweenArg = "#tween";
        protected static readonly string UnlockArg = "#unlock";
        protected static readonly string UnparentArg = "#unparent";
        protected static readonly string VelocityArg = "#velocity";
        protected static readonly string VibrateArg = "#vibrate";
        protected static readonly string VolatileArg = "#volatile";
        protected static readonly string VolumeArg = "#volume";
        protected static readonly string WalkAreaArg = "#walk-area";
        protected static readonly string WorldSizeArg = "#world-size";
        protected static readonly string ZeroPaddingArg = "#zero-padding";

        #endregion

        #region Constructor

        // Constructor
        protected Statement(Script script, StatementType statementType, string source, StatementBody body, int clauseCount)
        {
            this.Script = script;
            this.StatementType = statementType;
            this.Source = source;
            this.Body = body;
            this.ClauseCount = clauseCount;

            if (body.Clauses.Count != clauseCount && ClauseValidation)
                throw new ScriptException(this, $"You must specify {ClauseCount} clause(s).");

            if (this is not ConstCommand)
                CheckConstants();
        }

        #endregion

        #region Private members

        // CheckConstants
        private void CheckConstants()
        {
            for (var i = 0; i < Body.Clauses.Count; i++)
            {
                var token = Body.Clauses[i];

                if (ScriptEnvironment.IsConstant(token))
                {
                    var values = token.Split(',');

                    for (var j = 0; j < values.Length; j++)
                    {
                        if (!Session.ScriptEnvironment.IsConstantDeclared(values[j]))
                        {
                            throw new ScriptException(this, $"The constant '{values[j]}' is not declared.");
                        }
                    }
                }
            }
        }

        #endregion

        #region Protected members

        // AssertConstantName
        protected void AssertConstantName(int clauseIndex)
        {
            AssertConstantName(Body.Clauses[clauseIndex]);
        }

        // AssertConstantName
        protected void AssertConstantName(string name)
        {
            // Check if name starts with constant prefix
            if (!ScriptEnvironment.IsConstant(name))
            {
                throw new ScriptException(this, $"'{name}' is not a valid. Constant names must start with the '{ScriptSyntax.ConstantPrefix}' prefix.");
            }

            Parser.ParseName(this, name.Substring(1));
        }

        // AssertCounter
        protected Counter AssertCounter(int clauseIndex)
        {
            return AssertCounter(Body.Clauses[clauseIndex]);
        }

        // AssertCounter
        protected Counter AssertCounter(string name)
        {
            return Session.ScriptEnvironment.GetCounter(name) ?? throw new ScriptException($"Undeclared counter '{name}'.");
        }

        // AssertEntity
        protected T? AssertEntity<T>(int clauseIndex) where T : Entity
        {
            return Parser.ParseEntity<T>(this, clauseIndex);
        }

        // AssertEntity
        protected T? AssertEntity<T>(string name) where T : Entity
        {
            return Parser.ParseEntity<T>(this, name);
        }

        // AssertEntityNotNull
        protected T AssertEntityNotNull<T>(int clauseIndex) where T : Entity
        {
            return AssertEntityNotNull<T>(Body.Clauses[clauseIndex]);
        }

        // AssertEntityNotNull
        protected T AssertEntityNotNull<T>(string name) where T : Entity
        {
            var result = Parser.ParseEntity<T>(this, name) ?? throw ScriptExceptionBuilder.UnrecognizedEntity(this, name);
            return result;
        }

        // AssertEntityType
        protected Type AssertEntityClass(string className)
        {
            var type = Session.ScriptEnvironment.GetEntityType(className) ?? throw new ScriptException(this, $"'{className}' is not a valid registered entity type.");
            return type;
        }

        // AssertKeyword
        protected void AssertKeyword(int clauseIndex, params string[] matches)
        {
            for (var i = 0; i < matches.Length; i++)
            {
                if (Body.Clauses[clauseIndex] == matches[i])
                {
                    return;
                }
            }

            throw new ScriptException(this, $"'{Body.Clauses[clauseIndex]}' is not a valid keyword.");
        }

        // AssertRoutine
        protected Script? AssertRoutine(int clauseIndex)
        {
            return Parser.ParseRoutine(this, clauseIndex);
        }

        // AssertRoutine
        protected Script? AssertRoutine(string name)
        {
            return Parser.ParseRoutine(this, name);
        }

        // AssertRoutineNotNull
        protected Script AssertRoutineNotNull(int clauseIndex)
        {
            return AssertRoutineNotNull(Body.Clauses[clauseIndex]);
        }

        // AssertRoutineNotNull
        protected Script AssertRoutineNotNull(string name)
        {
            var result = Parser.ParseRoutine(this, name) ?? throw ScriptExceptionBuilder.ScriptNotFound(this, name);
            return result;
        }

        // CheckFlag
        protected Flag CheckFlag(string name)
        {
            return Session.ScriptEnvironment.GetFlag(name) ?? throw new ScriptException($"Undeclared flag '{name}'.");
        }

        // ClauseValidation
        protected virtual bool ClauseValidation => true;

        // RemoveQuotes
        protected string RemoveQuotes(int clauseIndex)
        {
            return Parser.RemoveQuotes(Body.Clauses[clauseIndex]);
        }

        #endregion

        // Body
        public StatementBody Body { get; }

        // ClauseCount
        public int ClauseCount { get; }

        // Game
        public EngendroGame Game => Script.Session.Game;

        // HasArg
        public bool HasArg(string argName) => Body.Args.Contains(argName);

        // HasArgs
        public bool HasArgs => Body.Args.Count > 0;

        // Name
        public string Name => Body.Owner;

        // Script
        public Script Script { get; }

        // Session
        public Session Session => Script.Session;

        // Source
        public string Source { get; }

        // StatementType
        public StatementType StatementType { get; }
    }
}
