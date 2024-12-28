using System.Linq.Expressions;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using MessageBox = System.Windows.MessageBox;
using LinqExpression = System.Linq.Expressions.Expression;

namespace Project.Tools
{
    internal static class DbUtils
    {
        public static Db db;

        static DbUtils()
        {
            try
            {
                db = new Db();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка подключения к БД\n{e}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Поиск в таблице
        public static List<T> GetSearchingValues<T>(string searchText) where T : class
        {
            var parameter = LinqExpression.Parameter(typeof(T), "e");
            var stringProperties = typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string) && !p.GetGetMethod().IsStatic);

            if (!stringProperties.Any())
                return new List<T>();

            LinqExpression orExpression = null;

            foreach (var prop in stringProperties)
            {
                var propertyExpression = LinqExpression.Property(parameter, prop);
                var likeExpression = LinqExpression.Call(
                    propertyExpression,
                    nameof(string.Contains),
                    Type.EmptyTypes,
                    LinqExpression.Constant(searchText, typeof(string))
                );

                orExpression = orExpression == null
                    ? likeExpression
                    : LinqExpression.OrElse(orExpression, likeExpression);
            }

            var lambda = LinqExpression.Lambda<Func<T, bool>>(orExpression, parameter);
            return db.Set<T>().Where(lambda).ToList();
        }
        
        // Подсчет всех записей в таблице
        public static int GetTableCount<TTable>() where TTable : class
        {
            
            using (var context = new Db())
            {
                return context.Set<TTable>().Count();
            }
        }
        
        // Получение данных из БД
        public static List<TTable> GetTablePagedValuesWithIncludes<TTable>(int page, int pageSize, string sortPropertyName = null) where TTable : class
        {
            using (var context = new Db())
            {
                var query = context.Set<TTable>().AsQueryable();

                // Получаем навигационные свойства
                var entityType = context.Model.FindEntityType(typeof(TTable));
                var navigationProperties = entityType.GetNavigations()
                    .Select(n => n.Name)
                    .ToList();

                // Применяем Include для загрузки всех связанных сущностей
                foreach (var property in navigationProperties)
                {
                    query = query.Include(property);
                }

                // Применяем сортировку, если указано поле
                if (!string.IsNullOrEmpty(sortPropertyName))
                {
                    var sortProperty = typeof(TTable).GetProperty(sortPropertyName);
                    if (sortProperty != null)
                    {
                        query = query.OrderBy(e => EF.Property<object>(e, sortPropertyName));
                    }
                }

                return query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
        }
    }
}