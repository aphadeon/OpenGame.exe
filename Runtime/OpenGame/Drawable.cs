using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGame.Runtime
{
    public class Drawable
    {
        public DateTime created_at;
        internal virtual void draw() { }
        public int z = 0;
    }
}

