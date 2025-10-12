using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TIMS_X.Core.Utils;

namespace TIMS_X.Core.Enums
{
    public enum EthnicityEnum
    {
        /// <summary>
        /// Unknown
        /// </summary>
        [Description("")]
        None = 0,
        /// <summary>
        /// Hispanic or Latino
        /// </summary>
        [Description("Hispanic or Latino")]
        HispanicOrLatino = 1,
        /// <summary>
        /// Not Hispanic Or Latino
        /// </summary>
        [Description("Not Hispanic Or Latino")]
        NotHispanicOrLatino = 2,

        [Description("African")]
        African = 3,
        [Description("Asian")]
        Asian = 4,
        [Description("Chinese")]
        Chinese = 5,
        [Description("Cook Island Maori")]
        CookIslandMaori = 6,
        [Description("European Not Defined")]
        EuropeanNotDefined = 7,
        [Description("Fijian")]
        Fijian = 8,
        [Description("Indian")]
        Indian = 9,
        [Description("Latin American / Hispanic")]
        LatinAmericanHispanic = 10,
        [Description("Maori")]
        Maori = 11,
        [Description("Middle Eastern")]
        MiddleEastern = 12,
        [Description("NZ European")]
        NZEuropean = 13,
        [Description("Niuean")]
        Niuean = 14,
        [Description("Not Stated")]
        NotStated = 15,
        [Description("Other")]
        Other = 16,
        [Description("Other Asian")]
        OtherAsian = 17,
        [Description("Other European")]
        OtherEuropean = 18,
        [Description("Pacific")]
        Pacific = 19,
        [Description("Samoan")]
        Samoan = 20,
        [Description("South East Asian")]
        SouthEastAsian = 21,
        [Description("Tokelauan")]
        Tokelauan = 22,
        [Description("Tongan")]
        Tongan = 23,
        [Description("Aboriginal/Torres Straight Islander")]
        AboriginalTorres = 24,
        [Description("South African")]
        SouthAfrican = 25,
        [Description("Pacific Islander")]
        PacificIslander = 26,
        [Description("United Kingdom")]
        UnitedKingdom = 27,
        /// <summary>
        /// Unknown
        /// </summary>
        [Description("Unknown")]
        Unknown = 28,

        /// <summary>
        /// Declined
        /// </summary>
        [Description("Declined")]
        Declined = 99
    }

    public class Ethnicity
    {
        /// <summary>
        /// Returns two groups of languages. The fist group represents the most common language choices for the given country.
        /// The second group is the rest of the language options
        /// </summary>
        /// <param name="qbLocale"></param>
        /// <returns></returns>
        public static List<Ethnicity> LoadAll(string qbLocale)
        {
            List<Ethnicity> list = new List<Ethnicity>();

            if (qbLocale.ToUpper() == "NZ" || qbLocale.ToUpper() == "AU")
            {
                list.Add(new Ethnicity(EthnicityEnum.AboriginalTorres,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.AboriginalTorres)));

                list.Add(new Ethnicity(EthnicityEnum.African,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.African)));

                list.Add(new Ethnicity(EthnicityEnum.Asian,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.Asian)));

                list.Add(new Ethnicity(EthnicityEnum.Chinese,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.Chinese)));

                list.Add(new Ethnicity(EthnicityEnum.CookIslandMaori,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.CookIslandMaori)));

                list.Add(new Ethnicity(EthnicityEnum.EuropeanNotDefined,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.EuropeanNotDefined)));

                list.Add(new Ethnicity(EthnicityEnum.Fijian,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.Fijian)));

                list.Add(new Ethnicity(EthnicityEnum.Indian,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.Indian)));

                list.Add(new Ethnicity(EthnicityEnum.LatinAmericanHispanic,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.LatinAmericanHispanic)));

                list.Add(new Ethnicity(EthnicityEnum.Maori,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.Maori)));

                list.Add(new Ethnicity(EthnicityEnum.MiddleEastern,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.MiddleEastern)));

                list.Add(new Ethnicity(EthnicityEnum.NZEuropean,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.NZEuropean)));

                list.Add(new Ethnicity(EthnicityEnum.Niuean,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.Niuean)));

                list.Add(new Ethnicity(EthnicityEnum.NotStated,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.NotStated)));

                list.Add(new Ethnicity(EthnicityEnum.Other,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.Other)));

                list.Add(new Ethnicity(EthnicityEnum.OtherAsian,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.OtherAsian)));

                list.Add(new Ethnicity(EthnicityEnum.OtherEuropean,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.OtherEuropean)));

                list.Add(new Ethnicity(EthnicityEnum.Pacific,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.Pacific)));

                list.Add(new Ethnicity(EthnicityEnum.Samoan,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.Samoan)));

                list.Add(new Ethnicity(EthnicityEnum.SouthAfrican,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.SouthAfrican)));

                list.Add(new Ethnicity(EthnicityEnum.SouthEastAsian,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.SouthEastAsian)));

                list.Add(new Ethnicity(EthnicityEnum.Tokelauan,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.Tokelauan)));

                list.Add(new Ethnicity(EthnicityEnum.Tongan,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.Tongan)));

                list.Add(new Ethnicity(EthnicityEnum.PacificIslander,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.PacificIslander)));

                list.Add(new Ethnicity(EthnicityEnum.UnitedKingdom,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.UnitedKingdom)));
            }
            else
            {
                list.Add(new Ethnicity(EthnicityEnum.HispanicOrLatino,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.HispanicOrLatino)));

                list.Add(new Ethnicity(EthnicityEnum.NotHispanicOrLatino,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.NotHispanicOrLatino)));

                list.Add(new Ethnicity(EthnicityEnum.Declined,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.Declined)));

                list.Add(new Ethnicity(EthnicityEnum.Unknown,
                    EnumUtilities.GetDescriptionFromEnum(EthnicityEnum.Unknown)));
            }

            return list;


            //var values = Enum.GetValues(typeof(EthnicityEnum))
            //           .Cast<EthnicityEnum>()
            //           .Select(ethnicity => new Ethnicity { Value = ethnicity, Name = EnumUtilities.GetDescriptionFromEnum(ethnicity) })
            //           .ToList();
            //return values;

        }

        public Ethnicity(EthnicityEnum value, string name)
        {
            Value = value;
            Name = name;
        }

        public string Name { get; set; }
        public EthnicityEnum Value { get; set; }
    }
}
