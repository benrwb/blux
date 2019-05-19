using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blux
{
    class ThresholdLookup
    {
        int[] outvalues;
        int[] thresholds;

        public ThresholdLookup(int nlevels)
        {
            outvalues = new int[nlevels]; // e.g. [0,127,255]
            float divisor = nlevels - 1;
            for (int i = 0; i < nlevels; i++)
            {
                outvalues[i] = (int)((255 / divisor) * i);
            }

            thresholds = new int[nlevels - 1]; // e.g. [85,170]
            for (int i = 0; i < (nlevels - 1); i++)
            {
                thresholds[i] = (int)((255 / (float)nlevels) * (i + 1));
            }
        }

        public int Lookup(int v)
        {
            for (int t = 0; t < thresholds.Length; t++)
            {
                if (v < thresholds[t])
                    return outvalues[t];
            }
            return outvalues[thresholds.Length];
        }
    }
}
