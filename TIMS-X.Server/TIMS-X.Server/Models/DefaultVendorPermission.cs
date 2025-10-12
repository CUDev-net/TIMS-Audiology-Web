namespace TIMS_X.Server.Models;

public class DefaultVendorPermission
{
	public VendorPermission Permission { get; set; }
	public int PermissionId { get; set; }

	public Vendor Vendor { get; set; }
	public int VendorId { get; set; }
}