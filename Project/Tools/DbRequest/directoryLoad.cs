using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Models;

namespace Project.Tools.DbRequest
{
    // передача запроса в виде
    // currentDict = new load(tag, new { paramentrOne = 1, paramentrTwo = "Home", paramentrThree = true});
    public class directoryLoad
    {
        private readonly IQueryable<object> _cachedQuery;
        private readonly List<ColumnDefinition> _columns;

        public directoryLoad(string tableTag, dynamic parameters = null)
        {
            var queryManager = new directoryTableManager(parameters);

            if (!queryManager.TryGetQuery(tableTag, out var query, out var columns))
                throw new ArgumentException($"Invalid table tag: {tableTag}");

            _cachedQuery = query;
            _columns = columns;
        }

        public List<ColumnDefinition> GetColumns() => _columns;

        public IEnumerable<object> GetPageData(int page, int pageSize, string searchQuery = "")
        {
            var query = ApplySearchFilter(_cachedQuery, searchQuery);
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public bool HasMoreData(int currentPage, int pageSize)
        {
            return _cachedQuery.Skip(currentPage * pageSize).Any();
        }

        private static IQueryable<object> ApplySearchFilter(IQueryable<object> query, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return query;

            return query.Where(x =>
                x.GetType().GetProperties().Any(p =>
                    p.GetValue(x) != null &&
                    p.GetValue(x).ToString().Contains(searchQuery, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
