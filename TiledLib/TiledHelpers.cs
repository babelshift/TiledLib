using System.Drawing;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

namespace TiledLib
{
    /// <summary>
    /// Provides a set of helpful methods for dealing with TMX data.
    /// </summary>
    public static class TiledHelpers
    {
        /// <summary>
        /// Flag used on incoming tile IDs to identify tiles that are horizontally flipped.
        /// </summary>
        public const uint FlippedHorizontallyFlag = 0x80000000;

        /// <summary>
        /// Flag used on incoming tile IDs to identify tiles that are vertically flipped.
        /// </summary>
        public const uint FlippedVerticallyFlag = 0x40000000;

        /// <summary>
        /// Decodes a tile ID providing an index as well as a SpriteEffects for the tile.
        /// </summary>
        /// <param name="tileID">The ID of the tile to decode.</param>
        /// <param name="tileIndex">The index of the tile in a tile set.</param>
        /// <param name="spriteEffects">The SpriteEffects to be used whend drawing the tile.</param>
        public static void DecodeTileID(uint tileID, out int tileIndex, out SpriteEffects spriteEffects)
        {
            // compute the SpriteEffects
            spriteEffects = SpriteEffects.None;
            if ((tileID & FlippedHorizontallyFlag) != 0)
                spriteEffects |= SpriteEffects.FlipHorizontally;
            if ((tileID & FlippedVerticallyFlag) != 0)
                spriteEffects |= SpriteEffects.FlipVertically;

            // strip out the flip flags to get the real index
            tileIndex = (int)(tileID & ~(FlippedVerticallyFlag | FlippedHorizontallyFlag));
        }

        /// <summary>
        /// Iterates all of the tile sets and builds external references to the textures. Useful if you want to just
        /// load the resulting map and not have to also load up textures. The external reference is stored on the
        /// TileSet's Texture field so make sure you serialize that if you call this method.
        /// </summary>
        public static void BuildTileSetTextures(MapContent input, ContentProcessorContext context, string textureRoot = "")
        {
            foreach (var tileSet in input.TileSets)
            {
                // get the real path to the image
                string path = Path.Combine(textureRoot, tileSet.Image);

                // the asset name is the entire path, minus extension, after the content directory
                string asset = string.Empty;
                if (path.StartsWith(Directory.GetCurrentDirectory()))
                    asset = path.Remove(tileSet.Image.LastIndexOf('.')).Substring(Directory.GetCurrentDirectory().Length + 1);
                else
                    asset = Path.GetFileNameWithoutExtension(path);

                // build the asset as an external reference
                OpaqueDataDictionary data = new OpaqueDataDictionary();
                data.Add("GenerateMipmaps", false);
                data.Add("ResizeToPowerOfTwo", false);
                data.Add("TextureFormat", TextureProcessorOutputFormat.Color);
                data.Add("ColorKeyEnabled", tileSet.ColorKey.HasValue);
                data.Add("ColorKeyColor", tileSet.ColorKey.HasValue ? tileSet.ColorKey.Value : Microsoft.Xna.Framework.Color.Magenta);
                tileSet.Texture = context.BuildAsset<Texture2DContent, Texture2DContent>(
                    new ExternalReference<Texture2DContent>(path), null, data, null, asset);
            }
        }

        /// <summary>
        /// Iterates all tile sets and generates the correct source rectangles for all tiles inside the tilesets.
        /// Additionally reads in the property information for the tiles and stores them alongside the source rectangles
        /// in a Tile object stored in the TileSet's Tiles list.
        /// </summary>
        public static void GenerateTileSourceRectangles(MapContent input, string textureRoot = "")
        {
            // do some processing on tile sets to load external textures and figure out tile regions
            foreach (var tileSet in input.TileSets)
            {
                // get the real path to the image
                string path = Path.Combine(textureRoot, tileSet.Image);

                // load the image so we can compute the individual tile source rectangles
                int imageWidth = 0;
                int imageHeight = 0;
                using (Image image = Image.FromFile(path))
                {
                    imageWidth = image.Width;
                    imageHeight = image.Height;
                }

                // remove the margins from our calculations
                imageWidth -= tileSet.Margin * 2;
                imageHeight -= tileSet.Margin * 2;

                // figure out how many tiles fit on the X axis
                int tileCountX = 0;
                while (tileCountX * tileSet.TileWidth < imageWidth)
                {
                    tileCountX++;
                    imageWidth -= tileSet.Spacing;
                }

                // figure out how many tiles fit on the Y axis
                int tileCountY = 0;
                while (tileCountY * tileSet.TileHeight < imageHeight)
                {
                    tileCountY++;
                    imageHeight -= tileSet.Spacing;
                }

                // make our tiles. tiles are numbered by row, left to right.
                for (int y = 0; y < tileCountY; y++)
                {
                    for (int x = 0; x < tileCountX; x++)
                    {
                        Tile tile = new Tile();

                        // calculate the source rectangle
                        int rx = tileSet.Margin + x * (tileSet.TileWidth + tileSet.Spacing);
                        int ry = tileSet.Margin + y * (tileSet.TileHeight + tileSet.Spacing);
                        tile.Source = new Microsoft.Xna.Framework.Rectangle(rx, ry, tileSet.TileWidth, tileSet.TileHeight);

                        // get any properties from the tile set
                        int index = tileSet.FirstId + (y * tileCountX + x);
                        if (tileSet.TileProperties.ContainsKey(index))
                        {
                            tile.Properties = tileSet.TileProperties[index];
                        }

                        // save the tile
                        tileSet.Tiles.Add(tile);
                    }
                }
            }
        }
    }
}
