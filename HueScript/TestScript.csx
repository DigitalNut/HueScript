// ff0000 = red
// 006400 = dark green
string color1 = "ff0000";
string color2 = "006400";
int pauseTime = 2000;

string set1 = "8,9";

Console.WriteLine("Check state");

Console.WriteLine("Light 8 is " + HueIsLightOn("8"));

HueTurnLightOn(set1);
Sleep(pauseTime);

bool state;
byte brightness;
HueGetLightState("8", out state, out brightness);
Console.WriteLine("Light 8 is " + state + "  brightness is " + brightness);

Sleep(pauseTime);
// change to red
HueChangeLightColor("8", color1);
Sleep(pauseTime);
// change to dark green
HueChangeLightColor("8", color2);

// if the light is on, change the brightness, wait 2 seconds and then turn them off
if (state)
{
    HueChangeLightState("8", null, 127);
    Sleep(pauseTime);
    HueChangeLightState("8", false, null);
}

// see if it's 6PM, turn on the lights
if (DateTime.Now.Hour == 18)
{
    HueTurnLightOn(set1);
}

