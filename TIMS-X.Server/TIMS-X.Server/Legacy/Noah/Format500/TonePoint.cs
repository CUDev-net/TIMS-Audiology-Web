using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TIMS_X.Server.Legacy.Noah.Enums;

namespace TIMS_X.Server.Legacy.Noah.Format500
{
	public class TonePoint
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public TonePoint()
		{
			MaskingFrequency = -1;
			StimulusFrequency = -1;
			MaskingLevel = -1m;
			StimulusLevel = -1m;
			TonePointStatus = PointStatusEnum.Normal;
		}

		#endregion Constructors

		#region TonePoint Members

		/// <summary>
		/// 
		/// </summary>
		public int MaskingFrequency
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal MaskingLevel
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public int StimulusFrequency
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public decimal StimulusLevel
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public PointStatusEnum TonePointStatus
		{
			get;
			set;
		}

		/// <summary>
		/// Serialize to XMl
		/// </summary>
		/// <returns></returns>
		public XElement Serialize()
		{
			var root = N500Audiogram.CreateElementWithNamespace( Constants.XML_TONEPOINTS );

			root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_STIMULUSFREQUENCY, StimulusFrequency.ToString() ) );
			root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_STIMULUSLEVEL, string.Format( "{0:0.0}", StimulusLevel ) ) );
			if( MaskingFrequency >= 0 )
			{
				root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_MASKINGFREQUENCY, MaskingFrequency.ToString() ) );
			}
			if( MaskingLevel >= 0m )
			{
				root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_MASKINGLEVEL, string.Format( "{0:0.0}", MaskingLevel ) ) );
			}
			root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_TONEPOINTSTATUS, TonePointStatus.ToString() ) );
			return root;
		}

		#endregion TonePoint Members

	}
}
