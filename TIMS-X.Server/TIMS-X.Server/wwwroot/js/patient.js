/* Handle image resized events */

var PatientPhotoDataUrl;
var PatientPhoto;
var InsuranceCardFrontDataUrl;
var InsuranceCardBackDataUrl;
var InsuranceCardFront;
var InsuranceCardBack;
var imageDestination;
var requestedWidth;
var requestedHeight;
var rotateDestination;
var rotateLeft;

var imageLoaded = function (loadEvent) {
	console.log('imageLoaded()');
	var img = loadEvent.target;
	//var img = imageSource;
	// Resize the image
	var canvas = document.createElement('canvas');
    
    if (img.width < requestedWidth) {
	    requestedWidth = img.width;
	}
    var newHeight = img.height * requestedWidth / img.width;
    if (newHeight > requestedHeight) {
        requestedWidth = img.width * requestedHeight / img.height;
        newHeight = requestedHeight;
	}
    canvas.width = requestedWidth;
	canvas.height = newHeight;
    canvas.getContext('2d').drawImage(img, 0, 0, requestedWidth, newHeight);
	var dataUrl = canvas.toDataURL('image/jpeg');
	var resizedImage = dataURLToBlob(dataUrl);
    setPhoto(imageDestination, resizedImage, dataUrl);
}

function rotateImageLoaded(loadEvent) {
    var img = loadEvent.target;
    var canvas = document.createElement('canvas');
    canvas.width = img.height;
    canvas.height = img.width;
    var context = canvas.getContext('2d');
    context.clearRect(0, 0, canvas.width, canvas.height);

    // save the unrotated context of the canvas so we can restore it later
    // the alternative is to untranslate & unrotate after drawing
    context.save();

    // move to the center of the canvas
    context.translate(canvas.width / 2, canvas.height / 2);

    // rotate the canvas to the specified degrees
    context.rotate((rotateLeft ? -90.0 : 90.0) * Math.PI / 180);

    // draw the image
    // since the context is rotated, the image will be rotated also
    context.drawImage(img, -img.width / 2, -img.height / 2);

    // we’re done with the rotating so restore the unrotated context
    context.restore();


    var dataUrl = canvas.toDataURL('image/jpeg');
    var rotatedImage = dataURLToBlob(dataUrl);
    setPhoto(rotateDestination, rotatedImage, dataUrl);
}

function rotateImage(dataUrl, destImage, rotation) {
    rotateDestination = destImage;
    rotateLeft = rotation;
    var img = new Image();
    img.onload = rotateImageLoaded;
    img.src = dataUrl;
}


//$(document).ready(function () {
//	var img = new Image();
//	img.onload = function () {
//		var urlCreator = window.URL || window.webkitURL;
//		var imageUrl = urlCreator.createObjectURL(img);
//		$("#photoupload").attr("src", imageUrl);
//		alert("image is loaded");
//	}
//	img.src = rawImage;
	
//	//$(img).appendTo('body');
//});

// Load the image
var fileReader = new FileReader();
fileReader.onload = function (readerEvent) {
    var img = new Image();
    img.onload = imageLoaded;
    img.onerror = function(errorImageEvent) { console.log('onerror called...'); };
    
    console.log('[Set Image Source]');
    img.src = readerEvent.target.result;

}

function resizeImage(file, destImage, width, height) {
    imageDestination = destImage;
    requestedWidth = width;
    requestedHeight = height;
	// Ensure it's an image
	if (file.type.match(/image.*/)) {

		fileReader.readAsDataURL(file);
	}
}


function rotatePatientPhotoLeft() {
    rotateImage(PatientPhotoDataUrl, DocType.PatientPhoto, true);
}
function rotatePatientPhotoRight() {
	rotateImage(PatientPhotoDataUrl, DocType.PatientPhoto, false);
}


function rotateInsFrontLeft() {
	rotateImage(InsuranceCardFrontDataUrl, DocType.InsuranceCardFront, true);
}
function rotateInsFrontRight() {
    rotateImage(InsuranceCardFrontDataUrl, DocType.InsuranceCardFront, false);
}


function rotateInsBackLeft() {
    rotateImage(InsuranceCardBackDataUrl, DocType.InsuranceCardBack, true);
}
function rotateInsBackRight() {
    rotateImage(InsuranceCardBackDataUrl, DocType.InsuranceCardBack, false);
}

var Documents = [];

const DocType = {
    PatientPhoto: 'patphoto',
    InsuranceCardFront: 'insfront',
    InsuranceCardFront: 'insback'
};

//function submitDocuments(patientGuid) {
//    var formData = new FormData();
//    if (PatientPhoto) {
//        formData.append('patientPhoto', PatientPhoto);
//    }
//    if (InsuranceCardFront) {
//        formData.append('insuranceCardFront', InsuranceCardFront);
//    }
//    if (InsuranceCardBack) {
//        formData.append('insuranceCardBack', InsuranceCardBack);
//    }

//    $.ajax({
//        type: "post",
//        url: "/web/Imaging/UploadDocuments",
//        contentType: false,
//        headers: { 'patientguid': patientGuid },
//        processData: false,
//        data: formData,
//        success: function (message) {
//            location.reload();
//            $("#frontinsupload").empty();
//            $("#backinsupload").empty();
//            $("#photoupload").empty();
//            $("#patientPhotoInput").val('');
//            $("#insCardBackInput").val('');
//            $("#insCardFrontInput").val('');
//        },
//        error: function () {
//            alert("An error occurred uploading files.");
//            location.reload(); 
//        }
//    });

//}


function goToPatientIntake(patientId) {
    $.ajax({
        type: "get",
        url: "/web/Patient/IntakeLink",
        contentType: false,
        headers: { 'patientId': patientId },
        processData: false,
        success: function (message) {
            window.location.href = message;
        },
        error: function (response) {
            alert("Error: " + response.responseJSON);
            //location.reload();
        }
    });
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

function submitDocumentsV2(patientGuid, patientId) {
    var formData = new FormData();
    if (PatientPhoto) {
        formData.append('patientPhoto', PatientPhoto);
    }
    if (InsuranceCardFront) {
        formData.append('insuranceCardFront', InsuranceCardFront);
    }
    if (InsuranceCardBack) {
        formData.append('insuranceCardBack', InsuranceCardBack);
    }
    var frontSlot = $("#InsuranceSlotFront").val();
    var backSlot = $("#InsuranceSlotFront").val();
    $.ajax({
        type: "post",
        url: "/web/Imaging/UploadDocumentsV2?frontSlot=" + frontSlot + "&backSlot=" + backSlot,
        contentType: false,
        headers: { 'patientguid': patientGuid },
        processData: false,
        data: formData,
        success: function (message) {
            window.location.href = '/SubmissionSuccess/' + patientId;

            //location.reload();
            //$("#frontinsupload").empty();
            //$("#backinsupload").empty();
            //$("#photoupload").empty();
            //$("#patientPhotoInput").val('');
            //$("#insCardBackInput").val('');
            //$("#insCardFrontInput").val('');
        },
        error: function () {
            alert("An error occurred uploading files.");
            location.reload();
        }
    });

}

function getDataUrl(img) {
    // Create canvas
    const canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');
    // Set width and height
    canvas.width = img.width;
    canvas.height = img.height;
    // Draw the image
    ctx.drawImage(img, 0, 0);
    return canvas.toDataURL('image/jpeg');
}




window.uploadInsFront = function() {
    resizeImage(event.target.files[0], DocType.InsuranceCardFront, 800, 457);
};
window.uploadInsBack = function () {
    resizeImage(event.target.files[0], DocType.InsuranceCardBack, 800, 457);
};

window.uploadPhoto = function() {
    resizeImage(event.target.files[0], DocType.PatientPhoto, 620, 465);
};

window.uploadDocuments = function () {
    event.target.files.forEach(function (file) {
        
    });
};


var patientPhotoControls = $('<span><a title="Rotate Left" class="btn btn-sm btn-light mr-2" onclick="rotatePatientPhotoLeft()"><i class="fas fa-chevron-left"/></a><a title="Rotate Right" class="btn btn-sm btn-light" onclick="rotatePatientPhotoRight()"><i class="fas fa-chevron-right"/></a></span >');
var insFrontControls = $('<span><a title="Rotate Left" class="btn btn-sm btn-light mr-2" onclick="rotateInsFrontLeft()"><i class="fas fa-chevron-left"/></a><a title="Rotate Right" class="btn btn-sm btn-light" onclick="rotateInsFrontRight()"><i class="fas fa-chevron-right"/></a></span >');
var insBackControls = $('<span><a title="Rotate Left" class="btn btn-sm btn-light mr-2" onclick="rotateInsBackLeft()"><i class="fas fa-chevron-left"/></a><a title="Rotate Right" class="btn btn-sm btn-light" onclick="rotateInsBackRight()"><i class="fas fa-chevron-right"/></a></span >');


function setPhoto(destImage, image, dataUrl) {
	var urlCreator = window.URL || window.webkitURL;
    var imageUrl = urlCreator.createObjectURL(image);
    var img = $('<img/>');
    img.attr('src', imageUrl);
    
    switch (destImage)
    {
		case DocType.PatientPhoto:
            PatientPhoto = image;
            PatientPhotoDataUrl = dataUrl;
			img.attr('class', "patient-photo");
			$("#photoupload").empty();
            $("#photoupload").append(img);
            $("#patientPhotoControls").empty();
            $("#patientPhotoControls").append(patientPhotoControls);
			break;
		case DocType.InsuranceCardFront:
            InsuranceCardFront = image;
            InsuranceCardFrontDataUrl = dataUrl;
            img.attr('class', "insurance-photo");
			$("#frontinsupload").empty();
            $("#frontinsupload").append(img);
            $("#insFrontControls").empty();
            $("#insFrontControls").append(insFrontControls);
			break;
		case DocType.InsuranceCardBack:
            InsuranceCardBack = image;
            InsuranceCardBackDataUrl = dataUrl;
			img.attr('class', "insurance-photo");
			$("#backinsupload").empty();
            $("#backinsupload").append(img);
            $("#insBackControls").empty();
            $("#insBackControls").append(insBackControls);
			break;
	}
}