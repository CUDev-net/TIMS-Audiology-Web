namespace TIMS_X.Server.Legacy.Noah
{
	/// <summary>
	/// Encapsulates the date for a signal test
	/// </summary>
	public class SignalTest
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public SignalTest()
		{
			mTestPoints = new Test[AbmConst.FMAX];
		}

		#endregion Constructors

		#region SignalTest Members

		/// <summary>
		/// Gets or sets a specic test point
		/// </summary>
		/// <param name="index">Index of the test to access</param>
		/// <returns>Test</returns>
		public Test this[int index]
		{
			get
			{
				// Check the index limits.
				if( index < 0 || index >= AbmConst.FMAX )
					return null;
				else
					return mTestPoints[index];
			}
			set
			{
				if( !(index < 0 || index >= AbmConst.FMAX) )
					mTestPoints[index] = value;
			}
		}

		#endregion SignalTest Members

		#region Private Members

		private readonly Test[] mTestPoints;

		#endregion Private Members

	}
}
