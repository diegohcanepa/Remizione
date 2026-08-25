using Adberration.Scripting;

namespace ScaryCastle
{
    /// <summary>
    /// ScriptExtensions
    /// </summary>
    public static class ScriptExtensions
    {
        extension(Script script)
        {
            // AssertItemDefinition
            public ItemDefinition AssertItemDefinition(string name)
            {
                var def = GameData.Items.Find(name);
                return def ?? throw new ScriptException(script, $"Item definition [{name}] does not exist.");
            }
        }
    }
}
