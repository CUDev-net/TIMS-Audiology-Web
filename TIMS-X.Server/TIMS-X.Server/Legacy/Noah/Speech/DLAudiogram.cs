namespace TIMS_X.Server.Legacy.Noah.Speech
{
	/// <summary>
	/// Contains the Word recognition data
	/// </summary>
	public class DLAudiogram : SpeechAudiogram
	{
		#region DLAudiogram Members

		/// <summary>
		/// Gets the number of points on the curve
		/// </summary>
		public override int NumberOfPoints
		{
			get
			{
				return 24;
			}
		}

		#endregion DLAudiogram Members

	}
}
