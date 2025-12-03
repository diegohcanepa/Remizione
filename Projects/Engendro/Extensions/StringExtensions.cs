namespace Engendro
{
    /// <summary>
    /// StringExtensions
    /// </summary>
    public static class StringExtensions
    {
        // CountWords
        public static int CountWords(this string text)
        {
            var index = 0;
            var result = 1;

            while (index <= text.Length - 1)
            {
                if (text[index] is ' ' or '\n' or '\t')
                    result++;

                index++;
            }

            return result;
        }
    }
}
