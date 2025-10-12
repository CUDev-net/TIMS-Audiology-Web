namespace TIMS_X.Server.Models;

public class CustomerVendorPermission
{
	public Customer Customer { get; set; }
	public int CustomerId { get; set; }
	public VendorPermission Permission { get; set; }
	public int PermissionId { get; set; }

	public Vendor Vendor { get; set; }

	public int VendorId { get; set; }
}