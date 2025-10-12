using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace TIMS_X.Server.Models;

public class TimsServer
{
	[Display(Name = "IP Address or Domain Name")]
	[Remote("ValidateServerAddress", "Validation", AdditionalFields = nameof(Id))]
	public string Address { get; set; }

	public bool CanDelete => Customers == null || !Customers.Any();


	public string CombinedName => $"{Name} ({Address})";

	public ICollection<Customer> Customers { get; set; }
	public DateTime DateCreated { get; set; }
	public bool DisableDelete => Customers != null && Customers.Any();
	public int Id { get; set; }
	public bool Inactive { get; set; }

	[Required]
	[StringLength(64)]
	[Remote("ValidateServerName", "Validation", AdditionalFields = nameof(Id))]
	public string Name { get; set; }
}