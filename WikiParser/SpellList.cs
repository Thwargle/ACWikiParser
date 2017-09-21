using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiParser
{
    class SpellList
    {
        private Dictionary<string, List<Spell>> _allSpells = new Dictionary<string, List<Spell>>();

        public void AddSpell(Spell spell)
        {
            string key = Key(spell);
            List<Spell> list = null;
            if (_allSpells.ContainsKey(key))
            {
                list = _allSpells[key];
            }
            else
            {
                list = new List<Spell>();
            }
            list.Add(spell);
            _allSpells[key] = list;
        }
        public List<List<Spell>> GetAllSpells() { return _allSpells.Values.ToList(); }
        private string Key(Spell spell)
        {
            return spell.Name.Trim() + "-" + spell.Type.Trim();
        }
    }
}
