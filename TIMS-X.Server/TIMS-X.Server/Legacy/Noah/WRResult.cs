using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TIMS_X.Server.Legacy.Noah
{
	public class WRResult
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public WRResult( string name )
		{
			Name = name;
			DBHLResult = new WRDBHLResult();
			MaskingResult = new WRMaskingResult();
			NumberOfWordsResult = new WRNumberOfWordsResult();
			PercentResult = new WRPercentResult();
		}

		#endregion Constructors

		#region WRResult Members

		public WRDBHLResult DBHLResult
		{
			get;
			private set;
		}

		public WRMaskingResult MaskingResult
		{
			get;
			private set;
		}

		public WRNumberOfWordsResult NumberOfWordsResult
		{
			get;
			private set;
		}




		/// <summary>
		/// Gets the name for this result
		/// </summary>
		public string Name
		{
			get;
			private set;
		}

		public WRPercentResult PercentResult
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
				if( element.Name == Constants.XML_WR_RESULT_DBHL )
				{
					DBHLResult.DeSerialize( element );
				}
				else if( element.Name == Constants.XML_WR_RESULT_PERCENT )
				{
					PercentResult.DeSerialize( element );
				}
				else if( element.Name == Constants.XML_WR_RESULT_MASKING )
				{
					MaskingResult.DeSerialize( element );
				}
				else if( element.Name == Constants.XML_WR_RESULT_NUMBEROFWORDS )
				{
					NumberOfWordsResult.DeSerialize( element );
				}
			}
		}

		/// <summary>
		/// Serializes the XML element
		/// </summary>
		/// <returns></returns>
		public XElement Serialize()
		{
			XElement element = new XElement( Name );

			if( DBHLResult != null )
			{
				element.Add( DBHLResult.Serialize() );
			}
			if( PercentResult != null )
			{
				element.Add( PercentResult.Serialize() );
			}
			if( MaskingResult != null )
			{
				element.Add( MaskingResult.Serialize() );
			}
			if( NumberOfWordsResult != null )
			{
				element.Add( NumberOfWordsResult.Serialize() );
			}

			return element;
		}

		#endregion WRResult Members

	}
}
