using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMS_X.Server.Legacy.Noah.Format500.Tympanometry
{
	public class ReflexCurveType
	{
		#region Constructors

		public ReflexCurveType()
		{
			ReflexPoints = new List<ReflexPointType>();
			ComplianceUnit = new ComplianceUnitType();
		}

		#endregion Constructors

		#region ReflexCurveType Members

		public ComplianceUnitType ComplianceUnit
		{
			get;
			set;
		}

		public List<ReflexPointType> ReflexPoints
		{
			get;
			set;
		}

		#endregion ReflexCurveType Members

	}
}
