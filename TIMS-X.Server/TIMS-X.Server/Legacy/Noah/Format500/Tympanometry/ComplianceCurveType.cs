using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMS_X.Server.Legacy.Noah.Format500.Tympanometry
{
	public class ComplianceCurveType
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public ComplianceCurveType()
		{
			CompliancePoints = new List<CompliancePointType>();
		}

		#endregion Constructors

		#region ComplianceCurveType Members

		public List<CompliancePointType> CompliancePoints
		{
			get;
			set;
		}

		public ComplianceUnitType ComplianceUnit
		{
			get;
			set;
		}

		#endregion ComplianceCurveType Members

	}
}
