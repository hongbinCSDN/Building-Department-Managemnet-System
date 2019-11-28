using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{
    public class MeetingRoomMemberListDisplayModel
    {
      
        public string MEETING_UUID { get; set; }
        public string Committee_Group_Id { get; set; }
        public List<MeetingRoomMemberModel> MemberList { get; set; }
        public void init()
        {
            MemberList = new List<MeetingRoomMemberModel>();
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                List<C_MEETING_MEMBER> mml = new List<C_MEETING_MEMBER>();
                //var q = from mm in db.C_MEETING_MEMBER
                //        join cm in db.C_COMMITTEE_MEMBER on mm.MEMBER_ID equals cm.UUID
                //        join cmapplicant in db.C_APPLICANT on cm.APPLICANT_ID equals cmapplicant.UUID
                //        where mm.MEETING_ID == MEETING_UUID
                //        select mm;
                var q = db.C_MEETING_MEMBER.Where(x => x.MEETING_ID == MEETING_UUID).Include(x => x.C_COMMITTEE_MEMBER)
                         .Include(x => x.C_COMMITTEE_MEMBER.C_APPLICANT);
                mml.AddRange(q);
                     
                foreach (var item in mml)
                {
                    MeetingRoomMemberModel member = new MeetingRoomMemberModel();

                    member.UUID = item.C_COMMITTEE_MEMBER.UUID;
                    member.Name = item.C_COMMITTEE_MEMBER.C_APPLICANT.SURNAME + " " + item.C_COMMITTEE_MEMBER.C_APPLICANT.GIVEN_NAME_ON_ID;
                    member.HKID = EncryptDecryptUtil.getDecryptHKID(item.C_COMMITTEE_MEMBER.C_APPLICANT.HKID);
                    member.PassportNo = EncryptDecryptUtil.getDecryptHKID(item.C_COMMITTEE_MEMBER.C_APPLICANT.PASSPORT_NO);
                    member.Rank = item.C_COMMITTEE_MEMBER.RANK;
                    member.Post = item.C_COMMITTEE_MEMBER.POST;
                    member.Career = item.C_COMMITTEE_MEMBER.CAREER;
                    member.Role = item.COMMITTEE_ROLE_ID;
                    member.Absent = item.IS_ABSENT;
                    MemberList.Add(member);
           
                }
            
                List<string> ToNewMemeberList = new List<string>();
               // if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_COMMITTEE_MEMBER))
               // {
                    List<string> TempnewMember = SessionUtil.DraftList<string>(ApplicationConstant.DRAFT_KEY_COMMITTEE_MEMBER);
                foreach (var u in TempnewMember)
                    {
                        if (MemberList.Exists(x => x.UUID == u))
                        {
                            
                        }
                        else
                        {
                            if (!ToNewMemeberList.Contains(u))
                            ToNewMemeberList.Add(u);
                        }
                    }
              //  }

                foreach (var item in ToNewMemeberList)
                {
                    MeetingRoomMemberModel mrm = new MeetingRoomMemberModel();
                
                    var query = (from cm in db.C_COMMITTEE_MEMBER
                                 join cgm in db.C_COMMITTEE_GROUP_MEMBER on cm.UUID equals cgm.MEMBER_ID into gj
                                 from x in gj.DefaultIfEmpty()
                                 join app in db.C_APPLICANT on cm.APPLICANT_ID equals app.UUID
                                 where cm.UUID == item
                                 select new MeetingRoomMemberModel
                                 {
                                     Role = x.COMMITTEE_ROLE_ID,
                                     HKID = app.HKID,
                                     PassportNo = app.PASSPORT_NO,                             
                                     sName = app.SURNAME,
                                     gName = app.GIVEN_NAME_ON_ID,
                                     Rank = cm.RANK,
                                     Post = cm.POST,
                                     Career = cm.CAREER,

                                 }).FirstOrDefault();
                    query.UUID = item;
                    query.HKID = EncryptDecryptUtil.getDecryptHKID(query.HKID);
                    query.Name = query.sName + " " + query.gName;
                    query.PassportNo = EncryptDecryptUtil.getDecryptHKID(query.PassportNo);

                    MemberList.Add(query);



                }

                List<string> TempDeleteMember = SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_COMMITTEE_MEMBER);
                foreach (var i in TempDeleteMember)
                {
                    MemberList.RemoveAll(x => x.UUID == i);
                }


                MemberList = MemberList.OrderBy(x => x.Name).ToList();

            }
        }

        public void loadMember()
        {
            MemberList = new List<MeetingRoomMemberModel>();
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
              
                //  List<C_COMMITTEE_GROUP_MEMBER> cgmList= new List<C_COMMITTEE_GROUP_MEMBER>();
                //var q = from cgm in db.C_COMMITTEE_GROUP_MEMBER
                //        join cm in db.C_COMMITTEE_MEMBER on cgm.MEMBER_ID equals cm.UUID
                //        join app in db.C_APPLICANT on cm.APPLICANT_ID equals app.UUID
                //        where cgm.COMMITTEE_GROUP_ID == Committee_Group_Id
                //        select cgm;
                var q = db.C_COMMITTEE_GROUP_MEMBER.Where(x => x.COMMITTEE_GROUP_ID == Committee_Group_Id)
                             .Include(x => x.C_COMMITTEE_MEMBER)
                             .Include(x => x.C_COMMITTEE_MEMBER.C_APPLICANT);
                             







                foreach (var item in q)
                {
                    MeetingRoomMemberModel member = new MeetingRoomMemberModel();

                    member.UUID = item.C_COMMITTEE_MEMBER.UUID;
                    member.Name = item.C_COMMITTEE_MEMBER.C_APPLICANT.SURNAME + " " + item.C_COMMITTEE_MEMBER.C_APPLICANT.GIVEN_NAME_ON_ID;
                    member.HKID = EncryptDecryptUtil.getDecryptHKID(item.C_COMMITTEE_MEMBER.C_APPLICANT.HKID);
                    member.PassportNo = EncryptDecryptUtil.getDecryptHKID(item.C_COMMITTEE_MEMBER.C_APPLICANT.PASSPORT_NO);
                    member.Rank = item.C_COMMITTEE_MEMBER.RANK;
                    member.Post = item.C_COMMITTEE_MEMBER.POST;
                    member.Career = item.C_COMMITTEE_MEMBER.CAREER;
                    member.Role = item.COMMITTEE_ROLE_ID;
                   //member.Absent = item.C_COMMITTEE_MEMBER.UUID;
                    MemberList.Add(member);

                }
                List<string> ToNewMemeberList = new List<string>();
                List<string> TempnewMember = SessionUtil.DraftList<string>(ApplicationConstant.DRAFT_KEY_COMMITTEE_MEMBER);
                foreach (var u in TempnewMember)
                {
                    if (MemberList.Exists(x => x.UUID == u))
                    {

                    }
                    else
                    {
                        if(!ToNewMemeberList.Contains(u))
                            ToNewMemeberList.Add(u);
                    }
                }

                foreach (var item in ToNewMemeberList)
                {
                    MeetingRoomMemberModel mrm = new MeetingRoomMemberModel();

                    var query = (from cm in db.C_COMMITTEE_MEMBER
                                 join cgm in db.C_COMMITTEE_GROUP_MEMBER on cm.UUID equals cgm.MEMBER_ID into gj
                                 from x in gj.DefaultIfEmpty()
                                 join app in db.C_APPLICANT on cm.APPLICANT_ID equals app.UUID
                                 where cm.UUID == item
                                 select new MeetingRoomMemberModel
                                 {
                                     Role = x.COMMITTEE_ROLE_ID,
                                     HKID = app.HKID,
                                     PassportNo = app.PASSPORT_NO,
                                     sName = app.SURNAME,
                                     gName = app.GIVEN_NAME_ON_ID,
                                     Rank = cm.RANK,
                                     Post = cm.POST,
                                     Career = cm.CAREER,

                                 }).FirstOrDefault();
                    query.UUID = item;
                    query.HKID = EncryptDecryptUtil.getDecryptHKID(query.HKID);
                    query.Name = query.sName + " " + query.gName;
                    query.PassportNo = EncryptDecryptUtil.getDecryptHKID(query.PassportNo);

                    MemberList.Add(query);



                }
                List<string> TempDeleteMember = SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_COMMITTEE_MEMBER);
                foreach (var i in TempDeleteMember)
                {
                    MemberList.RemoveAll(x => x.UUID == i);
                }
                MemberList = MemberList.OrderBy(x => x.Name).ToList();

            }

        }
        public IEnumerable<SelectListItem> GetCommitteeRole
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                            RegistrationConstant.SYSTEM_TYPE_COMMITTEE_ROLE
                           )
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID })
             );
            }
        }
    }
}