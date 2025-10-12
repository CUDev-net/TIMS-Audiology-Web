
$(document).ready(function () {
    $("#availableFieldsList").sortable({
        handle: '.grip-handle',
        placeholder: 'list-item-placeholder',
        start: function (e, ui) {
            ui.placeholder.height(ui.item.height());
        },
        over: function (event, ui) {
            var cl = ui.item.attr('class');
            $('.list-item-placeholder').addClass(cl);
        },
        update: function (event, ui) {
            var item = ui.item;
            var moveRightBtn = item.find('.moveRightBtn');
            moveRightBtn.prop('hidden', false);
            var moveLeftBtn = item.find('.moveLeftBtn');
            moveLeftBtn.prop('hidden', true);
        },
        connectWith: '#currentFieldList'
    });//.disableSelection();

    $("#currentFieldList").sortable({
        handle: '.grip-handle',
        placeholder: 'list-item-placeholder',
        start: function (e, ui) {
            ui.placeholder.height(ui.item.height());
        },
        over: function (event, ui) {
            var cl = ui.item.attr('class');
            $('.list-item-placeholder').addClass(cl);
        },
        update: function (event, ui) {
            var item = ui.item;
            var moveRightBtn = item.find('.moveRightBtn');
            moveRightBtn.prop('hidden', true);
            var moveLeftBtn = item.find('.moveLeftBtn');
            moveLeftBtn.prop('hidden', false);
        },
        connectWith: '#availableFieldsList'
    }).disableSelection();

    $("input:text").focus(function () { $(this).select(); });

    $('[id]').each(function () {
        var idList = $('[id="' + this.id + '"]');
        if (idList.length > 1) {
            idList.not(':last').remove();
        }
    });

    //$('[id]').each(function (i) {
    //    $('[id="' + this.id + '"]').slice(1).remove();
    //});

    //$("#availableFieldsList .list-item").each(function () {
    //    if ($("#currentFieldList").find(".on").length > 0) {
    //        ///do something
    //    }
    //    $(this).toggleClass('required-input', required);
    //});


});

var idCounter = 0;

function buildHeader(id, headerText) {
    return `
<div class="list-item" id="${id}">
    <div class="col-sm-auto p-0 m-0"><span class="grip-handle draggable"><svg width="16" height="40"><use href="#custom-gripper"/></svg></span></div>
    <div class="col ml-0 mr-0"><h4 class="section-header"> ${headerText} </h4></div>
    <div class="col-sm-auto text-right m-0 p-0">
        <button type="button" class="btn btn-sm btn-link pl-0 pr-0 moveRightBtn" title="Move field to the right" onclick="moveRight('#${id}');"><i class="fas fa-arrow-right"></i></button>
        <button type="button" class="btn btn-sm btn-link pl-0 pr-0 moveLeftBtn" title="Move field to the left" onclick="moveLeft('#${id}');" hidden><i class="fas fa-arrow-left"></i></button>
        <button name="edit" class="btn btm-sm btn-link pl-0 pr-0" title="Edit" data-itemid="${id}" data-toggle="modal" data-target="#sectionHeaderModal"><i class="fa fa-edit" aria-hidden="true"></i></button>
        <button name="delete" class="btn btm-sm btn-link pl-0" title="Delete" data-itemid="${id}" data-toggle="modal" data-target="#deleteFieldModal"><i class="fa fa-trash" aria-hidden="true"></i></button>
    </div>
</div >`;
}

function addSectionHeader() {
    var inputField = $("#sectionHeaderTextInput");
    var headerText = inputField.val();
    if (headerText) {

        if (fieldId) {
            $(fieldId).find('.section-header').text(headerText);
        } else {
            var id = "sh_" + (++idCounter);
            $("#availableFieldsList").prepend(buildHeader(id, headerText));
        }


    }
    inputField.val("");
    $("#sectionHeaderModal").modal('hide');
}

function buildText(id, text) {
    return `
<div class="list-item ml-1 mr-1" id="${id}">
    <div class="col-sm-auto p-0 ml-0"><span class="grip-handle draggable"><svg width="16" height="40"><use href="#custom-gripper"/></svg></span></div>
    <div class="col"><p class='text-text'>${text}</p></div>
    <div class="col-sm-auto text-right m-0 p-0">
        <button type="button" class="btn btn-sm btn-link pl-0 pr-0 moveRightBtn" title="Move field to the right" onclick="moveRight('#${id}');"><i class="fas fa-arrow-right"></i></button>
        <button type="button" class="btn btn-sm btn-link pl-0 pr-0 moveLeftBtn" title="Move field to the left" onclick="moveLeft('#${id}');" hidden><i class="fas fa-arrow-left"></i></button>
        <button name="edit" class="btn btm-sm btn-link pl-0 pr-0" title="Edit" data-itemid="${id}" data-toggle="modal" data-target="#textModal"><i class="fa fa-edit" aria-hidden="true"></i></button>
        <button name="delete" class="btn btm-sm btn-link pl-0" title="Delete" data-itemid="${id}" data-toggle="modal" data-target="#deleteFieldModal"><i class="fa fa-trash" aria-hidden="true"></i></button>
    </div>
</div >`;
}

function buildCheckbox(id, checkboxText, required) {
    return `
<div class="list-item ml-1 mr-1" id="${id}">
    <div class="col-sm-auto p-0 ml-0"><span class="grip-handle draggable"><svg width="16" height="40"><use href="#custom-gripper"/></svg></span></div>
    <div class="col-sm-auto p-0 mr-0 ml-2" id="${id}Required" ${(required ? '' : 'style="display:none;"')}><span style="color:darkred;font-weight:bold;">*</span></div>
    <div class="col"><div class="form-check"><input class="form-check-input ${(required ? 'required-input' : '')}" type="checkbox" value="" id="consentCheck${id}"><label class="form-check-label" for="consentCheck${id}">${checkboxText}</label></div></div>
    <div class="col-sm-auto text-right m-0 p-0">
        <button type="button" class="btn btn-sm btn-link pl-0 pr-0 moveRightBtn" title="Move field to the right" onclick="moveRight('#${id}');"><i class="fas fa-arrow-right"></i></button>
        <button type="button" class="btn btn-sm btn-link pl-0 pr-0 moveLeftBtn" title="Move field to the left" onclick="moveLeft('#${id}');" hidden><i class="fas fa-arrow-left"></i></button>
        <button name="edit" class="btn btm-sm btn-link pl-0 pr-0" title="Edit" data-itemid="${id}" data-toggle="modal" data-target="#checkboxModal"><i class="fa fa-edit" aria-hidden="true"></i></button>
        <button name="delete" class="btn btm-sm btn-link pl-0" title="Delete" data-itemid="${id}" data-toggle="modal" data-target="#deleteFieldModal"><i class="fa fa-trash" aria-hidden="true"></i></button>
    </div>
</div >`;
}

var fieldId;

function addCheckbox() {

    var inputField = $("#checkboxText");
    var checkboxText = inputField.val();
    if (checkboxText) {
        var required = $("#checkboxRequiredCheck").is(':checked');
        if (fieldId) {
            $(fieldId).find('.form-check-label').text(checkboxText);
            $(fieldId).find('.form-check-input').toggleClass('required-input', required);
            $(fieldId + "Required").toggle(required);
        }
        else {
            var id = "cb_" + (++idCounter);
            $("#availableFieldsList").prepend(buildCheckbox(id, checkboxText, required));
        }

        
    }
    inputField.val("");
    $("#checkboxModal").modal('hide');
}


function addText() {

    var inputField = $("#textText");
    var actualText = inputField.val();
    if (actualText) {
        if (fieldId) {
            $(fieldId).find('.text-text').text(actualText);
            
        }
        else {
            var id = "tb_" + (++idCounter);
            $("#availableFieldsList").prepend(buildText(id, actualText));
        }


    }
    inputField.val("");
    $("#textModal").modal('hide');
}



function getFormPreview(clearSite = false) {

    var parent = $('<div></div>');
    $("#currentFieldList .list-item").each(function (i, obj) {

        var item = $(obj).clone();

        if (clearSite && item[0].id == "siteDetails") {
            item.find('.text-center').each(function () {
                $(this).text('');
            });
        }

        // change from list-item to row
        item.removeClass('list-item');
        item.addClass('row');
        item.addClass('mt-2');

        // remove controls
        $('div:first', item).remove();
        $('div:last', item).remove();

        parent.append(item);
    });
    var result = parent.html();
    return parent.html();


}

function moveRight(selector) {
    var item = $(selector);
    var clone = item.clone();
    item.remove();
    $("#currentFieldList").prepend(clone);
    var moveRightBtn = clone.find('.moveRightBtn');
    moveRightBtn.prop('hidden', true);
    var moveLeftBtn = clone.find('.moveLeftBtn');
    moveLeftBtn.prop('hidden', false);
}

function moveLeft(selector) {
    var item = $(selector);
    var clone = item.clone();
    item.remove();
    $("#availableFieldsList").prepend(clone);
    var moveRightBtn = clone.find('.moveRightBtn');
    moveRightBtn.prop('hidden', false);
    var moveLeftBtn = clone.find('.moveLeftBtn');
    moveLeftBtn.prop('hidden', true);
}

function moveAllLeft() {
    $("#currentFieldList .list-item").each(function (i, obj) {
        var moveRightBtn = $(obj).find('.moveRightBtn');
        if (moveRightBtn.length > 0) moveRightBtn.prop('hidden', false);
        var moveLeftBtn = $(obj).find('.moveLeftBtn');
        if (moveLeftBtn.length > 0) moveLeftBtn.prop('hidden', true);
        $("#availableFieldsList").append(obj);
    });
    $("#currentFieldList").empty();
    $("#moveLeftModal").modal('hide');
}

function moveAllRight() {
    $("#availableFieldsList .list-item").each(function (i, obj) {
        var moveRightBtn = $(obj).find('.moveRightBtn');
        if(moveRightBtn.length > 0) moveRightBtn.prop('hidden', true);
        var moveLeftBtn = $(obj).find('.moveLeftBtn');
        if (moveLeftBtn.length > 0) moveLeftBtn.prop('hidden', false);
        $("#currentFieldList").append(obj);
    });
    $("#availableFieldsList").empty();
    $("#moveRightModal").modal('hide');
}
function deleteField() {
    $(fieldId).remove();
    $("#deleteFieldModal").modal('hide');
}

$('#formList').change(function () {
    $("#selectFormButton").removeClass('disabled');
    //$("#selectFormButton").attr('disabled', false);
});

$('#sectionHeaderModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget); // Button that triggered the modal
    var itemId = button.data('itemid');
    if (itemId) {
        fieldId = "#" + itemId;
        var item = $(fieldId);
        var currentLabel = item.find('.section-header');
        var currentLabelText = currentLabel.text();
        $("#sectionHeaderModalLabel").text("Edit Section Header");
        $("#sectionHeaderTextInput").val(currentLabelText);
    } else {
        fieldId = null;
        $("#sectionHeaderModalLabel").text("Add Section Header");
        $("#sectionHeaderTextInput").val("");
    }

    
});

function saveSiteLogo() {
    var logo = $("#siteLogoPreview").clone();
    logo.prop('id', "siteLogoPlaceholder");
    $("#siteLogoContainer").empty();
    logo.appendTo("#siteLogoContainer");
    $("#editLogoContainer").empty();
    $("#editSiteLogoModal").modal('hide');
}


function saveSiteDetails() {
    var isChecked = $("#editSiteDetailsHidePracticeName").is(':checked');
    $("#siteName").prop('hidden', isChecked);
    $("#editSiteDetailsModal").modal('hide');
}

function moveSiteLogo(amount) {
    var logoPreview = $("#siteLogoPreview");
    var paddingLeft = parseInt(logoPreview.css("padding-left"));
    if (isNaN(paddingLeft))
        paddingLeft = 0;

    paddingLeft += amount;
    logoPreview.css("padding-left", paddingLeft);
}

$('#editSiteLogoModal').on('show.bs.modal', function (event) {
    $("#editLogoContainer").empty();
    var logo = $("#siteLogoPlaceholder").clone();
    logo.prop('id', "siteLogoPreview");
    logo.appendTo("#editLogoContainer");
    //$("#editLogoContainer").append(logo);
});


//var a = $('#selector').html();
//var b = $('#selector').html(a);

$('#previewFormModal').on('show.bs.modal', function (event) {
    $("#previewFormBody").empty();
    $("#previewFormBody").append(getFormPreview());
});


function decodeEntities(encodedString) {
    var textArea = document.createElement('textarea');
    textArea.innerHTML = encodedString;
    return textArea.value;
}


$('#editFieldModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget); // Button that triggered the modal
    fieldId = "#" + button.data('itemid'); // Extract info from data-* attributes 
    var hideRequired = button.data('hiderequired'); // Extract info from data-* attributes
    $("#editFieldRequiredFormGroup").prop("hidden", hideRequired != null);
    var modal = $(this);
    var item = $(fieldId);
    var currentLabel = item.find('.col-form-label');
    if (currentLabel.length == 0) {
        currentLabel = item.find('.form-check-label');
    }
    if (currentLabel.length > 0) {
        $("#editFieldDialogText").val(currentLabel.text());
        $("#editFieldRequiredCheck").prop("checked", currentLabel.hasClass('required-field'));
    }
    
});

$('#editAddressModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget); // Button that triggered the modal
    fieldId = "#" + button.data('itemid'); // Extract info from data-* attributes 
    var isPatientAddress = fieldId == "#patientAddress";
    var item = $(fieldId);
    var currentLabel = item.find('.col-form-label');
    $("#editAddressLabelText").val(currentLabel.text());
    $("#editAddressRequiredCheck").prop("checked", currentLabel.hasClass('required-field'));
    if (isPatientAddress) {
        $("#editAddressLine1Placeholder").val($("#patientAddress1").attr('placeholder'));
        $("#editAddressLine2Placeholder").val($("#patientAddress2").attr('placeholder'));
        $("#editAddressCityPlaceholder").val($("#patientAddressCity").attr('placeholder'));
        $("#editAddressStatePlaceholder").val($("#patientStateList").find("option:first-child").text());
        $("#editAddressZipCodePlaceholder").val($("#patientAddressZipCode").attr('placeholder'));
    } else {
        $("#editAddressLine1Placeholder").val($("#insuredAddress1").attr('placeholder'));
        $("#editAddressLine2Placeholder").val($("#insuredAddress2").attr('placeholder'));
        $("#editAddressCityPlaceholder").val($("#insuredAddressCity").attr('placeholder'));
        $("#editAddressStatePlaceholder").val($("#insuredStateList").find("option:first-child").text());
        $("#editAddressZipCodePlaceholder").val($("#insuredAddressZipCode").attr('placeholder'));
    }
    
});

$('#checkboxModal').on('show.bs.modal', function (event) {
    var defaultText = "";
    var required = false;
    var button = $(event.relatedTarget); // Button that triggered the modal
    var itemId = button.data('itemid');
    if (itemId) {
        fieldId = "#" + itemId;
        var item = $(fieldId);
        var currentLabel = item.find('.form-check-label');
        defaultText = currentLabel.text();
        var cb = item.find('.form-check-input');
        required = $(fieldId + "Required").is(':visible');
        $("#checkboxHeaderModal").text("Edit Checkbox");
    } else {
        fieldId = null;
        $("#checkboxHeaderModal").text("Add Checkbox");
    }
    
    $("#checkboxText").val(defaultText);
    $("#checkboxRequiredCheck").prop("checked", required);
});


$('#textModal').on('show.bs.modal', function (event) {
    var defaultText = "";
    var button = $(event.relatedTarget); // Button that triggered the modal
    var itemId = button.data('itemid');
    if (itemId) {
        fieldId = "#" + itemId;
        var item = $(fieldId);
        var currentLabel = item.find('.text-text');
        defaultText = currentLabel.text();
        
        $("#textHeaderModal").text("Edit Text");
    } else {
        fieldId = null;
        $("#textHeaderModal").text("Add Text");
    }

    $("#textText").val(defaultText);
});

$('#deleteFieldModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget); // Button that triggered the modal
    fieldId = "#" + button.data('itemid'); // Extract info from data-* attributes 
    
    var item = $(fieldId);
    var prompt = "Delete field ";
    var currentLabel = item.find('.col-form-label');

    if (currentLabel.length == 0) {
        prompt = "Delete checkbox ";
        currentLabel = item.find('.form-check-label');
    }

    if (currentLabel.length > 0) {
        var txt = currentLabel.text();
        $("#deleteFieldPrompt").text(prompt + txt + "?");
    }
});


function saveEditField() {
    var inputField = $("#editFieldDialogText");
    var labelText = inputField.val();
    var required = $("#editFieldRequiredCheck").is(':checked');
    if (labelText) {
        var label = $(fieldId).find('.col-form-label');
        if (label.length == 0) {
            label = $(fieldId).find('.form-check-label');
        }
        if (label.length > 0) {
            label.text(labelText);
            label.toggleClass('required-field', required);
        }
        
    }

    if (fieldId === "#patientName") {
        $("#patientFirstName").toggleClass('required-input', required);
        $("#patientLastName").toggleClass('required-input', required);
    } else {
        $(fieldId + ' input').each(function () {
            $(this).toggleClass('required-input', required);
        });
    }

    inputField.val("");
    $("#editFieldRequiredCheck").prop("checked", false);
    $("#editFieldModal").modal('hide');

}

function saveAddress() {
    var inputField = $("#editAddressLabelText");
    var labelText = inputField.val();
    var label = $(fieldId).find('.col-form-label');
    var required = $("#editAddressRequiredCheck").is(':checked');
    label.toggleClass('required-field', required);

    var isPatientAddress = fieldId == "#patientAddress";

    if (isPatientAddress) {
        $("#patientAddress1").toggleClass('required-input', required);
        $("#patientAddressCity").toggleClass('required-input', required);
        $("#patientStateList").toggleClass('required-input', required);
        $("#patientAddressZipCode").toggleClass('required-input', required);
        $("#patientAddress1").attr('placeholder', $("#editAddressLine1Placeholder").val());
        $("#patientAddress2").attr('placeholder', $("#editAddressLine2Placeholder").val());
        $("#patientAddressCity").attr('placeholder', $("#editAddressCityPlaceholder").val());
        $("#patientAddressZipCode").attr('placeholder', $("#editAddressZipCodePlaceholder").val());
        $("#patientStateList").children().first().remove();
        var statePlaceholderText = $("#editAddressStatePlaceholder").val();
        $("#patientStateList").prepend('<option value="" disabled hidden selected="selected">' + statePlaceholderText + "</option>");
        $("#patientAddress2").toggle(!$("#editAddressHideLine2Check").is(':checked'));
        $("#patientAddressCity").toggle(!$("#editAddressHideCityCheck").is(':checked'));
        $("#patientStateList").toggle(!$("#editAddressHideStateCheck").is(':checked'));
        $("#patientAddressZipCode").toggle(!$("#editAddressHideZipCheck").is(':checked'));
    } else {
        $("#insuredAddress1").toggleClass('required-input', required);
        $("#insuredAddressCity").toggleClass('required-input', required);
        $("#insuredStateList").toggleClass('required-input', required);
        $("#insuredAddressZipCode").toggleClass('required-input', required);

        $("#insuredAddress1").attr('placeholder', $("#editAddressLine1Placeholder").val());
        $("#insuredAddress2").attr('placeholder', $("#editAddressLine2Placeholder").val());
        $("#insuredAddressCity").attr('placeholder', $("#editAddressCityPlaceholder").val());
        $("#insuredAddressZipCode").attr('placeholder', $("#editAddressZipCodePlaceholder").val());
        $("#insuredStateList").children().first().remove();
        var statePlaceholderText = $("#editAddressStatePlaceholder").val();
        $("#insuredStateList").prepend('<option value="" disabled hidden selected="selected">' + statePlaceholderText + "</option>");
        $("#insuredAddress2").toggle(!$("#editAddressHideLine2Check").is(':checked'));
        $("#insuredAddressCity").toggle(!$("#editAddressHideCityCheck").is(':checked'));
        $("#insuredStateList").toggle(!$("#editAddressHideStateCheck").is(':checked'));
        $("#insuredAddressZipCode").toggle(!$("#editAddressHideZipCheck").is(':checked'));
    }

    label.text(labelText);
    inputField.val("");
    $("#editAddressRequiredCheck").prop("checked", false);


    $("#editAddressModal").modal('hide');
}