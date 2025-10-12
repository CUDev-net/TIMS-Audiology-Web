namespace TIMS_X.Server.Legacy.Noah.Speech
{
	public class MCLAudiogram : SpeechAudiogram
	{
		#region MCLAudiogram Members

		/// <summary>
		/// Gets the number of points on the curve
		/// </summary>
		public override int NumberOfPoints
		{
			get
			{
				return 1;
			}
		}

		#endregion MCLAudiogram Members

	}
}
