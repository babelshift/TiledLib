using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TiledLib
{
    public enum MapObjectType : byte
    {
        Plain,
        Tile,
        Polygon,
        Polyline
    }

    public class MapObjectContent
    {
        public MapObjectType ObjectType = MapObjectType.Plain;

        public string Name = string.Empty;
        public string Type = string.Empty;
        public Rectangle Bounds;
        public int GID;
        public List<Vector2> Points = new List<Vector2>();
        public PropertyCollection Properties = new PropertyCollection();

        public MapObjectContent(XmlNode node, ContentImporterContext context)
        {
            // get the object's name and type
            if (node.Attributes["name"] != null)
                Name = node.Attributes["name"].Value;
            if (node.Attributes["type"] != null)
                Type = node.Attributes["type"].Value;

            // read the object properties
            if (node["properties"] != null)
                Properties = new PropertyCollection(node["properties"], context);

            // parse out the bounds of the object. values default to 0 if the attribute is missing from the node.
            int x = node.Attributes["x"] != null ? int.Parse(node.Attributes["x"].Value, CultureInfo.InvariantCulture) : 0;
            int y = node.Attributes["y"] != null ? int.Parse(node.Attributes["y"].Value, CultureInfo.InvariantCulture) : 0;
            int width = node.Attributes["width"] != null ? int.Parse(node.Attributes["width"].Value, CultureInfo.InvariantCulture) : 0;
            int height = node.Attributes["height"] != null ? int.Parse(node.Attributes["height"].Value, CultureInfo.InvariantCulture) : 0;
            Bounds = new Rectangle(x, y, width, height);

            // stores a string of points to parse out if this object is a polygon or polyline
            string pointsAsString = null;

            // if there's a GID, it's a tile object
            if (node.Attributes["gid"] != null)
            {
                ObjectType = MapObjectType.Tile;
                GID = int.Parse(node.Attributes["gid"].Value, CultureInfo.InvariantCulture);
            }
            // if there's a polygon node, it's a polygon object
            else if (node["polygon"] != null)
            {
                ObjectType = MapObjectType.Polygon;
                pointsAsString = node["polygon"].Attributes["points"].Value;
            }
            // if there's a polyline node, it's a polyline object
            else if (node["polyline"] != null)
            {
                ObjectType = MapObjectType.Polyline;
                pointsAsString = node["polyline"].Attributes["points"].Value;
            }

            // if we have some points to parse, we do that now
            if (pointsAsString != null)
            {
                // points are separated first by spaces
                string[] pointPairs = pointsAsString.Split(' ');
                foreach (string p in pointPairs)
                {
                    // then we split on commas
                    string[] coords = p.Split(',');

                    // then we parse the X/Y coordinates
                    Points.Add(new Vector2(
                        float.Parse(coords[0], CultureInfo.InvariantCulture), 
                        float.Parse(coords[1], CultureInfo.InvariantCulture)));
                }
            }
        }
    }
}
