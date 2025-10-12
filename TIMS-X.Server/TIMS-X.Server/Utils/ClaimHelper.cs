using System;
using System.Security.Claims;

namespace TIMS_X.Server.Utils;

public static class ClaimHelper
{
	public static int GetUserIdFromClaim(ClaimsPrincipal user)
	{
		var userIdClaim = user.FindFirst("User");
		if (userIdClaim == null)
			throw new Exception("Current does not have an id claim");

		var userId = userIdClaim.Value;
		if (!int.TryParse(userId, out var id)) throw new Exception($"User Id {userId} is not valid");
		return id;
	}
}