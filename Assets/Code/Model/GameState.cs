﻿using Assets.Code.Model.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model
{
    public class GameState {
        static UnitTemplate TEMPLATE_SUMMONER = new UnitTemplate("Summoner", 8, 6, 0);
        static UnitTemplate TEMPLATE_BASIC = new UnitTemplate("Summon", 7, 8, 3);

        public GameEventManager gameEventManager;
        public List<UnitTemplate> summonTemplates;
        public List<Chunk> chunks;
        public List<Unit> units;

        public GameState() {
            gameEventManager = new GameEventManager();
            summonTemplates = new List<UnitTemplate>();
            chunks = new List<Chunk>();
            chunks.Add(new Chunk(0, false));
            units = new List<Unit>();
            AddUnit(new Unit(this, UnitControlType.Summoner, new Vector3(0, 2.5f, -8), TEMPLATE_SUMMONER));
            AddUnit(new Unit(this, UnitControlType.Ally, new Vector3(0, -1.5f, 8), TEMPLATE_BASIC));
            AddUnit(new Unit(this, UnitControlType.Enemy, new Vector3(4, -1.5f, 8), TEMPLATE_BASIC));
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
