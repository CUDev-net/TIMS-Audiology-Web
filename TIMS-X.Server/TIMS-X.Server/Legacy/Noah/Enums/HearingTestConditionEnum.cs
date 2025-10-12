namespace TIMS_X.Server.Legacy.Noah.Enums
{
	/// <summary>
	/// Hearing Instrument Condition for the test
	/// </summary>
	public enum HearingTestConditionEnum
	{
		/// <summary>
		/// HI Condition for test unknown
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// HI Condition unavailable
		/// </summary>
		NoCondition = 2,
		/// <summary>
		/// Measured without HI
		/// </summary>
		UnAided = 1,
		/// <summary>
		/// Measured with HI 1
		/// </summary>
		Aided1 = 3,
		/// <summary>
		/// Measured with HI 2
		/// </summary>
		Aided2 = 4,
		/// <summary>
		/// User defined value 1
		/// </summary>
		User1 = 21,
		/// <summary>
		/// User defined value 2
		/// </summary>
		User2 = 22,
		/// <summary>
		/// User defined value 3
		/// </summary>
		User3 = 23,
		/// <summary>
		/// Not set yet
		/// </summary>
		NotSet = 999

	}
}
