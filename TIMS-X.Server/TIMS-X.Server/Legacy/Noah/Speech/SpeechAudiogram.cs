using System.Collections.Generic;
using System.IO;

namespace TIMS_X.Server.Legacy.Noah.Speech
{
	/// <summary>
	/// Base Speech Audiogram
	/// </summary>
	public abstract class SpeechAudiogram
	{
		#region SpeechAudiogram Members

		/// <summary>
		/// Gets the number of points on the curve
		/// </summary>
		public abstract int NumberOfPoints
		{
			get;
		}

		/// <summary>
		/// Loads data from blob
		/// </summary>
		/// <param name="src"></param>
		public void SetData( MemoryStream src )
		{
			MeasuringCondition = new MeasCond();
			MeasuringCondition.SetData( src );

			Curve = new List<SpeechPoint>( NumberOfPoints );
			for( int index = 0; index < NumberOfPoints; index++ )
			{
				var point = new SpeechPoint();
				point.SetData( src );
				Curve.Add( point );
			}
		}

		#endregion SpeechAudiogram Members

		#region Internal Members

		/// <summary>
		/// Gets the list of curves
		/// </summary>
		internal List<SpeechPoint> Curve
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the measuring condition
		/// </summary>
		internal MeasCond MeasuringCondition
		{
			get;
			private set;
		}

		#endregion Internal Members

	}
}
