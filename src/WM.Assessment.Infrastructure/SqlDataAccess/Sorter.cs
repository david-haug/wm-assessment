using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WM.Assessment.Application;

namespace WM.Assessment.Infrastructure.SqlDataAccess
{
    public class Sorter<T>
    {
        public IEnumerable<T> Sort(IEnumerable<T> items, SortItem[] sortItems)
        {
            if (sortItems == null || !sortItems.Any())
                return items;

            var propertyInfo = typeof(T).GetProperty(sortItems[0].Name,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var sorted = sortItems[0].IsDescending
                ? items.OrderByDescending(x => propertyInfo.GetValue(x, null))
                : items.OrderBy(x => propertyInfo.GetValue(x, null));

            for (var i = 1; i < sortItems.Length; i++)
                sorted = SortMore(sorted, sortItems[i]);

            return sorted;
        }

        private IOrderedEnumerable<T> SortMore(IOrderedEnumerable<T> items, SortItem sort)
        {
            var propertyInfo = typeof(T).GetProperty(sort.Name,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var sorted = sort.IsDescending
                ? items.ThenByDescending(x => propertyInfo.GetValue(x, null))
                : items.ThenBy(x => propertyInfo.GetValue(x, null));
            return sorted;
        }
    }
}