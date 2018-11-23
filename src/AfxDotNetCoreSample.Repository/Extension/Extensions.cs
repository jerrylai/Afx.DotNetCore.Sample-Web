using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;

using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace AfxDotNetCoreSample.Repository
{
    public enum DbLikeType
    {
        Left = 1,
        Right = 2,
        All =  3
    }

    public static class Extensions
    {
        public static string DbLike(this string value, DbLikeType type = DbLikeType.All)
        {
            if (!string.IsNullOrEmpty(value) && !value.Contains("%"))
            {
                switch (type)
                {
                    case DbLikeType.All:
                        value = "%" + value + "%";
                        break;
                    case DbLikeType.Left:
                        value = "%" + value;
                        break;
                    case DbLikeType.Right:
                        value = value + "%";
                        break;
                }
            }

            return value;
        }

        private static Dictionary<string, string> GetOrderbyDic(string orderby, System.Reflection.PropertyInfo[] propertyInfos)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(orderby))
            {
                var propertyrderarr = orderby.Split(',');
                foreach (var s in propertyrderarr)
                {
                    var propertyorder = s.Trim();
                    if (!string.IsNullOrEmpty(propertyorder))
                    {
                        var arr = propertyorder.Split(' ');
                        if (arr.Length > 2)
                        {
                            throw new ArgumentException($"orderby({propertyorder}) is error!", nameof(orderby));
                        }
                        var col = arr[0].Trim();
                        var p = propertyInfos.Where(q => string.Equals(q.Name, col, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (p == null) throw new ArgumentException($"orderby({col}) is error!", nameof(orderby));
                        string sort = null;
                        if (arr.Length == 2)
                        {
                            var st = arr[1].Trim();
                            if (string.Equals(st, "asc", StringComparison.OrdinalIgnoreCase))
                            {
                                sort = "ascending";
                            }
                            else if (string.Equals(st, "desc", StringComparison.OrdinalIgnoreCase))
                            {
                                sort = "descending";
                            }
                            else
                            {
                                if (p == null) throw new ArgumentException($"orderby({propertyorder}) is error!", nameof(orderby));
                            }
                        }

                        dic[p.Name] = sort;
                    }
                }
            }

            return dic;
        }

        private static string GetOrderby<T>(string orderby, string defaultOrderby)
        {
            var propertyInfos = typeof(T).GetProperties();
            if(string.IsNullOrEmpty(defaultOrderby)) throw new ArgumentNullException(nameof(defaultOrderby));
            var defaultOrderbyDic = GetOrderbyDic(defaultOrderby, propertyInfos);
            var orderbyDic = GetOrderbyDic(orderby, propertyInfos);

            StringBuilder result = new StringBuilder();
            foreach(var kv in orderbyDic)
            {
                if (string.IsNullOrEmpty(kv.Value)) result.Append($"{kv.Key},");
                else result.Append($"{kv.Key} {kv.Value},");
            }

            foreach(var kv in defaultOrderbyDic)
            {
                if (!orderbyDic.ContainsKey(kv.Key))
                {
                    if (string.IsNullOrEmpty(kv.Value)) result.Append($"{kv.Key},");
                    else result.Append($"{kv.Key} {kv.Value},");
                }
            }

            if(result.Length > 0) result.Remove(result.Length - 1, 1);
            
            return result.ToString();
        }

        public static PageDataOutputDto<T> ToPage<T>(this IQueryable<T> query, PageDataInputDto param, string defaultOrderby = "Id")
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (param.PageIndex < 1) throw new ArgumentException(nameof(param.PageIndex));
            if (param.PageSize < 1) throw new ArgumentException(nameof(param.PageSize));

            var orderby = GetOrderby<T>(param.Orderby, defaultOrderby);
            var vm = new PageDataOutputDto<T>();
            vm.TotalCount = query.Count();
            query = query.OrderBy(orderby);
            if (param.PageIndex > 1) query = query.Skip((param.PageIndex - 1) * param.PageSize);
            vm.Data = query.Take(param.PageSize).ToList();

            return vm;
        }
    }
}
