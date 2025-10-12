using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Legacy.Noah.Enums;
using TIMS_X.Server.Legacy.Noah.Format500.Enums;

namespace TIMS_X.Server.Legacy.Noah.Format500
{
	public class N500Audiogram : IPureToneAudioSupport, ISpeechSupport
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public N500Audiogram()
		{
			PureToneData = new PureToneData();

			ThreshodToneAudiogram = new List<Audiogram>();
			MCLToneAudiogram = new List<Audiogram>();
			UCLToneAudiogram = new List<Audiogram>();

			SpeechDiscriminationAudiogram = new List<SpeechAudiogram>();
			SpeechReceptionThresholdAudiogram = new List<SpeechAudiogram>();
			SpeechMostComfortableLevel = new List<SpeechAudiogram>();
			SpeechUncomfortableLevel = new List<SpeechAudiogram>();

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

		}

		#endregion Constructors

		#region N500Audiogram Members

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
		/// 
		/// </summary>
		public List<Audiogram> MCLToneAudiogram
		{
			get;
			private set;
		}

		/// <summary>
		/// Measurement notes
		/// </summary>
		public MeasurementNotes MeasurementNotes
		{
			get;
			set;
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
		/// Speech Discriminagion
		/// </summary>
		public List<SpeechAudiogram> SpeechDiscriminationAudiogram
		{
			get;
			private set;
		}

		/// <summary>
		/// Speech Discriminagion
		/// </summary>
		public List<SpeechAudiogram> SpeechMostComfortableLevel
		{
			get;
			private set;
		}

		/// <summary>
		/// Speech Discriminagion
		/// </summary>
		public List<SpeechAudiogram> SpeechReceptionThresholdAudiogram
		{
			get;
			private set;
		}

		/// <summary>
		/// Speech Discriminagion
		/// </summary>
		public List<SpeechAudiogram> SpeechUncomfortableLevel
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
		/// 
		/// </summary>
		public List<Audiogram> ThreshodToneAudiogram
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
		/// 
		/// </summary>
		public List<Audiogram> UCLToneAudiogram
		{
			get;
			private set;
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
		public List<SpeechResult> WR1BinuralList
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Left
		/// </summary>
		public List<SpeechResult> WR1LeftList
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Right
		/// </summary>
		public List<SpeechResult> WR1RightList
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Binural
		/// </summary>
		public List<SpeechResult> WR1BinuralAidedList
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Left
		/// </summary>
		public List<SpeechResult> WR1LeftAidedList
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Right
		/// </summary>
		public List<SpeechResult> WR1RightAidedList
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
		public List<SpeechResult> WR2BinuralList
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Left
		/// </summary>
		public List<SpeechResult> WR2LeftList
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Right
		/// </summary>
		public List<SpeechResult> WR2RightList
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
		/// Gets results for WR1 Binural
		/// </summary>
		public List<SpeechResult> WR2BinuralAidedList
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Left
		/// </summary>
		public List<SpeechResult> WR2LeftAidedList
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets results for WR1 Right
		/// </summary>
		public List<SpeechResult> WR2RightAidedList
		{
			get;
			private set;
		}



		/// <summary>
		/// Creates an element with the namespace
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static XElement CreateElementWithNamespace( string name, string value = null )
		{
			return value == null ? new XElement( NAMESPACE + name ) : new XElement( NAMESPACE + name, value );
		}

		/// <summary>
		/// Reads the results into result object
		/// </summary>
		public AudiogramTestResults GetResults()
		{
			// Always create a new result set
			mAudiogramResults = new AudiogramTestResults();
			_GetThresholdAudiogramResults();
			_GetMCLAudiogramResults();
			_GetUCLAudiogramResults();

			return mAudiogramResults;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="n500Xml"></param>
		public void Load( string n500Xml )
		{
			var xDocument = XDocument.Parse( n500Xml.Replace(((char)0x10).ToString(), string.Empty));

			// Build Audiogram for all normal type points.
			var xTHRnodes = xDocument.Root.Elements().Where( x => x.Name.LocalName == Constants.XML_TONETHRESHOLDAUDIOGRAM );
			_BuildAudiograms( xTHRnodes, ThreshodToneAudiogram );

			// Build Audiogram for MCL (Most Comfortable).
			var xMCLnodes = xDocument.Root.Elements().Where( x => x.Name.LocalName == Constants.XML_MOSTCOMFORTABLELEVEL );
			_BuildAudiograms( xMCLnodes, MCLToneAudiogram );

			// Build Audiogram for UCL (Most Uncomfortable).
			var xUCLnodes = xDocument.Root.Elements().Where( x => x.Name.LocalName == Constants.XML_UNCOMFORTABLELEVEL );
			_BuildAudiograms( xUCLnodes, UCLToneAudiogram );

			// Word Recognition
			var xspdiscnodes = xDocument.Root.Elements().Where( x => x.Name.LocalName == Constants.XML_SPEECHDISCRIMINATIONAUDIOGRAM );
			_BuildSpeechAudiograms( xspdiscnodes, Constants.XML_SPEECHDISCRIMINATIONPOINTS, SpeechDiscriminationAudiogram );
			_ProcessWordRecognitionAudiogram( SpeechDiscriminationAudiogram );

			// Speech
			var xsprthnodes = xDocument.Root.Elements().Where( x => x.Name.LocalName == Constants.XML_SPEECHRECEPTIONTHRESHOLDAUDIOGRAM );
			_BuildSpeechAudiograms( xsprthnodes, Constants.XML_SPEECHRECEPTIONPOINTS, SpeechReceptionThresholdAudiogram );

			var xspmclnodes = xDocument.Root.Elements().Where( x => x.Name.LocalName == Constants.XML_SPEECHMOSTCOMFORTABLELEVEL );
			_BuildSpeechAudiograms( xspmclnodes, Constants.XML_SPEECHMOSTCOMFORTABLEPOINT, SpeechMostComfortableLevel );

			var xspuclnodes = xDocument.Root.Elements().Where( x => x.Name.LocalName == Constants.XML_SPEECHUNCOMFORTABLELEVEL );
			_BuildSpeechAudiograms( xspuclnodes, Constants.XML_SPEECHUNCOMFORTABLEPOINT, SpeechUncomfortableLevel );

			_ProcessSpeechAudiogram( SpeechReceptionThresholdAudiogram, SRTSPEECH );
			_ProcessSpeechAudiogram( SpeechMostComfortableLevel, MCLSPEECH );
			_ProcessSpeechAudiogram( SpeechUncomfortableLevel, UCLSPEECH );
		}

		/// <summary>
		/// Serialize to XML string
		/// </summary>
		/// <returns></returns>
		public XElement Serialize()
		{
			var root = CreateElementWithNamespace( Constants.XML_HIMSAAUDIOMETRICSTANDARD );
			root.Add( new XAttribute( "ValidatedByNOAH", "false" ) );
			root.Add( new XAttribute( "Version", "500" ) );

			if( ThreshodToneAudiogram.Count > 0 )
			{
				foreach( var audiogram in ThreshodToneAudiogram )
				{
					root.Add( audiogram.Serialize( Constants.XML_TONETHRESHOLDAUDIOGRAM ) );
				}
			}
			if( MCLToneAudiogram.Count > 0 )
			{
				foreach( var audiogram in MCLToneAudiogram )
				{
					root.Add( audiogram.Serialize( Constants.XML_MOSTCOMFORTABLELEVEL ) );
				}
			}
			if( UCLToneAudiogram.Count > 0 )
			{
				foreach( var audiogram in UCLToneAudiogram )
				{
					root.Add( audiogram.Serialize( Constants.XML_UNCOMFORTABLELEVEL ) );
				}
			}
			if( MeasurementNotes != null )
			{
				root.Add( MeasurementNotes.Serialize() );
			}
			return root; //.ToString( SaveOptions.DisableFormatting );
		}

		#endregion N500Audiogram Members

		#region Private Members

		private const string MCLSPEECH = "MCL";
		private const string SRTSPEECH = "SRT";
		private const string UCLSPEECH = "UCL";
		private static XNamespace NAMESPACE = "http://www.himsa.com/Measurement/Audiogram";
		private AudiogramTestResults mAudiogramResults;

		/// <summary>
		/// Builds audiogram
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="audiograms"></param>
		private void _BuildAudiograms( IEnumerable<XElement> nodes, List<Audiogram> audiograms )
		{
			foreach( var xElement in nodes )
			{
				var audiogram = new Audiogram();
				// Nodes in the THR 
				foreach( var element in xElement.Elements() )
				{
					if( element.Name.LocalName == Constants.XML_AUDMEASUREMENTCONDITIONS )
					{
						_ParseMeasurementCondition( element, audiogram );
					}
					if( element.Name.LocalName == Constants.XML_TONEPOINTS )
					{
						var point = _BuildTonePoint( element );
						audiogram.Curve.Add( point );
					}
				}
				audiograms.Add( audiogram );
			}
		}

		/// <summary>
		/// Build speech audiograms
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="pointName"></param>
		/// <param name="speechAudiograms"></param>
		private void _BuildSpeechAudiograms( IEnumerable<XElement> nodes, string pointName, List<SpeechAudiogram> speechAudiograms )
		{
			foreach( var node in nodes )
			{
				var audiogram = new SpeechAudiogram();
				foreach( var element in node.Elements() )
				{
					if( element.Name.LocalName == Constants.XML_AUDMEASUREMENTCONDITIONS )
					{
						_ParseMeasurementCondition( element, audiogram );
					}
					if( element.Name.LocalName == pointName )
					{
						var point = _BuildSpeechScorePoint( element );
						audiogram.Points.Add( point );
					}
				}
				speechAudiograms.Add( audiogram );
			}
		}

		/// <summary>
		/// Fills the speech score point
		/// </summary>
		/// <param name="scorePoint"></param>
		/// <returns></returns>
		private SpeechResult _BuildSpeechPoint( SpeechScorePoint scorePoint )
		{
			var speechResult = new SpeechResult();
			if( scorePoint.StimulusLevel != SpeechScorePoint.UNDEFINED )
			{
				speechResult.DB = (int)scorePoint.StimulusLevel;
			}
			if( scorePoint.MaskingLevel != SpeechScorePoint.UNDEFINED )
			{
				speechResult.Mask = (int)scorePoint.MaskingLevel;
			}
			return speechResult;
		}

		/// <summary>
		/// Builds speech points
		/// </summary>
		/// <param name="xSpeechPoint"></param>
		/// <returns></returns>
		private SpeechScorePoint _BuildSpeechScorePoint( XElement xSpeechPoint )
		{
			var speechPoint = new SpeechScorePoint();
			foreach( var element in xSpeechPoint.Elements() )
			{
				decimal dTemp;
				int iTemp;
				switch( element.Name.LocalName )
				{
					case Constants.XML_STIMULUSLEVEL:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							speechPoint.StimulusLevel = dTemp;
						}
						break;
					case Constants.XML_MASKINGLEVEL:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							speechPoint.MaskingLevel = dTemp;
						}
						break;
					case Constants.XML_SCOREPERCENT:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							speechPoint.ScorePercent = dTemp;
						}
						break;
					case Constants.XML_NUMBEROFWORDS:
						if( int.TryParse( element.Value, out iTemp ) )
						{
							speechPoint.NumberOfWords = iTemp;
						}
						break;
				}
			}

			return speechPoint;
		}

		/// <summary>
		/// Builds the tone point from the node
		/// </summary>
		/// <param name="xTonePoint"></param>
		/// <returns></returns>
		private TonePoint _BuildTonePoint( XElement xTonePoint )
		{
			var tonePoint = new TonePoint();
			foreach( var element in xTonePoint.Elements() )
			{
				decimal dTemp;
				int iTemp;
				switch( element.Name.LocalName )
				{
					case Constants.XML_STIMULUSFREQUENCY:
						if( int.TryParse( element.Value, out iTemp ) )
						{
							tonePoint.StimulusFrequency = iTemp;
						}
						break;
					case Constants.XML_STIMULUSLEVEL:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							tonePoint.StimulusLevel = dTemp;
						}
						break;
					case Constants.XML_MASKINGFREQUENCY:
						if( int.TryParse( element.Value, out iTemp ) )
						{
							tonePoint.MaskingFrequency = iTemp;
						}
						break;
					case Constants.XML_MASKINGLEVEL:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							tonePoint.MaskingLevel = dTemp;
						}
						break;
					case Constants.XML_TONEPOINTSTATUS:
						tonePoint.TonePointStatus
							= EnumUtilities.Parse<PointStatusEnum>( element.Value );
						break;
				}
			}
			return tonePoint;
		}

		/// <summary>
		/// Builds a WR Point
		/// </summary>
		/// <param name="wordPoint"></param>
		/// <param name="scorePoint"></param>
		private void _BuildWordRecognitionPoint( SpeechResult wordPoint, SpeechScorePoint scorePoint )
		{
			if( scorePoint.StimulusLevel != SpeechScorePoint.UNDEFINED )
			{
				wordPoint.DB = (int)scorePoint.StimulusLevel;
			}
			if( scorePoint.MaskingLevel != SpeechScorePoint.UNDEFINED )
			{
				wordPoint.Mask = (int)scorePoint.MaskingLevel;
			}
			if( scorePoint.ScorePercent != SpeechScorePoint.UNDEFINED )
			{
				wordPoint.Percent = (int)scorePoint.ScorePercent;
			}
			if( scorePoint.NumberOfWords != SpeechScorePoint.UNDEFINED )
			{
				wordPoint.NumberOfWords = (int)scorePoint.NumberOfWords;
			}
		}

		private void _BuildWordRecognitionPointList( List<SpeechResult> wordPointList, List<SpeechScorePoint> scorePointList )
		{
			//wordPointList = new List<SpeechResult>();
			foreach( var sp in scorePointList )
			{
				var wordPoint = new SpeechResult();
				//wordPoint.DB = AbmConst.UNDEFINED;
				//wordPoint.Mask = AbmConst.UNDEFINED;
				//wordPoint.Percent = AbmConst.UNDEFINED;
				if( sp.StimulusLevel != SpeechScorePoint.UNDEFINED )
				{
					wordPoint.DB = (int)sp.StimulusLevel;
				}
				if( sp.MaskingLevel != SpeechScorePoint.UNDEFINED )
				{
					wordPoint.Mask = (int)sp.MaskingLevel;
				}
				if( sp.ScorePercent != SpeechScorePoint.UNDEFINED )
				{
					wordPoint.Percent = (int)sp.ScorePercent;
				}
				if( sp.NumberOfWords != SpeechScorePoint.UNDEFINED )
				{
					wordPoint.NumberOfWords = (int)sp.NumberOfWords;
				}

				wordPointList.Add(wordPoint);
			}
		
		}

		/// <summary>
		/// Gets the frequency position on the graph
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		private int _GetFrequencyPosition( int frequency )
		{
			int frequencyPosition = AbmConst.NOTMEASURED;
			for( int frequencyIndex = 0; frequencyIndex < AbmConst.FrequencyTable.Length; frequencyIndex++ )
			{
				if( AbmConst.FrequencyTable[frequencyIndex] == frequency )
				{
					frequencyPosition = frequencyIndex;
					break;
				}
			}
			return frequencyPosition;
		}

		/// <summary>
		/// Gets the threshold audiogram results
		/// </summary>
		private void _GetMCLAudiogramResults()
		{
			int testIndex = 0;
			foreach( var audiogram in MCLToneAudiogram )
			{
				TestSide side;
				int testType, maskTestType;
				_GetTestDetails( audiogram, out side, out testType, out maskTestType );
				foreach( var tonePoint in audiogram.Curve )
				{
					var frequency = tonePoint.StimulusFrequency;
					var frequencyPosition = _GetFrequencyPosition( frequency );
					if( frequencyPosition > -1 )
					{
						mAudiogramResults.GetTestResults( side ).SetTestSignalOutput( AbmConst.MCL, frequencyPosition, audiogram.MeasurementCondition.StimulusSignalOutput, testIndex );
						mAudiogramResults.GetTestResults( side ).SetTestIntensity( AbmConst.MCL, frequencyPosition, (int)tonePoint.StimulusLevel * 10, testIndex );
						mAudiogramResults.GetTestResults( side ).SetTestStatus( AbmConst.MCL, frequencyPosition, tonePoint.TonePointStatus, testIndex );
					}
				}
				testIndex++;
			}
		}

		/// <summary>
		/// Gets side and type for a test
		/// </summary>
		/// <param name="audiogram"></param>
		/// <param name="side"></param>
		/// <param name="testType"></param>
		/// <param name="maskTestType"></param>
		private void _GetTestDetails( Audiogram audiogram, out TestSide side, out int testType, out int maskTestType )
		{
			side = TestSide.Unknown;
			testType = AbmConst.ART;
			maskTestType = AbmConst.ART;
			switch( audiogram.MeasurementCondition.StimulusSignalOutput )
			{
				case SignalOutputEnum.AirConductorLeft:
					side = TestSide.Left;
					testType = AbmConst.AC;
					maskTestType = AbmConst.ACM;
					break;
				case SignalOutputEnum.BoneConductorLeft:
					testType = AbmConst.BC;
					side = TestSide.Left;
					maskTestType = AbmConst.BCM;
					break;
				case SignalOutputEnum.FreeFieldLeft:
					side = TestSide.Left;
					testType = AbmConst.SF;
					break;
				case SignalOutputEnum.InsertPhoneLeft:
					side = TestSide.Left;
					testType = AbmConst.IP;
					maskTestType = AbmConst.ACM;
					break;
				case SignalOutputEnum.AirConductorRight:
					side = TestSide.Right;
					testType = AbmConst.AC;
					maskTestType = AbmConst.ACM;
					break;
				case SignalOutputEnum.BoneConductorRight:
					testType = AbmConst.BC;
					side = TestSide.Right;
					maskTestType = AbmConst.BCM;
					break;
				case SignalOutputEnum.FreeFieldRight:
					side = TestSide.Right;
					testType = AbmConst.SF;
					break;
				case SignalOutputEnum.InsertPhoneRight:
					side = TestSide.Right;
					testType = AbmConst.IP;
					maskTestType = AbmConst.ACM;
					break;
				case SignalOutputEnum.AirConductorBinaural:
					side = TestSide.Both;
					testType = AbmConst.AC;
					maskTestType = AbmConst.ACM;
					break;
				case SignalOutputEnum.BoneConductorBinaural:
					testType = AbmConst.BC;
					side = TestSide.Both;
					maskTestType = AbmConst.BCM;
					break;
				case SignalOutputEnum.FreeFieldBinaural:
					testType = AbmConst.SF;
					side = TestSide.Both;
					break;
				case SignalOutputEnum.InsertPhoneBinaural:
					side = TestSide.Both;
					testType = AbmConst.IP;
					maskTestType = AbmConst.ACM;
					break;
				case SignalOutputEnum.NoSignalOutput:
					testType = AbmConst.MCL;
					break;
			}
		}

		/// <summary>
		/// Gets the threshold audiogram results
		/// </summary>
		private void _GetThresholdAudiogramResults()
		{
			int testIndex = 0;
			foreach( var audiogram in ThreshodToneAudiogram )
			{
				TestSide side;
				int testType, maskTestType;
				_GetTestDetails( audiogram, out side, out testType, out maskTestType );
				if(( audiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided1 ||
							audiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided2 ) &&
							testType != AbmConst.AC )	// Allow everything except the AC.
				{
					testType = AbmConst.SFA;
				}
				foreach( var tonePoint in audiogram.Curve )
				{
					var frequency = tonePoint.StimulusFrequency;
					var frequencyPosition = _GetFrequencyPosition( frequency );
					if( frequencyPosition > -1 )
					{
						mAudiogramResults.GetTestResults( side ).SetTestCondition1( testType, frequencyPosition, audiogram.MeasurementCondition.HearingInstrument_1_Condition, testIndex );
						mAudiogramResults.GetTestResults( side ).SetTestIntensity( testType, frequencyPosition, (int)tonePoint.StimulusLevel * 10, testIndex );
						mAudiogramResults.GetTestResults( side ).SetTestStatus( testType, frequencyPosition, tonePoint.TonePointStatus, testIndex );
					}
					if( (testType == AbmConst.AC || testType == AbmConst.BC || testType == AbmConst.IP) &&
							(audiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.UnAided ||
									audiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.NotSet) )
					{
						if( tonePoint.MaskingLevel >= 0 &&
								tonePoint.MaskingFrequency >= 0 && frequencyPosition > -1)
						{
							mAudiogramResults.GetTestResults( side ).SetTestIntensity( maskTestType, frequencyPosition, testType, testIndex );
							mAudiogramResults.GetTestResults( side ).SetTestStatus( maskTestType, frequencyPosition, tonePoint.TonePointStatus, testIndex );
						}
						if(( testType == AbmConst.AC || testType == AbmConst.IP) && frequencyPosition > -1 )
						{
							var status = tonePoint.TonePointStatus;
							if( status != PointStatusEnum.NotMeasurable && status != PointStatusEnum.NoResponse )
							{
								PureToneData.SetPTAData( side, frequency, (int)tonePoint.StimulusLevel * 10 );
							}
						}
					}
				}
				testIndex++;
			}
		}

		/// <summary>
		/// Gets the threshold audiogram results
		/// </summary>
		private void _GetUCLAudiogramResults()
		{
			int testIndex = 0;
			foreach( var audiogram in UCLToneAudiogram )
			{
				TestSide side;
				int testType, maskTestType;
				_GetTestDetails( audiogram, out side, out testType, out maskTestType );
				foreach( var tonePoint in audiogram.Curve )
				{
					var frequency = tonePoint.StimulusFrequency;
					var frequencyPosition = _GetFrequencyPosition( frequency );
					if( frequencyPosition > -1 )
					{
						mAudiogramResults.GetTestResults( side ).SetTestSignalOutput( AbmConst.UCL, frequencyPosition, audiogram.MeasurementCondition.StimulusSignalOutput, testIndex );
						mAudiogramResults.GetTestResults( side ).SetTestIntensity( AbmConst.UCL, frequencyPosition, (int)tonePoint.StimulusLevel * 10, testIndex );
						mAudiogramResults.GetTestResults( side ).SetTestStatus( AbmConst.UCL, frequencyPosition, tonePoint.TonePointStatus, testIndex );
					}
				}
				testIndex++;
			}
		}

		/// <summary>
		/// Parses the measurement condition
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="audiogram"></param>
		private void _ParseMeasurementCondition( XElement parent, IMeasurementAudiogram audiogram )
		{
			foreach( var element in parent.Elements() )
			{
				decimal dTemp;
				int iTemp;
				switch( element.Name.LocalName )
				{
					case Constants.XML_STIMULUSSIGNALTYPE:
						audiogram.MeasurementCondition.StimulusSignalType
							= EnumUtilities.Parse<SignalTypeEnum>( element.Value );
						break;
					case Constants.XML_MASKINGSIGNALTYPE:
						audiogram.MeasurementCondition.MaskingSignalType
							= EnumUtilities.Parse<SignalTypeEnum>( element.Value );
						break;
					case Constants.XML_STIMULUSSIGNALOUTPUT:
						audiogram.MeasurementCondition.StimulusSignalOutput
							= EnumUtilities.Parse<SignalOutputEnum>( element.Value );
						break;
					case Constants.XML_MASKINGSIGNALOUTPUT:
						audiogram.MeasurementCondition.MaskingSignalOutput
							= EnumUtilities.Parse<SignalOutputEnum>( element.Value );
						break;
					case Constants.XML_STIMULUSDBWEIGHTING:
						audiogram.MeasurementCondition.StimulusdBWeighting
							= EnumUtilities.Parse<DBWeightingTypeEnum>( element.Value );
						break;
					case Constants.XML_MASKINGDBWEIGHTING:
						audiogram.MeasurementCondition.MaskingdBWeighting
							= EnumUtilities.Parse<DBWeightingTypeEnum>( element.Value );
						break;
					case Constants.XML_STIMULUSPRESENTATIONTYPE:
						audiogram.MeasurementCondition.StimulusPresentationType
							= EnumUtilities.Parse<PresentationTypeEnum>( element.Value );
						break;
					case Constants.XML_MASKINGPRESENTATIONTYPE:
						audiogram.MeasurementCondition.MaskingPresentationType
							= EnumUtilities.Parse<PresentationTypeEnum>( element.Value );
						break;
					case Constants.XML_STIMULUSTRANSDUCERTYPE:
						audiogram.MeasurementCondition.StimulusTransducerType
							= EnumUtilities.Parse<TransducerTypeEnum>( element.Value );
						break;
					case Constants.XML_MASKINGTRANSDUCERTYPE:
						audiogram.MeasurementCondition.MaskingTransducerType
							= EnumUtilities.Parse<TransducerTypeEnum>( element.Value );
						break;
					case Constants.XML_TRANSDUCERDESCRIPTION:
						audiogram.MeasurementCondition.TransducerDescription
							= element.Value;
						break;
					case Constants.XML_STIMULUSTRANSDUCERCALIBRATIONSTANDARD:
						audiogram.MeasurementCondition.StimulusTransducerCalibrationStandard
							= EnumUtilities.Parse<TransducerCalibrationStandardTypeEnum>( element.Value );
						break;
					case Constants.XML_MASKINGTRANSDUCERCALIBRATIONSTANDARD:
						audiogram.MeasurementCondition.MaskingTransducerCalibrationStandard
							= EnumUtilities.Parse<TransducerCalibrationStandardTypeEnum>( element.Value );
						break;
					case Constants.XML_HEARINGINSTRUMENT_1_CONDITION:
						audiogram.MeasurementCondition.HearingInstrument_1_Condition
							= EnumUtilities.Parse<HearingTestConditionEnum>( element.Value );
						break;
					case Constants.XML_HEARINGINSTRUMENT_2_CONDITION:
						audiogram.MeasurementCondition.HearingInstrument_2_Condition
							= EnumUtilities.Parse<HearingTestConditionEnum>( element.Value );
						break;
					case Constants.XML_HEARINGINSTRUMENTDESCRIPTION:
						audiogram.MeasurementCondition.HearingInstrumentDescription = element.Value;
						break;
					case Constants.XML_STIMULUSAUXILIARY:
						audiogram.MeasurementCondition.StimulusAuxiliary
							= EnumUtilities.Parse<AuxiliaryParameterTypeEnum>( element.Value );
						break;
					case Constants.XML_MASKINGAUXILIARY:
						audiogram.MeasurementCondition.MaskingAuxiliary
							= EnumUtilities.Parse<AuxiliaryParameterTypeEnum>( element.Value );
						break;
					case Constants.XML_WORDLISTNAME:
						audiogram.MeasurementCondition.WordListName = element.Value;
						break;
					case Constants.XML_AUXILIARYPARAMETERDESCRIPTION:
						audiogram.MeasurementCondition.AuxiliaryParameterDescription = element.Value;
						break;
					case Constants.SPEECHTHRESHOLDTYPE:
						audiogram.MeasurementCondition.SpeechThresholdType
							= EnumUtilities.Parse<SpeechThresholdTypeEnum>( element.Value );
						break;
					case Constants.XML_STIMULUSONTIME:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.StimulusOnTime = dTemp;
						}
						break;
					case Constants.XML_MASKINGONTIME:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.MaskingOnTime = dTemp;
						}
						break;
					case Constants.XML_STIMULUSOFFTIME:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.StimulusOffTime = dTemp;
						}
						break;
					case Constants.XML_MASKINGOFFTIME:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.MaskingOffTime = dTemp;
						}
						break;
					case Constants.XML_STIMULUSSISIPARAMETER:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.StimulusSiSiParameter = dTemp;
						}
						break;
					case Constants.XML_MASKINGSISIPARAMETER:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.MaskingSiSiParameter = dTemp;
						}
						break;
					case Constants.XML_STIMULUSWARBLEMODULATION:
						if( int.TryParse( element.Value, out iTemp ) )
						{
							audiogram.MeasurementCondition.StimulusWarbleModulation = iTemp;
						}
						break;
					case Constants.XML_MASKINGWARBLEMODULATION:
						if( int.TryParse( element.Value, out iTemp ) )
						{
							audiogram.MeasurementCondition.MaskingWarbleModulation = iTemp;
						}
						break;
					case Constants.XML_STIMULUSWARBLEMODULATIONSIZE:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.StimulusWarbleModulationSize = dTemp;
						}
						break;
					case Constants.XML_MASKINGWARBLEMODULATIONSIZE:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.MaskingWarbleModulationSize = dTemp;
						}
						break;

					case Constants.XML_STIMULUSFREQUENCYMODULATION:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.StimulusFrequencyModulation = dTemp;
						}
						break;
					case Constants.XML_MASKINGFREQUENCYMODULATION:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.MaskingFrequencyModulation = dTemp;
						}
						break;

					case Constants.XML_STIMULUSAMPLITUDEMODULATION:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.StimulusAmplitudeModulation = dTemp;
						}
						break;
					case Constants.XML_MASKINGAMPLITUDEMODULATION:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.MaskingAmplitudeModulation = dTemp;
						}
						break;
					case Constants.XML_STIMULUSPULSEMODULATION:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.StimulusPulseModulation = dTemp;
						}
						break;
					case Constants.XML_MASKINGPULSEMODULATION:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.MaskingPulseModulation = dTemp;
						}
						break;
					case Constants.XML_STIMULUSPULSECYCLE:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.StimulusPulseCycle = dTemp;
						}
						break;
					case Constants.XML_MASKINGPULSECYCLE:
						if( decimal.TryParse( element.Value, out dTemp ) )
						{
							audiogram.MeasurementCondition.MaskingPulseCycle = dTemp;
						}
						break;
				}
			}
		}

		/// <summary>
		/// Processes Speech Audiograms
		/// </summary>
		private void _ProcessSpeechAudiogram( IEnumerable<SpeechAudiogram> speechAudiograms, string speechType )
		{
			foreach( var speechAudiogram in speechAudiograms )
			{
				if( speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.AirConductorRight ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorRight ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldRight ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.InsertPhoneRight )
				{
					if( speechType == MCLSPEECH )
						MCLRight = _BuildSpeechPoint( speechAudiogram.Points[0] );
					else if( speechType == UCLSPEECH )
						UCLRight = _BuildSpeechPoint( speechAudiogram.Points[0] );
					else
						SRTRight = _BuildSpeechPoint( speechAudiogram.Points[0] );
				}
				else if( speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.AirConductorLeft ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorLeft ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldLeft ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.InsertPhoneLeft )
				{
					if( speechType == MCLSPEECH )
						MCLLeft = _BuildSpeechPoint( speechAudiogram.Points[0] );
					else if( speechType == UCLSPEECH )
						UCLLeft = _BuildSpeechPoint( speechAudiogram.Points[0] );
					else
						SRTLeft = _BuildSpeechPoint( speechAudiogram.Points[0] );
				}
				else if( speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.AirConductorBinaural ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorBinaural ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldBinaural ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.InsertPhoneBinaural )
				{
					if( speechType == MCLSPEECH )
						MCLBinural = _BuildSpeechPoint( speechAudiogram.Points[0] );
					else if( speechType == UCLSPEECH )
						UCLBinural = _BuildSpeechPoint( speechAudiogram.Points[0] );
					else
						SRTBinural = _BuildSpeechPoint( speechAudiogram.Points[0] );
				}
			}
		}

		/// <summary>
		/// Process the word recogntion data
		/// </summary>
		/// <param name="speechAudiograms"></param>
		private void _ProcessWordRecognitionAudiogram( IEnumerable<SpeechAudiogram> speechAudiograms )
		{
			foreach( var speechAudiogram in speechAudiograms )
			{
				if( speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.AirConductorRight ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorRight ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldRight ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.InsertPhoneRight )
				{
					if( speechAudiogram.MeasurementCondition.MaskingSignalType != SignalTypeEnum.SpeechNoise )
					{
						if( speechAudiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided1 ||
							speechAudiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided2 )
						{
							_BuildWordRecognitionPoint( WR1RightAided, speechAudiogram.Points[0] );
							WR1RightAidedList = new List<SpeechResult>();
							_BuildWordRecognitionPointList( WR1RightAidedList, speechAudiogram.Points );
						}
						else
						{
							_BuildWordRecognitionPoint( WR1Right, speechAudiogram.Points[0] );
							WR1RightList = new List<SpeechResult>();
							_BuildWordRecognitionPointList( WR1RightList, speechAudiogram.Points );
						}
					}
					else
					{
						if( speechAudiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided1 ||
							speechAudiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided2 )
						{
							_BuildWordRecognitionPoint( WR2RightAided, speechAudiogram.Points[0] );
							WR2RightAidedList = new List<SpeechResult>();
							_BuildWordRecognitionPointList( WR2RightAidedList, speechAudiogram.Points );
						}
						else
						{
							_BuildWordRecognitionPoint( WR2Right, speechAudiogram.Points[0] );
							WR2RightList = new List<SpeechResult>();
							_BuildWordRecognitionPointList( WR2RightList, speechAudiogram.Points );
						}
					}
				}
				else if( speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.AirConductorLeft ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorLeft ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldLeft ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.InsertPhoneLeft )
				{
					if( speechAudiogram.MeasurementCondition.MaskingSignalType != SignalTypeEnum.SpeechNoise )
					{
						if( speechAudiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided1 ||
							speechAudiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided2 )
						{
							_BuildWordRecognitionPoint( WR1LeftAided, speechAudiogram.Points[0] );
							WR1LeftAidedList = new List<SpeechResult>();
							_BuildWordRecognitionPointList( WR1LeftAidedList, speechAudiogram.Points );
						}
						else
						{
							_BuildWordRecognitionPoint( WR1Left, speechAudiogram.Points[0] );
							WR1LeftList = new List<SpeechResult>();
							_BuildWordRecognitionPointList( WR1LeftList, speechAudiogram.Points );
						}

					}
					else
					{
						if( speechAudiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided1 ||
							speechAudiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided2 )
						{
							_BuildWordRecognitionPoint( WR2LeftAided, speechAudiogram.Points[0] );
							WR2LeftAidedList = new List<SpeechResult>();
							_BuildWordRecognitionPointList( WR2LeftAidedList, speechAudiogram.Points );
						}
						else
						{
							_BuildWordRecognitionPoint( WR2Left, speechAudiogram.Points[0] );
							WR2LeftList = new List<SpeechResult>();
							_BuildWordRecognitionPointList( WR2LeftList, speechAudiogram.Points );
						}
					}
				}
				else if( speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.AirConductorBinaural ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorBinaural ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldBinaural ||
					speechAudiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.InsertPhoneBinaural )
				{
					if( speechAudiogram.MeasurementCondition.MaskingSignalType != SignalTypeEnum.SpeechNoise )
					{
						if( speechAudiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided1 ||
							speechAudiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided2 )
						{
							_BuildWordRecognitionPoint( WR1BinuralAided, speechAudiogram.Points[0] );
							WR1BinuralAidedList = new List<SpeechResult>();
							_BuildWordRecognitionPointList( WR1BinuralAidedList, speechAudiogram.Points );
						}
						else
						{
							_BuildWordRecognitionPoint( WR1Binural, speechAudiogram.Points[0] );
							WR1BinuralList = new List<SpeechResult>();
							_BuildWordRecognitionPointList( WR1BinuralList, speechAudiogram.Points );
						}
					}
					else
					{
						if( speechAudiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided1 ||
							speechAudiogram.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided2 )
						{
							_BuildWordRecognitionPoint( WR2BinuralAided, speechAudiogram.Points[0] );
							WR2BinuralAidedList = new List<SpeechResult>();
							_BuildWordRecognitionPointList( WR2BinuralAidedList, speechAudiogram.Points );
						}
						else
						{
							_BuildWordRecognitionPoint( WR2Binural, speechAudiogram.Points[0] );
							WR2BinuralList = new List<SpeechResult>();
							_BuildWordRecognitionPointList( WR2BinuralList, speechAudiogram.Points );
						}
					}
				}
			}
		}

		#endregion Private Members

	}
}
