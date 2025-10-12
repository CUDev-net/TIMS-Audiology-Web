namespace TIMS_X.Server.Legacy.Noah
{
	public class SATResult : SpeechResultBase
	{
		#region SATResult Members

		/// <summary>
		/// Gets the description
		/// </summary>
		public override string Description
		{
			get
			{
				return "SAT";
			}
		}

		/// <summary>
		/// Gets the XML tag
		/// </summary>
		public override string XMLTag
		{
			get
			{
				return Constants.XML_SPEECH_RESULT_SAT;
			}
		}

		#endregion SATResult Members

	}
}
