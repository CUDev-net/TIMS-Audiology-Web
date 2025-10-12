using System;

namespace TIMS_X.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class TimsCollectionAttribute : Attribute
	{
		public TimsCollectionAttribute()
		{
		}

		public TimsCollectionAttribute(bool skipChildValidation)
		{
			SkipChildValidation = skipChildValidation;
		}

		public bool SkipChildValidation { get; }
	}
}