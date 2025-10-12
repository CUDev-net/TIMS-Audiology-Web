using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIMS_X.Server.Legacy.Noah
{
	public interface ISpeechSupport
	{
		#region ISpeechSupport Members

		/// <summary>
		/// Gets MCL results for binural
		/// </summary>
		SpeechResult MCLBinural
		{
			get;
		}

		/// <summary>
		/// Gets MCL results for left
		/// </summary>
		SpeechResult MCLLeft
		{
			get;
		}

		/// <summary>
		/// Gets MCL results for right
		/// </summary>
		SpeechResult MCLRight
		{
			get;
		}

		/// <summary>
		/// Gets SRT results for Binural
		/// </summary>
		SpeechResult SRTBinural
		{
			get;
		}

		/// <summary>
		/// Gets SRT results for left
		/// </summary>
		SpeechResult SRTLeft
		{
			get;
		}

		/// <summary>
		/// Gets SRT results for right
		/// </summary>
		SpeechResult SRTRight
		{
			get;
		}

		/// <summary>
		/// Gets MCL results for binural
		/// </summary>
		SpeechResult UCLBinural
		{
			get;
		}

		/// <summary>
		/// Gets MCL results for left
		/// </summary>
		SpeechResult UCLLeft
		{
			get;
		}

		/// <summary>
		/// Gets MCL results for right
		/// </summary>
		SpeechResult UCLRight
		{
			get;
		}

		/// <summary>
		/// Gets results for WR1 Binural
		/// </summary>
		SpeechResult WR1Binural
		{
			get;
		}

		/// <summary>
		/// Gets results for WR1 Left
		/// </summary>
		SpeechResult WR1Left
		{
			get;
		}

		/// <summary>
		/// Gets results for WR1 Right
		/// </summary>
		SpeechResult WR1Right
		{
			get;
		}

		/// <summary>
		/// Gets results for WR1 Binural
		/// </summary>
		SpeechResult WR2Binural
		{
			get;
		}

		/// <summary>
		/// Gets results for WR1 Left
		/// </summary>
		SpeechResult WR2Left
		{
			get;
		}

		/// <summary>
		/// Gets results for WR1 Right
		/// </summary>
		SpeechResult WR2Right
		{
			get;
		}

		/// <summary>
		/// Gets results for WR1 Binural
		/// </summary>
		SpeechResult WR1BinuralAided
		{
			get;
		}

		/// <summary>
		/// Gets results for WR1 Left
		/// </summary>
		SpeechResult WR1LeftAided
		{
			get;
		}

		/// <summary>
		/// Gets results for WR1 Right
		/// </summary>
		SpeechResult WR1RightAided
		{
			get;
		}

		/// <summary>
		/// Gets results for WR1 Binural
		/// </summary>
		SpeechResult WR2BinuralAided
		{
			get;
		}

		/// <summary>
		/// Gets results for WR1 Left
		/// </summary>
		SpeechResult WR2LeftAided
		{
			get;
		}

		/// <summary>
		/// Gets results for WR1 Right
		/// </summary>
		SpeechResult WR2RightAided
		{
			get;
		}


		#endregion ISpeechSupport Members

	}
}
