'use strict';

/* eslint-disable require-jsdoc, no-unused-vars */

var AppointmentTypes = [];

function AppointmentType() {
    this.id = null;
    this.name = null;
    this.checked = true;
    this.color = null;
    this.bgColor = null;
    this.borderColor = null;
    this.dragBgColor = null;
}

function findAppointmentType(id) {
    var found;

    AppointmentTypes.forEach(function (appointmentType) {
        if (appointmentType.id === id) {
            found = appointmentType;
        }
    });

    return found || AppointmentTypes[0];
}

function hexToRGBA(hex) {
    var radix = 16;
    var r = parseInt(hex.slice(1, 3), radix),
        g = parseInt(hex.slice(3, 5), radix),
        b = parseInt(hex.slice(5, 7), radix),
        a = parseInt(hex.slice(7, 9), radix) / 255 || 1;
    var rgba = 'rgba(' + r + ', ' + g + ', ' + b + ', ' + a + ')';

    return rgba;
}

function fetchAppointmentTypes(callback) {
    $.ajax({
        type: "get",
        url: "/web/Scheduler/AppointmentTypes",
        contentType: false,
        processData: false,
        success: function (timsApptTypes) {
            timsApptTypes.forEach(function (timsApptType) {
                var type = new AppointmentType();
                type.id = String(timsApptType.id);
                type.name = timsApptType.name;
                type.color = timsApptType.foregroundColor;
                type.bgColor = timsApptType.backgroundColor;
                type.dragBgColor = timsApptType.backgroundColor;
                type.borderColor = timsApptType.backgroundColor;
                AppointmentTypes.push(type);
            });
            callback();
        },
        error: function (message) {
            callback();
        }
    });
}


(function () {

    

    //var apptType;
    //var id = 0;

    //apptType = new AppointmentType();
    //id += 1;
    //apptType.id = String(id);
    //apptType.name = 'Fitting';
    //apptType.color = '#ffffff';
    //apptType.bgColor = '#9e5fff';
    //apptType.dragBgColor = '#9e5fff';
    //apptType.borderColor = '#9e5fff';
    //AppointmentTypes.push(apptType);

    //apptType = new AppointmentType();
    //id += 1;
    //apptType.id = String(id);
    //apptType.name = 'Company';
    //apptType.color = '#ffffff';
    //apptType.bgColor = '#00a9ff';
    //apptType.dragBgColor = '#00a9ff';
    //apptType.borderColor = '#00a9ff';
    //AppointmentTypes.push(apptType);

    //apptType = new AppointmentType();
    //id += 1;
    //apptType.id = String(id);
    //apptType.name = 'Family';
    //apptType.color = '#ffffff';
    //apptType.bgColor = '#ff5583';
    //apptType.dragBgColor = '#ff5583';
    //apptType.borderColor = '#ff5583';
    //AppointmentTypes.push(apptType);

    //apptType = new AppointmentType();
    //id += 1;
    //apptType.id = String(id);
    //apptType.name = 'Friend';
    //apptType.color = '#ffffff';
    //apptType.bgColor = '#03bd9e';
    //apptType.dragBgColor = '#03bd9e';
    //apptType.borderColor = '#03bd9e';
    //AppointmentTypes.push(apptType);
    
})();
