using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TiledLib
{
    public class PropertyCollection : Dictionary<string, string>
    {
        public PropertyCollection()
        { 
        }

        public PropertyCollection(XmlNode node, ContentImporterContext context)
        {
            foreach (XmlNode property in node.ChildNodes)
            {
                string name = property.Attributes["name"].Value;
                string value = property.Attributes["value"].Value;
                bool foundCopy = false;

                /* 
                 * A bug in Tiled will sometimes cause the file to contain identical copies of properties.
                 * I would fix it, but I'd have to dig into the Tiled code. instead, we'll detect exact
                 * duplicates here and log some warnings, failing only if the value is actually different.
                 * 
                 * To repro the bug, create two maps that use the same tileset. Open the first file in Tiled
                 * and set a property on a tile. Then open the second map and open the first back up. Look
                 * at the propertes on the tile. It will have two or three copies of the same property.
                 * 
                 * If you encounter the bug, you can remedy it in Tiled by closing the current file (Ctrl-F4
                 * or use Close from the File menu) and then reopen it. The tile will no longer have the
                 * copies of the property.
                 */
                foreach (var p in this)
                {
                    if (p.Key == name)
                    {
                        if (p.Value == value)
                        {
                            context.Logger.LogWarning("", new ContentIdentity(), "Multiple properties of name {0} found with value {1}", new object[] { name, value });
                            foundCopy = true;
                        }
                        else
                        {
                            throw new Exception(string.Format("Multiple properties of name {0} exist with different values: {1} and {2}", name, value, p.Value));
                        }
                    }
                }

                // we only want to add one copy of any duplicate properties
                if (!foundCopy)
                    Add(name, value);
            }
        }
    }
}
