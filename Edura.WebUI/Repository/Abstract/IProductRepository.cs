using Edura.WebUI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Edura.WebUI.Repository.Abstract
{
    //Product'a özel metod imzaları burada oluşturulur
    public interface IProductRepository : IGenericRepository<Product>
    {
        List<Product> GetTop5Products();
    }
}
