namespace TIMS_X.Server.Legacy.Noah
{
	public class UCLResult : SpeechResultBase
	{
		#region UCLResult Members

		/// <summary>
		/// Gets the description
		/// </summary>
		public override string Description
		{
			get
			{
				return "UCL";
			}
		}

		/// <summary>
		/// Gets the XML tag
		/// </summary>
		public override string XMLTag
		{
			get
			{
				return Constants.XML_SPEECH_RESULT_UCL;
			}
		}

		#endregion UCLResult Members

	}
}
