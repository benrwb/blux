using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace blux
{
    class MainMain
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        const int SW_SHOW = 5;

       

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        //[System.Diagnostics.DebuggerNonUserCodeAttribute()] // commented out to enable debugging
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main(string[] args)
        {
            Process currentProcess = Process.GetCurrentProcess();
            var runningProcess = (from process in Process.GetProcesses()
                                  where
                                    process.Id != currentProcess.Id &&
                                    process.ProcessName.Equals(currentProcess.ProcessName, StringComparison.Ordinal)
                                  select process).FirstOrDefault();


            //if (args.Length > 0 && args[0] == "/doit")
            //{
            //    if (runningProcess == null)
            //    {
            //        // only apply settings if b.lux is NOT already open 

            //        double red, green, blue;
            //        //MainMain.ColorTempToRGB(TempFromNow(), out red, out green, out blue);
            //        //SetGamma(red / 255, green / 255, blue / 255, false, 0);

            //        MainMain.FadeToRed_FromNow(out red, out green, out blue);
            //        SetGamma(red, green, blue);
            //    }
            //}
            //else
            //{
                // no command-line args

                if (runningProcess != null)
                {
                    // b.lux is already running, bring to front
                    // Note: Will not work if ShowInTaskbar="False"
                    ShowWindow(runningProcess.MainWindowHandle, SW_SHOW); // restore if minimised
                    SetForegroundWindow(runningProcess.MainWindowHandle); // activate and bring to front
                }
                else
                {
                    Method4_BuildLookups();
                    // b.lux is not already running, show the main window
                    blux.App app = new blux.App();
                    app.InitializeComponent();
                    app.Run();
                }
            //}
        }









        public static void CustomRamp(int[] transformR, int[] transformG, int[] transformB)
        {
            var ramp = new RAMP();
            ramp.Red = new ushort[256];
            ramp.Green = new ushort[256];
            ramp.Blue = new ushort[256];


            for (int i = 0; i <= 255; i++)
            {
                int value = i;
                ramp.Red[i] = (ushort)(transformR[i] << 8); // bitwise shift left
                ramp.Green[i] = (ushort)(transformG[i] << 8); // by 8 
                ramp.Blue[i] = (ushort)(transformB[i] << 8); // same as multiplying by 256
            }

            var screenDC = GetDC(IntPtr.Zero);
            var result = SetDeviceGammaRamp(screenDC, ref ramp);
            ReleaseDC(IntPtr.Zero, screenDC); // required otherwise will leak GDI objects

            if (result == false)
                // Can't go below 0.50 (3400K) unless flux is installed
                // and "Expand range" feature activated (flux.exe /unlockwingamma)
                throw new Exception("Failed to set gamma ramp");
        }







        // http://www.pinvoke.net/default.aspx/gdi32/setdevicegammaramp.html
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool SetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RAMP
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Red;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Green;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Blue;
        }

        private static Posterise.ReturnValues _posteriseLookup;

        public static void SetGamma(double red, double green, double blue, bool posterise)
        {
            if (red < 0.0 || red > 1.0 ||
                green < 0.0 || green > 1.0 ||
                blue < 0.0 || blue > 1.0)
                throw new Exception("Multiplier out of range");

            var ramp = new RAMP();
            ramp.Red = new ushort[256];
            ramp.Green = new ushort[256];
            ramp.Blue = new ushort[256];

            if (_posteriseLookup == null)
            {
                _posteriseLookup = new Posterise().ApplyMethod("posterise_8level_custom");
                // posterise_5level (125 colours)
                // posterise_5level_custom (125 colours)
                // posterise_4level_custom (64 colours)
            }

            for (int i = 0; i <= 255; i++)
            {
                int R_value = posterise ? _posteriseLookup.Red[i] : i;
                int G_value = posterise ? _posteriseLookup.Green[i] : i;
                int B_value = posterise ? _posteriseLookup.Blue[i] : i;

                ramp.Red[i] = (ushort)(Convert.ToByte(R_value * red) << 8); // bitwise shift left
                ramp.Green[i] = (ushort)(Convert.ToByte(G_value * green) << 8); // by 8 
                ramp.Blue[i] = (ushort)(Convert.ToByte(B_value * blue) << 8); // same as multiplying by 256
            }

            var screenDC = GetDC(IntPtr.Zero);
            var result = SetDeviceGammaRamp(screenDC, ref ramp);
            ReleaseDC(IntPtr.Zero, screenDC); // required otherwise will leak GDI objects

            if (result == false)
                // Can't go below 0.50 (3400K) unless flux is installed
                // and "Expand range" feature activated (flux.exe /unlockwingamma)
                throw new Exception("Failed to set gamma ramp");
				// Here's the reason why:
                //     "[SetDeviceGammaRamp], in contrast to the [other APIs] described later in this section,
                //     allows only a small deviation from an identity function. In fact, any entry
                //     in the ramp must be within 32768 of the identity value. This restriction means 
                //     that no app can turn the display completely black or to some other unreadable color."
                //  -- https://msdn.microsoft.com/en-us/library/windows/desktop/jj635732(v=vs.85).aspx#setting_gamma_control_with_dxgi
				// See also http://jonls.dk/2010/09/windows-gamma-adjustments/
				// (via https://stackoverflow.com/a/33268698/58241)
        }

        public static void Method3(double intensity, out double Red, out double Green, out double Blue)
        {
            // "intensity" ranges from 0.0 (nighttime) to 1.0 (daytime)

            // Green light not as bad as blue:
            // http://www.health.harvard.edu/staying-healthy/blue-light-has-a-dark-side

            // RED
            Red = 1.0;

            // GREEN
            // http://tutorial.math.lamar.edu/Classes/CalcI/LogFcns.aspx
            // We want to use the part of the common logarithm (Log10) chart 
            // between x = 0.2 and x = 1.
            // This will return a value between -0.7 and 0.
            // We add 1 to push it into the range 0.3 to 1.
            Green = Math.Log10(0.2 + (intensity * 0.8)) + 1;

            
            // BLUE
            // Blue scales linearly, but the range is 
            // "squashed" so that it hits zero when the slider
            // is down to about 15%.
            Blue = (intensity * 1.2) - 0.15;


            // Clamp values and convert from 0-1 to 0-255
            if (Green > 1) Green = 1; else if (Green < 0) Green = 0;
            if (Blue > 1) Blue = 1; else if (Blue < 0) Blue = 0;
            Red *= 255;
            Green *= 255;
            Blue *= 255;
        }




        class TempMapItem
        {
            public double In;
            public double Out; 
        }
        static List<TempMapItem> _GreenLookup, _BlueLookup;

        public static void Method4_BuildLookups()
        {
            _GreenLookup = new List<TempMapItem>();
            _GreenLookup.Add(new TempMapItem() { In = 0.0, Out = 0.27 });
            _GreenLookup.Add(new TempMapItem() { In = 0.1, Out = 0.44 });
            _GreenLookup.Add(new TempMapItem() { In = 0.2, Out = 0.56 });
            _GreenLookup.Add(new TempMapItem() { In = 0.3, Out = 0.65 });
            _GreenLookup.Add(new TempMapItem() { In = 0.4, Out = 0.72 });
            _GreenLookup.Add(new TempMapItem() { In = 0.5, Out = 0.78 });
            _GreenLookup.Add(new TempMapItem() { In = 0.6, Out = 0.84 });
            _GreenLookup.Add(new TempMapItem() { In = 0.9, Out = 0.98 });
            _GreenLookup.Add(new TempMapItem() { In = 1.0, Out = 1.00 });

            _BlueLookup = new List<TempMapItem>();
            _BlueLookup.Add(new TempMapItem() { In = 0.0, Out = 0.0 });
            _BlueLookup.Add(new TempMapItem() { In = 0.2, Out = 0.01 });
            _BlueLookup.Add(new TempMapItem() { In = 0.3, Out = 0.21 });
            _BlueLookup.Add(new TempMapItem() { In = 0.4, Out = 0.36 });
            _BlueLookup.Add(new TempMapItem() { In = 0.5, Out = 0.46 });
            _BlueLookup.Add(new TempMapItem() { In = 0.8, Out = 0.77 });
            _BlueLookup.Add(new TempMapItem() { In = 1.0, Out = 1.0 });
        }

        public static void Method4(double intensity, out double Red, out double Green, out double Blue)
        {
            // "intensity" ranges from 0.0 (nighttime) to 1.0 (daytime)
            Red = 1.0;
            Green = PerformLookup(_GreenLookup, intensity);
            Blue = PerformLookup(_BlueLookup, intensity);

            // Clamp values and convert from 0-1 to 0-255
            if (Green > 1) Green = 1; else if (Green < 0) Green = 0;
            if (Blue > 1) Blue = 1; else if (Blue < 0) Blue = 0;
            Red *= 255;
            Green *= 255;
            Blue *= 255;
        }

        private static double PerformLookup(List<TempMapItem> table, double intensity)
        {
            var exactMatch = table.SingleOrDefault(z => z.In == intensity);
            if (exactMatch != null)
                return exactMatch.Out;
            else
            {
                var upper = table.Where(z => z.In > intensity).OrderBy(z => z.In).First();
                var lower = table.Where(z => z.In < intensity).OrderByDescending(z => z.In).First();
                var posInRange = (intensity - lower.In) / (upper.In - lower.In);
                return (posInRange * (upper.Out - lower.Out)) + lower.Out;
            }
        }





        public static void Method5(double intensity, out double Red, out double Green, out double Blue)
        {
            // "intensity" ranges from 0.0 (nighttime) to 1.0 (daytime)


            // Calculated using polynomial trendlines (order=2) in Excel
            Red = 1.0;
            Green = (-0.6409 * Math.Pow(intensity, 2)) + (1.3624 * intensity) + 0.278; // changed from 0.2778 to 0.278 to ensure 1.0 --> 1.000
            Blue = (-0.6 * Math.Pow(intensity, 2)) + (1.9439 * intensity) - 0.3349;


            // Clamp values and convert from 0-1 to 0-255
            if (Green > 1) Green = 1; else if (Green < 0) Green = 0;
            if (Blue > 1) Blue = 1; else if (Blue < 0) Blue = 0;
            Red *= 255;
            Green *= 255;
            Blue *= 255;
        }


        public static void Method2(double temp, out double Red, out double Green, out double Blue)
        {
            // Green light not as bad as blue:
            // http://www.health.harvard.edu/staying-healthy/blue-light-has-a-dark-side

            if (temp == 6500)
            {
                Red = 255; Green = 255; Blue = 255;
                return;
            }

            var Temperature = temp / 100;

            // Calculate Red:
            Red = 255;

            // Calculate Green:
            Green = Temperature;
            Green = 99.5 * Math.Log(Green) - 161;
            if (Green < 0) Green = 0;
            if (Green > 255) Green = 255;

            // Calculate Blue:
            if (Temperature <= 19)
            {
                Blue = 0;
            }
            else
            {
                Blue = Temperature - 10;
                //Blue = 138.5177312231 * Math.Log(Blue) - 305.0447927307;
                Blue = 128 * Math.Log(Blue) - 305;
                if (Blue < 0) Blue = 0;
                if (Blue > 255) Blue = 255;
            }
        }

        public static void Method1(double temp, out double Red, out double Green, out double Blue)
        {
            // http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/ 
            // Start with a temperature, in Kelvin, somewhere between 1000 and 40000.  (Other values may work,
            // but I can't make any promises about the quality of the algorithm's estimates above 40000 K.)
            // Note also that the temperature and color variables need to be declared as floating-point.

            if (temp == 6500)
            {
                Red = 255; Green = 255; Blue = 255;
                return;
            }

            var Temperature = temp / 100;

            // Calculate Red:
            if (Temperature <= 66)
            {
                Red = 255;
            }
            else
            {
                Red = Temperature - 60;
                Red = 329.698727446 * Math.Pow(Red, -0.1332047592);
                if (Red < 0) Red = 0;
                if (Red > 255) Red = 255;
            }

            // Calculate Green:
            if (Temperature <= 66)
            {
                Green = Temperature;
                Green = 99.4708025861 * Math.Log(Green) - 161.1195681661;
                if (Green < 0) Green = 0;
                if (Green > 255) Green = 255;
            }
            else
            {
                Green = Temperature - 60;
                Green = 288.1221695283 * Math.Pow(Green, -0.0755148492);
                if (Green < 0) Green = 0;
                if (Green > 255) Green = 255;
            }

            // Calculate Blue:
            if (Temperature >= 66)
            {
                Blue = 255;
            }
            else
            {
                if (Temperature <= 19)
                {
                    Blue = 0;
                }
                else
                {
                    Blue = Temperature - 10;
                    Blue = 138.5177312231 * Math.Log(Blue) - 305.0447927307;
                    if (Blue < 0) Blue = 0;
                    if (Blue > 255) Blue = 255;
                }
            }
        }


        //static TimeSpan START_TIME = new TimeSpan(21, 0, 0);
        //static double DURATION = 3600; // duration in seconds. e.g. 3600 = 1 hour
        //static int END_COLOUR_TEMP = 1900;
        //static TimeSpan RESET_TIME = new TimeSpan(6, 0, 0); // when to reset back to 6500K

        //public static int TempFromNow()
        //{
        //    var now = DateTime.Now;

        //    // Deactivate on weekends
        //    if (now.DayOfWeek == DayOfWeek.Friday && now.TimeOfDay >= new TimeSpan(17, 0, 0)) return 6500;
        //    if (now.DayOfWeek == DayOfWeek.Saturday) return 6500;
        //    if (now.DayOfWeek == DayOfWeek.Sunday && now.TimeOfDay <= new TimeSpan(16, 0, 0)) return 6500;

        //    // Out of hours
        //    if (now.TimeOfDay < RESET_TIME) // e.g. Midnight - 6:00 am 
        //        return END_COLOUR_TEMP;
        //    if (now.TimeOfDay < START_TIME) // e.g 6:00 am - 10:00 pm
        //        return 6500;

        //    // After START_TIME...
        //    int seconds_elapsed = (int)(now.TimeOfDay - START_TIME).TotalSeconds;
        //    if (seconds_elapsed > DURATION) seconds_elapsed = (int)DURATION; // don't allow seconds_elapsed to exceed duration
        //    // I need to multiply the seconds by MULTIPLIER (colour temp change/number of seconds)
        //    return 6500 - (int)(seconds_elapsed * ((6500 - END_COLOUR_TEMP) / DURATION));
        //}




        //public static void FadeToRed_FromNow(out double Red, out double Green, out double Blue)
        //{
        //    Red = 1.0; Green = 1.0; Blue = 1.0;
        //    var now = DateTime.Now;
           
        //    // Different behaviour on weekends
        //    bool weekend = false;
        //    if (now.DayOfWeek == DayOfWeek.Friday && now.TimeOfDay >= new TimeSpan(17, 0, 0)) weekend = true;
        //    if (now.DayOfWeek == DayOfWeek.Saturday) weekend = true;
        //    if (now.DayOfWeek == DayOfWeek.Sunday && now.TimeOfDay <= new TimeSpan(16, 0, 0)) weekend = true;
        //    double END_BLUE_LEVEL = weekend
        //       ? 0.7  // fade to 1.00  0.80  0.70 on weekends
        //       : 0.4; // fade to 1.00  0.50  0.40 in the week


        //    // Out of hours
        //    double seconds_elapsed;
        //    if (now.TimeOfDay < RESET_TIME) // e.g. Midnight - 6:00 am 
        //        seconds_elapsed = DURATION;
        //    else if (now.TimeOfDay < START_TIME) // e.g 6:00 am - 10:00 pm
        //        return;
        //    else
        //    {
        //        // After START_TIME...
        //        seconds_elapsed = (now.TimeOfDay - START_TIME).TotalSeconds;
        //        if (seconds_elapsed > DURATION) seconds_elapsed = DURATION; // don't allow seconds_elapsed to exceed duration
        //    }

        //    // I need to multiply the seconds by MULTIPLIER (colour temp change/number of seconds)
        //    Blue = 1 - ((seconds_elapsed / DURATION) * (1.0 - END_BLUE_LEVEL));
        //    Green = Math.Min(Blue + 0.1, 1.0); 
        //}




        public class TimeValue
        {
            public double Offset { get; set; }
            public int Temp { get; set; }
        }

        public static Dictionary<int,int> BuildTimeOfDayLookup(string tsv)
        {
            var data = tsv
             .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
             .Select(z => z.Split('\t'))
             .Select(z => new TimeValue()
             {
                 Offset = TimeSpan.Parse(z[0]).TotalSeconds,
                 Temp = Convert.ToInt32(z[1])
             }).ToList();


            data.Add(new TimeValue() // ensure it wraps around to midnight correctly
            {
                Offset = TimeSpan.Parse("23:59:59").TotalSeconds + 1, // +1 to include 23:59:59 itself
                Temp = data[0].Temp
            });


            var lookup = new Dictionary<int, int>();
            for (int i = 0; i < data.Count - 1; i++)
            {
                double temp = data[i].Temp; // starting temp for this time period
                double seconds = data[i + 1].Offset - data[i].Offset; // total number of seconds
                double increment = (data[i + 1].Temp - data[i].Temp) / seconds; // how much to increment temp each second

                for (var t = data[i].Offset; t < data[i + 1].Offset; t++)
                {
                    // for each second between the two times,
                    // calculate the temp at that point

                    temp += increment;

                    lookup.Add((int)t, (int)temp);
                }

            }

            //var sb = new StringBuilder();
            //foreach (var item in lookup)
            //{
            //    sb.AppendFormat("{0}\t{1}\n", TimeSpan.FromSeconds(item.Key).ToString(), item.Value);
            //}
            //tb1.Text = sb.ToString();


            //this.Title = "Current: " + lookup[(int)DateTime.Now.TimeOfDay.TotalSeconds];

            return lookup;
        }


    }
}
