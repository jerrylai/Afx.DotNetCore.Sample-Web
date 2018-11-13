using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.Models;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.IRepository;

namespace AfxDotNetCoreSample.Repository
{
    /// <summary>
    /// 系统相关存储实现
    /// </summary>
    public partial class SystemRepository : BaseRepository, ISystemRepository
    {
        /// <summary>
        /// 更新数据库结构
        /// </summary>
        public virtual void InitDb()
        {
            InitDatabase.InitDb(this.GetContext());
        }

        /// <summary>
        /// 初始化系统数据
        /// </summary>
        /// <returns></returns>
        public virtual void InitData()
        {
            using (var db = this.GetContext())
            {
                this.InitRole(db);
                this.InitUser(db);
                this.InitRegion(db);
                this.InitWebMenu(db);
                this.InitRoleWebMenu(db);
                this.InitConfig(db);
            }
        }
                
    }
}
