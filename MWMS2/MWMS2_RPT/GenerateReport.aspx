<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GenerateReport.aspx.cs" Inherits="MWMS2_RPT._Default" %>

<%@ Register assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <CR:CrystalReportViewer ID="CrystalReportViewer1"  runat="server" ReportSourceID="CrystalReportSource1" AutoDataBind="True"  ToolPanelView="None" />
        <CR:CrystalReportSource ID="CrystalReportSource1" runat="server">

        </CR:CrystalReportSource>
    </div>
</asp:Content>
