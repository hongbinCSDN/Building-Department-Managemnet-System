
Insert Into P_S_System_Value(UUID , System_Type_ID , Code , Description ,Created_By,Created_Date,Modified_By,Modified_Date)
Select sys_guid() , UUID , 'A'  , 'a. 在完工證明書上沒有填寫正確的小型工程呈交編號；' , 'Dive' , Sysdate ,'Dive' , Sysdate  From P_S_System_type Where Type = 'Irregularities';

Insert Into P_S_System_Value(UUID , System_Type_ID , Code , Description ,Created_By,Created_Date,Modified_By,Modified_Date)
Select sys_guid() , UUID , 'B'  , 'b. 沒有填寫展開工程的日期；' , 'Dive' , Sysdate ,'Dive' , Sysdate  From P_S_System_type Where Type = 'Irregularities';

Insert Into P_S_System_Value(UUID , System_Type_ID , Code , Description ,Created_By,Created_Date,Modified_By,Modified_Date)
Select sys_guid() , UUID , 'C'  , 'c. 沒有填寫完成工程的日期；' , 'Dive' , Sysdate ,'Dive' , Sysdate  From P_S_System_type Where Type = 'Irregularities';

Insert Into P_S_System_Value(UUID , System_Type_ID , Code , Description ,Created_By,Created_Date,Modified_By,Modified_Date)
Select sys_guid() , UUID , 'D'  , 'd. 沒有填寫簽署的日期；' , 'Dive' , Sysdate ,'Dive' , Sysdate  From P_S_System_type Where Type = 'Irregularities';

Insert Into P_S_System_Value(UUID , System_Type_ID , Code , Description ,Created_By,Created_Date,Modified_By,Modified_Date)
Select sys_guid() , UUID , 'E'  , 'e. 沒有使用適當的指明表格；' , 'Dive' , Sysdate ,'Dive' , Sysdate  From P_S_System_type Where Type = 'Irregularities';

Insert Into P_S_System_Value(UUID , System_Type_ID , Code , Description ,Created_By,Created_Date,Modified_By,Modified_Date)
Select sys_guid() , UUID , 'F'  , 'f. 安排進行小型工程的人/訂明建築專業人士/訂明註冊承建商在呈交的指明表格上的簽署不是正本/並未簽署；' , 'Dive' , Sysdate ,'Dive' , Sysdate  From P_S_System_type Where Type = 'Irregularities';

Insert Into P_S_System_Value(UUID , System_Type_ID , Code , Description ,Created_By,Created_Date,Modified_By,Modified_Date)
Select sys_guid() , UUID , 'G'  , 'g. 沒有填寫有效的小型工程項目編號；' , 'Dive' , Sysdate ,'Dive' , Sysdate  From P_S_System_type Where Type = 'Irregularities';

Insert Into P_S_System_Value(UUID , System_Type_ID , Code , Description ,Created_By,Created_Date,Modified_By,Modified_Date)
Select sys_guid() , UUID , 'H'  , 'h. 沒有呈交所需的訂明圖則及詳圖/照片/圖則或工程描述；' , 'Dive' , Sysdate ,'Dive' , Sysdate  From P_S_System_type Where Type = 'Irregularities';

Insert Into P_S_System_Value(UUID , System_Type_ID , Code , Description ,Created_By,Created_Date,Modified_By,Modified_Date)
Select sys_guid() , UUID , 'I'  , 'i. 呈交的文件的規格及方式不當，例如散裝照片；' , 'Dive' , Sysdate ,'Dive' , Sysdate  From P_S_System_type Where Type = 'Irregularities';

Insert Into P_S_System_Value(UUID , System_Type_ID , Code , Description ,Created_By,Created_Date,Modified_By,Modified_Date)
Select sys_guid() , UUID , 'J'  , 'j. 沒有填寫由他人代為豎設招牌的人士的詳情；' , 'Dive' , Sysdate ,'Dive' , Sysdate  From P_S_System_type Where Type = 'Irregularities';

Insert Into P_S_System_Value(UUID , System_Type_ID , Code , Description ,Created_By,Created_Date,Modified_By,Modified_Date)
Select sys_guid() , UUID , 'K'  , 'k. 沒有填妥訂明建築專業人士/訂明註冊承建商的資料，例如英文/中文名稱、註冊證明書編號、註冊屆滿日期等；' , 'Dive' , Sysdate ,'Dive' , Sysdate  From P_S_System_type Where Type = 'Irregularities';

Insert Into P_S_System_Value(UUID , System_Type_ID , Code , Description ,Created_By,Created_Date,Modified_By,Modified_Date)
Select sys_guid() , UUID , 'L'  , 'l. 根據本署記錄，訂明建築專業人士/訂明註冊承建商的有關註冊已經失效/訂明註冊承建商並未就該小型工程項目註冊；' , 'Dive' , Sysdate ,'Dive' , Sysdate  From P_S_System_type Where Type = 'Irregularities';

Insert Into P_S_System_Value(UUID , System_Type_ID , Code , Description ,Created_By,Created_Date,Modified_By,Modified_Date)
Select sys_guid() , UUID , 'M'  , 'm. 訂明建築專業人士/訂明註冊承建商在呈交的指明表格上的簽署與本署記錄不吻合；' , 'Dive' , Sysdate ,'Dive' , Sysdate  From P_S_System_type Where Type = 'Irregularities';

Insert Into P_S_System_Value(UUID , System_Type_ID , Code , Description ,Created_By,Created_Date,Modified_By,Modified_Date)
Select sys_guid() , UUID , 'N'  , 'n. 其他:' , 'Dive' , Sysdate ,'Dive' , Sysdate  From P_S_System_type Where Type = 'Irregularities';