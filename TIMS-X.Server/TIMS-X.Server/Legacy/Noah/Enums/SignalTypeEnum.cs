namespace TIMS_X.Server.Legacy.Noah.Enums
{
	public enum SignalTypeEnum
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// No signal
		/// </summary>
		NoSignalApplied,
		/// <summary>
		/// Pure TOne
		/// </summary>
		PureTone,
		/// <summary>
		/// Warble
		/// </summary>
		Warble,
		/// <summary>
		/// Narrow band
		/// </summary>
		NarrowBandNoise,
		/// <summary>
		/// Speech
		/// </summary>
		SpeechNoise,
		/// <summary>
		/// White noise
		/// </summary>
		WhiteNoise,
		/// <summary>
		/// Pink noise
		/// </summary>
		PinkNoise,
		/// <summary>
		/// Aux
		/// </summary>
		AuxiliarySignal,
		/// <summary>
		/// Mic
		/// </summary>
		Microphone,
		/// <summary>
		/// User 1
		/// </summary>
		User1 = 21,
		/// <summary>
		/// User 2
		/// </summary>
		User2 = 22,
		/// <summary>
		/// User 3
		/// </summary>
		User3 = 23
	}
}
