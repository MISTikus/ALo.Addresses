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
        private readonly Func<Type, XmlSerializer> serializerFactory;

        public FiasUpdater(IOptions<Source> sourceOptions, FiasReader reader)
        {
            this.sourceOptions = sourceOptions;
            this.reader = reader;
            this.serializerFactory = t => new XmlSerializer(t);
        }

        internal async Task Update()
        {
            var sw = new System.Diagnostics.Stopwatch();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var fileName = @"D:\Downloads\Abyss\Fias\some.xml";
            //var fileName = @"D:\Downloads\Abyss\Fias\Xml\AS_ADDROBJ_20191201_203452ff-36b0-4d2f-956d-f73ec29b2440.XML";
            var i = 1;
            sw.Start();
            //await foreach (var data in this.reader.Read<Models.AddressObject>(fileName, "AddressObjects", this.serializerFactory(typeof(Models.AddressObject))))
            //{
            //    Console.WriteLine($"{i} : {data.Level}: {data.ShortTypeName} {data.OfficialName}");
            //    i++;
            //}
            Console.WriteLine($"Elapsed: {sw.Elapsed}");

            //fileName = @"D:\Downloads\Abyss\Fias\some_h.xml";
            fileName = @"D:\Downloads\Abyss\Fias\Xml\AS_HOUSE_20191201_f9a27344-f645-452c-8019-ae1354554774.XML";
            i = 1;
            sw.Restart();

            await foreach (var data in this.reader.Read<Models.HouseObject>(fileName, "Houses", this.serializerFactory(typeof(Models.HouseObject))))
            {
                Console.WriteLine($"{i} : {data.AddressId}: {data.HouseNumber} {data.BuildingNumber} {data.StructureNumber}");
                i++;
            }

            Console.WriteLine($"Elapsed: {sw.Elapsed}");
        }
    }
}
