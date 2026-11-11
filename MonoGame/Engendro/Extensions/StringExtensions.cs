namespace Engendro
{
    /// <summary>
    /// StringExtensions
    /// </summary>
    public static class StringExtensions
    {
        extension(string text)
        {
            // CountWords
            public int CountWords()
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
}
