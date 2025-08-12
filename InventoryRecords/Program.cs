using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public interface IInventoryEntity
{
    int Id { get; }
}

public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
        Console.WriteLine($"Item added: {item}");
    }

    public List<T> GetAll() => new List<T>(_log);

    public void SaveToFile()
    {
        try
        {
            string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            using (var writer = new StreamWriter(_filePath))
            {
                writer.Write(json);
            }
            Console.WriteLine("Data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("File not found. No data loaded.");
                return;
            }

            using (var reader = new StreamReader(_filePath))
            {
                string json = reader.ReadToEnd();
                var items = JsonSerializer.Deserialize<List<T>>(json);
                _log = items ?? new List<T>();
            }
            Console.WriteLine("Data loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
        }
    }
}

public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1111, "Laptop", 5, DateTime.Now));
        _logger.Add(new InventoryItem(1112, "Television", 10, DateTime.Now));
        _logger.Add(new InventoryItem(1103, "PS5 Console", 15, DateTime.Now));
        _logger.Add(new InventoryItem(1114, "Monitor", 19, DateTime.Now));
        _logger.Add(new InventoryItem(1106, "Printer", 3, DateTime.Now));
    }

    public void SaveData() => _logger.SaveToFile();

    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
        }
    }
}

class Program
{
    static void Main()
    {
        string filePath = "inventory.json";

        var app = new InventoryApp(filePath);
        app.SeedSampleData();
        app.SaveData();

        app = new InventoryApp(filePath);
        app.LoadData();
        app.PrintAllItems();
    }
}

