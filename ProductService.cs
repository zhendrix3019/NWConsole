using System;
using System.Linq;
using NLog;
using NWConsole.Model;
using Microsoft.EntityFrameworkCore;


public class ProductService
{
    private readonly NWContext _context;
    private readonly Logger _logger;

    public ProductService(NWContext context, Logger logger)
    {
        _context = context;
        _logger = logger;
    }

    public void AddNewProduct()
    {
        Console.WriteLine("Enter Product Name:");
        string productName = Console.ReadLine();
        Console.WriteLine("Enter Unit Price:");
        decimal unitPrice = decimal.Parse(Console.ReadLine()); // Add validation as needed
        Console.WriteLine("Is the product discontinued? (yes/no):");
        bool isDiscontinued = Console.ReadLine().ToLower() == "yes";

        var product = new Product { ProductName = productName, UnitPrice = unitPrice, Discontinued = isDiscontinued };
        _context.Products.Add(product);
        _context.SaveChanges();
        _logger.Info($"New product added: {productName}");
        Console.WriteLine("Product added successfully.");
    }

    public void EditProduct()
    {
        Console.WriteLine("Enter Product ID to edit:");
        int productId = int.Parse(Console.ReadLine()); // Add validation as needed
        var product = _context.Products.Find(productId);

        if (product == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        Console.WriteLine("Enter new Product Name (leave blank to keep current):");
        string newName = Console.ReadLine();
        if (!string.IsNullOrEmpty(newName))
        {
            product.ProductName = newName;
        }

        Console.WriteLine("Enter new Unit Price (leave blank to keep current):");
        string newPrice = Console.ReadLine();
        if (!string.IsNullOrEmpty(newPrice))
        {
            product.UnitPrice = decimal.Parse(newPrice);
        }

        _context.SaveChanges();
        _logger.Info($"Product updated: {product.ProductName}");
        Console.WriteLine("Product updated successfully.");
    }

    public void DisplayProducts()
    {
        Console.WriteLine("Choose an option:");
        Console.WriteLine("1: All Products");
        Console.WriteLine("2: Active Products");
        Console.WriteLine("3: Discontinued Products");
        var option = Console.ReadLine();

        IQueryable<Product> query = option switch
        {
            "1" => _context.Products,
            "2" => _context.Products.Where(p => !p.Discontinued),
            "3" => _context.Products.Where(p => p.Discontinued),
            _ => _context.Products
        };

        foreach (var product in query)
        {
            Console.WriteLine($"Product ID: {product.ProductId}, Name: {product.ProductName}, Discontinued: {product.Discontinued}");
        }
    }

    public void DisplaySpecificProduct()
    {
        Console.WriteLine("Enter Product ID to display:");
        int productId = int.Parse(Console.ReadLine()); // Add validation as needed
        var product = _context.Products.Find(productId);

        if (product == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        Console.WriteLine($"ID: {product.ProductId}, Name: {product.ProductName}, Price: {product.UnitPrice}, Discontinued: {product.Discontinued}");
    }
    public void DeleteProduct()
{
    try
    {
        Console.WriteLine("Enter Product ID to delete:");
        int productId = int.Parse(Console.ReadLine());
        var product = _context.Products.Include(p => p.OrderDetails).FirstOrDefault(p => p.ProductId == productId);
        if (product == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        if (product.OrderDetails.Any())
        {
            Console.WriteLine("Cannot delete product as it has related order details.");
        }
        else
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
            Console.WriteLine("Product deleted.");
        }
    }
    catch (Exception ex)
    {
        _logger.Error(ex, "Error deleting product.");
        Console.WriteLine("Error occurred while deleting product.");
    }
}

public void DeleteCategory()
{
    try
    {
        Console.WriteLine("Enter Category ID to delete:");
        int categoryId = int.Parse(Console.ReadLine());
        var category = _context.Categories.Include(c => c.Products).FirstOrDefault(c => c.CategoryId == categoryId);
        if (category == null)
        {
            Console.WriteLine("Category not found.");
            return;
        }

        if (category.Products.Any())
        {
            Console.WriteLine("Cannot delete category as it has related products.");
        }
        else
        {
            _context.Categories.Remove(category);
            _context.SaveChanges();
            Console.WriteLine("Category deleted.");
        }
    }
    catch (Exception ex)
    {
        _logger.Error(ex, "Error deleting category.");
        Console.WriteLine("Error occurred while deleting category.");
    }
}
}
