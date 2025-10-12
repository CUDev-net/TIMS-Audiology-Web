using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TIMS_X.Server.Legacy.Noah.Format500.Tympanometry.Enums;


namespace TIMS_X.Server.Legacy.Noah.Format500.Tympanometry
{
	public class TympanogramTestType
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public TympanogramTestType()
		{
			//CanalVolume = new ComplianceType();
			//ComplianceCurve = new ComplianceCurveType();
			//Gradient = new GradientType();
			//MaximumCompliance = new ComplianceType();
			//MeasurementCondition = new TympanogramMeasurementConditionsType();
			Pressure = -1;
			ResonanceFrequency = -1;
		}

		#endregion Constructors

		#region TympanogramTestType Members

		public ComplianceType CanalVolume
		{
			get;
			set;
		}

		public ComplianceCurveType ComplianceCurve
		{
			get;
			set;
		}

		public GradientType Gradient
		{
			get;
			set;
		}

		//public ComplianceType Gradient
		//{
		//	get;
		//	set;
		//}

		public ComplianceType MaximumCompliance
		{
			get;
			set;
		}

		public TympanogramMeasurementConditionsType MeasurementCondition
		{
			get;
			set;
		}

		public int Pressure
		{
			get;
			set;
		}

		public int ResonanceFrequency
		{
			get;
			set;
		}

		public TympanogramResultTypeEnum Result
		{
			get;
			set;
		}

		//public RecordingModeTypeEnum RecordingMode
		//{
		//	get;
		//	set;
		//}

		/// <summary>
		/// Serilize the node
		/// </summary>
		/// <param name="rootName"></param>
		/// <returns></returns>
		public XElement Serialize( string rootName )
		{
			var root = N500Tympanogram.CreateElementWithNamespace( rootName );
			if (MaximumCompliance != null)
			{
				root.Add( MaximumCompliance.Serialize( Constants.XML_MAXIMUMCOMPLIANCE ) );
			}
			if( CanalVolume != null )
			{
				root.Add( CanalVolume.Serialize( Constants.XML_CANALVOLUME ) );
			}
			if (MeasurementCondition != null)
			{
				root.Add(MeasurementCondition.Serialize());
			}
			root.Add( N500Tympanogram.CreateElementWithNamespace( Constants.XML_PRESSURE, Pressure.ToString() ) );

			if( Result != TympanogramResultTypeEnum.NotSet )
			{
				root.Add( N500Tympanogram.CreateElementWithNamespace( Constants.XML_RESULT, Result.ToString() ) );
			}

			return root;
		}

		#endregion TympanogramTestType Members

	}
}
