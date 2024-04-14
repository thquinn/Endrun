using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Model.Skills
{
    public class SkillDecision
    {
        public Skill skill;
        public string prompt;
        public RangePreviewType rangePreviewType;
        public float baseRadius;
        public HashSet<object> choices;

        public SkillDecision(Skill skill, string prompt, params object[] choices) {
            this.skill = skill;
            this.prompt = prompt;
            this.choices = new HashSet<object>(choices);
        }
        public SkillDecision(Skill skill, string prompt, RangePreviewType rangePreviewType, float baseRadius, params object[] choices) {
            this.skill = skill;
            this.prompt = prompt;
            this.rangePreviewType = rangePreviewType;
            this.baseRadius = baseRadius;
            this.choices = new HashSet<object>(choices);
        }
        public bool IsAChoice(object o) {
            return o != null && choices.Contains(o);
        }
        public void MakeDecision(object choice) {
            Debug.Assert(choices.Contains(choice));
            skill.Resolve(choice);
        }
    }

    public enum RangePreviewType {
        None, Ring, Sphere, Cone
    }
}
