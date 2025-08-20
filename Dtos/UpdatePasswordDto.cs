using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class UpdatePasswordDto
    {
        public string CurrentPassowrd { get; set; }
        public string NewPassword { get; set; }
    }
}
