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