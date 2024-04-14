using Assets.Code.Model.Skills;

namespace Assets.Code.Model.GameEvents {
    public class ActionDetail {
        public ActionType type;
        public Skill skill;
    }

    public enum ActionType {
        None, ActivateSkill
    }
}
