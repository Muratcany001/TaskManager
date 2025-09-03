using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.DocumentDtos
{
    public class UpdateDocumentDto
    {
         public int id {  get; set; }
        public IFormFile file { get; set; }
    }
}
