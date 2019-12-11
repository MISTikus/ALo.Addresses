using ALo.Addresses.FiasUpdater.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ALo.Addresses.FiasUpdater.Fias
{
    internal class FiasUpdater
    {
        private readonly IOptions<Source> sourceOptions;
        private readonly FiasReader reader;
        private readonly XmlSerializer serializer;

        public FiasUpdater(IOptions<Source> sourceOptions, FiasReader reader)
        {
            this.sourceOptions = sourceOptions;
            this.reader = reader;
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

            await foreach (var data in this.reader.Read<Models.AddressObject>(fileName, "AddressObjects", this.serializer))
            {
                Console.WriteLine($"{i} : {data.Level}: {data.ShortName} {data.OfficialName}");
                i++;
            }

            //using (var reader = XmlReader.Create(new StreamReader(fileName), new XmlReaderSettings { Async = true, IgnoreWhitespace = true }))
            //{
            //    reader.ReadToFollowing("AddressObjects");
            //    while (await reader.ReadAsync() && reader.NodeType != XmlNodeType.EndElement)
            //    {
            //        try
            //        {
            //            var data = this.serializer.Deserialize(reader.ReadSubtree()) as Models.AddressObject;
            //            Console.WriteLine($"{i} : {data.Level}: {data.ShortName} {data.OfficialName}");
            //        }
            //        catch (Exception e)
            //        {
            //            Console.WriteLine(e.Message);
            //        }
            //        i++;
            //    }
            //}

            Console.WriteLine($"Elapsed: {sw.Elapsed}");
        }
    }
}
