using System.Xml.Linq;

namespace TIMS_X.Server.Legacy.Noah
{
	public class SpeechData
	{
		#region Constructors

		/// <summary>
		/// Construcot
		/// </summary>
		public SpeechData()
		{
			MaskingData = new MaskingResult();
			MCLData = new MCLResult();
			UCLData = new UCLResult();
			SRTData = new SRTResult();
			SATData = new SATResult();
		}

		#endregion Constructors

		#region SpeechData Members

		/// <summary>
		/// Gets Masking data
		/// </summary>
		public MaskingResult MaskingData
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets MCL data
		/// </summary>
		public MCLResult MCLData
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets SAT data
		/// </summary>
		public SATResult SATData
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets SRT data
		/// </summary>
		public SRTResult SRTData
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets UCL data
		/// </summary>
		public UCLResult UCLData
		{
			get;
			private set;
		}

		/// <summary>
		/// Takes the data from the XML element and exports it to the task
		/// </summary>
		/// <param name="xElement"></param>
		public virtual void DeSerialize( XElement xElement )
		{
			foreach( var element in xElement.Elements() )
			{
				if( element.Name == Constants.XML_SPEECH_RESULT_MCL )
				{
					MCLData.DeSerialize( element );
				}
				else if( element.Name == Constants.XML_SPEECH_RESULT_UCL )
				{
					UCLData.DeSerialize( element );
				}
				else if( element.Name == Constants.XML_SPEECH_RESULT_SRT )
				{
					SRTData.DeSerialize( element );
				}
				else if( element.Name == Constants.XML_SPEECH_RESULT_SAT )
				{
					SATData.DeSerialize( element );
				}
				else if( element.Name == Constants.XML_SPEECH_RESULT_MASKING )
				{
					MaskingData.DeSerialize( element );
				}
			}
		}

		/// <summary>
		/// Serializes the XML element
		/// </summary>
		/// <returns></returns>
		public virtual XElement Serialize()
		{
			XElement element = new XElement( Constants.XML_SPEECH_RESULT );

			if( SRTData != null )
			{
				element.Add( SRTData.Serialize() );
			}
			if( SATData != null )
			{
				element.Add( SATData.Serialize() );
			}
			if( UCLData != null )
			{
				element.Add( UCLData.Serialize() );
			}
			if( SRTData != null )
			{
				element.Add( SRTData.Serialize() );
			}
			if( MCLData != null )
			{
				element.Add( MCLData.Serialize() );
			}
			if( MaskingData != null )
			{
				element.Add( MaskingData.Serialize() );
			}
			return element;
		}

		#endregion SpeechData Members

	}
}
