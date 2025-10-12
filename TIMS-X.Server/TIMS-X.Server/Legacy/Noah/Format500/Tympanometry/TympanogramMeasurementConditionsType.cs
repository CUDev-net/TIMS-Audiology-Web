using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TIMS_X.Server.Legacy.Noah.Format500.Tympanometry.Enums;


namespace TIMS_X.Server.Legacy.Noah.Format500.Tympanometry
{
	public class TympanogramMeasurementConditionsType
	{
		#region TympanogramMeasurementConditionsType Members

		public int ProbeFrequency
		{
			get;
			set;
		}

		public RecordingModeTypeEnum RecordingMode
		{
			get;
			set;
		}

		public int SweepSpeed
		{
			get;
			set;
		}

		/// <summary>
		/// Serailize to XML
		/// </summary>
		/// <returns></returns>
		public XElement Serialize()
		{
			var root = N500Tympanogram.CreateElementWithNamespace( Constants.XML_MEASUREMENTCONDITION );

			root.Add( N500Tympanogram.CreateElementWithNamespace( Constants.XML_SWEEPSPEED, SweepSpeed.ToString() ) );
			if( RecordingMode != RecordingModeTypeEnum.NotSet )
			{
				root.Add( N500Tympanogram.CreateElementWithNamespace( Constants.XML_RECORDMODE, RecordingMode.ToString() ) );
			}
			root.Add( N500Tympanogram.CreateElementWithNamespace( Constants.XML_PROBEFREQUENCY, ProbeFrequency.ToString() ) );

			return root;
		}

		#endregion TympanogramMeasurementConditionsType Members

	}
}
