using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Server.Data;

namespace TIMS_X.Server.Queries;

public class HaHistoryQuery
{
	private readonly HaHistoryDbContext _dbContext;

	public HaHistoryQuery(HaHistoryDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<List<HaHistory>> GetHaHistoriesAsync(int patientId)
	{
		var haHistories = await _dbContext.HaHistories
			.Include(ha => ha.Status)
			.Where(ha => ha.PatientId == patientId && !ha.IsAccessory && !ha.IsEarmold)
			.ToListAsync();
		return haHistories;
	}
}