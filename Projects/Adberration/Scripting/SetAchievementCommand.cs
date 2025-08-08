using Engendro;

namespace Adberration.Scripting
{
    // SetAchievementCommand
    // Arguments: {Id:String}
    internal sealed class SetAchievementCommand : NonAwaitableCommand
    {
        // Constructor
        internal SetAchievementCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            var achievementId = body.Clauses[0];
            if (AchievementManager.GetAchievement(achievementId) == null)
                throw new ScriptException(this, $"Achievement not found: '{achievementId}'.");
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (!Session.IsDemo)
                Session.Game.PlatformBridge.SetAchievement(Body.Clauses[0]);
        }
    }
}
