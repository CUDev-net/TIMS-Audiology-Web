using System;
using System.Collections.Generic;
using TIMS_X.BLL.VendorSync.Common;

namespace TIMS_X.BLL.VendorSync.Audigy;

public class HearingTest : DataSyncItem
{
    public HearingTest()
    {
        Audiograms = new List<Audiogram>();
    }

    public DateTime Date { get; set; }
    public int PatientId { get; set; }
    public List<Audiogram> Audiograms { get; set; }
}

public class Audiogram : DataSyncItem
{
    public Audiogram()
    {
        TonePoints = new List<TonePoint>();
        MeasurementCondition = new MeasurementCondition();
    }

    public MeasurementCondition MeasurementCondition { get; set; }
    public string Side { get; set; }
    public string AudiogramType { get; set; }
    public List<TonePoint> TonePoints { get; set; }
}

public class TonePoint : DataSyncItem
{
    public int MaskingFrequency { get; set; }
    public int StimulusFrequency { get; set; }
    public int MaskingLevel { get; set; }
    public int StimulusLevel { get; set; }
    public string TonePointStatus { get; set; }
}

public class MeasurementCondition : DataSyncItem
{
    public /*HearingTestConditionEnum*/ string HearingInstrument_1_Condition { get; set; }

    public /*HearingTestConditionEnum*/ string HearingInstrument_2_Condition { get; set; }

    public /*DBWeightingTypeEnum*/ string MaskingdBWeighting { get; set; }

    public /*PresentationTypeEnum*/ string MaskingPresentationType { get; set; }

    public /*SignalOutputEnum*/ string MaskingSignalOutput { get; set; }

    public /*NDMSignalTypeEnum*/ string MaskingSignalType { get; set; }

    public /*DBWeightingTypeEnum*/ string StimulusdBWeighting { get; set; }

    public /*PresentationTypeEnum*/ string StimulusPresentationType { get; set; }

    public /*SignalOutputEnum*/ string StimulusSignalOutput { get; set; }

    public /*NDMSignalTypeEnum*/ string StimulusSignalType { get; set; }
}