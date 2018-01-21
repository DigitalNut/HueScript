using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Q42.HueApi;
using Q42.HueApi.ColorConverters.HSB;
using Q42.HueApi.Interfaces;

namespace HueControlHelpers
{
    public class HueControl
    {
        static string  IP;
        static string  Key;

        public HueControl(string ip, string key)
        {
            IP = ip;
            Key = key;
        }

        /// <summary>
        /// Returns if light is on or off
        /// </summary>
        /// <param name="lightID">Light ID. E.g. "1"</param>
        /// <returns>true if light is on, else false</returns>
        public static bool HueIsLightOn(string lightID)
        {
            LocalHueClient client = GetClient();

            if (client == null)
                return false;

            var light = client.GetLightAsync(lightID);
            light.Wait();
            if (light == null)
                return false;

            if (light.Result == null)
                return false;

            return light.Result.State.On;
        }

        public static void HueTurnLightOn(string lightID)
        {
            HueChangeLightState(lightID, true, 255);
        }

        /// <summary>
        /// Gets the lights state and brightness
        /// </summary>
        /// <param name="lightID">Light ID. E.g. "1"</param>
        /// <param name="state"></param>
        /// <param name="brightness"></param>
        /// <returns></returns>
        public static bool HueGetLightState(string lightID, out bool state, out byte brightness)
        {
            LocalHueClient client = GetClient();

            state = false;
            brightness = 0;

            if (client == null)
                return false;

            var light = client.GetLightAsync(lightID);
            light.Wait();
            if (light == null)
                return false;

            if (light.Result == null)
                return false;

            state = light.Result.State.On;
            brightness = light.Result.State.Brightness;

            return true;
        }

        /// <summary>
        /// Change the light state
        /// </summary>
        /// <param name="light">Light ID or more then ID separated by commas. E.g. "1" or "1,2,4"</param>
        /// <param name="onOff">true to turn on light, false to turn off. Pass in null if you do not wish to change state</param>
        /// <param name="brightness">Brightness value 1 to 254. Pass in null if you do not wish to change brightness</param>
        public static void HueChangeLightState(string light, bool? onOff, byte? brightness)
        {
            string[] lights = light.Split(',');

            ChangeLightState(lights, onOff, brightness);
        }

        public static void ChangeLightState(string[] lights, bool? onOff, byte? brightness)
        {
            LocalHueClient client = GetClient();

            if (client == null)
                return;

            var command = new LightCommand();
            if (onOff != null)
            {
                if (onOff == true)
                    command.On = true;
                else
                    command.On = false;
            }

            if (brightness != null && brightness > 0)
                command.Brightness = brightness;

            //Console.WriteLine("Sending command");
            client.SendCommandAsync(command, lights).Wait();
        }

        /// <summary>
        /// Changes the light color
        /// </summary>
        /// <param name="lights">Light ID or more then ID separated by commas. E.g. "1" or "1,2,4"</param>
        /// <param name="color">Color value in hex, or basic color name or "Once", "Multi", "ColorLoop", "None". Color hex value format is RRGGBB e.g. "00AABB". 
        /// Basic color names such as "Red", "Blue", "Green" (Black is not allowed). </param>
        public static void HueChangeLightColor(string lights, string color)
        {
            if (String.IsNullOrEmpty(lights))
                return;

            string[] lightArray = lights.Split(',');

            ChangeLightColor(lightArray, color);
        }

        public static void ChangeLightColor(string[] lights, string color)
        {
            LocalHueClient client = GetClient();
            Console.WriteLine("ChangeLightColor" + client);

            if (client == null)
                return;

            var command = new LightCommand();
            if (!String.IsNullOrEmpty(color))
            {
                command.TurnOn();

                if (String.Compare(color, "Once", true) == 0)
                    command.Alert = Alert.Once;
                else if (String.Compare(color, "Multi", true) == 0)
                    command.Alert = Alert.Multiple;
                else if (String.Compare(color, "ColorLoop", true) == 0)
                    command.Effect = Effect.ColorLoop;
                else if (String.Compare(color, "None", true) == 0)
                {
                    command.Effect = Effect.None;
                    command.Alert = Alert.None;
                }
                else if (color.Length == 6 && ((color[0] >= '0' && color[0] <= '9') ||
                                                             (color[0] >= 'a' && color[0] <= 'f') ||
                                                             (color[0] >= 'A' && color[0] <= 'F')))
                {
                    //if (color.Length != 6)
                    //{
                    //    Console.WriteLine("Color value must be 6 characters. E.g. 00ff00");
                    //    return;
                    //}
                    command.SetColor(new Q42.HueApi.ColorConverters.RGBColor(color));
                }
                else
                {
                    // treat the color cmd line arg as a color name (e.g. "red")
                    // FromName will return RGB(0,0,0) if the name is not valid
                    System.Drawing.Color cc = System.Drawing.Color.FromName(color);
                    if (cc.ToArgb() == 0)
                    {
                        Console.WriteLine("Incorrect color option. Check spelling. Black in not allowed.");
                        return;
                    }
                    command.SetColor(new Q42.HueApi.ColorConverters.RGBColor((int)cc.R, (int)cc.G, (int)cc.B));
                }

                Console.WriteLine("Sending command: ChangeLightColor: " + lights);
                client.SendCommandAsync(command, lights).Wait();
            }

        }


        /// <summary>
        /// Helper function to create a HueCient
        /// </summary>
        /// <returns></returns>
        static LocalHueClient GetClient()
        {
            LocalHueClient client = null;

            string ip = GetOrFindIP().GetAwaiter().GetResult(); ;

            if (String.IsNullOrEmpty(ip))
                return null;

            IP = ip;

            client = new LocalHueClient(ip);
            client.Initialize(Key);

            if (!client.IsInitialized)
                return null;

            return client;
        }

        /// <summary>
        /// Return the command line IP address that was entered by the user or IP found by the bridge locater service
        /// </summary>
        /// <param name="ip"></param>
        static async Task<string> GetOrFindIP()
        {
            string ip = IP;

            if (String.IsNullOrEmpty(IP))
            {
                IBridgeLocator locator = new HttpBridgeLocator();
                IEnumerable<Q42.HueApi.Models.Bridge.LocatedBridge> bridgeIPs = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(10));

                ////For Windows 8 and .NET45 projects you can use the SSDPBridgeLocator which actually scans your network. 
                ////See the included BridgeDiscoveryTests and the specific .NET and .WinRT projects
                //SSDPBridgeLocator locator = new SSDPBridgeLocator();
                //IEnumerable<string> bridgeIPs = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(10));

                if (bridgeIPs.Any())
                {
                    ip = bridgeIPs.First().IpAddress;
                    Console.WriteLine("Bridge found using IP address: " + ip);
                }
                else
                {
                    Console.WriteLine("Scan did not find a Hue Bridge. Try suppling a IP address for the bridge");
                    return null;
                }
            }

            return ip;
        }

        /// <summary>
        /// Register this app with the Hue
        /// </summary>
        /// <param name="appName">App Name. e.g. HueScript</param>
        /// <param name="key">Key can be anything. e.g. huekey1234. This key is supplied when using the Hue</param>
        public void Register(string appName, string key)
        {
            string ip = GetOrFindIP().GetAwaiter().GetResult(); ;

            if (String.IsNullOrEmpty(ip))
            {
                Console.WriteLine("Could not find the Hue. If using -IP option check for correct IP/hostname ");
                return;
            }

            try
            {
                ILocalHueClient client = new LocalHueClient(ip);
                var appKey = client.RegisterAsync(appName, key).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception.");
                string err = e.Message;
                if (e.InnerException != null)
                    err = e.InnerException.Message;
                Console.WriteLine("Additional detail: " + err);
            }
        }
    }
}
