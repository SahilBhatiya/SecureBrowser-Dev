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

    $(".nav-link").removeClass("active");

    if (isPath(url, "Colleges")) {
        $("#Colleges").addClass("active");
    } else if (isPath(url, "Admin")) {
        $("#Admin").addClass("active");
    } else if (isPath(url, "Settings")) {
        $("#Settings").addClass("active");
    } else if (isPath(url, "Students")) {
        $("#Students").addClass("active");
    } else if (isPath(url, "Exam")) {
        $("#Exams").addClass("active");
    } else {
        $("#Home").addClass("active");
    }
}

function isPath(url,path) {
    if (url.includes(path))
        return true;
    else
        return false;
}

SetNavbar();