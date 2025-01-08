<%@ Page Title="" Language="C#" MasterPageFile="~/Data/MasterPage/Site.master" AutoEventWireup="true" CodeFile="frmSendEmailInvite.aspx.cs" EnableEventValidation="false" Inherits="frmSendEmailInvite" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <script src="../../Scripts/jquery-ui.js"></script>
     <link href="../../CSS/jquery-ui.css" rel="stylesheet" />
    <style>
        tr.clstrgdmapped > td{
            background-color:#f5f5f5;
        }

    </style>
    <script>
      
        $(document).ready(function () {
            fnUserList();
        });

        function fnValidaterUserType()
        {
            var UserType = $("#ConatntMatter_ddlUserType").val();
            var CycleId = $("#ConatntMatter_ddlCycle").val().split("^")[0];
            if(UserType == "0")
            {
                alert("Please select the user type")
                return false;
            }
            if (CycleId == "0") {
                alert("Please select the batch")
                return false;
            }
        }
        var MappingStatus = 0;
        function fnUserList() {

            //if (fnValidaterUserType() == false)
            //{
            //    return false;
            //}
            //var MailCreationStatus = $("#ConatntMatter_ddlCycle").val().split("^")[1];
            //var LoginId = $("#ConatntMatter_hdnLoginId").val();
            //var CycleDate = $("#ConatntMatter_ddlCycle option:selected").attr("CycleDate");

            ////var UserType = $("#ConatntMatter_ddlUserType").val();
            ////alert("CycleId=" + CycleId)
            ////alert("MeetingCreationStatus=" + MailCreationStatus)
            //$("#anchorbtn_other").hide();
            //$("#anchorbtn_gd").hide();
            //if (CycleId == 0)
            //{
                
            //    $("#ConatntMatter_divdrmmain").show();
            //    $("#anchorbtn_other").hide();
            //    $("#ConatntMatter_divdrmmain")[0].innerHTML = "";
            //    return false;   
            //}
          

            $("#dvFadeForProcessing").show();
            PageMethods.fngetdata(0, function (result) {
                $("#dvFadeForProcessing").hide();
                $("#divBTNS").hide();
                if (result.split("|")[0] == "2") {
                    alert("Error-" + result.split("|")[1]);
                } else if (result == "") {
                    $("#ConatntMatter_divdrmmain")[0].innerHTML = "No Participant Found!!!";
                }
                else {
                   
                    $("#ConatntMatter_divdrmmain").show();
                    $("#anchorbtn_other").show();
                   
                    $("#ConatntMatter_divdrmmain")[0].innerHTML = result.split("|")[0];
                    //---- this code add by satish --- //
                    $("#ConatntMatter_divdrmmain").prepend("<div id='tblheader'></div>");
                    if ($("#tbldbrlist").length > 0) {
                        var wid = $("#tbldbrlist").width(), thead = $("#tbldbrlist").find("thead").eq(0).html();
                        $("#tblheader").html("<table id='tblEmp_header' class='table table-bordered table-sm mb-0' style='width:" + (wid - 1) + "px;'><thead>" + thead + "</thead></table>");
                        $("#tbldbrlist").css({ "width": wid, "min-width": wid });

                        for (i = 0; i < $("#tbldbrlist").find("th").length; i++) {
                            var th_wid = $("#tbldbrlist").find("th")[i].clientWidth;
                            $("#tblEmp_header, #tbldbrlist").find("th").eq(i).css({ "min-width": th_wid, "width": th_wid });
                        }
                        $("#tbldbrlist").css("margin-top", "-" + ($("#tblEmp_header")[0].offsetHeight) + "px");

                        $('#dvtblbody').css({
                            'height': $(window).height() - 380,
                            'overflow-y': 'auto',
                            'overflow-x': 'hidden'
                        });
                        $("#divBTNS").show();
                        $(".mergerow").closest('tr').find('td').css('border-top', '2px solid black');
                    }
                    var activeIndex = parseInt($("#tablist").find("a.active").closest("li").index()) + 1;
                  //  fnShowDataAssigned(activeIndex);
                }
            },
                function (result) {
                    $("#dvFadeForProcessing").hide();
                    alert("Error-" + result._message);
                }
            )
        }
       
    </script>
    

    <script>

        //for saving 

        function check_uncheck_checkbox(isChecked) {
            if (isChecked) {
                $('input[type=checkbox]').each(function () {
                    this.checked = true;
                });
            } else {
                $('input[type=checkbox]').each(function () {
                    this.checked = false;
                });
            }
        }



        function fnSave(flg) {

            var flgvalid = true;
            var IsCnt = 0;
            var flgStatus = 1;

            var CycleId = $("#ConatntMatter_ddlCycle").val();
            var CycleDate = $("#ConatntMatter_ddlCycle option:selected").attr("CycleDate");

            var ArrDataSaving = []; var ArrDataMails = [];
            var hdnFlagValue  = $("#ConatntMatter_hdnMailFlag").val(1);
         
            // if (flg == 1) {
            //str.Append("<tr UserType= '" + UserType + "' EmailID = '" + EmailID +"'  EmpNodeID = '" + EmpNodeID +"' Fname ='" + Fname + "'  calenderstarttime='" + Calenderstarttime + "' calenderendtime='" + Calenderendtime + "' OrientationTime='"+ OrientationTime + "'>");

            var cntOneTonOne = 0; var OldParticipantCycleMappingId = 0; var startTime = ""; var timeselected = 0;
            $("#ConatntMatter_divdrmmain").find("#tbldbrlist input[flg=1]:checked").each(function () {
                var UserType = 1;// $(this).closest("tr").attr("UserType");
                var EmailId = $(this).closest("tr").attr("EmailID");
                
                var Fname = $(this).closest("tr").attr("Fname");
                var UserName = $(this).closest("tr").attr("UserName");
                var Password = $(this).closest("tr").attr("Password");
                var DBRName = $(this).closest("tr").attr("DBRName");
                ArrDataSaving.push({ DBRName: DBRName, EmailId: EmailId, Fname: Fname, MailStatus: '',UserName: UserName, Password: Password});
            });

         
            if (ArrDataSaving.length == 0) {
                alert("Kindly Select atleast one checkbox!")
                return false;
            }
           
            //$.extend(ArrDataSaving, ArrDataSavingGD);
            $("#dvFadeForProcessing").show();
            PageMethods.fnSave(ArrDataSaving, fnSave_Success, fnFailed, flgStatus);
        }
       
        function fnSave_Success(result, flgStatus) {
            $("#dvFadeForProcessing").hide();
            if (result.split("|")[0] == "0") {
                var tbl = $.parseJSON(result.split("|")[1]);
                var strHTML = "";
                if (tbl.length > 0) {

                    var style = "border-left: 1px solid #A0A0A0; border-bottom: 1px solid #A0A0A0;font-family:arial narrow;font-size:9pt;";
                    strHTML += "Kindly find below Mail status for each participant.</br>";
                    strHTML += ("<table cellpadding='2' cellspacing='0' style='font-family:arial narrow;font-size:8.5pt;border-right: 1px solid #A0A0A0; border-top: 1px solid #A0A0A0;text-align:center;width:100%'><thead>");
                    strHTML += ("<tr bgcolor='#26a6e7'>");
                    strHTML += "<th  style='" + style + ";color:#fff;text-align:left;padding:3px'>Sr.No</th>";
                    strHTML += "<th  style='" + style + ";color:#fff;text-align:left;padding:3px'>Participant Name</th>";
                    strHTML += "<th  style='" + style + ";color:#fff;text-align:left;padding:3px'>EmailId</th>";
                    strHTML += "<th  style='" + style + ";color:#fff;text-align:left;padding:3px'>Mail Status</th>";
                    strHTML += ("</tr></thead><tbody>");
                    //MeetingStartTime
                    var flgvalid = true;
                    var Oldparticipantcyclemappingid = 0;
                    var cnt = 1;
                    for (var i in tbl) {
                        var ParticipantAssessorMappingId = 0;// tbl[i]["ParticipantAssessorMappingId"];
                        var MailStatus = tbl[i]["MailStatus"];
                        var ParticipantName = tbl[i]["Fname"];
                        var EmailId = tbl[i]["EmailId"];
                       
                        var strColor = "";
                                             
                        strHTML += ("<tr " + strColor + ">");
                        strHTML += "<td  style='" + style + ";text-align:center;padding:3px'>" + cnt + "</td>";
                        strHTML += "<td  style='" + style + ";text-align:left;padding:3px'>" + ParticipantName + "</td>";
                        strHTML += "<td  style='" + style + ";text-align:left;padding:3px'>" + EmailId + "</td>";
                        strHTML += "<td  style='" + style + ";text-align:left;padding:3px'>" + MailStatus + "</td>";
                        strHTML += ("</tr>");
                        cnt++;
                       
                       
                    }
                  
                    strHTML += ("</tbody></table>");
                   
                }
                $("#dvAlert")[0].innerHTML = strHTML;
                $("#dvAlert").dialog({
                    title: "Alert!",
                    modal: true,
                    width: "750",
                    height: "450",
                    close: function () {
                        $(this).dialog('destroy');
                      
                    },
                    buttons: {
                        "OK": function () {
                            $(this).dialog('close');
                            fnUserList();
                        }
                    }
                })
            }
            else {
                $("#dvAlert")[0].innerHTML = "Error:" + result.split("|")[1];
                $("#dvAlert").dialog({
                    title: "Error:",
                    modal: true,
                    width: "auto",
                    height: "auto",
                    close: function () {
                        $(this).dialog('destroy');
                    },
                    buttons: {
                        "OK": function () {
                            $(this).dialog('close');
                        }
                    }
                })
            }
        }

        function fnFailed(result) {
            alert("Error-"+result._message);
            $("#dvFadeForProcessing").hide();
        }

    </script>



    <script type="text/javascript">
        var OldActive = 2;
        function fnShowDataAssigned(X) {
            var CycleID = $("#ConatntMatter_ddlCycle").val();

            if (CycleID == 0) {
                alert("Please select the Cycle Name");
                $("#ConatntMatter_ddlCycle").eq(0).focus();
                return false;
            }
            //    alert(CycleID)
           

            OldActive = X;
            if (X == 1)   ////// For New User mail
            {
                $("#tbldbrlist tbody tr").hide();
                $("#tbldbrlist  tbody tr[flgDisplayRow = 1]").css("display", "table-row")

                $("#ConatntMatter_hdnMailFlag").val(1);
            }
            else if (X == 2)  ///// For updated Scheduler Mail
            {
                $("#tbldbrlist tbody tr").hide();
                $("#tbldbrlist  tbody tr[flgDisplayRow = 3]").css("display", "table-row")
                $("#ConatntMatter_hdnMailFlag").val(2);
            }
            else {
                $("#tbldbrlist tbody tr").hide();    ////// For Resend
                $("#tbldbrlist  tbody tr[flgDisplayRow = 2]").css("display", "table-row")

                //$("#tbldbrlist  tbody tr").css("display", "table-row")
                $("#ConatntMatter_hdnMailFlag").val(1);
            }
            $("#tablist").find("a.active").removeClass("active");
            $("#tablist").find("a").eq(OldActive - 1).addClass("active");
            //PageMethods.fnDisplayParticpantAgCycle(CycleID, X, fnGetDisplaySuccessData, fnGetFailed, X);

        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ConatntMatter" runat="Server">
    <div class="section-title clearfix">
        <h3 class="text-center">Login Credential Mail To Users </h3>
        <div class="title-line-center"></div>
    </div>
     <%--<div class="form-group row" >
        <label for="ac" class="col-sm-2 col-form-label">Select User Type :</label>
        <div class="col-sm-4">
            <asp:DropDownList runat="server" ID="ddlUserType"  CssClass="form-control">
                <asp:ListItem Value="0">- Select - </asp:ListItem>
                   <asp:ListItem Value="1">Participant</asp:ListItem>
                   <asp:ListItem Value="2">Developer</asp:ListItem>
                   <asp:ListItem Value="3">EY Admin</asp:ListItem>
            </asp:DropDownList>
        </div>       
    </div>--%>
    
    <%--<div class="form-group row" >
        <label for="ac" class="col-sm-2 col-form-label">&nbsp;</label>
        <div class="col-sm-4">
            <input type="button" id="btnShowUsers" value="Show Users" onclick="fnUserList()" class="btns btn-submit btns-small">
            
        </div>       
    </div>--%>
    <div class="body-content" style="background-color:#fff">
       <%-- <!-- Nav tabs -->
        <ul class="nav nav-tabs" role="tablist" id="tablist">
            <li><a class="nav-link active" onclick="fnShowDataAssigned(1)" style="cursor:pointer">New Users Scheduler Email</a></li>
            <li ><a  class="nav-link"  onclick="fnShowDataAssigned(2)" style="cursor:pointer">Updated Scheduler Mail</a></li>
            <li style="display:none"><a class="nav-link " onclick="fnShowDataAssigned(3)" style="cursor:pointer">Resend Scheduler Email</a></li>
        </ul>--%>

             <div class="tab-content">
      
        <!-- Tab panes -->
             <div id="divdrmmain" runat="server" style="min-height:350px"></div>
    </div>
        </div>


    <div class="text-center" id="divBTNS" style="display: none;">
        <a href="###" class="btns btn-submit" onclick="return fnSave(1)" id="anchorbtn_other" style="display: none">Send Mail</a>
    </div>
   

    <div id="dvDialog" style="display: none"></div>
    <div id="dvAlert" style="display: none;"></div>
    <div id="dvFadeForProcessing" class="loader_bg">
        <div class="loader"></div>
    </div>
    <asp:HiddenField runat="server" ID="hdnLoginId" Value="0" />
    <asp:HiddenField runat="server" ID="hdnMenuId" Value="0" />
     <asp:HiddenField runat="server" ID="hdnMailFlag" Value="0" />
</asp:Content>

