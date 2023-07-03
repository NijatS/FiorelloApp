using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fir.Core.Entities
{
    public class Employee:BaseModel
    {
        [Required]
        public string FullName { get; set; }
        public string Description { get; set; }
        public string? Image { get; set; }
        public int PositionId { get; set; }
        public Position? Position { get; set; }
        [NotMapped]
        public IFormFile? FormFile { get; set; }
    }

}
