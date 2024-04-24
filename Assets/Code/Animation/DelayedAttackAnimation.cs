using Assets.Code.Model;
using Assets.Code.Model.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Code.Animation
{
    public class DelayedAttackAnimation : AnimationBase {
        Unit source, target;
        int amount;

        public DelayedAttackAnimation(Unit source, Unit target, int amount) : base(0) {
            this.source = source;
            this.target = target;
            this.amount = amount;
        }

        public override void Finish() {
            source.Attack(target, amount);
        }
    }
}
