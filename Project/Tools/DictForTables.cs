using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Tools
{
public class DictForTabl
{
    private readonly string _tableName;         // тег запроса
    private readonly Db _context;               // контекст базы данных

    private IQueryable<object> _cachedQuery;

    public List<ColumnDefinition> Columns { get; private set; }

    public DictForTabl(string tableName)
    {
        _tableName = tableName;
        _context = new Db();

        Columns = tableName switch
        {
            "CarsCountry" => new List<ColumnDefinition>
            {
                new ColumnDefinition("CountryId", "ID", false),
                new ColumnDefinition("CountryName", "Название"),
                new ColumnDefinition("CountryDelete", "Удалено", false)
            },
            "CarsColor" => new List<ColumnDefinition>
            {
                new ColumnDefinition("ColorId", "ID", false),
                new ColumnDefinition("ColorName", "Название"),
                new ColumnDefinition("ColorDelete", "Удалено", false)
            },
            _ => new List<ColumnDefinition>()
        };
    }

    public IEnumerable<object> GetPageData(int page, int pageSize, string searchQuery = "")
    {
        Console.WriteLine($"-------page: {page},-------pageSize: {pageSize},-------searchQuery: [{searchQuery}]");

        if (_cachedQuery == null)  // Проверка, кэшированы ли данные
        {
            _cachedQuery = _tableName switch
            {
                "CarsCountry" => _context.CarsCountries
                    .Select(x => new { x.CountryId, x.CountryName, CuuntryDelete = x.CountryDelete })
                    .OrderBy(x => x.CountryId), // Добавлена сортировка
                "CarsColor" => _context.CarsColors
                    .Select(x => new { x.ColorId, x.ColorName, x.ColorDelete })
                    .OrderBy(x => x.ColorId), // Добавлена сортировка
                _ => Enumerable.Empty<object>().AsQueryable()
            };
        }

        IQueryable<object> query = _cachedQuery;

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(x =>
                x.GetType().GetProperties().Any(p =>
                    p.GetValue(x) != null && 
                    p.GetValue(x).ToString().Contains(searchQuery, StringComparison.OrdinalIgnoreCase)));
        }

        return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }

    public void AddRecord()
    {
        if (_tableName == "CarsCountry")
        {
            _context.CarsCountries.Add(new CarsCountry { CountryName = "Новая страна", CountryDelete = false });
        }
        else if (_tableName == "CarsColor")
        {
            _context.CarsColors.Add(new CarsColor { ColorName = "Новый цвет", ColorDelete = false });
        }
        _context.SaveChanges();
        _cachedQuery = null;  // Очистка кэша после добавления новой записи
    }

    public bool HasMoreData(int currentPage, int pageSize)
    {
        IQueryable<object> query = _tableName switch
        {
            "CarsCountry" => _context.CarsCountries.OrderBy(x => x.CountryId),
            "CarsColor" => _context.CarsColors.OrderBy(x => x.ColorId),
            _ => Enumerable.Empty<object>().AsQueryable()
        };

        return query.Skip(currentPage * pageSize).Any();
    }
    
    
    
    
    
    // ------------------------------------ ASYNC ------------------------------------
    public async Task<IEnumerable<object>> GetPageDataAsync(int page, int pageSize, string searchQuery = "")
    {
        IQueryable<object> query = _tableName switch
        {
            "CarsCountry" => _context.CarsCountries.Select(x => new
            {
                x.CountryId,
                x.CountryName,
                x.CountryDelete
            }).AsQueryable(),
            "CarsColor" => _context.CarsColors.Select(x => new
            {
                x.ColorId,
                x.ColorName,
                x.ColorDelete
            }).AsQueryable(),
            _ => Enumerable.Empty<object>().AsQueryable()
        };

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(x =>
                x.GetType().GetProperties().Any(p =>
                    p.GetValue(x) != null &&
                    p.GetValue(x).ToString().Contains(searchQuery, StringComparison.OrdinalIgnoreCase)));
        }

        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }
    
    public async Task<bool> HasMoreDataAsync(int currentPage, int pageSize)
    {
        var query = _tableName switch
        {
            "CarsCountry" => _context.CarsCountries.AsQueryable(),
            "CarsColor" => _context.CarsColors.AsQueryable(),
            _ => Enumerable.Empty<object>().AsQueryable()
        };

        return await query.Skip(currentPage * pageSize).AnyAsync();
    }    
}















    // ------------------ Класс для получения настроек полей
    public class ColumnDefinition
    {
        public string ColumnName { get; }
        public string DisplayName { get; }
        public bool IsVisible { get; }

        public ColumnDefinition(string columnName, string displayName, bool isVisible = true)
        {
            ColumnName = columnName;
            DisplayName = displayName;
            IsVisible = isVisible;
        }
    }
}





