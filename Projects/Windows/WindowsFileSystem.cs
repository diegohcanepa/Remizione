using Adberration;
using Syroot.Windows.IO;
using System.IO;

namespace Remizione
{
    /// <summary>
    /// WindowsFileSystem
    /// </summary>
    public sealed partial class WindowsFileSystem : PlatformFileSystem
    {
        #region Constructor

        // Constructor
        public WindowsFileSystem()
        {
            // TODO: Set to true
            EncryptSaveFiles = false;
        }

        #endregion

        #region Protected members

        // DeleteFileCore
        protected override void DeleteFileCore(string path)
        {
            File.Delete(path);
        }

        // FileExistsCore
        protected override bool FileExistsCore(string path)
        {
            return File.Exists(path);
        }

        // ReadFileCore
        protected override Stream? ReadFileCore(string path)
        {
            if (FileExists(path))
            {
                using FileStream input = new(path, FileMode.Open, FileAccess.Read);
                MemoryStream output = new();
                input.CopyTo(output);
                output.Flush();
                output.Position = 0;
                return output;
            }

            return null;
        }

        // WriteFileCore
        protected override bool WriteFileCore(string path, Stream input)
        {
            // Does target exists?
            var targetDir = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(targetDir))
            {
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }
            }


            using (FileStream output = new(path, FileMode.Create, FileAccess.Write))
            {
                input.CopyTo(output);
                output.Flush();
            }

            return true;
        }

        #endregion

        // TargetDirectory
        public override string TargetDirectory
        {
            get
            {
                KnownFolder savedGamesFolder = new(KnownFolderType.SavedGames);
                return Path.Combine(savedGamesFolder.Path, GameSettings.GameFolder);
            }
        }
    }
}
