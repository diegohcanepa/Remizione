namespace Adberration.Scripting
{
    // GenerateRandomNumberCommand
    // Arguments: {Name} {Range:Int32Range}
    internal sealed class GenerateRandomNumberCommand : NonAwaitableCommand
    {
        // Constructor
        internal GenerateRandomNumberCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2)
        {
            Parser.ParseName(this, 0);
            Parser.ParseInt32Range(this, 1);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var name = Parser.ParseName(this, 0);
            var range = Parser.ParseInt32Range(this, 1);
            Session.GenerateRandomNumber(name, range);
        }
    }
}
