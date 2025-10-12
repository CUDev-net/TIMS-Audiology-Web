using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TIMS_X.Server.Legacy.Noah
{

	public class WordRecognitionData
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public WordRecognitionData()
		{
			WRResult1 = new WRResult( Constants.XML_WR_RESULT1 );
			WRResult2 = new WRResult( Constants.XML_WR_RESULT2 );
		}

		#endregion Constructors

		#region WordRecognitionData Members

		/// <summary>
		/// WR result 1
		/// </summary>
		public WRResult WRResult1
		{
			get;
			private set;
		}

		/// <summary>
		/// WR result 2
		/// </summary>
		public WRResult WRResult2
		{
			get;
			private set;
		}

		/// <summary>
		/// Takes the data from the XML element and exports it to the task
		/// </summary>
		/// <param name="xElement"></param>
		public void DeSerialize( XElement xElement )
		{
			foreach( var element in xElement.Elements() )
			{
				if( element.Name == Constants.XML_WR_RESULT1 )
				{
					WRResult1.DeSerialize( element );
				}
				else if( element.Name == Constants.XML_WR_RESULT2 )
				{
					WRResult2.DeSerialize( element );
				}
			}
		}

		/// <summary>
		/// Serializes the XML element
		/// </summary>
		/// <returns></returns>
		public XElement Serialize()
		{
			XElement element = new XElement( Constants.XML_WR_RESULT );

			if( WRResult1 != null )
			{
				element.Add( WRResult1.Serialize() );
			}
			if( WRResult2 != null )
			{
				element.Add( WRResult2.Serialize() );
			}
			return element;
		}

		#endregion WordRecognitionData Members

	}
}
