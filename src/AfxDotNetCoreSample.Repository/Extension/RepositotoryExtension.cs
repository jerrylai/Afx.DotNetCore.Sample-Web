using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.Repository
{
    public static class RepositotoryExtension
    {
        public static PageDataOutputDto<T> ToPageCount<T>(this IQueryable<T> query)
        {
            var vm = new PageDataOutputDto<T>();
            vm.TotalCount = query.Count();
            return vm;
        }

        public static void ToPageList<T>(this IQueryable<T> query, PageDataOutputDto<T> vm, PageDataInputDto pageParam)
        {
            pageParam.PageIndex = pageParam.PageIndex < 1 ? 1 : pageParam.PageIndex;
            pageParam.PageSize = pageParam.PageSize < 1 ? 10 : pageParam.PageSize;
            if (pageParam.PageIndex > 1) query = query.Skip((pageParam.PageIndex - 1) * pageParam.PageSize);
            vm.Data = query.Take(pageParam.PageSize).ToList();
        }
    }
}
