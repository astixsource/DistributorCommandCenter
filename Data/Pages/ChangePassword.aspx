<%@ Page Title="" Language="C#" MasterPageFile="~/Data/MasterPage/Site.master" AutoEventWireup="true" CodeFile="ChangePassword.aspx.cs" Inherits="Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <script src="../../BICss/lib/jquery/dist/jquery.min.js"></script>
    <link href="../../BICss/style/style.css" rel="stylesheet" />

    <script type="text/javascript">        
        $(document).ready(function () {
            $("#divRptTitle").html("Change Password");
            $("#divContainer").height($(window).height() - ($("nav.navbar").height() + $("div.card-header").height() + 15) + "px");

            if ($("#ConatntMatter_hdnIsFirst").val() == "1") {
                $("button.menu-button").remove();
                $("a[title='Home']").closest("li").remove();
                $("a[title='Change Password']").closest("li").remove();
            }
        });

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
        function fnChangePassword() {
            if ($("#ConatntMatter_txtNewPassword").val().trim() == "") {
                alert("Please enter the New Password !");
            }
            else if ($("#ConatntMatter_txtRePassword").val().trim() == "") {
                alert("Please re-enter the New Password for Confirmation !");
            }
            else if ($("#ConatntMatter_txtRePassword").val().trim() != $("#ConatntMatter_txtNewPassword").val().trim()) {
                alert("New Password & Confirm Password doesn't Match !");
            }
            else if (flgPasswordValidate == 1) {
                alert("Password must validate, all the Password Instructions !");
            }
            else {
                var UserId = $("#ConatntMatter_hdnUserId").val();
                var NewPassword = $("#ConatntMatter_txtNewPassword").val();

                $("#divloader").show();
                PageMethods.fnSubmit(UserId, NewPassword, fnSubmit_pass, fnFailed);
            }
        }
        function fnSubmit_pass(res) {
            if (res.split("|^|")[0] == "0") {
                alert("Password Changed successfully !");

                window.location.href = "Home.aspx";
            }
            else {
                $("#divloader").hide();
                alert("Error : " + res.split("|^|")[1]);
            }
        }
        function fnFailed(res) {
            $("#divloader").hide();
            alert("Error : " + res);
        }
    </script>
    <style type="text/css">
        .main-content {
            padding-bottom: 0 !important;
        }
        #divPassInstructionBlock i {
            margin-right: 6px;
        }
        #divPassInstructionBlock span {
            width: 100%;
            text-align: left;
            padding-left: 50px;
        }
        #divPassInstructionBlock span:nth-child(1) {
            padding-left: 30px;
        }
    </style>
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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ConatntMatter" runat="Server">
    <div class="card default_bg no_radius m_row pb-0">
        <div class="card-header green_bg pr-0">
            <div class="card-title-heading white_txt" id="divRptTitle" style="display: inline-block; width: 100%; text-align: center;"></div>
            <div class="card-title-actions blue_bg d-none" style="cursor: pointer">
                <img src="../../Images/refresh_icon.svg" onclick="fnRefreshPage()" class="icon" />
            </div>
        </div>
        <div class="text-center" id="divContainer">
            <div style="width: 40%; margin: auto; margin-top: 70px;">
                <div class="input-group frm-group-txt">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="fa fa-key"></i></span>
                    </div>
                    <asp:TextBox ID="txtNewPassword" runat="server" class="form-control" autocomplete="off" onkeyup="fnPasswordValidate(this);" placeholder="New Password"></asp:TextBox>
                </div>
                <div class="input-group frm-group-txt">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="fa fa-key"></i></span>
                    </div>
                    <asp:TextBox ID="txtRePassword" runat="server" class="form-control" autocomplete="off" placeholder="Confirm Password"></asp:TextBox>
                </div>
                <div class="input-group mb-4" id="divPassInstructionBlock">
                    <span class="pass-title">Password Instructions :</span>
                    <span iden="length" class="pass-lbl"><i class="fa fa-times"></i>Length must be in between 8 to 15 characters.</span>
                    <span iden="letter" class="pass-lbl"><i class="fa fa-times"></i>Must have one or more letter.</span>
                    <span iden="number" class="pass-lbl"><i class="fa fa-times"></i>Must have one or more number.</span>
                    <span iden="splchar" class="pass-lbl"><i class="fa fa-times"></i>Must have one or more special character (!@#$%^&*).</span>
                </div>
                <a href="#" class="btns btn-submit w-100" onclick="fnChangePassword()">Submit</a>
            </div>
        </div>
    </div>
    
    <div id="divloader" class="loader_bg">
        <div class="loader"></div>
    </div>

    <asp:HiddenField ID="hdnUserName" runat="server" />
    <asp:HiddenField ID="hdnLoginId" runat="server" />
    <asp:HiddenField ID="hdnNodeID" runat="server" />
    <asp:HiddenField ID="hdnUserId" runat="server" />
    
    <asp:HiddenField ID="hdnIsFirst" runat="server" />
</asp:Content>

