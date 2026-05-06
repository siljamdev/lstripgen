# LineStrip Generator
<img src="res/icon.png" width="200"/>

## Usage
This application lets you generate linestrips iteratively with a set of rules, and then view it as an image or text.  
It could also be described as a 90 degree l-system.  
Output files will be generated in the active directory.  

## CLI
Pass it the `-h` flag to get CLI help.  

Flags:  
|Flag|Description|
|---|---|
|-r <rules>|**Mandatory** flag. Sets the rules. (Info on the format with `-h`)|
|-i <number>|**Mandatory** flag. Sets the number of iterations|
|-s <strip>|Sets starting sequence. Same format as rules. Default is `0U`|
|-o <path>|Sets ouput path. `%` will be replaced by iteration count. Default is `%`|
|-t|Enables text mode. Output files will be .txt instead of .png|
|-c|Enables stdout mode. Output will be displayed as text on standard output|
|-p|Enables open mode. Output files will be opened when generated|
|-l|Only the last iteration will generate an output file|

## Installation
This application is available for Windows, Linux and MacOS.  
Download an executable from the [Releases](https://github.com/siljamdev/lstripgen/releases/latest).  

## Example rules
[Hilbert Curve](https://en.wikipedia.org/wiki/Hilbert_curve): Famous space filling curve `2X:;0U/0D:0C2X2W1X0X2C2E1C0X2C2E1E0W2E2C;0L/0R:0W2X2C1X0X2W2E1W0X2W2E1E0C2E2W;`  
[Dragon fractal](https://en.wikipedia.org/wiki/Dragon_curve): Famous self-similar fractal `1X:1X2C0X;2X:0X1C2X;` Starting sequence: `0R1D`  
J2: Cool fractal shape `0X:0X0C0X;`  
HalfFern: Space filling curve `0X:2E0W2X1X2X0C2E;1X:3W1X3C;2X:5X;3X:4X;4X:3X3X;5X:3X5X;` Starting sequence: `2U0R2D`  
Fern: Cool fractal shape `0X:3W0X3C;3X:2X0X2X;1X:2X1X;2X:1X;` Starting sequence: `0R`  
Flower: Cool flower-like shape `2X:2X2X2X;0X:1E0W1X2W1E0W1X0X1X0C1E2C1X0C1E;` Starting sequence: `0U0R0D0L`  
City: Random city-like grid `0X:0X0A;`  

## License
This software is licensed under the [MIT License](./LICENSE).