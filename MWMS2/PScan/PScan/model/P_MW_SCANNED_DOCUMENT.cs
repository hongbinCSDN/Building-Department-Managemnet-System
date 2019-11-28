using System;

public class P_MW_SCANNED_DOCUMENT
{
    public string UUID { get; set; }
    public string DSN_ID { get; set; }
    public string DSN_SUB { get; set; }
    public string FILE_PATH { get; set; }
    public Nullable<decimal> PAGE_COUNT { get; set; }
    public string DOCUMENT_TYPE { get; set; }
    public System.DateTime SCAN_DATE { get; set; }
    public string FILE_SIZE_CODE { get; set; }
    public string CREATED_BY { get; set; }
    public System.DateTime CREATED_DATE { get; set; }
    public string MODIFIED_BY { get; set; }
    public System.DateTime MODIFIED_DATE { get; set; }
    public string SUBMIT_TYPE { get; set; }
    public string FORM_TYPE { get; set; }
    public string DOC_TITLE { get; set; }
    public string RELATIVE_FILE_PATH { get; set; }
    public string STATUS_CODE { get; set; }
    public string FOLDER_TYPE { get; set; }
    public string RRM_SYN_STATUS { get; set; }

    public virtual P_MW_DSN P_MW_DSN { get; set; }
}
