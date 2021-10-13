using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EntityDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ShopContext context = new ShopContext();

            //await context.DeleteDatabase();  // xóa database: shopdata nếu tồn tại
            //await context.CreateDatabase();  // tạo lại database: shopdata
            //await context.InsertSampleData();
            Product product = await context.FindProduct(1);
                Console.WriteLine(product.Category.Name);
        }
    }
}
