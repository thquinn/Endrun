using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code
{
    public static class ID {
        static int i;

        public static int Get() {
            return i++;
        }
    }
}
