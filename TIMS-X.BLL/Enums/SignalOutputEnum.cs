namespace TIMS_X.BLL.Enums;

public enum SignalOutputEnum
{
    NotSet = -32767,
    Unknown = 0,
    NoSignalOutput,
    AirConductorLeft,
    AirConductorRight,
    AirConductorBinaural,
    BoneConductorLeft,
    BoneConductorRight,
    BoneConductorBinaural,
    FreeFieldLeft,
    FreeFieldRight,
    FreeFieldBinaural,
    InsertPhoneLeft,
    InsertPhoneRight,
    InsertPhoneBinaural,
    User1 = 21,
    User2 = 22,
    User3 = 23
}