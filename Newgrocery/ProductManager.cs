using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductManagerApp
{
    public static class ProductManager
    {
        // Specify the directory and JSON file location
        public static string FilePath = Path.Combine(
            @"C:\FAYLES\JsonDIR", // The desired directory path
            "products.json"); // JSON file name

        // Write products to JSON file
        public static async Task WriteJsonToFileAsync(List<Product> products)
        {
            // Ensure the directory exists
            string directoryPath = Path.GetDirectoryName(FilePath);
            Directory.CreateDirectory(directoryPath);

            string json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(FilePath, json);
        }

        // Read products from JSON file
        public static async Task<List<Product>> ReadJsonFromFileAsync()
        {
            if (!File.Exists(FilePath))
            {
                return new List<Product>();
            }

            string jsonString = await File.ReadAllTextAsync(FilePath);
            return JsonSerializer.Deserialize<List<Product>>(jsonString) ?? new List<Product>();
        }
    }
}
