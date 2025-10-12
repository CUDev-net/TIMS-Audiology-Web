using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TIMS_X.Server.Legacy.Noah.Format500.Tympanometry
{
	public class ComplianceValueType
	{
		#region ComplianceValueType Members

		public int ArgumentCompliance1
		{
			get;
			set;
		}

		public int ArgumentCompliance2
		{
			get;
			set;
		}

		/// <summary>
		/// Serialize object
		/// </summary>
		/// <returns></returns>
		public XElement Serialize()
		{
			var root = N500Tympanogram.CreateElementWithNamespace( Constants.XML_COMPLIANCEVALUE );

			root.Add( N500Tympanogram.CreateElementWithNamespace( Constants.XML_ARGUMENTCOMPLIANCE1, ArgumentCompliance1.ToString() ) );
			root.Add( N500Tympanogram.CreateElementWithNamespace( Constants.XML_ARGUMENTCOMPLIANCE2, ArgumentCompliance2.ToString() ) );

			return root;
		}

		#endregion ComplianceValueType Members

	}
}
