# FFPR-ColoredWindows
A mod for the FINAL FANTASY PIXEL REMASTERS that implements the ability to change the color of the text windows from blue to any color of your choosing, including editing the borders.


# Installation:
1. Install a Bleeding-Edge IL2CPP build of BepInEx 6, which can be found [here](https://builds.bepis.io/projects/bepinex_be) or used from a prior Memoria installation
2. Drop the BepInEx folder from the mod into the game's main directory and merge folders if prompted
3. Optional: in order to change color while the game is running, [sinai-dev's BepInExConfigManager](https://github.com/sinai-dev/BepInExConfigManager) will need to be installed


# Using custom windows:
In order to use modded windows with this, you will need to replace the images in BepInEx/plugins/ColoredWindows with their equivalents from the mod of your choosing.

# SpriteData:
SpriteData attributes can be accessed by creating a .txt file, filling in the wanted parameters, and then renaming the file extension to .spriteData.

SpriteData exposes the following parameters:
* Rect = the rectangle that "crops" the base Texture2D into a Sprite - format is [0,0,0,0]
* Pivot = the pivot point (in pixels) of the Sprite - format is [0,0]
* Border = the border to be used in [9-slice scaling](https://docs.unity3d.com/Manual/9SliceSprites.html) (used by WindowFrame01 by default) - format is [0,0,0,0]
* Type = Image parameter that determines how the Sprite should be displayed - options are Simple, Sliced, Tiled, Filled
* WrapMode = Texture2D parameter that defines wrapping on the texture - options are Clamp, Repeat, Mirror, MirrorOnce

# Paletted Images
Images can be told to use palettes by setting RecolorMode to UsePalette. Images that are created for paletted mode should consist of 8 colors (not including transparency): red, green, blue, cyan, magenta, yellow, white, and black. The color that replace each can be defined in the config section for that texture.

Colors can be in either 0-1 format separated by comments, or in #RRGGBBAA hex color format.

# Credits:
* Faospark - for testing and providing me with the edited gradients to bundle with the dll
* The BepInEx Official Discord Server - for answering all of my dumb Unity questions
