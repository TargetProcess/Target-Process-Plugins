using System.Security.Principal;
using System.Web;

namespace Tp.Core
{
	public interface IAuthentication
	{
		bool Authenticate(HttpContextBase context, out IPrincipal principal);
	}
}