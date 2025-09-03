using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.DocumentDtos
{
    public class CreateDocumentDto
    {
        public int taskId { get; set; }
        public string title { get; set; }
        public IFormFile file { get; set; }
    }
}
