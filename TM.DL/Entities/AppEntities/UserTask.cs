using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TM.DAL.Entities.AppEntities;
namespace TM.DAL.Entities.AppEntities
{
    public class UserTask
    {
        public int Id { get; set; }
        public int FirstUpdater { get; set; }
        public int LastUpdater { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? CurrentVersionId { get; set; }
        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        public TaskVersion? CurrentVersion { get; set; }
        [JsonIgnore]
        public ICollection<Document?> Documents { get; set; } = new List<Document>();
        [JsonIgnore]
        public ICollection<TaskVersion?> Versions { get; set; } = new List<TaskVersion>();
    }
}
