<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

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
    <script src="Scripts/jquery-3.6.0.js" type="text/javascript"></script>
    <script src="Scripts/site-custom.js" type="text/javascript"></script>

    <!-- WARNING: Respond.js doesn't work if you view the page via file: -->
    <!--[if lt IE 9]>
  <script src="Scripts/html5shiv.min.js"></script>
  <script src="Scripts/respond.min.js"></script>
<![endif]-->
</head>
<body>
    <form id="form1" runat="server">
        <div class="full-background">
            <img src="Images/login_bg.jpg" class="bg-img" />
        </div>
        <div class="loginfrm cls-4" style="display:none">
            <div class="login-box">
                <div class="login-logo">
                    <asp:Image ID="imgLogo1" runat="server" ImageUrl="~/Images/logo.svg" title="logo" />
                </div>
                <div class="login-box-msg">
                    <h3 class="title">Login To</h3>
                </div>
                <div class="login-box-body clearfix">
                    <div class="input-group frm-group-txt">
                        <div class="input-group-prepend">
                            <span class="input-group-text"><i class="fa fa-user"></i></span>
                        </div>
                        <asp:TextBox ID="txtLoginID" runat="server" class="form-control" autocomplete="off"></asp:TextBox>
                    </div>
                    <div class="input-group frm-group-txt">
                        <div class="input-group-prepend">
                            <span class="input-group-text"><i class="fa fa-lock"></i></span>
                        </div>
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" class="form-control" autocomplete="off"></asp:TextBox>
                    </div>
                    <%-- <asp:Button ID="btnSubmit" Text="Login" CssClass="btns btn-submit w-100" OnClick="btnSubmit_Click" runat="server"></asp:Button>
                    <div class="bottom-text">
                        <span class="forgotpwd">Forgot <a href="#" onclick="fnForgotPwd()">Password</a>?</span>
                    </div>--%>
                </div>
                <div class="text-center">
                    <div id="dvMessage" runat="server" class="mb-2 text-danger"></div>
                </div>
            </div>
            <div class="login-box alt">
                <div class="toggle"></div>
            </div>

            <div class="login-box"></div>
        </div>

        <input id="hdnResolution" type="hidden" runat="server" />
    </form>
</body>
</html>
