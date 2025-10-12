namespace TIMS_X.Server.Legacy.Noah
{
	/// <summary>
	/// An audiogram test results
	/// </summary>
	public class AudiogramTestResults
	{
		#region AudiogramTestResults Members

		/// <summary>
		/// Gets the both test results
		/// </summary>
		public TestResults BothResults
		{
			get
			{
				if( mBothTestResults == null )
				{
					mBothTestResults = new TestResults();
				}
				return mBothTestResults;
			}
		}

		/// <summary>
		/// Gets the left results
		/// </summary>
		public TestResults LeftResults
		{
			get
			{
				if( mLeftTestResults == null )
				{
					mLeftTestResults = new TestResults();
				}
				return mLeftTestResults;
			}
		}

		/// <summary>
		/// Gets the right results
		/// </summary>
		public TestResults RightResults
		{
			get
			{
				if( mRightTestResults == null )
				{
					mRightTestResults = new TestResults();
				}
				return mRightTestResults;
			}
		}

		/// <summary>
		/// Gets a specific side test results
		/// </summary>
		/// <param name="side">Side to get</param>
		/// <returns>Test results</returns>
		public TestResults GetTestResults( TestSide side )
		{
			switch( side )
			{
				case TestSide.Right:
					return RightResults;
				case TestSide.Left:
					return LeftResults;
				case TestSide.Both:
					return BothResults;
				default:
					return null;
			}
		}

		#endregion AudiogramTestResults Members

		#region Private Members

		private TestResults mBothTestResults;
		private TestResults mLeftTestResults;
		private TestResults mRightTestResults;

		#endregion Private Members

	}
}
