<%@ Page Title="Registration" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="SITConnect.Registration" ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/Form.css" rel="stylesheet" type="text/css" />

    <%-- DATERANGEPICKER http://www.daterangepicker.com/ --%>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/momentjs/latest/moment.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.min.js"></script>
    <link rel="stylesheet" type="text/css" href="https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.css" />


    <div class="form-content">
        <a href="Default.aspx" class="closeBtn">&times;</a>
        <div class="clearfix">
            <div class="col-md-7 text-col registration-text">
                <div class="text-wrapper">
                    <h1 class="text-title">One Of Us?</h1>
                    <p class="text-subtitle">If you have already joined us with an account, just login!<br />We've missed you...</p>
                    <br />
                    <asp:LinkButton runat="server" CssClass="btn-rect" OnClick="btn_login_click"><span>LOGIN</span></asp:LinkButton>
                </div>
            </div>
            <div class="col-md-5 form-col registration-col">
                <div class="form-wrapper registration-wrapper">
                    <h1 class="form-title">Create Account</h1>
                    <%if (Page.Items["errorMsg"] != null) {%>
                    <div class="alert alert-danger fade in">
                      <b>ERROR! </b><%: Page.Items["errorMsg"]%>
                    </div>
                    <%} else {%>
                    <br />
                    <%}%>
                    <div class="form-group">
                        <asp:Label runat="server" class="text-label" AssociatedControlId="tb_FirstName">First Name</asp:Label><br />
                        <asp:Textbox runat="server" class="text-input text-input-firsthalf" type="text" id="tb_FirstName" placeholder="Enter your first name" required="required" />
                        <i class="fas fa-user text-icon text-icon-half"></i>
                        <asp:Label runat="server" class="text-label text-label-half" AssociatedControlId="tb_LastName">Last Name</asp:Label>
                        <asp:Textbox runat="server" class="text-input text-input-secondhalf" type="text" id="tb_LastName" placeholder="Enter your last name" required="required" />
                        <br /><br />
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" class="text-label" AssociatedControlId="tb_EmailAddress">Email Address</asp:Label><br />
                        <asp:Textbox runat="server" class="text-input" type="email" id="tb_EmailAddress" placeholder="Enter your email address" required="required" pattern="[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$" />
                        <i class="far fa-envelope text-icon"></i>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" class="text-label" AssociatedControlId="tb_Password">Password</asp:Label><br />
                        <asp:Textbox runat="server" data-toggle="tooltip" data-html="true" data-placement="left" class="text-input" type="password" id="tb_Password" placeholder="Enter your password" autocomplete="new-password" onkeyup="javascript:validate();" required="required" />
                        <i class="fas fa-lock text-icon"></i>
                        <%--<asp:Label runat="server" ID="Password_error" CssClass="error-label"></asp:Label>--%>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" class="text-label" AssociatedControlId="tb_ConfirmPassword">Repeat Password</asp:Label><br />
                        <asp:Textbox runat="server" data-toggle="tooltip" data-html="true" data-placement="left" class="text-input" type="password" id="tb_ConfirmPassword" placeholder="Re-enter your password" autocomplete="new-password" onkeyup="javascript:validateRepeat();" required="required" />
                        <i class="fas fa-lock text-icon"></i>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" class="text-label" AssociatedControlId="tb_PhoneNumber">Phone Number</asp:Label><br />
                        <asp:Textbox runat="server"  class="text-input text-input-firsthalf" type="text" id="tb_PhoneNumber" placeholder="Enter your phone number" MaxLength="10" required="required" pattern="[0-9]{8,10}" />
                        <i class="fas fa-phone-alt text-icon text-icon-half"></i>
                        <asp:Label runat="server" class="text-label text-label-half" AssociatedControlId="tb_DOB">Date of Birth</asp:Label>
                        <asp:Textbox runat="server"  class="text-input text-input-secondhalf text-input-secondhalf-icon" type="text" id="tb_DOB" placeholder="Enter your birthdate" required="required" />
                        <i class="far fa-calendar-alt text-icon text-icon-secondhalf"></i>
                        <br /><br />
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" class="text-label" AssociatedControlId="tb_CardNumber">Card Number</asp:Label><br />
                        <asp:TextBox class="text-input editable" data-toggle="tooltip" data-html="true" data-placement="bottom" type="text" runat="server" ID="tb_CardNumber" placeholder="Card number" autocomplete="new-password" MaxLength="19" pattern="[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}" required="required" />
                        <i class="far fa-credit-card text-icon"></i>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" class="text-label" AssociatedControlId="tb_CardExpDate">Expiration Date</asp:Label><br />
                        <asp:TextBox class="text-input text-input-ced editable" type="text" runat="server" ID="tb_CardExpDate" placeholder="Card's expiration date" autocomplete="new-password" MaxLength="5" pattern="[0-9]{2}/[0-9]{2}" required="required" />
                        <i class="far fa-calendar-alt text-icon text-icon-half"></i>
                        <asp:Label runat="server" class="text-label text-label-cvc" AssociatedControlId="tb_CardVerification">CVC / CVV</asp:Label>
                        <asp:Textbox runat="server"  class="text-input text-input-cvc text-input-secondhalf-icon editable" type="text" id="tb_CardVerification" placeholder="Card's CVC / CVV" MaxLength="3" required="required" />
                        <i class="fas fa-shield-alt text-icon text-icon-cvc"></i>
                        <br /><br />
                    </div>
                    <br />
                    <asp:Button runat="server" CssClass="btn-rect btn-rect-full" ID="btn_submit" OnClick="btn_submit_click" Text="REGISTER" />
                    <br /><br />
                    <a href="Default.aspx" class="backBtn"><i class="fas fa-arrow-left"></i> <span>click to go back and shop</span></a>
                    <%--<asp:LinkButton runat="server" CssClass="btn-rect btn-rect-full" ID="btn_submit" OnClientClick="btn_submit_click"><span>REGISTER <i class="fas fa-chevron-right"></i></span></asp:LinkButton>--%>
                </div>
            </div>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            $('input[id="<%=tb_DOB.ClientID%>"]').daterangepicker({
                opens: 'right',
                drops: 'up',
                autoApply: true,
                singleDatePicker: true,
                showDropdowns: true,
                maxDate: new Date(),
                locale: {
                    format: 'DD/MM/YYYY'
                }
            }, function (start, end, label) {
            });

            window.onscroll = function () {
                $('input[id="<%=tb_DOB.ClientID%>"]').data('daterangepicker').hide();
                document.getElementById("<%=tb_DOB.ClientID%>").blur();
            }
        });

        function validate() {
            var element = document.getElementById("<%=tb_Password.ClientID%>");
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
                $("#<%=tb_Password.ClientID%>").attr('data-original-title', "<b>Password must contain:</b><br>" + errorMsg)
                    .tooltip('fixTitle')
                    .tooltip('show')
                    .unbind();
            }
            else {
                element.setCustomValidity("");
                $("#<%=tb_Password.ClientID%>").attr('data-original-title', "")
                    .tooltip('fixTitle')
                    .tooltip('hide');
            }

            if (document.getElementById("<%=tb_ConfirmPassword.ClientID%>").value.length > 0) {
                validateRepeat();
            }
        }

        function validateRepeat() {
            var element1 = document.getElementById("<%=tb_Password.ClientID%>");
            var str1 = element1.value;
            var element2 = document.getElementById("<%=tb_ConfirmPassword.ClientID%>");
            var str2 = element2.value;
            var errorMsg = "";

            if (str2 != str1) {
                element2.setCustomValidity("Passwords mismatch.");
                errorMsg += "Passwords do not match!";
            }

            // ERROR TOOLTIP
            if (errorMsg.length > 1 && str2.length > 0) {
                $("#<%=tb_ConfirmPassword.ClientID%>").attr('data-original-title', "<b>" + errorMsg + "<br>")
                    .tooltip('fixTitle')
                    .tooltip('show')
                    .unbind();
            }
            else {
                element2.setCustomValidity("");
                $("#<%=tb_ConfirmPassword.ClientID%>").attr('data-original-title', "")
                    .tooltip('fixTitle')
                    .tooltip('hide');
            }
        }


        // CARD
        $('#<%=tb_CardNumber.ClientID%>').keypress(function () {
            var rawNos = $(this).val().replace(/-/g, '');
            var cardLen = rawNos.length;
            if (cardLen !== 0 && cardLen <= 12 && cardLen % 4 == 0) {
                $(this).val($(this).val() + '-');
            }

            var str = $(this).val().replace(/-/g, "");
            console.log(str);
        });

        $('#<%=tb_CardExpDate.ClientID%>').keypress(function () {
            var cardLen = $(this).val().length;
            if (cardLen !== 0 && cardLen <= 2 && cardLen % 2 == 0) {
                $(this).val($(this).val() + '/');
            }
        });
    </script>
</asp:Content>
