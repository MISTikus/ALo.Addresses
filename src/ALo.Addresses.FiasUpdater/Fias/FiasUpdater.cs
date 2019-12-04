using ALo.Addresses.FiasUpdater.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ALo.Addresses.FiasUpdater.Fias
{
    internal class FiasUpdater
    {
        private readonly IOptions<Source> sourceOptions;
        private readonly XmlSerializer serializer;

        public FiasUpdater(IOptions<Source> sourceOptions)
        {
            this.sourceOptions = sourceOptions;
            this.serializer = new XmlSerializer(typeof(Models.AddressObject));
        }

        internal async Task Update()
        {
            var sw = new System.Diagnostics.Stopwatch();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // @"D:\Downloads\Abyss\Fias\some.xml";
            var fileName = @"D:\Downloads\Abyss\Fias\Xml\AS_ADDROBJ_20191201_203452ff-36b0-4d2f-956d-f73ec29b2440.XML";

            var i = 1;
            sw.Start();

            using (var reader = XmlReader.Create(new StreamReader(fileName), new XmlReaderSettings { Async = true, IgnoreWhitespace = true }))
            {
                reader.ReadToFollowing("AddressObjects");
                while (await reader.ReadAsync() && reader.NodeType != XmlNodeType.EndElement)
                {
                    try
                    {
                        var data = this.serializer.Deserialize(reader.ReadSubtree()) as Models.AddressObject;
                        Console.WriteLine($"{i} : {data.Level}: {data.ShortName} {data.OfficialName}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    i++;
                }
            }

            Console.WriteLine($"Elapsed: {sw.Elapsed}");
        }
    }
}
