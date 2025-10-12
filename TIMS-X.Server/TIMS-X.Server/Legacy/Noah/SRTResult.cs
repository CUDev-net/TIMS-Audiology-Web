namespace TIMS_X.Server.Legacy.Noah
{
	public class SRTResult : SpeechResultBase
	{
		#region SRTResult Members

		/// <summary>
		/// Gets the description
		/// </summary>
		public override string Description
		{
			get
			{
				return "SRT";
			}
		}

		/// <summary>
		/// Gets the XML tag
		/// </summary>
		public override string XMLTag
		{
			get
			{
				return Constants.XML_SPEECH_RESULT_SRT;
			}
		}

		#endregion SRTResult Members

	}
}
