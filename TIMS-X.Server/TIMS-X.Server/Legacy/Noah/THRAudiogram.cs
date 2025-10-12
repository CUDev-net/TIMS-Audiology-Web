using System.IO;
using TIMS_X.Server.Legacy.Noah.Enums;

namespace TIMS_X.Server.Legacy.Noah
{
	/// <summary>
	/// Threshold Audiogram, 24 points on a curve designating frequency and intensity and a measuring condition
	/// </summary>
	public class THRAudiogram
	{
		#region Constructors

		public THRAudiogram()
		{
			MeasuringCondition = new MeasCond();
			Curve = new TonePoint[24];
			for( int idx = 0; idx < 24; idx++ )
			{
				Curve[idx] = new TonePoint();
			}
		}

		#endregion Constructors

		#region THRAudiogram Members

		/// <summary>
		/// Gets the Hearing test condition
		/// </summary>
		public HearingTestConditionEnum Condition1
		{
			get
			{
				return MeasuringCondition.RCondition1;
			}
		}

		/// <summary>
		/// Gets the measuring condition
		/// </summary>
		public MeasCond MeasuringCondition
		{
			get;
			private set;
		}

		public SignalOutputEnum SignalOutput1
		{
			get
			{
				return MeasuringCondition.RSignalOutput1;
			}
		}

		public void SetData( MemoryStream src )
		{
			MeasuringCondition.SetData( src );
			for( int idx = 0; idx < 24; idx++ )
			{
				Curve[idx].SetData( src );
			}
		}

		/// <summary>
		/// Frequency of stimules channel
		/// </summary>
		public short TPFreq1( int idx )
		{
			return Curve[idx].TFreq1;
		}

		/// <summary>
		/// Frequency of masking channel
		/// </summary>
		public short TPFreq2( int idx )
		{
			return Curve[idx].TFreq2;
		}

		/// <summary>
		/// SPL of stimules channel X 10
		/// </summary>
		public short TPIntensity1( int idx )
		{
			return Curve[idx].TIntensity1;
		}

		/// <summary>
		/// SPL of masking channel X 10
		/// </summary>
		public short TPIntensity2( int idx )
		{
			return Curve[idx].TIntensity2;
		}

		public PointStatusEnum TPStatus( int idx )
		{
			return Curve[idx].TStatus;
		}

		#endregion THRAudiogram Members

		#region Private Members

		private TonePoint[] Curve;

		#endregion Private Members

	}
}
