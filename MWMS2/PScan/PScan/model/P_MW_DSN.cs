using System;

public class P_MW_DSN
{
    public string UUID { get; set; }
    public string RECORD_TYPE { get; set; }
    public string RECORD_ID { get; set; }
    public string DSN { get; set; }
    public Nullable<System.DateTime> MWU_RECEIVED_DATE { get; set; }
    public Nullable<System.DateTime> RD_RECEIVED_DATE { get; set; }
    public string FORM_CODE { get; set; }
    public string SCANNED_STATUS_ID { get; set; }
    public string CREATED_BY { get; set; }
    public System.DateTime CREATED_DATE { get; set; }
    public string MODIFIED_BY { get; set; }
    public System.DateTime MODIFIED_DATE { get; set; }
    public Nullable<System.DateTime> RD_DELIVERED_DATE { get; set; }
    public Nullable<System.DateTime> REGISTRY_DELIVERED_DATE { get; set; }
    public string SUBMIT_TYPE { get; set; }
    public string OUTSTANDING_REMOVED { get; set; }
    public string RE_ASSIGN { get; set; }
    public string ITEM_SEQUENCE_NO { get; set; }
    public string SSP_SUBMITTED { get; set; }
    public Nullable<System.DateTime> ISSUED_DATE { get; set; }
    public string MW_DSN_PARENT_KEY { get; set; }
    public string SUBMIT_FLOW { get; set; }
    public string NATURE { get; set; }
    public string FROM_IMPORT { get; set; }
    public string SUBMISSION_NATURE { get; set; }
    public P_MW_ACK_LETTER Related { get; set; }
}