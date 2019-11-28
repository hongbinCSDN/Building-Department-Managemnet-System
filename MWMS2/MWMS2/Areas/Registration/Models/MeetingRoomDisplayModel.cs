using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.Registration.Models
{
    public class MeetingRoomDisplayModel : EditFormModel, IValidatableObject
    {

        public string RegType { get; set; }
        public string MemberlistChanged { get; set; }
        public string TargetMeetingMemberToDelete { get; set; }

        public string MEETING_NUMBER { get; set; }
        public Dictionary<string,string> memberListRole {get;set;}
        public Dictionary<string, string> IsAbsentList { get; set; }

        public string SearchBy { get; set; }
        public string CSearchCommitteePanel { get; set; }
        public string CSearchCommittee { get; set; }
        public string CSearchYear { get; set; }
        public string CSearchCommitteeGroup { get; set; }
        public string CSearchMonth { get; set; }
        public IEnumerable<SelectListItem> GetCSearchCommitteePanel
        {
            get
            {
                //string r = "";
                //if (RegType == RegistrationConstant.REGISTRATION_TYPE_MWCA || RegType == RegistrationConstant.REGISTRATION_TYPE_MWIA)
                //{
                //    r = RegistrationConstant.REGISTRATION_TYPE_MW;
                //}

                return
                     SystemListUtil.GetSVListByRegTypeNType(RegType, RegistrationConstant.SYSTEM_TYPE_PANEL_TYPE)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID });
            }
        }
        public IEnumerable<SelectListItem> GetCSearchCommittee
        {
            get
            {
                //string r = "";
                //if (RegType == RegistrationConstant.REGISTRATION_TYPE_MWCA || RegType == RegistrationConstant.REGISTRATION_TYPE_MWIA)
                //{
                //    r = RegistrationConstant.REGISTRATION_TYPE_MW;
                //}

                return
                     SystemListUtil.GetSVListByRegTypeNType(RegType, RegistrationConstant.SYSTEM_TYPE_COMMITTEE_TYPE)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID });
            }
        }
        public IEnumerable<SelectListItem> GetCSearchYear
        {
         
            get
            {
                return
                     SystemListUtil.getNextYearAndLastTenYear()
                            .Select(o => new SelectListItem() { Text = o.ToString(), Value = o.ToString() });
            }
        }
        public IEnumerable<SelectListItem> GetCSearchMonth
        {

            get
            {
                return
                     SystemListUtil.getMonth()
                            .Select(o => new SelectListItem() { Text = o.ToString(), Value = o.ToString() });
            }
        }
        public IEnumerable<SelectListItem> GetCSearchCommitteeGroup
        {

            get
            {
                return
                     SystemListUtil.getAtoZ()
                            .Select(o => new SelectListItem() { Text = o, Value = o });
            }
        }
      
        public string OSearchSurname { get; set; }
        public string OSearchGivenname { get; set; }

       
        public List<string> NewMemberUUID { get; set; }

       public C_INTERVIEW_SCHEDULE C_INTERVIEW_SCHEDULE { get; set; } = new C_INTERVIEW_SCHEDULE();
       public DateTime? InterviewSchDateTime { get; set; }
       public C_S_ROOM C_S_ROOM { get; set; }
       public C_MEETING C_MEETING { get; set; }
       public C_S_SYSTEM_VALUE INT_SCH_C_S_SYSTEM_VALUE { get; set; }
       public C_S_SYSTEM_VALUE MEET_C_S_SYSTEM_VALUE { get; set; }
       public C_COMMITTEE C_COMMITTEE { get; set; }

        public IEnumerable<SelectListItem> ExistingCommitteeGroup
        {
            get
            {
                if (C_MEETING != null)
                {
                    var o = SystemListUtil.GetCommitteeGroupByUUID(C_MEETING.COMMITTEE_GROUP_ID);

                    if (o != null)
                    {
                        yield return new SelectListItem()
                        {
                            Text = o.YEAR + "" + o.MONTH + "-" + o.C_COMMITTEE.C_S_SYSTEM_VALUE.CODE + "-" + o.NAME
                                              ,
                            Value = o.UUID
                        };
                    }
                    else
                    {
                        yield return null;
                    }
                }
                else
                {
                    yield return null;
                }


            }
        }

        public List<C_MEETING_MEMBER> MeetingMemberList { get; set; }
        // public List<MeetingRoomMemberListDisplayModel> MemberList { get; set; }
        public MeetingRoomMemberListDisplayModel MemberList { get; set; } = new MeetingRoomMemberListDisplayModel();
        public IEnumerable<SelectListItem> GetTimeSession
        {

            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                            RegistrationConstant.SYSTEM_TYPE_TIME_SESSION
                           )
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID })
             ); 
            }
        }
        public IEnumerable<SelectListItem> RoomList
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetRoomLists(
             
                           )
                            .Select(o => new SelectListItem() { Text = o.ROOM, Value = o.UUID })
             );
            }
        }
       public IEnumerable<SelectListItem> GetCommitteeGroup
       {

           get
           {
                var temp = (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                   .Concat(SystemListUtil.GetCommitteeGroupByRegTypeAndYear(
                       RegType,InterviewSchDateTime.HasValue? InterviewSchDateTime.Value.Year: 1
                     )
                      .Select(o => new SelectListItem() { Text = o.YEAR + "" + o.MONTH + "-" + o.C_COMMITTEE.C_S_SYSTEM_VALUE.CODE + "-" + o.NAME, Value = o.UUID })
       );
                //   var temp = (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                //            .Concat(SystemListUtil.GetCommitteeGroupByRegTypeAndYear(
                //                RegType, C_INTERVIEW_SCHEDULE.INTERVIEW_DATE.Year
                //              )
                //               .Select(o => new SelectListItem() { Text = o.YEAR + "" + o.MONTH + "-" + o.C_COMMITTEE.C_S_SYSTEM_VALUE.CODE + "-" + o.NAME, Value = o.UUID })
                //);
                bool existingCommitteeResult = false;
                foreach (var item in temp)
                {
                    if(ExistingCommitteeGroup.FirstOrDefault() != null)
                    {
                        if (item.Text == ExistingCommitteeGroup.FirstOrDefault().Text)
                        {
                            existingCommitteeResult = true;
                        }
                    }
                }
                if (!existingCommitteeResult && ExistingCommitteeGroup.FirstOrDefault() != null)
                {
                  temp  =  temp.Concat(ExistingCommitteeGroup);
                   // temp.Concat(ExistingCommitteeGroup.Select(v =>  new SelectListItem() {Text=v.Text , Value=v.Value }) );
                }

                return temp;
                  
           }
       }
        public IEnumerable<SelectListItem> YESNOption
        {

            get
            {
                List<SelectListItem> list = new List<SelectListItem>();


                list.Add(
                    new SelectListItem
                    {
                        Text = "No",
                        Value = "N"
                    }
                    );
                list.Add(
                    new SelectListItem
                    {
                        Text = "Yes",
                        Value = "Y"
                    }
                    );
                return list;

            }
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {


            //  yield return ValidationUtil.Validate_Length(this, "C_APPLICANT.CHINESE_NAME");
            
            yield return ValidationUtil.Validate_Mandatory(this, "InterviewSchDateTime");
            yield return ValidationUtil.Validate_Mandatory(this, "C_INTERVIEW_SCHEDULE.INTERVIEW_DATE");
            yield return ValidationUtil.Validate_Mandatory(this, "C_INTERVIEW_SCHEDULE.TIME_SESSION_ID");
            yield return ValidationUtil.Validate_Mandatory(this, "C_INTERVIEW_SCHEDULE.ROOM_ID");
            yield return ValidationUtil.Validate_Mandatory(this, "C_MEETING.COMMITTEE_GROUP_ID");
        }

            //public IEnumerable<SelectListItem> GetCommitteeRole
            //{
            //    get
            //    {
            //        return
            //            (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
            //                .Concat(SystemListUtil.GetSVListByType(
            //                    RegistrationConstant.SYSTEM_TYPE_COMMITTEE_ROLE
            //                   )
            //                    .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID })
            //     );
            //    }
            //}

        }
}