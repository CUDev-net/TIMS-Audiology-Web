using System;
using System.Linq;
using TIMS_X.Core.Domain;

namespace TIMS_X.Server.Filters;

public class PatientQueryFilter
{
	public DateTime? BirthDate { get; set; }
	public string Email { get; set; }
	public string FirstName { get; set; }
	public string HomePhone { get; set; }
	public string LastName { get; set; }
	public string MobilePhone { get; set; }

	public IQueryable<Patient> ApplyTo(IQueryable<Patient> query)
	{
		query = query.Where(pat => pat.FirstName == FirstName && pat.LastName == LastName);
		if (!string.IsNullOrWhiteSpace(Email)) query = query.Where(pat => pat.Email == Email);
		if (!string.IsNullOrWhiteSpace(HomePhone)) query = query.Where(pat => pat.HomePhone == HomePhone);
		if (!string.IsNullOrWhiteSpace(MobilePhone)) query = query.Where(pat => pat.MobilePhone == MobilePhone);

		if (BirthDate.HasValue)
			query = query.Where(pat => pat.BirthDate.HasValue && pat.BirthDate.Value.Date == BirthDate.Value.Date);

		return query;
	}
}