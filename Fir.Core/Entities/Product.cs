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
    public class Product:BaseModel
    {
        [Required]
        public string Name { get; set; }
		[Required]
		public string Description { get; set; }
		[Required]
		public string AddTitle { get; set; }
		[Required]
		public string AddInfo { get; set; }
		[Required]
		public double Weight { get; set; }
		[Required]
		public string Dimension { get; set; }
		[Required]
		public double Price { get; set; }
		public int? DiscountId { get; set; }
        public Discount? Discount { get; set; }
        public bool InStock { get; set; }
        public List<ProductCategory>? ProductCategories { get; set; }
        public List<ProductTag>? ProductTags { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        [NotMapped]
        public List<int> CategoryIds { get; set; }
        [NotMapped]
        public List<int> TagIds { get; set; }
        [NotMapped]
        public List<IFormFile>? FormFiles { get; set; }
    }
}
