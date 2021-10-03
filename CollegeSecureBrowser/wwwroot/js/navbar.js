$(document).ready(function() {
    $(".list").click(function() {
        $(".list").removeClass("active");
        $(this).addClass("active");
    });
});

async function ToggleMenu() {
    $("#header-Menu").toggleClass("Collapse");
    $(".navigation").toggleClass("CollapseMenu");
    $(".Logo").toggleClass("Collage-Logo");
    $("#Support").toggleClass("Collapse-hide");
    $("#Support").toggleClass("mt-4");
}

async function SetNavbar() {
    const url = window.location.pathname;

    $(".list").removeClass("active");

    if (isPath(url, "Settings")) {
        $("#SettingsNav").addClass("active");
    } else {
        $("#DashboardNav").addClass("active");
    }
}

function isPath(url,path) {
    if (url.includes(path))
        return true;
    else
        return false;
}

SetNavbar();