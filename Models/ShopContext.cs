using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityDemo
{
    public class ShopContext : DbContext
    {

        // Chuỗi kết nối tới CSDL (MS SQL Server)
        protected string connect_str = @"
                Data Source=ADMIN\SQLEXPRESS;
                Initial Catalog=ShopDemo;
                Trusted_Connection=True;";

        public DbSet<Product> products { set; get; }      // bảng Products
        public DbSet<Category> categories { set; get; }   // bảng Category

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Tạo ILoggerFactory
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            optionsBuilder.UseSqlServer(connect_str)            // thiết lập làm việc với SqlServer
                          .UseLoggerFactory(loggerFactory);     // thiết lập logging
            optionsBuilder.UseLazyLoadingProxies();

        }

        // Tạo database
        public async Task CreateDatabase()
        {
            String databasename = Database.GetDbConnection().Database;

            Console.WriteLine("Tạo " + databasename);
            bool result = await Database.EnsureCreatedAsync();
            string resultstring = result ? "tạo  thành  công" : "đã có trước đó";
            Console.WriteLine($"CSDL {databasename} : {resultstring}");
        }

        // Xóa Database
        public async Task DeleteDatabase()
        {
            String databasename = Database.GetDbConnection().Database;
            Console.Write($"Có chắc chắn xóa {databasename} (y) ? ");
            string input = Console.ReadLine();

            // // Hỏi lại cho chắc
            if (input.ToLower() == "y")
            {
                bool deleted = await Database.EnsureDeletedAsync();
                string deletionInfo = deleted ? "đã xóa" : "không xóa được";
                Console.WriteLine($"{databasename} {deletionInfo}");
            }
        }
        // Chèn dữ liệu mẫu
        public async Task InsertSampleData()
        {
            // Thêm 2 danh mục vào Category
            var cate1 = new Category() { Name = "Cate1", Description = "Description1" };
            var cate2 = new Category() { Name = "Cate2", Description = "Description2" };
            await AddRangeAsync(cate1, cate2);
            await SaveChangesAsync();

            // Thêm 5 sản phẩm vào Products
            await AddRangeAsync(
                new Product() { Name = "Sản phẩm 1", Price = 12, Category = cate2 },
                new Product() { Name = "Sản phẩm 2", Price = 11, Category = cate2 },
                new Product() { Name = "Sản phẩm 3", Price = 33, Category = cate2 },
                new Product() { Name = "Sản phẩm 4(1)", Price = 323, Category = cate1 },
                new Product() { Name = "Sản phẩm 5(1)", Price = 333, Category = cate1 }

            );
            await SaveChangesAsync();
            // Các sản phầm chèn vào
            foreach (var item in products)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append($"ID: {item.ProductId}");
                stringBuilder.Append($"tên: {item.Name}");
                stringBuilder.Append($"Danh mục {item.CategoryId}({item.Category.Name})");
                Console.WriteLine(stringBuilder);
            }

            // ID: 1tên: Sản phẩm 2Danh mục 2(Cate2)
            // ID: 2tên: Sản phẩm 1Danh mục 2(Cate2)
            // ID: 3tên: Sản phẩm 3Danh mục 2(Cate2)
            // ID: 4tên: Sản phẩm 4(1)Danh mục 1(Cate1)
            // ID: 5tên: Sản phẩm 5(1)Danh mục 1(Cate1)

        }
        // Truy vấn lấy về Category theo ID
        public async Task<Category> FindCategory(int id)
        {

            var cate = await (from c in categories where c.CategoryId == id select c).FirstOrDefaultAsync();
            await Entry(cate)                     // lấy DbEntityEntry liên quan đến p
                   .Collection(cc => cc.products)  // lấy thuộc tính tập hợp, danh sách các sản phẩm
                   .LoadAsync();                   // nạp thuộc tính từ DB
            return cate;
        }
        public async Task<Product> FindProduct(int id)
        {
            var p = await (from c in products where c.ProductId == id select c).FirstOrDefaultAsync();
            return p;
        }
    }
}