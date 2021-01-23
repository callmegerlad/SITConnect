<%@ Page Title="Page Error" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Error404.aspx.cs" Inherits="SITConnect.CustomError.Error404" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../Content/Error.css" rel="stylesheet" type="text/css" />

    <div class="oops">
        <div class="oops-heading">
            <h1>Oops!</h1>
            <h3>We can't seem to find the page you're looking for!</h3>
        </div>
        <p><b>Error Code:</b> 404</p>
        <p>We apologise for the inconvenience. It seems like you're trying to access a page that has either been removed or never existed in the first place.</p>
        <a href="../Default" class="btn-rect"><span>Back to Home</span></a>
    </div>
</asp:Content>
