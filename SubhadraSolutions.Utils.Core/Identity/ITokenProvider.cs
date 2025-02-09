using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Identity;

public interface ITokenProvider
{
    Task<AccessTokenInfo> GetTokenAsync(CancellationToken cancellationToken);
}