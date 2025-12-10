using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPCPaintings.Framework
{
    public class HSVPair
    {
        public int hue;

        public int saturation;

        public int value;

        public HSVPair(int hue, int saturation, int value)
        {
            this.hue = hue;
            this.saturation = saturation;
            this.value = value;
        }
    }
}
