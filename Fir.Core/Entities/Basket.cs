using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fir.Core.Entities
{
    public class Basket:BaseModel
    {
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<BasketItem> basketItems { get; set; }
    }
}
