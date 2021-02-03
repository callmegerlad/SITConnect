<%@ Page Title="Page Error 403" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Error403.aspx.cs" Inherits="SITConnect.CustomError.Error403" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../Content/Error.css" rel="stylesheet" type="text/css" />

    <div class="oops">
        <div class="oops-heading">
            <h1>Oops!</h1>
            <h3>You do not have the required permissions!</h3>
        </div>
        <p><b>Error Code:</b> 403</p>
        <p>We apologise for the inconvenience. It seems like you're trying to access a page that you do not have the required permissions for.</p>
        <a href="../Default" class="btn-rect"><span>Back to Home</span></a>
    </div>
</asp:Content>
