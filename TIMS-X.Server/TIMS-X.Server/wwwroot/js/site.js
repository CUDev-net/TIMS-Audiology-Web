// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


function searchCustomers() {
    // Declare variables
    var input, filter, table, tr, cols, i, txtValue, match, title, defaultTitle, count;
    input = document.getElementById("customerSearchInput");
    filter = input.value.toUpperCase();
    var cb = document.getElementById('includeInactiveCustomers');
    var includeInactive = cb.checked;
    table = document.getElementById("customerTable");
    tr = table.getElementsByTagName("tr");
    defaultTitle = document.getElementById("customersDefaultTitle");
    title = document.getElementById("filteredTitle");

    if (filter.length === 0 && !includeInactive) {
        title.style.display = "none";
        defaultTitle.style.display = "";
    } else {
        title.style.display = "";
        defaultTitle.style.display = "none";
    }
        
    count = 0;
    // Loop through all table rows, and hide those who don't match the search query
    for (i = 0; i < tr.length; i++) {
        cols = tr[i].getElementsByTagName("td");
        if (cols.length === 0)
            continue;
        match = false;

        for (j = 1; j < 5; ++j) {
            if (cols[j]) {
                txtValue = cols[j].textContent || cols[j].innerText;
                match = txtValue.toUpperCase().indexOf(filter) > -1;
                if (match)
                    break;
            }
        }
        if (!includeInactive && tr[i].className === "disabled") {
            match = false;
        }
        tr[i].style.display = match ? "" : "none";
        count = match ? count + 1 : count;
    }
    title.textContent = count + " match" + (count == 1 ? "" : "es");


}





function filterInactive() {
    var includeInactive = document.getElementById('includeInactive').checked;
    $('#dataTable tr').each(function() {
        $(this).css("display", !includeInactive && $(this).hasClass("disabled") ? "none" : "");
    });
}

var submitButton = null;
function submitFormClick(button) {
    submitButton = button;
}

function submitForm() {
    if (submitButton !== null && submitButton.name === "delete") {
        return confirm("Delete?");
    }
    return true;
}

function submitCustomerForm() {
    if (submitButton !== null && submitButton.name === "delete") {
        return confirm("Confirm delete?");
    }

    var customerId = parseInt($('#Customer_Id').val());

    var permissions = [];

    $("div[id^='permissionContainer_']").each(function (i, e) {
        if (e.style.display === "") {
            var vendorId = parseInt(e.id.split("_")[1]);
            var permissionOptions = $("#vendor_" + vendorId + "_assigned" + " option");
            if (permissionOptions.length !== 0) {
                for (var i = 0; i < permissionOptions.length; ++i) {

                    var permObj = new Object();
                    permObj.CustomerId = customerId;
                    permObj.VendorId = vendorId;
                    permObj.PermissionId = parseInt(permissionOptions[i].value);
                    permissions.push(permObj);
                }
            }
        } 
    });
    var data = JSON.stringify(permissions);
    $('#permissionList').val(data);
    return true;
}


function submitVendorForm() {
    if (submitButton !== null && submitButton.name === "delete") {
        return confirm("Confirm delete?");
    }

    var vendorId = parseInt($('#Vendor_Id').val());

    var permissions = [];
    
    var permissionOptions = $("#dvpAssigned option");
    if (permissionOptions.length !== 0) {
        for (var i = 0; i < permissionOptions.length; ++i) {

            var permObj = new Object();
            permObj.VendorId = vendorId;
            permObj.PermissionId = parseInt(permissionOptions[i].value);
            permissions.push(permObj);
        }
    }
    var data = JSON.stringify(permissions);
    $('#permissionList').val(data);
    return true;
}


function submitPermissionForm() {
    if (submitButton !== null && submitButton.name === "delete") {
        return confirm("Confirm delete?");
    }

    var permissionId = parseInt($('#VendorPermission_Id').val());

    var urls = [];

    var urlOptions = $("#dvpAssigned option");
    if (urlOptions.length !== 0) {
        for (var i = 0; i < urlOptions.length; ++i) {

            var permObj = new Object();
            permObj.PermissionId = permissionId;
            permObj.ApiUrlId = parseInt(urlOptions[i].value);
            urls.push(permObj);
        }
    }
    var data = JSON.stringify(urls);
    $('#urlList').val(data);
    return true;
}

function enablePasswordChangeForm() {
    var pwField = document.getElementById('passwordField');
    var pwrField = document.getElementById('passwordRepeatField');
    var button = document.getElementById('passwordChangeSubmit');

    if (pwField.value.length === 0 || pwrField.value.length === 0)
        button.disabled = true;
    else
        button.disabled = !(pwField.value === pwrField.value);
}



function testConnection(serverId, database) {
    var result = false;
    $.ajax({
        type: "GET",
        url: "/web/Validation/ValidateDatabaseConnection?serverId=" + serverId + "&database=" + encodeURIComponent(database),
        contentType: "application/json; charset=utf-8",
        async: false,
        dataType: "json",
        success: function (response) {
            var parsedResponse = JSON.parse(response);
            if (parsedResponse) {
                result = true;
            } else {
                result = false;
            }
        },
        failure: function (response) {
            result = false;
        }
    });
    return result;
}

function runTest(id) {
    var messageDiv = $("#testConnectionMessage");
    messageDiv.empty();
    var serverId = $('#serverField').val();
    var database = $("#databaseField").val();

    $('<div class="loader my-auto" />').appendTo(messageDiv);

    if (testConnection(serverId, database)) {
        messageDiv.empty();
        $('<div class="alert alert-success"><i class="fas fa-check-circle"></i><span> Connection Verified.</span></div>').appendTo(messageDiv);
    } else {
        messageDiv.empty();
        $('<div class="alert alert-danger"><i class="fas fa-times-circle"></i><span> Connection Failed... </span><i class="far fa-frown"></i></div>').appendTo(messageDiv);
    }
    return false;
}

function runTest2(serverId, database, div) {
    div.innerHTML = '<div class="loader-sm my-auto" />';
    div.style.color = 'black';
    if (testConnection(serverId, database)) {
        div.style.color = 'green';
    } else {
        div.style.color = 'red';
    }
    div.innerHTML = '<i class="fas fa-circle"></i>';
    return false;
}

var toggleEditCreateModeVar = true;

function toggleEditCreateMode() {
    toggleEditCreateModeVar = !toggleEditCreateModeVar;
    $("#editPermissionsButton").html(toggleEditCreateModeVar ? 'done' : 'edit');
    $(".editPermissionsMode").each(function (i) {
        $(this).css('display', toggleEditCreateModeVar ? '' : 'none');
    });
    return false;
}

var toggleEditEditModeVar = false;

function toggleEditEditMode() {
    toggleEditEditModeVar = !toggleEditEditModeVar;
    $("#editPermissionsButton").html(toggleEditEditModeVar ? 'done' : 'edit');
    $(".editPermissionsMode").each(function (i) {
        $(this).css('display', toggleEditEditModeVar ? '' : 'none');
    });
    return false;
}

function toggleVendor(show, vendorId) {
    $('#permissionContainer_' + vendorId).css('display', show ? '' : 'none');
    var dropdownItem = $('#addVendor_' + vendorId);
    if (dropdownItem.length !== 0) {
        dropdownItem.css('display', show ? 'none' : '');
    }
    
    return false;
}

function randomString() {
    return Math.random().toString(36).substr(2); // remove `0.`
};

function generateApiKey() {
    $("#Vendor_ApiKey").val(randomString() + randomString());
    return false;
};


function submitVersion() {
    var ver = $("#SelectedUpdate").find('option:selected');
    return confirm("Install version " + ver.val() + "?\nUsers will be prompted to install this version after the next replication cycle.");
}

(function () {

    $("input[id*='btnright']").click(function (e) {
        var assignedSelector = '#' + e.currentTarget.dataset.assigned + ' option:selected';
        var unassignedSelector = '#' + e.currentTarget.dataset.unassigned;
        var selectedOpts = $(assignedSelector);
        if (selectedOpts.length == 0) {
            alert("Nothing to move.");
            e.preventDefault();
        }
        $(unassignedSelector).append($(selectedOpts).clone());
        $(selectedOpts).remove();
        e.preventDefault();
    });

    $("input[id*='btnleft']").click(function (e) {
        var assignedSelector = '#' + e.currentTarget.dataset.assigned;
        var unassignedSelector = '#' + e.currentTarget.dataset.unassigned + ' option:selected';
        var selectedOpts = $(unassignedSelector);
        if (selectedOpts.length == 0) {
            alert("Nothing to move.");
            e.preventDefault();
        }
        $(assignedSelector).append($(selectedOpts).clone());
        $(selectedOpts).remove();
        e.preventDefault();
    });

    $("input[id*='btnallleft']").click(function (e) {
        var assignedSelector = '#' + e.currentTarget.dataset.assigned;
        var unassignedSelector = '#' + e.currentTarget.dataset.unassigned + ' option';
        var selectedOpts = $(unassignedSelector);
        if (selectedOpts.length == 0) {
            alert("Nothing to move.");
            e.preventDefault();
        }
        $(assignedSelector).append($(selectedOpts).clone());
        $(selectedOpts).remove();
        e.preventDefault();
    });

    $("input[id*='btnallright']").click(function (e) {
        var assignedSelector = '#' + e.currentTarget.dataset.assigned + ' option';
        var unassignedSelector = '#' + e.currentTarget.dataset.unassigned;
        var selectedOpts = $(assignedSelector);
        if (selectedOpts.length == 0) {
            alert("Nothing to move.");
            e.preventDefault();
        }
        $(unassignedSelector).append($(selectedOpts).clone());
        $(selectedOpts).remove();
        e.preventDefault();
    });
    
}(jQuery));


