using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TiledLib
{
    public abstract class LayerContent
    {
        public string Name;
        public string Type;
        public int Width;
        public int Height;
        public float Opacity = 1f;
        public bool Visible = true;
        public PropertyCollection Properties = new PropertyCollection();

        public LayerContent(XmlNode node, ContentImporterContext context)
        {
            Type = node.Name;
            Name = node.Attributes["name"].Value;
            Width = int.Parse(node.Attributes["width"].Value, CultureInfo.InvariantCulture);
            Height = int.Parse(node.Attributes["height"].Value, CultureInfo.InvariantCulture);

            if (node.Attributes["opacity"] != null)
            {
                Opacity = float.Parse(node.Attributes["opacity"].Value, CultureInfo.InvariantCulture);
            }

            if (node.Attributes["visible"] != null)
            {
                Visible = int.Parse(node.Attributes["visible"].Value, CultureInfo.InvariantCulture) == 1;
            }

            XmlNode propertiesNode = node["properties"];
            if (propertiesNode != null)
            {
                Properties = new PropertyCollection(propertiesNode, context);
            }
        }
    }
}
