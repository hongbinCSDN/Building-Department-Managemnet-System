Insert into SYS_FUNC 
(
   uuid
  ,parent_id
  ,code
  ,description
  ,url
  ,seq
  ,use_type
  ,able_show
  ,able_list
  ,able_view_Details
  ,able_create
  ,able_edit
  ,able_delete
  ,is_active
)values
(
   '4900'
  ,'4000'
  ,'4900'
  ,'Scoring'
  ,'/Registration/Fn09SC_SC/Index'
  ,'4900'
  ,'TOP_MENU'
  ,'Y'
  ,'Y'
  ,'Y'
  ,'Y'
  ,'Y'
  ,'Y'
  ,'Y'
);


Insert into SYS_FUNC 
(
   uuid
  ,parent_id
  ,code
  ,description
  ,url
  ,seq
  ,use_type
  ,able_show
  ,able_list
  ,able_view_Details
  ,able_create
  ,able_edit
  ,able_delete
  ,is_active
)values
(
   '4901'
  ,'4900'
  ,'4901'
  ,'Scoring'
  ,'/Registration/Fn09SC_SC/Index'
  ,'4901'
  ,'MENU_ITEM'
  ,'Y'
  ,'Y'
  ,'Y'
  ,'Y'
  ,'Y'
  ,'Y'
  ,'Y'
);

Insert into SYS_ROLE_FUNC
(
   uuid
  ,sys_role_id
  ,sys_func_id
  ,can_list
  ,can_view_details
  ,can_create
  ,can_edit
  ,can_delete
  ,created_dt
  ,created_post     
)
values
(
  SYS_GUID()
  ,'1'
  ,'4900'
  ,'Y'
  ,'Y'
  ,'Y'
  ,''
  ,'Y'
  ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

Insert into SYS_ROLE_FUNC
(
   uuid
  ,sys_role_id
  ,sys_func_id
  ,can_list
  ,can_view_details
  ,can_create
  ,can_edit
  ,can_delete
  ,created_dt
  ,created_post     
)
values
(
  SYS_GUID()
  ,'2'
  ,'4900'
  ,'Y'
  ,'Y'
  ,'Y'
  ,''
  ,'Y'
  ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

Insert into SYS_ROLE_FUNC
(
   uuid
  ,sys_role_id
  ,sys_func_id
  ,can_list
  ,can_view_details
  ,can_create
  ,can_edit
  ,can_delete
  ,created_dt
  ,created_post     
)
values
(
  SYS_GUID()
  ,'1'
  ,'4901'
  ,'Y'
  ,'Y'
  ,'Y'
  ,'Y'
  ,'Y'
  ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

Insert into SYS_ROLE_FUNC
(
   uuid
  ,sys_role_id
  ,sys_func_id
  ,can_list
  ,can_view_details
  ,can_create
  ,can_edit
  ,can_delete
  ,created_dt
  ,created_post     
)
values
(
  SYS_GUID()
  ,'2'
  ,'4901'
  ,'Y'
  ,'Y'
  ,'Y'
  ,'Y'
  ,'Y'
  ,(Select  sysdate  from dual)
  ,'DBUSER04'
);

commit
