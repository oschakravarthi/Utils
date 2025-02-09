using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.ApiClient.Contracts;

public interface IConfigurationProvider
{
    Task<List<KeyValuePair<string, string>>> GetConfigurationAsync();
}