﻿
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MWMS2Interface.Entity
{
    public class EntityFilter : DbContext
    {

        public EntityFilter(string v) : base(v) { }
        public override int SaveChanges()
        {
            IEnumerable<DbEntityEntry> changedEntities = ChangeTracker.Entries();
            DateTime now = DateTime.Now;
            string loginPost = "SYSTEM";
            //string loginPost = "LAST_ACTION";
            foreach (DbEntityEntry changedEntity in changedEntities)
            {
                object entity = changedEntity.Entity;
                if (entity.GetType().GetProperty("UUID") == null) break;
                PropertyInfo uuidInfo = entity.GetType().GetProperty("UUID");
                string uuid = uuidInfo.GetValue(entity) as string;

                // edit mode/ create mode
                 SetProperty(entity, "MODIFIED_BY", loginPost);
                 SetProperty(entity, "MODIFIEDBY", loginPost);
                SetProperty(entity, "MODIFIED_DATE", now);
                SetProperty(entity, "MODIFIEDTIME", now);
                SetProperty(entity, "MODIFYTIME", now);

                if (string.IsNullOrWhiteSpace(uuid))
                {
                    //create mode
                    SetProperty(entity, "CREATED_DATE", now);
                    SetProperty(entity, "CREATED_DT", now);
                    SetProperty(entity, "CREATED_SUBMITTED_DATE", now);
                    SetProperty(entity, "CREATEDATE", now);
                    SetProperty(entity, "CREATETIME", now);
                    SetProperty(entity, "CREATED_BY", loginPost);
                    SetProperty(entity, "CREATED_POST", loginPost);
                    SetProperty(entity, "CREATED_SUBMITTED_BY", loginPost);
                    SetProperty(entity, "CREATEDBY", loginPost);
                    SetProperty(entity, "UUID", CommonUtil.NewUuid());
                }
            }
            return base.SaveChanges();
        }
        private void SetProperty(object entity, string key, object value)
        {
            if (entity.GetType().GetProperty(key) == null) return;
            PropertyInfo pInfo = entity.GetType().GetProperty(key);
            pInfo.SetValue(entity, value, null);
        }
    }
}