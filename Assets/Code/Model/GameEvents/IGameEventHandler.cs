using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Model.GameEvents
{
    public interface IGameEventHandler {
        public bool Handle(GameEvent e);
    }

    public interface IGameEventMonoBehaviourHandler : IGameEventHandler {
        public void Reregister(GameEventManager gameEventManager);
    }
}
