using System;
using System.Collections.Generic;
using System.Linq;

namespace KompilatoriLab1
{
    public class PascalEnumParser
    {
        public class SyntaxError
        {
            public string Fragment { get; set; }
            public int Line { get; set; }
            public int Column { get; set; }
            public string Description { get; set; }
            public int AbsolutePosition { get; set; }
        }

        public class ParseResult
        {
            public bool Success => Errors.Count == 0;
            public List<SyntaxError> Errors { get; } = new List<SyntaxError>();
        }

        private readonly List<PascalEnumScanner.Token> _tokens;
        private int _position;

        public PascalEnumParser(IEnumerable<PascalEnumScanner.Token> tokens)
        {
            _tokens = tokens
                .Where(t => t.Type != PascalEnumScanner.TokenType.ERROR)
                .ToList();
            _position = 0;
        }

        private PascalEnumScanner.Token Current =>
            _position < _tokens.Count ? _tokens[_position] : null;

        private bool IsAtEnd => _position >= _tokens.Count;

        private void Advance()
        {
            if (!IsAtEnd)
                _position++;
        }

        public ParseResult Parse()
        {
            var result = new ParseResult();

            if (_tokens.Count == 0)
            {
                result.Errors.Add(new SyntaxError
                {
                    Fragment = "<пустой ввод>",
                    Line = 1,
                    Column = 1,
                    Description = "Входная строка пуста.",
                    AbsolutePosition = 0
                });
                return result;
            }

            ParseEnumDecl(result);

            while (!IsAtEnd)
            {
                AddError(result, Current, $"Лишний фрагмент после завершения объявления: '{Current.Value}'.");
                Advance();
            }

            return result;
        }

        // <EnumDecl>   ::= type <Identifier> = ( <IdList> ) ;
        // <IdList>     ::= <Identifier> <IdListTail>
        // <IdListTail> ::= , <Identifier> <IdListTail> | ε

        private void ParseEnumDecl(ParseResult result)
        {
            ExpectKeyword(
                result,
                "type",
                "Ожидалось ключевое слово 'type'.",
                new[] { "type", "=", "(", ")", ";", "," });

            ExpectIdentifier(
                result,
                "Ожидался идентификатор имени перечисления.",
                new[] { "=", "(", ")", ";", "," });

            ExpectSymbol(
                result,
                "=",
                "Ожидался символ '=' после имени перечисления.",
                new[] { "(", ")", ";", "," });

            ExpectSymbol(
                result,
                "(",
                "Ожидался символ '(' после '='.",
                new[] { ")", ";", "," });

            ParseIdList(result);

            ExpectSymbol(
                result,
                ")",
                "Ожидался символ ')' после списка элементов перечисления.",
                new[] { ";" });

            ExpectSymbol(
                result,
                ";",
                "Ожидался символ ';' в конце объявления.",
                Array.Empty<string>());
        }

        private void ParseIdList(ParseResult result)
        {
            if (IsAtEnd)
            {
                AddEndOfInputError(result, "Ожидался идентификатор элемента перечисления.");
                return;
            }

            if (!IsIdentifier(Current))
            {
                AddError(result, Current, "Ожидался идентификатор элемента перечисления.");

                if (Current != null && Current.Value == ")")
                    return;

                Synchronize(new[] { ",", ")" });

                if (IsAtEnd || Current == null || Current.Value == ")")
                    return;
            }
            else
            {
                Advance();
            }

            while (!IsAtEnd && Current != null)
            {
                if (Current.Value == ",")
                {
                    Advance();

                    if (IsAtEnd)
                    {
                        AddEndOfInputError(result, "Ожидался идентификатор после запятой.");
                        return;
                    }

                    if (IsIdentifier(Current))
                    {
                        Advance();
                        continue;
                    }

                    AddError(result, Current, "Ожидался идентификатор после запятой.");

                    if (Current.Value == ")")
                        return;

                    Synchronize(new[] { ",", ")" });
                    continue;
                }

                if (Current.Value == ")")
                    return;

                if (IsIdentifier(Current))
                {
                    AddError(result, Current, "Ожидалась запятая между элементами перечисления.");
                    Advance();
                    continue;
                }

                AddError(result, Current, "Недопустимый элемент в списке перечисления.");

                if (Current.Value == ";")
                    return;

                Synchronize(new[] { ",", ")", ";" });
            }
        }

        private void ExpectKeyword(ParseResult result, string keyword, string errorText, string[] syncTokens)
        {
            if (!IsAtEnd &&
                Current.Type == PascalEnumScanner.TokenType.KEYWORD &&
                string.Equals(Current.Value, keyword, StringComparison.OrdinalIgnoreCase))
            {
                Advance();
                return;
            }

            Recover(result, errorText, syncTokens);
        }

        private void ExpectIdentifier(ParseResult result, string errorText, string[] syncTokens)
        {
            if (!IsAtEnd && IsIdentifier(Current))
            {
                Advance();
                return;
            }

            Recover(result, errorText, syncTokens);
        }

        private void ExpectSymbol(ParseResult result, string symbol, string errorText, string[] syncTokens)
        {
            if (!IsAtEnd && Current != null && Current.Value == symbol)
            {
                Advance();
                return;
            }

            Recover(result, errorText, syncTokens);
        }

        private bool IsIdentifier(PascalEnumScanner.Token token)
        {
            return token != null && token.Type == PascalEnumScanner.TokenType.IDENTIFIER;
        }

        private void Recover(ParseResult result, string errorText, string[] syncTokens)
        {
            if (IsAtEnd)
            {
                AddEndOfInputError(result, errorText);
                return;
            }

            AddError(result, Current, errorText);

            if (syncTokens.Any(t => string.Equals(t, Current.Value, StringComparison.OrdinalIgnoreCase)))
                return;

            Synchronize(syncTokens);
        }

        private void Synchronize(IEnumerable<string> syncTokens)
        {
            while (!IsAtEnd &&
                   Current != null &&
                   !syncTokens.Any(t => string.Equals(t, Current.Value, StringComparison.OrdinalIgnoreCase)))
            {
                Advance();
            }
        }

        private void AddEndOfInputError(ParseResult result, string description)
        {
            if (_tokens.Count == 0)
            {
                result.Errors.Add(new SyntaxError
                {
                    Fragment = "<конец ввода>",
                    Line = 1,
                    Column = 1,
                    Description = description,
                    AbsolutePosition = 0
                });
                return;
            }

            var last = _tokens[_tokens.Count - 1];

            result.Errors.Add(new SyntaxError
            {
                Fragment = "<конец ввода>",
                Line = last.Line,
                Column = last.EndColumn + 1,
                Description = description,
                AbsolutePosition = last.AbsolutePosition + last.Value.Length
            });
        }

        private void AddError(ParseResult result, PascalEnumScanner.Token token, string description)
        {
            result.Errors.Add(new SyntaxError
            {
                Fragment = token?.Value ?? "<null>",
                Line = token?.Line ?? 1,
                Column = token?.StartColumn ?? 1,
                Description = description,
                AbsolutePosition = token?.AbsolutePosition ?? 0
            });
        }
    }
}