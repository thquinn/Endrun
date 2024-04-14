using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Model.GameEvents {
    public class GameEventManager {
        Dictionary<GameEventType, List<GameEventHandler>> handlers;

        public GameEventManager() {
            handlers = new Dictionary<GameEventType, List<GameEventHandler>>();
        }

        public void Listen(GameEventType type, Predicate<GameEvent> predicate, Func<GameEvent, bool> func) {
            if (!handlers.ContainsKey(type)) {
                handlers[type] = new();
            }
            handlers[type].Add(new GameEventHandler(predicate, func));
        }
        public void Unregister(object o) {
            foreach (var l in handlers.Values) {
                for (int i = l.Count - 1; i >= 0; i--) {
                    if (l[i].func.Target == o) {
                        l.RemoveAt(i);
                    }
                }
            }
        }
        public void Trigger(GameEvent e) {
            if (handlers.ContainsKey(e.type)) {
                List<GameEventHandler> eHandlers = new List<GameEventHandler>(handlers[e.type]);
                for (int i = 0; i < eHandlers.Count; i++) {
                    GameEventHandler handler = eHandlers[i];
                    if (handler.predicate == null || handler.predicate.Invoke(e)) {
                        eHandlers.RemoveAt(i--);
                        if (handler.func.Invoke(e)) {
                            i = 0;
                        }
                    }
                }
            }
        }
    }

    public class GameEventHandler {
        // Note on handler predicates:
        //      Conditions for events can either be checked by the handler predicate or in the handler itself.
        // If a handler predicate returns false, that handler will never be checked again for that event even
        // if the event changes. That can be good optimization for conditions that check event fields that are
        // never changed by any handler, but conditions that fail now and may pass after the event is altered
        // by other handlers should be checked in the handler itself.
        //
        // Example: MonsterSpottedEnemy
        //      The charSource and charTarget are very unlikely to ever be changed by a handler, so these can
        // be checked by the predicate. (In practice, nothing ever modifies this event so it doesn't matter
        // where we check it.)
        //
        // Example: "If you would deal at least 20 damage, double it"
        //      Because you can expect many handlers to modify damage, the >=20 condition should be checked in
        // the handler, NOT the predicate. Just because damage isn't >=20 right now, doesn't mean it won't be
        // after other handlers touch it.
        //
        // Example: "1% chance to die each turn"
        //      Because the 1% chance is independent of event fields, that roll can be done by the predicate.
        public Predicate<GameEvent> predicate;
        public Func<GameEvent, bool> func; // returns true if the event was modified

        public GameEventHandler(Predicate<GameEvent> predicate, Func<GameEvent, bool> func) {
            this.predicate = predicate;
            this.func = func;
        }
    }

    public class GameEvent {
        public GameEventType type;
        public bool canceled;
        public Unit unitSource, unitTarget;
        public float amount;
        public float multiplier;
        public ActionDetail actionDetail;
        public GameEvent Trigger() {
            GameStateManagerScript.instance.gameState.gameEventManager.Trigger(this);
            return this;
        }
    }
    public enum GameEventType {
        None, Damage, TurnStart
    }
}
