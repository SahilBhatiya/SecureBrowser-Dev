const { remote } = require('electron');
const { getCurrentWindow, globalShortcut } = require('electron').remote;

const win = remote.getCurrentWindow();
var IntervalOfShow, IntervalOfFullscreen, elem = document.documentElement, isExamStatred;

function Reload() {
    window.location.reload();
}

function CloseWindow() {
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


function CreateAdmin() {
    const data = $('#CreateAdmin').serializeArray();

    const obj = getFormObj(data);

    if (!(obj["Password"] == $("#ConfirmPass").val() && obj["Password"] != "")) {
        DisplayMsg(1, "Error", "Password Doesnt Match Or Password Field Is Empty", 3000, "b");
        return;
    }

    $.ajax({
        type: "POST",
        url: `/Home/CreateAdmin`,
        data: data,
        success: function (data) {
            if (data.value.toString().includes("Exsits"))
                DisplayMsg(1, "Error!", data.value, 4000, "b");
            else
                window.location = "/Dashboard/";
        }
    });
}

function Login() {
    const data = $('#LoginAdmin').serializeArray();

    const obj = getFormObj(data);

    $.ajax({
        type: "POST",
        url: `/Home/LoginAdmin`,
        data: data,
        success: function (data) {
            if (data.value == "true") {
                window.location = "/Dashboard/";
            } else {
                DisplayMsg(1, "Invalid Credentials!", "Please Enter Correct Credentials", 4000, "b");
            }
        }
    });
}

function Logout() {
    $.ajax({
        type: "POST",
        url: `/Home/Logout`,
        success: function (data) {
            if (data.value == "true") {
                window.location = "/";
            }
        }
    });
}


function Connect() {
    $.ajax({
        type: "POST",
        url: `/Home/Connect`,
        success: function (data) {
            alert(data);
        }
    });

}


function getFormObj(data) {
    var formObj = {};
    $.each(data, function (i, input) {
        formObj[input.name] = input.value;
    });
    return formObj;
}


function UpdateCollege() {
    const data = $('#UpdateCollege').serializeArray();
    const obj = getFormObj(data);
    console.log(data);
    $.ajax({
        type: "POST",
        url: `/Colleges/EditPost`,
        data: data,
        success: function (data1) {
            if (data1.value.toString().includes("l")) {
                DisplayMsg(1, "Error!", data1.value, 4000, "b");
            }
            else {
                DisplayMsg(0, "Updated!", obj.Name + " Updated", 4000, "b");
            }

        }
    });
}


function ResetCollegePassword(email) {
    $.ajax({
        type: "POST",
        url: `/Colleges/ResetPassword`,
        data: {
            "email": email
        },
        success: function (data1) {
            if (data1.value.toString().includes("l")) {
                DisplayMsg(1, "Error!", "Cannot Reset", 4000, "b");
            }
            else {
                DisplayMsg(0, "Updated!", "Password Reset", 4000, "b");
            }

        }
    });
}



function RemoveCollege(email) {
    $.ajax({
        type: "POST",
        url: `/Colleges/DeleteCollege`,
        data: {
            "email": email
        },
        success: function (data1) {
            if (data1.value.toString().includes("l")) {
                DisplayMsg(1, "Error!", "Cannot Remove", 4000, "b");
            }
            else {
                DisplayMsg(0, "Removed!", "College Removed", 4000, "b");
                document.getElementById(email).remove();
            }

        }
    });

}


function CreateCollege() {
    const data = $('#CreateCollege').serializeArray();

    const obj = getFormObj(data);

    $.ajax({
        type: "POST",
        url: `/Colleges/CreateCollege`,
        data: data,
        success: function (data1) {
            if (data1.value.toString().includes("Exsits")) {
                DisplayMsg(1, "Error!", data1.value, 4000, "b");
            }

            else {
                DisplayMsg(0, "Created!", "College Created", 4000, "b");
            }

        }
    });
}