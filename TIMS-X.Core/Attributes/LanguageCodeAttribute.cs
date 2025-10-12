using System;

namespace TIMS_X.Core.Attributes
{
	public class LanguageCodeAttribute : Attribute
	{
		/// <summary>
		///     Constructor
		/// </summary>
		/// <param name="code"></param>
		public LanguageCodeAttribute(string code)
		{
			Code = code;
		}


		/// <summary>
		///     Gets or sets the
		/// </summary>
		public string Code { get; set; }
	}
}