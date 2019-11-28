
Insert into Sys_Func
(
   UUID
  ,PARENT_ID
  ,CODE
  ,DESCRIPTION
  ,URL
  ,SEQ
  ,USE_TYPE
  ,ABLE_SHOW
  ,ABLE_LIST
  ,ABLE_VIEW_DETAILS
  ,ABLE_CREATE
  ,ABLE_EDIT
  ,ABLE_DELETE
  ,IS_ACTIVE
)
VALUES
(
  '22031'
  ,'2203'
  ,'22031'
  ,'Submission Information'
  ,'MWProcessing/ComSubmission/SubmissionInfoPage'
  ,'22031'
  ,'MENU_ITEM'
  ,'N','Y','N','N','N','N','Y'
);

Insert into Sys_Role_Func
(
   uuid
   ,SYS_ROLE_ID
   ,SYS_FUNC_ID
   ,CAN_LIST
   ,CAN_VIEW_DETAILS
   ,CAN_CREATE
   ,CAN_EDIT
   ,CAN_DELETE
   ,created_dt
  ,created_post 
)
VALUES
(
   SYS_GUID()
   ,'1'
   ,'22031'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

Insert into Sys_Role_Func
(
   uuid
   ,SYS_ROLE_ID
   ,SYS_FUNC_ID
   ,CAN_LIST
   ,CAN_VIEW_DETAILS
   ,CAN_CREATE
   ,CAN_EDIT
   ,CAN_DELETE
   ,created_dt
  ,created_post 
)
VALUES
(
   SYS_GUID()
   ,'2'
   ,'22031'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

commit;