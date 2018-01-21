# HueScript
Use C# as your scripting language to control your Philip Hue products!

HueScript is a command line program that runs on Windows and Linux, Windows 10 IOT and the Raspberry Pi. 

```
-ip address          - [optional] IP address of HUE, otherwise will try to
                        locate it automatically
-key <key>           - [mandatory] App Key needed to connect to the Hue
-compile             - Compile only for debugging script. Does not run script.
-register <appName> <appkey>
                        - Registry App Name & App Key with the Hue. Requires Name and Key.
                        A Key must be registered with the Hue before using the other options
                        Example: HueCmd -register HueCmd SomeKey1234
Examples:
        HueCmd -ip 192.1.1.1 -key SomeKey1234 script1.csx
        HueCmd -key SomeKey1234 -compile script2.csx            Compiles script to check for errors. Does not run script
```

Sample script file:

```
string set1 = "1,2";
bool state;
byte brightness;
HueGetLightState("8", out state, out brightness);
Console.WriteLine("Light 8 is " + state + "  brightness is " + brightness);

if (DateTime.Now.Hour == 18)
{
    HueTurnLightOn(set1);
}

ChangeLightColor(set1, "ff0000");
```

uses https://github.com/Q42/Q42.HueApi
