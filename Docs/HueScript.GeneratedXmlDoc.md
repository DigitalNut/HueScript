# HueScript #

#### Method Program.PrintUsage

 Print usage 



---
## Type HueControlHelpers.HueControl

 Script class that exposes HUE Api. Methods that start with Hue* are for use in the script All other functions are for internal use 



---
## Type HueControlHelpers.HueControl.LightState

 State for a light 



---
#### Property HueControlHelpers.HueControl.LightState.On

 The On/Off set of the light 



---
#### Property HueControlHelpers.HueControl.LightState.Brightness

 Brightness level. 0 to 254 



---
#### Property HueControlHelpers.HueControl.LightState.Hue

 The 'hue' color component of the light. Note this property may be null 



---
#### Property HueControlHelpers.HueControl.LightState.Saturation

 The 'saturation' color component of the light. Note this property me be null 



---
#### Method HueControlHelpers.HueControl.#ctor(System.String,System.String)

 constructor. Note this set the IP/Key from the command line. Use HueSetIPKey for script setting. 

|Name | Description |
|-----|------|
|ip: |IP/hostname|
|key: |Hue app key|


---
#### Method HueControlHelpers.HueControl.HueSetIPKey(System.String,System.String)

 Set the IP/Key from the script. 

|Name | Description |
|-----|------|
|ip: |IP/hostname to set or null/blank to ignore|
|key: |App Key to set or null/blank to ignore|


---
#### Method HueControlHelpers.HueControl.HueIsLightOn(System.String)

 Returns if light is on or off 

|Name | Description |
|-----|------|
|lightID: |Light ID. E.g. "1"|
**Returns**: true if light is on, else false



---
#### Method HueControlHelpers.HueControl.HueTurnLightOn(System.String)

 Turns the light on (sets brightness to max) 

|Name | Description |
|-----|------|
|lightID: |Light ID. E.g. "1"|


---
#### Method HueControlHelpers.HueControl.HueGetLightState(System.String,System.Boolean@,System.Byte@)

 Gets the lights state (on/off) and brightness 

|Name | Description |
|-----|------|
|lightID: |Light ID. E.g. "1"|
|state: |True light is on. False if off|
|brightness: |Brightness of the light|
**Returns**: 



---
#### Method HueControlHelpers.HueControl.HueGetLightState(System.String,HueControlHelpers.HueControl.LightState@)

 Returns the complete light state 

|Name | Description |
|-----|------|
|lightID: |Light ID. E.g. "1"|
|state: |Returns the LightState setting of the light|
**Returns**: 



---
#### Method HueControlHelpers.HueControl.HueChangeLightState(System.String,System.Nullable{System.Boolean},System.Nullable{System.Byte})

 Change the light state 

|Name | Description |
|-----|------|
|light: |Light ID or more then ID separated by commas. E.g. "1" or "1,2,4"|
|onOff: |true to turn on light, false to turn off. Pass in null if you do not wish to change state|
|brightness: |Brightness value 1 to 254. Pass in null if you do not wish to change brightness|


---
#### Method HueControlHelpers.HueControl.HueChangeLightState(System.String,HueControlHelpers.HueControl.LightState)

 Change the light state 

|Name | Description |
|-----|------|
|light: |Light ID or more then ID separated by commas. E.g. "1" or "1,2,4"|
|state: |Fill in the LightState structure to set up the light|


---
#### Method HueControlHelpers.HueControl.HueChangeLightColor(System.String,System.String)

 Changes the light color 

|Name | Description |
|-----|------|
|lights: |Light ID or more then ID separated by commas. E.g. "1" or "1,2,4"|
|color: |Color value in hex, or basic color name or "Once", "Multi", "ColorLoop", "None". Color hex value format is RRGGBB e.g. "00AABB". Basic color names such as "Red", "Blue", "Green" (Black is not allowed). |


---
#### Method HueControlHelpers.HueControl.ChangeLightState(System.String[],System.Nullable{System.Boolean},System.Nullable{System.Byte})

 Internal function to change light state 

|Name | Description |
|-----|------|
|lights: |Light ID array>|
|onOff: |True to turn on light. Can be null to ignore|
|brightness: |Brightness value. Can be null to ignore|


---
#### Method HueControlHelpers.HueControl.ChangeLightState(System.String[],HueControlHelpers.HueControl.LightState)

 Internal function to set light set 

|Name | Description |
|-----|------|
|lights: |Light ID array|
|state: |State to change|


---
#### Method HueControlHelpers.HueControl.ChangeLightColor(System.String[],System.String)

 Internal function to change light color 

|Name | Description |
|-----|------|
|lights: |Light ID array|
|color: |Color to change. Hex value in the format 'rrggbb' or color string name|


---
#### Method HueControlHelpers.HueControl.GetClient

 Helper function to create a HueCient 

**Returns**: Client handle or null if can't connect



---
#### Method HueControlHelpers.HueControl.GetOrFindIP

 Return the command line IP address that was entered by the user or IP found by the bridge locater service 



---
#### Method HueControlHelpers.HueControl.Register(System.String,System.String)

 Register this app with the Hue 

|Name | Description |
|-----|------|
|appName: |App Name. e.g. HueScript|
|key: |Key can be anything. e.g. huekey1234. This key is supplied when using the Hue|


---


