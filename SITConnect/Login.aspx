<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="SITConnect.Login" ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/Form.css" rel="stylesheet" type="text/css" />
    <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>

    <div class="form-content">
        <a href="Default.aspx" class="closeBtn closeBtn-inverse">&times;</a>
        <div class="clearfix">
            <div class="col-md-5 form-col login-col">
                <div class="form-wrapper login-wrapper">
                    <h1 class="form-title">Login</h1>
                    <%if (Page.Items["errorMsg"] != null) {%>
                    <div class="alert alert-danger fade in">
                        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                        <b>ERROR! </b><%: Page.Items["errorMsg"]%>
                    </div>
                    <%} else {%>
                        <%if (Page.Items["successMsg"] != null) {%>
                        <div class="alert alert-success fade in">
                            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                            <b>CONGRATS! </b><%: Page.Items["successMsg"]%>
                        </div>
                        <%} else {%>
                        <%}%>
                    <%}%>
                    <br />
                    <div class="form-group">
                        <label class="text-label" for="tb_EmailAddress">Email Address</label><br />
                        <asp:Textbox runat="server" class="text-input" type="email" id="tb_EmailAddress" placeholder="Enter your email address" required="required" />
                        <i class="far fa-envelope text-icon"></i>
                    </div>
                    <div class="form-group">
                        <label class="text-label" for="tb_Password">Password</label><br />
                        <asp:Textbox runat="server"  class="text-input" type="password" id="tb_Password" placeholder="Enter your password" autocomplete="new-password" required="required" />
                        <i class="fas fa-lock text-icon"></i>
                    </div>
                    <br />
                    <asp:Button runat="server" CssClass="btn-rect btn-rect-full" ID="btn_submit" OnClick="btn_submit_click" Text="LOGIN" />
                    <br /><br />
                    <a href="Default.aspx" class="backBtn"><i class="fas fa-arrow-left"></i> <span>click to go back and shop</span></a>
                    <%--<asp:LinkButton runat="server" CssClass="btn-rect btn-rect-full" ID="btn_submit"><span>LOGIN <i class="fas fa-chevron-right"></i></span></asp:LinkButton>--%>
                </div>
            </div>
            <div class="col-md-7 text-col login-text">
                <div class="text-wrapper">
                    <h1 class="text-title">New Here?</h1>
                    <p class="text-subtitle">Sign up with us using your email address and start shopping!<br />We're excited to have you!</p>
                    <br />
                    <asp:LinkButton runat="server" CssClass="btn-rect" OnClick="btn_register"><span>REGISTER</span></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LeAKSQaAAAAAAfKMKanuxZabtNvpSiOMVtHtMyj', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</asp:Content>
