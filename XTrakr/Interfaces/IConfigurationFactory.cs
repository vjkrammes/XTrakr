using Microsoft.Extensions.Configuration;

namespace XTrakr.Interfaces;
public interface IConfigurationFactory
{
    IConfiguration Create(string filename, bool isOptional = false, string? directory = null);
}
