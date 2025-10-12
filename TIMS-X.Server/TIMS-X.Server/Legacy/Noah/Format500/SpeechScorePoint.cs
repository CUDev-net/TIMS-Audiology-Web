using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMS_X.Server.Legacy.Noah.Format500
{
	public class SpeechScorePoint
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public SpeechScorePoint()
		{
			ScorePercent = UNDEFINED;
			NumberOfWords = (int)UNDEFINED;
			MaskingLevel = UNDEFINED;
			StimulusLevel = UNDEFINED;
		}

		#endregion Constructors

		#region SpeechScorePoint Members

		public const decimal UNDEFINED = -1m;

		/// <summary>
		/// Masking level
		/// </summary>
		public decimal MaskingLevel
		{
			get;
			set;
		}

		/// <summary>
		/// Number of words
		/// </summary>
		public int NumberOfWords
		{
			get;
			set;
		}

		/// <summary>
		/// Score percent
		/// </summary>
		public decimal ScorePercent
		{
			get;
			set;
		}

		/// <summary>
		/// Stimulus level
		/// </summary>
		public decimal StimulusLevel
		{
			get;
			set;
		}

		#endregion SpeechScorePoint Members

	}
}
