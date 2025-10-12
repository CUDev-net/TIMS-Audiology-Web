using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMS_X.Server.Legacy.Noah.Format500
{
	public class SpeechAudiogram : IMeasurementAudiogram
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public SpeechAudiogram()
		{
			MeasurementCondition = new MeasurementCondition();
			Points = new List<SpeechScorePoint>();
		}

		#endregion Constructors

		#region SpeechAudiogram Members

		/// <summary>
		/// Measurement condition
		/// </summary>
		public MeasurementCondition MeasurementCondition
		{
			get;
			private set;
		}

		/// <summary>
		/// Points on the curve
		/// </summary>
		public List<SpeechScorePoint> Points
		{
			get;
			private set;
		}

		#endregion SpeechAudiogram Members

	}
}
