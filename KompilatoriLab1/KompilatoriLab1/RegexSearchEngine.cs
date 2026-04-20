using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KompilatoriLab1
{
    public class RegexSearchEngine
    {
        public enum SearchType
        {
            CanadianPostalCode,
            Username,
            Guid
        }

        public class SearchMatch
        {
            public string Value { get; set; }
            public int Line { get; set; }
            public int Column { get; set; }
            public int Length { get; set; }
            public int AbsolutePosition { get; set; }
        }

        public class SearchResult
        {
            public List<SearchMatch> Matches { get; } = new List<SearchMatch>();
        }

        public SearchResult FindMatches(string text, SearchType searchType)
        {
            var result = new SearchResult();

            if (string.IsNullOrEmpty(text))
                return result;

            string pattern = GetPattern(searchType);

            var regex = new Regex(
                pattern,
                RegexOptions.Multiline | RegexOptions.IgnoreCase);

            foreach (Match match in regex.Matches(text))
            {
                GetLineAndColumn(text, match.Index, out int line, out int column);

                result.Matches.Add(new SearchMatch
                {
                    Value = match.Value,
                    Line = line,
                    Column = column,
                    Length = match.Length,
                    AbsolutePosition = match.Index
                });
            }

            return result;
        }

        public string GetPattern(SearchType searchType)
        {
            switch (searchType)
            {
                case SearchType.CanadianPostalCode:
                    return @"\b[ABCEGHJ-NPRSTVXY]\d[ABCEGHJ-NPRSTV-Z][ -]?\d[ABCEGHJ-NPRSTV-Z]\d\b";

                case SearchType.Username:
                    return @"\b[a-z0-9_-]{8,16}\b";

                case SearchType.Guid:
                    return @"\b[0-9A-Fa-f]{8}-(?:[0-9A-Fa-f]{4}-){3}[0-9A-Fa-f]{12}\b";

                default:
                    throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null);
            }
        }

        private void GetLineAndColumn(string text, int absoluteIndex, out int line, out int column)
        {
            line = 1;
            column = 1;

            for (int i = 0; i < absoluteIndex; i++)
            {
                if (text[i] == '\n')
                {
                    line++;
                    column = 1;
                }
                else if (text[i] != '\r')
                {
                    column++;
                }
            }
        }
    }
}