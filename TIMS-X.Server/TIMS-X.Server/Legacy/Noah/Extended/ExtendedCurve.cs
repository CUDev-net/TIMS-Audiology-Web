using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TIMS_X.Server.Legacy.Noah.Enums;

namespace TIMS_X.Server.Legacy.Noah.Extended
{
	/// <summary>
	/// Point on the curve
	/// </summary>
	public class Point
	{
		#region Point Members

		public const string XML_FREQUENCY = "Frequency";
		public const string XML_LEVEL = "Level";
		public const string XML_MASKED = "Masked";
		public const string XML_POINT = "Point";
		public const string XML_STATUS = "Status";

		/// <summary>
		/// Frequency
		/// </summary>
		public int Frequency
		{
			get;
			set;
		}

		/// <summary>
		/// Level
		/// </summary>
		public decimal Level
		{
			get;
			set;
		}

		/// <summary>
		/// Is point masked
		/// </summary>
		public bool Masked
		{
			get;
			set;
		}

		/// <summary>
		/// Status
		/// </summary>
		public PointStatusEnum Status
		{
			get;
			set;
		}

		/// <summary>
		/// Deserialize object
		/// </summary>
		/// <param name="xPoint"></param>
		public void DeSerialize( XElement xPoint )
		{
			foreach( var xAttribute in xPoint.Attributes() )
			{
				switch( xAttribute.Name.ToString() )
				{
					case XML_LEVEL:
						Level = decimal.Parse( xAttribute.Value );
						break;
					case XML_FREQUENCY:
						Frequency = int.Parse( xAttribute.Value );
						break;
					case XML_STATUS:
						Status = (PointStatusEnum)Enum.Parse( typeof( PointStatusEnum ), xAttribute.Value );
						break;
					case XML_MASKED:
						Masked = xAttribute.Value == "True";
						break;
				}
			}
		}

		/// <summary>
		/// Serialize to XML
		/// </summary>
		/// <returns></returns>
		public XElement Serialize()
		{
			var root = new XElement( XML_POINT );
			root.Add( new XAttribute( XML_LEVEL, string.Format( "{0:0.0}", Level ) ) );
			root.Add( new XAttribute( XML_FREQUENCY, Frequency ) );
			root.Add( new XAttribute( XML_STATUS, Status.ToString() ) );
			root.Add( new XAttribute( XML_MASKED, Masked ? "True" : "False" ) );

			return root;
		}

		#endregion Point Members

	}

	/// <summary>
	/// Extended curve
	/// </summary>
	public class ExtendedCurve
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public ExtendedCurve()
		{
			Points = new List<Point>();
		}

		#endregion Constructors

		#region ExtendedCurve Members

		public const string XML_CONDITION1 = "Condition1";
		public const string XML_CONDITION2 = "Condition2";
		public const string XML_EXTENDED_CURVE = "ExtendedCurve";
		public const string XML_SIDE1 = "Side1";
		public const string XML_SIDE2 = "Side2";

		/// <summary>
		/// 
		/// </summary>
		public string Condition1
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Condition2
		{
			get;
			set;
		}

		public List<Point> Points
		{
			get;
			private set;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Side1
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Side2
		{
			get;
			set;
		}

		/// <summary>
		/// DeSerialize object
		/// </summary>
		/// <param name="xCurve"></param>
		public void DeSerialize( XElement xCurve )
		{
			foreach( var xAttribute in xCurve.Attributes() )
			{
				switch( xAttribute.Name.ToString() )
				{
					case XML_CONDITION1:
						Condition1 = xAttribute.Value;
						break;
					case XML_CONDITION2:
						Condition2 = xAttribute.Value;
						break;
					case XML_SIDE1:
						Side1 = xAttribute.Value;
						break;
					case XML_SIDE2:
						Side2 = xAttribute.Value;
						break;
				}
			}

			var xPoints = (from p in xCurve.Elements()
						   where p.Name == Point.XML_POINT
						   select p);
			foreach( var xPoint in xPoints )
			{
				var point = new Point();
				point.DeSerialize( xPoint );
				Points.Add( point );
			}
		}

		/// <summary>
		/// Serialize to XML
		/// </summary>
		/// <returns></returns>
		public XElement Serialize()
		{
			var root = new XElement( XML_EXTENDED_CURVE );
			root.Add( new XAttribute( XML_CONDITION1, Condition1 ) );
			root.Add( new XAttribute( XML_SIDE1, Side1 ) );
			if( !string.IsNullOrEmpty( Side2 ) && !string.IsNullOrEmpty( Condition2 ) )
			{
				root.Add( new XAttribute( XML_CONDITION2, Condition2 ) );
				root.Add( new XAttribute( XML_SIDE2, Side2 ) );
			}

			foreach( var point in Points )
			{
				root.Add( point.Serialize() );
			}
			return root;
		}

		#endregion ExtendedCurve Members

	}
}
