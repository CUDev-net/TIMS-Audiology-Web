using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Server.Data;

namespace TIMS_X.Server.Queries;

public class ClaimsQuery
{
	private readonly ClaimsDbContext _dbContext;

	public ClaimsQuery(ClaimsDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<List<ClaimTransaction>> GetClaimTransactionsForAppointmentAsync(int appointmentId)
	{
		var transactions =
			await _dbContext.ClaimTransactions.Where(x => x.AppointmentId == appointmentId).ToListAsync();
		return transactions;
	}

	public async Task<List<PosDocument>> GetPosDocumentsForAppointmentAsync(int appointmentId)
	{
		var docs = await _dbContext.PosDocuments.Where(x => x.AppointmentId == appointmentId).ToListAsync();
		return docs;
	}
}