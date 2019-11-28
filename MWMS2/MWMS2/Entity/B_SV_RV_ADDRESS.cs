//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MWMS2.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class B_SV_RV_ADDRESS
    {
        public string UUID { get; set; }
        public string FLAT { get; set; }
        public string FLOOR { get; set; }
        public string BLOCK { get; set; }
        public string BUILDING_NAME { get; set; }
        public string STREET_NO { get; set; }
        public string STREE_NAME { get; set; }
        public string DISTRICT { get; set; }
        public string REGION { get; set; }
        public string LOCATION { get; set; }
        public string ADDRESS_TYPE { get; set; }
        public string PERSON_CONTACT_ID { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public string BLOCK_ID { get; set; }
        public string UNIT_ID { get; set; }
        public string STREET_LOCATION_TABLE_ID { get; set; }
        public string ENGLISH_STREET_NAME { get; set; }
        public string ENGLISH_STREET_TYPE { get; set; }
        public string ENGLISH_STREET_DIRECTION { get; set; }
        public string ENGLISH_ST_TYPE_PRE_INDICATOR { get; set; }
        public string ENGLISH_ST_LOCATION_NAME_1 { get; set; }
        public string ENGLISH_ST_LOCATION_NAME_2 { get; set; }
        public string ENGLISH_ST_LOCATION_NAME_3 { get; set; }
        public string CHINESE_STREET_NAME { get; set; }
        public string CHINESE_STREET_TYPE { get; set; }
        public string CHINESE_STREET_DIRECTION { get; set; }
        public string CHINESE_ST_TYPE_PRE_INDICATOR { get; set; }
        public string CHINESE_ST_LOCATION_NAME_1 { get; set; }
        public string CHINESE_ST_LOCATION_NAME_2 { get; set; }
        public string CHINESE_ST_LOCATION_NAME_3 { get; set; }
        public Nullable<short> BUILDING_NO_NUMERIC { get; set; }
        public string BUILDING_NO_ALPHA { get; set; }
        public string BUILDING_NO_EXTENSION { get; set; }
        public Nullable<short> BLOCK_ID_NUMERIC { get; set; }
        public string BLOCK_ID_ALPHA { get; set; }
        public string BLOCK_ID_ALPHA_PRE_INDICATOR { get; set; }
        public string ENGLISH_BLOCK_DESCRIPTION { get; set; }
        public string CHINESE_BLOCK_DESCRIPTION { get; set; }
        public string BLOCK_DESC_PRECEDE_INDICATOR { get; set; }
        public string ENGLISH_BUILDING_NAME_LINE_1 { get; set; }
        public string ENGLISH_BUILDING_NAME_LINE_2 { get; set; }
        public string ENGLISH_BUILDING_NAME_LINE_3 { get; set; }
        public string CHINESE_BUILDING_NAME_LINE_1 { get; set; }
        public string CHINESE_BUILDING_NAME_LINE_2 { get; set; }
        public string CHINESE_BUILDING_NAME_LINE_3 { get; set; }
        public string ENGLISH_BLOCK_ADDRESS { get; set; }
        public string ENGLISH_BLOCK_ADDRESS_2 { get; set; }
        public string CHINESE_BLOCK_ADDRESS { get; set; }
        public string ENGLISH_DEVELOPMENT_NAME { get; set; }
        public string CHINESE_DEVELOPMENT_NAME { get; set; }
        public string FLOOR_CODE { get; set; }
        public string ENGLISH_FLOOR_DESCRIPTION { get; set; }
        public string CHINESE_FLOOR_DESCRIPTION { get; set; }
        public string ENGLISH_UNIT_NO { get; set; }
        public string CHINESE_UNIT_NO { get; set; }
        public string ASSESSMENT_NO { get; set; }
        public string ENGLISH_TENEMENT_DESCRIPTION { get; set; }
        public string CHINESE_TENEMENT_DESCRIPTION { get; set; }
        public string ENGLISH_RRM_ADDRESS { get; set; }
        public string ENGLISH_RRM_BUILDING { get; set; }
        public string CHINESE_RRM_ADDRESS { get; set; }
        public string CHINESE_RRM_BUILDING { get; set; }
        public string DISPLAY_STREET { get; set; }
        public string DISPLAY_STREET_NO { get; set; }
        public string DISPLAY_BUILDINGNAME { get; set; }
        public string DISPLAY_FLOOR { get; set; }
        public string DISPLAY_FLAT { get; set; }
        public string DISPLAY_DISTRICT { get; set; }
        public string DISPLAY_REGION { get; set; }
        public string ENGLISH_DISPLAY { get; set; }
        public string CHINESE_DISPLAY { get; set; }
        public string DISPLAY_STREET_CODE { get; set; }
        public string DISPLAY_BLOCK_ID { get; set; }
        public string RRM_SYN_STATUS { get; set; }
        public string SV_ADDRESS_ID { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string BCIS_BLOCK_ID { get; set; }
        public string FILE_REFERENCE_NO { get; set; }
    
        public virtual B_SV_ADDRESS B_SV_ADDRESS { get; set; }
    }
}
