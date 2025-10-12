using System;
using System.IO;
using System.Text;

namespace TIMS_X.Server.Legacy.Noah
{
	public class MeasurementNotes
	{
		#region MeasurementNotes Members

		/// <summary>
		/// Gets the last calibration date
		/// </summary>
		public DateTime? LastCalibrationDate
		{
			get
			{
				return mLastCalibrationDate.Date;
			}
		}

		/// <summary>
		/// Gets the make and model
		/// </summary>
		public string MakeModel
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the make and model
		/// </summary>
		public string SerialNumber
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the make and model
		/// </summary>
		public string TestMethod
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the make and model
		/// </summary>
		public string TestReliability
		{
			get;
			private set;
		}

		/// <summary>
		/// Sets the data from the blob to the properties
		/// </summary>
		/// <param name="src"></param>
		public void SetData( MemoryStream src )
		{
			BinaryReader br = new BinaryReader( src, Encoding.Unicode );

			var length = br.ReadUInt16();
			if( length > 41 )
			{
				length = 0;
			}

			var temp = br.ReadChars( 41 );
			MakeModel = new string( temp, 0, length );

			length = br.ReadUInt16();
			if( length > 41 )
			{
				length = 0;
			}
			temp = br.ReadChars( 41 );
			SerialNumber = new string( temp, 0, length );

			mLastCalibrationDate = new NoahDateTime();
			mLastCalibrationDate.SetData( src );

			length = br.ReadUInt16();
			if( length > 41 )
			{
				length = 0;
			}
			temp = br.ReadChars( 41 );
			TestMethod = new string( temp, 0, length );

			length = br.ReadUInt16();
			if( length > 41 )
			{
				length = 0;
			}
			temp = br.ReadChars( 41 );
			TestReliability = new string( temp, 0, length );
		}

		#endregion MeasurementNotes Members

		#region Private Members

		private NoahDateTime mLastCalibrationDate;

		#endregion Private Members

	}
}
