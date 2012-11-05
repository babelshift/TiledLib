using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TiledLib
{
    public class TileLayerContent : LayerContent
    {
        public uint[] Data;

        public TileLayerContent(XmlNode node, ContentImporterContext context)
            : base(node, context)
        {
            XmlNode dataNode = node["data"];
            Data = new uint[Width * Height];

            // figure out what encoding is being used, if any, and process
            // the data appropriately
            if (dataNode.Attributes["encoding"] != null)
            {
                string encoding = dataNode.Attributes["encoding"].Value;

                if (encoding == "base64")
                {
                    ReadAsBase64(node, dataNode);
                }
                else if (encoding == "csv")
                {
                    ReadAsCsv(node, dataNode);
                }
                else
                {
                    throw new Exception("Unknown encoding: " + encoding);
                }
            }
            else
            {
                // XML format simply lays out a lot of <tile gid="X" /> nodes inside of data.

                int i = 0;
                foreach (XmlNode tileNode in dataNode.SelectNodes("tile"))
                {
                    Data[i] = uint.Parse(tileNode.Attributes["gid"].Value, CultureInfo.InvariantCulture);
                    i++;
                }

                if (i != Data.Length)
                    throw new Exception("Not enough tile nodes to fill data");
            }
        }

        private void ReadAsCsv(XmlNode node, XmlNode dataNode)
        {
            // split the text up into lines
            string[] lines = node.InnerText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // iterate each line
            for (int i = 0; i < lines.Length; i++)
            {
                // split the line into individual pieces
                string[] indices = lines[i].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                // iterate the indices and store in our data
                for (int j = 0; j < indices.Length; j++)
                {
                    Data[i * Width + j] = uint.Parse(indices[j], CultureInfo.InvariantCulture);
                }
            }
        }

        private void ReadAsBase64(XmlNode node, XmlNode dataNode)
        {
            // get a stream to the decoded Base64 text
            Stream data = new MemoryStream(Convert.FromBase64String(node.InnerText), false);

            // figure out what, if any, compression we're using. the compression determines
            // if we need to wrap our data stream in a decompression stream
            if (dataNode.Attributes["compression"] != null)
            {
                string compression = dataNode.Attributes["compression"].Value;

                if (compression == "gzip")
                {
                    data = new GZipStream(data, CompressionMode.Decompress, false);
                }
                else if (compression == "zlib")
                {
                    data = new Ionic.Zlib.ZlibStream(data, Ionic.Zlib.CompressionMode.Decompress, false);
                }
                else
                {
                    throw new InvalidOperationException("Unknown compression: " + compression);
                }
            }

            // simply read in all the integers
            using (data)
            {
                using (BinaryReader reader = new BinaryReader(data))
                {
                    for (int i = 0; i < Data.Length; i++)
                    {
                        Data[i] = reader.ReadUInt32();
                    }
                }
            }
        }
    }
}
