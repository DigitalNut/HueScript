using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using HueControlHelpers;

namespace HueScript
{
    partial class Program
    {
        static readonly string VersionString = "1.1";

        struct StructCmdLineOptions
        {
            public string ip;               // Hostname or ip address of HUE
            public string key;              // Key required by the Hue hub
            public string register;         // If you want to register a key with the Hue

            public string scriptFile;       // C# Script file (.csx)
            public bool   compileOnly;      // Compile script to check for errors -- does not run the script
        }

        static StructCmdLineOptions CmdLineOptions;

        public static void Main(string[] args)
        {

            if (!ParseCmdLine(args))
            {
                Environment.Exit(1);
                return;
            }

            var hs = new HueControl(CmdLineOptions.ip, CmdLineOptions.key);

            // Does user want to register this program?
            if (!String.IsNullOrEmpty(CmdLineOptions.register))
            {
                hs.Register(CmdLineOptions.register, CmdLineOptions.key);
                Environment.Exit(0);
                return;
            }

            Console.WriteLine("HueScript starting. Compiling script...");

            string scriptFile = System.IO.File.ReadAllText(CmdLineOptions.scriptFile);

            var options = ScriptOptions.Default.WithReferences(typeof(HueScript.Program).Assembly).WithImports("System", 
                                                                                                               "System.Collections.Generic", 
                                                                                                               "System.Threading.Thread");

            var script = CSharpScript.Create(scriptFile, options, hs.GetType());
            var diag = script.Compile();
            if (diag.Length > 0 )
            {
                Console.WriteLine("Compile error in script (Line, Column: Message)");
                foreach (Diagnostic d in diag)
                {
                    // StartLinePosition.Line starts line at 0
                    Console.WriteLine((d.Location.GetMappedLineSpan().StartLinePosition.Line + 1) + ", " + (d.Location.GetMappedLineSpan().StartLinePosition.Character + 1) + ": " + d.GetMessage());
                }
                Environment.Exit(2);
                return;
            }

            if (CmdLineOptions.compileOnly)
            {
                if (diag.Length == 0)
                {
                    Console.WriteLine("No errors in script.");
                }
                Environment.Exit(3);
                return;
            }


            Console.WriteLine("Executing");
            script.RunAsync(hs).Wait();
            //var result = CSharpScript.RunAsync(scriptFile, options, hs).GetAwaiter().GetResult();
            //var result = CSharpScript.RunAsync("ChangeLightState(\"8\", false, 0); ChangeLightState(\"8\", true, 254); ChangeLightState(\"8\", false, 0);", options, hs).GetAwaiter().GetResult();

            Console.WriteLine("HueScript finished");

        }
    }
}
