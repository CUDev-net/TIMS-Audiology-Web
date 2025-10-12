using System.Collections.Generic;
using System.IO;

using TIMS_X.Server.Legacy.Noah.Enums;
using TIMS_X.Server.Legacy.Noah.Speech;

namespace TIMS_X.Server.Legacy.Noah
{
	/// <summary>
	/// Internal class for speech results
	/// </summary>
	public class SpeechResult
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public SpeechResult()
		{
			DB = AbmConst.UNDEFINED;
			Mask = AbmConst.UNDEFINED;
			Percent = AbmConst.UNDEFINED;
			NumberOfWords = AbmConst.UNDEFINED;
		}

		#endregion Constructors

		#region SpeechResult Members

		public int DB
		{
			get;
			set;
		}

		public int Mask
		{
			get;
			set;
		}

		public int Percent
		{
			get;
			set;
		}

		public int NumberOfWords
		{
			get;
			set;
		}


		#endregion SpeechResult Members

	}


	/// <summary>
	/// Noah 200 Audiogram test results
	/// </summary>
	public class N200 : IPureToneAudioSupport, ISpeechSupport
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="src">Memory stream</param>
		public N200( MemoryStream src )
		{
			PureToneData = new PureToneData();

			mThreshodToneAudiogram = new THRAudiogram[18];
			mMCLToneAudiogram = new THRAudiogram[6];
			mUCLToneAudiogram = new THRAudiogram[6];
			for( int idx = 0; idx < 18; idx++ )
			{
				mThreshodToneAudiogram[idx] = new THRAudiogram();
				mThreshodToneAudiogram[idx].SetData( src );
			}
			for( int idx = 0; idx < 6; idx++ )
			{
				mMCLToneAudiogram[idx] = new THRAudiogram();
				mMCLToneAudiogram[idx].SetData( src );
			}
			for( int idx = 0; idx < 6; idx++ )
			{
				mUCLToneAudiogram[idx] = new THRAudiogram();
				mUCLToneAudiogram[idx].SetData( src );
			}

			src.Position = 20624;
			DLAudiograms = new List<DLAudiogram>( 12 );
			WR1Binural = new SpeechResult();
			WR1Left = new SpeechResult();
			WR1Right = new SpeechResult();
			WR2Binural = new SpeechResult();
			WR2Left = new SpeechResult();
			WR2Right = new SpeechResult();
			WR1BinuralAided = new SpeechResult();
			WR1LeftAided = new SpeechResult();
			WR1RightAided = new SpeechResult();
			WR2BinuralAided = new SpeechResult();
			WR2LeftAided = new SpeechResult();
			WR2RightAided = new SpeechResult();

			for( int idx = 0; idx < 12; idx++ )
			{
				var dlAudiogram = new DLAudiogram();
				dlAudiogram.SetData( src );
				_ProcessWRAudiogram( dlAudiogram );
				DLAudiograms.Add( dlAudiogram );
			}

			SRTAudiograms = new List<SRTAudiogram>( 12 );
			for( int idx = 0; idx < 12; idx++ )
			{
				var audiogram = new SRTAudiogram();
				audiogram.SetData( src );
				_ProcessSRTAudiogram( audiogram );
				SRTAudiograms.Add( audiogram );
			}

			MCLAudiograms = new List<MCLAudiogram>( 12 );
			for( int idx = 0; idx < 12; idx++ )
			{
				var audiogram = new MCLAudiogram();
				audiogram.SetData( src );
				_ProcessMCLAudiogram( audiogram );
				MCLAudiograms.Add( audiogram );
			}

			UCLAudiograms = new List<UCLAudiogram>( 12 );
			for( int idx = 0; idx < 12; idx++ )
			{
				var audiogram = new UCLAudiogram();
				audiogram.SetData( src );
				_ProcessUCLAudiogram( audiogram );
				UCLAudiograms.Add( audiogram );
			}

			// Get device data
			src.Position = 35312;
			MeasurementNotes = new MeasurementNotes();
			MeasurementNotes.SetData( src );
		}

		#endregion Constructors

		#region N200 Members

		/// <summary>
		/// Gets the DLAudiograms
		/// </summary>
		public List<DLAudiogram> DLAudiograms
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the MCLAudiograms
		/// </summary>
		public List<MCLAudiogram> MCLAudiograms
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets MCL results for binural
		/// </summary>
		public SpeechResult MCLBinural
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets MCL results for left
		/// </summary>
		public SpeechResult MCLLeft
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets MCL results for right
		/// </summary>
		public SpeechResult MCLRight
		{
			get;
			private set;
		}

		/// <summary>
		/// MCL Tone
		/// </summary>
		public THRAudiogram[] MCLToneAudiogram
		{

			get
			{
				return mMCLToneAudiogram;
			}
		}

		/// <summary>
		/// Gets the device notes
		/// </summary>
		public MeasurementNotes MeasurementNotes
		{
			get;
			private set;
		}

		/// <summary>
		/// Pure Tone data for this audiogram
		/// </summary>
		public PureToneData PureToneData
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the SRTAudiogramS
		/// </summary>
		public List<SRTAudiogram> SRTAudiograms
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets SRT results for Binural
		/// </summary>
		public SpeechResult SRTBinural
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets SRT results for left
		/// </summary>
		public SpeechResult SRTLeft
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets SRT results for right
		/// </summary>
		public SpeechResult SRTRight
		{
			get;
			private set;
		}

		/// <summary>
		/// Threshold Tone
		/// </summary>
		public THRAudiogram[] ThreshodToneAudiogram
		{

			get
			{
				return mThreshodToneAudiogram;
			}
		}

		/// <summary>
		/// Gets the UCLAudiograms
		/// </summary>
		public List<UCLAudiogram> UCLAudiograms
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets MCL results for binural
		/// </summary>
		public SpeechResult UCLBinural
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets MCL results for left
		/// </summary>
		public SpeechResult UCLLeft
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets MCL results for right
		/// </summary>
		public SpeechResult UCLRight
		{
			get;
			private set;
		}

		/// <summary>
		/// UCL Tone
		/// </summary>
		public THRAudiogram[] UCLToneAudiogram
		{

			get
			{
				return mUCLToneAudiogram;
			}
		}

		/// <summary>
		/// Gets results for WR1 Binural
		/// </summary>
		public SpeechResult WR1Binural
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Left
		/// </summary>
		public SpeechResult WR1Left
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Right
		/// </summary>
		public SpeechResult WR1Right
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Binural
		/// </summary>
		public SpeechResult WR2Binural
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Left
		/// </summary>
		public SpeechResult WR2Left
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Right
		/// </summary>
		public SpeechResult WR2Right
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Binural
		/// </summary>
		public SpeechResult WR1BinuralAided
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Left
		/// </summary>
		public SpeechResult WR1LeftAided
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Right
		/// </summary>
		public SpeechResult WR1RightAided
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Binural
		/// </summary>
		public SpeechResult WR2BinuralAided
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Left
		/// </summary>
		public SpeechResult WR2LeftAided
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Right
		/// </summary>
		public SpeechResult WR2RightAided
		{
			get;
			private set;
		}


		/// <summary>
		/// Reads the results into result object
		/// </summary>
		public AudiogramTestResults GetResults()
		{
			// Always create a new result set
			mAudiogramResults = new AudiogramTestResults();

			for( int testIndex = 0; testIndex < AbmConst.NUMBER_THRAUDIOGRAMS; testIndex++ )
			{
				TestSide side = _GetSide( testIndex, 0 );
				if( side != TestSide.Unknown )
				{
					int off = AbmConst.ART;
					int test = _GetTest( testIndex );
					if( test == AbmConst.AC || test == AbmConst.IP )
					{
						off = AbmConst.ACM;
					}
					else if( test == AbmConst.BC )
					{
						off = AbmConst.BCM;
					}
					else if( test == AbmConst.SF )
					{
						if( mThreshodToneAudiogram[testIndex].Condition1 == HearingTestConditionEnum.Aided2 ||
							mThreshodToneAudiogram[testIndex].Condition1 == HearingTestConditionEnum.Aided1 )
						{
							test = AbmConst.SFA;
						}
					}
					if( test != AbmConst.ART )
					{
						_AudiogramToResultsRow( side, test, testIndex, off, AudiogramTypeEnum.Threshold );
					}
				}
				if( testIndex < AbmConst.NUMBER_UCL_MCLAUDIOGRAMS )
				{
					TestSide clside = _GetSide( testIndex, AbmConst.MCL );
					if( clside != TestSide.Unknown )
					{
						_AudiogramToResultsRow( clside, AbmConst.MCL, testIndex, 0, AudiogramTypeEnum.MCL );
					}

					clside = _GetSide( testIndex, AbmConst.UCL );
					if( clside != TestSide.Unknown )
					{
						_AudiogramToResultsRow( clside, AbmConst.UCL, testIndex, 0, AudiogramTypeEnum.UCL );
					}
				}
			}

			return mAudiogramResults;
		}

		#endregion N200 Members

		#region Private Members

		private THRAudiogram[] mMCLToneAudiogram;
		private THRAudiogram[] mThreshodToneAudiogram;
		private THRAudiogram[] mUCLToneAudiogram;
		private AudiogramTestResults mAudiogramResults;

		/// <summary>
		/// Writes the audigram results from the memory stream to the results object
		/// </summary>
		/// <param name="side">Side the test results are for</param>
		/// <param name="test">Test</param>
		/// <param name="idx">Index</param>
		/// <param name="off">Offset</param>
		/// <param name="which">Which audio gram test results this if for</param>
		private void _AudiogramToResultsRow( TestSide side, int test, int idx, int off, AudiogramTypeEnum which )
		{
			for( int pointIndex = 0; pointIndex < AbmConst.NUMBER_AUDIOGRAM_POINTS; pointIndex++ )
			{
				int freq = -1;
				switch( which )
				{
					case AudiogramTypeEnum.Threshold:
						freq = mThreshodToneAudiogram[idx].TPFreq1( pointIndex );
						break;
					case AudiogramTypeEnum.MCL:
						freq = mMCLToneAudiogram[idx].TPFreq1( pointIndex );
						break;
					case AudiogramTypeEnum.UCL:
						freq = mUCLToneAudiogram[idx].TPFreq1( pointIndex );
						break;
				}

				int frequencyPosition = AbmConst.NOTMEASURED;
				for( int frequencyIndex = 0; frequencyIndex < AbmConst.FrequencyTable.Length; frequencyIndex++ )
				{
					if( AbmConst.FrequencyTable[frequencyIndex] == freq )
					{
						frequencyPosition = frequencyIndex;
						break;
					}
				}

				if( frequencyPosition > -1 )
				{
					switch( which )
					{
						case AudiogramTypeEnum.Threshold:
							// Condition is needed to determine which SFA test this is
							mAudiogramResults.GetTestResults( side ).SetTestCondition1( test, frequencyPosition, mThreshodToneAudiogram[idx].Condition1, idx );

							mAudiogramResults.GetTestResults( side ).SetTestIntensity( test, frequencyPosition, mThreshodToneAudiogram[idx].TPIntensity1( pointIndex ), idx );
							mAudiogramResults.GetTestResults( side ).SetTestStatus( test, frequencyPosition, mThreshodToneAudiogram[idx].TPStatus( pointIndex ), idx );
							break;
						case AudiogramTypeEnum.MCL:
							mAudiogramResults.GetTestResults( side ).SetTestSignalOutput( test, frequencyPosition, mMCLToneAudiogram[idx].SignalOutput1, idx );
							mAudiogramResults.GetTestResults( side ).SetTestIntensity( test, frequencyPosition, mMCLToneAudiogram[idx].TPIntensity1( pointIndex ), idx );
							mAudiogramResults.GetTestResults( side ).SetTestStatus( test, frequencyPosition, mMCLToneAudiogram[idx].TPStatus( pointIndex ), idx );
							break;
						case AudiogramTypeEnum.UCL:
							mAudiogramResults.GetTestResults( side ).SetTestSignalOutput( test, frequencyPosition, mUCLToneAudiogram[idx].SignalOutput1, idx );
							mAudiogramResults.GetTestResults( side ).SetTestIntensity( test, frequencyPosition, mUCLToneAudiogram[idx].TPIntensity1( pointIndex ), idx );
							mAudiogramResults.GetTestResults( side ).SetTestStatus( test, frequencyPosition, mUCLToneAudiogram[idx].TPStatus( pointIndex ), idx );
							break;
					}

					if( test == AbmConst.AC || test == AbmConst.BC || test == AbmConst.IP )
					{
						if( mThreshodToneAudiogram[idx].TPIntensity2( pointIndex ) >= 0 &&
							 mThreshodToneAudiogram[idx].TPFreq2( pointIndex ) >= 0 )
						{
							mAudiogramResults.GetTestResults( side ).SetTestIntensity( off, frequencyPosition, test, idx );
							mAudiogramResults.GetTestResults( side ).SetTestStatus( off, frequencyPosition, mThreshodToneAudiogram[idx].TPStatus( pointIndex ), idx );
						}

						if( test == AbmConst.AC || test == AbmConst.IP )
						{
							var status = mThreshodToneAudiogram[idx].TPStatus( pointIndex );
							if( status != PointStatusEnum.NotMeasurable && status != PointStatusEnum.NoResponse )
							{
								PureToneData.SetPTAData( side, freq, mThreshodToneAudiogram[idx].TPIntensity1( pointIndex ) );
							}
						}
					}
				}
			}
		}

		private TestSide _GetSide( int idx, int which )
		{
			SignalOutputEnum signalOutput;
			TestSide results = TestSide.Unknown;
			switch( which )
			{
				case AbmConst.MCL:
					signalOutput = mMCLToneAudiogram[idx].SignalOutput1;
					break;
				case AbmConst.UCL:
					signalOutput = mUCLToneAudiogram[idx].SignalOutput1;
					break;
				default:
					signalOutput = mThreshodToneAudiogram[idx].SignalOutput1;
					break;
			}

			if( signalOutput == SignalOutputEnum.AirConductorRight ||
				signalOutput == SignalOutputEnum.BoneConductorRight ||
				signalOutput == SignalOutputEnum.FreeFieldRight ||
				signalOutput == SignalOutputEnum.InsertPhoneRight )
			{
				results = TestSide.Right;
			}
			if( signalOutput == SignalOutputEnum.AirConductorLeft ||
				signalOutput == SignalOutputEnum.BoneConductorLeft ||
				signalOutput == SignalOutputEnum.FreeFieldLeft ||
				signalOutput == SignalOutputEnum.InsertPhoneLeft )
			{
				results = TestSide.Left;
			}

			if( signalOutput == SignalOutputEnum.AirConductorBinaural ||
				signalOutput == SignalOutputEnum.BoneConductorBinaural ||
				signalOutput == SignalOutputEnum.FreeFieldBinaural ||
				signalOutput == SignalOutputEnum.InsertPhoneBinaural )
			{
				results = TestSide.Both;
			}
			return results;
		}

		private int _GetTest( int idx )
		{
			SignalOutputEnum sig = mThreshodToneAudiogram[idx].SignalOutput1;
			if( sig == SignalOutputEnum.AirConductorLeft || sig == SignalOutputEnum.AirConductorRight )
			{
				return AbmConst.AC;
			}
			if( sig == SignalOutputEnum.InsertPhoneRight || sig == SignalOutputEnum.InsertPhoneLeft || sig == SignalOutputEnum.InsertPhoneBinaural )
			{
				return AbmConst.IP;
			}
			else if( sig == SignalOutputEnum.BoneConductorLeft || sig == SignalOutputEnum.BoneConductorRight || sig == SignalOutputEnum.BoneConductorBinaural )
			{
				return AbmConst.BC;
			}
			else if( sig == SignalOutputEnum.FreeFieldLeft || sig == SignalOutputEnum.FreeFieldRight || sig == SignalOutputEnum.FreeFieldBinaural )
			{
				return AbmConst.SF;
			}
			else if( sig == SignalOutputEnum.NoSignalOutput )
			{
				return AbmConst.MCL;
			}
			return AbmConst.ART;
		}

		/// <summary>
		/// Processes MCL Speech audiogram
		/// </summary>
		/// <param name="audiogram"></param>
		private void _ProcessMCLAudiogram( MCLAudiogram audiogram )
		{
			// Right
			if( (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.AirConductorRight) ||
				(audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.BoneConductorRight) ||
				(audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.FreeFieldRight) ||
				(audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.InsertPhoneRight) )
			{
				MCLRight = new SpeechResult();
				if( audiogram.Curve[0].Intensity1 != AbmConst.UNDEFINED )
				{
					MCLRight.DB = audiogram.Curve[0].Intensity1 / 10;
				}
				if( audiogram.Curve[0].Intensity2 != AbmConst.UNDEFINED )
				{
					MCLRight.Mask = audiogram.Curve[0].Intensity2 / 10;
				}
			}
			// Left
			else if( (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.AirConductorLeft) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.BoneConductorLeft) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.FreeFieldLeft) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.InsertPhoneLeft) )
			{
				MCLLeft = new SpeechResult();
				if( audiogram.Curve[0].Intensity1 != AbmConst.UNDEFINED )
				{
					MCLLeft.DB = audiogram.Curve[0].Intensity1 / 10;
				}
				if( audiogram.Curve[0].Intensity2 != AbmConst.UNDEFINED )
				{
					MCLLeft.Mask = audiogram.Curve[0].Intensity2 / 10;
				}
			}
			// Binural
			else if( (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.AirConductorBinaural) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.BoneConductorBinaural) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.FreeFieldBinaural) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.InsertPhoneBinaural) )
			{
				MCLBinural = new SpeechResult();
				if( audiogram.Curve[0].Intensity1 != AbmConst.UNDEFINED )
				{
					MCLBinural.DB = audiogram.Curve[0].Intensity1 / 10;
				}
				if( audiogram.Curve[0].Intensity2 != AbmConst.UNDEFINED )
				{
					MCLBinural.Mask = audiogram.Curve[0].Intensity2 / 10;
				}
			}
		}

		/// <summary>
		/// Processes SRT audiogram
		/// </summary>
		/// <param name="audiogram"></param>
		private void _ProcessSRTAudiogram( SRTAudiogram audiogram )
		{
			// Right
			if( (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.AirConductorRight) ||
				(audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.BoneConductorRight) ||
				(audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.FreeFieldRight) ||
				(audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.InsertPhoneRight) )
			{
				SRTRight = new SpeechResult();
				if( audiogram.Curve[0].Intensity1 != AbmConst.UNDEFINED )
				{
					SRTRight.DB = audiogram.Curve[0].Intensity1 / 10;
				}
				if( audiogram.Curve[0].Intensity2 != AbmConst.UNDEFINED )
				{
					SRTRight.Mask = audiogram.Curve[0].Intensity2 / 10;
				}
			}
			// Left
			else if( (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.AirConductorLeft) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.BoneConductorLeft) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.FreeFieldLeft) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.InsertPhoneLeft) )
			{
				SRTLeft = new SpeechResult();
				if( audiogram.Curve[0].Intensity1 != AbmConst.UNDEFINED )
				{
					SRTLeft.DB = audiogram.Curve[0].Intensity1 / 10;
				}
				if( audiogram.Curve[0].Intensity2 != AbmConst.UNDEFINED )
				{
					SRTLeft.Mask = audiogram.Curve[0].Intensity2 / 10;
				}
			}
			// Binural
			else if( (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.AirConductorBinaural) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.BoneConductorBinaural) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.FreeFieldBinaural) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.InsertPhoneBinaural) )
			{
				SRTBinural = new SpeechResult();
				if( audiogram.Curve[0].Intensity1 != AbmConst.UNDEFINED )
				{
					SRTBinural.DB = audiogram.Curve[0].Intensity1 / 10;
				}
				if( audiogram.Curve[0].Intensity2 != AbmConst.UNDEFINED )
				{
					SRTBinural.Mask = audiogram.Curve[0].Intensity2 / 10;
				}
			}
		}

		/// <summary>
		/// Processes UCL Speech Audiogram
		/// </summary>
		/// <param name="audiogram"></param>
		private void _ProcessUCLAudiogram( UCLAudiogram audiogram )
		{
			// Right
			if( (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.AirConductorRight) ||
				(audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.BoneConductorRight) ||
				(audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.FreeFieldRight) ||
				(audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.InsertPhoneRight) )
			{
				UCLRight = new SpeechResult();
				if( audiogram.Curve[0].Intensity1 != AbmConst.UNDEFINED )
				{
					UCLRight.DB = audiogram.Curve[0].Intensity1 / 10;
				}
				if( audiogram.Curve[0].Intensity2 != AbmConst.UNDEFINED )
				{
					UCLRight.Mask = audiogram.Curve[0].Intensity2 / 10;
				}
			}
			// Left
			else if( (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.AirConductorLeft) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.BoneConductorLeft) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.FreeFieldLeft) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.InsertPhoneLeft) )
			{
				UCLLeft = new SpeechResult();
				if( audiogram.Curve[0].Intensity1 != AbmConst.UNDEFINED )
				{
					UCLLeft.DB = audiogram.Curve[0].Intensity1 / 10;
				}
				if( audiogram.Curve[0].Intensity2 != AbmConst.UNDEFINED )
				{
					UCLLeft.Mask = audiogram.Curve[0].Intensity2 / 10;
				}
			}
			// Binural
			else if( (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.AirConductorBinaural) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.BoneConductorBinaural) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.FreeFieldBinaural) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.InsertPhoneBinaural) )
			{
				UCLBinural = new SpeechResult();
				if( audiogram.Curve[0].Intensity1 != AbmConst.UNDEFINED )
				{
					UCLBinural.DB = audiogram.Curve[0].Intensity1 / 10;
				}
				if( audiogram.Curve[0].Intensity2 != AbmConst.UNDEFINED )
				{
					UCLBinural.Mask = audiogram.Curve[0].Intensity2 / 10;
				}
			}
		}

		/// <summary>
		/// Process word recognition audiogram
		/// </summary>
		/// <param name="audiogram"></param>
		private void _ProcessWRAudiogram( DLAudiogram audiogram )
		{
			// Right
			if( (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.AirConductorRight) ||
				(audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.BoneConductorRight) ||
				(audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.FreeFieldRight) ||
				(audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.InsertPhoneRight) )
			{
				if( audiogram.MeasuringCondition.RSignalType2 != SignalTypeEnum.SpeechNoise )
				{
					if( audiogram.Curve[0].Intensity1 != AbmConst.UNDEFINED )
					{
						WR1Right.DB = audiogram.Curve[0].Intensity1 / 10;
					}
					if( audiogram.Curve[0].Intensity2 != AbmConst.UNDEFINED )
					{
						WR1Right.Mask = audiogram.Curve[0].Intensity2 / 10;
					}
					if( audiogram.Curve[0].ScorePercent != AbmConst.UNDEFINED )
					{
						WR1Right.Percent = audiogram.Curve[0].ScorePercent / 100;
					}
				}
				else
				{
					if( audiogram.Curve[0].Intensity1 != AbmConst.UNDEFINED )
					{
						WR2Right.DB = audiogram.Curve[0].Intensity1 / 10;
					}
					if( audiogram.Curve[0].Intensity2 != AbmConst.UNDEFINED )
					{
						WR2Right.Mask = audiogram.Curve[0].Intensity2 / 10;
					}
					if( audiogram.Curve[0].ScorePercent != AbmConst.UNDEFINED )
					{
						WR2Right.Percent = audiogram.Curve[0].ScorePercent / 100;
					}
				}
			}
			// Left
			else if( (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.AirConductorLeft) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.BoneConductorLeft) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.FreeFieldLeft) ||
					 (audiogram.MeasuringCondition.RSignalOutput1 == SignalOutputEnum.InsertPhoneLeft) )
			{
				if( audiogram.MeasuringCondition.RSignalType2 != SignalTypeEnum.SpeechNoise )
				{
					if( audiogram.Curve[0].Intensity1 != AbmConst.UNDEFINED )
					{
						WR1Left.DB = audiogram.Curve[0].Intensity1 / 10;
					}
					if( audiogram.Curve[0].Intensity2 != AbmConst.UNDEFINED )
					{
						WR1Left.Mask = audiogram.Curve[0].Intensity2 / 10;
					}
					if( audiogram.Curve[0].ScorePercent != AbmConst.UNDEFINED )
					{
						WR1Left.Percent = audiogram.Curve[0].ScorePercent / 100;
					}
				}
				else
				{
					if( audiogram.Curve[0].Intensity1 != AbmConst.UNDEFINED )
					{
						WR2Left.DB = audiogram.Curve[0].Intensity1 / 10;
					}
					if( audiogram.Curve[0].Intensity2 != AbmConst.UNDEFINED )
					{
						WR2Left.Mask = audiogram.Curve[0].Intensity2 / 10;
					}
					if( audiogram.Curve[0].ScorePercent != AbmConst.UNDEFINED )
					{
						WR2Left.Percent = audiogram.Curve[0].ScorePercent / 100;
					}
				}
			}
			// Binural
			else
			{
				if( audiogram.MeasuringCondition.RSignalType2 != SignalTypeEnum.SpeechNoise )
				{
					if( audiogram.Curve[0].Intensity1 != AbmConst.UNDEFINED )
					{
						WR1Binural.DB = audiogram.Curve[0].Intensity1 / 10;
					}
					if( audiogram.Curve[0].Intensity2 != AbmConst.UNDEFINED )
					{
						WR1Binural.Mask = audiogram.Curve[0].Intensity2 / 10;
					}
					if( audiogram.Curve[0].ScorePercent != AbmConst.UNDEFINED )
					{
						WR1Binural.Percent = audiogram.Curve[0].ScorePercent / 100;
					}
				}
				else
				{
					if( audiogram.Curve[0].Intensity1 != AbmConst.UNDEFINED )
					{
						WR2Binural.DB = audiogram.Curve[0].Intensity1 / 10;
					}
					if( audiogram.Curve[0].Intensity2 != AbmConst.UNDEFINED )
					{
						WR2Binural.Mask = audiogram.Curve[0].Intensity2 / 10;
					}
					if( audiogram.Curve[0].ScorePercent != AbmConst.UNDEFINED )
					{
						WR2Binural.Percent = audiogram.Curve[0].ScorePercent / 100;
					}
				}
			}
		}

		#endregion Private Members

	}
}
