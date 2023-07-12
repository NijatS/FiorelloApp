using Fir.App.Context;
using Fir.App.Services.Interfaces;
using Fir.App.ViewModels;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Fir.App.Services.Implementations
{
    public class BasketService : IBasketService
    {
        private readonly FirDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public BasketService(FirDbContext context, IHttpContextAccessor contextAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public async Task AddBasket(int id)
        {
            if (!await _context.Products.AnyAsync(x => x.Id == id && !x.IsDeleted))
            {
                throw new Exception("Not found");
            }
            if (_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);
                Basket? basket = await _context.Baskets.
                    Include(x => x.BasketItems.Where(y => !y.IsDeleted)).
                    Where(x => !x.IsDeleted && x.AppUserId == appUser.Id).FirstOrDefaultAsync();
                if(basket is null)
                {
                    basket = new Basket()
                    {
                        AppUserId = appUser.Id,
                        CreatedDate = DateTime.Now,
                    };
                    await _context.AddAsync(basket);
                    BasketItem basketItem = new BasketItem()
                    {
                        Basket = basket,
                        Count = 1,
                        CreatedDate = DateTime.Now,
                        ProductId = id
                    };
                    await _context.AddAsync(basketItem);
                }
                else
                {
                    BasketItem basketItem = await _context.BasketItems.Where(x => x.ProductId == id && !x.IsDeleted).FirstOrDefaultAsync();
                    if(basketItem is null)
                    {
                        basketItem = new BasketItem()
                        {
                            Basket = basket,
                            Count = 1,
                            CreatedDate = DateTime.Now,
                            ProductId = id
                        };
                        await _context.AddAsync(basketItem);
                    }
                    else
                    {
                        basketItem.Count++;
                    }
                }
               await _context.SaveChangesAsync();
            }
            else
            {
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
        }
        public async Task<List<BasketItemVM>> GetAll()
        {
            if (_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);
                Basket? basket = await _context.Baskets.
                             Include(x => x.BasketItems.Where(y => !y.IsDeleted)).
                             ThenInclude(x=>x.Product).
                             ThenInclude(x => x.ProductImages).
                             Include(x => x.BasketItems.Where(y => !y.IsDeleted)).
                             ThenInclude(x => x.Product).
                             ThenInclude(x => x.Discount).
                             Where(x => !x.IsDeleted && x.AppUserId == appUser.Id).FirstOrDefaultAsync();

                if (basket is not null)
                {
                    List<BasketItemVM> basketItems = new List<BasketItemVM>();
                    foreach(var item in  basket.BasketItems)
                    {
                        basketItems.Add(new BasketItemVM
                        {
                            ProductId = item.ProductId,
                            Count = item.Count,
                            Image = item.Product.ProductImages.FirstOrDefault(x => x.isMain && !x.IsDeleted).Image,
                            Name = item.Product.Name,
                            Price = item.Product.DiscountId == null ? item.Product.Price : (item.Product.Price - (item.Product.Price * item.Product.Discount.Percent / 100))
                        });
                    }
                    return basketItems;
                }
            }
            else
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
                                ProductId = item.ProductId,
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
            }
            return new List<BasketItemVM>();

        }

        public async Task Remove(int id)
        {
            if (_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
               {
                    AppUser appUser = await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);
                    Basket? basket = await _context.Baskets.
                        Include(x => x.BasketItems.Where(y => !y.IsDeleted)).
                        Where(x => !x.IsDeleted && x.AppUserId == appUser.Id).FirstOrDefaultAsync();
                BasketItem basketItem =await _context.BasketItems.Where(x => !x.IsDeleted && x.BasketId == basket.Id && x.ProductId == id).FirstOrDefaultAsync();
                if(basketItem is not null)
                {
                    basketItem.IsDeleted = true;
                    await _context.SaveChangesAsync();
                }
                 }
            else {
                var JsonBasket = _contextAccessor.HttpContext.Request.Cookies["basket"];

                if (JsonBasket is not null)
                {
                    List<BasketVM>? baskets = JsonConvert.DeserializeObject<List<BasketVM>>(JsonBasket);
                    BasketVM basket = baskets.FirstOrDefault(x => x.ProductId == id);
                    if (basket is not null)
                    {
                        baskets.Remove(basket);
                        JsonBasket = JsonConvert.SerializeObject(baskets);
                        _contextAccessor.HttpContext.Response.Cookies.Append("basket", JsonBasket);
                    }
                }
            }
        }
    }
}
