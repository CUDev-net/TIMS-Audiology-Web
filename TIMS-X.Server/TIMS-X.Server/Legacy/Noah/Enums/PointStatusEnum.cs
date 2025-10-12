namespace TIMS_X.Server.Legacy.Noah.Enums
{
	/// <summary>
	/// Stats of an audigram point
	/// </summary>
	public enum PointStatusEnum
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// Nothing
		/// </summary>
		Normal,
		/// <summary>
		/// 
		/// </summary>
		AlwaysResponse,
		/// <summary>
		/// No response
		/// </summary>
		NoResponse,
		/// <summary>
		/// Unmeasurable
		/// </summary>
		NotMeasurable,
		/// <summary>
		/// Did not test
		/// </summary>
		DidNotTest,
		/// <summary>
		/// Could not test
		/// </summary>
		CouldNotTest,
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
		User3 = 23,
		/// <summary>
		/// Not set
		/// </summary>
		NotSet = 999
	}
}
