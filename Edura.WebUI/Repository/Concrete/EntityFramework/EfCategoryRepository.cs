using Edura.WebUI.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edura.WebUI.Entity;
using System.Linq.Expressions;
using Edura.WebUI.Models;
using Microsoft.EntityFrameworkCore;

namespace Edura.WebUI.Repository.Concrete.EntityFramework
{
    public class EfCategoryRepository : EfGenericRepository<Category>, ICategoryRepository
    {

        public EfCategoryRepository(EduraContext context) : base(context)
        {

        }

        public EduraContext EduraContext
        {
            get { return context as EduraContext; }
        }

        public IEnumerable<CategoryModel> GetAllWithProductCount()
        {
            return GetAll().Select(x => new CategoryModel()
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                Count = x.ProductCategories.Count()
            });
        }

        public Category GetByName(string name)
        {
            return EduraContext.Categories.Where(x => x.CategoryName == name).FirstOrDefault();
        }

        public void RemoveFromCategory(int ProductId, int CategoryId)
        {
            var cmd = $"delete from ProductCategory where ProductId={ProductId} and CategoryId={CategoryId}";
            context.Database.ExecuteSqlCommand(cmd);
        }
    }
}
