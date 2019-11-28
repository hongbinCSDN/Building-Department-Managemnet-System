using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using System.Collections.Generic;
using System.Linq;

namespace MWMS2.Areas.Registration.Models
{
    public class BuildingSafetyCheckListModel
    {

        public string RegType { get; set; }
        public string MasterUuid { get; set; }
        public List<BuildingSafetyItem> BsItems { get; set; }
        public void init()
        {
            BsItems = new List<BuildingSafetyItem>();
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                List< C_S_SYSTEM_VALUE> bsList = db.C_S_SYSTEM_VALUE.Where(o =>
                o.C_S_SYSTEM_TYPE.TYPE == RegistrationConstant.SYSTEM_TYPE_BUILDING_SAFETY_CODE
                && o.REGISTRATION_TYPE == RegType).ToList();
                List<C_BUILDING_SAFETY_INFO> sss = db.C_BUILDING_SAFETY_INFO.ToList();
                for (int i = 0; i < bsList.Count; i++)
                {
                    string bsUuid = bsList[i].UUID;
                    C_BUILDING_SAFETY_INFO bs = (
                        from t1 in db.C_BUILDING_SAFETY_INFO
                        join t2 in db.C_S_SYSTEM_VALUE on t1.C_S_SYSTEM_VALUE equals t2
                        where t1.MASTER_ID == MasterUuid && t2.UUID == bsUuid
                        orderby t2.ORDERING
                        select t1).FirstOrDefault();

               /* C_BUILDING_SAFETY_INFO bs = db.C_BUILDING_SAFETY_INFO.Where(o => 
                    o.MASTER_ID == MasterUuid 
                    && o.C_S_SYSTEM_VALUE.UUID == bsList[i].UUID).FirstOrDefault();*/
                    if ("Y".Equals(bsList[i].IS_ACTIVE))
                    {
                        bool itemChecked = false;
                        if (bs != null) itemChecked = true;
                        BsItems.Add(new BuildingSafetyItem()
                        {
                            Checked = itemChecked,
                            CheckListUuid = bsList[i].UUID,
                            Code = bsList[i].CODE,
                            Description = bsList[i].ENGLISH_DESCRIPTION
                        });
                    }
                    else
                    {
                        if (bs != null)
                        {
                            BsItems.Add(new BuildingSafetyItem()
                            {
                                Checked = true,
                                CheckListUuid = bsList[i].UUID,
                                Code = bsList[i].CODE,
                                Description = bsList[i].ENGLISH_DESCRIPTION
                            });
                        }

                    }

                }
            }
        }
    }


}