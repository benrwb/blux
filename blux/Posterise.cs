using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blux
{
    class Posterise
    {
        List<MethodDetails> _methods;


        public class MethodDetails
        {
            public string Name;
            public Func<int[],ReturnValues> Function;
            public int slider1Min;
            public int slider1Max;
            public int slider1Default;
            public int slider2Min;
            public int slider2Max;
            public int slider2Default;
            public int slider3Min;
            public int slider3Max;
            public int slider3Default;
            public int slider4Min;
            public int slider4Max;
            public int slider4Default;
        }


        public string[] GetMethodNames()
        {
            return _methods.Select(z => z.Name).ToArray();
        }

        public MethodDetails GetMethodDetails(string methodName)
        {
            return _methods.Single(z => z.Name == methodName);
        }


        public Posterise()
        {
            _methods = new List<MethodDetails>()
            {
                //new MethodDetails() { Name = "Linear", Function = linear },
                //new MethodDetails() { Name = "Noise1", Function = noise1, slider1Min = 0, slider1Max = 25 },
                //new MethodDetails() { Name = "Noise2", Function = noise2, slider1Min = 0, slider1Max = 50 },
                //new MethodDetails() { Name = "Noise3", Function = noise3, slider1Min = 1, slider1Max = 255 },
                //new MethodDetails() { Name = "Posterise1", Function = posterise1, slider1Min = 1, slider1Max = 10, slider1Default = 4 },
                //new MethodDetails() { Name = "Posterise2", Function = posterise2, slider1Min = 2, slider1Max = 10, slider1Default = 4 },
                //new MethodDetails() { Name = "Posterise_2BitCustom2", Function = posterise_2bit_custom2, slider1Min = 50, slider1Max = 200, slider2Min = 150, slider2Max = 250, slider1Default = 124, slider2Default = 231, slider3Min=50, slider3Max=240, slider3Default = 128 },
                new MethodDetails()
                {
                    Name = "posterise_2level_custom (8 colours)",
                    Function = posterise_2level_custom,
                    slider1Min = 100,
                    slider1Max = 255,
                    slider1Default = 173
                },
                new MethodDetails()
                {
                    Name = "posterise_3level_custom (27 colours)",
                    Function = posterise_3level_custom,
                    slider1Min = 50,
                    slider1Max = 200,
                    slider1Default = 80,
                    slider2Min = 150,
                    slider2Max = 250,
                    slider2Default = 230
                },
                new MethodDetails()
                {
                    Name = "posterise_4level_custom (64 colours)",
                    Function = posterise_4level_custom,
                    slider1Min = 50,
                    slider1Max = 200,
                    slider1Default = 80,
                    slider2Min = 150,
                    slider2Max = 250,
                    slider2Default = 171,
                    slider3Min = 150,
                    slider3Max = 250,
                    slider3Default = 236
                },
                new MethodDetails()
                {
                    Name = "posterise_5level_custom (125 colours)",
                    Function = posterise_5level_custom,
                    slider1Min = 10,
                    slider1Max = 150,
                    slider1Default = 53, // changed from 48 to 53 to make Excel scrollbars visible
                    slider2Min = 100,
                    slider2Max = 200,
                    slider2Default = 141,
                    slider3Min = 150,
                    slider3Max = 250,
                    slider3Default = 186,
                    slider4Min = 200,
                    slider4Max = 255,
                    slider4Default = 235
                },
                new MethodDetails() { Name = "Posterise_5level (125 colours)", Function = posterise_5level },
                new MethodDetails() { Name = "Posterise_6level (216 colours)", Function = posterise_6level },
                new MethodDetails() { Name = "Posterise_7level (343 colours)", Function = posterise_7level },
                new MethodDetails() { Name = "Posterise_8level (512 colours)", Function = posterise_8level },
            };
        }

        public class ReturnValues
        {
            public int[] Red;
            public int[] Green;
            public int[] Blue;
            public ReturnValues()
            {
                Red = new int[256];
                Green = new int[256];
                Blue = new int[256];
            }
            public void AssignValue(int position, int value)
            {
                // assign the same value to all 3 channels
                this.Red[position] = value;
                this.Green[position] = value;
                this.Blue[position] = value;
            }
        }

        public ReturnValues ApplyMethod(string methodName, int[] sliderValues = null)
        {
            var details = _methods.Single(z => z.Name == methodName);
            if (sliderValues == null)
                sliderValues = new[] { details.slider1Default, details.slider2Default, details.slider3Default, details.slider4Default };
            return details.Function(sliderValues);
        }


        //private void linear()
        //{
        //    foreach (var array in new[] { _transformR, _transformG, _transformB })
        //        for (int i = 0; i < 256; i++)
        //            array[i] = i;
        //}

        //private void noise1()
        //{
        //    for (int i = 0; i < 256; i++)
        //        foreach (var transform in new[] { _transformR, _transformG, _transformB })
        //            transform[i] = Clamp(i +
        //                (((i / 2) % 2 == 0) ? (int)slider1.Value : 0 - (int)slider1.Value));
        //}

        //private void noise2()
        //{
        //    Random r = new Random();

        //    for (int i = 0; i < 256; i++)
        //        foreach (var transform in new[] { _transformR, _transformG, _transformB })
        //            transform[i] = Clamp(i + (0 - ((int)slider1.Value / 2)) + r.Next((int)slider1.Value));
        //}

        //private void noise3()
        //{
        //    int step =(int)slider1.Value;
        //    for (int i = 0; i < 256; i++)
        //        foreach (var transform in new[] { _transformR, _transformG, _transformB })
        //            transform[i] = Clamp(i +
        //                (step - (i % step) - (i % step)));
        //}

        //private int Clamp(int val)
        //{
        //    if (val < 0) return 0;
        //    if (val > 255) return 255;
        //    return val;
        //}

        //private void posterise1()
        //{
        //    double posterise_multiplier = 255.0 / (int)slider1.Value;

        //    for (int i = 0; i < 256; i++)
        //        foreach (var transform in new[] { _transformR, _transformG, _transformB })
        //            transform[i] = (int)(Convert.ToInt32(i / posterise_multiplier) * posterise_multiplier);
        //}

        //private void posterise2()
        //{
        //    int posterise_level = (int)slider1.Value;

        //    if (posterise_level <= 1)
        //        throw new Exception("Posterise level 1 or below not allowed");
        //    double multiplier = 255.0 / (posterise_level - 1.0);

        //    foreach (var transform in new[] { _transformR, _transformG, _transformB })
        //    {
        //        for (int i = 0; i < 256; i++)
        //        {
        //            int index = (int)Math.Floor(i * (posterise_level / 256.0));
        //            transform[i] = (int)Math.Round(index * multiplier);
        //        }
        //    }
        //}

        //private void posterise_3level_custom2()
        //{
        //    int threshold1 = (int)slider1.Value;
        //    int threshold2 = (int)slider2.Value;
        //    int threshold3 = (int)slider3.Value;

        //    foreach (var transform in new[] { _transformR, _transformG, _transformB })
        //    {
        //        for (int i = 0; i < 256; i++)
        //        {
        //            if (transform == _transformG)
        //                // 3 bit
        //                transform[i] = i >= threshold2 ? 255
        //                : i >= threshold3 ? 170
        //                : i >= threshold1 ? 85
        //                : 0;
        //            else
        //                // 2 bit
        //                transform[i] = i >= threshold2 ? 255
        //                    : i >= threshold1 ? 128
        //                    : 0;
        //        }
        //    }
        //}


        private ReturnValues posterise_2level_custom(int[] sliderValues)
        {
            int threshold = sliderValues[0];
            var rv = new ReturnValues();

            for (int i = 0; i < 256; i++)
            {
                rv.AssignValue(i, i >= threshold ? 255 : 0);
            }
            return rv;
        }

        private ReturnValues posterise_3level_custom(int[] sliderValues)
        {
            int threshold1 = sliderValues[0];
            int threshold2 = sliderValues[1];
            var rv = new ReturnValues();

            for (int i = 0; i < 256; i++)
            {
                rv.AssignValue(i, i >= threshold2 ? 255
                                : i >= threshold1 ? 128
                                : 0);
            }
            return rv;
        }



        private ReturnValues posterise_4level_custom(int[] sliderValues)
        {
            int threshold1 = sliderValues[0];
            int threshold2 = sliderValues[1];
            int threshold3 = sliderValues[2];
            var rv = new ReturnValues();

            for (int i = 0; i < 256; i++)
            {
                rv.AssignValue(i, i >= threshold3 ? 255
                                : i >= threshold2 ? 170
                                : i >= threshold1 ? 85
                                : 0);
            }
            return rv;
        }

        private ReturnValues posterise_5level_custom(int[] sliderValues)
        {
            int threshold1 = sliderValues[0];
            int threshold2 = sliderValues[1];
            int threshold3 = sliderValues[2];
            int threshold4 = sliderValues[3];
            var rv = new ReturnValues();

            for (int i = 0; i < 256; i++)
            {
                rv.AssignValue(i, i >= threshold4 ? 255
                                : i >= threshold3 ? 192
                                : i >= threshold2 ? 128
                                : i >= threshold1 ? 64
                                : 0);
            }
            return rv;
        }

        private ReturnValues posterise_5level(int[] notused)
        {
            return posterise_nlevels(5);
        }

        private ReturnValues posterise_6level(int[] notused)
        {
            return posterise_nlevels(6);
        }

        private ReturnValues posterise_7level(int[] notused)
        {
            return posterise_nlevels(7);
        }

        private ReturnValues posterise_8level(int[] notused)
        {
            return posterise_nlevels(8);
        }

        private ReturnValues posterise_nlevels(int nlevels)
        {
            // Number of levels ^ 3 = Number of different colours
            // e.g. 256 ^ 3 = 16,777,216 (24-bit, 8-bit per channel)
            // e.g. 2 ^ 3 = 8 colours (red, yellow, green, cyan, blue, magenta, black, white)

            int[] outvalues = new int[nlevels]; // e.g. [0,127,255]
            float divisor = nlevels - 1;
            for (int i = 0; i < nlevels; i++)
            {
                outvalues[i] = (int)((255 / divisor) * i);
            }

            int[] thresholds = new int[nlevels - 1]; // e.g. [85,170]
            for (int i = 0; i < (nlevels - 1); i++)
            {
                thresholds[i] = (int)((255 / (float)nlevels) * (i + 1));
            }

            int Lookup(int v) // local function
            {
                for (int t = 0; t < thresholds.Length; t++)
                {
                    if (v < thresholds[t])
                        return outvalues[t];
                }
                return outvalues[thresholds.Length];
            }

            var rv = new ReturnValues();
            for (int i = 0; i < 256; i++)
            {
                rv.AssignValue(i, Lookup(i));
            }
            return rv;
        }
    }
}
