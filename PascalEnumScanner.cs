using System;
using System.Collections.Generic;

namespace KompilatoriLab1
{
    public class PascalEnumScanner
    {
        public enum TokenType
        {
            KEYWORD = 1,
            IDENTIFIER = 2,
            OPERATOR = 3,
            SEPARATOR = 4,
            PAREN = 5,
            ERROR = 99
        }

        public class Token
        {
            public TokenType Type { get; set; }
            public int TypeCode => (int)Type;
            public string Value { get; set; }
            public int Line { get; set; }
            public int StartColumn { get; set; }
            public int EndColumn { get; set; }
            public int AbsolutePosition { get; set; }
        }

        public class Error
        {
            public int Line { get; set; }
            public int Column { get; set; }
            public string Message { get; set; }
            public char Symbol { get; set; }
            public int AbsolutePosition { get; set; }
        }

        public class ScanResult
        {
            public List<Token> Tokens { get; } = new List<Token>();
            public List<Error> Errors { get; } = new List<Error>();
        }

        public ScanResult Scan(string text)
        {
            var result = new ScanResult();

            if (string.IsNullOrEmpty(text))
                return result;

            int pos = 0;
            int line = 1;
            int col = 1;

            while (pos < text.Length)
            {
                char c = text[pos];

                if (c == '\r')
                {
                    if (pos + 1 < text.Length && text[pos + 1] == '\n')
                        pos++;

                    pos++;
                    line++;
                    col = 1;
                    continue;
                }

                if (c == '\n')
                {
                    pos++;
                    line++;
                    col = 1;
                    continue;
                }

                if (char.IsWhiteSpace(c))
                {
                    pos++;
                    col++;
                    continue;
                }

                int startPos = pos;
                int startLine = line;
                int startCol = col;

                if (char.IsLetter(c))
                {
                    while (pos < text.Length &&
                           (char.IsLetterOrDigit(text[pos]) || text[pos] == '_'))
                    {
                        pos++;
                        col++;
                    }

                    string lexeme = text.Substring(startPos, pos - startPos);

                    result.Tokens.Add(new Token
                    {
                        Type = string.Equals(lexeme, "type", StringComparison.OrdinalIgnoreCase)
                            ? TokenType.KEYWORD
                            : TokenType.IDENTIFIER,
                        Value = lexeme,
                        Line = startLine,
                        StartColumn = startCol,
                        EndColumn = col - 1,
                        AbsolutePosition = startPos
                    });

                    continue;
                }

                if (c == '=')
                {
                    result.Tokens.Add(new Token
                    {
                        Type = TokenType.OPERATOR,
                        Value = "=",
                        Line = line,
                        StartColumn = col,
                        EndColumn = col,
                        AbsolutePosition = pos
                    });

                    pos++;
                    col++;
                    continue;
                }

                if (c == ',' || c == ';')
                {
                    result.Tokens.Add(new Token
                    {
                        Type = TokenType.SEPARATOR,
                        Value = c.ToString(),
                        Line = line,
                        StartColumn = col,
                        EndColumn = col,
                        AbsolutePosition = pos
                    });

                    pos++;
                    col++;
                    continue;
                }

                if (c == '(' || c == ')')
                {
                    result.Tokens.Add(new Token
                    {
                        Type = TokenType.PAREN,
                        Value = c.ToString(),
                        Line = line,
                        StartColumn = col,
                        EndColumn = col,
                        AbsolutePosition = pos
                    });

                    pos++;
                    col++;
                    continue;
                }

                result.Errors.Add(new Error
                {
                    Line = line,
                    Column = col,
                    Message = $"Недопустимый символ '{c}'",
                    Symbol = c,
                    AbsolutePosition = pos
                });

                result.Tokens.Add(new Token
                {
                    Type = TokenType.ERROR,
                    Value = c.ToString(),
                    Line = line,
                    StartColumn = col,
                    EndColumn = col,
                    AbsolutePosition = pos
                });

                pos++;
                col++;
            }

            return result;
        }
    }
}