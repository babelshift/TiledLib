# TiledLib

## About

TiledLib is a C# library for utilizing the Tiled map editor for use in a game made with XNA Game Studio 4.0. With the latest version (from 12/2/2011), the library is now targeted entirely to the parsing of the Tiled .TMX files as a Content Importer. This means that your game needs to create an appropriate processor to take the data provided and turn it into the correct types for your specific game.

The library no longer includes an official content processor or runtime system. There are a couple of reasons for this:

    Most games need to customize the rendering of the layers. Rather than trying to work around the default code, the library now assumes you're going to write this code.
    Most games need to do additional processing or logic when loading in the map files. Rather than doing this at runtime after loading a generic object model, the library now assumes you're going to write your own content processor to convert the TiledLib's MapContent object into your game's object model.

This change does bring more work for developers, however there are sample implementations in the Demos folder that show how to easily write these pieces. You can also use them as starting points for your own implementations. Additionally there is a TiledHelpers class in the pipeline for handling common operations such as decoding the tile IDs into their indices and SpriteEffects, building the tile set textures into external references, and computing the source rectangles for the tiles.

This does mean a big breaking change from previous versions, however it does clean up the system a little bit as well as bringing support for tile objects, polygon objects, and polyline objects (the latter two coming soon with Tiled 0.8). It also means that when you load the content into your game its already in a state more suited to your game without extra data you don't need and without having to do additional parsing of strings or logic that you could offload to build time. Lastly it means it's quicker for me to add additional support to the importing process for new features of Tiled without having to also figure out the "right" way to handle them at runtime.

And if you don't care to go down this route, feel free to go back to revision 5697c1d6b272 which was the last commit prior to this redesign. It will mean you won't have the latest importers and bug fixes, but it will give you the old end-to-end solution.

## License

Copyright (C) 2011 by Nick Gravelyn

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.