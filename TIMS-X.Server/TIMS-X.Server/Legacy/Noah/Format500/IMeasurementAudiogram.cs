using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMS_X.Server.Legacy.Noah.Format500
{
	public interface IMeasurementAudiogram
	{
		#region IMeasurementAudiogram Members

		/// <summary>
		/// Gets the measurement condition
		/// </summary>
		MeasurementCondition MeasurementCondition
		{
			get;
		}

		#endregion IMeasurementAudiogram Members

	}
}
