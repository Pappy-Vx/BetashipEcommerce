using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Products.Entities
{
    public sealed class ProductImage : Entity<Guid>
    {
        public string Url { get; private set; }
        public string AltText { get; private set; }
        public int SortOrder { get; private set; }
        public bool IsPrimary { get; private set; }

        private ProductImage(Guid id, string url, string altText, int sortOrder, bool isPrimary)
            : base(id)
        {
            Url = url;
            AltText = altText;
            SortOrder = sortOrder;
            IsPrimary = isPrimary;
        }

        private ProductImage() : base() { }

        public static ProductImage Create(string url, string altText, int sortOrder, bool isPrimary)
        {
            return new ProductImage(Guid.NewGuid(), url, altText, sortOrder, isPrimary);
        }
    }
}
