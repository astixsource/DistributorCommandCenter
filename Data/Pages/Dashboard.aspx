<%@ Page Title=""  Language="C#" MasterPageFile="~/Data/MasterPage/Site.master" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="Data_Dashboard_Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <script src="../../BICss/lib/jquery/dist/jquery.min.js"></script>
    <script src="../../Scripts/powerbi.min.js"></script>
    <script src="https://playground.powerbi.com/app/powerbi-report-authoring.min.js"></script>
    <link href="../../BICss/style/style.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {
            var c_h = $(".p_txt").outerHeight();
            $(".icon_right").css({
                height: c_h
            });
        });


        async function fnRefreshPage() {
            await _Embed_BasicEmbed();
        }
    </script>
    <script>
        async function _Embed_BasicEmbed() {
            let loadedResolve, reportLoaded = new Promise((res, rej) => { loadedResolve = res; });
            let renderedResolve, reportRendered = new Promise((res, rej) => { renderedResolve = res; });

            // Get models. models contains enums that can be used.
            models = window['powerbi-client'].models;

            // Embed a Power BI report in the given HTML element with the given configurations
            // Read more about how to embed a Power BI report in your application here: https://go.microsoft.com/fwlink/?linkid=2153590
            function embedPowerBIReport() {
                /*-----------------------------------------------------------------------------------+
                |    Don't change these values here: access token, embed URL and report ID.          | 
                |    To make changes to these values:                                                | 
                |    1. Save any other code changes to a text editor, as these will be lost.         |
                |    2. Select 'Start over' from the ribbon.                                         |
                |    3. Select a report or use an embed token.                                       |
                +-----------------------------------------------------------------------------------*/
                // Get a reference to the embedded report HTML element
                let embedContainer = $('#ConatntMatter_embedContainer')[0];

                // Read embed application token from Model
                let accessToken = $("#ConatntMatter_hdnEmbedaccesstoken").val();

                // console.log(accessToken);
                // You can embed different reports as per your need by changing the index
                // Read embed URL from Model
                let embedUrl = $("#ConatntMatter_hdnEmbedUrl").val();

                //console.log(embedUrl);
                // Read report Id from Model
                let embedReportId = $("#ConatntMatter_hdnReportId").val();

               

                
                // We give All permissions to demonstrate switching between View and Edit mode and saving report.
                let permissions = models.Permissions.All;

                // Create the embed configuration object for the report
                // For more information see https://go.microsoft.com/fwlink/?linkid=2153590
                let config = {
                    type: 'report',
                    tokenType: models.TokenType.Embed,
                    accessToken: accessToken,
                    embedUrl: embedUrl,
                    id: embedReportId,
                    permissions: permissions,
                    settings: {
                        panes: {
                            filters: {
                                visible: false
                            },
                            pageNavigation: {
                                visible: true
                            }
                        }
                    }
                };

                // Get a reference to the embedded report HTML element
                //let embedContainer = $('#embedContainer')[0];

                // Embed the report and display it within the div container.
                report = powerbi.embed(embedContainer, config);

                // report.off removes all event handlers for a specific event
                report.off("loaded");

                // report.on will add an event handler
                report.on("loaded", function () {
                    loadedResolve();
                    report.off("loaded");
                });

                // report.off removes all event handlers for a specific event
                report.off("error");

                report.on("error", function (event) {
                    console.log(event.detail);
                });

                // report.off removes all event handlers for a specific event
                report.off("rendered");

                // report.on will add an event handler
                report.on("rendered", function () {
                    renderedResolve();
                    report.off("rendered");
                });
            }

            embedPowerBIReport();
            await reportLoaded;

            // Insert here the code you want to run after the report is loaded

            await reportRendered;
            var SiteName = $("#ConatntMatter_hdnSiteName").val();
            console.log("Step2");
            const filter2 = {
                $schema: "http://powerbi.com/product/schema#basic",
                target: {
                    table: "Location Hierarchy",
                    column: "Site Name"
                },
                operator: "In",
                values: [SiteName]
            };

            // Retrieve the page collection and get the visuals for the active page.
            try {
                const pages = await report.getPages();
                console.log("pages");

                let log = "Report pages:";
                pages.forEach(function (page) {
                    log += "\n" + page.name + " - " + page.displayName;
                });
                console.log(log);
                // Retrieve the active page.
                let page = pages.filter(function (page) {
                    return page.isActive;
                })[0];
                console.log("page");
                const visuals = await page.getVisuals();

                // Retrieve the target visual.
                let slicer = visuals.filter(function (visual) {
                    return visual.type === "slicer" && visual.name === "f643732c7a6b02a872ec";
                })[0];

                // Set the slicer state which contains the slicer filters.
                await slicer.setSlicerState({ filters: [filter2] });
                console.log("Date slicer was set.");
            }
            catch (errors) {
                console.log(errors);
            }
            // Insert here the code you want to run after the report is rendered

        }
        $(document).ready(async function () {
            $("#divRptTitle").html("welcome " + $("#ConatntMatter_hdnUserName").val().toString().toUpperCase());
            $("#ConatntMatter_embedContainer").height($(window).height() - ($("nav.navbar").height() + $("div.card-header").height() + 34) + "px");
            await _Embed_BasicEmbed();
        });

       
        function fnPrintRpt() {
            // Trigger the print dialog for your browser.
            try {
                report.print();
            }

            catch (errors) {
                console.log(errors);
            }
        }
        function fnFullScreenRpt() {
            try {
                report.fullscreen();
            }
            catch (errors) {
                console.log(errors);
            }
        }
    </script>

    <style type="text/css">
        i.cls-i-btn {
            color: #000;
            cursor: pointer;
            font-size: 1.2rem;
        }

        .main-content {
            padding-bottom: 0 !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ConatntMatter" runat="Server">
    <div class="card default_bg no_radius m_row pb-0 ">
        <div class="card-header green_bg pr-0">
            <div class="card-title-heading white_txt" id="divRptTitle" style="display: inline-block; width: 100%; text-align: center;"></div>
            <i class="fa fa-window-maximize cls-i-btn" title="Full-Screen" onclick="fnFullScreenRpt();"></i>
            <i class="fa fa-print cls-i-btn ml-2 mr-2" title="Print" onclick="fnPrintRpt();"></i>
            <div class="card-title-actions blue_bg" style="cursor: pointer">
                <img src="../../Images/refresh_icon.svg" title="Refresh" onclick="fnRefreshPage()" class="icon" />
            </div>
        </div>
        <div class="card-body pb-0 w-100" id="embedContainer" runat="server"></div>
    </div>
    <asp:HiddenField runat="server" ID="hdnEmbedaccesstoken" Value="" />
    <asp:HiddenField runat="server" ID="hdnEmbedUrl" Value="" />
    <asp:HiddenField runat="server" ID="hdnReportId" Value="" />
    <asp:HiddenField ID="hdnUserName" runat="server" />
    <asp:HiddenField ID="hdnLoginId" runat="server" />
    <asp:HiddenField ID="hdnUserId" runat="server" />
    <asp:HiddenField ID="hdnSiteName" runat="server" />
    
</asp:Content>

