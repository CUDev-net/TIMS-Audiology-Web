using System;
using System.Runtime.InteropServices;

namespace TIMS_X.Server.Models.Noah;

/// <summary>
///     Struct used to export Noah data
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 8 + 8 + 8 + 8)]
[Serializable]
internal struct RootxSection
{
	[MarshalAs(UnmanagedType.I8)] internal long MagicNumber; // A magic number for better identification 
	[MarshalAs(UnmanagedType.I8)] internal long PatientDirectoryOffset; // File offset to the Patient directory
	[MarshalAs(UnmanagedType.I8)] internal long UserOffset; // File offset to the User section
	[MarshalAs(UnmanagedType.I8)] internal long MiscOffset; // File offset to the misc section
}