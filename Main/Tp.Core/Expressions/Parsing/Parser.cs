using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Sharpen;
using Tp.Core.Annotations;
using Tp.Core.Linq;

namespace Tp.Core.Expressions.Parsing
{
    public class ParserOptions
    {
        public ParserOptions(
            [NotNull] IReadOnlyDictionary<string, Type> knownTypes,
            [NotNull] [ItemNotNull] IReadOnlyCollection<MethodInfo> allowedExtensionMethods,
            [NotNull] ISurrogateGenerator surrogateGenerator,
            [NotNull] IEnumerableMethodStrategy enumerableMethodStrategy)
        {
            KnownTypes = Argument.NotNull(nameof(knownTypes), knownTypes);
            AllowedExtensionMethods = Argument.NotNull(nameof(allowedExtensionMethods), allowedExtensionMethods);
            SurrogateGenerator = Argument.NotNull(nameof(surrogateGenerator), surrogateGenerator);
            EnumerableMethodStrategy = Argument.NotNull(nameof(enumerableMethodStrategy), enumerableMethodStrategy);
        }

        [NotNull]
        public IEnumerableMethodStrategy EnumerableMethodStrategy { get; }

        [NotNull]
        public IReadOnlyDictionary<string, Type> KnownTypes { get; }

        [NotNull]
        [ItemNotNull]
        public IReadOnlyCollection<MethodInfo> AllowedExtensionMethods { get; }

        [NotNull]
        public ISurrogateGenerator SurrogateGenerator { get; }

        /// <summary>
        /// Should be as short as possible, see <see cref="ExpressionParserCache.BuildCacheKey"/> for details.
        /// </summary>
        public string GetCacheKey()
        {
            // It's implied that other properties don't actually differ in production,
            // so they are not included in a key
            return $"{EnumerableMethodStrategy.CacheKey};{SurrogateGenerator.GetCacheKey()}";
        }
    }

    public class Parser : IExpressionParser
    {
        private readonly ParserOptions _options;

        private static readonly Lazy<Parser> _defaultInstance = new Lazy<Parser>(() => new Parser(
            new ParserOptions(
                SharedParserUtils.DefaultKnownTypes,
                SharedParserUtils.DefaultExtensionMethods,
                SharedParserUtils.DefaultSurrogateGenerator,
                DefaultEnumerableMethodStrategy.Instance)));

        public static Parser DefaultInstance => _defaultInstance.Value;

        public Parser([NotNull] ParserOptions options)
        {
            _options = Argument.NotNull(nameof(options), options);
        }

        public string GetOptionsCacheKey() => _options.GetCacheKey();

        public Expression Parse(
            Expression it,
            Type resultType,
            Maybe<Type> baseTypeForNewClass,
            string expression)
        {
            var (parser, lexerErrorListener, parserErrorListener) = CreateAntlrParser(expression);
            var visitor = CreateVisitor(it, baseTypeForNewClass, resultType);
            var parseTree = parser.program();
            ThrowIfHasParsingErrors(lexerErrorListener, parserErrorListener);
            return visitor.Visit(parseTree);
        }

        public IReadOnlyList<DynamicOrdering> ParseOrdering(string ordering, Expression it)
        {
            var (parser, lexerErrorListener, parserErrorListener) = CreateAntlrParser(ordering);
            var buildExpressionVisitor = CreateVisitor(it, Maybe<Type>.Nothing, null);
            var orderingVisitor = new BuildOrderingVisitor(buildExpressionVisitor);
            var parseTree = parser.orderingProgram();
            ThrowIfHasParsingErrors(lexerErrorListener, parserErrorListener);
            return orderingVisitor.VisitOrderingProgram(parseTree);
        }

        [NotNull]
        private BuildExpressionVisitor CreateVisitor(
            Expression it,
            Maybe<Type> baseTypeForNewClass,
            Type resultType)
        {
            BuildExpressionVisitor visitor = null;
            var typeSystem = new TypeSystem(
                () => visitor.Literals,
                _options.KnownTypes, _options.AllowedExtensionMethods);
            visitor = new BuildExpressionVisitor(
                typeSystem,
                it,
                _options.EnumerableMethodStrategy,
                resultType,
                props => SharedParserUtils.CreateClass(props, baseTypeForNewClass.GetOrDefault()),
                _options.SurrogateGenerator);
            return visitor;
        }

        private static (Antlr.ExpressionParser, LexerErrorListener, ParserErrorListener) CreateAntlrParser(string expression)
        {
            var inputStream = new AntlrInputStream(expression);
            var lexer = new Antlr.ExpressionLexer(inputStream);
            var lexerErrorListener = new LexerErrorListener();
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(lexerErrorListener);

            var tokenStream = new CommonTokenStream(lexer);

            var parser = new Antlr.ExpressionParser(tokenStream);
            parser.RemoveErrorListeners();
            parser.RemoveParseListeners();
            var parserErrorListener = new ParserErrorListener();
            parser.AddErrorListener(parserErrorListener);

            return (parser, lexerErrorListener, parserErrorListener);
        }

        private class LexerErrorListener : IAntlrErrorListener<int>
        {
            private readonly List<(int ErrorPosition, string Message)> _errors = new List<(int ErrorPosition, string Message)>();

            public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine,
                string msg,
                RecognitionException e)
            {
                // TODO: provide better user-friendly messages
                _errors.Add((offendingSymbol, msg));
            }

            public void ThrowIfHasErrors()
            {
                if (_errors.Count > 0)
                {
                    var (errorPos, msg) = _errors[0];
                    throw new ParseException(msg.AsLocalized(), errorPos);
                }
            }
        }

        private class ParserErrorListener : IParserErrorListener
        {
            private readonly List<(int ErrorPosition, string Message)> _errors = new List<(int ErrorPosition, string Message)>();

            public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine,
                string msg,
                RecognitionException e)
            {
                // TODO: provide better user-friendly messages
                _errors.Add((BuildExpressionVisitor.GetErrorPos(offendingSymbol), msg));
            }

            public void ReportAmbiguity(Antlr4.Runtime.Parser recognizer, DFA dfa, int startIndex, int stopIndex, bool exact,
                BitSet ambigAlts, ATNConfigSet configs)
            {
                // TODO: provide better user-friendly messages
                _errors.Add((startIndex, "Parser ambiguity"));
            }

            public void ReportAttemptingFullContext(Antlr4.Runtime.Parser recognizer, DFA dfa, int startIndex, int stopIndex,
                BitSet conflictingAlts,
                SimulatorState conflictState)
            {
                // TODO: provide better user-friendly messages
                _errors.Add((startIndex, "Parser attempting full context"));
            }

            public void ReportContextSensitivity(Antlr4.Runtime.Parser recognizer, DFA dfa, int startIndex, int stopIndex, int prediction,
                SimulatorState acceptState)
            {
                // TODO: provide better user-friendly messages
                _errors.Add((startIndex, "Parser context sensitivity"));
            }

            public void ThrowIfHasErrors()
            {
                if (_errors.Count > 0)
                {
                    var (errorPos, msg) = _errors[0];
                    throw new ParseException(msg.AsLocalized(), errorPos);
                }
            }
        }

        private void ThrowIfHasParsingErrors(LexerErrorListener lexerErrorListener, ParserErrorListener parserErrorListener)
        {
            lexerErrorListener.ThrowIfHasErrors();
            parserErrorListener.ThrowIfHasErrors();
        }
    }
}
