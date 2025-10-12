using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TIMS_X.Core.Enums;

namespace TIMS_X.Server.Models;

public class Customer
{
	public Customer()
	{
		DateCreated = DateTime.Now;
		VendorPermissions = new HashSet<CustomerVendorPermission>();
		FormLinks = new HashSet<FormLink>();
	}

	public bool Active => !Inactive;

	[Required]
	[StringLength(128)]
	[Remote("ValidateCustomerDatabase", "Validation", AdditionalFields = nameof(ServerId) + "," + nameof(Id))]
	public string Database { get; set; }

	public DateTime DateCreated { get; set; }

	public DateTime DateUpdated { get; set; }
	public ICollection<FormLink> FormLinks { get; set; }
	public int Id { get; set; }

	[Required] public bool Inactive { get; set; }

	[Required] [StringLength(128)] public string Name { get; set; }

	[StringLength(256)] public string Notes { get; set; }

	[Required]
	[StringLength(16)]
	[Remote("ValidateCustomerOfficeCode", "Validation", AdditionalFields = nameof(Id))]
	public string OfficeCode { get; set; }

	public string PermissionList { get; set; }
	public TimsServer Server { get; set; }

	[Required(ErrorMessage = "Please choose an active server")]
	//[Remote(action: "ValidateCustomerDatabase", controller: "Validation", AdditionalFields = nameof(Database) + "," + nameof(Id))]
	public int ServerId { get; set; }

	public string SqlPassword { get; set; }

	public string SqlUser { get; set; }

	public TimsTimeZone? TimeZoneId { get; set; }
	public int UpdatedBy { get; set; }
	public SupportUser UpdatedByUser { get; set; }
	public ICollection<CustomerVendorPermission> VendorPermissions { get; set; }
}