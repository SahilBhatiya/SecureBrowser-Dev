const { remote } = require('electron');
const { getCurrentWindow, globalShortcut } = require('electron').remote;

const win = remote.getCurrentWindow();
var IntervalOfShow, IntervalOfFullscreen, elem = document.documentElement, isExamStatred;

win.webviewTag = true;

function Reload() {
    window.location.reload();
}

function CloseWindow() {
    StopInterval();
    win.close();
}

function MinimizeWindow() {
    if (!isExamStatred)
        win.minimize();
    else {

    }
}

function MaximizeeWindow() {
    if (!isExamStatred) {
        if (!win.isMaximized())
            win.maximize();
        else
            win.unmaximize();
    }

}

function startFunctionOnLoad() {
    if (window.location.toString().includes("/Home/CurrentExam")) {
        setTimeout(StartInterval(120), 3000)

    }
}
startFunctionOnLoad()


function StartInterval(fps = 240) {
    StopInterval();
    openFullscreen();
    win.setAlwaysOnTop(true, 'screen');
    IntervalOfShow = setInterval(() => {
        win.show();
    }, 1000 / fps);
    isExamStatred = true;
}

$(document).on("keydown", function (ev) {
    if ((ev.keyCode === 27 || ev.keyCode === 122 || ev.keyCode === 18 || ev.keyCode === 9 || ev.keyCode === 91 || ev.keyCode === 17 || ev.keyCode == 115) && isExamStatred == true) return false
})

function StopInterval() {
    isExamStatred = false;
    win.setAlwaysOnTop(false, 'screen');
    clearInterval(IntervalOfShow);
}

function openFullscreen() {
    elem.requestFullscreen();
}

function ExitFullscreen() {
    elem.exitFullscreen();
}

function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(showPosition);
    }
}

function showPosition(position) {
    var x = document.getElementById("pos");
    x.innerHTML = "Latitude: " + position.coords.latitude +
        "<br>Longitude: " + position.coords.longitude;
}

function GetAllExams() {
    console.log("helllo")
    $.ajax({
        type: "POST",
        url: `/Home/PrintExams`,
        data: {
            Email: $("#College").val()
        },
        success: function (data1) {
            if (!data1.value.toString().includes("option")) {
                DisplayMsg(1, "Error!", data1.value, 4000, "b");
            } else {
                console.log(data1.value.toString());
                document.getElementById("Exams").innerHTML = data1.value.toString();
                Loadselect()
            }
        }
    });
}


function LoginStudent() {
    $.ajax({
        type: "POST",
        url: `/Home/LoginStudent`,
        data: {
            Email: $("#College").val(),
            StudentEmail: $("#StudentEmail").val(),
            Password: $("#Password").val(),
        },
        success: function (data1) {
            if (!data1.value.toString().includes("t")) {
                DisplayMsg(1, "Error!", data1.value, 4000, "b");
            } else {
                window.location = `/Home/SelectExam?clg=${$("#College").val()}&email=${$("#StudentEmail").val()}&pass=${$("#Password").val()}`
            }
        }
    });
}



/* To Show Display Msg */
async function DisplayMsg(cssClass, HeadingText, ContentText, timer, pos) {

    /* cssClass => 0 - sucess
                => 1 - danger
                => 2 - warning
                => 3 - info
    */

    /* 
        pos => t -top
            => b - bottom

    */

    let classes = ["bg-success-alert", "bg-danger-alert", "bg-warning-alert", "bg-info-alert"];

    if (pos == "t") {
        $("#AlertMsg").css("top", "0px");
        $("#AlertMsg").css("bottom", "");
    } else {
        $("#AlertMsg").css("bottom", "0px");
        $("#AlertMsg").css("top", "");
    }



    classes.forEach(element => {
        $("#AlertMsg").removeClass(element);
    });

    $("#AlertMsg").addClass("show " + classes[cssClass]);
    $("#DisplayAlertHeading").text(HeadingText);
    $("#DisplayAlertContent").text(ContentText);

    /* To Remove DisplayMsg After Sometime */
    if (timer != 0) {
        removeDisplayMsg(timer);
    }
}

var DisplayMsgTimer;
/* To Remove DisplayMsg */
async function removeDisplayMsg(x) {
    clearTimeout(DisplayMsgTimer);
    DisplayMsgTimer = setTimeout(() => {
        if ($("#AlertMsg").css("top") == "0px") {
            $("#AlertMsg").css("top", "-100px");
            $("#AlertMsg").css("bottom", "");
        } else {
            $("#AlertMsg").css("bottom", "-100px");
            $("#AlertMsg").css("top", "");
        }
        $("#AlertMsg").removeClass("show");
    }, x);
}

var ExamLink

function LoadExamModal() {
    if ($("#Exam").val().trim() != "") {
        $.ajax({
            type: "POST",
            url: `/Home/LoadExam`,
            data: {
                clg: GetURLParameter('clg'),
                email: GetURLParameter('email'),
                pass: GetURLParameter('pass'),
                exam: $("#Exam").val(),
            },
            success: function (data1) {
                if (data1.value.toString().includes("false")) {
                    DisplayMsg(1, "Error!", data1.value, 4000, "b");
                } else {
                    const data2 = jQuery.parseJSON(data1.value)
                    const startTime = dateFormat(new Date(data2.Start))
                    const endTime = dateFormat(new Date(data2.End))
                    const Exam = data2.Exam
                    console.log(data2.Exam.Name)
                    $("#ExamName").text(`Name : ${Exam.Name}`)
                    $("#ExamStartTime").text(`Start Time : ${startTime}`)
                    $("#ExamEndTime").text(`End Time : ${endTime}`)
                    document.getElementById("ExamLink").href = `${Exam.Link}`
                    document.getElementById("ExamLink").href = `/Home/CurrentExam?examId=${Exam.Id}&studentEmail=${GetURLParameter('email')}&clgEmail=${GetURLParameter('clg')}&pass=${GetURLParameter('pass')}&endTime=${endTime}`
                    $('#ExamModal').modal('show')
                    ExamLink = Exam.Link
                }
            }
        });
    } else {
        DisplayMsg(1, "Error!", "Exam Not Selected", 4000, "b");
    }

}

function GetURLParameter(sParam) {
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++) {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == sParam) {
            return decodeURIComponent(sParameterName[1]);
        }
    }
}


function dateFormat(date) {
    var day = date.getDate()
    var month = date.getMonth() + 1
    var year = date.getFullYear()
    var hour = date.getHours()
    var min = date.getMinutes()
    var sec = date.getSeconds()
    var meridian = hour < 12 ? "AM" : "PM"
    hour = hour < 12 ? hour : hour - 12
    return `${day}/${month}/${year} - ${hour}:${min}:${sec} ${meridian}`
}

onload = () => {
    const webview = document.querySelector('webview')
    const indicator = document.querySelector('.indicator')

    const loadstart = () => {
        indicator.innerText = 'loading...'
    }

    const loadstop = () => {
        indicator.innerText = ''
    }

    webview.addEventListener('did-start-loading', loadstart)
    webview.addEventListener('did-stop-loading', loadstop)
}
