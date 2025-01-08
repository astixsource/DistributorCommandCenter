<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PasswordReset.aspx.cs" Inherits="frmLogin" %>

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
        var flgPasswordValidate = 1;

        function fnPasswordValidate(ctrl) {
            flgPasswordValidate = 0;
            var pass = $(ctrl).val();

            if (pass.length > 7 && pass.length < 16) {
                $("#divPassInstructionBlock").find("span[iden='length']").eq(0).addClass("pass-valid");
                $("#divPassInstructionBlock").find("span[iden='length']").eq(0).find("i").eq(0).removeClass("fa-times").addClass("fa-check");
            }
            else {
                flgPasswordValidate = 1;
                $("#divPassInstructionBlock").find("span[iden='length']").eq(0).removeClass("pass-valid");
                $("#divPassInstructionBlock").find("span[iden='length']").eq(0).find("i").eq(0).removeClass("fa-check").addClass("fa-times");
            }

            if (pass.match(/[a-zA-Z]/g)) {
                $("#divPassInstructionBlock").find("span[iden='letter']").eq(0).addClass("pass-valid");
                $("#divPassInstructionBlock").find("span[iden='letter']").eq(0).find("i").eq(0).removeClass("fa-times").addClass("fa-check");
            }
            else {
                flgPasswordValidate = 1;
                $("#divPassInstructionBlock").find("span[iden='letter']").eq(0).removeClass("pass-valid");
                $("#divPassInstructionBlock").find("span[iden='letter']").eq(0).find("i").eq(0).removeClass("fa-check").addClass("fa-times");
            }

            if (pass.match(/[0-9]/g)) {
                $("#divPassInstructionBlock").find("span[iden='number']").eq(0).addClass("pass-valid");
                $("#divPassInstructionBlock").find("span[iden='number']").eq(0).find("i").eq(0).removeClass("fa-times").addClass("fa-check");
            }
            else {
                flgPasswordValidate = 1;
                $("#divPassInstructionBlock").find("span[iden='number']").eq(0).removeClass("pass-valid");
                $("#divPassInstructionBlock").find("span[iden='number']").eq(0).find("i").eq(0).removeClass("fa-check").addClass("fa-times");
            }

            if (pass.match(/(?=.*[!@#$%^&*])/g)) {
                $("#divPassInstructionBlock").find("span[iden='splchar']").eq(0).addClass("pass-valid");
                $("#divPassInstructionBlock").find("span[iden='splchar']").eq(0).find("i").eq(0).removeClass("fa-times").addClass("fa-check");
            }
            else {
                flgPasswordValidate = 1;
                $("#divPassInstructionBlock").find("span[iden='splchar']").eq(0).removeClass("pass-valid");
                $("#divPassInstructionBlock").find("span[iden='splchar']").eq(0).find("i").eq(0).removeClass("fa-check").addClass("fa-times");
            }
        }

        function fnValidate(ctrl) {
            var flgVaild = true;
            if ($("#hdnCode").val() == "0") {
                flgVaild = false;
                $("#dvMessage").html("Link Expired. Kindly re-try by clicking 'Forget your Password' !");
            }
            else if ($("#txtNewPassword").val().trim() == "") {
                flgVaild = false;
                $("#dvMessage").html("Please enter the New Password !");
            }
            else if ($("#txtRePassword").val().trim() == "") {
                flgVaild = false;
                $("#dvMessage").html("Please re-enter the New Password for Confirmation !");
            }
            else if ($("#txtRePassword").val().trim() != $("#txtNewPassword").val().trim()) {
                flgVaild = false;
                $("#dvMessage").html("New Password & Confirm Password doesn't Match !");
            }
            else if (flgPasswordValidate == 1) {
                flgVaild = false;
                $("#dvMessage").html("Password must validate, all the Password Instructions !");
            }

            return flgVaild;
        }

        function fnLogin() {
            window.location.href = "frmLogin.aspx";
        }
    </script>

    
    <style type="text/css">
        .pass-title {
            color: #000000;
            font-size: 0.86rem;
            font-weight: 700;
        }
        .pass-lbl {
            color: #d70000;
            font-size: 0.8rem;
            font-weight: 600;
            margin-left: 16px;
        }

        .pass-valid {
            color: #00a600 !important;
        }
    </style>
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
                <div class="login-box-body clearfix">
                    <div class="input-group frm-group-txt">
                        <div class="input-group-prepend">
                            <span class="input-group-text"><i class="fa fa-key"></i></span>
                        </div>
                        <asp:TextBox ID="txtNewPassword" runat="server" class="form-control"  autocomplete="off" onkeyup="fnPasswordValidate(this);"  placeholder="New Password"></asp:TextBox>
                    </div>
                    <div class="input-group frm-group-txt">
                        <div class="input-group-prepend">
                            <span class="input-group-text"><i class="fa fa-key"></i></span>
                        </div>
                        <asp:TextBox ID="txtRePassword" runat="server" class="form-control"  autocomplete="off" placeholder="Confirm Password"></asp:TextBox>
                    </div>                    
                    <div class="input-group mb-4" id="divPassInstructionBlock">
                        <span class="pass-title">Password Instructions :</span>
                        <span iden="length" class="pass-lbl"><i class="fa fa-times"></i> Length must be in between 8 to 15 characters.</span>
                        <span iden="letter" class="pass-lbl"><i class="fa fa-times"></i> Must have one or more letter.</span>
                        <span iden="number" class="pass-lbl"><i class="fa fa-times"></i> Must have one or more number.</span>
                        <span iden="splchar" class="pass-lbl"><i class="fa fa-times"></i> Must have one or more special character (!@#$%^&*).</span>
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

        <input id="hdnCode" type="hidden" runat="server" />
    </form>
</body>
</html>
