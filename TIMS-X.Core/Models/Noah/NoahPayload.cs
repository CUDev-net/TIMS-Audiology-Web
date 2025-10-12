using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain.Noah;

namespace TIMS_X.Core.Models.Noah
{
    public class NoahPayload
    {
        public NoahPayload()
        {
            Sessions = new List<N4Session>();
            Actions = new Dictionary<int, List<N4Action>>();
            ActionReferences = new Dictionary<int, int[]>();
        }
        public N4Patient Patient { get; set; }
        public List<N4Session> Sessions { get; set; }
        public Dictionary<int, List<N4Action>> Actions { get; set; }
        public Dictionary<int, int[]> ActionReferences { get; set; }
    }
}
