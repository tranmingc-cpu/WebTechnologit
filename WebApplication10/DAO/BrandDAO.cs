using System.Collections.Generic;
using System.Linq;
using WebApplication10.Models;

namespace WebApplication10.DAO
{
    public class BrandDao
    {
        private readonly TechStoreDBEntities _db;

        public BrandDao(TechStoreDBEntities db)
        {
            _db = db;
        }

     
        public List<Brands> GetAll()
        {
            return _db.Brands
                      .OrderBy(b => b.BrandName)
                      .ToList();
        }

     
        public Brands GetById(int id)
        {
            return _db.Brands.FirstOrDefault(b => b.BrandId == id);
        }

        public void Add(Brands brand)
        {
            _db.Brands.Add(brand);
            _db.SaveChanges();
        }

        public void Update(Brands brand)
        {
            var existing = GetById(brand.BrandId);
            if (existing == null) return;

            existing.BrandName = brand.BrandName;
            existing.Country = brand.Country;

            _db.SaveChanges();
        }

        public bool Delete(int brandId, out string message)
        {
            var brand = GetById(brandId);
            if (brand == null)
            {
                message = "Thương hiệu không tồn tại.";
                return false;
            }

            //  nếu Brand đang được dùng bởi Product
            bool isUsed = _db.Products.Any(p => p.BrandId == brandId);
            if (isUsed)
            {
                message = "Không thể xóa thương hiệu đang được sử dụng bởi sản phẩm.";
                return false;
            }

            _db.Brands.Remove(brand);
            _db.SaveChanges();

            message = "Xóa thương hiệu thành công.";
            return true;
        }

    
        public bool ExistsByName(string brandName, int? excludeId = null)
        {
            return _db.Brands.Any(b =>
                b.BrandName == brandName &&
                (!excludeId.HasValue || b.BrandId != excludeId.Value)
            );
        }
    }
}
