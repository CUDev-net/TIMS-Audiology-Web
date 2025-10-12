using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TIMS_X.Server.Legacy.Noah.Extended
{
	public class ExtendedData
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public ExtendedData()
		{
			ExtendedCurves = new List<ExtendedCurve>();
		}

		#endregion Constructors

		#region ExtendedData Members

		/// <summary>
		/// Gets curves
		/// </summary>
		public List<ExtendedCurve> ExtendedCurves
		{
			get;
			private set;
		}

		/// <summary>
		/// Deserialize data
		/// </summary>
		/// <param name="data"></param>
		public void DeSerialize( string data )
		{
			var xDocument = XDocument.Parse( data );
			var xCurves = (from p in xDocument.Root.Elements()
						   where p.Name == ExtendedCurve.XML_EXTENDED_CURVE
						   select p);

			foreach( var xCurve in xCurves )
			{
				var curve = new ExtendedCurve();
				curve.DeSerialize( xCurve );
				ExtendedCurves.Add( curve );
			}
		}

		/// <summary>
		/// Serialize to XML
		/// </summary>
		/// <returns></returns>
		public XElement Serialize()
		{
			var root = new XElement( "TIMSExtendedAction" );

			foreach( var extendedCurve in ExtendedCurves )
			{
				root.Add( extendedCurve.Serialize() );
			}
			return root;
		}

		#endregion ExtendedData Members

	}
}
