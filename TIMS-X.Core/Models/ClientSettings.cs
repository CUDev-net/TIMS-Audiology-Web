using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace TIMS_X.Core.Models
{
    public class ClientSettings
    {
        public string DbInterfaceLogFile { get; set; }
        public string TimsLogFile { get; set; }
        public string WebToken { get; set; }
        public string OfficeCode { get; set; }
        public string ServerUrl { get; set; }
        public string ServerPort { get; set; }
        public bool UseADAuthentication { get; set; }
        public int UserId { get; set; }
        public int SiteId { get; set; }
        public List<int> SelectedProviders { get; set; }

        [JsonIgnore]
        public string BaseUrl => $"{ServerUrl}:{ServerPort}/";
        [JsonIgnore]
        public string NoahApi => $"{BaseUrl}api/Noah/";
        [JsonIgnore]
        public string UserApi => $"{BaseUrl}api/User/";
        [JsonIgnore]
        public string CustomerApi => $"{BaseUrl}api/Customer/";
        [JsonIgnore]
        public string PracticeApi => $"{BaseUrl}api/Practice/";
        [JsonIgnore]
        public string PatientApi => $"{BaseUrl}api/Patient/";

        [JsonIgnore]
        public string ProviderApi => $"{BaseUrl}api/Provider/";
        [JsonIgnore]
        public string SchedulerApi => $"{BaseUrl}api/Scheduler/";
        
    }
}
