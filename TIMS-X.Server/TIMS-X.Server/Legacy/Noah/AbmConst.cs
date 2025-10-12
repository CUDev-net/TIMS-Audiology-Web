namespace TIMS_X.Server.Legacy.Noah
{

	/// <summary>
	/// Side the audiogram should display on
	/// </summary>
	public enum TestSide
	{
		/// <summary>
		/// Left side
		/// </summary>
		Left,
		/// <summary>
		/// Right
		/// </summary>
		Right,
		/// <summary>
		/// Both
		/// </summary>
		Both,
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown = 99

	}

	/// <summary>
	/// Static class for the audiogram constants
	/// </summary>
	public static class AbmConst
	{
		#region AbmConst Members

		public const int UNDEFINED = -32767;
		public const int NUMBER_THRAUDIOGRAMS = 18;
		public const int NUMBER_UCL_MCLAUDIOGRAMS = 6;
		public const int NUMBER_AUDIOGRAM_POINTS = 24;
		public const int AC = 0;
		public const int ACM = 4;
		public const int ART = 8;
		public const int BC = 1;
		public const int BBC = 9;
		public const int BCM = 5;
		public static readonly int BM2ndOff = 840;
		public static readonly int BMAddtl = 200;
		public static readonly int BMH = 660;
		public static readonly int BMW = 640;
		public static readonly int BMXTitleOff = 56;
		public static readonly int BMYTitleOff = 36;

		public const int FMAX = 30;
		public const int IP = 9;
		public const int MAXRESULTS = 9;
		public const int MCL = 3;
		public static readonly int NOTMEASURED = -400;
		public static readonly int OUTOFRANGE = 1550;
		public const int SF = 6;
		public const int SFA = 7;
		public const int UCL = 2;

		public static readonly int[] FrequencyTable = new [] { 125, 250, 500, 750, 1000,
																		 1500, 2000, 3000, 4000, 6000,
																		 8000, 10000, 12000, 12500, 14000,
																		 16000, 18000, 20000, 22000 };

		public static readonly int[] intensities = new int[] { -100, -50, 0, 50, 100,
																					150, 200, 250, 300, 350,
																					400, 450, 500, 550, 600,
																					650, 700, 750, 800, 850,
																					900, 950, 1000, 1050, 1100,
																					1150, 1200, 1250, 1300, 1350 };

		public static readonly int[] xpos = new int[]{ 75, 155, 231, 278, 310,
																	 357, 389, 435, 466, 513,
																	 544, 561, 591, 593, 605, 621 };

		public static readonly int[] ypos = new int[] {				  061, 082, 102, 123, 143,
																	  164, 184, 205, 225, 246,
																	  266, 287, 307, 328, 348,
																	  369, 389, 410, 430, 451,
																	  471, 492, 512, 533, 553,
																	  574, 594, 615, 635, 656 };

		#endregion AbmConst Members

	}
}
