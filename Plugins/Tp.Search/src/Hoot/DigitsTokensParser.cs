using System;
using System.Collections.Generic;
using System.Text;

namespace hOOt
{
    public class DigitsTokensParser : ITokensParser
    {
        public Dictionary<string, int> Parse(string text)
        {
            var d = new Dictionary<string, int>();
            var runningDigits = new StringBuilder();
            for (int i = 0; i < text.Length; ++i)
            {
                char symbol = text[i];
                if (Char.IsDigit(symbol))
                {
                    runningDigits.Append(symbol);
                }
                else if (Char.IsLetter(symbol) || symbol == ' ')
                {
                    AddOrUpdate(d, runningDigits.ToString());
                    runningDigits.Clear();
                }
            }
            if (runningDigits.Length != 0)
            {
                AddOrUpdate(d, runningDigits.ToString());
                runningDigits.Clear();
            }
            return d;
        }

        private void AddOrUpdate(IDictionary<string, int> d, string digits)
        {
            if (digits.Length == 0)
            {
                return;
            }
            if (d.ContainsKey(digits))
            {
                d[digits] = d[digits] + 1;
            }
            else
            {
                d.Add(digits, 1);
            }
        }
    }
}
