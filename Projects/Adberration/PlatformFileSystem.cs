using Engendro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Adberration
{
    /// <summary>
    /// PlatformFileSystem
    /// </summary>
    public abstract class PlatformFileSystem
    {
        private readonly Dictionary<string, SaveFileHeader> cachedFiles = [];

        #region Private members

        // IsValidFileName
        private static bool IsValidFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            var path = Path.GetDirectoryName(fileName);
            if (path == null)
            {
                return false;
            }

            fileName = Path.GetFileName(fileName);

            return (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1) &&
                   (path.IndexOfAny(Path.GetInvalidPathChars()) == -1);
        }

        // AssertFileName
        private static void AssertFileName(string fileName)
        {
            if (!IsValidFileName(fileName))
            {
                throw new ArgumentException("Invalid file name.", nameof(fileName));
            }
        }

        #endregion

        #region Protected members

        // DeleteFileCore
        protected abstract void DeleteFileCore(string path);

        // FileExistsCore
        protected abstract bool FileExistsCore(string path);

        // ReadFileCore
        protected abstract Stream? ReadFileCore(string path);

        // WriteFileCore
        protected abstract bool WriteFileCore(string path, Stream input);

        #endregion

        // DeleteFile
        public bool DeleteFile(string fileName)
        {
            AssertFileName(fileName);
            fileName = GetPath(fileName);
            DeleteFileCore(fileName);
            cachedFiles.Remove(fileName);

            return !FileExists(fileName);
        }

        // EncryptSaveFiles (whether or not the game will encrypt save files when writing.)
        public bool EncryptSaveFiles { get; set; }

        // FileExists
        public bool FileExists(string fileName)
        {
            AssertFileName(fileName);

            fileName = GetPath(fileName);

            return FileExistsCore(fileName);
        }

        // GetPath
        public string GetPath(string fileName)
        {
            AssertFileName(fileName);

            if (!string.IsNullOrWhiteSpace(TargetDirectory))
                return Path.Combine(TargetDirectory, fileName);
            else
                return fileName;
        }

        // ReadFile
        public Stream? ReadFile(string fileName)
        {
            fileName = GetPath(fileName);

            if (!FileExists(fileName))
                return null;

            var result = ReadFileCore(fileName);
            if (result != null)
                result.Position = 0;

            return result;
        }

        // ReadSaveFileHeader
        public SaveFileHeader? ReadSaveFileHeader(string fileName)
        {
            return ReadSaveFileHeader(fileName, false);
        }

        // ReadSaveFileHeader
        public SaveFileHeader? ReadSaveFileHeader(string fileName, bool forceRead)
        {
            fileName = GetPath(fileName);

            // Try to retrieve cached instance
            if (!forceRead && cachedFiles.TryGetValue(fileName, out var cachedObj))
            {
                if (cachedObj is SaveFileHeader header)
                    return header;
            }

            if (ReadFile(fileName) is Stream fileInput)
            {
                var input = !XOREncryptor.IsEncryptedXml(fileInput) ? fileInput : XOREncryptor.AsStream(fileInput, XOREncryptor.EncryptionKey);

                using (fileInput)
                {
                    using (input)
                    {
                        using XmlReader reader = XmlReader.Create(input);

                        var chapter = 0;
                        var lastSaved = DateTime.MinValue;
                        var demo = false;
                        var playTime = TimeSpan.MinValue;
                        var difficulty = 0;
                        var progress = 0;
                        var version = string.Empty;

                        reader.MoveToContent();

                        // Chapter
                        if (reader.GetAttribute(GameSessionPersistenceAttributeName.Chapter.ToString()) is string chapterValue)
                            chapter = XmlConvert.ToInt32(chapterValue);

                        // Last Saved
                        if (reader.GetAttribute(GameSessionPersistenceAttributeName.LastSaved.ToString()) is string lastSavedValue)
                            lastSaved = XmlConvert.ToDateTime(lastSavedValue, XmlDateTimeSerializationMode.Local);

                        // Demo
                        if (reader.GetAttribute(GameSessionPersistenceAttributeName.Demo.ToString()) is string demoValue)
                            demo = XmlConvert.ToBoolean(demoValue);

                        // PlayTime
                        if (reader.GetAttribute(GameSessionPersistenceAttributeName.PlayTime.ToString()) is string totalPlayTimeValue)
                            playTime = XmlConvert.ToTimeSpan(totalPlayTimeValue);

                        // Difficulty
                        if (reader.GetAttribute(GameSessionPersistenceAttributeName.Difficulty.ToString()) is string difficultyValue)
                        {
                            if (int.TryParse(difficultyValue, out var difficultyNumber))
                                difficulty = difficultyNumber;
                        }

                        // Progress
                        if (reader.GetAttribute(GameSessionPersistenceAttributeName.Progress.ToString()) is string progressValue)
                            progress = XmlConvert.ToInt32(progressValue);

                        // Version
                        if (reader.GetAttribute(GameSessionPersistenceAttributeName.Version.ToString()) is string versionValue)
                            version = versionValue;

                        SaveFileHeader result = new(fileName, version, lastSaved, demo, chapter, playTime, difficulty, progress);

                        cachedFiles[fileName] = result;

                        return result;
                    }
                }
            }

            return null;
        }

        // TargetDirectory
        public abstract string TargetDirectory { get; }

        // WriteFile
        public bool WriteFile(string fileName, Stream input)
        {
            fileName = GetPath(fileName);
            cachedFiles.Remove(fileName);
            return WriteFileCore(fileName, input);
        }

        // WriteSaveFile
        public bool WriteSaveFile(SaveFileHeader header, Stream input)
        {
            var result = WriteFile(header.FileName, input);
            if (result)
                cachedFiles[header.FileName] = header;

            return result;
        }
    }
}