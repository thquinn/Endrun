using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Model.Traits
{
    public abstract class Trait {
        public Unit unit;
        public string name;
        public int level;
    }

    public enum TraitType
    {
        Active, Passive
    }
}
