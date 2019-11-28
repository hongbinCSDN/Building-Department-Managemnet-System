-- Audit Check %
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
  '10060112'
  ,'100601'
  ,'10060112'
  ,'Remind Letter Template Signature'
  ,'/Admin/PEM1103RemindLetterTemplateSignature/Index'
  ,'10060112'
  ,'MENU_ITEM'
  ,'Y','Y','Y','Y','Y','Y','Y'
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
   ,'10060112'
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
   ,'10060112'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

commit;