namespace TIMS_X.Server.Legacy.Noah
{
	public class MCLResult : SpeechResultBase
	{
		#region MCLResult Members

		/// <summary>
		/// Gets the description
		/// </summary>
		public override string Description
		{
			get
			{
				return "MCL";
			}
		}

		/// <summary>
		/// Gets the XML tag
		/// </summary>
		public override string XMLTag
		{
			get
			{
				return Constants.XML_SPEECH_RESULT_MCL;
			}
		}

		#endregion MCLResult Members

	}
}
