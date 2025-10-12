using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TIMS_X.Core.Attributes;
using TIMS_X.Core.Utils;

namespace TIMS_X.Core.Enums
{
    // We don't need all of the time zones in the world
    // this a list of time zones we need to support our customer base

    public enum TimeZoneCountry
    {
        [Description("United States")]
        USA,
        [Description("Canada")]
        CA,
        [Description("United Kingdom")]
        UK,
        [Description("United Arab Emirites")]
        UAE,
        [Description("New Zealand")]
        NZ,
        [Description("Australia")]
        AU
    }
    public enum TimsTimeZone
    {
        // United States
        [Description("Pacific Standard Time")]
        [Countries(new[] { TimeZoneCountry.USA, TimeZoneCountry.CA })]
        Pacific,
        [Description("Mountain Standard Time")]
        [Countries(new[] { TimeZoneCountry.USA, TimeZoneCountry.CA })]
        Mountain,
        [Description("Central Standard Time")]
        [Countries(new[] { TimeZoneCountry.USA, TimeZoneCountry.CA })]
        Central,
        [Description("Eastern Standard Time")]
        [Countries(new[] { TimeZoneCountry.USA, TimeZoneCountry.CA })]
        Eastern,

        // Canada uses all of US time zones + two more
        [Description("Atlantic Standard Time")]
        [Countries(new[] { TimeZoneCountry.CA })]
        Atlantic,
        [Description("Newfoundland Standard Time")]
        [Countries(new[] { TimeZoneCountry.CA })]
        Newfoundland,

        // UK
        [Description("Greenwich Mean Time")]
        [Countries(new[] { TimeZoneCountry.UK })]
        Greenwich,

        // UAE
        [Description("Arabian Standard Time")]
        [Countries(new[] { TimeZoneCountry.UAE })]
        Gulf,

        // Australia
        [Description("W. Australia Standard Time")]
        [Countries(new[] { TimeZoneCountry.AU })]
        AusWestern,
        [Description("Cen. Australia Standard Time")]
        [Countries(new[] { TimeZoneCountry.AU })]
        AusCentral,
        [Description("E. Australia Standard Time")]
        [Countries(new[] { TimeZoneCountry.AU })]
        AusEastern,

        // New Zealand
        [Description("New Zealand Standard Time")]
        [Countries(new[] { TimeZoneCountry.NZ })]
        NewZealand
    };

    public class TimsTimeZoneModel
    {
        public string Name { get; set; }
        public TimsTimeZone? Value { get; set; }
    }

    public static class TimsTimeZoneInfo
    {
        public static bool HasCountryAttribute(TimsTimeZone value, TimeZoneCountry country)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (CountriesAttribute[])fi.GetCustomAttributes(
                typeof(CountriesAttribute),
                false);

            if (attributes.Length > 0)
            {
                return attributes[0].Countries.Contains(country);
            }

            return false;
        }

        public static TimeZoneCountry[] GetCountries(TimsTimeZone value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (CountriesAttribute[])fi.GetCustomAttributes(
                typeof(CountriesAttribute),
                false);

            if (attributes.Length > 0)
            {
                return attributes[0].Countries;
            }

            return new TimeZoneCountry[] { };
        }

        public static Dictionary<string, List<TimsTimeZoneModel>> GetTimeZonesByCountry()
        {
            var timeZones = Enum.GetValues(typeof(TimsTimeZone))
               .Cast<TimsTimeZone>()
               .Select(value => new TimsTimeZoneModel {Name = EnumUtilities.GetDescriptionFromEnum(value), Value = value})
               .ToList();

            var result = new Dictionary<string, List<TimsTimeZoneModel>>();
            result.Add("None", new List<TimsTimeZoneModel> { new TimsTimeZoneModel { Name = "Default" } });
            foreach ( var tz in timeZones) 
            {
                var countries = GetCountries(tz.Value.Value);
                foreach(var country in countries)
                {
                    var countryName = EnumUtilities.GetDescriptionFromEnum(country);
                    if (!result.ContainsKey(countryName))
                    {
                        result[countryName] = new List<TimsTimeZoneModel>();
                    }

                    result[countryName].Add(tz);
                }
            }




            return result;
        }
    }
}
