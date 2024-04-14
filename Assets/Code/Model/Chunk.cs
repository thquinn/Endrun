using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model
{
    public class Chunk {
        public int index;
        public Vector3 position;
        public bool flipX, flipY;

        public Chunk(int index, Vector3 position, bool flipX, bool flipY) {
            this.position = position;
            this.index = index;
            this.flipX = flipX;
            this.flipY = flipY;
        }
    }
}
