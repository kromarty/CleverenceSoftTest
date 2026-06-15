using System.Text;

namespace CleverenceSoftTasks
{
    public class StringCompressor
    {
        public static string Compress(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = new StringBuilder();
            int i = 0;
            while (i < input.Length)
            {
                char current = input[i];
                int count = 1;
                while (i + count < input.Length && input[i + count] == current)
                    count++;

                result.Append(current);
                if (count > 1)
                    result.Append(count);

                i += count;
            }
            return result.ToString();
        }

        public static string Decompress(string compressed)
        {
            if (string.IsNullOrEmpty(compressed))
                return compressed;

            var result = new StringBuilder();
            int i = 0;
            while (i < compressed.Length)
            {
                char ch = compressed[i];
                i++;
                if (i < compressed.Length && char.IsDigit(compressed[i]))
                {
                    int start = i;
                    while (i < compressed.Length && char.IsDigit(compressed[i]))
                        i++;
                    int count = int.Parse(compressed.Substring(start, i - start));
                    result.Append(ch, count);
                }
                else
                {
                    result.Append(ch);
                }
            }
            return result.ToString();
        }
    }
}
