
ALTER TABLE W_S_SYSTEM_VALUE 
modify  DESCRIPTION_ENG VARCHAR2(400)

INSERT INTO MWMS2.W_S_SYSTEM_TYPE (UUID, TYPE, DESCRIPTION, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY)
VALUES (sys_guid(), 'Technical', 'Technical', SYSDATE, 'admin', SYSDATE, 'admin')



INSERT INTO MWMS2.W_S_SYSTEM_TYPE (UUID, TYPE, DESCRIPTION, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY)
VALUES (sys_guid(), 'Procedural', 'Procedural', SYSDATE, 'admin', SYSDATE, 'admin')

INSERT INTO MWMS2.W_S_SYSTEM_TYPE (UUID, TYPE, DESCRIPTION, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY)
VALUES (sys_guid(), 'Miscellaneous', 'Miscellaneous', SYSDATE, 'admin', SYSDATE, 'admin')


INSERT INTO MWMS2.W_S_SYSTEM_VALUE (UUID, DESCRIPTION_ENG, DESCRIPTION_CHI, S_SYSTEM_TYPE_ID, ORDERING, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY, REMARK, TYPE)
VALUES (sys_guid(), 'T1 Fail to carry out windows inspection/minor works personally (if needed)', null, 'E3316967999740B2B3EA4723D2B1F42A', null, SYSDATE, 'admin', SYSDATE, 'admin', null, 'Technical')

INSERT INTO MWMS2.W_S_SYSTEM_VALUE (UUID, DESCRIPTION_ENG, DESCRIPTION_CHI, S_SYSTEM_TYPE_ID, ORDERING, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY, REMARK, TYPE)
VALUES (sys_guid(), 'T2 Works not meeting required statutory or technical standards', null, 'E3316967999740B2B3EA4723D2B1F42A', null, SYSDATE, 'admin', SYSDATE, 'admin', null, 'Technical')


INSERT INTO MWMS2.W_S_SYSTEM_VALUE (UUID, DESCRIPTION_ENG, DESCRIPTION_CHI, S_SYSTEM_TYPE_ID, ORDERING, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY, REMARK, TYPE)
VALUES (sys_guid(), 'T3 Improper supervision of works', null, 'E3316967999740B2B3EA4723D2B1F42A', null, SYSDATE, 'admin', SYSDATE, 'admin', null, 'Technical')

INSERT INTO MWMS2.W_S_SYSTEM_VALUE (UUID, DESCRIPTION_ENG, DESCRIPTION_CHI, S_SYSTEM_TYPE_ID, ORDERING, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY, REMARK, TYPE)
VALUES (sys_guid(), 'T4 Minor irregularities in carrying out inspection and repair works (e.g. cracked window glazing unattended)', null, 'E3316967999740B2B3EA4723D2B1F42A', null, SYSDATE, 'admin', SYSDATE, 'admin', null, 'Technical')



INSERT INTO MWMS2.W_S_SYSTEM_VALUE (UUID, DESCRIPTION_ENG, DESCRIPTION_CHI, S_SYSTEM_TYPE_ID, ORDERING, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY, REMARK, TYPE)
VALUES (sys_guid(), 'P1 Misrepresent a material fact in the submitted documents', null, 'B8815D2AA3A74A7CB6A0C1AB7EADAE81', null, SYSDATE, 'admin', SYSDATE, 'admin', null, 'Procedural')


INSERT INTO MWMS2.W_S_SYSTEM_VALUE (UUID, DESCRIPTION_ENG, DESCRIPTION_CHI, S_SYSTEM_TYPE_ID, ORDERING, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY, REMARK, TYPE)
VALUES (sys_guid(), 'P2 Material deviation from submitted plans', null, 'B8815D2AA3A74A7CB6A0C1AB7EADAE81', null, SYSDATE, 'admin', SYSDATE, 'admin', null, 'Procedural')



INSERT INTO MWMS2.W_S_SYSTEM_VALUE (UUID, DESCRIPTION_ENG, DESCRIPTION_CHI, S_SYSTEM_TYPE_ID, ORDERING, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY, REMARK, TYPE)
VALUES (sys_guid(), 'P3 Fail to obtain prior approval and consent under the BO for carrying out building works other than minor works/designated exempted works/exempted works', null, 'B8815D2AA3A74A7CB6A0C1AB7EADAE81', null, SYSDATE, 'admin', SYSDATE, 'admin', null, 'Procedural')



INSERT INTO MWMS2.W_S_SYSTEM_VALUE (UUID, DESCRIPTION_ENG, DESCRIPTION_CHI, S_SYSTEM_TYPE_ID, ORDERING, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY, REMARK, TYPE)
VALUES (sys_guid(), 'P4 Not qualified to certify/carry out respective class/type/item of minor works', null, 'B8815D2AA3A74A7CB6A0C1AB7EADAE81', null, SYSDATE, 'admin', SYSDATE, 'admin', null, 'Procedural')



INSERT INTO MWMS2.W_S_SYSTEM_VALUE (UUID, DESCRIPTION_ENG, DESCRIPTION_CHI, S_SYSTEM_TYPE_ID, ORDERING, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY, REMARK, TYPE)
VALUES (sys_guid(), 'P5 Fail to submit prescribed/specified documents/plans within statutory period', null, 'B8815D2AA3A74A7CB6A0C1AB7EADAE81', null, SYSDATE, 'admin', SYSDATE, 'admin', null, 'Procedural')



INSERT INTO MWMS2.W_S_SYSTEM_VALUE (UUID, DESCRIPTION_ENG, DESCRIPTION_CHI, S_SYSTEM_TYPE_ID, ORDERING, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY, REMARK, TYPE)
VALUES (sys_guid(), 'P6 Insufficient documents/information', null, 'B8815D2AA3A74A7CB6A0C1AB7EADAE81', null, SYSDATE, 'admin', SYSDATE, 'admin', null, 'Procedural')


INSERT INTO MWMS2.W_S_SYSTEM_VALUE (UUID, DESCRIPTION_ENG, DESCRIPTION_CHI, S_SYSTEM_TYPE_ID, ORDERING, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY, REMARK, TYPE)
VALUES (sys_guid(), 'P7 Unauthorized use of the BD''s logo', null, 'B8815D2AA3A74A7CB6A0C1AB7EADAE81', null, SYSDATE, 'admin', SYSDATE, 'admin', null, 'Procedural')





INSERT INTO MWMS2.W_S_SYSTEM_VALUE (UUID, DESCRIPTION_ENG, DESCRIPTION_CHI, S_SYSTEM_TYPE_ID, ORDERING, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY, REMARK, TYPE)
VALUES (sys_guid(), 'P8 Repeatedly fail to respond to BD''s written warning concerning aspects of deficiencies or non-compliances without a reasonable explanation', null, 'B8815D2AA3A74A7CB6A0C1AB7EADAE81', null, SYSDATE, 'admin', SYSDATE, 'admin', null, 'Procedural')




INSERT INTO MWMS2.W_S_SYSTEM_VALUE (UUID, DESCRIPTION_ENG, DESCRIPTION_CHI, S_SYSTEM_TYPE_ID, ORDERING, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY, REMARK, TYPE)
VALUES (sys_guid(), 'M1 Special cases (e.g Blatant cases, extensive quantities or other ', null, '57EB569D873645C186D5E015393B1443', null, SYSDATE, 'admin', SYSDATE, 'admin', null, 'Miscellaneous')
