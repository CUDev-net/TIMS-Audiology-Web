using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TIMS_X.Server.Legacy.Noah.Format500
{
	public class MeasurementNotes
	{
		#region MeasurementNotes Members

		/// <summary>
		/// Calibration date
		/// </summary>
		public DateTime? AudiometerLastCalibration
		{
			get;
			set;
		}

		/// <summary>
		/// Audiometer make and model
		/// </summary>
		public string AudiometerMakeModel
		{
			get;
			set;
		}

		/// <summary>
		/// SN
		/// </summary>
		public string AudiometerSerialNumber
		{
			get;
			set;
		}

		/// <summary>
		/// Test method
		/// </summary>
		public string TestMethod
		{
			get;
			set;
		}

		/// <summary>
		/// Test reliability
		/// </summary>
		public string TestReliability
		{
			get;
			set;
		}

		/// <summary>
		/// Serialize to XML
		/// </summary>
		/// <returns></returns>
		public XElement Serialize()
		{
			var root = N500Audiogram.CreateElementWithNamespace( Constants.XML_TONENOTES );
			if( !string.IsNullOrEmpty( TestMethod ) )
			{
				root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_TESTMETHOD, TestMethod ) );
			}
			if( !string.IsNullOrEmpty( TestReliability ) )
			{
				root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_TESTRELIABILITY, TestReliability ) );
			}
			if( !string.IsNullOrEmpty( AudiometerMakeModel ) )
			{
				root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_AUDIOMETERMAKEMODEL, AudiometerMakeModel ) );
			}
			if( !string.IsNullOrEmpty( AudiometerSerialNumber ) )
			{
				root.Add( N500Audiogram.CreateElementWithNamespace( Constants.XML_AUDIOMETERSERIALNUMBER, AudiometerSerialNumber ) );
			}
			return root;
		}

		#endregion MeasurementNotes Members

	}
}
