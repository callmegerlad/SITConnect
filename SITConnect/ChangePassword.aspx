<%@ Page Title="Change Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="SITConnect.ChangePassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/Form.css" rel="stylesheet" type="text/css" />
    <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>

    <div class="form-content">
        <a href="Default.aspx" class="closeBtn closeBtn-inverse">&times;</a>
        <div class="clearfix">
            <div class="col-md-5 form-col login-col">
                <div class="form-wrapper login-wrapper" style="top:15vh;">
                    <h1 class="form-title">Change Password</h1>
                    <%if (Page.Items["passwordError"] != null) {%>
                    <div class="alert alert-danger fade in">
                        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                        <b>ERROR! </b><%: Page.Items["passwordError"]%>
                    </div>
                    <%}%>
                    <br />
                    <div class="form-group">
                        <label class="text-label" for="tb_PreviousPassword">Previous Password</label><br />
                        <asp:Textbox runat="server" class="text-input" type="password" id="tb_PreviousPassword" placeholder="Enter your previous password" autocomplete="new-password" required="required" />
                        <i class="fas fa-unlock text-icon"></i>
                    </div>
                    <div class="form-group">
                        <label class="text-label" for="tb_NewPassword">New Password</label><br />
                        <asp:Textbox runat="server" data-toggle="tooltip" data-html="true" data-placement="right" class="text-input" type="password" id="tb_NewPassword" placeholder="Enter your new password" autocomplete="new-password" required="required" onkeyup="validate();" />
                        <i class="fas fa-lock text-icon"></i>
                    </div>
                    <div class="form-group">
                        <label class="text-label" for="tb_NewPasswordConfirm">Confirm New Password</label><br />
                        <asp:Textbox runat="server" data-toggle="tooltip" data-html="true" data-placement="right" class="text-input" type="password" id="tb_NewPasswordConfirm" placeholder="Enter your new password again" autocomplete="new-password" required="required" onkeyup="validateChangePassword();" />
                        <i class="fas fa-lock text-icon"></i>
                    </div>
                    <br />
                    <asp:Button runat="server" CssClass="btn-rect btn-rect-full" ID="btn_submit" OnClick="btn_submit_click" Text="CHANGE PASSWORD" />
                    <br /><br />
                    <a href="javascript:logout();" class="backBtn"><i class="fas fa-arrow-left"></i> <span>click to logout</span></a>
                    <%--<asp:LinkButton runat="server" CssClass="btn-rect btn-rect-full" ID="btn_submit"><span>LOGIN <i class="fas fa-chevron-right"></i></span></asp:LinkButton>--%>
                </div>
            </div>
            <div class="col-md-7 text-col login-text">
                <div class="text-wrapper">
                    <h1 class="text-title">Oops!</h1>
                    <p class="text-subtitle">Looks like your old password has reached its maximum age.<br />To continue, please change your password.</p>
                    <br />
                    <%--<asp:LinkButton runat="server" CssClass="btn-rect" OnClick="btn_register"><span>REGISTER</span></asp:LinkButton>--%>
                </div>
            </div>
        </div>
    </div>
    <div hidden>
        <asp:Button runat="server" ID="logout_btn" OnClick="logout_btn_Click" UseSubmitBehavior="False" />
    </div>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LeAKSQaAAAAAAfKMKanuxZabtNvpSiOMVtHtMyj', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });

        function logout() {
            var btn = document.getElementById("<%=logout_btn.ClientID %>");
            btn.click();
        }


        // New Password validation
        function validate() {
            var element = document.getElementById("<%=tb_NewPassword.ClientID%>");
            var str = element.value;
            var errorMsg = "";

            if (str.length < 8) {
                element.setCustomValidity("Password must be at least 8 characters in length.");
                errorMsg += "- at least 8 characters in length.<br>";
            }

            if (str.search(/[0-9]/) == -1) {
                element.setCustomValidity("Password must contain 1 or more numbers.");
                errorMsg += "- 1 or more numbers.<br>";
            }

            if (str.search(/[a-z]/) == -1 || str.search(/[A-Z]/) == -1) {
                element.setCustomValidity("Password must have upper and lowercase letters.");
                errorMsg += "- upper and lowercase letters.<br>";
            }

            if (str.search(/[^a-zA-Z0-9]/) == -1) {
                element.setCustomValidity("Password must have at least 1 special character.");
                errorMsg += "- at least 1 special character.<br>";
            }

            // ERROR TOOLTIP
            if (errorMsg.length > 1) {
                $("#<%=tb_NewPassword.ClientID%>").attr('data-original-title', "<b>Password must contain:</b><br>" + errorMsg)
                    .tooltip('fixTitle')
                    .tooltip('show')
                    .unbind();
            }
            else {
                element.setCustomValidity("");
                $("#<%=tb_NewPassword.ClientID%>").attr('data-original-title', "")
                    .tooltip('fixTitle')
                    .tooltip('hide');
            }

            // validate confirm password
            validateChangePassword();
        }

        function validateChangePassword() {
            var element1 = document.getElementById("<%=tb_NewPassword.ClientID%>");
            var str1 = element1.value;
            var element2 = document.getElementById("<%=tb_NewPasswordConfirm.ClientID%>");
            var str2 = element2.value;
            var errorMsg = "";

            if (str2 != str1) {
                element2.setCustomValidity("New passwords mismatch.");
                errorMsg += "New passwords do not match!";
            }
            else {
                element2.setCustomValidity("");
            }

            // ERROR TOOLTIP
            if (errorMsg.length > 0 && str2.length > 0) {
                $("#<%=tb_NewPasswordConfirm.ClientID%>").attr('data-original-title', "<b>" + errorMsg + "<br>")
                    .tooltip('fixTitle')
                    .tooltip('show')
                    .unbind();
            }
            else {
                element2.setCustomValidity("");
                $("#<%=tb_NewPasswordConfirm.ClientID%>").attr('data-original-title', "")
                    .tooltip('fixTitle')
                    .tooltip('hide');
            }
        }
    </script>
</asp:Content>
