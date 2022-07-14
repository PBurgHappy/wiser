﻿import { TrackJS } from "trackjs";
import { Wiser2 } from "../../Base/Scripts/Utils.js";
import "../../Base/Scripts/Processing.js";
require("@progress/kendo-ui/js/kendo.all.js");
require("@progress/kendo-ui/js/cultures/kendo.culture.nl-NL.js");
require("@progress/kendo-ui/js/messages/kendo.messages.nl-NL.js");

import "../../../Core/Scss/fonts.scss"; // TEMP
import "../../../Core/Scss/icons.scss"; // TEMP
import "../css/Dashboard.scss";
import login from "../../../Core/Scripts/components/login";

// Any custom settings can be added here. They will overwrite most default settings inside the module.
const moduleSettings = {
};

((jQuery, moduleSettings) => {
    class Dashboard {
        constructor(settings) {
            kendo.culture("nl-NL");

            // Base settings.
            this.settings = {};
            Object.assign(this.settings, settings);

            // Other.
            this.mainLoader = null;

            this.itemsData = null;

            // Fire event on page ready for direct actions
            document.addEventListener("DOMContentLoaded", () => {
                this.onPageReady();
            });
        }

        async onPageReady() {
            this.mainLoader = $("#mainLoader");

            // Setup processing.
            document.addEventListener("processing.Busy", this.toggleMainLoader.bind(this, true));
            document.addEventListener("processing.Idle", this.toggleMainLoader.bind(this, false));

            // Setup any settings from the body element data. These settings are added via the Wiser backend and they take preference.
            Object.assign(this.settings, $(document.body).data());

            // this.useExportMode = document.getElementById("useExportMode").checked;

            if (this.settings.trackJsToken) {
                TrackJS.install({
                    token: this.settings.trackJsToken
                });
            }

            // Add logged in user access token to default authorization headers for all jQuery ajax requests.
            $.ajaxSetup({
                headers: { "Authorization": `Bearer ${localStorage.getItem("accessToken")}` }
            });

            // Show an error if the user is no longer logged in.
            const accessTokenExpires = localStorage.getItem("accessTokenExpiresOn");
            if (!accessTokenExpires || accessTokenExpires <= new Date()) {
                Wiser2.alert({
                    title: "Niet ingelogd",
                    content: "U bent niet (meer) ingelogd. Ververs a.u.b. de pagina en probeer het opnieuw."
                });

                this.toggleMainLoader(false);
                return;
            }

            const user = JSON.parse(localStorage.getItem("userData"));
            this.settings.oldStyleUserId = user.oldStyleUserId;
            this.settings.username = user.adminAccountName ? `Happy Horizon (${user.adminAccountName})` : user.name;
            this.settings.adminAccountLoggedIn = !!user.adminAccountName;

            const userData = await Wiser2.getLoggedInUserData(this.settings.wiserApiRoot);
            this.settings.userId = userData.encryptedId;
            this.settings.customerId = userData.encryptedCustomerId;
            this.settings.zeroEncrypted = userData.zeroEncrypted;
            this.settings.wiser2UserId = userData.id;

            // Initialize the rest.
            this.initializeKendoElements();

            // Start the period picker as read-only.
            $("#periodPicker").getKendoDateRangePicker().readonly(true);

            // Update data.
            window.processing.addProcess("dataUpdate");
            await Promise.all([
                this.updateBranches(),
                this.updateData()
            ]);
            window.processing.removeProcess("dataUpdate");

            this.setBindings();
        }

        /**
         * Shows or hides the main (full screen) loader.
         * @param {boolean} show True to show the loader, false to hide it.
         */
        toggleMainLoader(show) {
            this.mainLoader.toggleClass("loading", show);
        }

        setBindings() {
            const itemsTypeFilterButtons = Array.from(document.getElementById("itemsTypeFilterButtons").querySelectorAll("button"));
            itemsTypeFilterButtons.forEach((button) => {
                button.addEventListener("click", (event) => {
                    itemsTypeFilterButtons.filter((btn) => btn !== event.currentTarget).forEach((btn) => btn.classList.remove("selected"));
                    event.currentTarget.classList.add("selected");

                    this.updateItemsDataChart();
                });
            });

            const userDataTypeFilterButtons = Array.from(document.getElementById("userDataTypeFilterButtons").querySelectorAll("button"));
            userDataTypeFilterButtons.forEach((button) => {
                button.addEventListener("click", (event) => {
                    userDataTypeFilterButtons.filter((btn) => btn !== event.currentTarget).forEach((btn) => btn.classList.remove("selected"));
                    event.currentTarget.classList.add("selected");

                    this.updateUserDataChart();
                });
            });

            $("#periodFilter").getKendoDropDownList().bind("change", this.onPeriodFilterChange.bind(this));

            $("#periodPicker").getKendoDateRangePicker().bind("change", async () => {
                window.processing.addProcess("dataUpdate");
                await this.updateData();
                window.processing.removeProcess("dataUpdate");
            });

            $("#branchesSelect").getKendoDropDownList().bind("change", async () => {
                window.processing.addProcess("dataUpdate");
                await this.updateData();
                window.processing.removeProcess("dataUpdate");
            });

            document.getElementById("refreshDataButton").addEventListener("click", () => {
                Wiser2.showConfirmDialog("Weet u zeker dat u de data wilt verversen? Dit is mogelijk een zware belasting op de database.",
                    "Verversen",
                    "Annuleren",
                    "Verversen").then(async () => {
                    window.processing.addProcess("dataUpdate");
                    await this.updateData(true);
                    window.processing.removeProcess("dataUpdate");
                });
            });
        }

        initializeKendoElements() {
            // create ComboBox from select HTML element
            $(".combo-select").kendoComboBox();

            // create DropDownList from select HTML element
            $(".drop-down-list").kendoDropDownList();

            // create DateRangePicker
            $(".daterangepicker").kendoDateRangePicker({
                "messages": {
                    "startLabel": "van",
                    "endLabel": "tot"
                },
                culture: "nl-NL",
                format: "dd/MM/yyyy"
            });

            // create Tiles
            var tileLayout = $("#tiles").kendoTileLayout({
                containers: [{
                    colSpan: 7,
                    rowSpan: 4,
                    header: {
                        text: "Data"
                    },
                    bodyTemplate: kendo.template($("#data-chart-template").html())
                }, {
                    colSpan: 5,
                    rowSpan: 4,
                    header: {
                        text: "Gebruikers"
                    },
                    bodyTemplate: kendo.template($("#users-chart-template").html())
                }, {
                    colSpan: 7,
                    rowSpan: 2,
                    header: {
                        text: "Abonnement"
                    },
                    bodyTemplate: kendo.template($("#subscriptions-chart-template").html())
                }, {
                    colSpan: 5,
                    rowSpan: 2,
                    header: {
                        text: "Update log"
                    },
                    bodyTemplate: kendo.template($("#update-log").html())
                }, {
                    colSpan: 12,
                    rowSpan: 2,
                    header: {
                        text: "Services"
                    },
                    bodyTemplate: kendo.template($("#services-grid-template").html())
                }, {
                    colSpan: 4,
                    rowSpan: 2,
                    header: {
                        text: ""
                    },
                    bodyTemplate: kendo.template($("#numbers").html())
                }, {
                    colSpan: 4,
                    rowSpan: 2,
                    header: {
                        text: ""
                    },
                    bodyTemplate: kendo.template($("#status-chart-template").html())
                }, {
                    colSpan: 4,
                    rowSpan: 2,
                    header: {
                        text: ""
                    },
                    bodyTemplate: kendo.template($("#dataselector-rate").html())
                }],
                columns: 12,
                columnsWidth: 300,
                gap: {
                    columns: 30,
                    rows: 30
                },
                rowsHeight: 125,
                reorderable: true,
                resizable: true,
                resize: function (e) {
                    var rowSpan = e.container.css("grid-column-end");
                    var chart = e.container.find(".k-chart").data("kendoChart");
                    // hide chart labels when the space is limited
                    if (rowSpan === "span 1" && chart) {
                        chart.options.categoryAxis.labels.visible = false;
                        chart.redraw();
                    }
                    // show chart labels when the space is enough
                    if (rowSpan !== "span 1" && chart) {
                        chart.options.categoryAxis.labels.visible = true;
                        chart.redraw();
                    }

                    // for widgets that do not auto resize
                    // https://docs.telerik.com/kendo-ui/styles-and-layout/using-kendo-in-responsive-web-pages
                    kendo.resize(e.container, true);
                }
            }).data("kendoTileLayout");

            // create Charts
            function createChart() {
                // create Column Chart
                $("#data-chart").kendoChart({
                    title: {
                        text: "Aantal items in Wiser"
                    },
                    legend: {
                        position: "top"
                    },
                    seriesDefaults: {
                        type: "column"
                    },
                    series: [{
                        name: "Actief",
                        field: "amountOfItems",
                        color: "#FF6800"
                    }, {
                        name: "Archief",
                        field: "amountOfArchivedItems",
                        color: "#2ECC71"
                    }],
                    valueAxis: {
                        labels: {
                            format: "{0}"
                        },
                        line: {
                            visible: false
                        },
                        axisCrossingValue: 0
                    },
                    categoryAxis: {
                        categories: [],
                        line: {
                            visible: false
                        },
                        labels: {
                            padding: { top: 10 }
                        }
                    },
                    tooltip: {
                        visible: true,
                        format: "{0}",
                        template: "#= series.name #: #= value #"
                    }
                });

                // create Pie Chart
                $("#users-chart").kendoChart({
                    title: {
                        position: "top",
                        text: "Top 10 gebruikers met meeste tijd / aantal x ingelogd"
                    },
                    legend: {
                        visible: false
                    },
                    chartArea: {
                        background: ""
                    },
                    seriesDefaults: {
                        labels: {
                            visible: true,
                            background: "transparent",
                            template: "#= category #: \n #= value#%"
                        }
                    },
                    series: [{
                        type: "pie",
                        startAngle: 90,
                        data: [{
                            category: "Rest",
                            value: 0,
                            color: "#2ECC71"
                        }, {
                            category: "Top 10 gebruikers",
                            value: 0,
                            color: "#FF6800"
                        }]
                    }],
                    tooltip: {
                        visible: true,
                        format: "{0}%"
                    }
                });

                // create Pie Chart
                $("#status-chart").kendoChart({
                    title: {
                        text: "Openstaande agenderingen per gebruiker",
                        visible: false
                    },
                    legend: {
                        visible: false
                    },
                    chartArea: {
                        background: ""
                    },
                    seriesDefaults: {
                        labels: {
                            visible: false,
                            background: "transparent",
                            template: "#= category #: \n #= value#%"
                        }
                    },
                    series: [{
                        type: "pie",
                        startAngle: 90,
                        data: [{
                            category: "Bram",
                            value: 26,
                            color: "#2ECC71"
                        }, {
                            category: "Freek",
                            value: 34,
                            color: "#FFE162"
                        }, {
                            category: "Mandy",
                            value: 30,
                            color: "#FF6800"
                        }, {
                            category: "Paulien",
                            value: 10,
                            color: "#CC0000"
                        }]
                    }],
                    tooltip: {
                        visible: true,
                        template: "#= category #: #= value#%"
                    }
                });
            }

            $(document).ready(createChart);
            $(document).bind("kendo:skinChange", createChart);
        }

        async updateBranches() {
            const branches = await Wiser2.api({
                url: `${this.settings.wiserApiRoot}branches`
            });

            let dataSource = [
                { text: "Actieve branch", value: 0 },
                { text: "Alle branches", value: -1 }
            ];
            dataSource = dataSource.concat(branches.map(ds => {
                return {
                    text: ds.name,
                    value: ds.id
                };
            }));

            const branchesSelect = $("#branchesSelect").getKendoDropDownList();
            branchesSelect.setDataSource(dataSource);

            // Select first item, which is always the current branch.
            branchesSelect.select(0);
        }

        /**
         * Retrieves data from the server.
         */
        async updateData(forceRefresh = false) {
            const dateRange = $("#periodPicker").getKendoDateRangePicker().range();

            let periodFrom = null;
            let periodTo = null;
            if (dateRange !== null) {
                periodFrom = kendo.toString(dateRange.start, "yyyy-MM-dd");
                periodTo = kendo.toString(dateRange.end, "yyyy-MM-dd");
            }

            const getParameters = {
                branchId: $("#branchesSelect").getKendoDropDownList().value()
            };
            if (periodFrom !== null) {
                getParameters.periodFrom = periodFrom;
            }
            if (periodTo !== null) {
                getParameters.periodTo = periodTo;
            }
            if (forceRefresh) {
                getParameters.forceRefresh = forceRefresh;
            }

            const data = await Wiser2.api({
                url: `${this.settings.wiserApiRoot}dashboard`,
                data: getParameters
            });
            if (!data) {
                return;
            }

            // Update entity usage.
            this.itemsData = data.items;
            this.updateItemsDataChart();

            // Update user data.
            const loginCountTotal = data.userLoginCountTop10 + data.userLoginCountOther;
            let loginCountTop10Percentage = 0;
            let loginCountOtherPercentage = 0;

            if (loginCountTotal > 0) {
                loginCountTop10Percentage = Math.round(((data.userLoginCountTop10 / loginCountTotal) * 100) * 10) / 10;
                loginCountOtherPercentage = 100 - loginCountTop10Percentage;
            }

            const loginTimeTotal = data.userLoginTimeTop10 + data.userLoginTimeOther;
            let loginTimeTop10Percentage = 0;
            let loginTimeOtherPercentage = 0;

            if (loginTimeTotal > 0) {
                loginTimeTop10Percentage = Math.round(((data.userLoginTimeTop10 / loginTimeTotal) * 100) * 10) / 10;
                loginTimeOtherPercentage = 100 - loginTimeTop10Percentage;
            }

            this.userData = {
                loginCount: [
                    {
                        category: "Rest",
                        value: loginCountOtherPercentage,
                        color: "#2ECC71"
                    },
                    {
                        category: "Top 10 gebruikers",
                        value: loginCountTop10Percentage,
                        color: "#FF6800"
                    }
                ],
                loginTime: [
                    {
                        category: "Rest",
                        value: loginTimeOtherPercentage,
                        color: "#2ECC71"
                    },
                    {
                        category: "Top 10 gebruikers",
                        value: loginTimeTop10Percentage,
                        color: "#FF6800"
                    }
                ]
            };

            this.updateUserDataChart();
        }

        updateItemsDataChart() {
            const filter = document.getElementById("itemsTypeFilterButtons").querySelector("button.selected").dataset.filter;
            const categories = this.itemsData[filter].map(e => e.entityName);

            const dataChart = $("#data-chart").getKendoChart();
            dataChart.setOptions({
                categoryAxis: {
                    categories: categories
                }
            });
            dataChart.setDataSource(this.itemsData[filter]);
        }

        updateUserDataChart() {
            const filter = document.getElementById("userDataTypeFilterButtons").querySelector("button.selected").dataset.filter;
            const usersChart = $("#users-chart").getKendoChart();
            usersChart.findSeriesByIndex(0).data(this.userData[filter]);
        }

        async onPeriodFilterChange(event) {
            const currentDate = new Date();
            const value = event.sender.value();
            let range = null;
            let readonly = true;
            let updateData = true;
            switch (value) {
                case "currentMonth":
                    range = {
                        start: new Date(currentDate.getFullYear(), currentDate.getMonth(), 1),
                        end: new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 1)
                    };
                    range.end.setDate(range.end.getDate() - 1);
                    break;
                case "lastMonth":
                    range = {
                        start: new Date(currentDate.getFullYear(), currentDate.getMonth() - 1, 1),
                        end: new Date(currentDate.getFullYear(), currentDate.getMonth(), 1)
                    };
                    range.end.setDate(range.end.getDate() - 1);
                    break;
                case "currentYear":
                    range = {
                        start: new Date(currentDate.getFullYear(), 0, 1),
                        end: new Date(currentDate.getFullYear() + 1, 0, 1)
                    };
                    range.end.setDate(range.end.getDate() - 1);
                    break;
                case "lastYear":
                    range = {
                        start: new Date(currentDate.getFullYear() - 1, 0, 1),
                        end: new Date(currentDate.getFullYear(), 0, 1)
                    };
                    range.end.setDate(range.end.getDate() - 1);
                    break;
                case "custom":
                    readonly = false;
                    updateData = false;
                    break;
            }

            const periodPicker = $("#periodPicker").getKendoDateRangePicker();
            if (range !== null) {
                periodPicker.range(range);
            }
            periodPicker.readonly(readonly);

            if (updateData) {
                window.processing.addProcess("dataUpdate");
                await this.updateData();
                window.processing.removeProcess("dataUpdate");
            }
        }
    }

    window.dashboard = new Dashboard(moduleSettings);
})($, moduleSettings);