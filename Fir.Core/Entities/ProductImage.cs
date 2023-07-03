using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fir.Core.Entities
{
    public class ProductImage:BaseModel
    {
        public bool isMain { get; set; }
        public string? Image { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
