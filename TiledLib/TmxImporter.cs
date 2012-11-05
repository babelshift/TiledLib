using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TiledLib
{
    [ContentImporter(".tmx", DisplayName = "TMX Importer - TiledLib")]
    public class TmxImporter : ContentImporter<MapContent>
    {
        public override MapContent Import(string filename, ContentImporterContext context)
        {
            // load the xml
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            // create the content from the xml
            MapContent content = new MapContent(doc, context);

            // save the filename and directory for the processor to use if it needs to
            content.Filename = filename;
            content.Directory = filename.Remove(filename.LastIndexOf('\\'));

            return content;
        }
    }
}
