using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model
{
    public class GameState {
        public List<Chunk> chunks;
        public List<Unit> units;

        public GameState() {
            chunks = new List<Chunk>();
            chunks.Add(new Chunk(0, false));
            units = new List<Unit>();
            Unit summoner = new Unit(this, true, new Vector3(0, 2.5f, -8));
            summoner.isSummoner = true;
            AddUnit(summoner);
            AddUnit(new Unit(this, true, new Vector3(0, -1.5f, 8)));
        }
        public void AddUnit(Unit unit) {
            if (units.Count == 0) {
                units.Add(unit);
                return;
            }
            for (int i = 1; i < units.Count; i++) {
                if (units[i].ticksUntilTurn > i) {
                    unit.ticksUntilTurn = i;
                    units.Insert(i, unit);
                    return;
                }
            }
            unit.ticksUntilTurn = units.Count;
            units.Add(unit);
        }
    }
}
