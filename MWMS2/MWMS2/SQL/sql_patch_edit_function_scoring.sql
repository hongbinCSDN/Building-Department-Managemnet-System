create or replace noneditionable function C_APPLICANT_OFFENSE_COURSE_TOTALSCORE
(
       Appl_UUID in varchar2
)
return number
IS
total_value number;
compCount Number;
indCount Number;
Begin
  Select Count(cai.Uuid) into compCount from C_COMP_APPLICANT_INFO cai Where cai.applicant_id = Appl_UUID;
  Select Count(cia.uuid) into indCount from C_IND_APPLICATION  cia Where cia.applicant_id = Appl_UUID;
  if  compCount > 0 then 
      Select (
      (Select NVL((Select  SUM(NVL(wto.score,0)) from W_WL wl inner join W_WL_TYPE_OF_OFFENSE wto on wl.uuid = wto.wl_uuid
        Where wl.as_uuid = Appl_UUID),0) from dual) 
       -
       (Select NVL((Select SUM(NVL(casc.COURSE_SCORE,0)) 
        from C_APPLICANT_SCORING cas 
        inner join C_APPLICANT_SCORING_COURSE casc on cas.UUID = casc.C_APPLICANT_SCORING_ID
        Where cas.as_uuid = Appl_UUID),0) from dual )
        ) total into total_value from dual;
   elsif indCount > 0 then 
       Select (
       (Select NVL((Select  SUM(NVL(wto.score,0)) from W_WL wl inner join W_WL_TYPE_OF_OFFENSE wto on wl.uuid = wto.wl_uuid
        Where wl.registration_no = (select indApp.FILE_REFERENCE_NO 
                                FROM C_APPLICANT Appl
                                inner join C_ind_application indApp on indApp.APPLICANT_ID = Appl.UUID                                                       
                                where 1=1 
                                and Appl.uuid= Appl_UUID)),0) from dual) 
         -
         (Select NVL((Select SUM(NVL(casc.COURSE_SCORE,0)) 
        from C_APPLICANT_SCORING cas 
        inner join C_APPLICANT_SCORING_COURSE casc on cas.UUID = casc.C_APPLICANT_SCORING_ID
        Where cas.registration_no = (select indApp.FILE_REFERENCE_NO 
                                    FROM C_APPLICANT Appl
                                    inner join C_ind_application indApp on indApp.APPLICANT_ID = Appl.UUID                                                    
                                    where 1=1 
                                    and Appl.uuid= Appl_UUID)),0) from dual)
        ) total into total_value from dual;
    Else
        Select 0 into total_value from dual;
    End if;
    
    return NVL(total_value,0);
End C_APPLICANT_OFFENSE_COURSE_TOTALSCORE;
