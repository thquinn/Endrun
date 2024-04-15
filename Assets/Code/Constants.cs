using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code
{
    public static class Constants {
        public static int BALANCE_CHUNK_TIMER = 50; // # ticks before
        public static int BALANCE_BASE_TURN_TICKS = 10;

        public static float COMBAT_MELEE_RADIUS = 2.25f;
        public static float COMBAT_SUMMON_RADIUS = 4f;
        public static float COMBAT_CONE_RANGE_MULTIPLIER = .1f; // each unit of height advantage increases the range multiplier by this much

        public static Color COLOR_ALLY = new Color(0.5019608f, 0.8156863f, 1f);
        public static Color COLOR_ENEMY = new Color(1f, 0.7490196f, 0.7490196f);
    }
}
