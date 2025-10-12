using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Server.Data;

namespace TIMS_X.Server.Queries;

public class NoahDataMiningQuery
{
	private readonly NoahDbContext _dbContext;

	public NoahDataMiningQuery(NoahDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task DeleteNdmActionAsync(int noahActionId)
	{
		try
		{
			var existingAction = await _dbContext.NdmActions.FirstOrDefaultAsync(x => x.ActionId == noahActionId);
			// These data fields are queried in other methods and are not used by the caller of this method.
			// No need to transfer this data over the network
			if (existingAction != null)
			{
				_dbContext.Remove(existingAction);
				await _dbContext.SaveChangesAsync();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}
	}

	public async Task<NdmSearchCriteria> GetOpportunityTrackingCriteriaAsync()
	{
		var criteria = await _dbContext.NdmSearchCriterias
			.Include(x => x.SearchPoints)
			.FirstOrDefaultAsync(x => x.UsedForOpportunityTracking);
		return criteria;
	}

	public async Task<List<NdmAudiogram>> GetPatientAudiogramsAsync(int patientId, NdmSide side)
	{
		var audiograms = await _dbContext.NdmAudiograms.Where(x => x.PatientId == patientId && x.Side == (int)side)
			.ToListAsync();
		return audiograms;
	}

	public async Task<bool> IsNoahDataMiningEnabledAsync()
	{
		var result = false;
		try
		{
			result = await _dbContext.Practices.Select(x => x.UsesNoahDataMining).FirstOrDefaultAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task PutNdmActionAsync(NdmAction action)
	{
		try
		{
			if (action.Id == 0)
				await _dbContext.NdmActions.AddAsync(action);
			else
				_dbContext.NdmActions.Attach(action).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}
}