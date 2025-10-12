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

 
