using System.IO;

using TIMS_X.Server.Legacy.Noah.Enums;

namespace TIMS_X.Server.Legacy.Noah
{
	/// <summary>
	/// Class for storing the audiogram results for the N100 tests
	/// </summary>
	public class N100
	{
		#region Constructors

		/// <summary>
		/// Contructor
		/// </summary>
		/// <param name="src">Noah stream with results</param>
		public N100( MemoryStream src )
		{
			mToneTHRAudiogram = new THRAudiogram100[6];
			mToneMCLAudiogram = new THRAudiogram100[6];
			mToneUCLAudiogram = new THRAudiogram100[6];
			for( int idx = 0; idx < 6; idx++ )
			{
				mToneTHRAudiogram[idx] = new THRAudiogram100();
				mToneTHRAudiogram[idx].SetData( src );
			}
			for( int idx = 0; idx < 6; idx++ )
			{
				mToneMCLAudiogram[idx] = new THRAudiogram100();
				mToneMCLAudiogram[idx].SetData( src );
			}
			for( int idx = 0; idx < 6; idx++ )
			{
				mToneUCLAudiogram[idx] = new THRAudiogram100();
				mToneUCLAudiogram[idx].SetData( src );
			}
		}

		#endregion Constructors

		#region N100 Members

		/// <summary>
		/// MCL Tone audiograms
		/// </summary>
		public THRAudiogram100[] MCLToneAudiogram
		{
			get
			{
				return mToneMCLAudiogram;
			}
		}

		/// <summary>
		/// Threshold Tone audiograms
		/// </summary>
		public THRAudiogram100[] ThreshodToneAudiogram
		{
			get
			{
				return mToneTHRAudiogram;
			}
		}

		/// <summary>
		/// UCL Tone audiograms
		/// </summary>
		public THRAudiogram100[] UCLToneAudiogram
		{
			get
			{
				return mToneUCLAudiogram;
			}
		}

		/// <summary>
		/// Gets the audiogram test results from the memory stream
		/// </summary>
		/// <returns>Audiogram test results</returns>
		public AudiogramTestResults GetResults()
		{
			// Always create a new result set
			mAudiogramResults = new AudiogramTestResults();

			for( int i = 0; i < 6; i++ )
			{
				TestSide side = _GetSide( i, 0 );

				if( side != TestSide.Unknown )
				{
					int off = AbmConst.ART;
					int t = _GetTest( i );
					if( t == AbmConst.AC )
					{
						off = AbmConst.ACM;
					}
					else if( t == AbmConst.BC )
					{
						off = AbmConst.BCM;
					}
					_WriteAudiogramToResults( side, t, i, off, 0 );
				}
				if( i < 6 )
				{
					TestSide clside = _GetSide( i, AbmConst.MCL );
					if( clside != TestSide.Unknown )
					{
						_WriteAudiogramToResults( clside, AbmConst.MCL, i, 0, 1 );
					}

					clside = _GetSide( i, AbmConst.UCL );
					if( clside != TestSide.Unknown )
					{
						_WriteAudiogramToResults( clside, AbmConst.UCL, i, 0, 2 );
					}
				}
			}

			return mAudiogramResults;
		}

		#endregion N100 Members

		#region Private Members

		private THRAudiogram100[] mToneMCLAudiogram;
		private THRAudiogram100[] mToneTHRAudiogram;
		private THRAudiogram100[] mToneUCLAudiogram;
		private AudiogramTestResults mAudiogramResults;

		private TestSide _GetSide( int idx, int which )
		{
			SignalOutputEnum signalOutput = (SignalOutputEnum)mToneTHRAudiogram[idx].SignalOutput1;
			TestSide results = TestSide.Unknown;
			switch( which )
			{
				case AbmConst.MCL:
					signalOutput = (SignalOutputEnum)mToneMCLAudiogram[idx].SignalOutput1;
					break;
				case AbmConst.UCL:
					signalOutput = (SignalOutputEnum)mToneUCLAudiogram[idx].SignalOutput1;
					break;
				default:
					signalOutput = (SignalOutputEnum)mToneTHRAudiogram[idx].SignalOutput1;
					break;
			}

			if( signalOutput == SignalOutputEnum.AirConductorRight ||
				 signalOutput == SignalOutputEnum.BoneConductorRight /*||
				 signalOutput == SignalOutputEnum.so_FFR ||
				 signalOutput == SignalOutputEnum.so_IPR*/ )
			{
				results = TestSide.Right;
			}
			else if( signalOutput == SignalOutputEnum.AirConductorLeft ||
						signalOutput == SignalOutputEnum.BoneConductorLeft /*||
						signalOutput == SignalOutputEnum.so_FFL ||
						signalOutput == SignalOutputEnum.so_IPL */)
			{
				results = TestSide.Left;
			}
			if( (signalOutput == SignalOutputEnum.BoneConductorBinaural) ||
				(signalOutput == SignalOutputEnum.FreeFieldBinaural) /*||
				(signalOutput == SignalOutputEnum.so_ACBin) ||
				(signalOutput == SignalOutputEnum.so_IPBin) */)
			{
				results = TestSide.Both;
			}

			return results;
		}

		/// <summary>
		/// Gets the test type
		/// </summary>
		/// <param name="idx">Index</param>
		/// <returns>Test type</returns>
		private int _GetTest( int idx )
		{
			SignalOutputEnum sig = (SignalOutputEnum)mToneTHRAudiogram[idx].SignalOutput1;
			if( sig == SignalOutputEnum.AirConductorLeft || sig == SignalOutputEnum.AirConductorRight || sig == SignalOutputEnum.AirConductorBinaural )
			{
				return AbmConst.AC;
			}
			else if( sig == SignalOutputEnum.BoneConductorLeft || sig == SignalOutputEnum.BoneConductorRight || sig == SignalOutputEnum.BoneConductorBinaural )
			{
				return AbmConst.BC;
			}
			else if( sig == SignalOutputEnum.FreeFieldLeft || sig == SignalOutputEnum.FreeFieldRight || sig == SignalOutputEnum.FreeFieldBinaural )
			{
				return AbmConst.UCL;
			}
			else if( sig == SignalOutputEnum.NoSignalOutput )
			{
				return AbmConst.MCL;
			}
			return AbmConst.AC;
		}

		/// <summary>
		/// Writes the audiogram to the results object
		/// </summary>
		/// <param name="side">Which side the results are for</param>
		/// <param name="test">Test</param>
		/// <param name="idx">Index</param>
		/// <param name="off">Offset</param>
		/// <param name="which">Which type of test results to store this in</param>
		private void _WriteAudiogramToResults( TestSide side, int test, int idx, int off, int which )
		{
			for( int ndx = 0; ndx < 24; ndx++ )
			{
				int freq = -1;
				if( which == 0 )
				{
					freq = mToneTHRAudiogram[idx].TPFreq1( ndx );

				}
				else if( which == 1 )
				{
					freq = mToneMCLAudiogram[idx].TPFreq1( ndx );

				}
				else if( which == 2 )
				{
					freq = mToneUCLAudiogram[idx].TPFreq1( ndx );

				}
				int pos = AbmConst.NOTMEASURED;
				for( int fdx = 0; fdx < 19; fdx++ )
				{
					if( AbmConst.FrequencyTable[fdx] == freq )
					{
						pos = fdx;
						break;
					}
				}

				if( pos > -1 )
				{
					if( which == 0 )
					{
						mAudiogramResults.GetTestResults( side ).SetTestIntensity( test, pos, mToneTHRAudiogram[idx].TPIntensity1( ndx ), idx );
						mAudiogramResults.GetTestResults( side ).SetTestStatus( test, pos, mToneTHRAudiogram[idx].TPStatus( ndx ), idx );
					}
					else if( which == 1 )
					{
						mAudiogramResults.GetTestResults( side ).SetTestIntensity( test, pos, mToneMCLAudiogram[idx].TPIntensity1( ndx ), idx );
						mAudiogramResults.GetTestResults( side ).SetTestStatus( test, pos, mToneMCLAudiogram[idx].TPStatus( ndx ), idx );
					}
					else if( which == 2 )
					{
						mAudiogramResults.GetTestResults( side ).SetTestIntensity( test, pos, mToneUCLAudiogram[idx].TPIntensity1( ndx ), idx );
						mAudiogramResults.GetTestResults( side ).SetTestStatus( test, pos, mToneUCLAudiogram[idx].TPStatus( ndx ), idx );
					}

					if( test == AbmConst.AC || test == AbmConst.BC )
					{
						if( mToneTHRAudiogram[idx].TPIntensity2( ndx ) > 0 ||
							 mToneTHRAudiogram[idx].TPFreq2( ndx ) > 0 )
						{
							mAudiogramResults.GetTestResults( side ).SetTestIntensity( off, pos, test, idx );
							mAudiogramResults.GetTestResults( side ).SetTestStatus( off, pos, mToneTHRAudiogram[idx].TPStatus( ndx ), idx );
						}
					}
				}
			}
		}

		#endregion Private Members

	}
}
