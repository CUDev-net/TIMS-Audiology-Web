using System;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Attributes
{
	public class AreaAttribute : Attribute
	{
		#region Constructors

		/// <summary>
		///     Constructor
		/// </summary>
		/// <param name="area"></param>
		public AreaAttribute(SettingAreaEnum area)
		{
			Area = area;
		}

		#endregion Constructors

		#region AreaAttribute Members

		public SettingAreaEnum Area { get; set; }

		#endregion AreaAttribute Members
	}
}