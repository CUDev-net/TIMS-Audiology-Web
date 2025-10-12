using System;

namespace TIMS_X.Core.Attributes
{
	public class XmlTagAttribute : Attribute
	{
		#region Constructors

		/// <summary>
		///     Constructor
		/// </summary>
		/// <param name="area"></param>
		public XmlTagAttribute(string name)
		{
			Name = name;
		}

		#endregion Constructors

		#region AreaAttribute Members

		public string Name { get; set; }

		#endregion AreaAttribute Members
	}
}