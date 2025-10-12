using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMS_X.Server.Legacy.Noah.Format500.Tympanometry
{
	public class ReflexPointType
	{
		#region Constructors

		public ReflexPointType()
		{
			Compliance = new ComplianceValueType();
		}

		#endregion Constructors

		#region ReflexPointType Members

		public ComplianceValueType Compliance
		{
			get;
			set;
		}

		public decimal Time
		{
			get;
			set;
		}

		#endregion ReflexPointType Members

	}
}
