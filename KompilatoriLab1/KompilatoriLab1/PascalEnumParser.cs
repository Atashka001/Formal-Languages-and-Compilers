using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KompilatoriLab1
{
    public class PascalEnumParser
    {
        public const int LexicalCascadeProximityRadius = 0;

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
            public bool Success
            {
                get { return Errors.Count == 0; }
            }

            public List<SyntaxError> Errors { get; } = new List<SyntaxError>();

            public List<AstNode> AstNodes { get; } = new List<AstNode>();

            public string AstText
            {
                get
                {
                    if (AstNodes.Count == 0)
                        return "AST не построено.";

                    var sb = new StringBuilder();

                    for (int i = 0; i < AstNodes.Count; i++)
                    {
                        if (i > 0)
                            sb.AppendLine();

                        AstNodes[i].Print(sb, "", true);
                    }

                    return sb.ToString();
                }
            }
        }

        public abstract class AstNode
        {
            public abstract void Print(StringBuilder sb, string indent, bool isLast);
        }

        public class EnumDeclNode : AstNode
        {
            public string Name { get; set; }
            public List<EnumValueNode> Values { get; } = new List<EnumValueNode>();

            public override void Print(StringBuilder sb, string indent, bool isLast)
            {
                sb.AppendLine("EnumDeclNode");
                sb.AppendLine("├── keyword: \"type\"");
                sb.AppendLine($"├── name: \"{Name}\"");
                sb.AppendLine("├── baseType: EnumTypeNode");
                sb.AppendLine($"│   └── name: \"{Name}\"");
                sb.AppendLine("└── values: EnumValueListNode");

                for (int i = 0; i < Values.Count; i++)
                {
                    bool last = i == Values.Count - 1;
                    Values[i].Print(sb, "    ", last);
                }
            }
        }

        public class EnumValueNode : AstNode
        {
            public string Name { get; set; }
            public int Ordinal { get; set; }

            public override void Print(StringBuilder sb, string indent, bool isLast)
            {
                string branch = isLast ? "└── " : "├── ";
                string childIndent = isLast ? "    " : "│   ";

                sb.AppendLine($"{indent}{branch}EnumValueNode");
                sb.AppendLine($"{indent}{childIndent}├── name: \"{Name}\"");
                sb.AppendLine($"{indent}{childIndent}└── ordinal: {Ordinal}");
            }
        }

        private class SymbolInfo
        {
            public string Name { get; set; }
            public string Kind { get; set; }
            public int Line { get; set; }
            public int Column { get; set; }
        }

        private class SymbolTable
        {
            private readonly Dictionary<string, SymbolInfo> _symbols =
                new Dictionary<string, SymbolInfo>(StringComparer.OrdinalIgnoreCase);

            public bool Declare(string name, string kind, int line, int column)
            {
                if (_symbols.ContainsKey(name))
                    return false;

                _symbols[name] = new SymbolInfo
                {
                    Name = name,
                    Kind = kind,
                    Line = line,
                    Column = column
                };

                return true;
            }
        }

        private readonly List<PascalEnumScanner.Token> _tokens;
        private readonly List<PascalEnumScanner.Error> _lexicalErrors;
        private readonly SymbolTable _symbolTable = new SymbolTable();

        private int _position;
        private bool _hasParsedAnyDeclaration;

        public PascalEnumParser(
            IEnumerable<PascalEnumScanner.Token> tokens,
            IEnumerable<PascalEnumScanner.Error> lexicalErrors = null)
        {
            _tokens = tokens
                .Where(t => t.Type != PascalEnumScanner.TokenType.ERROR)
                .ToList();

            _lexicalErrors = lexicalErrors != null
                ? lexicalErrors.ToList()
                : new List<PascalEnumScanner.Error>();

            _position = 0;
            _hasParsedAnyDeclaration = false;
        }

        private PascalEnumScanner.Token Current
        {
            get { return _position < _tokens.Count ? _tokens[_position] : null; }
        }

        private PascalEnumScanner.Token Peek(int offset = 1)
        {
            int index = _position + offset;
            return index < _tokens.Count ? _tokens[index] : null;
        }

        private bool IsAtEnd
        {
            get { return _position >= _tokens.Count; }
        }

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
                if (_lexicalErrors.Count > 0)
                    return result;

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

            while (!IsAtEnd)
            {
                if (IsDeclarationStart(Current) || LooksLikeEnumDeclarationStart(_position))
                {
                    ParseEnumDecl(result);
                    _hasParsedAnyDeclaration = true;
                    continue;
                }

                ConsumeGarbageBeforeDeclaration(result);
            }

            return result;
        }

        private void ParseEnumDecl(ParseResult result)
        {
            bool enumNameIsUnique = false;
            bool hasEnumName = false;
            bool hasOpeningParen = false;

            EnumDeclNode enumNode = null;

            if (IsTypeKeyword(Current))
            {
                Advance();
            }
            else if (IsTypeLikeTypo(Current))
            {
                AddError(result, Current, "Ожидалось ключевое слово 'type'. Возможно, допущена опечатка.");
                Advance();
            }
            else
            {
                bool typeMissing =
                    IsTokenIdentifier(Current) &&
                    Peek() != null &&
                    Peek().Value == "=";

                if (typeMissing)
                {
                    AddError(result, Current, "Ожидалось ключевое слово 'type'. Ключевое слово отсутствует.");
                }
                else
                {
                    string found = Current != null && Current.Value != null ? Current.Value : "<null>";
                    AddError(result, Current, $"Ожидалось ключевое слово 'type'. Найдено '{found}' вместо 'type'.");
                    Advance();
                }
            }

            while (!IsAtEnd &&
                   Current != null &&
                   Current.Value == "(" &&
                   Peek() != null &&
                   IsTokenIdentifier(Peek()))
            {
                AddError(result, Current, "Лишний символ '(' после ключевого слова 'type'.");
                Advance();
            }

            if (!IsAtEnd && IsTokenIdentifier(Current))
            {
                int chainStart = _position;
                int chainEnd = _position;
                int look = _position + 1;

                while (look < _tokens.Count && IsTokenIdentifier(_tokens[look]))
                {
                    chainEnd = look;
                    look++;
                }

                bool followedByEqOrParen =
                    look < _tokens.Count &&
                    _tokens[look] != null &&
                    (_tokens[look].Value == "=" || _tokens[look].Value == "(");

                if (chainEnd > chainStart && followedByEqOrParen)
                {
                    AddGroupedError(result, chainStart, chainEnd - 1, "Лишний фрагмент перед именем перечисления.");
                    _position = chainEnd;
                }
            }

            if (IsAtEnd)
            {
                AddEndOfInputError(result, "Ожидался идентификатор имени перечисления.");
                return;
            }

            if (IsTokenIdentifier(Current))
            {
                var enumNameToken = Current;
                string enumName = enumNameToken.Value;

                hasEnumName = true;

                enumNameIsUnique = _symbolTable.Declare(
                    enumName,
                    "enum",
                    enumNameToken.Line,
                    enumNameToken.StartColumn);

                if (!enumNameIsUnique)
                {
                    AddError(
                        result,
                        enumNameToken,
                        $"Ошибка: идентификатор \"{enumName}\" уже объявлен ранее.");
                }
                else
                {
                    enumNode = new EnumDeclNode
                    {
                        Name = enumName
                    };
                }

                Advance();

                while (!IsAtEnd &&
                       IsTokenIdentifier(Current) &&
                       Current.Value != "=" &&
                       !LooksLikeEnumItemStart(Current))
                {
                    AddError(result, Current, "Лишний фрагмент в имени перечисления.");
                    Advance();
                }
            }
            else
            {
                AddError(result, Current, "Ожидался идентификатор имени перечисления.");
                SyncTo("=", "(", ",", ")", ";");

                if (IsAtEnd)
                    return;
            }

            if (IsAtEnd)
            {
                AddEndOfInputError(result, "Ожидался символ '=' после имени перечисления.");
                return;
            }

            if (Current.Value == "=")
            {
                Advance();

                while (!IsAtEnd && Current.Value == "=")
                {
                    AddError(result, Current, "Лишний символ '=' после '='.");
                    Advance();
                }
            }
            else
            {
                AddError(result, Current, "Ожидался символ '=' после имени перечисления.");

                if (Current.Value == "(" || LooksLikeEnumItemStart(Current))
                {
                }
                else
                {
                    SyncTo("=", "(", ",", ")", ";");

                    if (!IsAtEnd && Current.Value == "=")
                    {
                        Advance();

                        while (!IsAtEnd && Current.Value == "=")
                        {
                            AddError(result, Current, "Лишний символ '=' после '='.");
                            Advance();
                        }
                    }
                }
            }

            if (!IsAtEnd && Current != null && IsTokenIdentifier(Current))
            {
                int gapStart = _position;
                int look = _position;

                while (look < _tokens.Count && IsTokenIdentifier(_tokens[look]))
                    look++;

                if (look < _tokens.Count && _tokens[look] != null && _tokens[look].Value == "(")
                {
                    if (look - 1 >= gapStart)
                        AddGroupedError(result, gapStart, look - 1, "Лишний фрагмент между '=' и '('.");

                    _position = look;
                }
            }

            if (IsAtEnd)
            {
                AddEndOfInputError(result, "Ожидался символ '(' после '='.");
                return;
            }

            if (Current.Value == "(")
            {
                hasOpeningParen = true;
                Advance();
            }
            else
            {
                if (LooksLikeEnumItemStart(Current) ||
                    Current.Type == PascalEnumScanner.TokenType.NUMBER ||
                    Current.Type == PascalEnumScanner.TokenType.BOOLEAN)
                {
                    AddError(result, Current, "Ожидался символ '(' после '='.");
                    hasOpeningParen = false;
                }
                else
                {
                    AddGroupedUnexpectedUntil(result, "Ожидался символ '(' после '='.", "(", ";", "type");

                    if (!IsAtEnd && Current.Value == "(")
                    {
                        hasOpeningParen = true;
                        Advance();
                    }
                }
            }

            if (IsAtEnd)
            {
                AddEndOfInputError(result, "Ожидался идентификатор элемента перечисления.");
                return;
            }

            ParseIdList(result, enumNode);

            if (hasOpeningParen)
            {
                if (IsAtEnd)
                {
                    AddEndOfInputError(result, "Ожидался символ ')' после списка элементов перечисления.");
                    AddAstIfPossible(result, hasEnumName, enumNameIsUnique, enumNode);
                    return;
                }

                if (Current.Value == ")")
                {
                    Advance();

                    while (!IsAtEnd && Current.Value == ")")
                    {
                        AddError(result, Current, "Лишний символ ')' после списка элементов перечисления.");
                        Advance();
                    }
                }
                else
                {
                    AddError(result, Current, "Ожидался символ ')' после списка элементов перечисления.");
                    SyncTo(")", ";", "type");

                    if (!IsAtEnd && Current.Value == ")")
                    {
                        Advance();

                        while (!IsAtEnd && Current.Value == ")")
                        {
                            AddError(result, Current, "Лишний символ ')' после списка элементов перечисления.");
                            Advance();
                        }
                    }
                }
            }
            else
            {
                if (!IsAtEnd && Current != null && Current.Value == ")")
                {
                    Advance();

                    while (!IsAtEnd && Current != null && Current.Value == ")")
                    {
                        AddError(result, Current, "Лишний символ ')' после списка элементов перечисления.");
                        Advance();
                    }
                }
            }

            if (IsAtEnd)
            {
                AddEndOfInputError(result, "Ожидался символ ';' в конце объявления.");
                AddAstIfPossible(result, hasEnumName, enumNameIsUnique, enumNode);
                return;
            }

            if (Current.Value == ";")
            {
                Advance();
            }
            else
            {
                HandleTailAfterList(result);
            }

            AddAstIfPossible(result, hasEnumName, enumNameIsUnique, enumNode);
        }

        private void ParseIdList(ParseResult result, EnumDeclNode enumNode)
        {
            bool expectIdentifier = true;
            bool hasAnyItem = false;

            while (!IsAtEnd && Current != null)
            {
                if (Current.Value == ")")
                {
                    if (expectIdentifier)
                    {
                        if (!hasAnyItem)
                            AddError(result, Current, "Список элементов перечисления не может быть пустым.");
                        else
                            AddError(result, Current, "Ожидался идентификатор элемента перечисления.");
                    }

                    return;
                }

                if (Current.Value == ";")
                {
                    if (expectIdentifier)
                    {
                        AddError(result, Current, "Ожидался идентификатор элемента перечисления.");
                        Advance();
                        continue;
                    }

                    AddError(result, Current, "Ожидалась запятая между элементами перечисления.");
                    Advance();

                    if (IsAtEnd || Current != null && Current.Value == ")")
                        return;

                    expectIdentifier = true;
                    continue;
                }

                if (Current.Value == ",")
                {
                    if (expectIdentifier)
                        AddError(result, Current, "Ожидался идентификатор элемента перечисления.");

                    Advance();
                    expectIdentifier = true;
                    continue;
                }

                if (Current.Value == "(")
                {
                    AddError(result, Current, "Лишний символ '(' в списке элементов перечисления.");
                    Advance();
                    continue;
                }

                if (expectIdentifier)
                {
                    if (Current.Type == PascalEnumScanner.TokenType.NUMBER)
                    {
                        AddError(
                            result,
                            Current,
                            $"Ошибка: числовое значение \"{Current.Value}\" недопустимо в перечислении. Ожидался идентификатор.");

                        Advance();
                        expectIdentifier = false;
                        hasAnyItem = true;
                        continue;
                    }

                    if (Current.Type == PascalEnumScanner.TokenType.BOOLEAN)
                    {
                        AddError(
                            result,
                            Current,
                            $"Ошибка: логическое значение \"{Current.Value}\" недопустимо в перечислении. Ожидался идентификатор.");

                        Advance();
                        expectIdentifier = false;
                        hasAnyItem = true;
                        continue;
                    }

                    if (IsTokenIdentifier(Current))
                    {
                        AddEnumValue(result, enumNode, Current);
                        Advance();

                        expectIdentifier = false;
                        hasAnyItem = true;
                        continue;
                    }

                    AddError(result, Current, "Ожидался идентификатор элемента перечисления.");
                    Advance();
                    continue;
                }

                if (Current.Type == PascalEnumScanner.TokenType.NUMBER)
                {
                    AddError(result, Current, "Ожидалась запятая между элементами перечисления.");
                    AddError(
                        result,
                        Current,
                        $"Ошибка: числовое значение \"{Current.Value}\" недопустимо в перечислении. Ожидался идентификатор.");

                    Advance();
                    expectIdentifier = false;
                    hasAnyItem = true;
                    continue;
                }

                if (Current.Type == PascalEnumScanner.TokenType.BOOLEAN)
                {
                    AddError(result, Current, "Ожидалась запятая между элементами перечисления.");
                    AddError(
                        result,
                        Current,
                        $"Ошибка: логическое значение \"{Current.Value}\" недопустимо в перечислении. Ожидался идентификатор.");

                    Advance();
                    expectIdentifier = false;
                    hasAnyItem = true;
                    continue;
                }

                if (IsTokenIdentifier(Current))
                {
                    AddError(result, Current, "Ожидалась запятая между элементами перечисления.");

                    AddEnumValue(result, enumNode, Current);
                    Advance();

                    expectIdentifier = false;
                    hasAnyItem = true;
                    continue;
                }

                AddError(result, Current, "Недопустимый элемент в списке перечисления.");
                Advance();
            }
        }

        private void AddEnumValue(ParseResult result, EnumDeclNode enumNode, PascalEnumScanner.Token token)
        {
            if (token == null)
                return;

            string itemName = token.Value;

            bool itemIsUnique = _symbolTable.Declare(
                itemName,
                "enum_value",
                token.Line,
                token.StartColumn);

            if (!itemIsUnique)
            {
                AddError(
                    result,
                    token,
                    $"Ошибка: идентификатор \"{itemName}\" уже объявлен ранее.");

                return;
            }

            if (enumNode != null)
            {
                enumNode.Values.Add(new EnumValueNode
                {
                    Name = itemName,
                    Ordinal = enumNode.Values.Count
                });
            }
        }

        private void AddAstIfPossible(
            ParseResult result,
            bool hasEnumName,
            bool enumNameIsUnique,
            EnumDeclNode enumNode)
        {
            if (hasEnumName &&
                enumNameIsUnique &&
                enumNode != null &&
                enumNode.Values.Count > 0 &&
                !result.AstNodes.Contains(enumNode))
            {
                result.AstNodes.Add(enumNode);
            }
        }

        private void HandleTailAfterList(ParseResult result)
        {
            if (IsAtEnd || Current == null)
                return;

            if (IsTokenIdentifier(Current) ||
                Current.Type == PascalEnumScanner.TokenType.NUMBER ||
                Current.Type == PascalEnumScanner.TokenType.BOOLEAN)
            {
                AddError(result, Current, "Лишний элемент после завершения списка перечисления.");
                Advance();

                while (!IsAtEnd && Current != null && Current.Value == ")")
                {
                    AddError(result, Current, "Лишний символ ')' после завершения объявления.");
                    Advance();
                }

                if (!IsAtEnd && Current != null && Current.Value == ";")
                {
                    Advance();
                    return;
                }

                if (!IsAtEnd && Current != null && !IsDeclarationStart(Current))
                {
                    int start = _position;

                    while (!IsAtEnd && Current != null && Current.Value != ";" && !IsDeclarationStart(Current))
                        Advance();

                    int end = _position - 1;

                    if (end >= start)
                        AddGroupedError(result, start, end, "Лишний фрагмент после завершения объявления.");

                    if (!IsAtEnd && Current != null && Current.Value == ";")
                        Advance();
                }

                return;
            }

            if (Current.Value == ")")
            {
                AddError(result, Current, "Лишний символ ')' после завершения объявления.");
                Advance();

                while (!IsAtEnd && Current != null && Current.Value == ")")
                {
                    AddError(result, Current, "Лишний символ ')' после завершения объявления.");
                    Advance();
                }

                if (!IsAtEnd && Current != null && Current.Value == ";")
                {
                    Advance();
                    return;
                }
            }

            int startIndex = _position;

            while (!IsAtEnd && Current != null && Current.Value != ";" && !IsDeclarationStart(Current))
                Advance();

            int endIndex = _position - 1;

            if (endIndex >= startIndex)
            {
                AddGroupedError(result, startIndex, endIndex, "Лишний фрагмент после завершения объявления.");
            }
            else
            {
                AddError(result, Current, "Ожидался символ ';' в конце объявления.");
            }

            if (!IsAtEnd && Current != null && Current.Value == ";")
                Advance();
        }

        private void ConsumeGarbageBeforeDeclaration(ParseResult result)
        {
            if (IsAtEnd || Current == null)
                return;

            int start = _position;

            while (!IsAtEnd && Current != null && !IsDeclarationStart(Current))
                Advance();

            if (start < _position)
            {
                if (_position - start == 1 && _tokens[start].Value == ";")
                {
                    AddError(
                        result,
                        _tokens[start],
                        _hasParsedAnyDeclaration
                            ? "Лишний символ ';' между объявлениями."
                            : "Лишний символ ';' перед объявлением перечисления.");
                }
                else
                {
                    AddGroupedError(result, start, _position - 1, "Лишний фрагмент перед объявлением перечисления.");
                }
            }
        }

        private void SyncTo(params string[] stopValues)
        {
            while (!IsAtEnd && Current != null)
            {
                if (MatchesAnyStop(Current, stopValues))
                    return;

                Advance();
            }
        }

        private void AddGroupedUnexpectedUntil(ParseResult result, string description, params string[] stopValues)
        {
            if (IsAtEnd || Current == null)
                return;

            int start = _position;

            while (!IsAtEnd && Current != null && !MatchesAnyStop(Current, stopValues))
                Advance();

            int end = _position - 1;

            if (end >= start)
                AddGroupedError(result, start, end, description);
            else
                AddError(result, Current, description);
        }

        private bool MatchesAnyStop(PascalEnumScanner.Token token, params string[] stopValues)
        {
            foreach (string stop in stopValues)
            {
                if (stop == "type")
                {
                    if (IsDeclarationStart(token))
                        return true;
                }
                else
                {
                    if (token.Value == stop)
                        return true;
                }
            }

            return false;
        }

        private void AddGroupedError(ParseResult result, int startIndex, int endIndex, string description)
        {
            if (startIndex < 0 || endIndex < startIndex || startIndex >= _tokens.Count)
                return;

            if (endIndex >= _tokens.Count)
                endIndex = _tokens.Count - 1;

            var first = _tokens[startIndex];
            var sb = new StringBuilder();

            for (int i = startIndex; i <= endIndex; i++)
            {
                if (sb.Length > 0)
                    sb.Append(' ');

                sb.Append(_tokens[i].Value);
            }

            result.Errors.Add(new SyntaxError
            {
                Fragment = sb.ToString(),
                Line = first.Line,
                Column = first.StartColumn,
                AbsolutePosition = first.AbsolutePosition,
                Description = description
            });
        }

        private bool IsDeclarationStart(PascalEnumScanner.Token token)
        {
            return IsTypeKeyword(token) || IsTypeLikeTypo(token);
        }

        private bool LooksLikeEnumDeclarationStart(int startIndex)
        {
            if (startIndex < 0 || startIndex >= _tokens.Count)
                return false;

            for (int i = startIndex; i < _tokens.Count; i++)
            {
                if (_tokens[i].Value == "=")
                    return true;

                if (i > startIndex && IsDeclarationStart(_tokens[i]))
                    return false;
            }

            return false;
        }

        private bool IsTypeKeyword(PascalEnumScanner.Token token)
        {
            return token != null &&
                   token.Type == PascalEnumScanner.TokenType.KEYWORD &&
                   string.Equals(token.Value, "type", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsTypeLikeTypo(PascalEnumScanner.Token token)
        {
            if (token == null || token.Type != PascalEnumScanner.TokenType.IDENTIFIER)
                return false;

            string value = token.Value ?? string.Empty;

            if (string.Equals(value, "type", StringComparison.OrdinalIgnoreCase))
                return true;

            if (value.Length >= 3 &&
                value.Length <= 5 &&
                value.StartsWith("typ", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private bool IsTokenIdentifier(PascalEnumScanner.Token token)
        {
            return token != null && token.Type == PascalEnumScanner.TokenType.IDENTIFIER;
        }

        private bool LooksLikeEnumItemStart(PascalEnumScanner.Token token)
        {
            return IsTokenIdentifier(token) ||
                   token != null && token.Type == PascalEnumScanner.TokenType.NUMBER ||
                   token != null && token.Type == PascalEnumScanner.TokenType.BOOLEAN;
        }

        private void AddEndOfInputError(ParseResult result, string description)
        {
            if (_tokens.Count == 0)
            {
                result.Errors.Add(new SyntaxError
                {
                    Fragment = "<пусто>",
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
                Fragment = last.Value,
                Line = last.Line,
                Column = last.EndColumn,
                Description = description,
                AbsolutePosition = last.AbsolutePosition
            });
        }

        private void AddError(ParseResult result, PascalEnumScanner.Token token, string description)
        {
            result.Errors.Add(new SyntaxError
            {
                Fragment = token != null ? token.Value : "<null>",
                Line = token != null ? token.Line : 1,
                Column = token != null ? token.StartColumn : 1,
                Description = description,
                AbsolutePosition = token != null ? token.AbsolutePosition : 0
            });
        }

        public static IEnumerable<SyntaxError> FilterCascadingAgainstLexical(
            IEnumerable<SyntaxError> syntaxErrors,
            IEnumerable<PascalEnumScanner.Error> lexicalErrors)
        {
            if (syntaxErrors == null)
                yield break;

            foreach (var error in syntaxErrors)
                yield return error;
        }
    }
}
