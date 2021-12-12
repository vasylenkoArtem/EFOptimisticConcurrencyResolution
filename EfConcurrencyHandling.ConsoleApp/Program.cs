using System;
using System.Linq;
using EfConcurrencyHandling.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EfConcurrencyHandling.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            UpdateRowWithNewDataDataViaPureSql("Name1");

            using (var context = new StoreContext())
            {
                var product = context.Products.FirstOrDefault(p => p.Id == 1);

                product.Name = "Name2";

                // The problem after query the product and EF know that Name is "Name1"
                // But in the next line we updates Name with another value that EF doesn't expect as a current Db value, 
                // EF executes query when try to update Name like this: 
                // update Product set Name = 'Name2' where Id = 1 and Name = 'Name1';
                // Next EF see that affectedRows if equal to 0
                // But if use RowVersion concurrency pattern, EF will throw concurrency exception for the any changes in the row
                // even the same values, because RowVersion will be updated via SQL automatically
                UpdateRowWithNewDataDataViaPureSql("Name5");

                var affectedRows = context.SaveChanges();

                Console.WriteLine($"{affectedRows} rows affected");
            }

            Console.ReadKey();
        }

        public static void UpdateRowWithNewDataDataViaPureSql(string name)
        {
            string connectionString = StoreContext.ConnectionString;

            var sql = $"update Products set Name = '{name}'";

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
            }
        }
    }
}