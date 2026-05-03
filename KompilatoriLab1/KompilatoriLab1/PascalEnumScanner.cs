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
            NUMBER = 3,
            BOOLEAN = 4,
            OPERATOR = 5,
            SEPARATOR = 6,
            PAREN = 7,
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

                if (char.IsDigit(c))
                {
                    while (pos < text.Length && char.IsDigit(text[pos]))
                    {
                        pos++;
                        col++;
                    }

                    string number = text.Substring(startPos, pos - startPos);

                    result.Tokens.Add(new Token
                    {
                        Type = TokenType.NUMBER,
                        Value = number,
                        Line = startLine,
                        StartColumn = startCol,
                        EndColumn = col - 1,
                        AbsolutePosition = startPos
                    });

                    continue;
                }

                if (IsIdentifierLikeStart(c))
                {
                    while (pos < text.Length && IsIdentifierLikePart(text[pos]))
                    {
                        pos++;
                        col++;
                    }

                    string lexeme = text.Substring(startPos, pos - startPos);

                    if (IsValidAsciiIdentifier(lexeme))
                    {
                        TokenType tokenType;

                        if (string.Equals(lexeme, "type", StringComparison.OrdinalIgnoreCase))
                        {
                            tokenType = TokenType.KEYWORD;
                        }
                        else if (string.Equals(lexeme, "true", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(lexeme, "false", StringComparison.OrdinalIgnoreCase))
                        {
                            tokenType = TokenType.BOOLEAN;
                        }
                        else
                        {
                            tokenType = TokenType.IDENTIFIER;
                        }

                        result.Tokens.Add(new Token
                        {
                            Type = tokenType,
                            Value = lexeme,
                            Line = startLine,
                            StartColumn = startCol,
                            EndColumn = col - 1,
                            AbsolutePosition = startPos
                        });
                    }
                    else
                    {
                        string message;

                        if (lexeme.Length > 0 && char.IsDigit(lexeme[0]))
                            message = $"Недопустимый идентификатор '{lexeme}': идентификатор не может начинаться с цифры.";
                        else if (lexeme.Length > 0 && lexeme[0] == '_')
                            message = $"Недопустимый идентификатор '{lexeme}': идентификатор должен начинаться с латинской буквы.";
                        else
                            message = $"Недопустимый идентификатор '{lexeme}': используйте только латинские буквы, цифры и '_'.";

                        int firstInvalidIndex = -1;
                        var invalidCharIndexes = new List<int>();

                        for (int i = 0; i < lexeme.Length; i++)
                        {
                            char ch = lexeme[i];

                            bool valid = i == 0
                                ? IsAsciiLetter(ch)
                                : IsAsciiIdentifierPart(ch);

                            if (!valid)
                            {
                                if (firstInvalidIndex == -1)
                                    firstInvalidIndex = i;

                                invalidCharIndexes.Add(i);
                            }
                        }

                        bool emittedNonDigitSymbolErrors = false;

                        foreach (int idx in invalidCharIndexes)
                        {
                            char bad = lexeme[idx];

                            if (char.IsDigit(bad))
                                continue;

                            emittedNonDigitSymbolErrors = true;

                            result.Errors.Add(new Error
                            {
                                Line = startLine,
                                Column = startCol + idx,
                                Message = $"Недопустимый символ '{bad}'",
                                Symbol = bad,
                                AbsolutePosition = startPos + idx
                            });
                        }

                        if (!emittedNonDigitSymbolErrors)
                        {
                            if (firstInvalidIndex >= 0)
                            {
                                char firstInvalidChar = lexeme[firstInvalidIndex];

                                result.Errors.Add(new Error
                                {
                                    Line = startLine,
                                    Column = startCol + firstInvalidIndex,
                                    Message = message,
                                    Symbol = firstInvalidChar,
                                    AbsolutePosition = startPos + firstInvalidIndex
                                });
                            }
                            else
                            {
                                result.Errors.Add(new Error
                                {
                                    Line = startLine,
                                    Column = startCol,
                                    Message = message,
                                    Symbol = lexeme[0],
                                    AbsolutePosition = startPos
                                });
                            }
                        }

                        result.Tokens.Add(new Token
                        {
                            Type = TokenType.IDENTIFIER,
                            Value = lexeme,
                            Line = startLine,
                            StartColumn = startCol,
                            EndColumn = col - 1,
                            AbsolutePosition = startPos
                        });
                    }

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

        private static bool IsAsciiLetter(char c)
        {
            return c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z';
        }

        private static bool IsAsciiIdentifierPart(char c)
        {
            return IsAsciiLetter(c) || char.IsDigit(c) || c == '_';
        }

        private static bool IsValidAsciiIdentifier(string lexeme)
        {
            if (string.IsNullOrEmpty(lexeme))
                return false;

            if (!IsAsciiLetter(lexeme[0]))
                return false;

            for (int i = 1; i < lexeme.Length; i++)
            {
                if (!IsAsciiIdentifierPart(lexeme[i]))
                    return false;
            }

            return true;
        }

        private static bool IsIdentifierLikeStart(char c)
        {
            return !char.IsWhiteSpace(c) &&
                   c != '=' &&
                   c != ',' &&
                   c != ';' &&
                   c != '(' &&
                   c != ')';
        }

        private static bool IsIdentifierLikePart(char c)
        {
            return IsIdentifierLikeStart(c);
        }
    }
}
