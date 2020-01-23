using System.Collections.Generic;
using System.IO;

namespace ALo.Addresses.FiasUpdater.Infrastructure
{
    internal class SystemFacade : ISystemFacade
    {
        public IEnumerable<string> GetFilesAsync(string folder) => Directory.GetFiles(folder);
    }

    internal interface ISystemFacade
    {
        IEnumerable<string> GetFilesAsync(string folder);
    }
}
