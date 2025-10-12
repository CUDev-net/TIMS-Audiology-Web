using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMS_X.Server.Legacy.Noah.Format500.Tympanometry.Enums;


namespace TIMS_X.Server.Legacy.Noah.Format500.Tympanometry
{
	public class GradientType
	{
		#region GradientType Members

		public UnitTypeEnum GradientUnit
		{
			get;
			set;
		}

		public ComplianceValueType GradientValue
		{
			get;
			set;
		}

		#endregion GradientType Members

	}
}
