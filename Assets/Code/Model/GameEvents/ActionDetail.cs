using Assets.Code.Model.Traits;

namespace Assets.Code.Model.GameEvents {
    public class ActionDetail {
        public ActionType type;
        public Trait trait;
    }

    public enum ActionType {
        None, ActivateTrait
    }
}
