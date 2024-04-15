using Assets.Code.Model.Skills;
using UnityEngine;

namespace Assets.Code.Model.GameEvents {
    public class ActionDetail {
        public ActionType type;
        public Skill skill;
        public Vector3[] positions;
    }

    public enum ActionType {
        None, ActivateSkill
    }
}
