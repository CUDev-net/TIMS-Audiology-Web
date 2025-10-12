using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Legacy.Noah.Format500.Tympanometry.Enums;

namespace TIMS_X.Server.Legacy.Noah.Format500.Tympanometry
{
	/// <summary>
	/// 500 format tympanogram
	/// </summary>
	public class N500Tympanogram
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public N500Tympanogram()
		{
			TympanogramTests = new List<TympanogramTestType>();
			ReflexTests = new List<ReflexTestType>();
		}

		#endregion Constructors

		#region N500Tympanogram Members

		/// <summary>
		/// Ear drum intact test
		/// </summary>
		public EustachianTubeFunctionIntactEarDrumTest EustachianTubeFunctionIntactEarDrumTest
		{
			get;
			set;
		}

		/// <summary>
		/// EustachianTubeFunctionPerforatedEarDrumTest
		/// </summary>
		public EustachianTubeFunctionPerforatedEarDrumTest EustachianTubeFunctionPerforatedEarDrumTest
		{
			get;
			set;
		}

		/// <summary>
		/// Up to 16
		/// </summary>
		public List<ReflexTestType> ReflexTests
		{
			get;
			private set;
		}

		/// <summary>
		/// Up to 3
		/// </summary>
		public List<TympanogramTestType> TympanogramTests
		{
			get;
			private set;
		}

		/// <summary>
		/// Creates an element with the namespace
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static XElement CreateElementWithNamespace( string name, string value = null )
		{
			return value == null ? new XElement( NAMESPACE + name ) : new XElement( NAMESPACE + name, value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="n500Xml"></param>
		public void Load( string n500Xml )
		{
			var xDocument = XDocument.Parse( n500Xml );
			var xTympTestNodes = xDocument.Root.Elements().Where( x => x.Name.LocalName == Constants.XML_TYMPANOGRAMTEST );
			foreach( var node in xTympTestNodes )
			{
				TympanogramTestType tympTest = _ParseTympanogramTestType( node );
				TympanogramTests.Add( tympTest );
			}

			var xReflexNodes = xDocument.Root.Elements().Where( x => x.Name.LocalName == Constants.XML_REFLEXTEST );
			foreach( var node in xReflexNodes )
			{
				ReflexTestType reflexTest = _ParseReflexTest( node );
				ReflexTests.Add( reflexTest );
			}

			var xEustachianNode = xDocument.Root.Elements().FirstOrDefault( x => x.Name.LocalName == Constants.XML_EUSTACHIANTUBEFUNCTIONINTACTEARDRUMTEST );
			if( xEustachianNode != null )
			{
				EustachianTubeFunctionIntactEarDrumTest = _ParseEustachianTubeFunctionIntactEarDrumTest( xEustachianNode );
			}

			var xEustachianPerfNode = xDocument.Root.Elements().FirstOrDefault( x => x.Name.LocalName == Constants.XML_EUSTACHIANTUBEFUNCTIONPERFORATEDEARDRUMTEST );
			if( xEustachianPerfNode != null )
			{
				EustachianTubeFunctionPerforatedEarDrumTest = _ParseEustachianTubeFunctionPerforatedEarDrumTest( xEustachianPerfNode );
			}
		}

		public void Load100( short[] nPublicData )
		{
			short[] nPublicData0 = new short[769];

			for( int i = 0; i < 3; i++ )
			{
				Array.Copy( nPublicData, i * 769, nPublicData0, 0, 769 );

				if( nPublicData0 == null )	// || nPublicData0[0] == -32767 )
					continue;

				TympanogramTestType tympTest = new TympanogramTestType();
				tympTest.ComplianceCurve = new ComplianceCurveType();
				tympTest.ComplianceCurve.CompliancePoints = new List<CompliancePointType>();
				tympTest.ComplianceCurve.ComplianceUnit = new ComplianceUnitType();

				int knt = 1;
				int pressure = 0;
				int count1 = 0;
				int count2 = 0;
				int cnt = 1;
				bool breakIt = false;
				short sx;

				foreach( var s in nPublicData0 )
				{
					if (cnt == 1 && s == -32767 && i == 0)
						sx = 0;
					else
						sx = s;
					if( sx == -32767 )
					{
						break;
					}

					switch( knt )
					{
						case 1:
							pressure = sx;
							knt++;
							break;
						case 2:
							count1 = sx;
							knt++;
							break;
						case 3:
							count2 = sx;
							var comp = new CompliancePointType();
							comp.Pressure = pressure;
							comp.Compliance = new ComplianceValueType();
							comp.Compliance.ArgumentCompliance1 = count1;
							comp.Compliance.ArgumentCompliance2 = count2;
							tympTest.ComplianceCurve.CompliancePoints.Add( comp );
							knt = 1;
							if( cnt++ == 250 )
								breakIt = true;
							break;
					}
					if( breakIt )
						break;
				}

				// Header Data at end of the pressure - compliance data
				//
				// 00 - Point 1 Unit of Measure Curve
				// 01 - Point 2 Unit of Measure Curve
				// 02 - Point 1 of Max Compliance
				// 03 - Point 2 of Max Compliance
				// 04 - Point 1 Unit of Measure
				// 05 - Point 2 Unit of Measure
				// 06 - Point 1 of Canal Volume
				// 07 - Point 2 of Canal Volume
				// 08 - Point 1 Unit of Measure
				// 09 - Point 2 Unit of Measure
				// 10 - Point 1 of Gradient
				// 11 - Point 2 of Gradient
				// 12 - Gradient Unit of Measure
				// 13 - Pressure of Max Compliance
				// 14
				// 15 - Shape Code
				// 16 - Sweep Speed
				// 17 - Compensated Code
				// 18 - Probe Frequency

				cnt = 250 * 3;

				tympTest.ComplianceCurve.ComplianceUnit = new ComplianceUnitType();
				tympTest.ComplianceCurve.ComplianceUnit.ArgumentUnit1 = (UnitTypeEnum)(nPublicData0[cnt + 0] == -32767 ? 0 : nPublicData0[cnt + 0]);
				tympTest.ComplianceCurve.ComplianceUnit.ArgumentUnit2 = (UnitTypeEnum)(nPublicData0[cnt + 1] == -32767 ? 0 : nPublicData0[cnt + 1]);

				tympTest.MaximumCompliance = new ComplianceType();
				tympTest.MaximumCompliance.ComplianceValue = new ComplianceValueType();
				tympTest.MaximumCompliance.ComplianceValue.ArgumentCompliance1 = nPublicData0[cnt + 2] == -32767 ? 0 : nPublicData0[cnt + 2];
				tympTest.MaximumCompliance.ComplianceValue.ArgumentCompliance2 = nPublicData0[cnt + 3] == -32767 ? 0 : nPublicData0[cnt + 3];
				tympTest.MaximumCompliance.ComplianceUnit = new ComplianceUnitType();
				tympTest.MaximumCompliance.ComplianceUnit.ArgumentUnit1 = (UnitTypeEnum)(nPublicData0[cnt + 4] == -32767 ? 0 : nPublicData0[cnt + 4]);
				tympTest.MaximumCompliance.ComplianceUnit.ArgumentUnit2 = (UnitTypeEnum)(nPublicData0[cnt + 5] == -32767 ? 0 : nPublicData0[cnt + 5]);

				tympTest.CanalVolume = new ComplianceType();
				tympTest.CanalVolume.ComplianceValue = new ComplianceValueType();
				tympTest.CanalVolume.ComplianceValue.ArgumentCompliance1 = nPublicData0[cnt + 6] == -32767 ? 0 : nPublicData0[cnt + 6];
				tympTest.CanalVolume.ComplianceValue.ArgumentCompliance2 = nPublicData0[cnt + 7] == -32767 ? 0 : nPublicData0[cnt + 7];
				tympTest.CanalVolume.ComplianceUnit = new ComplianceUnitType();
				tympTest.CanalVolume.ComplianceUnit.ArgumentUnit1 = (UnitTypeEnum)(nPublicData0[cnt + 8] == -32767 ? 0 : nPublicData0[cnt + 8]);
				tympTest.CanalVolume.ComplianceUnit.ArgumentUnit2 = (UnitTypeEnum)(nPublicData0[cnt + 9] == -32767 ? 0 : nPublicData0[cnt + 9]);

				tympTest.Gradient = new GradientType();
				tympTest.Gradient.GradientValue = new ComplianceValueType();
				tympTest.Gradient.GradientValue.ArgumentCompliance1 = nPublicData0[cnt + 10] == -32767 ? 0 : nPublicData0[cnt + 10] == -32767 ? 0 : nPublicData0[cnt + 10];
				tympTest.Gradient.GradientValue.ArgumentCompliance2 = nPublicData0[cnt + 11] == -32767 ? 0 : nPublicData0[cnt + 11] == -32767 ? 0 : nPublicData0[cnt + 11];
				tympTest.Gradient.GradientUnit = (UnitTypeEnum)(nPublicData0[cnt + 12] == -32767 ? 0 : nPublicData0[cnt + 12]);

				tympTest.Pressure = nPublicData0[cnt + 13] == -32767 ? 0 : nPublicData0[cnt + 13];

				tympTest.Result = nPublicData0[cnt + 15] == -32767 ? 0 : (TympanogramResultTypeEnum)(nPublicData0[cnt + 15] == -32767 ? 0 : nPublicData0[cnt + 15]);

				tympTest.MeasurementCondition = new TympanogramMeasurementConditionsType();
				tympTest.MeasurementCondition.SweepSpeed = nPublicData0[cnt + 16] == -32767 ? 0 : nPublicData0[cnt + 16];
				tympTest.MeasurementCondition.RecordingMode = new RecordingModeTypeEnum();
				tympTest.MeasurementCondition.RecordingMode = (RecordingModeTypeEnum)(nPublicData0[cnt + 17] == -32767 ? 0 : nPublicData0[cnt + 17]);
				tympTest.MeasurementCondition.ProbeFrequency = nPublicData0[cnt + 18] == -32767 ? 0 : nPublicData0[cnt + 18];

				tympTest.MeasurementCondition.RecordingMode = (RecordingModeTypeEnum)(nPublicData0[cnt + 17] == -32767 ? 0 : nPublicData0[cnt + 17]);

				TympanogramTests.Add( tympTest );
			}


			// REFLEX 100 Load

			nPublicData0 = new short[398];

			for( int i = 0; i < 16; i++ )
			{
				Array.Copy( nPublicData, 3 * 769 + i * 398     , nPublicData0, 0,398 );

				if( nPublicData0 == null || nPublicData0[0] == -32767 )
					continue;

				ReflexTestType reflexTest = new ReflexTestType();
				reflexTest.ReflexCurve = new ReflexCurveType();
				reflexTest.ReflexCurve.ReflexPoints = new List<ReflexPointType>();  
				reflexTest.ReflexCurve.ComplianceUnit = new ComplianceUnitType();

				int kntr = 1;
				int time = 0;
				int count1r = 0;
				int count2r = 0;
				int cntr = 1;
				bool breakItr = false;

				foreach (var s in nPublicData0)
				{
					if (s == -32767)
					{
						break;
					}
					switch (kntr)
					{
						case 1:
							time = s;
							kntr++;
							break;
						case 2:
							count1r = s;
							kntr++;
							break;
						case 3:
							count2r = s;
							var comp = new ReflexPointType();
							comp.Time = Convert.ToDecimal(time) * Convert.ToDecimal( 0.001 );
							comp.Compliance = new ComplianceValueType();
							comp.Compliance.ArgumentCompliance1 = count1r;
							comp.Compliance.ArgumentCompliance2 = count2r;
							reflexTest.ReflexCurve.ReflexPoints.Add(comp);
							kntr = 1;
							if (cntr++ == 128)
								breakItr = true;
							break;
					}
					if (breakItr)
						break;
				}

				cntr = 128 * 3;

				reflexTest.ReflexCurve.ComplianceUnit.ArgumentUnit1 = EnumUtilities.Parse<UnitTypeEnum>( nPublicData0[cntr + 0] );
				reflexTest.ReflexCurve.ComplianceUnit.ArgumentUnit2 = EnumUtilities.Parse<UnitTypeEnum>( nPublicData0[cntr + 1] );

				reflexTest.ResultOfReflexTest = nPublicData0[cntr + 2];

				reflexTest.ImpedanceMeasurementCondition = new ImpedanceMeasurementConditionType();
				reflexTest.ImpedanceMeasurementCondition.SignalLevel = Convert.ToDecimal( nPublicData0[cntr + 3] ) * Convert.ToDecimal( 0.1 );
				reflexTest.ImpedanceMeasurementCondition.SignalType = EnumUtilities.Parse<SignalTypeEnum>( nPublicData0[cntr + 4] );
				reflexTest.ImpedanceMeasurementCondition.SignalOutput = EnumUtilities.Parse<SignalOutputTypeEnum>( nPublicData0[cntr + 5] );
				reflexTest.ImpedanceMeasurementCondition.Frequency = nPublicData0[cntr + 6];
				reflexTest.ImpedanceMeasurementCondition.Pressure = nPublicData0[cntr + 7];
				reflexTest.ImpedanceMeasurementCondition.ProbeFrequency = nPublicData0[cntr + 8];

				reflexTest.ImpedanceMeasurementCondition.TestType = EnumUtilities.Parse<ReflexTestTypeTypeEnum>( nPublicData0[cntr + 9] );


				ReflexTests.Add(reflexTest);

			}

			//var xDocument = XDocument.Parse( n500Xml );
			//var xTympTestNodes = xDocument.Root.Elements().Where( x => x.Name.LocalName == Constants.XML_TYMPANOGRAMTEST );
			//int iTemp;
			//foreach( var node in xTympTestNodes )
			//{
			//TympanogramTestType tympTest = _ParseTympanogramTestType( node );
			//	TympanogramTests.Add( tympTest );
			//}

			//var xReflexNodes = xDocument.Root.Elements().Where( x => x.Name.LocalName == Constants.XML_REFLEXTEST );
			//foreach( var node in xReflexNodes )
			//{
			//	ReflexTestType reflexTest = _ParseReflexTest( node );
			//	ReflexTests.Add( reflexTest );
			//}

			//var xEustachianNode = xDocument.Root.Elements().FirstOrDefault( x => x.Name.LocalName == Constants.XML_EUSTACHIANTUBEFUNCTIONINTACTEARDRUMTEST );
			//if( xEustachianNode != null )
			//{
			//	EustachianTubeFunctionIntactEarDrumTest = _ParseEustachianTubeFunctionIntactEarDrumTest( xEustachianNode );
			//}

			//var xEustachianPerfNode = xDocument.Root.Elements().FirstOrDefault( x => x.Name.LocalName == Constants.XML_EUSTACHIANTUBEFUNCTIONPERFORATEDEARDRUMTEST );
			//if( xEustachianPerfNode != null )
			//{
			//	EustachianTubeFunctionPerforatedEarDrumTest = _ParseEustachianTubeFunctionPerforatedEarDrumTest( xEustachianPerfNode );
			//}
		}

		/// <summary>
		/// Serialize to XML string
		/// </summary>
		/// <returns></returns>
		public XElement Serialize()
		{
			var root = CreateElementWithNamespace( Constants.XML_ACOUSTICIMPEDANCECOMPLETEMEASUREMENT );
			root.Add( new XAttribute( "ValidatedByNOAH", "false" ) );
			root.Add( new XAttribute( "Version", "500" ) );

			if( TympanogramTests.Count > 0 )
			{
				foreach( var tympanogramTestType in TympanogramTests )
				{
					root.Add( tympanogramTestType.Serialize( Constants.XML_TYMPANOGRAMTEST ) );
				}
			}

			return root;
		}

		#endregion N500Tympanogram Members

		#region Private Members

		private static XNamespace NAMESPACE = "http://www.himsa.com/Measurement/Impedance";

		/// <summary>
		/// Parses compliance 
		/// </summary>
		/// <param name="xElement"></param>
		/// <returns></returns>
		private ComplianceType _ParseCompliance( XElement xElement )
		{
			var compliance = new ComplianceType();

			foreach( var node in xElement.Elements() )
			{
				switch( node.Name.LocalName )
				{
					case Constants.XML_COMPLIANCEVALUE:
						compliance.ComplianceValue = _ParseComplianceValue( node );
						break;
					case Constants.XML_COMPLIANCEUNIT:
						compliance.ComplianceUnit = _ParseComplianceUnit( node );
						break;
				}
			}
			return compliance;
		}

		/// <summary>
		/// Parses compliance node
		/// </summary>
		/// <param name="complianceNode"></param>
		/// <returns></returns>
		private ComplianceCurveType _ParseComplianceCurve( XElement complianceNode )
		{
			var complianceCurve = new ComplianceCurveType();
			CompliancePointType point;
			foreach( var xElement in complianceNode.Elements() )
			{
				switch( xElement.Name.LocalName )
				{
					case Constants.XML_COMPLIANCEPOINT:
						point = _ParseCompliancePoint( xElement );
						complianceCurve.CompliancePoints.Add( point );
						break;
					case Constants.XML_COMPLIANCEUNIT:
						complianceCurve.ComplianceUnit = _ParseComplianceUnit( xElement );
						break;
				}
			}
			return complianceCurve;
		}

		/// <summary>
		/// Parse compliance point
		/// </summary>
		/// <param name="pointNode"></param>
		/// <returns></returns>
		private CompliancePointType _ParseCompliancePoint( XElement pointNode )
		{
			var point = new CompliancePointType();
			int iTemp;
			foreach( var xElement in pointNode.Elements() )
			{
				switch( xElement.Name.LocalName )
				{
					case Constants.XML_PRESSURE:
						if( int.TryParse( xElement.Value, out iTemp ) )
						{
							point.Pressure = iTemp;
						}
						break;
					case Constants.XML_COMPLIANCE:
						point.Compliance = _ParseComplianceValue( xElement );
						break;
				}
			}

			return point;
		}

		/// <summary>
		/// Parses complicance unit
		/// </summary>
		/// <param name="complianceUnitNode"></param>
		/// <returns></returns>
		private ComplianceUnitType _ParseComplianceUnit( XElement complianceUnitNode )
		{
			var unit = new ComplianceUnitType();
			foreach( var xElement in complianceUnitNode.Elements() )
			{
				switch( xElement.Name.LocalName )
				{
					case Constants.XML_ARGUMENTUNIT1:
						unit.ArgumentUnit1 = EnumUtilities.Parse<UnitTypeEnum>( xElement.Value );
						break;
					case Constants.XML_ARGUMENTUNIT2:
						unit.ArgumentUnit2 = EnumUtilities.Parse<UnitTypeEnum>( xElement.Value );
						break;
				}
			}

			return unit;
		}

		/// <summary>
		/// Parses complicance unit
		/// </summary>
		/// <param name="complianceUnitNode"></param>
		/// <returns></returns>
		private UnitTypeEnum _ParseGradientUnit( XElement complianceUnitNode )
		{
			var unit = new UnitTypeEnum();
			foreach( var xElement in complianceUnitNode.Elements() )
			{
				switch( xElement.Name.LocalName )
				{
					case Constants.XML_ARGUMENTUNIT1:
						unit = EnumUtilities.Parse<UnitTypeEnum>( xElement.Value );
						break;
					//case Constants.XML_ARGUMENTUNIT2:
					//	unit.ArgumentUnit2 = EnumUtilities.Parse<UnitTypeEnum>( xElement.Value );
					//	break;
				}
			}

			return unit;
		}


		/// <summary>
		/// Parse compliance value node
		/// </summary>
		/// <param name="complianceValueNode"></param>
		/// <returns></returns>
		private ComplianceValueType _ParseComplianceValue( XElement complianceValueNode )
		{
			var complianceValue = new ComplianceValueType();
			int iTemp;
			foreach( var xElement in complianceValueNode.Elements() )
			{
				switch( xElement.Name.LocalName )
				{
					case Constants.XML_ARGUMENTCOMPLIANCE1:
						if( int.TryParse( xElement.Value, out iTemp ) )
						{
							complianceValue.ArgumentCompliance1 = iTemp;
						}
						break;
					case Constants.XML_ARGUMENTCOMPLIANCE2:
						if( int.TryParse( xElement.Value, out iTemp ) )
						{
							complianceValue.ArgumentCompliance2 = iTemp;
						}
						break;
				}
			}
			return complianceValue;
		}

		/// <summary>
		/// Parse EustachianTubeFunctionIntactEarDrumTest
		/// </summary>
		/// <param name="xElement"></param>
		/// <returns></returns>
		private EustachianTubeFunctionIntactEarDrumTest _ParseEustachianTubeFunctionIntactEarDrumTest( XElement xElement )
		{
			var eustachianTest = new EustachianTubeFunctionIntactEarDrumTest();

			int iTemp;
			ComplianceCurveType curve;
			foreach( var node in xElement.Elements() )
			{
				switch( node.Name.LocalName )
				{
					case Constants.XML_COMPLIANCECURVE:
						curve = _ParseComplianceCurve( node );
						eustachianTest.ComplianceCurve.Add( curve );
						break;
					case Constants.XML_PRESSURE:
						if( int.TryParse( node.Value, out iTemp ) )
						{
							eustachianTest.Pressure.Add( iTemp );
						}
						break;
					case Constants.XML_CANALVOLUME:
						eustachianTest.CanalVolume = _ParseCompliance( node );
						break;
					case Constants.XML_MEASUREMENTCONDITION:
						eustachianTest.MeasurementCondition = _ParseMeasurementCondition( node );
						break;
				}
			}
			return eustachianTest;
		}

		/// <summary>
		/// Parses EustachianTubeFunctionPerforatedEarDrumTest
		/// </summary>
		/// <param name="xElement"></param>
		/// <returns></returns>
		private EustachianTubeFunctionPerforatedEarDrumTest _ParseEustachianTubeFunctionPerforatedEarDrumTest( XElement xElement )
		{
			var eustachianPerfTest = new EustachianTubeFunctionPerforatedEarDrumTest();
			int iTemp;
			decimal dTemp;
			PressurePointType pressurePoint;
			foreach( var node in xElement.Elements() )
			{
				switch( node.Name.LocalName )
				{
					case Constants.XML_TIMEBASE:
						if( decimal.TryParse( node.Value, out dTemp ) )
						{
							eustachianPerfTest.TimeBase = dTemp;
						}
						break;
					case Constants.XML_CLOSEPOINT:
						if( int.TryParse( node.Value, out iTemp ) )
						{
							eustachianPerfTest.ClosePoints.Add( iTemp );
						}
						break;
					case Constants.XML_OPENPOINT:
						if( int.TryParse( node.Value, out iTemp ) )
						{
							eustachianPerfTest.OpenPoints.Add( iTemp );
						}
						break;
					case Constants.XML_PRESSUREPOINT:
						pressurePoint = _ParsePressurePoint( node );
						eustachianPerfTest.PressurePoints.Add( pressurePoint );
						break;
				}
			}
			return eustachianPerfTest;
		}

		private GradientType _ParseGradient( XElement xElement )
		{
			var gradient = new GradientType();

			foreach( var node in xElement.Elements() )
			{
				switch( node.Name.LocalName )
				{
					case Constants.XML_GRADIENTVALUE:
						gradient.GradientValue = _ParseComplianceValue( node );
						break;
					case Constants.XML_GRADIENTUNIT:
						gradient.GradientUnit = _ParseGradientUnit( node );
						break;
				}
			}
			return gradient;
		}

		/// <summary>
		/// Parses impedance m condition
		/// </summary>
		/// <param name="xElement"></param>
		/// <returns></returns>
		private ImpedanceMeasurementConditionType _ParseImpedanceMeasurementCondition( XElement xElement )
		{
			var measurementCondition = new ImpedanceMeasurementConditionType();
			int iTemp;
			decimal dTemp;
			foreach( var node in xElement.Elements() )
			{
				switch( node.Name.LocalName )
				{
					case Constants.XML_SIGNALLEVEL:
						if( decimal.TryParse( node.Value, out dTemp ) )
						{
							measurementCondition.SignalLevel = dTemp;
						}
						break;
					case Constants.XML_SIGNALTYPE:
						measurementCondition.SignalType = EnumUtilities.Parse<SignalTypeEnum>( node.Value );
						break;
					case Constants.XML_SIGNALOUTPUT:
						measurementCondition.SignalOutput = EnumUtilities.Parse<SignalOutputTypeEnum>( node.Value );
						break;
					case Constants.XML_FREQUENCY:
						if( int.TryParse( node.Value, out iTemp ) )
						{
							measurementCondition.Frequency = iTemp;
						}
						break;
					case Constants.XML_PRESSURE:
						if( int.TryParse( node.Value, out iTemp ) )
						{
							measurementCondition.Pressure = iTemp;
						}
						break;
					case Constants.XML_PROBEFREQUENCY:
						if( int.TryParse( node.Value, out iTemp ) )
						{
							measurementCondition.ProbeFrequency = iTemp;
						}
						break;
					case Constants.XML_TESTTYPE:
						measurementCondition.TestType = EnumUtilities.Parse<ReflexTestTypeTypeEnum>( node.Value );
						break;
					case Constants.XML_CANALVOLUME:
						measurementCondition.CanalVolume = _ParseCompliance( node );
						break;
				}
			}
			return measurementCondition;
		}

		/// <summary>
		/// Parse measurement condition
		/// </summary>
		/// <param name="xElement"></param>
		/// <returns></returns>
		private TympanogramMeasurementConditionsType _ParseMeasurementCondition( XElement xElement )
		{
			var measurementCondition = new TympanogramMeasurementConditionsType();

			int iTemp;
			foreach( var node in xElement.Elements() )
			{
				switch( node.Name.LocalName )
				{
					case Constants.XML_SWEEPSPEED:
						if( int.TryParse( node.Value, out iTemp ) )
						{
							measurementCondition.SweepSpeed = iTemp;
						}
						break;
					case Constants.XML_PROBEFREQUENCY:
						if( int.TryParse( node.Value, out iTemp ) )
						{
							measurementCondition.ProbeFrequency = iTemp;
						}
						break;
					case Constants.XML_RECORDMODE:
						measurementCondition.RecordingMode = EnumUtilities.Parse<RecordingModeTypeEnum>( node.Value );
						break;
				}
			}

			return measurementCondition;
		}

		/// <summary>
		/// Parse pressurepoint
		/// </summary>
		/// <param name="xElement"></param>
		/// <returns></returns>
		private PressurePointType _ParsePressurePoint( XElement xElement )
		{
			var point = new PressurePointType();
			int iTemp;
			decimal dTemp;
			foreach( var node in xElement.Elements() )
			{
				switch( node.Name.LocalName )
				{
					case Constants.XML_TIME:
						if( decimal.TryParse( node.Value, out dTemp ) )
						{
							point.Time = dTemp;
						}
						break;
					case Constants.XML_PRESSURE:
						if( int.TryParse( node.Value, out iTemp ) )
						{
							point.Pressure = iTemp;
						}
						break;
				}
			}
			return point;
		}

		/// <summary>
		/// Parse the reflex curve
		/// </summary>
		/// <param name="xElement"></param>
		/// <returns></returns>
		private ReflexCurveType _ParseReflexCurve( XElement xElement )
		{
			var reflexCurve = new ReflexCurveType();
			ReflexPointType point;
			foreach( var node in xElement.Elements() )
			{
				switch( node.Name.LocalName )
				{
					case Constants.XML_REFLEXPOINT:
						point = _ParseReflexPoint( node );
						reflexCurve.ReflexPoints.Add( point );
						break;
					case Constants.XML_COMPLIANCEUNIT:
						reflexCurve.ComplianceUnit = _ParseComplianceUnit( node );
						break;
				}
			}
			return reflexCurve;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xElement"></param>
		/// <returns></returns>
		private ReflexPointType _ParseReflexPoint( XElement xElement )
		{
			var point = new ReflexPointType();
			int iTemp;
			decimal dTemp;
			foreach( var node in xElement.Elements() )
			{
				switch( node.Name.LocalName )
				{
					case Constants.XML_TIME:
						if( decimal.TryParse( node.Value, out dTemp ) )
						{
							point.Time = dTemp;
						}
						break;
					case Constants.XML_PRESSURE:
						if( int.TryParse( node.Value, out iTemp ) )
						{
							point.Time = iTemp;
						}
						break;
					case Constants.XML_COMPLIANCE:
						point.Compliance = _ParseComplianceValue( node );
						break;
				}
			}
			return point;
		}

		/// <summary>
		/// Parse reflex test
		/// </summary>
		/// <param name="xElement"></param>
		/// <returns></returns>
		private ReflexTestType _ParseReflexTest( XElement xElement )
		{
			int iTemp;
			var relflexTest = new ReflexTestType();
			foreach( var node in xElement.Elements() )
			{
				switch( node.Name.LocalName )
				{
					case Constants.XML_REFLEXCURVE:
						relflexTest.ReflexCurve = _ParseReflexCurve( node );
						break;
					case Constants.XML_RESULTOFREFLEXTEST:
						if( int.TryParse( node.Value, out iTemp ) )
						{
							relflexTest.ResultOfReflexTest = iTemp;
						}
						break;
					case Constants.XML_IMPEDANCEMEASUREMENTCONDITION:
						relflexTest.ImpedanceMeasurementCondition = _ParseImpedanceMeasurementCondition( node );
						break;
				}
			}
			return relflexTest;
		}

		/// <summary>
		/// Parses Tympanogram test
		/// </summary>
		/// <param name="xElement"></param>
		/// <returns></returns>
		private TympanogramTestType _ParseTympanogramTestType( XElement xElement )
		{
			var tympTest = new TympanogramTestType();
			int iTemp;
			foreach( var tympNode in xElement.Elements() )
			{
				switch( tympNode.Name.LocalName )
				{
					case Constants.XML_COMPLIANCECURVE:
						tympTest.ComplianceCurve = _ParseComplianceCurve( tympNode );
						break;
					case Constants.XML_MAXIMUMCOMPLIANCE:
						tympTest.MaximumCompliance = _ParseCompliance( tympNode );
						break;
					case Constants.XML_CANALVOLUME:
						tympTest.CanalVolume = _ParseCompliance( tympNode );
						break;
					case Constants.XML_GRADIENT:
						tympTest.Gradient = _ParseGradient( tympNode );
						break;
					case Constants.XML_PRESSURE:
						if( int.TryParse( tympNode.Value, out iTemp ) )
						{
							tympTest.Pressure = iTemp;
						}
						break;
					case Constants.XML_RESONANCEFREQUENCY:
						if( int.TryParse( tympNode.Value, out iTemp ) )
						{
							tympTest.ResonanceFrequency = iTemp;
						}
						break;
					case Constants.XML_RESULT:
						tympTest.Result = EnumUtilities.Parse<TympanogramResultTypeEnum>( tympNode.Value );
						break;
					case Constants.XML_MEASUREMENTCONDITION:
						tympTest.MeasurementCondition = _ParseMeasurementCondition( tympNode );
						break;
					//case Constants.XML_RECORDMODE:
					//	tympTest.RecordingMode = EnumUtilities.Parse<RecordingModeTypeEnum>( tympNode.Value );
					//	break;

				}
			}
			return tympTest;
		}

		#endregion Private Members

		#region Other

		/// <summary>
		/// The class for the set of points.
		/// </summary>
		public class TympData
		{
			#region TympData Members

			public int tympX;
			public int tympY;

			#endregion TympData Members

		}

		#endregion Other

	}
}
