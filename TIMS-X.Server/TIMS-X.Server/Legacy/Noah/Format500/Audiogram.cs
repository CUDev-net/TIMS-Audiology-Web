using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace TIMS_X.Server.Legacy.Noah.Format500
{
	/// <summary>
	/// 500 Format Audiogram
	/// </summary>
	public class Audiogram : IMeasurementAudiogram
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public Audiogram()
		{
			MeasurementCondition = new MeasurementCondition();
			Curve = new List<TonePoint>();
		}

		#endregion Constructors

		#region Audiogram Members

		/// <summary>
		/// Curve for Audiogram
		/// </summary>
		public List<TonePoint> Curve
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the measurement condition
		/// </summary>
		public MeasurementCondition MeasurementCondition
		{
			get;
			private set;
		}

		/// <summary>
		/// Serialize to XML
		/// </summary>
		/// <returns></returns>
		public XElement Serialize( string rootName )
		{
			var root = N500Audiogram.CreateElementWithNamespace( rootName );

			root.Add(MeasurementCondition.Serialize());

			foreach (var tonePoint in Curve)
			{
				root.Add(tonePoint.Serialize());
			}
			return root;
		}

		#endregion Audiogram Members

	}
}
