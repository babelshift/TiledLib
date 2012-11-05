using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TiledLib
{
    public enum Orientation : byte
    {
        Orthogonal,
        Isometric,
    }

    public class MapContent
    {
        public string Filename;
        public string Directory;

        public string Version = string.Empty;
        public Orientation Orientation;
        public int Width;
        public int Height;
        public int TileWidth;
        public int TileHeight;
        public PropertyCollection Properties = new PropertyCollection();

        public List<TileSetContent> TileSets = new List<TileSetContent>();
        public List<LayerContent> Layers = new List<LayerContent>();

        public MapContent(XmlDocument document, ContentImporterContext context)
        {
            XmlNode mapNode = document["map"];

            Version = mapNode.Attributes["version"].Value;
            Orientation = (Orientation)Enum.Parse(typeof(Orientation), mapNode.Attributes["orientation"].Value, true);
            Width = int.Parse(mapNode.Attributes["width"].Value, CultureInfo.InvariantCulture);
            Height = int.Parse(mapNode.Attributes["height"].Value, CultureInfo.InvariantCulture);
            TileWidth = int.Parse(mapNode.Attributes["tilewidth"].Value, CultureInfo.InvariantCulture);
            TileHeight = int.Parse(mapNode.Attributes["tileheight"].Value, CultureInfo.InvariantCulture);

            XmlNode propertiesNode = document.SelectSingleNode("map/properties");
            if (propertiesNode != null)
            {
                Properties = new PropertyCollection(propertiesNode, context);
            }

            foreach (XmlNode tileSet in document.SelectNodes("map/tileset"))
            {
                if (tileSet.Attributes["source"] != null)
                {
                    TileSets.Add(new ExternalTileSetContent(tileSet, context));
                }
                else
                {
                    TileSets.Add(new TileSetContent(tileSet, context));
                }
            }

            foreach (XmlNode layerNode in document.SelectNodes("map/layer|map/objectgroup"))
            {
                LayerContent layerContent;

                if (layerNode.Name == "layer")
                {
                    layerContent = new TileLayerContent(layerNode, context);
                }
                else if (layerNode.Name == "objectgroup")
                {
                    layerContent = new MapObjectLayerContent(layerNode, context);
                }
                else
                {
                    throw new Exception("Unknown layer name: " + layerNode.Name);
                }

                // Layer names need to be unique for our lookup system, but Tiled
                // doesn't require unique names.
                string layerName = layerContent.Name;
                int duplicateCount = 2;

                // if a layer already has the same name...
                if (Layers.Find(l => l.Name == layerName) != null)
                {
                    // figure out a layer name that does work
                    do
                    {
                        layerName = string.Format("{0}{1}", layerContent.Name, duplicateCount);
                        duplicateCount++;
                    } while (Layers.Find(l => l.Name == layerName) != null);

                    // log a warning for the user to see
                    context.Logger.LogWarning(string.Empty, new ContentIdentity(), "Renaming layer \"{1}\" to \"{2}\" to make a unique name.", layerContent.Type, layerContent.Name, layerName);

                    // save that name
                    layerContent.Name = layerName;
                }

                Layers.Add(layerContent);
            }
        }
    }
}
