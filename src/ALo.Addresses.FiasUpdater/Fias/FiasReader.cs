using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ALo.Addresses.FiasUpdater.Fias
{
    public class FiasReader
    {
        public async IAsyncEnumerable<T> Read<T>(string fileName, string rootNode, XmlSerializer serializer) where T : class
        {
            using var reader = XmlReader.Create(new StreamReader(fileName), new XmlReaderSettings { Async = true, IgnoreWhitespace = true });
            reader.ReadToFollowing(rootNode);
            while (await reader.ReadAsync() && reader.NodeType != XmlNodeType.EndElement)
            {
                var data = serializer.Deserialize(reader.ReadSubtree()) as T;
                yield return data;
            }
        }
    }
}
