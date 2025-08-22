using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TM.DAL.Entities.AppEntities
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string FilePath { get; set; }
        public int? TaskId { get; set; }
        public int? TaskVersionId { get; set; }
        public UserTask? Task { get; set; }
        public bool IsActive { get; set; }

    }
}
