using System.IO;
using TIMS_X.Server.Legacy.Noah.Enums;

namespace TIMS_X.Server.Legacy.Noah
{
	class TonePoint
	{
		#region Constructors

		public TonePoint()
		{
			Freq1 = 0;
			Intensity1 = 0;
			Freq2 = 0;
			Intensity2 = 0;
			Status = 0;
		}

		#endregion Constructors

		#region TonePoint Members

		/// <summary>
		/// Frequency of stimulus channel
		/// </summary>
		public short TFreq1
		{
			get
			{
				return Freq1;
			}
		}

		/// <summary>
		/// Frequency of masking channel
		/// </summary>
		public short TFreq2
		{
			get
			{
				return Freq2;
			}
		}

		/// <summary>
		/// SPL of stimulus channel
		/// </summary>
		public short TIntensity1
		{
			get
			{
				return Intensity1;
			}
		}

		/// <summary>
		/// SPL of masking channel
		/// </summary>
		public short TIntensity2
		{
			get
			{
				return Intensity2;
			}
		}

		public PointStatusEnum TStatus
		{
			get
			{
				return (PointStatusEnum)Status;
			}
		}

		public void SetData( MemoryStream src )
		{
			BinaryReader br = new BinaryReader( src );
			Freq1 = br.ReadInt16();
			Intensity1 = br.ReadInt16();
			Freq2 = br.ReadInt16();
			Intensity2 = br.ReadInt16();
			Status = br.ReadInt16();
		}

		#endregion TonePoint Members

		#region Private Members

		// THertz
		private short Freq1;
		//THertz
		private short Freq2;
		//TdB10
		private short Intensity1;
		//TdB10
		private short Intensity2;
		//TPointStatus
		private short Status;

		#endregion Private Members

	}
}
