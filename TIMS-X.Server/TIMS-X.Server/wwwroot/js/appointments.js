'use strict';

/*eslint-disable*/




var AppointmentList = [];

function AppointmentInfo() {
    this.id = null;
    this.calendarId = null;

    this.providerId = 0;
    this.statusId = 0;
    this.siteId = 0;
    this.patientId = 0;
    this.marketingId = 0;
    this.resourceId = 0;
    this.authorizationId = 0;
    this.title = null;
    this.body = null;
    this.isAllday = false;
    this.start = null;
    this.end = null;
    this.category = '';
    this.dueDateClass = '';

    this.color = null;
    this.bgColor = null;
    this.dragBgColor = null;
    this.borderColor = null;
    this.customStyle = '';

    this.isFocused = false;
    this.isPending = false;
    this.isVisible = true;
    this.isReadOnly = false;
    this.goingDuration = 0;
    this.comingDuration = 0;
    this.recurrenceRule = '';
    this.state = '';

    this.raw = {
        memo: '',
        hasToOrCc: false,
        hasRecurrenceRule: false,
        location: null,
        class: 'public', // or 'private'
        creator: {
            name: '',
            avatar: '',
            company: '',
            email: '',
            phone: ''
        }
    };
}

function generateTime(appt, renderStart, renderEnd) {
    var startDate = moment(renderStart.getTime())
    var endDate = moment(renderEnd.getTime());
    var diffDate = endDate.diff(startDate, 'days');

    appt.isAllday = false;
    appt.category = 'time';

    startDate.add(chance.integer({ min: 0, max: diffDate }), 'days');
    startDate.hours(chance.integer({ min: 0, max: 23 }))
    startDate.minutes(chance.bool() ? 0 : 30);
    appt.start = startDate.toDate();

    endDate = moment(startDate);
    
    appt.end = endDate
        .add(chance.integer({ min: 1, max: 4 }), 'hour')
        .toDate();

    if (chance.bool({ likelihood: 20 })) {
        appt.goingDuration = chance.integer({ min: 30, max: 120 });
        appt.comingDuration = chance.integer({ min: 30, max: 120 });;

        if (chance.bool({ likelihood: 50 })) {
            appt.end = appt.start;
        }
    }
}

function generateNames() {
    var names = [];
    var i = 0;
    var length = chance.integer({ min: 1, max: 10 });

    for (; i < length; i += 1) {
        names.push(chance.name());
    }

    return names;
}

function generateRandomAppointment(apptType, renderStart, renderEnd) {
    var appt = new AppointmentInfo();

    appt.id = chance.guid();
    appt.calendarId = apptType.id;

    appt.title = chance.sentence({ words: 3 });
    appt.body = chance.bool({ likelihood: 20 }) ? chance.sentence({ words: 10 }) : '';
    appt.isReadOnly = chance.bool({ likelihood: 20 });
    generateTime(appt, renderStart, renderEnd);

    appt.isPrivate = chance.bool({ likelihood: 10 });
    appt.location = chance.address();
    appt.attendees = chance.bool({ likelihood: 70 }) ? generateNames() : [];
    appt.recurrenceRule = chance.bool({ likelihood: 20 }) ? 'repeated events' : '';
    appt.state = chance.bool({ likelihood: 20 }) ? 'Free' : 'Busy';
    appt.color = apptType.color;
    appt.bgColor = apptType.bgColor;
    appt.dragBgColor = apptType.dragBgColor;
    appt.borderColor = apptType.borderColor;

    appt.raw.memo = chance.sentence();
    appt.raw.creator.name = chance.name();
    appt.raw.creator.avatar = chance.avatar();
    appt.raw.creator.company = chance.company();
    appt.raw.creator.email = chance.email();
    appt.raw.creator.phone = chance.phone();

    if (chance.bool({ likelihood: 20 })) {
        var travelTime = chance.minute();
        appt.goingDuration = travelTime;
        appt.comingDuration = travelTime;
    }

    AppointmentList.push(appt);
}

function generateAppointments(viewName, renderStart, renderEnd) {
    AppointmentList = [];
    AppointmentTypes.forEach(function (apptType) {
        var i = 0, length = 10;
        if (viewName === 'month') {
            length = 3;
        } else if (viewName === 'day') {
            length = 4;
        }
        for (; i < length; i += 1) {
            generateRandomAppointment(apptType, renderStart, renderEnd);
        }
    });
}

function fetchAppointments(from, to, callback) {
    AppointmentList = [];
    var url = "/web/Scheduler/Search?from=" + from._date.toISOString() + "&to=" + to._date.toISOString();
    $(".provider-checkbox:checked").each(function () {
        url = url + '&pid=' + $(this)[0].id;
    });
    $(".site-checkbox:checked").each(function () {
        url = url + '&sid=' + $(this)[0].id;
    });
    $(".resource-checkbox:checked").each(function () {
        url = url + '&rid=' + $(this)[0].id;
    });

    $.ajax({
        type: "get",
        url: url,
        contentType: false,
        processData: false,
        success: function (appointments) {
            appointments.forEach(function (appointment) {
                var appt = new AppointmentInfo();
                appt.id = appointment.id;
                appt.calendarId = appointment.typeId;
                appt.providerId = appointment.providerId;
                appt.siteId = appointment.siteId;
                appt.statusId = appointment.statusId;
                appt.patientId = appointment.patientId;
                appt.marketingId = appointment.marketingId;
                appt.resourceId = appointment.resourceId;
                appt.authorizationId = appointment.authorizationId;
                appt.title = appointment.patient + '; ' + appointment.type;
                appt.body = appointment.type;
                appt.isReadOnly = false;
                appt.isAllday = false;
                appt.category = 'time';
                appt.start = Date.parse(appointment.fromDate);
                appt.end = Date.parse(appointment.toDate);
                appt.isPrivate = false;
                appt.location = appointment.site;
                appt.attendees = [appointment.provider, appointment.patient];
                //appt.recurrenceRule = chance.bool({ likelihood: 20 }) ? 'repeated events' : '';
                appt.state = appointment.status;
                var apptType = findAppointmentType(String(appointment.typeId));
                appt.color = apptType.color;
                appt.bgColor = apptType.bgColor;
                appt.dragBgColor = apptType.dragBgColor;
                appt.borderColor = apptType.borderColor;
                
                //appt.raw.memo = chance.sentence();
                //appt.raw.creator.name = chance.name();
                //appt.raw.creator.avatar = chance.avatar();
                //appt.raw.creator.company = chance.company();
                //appt.raw.creator.email = chance.email();
                //appt.raw.creator.phone = chance.phone();

                //if (chance.bool({ likelihood: 20 })) {
                //    var travelTime = chance.minute();
                //    appt.goingDuration = travelTime;
                //    appt.comingDuration = travelTime;
                //}

                AppointmentList.push(appt);
            });
            callback();
        },
        error: function (message) {
            callback();
        }
    });
}


function updateAppointment(appointment, changes, ignoreWarnings, callback) {
    
    var model = new Object();
    model.id = appointment.id;
    if (changes.start) {
        model.startsAt = changes.start._date.toISOString();
    } else {
        model.startsAt = appointment.start._date.toISOString();
    }
    
    if (currentChanges.end) {
        model.endsAt = changes.end._date.toISOString();
    } else {
        model.endsAt = appointment.end._date.toISOString();
    }

    model.appointmentTypeId = appointment.calendarId;
    model.providerId = appointment.providerId;
    model.appointmentStatusId = appointment.statusId;
    model.siteId = appointment.siteId;
    model.patientId = appointment.patientId;
    model.marketingId = appointment.marketingId;
    model.resourceId = appointment.resourceId;
    model.authorizationId = appointment.authorizationId;
    $.ajax({
        type: "post",
        contentType: "application/json; charset=utf-8",
        url: "/web/Scheduler/UpdateAppointment?ignoreWarnings="+ignoreWarnings,
        data: JSON.stringify(model),
        success: function (data) {
            callback(data["errors"], data["warnings"]);
        },
        error: function (data) {
            callback([], []);
        }
    });
}