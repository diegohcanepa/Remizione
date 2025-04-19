using System.Text;

namespace TextRepositoryEditor
{
    /// <summary>
    /// XOREncryptor
    /// </summary>
    public static class XOREncryptor
    {
        #region Private members

        // TextToStream
        private static Stream TextToStream(string text)
        {
            var result = new MemoryStream();

            using (var sw = new StreamWriter(result, Encoding.UTF8, 4096, true))
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
                return new MemoryStream();

            var text = AsString(input, key);

            return TextToStream(text);
        }

        // AsString
        public static string AsString(Stream input, string key)
        {
            if (input.Length == 0)
                return string.Empty;

            using var r = new StreamReader(input);
            var text = r.ReadToEnd();
            text = AsString(text, key);
            return text;
        }

        // AsString
        public static string AsString(string input, string key)
        {
            CodeContract.NotEmpty(input, nameof(input));
            CodeContract.NotEmpty(key, nameof(key));

            char[] keyChars = key.ToCharArray();
            char[] output = new char[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (char)(input[i] ^ keyChars[i % keyChars.Length]);
            }

            return new string(output);
        }

        // IsXmlContent
        public static bool IsXmlContent(Stream input)
        {
            const string xmlHeader = "<?xml";

            var position = input.Position;
            try
            {
                using var output = new StreamReader(input, Encoding.UTF8, false, xmlHeader.Length, true);

                var buffer = new char[xmlHeader.Length];

                if (output.ReadBlock(buffer, 0, xmlHeader.Length) < xmlHeader.Length)
                    return false;

                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] != xmlHeader[i])
                        return false;
                }

                return true;
            }
            finally
            {
                input.Position = position;
            }
        }
    }
}
