using System.IO;
using TIMS_X.Server.Legacy.Noah.Enums;

namespace TIMS_X.Server.Legacy.Noah
{
	public class THRAudiogram100
	{
		public MeasCond100 MeasuringCondition
		{
			get { return TMeasCond; }
		}

		private MeasCond100 TMeasCond; 	// Measuring Conditions
		private TonePoint[] Curve;	// 24 Threshold points

		public THRAudiogram100()
		{
			TMeasCond = new MeasCond100();
			Curve = new TonePoint[24];
			for(int idx = 0; idx < 24; idx++)
			{
				Curve[idx] = new TonePoint();
			}
		}

		public void SetData(MemoryStream src)
		{
			TMeasCond.SetData(src);
			for (int idx = 0; idx < 24; idx++)
			{
				Curve[idx].SetData(src);
			}
		}

		public short SignalOutput1{get{return TMeasCond.RSignalOutput1;}}
		public short Condition1{get{return TMeasCond.RCondition1;}}

		public short TPFreq1(int idx)
		{
			return Curve[idx].TFreq1;
		}

		public short TPFreq2(int idx)
		{
			return Curve[idx].TFreq2;
		}

		public short TPIntensity1(int idx)
		{
			return Curve[idx].TIntensity1;
		}

		public short TPIntensity2(int idx)
		{
			return Curve[idx].TIntensity2;
		}

		public PointStatusEnum TPStatus( int idx )
		{
			return Curve[idx].TStatus;
		}
	}
}
