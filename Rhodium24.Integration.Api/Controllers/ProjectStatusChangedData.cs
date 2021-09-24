using System;

namespace Rhodium24.Integration.Api.Controllers
{
    public class ProjectStatusChangedData
    {
        public string ProjectStatus { get; set; }
        public Guid PartyId { get; set; }
        public Guid ProjectId { get; set; }
    }
}