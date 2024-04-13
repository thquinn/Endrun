using Assets.Code.Model.Traits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model
{
    public struct UnitTemplate
    {
        public string name;
        public Vector2Int hp;
        public Vector2 movement;
        public int focusCost;
        public List<Trait> traits;

        public UnitTemplate(string name, int hp, float movement, int cost, params Trait[] traits) {
            this.name = name;
            this.hp = new Vector2Int(hp, hp);
            this.movement = new Vector2(movement, movement);
            this.focusCost = cost;
            this.traits = new List<Trait>(traits);
        }
    }
}
