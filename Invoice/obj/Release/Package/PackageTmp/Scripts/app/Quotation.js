﻿$(document).ready(function () {
    // local db

    function initDatabase() {
        try {
            if (!window.openDatabase) {
                alert("Databases are not supported in this browser.");
            } else {
                var shortName = "Invoice";
                var version = "1.0";
                var displayName = "Invoice";
                var maxSize = 200000; //  bytes
                Inventory = openDatabase(shortName, version, displayName, maxSize);
                createDbItemTable();
            }
        } catch (e) {
            if (e == 2) {
                // Version number mismatch.
                console.log("Invalid database version.");
            } else {
                console.log("Unknown error " + e + ".");
            }
            return;
        }
    }

    function createDbItemTable() {
        Inventory.transaction(function (transaction) {
            transaction.executeSql("DROP TABLE IF EXISTS DbItem", []);
            transaction.executeSql("CREATE TABLE DbItem(ItemCode TEXT NOT NULL,ItemName TEXT NOT NULL,ItemDescription TEXT NOT NULL,HsnSac TEXT NOT NULL,Price DECIMAL NOT NULL,InclusiveTax TEXT NOT NULL,UnitId INT NOT NULL,TaxRate DECIMAL NOT NULL,Cess DECIMAL NOT NULL)", []);

            for (var i = 0; i < itemList.length; i++) {
                transaction.executeSql("INSERT INTO DbItem(ItemCode,ItemName,ItemDescription,HsnSac,Price,InclusiveTax,UnitId,TaxRate,Cess) VALUES(?,?,?,?,?,?,?,?,?)", [itemList[i].ItemCode, itemList[i].ItemName, itemList[i].ItemDescription, itemList[i].HsnSac, itemList[i].Price, itemList[i].InclusiveTax, itemList[i].UnitId, itemList[i].TaxRate, itemList[i].Cess]);
            }
        });
    }

    initDatabase();

    var itemCodeList = [];

    Inventory.transaction(function (tx) {
        tx.executeSql("SELECT ItemCode FROM DbItem", [], function (tx, results) {
            for (var i = 0; i < results.rows.length; i++) {
                itemCodeList.push(results.rows.item(i).ItemCode);
            }
        });
    });

    // end local db

    $("#tblItems").on("focus", ".txtItemCode", function () {
        $(this).autocomplete({
            source: itemCodeList,
            autoFocus: true,
        });
    });

    function addRow() {
        $.ajax({
            url: "../../Content/Quotation_Item_Template.txt",
            dataType: "text",
            success: function (data) {
                $("#tblItems").append(data);

                var rowNo = $("#tblItems .rowItem").length;
                var rowItem = $("#tblItems .rowItem:nth-child(" + rowNo + ")");
                var lblRowNo = $(".lblRowNo", $(rowItem));
                $(lblRowNo).html("Item#" + rowNo);

                $.each(taxList, function (i, tax) {
                    $(".cmbTaxRate").append($("<option>", {
                        value: tax.TaxRate,
                        text: tax.TaxName
                    }));
                });

                $.each(unitList, function (i, unit) {
                    $(".cmbUnit").append($("<option>", {
                        value: unit.UnitId,
                        text: unit.UnitName
                    }));
                });

                if ($("#DiscountOnAll").is(":checked")) {
                    $(".txtDiscount").prop("disabled", true);
                    $(".txtDiscountRs").prop("disabled", true);
                }
                if ($("#PlaceOfSupply").val() == sessionCompanyState) {
                    $(".divCgst").show();
                    $(".divSgst").show();
                    $(".divIgst").hide();
                }
                else {
                    $(".divCgst").hide();
                    $(".divSgst").hide();
                    $(".divIgst").show();
                }
            }
        });
    }

    $("#CustomerId").select2();

    $("#TotalDiscount").prop("disabled", true);
    $("#TotalDiscountRs").prop("disabled", true);

    function getParameterByName(sParam) {
        var sPageUrl = window.location.search.substring(1);
        var sUrlVariables = sPageUrl.split("&");

        for (var i = 0; i < sUrlVariables.length; i++) {
            var sParameterName = sUrlVariables[i].split("=");
            if (sParameterName[0].toLowerCase() == sParam.toLowerCase()) {
                return decodeURI(sParameterName[1]).toUpperCase();
            }
        }
        return null;
    }

    if (getParameterByName("QuotationNo") != null) {
        $.each(itemDetails, function (i, itemDetail) {
            $.ajax({
                url: "../../Content/Quotation_Item_Template.txt",
                dataType: "text",
                success: function (data) {
                    $("#tblItems").append(data);

                    var rowNo = (i + 1);
                    var rowItem = $("#tblItems .rowItem:nth-child(" + rowNo + ")");
                    var lblRowNo = $(".lblRowNo", $(rowItem));
                    $(lblRowNo).html("Item#" + rowNo);

                    $.each(taxList, function (k, tax) {
                        $(".cmbTaxRate").append($("<option>", {
                            value: tax.TaxRate,
                            text: tax.TaxName
                        }));
                    });

                    $.each(unitList, function (i, unit) {
                        $(".cmbUnit").append($("<option>", {
                            value: unit.UnitId,
                            text: unit.UnitName
                        }));
                    });

                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .txtItemCode").val(itemDetail.ItemCode);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .txtItemName").val(itemDetail.ItemName);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .txtItemDescription").val(itemDetail.ItemDescription);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .txtHsnSac").val(itemDetail.HsnSac);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .txtPrice").val(itemDetail.Price);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .chkInclusiveTax").prop("checked", itemDetail.InclusiveTax);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .txtQuantity").val(itemDetail.Quantity);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .cmbUnit").val(itemDetail.UnitId);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .txtDiscount").val(itemDetail.Discount);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .txtDiscountRs").val(itemDetail.DiscountRs);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .lblTaxableValue").html(itemDetail.TaxableValue);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .cmbTaxRate").val(itemDetail.TaxRate);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .lblCgstRate").html(itemDetail.CgstRate);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .lblSgstRate").html(itemDetail.SgstRate);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .lblIgstRate").html(itemDetail.IgstRate);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .lblCgstAmount").html(itemDetail.CgstAmount);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .lblSgstAmount").html(itemDetail.SgstAmount);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .lblIgstAmount").html(itemDetail.IgstAmount);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .txtCess").val(itemDetail.Cess);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .lblCessRs").val(itemDetail.CessRs);
                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .lblAmount").html(itemDetail.Amount);

                    $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .txtQuantity").prop("disabled", false);

                    if ($("#DiscountOnAll").is(":checked")) {
                        $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .txtDiscount").prop("disabled", true);
                        $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .txtDiscountRs").prop("disabled", true);
                    }
                    else {
                        $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .txtDiscount").prop("disabled", false);
                        $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .txtDiscountRs").prop("disabled", false);
                    }

                    if ($("#PlaceOfSupply").val() == sessionCompanyState) {
                        $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .divCgst").show();
                        $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .divSgst").show();
                        $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .divIgst").hide();
                    }
                    else {
                        $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .divCgst").hide();
                        $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .divSgst").hide();
                        $("#tblItems .rowItem:nth-child(" + (i + 1) + ") .divIgst").show();
                    }
                }
            });
        });

        $("#DiscountOnAll").prop("checked", modelDiscountOnAll);

        if ($("#DiscountOnAll").is(":checked")) {
            $("#TotalDiscount").prop("disabled", false);
            $("#TotalDiscountRs").prop("disabled", false);
        }
        else {
            $("#TotalDiscount").prop("disabled", true);
        }

        $("#RoundOff").prop("checked", modelRoundOff);
    }

    function CalcRowItem(rowItem) {
        var txtPrice = $(".txtPrice", $(rowItem));
        var chkInclusiveTax = $(".chkInclusiveTax", $(rowItem));
        var cmbTaxRate = $(".cmbTaxRate", $(rowItem));
        var txtQuantity = $(".txtQuantity", $(rowItem));
        var txtDiscount = $(".txtDiscount", $(rowItem));
        var txtDiscountRs = $(".txtDiscountRs", $(rowItem));
        var lblTaxableValue = $(".lblTaxableValue", $(rowItem));
        var lblCgstRate = $(".lblCgstRate", $(rowItem));
        var lblSgstRate = $(".lblSgstRate", $(rowItem));
        var lblIgstRate = $(".lblIgstRate", $(rowItem));
        var lblCgstAmount = $(".lblCgstAmount", $(rowItem));
        var lblSgstAmount = $(".lblSgstAmount", $(rowItem));
        var lblIgstAmount = $(".lblIgstAmount", $(rowItem));
        var txtCess = $(".txtCess", $(rowItem));
        var lblCessRs = $(".lblCessRs", $(rowItem));
        var lblAmount = $(".lblAmount", $(rowItem));

        if ($(chkInclusiveTax).is(":checked")) {
            var amountAfterTax = ($(txtPrice).val() * $(txtQuantity).val()) / (1 + ($(cmbTaxRate).val() / 100));

            var discountRs = amountAfterTax / 100 * $(txtDiscount).val();
            var amountAfterDiscount = amountAfterTax - discountRs;

            var gstAfterDiscount = amountAfterDiscount / 100 * $(cmbTaxRate).val();
            var cessAfterDiscount = amountAfterDiscount / 100 * $(txtCess).val();
            var taxableValue = amountAfterDiscount;
            var amount = amountAfterDiscount + gstAfterDiscount + cessAfterDiscount;
        }
        else {
            var discountRs = ($(txtPrice).val() * $(txtQuantity).val()) / 100 * $(txtDiscount).val();
            var amountAfterDiscount = ($(txtPrice).val() * $(txtQuantity).val()) - discountRs;

            var gstAfterDiscount = amountAfterDiscount / 100 * $(cmbTaxRate).val();
            var cessAfterDiscount = amountAfterDiscount / 100 * $(txtCess).val();
            var taxableValue = amountAfterDiscount;
            var amount = amountAfterDiscount + gstAfterDiscount + cessAfterDiscount;
        }

        $(txtDiscountRs).val(discountRs.toFixed(2));
        $(lblTaxableValue).html(taxableValue.toFixed(2));
        $(lblCessRs).html(cessAfterDiscount.toFixed(2));

        if ($("#PlaceOfSupply").val() == sessionCompanyState) {
            $(lblCgstRate).html(($(cmbTaxRate).val() / 2).toFixed(2));
            $(lblSgstRate).html(($(cmbTaxRate).val() / 2).toFixed(2));
            $(lblIgstRate).html("0.00");

            $(lblCgstAmount).html((gstAfterDiscount / 2).toFixed(2));
            $(lblSgstAmount).html((gstAfterDiscount / 2).toFixed(2));
            $(lblIgstAmount).html("0.00");
        }
        else {
            $(lblCgstRate).html("0.00");
            $(lblSgstRate).html("0.00");
            $(lblIgstRate).html($(cmbTaxRate).val());

            $(lblCgstAmount).html("0.00");
            $(lblSgstAmount).html("0.00");
            $(lblIgstAmount).html(gstAfterDiscount.toFixed(2));
        }

        $(lblAmount).html(amount.toFixed(2));

        calcTotal();
    }

    function calcRow() {
        var itemCount = $("#tblItems .rowItem").length;

        for (var i = 1; i <= itemCount; i++) {
            var rowItem = $("#tblItems .rowItem:nth-child(" + i + ")");

            var txtPrice = $(".txtPrice", $(rowItem));
            var chkInclusiveTax = $(".chkInclusiveTax", $(rowItem));
            var cmbTaxRate = $(".cmbTaxRate", $(rowItem));
            var txtQuantity = $(".txtQuantity", $(rowItem));
            var txtDiscount = $(".txtDiscount", $(rowItem));
            var txtDiscountRs = $(".txtDiscountRs", $(rowItem));
            var lblTaxableValue = $(".lblTaxableValue", $(rowItem));
            var divCgst = $(".divCgst", $(rowItem));
            var divSgst = $(".divSgst", $(rowItem));
            var divIgst = $(".divIgst", $(rowItem));
            var lblCgstRate = $(".lblCgstRate", $(rowItem));
            var lblSgstRate = $(".lblSgstRate", $(rowItem));
            var lblIgstRate = $(".lblIgstRate", $(rowItem));
            var lblCgstAmount = $(".lblCgstAmount", $(rowItem));
            var lblSgstAmount = $(".lblSgstAmount", $(rowItem));
            var lblIgstAmount = $(".lblIgstAmount", $(rowItem));
            var txtCess = $(".txtCess", $(rowItem));
            var lblCessRs = $(".lblCessRs", $(rowItem));
            var lblAmount = $(".lblAmount", $(rowItem));

            if ($(chkInclusiveTax).is(":checked")) {
                var amountAfterTax = ($(txtPrice).val() * $(txtQuantity).val()) / (1 + ($(cmbTaxRate).val() / 100));

                var discountRs = amountAfterTax / 100 * $(txtDiscount).val();
                var amountAfterDiscount = amountAfterTax - discountRs;

                var gstAfterDiscount = amountAfterDiscount / 100 * $(cmbTaxRate).val();
                var cessAfterDiscount = amountAfterDiscount / 100 * $(txtCess).val();
                var taxableValue = amountAfterDiscount;
                var amount = amountAfterDiscount + gstAfterDiscount + cessAfterDiscount;
            }
            else {
                var discountRs = ($(txtPrice).val() * $(txtQuantity).val()) / 100 * $(txtDiscount).val();
                var amountAfterDiscount = ($(txtPrice).val() * $(txtQuantity).val()) - discountRs;

                var gstAfterDiscount = amountAfterDiscount / 100 * $(cmbTaxRate).val();
                var cessAfterDiscount = amountAfterDiscount / 100 * $(txtCess).val();
                var taxableValue = amountAfterDiscount;
                var amount = amountAfterDiscount + gstAfterDiscount + cessAfterDiscount;
            }

            $(txtDiscountRs).val(discountRs.toFixed(2));
            $(lblTaxableValue).html(taxableValue.toFixed(2));
            $(lblCessRs).html(cessAfterDiscount.toFixed(2));

            if ($("#PlaceOfSupply").val() == sessionCompanyState) {
                $(lblCgstRate).html(($(cmbTaxRate).val() / 2).toFixed(2));
                $(lblSgstRate).html(($(cmbTaxRate).val() / 2).toFixed(2));
                $(lblIgstRate).html("0.00");

                $(lblCgstAmount).html((gstAfterDiscount / 2).toFixed(2));
                $(lblSgstAmount).html((gstAfterDiscount / 2).toFixed(2));
                $(lblIgstAmount).html("0.00");

                $(divCgst).show();
                $(divSgst).show();
                $(divIgst).hide();
            }
            else {
                $(lblCgstRate).html("0.00");
                $(lblSgstRate).html("0.00");
                $(lblIgstRate).html($(cmbTaxRate).val());

                $(lblCgstAmount).html("0.00");
                $(lblSgstAmount).html("0.00");
                $(lblIgstAmount).html(gstAfterDiscount.toFixed(2));

                $(divCgst).hide();
                $(divSgst).hide();
                $(divIgst).show();
            }

            $(lblAmount).html(amount.toFixed(2));
        }

        calcTotal();
    }

    function calcTotal() {
        var totalValue = 0;
        var totalDiscountRs = 0;
        var totalTaxableValue = 0;
        var cGst = 0;
        var sGst = 0;
        var iGst = 0;
        var totalTaxValue = 0;
        var cess = 0;
        var total = 0;

        var itemCount = $("#tblItems .rowItem").length;

        for (var i = 1; i <= itemCount; i++) {
            var rowItem = $("#tblItems .rowItem:nth-child(" + i + ")");

            var txtPrice = $(".txtPrice", $(rowItem));
            var chkInclusiveTax = $(".chkInclusiveTax", $(rowItem));
            var txtQuantity = $(".txtQuantity", $(rowItem));
            var txtDiscountRs = $(".txtDiscountRs", $(rowItem));
            var lblTaxableValue = $(".lblTaxableValue", $(rowItem));
            var lblCgstAmount = $(".lblCgstAmount", $(rowItem));
            var lblSgstAmount = $(".lblSgstAmount", $(rowItem));
            var lblIgstAmount = $(".lblIgstAmount", $(rowItem));
            var lblCessRs = $(".lblCessRs", $(rowItem));
            var lblAmount = $(".lblAmount", $(rowItem));

            totalDiscountRs += Number($(txtDiscountRs).val());
            totalTaxableValue += Number($(lblTaxableValue).html());
            cGst += Number($(lblCgstAmount).html());
            sGst += Number($(lblSgstAmount).html());
            iGst += Number($(lblIgstAmount).html());
            totalTaxValue += (Number($(lblCgstAmount).html()) + Number($(lblSgstAmount).html()) + Number($(lblIgstAmount).html()));
            cess += Number($(lblCessRs).html());
            total += Number($(lblAmount).html());
            totalValue += Number($(lblTaxableValue).html()) - Number($(txtDiscountRs).val());
        }

        $("#TotalValue").val(totalValue.toFixed(2));
        $("#TotalDiscountRs").val(totalDiscountRs.toFixed(2));
        $("#TotalTaxableValue").val(totalTaxableValue.toFixed(2));
        $("#TotalCgstAmount").val(cGst.toFixed(2));
        $("#TotalSgstAmount").val(sGst.toFixed(2));
        $("#TotalIgstAmount").val(iGst.toFixed(2));
        $("#TotalTaxValue").val(totalTaxValue.toFixed(2));
        $("#TotalCessRs").val(cess.toFixed(2));
        $("#Total").html(total.toFixed(2));

        if ($("#RoundOff").is(":checked")) {
            $("#RoundOffValue").val((total.toFixed() - total).toFixed(2));
            $("#Total").html(total.toFixed());
        }
        else {
            $("#RoundOffValue").val("0.00");
            $("#Total").html(total.toFixed(2));
        }
    }

    function setRowNo() {
        var itemCount = $("#tblItems .rowItem").length;

        for (var i = 1; i <= itemCount; i++) {
            var rowItem = $("#tblItems .rowItem:nth-child(" + i + ")");

            var lblRowNo = $(".lblRowNo", $(rowItem));

            $(lblRowNo).html("Item#" + i);
        }
    }

    function addDays(startDate, numberOfDays) {
        var newStartDate = new Date(startDate);
        var newDate = new Date(newStartDate.getTime() + numberOfDays * 86400000);

        var dd = ("0" + newDate.getDate()).slice(-2);
        var mm = ("0" + (newDate.getMonth() + 1)).slice(-2);
        var yyyy = newDate.getFullYear();

        return (yyyy + "-" + mm + "-" + dd);
    }

    $("#QuotationDate").change(function () {
        var quotationDate = $("#QuotationDate").val();
        var paymentTerm = $("#PaymentTerm").val();

        if (quotationDate != "") {
            if (paymentTerm != "-1") {
                $("#DueDate").val(addDays(quotationDate, paymentTerm));
            }
        }
    });

    $("#PaymentTerm").change(function () {
        var quotationDate = $("#QuotationDate").val();
        var paymentTerm = $("#PaymentTerm").val();

        if (quotationDate != "") {
            if (paymentTerm != "-1") {
                $("#DueDate").val(addDays(quotationDate, paymentTerm));
            }
        }
    });

    $("#DueDate").change(function () {
        $("#PaymentTerm").val("-1");
    });

    $("#PlaceOfSupply").change(function () {
        calcRow();
    });

    $("#btnAddRow").click(function () {
        addRow();
    });

    $("#tblItems").on("change", ".txtItemCode", function () {
        var rowItem = this.closest(".rowItem");

        var txtItemCode = $(".txtItemCode", $(rowItem));
        var txtItemName = $(".txtItemName", $(rowItem));
        var txtItemDescription = $(".txtItemDescription", $(rowItem));
        var txtHsnSac = $(".txtHsnSac", $(rowItem));
        var txtPrice = $(".txtPrice", $(rowItem));
        var chkInclusiveTax = $(".chkInclusiveTax", $(rowItem));
        var cmbUnit = $(".cmbUnit", $(rowItem));
        var cmbTaxRate = $(".cmbTaxRate", $(rowItem));
        var txtCess = $(".txtCess", $(rowItem));
        var txtQuantity = $(".txtQuantity", $(rowItem));
        var txtDiscount = $(".txtDiscount", $(rowItem));

        Inventory.transaction(function (tx) {
            tx.executeSql("SELECT ItemName,ItemDescription,HsnSac,Price,InclusiveTax,UnitId,TaxRate,Cess FROM DbItem where ItemCode=?", [txtItemCode.val()], function (tx, results) {
                if (results.rows.length > 0) {
                    $(txtItemName).val(results.rows.item(0).ItemName);
                    $(txtItemDescription).val(results.rows.item(0).ItemDescription);
                    $(txtHsnSac).val(results.rows.item(0).HsnSac);
                    $(txtPrice).val(results.rows.item(0).Price);
                    $(chkInclusiveTax).prop("checked", $.parseJSON(results.rows.item(0).InclusiveTax));
                    $(cmbUnit).val(results.rows.item(0).UnitId);
                    $(cmbTaxRate).val(results.rows.item(0).TaxRate);
                    $(txtCess).val(results.rows.item(0).Cess);
                    $(txtQuantity).val(1);

                    if ($("#DiscountOnAll").is(":checked"))
                        $(txtDiscount).val($("#TotalDiscount").val());

                    CalcRowItem(rowItem);

                    $(txtQuantity).prop("disabled", false);
                }
                else {
                    $(txtItemCode).val("");
                    $(txtItemName).val("");
                    $(txtItemDescription).val("");
                    $(txtHsnSac).val("");
                    $(txtPrice).val("0.00");
                    $(chkInclusiveTax).prop("checked", false);
                    $(cmbUnit).html("");
                    $(cmbTaxRate).val("0");
                    $(txtCess).val("0");
                    $(txtQuantity).val("0.00");

                    if ($("#DiscountOnAll").is(":checked"))
                        $(txtDiscount).val($("#TotalDiscount").val());

                    CalcRowItem(rowItem);

                    $(txtQuantity).prop("disabled", true);
                    $(txtItemCode).focus();
                }
            });
        });
    });

    $("#tblItems").on("change", ".txtPrice", function () {
        var rowItem = this.closest(".rowItem");

        CalcRowItem(rowItem);
    });

    $("#tblItems").on("change", ".chkInclusiveTax", function () {
        var rowItem = this.closest(".rowItem");

        CalcRowItem(rowItem);
    });

    $("#tblItems").on("change", ".cmbTaxRate", function () {
        var rowItem = this.closest(".rowItem");

        CalcRowItem(rowItem);
    });

    $("#tblItems").on("change", ".txtCess", function () {
        var rowItem = this.closest(".rowItem");

        CalcRowItem(rowItem);
    });

    $("#tblItems").on("change", ".txtQuantity", function () {
        var rowItem = this.closest(".rowItem");

        CalcRowItem(rowItem);
    });

    $("#tblItems").on("change", ".txtDiscount", function () {
        var rowItem = this.closest(".rowItem");

        var txtPrice = $(".txtPrice", $(rowItem));
        var txtQuantity = $(".txtQuantity", $(rowItem));
        var txtDiscount = $(".txtDiscount", $(rowItem));
        var txtDiscountRs = $(".txtDiscountRs", $(rowItem));

        var discountRs = ($(txtPrice).val() * $(txtQuantity).val()) / 100 * $(txtDiscount).val();
        $(txtDiscountRs).val(discountRs.toFixed(2));

        CalcRowItem(rowItem);
    });

    $("#tblItems").on("click", ".btnRemoveItemRow", function () {
        var rowItem = this.closest(".rowItem");

        var btnRemoveItemRow = $(".btnRemoveItemRow", $(rowItem));
        var btnRestoreItemRow = $(".btnRestoreItemRow", $(rowItem));
        var btnConfirmRemoveItemRow = $(".btnConfirmRemoveItemRow", $(rowItem));

        btnRemoveItemRow.hide();
        btnRestoreItemRow.show();
        btnConfirmRemoveItemRow.show();
    });

    $("#tblItems").on("click", ".btnRestoreItemRow", function () {
        var rowItem = this.closest(".rowItem");

        var btnRemoveItemRow = $(".btnRemoveItemRow", $(rowItem));
        var btnRestoreItemRow = $(".btnRestoreItemRow", $(rowItem));
        var btnConfirmRemoveItemRow = $(".btnConfirmRemoveItemRow", $(rowItem));

        btnRemoveItemRow.show();
        btnRestoreItemRow.hide();
        btnConfirmRemoveItemRow.hide();
    });

    $("#tblItems").on("click", ".btnConfirmRemoveItemRow", function () {
        var rowItem = this.closest(".rowItem");

        $(rowItem).remove();

        calcTotal();

        setRowNo();
    });

    $("#DiscountOnAll").change(function () {
        if ($("#DiscountOnAll").is(":checked")) {
            $("#TotalDiscount").prop("disabled", false);
            $("#TotalDiscountRs").prop("disabled", false);
        }
        else {
            $("#TotalDiscount").prop("disabled", true);
        }

        $("#TotalDiscount").val("0");

        var itemCount = $("#tblItems .rowItem").length;

        for (var i = 1; i <= itemCount; i++) {
            var rowItem = $("#tblItems .rowItem:nth-child(" + i + ")");

            var txtPrice = $(".txtPrice", $(rowItem));
            var chkInclusiveTax = $(".chkInclusiveTax", $(rowItem));
            var cmbTaxRate = $(".cmbTaxRate", $(rowItem));
            var txtQuantity = $(".txtQuantity", $(rowItem));
            var txtDiscount = $(".txtDiscount", $(rowItem));
            var txtDiscountRs = $(".txtDiscountRs", $(rowItem));
            var lblTaxableValue = $(".lblTaxableValue", $(rowItem));
            var lblCgstRate = $(".lblCgstRate", $(rowItem));
            var lblSgstRate = $(".lblSgstRate", $(rowItem));
            var lblIgstRate = $(".lblIgstRate", $(rowItem));
            var lblCgstAmount = $(".lblCgstAmount", $(rowItem));
            var lblSgstAmount = $(".lblSgstAmount", $(rowItem));
            var lblIgstAmount = $(".lblIgstAmount", $(rowItem));
            var txtCess = $(".txtCess", $(rowItem));
            var lblCessRs = $(".lblCessRs", $(rowItem));
            var lblAmount = $(".lblAmount", $(rowItem));

            if ($("#DiscountOnAll").is(":checked")) {
                $("#tblItems .rowItem:nth-child(" + i + ") .txtDiscount").prop("disabled", true);
                $("#tblItems .rowItem:nth-child(" + i + ") .txtDiscountRs").prop("disabled", true);
            }
            else {
                $("#tblItems .rowItem:nth-child(" + i + ") .txtDiscount").prop("disabled", false);
                $("#tblItems .rowItem:nth-child(" + i + ") .txtDiscountRs").prop("disabled", false);
            }

            $(txtDiscount).val("0");

            if ($(chkInclusiveTax).is(":checked")) {
                var amountAfterTax = ($(txtPrice).val() * $(txtQuantity).val()) / (1 + ($(cmbTaxRate).val() / 100));

                var discountRs = amountAfterTax / 100 * $(txtDiscount).val();
                var amountAfterDiscount = amountAfterTax - discountRs;

                var gstAfterDiscount = amountAfterDiscount / 100 * $(cmbTaxRate).val();
                var cessAfterDiscount = amountAfterDiscount / 100 * $(txtCess).val();
                var taxableValue = amountAfterDiscount;
                var amount = amountAfterDiscount + gstAfterDiscount + cessAfterDiscount;
            }
            else {
                var discountRs = ($(txtPrice).val() * $(txtQuantity).val()) / 100 * $(txtDiscount).val();
                var amountAfterDiscount = ($(txtPrice).val() * $(txtQuantity).val()) - discountRs;

                var gstAfterDiscount = amountAfterDiscount / 100 * $(cmbTaxRate).val();
                var cessAfterDiscount = amountAfterDiscount / 100 * $(txtCess).val();
                var taxableValue = amountAfterDiscount;
                var amount = amountAfterDiscount + gstAfterDiscount + cessAfterDiscount;
            }

            $(txtDiscountRs).val(discountRs.toFixed(2));
            $(lblTaxableValue).html(taxableValue.toFixed(2));
            $(lblCessRs).html(cessAfterDiscount.toFixed(2));

            if ($("#PlaceOfSupply").val() == sessionCompanyState) {
                $(lblCgstRate).html(($(cmbTaxRate).val() / 2).toFixed(2));
                $(lblSgstRate).html(($(cmbTaxRate).val() / 2).toFixed(2));
                $(lblIgstRate).html("0.00");

                $(lblCgstAmount).html((gstAfterDiscount / 2).toFixed(2));
                $(lblSgstAmount).html((gstAfterDiscount / 2).toFixed(2));
                $(lblIgstAmount).html("0.00");
            }
            else {
                $(lblCgstRate).html("0.00");
                $(lblSgstRate).html("0.00");
                $(lblIgstRate).html($(cmbTaxRate).val());

                $(lblCgstAmount).html("0.00");
                $(lblSgstAmount).html("0.00");
                $(lblIgstAmount).html(gstAfterDiscount.toFixed(2));
            }

            $(lblAmount).html(amount.toFixed(2));
        }

        calcTotal();
    });

    $("#TotalDiscount").change(function () {
        var itemCount = $("#tblItems .rowItem").length;

        for (var i = 1; i <= itemCount; i++) {
            var rowItem = $("#tblItems .rowItem:nth-child(" + i + ")");

            var txtPrice = $(".txtPrice", $(rowItem));
            var chkInclusiveTax = $(".chkInclusiveTax", $(rowItem));
            var cmbTaxRate = $(".cmbTaxRate", $(rowItem));
            var txtQuantity = $(".txtQuantity", $(rowItem));
            var txtDiscount = $(".txtDiscount", $(rowItem));
            var txtDiscountRs = $(".txtDiscountRs", $(rowItem));
            var lblTaxableValue = $(".lblTaxableValue", $(rowItem));
            var lblCgstRate = $(".lblCgstRate", $(rowItem));
            var lblSgstRate = $(".lblSgstRate", $(rowItem));
            var lblIgstRate = $(".lblIgstRate", $(rowItem));
            var lblCgstAmount = $(".lblCgstAmount", $(rowItem));
            var lblSgstAmount = $(".lblSgstAmount", $(rowItem));
            var lblIgstAmount = $(".lblIgstAmount", $(rowItem));
            var txtCess = $(".txtCess", $(rowItem));
            var lblCessRs = $(".lblCessRs", $(rowItem));
            var lblAmount = $(".lblAmount", $(rowItem));

            $(txtDiscount).val($("#TotalDiscount").val());

            if ($(chkInclusiveTax).is(":checked")) {
                var amountAfterTax = ($(txtPrice).val() * $(txtQuantity).val()) / (1 + ($(cmbTaxRate).val() / 100));

                var discountRs = amountAfterTax / 100 * $(txtDiscount).val();
                var amountAfterDiscount = amountAfterTax - discountRs;

                var gstAfterDiscount = amountAfterDiscount / 100 * $(cmbTaxRate).val();
                var cessAfterDiscount = amountAfterDiscount / 100 * $(txtCess).val();
                var taxableValue = amountAfterDiscount;
                var amount = amountAfterDiscount + gstAfterDiscount + cessAfterDiscount;
            }
            else {
                var discountRs = ($(txtPrice).val() * $(txtQuantity).val()) / 100 * $(txtDiscount).val();
                var amountAfterDiscount = ($(txtPrice).val() * $(txtQuantity).val()) - discountRs;

                var gstAfterDiscount = amountAfterDiscount / 100 * $(cmbTaxRate).val();
                var cessAfterDiscount = amountAfterDiscount / 100 * $(txtCess).val();
                var taxableValue = amountAfterDiscount;
                var amount = amountAfterDiscount + gstAfterDiscount + cessAfterDiscount;
            }

            $(txtDiscountRs).val(discountRs.toFixed(2));
            $(lblCessRs).html(cessAfterDiscount.toFixed(2));
            $(lblTaxableValue).html(taxableValue.toFixed(2));

            if ($("#PlaceOfSupply").val() == sessionCompanyState) {
                $(lblCgstRate).html(($(cmbTaxRate).val() / 2).toFixed(2));
                $(lblSgstRate).html(($(cmbTaxRate).val() / 2).toFixed(2));
                $(lblIgstRate).html("0.00");

                $(lblCgstAmount).html((gstAfterDiscount / 2).toFixed(2));
                $(lblSgstAmount).html((gstAfterDiscount / 2).toFixed(2));
                $(lblIgstAmount).html("0.00");
            }
            else {
                $(lblCgstRate).html("0.00");
                $(lblSgstRate).html("0.00");
                $(lblIgstRate).html($(cmbTaxRate).val());

                $(lblCgstAmount).html("0.00");
                $(lblSgstAmount).html("0.00");
                $(lblIgstAmount).html(gstAfterDiscount.toFixed(2));
            }

            $(lblAmount).html(amount.toFixed(2));
        }

        calcTotal();
    });

    $("#RoundOff").change(function () {
        calcTotal();
    });

    $(document).on('keyup keypress', 'form input[type="text"]', function (e) {
        if (e.keyCode == 13) {
            e.preventDefault();
            return false;
        }
    });

    $(document).on("submit", "#formQuotation", function () {
        $("#btnSubmit").val("Please wait..");
        $("#btnSubmit").attr("disabled", "disabled");

        calcRow();
        calcTotal();

        var itemDetails = [];

        var itemCount = $("#tblItems .rowItem").length;

        for (var i = 1; i <= itemCount; i++) {
            var rowItem = $("#tblItems .rowItem:nth-child(" + i + ")");

            var txtItemCode = $(".txtItemCode", $(rowItem));

            if ($(txtItemCode).val() != "") {
                var txtItemName = $(".txtItemName", $(rowItem));
                var txtItemDescription = $(".txtItemDescription", $(rowItem));
                var txtHsnSac = $(".txtHsnSac", $(rowItem));
                var txtPrice = $(".txtPrice", $(rowItem));
                var chkInclusiveTax = $(".chkInclusiveTax", $(rowItem));
                var cmbTaxRate = $(".cmbTaxRate", $(rowItem));
                var txtQuantity = $(".txtQuantity", $(rowItem));
                var cmbUnit = $(".cmbUnit", $(rowItem));
                var txtDiscount = $(".txtDiscount", $(rowItem));
                var txtDiscountRs = $(".txtDiscountRs", $(rowItem));
                var lblTaxableValue = $(".lblTaxableValue", $(rowItem));
                var lblCgstRate = $(".lblCgstRate", $(rowItem));
                var lblSgstRate = $(".lblSgstRate", $(rowItem));
                var lblIgstRate = $(".lblIgstRate", $(rowItem));
                var lblCgstAmount = $(".lblCgstAmount", $(rowItem));
                var lblSgstAmount = $(".lblSgstAmount", $(rowItem));
                var lblIgstAmount = $(".lblIgstAmount", $(rowItem));
                var txtCess = $(".txtCess", $(rowItem));
                var lblCessRs = $(".lblCessRs", $(rowItem));
                var lblAmount = $(".lblAmount", $(rowItem));

                if ($(chkInclusiveTax).is(":checked"))
                    var taxablePrice = $(lblTaxableValue).html() / $(txtQuantity).val();
                else
                    var taxablePrice = $(txtPrice).val();

                itemDetails.push({
                    ItemCode: $(txtItemCode).val(),
                    ItemName: $(txtItemName).val(),
                    ItemDescription: $(txtItemDescription).val(),
                    HsnSac: $(txtHsnSac).val(),
                    Price: $(txtPrice).val(),
                    InclusiveTax: $(chkInclusiveTax).is(":checked"),
                    Quantity: $(txtQuantity).val(),
                    UnitId: $(cmbUnit).val(),
                    SubAmount: taxablePrice * Number($(txtQuantity).val()),
                    Discount: $(txtDiscount).val(),
                    DiscountRs: $(txtDiscountRs).val(),
                    TaxablePrice: taxablePrice,
                    TaxableValue: $(lblTaxableValue).html(),
                    TaxRate: $(cmbTaxRate).val(),
                    CgstRate: $(lblCgstRate).html(),
                    SgstRate: $(lblSgstRate).html(),
                    IgstRate: $(lblIgstRate).html(),
                    CgstAmount: $(lblCgstAmount).html(),
                    SgstAmount: $(lblSgstAmount).html(),
                    IgstAmount: $(lblIgstAmount).html(),
                    Cess: $(txtCess).val(),
                    CessRs: $(lblCessRs).html(),
                    Amount: $(lblAmount).html()
                });
            }
        }

        var model = {
            QuotationNo: getParameterByName("QuotationNo"),
            Prefix: $("#Prefix").val(),
            QuotationName: $("#QuotationName").val(),
            QuotationDate: $("#QuotationDate").val(),
            CustomerId: $("#CustomerId").val(),
            PlaceOfSupply: $("#PlaceOfSupply").val(),
            ReverseCharge: $("#ReverseCharge").val(),
            DiscountOnAll: $("#DiscountOnAll").is(":checked"),
            TotalDiscount: $("#TotalDiscount").val(),
            DeliveryNote: $("#DeliveryNote").val(),
            DeliveryNoteDate: $("#DeliveryNoteDate").val(),
            PaymentMethod: $("#PaymentMethod").val(),
            PaymentTerm: $("#PaymentTerm option:selected").text(),
            DueDate: $("#DueDate").val(),
            SupplierReference: $("#SupplierReference").val(),
            OtherReference: $("#OtherReference").val(),
            PoNo: $("#PoNo").val(),
            PoDate: $("#PoDate").val(),
            DespatchDocumentNo: $("#DespatchDocumentNo").val(),
            DespatchedThrough: $("#DespatchedThrough").val(),
            TotalValue: $("#TotalValue").val(),
            TotalDiscountRs: $("#TotalDiscountRs").val(),
            TotalTaxableValue: $("#TotalTaxableValue").val(),
            TotalCgstAmount: $("#TotalCgstAmount").val(),
            TotalSgstAmount: $("#TotalSgstAmount").val(),
            TotalIgstAmount: $("#TotalIgstAmount").val(),
            TotalTaxValue: $("#TotalTaxValue").val(),
            TotalCessRs: $("#TotalCessRs").val(),
            RoundOff: $("#RoundOff").is(":checked"),
            RoundOffValue: $("#RoundOffValue").val(),
            Total: $("#Total").html(),
            Terms: $("#Terms").val(),
            QuotationItemDetails: itemDetails,
        };

        $.ajax({
            context: document.body,
            data: '{Model: ' + JSON.stringify(model) + '}',
            error: function (e) { swal(e.Message, "", "error"); },
            success: function (data) {
                if (data.success === true) {
                    window.open("/Sale/PrintQuotation?QuotationNo=" + data.QuotationNo, "_blank");
                    window.location = "/Sale/QuotationList";
                } else if (data.success === false) {
                    swal(data.Message, "", "error");
                }
            },
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            url: "/Sale/SaveQuotation"
        });

        return false;
    });
});