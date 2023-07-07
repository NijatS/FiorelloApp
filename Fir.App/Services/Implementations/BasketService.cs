using Fir.App.Context;
using Fir.App.Services.Interfaces;
using Fir.App.ViewModels;
using Fir.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Fir.App.Services.Implementations
{
    public class BasketService : IBasketService
    {
        private readonly FirDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public BasketService(FirDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task AddBasket(int id)
        {
            if (!await _context.Products.AnyAsync(x => x.Id == id && !x.IsDeleted))
            {
                throw new Exception("Not found");
            }
            var CookiesJson = _contextAccessor.HttpContext?.Request.Cookies["basket"];
            if (CookiesJson is null)
            {
                List<BasketVM> baskets = new List<BasketVM>();
                BasketVM basket = new BasketVM
                {
                    ProductId = id,
                    Count = 1
                };
                baskets.Add(basket);
                CookiesJson = JsonConvert.SerializeObject(baskets);
                _contextAccessor.HttpContext?.Response.Cookies.Append("basket", CookiesJson);
            }
            else
            {
                List<BasketVM>? baskets = JsonConvert.
                    DeserializeObject<List<BasketVM>>(CookiesJson);
                BasketVM? basket = baskets.FirstOrDefault(x => x.ProductId == id);
                if (basket is null)
                {
                    BasketVM basket1 = new BasketVM
                    {
                        ProductId = id,
                        Count = 1
                    };
                    baskets.Add(basket1);
                }
                else
                {
                    basket.Count++;
                }
                CookiesJson = JsonConvert.SerializeObject(baskets);
                _contextAccessor.HttpContext?.Response.Cookies.Append("basket", CookiesJson);
            }
        }

        public async Task<List<BasketItemVM>> GetAll()
        {
            var CookiesJson = _contextAccessor.HttpContext?.Request.Cookies["basket"];
            if (CookiesJson is not null)
            {
                List<BasketVM>? baskets = JsonConvert.DeserializeObject<List<BasketVM>>(CookiesJson);

                List<BasketItemVM> basketItems = new();
                foreach (var item in baskets)
                {
                    Product? product = await _context.Products
                        .Where(x => x.Id == item.ProductId && !x.IsDeleted).
                        Include(x => x.Discount).
                        Include(x => x.ProductImages).
                        FirstOrDefaultAsync();
                    if (product is not null)
                    {
                        BasketItemVM basketItem = new BasketItemVM
                        {
                            Name = product.Name,
                            Count = item.Count,
                            Image = product.ProductImages.Where(x => x.isMain && !x.IsDeleted).FirstOrDefault().Image,
                            Price = product.DiscountId == null ? product.Price : (product.Price - (product.Price * product.Discount.Percent / 100))
                        };
                        basketItems.Add(basketItem);
                    }
                }
                return basketItems;
            }
           
            return new List<BasketItemVM>();
        }
    }
}
