Update Sys_Func set URL = '/PEM1103/PEM1103' Where UUID = '100601';
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
  '1006011'
  ,'100601'
  ,'1006011'
  ,'Audit Check %'
  ,'/Admin/PEM1103AuditCheck/PEM1103AuditCheckPercentage'
  ,'1006011'
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
   ,'1006011'
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
   ,'1006011'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

--Validation Rules for Notification Days
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
  '1006012'
  ,'100601'
  ,'1006012'
  ,'Validation Rules for Notification Days'
  ,'/Admin/PEM1103VRND/Index'
  ,'1006012'
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
   ,'1006012'
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
   ,'1006012'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

--MW Number Prefix Index
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
  '1006013'
  ,'100601'
  ,'1006013'
  ,'MW Number Prefix'
  ,'/Admin/PEM1103MWNumberPrefix/Index'
  ,'1006013'
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
   ,'1006013'
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
   ,'1006013'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

--Import MW Item Description
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
  '1006014'
  ,'100601'
  ,'1006014'
  ,'Import MW Item Description'
  ,'/Admin/PEM1103ImportMWItem/Index'
  ,'1006014'
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
   ,'1006014'
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
   ,'1006014'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

--Public Hoilday
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
  '1006015'
  ,'100601'
  ,'1006015'
  ,'Public Holiday'
  ,'/Admin/PEM1103PublicHoliday/Index'
  ,'1006015'
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
   ,'1006015'
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
   ,'1006015'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

--Audit Percentage
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
  '1006016'
  ,'100601'
  ,'1006016'
  ,'Audit Percentage'
  ,'/Admin/PEM1103AuditPercentage/Index'
  ,'1006016'
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
   ,'1006016'
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
   ,'1006016'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);


--Number of daily Direct Return Over Counter
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
  '1006017'
  ,'100601'
  ,'1006017'
  ,'Number of daily Direct Return Over Counter'
  ,'/Admin/PEM1103NoOfDirectReturn/Index'
  ,'1006017'
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
   ,'1006017'
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
   ,'1006017'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

--Number of Validated Structures
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
  '1006018'
  ,'100601'
  ,'1006018'
  ,'Number of Validated Structures'
  ,'/Admin/PEM1103NumValidatedStructures/Index'
  ,'1006018'
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
   ,'1006018'
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
   ,'1006018'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);


--To Details
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
  '1006019'
  ,'100601'
  ,'1006019'
  ,'To Details'
  ,'/Admin/PEM1103ToDetails/Index'
  ,'1006019'
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
   ,'1006019'
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
   ,'1006019'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);


--Day Back
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
  '10060110'
  ,'100601'
  ,'10060110'
  ,'Day Back'
  ,'/Admin/PEM1103DayBack/Index'
  ,'10060110'
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
   ,'10060110'
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
   ,'10060110'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);


--Acknowledgement Letter Template Signature
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
  '10060111'
  ,'100601'
  ,'10060111'
  ,'Acknowledgement Letter Template Signature'
  ,'/Admin/PEM1103AckLetterTemplateSignature/Index'
  ,'10060111'
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
   ,'10060111'
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
   ,'10060111'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);




commit;
