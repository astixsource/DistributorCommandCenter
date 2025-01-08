<%@ Page Title="" Language="C#" MasterPageFile="~/Data/MasterPage/Site.master" AutoEventWireup="true" CodeFile="frmChannelChangeDate.aspx.cs" Inherits="frmChannelChangeDate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <%--<script src="../../BICss/lib/jquery/dist/jquery.min.js"></script>
    <link href="../../BICss/style/style.css" rel="stylesheet" />--%>

    <script type="text/javascript">
        
        function fnFailed(result) {
            $("#divloader").hide();
            alert("Error : " + result);
        }

        $(document).ready(function () {
            //$("#divRptTitle").html("JBP Data");

           // fnShowReport();
            $("#txtStartDate").datepicker({
                dateFormat: 'mm/dd/yy',
                changeMonth: true,
                changeYear: true,
                showOn: "button",
                buttonImage: "../../Images/Icons/calendar-icon.jpg",
                buttonImageOnly: true,
                buttonText: "Select date",
                onClose: function (selectedDate) {
                    $("#txtEndDate").datepicker("option", "minDate", selectedDate);
                }
            });
            //  $("#ConatntMatter_txtDateTo").val(new Date().localeFormat('dd-MMM-yyyy'))
            $("#txtEndDate").datepicker({
                dateFormat: 'mm/dd/yy',
                minDate: $("#txtStartDate").val(),
                showOn: "button",
                buttonImage: "../../Images/Icons/calendar-icon.jpg",
                buttonImageOnly: true,
                buttonText: "Select date",
                changeMonth: true,
                changeYear: true,
            });

        });

        //function fnShowReport() {
        //    $("#divloader").show();
        //    var Month = $("#ConatntMatter_ddlMonthYear").val().split("-")[0];
        //    //var Year = $("#ConatntMatter_ddlMonthYear").val().split("-")[1];
        //    var NodeID = $("#ConatntMatter_hdnNodeID").val();

        //    PageMethods.ChanelChangeManageDate(Month, NodeID, GetReport_pass, fnFailed);
        //}
        function GetReport_pass(result) {
            $("#divloader").hide();

            if (result.split("|^|")[0] == "0") {
                $("#divReport").html(result.split("|^|")[1]);
            }
            else {
                $("#divHeader").html("");
                $("#divReport").html("Error : " + result.split("|^|")[1]);
            }
        }

        function fnEdit(ctrl) {
            $(ctrl).closest("tr").attr("flgEdit", "1");
            $(ctrl).closest("tr").attr("NewImg", $(ctrl).closest("tr").attr("Img"));

            $(ctrl).closest("tr").find("select").removeAttr("disabled");
            $(ctrl).closest("td").eq(0).html("<img title='Save' src='../../Images/save_blue.png' onclick='fnIndividualSave(this);'/><img title='Cancel' src='../../Images/cancel_blue.png' class='ml-2' onclick='fnCancel(this);'/>");
        }
        function fnCancel(ctrl) {
            $(ctrl).closest("tr").attr("flgEdit", "0");
            $(ctrl).closest("tr").attr("IsInTemp", "0");
            $(ctrl).closest("tr").attr("NewImg", $(ctrl).closest("tr").attr("Img"));

            $(ctrl).closest("tr").find("select").val($(ctrl).closest("tr").attr("IsJBPDone"));
            $(ctrl).closest("tr").find("select").prop("disabled", true);

            $(ctrl).closest("td").eq(0).html("<img src='../../Images/edit_blue.png' alt='edit' onclick='fnEdit(this)'/>");
        }
        function fnIndividualSave() {

            if ($("#ConatntMatter_ddlMonthYear").val() == "0") {
                alert("Please select Month !");
                $("#ConatntMatter_ddlMonthYear").focus();
            }
            else if ($("#txtStartDate").val() == "") {
                alert("Please select start date !");
                $("#txtStartDate").focus();
            }
            else if ($("#txtEndDate").val() == "") {
                alert("Please select end date !");
                $("#txtEndDate").focus();
            }
            else {
                $("#divloader").show();
                var Month = $("#ConatntMatter_ddlMonthYear").val().split("-")[0];
                var Year = $("#ConatntMatter_ddlMonthYear").val().split("-")[1];
                var StartDate = $("#txtStartDate").val();
                var EndDate = $("#txtEndDate").val();
                var LoginId = $("#ConatntMatter_hdnLoginId").val();

                PageMethods.fnSave(Month, Year, StartDate, EndDate, LoginId, fnSave_pass, fnFailed);
            }
        }
        function fnSave_pass(res, Img) {
            $("#divloader").hide();
            if (res.split("|^|")[0] == "0") {
                alert("Saved successfully !");
                $("#divReport").html("Saved successfully !");
            }
            else {
                fnFailed(res.split("|^|")[1]);
                $("#divReport").html(res.split("|^|")[1]);
            }

        }

        function fnCHangeMonth() {
            if ($("#ConatntMatter_ddlMonthYear").val() == "0") {
                $("#txtStartDate").val("");
                $("#txtEndDate").val("");
            }
            else {
                var str = $("#ConatntMatter_ddlMonthYear").val();
                $("#txtStartDate").val(str.split("-")[2]);
                $("#txtEndDate").val(str.split("-")[3]);
            }
        }

        function fnImg(ctrl) {
            $("#divImage").dialog({
                title: $(ctrl).closest("tr").find("td").eq(4).html() + " :",
                modal: true,
                width: "50%",
                open: function () {
                    $("#ConatntMatter_hdnBranchSubDNodeID").val($(ctrl).closest("tr").attr("strId"));

                    if ($(ctrl).closest("tr").attr("flgEdit") == "0")
                        $("#btnUpload").hide();
                    else
                        $("#btnUpload").show();

                    if ($(ctrl).closest("tr").attr("flgEdit") == "0")
                        $("#JBPImg").attr("src", "../../Files/JBP/" + $(ctrl).closest("tr").attr("Img"));
                    else if ($(ctrl).closest("tr").attr("NewImg") == "")
                        $("#JBPImg").attr("src", "../../Files/no-image.png");
                    else if ($(ctrl).closest("tr").attr("IsInTemp") == "0")
                        $("#JBPImg").attr("src", "../../Files/JBP/" + $(ctrl).closest("tr").attr("NewImg"));
                    else
                        $("#JBPImg").attr("src", "../../Files/JBP/Temp/" + $(ctrl).closest("tr").attr("NewImg"));
                },
                close: function () {
                    $("#divImage").dialog('destroy');
                },
                buttons: [
                    {
                        text: "Close",
                        "class": "btns btn-submit",
                        click: function () {
                            $("#divImage").dialog('close');
                        }
                    }
                ]
            });
        }
        function fnClicktoUploadImg() {
            $("#fileUploader").click();
        }
        function fnUploadImg() {
            if ($("#fileUploader").get(0).files.length > 0) {
                var LoginId = $("#ConatntMatter_hdnLoginId").val();
                var UserId = $("#ConatntMatter_hdnUserId").val();
                var NodeID = $("#ConatntMatter_hdnNodeID").val();

                var data = new FormData();
                var file = $("#fileUploader").get(0).files;
                data.append(file[0].name, file[0]);
                data.append("UserID", UserId);
                data.append("NodeID", NodeID);
                data.append("LoginID", LoginId);
                data.append("FileID", $("#ConatntMatter_hdnBranchSubDNodeID").val());
                data.append("FileName", "");
                data.append("Actionflg", "1");

                $("#divloader").show();
                $.ajax({
                    url: "../../FileUploaderHandler.ashx",
                    type: "POST",
                    data: data,
                    async: true,
                    contentType: false,
                    processData: false,
                    success: function (result) {
                        if (result.split("^")[0] == "0") {
                            $("#fileUploader").val('');
                            $("#JBPImg").attr("src", "../../Files/JBP/Temp/" + result.split("^")[1]);
                            $("#tblJBP").find("tbody").eq(0).find("tr[strId='" + $("#ConatntMatter_hdnBranchSubDNodeID").val() + "']").eq(0).attr("IsInTemp", "1");
                            $("#tblJBP").find("tbody").eq(0).find("tr[strId='" + $("#ConatntMatter_hdnBranchSubDNodeID").val() + "']").eq(0).attr("NewImg", result.split("^")[1]);
                        }
                        else
                            alert("Error :" + result.split("^")[1]);

                        $("#divloader").hide();
                    },
                    error: function (err) {
                        alert("Error :" + err.statusText);
                        $("#divloader").hide();
                    }
                });
            }
        }
        function fnMovefromTemp(ImgName) {
            var LoginId = $("#ConatntMatter_hdnLoginId").val();
            var UserId = $("#ConatntMatter_hdnUserId").val();
            var NodeID = $("#ConatntMatter_hdnNodeID").val();

            var data = new FormData();
            data.append("UserID", UserId);
            data.append("NodeID", NodeID);
            data.append("LoginID", LoginId);
            data.append("FileID", "");
            data.append("FileName", ImgName);
            data.append("Actionflg", "9");

            $("#divloader").show();
            $.ajax({
                url: "../../FileUploaderHandler.ashx",
                type: "POST",
                data: data,
                async: true,
                contentType: false,
                processData: false,
                success: function (result) {
                    if (result.split("^")[0] == "0") {
                        $("#fileUploader").val('');
                    }
                    else
                        alert("Error :" + result.split("^")[1]);

                    $("#divloader").hide();
                },
                error: function (err) {
                    alert("Error :" + err.statusText);
                    $("#divloader").hide();
                }
            });
        }

        function fnChangeFromdate() {
            
            $("#txtEndDate").attr("min", $("#txtStartDate").val());
            if ($("#txtEndDate").val() != "") {
                if ($("#txtStartDate").val() > $("#txtEndDate").val()) {
                    $("#txtEndDate").val("");
                }
            }
        }

    </script>
    <style type="text/css">
        #btnUpload {
            cursor: pointer;
            font-size: 1rem;
            font-weight: 700;
        }

            #btnUpload:hover {
                text-decoration: underline;
            }

        #divReport {
            overflow-y: auto;
            overflow-x: hidden;
        }

        table.clsJBP > thead > tr > th {
            color: #093D6D;
            font-size: 0.8rem;
            background: #f5f8ff;
            text-align: center;
            vertical-align: middle;
            text-transform: uppercase;
            border-bottom: 2px solid #006092 !important;
        }

        table.clsJBP tr th:nth-child(1),
        table.clsJBP tr th:nth-child(7) {
            width: 6%;
        }

        table.clsJBP tr th:nth-child(2),
        table.clsJBP tr th:nth-child(4) {
            width: 16%;
        }

        table.clsJBP tr th:nth-child(3),
        table.clsJBP tr th:nth-child(6) {
            width: 10%;
        }

        table.clsJBP tr th:nth-child(8) {
            width: 8%;
        }

        table.clsJBP td {
            font-size: 0.8rem;
        }

            table.clsJBP td > img {
                cursor: pointer;
            }

            table.clsJBP td > select {
                width: 100%;
            }

        table.clsJBP tr td:nth-child(1),
        table.clsJBP tr td:nth-child(3),
        table.clsJBP tr td:nth-child(6),
        table.clsJBP tr td:nth-child(7),
        table.clsJBP tr td:nth-child(8) {
            text-align: center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ConatntMatter" runat="Server">
    <div class="card default_bg no_radius m_row">
        <div class="card-header green_bg pr-0">
            <div class="card-title-heading white_txt" id="divRptTitle" style="display: inline-block; width: 100%; text-align: center;">Manage Date</div>
        </div>
        <div class="ml-4 mt-3">
            <span style="font-size: 1rem; font-weight: 700;">Month-Year : </span>
            <asp:DropDownList ID="ddlMonthYear" runat="server" onchange="fnCHangeMonth()" CssClass="form-control form-control-sm d-inline-block bg-transparent ml-2"  Style="width: 150px;"></asp:DropDownList>

            <span style="font-size: 1rem; font-weight: 700;margin-left:5px">Start Date : </span>
            <input type="text" onchange="fnChangeFromdate()" readonly id="txtStartDate" class="form-control form-control-sm d-inline-block bg-transparent" style="width: 120px;" />

            <span style="font-size: 1rem; font-weight: 700;margin-left:5px">End Date : </span>
            <input type="text" readonly  id="txtEndDate" class="form-control form-control-sm d-inline-block bg-transparent" style="width: 120px;" />

            <input type="button" class="btns btn-submit ml-3" onclick="fnIndividualSave()" value="Save" />
        </div>
        <div class="card-body pt-0" id="divReport">
        </div>
    </div>

    <div id="divloader" class="loader_bg">
        <div class="loader"></div>
    </div>

   
    <asp:HiddenField ID="hdnUserName" runat="server" />
    <asp:HiddenField ID="hdnLoginId" runat="server" />
    <asp:HiddenField ID="hdnNodeID" runat="server" />
    <asp:HiddenField ID="hdnUserId" runat="server" />

    <asp:HiddenField ID="hdnBranchSubDNodeID" runat="server" />
</asp:Content>

