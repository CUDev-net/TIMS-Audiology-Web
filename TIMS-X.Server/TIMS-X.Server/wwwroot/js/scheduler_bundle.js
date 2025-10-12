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
'use strict';
var cal;


function setRenderRangeText() {
    var renderRange = document.getElementById('renderRange');
    var options = cal.getOptions();
    var viewName = cal.getViewName();
    var html = [];
    if (viewName === 'day') {
        html.push(moment(cal.getDate().getTime()).format('YYYY.MM.DD'));
    } else if (viewName === 'month' &&
        (!options.month.visibleWeeksCount || options.month.visibleWeeksCount > 4)) {
        html.push(moment(cal.getDate().getTime()).format('YYYY.MM'));
    } else {
        html.push(moment(cal.getDateRangeStart().getTime()).format('YYYY.MM.DD'));
        html.push(' ~ ');
        html.push(moment(cal.getDateRangeEnd().getTime()).format(' MM.DD'));
    }
    renderRange.innerHTML = html.join('');
}



function fillAppointments() {
    cal.clear();
    //generateAppointments(cal.getViewName(), cal.getDateRangeStart(), cal.getDateRangeEnd());
    //cal.createSchedules(AppointmentList);
    //refreshAppointmentTypeVisibility();
    fetchAppointments(cal.getDateRangeStart(), cal.getDateRangeEnd(), function () {
        cal.createSchedules(AppointmentList);
        refreshAppointmentTypeVisibility();
    });
    
}


function refreshAppointmentTypeVisibility() {
    var calendarElements = Array.prototype.slice.call(document.querySelectorAll('#calendarList input'));

    AppointmentTypes.forEach(function (calendar) {
        cal.toggleSchedules(calendar.id, !calendar.checked, false);
    });

    cal.render(true);

    calendarElements.forEach(function (input) {
        var span = input.nextElementSibling;
        span.style.backgroundColor = input.checked ? span.style.borderColor : 'transparent';
    });
}


function setDropdownCalendarType() {
    var calendarTypeName = document.getElementById('calendarTypeName');
    var calendarTypeIcon = document.getElementById('calendarTypeIcon');
    var options = cal.getOptions();
    var type = cal.getViewName();
    var iconClassName;

    if (type === 'day') {
        type = 'Day';
        iconClassName = 'calendar-icon ic_view_day';
    } else if (type === 'week') {
        if (options.week.workweek) {
            type = '5 Day';
        } else {
            type = 'Week';
        }
        iconClassName = 'calendar-icon ic_view_week';
    } else {
        type = 'Month';
        iconClassName = 'calendar-icon ic_view_month';
    }

    calendarTypeName.innerHTML = type;
    calendarTypeIcon.className = iconClassName;
}

function getDataAction(target) {
    return target.dataset ? target.dataset.action : target.getAttribute('data-action');
}

/**
 * A listener for click the menu
 * @param {Event} e - click event
 */
function onClickMenu(e) {
    var target = $(e.target).closest('a[role="menuitem"]')[0];
    var action = getDataAction(target);
    var options = cal.getOptions();
    var viewName = '';

    console.log(target);
    console.log(action);
    switch (action) {
        case 'toggle-daily':
            viewName = 'day';
            break;
        case 'toggle-5-day':
            viewName = 'week';
            options.week.workweek = true;
            break;
        case 'toggle-weekly':
            viewName = 'week';
            break;
        case 'toggle-monthly':
            options.month.visibleWeeksCount = 0;
            viewName = 'month';
            break;
        default:
            break;
    }

    cal.setOptions(options, true);
    cal.changeView(viewName, true);

    setDropdownCalendarType();
    setRenderRangeText();
    fillAppointments();
}


var resizeThrottled = tui.util.throttle(function () {
    cal.render();
}, 50);


function saveNewAppointment(apptData) {
    //var calendar = apptData.calendar || findAppointmentType(apptData.calendarId);
    //var appt = {
    //    id: String(chance.guid()),
    //    title: apptData.title,
    //    isAllDay: apptData.isAllDay,
    //    start: apptData.start,
    //    end: apptData.end,
    //    category: apptData.isAllDay ? 'allday' : 'time',
    //    dueDateClass: '',
    //    color: calendar.color,
    //    bgColor: calendar.bgColor,
    //    dragBgColor: calendar.bgColor,
    //    borderColor: calendar.borderColor,
    //    location: apptData.location,
    //    raw: {
    //        class: apptData.raw['class']
    //    },
    //    state: apptData.state
    //};
    //if (calendar) {
    //    apptData.calendarId = calendar.id;
    //    apptData.color = calendar.color;
    //    apptData.bgColor = calendar.bgColor;
    //    apptData.borderColor = calendar.borderColor;
    //}

    //cal.createSchedules([appt]);

    //refreshScheduleVisibility();
}

var currentAppt;
var currentChanges;

function handleWarningDlgYes() {
    var dlg = $('#ApptWarningDialog');
    dlg.modal('toggle');
    updateAppointment(currentAppt, currentChanges, true, function (errors, warnings) {
        
        if (errors.length > 0) {
            showErrors(errors);
            return;
        }
        cal.updateSchedule(currentAppt.id, currentAppt.calendarId, currentChanges);
    });
}
function handleConfirmUpdateYes() {
    var dlg = $('#ConfirmDialog');
    dlg.modal('toggle');

    updateAppointment(currentAppt, currentChanges, false, function (errors, warnings) {

        if (errors.length > 0) {
            showErrors(errors);
            return;
        }
        if (warnings.length > 0) {
            showWarnings(warnings);
            return;
        }

        cal.updateSchedule(currentAppt.id, currentAppt.calendarId, currentChanges);
    });
}
function showConfirmationDialog(appt, message1, message2) {
    var dlg = $('#ConfirmDialog');
    const $apptTitle = $('<span>', { class: "badge" });
    $apptTitle.css("background-color", appt.bgColor);
    $apptTitle.css("color", appt.color);
    $apptTitle.text(appt.title);
    const $titleContainer = $('<h5>');
    $titleContainer.text("Confirm Change for ");
    $apptTitle.appendTo($titleContainer);
    var modalHeader = dlg.find('.modal-title');
    modalHeader.empty();
    $titleContainer.appendTo(modalHeader);
    
    // <span class="badge badge-secondary mr-1">@User.FindFirstValue(StringConstants.OfficeCode)</span><span>@User.FindFirstValue(ClaimTypes.Name)</span>
    
    dlg.find('.modal-body-text').text(message1);
    dlg.find('.modal-body-text2').text(message2);
    dlg.modal();
}

function showWarnings(warnings) {
    var dlg = $('#ApptWarningDialog');
    var tgt = dlg.find('.modal-body');
    tgt.empty();

    const $ul = $('<ul>', { class: "appt-warnings" }).append(
        warnings.map(warning =>
            $("<li>").append($("<span>").text(warning))
        )
    );
    $ul.appendTo(tgt);

    dlg.modal();
}

function showErrors(errors) {
    var dlg = $('#ApptErrorDialog');
    var tgt = dlg.find('.modal-body');
    tgt.empty();

    const $ul = $('<ul>', { class: "appt-errors" }).append(
        errors.map(err =>
            $("<li>").append($("<span>").text(err))
        )
    );
    $ul.appendTo(tgt);

    dlg.modal();
}

function getDurationText(fromDate, toDate) {

    var from = fromDate.getTime();
    var to = toDate.getTime();
    var delta = Math.abs(to - from) / 1000;
    var days = Math.floor(delta / 86400);
    delta -= days * 86400;
    var oldHours = Math.floor(delta / 3600) % 24;
    delta -= oldHours * 3600;
    var minutes = Math.floor(delta / 60) % 60;


    var duration = '';
    if (oldHours > 0) {
        if (oldHours == 1) {
            duration = '1 hour';
        } else {
            duration = '' + oldHours + ' hours';
        }
    }
    if (minutes > 0) {
        if (duration.length > 0) {
            duration = duration + ', ';
        } 
        duration = duration + minutes + ' minutes';
    }


    return duration;
}

function getTimeText(date) {
    var hour = date.getHours();
    var ampm = hour < 12 ? "AM" : "PM";
    hour = (hour < 10 ? "0" : "") + hour;
    var min = date.getMinutes();
    min = (min < 10 ? "0" : "") + min;
    return hour + ":" + min + " " + ampm;
}

function getAppointmentDateTimeText(fromDate, toDate) {
    var dt = getLocaleShortDateString(fromDate);
    var fromTime = getTimeText(fromDate);
    var toTime = getTimeText(toDate);
    return dt + ' ' + fromTime + ' - ' + toTime;
}

function getLocaleShortDateString(d) {
    var f = { "en-US": "M/d/yyyy", "en-CA": "dd/MM/yyyy", "en-GB": "dd/MM/yyyy", "en-AU": "d/MM/yyyy", "en-NZ": "d/MM/yyyy"};
    var l = navigator.language ? navigator.language : navigator['userLanguage'], y = d.getFullYear(), m = d.getMonth() + 1, d = d.getDate();
    f = (l in f) ? f[l] : "MM/dd/yyyy";
    function z(s) { s = '' + s; return s.length > 1 ? s : '0' + s; }
    f = f.replace(/yyyy/, y); f = f.replace(/yy/, String(y).substr(2));
    f = f.replace(/MM/, z(m)); f = f.replace(/M/, m);
    f = f.replace(/dd/, z(d)); f = f.replace(/d/, d);
    return f;
}

function setEventListeners() {
    $('#calmenu').on('click', onClickNavi);
    $('.dropdown-menu a[role="menuitem"]').on('click', onClickMenu);

/*

    $('#btn-save-schedule').on('click', onNewSchedule);
    $('#btn-new-schedule').on('click', createNewSchedule);

    $('#dropdownMenu-calendars-list').on('click', onChangeNewScheduleCalendar);*/

    window.addEventListener('resize', resizeThrottled);


    // event handlers
    cal.on({
        'clickMore': function (e) {
            console.log('clickMore', e);
        },
        'clickSchedule': function (e) {
            console.log('clickSchedule', e);
        },
        'clickDayname': function (date) {
            console.log('clickDayname', date);
        },
        'beforeCreateSchedule': function (e) {
            console.log('beforeCreateSchedule', e);
            saveNewAppointment(e);
        },
        'beforeUpdateSchedule': function (e) {
            currentAppt = e.schedule;
            currentChanges = e.changes;

            if (currentChanges.start || currentChanges.end) {


                if ((!currentChanges.start) || (!currentChanges.end)) {
                    // the duration changed.
                    var newFrom = e.schedule.start._date;
                    var newTo = e.schedule.end._date;

                    if (currentChanges.start) {
                        newFrom = currentChanges.start._date;
                    }
                    else {
                        newTo = currentChanges.end._date;
                    }

                    var oldDuration = 'Old Duration: ' + getDurationText(e.schedule.start._date, e.schedule.end._date);
                    var newDuration = 'New Duration: ' + getDurationText(newFrom, newTo);
                    showConfirmationDialog(e.schedule, oldDuration, newDuration);


                } else {
                    var oldDate = 'Old Time: ' + getAppointmentDateTimeText(e.schedule.start._date, e.schedule.end._date);
                    var newDate = 'New Time: ' + getAppointmentDateTimeText(currentChanges.start._date, currentChanges.end._date);

                    // the appointment moved
                    showConfirmationDialog(e.schedule, oldDate, newDate);
                }
            } else {

                updateAppointment(currentAppt, currentChanges, false, function (errors, warnings) {

                    if (errors.length > 0) {
                        showErrors(errors);
                        return;
                    }
                    if (warnings.length > 0) {
                        showWarnings(warnings);
                        return;
                    }

                    cal.updateSchedule(currentAppt.id, currentAppt.calendarId, currentChanges);
                });
            } 

            
            //console.log('beforeUpdateSchedule', e);
            
            //refreshScheduleVisibility();
        },
        'beforeDeleteSchedule': function (e) {
            console.log('beforeDeleteSchedule', e);
            cal.deleteSchedule(e.schedule.id, e.schedule.calendarId);
        },
        'afterRenderSchedule': function (e) {
            var schedule = e.schedule;
            // var element = cal.getElement(schedule.id, schedule.calendarId);
            // console.log('afterRenderSchedule', element);
        },
        'clickTimezonesCollapseBtn': function (timezonesCollapsed) {
            console.log('timezonesCollapsed', timezonesCollapsed);

            if (timezonesCollapsed) {
                cal.setTheme({
                    'week.daygridLeft.width': '77px',
                    'week.timegridLeft.width': '77px'
                });
            } else {
                cal.setTheme({
                    'week.daygridLeft.width': '60px',
                    'week.timegridLeft.width': '60px'
                });
            }

            return true;
        }
    });
}



function onClickNavi(e) {
    var action = getDataAction(e.target);
    switch (action) {
    case 'move-prev':
        cal.prev();
        break;
    case 'move-next':
        cal.next();
        break;
    case 'move-today':
        cal.today();
            break;
    default:
        return;
    }

    setRenderRangeText();
    fillAppointments();
}


//function execute_test() {
//    var result = false;
//    $.ajax({
//        type: "GET",
//        url: "/web/Scheduler/Search?from=" + serverId + "&database=" + encodeURIComponent(database),
//        contentType: "application/json; charset=utf-8",
//        async: false,
//        dataType: "json",
//        success: function (response) {
//            var parsedResponse = JSON.parse(response);
//            if (parsedResponse) {
//                result = true;
//            } else {
//                result = false;
//            }
//        },
//        failure: function (response) {
//            result = false;
//        }
//    });
//    return result;
//}


/* eslint-disable */
function init() {
    cal = new tui.Calendar('#calendar', {
        defaultView: 'week',
        taskView: false,
        milestoneView: false,
        isAllDay: false,
        useCreationPopup: false,
        useDetailPopup: true
    });
    cal.setCalendars(AppointmentTypes);
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        cal.changeView('day', true);
    }
    setRenderRangeText();
    setDropdownCalendarType();
    setEventListeners();
    //fillAppointments();
    fetchAppointmentTypes(function () {
        fillAppointments();
    });


    

    
    
}



function saveFilter() {
    $("#SaveFilterButton").attr("disabled", true);
    $('#sidebar').toggleClass('active');
    fillAppointments();
}
function cancelFilter() {
    $('#sidebar').toggleClass('active');
}

function filterChanged() {
    $('#SaveFilterButton').removeAttr("disabled");
}

$(document).ready(function () {

    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });
    $('#sidebarCollapse2').on('click', function () {
        $('#sidebar').toggleClass('active');
    });
    $(".site-checkbox").each(function () {
        $(this).on("change", (event) => {
            var container = document.getElementById('sidebar');

            var resources = container.getElementsByClassName("resource-item");
            for (var i = 0, len = resources.length | 0; i < len; i = i + 1 | 0) {
                var siteId = resources[i].getAttribute("data-siteId");
                if (siteId != event.target.getAttribute("data-siteId")) {
                    continue;
                }
                resources[i].classList.toggle("active");
                if (resources[i].style.display === "none") {
                    resources[i].style.display = "block";
                } else {
                    resources[i].style.display = "none";
                }
            }
            //event.target.parentElement.parentElement.parentElement.querySelector(".nested").classList.toggle("active");
        });
    });

    var ignoreChecks = false;
    
    $("#CheckAllProviders").on("change", (event) => {
        
        var items = event.target.parentElement.parentElement.parentElement.querySelectorAll(".provider-checkbox");
        ignoreChecks = true;
        items.forEach(function (item) {
            item.checked = event.target.checked;
        });
        ignoreChecks = false;
    });

    $("#CheckAllSites").on("change", (event) => {

        var items = event.target.parentElement.parentElement.parentElement.querySelectorAll(".site-checkbox");
        ignoreChecks = true;
        items.forEach(function (item) {
            item.checked = event.target.checked;
        });
        ignoreChecks = false;
    });

    $(".CheckAllResources").each(function () {
        $(this).on("change", (event) => {
            var items = event.target.parentElement.parentElement.querySelectorAll(".resource-checkbox");
            ignoreChecks = true;
            items.forEach(function (item) {
                item.checked = event.target.checked;
            });
            ignoreChecks = false;
        });
    });

    $(".resource-checkbox").each(function () {
        $(this).on("change", (event) => {
            if (!ignoreChecks) {
                var checkAll = event.target.parentElement.parentElement.querySelector(".CheckAllResources");
                var resources = event.target.parentElement.parentElement.querySelectorAll(".resource-checkbox");
                var allChecked = true;
                resources.forEach(function (resource) {
                    if (!resource.checked) {
                        allChecked = false;
                    }
                });
                checkAll.checked = allChecked;
            }
        });
    });

    

    init();

    
});

 
