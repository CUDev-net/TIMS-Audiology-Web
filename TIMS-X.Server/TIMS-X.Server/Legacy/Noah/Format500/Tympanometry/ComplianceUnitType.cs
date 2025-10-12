using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TIMS_X.Server.Legacy.Noah.Format500.Tympanometry.Enums;


namespace TIMS_X.Server.Legacy.Noah.Format500.Tympanometry
{
	public class ComplianceUnitType
	{
		#region ComplianceUnitType Members

		/// <summary>
		/// 
		/// </summary>
		public UnitTypeEnum ArgumentUnit1
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public UnitTypeEnum ArgumentUnit2
		{
			get;
			set;
		}

		public XElement Serialize()
		{
			var root = N500Tympanogram.CreateElementWithNamespace( Constants.XML_COMPLIANCEUNIT );

			root.Add( N500Tympanogram.CreateElementWithNamespace( Constants.XML_ARGUMENTCOMPLIANCE1, ArgumentUnit1.ToString() ) );
			root.Add( N500Tympanogram.CreateElementWithNamespace( Constants.XML_ARGUMENTCOMPLIANCE2, ArgumentUnit1.ToString() ) );

			return root;

		}

		#endregion ComplianceUnitType Members

	}
}
