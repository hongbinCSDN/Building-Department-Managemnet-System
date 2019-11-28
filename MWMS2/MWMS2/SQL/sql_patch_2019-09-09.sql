

DROP TABLE P_WF_TASK_USER CASCADE;


DROP TABLE P_WF_TASK CASCADE;


DROP TABLE P_WF_INFO CASCADE;

CREATE TABLE P_WF_INFO
	(
	UUID           VARCHAR2 (36) NOT NULL,
	RECORD_ID      VARCHAR2 (36),
	RECORD_TYPE    VARCHAR2 (36),
	CURRENT_STATUS VARCHAR2 (36),
	CREATED_BY     VARCHAR2 (100),
	CREATED_DATE   DATE,
	MODIFIED_BY    VARCHAR2 (100),
	MODIFIED_DATE  DATE,
	CONSTRAINT P_WF_INFO_PK PRIMARY KEY (UUID)
	);

CREATE TABLE P_WF_TASK
	(
	UUID          VARCHAR2 (36) NOT NULL,
	P_WF_INFO_ID  VARCHAR2 (36),
	TASK_CODE     VARCHAR2 (36),
	STATUS        VARCHAR2 (36),
	START_TIME    DATE,
	END_TIME      DATE,
	CREATED_BY    VARCHAR2 (100),
	CREATED_DATE  DATE,
	MODIFIED_BY   VARCHAR2 (100),
	MODIFIED_DATE DATE,
	CONSTRAINT P_WF_TASK_PK PRIMARY KEY (UUID),
	CONSTRAINT FK_P_WF_TASK_WF_INFO FOREIGN KEY (P_WF_INFO_ID) REFERENCES MWMS2.P_WF_INFO (UUID)
	);

CREATE TABLE P_WF_TASK_USER
	(
	UUID          VARCHAR2 (36) NOT NULL,
	P_WF_TASK_ID  VARCHAR2 (36),
	USER_ID       VARCHAR2 (36),
	START_TIME    DATE,
	ACTION_TIME   DATE,
	CREATED_BY    VARCHAR2 (100),
	CREATED_DATE  DATE,
	MODIFIED_BY   VARCHAR2 (100),
	MODIFIED_DATE DATE,
	STATUS        VARCHAR2 (36),
	CONSTRAINT P_WF_TASK_USER_PK PRIMARY KEY (UUID),
	FOREIGN KEY (P_WF_TASK_ID) REFERENCES MWMS2.P_WF_TASK (UUID)
	);
