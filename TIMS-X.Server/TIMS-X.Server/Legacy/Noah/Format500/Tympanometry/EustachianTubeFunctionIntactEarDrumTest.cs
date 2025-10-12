using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMS_X.Server.Legacy.Noah.Format500.Tympanometry
{
	public class EustachianTubeFunctionIntactEarDrumTest
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public EustachianTubeFunctionIntactEarDrumTest()
		{
			Pressure = new List<int>();
			CanalVolume = new ComplianceType();
			MeasurementCondition = new TympanogramMeasurementConditionsType();
			ComplianceCurve = new List<ComplianceCurveType>();
		}

		#endregion Constructors

		#region EustachianTubeFunctionIntactEarDrumTest Members

		/// <summary>
		/// 
		/// </summary>
		public ComplianceType CanalVolume
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public List<ComplianceCurveType> ComplianceCurve
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public TympanogramMeasurementConditionsType MeasurementCondition
		{
			get;
			set;
		}

		/// <summary>
		/// Pressure
		/// </summary>
		public List<int> Pressure
		{
			get;
			set;
		}

		#endregion EustachianTubeFunctionIntactEarDrumTest Members

	}
}
