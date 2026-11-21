using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wktime
{
    class Program
    {
        private static readonly string AppName = "wktime";

        static int Main(string[] args)
        {
            Console.WriteLine(AppName);
            Console.WriteLine();

            if (args.Length != 1)
            {
                Console.WriteLine("USAGE:");
                Console.WriteLine("\tpass your raw clocking in/out data as the sole argument.");
                Console.WriteLine();
                Console.WriteLine("Examples:");
                Console.WriteLine($"\t{AppName} 8,1306");
                Console.WriteLine($"\t{AppName} 655,12,1232,1644");
                Console.WriteLine($"\t{AppName} 8,1244,1330,1703");
                Console.WriteLine($"\t{AppName} 7,902,906,1202,1233,1505,1511,1658");

                return 0;
            }

            WorkTimeData data;

            try
            {
                data = new WorkTimeData(DateTime.Now.Date, args[0]);

                Console.WriteLine($"Raw time data: {args[0]}");
                Console.WriteLine();
                Console.WriteLine($"Normalized time data: {data.NormalizedTimeData}");
                Console.WriteLine();
                Console.WriteLine(data.HumanReadableStats);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                return 1;
            }

            return 0;
        }
    }
}
