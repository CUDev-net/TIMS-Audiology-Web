
var sigPadCanvas;
var signaturePad;
$(document).ready(function () {

    sigPadCanvas = document.getElementById('signature-pad');
    if (sigPadCanvas) {
        window.onresize = resizeSigPad;
        resizeSigPad();


        signaturePad = new SignaturePad(sigPadCanvas, {
            backgroundColor: 'rgb(255, 255, 255)' // necessary for saving image as JPEG; can be removed is only saving as PNG or SVG
        });

        signaturePad.addEventListener("beginStroke", () => {
            $("#signature-pad").removeClass("missing-input");
        }, { once: false });

    }
    

    $("input").change(function(){
        if ($(this).is(':checkbox')) {
            var checked = $(this).is(':checked');
            if(checked) {
                $(this).parent().parent().parent().removeClass('missing-input');
            }
        }
        else if($(this).val().length > 0)
        {
            $(this).removeClass("missing-input");
        }

    });

    
    const postDataForm = document.getElementById('PatientIntakeForm');
    postDataForm.addEventListener('submit', function (e) {
        e.preventDefault();

        if (!validatePatientForm()) {
            $("#infoModal").modal('show');
            return;
        }

        // disable the submit button
        $('#PatientIntakeForm input[type="submit"]').attr('disabled', true);

        copyFormFields();
        takeScreenshot();
        
    });
});

function clearSignature() {
    signaturePad.clear();
}

function undoSignatureStroke() {
    var data = signaturePad.toData();
    if (data) {
        data.pop(); // remove the last dot or line
        signaturePad.fromData(data);
    }
}

// Adjust canvas coordinate space taking into account pixel ratio,
// to make it look crisp on mobile devices.
// This also causes canvas to be cleared.
function resizeSigPad() {
    // When zoomed out to less than 100%, for some very strange reason,
    // some browsers report devicePixelRatio as less than 1
    // and only part of the canvas is cleared then.
    var ratio = Math.max(window.devicePixelRatio || 1, 1);
    sigPadCanvas.width = sigPadCanvas.offsetWidth * ratio;
    sigPadCanvas.height = sigPadCanvas.offsetHeight * ratio;
    sigPadCanvas.getContext("2d").scale(ratio, ratio);
    if (signaturePad) {
        signaturePad.clear();
    }
    //signaturePad.clear();
}

function validatePatientForm() {
    var valid = true;
    var errorMessage = '';

    $('.required-input').each(function () {

        if ($(this).is(':checkbox')) {
            var checked = $(this).is(':checked');
            if (!checked) {



                valid = false;
                if (!errorMessage)
                    errorMessage += "<span>* Missing Required Fields</span><br />";
            }
            $(this).parent().parent().parent().toggleClass('missing-input', !checked);
        } else {
            var value = $(this).val();
            if (!value) {
                valid = false;
                if (!errorMessage)
                    errorMessage += "<span>* Missing Required Fields</span><br />";
            }

            $(this).toggleClass('missing-input', !value);
        }
    });

    var patSig = $("#patSignature");
    if (patSig.length > 0 && patSig.find('.required-field').length > 0) {
        if (signaturePad.isEmpty()) {
            valid = false;
            errorMessage += `<span>* Signature is required.</span><br />`;
            $("#signature-pad").toggleClass('missing-input', true);
        }
    }
    

    var emailAddr = $("#PatientEmailAddressInput");
    if (emailAddr.length > 0) {
        var value = emailAddr.val();
        if (value) {
            var isValidEmail = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(value);

            if (!isValidEmail) {
                emailAddr.toggleClass('missing-input', true);
                valid = false;
                errorMessage += `<span>* Invalid Email '${value}'</span><br />`;
            }

        }
    }

    var homePhone = $("#patientHomePhoneInput");
    if (homePhone.length > 0) {
        var value = homePhone.val();
        if (value) {
            var isValidPhone = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im.test(value);

            if (!isValidPhone) {
                homePhone.toggleClass('missing-input', true);
                valid = false;
                errorMessage += `<span>* Invalid Home Phone '${value}'</span><br />`;
            }
        }
    }

    var workPhone = $("#patientWorkPhoneInput");
    if (workPhone.length > 0) {
        var value = workPhone.val();
        if (value) {
            var isValidPhone = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im.test(value);

            if (!isValidPhone) {
                workPhone.toggleClass('missing-input', true);
                valid = false;
                errorMessage += `<span>* Invalid Work Phone '${value}'</span><br />`;
            }
        }
    }

    var mobilePhone = $("#patientMobilePhoneInput");
    if (mobilePhone.length > 0) {
        var value = mobilePhone.val();
        if (value) {
            var isValidPhone = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im.test(value);

            if (!isValidPhone) {
                mobilePhone.toggleClass('missing-input', true);
                valid = false;
                errorMessage += `<span>* Invalid Mobile Phone '${value}'</span><br />`;
            }
        }
    }

    var otherPhone = $("#patientOtherPhoneInput");
    if (otherPhone.length > 0) {
        var value = otherPhone.val();
        if (value) {
            var isValidPhone = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im.test(value);

            if (!isValidPhone) {
                otherPhone.toggleClass('missing-input', true);
                valid = false;
                errorMessage += `<span>* Invalid Other Phone '${value}'</span><br />`;
            }
        }
    }

    var contactPhoneInput = $("#contactPhoneInput");
    if (contactPhoneInput.length > 0) {
        var value = contactPhoneInput.val();
        if (value) {
            var isValidPhone = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im.test(value);

            if (!isValidPhone) {
                contactPhoneInput.toggleClass('missing-input', true);
                valid = false;
                errorMessage += `<span>* Invalid Emergency Contact Phone '${value}'</span><br />`;
            }
        }
    }

    if (!valid) {
        $("#infoModalDescription").html(errorMessage);
        $('.missing-input:first').get(0).scrollIntoView();
        window.scrollBy(0, -80);
    }

    return valid;
}

var doSubmitForm = false;

function yesNoModalConfirmed() {
    doSubmitForm = true;
    $("#yesNoModal").modal('hide');
}


/* Utility function to convert a canvas to a BLOB */
var dataURLToBlob = function (dataURL) {
    var BASE64_MARKER = ';base64,';
    if (dataURL.indexOf(BASE64_MARKER) == -1) {
        var parts = dataURL.split(',');
        var contentType = parts[0].split(':')[1];
        var raw = parts[1];

        return new Blob([raw], { type: contentType });
    }

    var parts = dataURL.split(BASE64_MARKER);
    var contentType = parts[0].split(':')[1];
    var raw = window.atob(parts[1]);
    var rawLength = raw.length;

    var uInt8Array = new Uint8Array(rawLength);

    for (var i = 0; i < rawLength; ++i) {
        uInt8Array[i] = raw.charCodeAt(i);
    }

    return new Blob([uInt8Array], { type: contentType });
}


var formScreenshot = { done: false, imageBlob: new Blob() };

function takeScreenshot() {
    formScreenshot.done = false;
    let patientForm = document.getElementById('patientForm');
    html2canvas(patientForm).then(
        function (canvas) {
            var dataUrl = canvas.toDataURL('image/jpeg');
            formScreenshot.imageBlob = dataURLToBlob(dataUrl);
            //let container = new DataTransfer();
            //let file = new File([imageBlob], "intake_form.jpg", { type: "image/jpeg", lastModified: new Date().getTime() });
            //container.items.add(file);
            //document.getElementById('FormPhoto_X').files = container.files;
            //$("#FormPhoto_X").val(patientFormImage);
            formScreenshot.done = true;
            var result = confirm("Submit Form?");
            if (result) {
                var submitForm = document.getElementById("PatientIntakeForm");
                const postUrl = submitForm.action;
                const formData = new FormData(submitForm);
                formData.append('FormScreenshot', formScreenshot.imageBlob);
                fetch(postUrl, {
                    method: 'post',
                    body: formData
                }).then(function (response) {
                    window.location.href = '/FormSubmitted';
                });
            }
            //$("#yesNoModal").modal('show');
            //return doSubmitForm;
        }
    );
}


function submitPatientForm() {

    if (formScreenshot.done) {
        var result = confirm("Submit Form?");
        formScreenshot.done = false;
        return result;
        //$("#yesNoModal").modal('show');
        //return doSubmitForm;
    }

    if (!validatePatientForm()) {
        $("#infoModal").modal('show');
        return false;
    }

    // disable the submit button
    $('#PatientIntakeForm input[type="submit"]').attr('disabled', true);

    copyFormFields();
    takeScreenshot();

    return false;
}

function copyAddressFromPatient() {
    $("#insuredAddress1").val($("#patientAddress1").val());
    if (!$("#insuredAddress2").is(':hidden')) {
        $("#insuredAddress2").val($("#patientAddress2").val());
    }
    if (!$("#insuredAddressCity").is(':hidden')) {
        $("#insuredAddressCity").val($("#patientAddressCity").val());
    }
    if (!$("#insuredAddressZipCode").is(':hidden')) {
        $("#insuredAddressZipCode").val($("#patientAddressZipCode").val());
    }
    if (!$("#insuredStateList").is(':hidden')) {
        var selectedStateText = $("#patientStateList").find(':selected').text();
        if (selectedStateText == "State")
            selectedStateText = "";

        $('#insuredStateList option').filter(function () { return $(this).html() == selectedStateText; }).prop('selected', true);
    }
}

function copySecAddressFromPatient() {
    $("#insuredAddress3").val($("#patientAddress1").val());
    if (!$("#insuredAddress4").is(':hidden')) {
        $("#insuredAddress4").val($("#patientAddress2").val());
    }
    if (!$("#insuredAddressCity2").is(':hidden')) {
        $("#insuredAddressCity2").val($("#patientAddressCity").val());
    }
    if (!$("#insuredAddressZipCode2").is(':hidden')) {
        $("#insuredAddressZipCode2").val($("#patientAddressZipCode").val());
    }
    if (!$("#insuredStateList2").is(':hidden')) {
        var selectedStateText = $("#patientStateList").find(':selected').text();
        if (selectedStateText == "State")
            selectedStateText = "";
        $('#insuredStateList2 option').filter(function () { return $(this).html() == selectedStateText; }).prop('selected', true);
    }
}


function copyFormFields() {
    var patientName = $("#patientName");
    if (patientName.length > 0) {
        $("#PatientFirstName_X").val($("#patientFirstName").val());
        $("#PatientInitial_X").val($("#patientMI").val());
        $("#PatientLastName_X").val($("#patientLastName").val());
    }

    var patientEmail = $("#patientEmail");
    if (patientEmail.length > 0) {
        $("#PatientEmail_X").val($("#PatientEmailAddressInput").val());
    }

    var patientDtOfBirth = $("#patientDOB");
    if (patientDtOfBirth.length > 0) {
        $("#PatientDtOfBirth_X").val($("#patientDtOfBirthInput").val());
    }

    var patientAddress = $("#patientAddress");
    if (patientAddress.length > 0) {
        $("#PatientAddr1_X").val($("#patientAddress1").val());
        $("#PatientAddr2_X").val($("#patientAddress2").val());
        $("#PatientCity_X").val($("#patientAddressCity").val());
        var selectedStateText = $("#patientStateList").find(':selected').text();
        if (selectedStateText == "State")
            selectedStateText = "";
        $("#PatientState_X").val(selectedStateText);
        $("#PatientZip_X").val($("#patientAddressZipCode").val());
    }

    var respParty = $("#responsibleParty");
    if (respParty.length > 0) {
        $("#PatientRespParty_X").val($("#responsiblePartyInput").val());
    }

    var checkedPrimaryPhone = $('input[name="primaryPhoneGroup"]:checked');
    if (checkedPrimaryPhone.length > 0) {
        if (checkedPrimaryPhone[0].id == "PrimaryPhoneHome")
            $("#PatientPrimaryPhone_X").val(1);
        else if (checkedPrimaryPhone[0].id == "PrimaryPhoneWork")
            $("#PatientPrimaryPhone_X").val(2);
        else if (checkedPrimaryPhone[0].id == "PrimaryPhoneMobile")
            $("#PatientPrimaryPhone_X").val(3);
        else if (checkedPrimaryPhone[0].id == "PrimaryPhoneOther")
            $("#PatientPrimaryPhone_X").val(4);
    }

    var patientHomePhone = $("#patientHomePhoneInput");
    if (patientHomePhone.length > 0) {
        $("#PatientHomePhone_X").val(patientHomePhone.val());
    }

    var patientMobilePhone = $("#patientMobilePhoneInput");
    if (patientMobilePhone.length > 0) {
        $("#PatientMobilePhone_X").val(patientMobilePhone.val());
    }

    var patientWorkPhone = $("#patientWorkPhoneInput");
    if (patientWorkPhone.length > 0) {
        $("#PatientWorkPhone_X").val(patientWorkPhone.val());
    }

    var patientOtherPhone = $("#patientOtherPhoneInput");
    if (patientOtherPhone.length > 0) {
        $("#PatientOtherPhone_X").val(patientOtherPhone.val());
    }

    var selectedMaritalStatus = $("#maritalStatusInput").find(':selected');
    if (selectedMaritalStatus.length > 0) {
        var value = selectedMaritalStatus.val();
        $("#MaritalStatusId_X").val(value);
    }

    var selectedEmplStatus = $("#emplStatusInput").find(':selected');
    if (selectedEmplStatus.length > 0) {
        var value = selectedEmplStatus.val();
        $("#EmplStatusId_X").val(value);
    }

    var selectedLang = $("#preferredLangInput").find(':selected');
    if (selectedLang.length > 0) {
        var value = selectedLang.val();
        $("#Language_X").val(value);
    }

    var selectedRace = $("#raceInput").find(':selected');
    if (selectedRace.length > 0) {
        var value = selectedRace.val();
        $("#Race_X").val(value);
    }

    var selectedEthnicity = $("#ethnicityInput").find(':selected');
    if (selectedEthnicity.length > 0) {
        var value = selectedEthnicity.val();
        $("#Ethnicity_X").val(value);
    }

    var selectedSex = $("#sexInput").find(':selected');
    if (selectedSex.length > 0) {
        var value = selectedSex.val();
        $("#Sex_X").val(value);
    }

    var contact = $("#contactInput");
    if (contact.length > 0) {
        $("#Contact_X").val(contact.val());
    }

    var contactPhone = $("#contactPhoneInput");
    if (contactPhone.length > 0) {
        $("#ContactPhone_X").val(contactPhone.val());
    }

    var refPhys = $("#refPhysInput");
    if (refPhys.length > 0) {
        $("#RefPhysician_X").val(refPhys.val());
    }

    var releaseSig = $("#releaseSignature");
    if (releaseSig.length > 0) {
        var isChecked = $("#releaseSignatureCheck").is(':checked');
        $("#ReleaseSignature_X").val(isChecked);
        var releaseDate = $("#releaseSignatureDate").val();
        $("#ReleaseSignatureDate_X").val(releaseDate);
    }

    var assignBenefits = $("#assignBenefits");
    if (assignBenefits.length > 0) {
        var isChecked = $("#assignBenefitsCheck").is(':checked');
        $("#AssignBenefits_X").val(isChecked);
        var dt = $("#assignBenefitsDate").val();
        $("#AssignBenefitsDate_X").val(dt);
    }

    var voiceNotif = $("#apptNotifPhone");
    if (voiceNotif.length > 0) {
        $("#VoiceNotifications_X").val(voiceNotif.is(':checked'));
    }
    var textNotif = $("#apptNotifText");
    if (textNotif.length > 0) {
        $("#TextNotifications_X").val(textNotif.is(':checked'));
    }
    var emailNotif = $("#apptNotifEmail");
    if (emailNotif.length > 0) {
        $("#EmailNotifications_X").val(emailNotif.is(':checked'));
    }

    var insCoName = $("#insCoNameInput");
    if (insCoName.length > 0) {
        $("#InsCoName_X").val(insCoName.val());
    }
    var insIdNum = $("#insIdNumInput");
    if (insIdNum.length > 0) {
        $("#InsIdNum_X").val(insIdNum.val());
    }
    var insGroupNum = $("#insGroupNumInput");
    if (insGroupNum.length > 0) {
        $("#InsGroupNum_X").val(insGroupNum.val());
    }
    var insuredRelation = $("#insuredRelationInput").find(':selected');
    if (insuredRelation.length > 0) {
        var value = insuredRelation.val();
        $("#RelationToInsured_X").val(value);
    }
    var insuredName = $("#insuredName");
    if (insuredName.length > 0) {
        $("#InsFirstName_X").val($("#insuredFirstName").val());
        $("#InsInitial_X").val($("#insuredMI").val());
        $("#InsLastName_X").val($("#insuredLastName").val());
    }

    var insuredAddress1 = $("#insuredAddress1");
    if (insuredAddress1.length > 0) {
        $("#InsAddress1_X").val(insuredAddress1.val());
    }

    var insuredAddress2 = $("#insuredAddress2");
    if (insuredAddress2.length > 0) {
        $("#InsAddress2_X").val(insuredAddress2.val());
    }

    var insuredCity = $("#insuredAddressCity");
    if (insuredCity.length > 0) {
        $("#InsCity_X").val(insuredCity.val());
    }

    var insuredStateList = $("#insuredStateList");
    if (insuredStateList.length > 0) {
        var selectedStateText1 = $("#insuredStateList").find(':selected').text();
        if (selectedStateText1 == "State")
            selectedStateText1 = "";
        $("#InsState_X").val(selectedStateText1);
    }

    var insuredZip = $("#insuredAddressZipCode");
    if (insuredZip.length > 0) {
        $("#InsZip_X").val(insuredZip.val());
    }

    var insuredPhone = $("#insuredPhoneInput");
    if (insuredPhone.length > 0) {
        $("#InsPhone_X").val(insuredPhone.val());
    }

    var selectedSex2 = $("#insuredSexInput").find(':selected');
    if (selectedSex2.length > 0) {
        var value = selectedSex2.val();
        $("#InsSex_X").val(value);
    }

    var insuredDOB = $("#insuredDOBInput");
    if (insuredDOB.length > 0) {
        $("#InsDtOfBirth_X").val(insuredDOB.val());
    }


    var insCoName2 = $("#insCoNameInput2");
    if (insCoName2.length > 0) {
        $("#SecInsCoName_X").val(insCoName2.val());
    }
    var insIdNum2 = $("#insIdNumInput2");
    if (insIdNum2.length > 0) {
        $("#SecInsIdNum_X").val(insIdNum2.val());
    }
    var insGroupNum2 = $("#insGroupNumInput2");
    if (insGroupNum2.length > 0) {
        $("#SecInsGroupNum_X").val(insGroupNum2.val());
    }
    var insuredRelation2 = $("#insuredRelationInput2").find(':selected');
    if (insuredRelation2.length > 0) {
        var value = insuredRelation2.val();
        $("#SecRelationToInsured_X").val(value);
    }
    var insuredName2 = $("#insuredName2");
    if (insuredName2.length > 0) {
        $("#SecInsFirstName_X").val($("#insuredFirstName2").val());
        $("#SecInsInitial_X").val($("#insuredMI2").val());
        $("#SecInsLastName_X").val($("#insuredLastName2").val());
    }

    var insuredAddress3 = $("#insuredAddress3");
    if (insuredAddress3.length > 0) {
        $("#SecInsAddress1_X").val(insuredAddress3.val());
    }

    var insuredAddress4 = $("#insuredAddress4");
    if (insuredAddress4.length > 0) {
        $("#SecInsAddress2_X").val(insuredAddress4.val());
    }

    var insuredCity2 = $("#insuredAddressCity2");
    if (insuredCity2.length > 0) {
        $("#SecInsCity_X").val(insuredCity2.val());
    }

    var insuredStateList2 = $("#insuredStateList2");
    if (insuredStateList2.length > 0) {
        var selectedStateText1 = $("#insuredStateList2").find(':selected').text();
        if (selectedStateText1 == "State")
            selectedStateText1 = "";
        $("#SecInsState_X").val(selectedStateText1);
    }

    var insuredZip2 = $("#insuredAddressZipCode2");
    if (insuredZip2.length > 0) {
        $("#SecInsZip_X").val(insuredZip2.val());
    }

    var insuredPhone2 = $("#insuredPhoneInput2");
    if (insuredPhone2.length > 0) {
        $("#SecInsPhone_X").val(insuredPhone2.val());
    }

    var selectedSex3 = $("#insuredSexInput2").find(':selected');
    if (selectedSex3.length > 0) {
        var value = selectedSex3.val();
        $("#SecInsSex_X").val(value);
    }

    var insuredDOB2 = $("#insuredDOBInput2");
    if (insuredDOB2.length > 0) {
        $("#SecInsDtOfBirth_X").val(insuredDOB2.val());
    }
}
