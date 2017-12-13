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
        enum ParseState { Nothing, NPC, Spell };
        private ParseState _state = ParseState.Nothing;
        private Spell _currentSpell = null;
        private SpellList _allSpells = new SpellList();
        private Npc _currentNpc = null;
        private NpcList _allNpcs = new NpcList();
        private int _badNpcCoords;
        private int _multipleNpcCoords;
        private int _singleNpcCoords;
        private int _learnableSpell;
        private int _itemSpell;
        private int _creatureSpell;
        private int _badSpells;
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
                    case ParseState.Spell:
                        ParseSpell(line);
                        break;
                }
            }
            WriteAllNpcs();
            Console.WriteLine("-----");
            Console.WriteLine("Single NPC Coords: {0}", _singleNpcCoords);
            Console.WriteLine("Multiple NPC Coords: {0}", _multipleNpcCoords);
            Console.WriteLine("Bad NPC Coords: {0}", _badNpcCoords);
            WriteAllSpells();
            Console.WriteLine("-----");
            Console.WriteLine("Learnable Spells: {0}", _learnableSpell);
            Console.WriteLine("Item Spells: {0}", _itemSpell);
            Console.WriteLine("Creature Spells: {0}", _creatureSpell);
            Console.WriteLine("Bad Spells: {0}", _badSpells);

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
                string fname = "npcs.json";
                System.IO.File.WriteAllText(fname, text);
            }
        }

        private void WriteAllSpells()
        {
            var originalSpellList = _allSpells.GetAllSpells();
            var combinedList = new List<Spell>();
            foreach (var spellset in originalSpellList)
            {
                string name = Encode(spellset[0].Name);
                string type = Encode(spellset[0].Type);
                var joinedSpell = new Spell() { Name = name, Type = type};
                combinedList.Add(joinedSpell);
            }
            combinedList.Sort();
            string text = JsonConvert.SerializeObject(combinedList, Formatting.Indented);
            if (text != null)
            {
                string fname = "spells.json";
                System.IO.File.WriteAllText(fname, text);
            }
        }


        /*
        {{Spell Table|{{Spell Row|Type=Learnable|Acid Bane I|30 min|25.0|10|1|Increases a shield or piece of armor's resistance to acid damage by 10%. Target yourself to cast this spell on all of your equipped armor.|{{Spell Formula|Lead Scarab|Hyssop|Powdered Onyx|Gypsum|Ashwood Talisman}}}}
        */

        private void ParseSpell(string line)
        {
            if (line.Contains("Type=Learnable|"))
            {
                int offset = line.IndexOf('|');
                if (offset < 0) return;
                string[] lineArray = line.Split('|');
                _currentSpell.Type = lineArray[1].ToString();
                _currentSpell.Name = lineArray[2].ToString();
                ++_learnableSpell;
                _allSpells.AddSpell(_currentSpell);
                return;
            }
            if (line.Contains("Type=Item|"))
            {
                int offset = line.IndexOf('|');
                if (offset < 0) return;
                string[] lineArray = line.Split('|');
                _currentSpell.Type = lineArray[1].ToString();
                _currentSpell.Name = lineArray[2].ToString();
                ++_itemSpell;
                _allSpells.AddSpell(_currentSpell);
                return;
            }
            if (line.Contains("Type=Creature|"))
            {
                int offset = line.IndexOf('|');
                if (offset < 0) return;
                string[] lineArray = line.Split('|');
                _currentSpell.Type = lineArray[1].ToString();
                _currentSpell.Name = lineArray[2].ToString();
                ++_creatureSpell;
                _allSpells.AddSpell(_currentSpell);
                return;
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
            if (line.Contains("{{Spell Row"))
            {
                _state = ParseState.Spell;
                _currentSpell = new Spell();
                return;
            }
        }
    }
}
