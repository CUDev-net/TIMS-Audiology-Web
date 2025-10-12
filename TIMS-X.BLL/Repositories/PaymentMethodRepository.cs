using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IPaymentMethodRepository
{
    Task<PaymentMethod> Add(PaymentMethod paymentMethod);
    void Delete(int id);
    Task<PaymentMethod> Get(int id);
    Task<List<PaymentMethod>> GetAll(bool includeInactive);
    Task<PaymentMethod> Update(PaymentMethod paymentMethod);
}

public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly IPaymentMethodUnitOfWork _paymentMethodUnitOfWork;

    public PaymentMethodRepository(IPaymentMethodUnitOfWork paymentMethodUnitOfWork)
    {
        _paymentMethodUnitOfWork = paymentMethodUnitOfWork;
    }

    public async Task<PaymentMethod> Add(PaymentMethod paymentMethod)
    {
        return await _paymentMethodUnitOfWork.Add(paymentMethod);
    }

    public void Delete(int id)
    {
        _paymentMethodUnitOfWork.Delete(id);
    }

    public async Task<PaymentMethod> Get(int id)
    {
        return await _paymentMethodUnitOfWork.GetPaymentMethod(id);
    }

    public async Task<List<PaymentMethod>> GetAll(bool includeInactive)
    {
        return await _paymentMethodUnitOfWork.GetPaymentMethods(x => includeInactive || !x.Inactive);
    }

    public async Task<PaymentMethod> Update(PaymentMethod paymentMethod)
    {
        return await _paymentMethodUnitOfWork.Update(paymentMethod);
    }
}