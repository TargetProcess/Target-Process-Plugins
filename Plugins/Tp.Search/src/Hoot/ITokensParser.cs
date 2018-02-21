using System.Collections.Generic;

namespace hOOt
{
    public interface ITokensParser
    {
        Dictionary<string, int> Parse(string text);
    }
}
