using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TM.DAL.Entities.AppEntities
{
    public class TaskVersion
    {
        public int Id { get; set; }
        public int VersionNumber { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public string Status { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? TaskId { get; set; }
        public UserTask? Task { get; set; }
        public List<Document> Documents { get; set; } = new List<Document>();

    }
}
