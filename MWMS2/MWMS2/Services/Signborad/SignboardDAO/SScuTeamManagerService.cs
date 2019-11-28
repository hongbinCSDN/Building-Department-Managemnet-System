using MWMS2.Constant;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class SScuTeamManagerService
    {
        public List<B_S_SCU_TEAM> getWFUserList(string position)
        {
            List<B_S_SCU_TEAM> list = new List<B_S_SCU_TEAM>();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                //var list = db.B_S_SCU_TEAM.Select(x => x.SYS_POST_ID).ToList();

                //using (EntitiesAuth dbAuth = new EntitiesAuth())
                //{
                //    var rankPoslist = dbAuth.SYS_POST.Where(x => list.Contains(x.UUID) && x.SYS_RANK.RANK_GROUP == position 
                //    && x.SYS_UNIT_ID == dbAuth.SYS_UNIT.Where(unit => unit.CODE == "SU").FirstOrDefault().UUID  ).Select(x=>x.UUID).ToList();
                //    resultUserAccount = db.B_S_SCU_TEAM.Where(x => rankPoslist.Contains(x.SYS_POST_ID) && x.SYS_POST_ID != null).ToList();
                //}
                // resultUserAccount = db.B_S_SCU_TEAM.Where(x => x.POSITION == position).Where(x => x.SYS_POST_ID != null).ToList();

                string queryString = "";
                if(SignboardConstant.S_USER_ACCOUNT_RANK_TO.Equals(position))
                {

                    queryString = " \r\n select * from B_S_SCU_TEAM V1 right join "
                               + " \r\n\t ( select distinct T2.child_sys_post_id from sys_post T1 "
                               + " \r\n\t inner join sys_rank T4 on T1.sys_rank_id = T4.uuid "
                               + " \r\n\t inner join b_s_scu_team T2 on T1.uuid = T2.child_sys_post_id "
                               + "\r\n\t where T4.rank_group in ('" + position + "') "
                               + " \r\n\t ) V2 on V1.child_sys_post_id = V2.child_sys_post_id";
                }
                else
                {
                    queryString = " \r\n select * from B_S_SCU_TEAM V1 right join "
                                + " \r\n\t ( select distinct T2.sys_post_id from sys_post T1 "
                                + " \r\n\t inner join sys_rank T4 on T1.sys_rank_id = T4.uuid "
                                + " \r\n\t inner join b_s_scu_team T2 on T1.uuid = T2.sys_post_id "
                                + "\r\n\t where T4.rank_group in ('" + position + "') "
                                + " \r\n\t ) V2 on V1.sys_post_id = V2.sys_post_id";
                }

                list = db.B_S_SCU_TEAM.SqlQuery(queryString).ToList();

                return list;
            }
        }
        public B_S_SCU_TEAM getParents(string userAccoutnUUID)
        {
            List<B_S_SCU_TEAM> resultUserAccount = new List<B_S_SCU_TEAM>();
            B_S_SCU_TEAM result = new B_S_SCU_TEAM();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                resultUserAccount = db.B_S_SCU_TEAM.Where(x => x.CHILD_SYS_POST_ID == userAccoutnUUID).ToList();
            }
            if (resultUserAccount.Count() != 0)
            {
                result = resultUserAccount[0];
            }
            return result;
        }
        public B_S_SCU_TEAM getWFUser(string position)
        {
            List<B_S_SCU_TEAM> resultUserAccount = new List<B_S_SCU_TEAM>();
            B_S_SCU_TEAM UserAccount = new B_S_SCU_TEAM();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                var list = db.B_S_SCU_TEAM.Select(x => x.SYS_POST_ID).ToList();

                using (EntitiesAuth dbAuth = new EntitiesAuth())
                {
                    var rankPoslist = dbAuth.SYS_POST.Where(x => list.Contains(x.UUID) && x.SYS_RANK.RANK_GROUP == position).Select(x => x.UUID).ToList();
                    resultUserAccount = db.B_S_SCU_TEAM.Where(x => rankPoslist.Contains(x.SYS_POST_ID) && x.SYS_POST_ID != null).ToList();
                }


                // resultUserAccount = db.B_S_SCU_TEAM.Where(x => x.POSITION == position).Where(x => x.SYS_POST_ID != null).ToList();
            }
            if (resultUserAccount.Count != 0)
            {
                UserAccount = resultUserAccount[0];
            }
            return UserAccount;
        }
    }
}