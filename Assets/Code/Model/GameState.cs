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
            Unit summoner = new Unit(true, new Vector3(0, 2.5f, -8));
            summoner.isSummoner = true;
            units.Add(summoner);
        }
    }
}
