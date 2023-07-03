using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fir.Core.Entities
{
    public class Tag:BaseModel
    {
        [Required]

        public string Name { get; set; }
        public List<ProductTag>? ProductTags { get; set; }
    }
}
