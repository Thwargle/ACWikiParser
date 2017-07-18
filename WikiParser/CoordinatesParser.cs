using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WikiParser
{
    class CoordinatesParser
    {
        public enum ResultCode { Success, Failure, BadParse };
        public class Result { public ResultCode Code; public List<string> CoordsList; public Result() { Code = ResultCode.Failure; } }
        public Result FindCoordinates(string text)
        {
            const string pattern = @"\d+\.?\d*[NS],?\s*\d+\.?\d*[EW]";
            Regex r = new Regex(pattern);
            var matches = r.Matches(text);
            if (matches.Count == 0) { return new Result() { Code = ResultCode.Failure }; }
            var coordsList = new List<string>();
            foreach (Match match in matches)
            {
                if (match.Groups.Count != 1) { return new Result() { Code = ResultCode.BadParse }; }
                string coords = match.Groups[0].Value;
                coordsList.Add(coords);
            }
            var result = new Result() { Code = ResultCode.Success, CoordsList = coordsList };
            return result;
        }
    }
}
