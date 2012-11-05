using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace TiledLib
{
    public class Tile
    {
        public Rectangle Source;
        public PropertyCollection Properties = new PropertyCollection();
    }

    public class TileSetContent
    {
        public int FirstId;
        public string Name;
        public int TileWidth;
        public int TileHeight;
        public int Spacing;
        public int Margin;
        public string Image;
        public Color? ColorKey;
        public ExternalReference<Texture2DContent> Texture;
        public List<Tile> Tiles = new List<Tile>();
        public Dictionary<int, PropertyCollection> TileProperties = new Dictionary<int, PropertyCollection>();

        public TileSetContent(XmlNode node, ContentImporterContext context)
        {
            FirstId = int.Parse(node.Attributes["firstgid"].Value, CultureInfo.InvariantCulture);

            var preparedNode = this.PrepareXmlNode(node);
            this.Initialize(preparedNode, context);
        }

        private void Initialize(XmlNode node, ContentImporterContext context)
        {
            this.Name = node.Attributes["name"].Value;
            this.TileWidth = int.Parse(node.Attributes["tilewidth"].Value, CultureInfo.InvariantCulture);
            this.TileHeight = int.Parse(node.Attributes["tileheight"].Value, CultureInfo.InvariantCulture);

            if (node.Attributes["spacing"] != null)
            {
                this.Spacing = int.Parse(node.Attributes["spacing"].Value, CultureInfo.InvariantCulture);
            }

            if (node.Attributes["margin"] != null)
            {
                this.Margin = int.Parse(node.Attributes["margin"].Value, CultureInfo.InvariantCulture);
            }

            XmlNode imageNode = node["image"];
            this.Image = imageNode.Attributes["source"].Value;

            // if the image is in any director up from us, just take the filename
            if (this.Image.StartsWith(".."))
                this.Image = Path.GetFileName(this.Image);

            if (imageNode.Attributes["trans"] != null)
            {
                string color = imageNode.Attributes["trans"].Value;
                string r = color.Substring(0, 2);
                string g = color.Substring(2, 2);
                string b = color.Substring(4, 2);
                this.ColorKey = new Color((byte)Convert.ToInt32(r, 16), (byte)Convert.ToInt32(g, 16), (byte)Convert.ToInt32(b, 16));
            }
            foreach (XmlNode tileProperty in node.SelectNodes("tile"))
            {
                int id = this.FirstId + int.Parse(tileProperty.Attributes["id"].Value, CultureInfo.InvariantCulture);
                var properties = new PropertyCollection();

                XmlNode propertiesNode = tileProperty["properties"];
                if (propertiesNode != null)
                {
                    properties = new PropertyCollection(propertiesNode, context);
                }

                this.TileProperties.Add(id, properties);
            }
        }

        protected virtual XmlNode PrepareXmlNode(XmlNode root)
        {
            return root;
        }
    }
}
