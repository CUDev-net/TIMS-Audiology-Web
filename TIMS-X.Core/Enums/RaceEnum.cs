using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TIMS_X.Core.Utils;

namespace TIMS_X.Core.Enums
{
    public enum RaceEnum
    {
        /// <summary>
        /// Empty
        /// </summary>
        [Description("")]
        None = 0,
        /// <summary>
        /// American Indian or Alaska Native
        /// </summary>
        [Description("American Indian or Alaska Native")]
        AmericanIndianOrAlaskaNative = 1,
        /// <summary>
        /// Asian
        /// </summary>
        [Description("Asian")]
        Asian = 2,
        /// <summary>
        /// Black or African American
        /// </summary>
        [Description("Black or African American")]
        BlackOrAfricanAmerican = 3,
        /// <summary>
        /// Native Hawaiian or Other Pacific Islander
        /// </summary>
        [Description("Native Hawaiian or Other Pacific Islander")]
        NativeHawaiianOrOtherPacificIslander = 4,
        /// <summary>
        /// White
        /// </summary>
        [Description("White")]
        White = 5,
        /// <summary>
        /// Declined
        /// </summary>
        [Description("Declined")]
        Declined = 6,
        /// <summary>
        /// Unknown
        /// </summary>
        [Description("Unknown")]
        Unknown = 7,
        /// <summary>
        /// Hispanic or Latino
        /// </summary>
        [Description("Hispanic or Latino")]
        HispanicOrLatino = 8,
        /// <summary>
        /// Bi or Multi-Racial
        /// </summary>
        [Description("Bi or Multi-Racial")]
        BiOrMultiRacial = 9
    }

    public class Race
    {
        /// <summary>
        /// Returns two groups of languages. The fist group represents the most common language choices for the given country.
        /// The second group is the rest of the language options
        /// </summary>
        /// <param name="qbLocale"></param>
        /// <returns></returns>
        public static List<Race> LoadAll()
        {
            //var values = Enum.GetValues(typeof(RaceEnum))
            //           .Cast<RaceEnum>()
            //           .Select(race => new Race { Value = race, Name = EnumUtilities.GetDescriptionFromEnum(race) })
            //           .ToList();
            //return values;

            List<Race> list = new List<Race>();

            list.Add(new Race(RaceEnum.AmericanIndianOrAlaskaNative,
                EnumUtilities.GetDescriptionFromEnum(RaceEnum.AmericanIndianOrAlaskaNative)));

            list.Add(new Race(RaceEnum.Asian,
                EnumUtilities.GetDescriptionFromEnum(RaceEnum.Asian)));

            list.Add(new Race(RaceEnum.BiOrMultiRacial,
                EnumUtilities.GetDescriptionFromEnum(RaceEnum.BiOrMultiRacial)));

            list.Add(new Race(RaceEnum.BlackOrAfricanAmerican,
                EnumUtilities.GetDescriptionFromEnum(RaceEnum.BlackOrAfricanAmerican)));

            list.Add(new Race(RaceEnum.HispanicOrLatino,
                EnumUtilities.GetDescriptionFromEnum(RaceEnum.HispanicOrLatino)));

            list.Add(new Race(RaceEnum.NativeHawaiianOrOtherPacificIslander,
                EnumUtilities.GetDescriptionFromEnum(RaceEnum.NativeHawaiianOrOtherPacificIslander)));

            list.Add(new Race(RaceEnum.White,
                EnumUtilities.GetDescriptionFromEnum(RaceEnum.White)));

            list.Add(new Race(RaceEnum.Declined,
                EnumUtilities.GetDescriptionFromEnum(RaceEnum.Declined)));

            list.Add(new Race(RaceEnum.Unknown,
                EnumUtilities.GetDescriptionFromEnum(RaceEnum.Unknown)));

            return list;

        }

        public Race(RaceEnum value, string name)
        {
            Value = value;
            Name = name;
        }

        public string Name { get; set; }
        public RaceEnum Value { get; set; }
    }
}
