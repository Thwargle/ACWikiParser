using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WikiParser
{
    class CoordinatesParser
    {
        public enum ResultCode { Success, Failure, Multiple, BadParse };
        public class Result { public ResultCode Code; public string Coords; public Result() { Code = ResultCode.Failure; } }
        public Result FindCoordinates(string text)
        {
            const string pattern = @"\d+\.?\d*[NS],?\s*\d+\.?\d*[EW]";
            Regex r = new Regex(pattern);
            var matches = r.Matches(text);
            if (matches.Count == 0) { return new Result() { Code = ResultCode.Failure }; }
            if (matches.Count > 1) { return new Result() { Code = ResultCode.Multiple }; }
            if (matches[0].Groups.Count != 1) { return new Result() { Code = ResultCode.BadParse }; }
            string coords = matches[0].Groups[0].Value;
            var result = new Result() { Code = ResultCode.Success, Coords = coords };
            return result;
        }
    }
}
