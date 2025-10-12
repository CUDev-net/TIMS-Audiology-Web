using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIMS_X.Server.Legacy.Noah
{
	/// <summary>
	/// Support pure tone averages
	/// </summary>
	public interface IPureToneAudioSupport
	{
		#region IPureToneAudioSupport Members

		/// <summary>
		/// Gets the pure tone data
		/// </summary>
		PureToneData PureToneData
		{
			get;
		}

		#endregion IPureToneAudioSupport Members

	}
}
