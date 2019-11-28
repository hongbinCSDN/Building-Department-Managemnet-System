﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MWMS2.PrintBarcodeServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PrintBarcodeServiceReference.PrinterWebSerSoap")]
    public interface PrinterWebSerSoap {
        
        // CODEGEN: Generating message contract since element name DSN from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/PrintBarcodeLabel", ReplyAction="*")]
        MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelResponse PrintBarcodeLabel(MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/PrintBarcodeLabel", ReplyAction="*")]
        System.Threading.Tasks.Task<MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelResponse> PrintBarcodeLabelAsync(MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class PrintBarcodeLabelRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="PrintBarcodeLabel", Namespace="http://tempuri.org/", Order=0)]
        public MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelRequestBody Body;
        
        public PrintBarcodeLabelRequest() {
        }
        
        public PrintBarcodeLabelRequest(MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class PrintBarcodeLabelRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string DSN;
        
        public PrintBarcodeLabelRequestBody() {
        }
        
        public PrintBarcodeLabelRequestBody(string DSN) {
            this.DSN = DSN;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class PrintBarcodeLabelResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="PrintBarcodeLabelResponse", Namespace="http://tempuri.org/", Order=0)]
        public MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelResponseBody Body;
        
        public PrintBarcodeLabelResponse() {
        }
        
        public PrintBarcodeLabelResponse(MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class PrintBarcodeLabelResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string PrintBarcodeLabelResult;
        
        public PrintBarcodeLabelResponseBody() {
        }
        
        public PrintBarcodeLabelResponseBody(string PrintBarcodeLabelResult) {
            this.PrintBarcodeLabelResult = PrintBarcodeLabelResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface PrinterWebSerSoapChannel : MWMS2.PrintBarcodeServiceReference.PrinterWebSerSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PrinterWebSerSoapClient : System.ServiceModel.ClientBase<MWMS2.PrintBarcodeServiceReference.PrinterWebSerSoap>, MWMS2.PrintBarcodeServiceReference.PrinterWebSerSoap {
        
        public PrinterWebSerSoapClient() {
        }
        
        public PrinterWebSerSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PrinterWebSerSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PrinterWebSerSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PrinterWebSerSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelResponse MWMS2.PrintBarcodeServiceReference.PrinterWebSerSoap.PrintBarcodeLabel(MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelRequest request) {
            return base.Channel.PrintBarcodeLabel(request);
        }
        
        public string PrintBarcodeLabel(string DSN) {
            MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelRequest inValue = new MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelRequest();
            inValue.Body = new MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelRequestBody();
            inValue.Body.DSN = DSN;
            MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelResponse retVal = ((MWMS2.PrintBarcodeServiceReference.PrinterWebSerSoap)(this)).PrintBarcodeLabel(inValue);
            return retVal.Body.PrintBarcodeLabelResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelResponse> MWMS2.PrintBarcodeServiceReference.PrinterWebSerSoap.PrintBarcodeLabelAsync(MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelRequest request) {
            return base.Channel.PrintBarcodeLabelAsync(request);
        }
        
        public System.Threading.Tasks.Task<MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelResponse> PrintBarcodeLabelAsync(string DSN) {
            MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelRequest inValue = new MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelRequest();
            inValue.Body = new MWMS2.PrintBarcodeServiceReference.PrintBarcodeLabelRequestBody();
            inValue.Body.DSN = DSN;
            return ((MWMS2.PrintBarcodeServiceReference.PrinterWebSerSoap)(this)).PrintBarcodeLabelAsync(inValue);
        }
    }
}