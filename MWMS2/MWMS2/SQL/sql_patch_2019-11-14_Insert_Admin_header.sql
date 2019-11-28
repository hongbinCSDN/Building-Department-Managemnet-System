--Adminstration > MW Processing > System Parameter Management > Book mark address list
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
  '10060113'
  ,'100601'
  ,'10060113'
  ,'Book mark address list'
  ,'/Admin/PEMBookMarkAddress/Index'
  ,'10060113'
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
   ,'10060113'
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
   ,'10060113'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

--Adminstration > MW Processing > System Parameter Management > Direct Return Roster
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
  '10060114'
  ,'100601'
  ,'10060114'
  ,'Direct Return Roster'
  ,'/Admin/DirectReturnRoster/Index'
  ,'10060114'
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
   ,'10060114'
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
   ,'10060114'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

--Adminstration > Signboard > System Parameter Management > Audit Check %
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
  '1007011'
  ,'100701'
  ,'1007011'
  ,'Audit Check %'
  ,'/Admin/SPMAuditCheckPercentage/SPMAuditCheckPercentage'
  ,'1007011'
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
   ,'1007011'
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
   ,'1007011'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

--Adminstration > Signboard > System Parameter Management > Workflow Setting
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
  '1007012'
  ,'100701'
  ,'1007012'
  ,'Workflow Setting'
  ,'/Admin/SPMWorkflowSetting/SPMWorkflowSetting'
  ,'1007012'
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
   ,'1007012'
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
   ,'1007012'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

--Adminstration > Signboard > System Parameter Management > Validation Item
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
  '1007013'
  ,'100701'
  ,'1007013'
  ,'Validation Item'
  ,'/Admin/SPMValidationItem/SPMValidationItem'
  ,'1007013'
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
   ,'1007013'
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
   ,'1007013'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

--Adminstration > Signboard > System Parameter Management > Letter Template Upload
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
  '1007014'
  ,'100701'
  ,'1007014'
  ,'Letter Template Upload'
  ,'/Admin/SPMLetterTemplate/SPMLetterTemplate'
  ,'1007014'
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
   ,'1007014'
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
   ,'1007014'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

--Adminstration > Signboard > System Parameter Management > District Maintenance
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
  '1007015'
  ,'100701'
  ,'1007015'
  ,'District Maintenance'
  ,'/Admin/SPMDistrictMaintenance/SPMDistrictMaintenance'
  ,'1007015'
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
   ,'1007015'
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
   ,'1007015'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

--Adminstration > Signboard > System Parameter Management > Link Maintenance
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
  '1007016'
  ,'100701'
  ,'1007016'
  ,'Link Maintenance'
  ,'/Admin/SPMLinkMaintenance/SPMLinkMaintenance'
  ,'1007016'
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
   ,'1007016'
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
   ,'1007016'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

commit;