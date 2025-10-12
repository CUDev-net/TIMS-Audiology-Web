namespace TIMS_X.Server.Legacy.Noah
{
	public class MaskingResult : SpeechResultBase
	{
		#region MaskingResult Members

		/// <summary>
		/// Gets the description
		/// </summary>
		public override string Description
		{
			get
			{
				return "Masking";
			}
		}

		/// <summary>
		/// Gets the XML tag
		/// </summary>
		public override string XMLTag
		{
			get
			{
				return Constants.XML_SPEECH_RESULT_MASKING;
			}
		}

		#endregion MaskingResult Members

	}
}
