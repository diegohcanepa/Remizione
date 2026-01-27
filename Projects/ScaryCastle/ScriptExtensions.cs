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
            public void AssertItemDefinition(string name)
            {
                if (ItemDefinition.Find(name) == null)
                    throw new ScriptException(script, $"Item definition [{name}] does not exist.");
            }
        }
    }
}
