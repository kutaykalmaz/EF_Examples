using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace _NET
{
    public class ShopContext : DbContext
    {

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=shop.db");
            /* 
            optionsBuilder.LogTo(Console.WriteLine);
            this code logs what you do in database to console
            */
        }

    }

    public class Product
    {
        /*
        If you want to define primary key of table, 
        you must use property name like 'Id' or '<type_name>Id'.
        Or if you want to define custom name primary key, use [Key] before property.
        Do not forget to add 'System.ComponentModel.DataAnnotations' before specifying this data field.
        Primary Key(Id, <type_name>Id)
        */
        public int ProductId { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {

            
        }

        static void deleteProductwithStateChange(int id)
        {
            using (var db = new ShopContext())
            {
                // This function do not use select query.

                var p = new Product() { ProductId = 5 };

                // db.Products.Remove(p);
                db.Entry(p).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        static void deleteProduct(int id)
        {
            using (var db = new ShopContext())
            {
                var p = db.Products.FirstOrDefault(i => i.ProductId == id);

                if (p != null)
                {
                    db.Products.Remove(p);
                    Console.WriteLine("The data has been removed.");
                    db.SaveChanges();
                }
            }
        }

        static void updateProductwithAttach()
        {
            using (var db = new ShopContext())
            {
                var entity = new Product() { ProductId = 1 };

                // Attach function starts change tracking.
                db.Products.Attach(entity);

                entity.Price = 6500;

                Console.WriteLine("The data has been updated.");

                db.SaveChanges();
            }
        }

        static void updateProduct()
        {
            /* 
            This function uses Select sql query. By this way also uses change tracking.
            If you don't want to use change tracking for example you just want to read a data,
            you should use AsNoTracking function. In this way, if a change is made, it will not be
            reflected in the database.
            */
            using (var db = new ShopContext())
            {
                var p = db.Products.Where(i => i.ProductId == 1).FirstOrDefault();
                if (p != null)
                {
                    p.Price *= 1.2m;

                    db.SaveChanges();
                    Console.WriteLine("The data has been updated.");
                }
            }
        }

        static void getProductByName(string name)
        {
            using (var context = new ShopContext())
            {
                /* 
                If there is a data that matches the value given into the function,
                it returns that data, otherwise it returns null by 'FirstOrDefault' function.
                */
                var products = context
                                .Products
                                .Where(p => p.Name.ToLower().Contains(name.ToLower()))
                                .Select(product => new
                                {
                                    product.Name,
                                    product.Price
                                })
                                .ToList();

                foreach (var product in products)
                {
                    Console.WriteLine($"Name : {product.Name} Price : {product.Price}");
                }
            }

        }

        static void getProductById(int id)
        {
            using (var context = new ShopContext())
            {
                /* 
                If there is a data that matches the value given into the function,
                it returns that data, otherwise it returns null by 'FirstOrDefault' function.
                */
                var result = context
                                .Products
                                .Where(p => p.ProductId == id)
                                .Select(product => new
                                {
                                    product.Name,
                                    product.Price
                                })
                                .FirstOrDefault();

                Console.WriteLine($"Name : {result.Name} Price : {result.Price}");
            }

        }

        static void getAllProducts()
        {
            using (var context = new ShopContext())
            {
                var products = context
                .Products
                .Select(product => new
                {
                    /*
                    Specification about table's column.
                    We are just going to get 'Name' and 'Price' column from table with that situation.
                    Otherwise (without Select) this code will also get Id column.
                    */
                    product.Name,
                    product.Price
                })
                .ToList();

                foreach (var product in products)
                {
                    Console.WriteLine($"Name : {product.Name} Price : {product.Price}");
                }
            }
        }

        static void addProducts()
        {
            using (var db = new ShopContext())
            {
                var products = new List<Product>()
                {
                    new Product { Name = "Iphone XS", Price = 6000 },
                    new Product { Name = "Iphone XS Max", Price = 7000 },
                    new Product { Name = "Iphone 11", Price = 8000 },
                    new Product { Name = "Iphone 11 Pro", Price = 5000 }
                };
                // Addrange() allows you to add multiple rows to the table.
                db.Products.AddRange(products);

                db.SaveChanges();

                Console.WriteLine("The datasets you entered has been succesfully added.");
            }
        }

        static void addProduct()
        {
            using (var db = new ShopContext())
            {
                var product = new Product { Name = "Iphone 11 Pro Max", Price = 10000 };

                db.Products.Add(product);

                db.SaveChanges();

                Console.WriteLine("The data you entered has been succesfully added.");
            }
        }
    }
}
