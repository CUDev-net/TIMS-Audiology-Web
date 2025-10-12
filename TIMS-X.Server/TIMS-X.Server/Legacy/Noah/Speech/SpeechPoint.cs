using System.IO;

namespace TIMS_X.Server.Legacy.Noah.Speech
{
	internal class SpeechPoint
	{

		/// <summary>
		/// Loads data from blob
		/// </summary>
		/// <param name="src"></param>
		public void SetData( MemoryStream src )
		{
			var br = new BinaryReader( src );
			Intensity1 = br.ReadInt16();
			Intensity2 = br.ReadInt16();
			ScorePercent = br.ReadInt16();
			Words = br.ReadInt16();
		}

		/// <summary>
		/// Gets the intensity of stimulus channel
		/// </summary>
		public short Intensity1
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the intensity of masking channel
		/// </summary>
		public short Intensity2
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the score percent
		/// </summary>
		public short ScorePercent
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the number of words
		/// </summary>
		public short Words
		{
			get;
			private set;
		}




	}
}
