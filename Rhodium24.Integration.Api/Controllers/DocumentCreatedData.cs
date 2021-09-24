using System;

namespace Rhodium24.Integration.Api.Controllers
{
    public class DocumentCreatedData
    {
        public string DocumentType { get; set; }
        public string FileName { get; set; }
        public string ResourceUri { get; set; }
        public string GlnNumberSender { get; set; }
        public string GlnNumberReceiver { get; set; }
        public Guid PartyId { get; set; }
        public Guid ProjectId { get; set; }
    }
}