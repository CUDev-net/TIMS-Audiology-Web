using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMS_X.Server.Legacy.Noah.Format500.Tympanometry
{
	public class EustachianTubeFunctionPerforatedEarDrumTest
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public EustachianTubeFunctionPerforatedEarDrumTest()
		{
			ClosePoints = new List<int>();
			OpenPoints = new List<int>();
			PressurePoints = new List<PressurePointType>();
		}

		#endregion Constructors

		#region EustachianTubeFunctionPerforatedEarDrumTest Members

		/// <summary>
		/// Open opoint
		/// </summary>
		public List<int> ClosePoints
		{
			get;
			set;
		}

		/// <summary>
		/// Open opoint
		/// </summary>
		public List<int> OpenPoints
		{
			get;
			set;
		}

		/// <summary>
		/// Pressure points
		/// </summary>
		public List<PressurePointType> PressurePoints
		{
			get;
			set;
		}

		/// <summary>
		/// Time base
		/// </summary>
		public decimal TimeBase
		{
			get;
			set;
		}

		#endregion EustachianTubeFunctionPerforatedEarDrumTest Members

	}
}
