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
        public Predicate<object> predicate;
        public RangePreviewType groundTargetingType;
        public float groundTargetingRadius;

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
        public SkillDecision(Skill skill, string prompt, RangePreviewType rangePreviewType, float baseRadius, Predicate<object> predicate, RangePreviewType groundTargetingType, float groundTargetingRadius) {
            this.skill = skill;
            this.prompt = prompt;
            this.rangePreviewType = rangePreviewType;
            this.baseRadius = baseRadius;
            this.predicate = predicate;
            this.groundTargetingType = groundTargetingType;
            this.groundTargetingRadius = groundTargetingRadius;
        }
        public bool IsValid(object o) {
            if (o == null) return false;
            bool isChoice = choices != null && choices.Contains(o);
            bool isPredicated = predicate != null && predicate.Invoke(o);
            return isChoice || isPredicated;
        }
        public void MakeDecision(object choice) {
            Debug.Assert(IsValid(choice));
            skill.Resolve(choice);
        }
    }

    public enum RangePreviewType {
        None, Ring, Sphere, Cone
    }
}
