using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Domain.Noah;
using TIMS_X.Core.Enums;
using TIMS_X.Server.Legacy.Noah;
using TIMS_X.Server.Legacy.Noah.Enums;
using TIMS_X.Server.Legacy.Noah.Format500;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Services;

public class NoahDataMiningService : INoahDataMiningService
{
	private const int INVALID_VALUE = -32767;
	private readonly NoahDataMiningQuery _ndmQuery;

	public NoahDataMiningService(NoahDataMiningQuery ndmQuery)
	{
		_ndmQuery = ndmQuery;
	}

	public async Task<Tuple<bool, bool>> IsEarAidableAsync(Patient patient, TestSide side)
	{
		var isAidable = false;
		var hasAudiogram = false;

		var criteria = await _ndmQuery.GetOpportunityTrackingCriteriaAsync();
		if (criteria == null)
			throw new NullReferenceException("No Audio Search Criteria found for Opportunity Tracking");

		var ndmSide = NdmSide.Left;
		if (side == TestSide.Right)
			ndmSide = NdmSide.Right;
		else if (side == TestSide.Both)
			ndmSide = NdmSide.Binaural;

		var audiograms = await _ndmQuery.GetPatientAudiogramsAsync(patient.Id, ndmSide);

		if (audiograms.Any())
		{
			hasAudiogram = true;
			isAidable = _ValidateOpportunityTrackingCriteria(audiograms, criteria);
		}
		else
		{
			isAidable = true;
		}

		return new Tuple<bool, bool>(hasAudiogram, isAidable);
	}

	public async Task<NdmAction> ProcessNoahActionAsync(N4Action action, byte[] decompressedPublicData, int patientId)
	{
		NdmAction ndmAction = null;
		var isNoahDataMiningEnabled = await _ndmQuery.IsNoahDataMiningEnabledAsync();

		if (isNoahDataMiningEnabled && decompressedPublicData != null)
		{
			await _ndmQuery.DeleteNdmActionAsync(action.Id);


			if (action.DataTypeCode == 1)
			{
				switch (action.DataFmtCodeStd)
				{
					case 100:
						ndmAction = _ProcessN100Audiogram(decompressedPublicData, patientId);
						break;
					case 200:
						ndmAction = _ProcessN200Audiogram(decompressedPublicData, patientId);
						break;
					case 500:
						ndmAction = _ProcessN500Audiogram(decompressedPublicData, patientId);
						break;
					case 502:
						ndmAction = _ProcessN502Audiogram(decompressedPublicData, patientId);
						break;
				}

				if (ndmAction != null)
				{
					ndmAction.ActionId = action.Id;
					ndmAction.AudiogramDate = action.UpdatedDate ?? SqlDateTime.MinValue.Value;
					if (ndmAction.Audiograms.Any()) await _ndmQuery.PutNdmActionAsync(ndmAction);
				}
			}
		}

		return ndmAction;
	}

	private NdmSide _GetSide(NdmMeasurementCondition measurementCondition)
	{
		if ((SignalOutputEnum)measurementCondition.StimulusSignalOutput == SignalOutputEnum.AirConductorRight ||
		    (SignalOutputEnum)measurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorRight ||
		    (SignalOutputEnum)measurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldRight ||
		    (SignalOutputEnum)measurementCondition.StimulusSignalOutput == SignalOutputEnum.InsertPhoneRight)
			return NdmSide.Right;
		if ((SignalOutputEnum)measurementCondition.StimulusSignalOutput == SignalOutputEnum.AirConductorLeft ||
		    (SignalOutputEnum)measurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorLeft ||
		    (SignalOutputEnum)measurementCondition.StimulusSignalOutput == SignalOutputEnum.FreeFieldLeft ||
		    (SignalOutputEnum)measurementCondition.StimulusSignalOutput == SignalOutputEnum.InsertPhoneLeft)
			return NdmSide.Left;

		return NdmSide.Binaural;
	}

	private bool _IsCriteriaMetForAudiogram(NdmAudiogram audiogram, NdmSearchCriteria criteria)
	{
		var meetsCrtieria = false;
		if (audiogram != null)
			meetsCrtieria =
				AreConditionsMetForAudiogram(audiogram, criteria.SearchPoints, criteria.SeverityMinPoints.Value);
		return meetsCrtieria;
	}

	private NdmAudiogram _Process100ToneAudiogram(THRAudiogram100 audiogram, AudiogramTypeEnum audiogramType)
	{
		// Valid data, save the curve
		var ndmAudiogram = new NdmAudiogram
		{
			AudiogramType = (int)audiogramType
		};
		var ndmMeasurementCondition = new NdmMeasurementCondition
		{
			StimulusSignalType = audiogram.MeasuringCondition.RSignalType1,
			MaskingSignalType = audiogram.MeasuringCondition.RSignalType2,
			StimulusSignalOutput = audiogram.MeasuringCondition.RSignalOutput1,
			MaskingSignalOutput = audiogram.MeasuringCondition.RSignalOutput2,
			StimulusdBWeighting = audiogram.MeasuringCondition.RdBWeighting1,
			MaskingdBWeighting = audiogram.MeasuringCondition.RdBWeighting2,
			StimulusPresentationType = audiogram.MeasuringCondition.RPresentType1,
			MaskingPresentationType = audiogram.MeasuringCondition.RPresentType2,
			HearingInstrument1Condition = audiogram.MeasuringCondition.RCondition1,
			HearingInstrument2Condition = audiogram.MeasuringCondition.RCondition2
		};
		ndmAudiogram.MeasurementCondition = ndmMeasurementCondition;
		ndmAudiogram.Side = (int)_GetSide(ndmMeasurementCondition);

		for (var pointIndex = 0; pointIndex < AbmConst.NUMBER_AUDIOGRAM_POINTS; pointIndex++)
		{
			var frequency = audiogram.TPFreq1(pointIndex);
			if (frequency != AbmConst.UNDEFINED)
			{
				var status = audiogram.TPStatus(pointIndex);
				if (status == PointStatusEnum.AlwaysResponse || status == PointStatusEnum.Normal)
				{
					var mfreq = audiogram.TPFreq2(pointIndex);

					var level = audiogram.TPIntensity1(pointIndex);
					var mlevel = audiogram.TPIntensity2(pointIndex);

					var tonePoint = new NdmTonePoint
					{
						StimulusFrequency = frequency,
						MaskingFrequency = mfreq,
						StimulusLevel = level,
						MaskingLevel = mlevel,
						TonePointStatus = (int)status
					};
					ndmAudiogram.TonePoints.Add(tonePoint);
				}
			}
		}

		return ndmAudiogram;
	}

	private NdmAudiogram _Process200ToneAudiogram(THRAudiogram audiogram, AudiogramTypeEnum audiogramType)
	{
		var ndmAudiogram = new NdmAudiogram
		{
			AudiogramType = (int)audiogramType
		};
		var ndmMeasurementCondition = new NdmMeasurementCondition
		{
			StimulusSignalType = (int)audiogram.MeasuringCondition.RSignalType1,
			MaskingSignalType = (int)audiogram.MeasuringCondition.RSignalType2,
			StimulusSignalOutput = (int)audiogram.MeasuringCondition.RSignalOutput1,
			MaskingSignalOutput = (int)audiogram.MeasuringCondition.RSignalOutput2,
			StimulusdBWeighting = audiogram.MeasuringCondition.RdBWeighting1,
			MaskingdBWeighting = audiogram.MeasuringCondition.RdBWeighting2,
			StimulusPresentationType = audiogram.MeasuringCondition.RPresentType1,
			MaskingPresentationType = audiogram.MeasuringCondition.RPresentType2,
			HearingInstrument1Condition = (int)audiogram.MeasuringCondition.RCondition1,
			HearingInstrument2Condition = (int)audiogram.MeasuringCondition.RCondition2
		};
		ndmAudiogram.MeasurementCondition = ndmMeasurementCondition;
		ndmAudiogram.Side = (int)_GetSide(ndmMeasurementCondition);

		for (var pointIndex = 0; pointIndex < AbmConst.NUMBER_AUDIOGRAM_POINTS; pointIndex++)
		{
			var frequency = audiogram.TPFreq1(pointIndex);
			if (frequency != AbmConst.UNDEFINED)
			{
				var status = audiogram.TPStatus(pointIndex);
				if (status == PointStatusEnum.AlwaysResponse || status == PointStatusEnum.Normal)
				{
					var mfreq = audiogram.TPFreq2(pointIndex);

					var level = audiogram.TPIntensity1(pointIndex);
					var mlevel = audiogram.TPIntensity2(pointIndex);

					var tonePoint = new NdmTonePoint
					{
						StimulusFrequency = frequency,
						MaskingFrequency = mfreq,
						StimulusLevel = level,
						MaskingLevel = mlevel,
						TonePointStatus = (int)status
					};
					ndmAudiogram.TonePoints.Add(tonePoint);
				}
			}
		}

		return ndmAudiogram;
	}

	private NdmAudiogram _Process500ToneAudiogram(Audiogram audiogram, AudiogramTypeEnum audiogramType)
	{
		// Valid data, save the curve
		var ndmAudiogram = new NdmAudiogram
		{
			AudiogramType = (int)audiogramType
		};

		var ndmMeasurementCondition = new NdmMeasurementCondition
		{
			StimulusSignalType = (int)audiogram.MeasurementCondition.StimulusSignalType,
			MaskingSignalType = (int)audiogram.MeasurementCondition.MaskingSignalType,
			StimulusSignalOutput = (int)audiogram.MeasurementCondition.StimulusSignalOutput,
			MaskingSignalOutput = (int)audiogram.MeasurementCondition.MaskingSignalOutput,
			StimulusdBWeighting = (int)audiogram.MeasurementCondition.StimulusdBWeighting,
			MaskingdBWeighting = (int)audiogram.MeasurementCondition.MaskingdBWeighting,
			StimulusPresentationType = (int)audiogram.MeasurementCondition.StimulusPresentationType,
			MaskingPresentationType = (int)audiogram.MeasurementCondition.MaskingPresentationType,
			HearingInstrument1Condition = (int)audiogram.MeasurementCondition.HearingInstrument_1_Condition,
			HearingInstrument2Condition = (int)audiogram.MeasurementCondition.HearingInstrument_2_Condition
		};
		ndmAudiogram.MeasurementCondition = ndmMeasurementCondition;
		ndmAudiogram.Side = (int)_GetSide(ndmMeasurementCondition);

		foreach (var point in audiogram.Curve)
			if (point.TonePointStatus == PointStatusEnum.AlwaysResponse ||
			    point.TonePointStatus == PointStatusEnum.Normal)
			{
				var tonePoint = new NdmTonePoint
				{
					StimulusFrequency = point.StimulusFrequency,
					MaskingFrequency = point.MaskingFrequency,
					StimulusLevel = (int)point.StimulusLevel * 10,
					MaskingLevel = (int)point.MaskingLevel * 10,
					TonePointStatus = (int)point.TonePointStatus
				};
				ndmAudiogram.TonePoints.Add(tonePoint);
			}

		return ndmAudiogram;
	}

	private NdmAudiogram _Process502ToneAudiogram(Audiogram audiogram, AudiogramTypeEnum audiogramType)
	{
		// Valid data, save the curve
		var ndmAudiogram = new NdmAudiogram
		{
			AudiogramType = (int)audiogramType
		};

		var ndmMeasurementCondition = new NdmMeasurementCondition
		{
			StimulusSignalType = (int)audiogram.MeasurementCondition.StimulusSignalType,
			MaskingSignalType = (int)audiogram.MeasurementCondition.MaskingSignalType,
			StimulusSignalOutput = (int)audiogram.MeasurementCondition.StimulusSignalOutput,
			MaskingSignalOutput = (int)audiogram.MeasurementCondition.MaskingSignalOutput,
			StimulusdBWeighting = (int)audiogram.MeasurementCondition.StimulusdBWeighting,
			MaskingdBWeighting = (int)audiogram.MeasurementCondition.MaskingdBWeighting,
			StimulusPresentationType = (int)audiogram.MeasurementCondition.StimulusPresentationType,
			MaskingPresentationType = (int)audiogram.MeasurementCondition.MaskingPresentationType,
			HearingInstrument1Condition = (int)audiogram.MeasurementCondition.HearingInstrument_1_Condition,
			HearingInstrument2Condition = (int)audiogram.MeasurementCondition.HearingInstrument_2_Condition
		};
		ndmAudiogram.MeasurementCondition = ndmMeasurementCondition;
		ndmAudiogram.Side = (int)_GetSide(ndmMeasurementCondition);

		foreach (var point in audiogram.Curve)
			if (point.TonePointStatus == PointStatusEnum.AlwaysResponse ||
			    point.TonePointStatus == PointStatusEnum.Normal)
			{
				var tonePoint = new NdmTonePoint
				{
					StimulusFrequency = point.StimulusFrequency,
					MaskingFrequency = point.MaskingFrequency,
					StimulusLevel = (int)point.StimulusLevel * 10,
					MaskingLevel = (int)point.MaskingLevel * 10,
					TonePointStatus = (int)point.TonePointStatus
				};
				ndmAudiogram.TonePoints.Add(tonePoint);
			}

		return ndmAudiogram;
	}

	private NdmAction _ProcessN100Audiogram(byte[] publicData, int patientId)
	{
		var ndmAction = new NdmAction { CreatedDate = DateTime.Now };
		using (var tgt = new MemoryStream(publicData))
		{
			var n100 = new N100(tgt);
			foreach (var audiogram in n100.ThreshodToneAudiogram)
				if (audiogram.SignalOutput1 != INVALID_VALUE &&
				    audiogram.SignalOutput1 != (int)SignalOutputEnum.NoSignalOutput &&
				    audiogram.SignalOutput1 != (int)SignalOutputEnum.Unknown)
				{
					var audiogramType = AudiogramTypeEnum.Threshold;
					if (audiogram.SignalOutput1 == (int)SignalOutputEnum.BoneConductorLeft ||
					    audiogram.SignalOutput1 == (int)SignalOutputEnum.BoneConductorRight ||
					    audiogram.SignalOutput1 == (int)SignalOutputEnum.BoneConductorBinaural)
						audiogramType = AudiogramTypeEnum.BC;
					var ndmAudiogram = _Process100ToneAudiogram(audiogram, audiogramType);
					ndmAudiogram.PatientId = patientId;
					ndmAction.Audiograms.Add(ndmAudiogram);
				}
		}

		return ndmAction;
	}

	private NdmAction _ProcessN200Audiogram(byte[] publicData, int patientId)
	{
		var ndmAction = new NdmAction { CreatedDate = DateTime.Now };
		using (var tgt = new MemoryStream(publicData))
		{
			var n200 = new N200(tgt);
			foreach (var audiogram in n200.ThreshodToneAudiogram)
				if (audiogram.SignalOutput1 != SignalOutputEnum.NotSet &&
				    audiogram.SignalOutput1 != SignalOutputEnum.NoSignalOutput &&
				    audiogram.SignalOutput1 != SignalOutputEnum.Unknown)
				{
					var audiogramType = AudiogramTypeEnum.Threshold;
					if (audiogram.SignalOutput1 == SignalOutputEnum.BoneConductorLeft ||
					    audiogram.SignalOutput1 == SignalOutputEnum.BoneConductorRight ||
					    audiogram.SignalOutput1 == SignalOutputEnum.BoneConductorBinaural)
						audiogramType = AudiogramTypeEnum.BC;
					var ndmAudiogram = _Process200ToneAudiogram(audiogram, audiogramType);
					ndmAudiogram.PatientId = patientId;
					ndmAction.Audiograms.Add(ndmAudiogram);
				}
		}

		return ndmAction;
	}

	private NdmAction _ProcessN500Audiogram(byte[] publicData, int patientId)
	{
		var ndmAction = new NdmAction
		{
			CreatedDate = DateTime.Now
		};

		using (var tgt = new MemoryStream(publicData))
		using (var sr = new StreamReader(tgt))
		{
			var n500Audiogram = new N500Audiogram();
			n500Audiogram.Load(sr.ReadToEnd());
			foreach (var audiogram in n500Audiogram.ThreshodToneAudiogram)
				if (audiogram.MeasurementCondition.StimulusSignalOutput != SignalOutputEnum.NoSignalOutput &&
				    audiogram.MeasurementCondition.StimulusSignalOutput != SignalOutputEnum.Unknown)
				{
					var audiogramType = AudiogramTypeEnum.Threshold;
					if (audiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorLeft ||
					    audiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorRight ||
					    audiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorBinaural)
						audiogramType = AudiogramTypeEnum.BC;
					var ndmAudiogram = _Process500ToneAudiogram(audiogram, audiogramType);
					ndmAudiogram.PatientId = patientId;
					ndmAction.Audiograms.Add(ndmAudiogram);
				}

			foreach (var audiogram in n500Audiogram.MCLToneAudiogram)
				if (audiogram.MeasurementCondition.StimulusSignalOutput != SignalOutputEnum.NoSignalOutput &&
				    audiogram.MeasurementCondition.StimulusSignalOutput != SignalOutputEnum.Unknown)
				{
					var ndmAudiogram = _Process500ToneAudiogram(audiogram, AudiogramTypeEnum.MCL);
					ndmAudiogram.PatientId = patientId;
					ndmAction.Audiograms.Add(ndmAudiogram);
				}

			foreach (var audiogram in n500Audiogram.UCLToneAudiogram)
				if (audiogram.MeasurementCondition.StimulusSignalOutput != SignalOutputEnum.NoSignalOutput &&
				    audiogram.MeasurementCondition.StimulusSignalOutput != SignalOutputEnum.Unknown)
				{
					var ndmAudiogram = _Process500ToneAudiogram(audiogram, AudiogramTypeEnum.UCL);
					ndmAudiogram.PatientId = patientId;
					ndmAction.Audiograms.Add(ndmAudiogram);
				}
		}

		return ndmAction;
	}

	private NdmAction _ProcessN502Audiogram(byte[] publicData, int patientId)
	{
		var ndmAction = new NdmAction
		{
			CreatedDate = DateTime.Now
		};

		using (var tgt = new MemoryStream(publicData))
		using (var sr = new StreamReader(tgt))
		{
			var n500Audiogram = new N500Audiogram();
			n500Audiogram.Load(sr.ReadToEnd());
			foreach (var audiogram in n500Audiogram.ThreshodToneAudiogram)
				if (audiogram.MeasurementCondition.StimulusSignalOutput != SignalOutputEnum.NoSignalOutput &&
				    audiogram.MeasurementCondition.StimulusSignalOutput != SignalOutputEnum.Unknown)
				{
					var audiogramType = AudiogramTypeEnum.Threshold;
					if (audiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorLeft ||
					    audiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorRight ||
					    audiogram.MeasurementCondition.StimulusSignalOutput == SignalOutputEnum.BoneConductorBinaural)
						audiogramType = AudiogramTypeEnum.BC;
					var ndmAudiogram = _Process502ToneAudiogram(audiogram, audiogramType);
					ndmAudiogram.PatientId = patientId;
					ndmAction.Audiograms.Add(ndmAudiogram);
				}

			foreach (var audiogram in n500Audiogram.MCLToneAudiogram)
				if (audiogram.MeasurementCondition.StimulusSignalOutput != SignalOutputEnum.NoSignalOutput &&
				    audiogram.MeasurementCondition.StimulusSignalOutput != SignalOutputEnum.Unknown)
				{
					var ndmAudiogram = _Process502ToneAudiogram(audiogram, AudiogramTypeEnum.MCL);
					ndmAudiogram.PatientId = patientId;
					ndmAction.Audiograms.Add(ndmAudiogram);
				}

			foreach (var audiogram in n500Audiogram.UCLToneAudiogram)
				if (audiogram.MeasurementCondition.StimulusSignalOutput != SignalOutputEnum.NoSignalOutput &&
				    audiogram.MeasurementCondition.StimulusSignalOutput != SignalOutputEnum.Unknown)
				{
					var ndmAudiogram = _Process502ToneAudiogram(audiogram, AudiogramTypeEnum.UCL);
					ndmAudiogram.PatientId = patientId;
					ndmAction.Audiograms.Add(ndmAudiogram);
				}
		}

		return ndmAction;
	}

	private bool _ValidateOpportunityTrackingCriteria(List<NdmAudiogram> audiograms, NdmSearchCriteria criteria)
	{
		var meetsCriteria = false;
		if (criteria.IsPureTone)
		{
			var lastAudiogram = audiograms.FirstOrDefault(x => x.AudiogramType == (int)AudiogramTypeEnum.Threshold);
			meetsCriteria = _IsCriteriaMetForAudiogram(lastAudiogram, criteria);
		}

		if (!meetsCriteria && criteria.IsMcl)
		{
			var lastAudiogram = audiograms.FirstOrDefault(x => x.AudiogramType == (int)AudiogramTypeEnum.MCL);
			meetsCriteria = _IsCriteriaMetForAudiogram(lastAudiogram, criteria);
		}

		if (!meetsCriteria && criteria.IsUcl)
		{
			var lastAudiogram = audiograms.FirstOrDefault(x => x.AudiogramType == (int)AudiogramTypeEnum.UCL);
			meetsCriteria = _IsCriteriaMetForAudiogram(lastAudiogram, criteria);
		}

		if (criteria.IsBc)
		{
			var lastAudiogram = audiograms.FirstOrDefault(x => x.AudiogramType == (int)AudiogramTypeEnum.BC);
			meetsCriteria = _IsCriteriaMetForAudiogram(lastAudiogram, criteria);
		}

		return meetsCriteria;
	}

	public static bool AreConditionsMetForAudiogram(NdmAudiogram targetAudiogram, IEnumerable<NdmSearchPoint> points,
		int minPoints)
	{
		var numberOfPointsMatched = 0;
		foreach (var ndmSearchPoint in points)
		{
			// Get the Freq
			var target =
				targetAudiogram.TonePoints.FirstOrDefault(x => x.StimulusFrequency == ndmSearchPoint.Frequency);
			if (target != null)
			{
				var stimulusLevel = target.StimulusLevel == 0 ? 0 : target.StimulusLevel / 10;
				if (ndmSearchPoint.LevelLowerBound.HasValue && ndmSearchPoint.LevelUpperBound.HasValue)
				{
					if (stimulusLevel >= ndmSearchPoint.LevelLowerBound &&
					    stimulusLevel <= ndmSearchPoint.LevelUpperBound)
						numberOfPointsMatched++;
				}
				else if (ndmSearchPoint.LevelUpperBound.HasValue)
				{
					if (stimulusLevel <= ndmSearchPoint.LevelUpperBound) numberOfPointsMatched++;
				}
				else if (ndmSearchPoint.LevelLowerBound.HasValue)
				{
					if (stimulusLevel >= ndmSearchPoint.LevelLowerBound) numberOfPointsMatched++;
				}
			}
		}

		return numberOfPointsMatched >= minPoints;
	}
}