using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code
{
    public static class Constants {
        public static int BALANCE_CHUNK_TIMER = 61; // # ticks before
        public static int BALANCE_BASE_TURN_TICKS = 10;

        public static int UPGRADES_PER_FOCUS_COST_INCREASE = 3;
        public static int UPGRADE_UNIT_HP = 2;
        public static float UPGRADE_UNIT_MOVEMENT = 1.5f;
        public static int UPGRADE_MAX_FOCUS = 2;

        public static float SPAWN_MIN_DISTANCE_BETWEEN = 1.5f; // enemies, mana, etc must spawn at least this far apart
        public static int SPAWN_MANA_CRYSTALS_PER_LEVEL = 2;

        public static float COMBAT_MELEE_RADIUS = 2.25f;
        public static float COMBAT_SUMMON_RADIUS = 4f;
        public static float COMBAT_CONE_RANGE_MULTIPLIER = .1f; // each unit of height advantage increases the range multiplier by this much
        public static float MANA_CRYSTAL_COLLECT_RANGE = 2f;

        public static Color COLOR_ALLY = new Color(0.5019608f, 0.8156863f, 1f);
        public static Color COLOR_ENEMY = new Color(1f, 0.7490196f, 0.7490196f);
        public static int UI_WARN_TICKS = 25; // if there are this many ticks or fewer left in the left, warn the player in the UI
    }
}
