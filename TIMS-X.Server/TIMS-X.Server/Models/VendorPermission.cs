using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace TIMS_X.Server.Models;

public class VendorPermission
{
	public VendorPermission()
	{
		AssociatedCustomers = new HashSet<CustomerVendorPermission>();
		AssociatedDefaultPermissions = new HashSet<DefaultVendorPermission>();
		ApiUrls = new HashSet<VendorPermissionApiUrl>();
	}

	public ICollection<VendorPermissionApiUrl> ApiUrls { get; set; }
	public ICollection<CustomerVendorPermission> AssociatedCustomers { get; set; }
	public ICollection<DefaultVendorPermission> AssociatedDefaultPermissions { get; set; }
	public DateTime DateCreated { get; set; }

	[StringLength(256)] public string Description { get; set; }

	public bool DisableDelete => (AssociatedCustomers != null && AssociatedCustomers.Any()) ||
	                             (AssociatedDefaultPermissions != null && AssociatedDefaultPermissions.Any());

	public int Id { get; set; }

	[Required] public bool Inactive { get; set; }

	[Required]
	[StringLength(64)]
	[Remote("ValidateVendorPermissionName", "Validation", AdditionalFields = nameof(Id))]
	public string Name { get; set; }

	public string UrlsJson { get; set; }
}