
ALTER TABLE P_WF_TASKTOUSER ADD 
(
POST_CODE VARCHAR2(200) NULL
);
ALTER TABLE P_WF_ROUND_ROBIN ADD AREA NUMBER(20) NULL;


ALTER TABLE P_S_SCU_TEAM
ADD CONSTRAINT fk_P_S_SCU_TEAM_FK1
  FOREIGN KEY (SYS_POST_ID)
  REFERENCES SYS_POST(UUID);
  

ALTER TABLE P_S_SCU_TEAM
ADD CONSTRAINT fk_P_S_SCU_TEAM_FK2
  FOREIGN KEY (CHILD_SYS_POST_ID)
  REFERENCES SYS_POST(UUID);

  UPDATE B_S_SCU_TEAM SET PARENT_ID = NULL;
COMMIT;