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
    /// <summary>
    /// Script class that exposes HUE Api. Methods that start with Hue* are for use in the script
    /// All other functions are for internal use
    /// </summary>
    public class HueControl
    {
        static private LocalHueClient _client = null;

        static string IP;
        static string Key;

        /// <summary>
        /// State for a light
        /// </summary>
        public class LightState
        {
            /// <summary>
            /// The On/Off set of the light
            /// </summary>
            public bool On { get; set; }
            /// <summary>
            /// Brightness level. 0 to 254
            /// </summary>
            public byte Brightness { get; set; }
            /// <summary>
            /// The 'hue' color component of the light. Note this property may be null
            /// </summary>
            public int? Hue { get; set; }
            /// <summary>
            /// The 'saturation' color component of the light. Note this property me be null
            /// </summary>
            public int? Saturation { get; set; }
        }

        /// <summary>
        /// constructor. Note this set the IP/Key from the command line. 
        /// Use HueSetIPKey for script setting.
        /// </summary>
        /// <param name="ip">IP/hostname</param>
        /// <param name="key">Hue app key</param>
        public HueControl(string ip, string key)
        {
            IP = ip;
            Key = key;
        }


        #region External Script API
        // ---------------------------------------------------------------------------------------------------------
        // External Script API

        /// <summary>
        /// Set the IP/Key from the script. 
        /// </summary>
        /// <param name="ip">IP/hostname to set or null/blank to ignore</param>
        /// <param name="key">App Key to set or null/blank to ignore</param>
        public static void HueSetIPKey(string ip, string key)
        {
            if (!String.IsNullOrEmpty(ip))
                IP = ip;
            if (!String.IsNullOrEmpty(key))
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

        /// <summary>
        /// Turns the light on (sets brightness to max)
        /// </summary>
        /// <param name="lightID">Light ID. E.g. "1"</param>
        public static void HueTurnLightOn(string lightID)
        {
            HueChangeLightState(lightID, true, 255);
        }

        /// <summary>
        /// Gets the lights state (on/off) and brightness
        /// </summary>
        /// <param name="lightID">Light ID. E.g. "1"</param>
        /// <param name="state">True light is on. False if off</param>
        /// <param name="brightness">Brightness of the light</param>
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
        /// Returns the complete light state
        /// </summary>
        /// <param name="lightID">Light ID. E.g. "1"</param>
        /// <param name="state">Returns the LightState setting of the light</param>
        /// <returns></returns>
        public static bool HueGetLightState(string lightID, out LightState state)
        {
            LocalHueClient client = GetClient();

            state = new LightState();

            if (client == null)
                return false;

            var light = client.GetLightAsync(lightID);
            light.Wait();
            if (light == null)
                return false;

            if (light.Result == null)
                return false;

            //state = light.Result.State;
            state.On = light.Result.State.On;
            state.Brightness = light.Result.State.Brightness;
            state.Hue = light.Result.State.Hue;
            state.Saturation = light.Result.State.Saturation;

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
#if DEBUG
            Console.WriteLine($"Light string: {light}   Array size: {lights.Count()}  ON?: {onOff} Brightness?: {brightness} ");
#endif

            ChangeLightState(lights, onOff, brightness);
        }

        /// <summary>
        /// Change the light state
        /// </summary>
        /// <param name="light">Light ID or more then ID separated by commas. E.g. "1" or "1,2,4"</param>
        /// <param name="state">Fill in the LightState structure to set up the light</param>
        public static void HueChangeLightState(string light, LightState state)
        {
            string[] lights = light.Split(',');
#if DEBUG
            Console.WriteLine($"Light string: {light}   Array size: {lights.Count()}");
#endif

            ChangeLightState(lights, state);
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
            //Console.WriteLine($"HueScript light: {lights}   lightarray: Size: {lightArray.Count()}");

            ChangeLightColor(lightArray, color);
        }
        #endregion

        #region Internal Methods
        // ---------------------------------------------------------------------------------------------------------
        // Internal routines


        /// <summary>
        /// Internal function to change light state
        /// </summary>
        /// <param name="lights">Light ID array></param>
        /// <param name="onOff">True to turn on light. Can be null to ignore</param>
        /// <param name="brightness">Brightness value. Can be null to ignore</param>
        private static void ChangeLightState(string[] lights, bool? onOff, byte? brightness)
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
        /// Internal function to set light set
        /// </summary>
        /// <param name="lights">Light ID array</param>
        /// <param name="state">State to change</param>
        private static void ChangeLightState(string[] lights, LightState state)
        {
            LocalHueClient client = GetClient();

            if (client == null)
                return;

            var command = new LightCommand();
            if (state.On == true)
                command.On = true;
            else
                command.On = false;

            if (state.Brightness > 0)
                command.Brightness = state.Brightness;

            if (state.Hue != null)
                command.Hue = state.Hue;

            if (state.Saturation != null)
                command.Saturation = state.Saturation;

            //Console.WriteLine("Sending command");
            client.SendCommandAsync(command, lights).Wait();
        }

        /// <summary>
        /// Internal function to change light color
        /// </summary>
        /// <param name="lights">Light ID array</param>
        /// <param name="color">Color to change. Hex value in the format 'rrggbb' or color string name</param>
        private static void ChangeLightColor(string[] lights, string color)
        {
            LocalHueClient client = GetClient();
            
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

                //Console.WriteLine("Sending command: ChangeLightColor: " + lights);
                client.SendCommandAsync(command, lights).Wait();
            }

        }


        /// <summary>
        /// Helper function to create a HueCient
        /// </summary>
        /// <returns>Client handle or null if can't connect</returns>
        private static LocalHueClient GetClient()
        {
            // do we already have a connection?
            if (_client != null)
            {
                return _client;
            }

            string ip = GetOrFindIP().GetAwaiter().GetResult(); ;

            if (String.IsNullOrEmpty(ip))
                return null;

            IP = ip;

            _client = new LocalHueClient(ip);
            _client.Initialize(Key);

            if (!_client.IsInitialized)
            {
                _client = null;
                return null;
            }

            return _client;
        }

        /// <summary>
        /// Return the command line IP address that was entered by the user or IP found by the bridge locater service
        /// </summary>
        private static async Task<string> GetOrFindIP()
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
                    Console.WriteLine("Bridge found.  Using IP address: " + ip);
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
        #endregion
    }
}
