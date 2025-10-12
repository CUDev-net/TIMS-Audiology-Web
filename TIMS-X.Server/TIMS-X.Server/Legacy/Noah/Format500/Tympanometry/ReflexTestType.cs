using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMS_X.Server.Legacy.Noah.Format500.Tympanometry
{
	public class ReflexTestType
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public ReflexTestType()
		{
			ImpedanceMeasurementCondition = new ImpedanceMeasurementConditionType();
			ReflexCurve = new ReflexCurveType();
		}

		#endregion Constructors

		#region ReflexTestType Members

		public ImpedanceMeasurementConditionType ImpedanceMeasurementCondition
		{
			get;
			set;
		}

		public ReflexCurveType ReflexCurve
		{
			get;
			set;
		}

		public int ResultOfReflexTest
		{
			get;
			set;
		}

		#endregion ReflexTestType Members

	}
}
