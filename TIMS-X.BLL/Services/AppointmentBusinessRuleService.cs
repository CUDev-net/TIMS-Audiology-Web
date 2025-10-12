using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Services;

public interface IAppointmentBusinessRuleService
{
    Task ApplyBillToProviderRule(Appointment appointment);
}

public class AppointmentBusinessRuleService : IAppointmentBusinessRuleService
{
    private readonly IProvidersUnitOfWork _providersUnitOfWork;

    public AppointmentBusinessRuleService(IProvidersUnitOfWork providersUnitOfWork)
    {
        _providersUnitOfWork = providersUnitOfWork;
    }

    public async Task ApplyBillToProviderRule(Appointment appointment)
    {
        // If BilToProvider set and billto is provider, do nothing
        if (appointment.BillToProviderId != 0 && appointment.BillToProviderId == appointment.ProviderId)
            return;

        if (appointment.Id == 0 && appointment.BillToProviderId == 0)
        {
            // Get the provider
            Provider provider;
            if (appointment.Provider != null && appointment.Provider.Id == appointment.ProviderId)
                provider = appointment.Provider;
            else
                provider = await _providersUnitOfWork.GetProvider(appointment.ProviderId);

            // Either bill the provider directly or delegate.
            appointment.BillToProviderId = provider.NotBillable ? provider.BillToId : provider.Id;
        }
    }
}