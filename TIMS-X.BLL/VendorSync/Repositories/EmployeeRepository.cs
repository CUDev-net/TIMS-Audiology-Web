using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.BLL.VendorSync.Audigy;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.VendorSync.Repositories;

public interface IEmployeeRepository
{
    Task<Employee> GetEmployee(int id);

    Task<List<Employee>> GetEmployees(bool includeInactive);
}

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IProvidersUnitOfWork _providersUnitOfWork;
    private readonly IUserUnitOfWork _userUnitOfWork;

    public EmployeeRepository(IUserUnitOfWork userUnitOfWork, IProvidersUnitOfWork providerUnitOfWork)
    {
        _userUnitOfWork = userUnitOfWork;
        _providersUnitOfWork = providerUnitOfWork;
    }

    public async Task<Employee> GetEmployee(int id)
    {
        var employee = await _userUnitOfWork.GetUser(id);

        if (employee == null) return null;
        return MapUserToEmployee(employee);
    }

    public async Task<List<Employee>> GetEmployees(bool includeInactive)
    {
        var employees = new List<Employee>();
        var rawItems = _userUnitOfWork.GetUsers(u => includeInactive || !u.Inactive);
        foreach (var user in await rawItems.ToListAsync())
        {
            var employee = MapUserToEmployee(user);
            employee.IsAudiologist = _providersUnitOfWork.GetProviderSummaries(p => p.UserId == employee.Id).Any();
            employees.Add(employee);
        }

        return employees;
    }

    public static Employee MapUserToEmployee(User user)
    {
        return new Employee
        {
            Id = user.Id,
            Name = user.Name,
            SiteId = user.SiteId,
            IsActive = !user.Inactive
        };
    }
}