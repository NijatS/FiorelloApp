using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fir.Core.Entities
{
    public class Position:BaseModel
    {
        [Required(ErrorMessage ="Fill the gap")]
        [StringLength(30)]
        [DisplayName("Position's Name")]
        public string Name { get; set; }
        public List<Employee>? Employees { get; set; }   
    }
}
