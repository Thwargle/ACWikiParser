using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using Newtonsoft.Json;

namespace WikiParser
{
    class Parser
    {
        enum ParseState { Nothing, NPC };
        private ParseState _state = ParseState.Nothing;
        private Npc _currentNpc = null;
        private NpcList _allNpcs = new NpcList();
        private int _badNpcCoords;
        private int _multipleNpcCoords;
        private int _singleNpcCoords;
        public void Parse(StreamReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                switch (_state)
                {
                    case ParseState.Nothing:
                        ParseUnknownLine(line);
                        break;
                    case ParseState.NPC:
                        ParseNpcLine(line);
                        break;
                }
            }
            WriteAllNpcs();
            Console.WriteLine("-----");
            Console.WriteLine("Single NPC Coords: {0}", _singleNpcCoords);
            Console.WriteLine("Multiple NPC Coords: {0}", _multipleNpcCoords);
            Console.WriteLine("Bad NPC Coords: {0}", _badNpcCoords);
        }

        private void WriteAllNpcs()
        {
            var originalNpcList = _allNpcs.GetAllNpcs();
            var combinedList = new List<Npc>();
            foreach (var npcset in originalNpcList)
            {
                string name = Encode(npcset[0].Name);
                string type = Encode(npcset[0].Type);
                string desc = Encode(npcset[0].Description); // These might not all be the same but we don't care, we just use the first
                var joinedNpc = new Npc() { Name = name, Type = type, Description = desc };
                foreach (var lilnpc in npcset)
                {
                    joinedNpc.Coordinates.AddRange(lilnpc.Coordinates);
                }
                combinedList.Add(joinedNpc);
            }
            combinedList.Sort();
            string text = JsonConvert.SerializeObject(combinedList, Formatting.Indented);
            if (text != null)
            {
                string fname = "npcs.txt";
                System.IO.File.WriteAllText(fname, text);
            }
        }

        private void ParseNpcLine(string line)
        {
            if (line.StartsWith(" | NPC Name ="))
            {
                int offset = line.IndexOf('=');
                if (offset < 0) return;
                _currentNpc.Name = line.Substring(offset + 1).Trim();
                return;
            }
            if (line.StartsWith(" | Location ="))
            {
                int offset = line.IndexOf('=');
                if (offset < 0) return;
                string ctext = line.Substring(offset + 1).Trim();
                var cresult = (new CoordinatesParser()).FindCoordinates(ctext);
                if (cresult.Code == CoordinatesParser.ResultCode.Failure)
                {
                    Console.WriteLine("Bad NPC Coords: {0}", ctext);
                    ++_badNpcCoords;
                }
                else if (cresult.Code == CoordinatesParser.ResultCode.BadParse)
                {
                    Console.WriteLine("Parser failed due to capture groups: {0}", ctext);
                    ++_badNpcCoords;
                }
                else if (cresult.Code == CoordinatesParser.ResultCode.Success)
                {
                    _currentNpc.Coordinates = cresult.CoordsList;
                    if (cresult.CoordsList.Count > 1)
                    {
                        ++_multipleNpcCoords;
                    }
                    else
                    {
                        ++_singleNpcCoords;
                    }
                }
                else
                {
                    Console.WriteLine("Switch failure: {0}", ctext);
                    throw new Exception("bad code");

                }
                return;
            }
            if (line.StartsWith(" |     Type ="))
            {
                int offset = line.IndexOf('=');
                if (offset < 0) return;
                _currentNpc.Type = line.Substring(offset + 1).Trim();
                return;
            }
            if (line.StartsWith(" |  Details ="))
            {
                int offset = line.IndexOf('=');
                if (offset < 0) return;
                _currentNpc.Description = line.Substring(offset + 1).Trim();
                return;
            }
            if (line.StartsWith("}}"))
            {
                if (_currentNpc.Coordinates.Count > 0)
                {
                    _allNpcs.AddNpc(_currentNpc);
                }
                _state = ParseState.Nothing;
            }
        }

        private static string EncodeCoordsList(List<string> coordsList)
        {
            string txt = string.Join("^", coordsList.ToArray());
            return txt;
        }
        private static string Encode(string txt)
        {
            if (txt == null) { txt = ""; }
            txt = txt.Replace("|", ",> ");
            txt = txt.Replace("^", ",> ");
            txt = txt.Replace("[", "").Replace("]", "");
            // Throw away anything after {{
            int index = txt.IndexOf("{{");
            if (index > 0)
            {
                txt = txt.Substring(0, index);
            }
            txt = txt.Replace("{", "");
            return txt;
        }
        private void ParseUnknownLine(string line)
        {
            if (line == "{{NPC Row")
            {
                _state = ParseState.NPC;
                _currentNpc = new Npc();
                return;
            }
        }
    }
}
