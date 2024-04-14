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
        public bool flipX, flipZ;

        public Chunk(int index, bool flipX, bool flipZ) {
            this.index = index;
            this.flipX = flipX;
            this.flipZ = flipZ;
        }
    }
}
