using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // UseItemCommand
    // Arguments: {ItemName} animation {AnimationName}
    [ForceAwait]
    internal sealed class UseItemCommand : NonAwaitableCommand
    {
        private readonly string animationName;
        private readonly ItemDefinition definition;

        // Constructor
        internal UseItemCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 3)
        {
            definition = Script.AssertItemDefinition(Body.Clauses[0]);
            AssertKeyword(1, "animation");
            animationName = Body.Clauses[2].ToString();
        }

        #region Private members

        // HandleInPlaceItem
        private void HandleInPlaceItem(Actor actor)
        {
            switch (definition.InPlaceEffectType)
            {
                case InPlaceEffectType.None:
                    break;

                // Lightning
                case InPlaceEffectType.Lightning:
                    var lightning = new LightningRite(target, item);
                    Session.Room?.Children.Add(lightning);
                    break;

                default:
                    break;
            }
        }

        // HandleProjectileItem
        private void HandleProjectileItem(Actor actor)
        {
            if (definition.Projectile != null)
                actor.LaunchProjectile(animationName, definition.Projectile);
        }

        #endregion

        // OnExecute
        protected override void OnExecute()
        {
            if ((Session as GameSession )?.Player is not Actor player || player.IsDead)
                return;

            if (definition.UsageScope == ItemUsageScope.Projectile)
            {
                HandleProjectileItem(player);
            }
            else if (definition.UsageScope == ItemUsageScope.InPlace)
            {
                HandleInPlaceItem(player);
            }
        }
    }
}