<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserProfile.aspx.cs" Inherits="SITConnect.UserProfile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/User.css" rel="stylesheet" type="text/css" />

    <%-- DATERANGEPICKER http://www.daterangepicker.com/ --%>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/momentjs/latest/moment.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.min.js"></script>
    <link rel="stylesheet" type="text/css" href="https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.css" />


    <div class="clearfix">
        <div class="col-md-4 profile-left">
            <div class="profile-banner">
                <div class="profile-banner-bg"></div>
                <div class="profile-nameplate">
                    <div class="user-avatar">
                        <p id="user-avatar"><asp:Literal runat="server" Id="userAvatar"></asp:Literal></p>
                    </div>
                    <div class="user-nameplate">
                        <h1 id="user-name"><asp:Literal runat="server" Id="userName"></asp:Literal></h1>
                        <h5 id="user-email"><asp:Literal runat="server" Id="userEmail"></asp:Literal></h5>
                    </div>
                </div>
                <div class="profile-nav">
                    <ul>
                        <li><a id="tab-1" class="tab active" href="javascript:switchTab(1);javascript:hideMsg();"><i class="fas fa-info-circle"></i> Account Information</a></li>
                        <li><a id="tab-2" class="tab" href="javascript:switchTab(2);javascript:hideMsg();"><i class="fas fa-unlock"></i> Change Password</a></li>
                        <li><a id="tab-3" class="tab" href="javascript:switchTab(3);javascript:hideMsg();"><i class="far fa-credit-card"></i> Billing Information</a></li>
                        <li><a id="tab-4" class="tab" href="javascript:switchTab(4);javascript:hideMsg();"><i class="fas fa-history"></i> Order History</a></li>
                        <li><a id="tab-5" class="tab" href="javascript:switchTab(5);javascript:hideMsg();"><i class="fas fa-sign-out-alt"></i> Logout</a></li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="col-md-8 profile-right">
            <div class="profile-content">
                <h1>My Profile</h1>
                <div class="pc-1 pc active">
                    <h3>Account Information</h3>
                    <%if (Page.Items["updateMsg"] != null) {%>
                    <div class="alert alert-success fade in">
                        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                        <b>SUCCESS! </b><%: Page.Items["updateMsg"]%>
                    </div>
                    <%}%>
                    <div class="pc-section">
                        <div class="form-edit">
                            <asp:Label runat="server" class="text-label" AssociatedControlId="tb_FirstName_Edit">First Name</asp:Label><br />
                            <asp:Textbox runat="server" class="text-input text-input-firsthalf" type="text" id="tb_FirstName_Edit" placeholder="Enter your first name" required="required" />
                            <i class="fas fa-user text-icon text-icon-half"></i>
                            <asp:Label runat="server" class="text-label text-label-half" AssociatedControlId="tb_LastName_Edit">Last Name</asp:Label>
                            <asp:Textbox runat="server" class="text-input text-input-secondhalf" type="text" id="tb_LastName_Edit" placeholder="Enter your last name" required="required" />
                            <br /><br />
                        </div>
                        <%--<div class="form-edit">
                            <asp:Label runat="server" class="text-label" AssociatedControlId="tb_EmailAddress_Edit">Email Address</asp:Label><br />
                            <asp:TextBox class="text-input" type="email" runat="server" ID="tb_EmailAddress_Edit" placeholder="Enter your email address" autocomplete="off"></asp:TextBox>
                            <i class="far fa-envelope text-icon"></i>
                        </div>--%>
                        <div class="form-edit">
                            <asp:Label runat="server" class="text-label" AssociatedControlId="tb_PhoneNumber_Edit">Phone Number</asp:Label><br />
                            <asp:TextBox class="text-input text-input-firsthalf" type="text" runat="server" ID="tb_PhoneNumber_Edit" placeholder="Enter your email address" MaxLength="10" autocomplete="off"></asp:TextBox>
                            <i class="fas fa-phone-alt text-icon text-icon-half"></i>
                            <asp:Label runat="server" class="text-label text-label-half" AssociatedControlId="tb_DOB_Edit">Date of Birth</asp:Label>
                            <asp:Textbox runat="server"  class="text-input text-input-secondhalf text-input-secondhalf-icon" type="text" id="tb_DOB_Edit" placeholder="Enter your birthdate" required="required" />
                            <i class="far fa-calendar-alt text-icon text-icon-secondhalf"></i>
                            <br /><br />
                        </div>
                        <button type='button' id="editBtn" class="btn-rect saveBtn" onclick="editable();"><span><i class="far fa-edit" id="editIcon"></i> <b id="editText" class="saveText">Edit</b></span></button>
                        <div hidden>
                            <asp:Button runat="server" ID="btnSubmit" OnClick="btnSubmit_Click" />
                        </div>
                    </div>
                </div>
                <div class="pc-2 pc">
                    <h3>Change Password</h3>
                    <%if (Page.Items["updateMsg"] != null) {%>
                    <div class="alert alert-success fade in">
                        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                        <b>SUCCESS! </b><%: Page.Items["updateMsg"]%>
                    </div>
                    <%}%>
                    <%if (Page.Items["errorMsg"] != null) {%>
                    <div class="alert alert-danger fade in">
                        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                        <b>ERROR! </b><%: Page.Items["errorMsg"]%>
                    </div>
                    <%}%>
                    <div class="form-edit">
                        <asp:Label runat="server" class="text-label" AssociatedControlID="tb_CurrentPassword">Current Password</asp:Label>
                        <asp:TextBox runat="server" class="text-input editable" TextMode="Password" ID="tb_CurrentPassword" placeholder="Enter your current password" autocomplete="new-password"></asp:TextBox>
                        <i class="fas fa-unlock text-icon"></i>
                    </div>
                    <div class="form-edit">
                        <asp:Label runat="server" class="text-label" AssociatedControlID="tb_NewPassword">New Password</asp:Label>
                        <asp:TextBox runat="server" data-toggle="tooltip" data-html="true" data-placement="left" class="text-input editable" TextMode="Password" ID="tb_NewPassword" placeholder="Enter your new password" autocomplete="new-password" onkeyup="validate();"></asp:TextBox>
                        <i class="fas fa-lock text-icon"></i>
                    </div>
                    <div class="form-edit">
                        <asp:Label runat="server" class="text-label" AssociatedControlID="tb_NewPasswordConfirm">Confirm New Password</asp:Label>
                        <asp:TextBox runat="server" data-toggle="tooltip" data-html="true" data-placement="left" class="text-input editable" TextMode="Password" ID="tb_NewPasswordConfirm" placeholder="Enter your new password again" autocomplete="new-password" onkeyup="validateChangePassword();"></asp:TextBox>
                        <i class="fas fa-lock text-icon"></i>
                    </div>
                    <button style="width:250px;" type='button' class="btn-rect saveBtn" onclick="changePassword();"><span><i class="far fa-check-circle" id="changePasswordIcon"></i> <b id="changePasswordText" class="saveText">Change Password</b></span></button>
                    <div hidden>
                        <asp:Button runat="server" ID="changePassword" OnClick="changePassword_Click" />
                    </div>
                </div>
                <div class="pc-3 pc">
                    <h3>Billing Information</h3>
                    <%if (Page.Items["updateMsg"] != null) {%>
                    <div class="alert alert-success fade in">
                        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                        <b>SUCCESS! </b><%: Page.Items["updateMsg"]%>
                    </div>
                    <%}%>
                    <%if (Page.Items["errorMsg"] != null) {%>
                    <div class="alert alert-danger fade in">
                        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                        <b>ERROR! </b><%: Page.Items["errorMsg"]%>
                    </div>
                    <%}%>
                    <div class="form-edit">
                        <asp:Label runat="server" class="text-label" AssociatedControlId="tb_CardNumber">Card Number</asp:Label><br />
                        <asp:TextBox class="text-input editable" data-toggle="tooltip" data-html="true" data-placement="bottom" type="text" runat="server" ID="tb_CardNumber" placeholder="Card number" autocomplete="new-password" MaxLength="19" pattern="[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}"/>
                        <i class="far fa-credit-card text-icon"></i>
                    </div>
                    <div class="form-edit">
                        <asp:Label runat="server" class="text-label" AssociatedControlId="tb_CardExpDate">Expiration Date</asp:Label><br />
                        <asp:TextBox class="text-input text-input-ced editable" type="text" runat="server" ID="tb_CardExpDate" placeholder="Card's expiration date" autocomplete="new-password" MaxLength="5" pattern="[0-9]{2}/[0-9]{2}"/>
                        <i class="far fa-calendar-alt text-icon text-icon-half"></i>
                        <asp:Label runat="server" class="text-label text-label-cvc" AssociatedControlId="tb_CardVerification">CVC / CVV</asp:Label>
                        <asp:Textbox runat="server"  class="text-input text-input-cvc text-input-secondhalf-icon editable" type="text" id="tb_CardVerification" placeholder="Card's CVC / CVV" MaxLength="3" />
                        <i class="fas fa-shield-alt text-icon text-icon-cvc"></i>
                        <br /><br />
                    </div>
                    <button type='button' class="btn-rect saveBtn" onclick="saveCard();"><span><i class="far fa-check-circle" id="saveIcon"></i> <b id="saveText" class="saveText">Save</b></span></button>
                    <div hidden>
                        <asp:Button runat="server" ID="saveCardBtn" OnClick="saveCardBtn_Click" />
                    </div>
                </div>
                <div class="pc-4 pc">
                    <h3>Order History</h3>
                </div>
                <div class="pc-5 pc">
                    <h3>Logout</h3>
                    <h4>Are you sure you want to log out?</h4>
                    <br />
                    <button type='button' id="logoutBtn" class="btn-rect btn-small" onclick="logout();"><span>Yes, logout</span></button>
                    <div hidden>
                        <asp:Button runat="server" ID="btnLogout" Text="Logout" OnClick="logout"/>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <asp:Button ID="HiddenButton" style="display:none;" OnClientClick="return false;" runat="server" Text="Button" />
    <script type="text/javascript">
        $(document).ready(function () {
            load();
            // DateRangePicker
            $('input[id="<%=tb_DOB_Edit.ClientID%>"]').daterangepicker({
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

            // adds dashes
            var str = document.getElementById("<%=tb_CardNumber.ClientID%>").value;
            if (str.length > 0 && !str.includes("-")) {
                var parts = str.match(/.{1,4}/g);
                document.getElementById("<%=tb_CardNumber.ClientID%>").value = parts.join("-");
            }

            // adds slash
            var str = document.getElementById("<%=tb_CardExpDate.ClientID%>").value;
            if (str.length > 0 && !str.includes("/")) {
                var parts = str.match(/.{1,2}/g);
                document.getElementById("<%=tb_CardExpDate.ClientID%>").value = parts.join("/");
            }

            window.onscroll = function () {
                $('input[id="<%=tb_DOB_Edit.ClientID%>"]').data('daterangepicker').hide();
                document.getElementById("<%=tb_DOB_Edit.ClientID%>").blur();
            }
        });

        function load() {
            // readonly text fields, since having readonly attribute on asp button causes postback issues
            $("#<%=tb_FirstName_Edit.ClientID%>").attr("readonly","readonly");
            $("#<%=tb_LastName_Edit.ClientID%>").attr("readonly", "readonly");
            <%--$("#<%=tb_EmailAddress_Edit.ClientID%>").attr("readonly","readonly");--%>
            $("#<%=tb_PhoneNumber_Edit.ClientID%>").attr("readonly", "readonly");
            $("#<%=tb_DOB_Edit.ClientID%>").attr("readonly", "readonly");

            <%if (Page.Items["tabNo"] != null) {%>
            switchTab(<%=Page.Items["tabNo"]%>);
            <%}%>
        }

        function hideMsg() {
            $('.alert').hide();
        }

        function switchTab(tabNo) {
            var all = $(".pc").map(function () {
                $("#tab-" + this.classList[0].slice(-1)).removeClass("active");
                if (this.classList.contains("active")) {
                    this.classList.remove("active");
                }

                if (this.classList.contains("pc-" + tabNo.toString())) {
                    this.classList.add("active");
                    $("#tab-" + tabNo).addClass("active");
                }
            });
            var all = $(".text-input").map(function () {
                document.getElementById(this.id).setCustomValidity("");
            });
            //if (tabNo == "2") {
            //    validate();
            //}
        }

        function editable() {
            // make form fields editable
            $("#<%=tb_FirstName_Edit.ClientID%>").addClass("editable");
            $("#<%=tb_FirstName_Edit.ClientID%>").removeAttr("readonly");

            $("#<%=tb_LastName_Edit.ClientID%>").addClass("editable");
            $("#<%=tb_LastName_Edit.ClientID%>").removeAttr("readonly");

            <%--
            $("#<%=tb_EmailAddress_Edit.ClientID%>").addClass("editable");
            $("#<%=tb_EmailAddress_Edit.ClientID%>").removeAttr("readonly");
            --%>

            $("#<%=tb_PhoneNumber_Edit.ClientID%>").addClass("editable");
            $("#<%=tb_PhoneNumber_Edit.ClientID%>").removeAttr("readonly");

            $("#<%=tb_DOB_Edit.ClientID%>").addClass("editable");
            $("#<%=tb_DOB_Edit.ClientID%>").removeAttr("readonly");

            // change text and icon of edit button
            $("#editText").html("Save");
            $("#editIcon").removeClass("fa-edit");
            $("#editIcon").addClass("fa-check-circle");

            // change button onclick event
            $("#editBtn").attr("onclick","save();");
        }

        function save() {
            // Check input fields validity
            <%--var email = $("#<%=tb_EmailAddress_Edit.ClientID%>").val();--%>
            //var emailRegex = /^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$/;
            //console.log("test " + emailRegex.test(email));

            <%--if (emailRegex.test(email)) {--%>
                // make form fields UNEDITABLE
            $("#<%=tb_FirstName_Edit.ClientID%>").removeClass("editable");
            $("#<%=tb_FirstName_Edit.ClientID%>").attr("readonly", "readonly");

            $("#<%=tb_LastName_Edit.ClientID%>").removeClass("editable");
            $("#<%=tb_LastName_Edit.ClientID%>").attr("readonly", "readonly");

            <%--
                $("#<%=tb_EmailAddress_Edit.ClientID%>").removeClass("editable");
                $("#<%=tb_EmailAddress_Edit.ClientID%>").attr("readonly", "readonly");
            --%>

            $("#<%=tb_PhoneNumber_Edit.ClientID%>").removeClass("editable");
            $("#<%=tb_PhoneNumber_Edit.ClientID%>").attr("readonly", "readonly");

            $("#<%=tb_DOB_Edit.ClientID%>").removeClass("editable");
            $("#<%=tb_DOB_Edit.ClientID%>").attr("readonly", "readonly");

            // change text and icon of edit button
            $("#editText").html("Edit");
            $("#editIcon").addClass("fa-edit");
            $("#editIcon").removeClass("fa-check-circle");

            // change button onclick event
            $("#editBtn").attr("onclick", "editable();");

            // call backend function to save info
            var btnSubmit = document.getElementById("<%=btnSubmit.ClientID %>");
            btnSubmit.click();
            <%--
            } else {
                $("#<%=tb_EmailAddress_Edit.ClientID%>").attr('data-original-title', "Email is invalid!")
                    .tooltip('fixTitle')
                    .tooltip('show')
                    .unbind();
            }
            --%>
        }

        function logout() {
            var btnLogout = document.getElementById("<%=btnLogout.ClientID %>");
            btnLogout.click();
        }

        // Credit Card stuff
        function saveCard() {
            var saveBtn = document.getElementById("<%=saveCardBtn.ClientID %>");
            saveBtn.click();
        }

        $('#<%=tb_CardNumber.ClientID%>').keypress(function () {
            var rawNos = $(this).val().replace(/-/g, '');
            var cardLen = rawNos.length;
            if (cardLen !== 0 && cardLen <= 12 && cardLen % 4 == 0) {
                $(this).val($(this).val() + '-');
            }

            var str = $(this).val().replace(/-/g, "");
            console.log(str);

<%--            var regex = /^6(?:011\d{12}|5\d{14}|4[4-9]\d{13}|22(?:1(?:2[6-9]|[3-9]\d)|[2-8]\d{2}|9(?:[01]\d|2[0-5]))\d{10})$/;
            var isValid = false;

            if (regex.test(str)) {
                isValid = true;
            } else {
                document.getElementById("<%=tb_CardNumber.ClientID%>").setCustomValidity("Invalid Credit Card Number.");
            }--%>
        });

        $('#<%=tb_CardExpDate.ClientID%>').keypress(function () {
            var cardLen = $(this).val().length;
            if (cardLen !== 0 && cardLen <= 2 && cardLen % 2 == 0) {
                $(this).val($(this).val() + '/');
            }
        });

        function changePassword() {
            // call backend function to save info
            validate();
            var btnSubmit = document.getElementById("<%=changePassword.ClientID %>");
            btnSubmit.click();
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
    </script>
</asp:Content>
