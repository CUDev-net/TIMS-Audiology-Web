using TIMS_X.Server.Legacy.Noah.Enums;

namespace TIMS_X.Server.Legacy.Noah
{
	public class TestResults
	{
		#region TestResults Members

		/// <summary>
		/// Gets the AC masked test array 
		/// </summary>
		public SignalTest[] ACMTest
		{
			get
			{
				if( mAcmTest == null )
				{
					mAcmTest = new SignalTest[AbmConst.NUMBER_THRAUDIOGRAMS];
					for( var i = 0; i < AbmConst.NUMBER_THRAUDIOGRAMS; i++ )
						mAcmTest[i] = new SignalTest();
				}
				return mAcmTest;

			}
		}

		/// <summary>
		/// Gets the AC test array
		/// </summary>
		public SignalTest[] ACTest
		{
			get
			{
				if( mAcTest == null )
				{
					mAcTest = new SignalTest[AbmConst.NUMBER_THRAUDIOGRAMS];
					for( var i = 0; i < AbmConst.NUMBER_THRAUDIOGRAMS; i++ )
						mAcTest[i] = new SignalTest();
				}
				return mAcTest;

			}
		}

		/// <summary>
		/// Gets the ART test array
		/// </summary>
		public SignalTest[] ARTTest
		{
			get
			{
				if( mArtTest == null )
				{
					mArtTest = new SignalTest[AbmConst.NUMBER_THRAUDIOGRAMS];
					for( var i = 0; i < AbmConst.NUMBER_THRAUDIOGRAMS; i++ )
						mArtTest[i] = new SignalTest();
				}
				return mArtTest;

			}
		}

		/// <summary>
		/// Gets the BC masked test array
		/// </summary>
		public SignalTest[] BCMTest
		{
			get
			{
				if( mBcmTest == null )
				{
					mBcmTest = new SignalTest[AbmConst.NUMBER_THRAUDIOGRAMS];
					for( var i = 0; i < AbmConst.NUMBER_THRAUDIOGRAMS; i++ )
						mBcmTest[i] = new SignalTest();
				}
				return mBcmTest;

			}
		}

		/// <summary>
		/// Gets the BC test array
		/// </summary>
		public SignalTest[] BCTest
		{
			get
			{
				if( mBcTest == null )
				{
					mBcTest = new SignalTest[AbmConst.NUMBER_THRAUDIOGRAMS];
					for( var i = 0; i < AbmConst.NUMBER_THRAUDIOGRAMS; i++ )
						mBcTest[i] = new SignalTest();
				}
				return mBcTest;
			}
		}

		/// <summary>
		/// Gets the Inser Phones test array
		/// </summary>
		public SignalTest[] IPTest
		{
			get
			{
				if( mIpTest == null )
				{
					mIpTest = new SignalTest[AbmConst.NUMBER_THRAUDIOGRAMS];
					for( var i = 0; i < AbmConst.NUMBER_THRAUDIOGRAMS; i++ )
						mIpTest[i] = new SignalTest();
				}
				return mIpTest;

			}
		}

		/// <summary>
		/// Gets the MCL test array
		/// </summary>
		public SignalTest[] MCLTest
		{
			get
			{
				if( mMclTest == null )
				{
					mMclTest = new SignalTest[AbmConst.NUMBER_THRAUDIOGRAMS];
					for( var i = 0; i < AbmConst.NUMBER_THRAUDIOGRAMS; i++ )
						mMclTest[i] = new SignalTest();
				}
				return mMclTest;
			}
		}

		/// <summary>
		/// Gets the SFA test array
		/// </summary>
		public SignalTest[] SFATest
		{
			get
			{
				if( mSfaTest == null )
				{
					mSfaTest = new SignalTest[AbmConst.NUMBER_THRAUDIOGRAMS];
					for( var i = 0; i < AbmConst.NUMBER_THRAUDIOGRAMS; i++ )
						mSfaTest[i] = new SignalTest();
				}
				return mSfaTest;
			}
		}

		/// <summary>
		/// Gets the SF test array
		/// </summary>
		public SignalTest[] SFTest
		{
			get
			{
				if( mSfTest == null )
				{
					mSfTest = new SignalTest[AbmConst.NUMBER_THRAUDIOGRAMS];
					for( var i = 0; i < AbmConst.NUMBER_THRAUDIOGRAMS; i++ )
						mSfTest[i] = new SignalTest();
				}
				return mSfTest;
			}
		}

		/// <summary>
		/// Gets the UCL test array
		/// </summary>
		public SignalTest[] UCLTest
		{
			get
			{
				if( mUclTest == null )
				{
					mUclTest = new SignalTest[AbmConst.NUMBER_THRAUDIOGRAMS];
					for( var i = 0; i < AbmConst.NUMBER_THRAUDIOGRAMS; i++ )
						mUclTest[i] = new SignalTest();
				}
				return mUclTest;
			}
		}

		/// <summary>
		/// Gets a specific test from the resutls
		/// </summary>
		/// <param name="testType">Test type to get</param>
		/// <returns>Test(array of Test objects, each representing a position)</returns>
		public SignalTest[] GetTest( int testType )
		{
			switch( testType )
			{
				case AbmConst.AC:
					return ACTest;
				case AbmConst.BC:
					return BCTest;
				case AbmConst.UCL:
					return UCLTest;
				case AbmConst.MCL:
					return MCLTest;
				case AbmConst.ACM:
					return ACMTest;
				case AbmConst.BCM:
					return BCMTest;
				case AbmConst.SF:
					return SFTest;
				case AbmConst.SFA:
					return SFATest;
				case AbmConst.ART:
					return ARTTest;
				case AbmConst.IP:
					return IPTest;
				default:
					return null;
			}
		}

		public void SetTestCondition1( int testType, int position, HearingTestConditionEnum condition1, int index )
		{
			SignalTest[] test = GetTest( testType );
			if( test[index][position] == null )
			{
				test[index][position] = new Test();
			}
			test[index][position].Condition1 = condition1;
		}

		public void SetTestFrequency( int testType, int position, int frequency, int index )
		{
			SignalTest[] test = GetTest( testType );
			if( test[index][position] == null )
			{
				test[index][position] = new Test();
			}
			test[index][position].Frequency1 = frequency;
		}

		public void SetTestIntensity( int testType, int position, int intensity, int index )
		{
			SignalTest[] test = GetTest( testType );
			if( test[index][position] == null )
			{
				test[index][position] = new Test();
			}
			test[index][position].Intensity1 = intensity;
		}

		public void SetTestSignalOutput( int testType, int position, SignalOutputEnum signalOutput, int index )
		{
			SignalTest[] test = GetTest( testType );
			if( test[index][position] == null )
			{
				test[index][position] = new Test();
			}
			test[index][position].SignalOutput = signalOutput;
		}

		public void SetTestStatus( int testType, int position, PointStatusEnum status, int index )
		{
			SignalTest[] test = GetTest( testType );
			if( test[index][position] == null )
			{
				test[index][position] = new Test();
			}
			test[index][position].Status = (PointStatusEnum)status;
		}

		#endregion TestResults Members

		#region Private Members

		private SignalTest[] mAcmTest;
		private SignalTest[] mAcTest;
		private SignalTest[] mArtTest;
		private SignalTest[] mBcmTest;
		private SignalTest[] mBcTest;
		private SignalTest[] mIpTest;
		private SignalTest[] mMclTest;
		private SignalTest[] mSfaTest;
		private SignalTest[] mSfTest;
		private SignalTest[] mUclTest;

		#endregion Private Members

	}
}
