using System;

namespace Adberration
{
    /// <summary>
    /// SaveFileHeader
    /// </summary>
    public sealed class SaveFileHeader
    {
        // Constructor
        public SaveFileHeader(string fileName, string version, DateTime lastSaved, bool demo, int chapter, TimeSpan playTime, int difficulty, int progress)
        {
            this.FileName = fileName;
            this.LastSaved = lastSaved;
            this.Demo = demo;
            this.Chapter = chapter;
            this.PlayTime = playTime;
            this.Difficulty = difficulty;
            this.Progress = progress;
            this.Version = version;
        }

        // Chapter
        public int Chapter { get; }

        // Demo
        public bool Demo { get; }

        // Difficulty
        public int Difficulty { get; }

        // FileName
        public string FileName { get; }

        // LastSaved
        public DateTime LastSaved { get; }

        // PlayTime
        public TimeSpan PlayTime { get; }

        // Progress
        public int Progress { get; }

        // Version
        public string Version { get; }
    }
}
