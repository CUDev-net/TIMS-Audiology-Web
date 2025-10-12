using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TIMS_X.Server.Legacy.Noah
{
	public class PureToneData
	{
		#region PuretToneData Members

		/// <summary>
		/// Gets PTA Left 2 frequencey test results
		/// </summary>
		public int? PTA2FrequencyLeft
		{
			get
			{
				if( mPTA500Left.HasValue && mPTA1000Left.HasValue )
				{
					//return (mPTA500Left.Value + mPTA1000Left.Value) / 20;
					return Convert.ToInt32( Math.Round( Convert.ToDecimal( mPTA500Left.Value + mPTA1000Left.Value ) / 20, MidpointRounding.AwayFromZero ) );
				}
				return null;
			}
		}

		/// <summary>
		/// Gets PTA Right 2 frequencey test results
		/// </summary>
		public int? PTA2FrequencyRight
		{
			get
			{
				if( mPTA500Right.HasValue && mPTA1000Right.HasValue )
				{
					//return (mPTA500Right.Value + mPTA1000Right.Value) / 20;
					return Convert.ToInt32( Math.Round( Convert.ToDecimal( mPTA500Right.Value + mPTA1000Right.Value ) / 20, MidpointRounding.AwayFromZero ) );
				}
				return null;
			}
		}

		/// <summary>
		/// Gets PTA Left 3 frequencey test results
		/// </summary>
		public int? PTA3FrequencyLeft
		{
			get
			{
				if( mPTA500Left.HasValue && mPTA1000Left.HasValue && mPTA2000Left.HasValue )
				{
					return Convert.ToInt32( Math.Round( Convert.ToDecimal( mPTA500Left.Value + mPTA1000Left.Value + mPTA2000Left.Value ) / 30, MidpointRounding.AwayFromZero ) );
				}
				return null;
			}
		}

		/// <summary>
		/// Gets PTA Right 3 frequencey test results
		/// </summary>
		public int? PTA3FrequencyRight
		{
			get
			{
				if( mPTA500Right.HasValue && mPTA1000Right.HasValue && mPTA2000Right.HasValue )
				{
					//return (mPTA500Right.Value + mPTA1000Right.Value + mPTA2000Right.Value) / 30;
					return Convert.ToInt32( Math.Round( Convert.ToDecimal( mPTA500Right.Value + mPTA1000Right.Value + mPTA2000Right.Value ) / 30, MidpointRounding.AwayFromZero ) );
				}
				return null;
			}
		}

		/// <summary>
		/// Gets PTA Left 4 frequencey test results
		/// </summary>
		public int? PTA4FrequencyLeft
		{
			get
			{
				if( mPTA500Left.HasValue && mPTA1000Left.HasValue && mPTA2000Left.HasValue && mPTA4000Left.HasValue )
				{
					//return (mPTA500Left.Value + mPTA1000Left.Value + mPTA2000Left.Value + mPTA4000Left.Value) / 40;
					return Convert.ToInt32( Math.Round( Convert.ToDecimal( mPTA500Left.Value + mPTA1000Left.Value + mPTA2000Left.Value + mPTA4000Left.Value ) / 40, MidpointRounding.AwayFromZero ) );
				}
				return null;
			}
		}

		/// <summary>
		/// Gets PTA Right 4 frequencey test results
		/// </summary>
		public int? PTA4FrequencyRight
		{
			get
			{
				if( mPTA500Right.HasValue && mPTA1000Right.HasValue && mPTA2000Right.HasValue && mPTA4000Right.HasValue )
				{
					//return (mPTA500Right.Value + mPTA1000Right.Value + mPTA2000Right.Value + mPTA4000Right.Value) / 40;
					return Convert.ToInt32( Math.Round( Convert.ToDecimal( mPTA500Right.Value + mPTA1000Right.Value + mPTA2000Right.Value + mPTA4000Right.Value ) / 40, MidpointRounding.AwayFromZero ) );
				}
				return null;
			}
		}

		/// <summary>
		/// Sets the data
		/// </summary>
		/// <param name="side"></param>
		/// <param name="frequency"></param>
		/// <param name="intensity"></param>
		public void SetPTAData( TestSide side, int frequency, int intensity )
		{
			if( side == TestSide.Left )
			{
				switch( frequency )
				{
					case 500:
						mPTA500Left = intensity;
						break;
					case 1000:
						mPTA1000Left = intensity;
						break;
					case 2000:
						mPTA2000Left = intensity;
						break;
					case 4000:
						mPTA4000Left = intensity;
						break;
				}
			}
			else
			{
				switch( frequency )
				{
					case 500:
						mPTA500Right = intensity;
						break;
					case 1000:
						mPTA1000Right = intensity;
						break;
					case 2000:
						mPTA2000Right = intensity;
						break;
					case 4000:
						mPTA4000Right = intensity;
						break;
				}
			}
		}

		#endregion PuretToneData Members

		#region Private Members

		private int? mPTA1000Left;
		private int? mPTA1000Right;
		private int? mPTA2000Left;
		private int? mPTA2000Right;
		private int? mPTA4000Left;
		private int? mPTA4000Right;
		private int? mPTA500Left;
		private int? mPTA500Right;

		#endregion Private Members

	}
}
