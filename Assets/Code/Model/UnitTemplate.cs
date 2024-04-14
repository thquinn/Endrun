using Assets.Code.Model.Skills;
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
        public string iconID;
        public Vector2Int hp;
        public Vector2 movement;
        public int focusCost;
        public List<Skill> skills;

        public UnitTemplate(string name, string iconID, int hp, float movement, int focusCost, params Skill[] skills) {
            this.name = name;
            this.iconID = iconID;
            this.hp = new Vector2Int(hp, hp);
            this.movement = new Vector2(movement, movement);
            this.focusCost = focusCost;
            this.skills = new List<Skill>(skills);
        }
    }
}
