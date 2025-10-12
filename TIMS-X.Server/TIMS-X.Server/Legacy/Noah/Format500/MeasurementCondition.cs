using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TIMS_X.Server.Legacy.Noah.Enums;
using TIMS_X.Server.Legacy.Noah.Format500.Enums;

namespace TIMS_X.Server.Legacy.Noah.Format500
{
	public class MeasurementCondition
	{
		#region Constructors

		/// <summary>
		/// Condition
		/// </summary>
		public MeasurementCondition()
		{
			StimulusSignalType = SignalTypeEnum.NoSignalApplied;
			MaskingSignalType = SignalTypeEnum.NoSignalApplied;
			StimulusSignalOutput = SignalOutputEnum.NoSignalOutput;
			MaskingSignalOutput = SignalOutputEnum.NoSignalOutput;
			StimulusdBWeighting = DBWeightingTypeEnum.NodBWeighting;
			MaskingdBWeighting = DBWeightingTypeEnum.NodBWeighting;
			StimulusPresentationType = PresentationTypeEnum.NoPresentationType;
			MaskingPresentationType = PresentationTypeEnum.NoPresentationType;
			HearingInstrument_1_Condition = HearingTestConditionEnum.NotSet;
			HearingInstrument_2_Condition = HearingTestConditionEnum.NotSet;
		}

		#endregion Constructors

		#region MeasurementCondition Members

		/// <summary>
		/// 
		/// </summary>
		public string AuxiliaryParameterDescription
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public HearingTestConditionEnum HearingInstrument_1_Condition
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public HearingTestConditionEnum HearingInstrument_2_Condition
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public string HearingInstrumentDescription
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal MaskingAmplitudeModulation
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public AuxiliaryParameterTypeEnum MaskingAuxiliary
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public DBWeightingTypeEnum MaskingdBWeighting
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal MaskingFrequencyModulation
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal MaskingOffTime
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal MaskingOnTime
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public PresentationTypeEnum MaskingPresentationType
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal MaskingPulseCycle
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal MaskingPulseModulation
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public SignalOutputEnum MaskingSignalOutput
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public SignalTypeEnum MaskingSignalType
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal MaskingSiSiParameter
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public TransducerCalibrationStandardTypeEnum MaskingTransducerCalibrationStandard
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public TransducerTypeEnum MaskingTransducerType
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public int MaskingWarbleModulation
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal MaskingWarbleModulationSize
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public SpeechThresholdTypeEnum SpeechThresholdType
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal StimulusAmplitudeModulation
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public AuxiliaryParameterTypeEnum StimulusAuxiliary
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public DBWeightingTypeEnum StimulusdBWeighting
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal StimulusFrequencyModulation
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal StimulusOffTime
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal StimulusOnTime
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public PresentationTypeEnum StimulusPresentationType
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal StimulusPulseCycle
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal StimulusPulseModulation
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public SignalOutputEnum StimulusSignalOutput
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public SignalTypeEnum StimulusSignalType
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal StimulusSiSiParameter
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public TransducerCalibrationStandardTypeEnum StimulusTransducerCalibrationStandard
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public TransducerTypeEnum StimulusTransducerType
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public int StimulusWarbleModulation
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal StimulusWarbleModulationSize
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public string TransducerDescription
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public string WordListName
		{
			get;
			set;
		}

		/// <summary>
		/// Serialize to XML
		/// </summary>
		/// <returns></returns>
		public XElement Serialize()
		{
			var root = N500Audiogram.CreateElementWithNamespace( Constants.XML_AUDMEASUREMENTCONDITIONS );

			root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_STIMULUSSIGNALTYPE, StimulusSignalType.ToString() ) );
			root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_MASKINGSIGNALTYPE, MaskingSignalType.ToString() ) );

			root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_STIMULUSSIGNALOUTPUT, StimulusSignalOutput.ToString() ) );
			root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_MASKINGSIGNALOUTPUT, MaskingSignalOutput.ToString() ) );

			root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_STIMULUSDBWEIGHTING, StimulusdBWeighting.ToString() ) );
			root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_MASKINGDBWEIGHTING, MaskingdBWeighting.ToString() ) );

			root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_STIMULUSPRESENTATIONTYPE, StimulusPresentationType.ToString() ) );
			root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_MASKINGPRESENTATIONTYPE, MaskingPresentationType.ToString() ) );

			if( HearingInstrument_1_Condition != HearingTestConditionEnum.Unknown )
			{
				root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_HEARINGINSTRUMENT_1_CONDITION, HearingInstrument_1_Condition.ToString() ) );
			}
			if( HearingInstrument_2_Condition != HearingTestConditionEnum.Unknown )
			{
				root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_HEARINGINSTRUMENT_1_CONDITION, HearingInstrument_2_Condition.ToString() ) );
			}

			return root;
		}

		#endregion MeasurementCondition Members

	}
}
