using System.ComponentModel;
using System.Xml.Linq;

namespace TIMS_X.Server.Legacy.Noah
{
	public abstract class SpeechResultBase
	{
		#region Constructors

		#endregion Constructors

		#region SpeechResult Members

		/// <summary>
		/// Gets or sets binural results
		/// </summary>
		public string Binural
		{
			get
			{
				return mBinural;
			}
			set
			{
				var temp = value;
				if( temp != mBinural )
				{
					mBinural = temp;
				}
			}
		}

		/// <summary>
		/// Gets or sets binural results
		/// </summary>
		public string BinuralAided
		{
			get
			{
				return mBinuralAided;
			}
			set
			{
				var temp = value;
				if( temp != mBinuralAided )
				{
					mBinuralAided = temp;
				}
			}
		}


		/// <summary>
		/// Gets or sets binural results
		/// </summary>
		public abstract string Description
		{
			get;
		}

		/// <summary>
		/// Gets or sets binural results
		/// </summary>
		public string Left
		{
			get
			{
				return mLeft;
			}
			set
			{
				var temp = value;
				if( temp != mLeft )
				{
					mLeft = temp;
				}
			}
		}

		/// <summary>
		/// Gets or sets binural results
		/// </summary>
		public string Right
		{
			get
			{
				return mRight;
			}
			set
			{
				var temp = value;
				if( temp != mRight )
				{
					mRight = temp;
				}
			}
		}

		/// <summary>
		/// Gets or sets binural results
		/// </summary>
		public string LeftAided
		{
			get
			{
				return mLeftAided;
			}
			set
			{
				var temp = value;
				if( temp != mLeftAided )
				{
					mLeftAided = temp;
				}
			}
		}

		/// <summary>
		/// Gets or sets binural results
		/// </summary>
		public string RightAided
		{
			get
			{
				return mRightAided;
			}
			set
			{
				var temp = value;
				if( temp != mRightAided )
				{
					mRightAided = temp;
				}
			}
		}


		/// <summary>
		/// Gets or sets binural results
		/// </summary>
		public string Soudfield
		{
			get
			{
				return mSoudfield;
			}
			set
			{
				var temp = value;
				if( temp != mSoudfield )
				{
					mSoudfield = temp;
				}
			}
		}

		/// <summary>
		/// Gets or sets binural results
		/// </summary>
		public string SoudfieldAided
		{
			get
			{
				return mSoudfieldAided;
			}
			set
			{
				var temp = value;
				if( temp != mSoudfieldAided )
				{
					mSoudfieldAided = temp;
				}
			}
		}

		/// <summary>
		/// Gets the XMLTag used in serialization
		/// </summary>
		public abstract string XMLTag
		{
			get;
		}

		/// <summary>
		/// Begins an edit on an object.
		/// </summary>
		public void BeginEdit()
		{
			mCacheBinural = Binural;
			mCacheRight = Right;
			mCacheLeft = Left;
			mCacheSoudfield = Soudfield;
			mCacheSoudfieldAided = SoudfieldAided;
		}

		/// <summary>
		/// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> call.
		/// </summary>
		public void CancelEdit()
		{
			Binural = mCacheBinural;
			Right = mCacheRight;
			Left = mCacheLeft;
			BinuralAided = mCacheBinural;
			RightAided = mCacheRight;
			LeftAided = mCacheLeft;
			Soudfield = mCacheSoudfield;
			SoudfieldAided = mCacheSoudfieldAided;
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
					case Constants.XML_SPEECH_RESULT_BINURAL:
						if( !string.IsNullOrEmpty( attribute.Value ) )
						{
							Binural = attribute.Value;
						}
						break;
					case Constants.XML_SPEECH_RESULT_RIGHT:
						if( !string.IsNullOrEmpty( attribute.Value ) )
						{
							Right = attribute.Value;
						}
						break;
					case Constants.XML_SPEECH_RESULT_LEFT:
						if( !string.IsNullOrEmpty( attribute.Value ) )
						{
							Left = attribute.Value;
						}
						break;
					case Constants.XML_SPEECH_RESULT_BINURAL_AIDED:
						if( !string.IsNullOrEmpty( attribute.Value ) )
						{
							BinuralAided = attribute.Value;
						}
						break;
					case Constants.XML_SPEECH_RESULT_RIGHT_AIDED:
						if( !string.IsNullOrEmpty( attribute.Value ) )
						{
							RightAided = attribute.Value;
						}
						break;
					case Constants.XML_SPEECH_RESULT_LEFT_AIDED:
						if( !string.IsNullOrEmpty( attribute.Value ) )
						{
							LeftAided = attribute.Value;
						}
						break;

					case Constants.XML_SPEECH_RESULT_SOUNDFIELD:
						if( !string.IsNullOrEmpty( attribute.Value ) )
						{
							Soudfield = attribute.Value;
						}
						break;
					case Constants.XML_SPEECH_RESULT_SOUNDFIELD_AIDED:
						if( !string.IsNullOrEmpty( attribute.Value ) )
						{
							SoudfieldAided = attribute.Value;
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
			XElement element = new XElement( XMLTag );

			element.Add( new XAttribute( Constants.XML_SPEECH_RESULT_BINURAL, string.IsNullOrEmpty( Binural ) ? string.Empty : Binural ));
			element.Add( new XAttribute( Constants.XML_SPEECH_RESULT_RIGHT, string.IsNullOrEmpty( Right ) ? string.Empty : Right ) );
			element.Add( new XAttribute( Constants.XML_SPEECH_RESULT_LEFT, string.IsNullOrEmpty( Left ) ? string.Empty : Left ) );
			element.Add( new XAttribute( Constants.XML_SPEECH_RESULT_BINURAL_AIDED, string.IsNullOrEmpty( BinuralAided ) ? string.Empty : BinuralAided ) );
			element.Add( new XAttribute( Constants.XML_SPEECH_RESULT_RIGHT_AIDED, string.IsNullOrEmpty( RightAided ) ? string.Empty : RightAided ) );
			element.Add( new XAttribute( Constants.XML_SPEECH_RESULT_LEFT_AIDED, string.IsNullOrEmpty( LeftAided ) ? string.Empty : LeftAided ) );
			element.Add( new XAttribute( Constants.XML_SPEECH_RESULT_SOUNDFIELD, string.IsNullOrEmpty( Soudfield ) ? string.Empty : Soudfield ) );
			element.Add( new XAttribute( Constants.XML_SPEECH_RESULT_SOUNDFIELD_AIDED, string.IsNullOrEmpty( SoudfieldAided ) ? string.Empty : SoudfieldAided ) );

			return element;
		}

		#endregion SpeechResult Members

		#region Private Members

		private string mBinural;
		private string mBinuralAided;
		private string mCacheBinural;
		private string mCacheLeft;
		private string mCacheRight;
		private string mCacheSoudfield;
		private string mCacheSoudfieldAided;
		private string mLeft;
		private string mRight;
		private string mLeftAided;
		private string mRightAided;
		private string mSoudfield;
		private string mSoudfieldAided;

		#endregion Private Members

	}
}
