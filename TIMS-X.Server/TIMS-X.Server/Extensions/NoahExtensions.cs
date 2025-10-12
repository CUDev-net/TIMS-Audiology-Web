using System;
using TIMS_X.Core.Domain.Noah;

namespace TIMS_X.Server.Extensions;

public static class NoahExtensions
{
	public static N4Action Clone(this N4Action action)
	{
		return new N4Action
		{
			IsArchived = action.IsArchived,
			SessionId = action.SessionId,
			CreatedDate = action.CreatedDate,
			UpdatedDate = action.UpdatedDate,
			UpdatedUserId = action.UpdatedUserId,
			ModuleId = action.ModuleId,
			DevTypeCode = action.DevTypeCode,
			DataTypeCode = action.DataTypeCode,
			DataFmtCodeStd = action.DataFmtCodeStd,
			DataFmtCodePriv = action.DataFmtCodePriv,
			Description = action.Description ?? string.Empty,
			Removed = action.Removed,
			Hidden = action.Hidden,
			ActionGroup = action.ActionGroup,
			ActionGuid = Guid.Empty,
			PublicData = action.PublicData,
			CompressedPublicBlob = action.CompressedPublicBlob,
			CompressedPrivateBlob = action.CompressedPrivateBlob,
			PrivateData = action.PrivateData
		};
	}

	public static N4UnboundAction Clone(this N4UnboundAction action)
	{
		return new N4UnboundAction
		{
			IsArchived = action.IsArchived,
			VersionNo = action.VersionNo,
			ModuleId = action.ModuleId,
			DevTypeCode = action.DevTypeCode,
			DataTypeCode = action.DataTypeCode,
			DataFmtCodeStd = action.DataFmtCodeStd,
			DataFmtCodePriv = action.DataFmtCodePriv,
			Description = action.Description ?? string.Empty,
			Removed = action.Removed,
			Hidden = action.Hidden,
			ActionGroup = action.ActionGroup,
			ActionGuid = action.ActionGuid == Guid.Empty ? Guid.NewGuid() : action.ActionGuid,
			PublicData = action.PublicData,
			CompressedPublicBlob = action.CompressedPublicBlob,
			CompressedPrivateBlob = action.CompressedPrivateBlob,
			PrivateData = action.PrivateData
		};
	}
}