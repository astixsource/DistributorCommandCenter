<%@ Page Title="" Language="C#" MasterPageFile="~/Data/MasterPage/Site.master" AutoEventWireup="true" CodeFile="HawkeyeWSTarget.aspx.cs" Inherits="HawkeyeWSTarget" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <%--<script src="../../BICss/lib/jquery/dist/jquery.min.js"></script>
    <link href="../../BICss/style/style.css" rel="stylesheet" />--%>

    <script type="text/javascript">
        function fnFailed(result) {
            $("#divloader").hide();
            alert("Error : " + result);
        }

        $(document).ready(function () {
            $("#divRptTitle").html("WS Store Target");
            $("#divContainer").height($(window).height() - ($("nav.navbar").height() + $("div.card-header").height() + 15) + "px");
        });


        function fnSelectFile() {
            var obj = $("#FileUploader").val().split("\\");
            var filename = obj[obj.length - 1];
            $("#FileUploader").next().html(filename);
        }
        function fnUploadData() {
            if ($("#FileUploader").get(0).files.length == 0) {
                $("#ConatntMatter_divMsg").html("Please Select the File !");
            }
            else {
                var file = $("#FileUploader").get(0).files;

                var data = new FormData();
                data.append(file[0].name, file[0]);
                data.append("MonthYr", $("#ConatntMatter_ddlMonthYear").val());
                data.append("LoginId", $("#ConatntMatter_hdnLoginId").val());
                data.append("FileType", "2");   //1: SubD; 2: WS

                //$("#divloader").show();
                $("#ConatntMatter_divMsg").html("Please wait while Target is uploading ...");
                $.ajax({
                    url: "../../ExcelReadAndBulkCopyUpload.ashx",
                    type: "POST",
                    data: data,
                    async: true,
                    contentType: false,
                    processData: false,
                    success: function (result) {
                        if (result.split("^")[0] == "0") {
                            $("#ConatntMatter_divMsg").html(result.split("^")[1]);
                            $("#FileUploader").next().html('');
                            $("#FileUploader").val('');
                        }
                        else {
                            $("#ConatntMatter_divMsg").html(result.split("^")[1]);
                        }
                        $("#divloader").hide();
                    },
                    error: function (err) {
                        $("#ConatntMatter_divMsg").html("Error : " + err.statusText);
                        $("#divloader").hide();
                    }
                });
            }
        }


    </script>
    <style type="text/css">
        .main-content {
            padding-bottom: 0 !important;
        }

        .clslbl {
            font-size: 0.9rem; 
            font-weight: 600;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ConatntMatter" runat="Server">
    <div class="card default_bg no_radius m_row">
        <div class="card-header green_bg pr-0">
            <div class="card-title-heading white_txt" id="divRptTitle" style="display: inline-block; width: 100%; text-align: center;"></div>
            <div class="card-title-actions blue_bg" style="cursor: pointer">
                <img src="../../Images/refresh_icon.svg" onclick="fnRefreshPage()" class="icon" />
            </div>
        </div>
        <div id="divContainer">
            <div class="m-4 p-3 row" style="border: 1px solid #00b300; border-radius: 4px; background: #f9fff9;">
                
                <div class="col-4 mb-2">
                    <span class="clslbl">Month-Year : </span>
                    <asp:DropDownList ID="ddlMonthYear" runat="server" CssClass="form-control form-control-sm d-inline-block bg-white ml-2" Style="width: 165px;"></asp:DropDownList>                    
                </div>                
                <div class="col-8 mb-2 text-right">
                    <asp:Button ID="btnDownloadTemplate" runat="server" Text="Download" class='btns btn-submit pt-1 pr-3 pb-1 pl-3' title="Click to download" OnClick="btnDownloadTemplate_Click" />
                </div>

                <div class="col-2 clslbl pt-2" >
                    Please select the File : 
                </div>
                <div class="col-8">
                    <input type="file" id="FileUploader" name="FileUploader" onchange="fnSelectFile();" class="form-control-sm" style="width: 85px; padding-left: 0;" />
                    <span></span>
                </div>
                <%--<div class="col-2 pt-2 text-right">
                    <a href="#" onclick="fnDownloadTemplate();"><img class="mr-2" src="../../Images/excel.gif" style="height: 18px;" />Sample Template</a>
                </div>--%>
            </div>
            <div class="w-100 pt-4 text-center">
                <input id="btnUpload" type='button' class='btns btn-submit' title="Click to Upload" value='Upload Target' onclick='fnUploadData();' />
                <div id="divMsg" runat="server" class="w-50 mt-4 ml-auto mr-auto" style="color: #ff0000; font-size: 0.9rem; font-weight: 600;"></div>
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
</asp:Content>

