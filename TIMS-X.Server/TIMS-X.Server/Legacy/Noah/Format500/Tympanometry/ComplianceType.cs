using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TIMS_X.Server.Legacy.Noah.Format500.Tympanometry
{
	public class ComplianceType
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public ComplianceType()
		{
			ComplianceUnit = new ComplianceUnitType();
			ComplianceValue = new ComplianceValueType();
		}

		#endregion Constructors

		#region ComplianceType Members

		public ComplianceUnitType ComplianceUnit
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public ComplianceValueType ComplianceValue
		{
			get;
			set;
		}

		/// <summary>
		/// Serialize
		/// </summary>
		/// <returns></returns>
		public XElement Serialize(string rootName)
		{
			var root = N500Tympanogram.CreateElementWithNamespace( rootName );

			root.Add( ComplianceValue.Serialize() );
			root.Add( ComplianceUnit.Serialize() );

			return root;
		}

		#endregion ComplianceType Members

	}
}
