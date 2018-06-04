using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenglFirst
{
    class point
    {
        public decimal x;
        public decimal y;
        public bool eaten;
        public point(decimal x, decimal y, bool eaten) {
            this.x = x;
            this.y = y;
            this.eaten = eaten;

        }
    }
}
