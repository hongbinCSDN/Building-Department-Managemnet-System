-- 
create table B_EFSS_SUBMISSION_MAP (
    uuid VARCHAR2(36),
    EFSS_SUBMISSION_NO NVARCHAR2(20) not null,
    SMM_REFERENCE_NO VARCHAR2(36) not null,
	DSN VARCHAR2(50) not null,
    created_by VARCHAR2(20) not null,
    created_date date not null,
    modified_by VARCHAR2(20) not null,
    modified_date date not null,
    constraint B_EFSS_SUBMISSION_MAP_UUID_PK primary key (uuid) 
)