using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace TIMS_X.Server.Legacy.Noah
{
	public class TestData
	{
		#region TestData Members

		public string Audiometer
		{
			get
			{
				return mAudiometer;
			}
			set
			{
				var temp = value;
				if( temp != mAudiometer )
				{
					mAudiometer = temp;
				}
			}
		}

		public DateTime? Calibration
		{
			get
			{
				return mCalibration;
			}
			set
			{
				var temp = value;
				if( temp != mCalibration )
				{
					mCalibration = temp;
				}
			}
		}

		public string Method
		{
			get
			{
				return mMethod;
			}
			set
			{
				var temp = value;
				if( temp != mMethod )
				{
					mMethod = temp;
				}
			}
		}

		public string Notes
		{
			get
			{
				return mNotes;
			}
			set
			{
				var temp = value;
				if( temp != mNotes )
				{
					mNotes = temp;
				}
			}
		}

		public string Reliability
		{
			get
			{
				return mReliability;
			}
			set
			{
				var temp = value;
				if( temp != mReliability )
				{
					mReliability = temp;
				}
			}
		}

		public string SerialNumber
		{
			get
			{
				return mSerialNumber;
			}
			set
			{
				var temp = value;
				if( temp != mSerialNumber )
				{
					mSerialNumber = temp;
				}
			}
		}

		public string Transducer
		{
			get
			{
				return mTransducer;
			}
			set
			{
				var temp = value;
				if( temp != mTransducer )
				{
					mTransducer = temp;
				}
			}
		}

		/// <summary>
		/// Begins an edit on an object.
		/// </summary>
		public void BeginEdit()
		{
			mCacheAudiometer = Audiometer;
			mCacheCalibration = Calibration.HasValue ? Calibration : null;
			mCacheMethod = Method;
			mCacheReliability = Reliability;
			mCacheTransducer = Transducer;
			mCacheNotes = Notes;
			mCacheSerialNumber = SerialNumber;
		}

		/// <summary>
		/// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> call.
		/// </summary>
		public void CancelEdit()
		{
			Audiometer = mCacheAudiometer;
			Calibration = mCacheCalibration.HasValue ? mCacheCalibration : null;
			Method = mCacheMethod;
			Reliability = mCacheReliability;
			Transducer = mCacheTransducer;
			Notes = mCacheNotes;
			SerialNumber = mCacheSerialNumber;
		}

		/// <summary>
		/// Takes the data from the XML element and exports it to the task
		/// </summary>
		/// <param name="xElement"></param>
		public virtual void DeSerialize( XElement xElement )
		{
			foreach( var attribute in xElement.Attributes() )
			{
				switch( attribute.Name.ToString() )
				{
					case Constants.XML_TEST_AUDIOMETER:
						if( !string.IsNullOrEmpty( attribute.Value ) )
						{
							Audiometer = attribute.Value;
						}
						break;
					case Constants.XML_TEST_CALIBRATION:
						if( !string.IsNullOrEmpty( attribute.Value ) )
						{
							DateTime tempTime;
							if( DateTime.TryParse( attribute.Value, out tempTime ) )
							{

								Calibration = tempTime;
							}
						}
						break;
					case Constants.XML_TEST_METHOD:
						if( !string.IsNullOrEmpty( attribute.Value ) )
						{
							Method = attribute.Value;
						}
						break;
					case Constants.XML_TEST_RELIABILITY:
						if( !string.IsNullOrEmpty( attribute.Value ) )
						{
							Reliability = attribute.Value;
						}
						break;
					case Constants.XML_TEST_NOTES:
						if( !string.IsNullOrEmpty( attribute.Value ) )
						{
							Notes = attribute.Value;
						}
						break;
					case Constants.XML_TEST_TRANSDUCER:
						if( !string.IsNullOrEmpty( attribute.Value ) )
						{
							Transducer = attribute.Value;
						}
						break;
					case Constants.XML_TEST_SERIAL_NUMBER:
						if( !string.IsNullOrEmpty( attribute.Value ) )
						{
							SerialNumber = attribute.Value;
						}
						break;
				}
			}
		}

		/// <summary>
		/// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> or <see cref="M:System.ComponentModel.IBindingList.AddNew"/> call into the underlying object.
		/// </summary>
		public void EndEdit()
		{
			BeginEdit();
		}

		/// <summary>
		/// Serializes the XML element
		/// </summary>
		/// <returns></returns>
		public virtual XElement Serialize()
		{
			XElement element = new XElement( Constants.XML_TEST_SETUP );

			element.Add( new XAttribute( Constants.XML_TEST_AUDIOMETER,
									   string.IsNullOrEmpty( Audiometer ) ? string.Empty : Audiometer ) );
			element.Add( new XAttribute( Constants.XML_TEST_TRANSDUCER,
									   string.IsNullOrEmpty( Transducer ) ? string.Empty : Transducer ) );
			element.Add( new XAttribute( Constants.XML_TEST_METHOD,
									   string.IsNullOrEmpty( Method ) ? string.Empty : Method ) );
			element.Add( new XAttribute( Constants.XML_TEST_NOTES,
									   string.IsNullOrEmpty( Notes ) ? string.Empty : Notes ) );
			element.Add( new XAttribute( Constants.XML_TEST_RELIABILITY,
									   string.IsNullOrEmpty( Reliability ) ? string.Empty : Reliability ) );
			element.Add( new XAttribute( Constants.XML_TEST_SERIAL_NUMBER,
									   string.IsNullOrEmpty( SerialNumber ) ? string.Empty : SerialNumber ) );

			if( Calibration.HasValue )
			{
				element.Add( new XAttribute( Constants.XML_TEST_CALIBRATION, Calibration.Value.ToShortDateString() ) );
			}

			return element;

		}

		#endregion TestData Members

		#region Private Members

		private DateTime? mCacheCalibration;
		private DateTime? mCalibration;
		private string mAudiometer;
		private string mCacheAudiometer;
		private string mCacheMethod;
		private string mCacheNotes;
		private string mCacheReliability;
		private string mCacheSerialNumber;
		private string mCacheTransducer;
		private string mMethod;
		private string mNotes;
		private string mReliability;
		private string mSerialNumber;
		private string mTransducer;

		#endregion Private Members

	}
}
