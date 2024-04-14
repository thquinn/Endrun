using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code
{
    public static class Constants {
        public static int BALANCE_CHUNK_TIMER = 50; // # ticks before

        public static float COMBAT_MELEE_RADIUS = 2.25f;
        public static float COMBAT_SUMMON_RADIUS = 6f;
        public static float COMBAT_CONE_RANGE_MULTIPLIER = .1f; // each unit of height advantage increases the range multiplier by this much
    }
}
