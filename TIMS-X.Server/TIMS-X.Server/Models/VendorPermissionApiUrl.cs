namespace TIMS_X.Server.Models;

public class VendorPermissionApiUrl
{
	public ApiUrl ApiUrl { get; set; }
	public int ApiUrlId { get; set; }
	public VendorPermission Permission { get; set; }
	public int PermissionId { get; set; }
}