using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Model
{
    public class Chunk {
        public int index;
        public bool flipped;

        public Chunk(int index, bool flipped) {
            this.index = index;
            this.flipped = flipped;
        }
    }
}
