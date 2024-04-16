using Assets.Code.Model.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model
{
    public class ManaCrystal {
        public Vector3 position;
        Vector3 collisionPosition;
        public GameState gameState;
        public bool collected;

        public ManaCrystal(GameState gameState, Vector3 position) {
            this.gameState = gameState;
            this.position = position;
            collisionPosition = position + new Vector3(0, 1.5f, 0);
            gameState.gameEventManager.Listen(
                GameEventType.MovementSegment,
                e => e.unitSource.playerControlled,
                Handle
            );
        }

        public bool IsOffChunks() {
            float xMin = -10, xMax = 10;
            float zMin = gameState.chunks[0].position.z - 10;
            float zMax = gameState.chunks[1].position.z + 10;
            return position.x < xMin || position.x > xMax || position.z < zMin || position.z > zMax;
        }

        bool Handle(GameEvent e) {
            // TODO: this doesn't factor in units of different sizes.
            if (collected) { return false; }
            if (Vector3.Distance(collisionPosition, e.unitSource.position) <= Constants.MANA_CRYSTAL_COLLECT_RANGE) {
                gameState.GainMana(e.unitSource.isSummoner ? 2 : 1);
                collected = true;
                gameState.manaCrystals.Remove(this);
                gameState.gameEventManager.Unregister(this);
            }
            return false;
        }
    }
}
