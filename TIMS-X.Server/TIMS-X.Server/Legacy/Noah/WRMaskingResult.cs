using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMS_X.Server.Legacy.Noah
{
	public class WRMaskingResult : SpeechResultBase
	{
		#region WRMaskingResult Members

		/// <summary>
		/// Gets the description
		/// </summary>
		public override string Description
		{
			get
			{
				return "Mask";
			}
		}

		/// <summary>
		/// Gets the XML tag
		/// </summary>
		public override string XMLTag
		{
			get
			{
				return Constants.XML_WR_RESULT_MASKING;
			}
		}

		#endregion WRMaskingResult Members

	}
}
