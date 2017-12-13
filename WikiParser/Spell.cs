using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiParser
{
    class Spell : IComparable
    {
        public string Name;
        public string Type;

        public int CompareTo(object that)
        {
            if (that is Spell) { return CompareTo(that as Spell); }
                return 0;
        }
        public int CompareTo(Spell that)
        {
            int c = Name.CompareTo(that.Name);
            if (c != 0) return c;
                return Type.CompareTo(that.Type);
        }
    }
}
