using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.Models;
using Afx.Utils;
using AfxDotNetCoreSample.Common;
using System.Data;

namespace AfxDotNetCoreSample.Repository
{
    public partial class SystemRepository
    {
        private void InitRegion(AfxContext db)
        {
            if (db.Region.Count() == 0)
            {
                List<RegionLevel> regionLevels = null;
                List<Region> regions = this.GetRegions(out regionLevels);
                foreach (var region in regions)
                {
                    var m = db.Region.Where(q => q.Id == region.Id).FirstOrDefault();
                    if (m == null)
                    {
                        using (db.BeginTransaction(IsolationLevel.ReadCommitted))
                        {
                            db.Add(region);
                            var levels = regionLevels.FindAll(q => q.RegionId == region.Id);
                            db.RegionLevel.AddRange(levels);
                            db.SaveChanges();
                            db.Commit();
                        }
                    }
                }
            }
        }

        private List<Region> GetRegions(out List<RegionLevel> regionLevels)
        {
            List<Region> list = new List<Region>();
            regionLevels = new List<RegionLevel>();
            string path = PathUtils.GetFileFullPath("Config/region.txt");
            if (!System.IO.File.Exists(path)) throw new System.IO.FileNotFoundException("Config/region.txt");
            using (var fs = System.IO.File.OpenText(path))
            {
                string s = null;
                while ((s = fs.ReadLine()) != null)
                {
                    string[] arr = s.Trim().Split(' ', '\t');
                    if (arr.Length >= 2)
                    {
                        string code = arr[0].Trim();
                        string name = arr[1].Trim();
                        if (code.Length == 6)
                        {
                            Region parent = null;
                            if (code.Substring(2, 2) == "00" && code.Substring(4) == "00")
                            {

                            }
                            else if (code.Substring(2, 2) != "00" && code.Substring(4) == "00")
                            {
                                var parentid = code.Substring(0, 2) + "0000";
                                parent = list.Find(q => q.Id == parentid);
                            }
                            else
                            {
                                var parentid = code.Substring(0, 4) + "00";
                                parent = list.Find(q => q.Id == parentid);
                                if (parent == null) parentid = code.Substring(0, 2) + "0000";
                                parent = list.Find(q => q.Id == parentid);
                            }

                            var m = new Region()
                            {
                                Id = code,
                                ParentId = parent?.Id,
                                Level = (parent?.Level ?? 0) + 1,
                                Name = name,
                                IsDelete = false,
                            };
                            list.Add(m);

                            if (parent != null)
                            {
                                var plist = regionLevels.FindAll(q => q.RegionId == parent.Id);
                                foreach (var l in plist)
                                {
                                    var pl = new RegionLevel()
                                    {
                                        Id = code + "-" + l.ParentLevel.ToString("d2"),
                                        ParentId = l.ParentId,
                                        ParentLevel = l.ParentLevel,
                                        RegionId = code
                                    };
                                    regionLevels.Add(pl);
                                }
                            }

                            var level = new RegionLevel()
                            {
                                Id = code + "-" + m.Level.ToString("d2"),
                                ParentId = code,
                                ParentLevel = m.Level,
                                RegionId = code
                            };
                            regionLevels.Add(level);
                        }
                    }
                }
            }

            return list;
        }
    }
}
