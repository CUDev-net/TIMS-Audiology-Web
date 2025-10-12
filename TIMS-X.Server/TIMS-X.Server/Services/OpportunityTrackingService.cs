using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;
using TIMS_X.Server.Legacy.Noah;
using TIMS_X.Server.Legacy.Noah.Format500;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Utils;

namespace TIMS_X.Server.Services;

public class OpportunityTrackingService
{
	private readonly HaHistoryQuery _haHistoryQuery;
	private readonly INoahDataMiningService _noahDataMiningService;
	private readonly NoahQuery _noahQuery;
	private readonly PatientQuery _patientQuery;
	private readonly PracticeQuery _practiceQuery;

	public OpportunityTrackingService(PatientQuery patientQuery,
		HaHistoryQuery haHistoryQuery,
		PracticeQuery practiceQuery,
		NoahQuery noahQuery,
		INoahDataMiningService noahDataMiningService)
	{
		_patientQuery = patientQuery;
		_haHistoryQuery = haHistoryQuery;
		_practiceQuery = practiceQuery;
		_noahQuery = noahQuery;
		_noahDataMiningService = noahDataMiningService;
	}

	/// <summary>
	///     Gets the actual frequency for a test
	/// </summary>
	/// <param name="test"></param>
	/// <param name="side"></param>
	/// <param name="testResults"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	private decimal _GetTestFrequency(int test, TestSide side, AudiogramTestResults testResults, int index)
	{
		var frequency = 0.0m;
		for (var signalIndex = 0; signalIndex < AbmConst.NUMBER_THRAUDIOGRAMS; signalIndex++)
			if (testResults.GetTestResults(side).GetTest(test)[signalIndex][index] != null)
			{
				frequency = testResults.GetTestResults(side).GetTest(test)[signalIndex][index].Intensity1 / 10.0m;
				break;
			}

		return frequency;
	}

	private bool _IsAudiogramSideAboveThreshold(TestSide side, AudiogramTestResults testResults,
		OpportunityTrackingRules otRules)
	{
		var isAboveThreshold = _IsAudiogramTestAboveThreshold(AbmConst.AC, side, testResults, otRules);
		if (!isAboveThreshold)
			isAboveThreshold = _IsAudiogramTestAboveThreshold(AbmConst.BC, side, testResults, otRules);
		if (!isAboveThreshold)
			isAboveThreshold = _IsAudiogramTestAboveThreshold(AbmConst.MCL, side, testResults, otRules);

		if (!isAboveThreshold)
			isAboveThreshold = _IsAudiogramTestAboveThreshold(AbmConst.UCL, side, testResults, otRules);
		if (!isAboveThreshold)
			isAboveThreshold = _IsAudiogramTestAboveThreshold(AbmConst.SF, side, testResults, otRules);
		if (!isAboveThreshold)
			isAboveThreshold = _IsAudiogramTestAboveThreshold(AbmConst.SFA, side, testResults, otRules);
		if (!isAboveThreshold)
			isAboveThreshold = _IsAudiogramTestAboveThreshold(AbmConst.IP, side, testResults, otRules);
		return isAboveThreshold;
	}

	private bool _IsAudiogramTestAboveThreshold(int test, TestSide side, AudiogramTestResults testResults,
		OpportunityTrackingRules otRules)
	{
		var isAboveThreshold = false;

		var freq125 = _GetTestFrequency(test, side, testResults, 0);
		var freq250 = _GetTestFrequency(test, side, testResults, 1);
		var freq500 = _GetTestFrequency(test, side, testResults, 2);
		var freq750 = _GetTestFrequency(test, side, testResults, 3);
		var freq1000 = _GetTestFrequency(test, side, testResults, 4);
		var freq1500 = _GetTestFrequency(test, side, testResults, 5);
		var freq2000 = _GetTestFrequency(test, side, testResults, 6);
		var freq3000 = _GetTestFrequency(test, side, testResults, 7);
		var freq4000 = _GetTestFrequency(test, side, testResults, 8);
		var freq6000 = _GetTestFrequency(test, side, testResults, 9);
		var freq8000 = _GetTestFrequency(test, side, testResults, 10);

		if (freq125 > otRules.Threshold125 ||
		    freq250 > otRules.Threshold250 ||
		    freq500 > otRules.Threshold500 ||
		    freq750 > otRules.Threshold750 ||
		    freq1000 > otRules.Threshold1000 ||
		    freq1500 > otRules.Threshold1500 ||
		    freq2000 > otRules.Threshold2000 ||
		    freq3000 > otRules.Threshold3000 ||
		    freq4000 > otRules.Threshold4000 ||
		    freq6000 > otRules.Threshold6000 ||
		    freq8000 > otRules.Threshold8000)
			isAboveThreshold = true;
		return isAboveThreshold;
	}


	private async Task<Tuple<bool, bool>> _IsPatientEarAidableAsync(Patient patient, TestSide side,
		OpportunityTrackingRules otRules, bool useNoahDataMining)
	{
		if (useNoahDataMining) return await _noahDataMiningService.IsEarAidableAsync(patient, side);

		var isAidable = true;
		var hasAudiogram = false;
		var action = await _noahQuery.GetNewestActionAsync(patient.Id);
		if (action != null)
		{
			hasAudiogram = true;
			var publicData = CompressionHelper.DecompressAsync(action.PublicData);
			AudiogramTestResults testResults;
			using (var tgt = new MemoryStream(publicData))
			{
				if (action.DataFmtCodeStd == 200)
				{
					var n200Audiogram = new N200(tgt);
					testResults = n200Audiogram.GetResults();
				}
				else if (action.DataFmtCodeStd == 500)
				{
					using (var sr = new StreamReader(tgt))
					{
						var n500Audiogram = new N500Audiogram();
						n500Audiogram.Load(sr.ReadToEnd());
						testResults = n500Audiogram.GetResults();
					}
				}
				else
				{
					var n100Audiogram = new N100(tgt);
					testResults = n100Audiogram.GetResults();
				}
			}

			isAidable = _IsAudiogramSideAboveThreshold(side, testResults, otRules);
			if (!isAidable) isAidable = _IsAudiogramSideAboveThreshold(TestSide.Both, testResults, otRules);
		}

		return new Tuple<bool, bool>(hasAudiogram, isAidable);
	}

	public async Task<Tuple<OpportunityStatusEnum, string>> GetAppointmentOpportunityStatusAsync(int patientId)
	{
		var opportunityStatus = OpportunityStatusEnum.TwoEarsAidable;
		var details = "Patient is an opportunity";

		try
		{
			var patient = await _patientQuery.GetDomainPatientAsync(patientId);

			var businessRules = await _practiceQuery.GetBusinessRulesAsync();
			var practice = await _practiceQuery.GetPracticeAsync();
			var statusSet = false;
			var oneEarMarketable = false;

			var noMarketingRestriction =
				await _patientQuery.LookupRestrictionAsync(CommunicationRestriction.NO_MARKETING);
			if (noMarketingRestriction != null)
				if (patient.Restrictions.Any(x => x.RestrictionId == noMarketingRestriction.Id))
				{
					opportunityStatus = OpportunityStatusEnum.NoMarketing;
					details = "Patient is not an opportunity, has a communication restriction of No Marketing";
					statusSet = true;
				}

			//Log.WriteToLog(LogLevel.INFO, "Could not find no marketing communication restriction");
			if (!statusSet)
			{
				var noMarketing1EarRestriction =
					await _patientQuery.LookupRestrictionAsync(CommunicationRestriction.NO_MARKETING_1EAR);
				if (noMarketing1EarRestriction != null)
					if (patient.Restrictions.Any(x => x.RestrictionId == noMarketing1EarRestriction.Id))
						oneEarMarketable = true;
			}

			if (!statusSet)
			{
				var haHistories = await _haHistoryQuery.GetHaHistoriesAsync(patientId);
				var leftHaHistories = haHistories.Where(x => x.Side == HaSide.Left)
					.OrderByDescending(o => o.PurchaseDate).ThenBy(o => o.UpdatedDate);
				var rightHaHistories = haHistories.Where(x => x.Side == HaSide.Right)
					.OrderByDescending(o => o.PurchaseDate).ThenBy(o => o.UpdatedDate);

				HaHistory leftHistory = null, rightHistory = null;
				foreach (var haHistory in leftHaHistories)
				{
					var trackingType = (OpportunityTrackingType)haHistory.Status.OpptTrackingType;
					switch (trackingType)
					{
						case OpportunityTrackingType.None:
							break;
						case OpportunityTrackingType.Active:
							leftHistory = haHistory;
							break;
						case OpportunityTrackingType.ActiveWithThreshold:
							if (haHistory.PurchaseDate.HasValue)
								leftHistory = haHistory;
							break;
					}
				}

				foreach (var haHistory in rightHaHistories)
				{
					var trackingType = (OpportunityTrackingType)haHistory.Status.OpptTrackingType;
					switch (trackingType)
					{
						case OpportunityTrackingType.None:
							break;
						case OpportunityTrackingType.Active:
							rightHistory = haHistory;
							break;
						case OpportunityTrackingType.ActiveWithThreshold:
							if (haHistory.PurchaseDate.HasValue)
								rightHistory = haHistory;
							break;
					}
				}

				var leftAidExpired = false;
				var rightAidExpired = false;

				var hasOneEarMedicalCondition = false;
				var oneEarMedicalCondition =
					await _patientQuery.LookupMedicalConditionAsync(MedicalCondition.ONE_EAR_AIDABLE);
				if (oneEarMedicalCondition != null)
					hasOneEarMedicalCondition =
						patient.MedicalConditions.Any(x => x.ConditionId == oneEarMedicalCondition.Id);


				var activeLeftSide = false;
				if (leftHistory != null)
				{
					if ((OpportunityTrackingType)leftHistory.Status.OpptTrackingType ==
					    OpportunityTrackingType.ActiveWithThreshold)
					{
						// We have a left aid that is in use
						var timeLength = DateTime.Now.Subtract(leftHistory.PurchaseDate ?? new DateTime());
						activeLeftSide = timeLength.TotalDays < 30 * businessRules.OpportunityTracking.ThresholdMonths;
						if (!activeLeftSide) leftAidExpired = true;
					}
					else
					{
						activeLeftSide = true;
					}
				}

				var activeRightSide = false;
				if (rightHistory != null)
				{
					if ((OpportunityTrackingType)rightHistory.Status.OpptTrackingType ==
					    OpportunityTrackingType.ActiveWithThreshold)
					{
						// We have a left aid that is in use
						var timeLength = DateTime.Now.Subtract(rightHistory.PurchaseDate ?? new DateTime());
						activeRightSide = timeLength.TotalDays < 30 * businessRules.OpportunityTracking.ThresholdMonths;
						if (!activeRightSide) rightAidExpired = true;
					}
					else
					{
						activeRightSide = true;
					}
				}

				// Active hearing aid on both sides, no opportunity
				if (activeLeftSide && activeRightSide)
				{
					opportunityStatus = OpportunityStatusEnum.CurrentUser;
					details = "Patient is not an opportunity, has 2 current hearing aids";
				}
				else if (activeLeftSide || activeRightSide)
				{
					var activeEar = activeLeftSide ? "left" : "right";
					var otherEar = activeLeftSide ? "right" : "left";

					// Acitve hearing aid on one side, what about the other?
					if (oneEarMarketable)
					{
						// Do not market to other side
						opportunityStatus = OpportunityStatusEnum.NoOpportunity;
						details = string.Format(
							"Patient is not an opportunity, has current hearing aid for the {0} ear and No Marketing One Ear communication restriction",
							activeEar);
					}
					else if (rightAidExpired || leftAidExpired)
					{
						// One of the aids is out of date
						if (rightAidExpired)
						{
							opportunityStatus = OpportunityStatusEnum.ReSell1Ear;
							details =
								"Patient is an opportunity, has left hearing aid that current and right hearing aid that is not";
						}

						if (leftAidExpired)
						{
							opportunityStatus = OpportunityStatusEnum.ReSell1Ear;
							details =
								"Patient is an opportunity, has right hearing aid that current and left hearing aid that is not";
						}
					}
					else
					{
						// Is there hearing loss to the other side?
						bool oneEarAidable;
						bool hasTest;
						(hasTest, oneEarAidable) = await _IsPatientEarAidableAsync(patient,
							activeLeftSide ? TestSide.Right : TestSide.Left,
							businessRules.OpportunityTracking, practice.UsesNoahDataMining);

						if (oneEarAidable)
						{
							opportunityStatus = OpportunityStatusEnum.OneEarAidable;
							if (hasTest)
							{
								details = string.Format(
									"Patient is an opportunity, has current hearing aid on the {0} ear and hearing loss in the {1} ear",
									activeEar, otherEar);
								opportunityStatus = OpportunityStatusEnum.TestedNotSold1Ear;
							}
							else
							{
								if (hasOneEarMedicalCondition)
								{
									details = string.Format(
										"Patient is not an opportunity, has current hearing aid for the {0} ear and a medical condtion set to One Ear Aidable",
										activeEar);
									opportunityStatus = OpportunityStatusEnum.NoOpportunity;
								}
								else
								{
									details = string.Format(
										"Patient is an opportunity, has current hearing aid on the {0} ear and no hearing test for the {1} ear",
										activeEar, otherEar);
								}
							}
						}
						else
						{
							opportunityStatus = OpportunityStatusEnum.CurrentUser;
							details = string.Format(
								"Patient is a current user, has current hearing aid on the {0} ear and no hearing loss in the {1} ear",
								activeEar, otherEar);
						}
					}
				}
				else
				{
					// Not active on either side
					if (leftHistory != null && rightHistory != null)
					{
						// 2 outdated aids, resell them both
						if (oneEarMarketable)
						{
							opportunityStatus = OpportunityStatusEnum.ReSell1Ear;
							details =
								"Patient is an opportunity, has 1 hearing aid that is not current and No Marketing One Ear communication restriction";
						}
						else
						{
							opportunityStatus = OpportunityStatusEnum.ReSell2Ears;
							details = "Patient is an opportunity, has 2 hearing aids that are not current";
						}
					}
					else if (leftHistory != null || rightHistory != null)
					{
						var ear = leftHistory == null ? "right" : "left";
						// One side has an outdated HA History
						if (oneEarMarketable)
						{
							opportunityStatus = OpportunityStatusEnum.ReSell1Ear;
							details = string.Format(
								"Patient is an opportunity, has {0} hearing aid that is not current and No Marketing One Ear communication restriction",
								ear);
						}
						else
						{
							var testSide = leftHistory == null ? TestSide.Left : TestSide.Right;
							var (hasTest, earAidable) = await _IsPatientEarAidableAsync(patient, testSide,
								businessRules.OpportunityTracking, practice.UsesNoahDataMining);
							if (earAidable)
							{
								opportunityStatus = OpportunityStatusEnum.ReSell2Ears;
								if (hasTest)
								{
									details = string.Format(
										"Patient is an opportunity, has {0} hearing aid that is not current and hearing loss for the other ear",
										ear);
								}
								else
								{
									if (hasOneEarMedicalCondition)
										details = string.Format(
											"Patient is an opportunity, has {0} hearing aid that is not current and a medical condtion set to One Ear Aidable",
											ear);
									else
										details = string.Format(
											"Patient is an opportunity, has {0} hearing aid that is not current and no hearing test for the other ear",
											ear);
								}
							}
							else
							{
								opportunityStatus = OpportunityStatusEnum.ReSell1Ear;
								details = string.Format(
									"Patient is an opportunity, has {0} hearing aid that is not current and no hearing loss for the other ear",
									ear);
							}
						}
					}
					else
					{
						// No HA Histories, check if we have hearing loss
						bool hasTest, leftEarAidable, rightEarAidable;
						(hasTest, leftEarAidable) = await _IsPatientEarAidableAsync(patient, TestSide.Left,
							businessRules.OpportunityTracking, practice.UsesNoahDataMining);
						(hasTest, rightEarAidable) = await _IsPatientEarAidableAsync(patient, TestSide.Right,
							businessRules.OpportunityTracking, practice.UsesNoahDataMining);

						if (leftEarAidable && rightEarAidable)
						{
							if (oneEarMarketable)
							{
								if (hasTest)
								{
									details =
										"Patient is an opportunity, has one ear with hearing loss and No Marketing One Ear communication restriction";
									opportunityStatus = OpportunityStatusEnum.TestedNotSold1Ear;
								}
								else
								{
									if (hasOneEarMedicalCondition)
									{
										details =
											"Patient is an opportunity, has a medical condtion set to One Ear Aidable and No Marketing One Ear communication restriction";
										opportunityStatus = OpportunityStatusEnum.OneEarAidable;
									}
									else
									{
										details =
											"Patient is an opportunity, has no hearing test for one ear and No Marketing One Ear communication restriction";
										opportunityStatus = OpportunityStatusEnum.OneEarAidable;
									}
								}
							}
							else
							{
								if (hasTest)
								{
									details =
										"Patient is an opportunity, has no hearing aids and both ears have hearing loss";
									opportunityStatus = OpportunityStatusEnum.TestedNotSold;
								}
								else
								{
									if (hasOneEarMedicalCondition)
									{
										details =
											"Patient is an opportunity, has no hearing aids and a medical condtion set to One Ear Aidable";
										opportunityStatus = OpportunityStatusEnum.OneEarAidable;
									}
									else
									{
										details =
											"Patient is an opportunity, has no hearing aids and no hearing test for both ears";
										opportunityStatus = OpportunityStatusEnum.TwoEarsAidable;
									}
								}
							}
						}
						else if (leftEarAidable || rightEarAidable)
						{
							// Only one ear aidable
							if (oneEarMarketable)
							{
								opportunityStatus = OpportunityStatusEnum.NoMarketingOneEar;
								details =
									"Patient is not an opportunity, has one ear with hearing loss but has No Marketing One Ear communication restriction";
							}
							else
							{
								var ear = leftEarAidable ? "left" : "right";
								details = string.Format(
									"Patient is an opportunity, has {0} ear with hearing loss and no hearing loss for the other ear",
									ear);
								opportunityStatus = OpportunityStatusEnum.TestedNotSold1Ear;
							}
						}
						else
						{
							opportunityStatus = OpportunityStatusEnum.NoOpportunity;
							details = "Patient is not an opportunity, has no hearing loss in either ear";
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			details = "Error creating Opportunity Status.  Error: " + ex.Message;
			//Log.WriteToLog(LogLevel.ERROR, "Error creating OT Status.  Error: " + ex.Message);
		}

		return new Tuple<OpportunityStatusEnum, string>(opportunityStatus, details);
	}
}