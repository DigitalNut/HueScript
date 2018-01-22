using System;
using System.Collections.Generic;
using System.Text;

namespace HueScript
{
    partial class Program
    {
        static bool ParseCmdLine(string[] args)
        {
            int len = args.Length;

            if (len == 0)
            {
                PrintUsage();
                return false;
            }

            CmdLineOptions.compileOnly = false;

            try
            {
                int i = 0;
                while (i < len)
                {
                    switch (args[i])
                    {
                        case "-ip":
                            i++;
                            CmdLineOptions.ip = args[i];
                            break;
                        case "-key":
                            i++;
                            CmdLineOptions.key = args[i];
                            break;
                        case "-register":
                            i++;
                            CmdLineOptions.register = args[i];
                            i++;
                            CmdLineOptions.key = args[i];
                            break;
                        case "-file":
                            i++;
                            CmdLineOptions.scriptFile = args[i];
                            break;
                        case "-compile":
                            CmdLineOptions.compileOnly = true;
                            break;
                        default:
                            if (String.IsNullOrEmpty(CmdLineOptions.scriptFile))
                            {
                                CmdLineOptions.scriptFile = args[i];
                            }
                            else
                            {
                                PrintUsage();
                                return false;
                            }
                            break;
                    }
                    i++;
                }
            }
            catch
            {
                Console.WriteLine("Invalid or missing parameters. Please check.");
                Console.WriteLine("Enter command with no parameters to see help");
                return false;
            }

            if (String.IsNullOrEmpty(CmdLineOptions.scriptFile))
            {
                Console.WriteLine("Missing script filename.");
                return false;
            }

            if (CmdLineOptions.compileOnly == false && String.IsNullOrEmpty(CmdLineOptions.key))
            {
                Console.WriteLine("Missing key. Is you don't have a key, use the register option to assign one");
                return false;
            }

            if (CmdLineOptions.compileOnly == false && !String.IsNullOrEmpty(CmdLineOptions.register))
            {
                if (String.IsNullOrEmpty(CmdLineOptions.key))
                {
                    Console.WriteLine("Missing key. Register options requires both app name and key");
                    return false;
                }
            }

            if (!System.IO.File.Exists(CmdLineOptions.scriptFile))
            {
                Console.WriteLine("Script file not found: " + CmdLineOptions.scriptFile);
                return false;
            }

            return true;
        }


        /// <summary>
        /// Print usage
        /// </summary>
        static void PrintUsage()
        {
            Console.WriteLine("HueScript " + VersionString);
            Console.WriteLine(" by DigitalNut");
            Console.WriteLine(" Control the Philip Hue using C# as the scripting language\n");
            Console.WriteLine("-ip address          - [optional] IP address of HUE, otherwise will try to \n\t\t\tlocate it automatically");
            Console.WriteLine("-key <key>           - [mandatory] App Key needed to connect to the Hue");
            Console.WriteLine("-compile             - Compile only for debugging script. Does not run script.");
            Console.WriteLine("-register <appName> <appkey>  \n\t\t\t- Registry App Name & App Key with the Hue. Requires Name and Key. \n" +
                              "\t\t\tA Key must be registered with the Hue before using the other options \n" +
                              "\t\t\tExample: HueCmd -register HueCmd SomeKey1234");
            Console.WriteLine("Examples:");
            Console.WriteLine("\tHueScript -ip 192.1.1.1 -key SomeKey1234 script1.csx");
            Console.WriteLine("\tHueScript -key SomeKey1234 -compile script2.csx \t\tCompiles script to check for errors. Does not run script");
        }
    }
}
