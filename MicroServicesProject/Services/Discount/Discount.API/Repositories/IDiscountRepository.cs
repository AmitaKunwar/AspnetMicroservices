using Discount.API.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API.Repositories
{
   public interface IDiscountRepository
    {
        Task<Coupon> GetDiscount(string productname);
        Task<bool> CreateDiscount(Coupon coupon );
        Task<bool> UpdateDiscount(Coupon coupon);

        Task<bool> DeleteDiscount(string productname);
    }
}
