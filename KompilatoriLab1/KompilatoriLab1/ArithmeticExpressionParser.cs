using System;
using System.Collections.Generic;
using System.Text;

namespace KompilatoriLab1
{
    public class ArithmeticExpressionParser
    {
        public enum TokenType
        {
            NUMBER,
            IDENTIFIER,
            PLUS,
            MINUS,
            MULTIPLY,
            DIVIDE,
            LPAREN,
            RPAREN,
            END,
            ERROR
        }

        public class Token
        {
            public TokenType Type { get; set; }
            public string Value { get; set; }
            public int Position { get; set; }
        }

        public class SyntaxError
        {
            public string Fragment { get; set; }
            public int Position { get; set; }
            public string Description { get; set; }
        }

        public class Tetrad
        {
            public string Op { get; set; }
            public string Arg1 { get; set; }
            public string Arg2 { get; set; }
            public string Result { get; set; }

            public override string ToString()
            {
                return $"({Op}, {Arg1}, {Arg2}, {Result})";
            }
        }

        public class ParseResult
        {
            public bool Success { get; set; }
            public List<SyntaxError> Errors { get; } = new List<SyntaxError>();
            public List<Tetrad> Tetrads { get; } = new List<Tetrad>();
            public string PolishNotation { get; set; } = "";
            public double? ComputedValue { get; set; }
            public string AstText { get; set; } = "";
            public List<Token> Tokens { get; } = new List<Token>();
        }

        private string _input;
        private int _position;
        private List<Token> _tokens;
        private int _tokenIndex;
        private ParseResult _result;
        private int _tempCounter = 0;
        private Dictionary<string, double> _variables;
        private int _lastErrorPosition = -1;

        public ArithmeticExpressionParser()
        {
            _variables = new Dictionary<string, double>();
        }

        public void SetVariable(string name, double value)
        {
            _variables[name] = value;
        }

        public void ClearVariables()
        {
            _variables.Clear();
        }

        public ParseResult Parse(string input)
        {
            _result = new ParseResult();
            _input = input ?? "";
            _position = 0;
            _tempCounter = 0;
            _lastErrorPosition = -1;

            _tokens = LexicalAnalysis();
            foreach (var token in _tokens)
            {
                _result.Tokens.Add(token);
            }

            if (_tokens.Count == 0)
            {
                _result.Errors.Add(new SyntaxError
                {
                    Fragment = "",
                    Position = 0,
                    Description = "Пустой ввод"
                });
                _result.Success = false;
                return _result;
            }

            _tokenIndex = 0;
            _tempCounter = 0;
            _result.Tetrads.Clear();

            ParseE();

            if (_tokenIndex < _tokens.Count && _tokens[_tokenIndex].Type != TokenType.END)
            {
                AddError(_tokens[_tokenIndex].Position, $"Неожиданный токен: {_tokens[_tokenIndex].Value}");
            }

            _result.Success = _result.Errors.Count == 0;

            _result.PolishNotation = BuildPolishNotation();

            if (_result.Success)
            {
                _result.ComputedValue = EvaluateExpression();
                if (_result.ComputedValue == null && _result.Errors.Count == 0)
                {
                    AddError(0, "Невозможно вычислить значение: не заданы значения переменных");
                }
            }

            _result.AstText = BuildAstText();
            return _result;
        }

        private void AddError(int position, string description)
        {
            if (_lastErrorPosition == position) return;

            _lastErrorPosition = position;

            string fragment = "";
            if (position >= 0 && position < _input.Length)
            {
                int start = position;
                int end = position;
                while (start > 0 && !char.IsWhiteSpace(_input[start - 1]) &&
                       _input[start - 1] != '(' && _input[start - 1] != ')' &&
                       _input[start - 1] != '+' && _input[start - 1] != '-' &&
                       _input[start - 1] != '*' && _input[start - 1] != '/')
                    start--;
                while (end < _input.Length - 1 && !char.IsWhiteSpace(_input[end + 1]) &&
                       _input[end + 1] != '(' && _input[end + 1] != ')' &&
                       _input[end + 1] != '+' && _input[end + 1] != '-' &&
                       _input[end + 1] != '*' && _input[end + 1] != '/')
                    end++;
                fragment = _input.Substring(start, end - start + 1);
            }

            _result.Errors.Add(new SyntaxError
            {
                Fragment = fragment,
                Position = position,
                Description = description
            });
        }

        private List<Token> LexicalAnalysis()
        {
            var tokens = new List<Token>();
            int pos = 0;

            while (pos < _input.Length)
            {
                char c = _input[pos];

                if (char.IsWhiteSpace(c))
                {
                    pos++;
                    continue;
                }

                if (char.IsDigit(c))
                {
                    int start = pos;
                    while (pos < _input.Length && char.IsDigit(_input[pos]))
                        pos++;
                    string number = _input.Substring(start, pos - start);
                    tokens.Add(new Token { Type = TokenType.NUMBER, Value = number, Position = start });
                    continue;
                }

                if (char.IsLetter(c))
                {
                    int start = pos;
                    while (pos < _input.Length && char.IsLetterOrDigit(_input[pos]))
                        pos++;
                    string id = _input.Substring(start, pos - start);
                    tokens.Add(new Token { Type = TokenType.IDENTIFIER, Value = id, Position = start });
                    continue;
                }

                switch (c)
                {
                    case '+':
                        tokens.Add(new Token { Type = TokenType.PLUS, Value = "+", Position = pos });
                        break;
                    case '-':
                        tokens.Add(new Token { Type = TokenType.MINUS, Value = "-", Position = pos });
                        break;
                    case '*':
                        tokens.Add(new Token { Type = TokenType.MULTIPLY, Value = "*", Position = pos });
                        break;
                    case '/':
                        tokens.Add(new Token { Type = TokenType.DIVIDE, Value = "/", Position = pos });
                        break;
                    case '(':
                        tokens.Add(new Token { Type = TokenType.LPAREN, Value = "(", Position = pos });
                        break;
                    case ')':
                        tokens.Add(new Token { Type = TokenType.RPAREN, Value = ")", Position = pos });
                        break;
                    default:
                        AddError(pos, $"Недопустимый символ: '{c}'");
                        break;
                }
                pos++;
            }

            tokens.Add(new Token { Type = TokenType.END, Value = "", Position = _input.Length });
            return tokens;
        }

        private Token CurrentToken => _tokenIndex < _tokens.Count ? _tokens[_tokenIndex] : null;

        private void Advance()
        {
            if (_tokenIndex < _tokens.Count)
                _tokenIndex++;
        }

        private bool Match(TokenType type)
        {
            if (CurrentToken != null && CurrentToken.Type == type)
            {
                Advance();
                return true;
            }
            return false;
        }

        private string NewTemp()
        {
            return $"t{_tempCounter++}";
        }

        private bool IsOperator(TokenType type)
        {
            return type == TokenType.PLUS || type == TokenType.MINUS ||
                   type == TokenType.MULTIPLY || type == TokenType.DIVIDE;
        }

        private bool IsUnaryMinus()
        {
            if (CurrentToken == null || CurrentToken.Type != TokenType.MINUS)
                return false;

            if (_tokenIndex == 0)
                return true;

            Token prev = _tokens[_tokenIndex - 1];
            return prev.Type == TokenType.LPAREN || prev.Type == TokenType.END || IsOperator(prev.Type);
        }

        private void ParseE()
        {
            ParseT();
            ParseA();
        }

        private void ParseA()
        {
            if (CurrentToken == null) return;

            if (CurrentToken.Type == TokenType.PLUS)
            {
                Advance();
                ParseT();
                ParseA();
            }
            else if (CurrentToken.Type == TokenType.MINUS)
            {
                Advance();
                ParseT();
                ParseA();
            }
        }

        private void ParseT()
        {
            ParseF();
            ParseB();
        }

        private void ParseB()
        {
            if (CurrentToken == null) return;

            if (CurrentToken.Type == TokenType.MULTIPLY)
            {
                Advance();
                ParseF();
                ParseB();
            }
            else if (CurrentToken.Type == TokenType.DIVIDE)
            {
                Advance();
                ParseF();
                ParseB();
            }
        }

        private void ParseF()
        {
            if (CurrentToken == null)
            {
                AddError(_input.Length, "Ожидался операнд после оператора");
                return;
            }

            if (IsUnaryMinus())
            {
                Advance();
                ParseF();
                return;
            }

            if (CurrentToken.Type == TokenType.NUMBER)
            {
                Advance();
                return;
            }
            else if (CurrentToken.Type == TokenType.IDENTIFIER)
            {
                Advance();
                return;
            }
            else if (CurrentToken.Type == TokenType.LPAREN)
            {
                int parenPos = CurrentToken.Position;
                Advance();
                ParseE();

                if (CurrentToken == null)
                {
                    AddError(parenPos, "Ожидалась закрывающая скобка ')'");
                }
                else if (CurrentToken.Type == TokenType.RPAREN)
                {
                    Advance();
                }
                else
                {
                    AddError(parenPos, "Ожидалась закрывающая скобка ')'");
                }
                return;
            }
            else if (CurrentToken.Type == TokenType.RPAREN)
            {
                AddError(CurrentToken.Position, "Неожиданная закрывающая скобка ')'");
                Advance();
                ParseF();
                return;
            }
            else if (CurrentToken.Type == TokenType.PLUS || CurrentToken.Type == TokenType.MINUS ||
                     CurrentToken.Type == TokenType.MULTIPLY || CurrentToken.Type == TokenType.DIVIDE)
            {
                AddError(CurrentToken.Position, $"Ожидался операнд после оператора '{CurrentToken.Value}'");
                Advance();
                ParseF();
                return;
            }
            else if (CurrentToken.Type == TokenType.END)
            {
                AddError(_input.Length, "Ожидался операнд после оператора");
                return;
            }
            else
            {
                AddError(CurrentToken.Position, $"Неожиданный токен: {CurrentToken.Value}");
                Advance();
                ParseF();
                return;
            }
        }

        private string BuildPolishNotation()
        {
            if (_result.Tetrads.Count == 0 && _tokens.Count == 0)
                return "";

            return BuildPolishFromExpression();
        }

        private string BuildPolishFromExpression()
        {
            var output = new List<string>();
            var operators = new Stack<string>();
            bool lastWasOperand = false;

            foreach (var token in _tokens)
            {
                if (token.Type == TokenType.END) break;

                switch (token.Type)
                {
                    case TokenType.NUMBER:
                    case TokenType.IDENTIFIER:
                        output.Add(token.Value);
                        lastWasOperand = true;
                        break;

                    case TokenType.PLUS:
                    case TokenType.MINUS:
                        if (!lastWasOperand && token.Type == TokenType.MINUS)
                        {
                            output.Add("0");
                            while (operators.Count > 0 && operators.Peek() != "(")
                            {
                                output.Add(operators.Pop());
                            }
                            operators.Push(token.Value);
                        }
                        else
                        {
                            while (operators.Count > 0 && operators.Peek() != "(")
                            {
                                output.Add(operators.Pop());
                            }
                            operators.Push(token.Value);
                        }
                        lastWasOperand = false;
                        break;

                    case TokenType.MULTIPLY:
                    case TokenType.DIVIDE:
                        while (operators.Count > 0 && (operators.Peek() == "*" || operators.Peek() == "/"))
                        {
                            output.Add(operators.Pop());
                        }
                        operators.Push(token.Value);
                        lastWasOperand = false;
                        break;

                    case TokenType.LPAREN:
                        operators.Push("(");
                        lastWasOperand = false;
                        break;

                    case TokenType.RPAREN:
                        while (operators.Count > 0 && operators.Peek() != "(")
                        {
                            output.Add(operators.Pop());
                        }
                        if (operators.Count > 0 && operators.Peek() == "(")
                            operators.Pop();
                        lastWasOperand = true;
                        break;
                }
            }

            while (operators.Count > 0)
            {
                output.Add(operators.Pop());
            }

            return string.Join(" ", output);
        }

        private double? EvaluateExpression()
        {
            var stack = new Stack<double>();

            string polish = BuildPolishFromExpression();
            string[] tokensArray = polish.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokensArray)
            {
                if (double.TryParse(token, out double num))
                {
                    stack.Push(num);
                }
                else if (_variables.ContainsKey(token))
                {
                    stack.Push(_variables[token]);
                }
                else if (token == "+" || token == "-" || token == "*" || token == "/")
                {
                    if (stack.Count < 2)
                    {
                        if (token == "-" && stack.Count == 1)
                        {
                            double a = stack.Pop();
                            stack.Push(-a);
                        }
                        else
                            return null;
                    }
                    else
                    {
                        double b = stack.Pop();
                        double a = stack.Pop();

                        switch (token)
                        {
                            case "+": stack.Push(a + b); break;
                            case "-": stack.Push(a - b); break;
                            case "*": stack.Push(a * b); break;
                            case "/":
                                if (Math.Abs(b) < 1e-10)
                                {
                                    AddError(0, "Деление на ноль");
                                    return null;
                                }
                                stack.Push(a / b);
                                break;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }

            return stack.Count == 1 ? stack.Pop() : (double?)null;
        }

        private string BuildAstText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== АБСТРАКТНОЕ СИНТАКСИЧЕСКОЕ ДЕРЕВО (AST) ===");
            sb.AppendLine();

            if (_tokens.Count == 0)
            {
                sb.AppendLine("Нет токенов для построения AST");
                return sb.ToString();
            }

            BuildAstRecursive(sb, "", true);

            return sb.ToString();
        }

        private void BuildAstRecursive(StringBuilder sb, string indent, bool isLast)
        {
            sb.Append(indent);
            sb.Append(isLast ? "└── " : "├── ");
            sb.AppendLine("Expression");

            string subIndent = indent + (isLast ? "    " : "│   ");
            BuildAstFromTokens(sb, subIndent, 0, _tokens.Count - 1);
        }

        private void BuildAstFromTokens(StringBuilder sb, string indent, int start, int end)
        {
            if (start > end) return;

            int minPriority = int.MaxValue;
            int opIndex = -1;

            for (int i = start; i <= end; i++)
            {
                var token = _tokens[i];
                if (token.Type == TokenType.END) continue;

                int priority = GetPriority(token.Type);
                if (priority > 0 && priority <= minPriority)
                {
                    minPriority = priority;
                    opIndex = i;
                }
            }

            if (opIndex != -1 && minPriority != int.MaxValue)
            {
                var opToken = _tokens[opIndex];
                sb.Append(indent);
                sb.Append("├── ");
                sb.AppendLine($"Operator: {opToken.Value}");

                string childIndent = indent + "│   ";
                sb.Append(childIndent);
                sb.AppendLine("├── Left:");
                BuildAstFromTokens(sb, childIndent + "│   ", start, opIndex - 1);

                sb.Append(childIndent);
                sb.AppendLine("└── Right:");
                BuildAstFromTokens(sb, childIndent + "    ", opIndex + 1, end);
            }
            else
            {
                for (int i = start; i <= end; i++)
                {
                    var token = _tokens[i];
                    if (token.Type == TokenType.END) continue;

                    if (token.Type == TokenType.NUMBER || token.Type == TokenType.IDENTIFIER)
                    {
                        sb.Append(indent);
                        sb.Append(i == end ? "└── " : "├── ");
                        string typeName = token.Type == TokenType.NUMBER ? "Number" : "Variable";
                        sb.AppendLine($"Leaf: {token.Value} ({typeName})");
                    }
                }
            }
        }

        private int GetPriority(TokenType type)
        {
            switch (type)
            {
                case TokenType.PLUS:
                case TokenType.MINUS:
                    return 1;
                case TokenType.MULTIPLY:
                case TokenType.DIVIDE:
                    return 2;
                default:
                    return -1;
            }
        }
    }
}