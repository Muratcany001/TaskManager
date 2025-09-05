using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TM.DAL.Entities.AppEntities
{
    public class Token
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TokenString { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpireAt {  get; set; }
        public bool IsActive { get; set; } = false;

        public User user { get; set; }
    }
}
