<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Splash.aspx.cs" Inherits="Splash" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=10,IE=edge,chrome=1">
    <link href="../Images/favicon.ico" rel="shortcut icon" type="image/x-icon" />

    <title>Merico</title>
    <!-- Latest compiled and minified CSS -->
    <link href="CSS/style.css" rel="stylesheet" type="text/css" />

    <!-- jQuery (necessary for Bootstrap's JavaScript plugins) -->
    <script src="Scripts/jquery-1.12.4.js" type="text/javascript"></script>
    <script src="Scripts/site-custom.js" type="text/javascript"></script>
    <script type="text/javascript">
        setTimeout(function () {
            window.location.href = 'Login.aspx';
        }, 5000);
    </script>
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

        <div class="splash-logo">
            <asp:Image ID="imgLogo1" runat="server" ImageUrl="~/Images/logo.svg" title="logo" />
        </div>
    </form>
</body>
</html>
