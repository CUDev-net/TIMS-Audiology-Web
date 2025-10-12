using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.DAL.DAL.UoWs;
using TIMS_X.DAL.Dtos;

namespace TIMS_X.BLL.Repositories;

public interface IUserRepository
{
    Task<User> GetUser(int id);
    Task<UserDto> GetUserAndPatients(int id);
    Task<List<PatientSummary>> GetUserPatients(int userId);
    Task<List<SettingEnum>> GetUserPermissions(int userId);
    Task<Site> GetUserSite(int userId);
    Task<User> Update(User user);
    Task<List<PatientSummary>> UpdateRecentPatientList(int userId, int patientId);
}

public class UserRepository : IUserRepository
{
    private const string XML_SETTING = "Setting";
    private const string XML_LAST_PATIENT_LIST = "LstPtLst";
    private const string XML_NAME = "Name";
    private const string XML_VALUE = "Value";
    private readonly ILastPatientUnitOfWork _lastPatientUnitOfWork;
    private readonly IPatientsUnitOfWork _patientsUnitOfWork;
    private readonly ISiteUnitOfWork _siteUnitOfWork;
    private readonly IUserUnitOfWork _userUnitOfWork;

    public UserRepository(IUserUnitOfWork userUnitOfWork, IPatientsUnitOfWork patientsUnitOfWork,
        ISiteUnitOfWork siteUnitOfWork,
        ILastPatientUnitOfWork lastPatientUnitOfWork)
    {
        _userUnitOfWork = userUnitOfWork;
        _patientsUnitOfWork = patientsUnitOfWork;
        _siteUnitOfWork = siteUnitOfWork;
        _lastPatientUnitOfWork = lastPatientUnitOfWork;
    }

    public async Task<List<SettingEnum>> GetUserPermissions(int userId)
    {
        var userReferences = await _userUnitOfWork
            .GetUsers(u => u.Id == userId, null,
                x => x.Include(u => u.UserReferences)
                    .ThenInclude(g => g.UserGroup)
                    .ThenInclude(g => g.Settings)
            ).Select( u => u.UserReferences).ToListAsync();

        var settings = new List<SettingEnum>();
        foreach (var userReference in userReferences)
        {
            foreach (var useGroupReference in userReference)
            {
                var userGroup = useGroupReference.UserGroup;
                if (userGroup != null && !userGroup.Inactive)
                {
                    settings.AddRange(userGroup.Settings.Select(setting => setting.PermissionType));
                }
            }
        }
        return settings.Distinct().ToList();
    }

    public async Task<User> Update(User user)
    {
        return await _userUnitOfWork.Update(user);
    }

    public async Task<User> GetUser(int id)
    {
        var user = await _userUnitOfWork.GetUser(id);

        return user;
    }

    public async Task<Site> GetUserSite(int userId)
    {
        Site site = null;
        var user = await _userUnitOfWork.GetUser(userId);
        var sites = await _siteUnitOfWork.GetSiteSummaries(s => s.Id == user.Id);
        if (user != null) site = sites.FirstOrDefault();

        return site;
    }

    public async Task<List<PatientSummary>> GetUserPatients(int userId)
    {
        var lastPatientList = await _lastPatientUnitOfWork.GetLastPatientList(userId);
        return await CreateLastPatientSummaries(lastPatientList);
    }

    public async Task<UserDto> GetUserAndPatients(int id)
    {
        var user = await _userUnitOfWork.GetUser(id);
        var userDto = new UserDto(user);

        var lastPatientList = await _lastPatientUnitOfWork.GetLastPatientList(id);
        if (lastPatientList != null) userDto.LastPatientSummaries = await CreateLastPatientSummaries(lastPatientList);
        return userDto;
    }

    public async Task<List<PatientSummary>> UpdateRecentPatientList(int userId, int patientId)
    {
        var patientList = await _lastPatientUnitOfWork.GetLastPatientList(userId);
        var patientIds = new List<int>();
        if (patientList == null || string.IsNullOrWhiteSpace(patientList.PatientListXml))
            patientList = new LastPatientList { UserId = userId };
        else
            patientIds = _ParseLastPatientIds(patientList);
        var newList = new List<int> { patientId };
        if (patientIds.Contains(patientId))
        {
            patientIds.Remove(patientId);
            newList.AddRange(patientIds);
        }
        else
        {
            newList.AddRange(patientIds.Count > 19 ? patientIds.GetRange(0, 19) : patientIds);
        }

        patientList.PatientListXml = _SerializeLastPatientList(newList).ToString(SaveOptions.DisableFormatting);
        if (patientList.Id > 0)
            await _lastPatientUnitOfWork.Update(patientList);
        else
            await _lastPatientUnitOfWork.Add(patientList);
        return await CreateLastPatientSummaries(patientList);
    }

    private List<int> _ParseLastPatientIds(LastPatientList patientList)
    {
        var patientIds = new List<int>();
        var xmlList = XDocument.Parse(patientList.PatientListXml).Root;
        var xValue = (from p in xmlList.Attributes()
            where p.Name == XML_VALUE
            select p).FirstOrDefault();
        if (xValue != null)
        {
            var temp = xValue.Value;
            var items = temp.Split(',');
            foreach (var item in items)
            {
                int value;
                if (int.TryParse(item, out value)) patientIds.Add(value);
            }
        }

        return patientIds;
    }

    private XElement _SerializeLastPatientList(List<int> patientIds)
    {
        var element = new XElement(XML_SETTING);
        var sb = new StringBuilder();
        var count = 0;
        foreach (var patientID in patientIds)
        {
            sb.Append(patientID.ToString());
            sb.Append(',');
            count++;
            if (count > 19) break;
        }

        element.Add(new XAttribute(XML_NAME, XML_LAST_PATIENT_LIST));
        element.Add(new XAttribute(XML_VALUE, sb.ToString().Trim(',')));
        return element;
    }

    private async Task<List<PatientSummary>> CreateLastPatientSummaries(LastPatientList patientList)
    {
        var patientSummaries = new List<PatientSummary>();
        if (!string.IsNullOrEmpty(patientList.PatientListXml))
        {
            var ids = _ParseLastPatientIds(patientList);
            foreach (var patientId in ids)
            {
                var p = await _patientsUnitOfWork.GetPatientSummary(patientId);
                if (p != null) patientSummaries.Add(p);
            }
        }

        return patientSummaries;
    }
}