using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarterKit
{
	public interface ICookieKeyRepository
	{
		Task<bool> Save(CookieKeys cookieKeys);
		Task<List<CookieKeys>> GetAll();
	}
}