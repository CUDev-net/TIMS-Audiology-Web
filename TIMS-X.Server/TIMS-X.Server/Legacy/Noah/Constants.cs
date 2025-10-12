using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMS_X.Server.Legacy.Noah
{
	public class Constants
	{
		public const string XML_TEST_SETUP = "TSETUP";
		public const string XML_TEST_AUDIOMETER = "TAMTR";
		public const string XML_TEST_CALIBRATION = "TCAL";
		public const string XML_TEST_RELIABILITY = "TREL";
		public const string XML_TEST_METHOD = "TMETHOD";
		public const string XML_TEST_TRANSDUCER = "TTRANSD";
		public const string XML_TEST_NOTES = "TNOTES";
		public const string XML_TEST_SERIAL_NUMBER = "TSERIALNUM";

		public const string XML_WR_RESULT = "WRRESULTS";
		public const string XML_WR_RESULT1 = "WRRESULT1";
		public const string XML_WR_RESULT2 = "WRRESULT2";
		public const string XML_WR_RESULT_DBHL = "WRDBHL";
		public const string XML_WR_RESULT_PERCENT = "WRPCNT";
		public const string XML_WR_RESULT_MASKING = "WRMSK";
		public const string XML_WR_RESULT_NUMBEROFWORDS = "WRWSK";


		public const string XML_SPEECH_RESULT = "SPEECHRESULTS";
		public const string XML_SPEECH_RESULT_SAT = "SPSAT";
		public const string XML_SPEECH_RESULT_SRT = "SPSRT";
		public const string XML_SPEECH_RESULT_MCL = "SPMCL";
		public const string XML_SPEECH_RESULT_UCL = "SPUCL";
		public const string XML_SPEECH_RESULT_MASKING = "SPMSK";

		public const string XML_SPEECH_RESULT_BINURAL = "SPR_BIN";
		public const string XML_SPEECH_RESULT_RIGHT = "SPR_RT";
		public const string XML_SPEECH_RESULT_LEFT = "SPR_LFT";

		public const string XML_SPEECH_RESULT_BINURAL_AIDED = "SPR_BINA";
		public const string XML_SPEECH_RESULT_RIGHT_AIDED = "SPR_RTA";
		public const string XML_SPEECH_RESULT_LEFT_AIDED = "SPR_LFTA";

		public const string XML_SPEECH_RESULT_SOUNDFIELD = "SPR_SF";
		public const string XML_SPEECH_RESULT_SOUNDFIELD_AIDED = "SPR_SFA";

		public const string XML_SOUNDFIELDAIDED = "SoundFieldAided";
		public const string XML_SOUNDFIELDCOCHLEARIMPLANT = "SoundFieldCochlearImplant";
		public const string XML_SOUNDFIELDHYBRID = "SoundFieldHybrid";
	}
}
