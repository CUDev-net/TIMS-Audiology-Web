using System;

namespace TIMS_X.Core.Attributes
{
	public class TimsObjectAttribute : Attribute
	{
		public TimsObjectAttribute()
		{
		}

		public TimsObjectAttribute(bool skipChildValidation)
		{
			SkipChildValidation = skipChildValidation;
		}

		public bool SkipChildValidation { get; }
	}
}