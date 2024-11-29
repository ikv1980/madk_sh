using Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Project.Tools.DbRequest
{
    public class TableQueryManager
    {
        private readonly Db _dbContext;
        private readonly Dictionary<string, List<ColumnDefinition>> _columnsMapping;

        public TableQueryManager()
        {
            _dbContext = DbUtils.db;

            _columnsMapping = new Dictionary<string, List<ColumnDefinition>>
            {
                ["country"] = new List<ColumnDefinition>
                {
                    new ColumnDefinition("CountryName", "Название")
                },
                ["color"] = new List<ColumnDefinition>
                {
                    new ColumnDefinition("ColorName", "Название")
                },
                ["type"] = new List<ColumnDefinition>
                {
                    new ColumnDefinition("TypeName", "Название")
                },
                ["page"] = new List<ColumnDefinition>
                {
                    new ColumnDefinition("PageNumber", "№"),
                    new ColumnDefinition("PageNameEng", "Название_ENG"),
                    new ColumnDefinition("PageNameRus", "Название_RUS"),
                    new ColumnDefinition("PageIcon", "Иконка")
                }
            };
        }

        public bool TryGetQuery(string tableTag, out IQueryable<object> query, out List<ColumnDefinition> columns)
        {
            query = null;
            columns = null;

            switch (tableTag)
            {
                case "country":
                    query = _dbContext.CarsCountries.Select(x => new { x.CountryName });
                    columns = _columnsMapping[tableTag];
                    break;
                case "color":
                    query = _dbContext.CarsColors.Select(x => new { x.ColorName });
                    columns = _columnsMapping[tableTag];
                    break;
                case "type":
                    query = _dbContext.CarsTypes.Select(x => new { x.TypeName });
                    columns = _columnsMapping[tableTag];
                    break;
                case "page":
                    query = _dbContext.SitePages.Select(x => new { x.PageNumber, x.PageNameEng, x.PageNameRus, x.PageIcon });
                    columns = _columnsMapping[tableTag];
                    break;
                default:
                    return false;
            }

            return true;
        }

        public List<ColumnDefinition> GetColumns(string tableTag)
        {
            if (_columnsMapping.TryGetValue(tableTag, out var columns))
                return columns;

            throw new ArgumentException($"Invalid table tag: {tableTag}");
        }
    }

    public class ColumnDefinition
    {
        public string ColumnName { get; }
        public string DisplayName { get; }

        public ColumnDefinition(string columnName, string displayName)
        {
            ColumnName = columnName;
            DisplayName = displayName;
        }
    }
}
