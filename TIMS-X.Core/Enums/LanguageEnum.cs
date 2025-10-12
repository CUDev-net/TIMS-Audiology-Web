using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TIMS_X.Core.Attributes;
using TIMS_X.Core.Utils;

namespace TIMS_X.Core.Enums
{
    public enum LanguageEnum
    {
        /// <summary>
        /// English
        /// </summary>
        [Description("")]
        None = 0,
        /// <summary>
        /// English
        /// </summary>
        [Description("English, United States"), LanguageCode("en-US"), TwilioAliceSupport]
        English = 1,
        /// <summary>
        /// Spanish
        /// </summary>
        [Description("Spanish, Mexico"), LanguageCode("es-MX"), TwilioAliceSupport]
        Spanish = 2,
        /// <summary>
        /// German
        /// </summary>
        [Description("German, Germany"), LanguageCode("de-DE"), TwilioAliceSupport]
        German = 3,
        /// <summary>
        /// French
        /// </summary>
        [Description("French, Canada"), LanguageCode("fr-CA"), TwilioAliceSupport]
        French = 4,
        /// <summary>
        /// Russian
        /// </summary>
        [Description("Russian, Russia"), LanguageCode("ru-RU"), TwilioAliceSupport]
        Russian = 5,
        /// <summary>
        /// Italian
        /// </summary>
        [Description("Italian, Italy"), LanguageCode("it-IT"), TwilioAliceSupport]
        Italian = 6,
        /// <summary>
        /// Chinese-Mandarin
        /// </summary>
        [Description("Chinese, Mandarin"), LanguageCode("zh-CN"), TwilioAliceSupport]
        ChineseMandarin = 7,
        /// <summary>
        /// ChineseCantonese
        /// </summary>
        [Description("Chinese, Cantonese"), LanguageCode("zh-HK"), TwilioAliceSupport]
        ChineseCantonese = 8,
        /// <summary>
        /// Japanese
        /// </summary>
        [Description("Japanese, Japan"), LanguageCode("ja-JP"), TwilioAliceSupport]
        Japanese = 9,
        /// <summary>
        /// Korean
        /// </summary>
        [Description("Korean, Korea"), LanguageCode("ko-KR"), TwilioAliceSupport]
        Korean = 10,
        /// <summary>
        /// Vietnamese
        /// </summary>
        [Description("Vietnamese, Vietnam"), LanguageCode("vi-VN")]
        Vietnamese = 11,
        /// <summary>
        /// Tagalog
        /// </summary>
        [Description("Tagalog, Phillipines"), LanguageCode("tl-PH")]
        Tagalog = 12,
        /// <summary>
        /// Arabic
        /// </summary>
        [Description("Arabic, Iraq"), LanguageCode("ar-IQ")]
        Arabic = 13,
        /// <summary>
        /// Indo-Pacific
        /// </summary>
        [Description("Indo-Pacific")]
        IndoPacific = 14,
        /// <summary>
        /// Gaelic
        /// </summary>
        [Description("Gaelic")]
        Gaelic = 15,
        /// <summary>
        /// American Sign Language
        /// </summary>
        [Description("American Sign Language")]
        AmericanSignLanguage = 16,
        /// <summary>
        /// Other
        /// </summary>
        [Description("Other")]
        Other = 17,
        /// <summary>
        /// Gaelic
        /// </summary>
        [Description("Dutch, Netherlands"), LanguageCode("nl-NL")]
        Dutch = 18,
        /// <summary>
        /// Mixtec
        /// </summary>
        [Description("Mixtec")]
        Mixtec = 19,
        /// <summary>
        /// Hmong
        /// </summary>
        [Description("Hmong")]
        Hmong = 20,
        /// <summary>
        /// TeReoMaori
        /// </summary>
        [Description("Te Reo Maori")]
        TeReoMaori = 21,
        /// <summary>
        /// Danish
        /// </summary>
        [Description("Danish, Denmark"), LanguageCode("da-DK"), TwilioAliceSupport]
        Danish = 22,
        /// <summary>
        /// EnglishAustralia
        /// </summary>
        [Description("English, Australia"), LanguageCode("en-AU"), TwilioAliceSupport]
        EnglishAustralia = 23,
        /// <summary>
        /// EnglishCanada
        /// </summary>
        [Description("English, Canada"), LanguageCode("en-CA"), TwilioAliceSupport]
        EnglishCanada = 24,
        /// <summary>
        /// EnglishUK
        /// </summary>
        [Description("English, UK"), LanguageCode("en-GB"), TwilioAliceSupport]
        EnglishUk = 25,
        /// <summary>
        /// EnglishIndia
        /// </summary>
        [Description("English, India"), LanguageCode("en-IN"), TwilioAliceSupport]
        EnglishIndia = 26,
        /// <summary>
        /// Catalan
        /// </summary>
        [Description("Catalan, Spain"), LanguageCode("ca-ES"), TwilioAliceSupport]
        Catalan = 27,
        /// <summary>
        /// SpanishSpain
        /// </summary>
        [Description("Spanish, Spain"), LanguageCode("es-ES"), TwilioAliceSupport]
        SpanishSpain = 28,
        /// <summary>
        /// Finnish
        /// </summary>
        [Description("Finnish, Finland"), LanguageCode("fi-FI"), TwilioAliceSupport]
        Finnish = 29,
        /// <summary>
        /// FrenchFrance
        /// </summary>
        [Description("French, France"), LanguageCode("fr-FR"), TwilioAliceSupport]
        FrenchFrance = 30,
        /// <summary>
        /// Norwegian
        /// </summary>
        [Description("Norwegian, Norway"), LanguageCode("nb-NO"), TwilioAliceSupport]
        Norwegian = 31,
        /// <summary>
        /// Polish
        /// </summary>
        [Description("Polish, Poland"), LanguageCode("pl-PL"), TwilioAliceSupport]
        Polish = 32,
        /// <summary>
        /// PortugueseBrazil
        /// </summary>
        [Description("Portuguese, Brazil"), LanguageCode("pt-BR"), TwilioAliceSupport]
        Portuguese = 33,
        /// <summary>
        /// PortuguesePortugal
        /// </summary>
        [Description("Portuguese, Portugal"), LanguageCode("pt-PT"), TwilioAliceSupport]
        PortuguesePortugal = 34,
        /// <summary>
        /// Swedish
        /// </summary>
        [Description("Swedish, Sweden"), LanguageCode("sv-SE"), TwilioAliceSupport]
        Swedish = 35,
        /// <summary>
        /// ChineseTaiwaneseMandarin
        /// </summary>
        [Description("Chinese, Taiwanese Mandarin"), LanguageCode("zh-TW"), TwilioAliceSupport]
        ChineseTaiwaneseMandarin = 36,
        /// <summary>
        /// Unknown
        /// </summary>
        [Description("Unknown")]
        Unknown = 37,
        /// <summary>
        /// Ukranian
        /// </summary>
        [Description("Ukranian")]
        Ukranian = 38,

        /// <summary>
        /// Unknown
        /// </summary>
        [Description("Swahili")]
        Swahili = 39,


        /// <summary>
        /// Declined
        /// </summary>
        [Description("Declined")]
        Declined = 99
    }

    public class Language
    {
        /// <summary>
        /// Returns two groups of languages. The fist group represents the most common language choices for the given country.
        /// The second group is the rest of the language options
        /// </summary>
        /// <param name="qbLocale"></param>
        /// <returns></returns>
        public static List<Language> LoadAll(string qbLocale = "us")
        {
            var values = Enum.GetValues(typeof(LanguageEnum))
                       .Cast<LanguageEnum>()
                       .Select(lang => new Language{Value= lang, Name=EnumUtilities.GetDescriptionFromEnum(lang) })
                       .ToList();
            
            // Try to put most common language choices at the top of the list according to customer country

            // Start with None
            List<Language> result = values.Where(x => x.Value == LanguageEnum.None).ToList();
            
            switch (qbLocale.ToLower())
            {
                case "us":
                    // Take English US, Spanish Mexico
                    result.AddRange(values.Where(x => x.Value == LanguageEnum.English || x.Value == LanguageEnum.Spanish).OrderBy(x => x.Name));
                    break;
                case "ca":
                    // Take Canadian US English US, Canadian French
                    result.AddRange(values.Where(x => x.Value == LanguageEnum.English || x.Value == LanguageEnum.EnglishCanada || x.Value == LanguageEnum.French).OrderBy(x => x.Name));
                    break;
                case "nz":
                case "au":
                    // Take English AU
                    result.AddRange(values.Where(x => x.Value == LanguageEnum.EnglishAustralia).OrderBy(x => x.Name));
                    break;
                case "uk":
                    // Take English UK
                    result.AddRange(values.Where(x => x.Value == LanguageEnum.EnglishUk).OrderBy(x => x.Name));
                    break;
               
            }

            // Remove the special values and the ones we have already added to result
            var specialValues = values.Where(x => x.Value == LanguageEnum.Declined || x.Value == LanguageEnum.Other || x.Value == LanguageEnum.Unknown).OrderBy(x => x.Name).ToList();
            values = values.Except(specialValues).Except(result).OrderBy(x => x.Name).ToList();
            result.AddRange(values);
            result.AddRange(specialValues);
            return result;

        }
        public string Name { get; set; }
        public LanguageEnum Value { get; set; }
    }
}
