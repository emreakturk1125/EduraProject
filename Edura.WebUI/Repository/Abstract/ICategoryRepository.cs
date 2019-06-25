using Edura.WebUI.Entity;
using Edura.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Edura.WebUI.Repository.Abstract
{
    //Category'ye özel metod imzaları burada oluşturulur
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Category GetByName(string name);
        IEnumerable<CategoryModel> GetAllWithProductCount();
        void RemoveFromCategory(int ProductId,int CategoryId);
    }
}
