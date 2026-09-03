using Adberration.Scripting;

namespace Remizione.Scripting
{
    /// <summary>
    /// ScriptLibraryExtensions
    /// </summary>
    public static class ScriptLibraryExtensions
    {
        extension(ScriptLibrary scriptLibrary)
        {
            // FindItemRoutine
            public Script? FindItemRoutine(string itemName, Verb verb)
            {
                var routineName = $"Item-{itemName}-{verb}";
                return scriptLibrary.FindRoutine(routineName);
            }

            // FindRunRoutine
            public Script? FindRunRoutine(Run run, RunStage runStage)
            {
                var routineName = $"Run-{run.Definition.Name}-{run.FloorIndex}-{runStage}";
                return scriptLibrary.FindRoutine(routineName);
            }
        }
    }
}
