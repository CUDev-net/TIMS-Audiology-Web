using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMS_X.Server.Legacy.Noah.Format500.Tympanometry.Enums;


namespace TIMS_X.Server.Legacy.Noah.Format500.Tympanometry
{
	public class ImpedanceMeasurementConditionType
	{
		#region Constructors

		public ImpedanceMeasurementConditionType()
		{
			CanalVolume = new ComplianceType();
		}

		#endregion Constructors

		#region ImpedanceMeasurementConditionType Members

		public ComplianceType CanalVolume
		{
			get;
			set;
		}

		public int Frequency
		{
			get;
			set;
		}

		public int Pressure
		{
			get;
			set;
		}

		public int ProbeFrequency
		{
			get;
			set;
		}

		public decimal SignalLevel
		{
			get;
			set;
		}

		public SignalOutputTypeEnum SignalOutput
		{
			get;
			set;
		}

		public SignalTypeEnum SignalType
		{
			get;
			set;
		}

		public ReflexTestTypeTypeEnum TestType
		{
			get;
			set;
		}

		#endregion ImpedanceMeasurementConditionType Members

	}
}
