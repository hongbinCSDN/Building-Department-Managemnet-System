using MWMS2Interface.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWMS2Interface
{
    class QPCount
    {
        static void Main(string[] args)
        {
            UpdateQPCount();
        }
        static public void UpdateQPCount()
        {

            String queryStr =
    "select code, sum(yes_cnt) as yes_cnt, sum(no_cnt) as no_cnt, sum(no_ind_cnt) as no_ind_cnt, sum(seq) as seq from " +
    "( " +
    "select ip_yes.code, ip_yes.count as yes_cnt, ip_no.count as no_cnt, ip_no_ind.count as no_ind_cnt, 0 AS seq from( " +
    "( " +
    "SELECT code, sum(count) AS count, 0 AS seq FROM( " +
    "select scatgrp.code, count(*) as count, 0 as seq " +
    "from c_ind_certificate c, c_s_category_code scat, c_s_system_value scatgrp, c_ind_application iapp, c_s_system_value status " +
    "where c.CATEGORY_ID = scat.UUID " +
    "and scat.CATEGORY_GROUP_ID = scatgrp.UUID " +
    "and iapp.uuid = c.MASTER_ID " +
    "and status.uuid = c.application_status_id " +
    "and (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
    "and scatgrp.code in ('AP', 'RSE', 'RI') " +
    "and willingness_qp = 'Y' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    "group by scatgrp.code " +
    "UNION ALL " +
    "SELECT 'AP', 0, 0 FROM DUAL " +
    "UNION ALL " +
    "SELECT 'RSE', 0, 0 FROM DUAL " +
    "UNION ALL " +
    "SELECT 'RI', 0, 0 FROM DUAL) GROUP BY code " +
    ") ip_yes " +
    "LEFT JOIN " +
    "(select scatgrp.code, count(*) as count, 0 as seq " +
    "from c_ind_certificate c, c_s_category_code scat, c_s_system_value scatgrp, c_ind_application iapp, c_s_system_value status " +
    "where c.CATEGORY_ID = scat.UUID " +
    "and scat.CATEGORY_GROUP_ID = scatgrp.UUID " +
    "and iapp.uuid = c.MASTER_ID " +
    "and status.uuid = c.application_status_id " +
    "and (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
    "and scatgrp.code in ('AP', 'RSE', 'RI') " +
    "and willingness_qp = 'N' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    "group by scatgrp.code) ip_no on ip_yes.code = ip_no.code " +
    "LEFT JOIN " +
    "(select scatgrp.code, count(*) as count, 0 as seq " +
    "from c_ind_certificate c, c_s_category_code scat, c_s_system_value scatgrp, c_ind_application iapp, c_s_system_value status " +
    "where c.CATEGORY_ID = scat.UUID " +
    "and scat.CATEGORY_GROUP_ID = scatgrp.UUID " +
    "and iapp.uuid = c.MASTER_ID " +
    "and status.uuid = c.application_status_id " +
    "and (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
    "and scatgrp.code in ('AP', 'RSE', 'RI') " +
    "and (willingness_qp = 'I' or willingness_qp is null) " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    "group by scatgrp.code) ip_no_ind on ip_yes.code = ip_no_ind.code " +
    ") " +

    "union all " +
    "select cgc_yes.code, cgc_yes.count, cgc_no.count, cgc_no_ind.count, 0 AS seq from( " +
    "( " +
    "SELECT code, sum(count) AS count, 0 AS seq from( " +
    "select 'RGBC' AS code, count(*) as count, 0 as seq from(" +
    "select distinct c.file_reference_no " +
    "from c_comp_application c, c_s_system_value status, c_s_category_code scat " +
    "where c.REGISTRATION_TYPE = 'CGC' " +
    "AND c.APPLICATION_STATUS_ID = status.UUID " +
    "AND scat.UUID = c.CATEGORY_ID " +
    "AND scat.CODE = 'GBC' " +
    "and c.CERTIFICATION_NO IS NOT NULL " +
    "and willingness_qp = 'Y' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date)) " +

    "UNION ALL " +
    "SELECT 'RGBC', 0, 0 FROM DUAL) GROUP BY code " +
    ") cgc_yes " +
    "LEFT JOIN " +
    "(" +
    "select 'RGBC' AS code, count(*) as count, 0 as seq from(" +
    "select distinct c.file_reference_no " +
    "from c_comp_application c, c_s_system_value status, c_s_category_code scat " +
    "where c.REGISTRATION_TYPE = 'CGC' " +
    "AND c.APPLICATION_STATUS_ID = status.UUID " +
    "AND scat.UUID = c.CATEGORY_ID " +
    "AND scat.CODE = 'GBC' " +
    "and c.CERTIFICATION_NO IS NOT NULL " +
    "and willingness_qp = 'N' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date))) " +

    "cgc_no on cgc_yes.code = cgc_no.code " +
    "LEFT JOIN " +
    "(" +
    "select 'RGBC' AS code, count(*) as count, 0 as seq from(" +
    "select distinct c.file_reference_no " +
    "from c_comp_application c, c_s_system_value status, c_s_category_code scat " +
    "where c.REGISTRATION_TYPE = 'CGC' " +
    "AND c.APPLICATION_STATUS_ID = status.UUID " +
    "AND scat.UUID = c.CATEGORY_ID " +
    "AND scat.CODE = 'GBC' " +
    "and c.CERTIFICATION_NO IS NOT NULL " +
    "and (willingness_qp = 'I' or willingness_qp is null) " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date))) " +

    "cgc_no_ind on cgc_yes.code = cgc_no_ind.code) " +

    "union all " +
    "select imw_yes.code, imw_yes.count, imw_no.count, imw_no_ind.count, 0 AS seq from( " +
    "( " +
    "SELECT code, sum(count) AS count, 0 AS seq from( " +
    "select 'RMWC(Individual) - Item 3.6' as code, count(*) as count, 0 as seq " +
    "from c_ind_application iapp, c_ind_certificate c, c_s_system_value status " +
    "where iapp.uuid = c.MASTER_ID " +
    "and iapp.REGISTRATION_TYPE = 'IMW' " +
    "and status.uuid = c.application_status_id " +
    "and  (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
    "and iapp.uuid in (select master_id from c_ind_application_mw_item iitem, c_s_system_value sitem, c_s_system_value version " +
    "where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6') " +
    "and version.uuid = sitem.sv_mwitem_version_id " +
    "and version.code = (select english_description from c_s_system_value current_version where code = '" + "MWI_VERSION_DISPLAY_INDICATOR" + "' )) " +
    "and willingness_qp = 'Y' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    "group by iapp.REGISTRATION_TYPE " +
    "UNION ALL " +
    "SELECT 'RMWC(Individual) - Item 3.6', 0, 0 FROM DUAL) GROUP BY code " +
    ") imw_yes " +
    "LEFT JOIN " +
    "(" +
    "select 'RMWC(Individual) - Item 3.6'  as code, count(*) as count, 0 as seq " +
    "from c_ind_application iapp, c_ind_certificate c, c_s_system_value status " +
    "where iapp.uuid = c.MASTER_ID " +
    "and iapp.REGISTRATION_TYPE = 'IMW' " +
    "and status.uuid = c.application_status_id " +
    "and  (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
    "and iapp.uuid in (select master_id from c_ind_application_mw_item iitem, c_s_system_value sitem, c_s_system_value version " +
    "where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6') " +
    "and version.uuid = sitem.sv_mwitem_version_id " +
    "and version.code = (select english_description from c_s_system_value current_version where code = '" + "MWI_VERSION_DISPLAY_INDICATOR" + "' )) " +
    "and willingness_qp = 'N' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    "group by iapp.REGISTRATION_TYPE) imw_no on imw_yes.code = imw_no.code " +
    "LEFT JOIN " +
    "(" +
    "select 'RMWC(Individual) - Item 3.6' as code, count(*) as count, 0 as seq " +
    "from c_ind_application iapp, c_ind_certificate c, c_s_system_value status " +
    "where iapp.uuid = c.MASTER_ID " +
    "and iapp.REGISTRATION_TYPE = 'IMW' " +
    "and status.uuid = c.application_status_id " +
    "and  (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
    "and iapp.uuid in (select master_id from c_ind_application_mw_item iitem, c_s_system_value sitem, c_s_system_value version " +
    "where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6') " +
    "and version.uuid = sitem.sv_mwitem_version_id " +
    "and version.code = (select english_description from c_s_system_value current_version where code = '" + "MWI_VERSION_DISPLAY_INDICATOR" + "' )) " +
    "and (willingness_qp = 'I' or willingness_qp is null) " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    "group by iapp.REGISTRATION_TYPE) imw_no_ind on imw_yes.code = imw_no_ind.code " +
    ") " +


    "union all " +
    "select cmw_yes.code, cmw_yes.count, cmw_no.count, cmw_no_ind.count, 0 AS seq from( " +
    "( " +
    "SELECT code, sum(count) AS count, 0 AS seq FROM( " +
    "select 'RMWC(Company) - Type A' as code, count(*) as count, 0 as seq from( " +
    "select distinct c.file_reference_no " +
    "from c_comp_application c, c_s_system_value status, c_s_system_value s_role, c_comp_applicant_info app_info " +
    ", c_s_system_value s_status, c_applicant app " +
    "where c.REGISTRATION_TYPE = 'CMW' " +
    "and status.uuid = c.application_status_id " +
    "and c.uuid=app_info.master_id " +
    "and app_info.applicant_role_id=s_role.uuid " +
    "and app_info.applicant_status_id= s_status.uuid " +
    "and app_info.applicant_id=app.uuid " +
    "and s_role.code like 'A%'  " +
    "and s_status.code= '1' " +
    "and app_info.accept_date is not null " +
    "and ( (app_info.removal_date is null) or  (app_info.removal_date < current_date) ) " +
    "and c.certification_no is not null " +
    "and app_info.uuid in (  " +
    "select cmw.company_applicants_id " +
    "from c_comp_applicant_mw_item cmw " +
    "inner join c_s_system_value mwcode on cmw.item_type_id = mwcode.uuid " +
    "where mwcode.code in('Type A')) " +
    "and willingness_qp = 'Y' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date)) " +

    "UNION ALL " +
    "SELECT 'RMWC(Company) - Type A', 0, 0 FROM DUAL) GROUP BY code " +
    ") cmw_yes " +
    "LEFT JOIN " +
    "(" +
    "select 'RMWC(Company) - Type A' as code, count(*) as count, 0 as seq from( " +
    "select distinct c.file_reference_no " +
    "from c_comp_application c, c_s_system_value status, c_s_system_value s_role, c_comp_applicant_info app_info " +
    ", c_s_system_value s_status, c_applicant app " +
    "where c.REGISTRATION_TYPE = 'CMW' " +
    "and status.uuid = c.application_status_id " +
    "and c.uuid=app_info.master_id " +
    "and app_info.applicant_role_id=s_role.uuid " +
    "and app_info.applicant_status_id= s_status.uuid " +
    "and app_info.applicant_id=app.uuid " +
    "and s_role.code like 'A%'  " +
    "and s_status.code= '1' " +
    "and app_info.accept_date is not null " +
    "and ( (app_info.removal_date is null) or  (app_info.removal_date < current_date) ) " +
    "and c.certification_no is not null " +
    "and app_info.uuid in (  " +
    "select cmw.company_applicants_id " +
    "from c_comp_applicant_mw_item cmw " +
    "inner join c_s_system_value mwcode on cmw.item_type_id = mwcode.uuid " +
    "where mwcode.code in('Type A')) " +
    "and willingness_qp = 'N' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date)) " +

    ") cmw_no on cmw_yes.code = cmw_no.code " +
    "LEFT JOIN " +
    "(" +
    "select 'RMWC(Company) - Type A' as code, count(*) as count, 0 as seq from( " +
    "select distinct c.file_reference_no " +
    "from c_comp_application c, c_s_system_value status, c_s_system_value s_role, c_comp_applicant_info app_info " +
    ", c_s_system_value s_status, c_applicant app " +
    "where c.REGISTRATION_TYPE = 'CMW' " +
    "and status.uuid = c.application_status_id " +
    "and c.uuid=app_info.master_id " +
    "and app_info.applicant_role_id=s_role.uuid " +
    "and app_info.applicant_status_id= s_status.uuid " +
    "and app_info.applicant_id=app.uuid " +
    "and s_role.code like 'A%'  " +
    "and s_status.code= '1' " +
    "and app_info.accept_date is not null " +
    "and ( (app_info.removal_date is null) or  (app_info.removal_date < current_date) ) " +
    "and c.certification_no is not null " +
    "and app_info.uuid in (  " +
    "select cmw.company_applicants_id " +
    "from c_comp_applicant_mw_item cmw " +
    "inner join c_s_system_value mwcode on cmw.item_type_id = mwcode.uuid " +
    "where mwcode.code in('Type A')) " +
    "and (willingness_qp = 'I' or willingness_qp is null) " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date)) " +

    ") cmw_no_ind on cmw_yes.code = cmw_no_ind.code " +
    ") " +
    "UNION ALL " +
    "SELECT 'No. of Person' AS code, ind_yes.count, ind_no.count, ind_no_ind.count, 0 FROM ( " +
    "(SELECT count(DISTINCT hkid) AS count FROM ( " +
    "select hkid || passport_no AS hkid " +
    "from c_ind_application iapp, c_ind_certificate c, c_APPLICANT app, c_s_system_value status " +
    "where iapp.uuid = c.MASTER_ID " +
    "AND iapp.APPLICANT_ID = app.UUID " +
    "and iapp.REGISTRATION_TYPE = 'IMW' " +
    "and status.uuid = c.application_status_id " +
    "and  (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
    "and iapp.uuid in (select master_id from c_ind_application_mw_item iitem, c_s_system_value sitem, c_s_system_value version " +
    "where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6') " +
    "and version.uuid = sitem.sv_mwitem_version_id " +
    "and version.code = (select english_description from c_s_system_value current_version where code = '" + "MWI_VERSION_DISPLAY_INDICATOR" + "' )) " +
    "and willingness_qp = 'Y' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    "group by iapp.REGISTRATION_TYPE, hkid || passport_no " +
    "UNION ALL " +
    "select hkid || passport_no AS hkid " +
    "from c_ind_certificate c, c_s_category_code scat, c_s_system_value scatgrp, c_ind_application iapp, c_APPLICANT app, c_s_system_value status " +
    "where c.CATEGORY_ID = scat.UUID " +
    "and scat.CATEGORY_GROUP_ID = scatgrp.UUID " +
    "and iapp.uuid = c.MASTER_ID " +
    "AND app.UUID = iapp.APPLICANT_ID " +
    "and status.uuid = c.application_status_id " +
    "and  (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
    "and scatgrp.code in ('AP', 'RSE', 'RI') " +
    "and willingness_qp = 'Y' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    ") " +
    ")ind_yes " +
    "LEFT JOIN " +
    "( " +
    "SELECT count(DISTINCT hkid) AS count FROM ( " +
    "select hkid || passport_no AS hkid " +
    "from c_ind_application iapp, c_ind_certificate c, c_APPLICANT app, c_s_system_value status " +
    "where iapp.uuid = c.MASTER_ID " +
    "AND iapp.APPLICANT_ID = app.UUID " +
    "and iapp.REGISTRATION_TYPE = 'IMW' " +
    "and status.uuid = c.application_status_id " +
    "and  (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
    "and iapp.uuid in (select master_id from c_ind_application_mw_item iitem, c_s_system_value sitem, c_s_system_value version " +
    "where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6') " +
    "and version.uuid = sitem.sv_mwitem_version_id " +
    "and version.code = (select english_description from c_s_system_value current_version where code = '" + "MWI_VERSION_DISPLAY_INDICATOR" + "' )) " +
    "and willingness_qp = 'N' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    "group by iapp.REGISTRATION_TYPE, hkid || passport_no " +
    "UNION ALL " +
    "select hkid || passport_no AS hkid " +
    "from c_ind_certificate c, c_s_category_code scat, c_s_system_value scatgrp, c_ind_application iapp, c_APPLICANT app, c_s_system_value status " +
    "where c.CATEGORY_ID = scat.UUID " +
    "and scat.CATEGORY_GROUP_ID = scatgrp.UUID " +
    "and iapp.uuid = c.MASTER_ID " +
    "AND app.UUID = iapp.APPLICANT_ID " +
    "and status.uuid = c.application_status_id " +
    "and  (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
    "and scatgrp.code in ('AP', 'RSE', 'RI') " +
    "and willingness_qp = 'N' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    ") " +
    ")ind_no ON 1=1 " +
    "LEFT JOIN " +
    "( " +
    "SELECT count(DISTINCT hkid) AS count FROM ( " +
    "select hkid || passport_no AS hkid " +
    "from c_ind_application iapp, c_ind_certificate c, c_APPLICANT app, c_s_system_value status " +
    "where iapp.uuid = c.MASTER_ID " +
    "AND iapp.APPLICANT_ID = app.UUID " +
    "and iapp.REGISTRATION_TYPE = 'IMW' " +
    "and status.uuid = c.application_status_id " +
    "and  (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
    "and iapp.uuid in (select master_id from c_ind_application_mw_item iitem, c_s_system_value sitem, c_s_system_value version " +
    "where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6') " +
    "and version.uuid = sitem.sv_mwitem_version_id " +
    "and version.code = (select english_description from c_s_system_value current_version where code = '" + "MWI_VERSION_DISPLAY_INDICATOR" + "' )) " +
    "and (willingness_qp = 'I' OR willingness_qp IS NULL) " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    "group by iapp.REGISTRATION_TYPE, hkid || passport_no " +
    "UNION ALL " +
    "select hkid || passport_no AS hkid " +
    "from c_ind_certificate c, c_s_category_code scat, c_s_system_value scatgrp, c_ind_application iapp, C_APPLICANT app, c_s_system_value status " +
    "where c.CATEGORY_ID = scat.UUID " +
    "and scat.CATEGORY_GROUP_ID = scatgrp.UUID " +
    "and iapp.uuid = c.MASTER_ID " +
    "AND app.UUID = iapp.APPLICANT_ID " +
    "and status.uuid = c.application_status_id " +
    "and  (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
    "and scatgrp.code in ('AP', 'RSE', 'RI') " +
    "and (willingness_qp = 'I' OR willingness_qp IS NULL) " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    ") " +
    ")ind_no_ind ON 1=1 " +
    ") " +
    "UNION ALL " +
    "SELECT 'No. of Company' AS code, comp_yes.count, comp_no.count, comp_no_ind.count, 0 FROM ( " +
    "(SELECT count(DISTINCT br_no) AS count FROM ( " +
    "select c.BR_NO " +
    "from c_comp_application c, c_s_system_value status, c_s_category_code scat " +
    "where c.REGISTRATION_TYPE = 'CGC' " +
    "and status.uuid = c.application_status_id " +
    "AND c.CATEGORY_ID = scat.UUID " +
    "AND scat.CODE = 'GBC' " +
    "and c.CERTIFICATION_NO IS NOT NULL " +
    "and willingness_qp = 'Y' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    "group by c.REGISTRATION_TYPE, c.br_no " +
    "UNION ALL " +
    "select c.BR_NO  " +
    "from c_comp_application c, c_s_system_value status, c_s_system_value s_role, c_comp_applicant_info app_info " +
    ", c_s_system_value s_status, c_applicant app " +
    "where c.REGISTRATION_TYPE = 'CMW' " +
    "and status.uuid = c.application_status_id " +
    "and c.uuid=app_info.master_id " +
    "and app_info.applicant_role_id=s_role.uuid " +
    "and app_info.applicant_status_id= s_status.uuid " +
    "and app_info.applicant_id=app.uuid " +
    "and s_role.code like 'A%' " +
    "and s_status.code= '1' " +
    "and app_info.accept_date is not null " +
    "and ( (app_info.removal_date is null) or  (app_info.removal_date < current_date) ) " +
    "and c.certification_no is not null " +
    "and app_info.uuid in ( " +
    "select cmw.company_applicants_id " +
    "from c_comp_applicant_mw_item cmw  " +
    "inner join c_s_system_value mwcode on cmw.item_type_id = mwcode.uuid " +
    //"where mwcode.code in('Type A', 'Type G')) " + 
    "where mwcode.code in('Type A')) " +
    "and willingness_qp = 'Y' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    "group by c.REGISTRATION_TYPE, c.br_no " +
    ") " +
    ") comp_yes " +
    "LEFT JOIN " +
    "( " +
    "SELECT count(DISTINCT br_no) AS count FROM ( " +
    "select c.BR_NO " +
    "from c_comp_application c, c_s_system_value status, c_s_category_code scat " +
    "where c.REGISTRATION_TYPE = 'CGC' " +
    "and status.uuid = c.application_status_id " +
    "AND c.CATEGORY_ID = scat.UUID " +
    "AND scat.CODE = 'GBC' " +
    "and c.CERTIFICATION_NO IS NOT NULL " +
    "and willingness_qp = 'N' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    "group by c.REGISTRATION_TYPE, c.br_no " +
    "UNION ALL " +
    "select c.BR_NO  " +
    "from c_comp_application c, c_s_system_value status, c_s_system_value s_role, c_comp_applicant_info app_info " +
    ", c_s_system_value s_status, c_applicant app " +
    "where c.REGISTRATION_TYPE = 'CMW' " +
    "and status.uuid = c.application_status_id " +
    "and c.uuid=app_info.master_id " +
    "and app_info.applicant_role_id=s_role.uuid " +
    "and app_info.applicant_status_id= s_status.uuid " +
    "and app_info.applicant_id=app.uuid " +
    "and s_role.code like 'A%' " +
    "and s_status.code= '1' " +
    "and app_info.accept_date is not null " +
    "and ( (app_info.removal_date is null) or  (app_info.removal_date < current_date) ) " +
    "and c.certification_no is not null " +
    "and app_info.uuid in ( " +
    "select cmw.company_applicants_id " +
    "from c_comp_applicant_mw_item cmw  " +
    "inner join c_s_system_value mwcode on cmw.item_type_id = mwcode.uuid " +
    //"where mwcode.code in('Type A', 'Type G')) " + 
    "where mwcode.code in('Type A')) " +
    "and willingness_qp = 'N' " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    "group by c.REGISTRATION_TYPE, c.br_no " +
    ") " +
    ")comp_no ON 1=1 " +
    "LEFT JOIN " +
    "( " +
    "SELECT count(DISTINCT br_no) AS count FROM ( " +
    "select c.BR_NO " +
    "from c_comp_application c, c_s_system_value status, c_s_category_code scat " +
    "where c.REGISTRATION_TYPE = 'CGC' " +
    "and status.uuid = c.application_status_id " +
    "AND c.CATEGORY_ID = scat.UUID " +
    "AND scat.CODE = 'GBC' " +
    "and c.CERTIFICATION_NO IS NOT NULL " +
    "and (willingness_qp = 'I' OR willingness_qp IS NULL) " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    "group by c.REGISTRATION_TYPE, c.br_no " +
    "UNION ALL " +
    "select c.BR_NO  " +
    "from c_comp_application c, c_s_system_value status, c_s_system_value s_role, c_comp_applicant_info app_info " +
    ", c_s_system_value s_status, c_applicant app " +
    "where c.REGISTRATION_TYPE = 'CMW' " +
    "and status.uuid = c.application_status_id " +
    "and c.uuid=app_info.master_id " +
    "and app_info.applicant_role_id=s_role.uuid " +
    "and app_info.applicant_status_id= s_status.uuid " +
    "and app_info.applicant_id=app.uuid " +
    "and s_role.code like 'A%' " +
    "and s_status.code= '1' " +
    "and app_info.accept_date is not null " +
    "and ( (app_info.removal_date is null) or  (app_info.removal_date < current_date) ) " +
    "and c.certification_no is not null " +
    "and app_info.uuid in ( " +
    "select cmw.company_applicants_id " +
    "from C_comp_applicant_mw_item cmw  " +
    "inner join c_s_system_value mwcode on cmw.item_type_id = mwcode.uuid " +
    //"where mwcode.code in('Type A', 'Type G')) " + 
    "where mwcode.code in('Type A')) " +
    "and (willingness_qp = 'I' OR willingness_qp IS NULL) " +
    "AND ((c.expiry_date is not null and c.expiry_date >= current_date) " +
    "OR (c.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') " +
    "and c.expiry_date < current_date)) " +
    "AND (c.removal_date is null or c.removal_date > current_date) " +
    "group by c.REGISTRATION_TYPE, c.br_no " +
    ") " +
    ")comp_no_ind ON 1=1 " +
    ") " +
    "union all " +
    "select 'AP', 0, 0, 0, 1 as seq from dual " +
    "union all " +
    "select 'RSE', 0, 0, 0, 2 as seq from dual " +
    "union all " +
    "select 'RI', 0, 0, 0, 3 as seq from dual " +
    "union all " +
    "select 'RGBC', 0, 0, 0, 4 as seq from dual " +
    /*"union all " +
    "select 'RMWC(Company)', 0, 0, 0, 5 as seq from dual " +
    "union all " +
    "select 'RMWC(Individual)', 0, 0, 0, 6 as seq from dual " +*/
    "union all " +
    "select 'RMWC(Company) - Type A', 0, 0, 0, 7 as seq from dual " +
    /*"union all " +
    "select 'RMWC(Company) - Type G', 0, 0, 0, 8 as seq from dual " +
    "union all " +
    "select 'RMWC(Company) - Type A & G', 0, 0, 0, 9 as seq from dual " +*/
    "union all " +
    "select 'RMWC(Individual) - Item 3.6', 0, 0, 0, 10 as seq from dual " +
    /*"union all " +
    "select 'RMWC(Individual) - Item 3.7', 0, 0, 0, 11 as seq from DUAL " +*/
    "union all " +
    "select 'No. of Person', 0, 0, 0, 12 as seq from DUAL " +
    "union all " +
    "select 'No. of Company', 0, 0, 0, 13 as seq from dual " +
    ") " +
    "group by code " +
    "order by seq ";


            QPCountModel model = new QPCountModel();
            model.Query = queryStr;
            model.Search();

             var a = model.Data;

            foreach (var item in model.Data)
            {
                C_QP_COUNT qpc = new C_QP_COUNT();
                qpc.UUID = Guid.NewGuid().ToString().Replace("-", "");
                qpc.COUNT_DATE = DateTime.Now;
                qpc.COUNT_TYPE = item["CODE"].ToString();
                qpc.YES = Int32.Parse(item["YES_CNT"].ToString());
                qpc.NO = Int32.Parse(item["NO_IND_CNT"].ToString());
                qpc.NO_INDICATION = Int32.Parse(item["YES_CNT"].ToString());


                using (EntitiesProcessing db = new EntitiesProcessing())
                {
                    db.C_QP_COUNT.Add(qpc);
                    db.SaveChanges();
                }
                    

            }

        }
        

    }
}
