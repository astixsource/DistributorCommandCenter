<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ForgetPassword.aspx.cs" Inherits="frmLogin" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=10,IE=edge,chrome=1">
    <link href="../Images/favicon.ico" rel="shortcut icon" type="image/x-icon" />

    <title>Distributor Command Center</title>
    <!-- Latest compiled and minified CSS -->
    <link href="CSS/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="CSS/style.css" rel="stylesheet" type="text/css" />
    <link href="CSS/jquery-ui.css" rel="stylesheet" type="text/css" />

    <!-- jQuery (necessary for Bootstrap's JavaScript plugins) -->
    <script src="Scripts/jquery-1.12.4.js" type="text/javascript"></script>
    <script src="Scripts/site-custom.js" type="text/javascript"></script>

    <!-- WARNING: Respond.js doesn't work if you view the page via file: -->
    <!--[if lt IE 9]>
  <script src="Scripts/html5shiv.min.js"></script>
  <script src="Scripts/respond.min.js"></script>
<![endif]-->
    <script type="text/javascript">
        function fnValidate(ctrl) {
            var flgVaild = true;
            if ($("#txtEmail").val().trim() == "") {
                flgVaild = false;
                $("#dvMessage").html("Please enter your Email-ID. ");
            }
            else if (!$("#txtEmail").val().match(/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/)) {
                flgVaild = false;
                $("#dvMessage").html("Please enter valid Email-ID. ");
            }
            return flgVaild;
        }

        function fnLogin() {
            window.location.href = "frmLogin.aspx";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="full-background">
            <img src="Images/bg.jpg" class="bg-img" />
        </div>
        <div class="loginfrm cls-4" style="max-width: 440px;">
            <div class="login-box">
                <div class="login-logo">
                    <asp:Image ID="imgLogo1" Visible="false" runat="server" ImageUrl="~/Images/P_G_logo.svg" title="logo" />
                </div>
                <div class="login-box-msg">
                    <h3 class="title" id="lblTittle" runat="server"></h3>
                </div>
                <div style="color: #777; font-weight: 600; font-size: 1rem; text-align: center; padding-bottom: 10px;">Please Provide your Registered Email-id.<br />
                    Password reset link will be shared on the same.</div>
                <div class="login-box-body clearfix">
                    <div class="input-group frm-group-txt">
                        <div class="input-group-prepend">
                            <span class="input-group-text"><i class="fa fa-envelope"></i></span>
                        </div>
                        <asp:TextBox ID="txtEmail" runat="server" class="form-control" autocomplete="off" placeholder="Register Email ID"></asp:TextBox>
                    </div>
                    <asp:Button ID="btnSubmit" Text="Submit" CssClass="btns btn-submit w-100" OnClientClick="return fnValidate();" OnClick="btnSubmit_Click" runat="server"></asp:Button>
                    <div class="bottom-text" style="margin-top: 5px; text-align: right;">
                        <span class="forgotpwd"><a href="#" onclick="fnLogin()">Back to Sign-In</a> ?</span>
                    </div>
                </div>
                <div class="text-center">
                    <div id="dvMessage" runat="server" class="m-4 text-danger" style="margin-top: 0 !important;"></div>
                </div>
            </div>
            <div class="login-box alt">
                <div class="toggle"></div>
            </div>

            <div class="login-box"></div>
        </div>

        <input id="hdnFlagType" type="hidden" runat="server" />
    </form>
</body>
</html>
