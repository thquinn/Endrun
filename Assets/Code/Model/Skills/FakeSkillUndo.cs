using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Model.Skills
{
    public class FakeSkillUndo : ActiveSkill {
        public FakeSkillUndo(Unit unit) : base(-1) {
            name = "Undo";
            this.unit = unit;
        }
        public FakeSkillUndo(FakeSkillUndo other) : base(other) { }
        public override Skill Clone() {
            return new FakeSkillUndo(this);
        }

        public override bool CanActivate() {
            return GameStateManagerScript.instance.undoHistory.Count > 0;
        }
        public override bool RequiresAction() {
            return false;
        }

        public override string GetDescription() {
            return string.Format($"Undo the last action.");
        }
        public override string GetIconID() {
            return "undo";
        }

        public override bool WriteUndoHistory() {
            return false;
        }
        public override void Resolve(object choice) {
            GameStateManagerScript.instance.Undo();
        }
    }
}
