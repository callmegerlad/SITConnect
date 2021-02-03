<%@ Page Title="Page Error 500" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Error500.aspx.cs" Inherits="SITConnect.CustomError.Error500" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../Content/Error.css" rel="stylesheet" type="text/css" />

    <div class="oops">
        <div class="oops-heading">
            <h1>Oops!</h1>
            <h3>An internal server error has occurred!</h3>
        </div>
        <p><b>Error Code:</b> 500</p>
        <p>We apologise for the inconvenience. We had some technical problems during your last operation. Please try again.</p>
        <a href="../Default" class="btn-rect"><span>Back to Home</span></a>
    </div>
</asp:Content>
