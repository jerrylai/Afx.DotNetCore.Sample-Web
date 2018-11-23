using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.IService;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.Common;
using Afx.Utils;

namespace AfxDotNetCoreSample.Service
{
    public class RegionService : BaseService, IRegionService
    {
        protected virtual IRegionRepository regionRepository => this.GetRepository<IRegionRepository>();

        protected virtual IRegionLevelRepository regionLevelRepository => this.GetRepository<IRegionLevelRepository>();

        public virtual bool Add(RegionDto vm)
        {
            bool result = false;
            if (vm == null) throw new ApiParamNullException(nameof(vm));
            if (string.IsNullOrEmpty(vm.Name)) throw new ApiParamNullException(nameof(vm.Name));
            if (!string.IsNullOrEmpty(vm.ParentId))
            {
                var pm = this.regionRepository.Get(vm.ParentId);
                if (pm == null) throw new ApiParamException(nameof(vm.ParentId));
                vm.Level = pm.Level + 1;
            }
            else
            {
                vm.ParentId = null;
                vm.Level = 1;
            }
            var count = this.regionRepository.Add(vm);
            result = true;

            return result;
        }

        public virtual bool Delete(string id)
        {
            bool result = false;
            if (string.IsNullOrEmpty(id)) throw new ApiParamNullException(nameof(id));
            var child = this.regionRepository.GetChild(id);
            if (child.Count > 0) throw new ApiException("请先删除子节点！");
            var count = this.regionRepository.Delete(id);
            result = true;

            return result;
        }

        public virtual RegionDto Get(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ApiParamNullException(nameof(id));
            var vm = this.regionRepository.Get(id);

            return vm;
        }

        public virtual List<RegionDto> GetChildList(string parentId)
        {
            var idlist = this.regionRepository.GetChild(parentId);
            var list = new List<RegionDto>(idlist.Count);
            foreach(var id in idlist)
            {
                var vm = this.regionRepository.Get(id);
                list.Add(vm);
            }

            return list;
        }

        public virtual List<string> GetChildId(string parentId)
        {
            var idlist = this.regionRepository.GetChild(parentId);

            return idlist;
        }

        public virtual List<RegionLevelDto> GetLevelList(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ApiParamNullException(nameof(id));
            var list = this.regionLevelRepository.GetList(id);

            return list;
        }

        public virtual List<RegionDto> GetParentList(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ApiParamNullException(nameof(id));
            var levellist = this.regionLevelRepository.GetList(id);
            var list = new List<RegionDto>(levellist.Count);
            foreach(var level in levellist)
            {
                var vm = this.regionRepository.Get(level.ParentId);
                list.Add(vm);
            }

            return list;
        }

        public virtual bool Update(RegionDto vm)
        {
            if(vm == null) throw new ApiParamNullException(nameof(vm));
            if (string.IsNullOrEmpty(vm.Id)) throw new ApiParamNullException(nameof(vm.Id));
            if (string.IsNullOrEmpty(vm.Name)) throw new ApiParamNullException(nameof(vm.Name));
            this.regionRepository.Update(vm);

            return true;
        }

        public virtual string GetFullName(string id, string separator = null)
        {
            string fullname = null;
            if (!string.IsNullOrEmpty(id))
            {
                var list = this.GetParentList(id);
                StringBuilder str = new StringBuilder();
                foreach(var r in list.OrderBy(q=>q.Level))
                {
                    str.Append(r.Name);
                    if (!string.IsNullOrWhiteSpace(separator)) str.Append(separator);
                }

                if (str.Length > 0 && !string.IsNullOrWhiteSpace(separator)) str.Remove(str.Length - separator.Length, separator.Length);

                fullname = str.ToString();
            }

            return fullname;
        }
    }
}
