using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Edura.WebUI.Entity
{
    public class ProductAttribute
    {
        [Key]
        public int ProductAttributedId { get; set; }

        public string Attribute { get; set; }
        public string Value { get; set; }

        public int ProductId { get; set; }
        public Product Product  { get; set; }
    }
}
