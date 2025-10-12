using System;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Domain.Noah;
using TIMS_X.Server.Legacy.Noah;

namespace TIMS_X.Server.Services;

public interface INoahDataMiningService
{
	Task<Tuple<bool, bool>> IsEarAidableAsync(Patient patient, TestSide side);
	Task<NdmAction> ProcessNoahActionAsync(N4Action action, byte[] decompressedPublicData, int patientId);
}