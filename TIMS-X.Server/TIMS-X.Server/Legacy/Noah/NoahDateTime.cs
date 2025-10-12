using System;
using System.IO;
using System.Text;

namespace TIMS_X.Server.Legacy.Noah
{
	public class NoahDateTime
	{
		#region NoahDateTime Members

		/// <summary>
		/// Gets the date piece
		/// </summary>
		public DateTime? Date
		{
			get
			{
				if( !string.IsNullOrEmpty( mDateTimeString ) )
				{
					DateTime temp;
					if( DateTime.TryParse( mDateTimeString, out temp ) )
					{
						return temp;
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Sets the data in the properties
		/// </summary>
		/// <param name="src"></param>
		public void SetData( MemoryStream src )
		{
			BinaryReader br = new BinaryReader( src, Encoding.Unicode );

			var length = br.ReadUInt16();
			if( length > 25 )
			{
				length = 0;
			}
			var temp = br.ReadChars( 25 );
			mDateTimeString = new string( temp, 0, length );

			length = br.ReadUInt16();
			if( length > 7 )
			{
				length = 0;
			}
			temp = br.ReadChars( 7 );
			mDateTimeString = new string( temp, 0, length );
		}

		#endregion NoahDateTime Members

		#region Private Members

		private string mDateTimeString;

	    #endregion Private Members

	}
}
