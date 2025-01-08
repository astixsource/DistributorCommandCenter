<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmSessionTimeOut.aspx.cs" Inherits="frmSessionTimeOut" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=10,IE=edge,chrome=1">
    <link href="Images/favicon.ico" rel="shortcut icon" type="image/x-icon" />
    <title>Power BI Reports</title>
    <link href="CSS/font-awesome.css" rel="stylesheet" type="text/css" />
    <link href="CSS/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="CSS/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="CSS/style.css" rel="stylesheet" type="text/css" />
    <style>
        .loginfrm1.cls-4 {
    margin-left: auto;
    margin-right: auto;
    animation-name: slideDown;
    -webkit-animation-name: slideDown;
    animation-duration: .6s;
    -webkit-animation-duration: .6s;
    animation-timing-function: ease;
    -webkit-animation-timing-function: ease;
    visibility: visible !important;
}
.loginfrm1 {
    -webkit-background-clip: padding-box;
    background-clip: padding-box;
    max-width: 320px;
    position: relative;
    visibility: hidden;
}
    </style>

    <!-- Latest compiled and minified JavaScript -->

    <script src="Scripts/jquery-3.6.0.js" type="text/javascript"></script>
    <script src="Scripts/popper.min.js" type="text/javascript"></script>
    <script src="Scripts/bootstrap.min.js" type="text/javascript"></script>
    <script src="Scripts/site-custom.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui.js" type="text/javascript"></script>
    <script type="text/javascript">
        function preventBack() { window.history.forward(); }
        setTimeout("preventBack()", 0);
        window.onunload = function () { null };


        $(document).ready(function () {
            $(".loginfrm1").css({
                marginTop: ($(window).height() - $(".loginfrm1").outerHeight())/4 + "px"
            });
        })
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="site-background">
            <img src="Images/bg.jpg" class="bg-img" />
        </div>
        <nav class="navbar fixed-top">
            <div class="container">
                <div class="card_item w-100">
                    <asp:Image ID="imgLogo1" runat="server"   Visible="false" ImageUrl="~/Images/P_G_logo.svg" alt="Logo" class="logo" />
                </div>
            </div>
        </nav>
        <div class="container main-content">
            <div class="loginfrm1 cls-4" style="max-width: 515px; width: 515px">
                <div class="login-box" style="background: transparent !important; border: none !important; box-shadow: none">
                    <div class="mb-3" runat="server" id="divMsgContainer">
                        Oops, Your session has expired!
                    </div>
                    <!-- /.login-box-body -->
                    <div class="logfooter">
                        <asp:Button ID="btnCGSubmit" OnClick="btnCGSubmit_Click" Text="Click To Re-Login" CssClass="btn btn-primary" runat="server" />
                    </div>
                </div>
                <!-- /.login-box -->
            </div>
        </div>

    </form>
</body>
</html>
