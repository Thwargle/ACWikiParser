using System;
using System.Collections.Generic;
using System.Text;

namespace WikiParser
{
    class Npc : IComparable
    {
        public string Name;
        public List<string> Coordinates = new List<string>();
        public string Type;
        public string Description;

        public int CompareTo(object that)
        {
            if (that is Npc) { return CompareTo(that as Npc); }
            return 0;
        }
        public int CompareTo(Npc that)
        {
            int c = Name.CompareTo(that.Name);
            if (c != 0) return c;
                return Type.CompareTo(that.Type);
        }
    }
}
