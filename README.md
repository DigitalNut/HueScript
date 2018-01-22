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
        HueScript -ip 192.1.1.1 -key SomeKey1234 script1.csx
        HueScript -key SomeKey1234 -compile script2.csx            Compiles script to check for errors. Does not run script
```

## C# API
Most basic C# api is available, plus additional command to control the Hue.

 - `bool HueIsLightOn(string lightID)` - returns true if light is on, otherwise false <br />
`if (HueIsLightOn("1")) { // then do something }`
 - `HueTurnLightOn(string lightID)` - Turn on light <br />
 `HueTurnLightOn("2");`
 - `bool HueGetLightState(string lightID, out bool state, out byte brightness)` - Returns the light state. Return value is true if function succeeded to get values<br />
 state = light is on (true) or off (false). <br />
 brightness = brightness value from 0 to 254 <br />
`HueGetLightState("2", out state, out brightness)`
 - `HueChangeLightState(string lights, bool? onOff, byte? brightness)` - Changes the light state <br />
 `HueChangeLightState("2,3", true, 127);`
 - `HueChangeLightColor(string lights, string color)` - Changes the light color of one or more lights <br />
 `HueChangeLightColor("2,3", "00ff00");` <br />
 `HueChangeLightColor("2,3", "red"); // only basic color are supported (e.g. 'red', 'blue', 'green', 'aqua', etc...)`


### Sample script file:

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
