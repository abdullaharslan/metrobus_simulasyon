using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xna_metrobus
{
    public static class Mesafe
    {
        public static float ToPixel(float distanceInMeter)
        {
            return distanceInMeter /3.5f*3.2f;
        }

        internal static float ToMetre(float distanceInPixel)
        {
            return distanceInPixel / 3.2f * 3.5f;
        }
    }
}
