using TIMS_X.Server.Legacy.Noah.Enums;

namespace TIMS_X.Server.Legacy.Noah
{
	/// <summary>
	/// Results of one specific test
	/// </summary>
	public class Test
	{
		#region Test Members

		/// <summary>
		/// Condition 1, determines the difference between a SFA and SFA 2 test
		/// </summary>
		public HearingTestConditionEnum Condition1
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the frequency of the test
		/// </summary>
		public int Frequency1
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Intensity of the test
		/// </summary>
		public int Intensity1
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the status of the test
		/// </summary>
		public SignalOutputEnum SignalOutput
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the status of the test
		/// </summary>
		public PointStatusEnum Status
		{
			get;
			set;
		}

		#endregion Test Members

	}
}
