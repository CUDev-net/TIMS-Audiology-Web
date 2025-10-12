using System;
using System.Windows.Forms;
using Himsa.Noah.BusinessAPI;

namespace TIMS_X.CloudAssistant.Utils
{
	public class NoahCallbackHandler : IBSCallBack
	{
		#region NoahCallbackHandler Members

		public bool GetPatientIDsByInfo(object PatientInfo, out int[] PatientID)
		{
			throw new NotImplementedException();
		}

		public bool Login(out int UserID)
		{
			MessageBox.Show("Noah 4: Login from Module not supported", "TIMS Noah", MessageBoxButtons.OK,
				MessageBoxIcon.Information);
			UserID = -1;
			return false;
		}

		public bool SetPatientIDByInfo(int[] PatientIDs, out int PatientID)
		{
			MessageBox.Show("Noah 4: Set Patient from Module not supported", "TIMS Noah", MessageBoxButtons.OK,
				MessageBoxIcon.Information);
			PatientID = -1;
			return false;
		}

		#endregion NoahCallbackHandler Members
	}
}