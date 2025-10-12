using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TIMS_X.Core.Enums
{
	/// <summary>
	/// Enum for patient report category
	/// </summary>
	public enum PatientReportCategoryEnum
	{
		/// <summary>
		/// 
		/// </summary>
		[Description("Diagnostic Reports")]
		DiagnosticReport = 1,

		/// <summary>
		/// 
		/// </summary>
		[Description("Fax Cover Page")]
		FaxCoverPage = 2,

		/// <summary>
		/// 
		/// </summary>
		[Description("SLP Reports")]
		SLPReport = 3,

		/// <summary>
		/// Used to store digital intake sheet
		/// </summary>
		[Description("Digital Intake Sheet")]
		DigitalIntakeSheet = 4
	}
}
