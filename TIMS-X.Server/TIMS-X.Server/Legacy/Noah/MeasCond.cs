using System.IO;
using System.Text;
using TIMS_X.Server.Legacy.Noah.Enums;

namespace TIMS_X.Server.Legacy.Noah
{
	public class MeasCond
	{
		//TSignalType
		private short SignalType1;
		private short SignalType2;
		//THertz
		private short WarbleModFreq1;
		private short WarbleModFreq2;
		//TPct100
		private short WarbleModSize1;
		private short WarbleModSize2;
		//TAUXParm
		private short AUXParm1;
		private short AUXParm2;
		//TSignalOutput
		private short SignalOutput1;
		private short SignalOutput2;
		//TPresentType
		private short PresentType1;
		private short PresentType2;
		//THertz10
		private short PulseModFreq1;
		private short PulseModFreq2;
		//TPct100
		private short PulseDutyCycle1;
		private short PulseDutyCycle2;
		//TdB10
		private short AMModSize1;
		private short AMModSize2;
		//TPct100
		private short FMModSize1;
		private short FMModSize2;
		//TTime1000
		private short OnTime1;
		private short onTime2; 
		//TTime1000
		private short OffTime1;
		private short OffTime2; 
		//TdB10
		private short SiSiParm1;
		private short SiSiParm2;
		//TTransType
		private short TransType1;
		private short TransType2;
		//TTransCalStand
		private short TransCalStand1;
		private short TransCalStand2;
		//TdBWeighting
		private short dBWeighting1;
		private short dBWeighting2;
		//THICondition
		private short Condition1;
		private short Condition2;
		//SpeechThresholdType
		private short SpeechThresType;
		//unsigned short
		private ushort mLenHIDescr;	// Length of AidDescr1     2 oct
		//UCODE
		private ushort []mHIDescr;    	// 16 Aid Description        32 oct	unsigned short	LenAUXParmDescr;	// Length of AUXParmDescr  2 oct
		//UCODE
		private ushort []mAUXParmDescr;	// 16 AUX Parameter Descr.   32 oct	unsigned short	LenWordListName;	// Length of TransDescr -  2 oct
		//unsigned short
		private ushort mLenAUXParmDescr;	// XXXxxxXXX
		//UCODE
		private ushort []mWordListName; 	// 16 Word List Name         32 oct 
		//unsigned short
		private ushort mLenWordListName;	// XXXxxxXXX
		// total length 206

		public MeasCond()
		{
			mHIDescr = new ushort[16];
			mAUXParmDescr = new ushort[16];
			mWordListName = new ushort[16];
			for(int idx = 0; idx < 16; idx++)
			{
				mHIDescr[idx] = 0;
				mAUXParmDescr[idx] = 0;
				mWordListName[idx] = 0;
			}

			SignalType1 = 0;
			SignalType2 = 0;
			WarbleModFreq1 = 0;
			WarbleModFreq2 = 0;
			WarbleModSize1 = 0;
			WarbleModSize2 = 0;
			AUXParm1 = 0;
			AUXParm2 = 0;
			SignalOutput1 = 0;
			SignalOutput2 = 0;
			PresentType1 = 0;
			PresentType2 = 0;
			PulseModFreq1 = 0;
			PulseModFreq2 = 0;
			PulseDutyCycle1 = 0;
			PulseDutyCycle2 = 0;
			AMModSize1 = 0;
			AMModSize2 = 0;
			FMModSize1 = 0;
			FMModSize2 = 0;
			OnTime1 = 0;
			onTime2 = 0;
			OffTime1 = 0;
			OffTime2 = 0;
			SiSiParm1 = 0;
			SiSiParm2 = 0;
			TransType1 = 0;
			TransType2 = 0;
			TransCalStand1 = 0;
			TransCalStand2 = 0;
			dBWeighting1 = 0;
			dBWeighting2 = 0;
			Condition1 = 0;
			Condition2 = 0;
			SpeechThresType = 0;
			mLenHIDescr = 0;
			mLenAUXParmDescr = 0;
			mLenWordListName = 0;
		}

		public void SetData(MemoryStream src)
		{
			BinaryReader br = new BinaryReader( src, Encoding.Unicode );
			SignalType1 = br.ReadInt16();
			SignalType2 = br.ReadInt16();
			WarbleModFreq1 = br.ReadInt16();
			WarbleModFreq2 = br.ReadInt16();
			WarbleModSize1 = br.ReadInt16();
			WarbleModSize2 = br.ReadInt16();
			AUXParm1 = br.ReadInt16();
			AUXParm2 = br.ReadInt16();
			SignalOutput1 = br.ReadInt16();
			SignalOutput2 = br.ReadInt16();
			PresentType1 = br.ReadInt16();
			PresentType2 = br.ReadInt16();
			PulseModFreq1 = br.ReadInt16();
			PulseModFreq2 = br.ReadInt16();
			PulseDutyCycle1 = br.ReadInt16();
			PulseDutyCycle2 = br.ReadInt16();
			AMModSize1 = br.ReadInt16();
			AMModSize2 = br.ReadInt16();
			FMModSize1 = br.ReadInt16();
			FMModSize2 = br.ReadInt16();
			OnTime1 = br.ReadInt16();
			onTime2 = br.ReadInt16(); 
			OffTime1 = br.ReadInt16();
			OffTime2 = br.ReadInt16();
			SiSiParm1 = br.ReadInt16();
			SiSiParm2 = br.ReadInt16();
			TransType1 = br.ReadInt16();
			TransType2 = br.ReadInt16();
			TransCalStand1 = br.ReadInt16();
			TransCalStand2 = br.ReadInt16();
			dBWeighting1 = br.ReadInt16();
			dBWeighting2 = br.ReadInt16();
			Condition1 = br.ReadInt16();
			Condition2 = br.ReadInt16();
			SpeechThresType = br.ReadInt16();
			var length = br.ReadUInt16();
			if( length > 16 )
			{
				length = 0;
			}
			var temp = br.ReadChars(16);
			TransducerDescription = new string( temp, 0, length );

			mLenHIDescr = br.ReadUInt16();
			if( mLenHIDescr > 16 )
			{
				mLenHIDescr = 0;
			}
			for(int idx = 0; idx < 16; idx++)
			{
				mHIDescr[idx] = br.ReadUInt16();
			}

			mLenAUXParmDescr = br.ReadUInt16();
			if( mLenAUXParmDescr > 16 )
			{
				mLenAUXParmDescr = 0;
			}
			for(int idx = 0; idx < 16; idx++)
			{
				mAUXParmDescr[idx] = br.ReadUInt16();
			}

			mLenWordListName = br.ReadUInt16();
			if( mLenWordListName > 16 )
			{
				mLenWordListName = 0;
			}
			for(int idx = 0; idx < 16; idx++)
			{
				mWordListName[idx] = br.ReadUInt16();
			}
		}

		/// <summary>
		/// Gets the transducer description
		/// </summary>
		public string TransducerDescription
		{
			get;
			private set;
		}

		public SignalTypeEnum RSignalType1
		{
			get
			{
				return (SignalTypeEnum)SignalType1;
			}
		}
		public SignalTypeEnum RSignalType2
		{
			get
			{
				return (SignalTypeEnum)SignalType2;
			}
		}
		public short RWarbleModFreq1{get{return WarbleModFreq1;}}
		public short RWarbleModFreq2{get{return WarbleModFreq2;}}
		public short RWarbleModSize1{get{return WarbleModSize1;}}
		public short RWarbleModSize2{get{return WarbleModSize2;}}
		public short RAUXParm1{get{return AUXParm1;}}
		public short RAUXParm2{get{return AUXParm2;}}
		public SignalOutputEnum RSignalOutput1
		{
			get
			{
				return (SignalOutputEnum)SignalOutput1;
			}
		}
		public SignalOutputEnum RSignalOutput2
		{
			get
			{
				return (SignalOutputEnum)SignalOutput2;
			}
		}
		public short RPresentType1{get{return PresentType1;}}
		public short RPresentType2{get{return PresentType2;}}
		public short RPulseModFreq1{get{return PulseModFreq1;}}
		public short RPulseModFreq2{get{return PulseModFreq2;}}
		public short RPulseDutyCycle1{get{return PulseDutyCycle1;}}
		public short RPulseDutyCycle2{get{return PulseDutyCycle2;}}
		public short RAMModSize1{get{return AMModSize1;}}
		public short RAMModSize2{get{return AMModSize2;}}
		public short RFMModSize1{get{return FMModSize1;}}
		public short RFMModSize2{get{return FMModSize2;}}
		public short ROnTime1{get{return OnTime1;}}
		public short RonTime2{get{return onTime2;}}
		public short ROffTime1{get{return OffTime1;}}
		public short ROffTime2{get{return OffTime2;}}
		public short RSiSiParm1{get{return SiSiParm1;}}
		public short RSiSiParm2{get{return SiSiParm2;}}
		public short RTransType1{get{return TransType1;}}
		public short RTransType2{get{return TransType2;}}
		public short RTransCalStand1{get{return TransCalStand1;}}
		public short RTransCalStand2{get{return TransCalStand2;}}
		public short RdBWeighting1{get{return dBWeighting1;}}
		public short RdBWeighting2{get{return dBWeighting2;}}
		public HearingTestConditionEnum RCondition1
		{
			get
			{
				return (HearingTestConditionEnum)Condition1;
			}
		}
		public HearingTestConditionEnum RCondition2
		{
			get
			{
				return (HearingTestConditionEnum)Condition2;
			}
		}
		public short RSpeechThresType{get{return SpeechThresType;}}
		public ushort RLenHIDescr{get{return mLenHIDescr;}}
		public ushort[] RHIDescr{get{return mHIDescr;}}
		public ushort[] RAUXParmDescr{get{return mAUXParmDescr;}}
		public ushort RLenAUXParmDescr{get{return mLenAUXParmDescr;}}
		public ushort[] RWordListName{get{return mWordListName;}}
		public ushort RLenWordListName{get{return mLenWordListName;}}

	}
}
