using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TIMS_X.Server.Legacy.Noah.Enums;
using TIMS_X.Server.Legacy.Noah.Format500;
using TIMS_X.Server.Legacy.Noah.Format500.Enums;

namespace TIMS_X.Server.Legacy.Noah
{
	/// <summary>
	/// Base class to import data from Audbase
	/// </summary>
	public abstract class AudiogramImportBase
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		protected AudiogramImportBase()
		{
			_ResetAudiograms();
		}

		#endregion Constructors

		#region AudiogramImportBase Members

		/// <summary>
		/// The Audiogram
		/// </summary>
		public N500Audiogram N500Audiogram
		{
			get;
			protected set;
		}

		/// <summary>
		/// Builds an N500 Audiogram from audbase data
		/// </summary>
		/// <returns></returns>
		public void BuildN500Audiogram()
		{
			N500Audiogram = new N500Audiogram();
			BuildAudioData();
			BuildSpeechData();
			BuildTympanograms();
		}

		#endregion AudiogramImportBase Members

		#region Protected Members

		protected static int NO_RESPONSE_OFFSET = 200;
		protected static string COULD_NOT_TEST = "400";
		protected static string NOT_DONE = "-32767";
		protected static string UNSPECIFIED = "300";
		protected Dictionary<string, int> mFrequencyLookup = new Dictionary<string, int>();
		protected List<Audiogram> mAudiograms;
		protected List<Audiogram> mMCLAudiograms;
		protected List<Audiogram> mUCLAudiograms;

		/// <summary>
		/// Creates a new pure tone audiogram
		/// </summary>
		/// <param name="signalOutputType"></param>
		/// <param name="maskedSignalOutputType"></param>
		/// <returns></returns>
		protected Audiogram _CreatePureToneAudiogram( SignalOutputEnum signalOutputType, SignalOutputEnum maskedSignalOutputType )
		{
			var audiogram = new Audiogram();
			audiogram.MeasurementCondition.StimulusSignalType = TIMS_X.Server.Legacy.Noah.Enums.SignalTypeEnum.PureTone;

			audiogram.MeasurementCondition.StimulusSignalOutput = signalOutputType;
			audiogram.MeasurementCondition.MaskingSignalOutput = maskedSignalOutputType;

			audiogram.MeasurementCondition.StimulusdBWeighting = DBWeightingTypeEnum.HL;
			audiogram.MeasurementCondition.MaskingdBWeighting = DBWeightingTypeEnum.HL;
			audiogram.MeasurementCondition.StimulusPresentationType = PresentationTypeEnum.Continuous;
			audiogram.MeasurementCondition.MaskingPresentationType = PresentationTypeEnum.Continuous;
			audiogram.MeasurementCondition.HearingInstrument_1_Condition = HearingTestConditionEnum.UnAided;

			return audiogram;
		}

		/// <summary>
		/// Create new tone point
		/// </summary>
		/// <param name="frequency"></param>
		/// <param name="level"></param>
		/// <param name="masked"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		protected Format500.TonePoint _CreateTonePoint( int frequency, decimal level, bool masked, PointStatusEnum status )
		{
			var tonePoint = new Format500.TonePoint()
			{
				StimulusFrequency = frequency,
				StimulusLevel = level,
				TonePointStatus = status
			};
			if( masked )
			{
				tonePoint.MaskingFrequency = 0;
				tonePoint.MaskingLevel = 0m;
			}
			return tonePoint;
		}

		/// <summary>
		/// Gets the AC Audiogram
		/// </summary>
		/// <param name="right"></param>
		/// <returns></returns>
		protected Audiogram _GetACAudiogram( bool right )
		{
			Audiogram target = null;
			if( right )
			{
				target =
					mAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.AirConductorRight );
			}
			else
			{
				target =
					mAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.AirConductorLeft );
			}
			if( target == null )
			{
				if( right )
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.AirConductorRight, SignalOutputEnum.AirConductorLeft );
				}
				else
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.AirConductorLeft, SignalOutputEnum.AirConductorRight );
				}
				mAudiograms.Add( target );
			}
			return target;
		}

		/// <summary>
		/// Gets the AC Audiogram
		/// </summary>
		/// <param name="right"></param>
		/// <returns></returns>
		protected Audiogram _GetBCAudiogram( bool right )
		{
			Audiogram target = null;
			if( right )
			{
				target =
					mAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorRight );
			}
			else
			{
				target =
					mAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorLeft );
			}
			if( target == null )
			{
				if( right )
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.BoneConductorRight, SignalOutputEnum.BoneConductorLeft );
				}
				else
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.BoneConductorLeft, SignalOutputEnum.BoneConductorRight );
				}
				mAudiograms.Add( target );
			}
			return target;
		}

		/// <summary>
		/// Gets the MCL Audiogram
		/// </summary>
		/// <param name="side"></param>
		/// <returns></returns>
		protected Audiogram _GetMCLAudiogram( Side side )
		{
			Audiogram target = null;
			if( side == Side.Right )
			{
				target =
					mMCLAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.InsertPhoneRight );
			}
			else if( side == Side.Left )
			{
				target =
					mMCLAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.InsertPhoneLeft );
			}
			else
			{
				target =
					mMCLAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.InsertPhoneBinaural );
			}
			if( target == null )
			{
				if( side == Side.Right )
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.InsertPhoneRight, SignalOutputEnum.InsertPhoneLeft );
				}
				else if( side == Side.Left )
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.InsertPhoneLeft, SignalOutputEnum.InsertPhoneRight );
				}
				else
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.InsertPhoneBinaural, SignalOutputEnum.InsertPhoneBinaural );
				}
				mMCLAudiograms.Add( target );
			}
			return target;
		}

		/// <summary>
		/// Gets soundfield aided Audiogram
		/// </summary>
		/// <param name="side"></param>
		/// <returns></returns>
		protected Audiogram _GetSFAAudiogram( Side side )
		{
			Audiogram target = null;
			if( side == Side.Right )
			{
				target =
					mAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldRight
						&& x.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided1 );
			}
			else if( side == Side.Left )
			{
				target =
					mAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldLeft
						&& x.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided1 );
			}
			else
			{
				target =
					mAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldBinaural
						&& x.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.Aided1 );
			}
			if( target == null )
			{
				if( side == Side.Right )
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.FreeFieldRight, SignalOutputEnum.FreeFieldLeft );
				}
				else if( side == Side.Left )
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.FreeFieldLeft, SignalOutputEnum.FreeFieldRight );
				}
				else
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.FreeFieldBinaural, SignalOutputEnum.FreeFieldBinaural );
				}
				target.MeasurementCondition.HearingInstrument_1_Condition = HearingTestConditionEnum.Aided1;
				mAudiograms.Add( target );
			}
			return target;
		}

		/// <summary>
		/// Gets soundfield Audiogram
		/// </summary>
		/// <param name="side"></param>
		/// <returns></returns>
		protected Audiogram _GetSFAudiogram( Side side )
		{
			Audiogram target = null;
			if( side == Side.Right )
			{
				target =
					mAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldRight
						&& x.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.UnAided );
			}
			else if( side == Side.Left )
			{
				target =
					mAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldLeft
						&& x.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.UnAided );
			}
			else
			{
				target =
					mAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldBinaural
						&& x.MeasurementCondition.HearingInstrument_1_Condition == HearingTestConditionEnum.UnAided );
			}
			if( target == null )
			{
				if( side == Side.Right )
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.FreeFieldRight, SignalOutputEnum.FreeFieldLeft );
				}
				else if( side == Side.Left )
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.FreeFieldLeft, SignalOutputEnum.FreeFieldRight );
				}
				else
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.FreeFieldBinaural, SignalOutputEnum.FreeFieldBinaural );
				}
				target.MeasurementCondition.HearingInstrument_1_Condition = HearingTestConditionEnum.UnAided;
				mAudiograms.Add( target );
			}
			return target;
		}

		/// <summary>
		/// Gets the UCL Audiogram
		/// </summary>
		/// <param name="side"></param>
		/// <returns></returns>
		protected Audiogram _GetUCLAudiogram( Side side )
		{
			Audiogram target = null;
			if( side == Side.Right )
			{
				target =
					mUCLAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.InsertPhoneRight );
			}
			else if( side == Side.Left )
			{
				target =
					mUCLAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.InsertPhoneLeft );
			}
			else
			{
				target =
					mUCLAudiograms.FirstOrDefault( x => x.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.InsertPhoneBinaural );
			}
			if( target == null )
			{
				if( side == Side.Right )
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.InsertPhoneRight, SignalOutputEnum.InsertPhoneLeft );
				}
				else if( side == Side.Left )
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.InsertPhoneLeft, SignalOutputEnum.InsertPhoneRight );
				}
				else
				{
					target = _CreatePureToneAudiogram( SignalOutputEnum.InsertPhoneBinaural, SignalOutputEnum.InsertPhoneBinaural );
				}
				mUCLAudiograms.Add( target );
			}
			return target;
		}

		/// <summary>
		/// Resets the audigram arrays
		/// </summary>
		protected void _ResetAudiograms()
		{
			mAudiograms = new List<Audiogram>();
			mMCLAudiograms = new List<Audiogram>();
			mUCLAudiograms = new List<Audiogram>();
		}

		/// <summary>
		/// Builds the Audio data
		/// </summary>
		/// <remarks>Make sure audiogram data has been processed and audiogram filled before calling this</remarks>
		protected virtual void BuildAudioData()
		{
			foreach( var audiogram in mAudiograms )
			{
				if( (audiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.AirConductorLeft) ||
					(audiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.AirConductorRight) ||
					(audiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorLeft) ||
					(audiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorRight) ||
					(audiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldLeft) ||
					(audiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldRight) )
				{
					N500Audiogram.ThreshodToneAudiogram.Add( audiogram );
				}
			}
			foreach( var audiogram in mMCLAudiograms )
			{
				N500Audiogram.MCLToneAudiogram.Add( audiogram );
			}
			foreach( var audiogram in mUCLAudiograms )
			{
				N500Audiogram.UCLToneAudiogram.Add( audiogram );
			}
		}

		/// <summary>
		/// Build the speech data
		/// </summary>
		protected abstract void BuildSpeechData();

		/// <summary>
		/// Build left and right tympanograms
		/// </summary>
		protected abstract void BuildTympanograms();

		#endregion Protected Members

		#region Other

		/// <summary>
		/// Side of data
		/// </summary>
		protected enum Side
		{
			/// <summary>
			/// Right
			/// </summary>
			Right,
			/// <summary>
			/// Left
			/// </summary>
			Left,
			/// <summary>
			/// Binaural
			/// </summary>
			Binaural
		}

		#endregion Other

	}


}
