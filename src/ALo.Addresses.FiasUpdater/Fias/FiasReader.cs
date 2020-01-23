using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace ALo.Addresses.FiasUpdater.Fias
{
    public class FiasReader
    {
        public delegate void ProgressChangedEventHander(object sender, double progressPersentage);
        public event ProgressChangedEventHander ProgressChanged;

        public async IAsyncEnumerable<T> Read<T>(string fileName, string rootNode, XmlSerializer serializer,
            [EnumeratorCancellation]CancellationToken cancellationToken, int length = 0, int skip = 0, int take = -1) where T : class
        {
            using var sreamReader = new StreamReader(fileName);
            using var reader = XmlReader.Create(sreamReader, new XmlReaderSettings { Async = true, IgnoreWhitespace = true });
            reader.ReadToFollowing(rootNode);
            long lastPosition = 0;
            var previous = "";
            var i = -1;
            while (await reader.ReadAsync() && reader.NodeType != XmlNodeType.EndElement)
            {
                i++;

                if (cancellationToken.IsCancellationRequested)
                    break;

                if (lastPosition != sreamReader.BaseStream.Position)
                {
                    lastPosition = sreamReader.BaseStream.Position;
                    var progress = 100.0 * lastPosition / sreamReader.BaseStream.Length;
                    if (progress.ToString($"F{length}") != previous)
                    {
                        previous = progress.ToString($"F{length}");
                        ProgressChanged.Invoke(this, progress);
                    }
                }

                if (i < skip)
                    continue;
                if (take >= 0 && i > skip + take)
                    break;

                var data = serializer.Deserialize(reader.ReadSubtree()) as T;

                yield return data;
            }
        }
    }
}
