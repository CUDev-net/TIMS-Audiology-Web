using System;

namespace TIMS_X.Server.Models;

public class TimsUpdate
{
	public string BlobName { get; set; }
	public DateTime ReleaseDate { get; set; }
	public string Version { get; set; }

	public override string ToString()
	{
		return $"{Version} - {ReleaseDate.ToShortDateString()}";
	}
}