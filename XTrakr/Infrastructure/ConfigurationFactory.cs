using Microsoft.Extensions.Configuration;

using System.IO;

using XTrakr.Interfaces;

namespace XTrakr.Infrastructure;
public class ConfigurationFactory : IConfigurationFactory
{
    public IConfiguration Create(string filename, bool isOptional = false, string? directory = null)
    {
        var dir = string.IsNullOrWhiteSpace(directory) ? Directory.GetCurrentDirectory() : directory;
        var ret = new ConfigurationBuilder()
            .SetBasePath(dir)
            .AddJsonFile(filename, optional: isOptional, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        return ret;
    }
}
