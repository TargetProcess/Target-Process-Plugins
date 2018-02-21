using System.Collections.Generic;

namespace hOOt
{
    public class CharacterTokensParser : ITokensParser
    {
        private readonly int _minStringLengthToSearch;
        private readonly int _maxStringLengthIgnore;

        public CharacterTokensParser(int minStringLengthToSearch = 2, int maxStringLengthIgnore = 60)
        {
            _minStringLengthToSearch = minStringLengthToSearch;
            _maxStringLengthIgnore = maxStringLengthIgnore;
        }

        public Dictionary<string, int> Parse(string text)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>(50000);

            char[] chars = text.ToCharArray();
            int index = 0;
            int run = -1;
            int count = chars.Length;
            while (index < count)
            {
                char c = chars[index++];
                if (!char.IsLetter(c))
                {
                    if (run != -1)
                    {
                        ParseString(dic, chars, index, run);
                        run = -1;
                    }
                }
                else if (run == -1)
                    run = index - 1;
            }

            if (run != -1)
            {
                ParseString(dic, chars, index, run);
                run = -1;
            }

            return dic;
        }

        private void ParseString(Dictionary<string, int> dic, char[] chars, int end, int start)
        {
            // check if upper lower case mix -> extract words
            int uppers = 0;
            bool found = false;
            for (int i = start; i < end; i++)
            {
                if (char.IsUpper(chars[i]))
                    uppers++;
            }
            // not all uppercase
            if (uppers != end - start - 1)
            {
                int lastUpper = start;

                string word = "";
                for (int i = start + 1; i < end; i++)
                {
                    char c = chars[i];
                    if (char.IsUpper(c))
                    {
                        found = true;
                        word = new string(chars, lastUpper, i - lastUpper).ToLowerInvariant().Trim();
                        AddDictionary(dic, word);
                        lastUpper = i;
                    }
                }
                if (lastUpper > start)
                {
                    string last = new string(chars, lastUpper, end - lastUpper - 1).ToLowerInvariant().Trim();
                    if (word != last)
                        AddDictionary(dic, last);
                }
            }
            if (found == false)
            {
                var length = end == chars.Length ? end - start : end - start - 1;
                string s = new string(chars, start, length).ToLowerInvariant().Trim();
                AddDictionary(dic, s);
            }
        }

        private void AddDictionary(Dictionary<string, int> dic, string word)
        {
            int l = word.Length;
            if (l > _maxStringLengthIgnore)
            {
                return;
            }
            if (l < _minStringLengthToSearch)
            {
                return;
            }
            //if (char.IsLetter(word[l - 1]) == false)
            //{
            //	word = new string(word.ToCharArray(), 0, l - 2);
            //}
            if (word.Length < 2)
            {
                return;
            }
            int cc;
            if (dic.TryGetValue(word, out cc))
            {
                dic[word] = ++cc;
            }
            else
            {
                dic.Add(word, 1);
            }
        }
    }
}
