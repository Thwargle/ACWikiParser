using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WikiParser
{
    class Parser
    {
        enum ParseState { Nothing, NPC };
        private ParseState _state = ParseState.Nothing;
        private Npc _npc = null;
        private int _badNpcCoords;
        private int _multipleNpcCoords;
        private int _singleNpcCoords;
        public void Parse(StreamReader reader)
        {
            // Clear files
            WriteNpc(null);

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
            Console.WriteLine("-----");
            Console.WriteLine("Single NPC Coords: {0}", _singleNpcCoords);
            Console.WriteLine("Multiple NPC Coords: {0}", _multipleNpcCoords);
            Console.WriteLine("Bad NPC Coords: {0}", _badNpcCoords);
        }

        private void ParseNpcLine(string line)
        {
            if (line.StartsWith(" | NPC Name ="))
            {
                int offset = line.IndexOf('=');
                if (offset < 0) return;
                _npc.Name = line.Substring(offset + 1).Trim();
                return;
            }
            if (line.StartsWith(" | Location ="))
            {
                int offset = line.IndexOf('=');
                if (offset < 0) return;
                string ctext = line.Substring(offset + 1).Trim();
                var cresult = (new CoordinatesParser()).FindCoordinates(ctext);
                if (cresult.Code == CoordinatesParser.ResultCode.Multiple)
                {
                    Console.WriteLine("Multiple NPC Coords: {0}", ctext);
                    ++_multipleNpcCoords;

                }
                else if (cresult.Code == CoordinatesParser.ResultCode.Failure)
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
                    _npc.Coordinates = cresult.CoordsList;
                    ++_singleNpcCoords;
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
                _npc.Type = line.Substring(offset + 1).Trim();
                return;
            }
            if (line.StartsWith(" |  Details ="))
            {
                int offset = line.IndexOf('=');
                if (offset < 0) return;
                _npc.Description = line.Substring(offset + 1).Trim();
                return;
            }
            if (line.StartsWith("}}"))
            {
                if (_npc.Coordinates.Count > 0)
                {
                    WriteNpc(_npc);
                }
                _state = ParseState.Nothing;
            }
        }

        private void WriteNpc(Npc npc)
        {
            if (npc == null)
            {
                File.Delete("npcs.txt");
                return;
            }
            using (StreamWriter writer = File.AppendText("npcs.txt"))
            {
                writer.WriteLine("{0}|{1}|{2}|{3}", Encode(npc.Name), EncodeCoordsList(npc.Coordinates), Encode(npc.Type), Encode(npc.Description));
                writer.Flush();
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
            return txt; // TODO check for vertical line
        }
        private void ParseUnknownLine(string line)
        {
            if (line == "{{NPC Row")
            {
                _state = ParseState.NPC;
                _npc = new Npc();
                return;
            }
        }
    }
}
