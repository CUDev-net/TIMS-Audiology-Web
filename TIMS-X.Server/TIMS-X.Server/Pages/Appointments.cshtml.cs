using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TIMS_X.Core.Models;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Pages;
#if DEBUG
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
#else
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Hidden")]
#endif
public class AppointmentsModel : PageModel
{
	private readonly ProviderQuery _providerQuery;
	private readonly PracticeQuery _practiceQuery;

	public AppointmentsModel(PracticeQuery practiceQuery, ProviderQuery providerQuery)
	{
		_practiceQuery = practiceQuery;
		_providerQuery = providerQuery;
	}

	public List<CheckableSite> Sites { get; set; }
	public List<CheckedItem<ProviderItem>> Providers { get; set; }

	public async Task OnGetAsync()
	{
		var sites = await _practiceQuery.GetSitesAsync(false);
		Sites = sites.Select(x => new CheckableSite(x)).ToList();
		var providers = await _providerQuery.GetProvidersAsync(false);
		Providers = providers.Select(x => new CheckedItem<ProviderItem>(x)).ToList();
	}
}