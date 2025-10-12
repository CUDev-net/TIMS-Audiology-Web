using System;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Attributes
{
	public class CountriesAttribute : Attribute
	{
		#region Constructors

		/// <summary>
		///     Constructor
		/// </summary>
		/// <param name="country"></param>
		public CountriesAttribute(TimeZoneCountry[] countries)
		{
			Countries = countries;
		}

		#endregion Constructors

		#region AreaAttribute Members

		public TimeZoneCountry[] Countries { get; set; }

		#endregion AreaAttribute Members
	}
}