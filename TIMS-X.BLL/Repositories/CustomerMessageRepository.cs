using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface ICustomerMessageRepository
{
    Task<CustomerMessage> Add(CustomerMessage customerMessage);
    void Delete(int id);
    Task<CustomerMessage> Get(int id);
    Task<List<CustomerMessage>> GetAll(bool includeInactive);
    Task<CustomerMessage> Update(CustomerMessage customerMessage);
}

public class CustomerMessageRepository : ICustomerMessageRepository
{
    private readonly ICustomerMessageUnitOfWork _customerMessageUnitOfWork;

    public CustomerMessageRepository(ICustomerMessageUnitOfWork customerMessageUnitOfWork)
    {
        _customerMessageUnitOfWork = customerMessageUnitOfWork;
    }

    public async Task<CustomerMessage> Add(CustomerMessage customerMessage)
    {
        return await _customerMessageUnitOfWork.Add(customerMessage);
    }

    public void Delete(int id)
    {
        _customerMessageUnitOfWork.Delete(id);
    }

    public async Task<CustomerMessage> Get(int id)
    {
        return await _customerMessageUnitOfWork.GetCustomerMessage(id);
    }

    public async Task<List<CustomerMessage>> GetAll(bool includeInactive)
    {
        return await _customerMessageUnitOfWork.GetCustomerMessages(x => includeInactive || !x.Inactive);
    }

    public async Task<CustomerMessage> Update(CustomerMessage customerMessage)
    {
        return await _customerMessageUnitOfWork.Update(customerMessage);
    }
}