
# HueScript
Use C# as your scripting language to control your Philip Hue products!

HueScript is a command line program that runs on Windows and Linux, Windows 10 IOT and the Raspberry Pi. 
Prebuilt executable is in the EXE folder for desktop and ARM version (works on Windows 10 IOT Raspberry PI).

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

Most basic C# api is available, plus additional command to control the Hue.

```
Hue Api exposed:
* bool HueIsLightOn(string lightID) - returns if light is on
e.g. if (HueIsLightOn("1")) { // then do something }
* HueTurnLightOn(string lightID) - Turn on light. 
e.g. HueTurnLightOn("2");
* bool HueGetLightState(string lightID, out bool state, out byte brightness)
state - light is on (true) or off (false)
brightness - brightness value from 0 to 254
e.g. HueGetLightState("2", out state, out brightness)
* HueChangeLightState(string lights, bool? onOff, byte? brightness)
Change the state of one or more lights
e.g. HueChangeLightState("2,3", true, 127);
* HueChangeLightColor(string lights, string color)
Change color sate of a light
e.g. HueChangeLightColor("2,3", "00ff00");
or HueChangeLightColor("2,3", "red"); // only basic color are supported (e.g. 'red', 'blue', 'green', 'aqua', etc...)

```

Sample script file:

```
string set1 = "1,2";
int pauseTime = 2000;

bool state;
byte brightness;
HueGetLightState("8", out state, out brightness);
Console.WriteLine("Light 8 is " + state + "  brightness is " + brightness);

if (DateTime.Now.Hour == 18)
{
    HueTurnLightOn("1");
    // set light 4 to on and brightness to 127
    HueChangeLightState("4", true, 127);
}

HueChangeLightColor(set1, "ff0000");
Sleep(pauseTime)
HueChangeLightColor(set1, "00ff00");
Sleep(pauseTime)
HueChangeLightColor(set1, "0000ff");
```

uses https://github.com/Q42/Q42.HueApi
