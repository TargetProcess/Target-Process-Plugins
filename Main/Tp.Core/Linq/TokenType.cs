namespace System.Linq.Dynamic
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable IdentifierTypo
    public enum TokenType
    {
        //Non terminal tokens:
        _NONE_ = 0,
        _UNDETERMINED_ = 1,

        //Non terminal tokens:
        Start = 2,
        Expression = 3,
        LogicalOr = 4,
        LogicalAnd = 5,
        Comparison = 6,
        ValueList = 7,
        Additive = 8,
        Multiplicative = 9,
        Unary = 10,
        Primary = 11,
        Value = 12,
        NewJson = 13,
        Field = 14,
        Statement = 15,
        Call = 16,

        //Terminal tokens:
        QUESTION = 17,
        COLON = 18,
        ANY = 19,
        OR = 20,
        AND = 21,
        COMPARISON = 22,
        ADDITIVE = 23,
        MULTIPLICATIVE = 24,
        UNARY = 25,
        DOT = 26,
        IDENTIFIER = 27,
        SEMICOLONNAME = 28,
        ASNAME = 29,
        STRING = 30,
        INTEGER = 31,
        REAL = 32,
        OPENPAREN = 33,
        CLOSEPAREN = 34,
        OPENCURLY = 35,
        CLOSECURLY = 36,
        IN = 37,
        COMMA = 38,
        OPENSQUARE = 39,
        CLOSESQUARE = 40,
        NEW = 41,
        EOF = 42,
        WHITESPACE = 43
    }
    // ReSharper restore InconsistentNaming
    // ReSharper restore IdentifierTypo
}