using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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


            if (args.Length > 0 && args[0] == "/doit")
            {
                if (runningProcess == null)
                {
                    // only apply settings if b.lux is NOT already open 

                    double red, green, blue;
                    //MainMain.ColorTempToRGB(TempFromNow(), out red, out green, out blue);
                    //SetGamma(red / 255, green / 255, blue / 255, false, 0);

                    MainMain.FadeToRed_FromNow(out red, out green, out blue);
                    SetGamma(red, green, blue, false, 0);
                }
            }
            else
            {
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
                    // b.lux is not already running, show the main window
                    blux.App app = new blux.App();
                    app.InitializeComponent();
                    app.Run();
                }
            }
        }





        











        // http://www.pinvoke.net/default.aspx/gdi32/setdevicegammaramp.html
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

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

        public static void SetGamma(double red, double green, double blue, bool posterise, int posterise_levels)
        {
            if (red < 0.0 || red > 1.0 ||
                green < 0.0 || green > 1.0 ||
                blue < 0.0 || blue > 1.0)
                throw new Exception("Multiplier out of range");

            var ramp = new RAMP();
            ramp.Red = new ushort[256];
            ramp.Green = new ushort[256];
            ramp.Blue = new ushort[256];

            double posterise_multiplier = posterise
                ? 255 / posterise_levels
                : 1;

            for (int i = 0; i <= 255; i++)
            {
                int value = i;

                if (posterise)
                {
                    value = (int)(Convert.ToInt32(value / posterise_multiplier) * posterise_multiplier);
                }

                ramp.Red[i] = (ushort)(Convert.ToByte(value * red) << 8); // bitwise shift left
                ramp.Green[i] = (ushort)(Convert.ToByte(value * green) << 8); // by 8 
                ramp.Blue[i] = (ushort)(Convert.ToByte(value * blue) << 8); // same as multiplying by 256
            }

            if (false == SetDeviceGammaRamp(GetDC(IntPtr.Zero), ref ramp))
                // Can't go below 0.50 (3400K) unless flux is installed
                // and "Expand range" feature activated (flux.exe /unlockwingamma)
                throw new Exception("Failed to set gamma ramp");
        }


        public static void ColorTempToRGB(double temp, out double Red, out double Green, out double Blue)
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


        static TimeSpan START_TIME = new TimeSpan(21, 0, 0);
        static double DURATION = 3600; // duration in seconds. e.g. 3600 = 1 hour
        static int END_COLOUR_TEMP = 1900;
        static TimeSpan RESET_TIME = new TimeSpan(6, 0, 0); // when to reset back to 6500K

        public static int TempFromNow()
        {
            var now = DateTime.Now;

            // Deactivate on weekends
            if (now.DayOfWeek == DayOfWeek.Friday && now.TimeOfDay >= new TimeSpan(17, 0, 0)) return 6500;
            if (now.DayOfWeek == DayOfWeek.Saturday) return 6500;
            if (now.DayOfWeek == DayOfWeek.Sunday && now.TimeOfDay <= new TimeSpan(16, 0, 0)) return 6500;

            // Out of hours
            if (now.TimeOfDay < RESET_TIME) // e.g. Midnight - 6:00 am 
                return END_COLOUR_TEMP;
            if (now.TimeOfDay < START_TIME) // e.g 6:00 am - 10:00 pm
                return 6500;

            // After START_TIME...
            int seconds_elapsed = (int)(now.TimeOfDay - START_TIME).TotalSeconds;
            if (seconds_elapsed > DURATION) seconds_elapsed = (int)DURATION; // don't allow seconds_elapsed to exceed duration
            // I need to multiply the seconds by MULTIPLIER (colour temp change/number of seconds)
            return 6500 - (int)(seconds_elapsed * ((6500 - END_COLOUR_TEMP) / DURATION));
        }




        public static void FadeToRed_FromNow(out double Red, out double Green, out double Blue)
        {
            Red = 1.0; Green = 1.0; Blue = 1.0;
            var now = DateTime.Now;
           
            // Different behaviour on weekends
            bool weekend = false;
            if (now.DayOfWeek == DayOfWeek.Friday && now.TimeOfDay >= new TimeSpan(17, 0, 0)) weekend = true;
            if (now.DayOfWeek == DayOfWeek.Saturday) weekend = true;
            if (now.DayOfWeek == DayOfWeek.Sunday && now.TimeOfDay <= new TimeSpan(16, 0, 0)) weekend = true;
            double END_BLUE_LEVEL = weekend
               ? 0.7  // fade to 1.00  0.80  0.70 on weekends
               : 0.4; // fade to 1.00  0.50  0.40 in the week


            // Out of hours
            double seconds_elapsed;
            if (now.TimeOfDay < RESET_TIME) // e.g. Midnight - 6:00 am 
                seconds_elapsed = DURATION;
            else if (now.TimeOfDay < START_TIME) // e.g 6:00 am - 10:00 pm
                return;
            else
            {
                // After START_TIME...
                seconds_elapsed = (now.TimeOfDay - START_TIME).TotalSeconds;
                if (seconds_elapsed > DURATION) seconds_elapsed = DURATION; // don't allow seconds_elapsed to exceed duration
            }

            // I need to multiply the seconds by MULTIPLIER (colour temp change/number of seconds)
            Blue = 1 - ((seconds_elapsed / DURATION) * (1.0 - END_BLUE_LEVEL));
            Green = Math.Min(Blue + 0.1, 1.0); 
        }


    }
}
