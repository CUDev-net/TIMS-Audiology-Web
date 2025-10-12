namespace TIMS_X.Server.Legacy.Noah.Speech
{
	public class SRTAudiogram : SpeechAudiogram
	{
		#region SRTAudiogram Members

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

		#endregion SRTAudiogram Members

	}
}
