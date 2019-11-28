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
   '4990'
  ,'4000'
  ,'4990'
  ,'Admin Report'
  ,'/Registration/Fn08ADM_RPT/Index'
  ,'4990'
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
   '4991'
  ,'4990'
  ,'4991'
  ,'Admin Report'
  ,'/Registration/Fn08ADM_RPT/Index'
  ,'4991'
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
  ,'4990'
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
  ,'4990'
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
  ,'1'
  ,'4991'
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
  ,'4991'
  ,'Y'
  ,'Y'
  ,'Y'
  ,'Y'
  ,'Y'
  ,(Select  sysdate  from dual)
  ,'DBUSER04'
);


Insert into C_S_SYSTEM_VALUE
values
(
  SYS_GUID()
  ,'ALL'
  ,(Select st.uuid from C_S_SYSTEM_TYPE st Where st.type = 'INTERVIEW_RESULT' )
  ,'OS'
  ,null
  ,'O/S LETTER ISSUED'
  ,''
  ,'Y'
  ,11
  ,'SYSTEM'
  ,(Select  sysdate  from dual)
  ,'SYSTEM'
  ,(Select  sysdate  from dual)
  ,null
  ,null
)

commit
