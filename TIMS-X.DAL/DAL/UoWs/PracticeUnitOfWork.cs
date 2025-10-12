using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json.Linq;
using TIMS_X.Core.Attributes;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Models;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IPracticeUnitOfWork : IUnitOfWork
{
    Task<Practice> GetPractice(Func<IQueryable<Practice>, IIncludableQueryable<Practice, object>> includes = null);
    Task<string> GetPracticeAudigyId(string officeCode = null);
    Task<BusinessRules> GetPracticeBusinessRules();
    Task<PracticeSummary> GetPracticeSummary();
}

public class PracticeUnitOfWork : UnitOfWorkBase, IPracticeUnitOfWork
{
    private readonly Dictionary<string, string> mAudigyPracticeIds;
    public PracticeUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
        mAudigyPracticeIds = AudigyCustomers.GetCustomers();
    }

    protected override string TableName => nameof(Practice);

    public async Task<Practice> GetPractice(
        Func<IQueryable<Practice>, IIncludableQueryable<Practice, object>> includes = null)
    {
        return await Single(null, includes);
    }

    public async Task<string> GetPracticeAudigyId(string officeCode = null)
    {
        if(string.IsNullOrEmpty(officeCode))
        {
            Func<IQueryable<Practice>, IIncludableQueryable<Practice, object>> includes = null;
            var practice = await Single(null, includes);
            officeCode = practice.OfficeCode;
        }
        if (!string.IsNullOrEmpty(officeCode))
        {
            officeCode = officeCode.ToUpper();

            if (mAudigyPracticeIds.ContainsKey(officeCode))
            {
                return mAudigyPracticeIds[officeCode];
            }
        }

        return null;
    }

    public async Task<BusinessRules> GetPracticeBusinessRules()
    {
        var practice = await Get<Practice>().Select(p => new Practice
        {
            BusinessRules = p.BusinessRules
        }).FirstOrDefaultAsync();

        var xDocument = XDocument.Parse(practice.BusinessRules);
        if (xDocument.Root == null) return null;

        var xRules = xDocument.Root.Elements()
            .Where(p => p.Name == BusinessRules.XML_TAG_BR || p.Name == BusinessRules.XML_TAG_BUSINESS_RULE)
            .ToList();

        var businessRules = new BusinessRules();
        var classProperties = typeof(BusinessRules).GetProperties();

        var propertyMap = new Dictionary<string, PropertyInfo>();

        foreach (var property in classProperties)
        {
            string xmlTag = null;
            var xmlTagAttribute = (XmlTagAttribute)Attribute.GetCustomAttribute(property, typeof(XmlTagAttribute));
            if (xmlTagAttribute != null) xmlTag = xmlTagAttribute.Name;

            if (!string.IsNullOrWhiteSpace(xmlTag)) propertyMap[xmlTag] = property;
        }

        foreach (var xElement in xRules)
        {
            var xName = xElement.Attributes().FirstOrDefault(x => x.Name == BusinessRules.XML_ATTRIBUTE_NAME);
            if (xName != null)
            {
                var name = xName.Value;
                if (name == BusinessRules.XML_TAG_OPPORTUNITY_TRACKING)
                {
                    var xThresholdMonths = xElement.Descendants().First(x =>
                        x.Name == nameof(businessRules.OpportunityTracking.ThresholdMonths));

                    var xValue = xThresholdMonths.Attributes().First(x => x.Name == BusinessRules.XML_ATTRIBUTE_VALUE);

                    if (int.TryParse(xValue.Value, out var value))
                        businessRules.OpportunityTracking.ThresholdMonths = value;

                    var otProperties = typeof(OpportunityTrackingRules).GetProperties();
                    var xThresholdValues = xElement.Descendants()
                        .FirstOrDefault(x => x.Name == BusinessRules.XML_TAG_THRESHOLD_VALUES);
                    if (xThresholdValues != null)
                        foreach (var xAttribute in xThresholdValues.Attributes())
                            if (decimal.TryParse(xAttribute.Value, out var threshold))
                            {
                                var property = otProperties.FirstOrDefault(x => x.Name == xAttribute.Name);
                                if (property != null) property.SetValue(businessRules.OpportunityTracking, threshold);
                            }
                }
                else if (propertyMap.ContainsKey(name))
                {
                    var property = propertyMap[name];

                    var strValue = xElement.Attributes()
                        .FirstOrDefault(x => x.Name == BusinessRules.XML_ATTRIBUTE_VALUE)?.Value;

                    if (!string.IsNullOrWhiteSpace(strValue))
                    {
                        if (property.PropertyType == typeof(bool))
                        {
                            var value = string.Equals(strValue, BusinessRules.XML_VALUE_TRUE,
                                StringComparison.CurrentCultureIgnoreCase);

                            property.SetValue(businessRules, value);
                        }
                        else if (property.PropertyType == typeof(int))
                        {
                            if (int.TryParse(strValue, out var value)) property.SetValue(businessRules, value);
                        }
                        else if (property.PropertyType == typeof(long))
                        {
	                        if (long.TryParse(strValue, out var value)) property.SetValue(businessRules, value);
                        }
                        else
                        {
							property.SetValue(businessRules, strValue);
						}
					}
                }
            }
        }

        return businessRules;
    }

    public async Task<PracticeSummary> GetPracticeSummary()
    {
        return await Get<Practice>().Select(p => new PracticeSummary
        {
            Id = p.Id,
            Inactive = p.Inactive,
            Fax = p.Fax,
            EmailDisclaimer = p.EmailDisclaimer,
            OfficeCode = p.OfficeCode,
            TaxId = p.TaxId,
            Name = p.Name,
            UseBlockScheduling = p.UseBlockScheduling,
            UsesNoahDataMining = p.UsesNoahDataMining,
            UsesAdAuthentication = p.UsesAdAuthentication,
            LinkAppointmentHistory = p.LinkAppointmentHistory,
            FirstName = p.FirstName,
            LastName = p.LastName,
            BillingAddress1 = p.BillingAddress1,
            BillingAddress2 = p.BillingAddress2,
            BillingCity = p.BillingCity,
            BillingState = p.BillingState,
            BillingZipCode = p.BillingZipCode,
            BillingPhoneNumber = p.BillingPhoneNumber,
            UseSiteAddressForReports = p.UseSiteAddressForReports,
            UpdatedDate = p.UpdatedDate,
            Locale = p.QbLocale
        }).FirstOrDefaultAsync();
    }
}