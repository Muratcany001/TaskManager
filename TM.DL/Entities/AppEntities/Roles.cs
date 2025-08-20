using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TM.DAL.Entities.AppEntities
{
    public class Roles
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
