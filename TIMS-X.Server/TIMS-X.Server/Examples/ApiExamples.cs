using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;

namespace TIMS_X.Server.Examples;

public class FindAppointmentOpeningsExample : IExamplesProvider<List<DateTime>>
{
	public List<DateTime> GetExamples()
	{
		return new List<DateTime>
		{
			new(2019, 1, 1, 9, 0, 0),
			new(2019, 1, 1, 9, 30, 0),
			new(2019, 1, 1, 10, 0, 0),
			new(2019, 1, 1, 13, 30, 0),
			new(2019, 1, 1, 14, 0, 0),
			new(2019, 1, 1, 14, 30, 0)
		};
	}
}