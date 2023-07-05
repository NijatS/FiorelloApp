using Fir.Core.Entities;

namespace Fir.App.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Category> categories { get; set; }
        public IEnumerable<Employee> employees { get; set; }
        public IEnumerable<Blog> blogs { get; set; }
        public IEnumerable<Product> products { get; set; }
    }
}
