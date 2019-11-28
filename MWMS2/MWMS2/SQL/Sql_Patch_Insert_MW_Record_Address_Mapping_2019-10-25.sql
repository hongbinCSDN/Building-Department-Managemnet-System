
-- MW Record Address Mapping %
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
  '2113'
  ,'2100'
  ,'2113'
  ,'MW Record Address Mapping'
  ,'MWProcessing/Fn02MWUR_RAM/Index'
  ,'2113'
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
   ,'2113'
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
   ,'2113'
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
   ,'a1d0ca4a9adc419fa1f657c61911797a'
   ,'2113'
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
   ,'GUEST'
   ,'2113'
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
   ,'c13dfb970b9d4953bbf09edb4c8d483a'
   ,'2113'
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
   ,'f1a926df7c454740b1b323b26f682809'
   ,'2113'
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
   ,'7bdc5af33f0649c3a0f1c108e60f1ce7'
   ,'2113'
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
   ,'1961183858ad4ebea20c6adab36e8c8a'
   ,'2113'
   ,'Y','Y','Y','Y','Y'
   ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

commit;