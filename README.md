Pop Unity Code/Package
==========================

This repository intends to be a replacement for the aging & disorganised collection of code of various repositories including;
 - https://github.com/SoylentGraham/PopUnityCommon
 - https://github.com/NewChromantics/PopCodecs 

But more importantly, common code for PopH264, PopYuv, PopCameraDevice, and more plugins.

Trying to get this distributed as a package, but for now, could be included as a submodule.


Pop.PixelFormat
-------------------
This is an enum matching the names of various pixel formats that come out of Pop plugins.

PopSubArray
-------------------
This implements a `SubArray()` function to arrays, that allows you to easily copy a section of an array into a new one
`
using Pop;	//	required as code is in a namespace to avoid conflicts
byte[] MyBigArray;
var SmallArray = MyBigArray.SubArray( MyBigArray.length/2 );
`