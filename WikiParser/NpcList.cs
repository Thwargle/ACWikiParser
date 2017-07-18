using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiParser
{
    class NpcList
    {
        private Dictionary<string, List<Npc>> _allNpcs = new Dictionary<string, List<Npc>>();
        public void AddNpc(Npc npc)
        {
            string key = Key(npc);
            List<Npc> list = null;
            if (_allNpcs.ContainsKey(key))
            {
                list = _allNpcs[key];
            }
            else
            {
                list = new List<Npc>();
            }
            list.Add(npc);
            _allNpcs[key] = list;
        }
        public List<List<Npc>> GetAllNpcs() { return _allNpcs.Values.ToList(); }
        private string Key(Npc npc)
        {
            return npc.Name.Trim() + "-" + npc.Type.Trim();
        }
    }
}
