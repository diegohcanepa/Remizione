using System.IO;
using System.Text;

namespace Engendro
{
    /// <summary>
    /// XOREncryptor
    /// </summary>
    public static class XOREncryptor
    {

        #region Private members

        // TextToStream
        private static MemoryStream TextToStream(string text)
        {
            MemoryStream result = new();

            using (StreamWriter sw = new(result, Encoding.UTF8, 4096, true))
            {
                sw.Write(text.ToCharArray());
                sw.Flush();
                result.Position = 0;
            }

            return result;
        }

        #endregion

        // AsStream
        public static Stream AsStream(string input, string key)
        {
            CodeContract.NotEmpty(input, nameof(input));
            CodeContract.NotEmpty(key, nameof(key));

            var text = AsString(input, key);

            return TextToStream(text);
        }

        // AsStream
        public static Stream AsStream(Stream input, string key)
        {
            if (input.Length == 0)
            {
                return new MemoryStream();
            }

            var text = AsString(input, key);

            return TextToStream(text);
        }

        // AsString
        public static string AsString(Stream input, string key)
        {
            if (input.Length == 0)
            {
                return string.Empty;
            }

            using StreamReader r = new(input);
            var text = r.ReadToEnd();
            text = AsString(text, key);
            return text;
        }

        // AsString
        public static string AsString(string input, string key)
        {
            CodeContract.NotEmpty(input, nameof(input));
            CodeContract.NotEmpty(key, nameof(key));

            var keyChars = key.ToCharArray();
            var output = new char[input.Length];

            for (var i = 0; i < input.Length; i++)
            {
                output[i] = (char)(input[i] ^ keyChars[i % keyChars.Length]);
            }

            return new string(output);
        }

        // EncryptionKey
        public static string EncryptionKey
        {
            get;
            set
            {
                if (value != field)
                {
                    CodeContract.NotEmpty(value, nameof(value));
                    field = value;
                }
            }
        } = "NIEPOX";

        // IsEncryptedXml
        public static bool IsEncryptedXml(Stream input)
        {
            const string xmlHeader = "<?xml";

            var position = input.Position;
            try
            {
                using StreamReader output = new(input, Encoding.UTF8, false, xmlHeader.Length, true);

                var buffer = new char[xmlHeader.Length];

                if (output.ReadBlock(buffer, 0, xmlHeader.Length) < xmlHeader.Length)
                    return true;

                for (var i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] != xmlHeader[i])
                        return true;
                }

                return false;
            }
            finally
            {
                input.Position = position;
            }
        }
    }
}
