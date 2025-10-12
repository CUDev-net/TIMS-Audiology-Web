using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.BLL.Enums;
using TIMS_X.BLL.VendorSync.Audigy;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.VendorSync.Repositories;

public interface IDataMiningRepository
{
    Task<List<HearingTest>> GetAudiograms(int patientId);
}

public class DataMiningRepository : IDataMiningRepository
{
    private readonly INdmAudiogramUnitOfWork _ndmAudiogramUnitOfWork;

    public DataMiningRepository(INdmAudiogramUnitOfWork ndmAudiogramUnitOfWork)
    {
        _ndmAudiogramUnitOfWork = ndmAudiogramUnitOfWork;
    }

    public async Task<List<HearingTest>> GetAudiograms(int patientId)
    {
        var ndmAudiograms = _ndmAudiogramUnitOfWork.GetAudiograms(
            a => a.PatientId == patientId, null, _Includes);
        var hearingTests = new List<HearingTest>();
        var results = await ndmAudiograms.ToListAsync();
        foreach (var action in results.ToLookup(g => g.ActionId))
        {
            var ndmAction = action.First().Action;
            var test = new HearingTest
            {
                Id = ndmAction.Id,
                PatientId = patientId,
                Date = ndmAction.AudiogramDate
            };
            foreach (var ndmAudiogram in action)
            {
                var audiogram = new Audiogram
                {
                    Id = ndmAudiogram.Id,
                    Side = ((NDMSideEnum)ndmAudiogram.Side).ToString(),
                    AudiogramType = ((AudiogramTypeEnum)ndmAudiogram.AudiogramType).ToString(),
                };
                if (ndmAudiogram.MeasurementCondition != null)
                {
                    audiogram.MeasurementCondition = new MeasurementCondition
                    {
                        Id = ndmAudiogram.MeasurementCondition.Id,
                        MaskingSignalType = ((NDMSignalTypeEnum)ndmAudiogram.MeasurementCondition.MaskingSignalType).ToString(),
                        StimulusPresentationType = ((PresentationTypeEnum)ndmAudiogram.MeasurementCondition.StimulusPresentationType).ToString(),
                        StimulusSignalOutput = ((SignalOutputEnum)ndmAudiogram.MeasurementCondition.StimulusSignalOutput).ToString(),
                        StimulusSignalType = ((NDMSignalTypeEnum)ndmAudiogram.MeasurementCondition.StimulusSignalType).ToString(),
                        StimulusdBWeighting = ((DBWeightingTypeEnum)ndmAudiogram.MeasurementCondition.StimulusdBWeighting).ToString(),
                        MaskingPresentationType = ((PresentationTypeEnum)ndmAudiogram.MeasurementCondition.MaskingPresentationType).ToString(),
                        MaskingSignalOutput = ((SignalOutputEnum)ndmAudiogram.MeasurementCondition.MaskingSignalOutput).ToString(),
                        MaskingdBWeighting = ((DBWeightingTypeEnum)ndmAudiogram.MeasurementCondition.MaskingdBWeighting).ToString(),
                        HearingInstrument_1_Condition = ((HearingTestConditionEnum)ndmAudiogram.MeasurementCondition.HearingInstrument1Condition).ToString(),
                        HearingInstrument_2_Condition = ((HearingTestConditionEnum)ndmAudiogram.MeasurementCondition.HearingInstrument2Condition).ToString(),

                    };
                }
                foreach (var ndmTonePoint in ndmAudiogram.TonePoints)
                {
                    var tonePoint = new TonePoint
                    {
                        Id = ndmTonePoint.Id,
                        MaskingFrequency = ndmTonePoint.MaskingFrequency,
                        StimulusFrequency = ndmTonePoint.StimulusFrequency,
                        MaskingLevel = ndmTonePoint.MaskingLevel,
                        StimulusLevel = ndmTonePoint.StimulusLevel / 10,
                        TonePointStatus = ((TonePointStatusEnum)ndmTonePoint.TonePointStatus).ToString(),
                    };
                    audiogram.TonePoints.Add(tonePoint);
                }
                test.Audiograms.Add(audiogram);
            }
            hearingTests.Add(test);
        }

        return hearingTests;
    }

    private IIncludableQueryable<NdmAudiogram, object> _Includes(IQueryable<NdmAudiogram> a)
    {
        return a.Include(x => x.TonePoints)
            .Include(x => x.MeasurementCondition)
            .Include(x => x.Action);
    }
}
