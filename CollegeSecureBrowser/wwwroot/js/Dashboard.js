function SetUserName() {
    $.ajax({
        type: "POST",
        url: `/Dashboard/GetUser`,
        success: function (data) {
            const newData = JSON.parse(data.value);
            if (newData.Name != null) {

                $("#UsernameDropdown").text(newData.Name.substring(0, 10) + "...");
                $(".Header-User").removeClass("d-none");
            }
        }
    });
}

SetUserName();

function UpdateBasicDetails() {
    const data = $('#UpdateBasicDetails').serializeArray();

    const obj = getFormObj(data);

    $.ajax({
        type: "POST",
        url: `/Dashboard/UpdateCollege`,
        data: data,
        success: function (data) {
            if (data.value == "true") {
                DisplayMsg(0, "Updated!", "Basic Details Updated", 4000, "b");
            } else {
                DisplayMsg(1, "Error!", "Cannot Update", 4000, "b");
            }
        }
    });
}

function UpdatePassword() {
    const data = $('#UpdatePasswordForm').serializeArray();

    const obj = getFormObj(data);
    if (obj.NewPassword == obj.Password1) {
        $.ajax({
            type: "POST",
            url: `/Dashboard/UpdatePassword`,
            data: data,
            success: function (data) {
                if (data.value == "true") {
                    DisplayMsg(0, "Updated!", "Password Changed", 4000, "b");
                } else {
                    DisplayMsg(1, "Error!", "Password Doesnt Match", 4000, "b");
                }
            }
        });
    } else {
        DisplayMsg(1, "Error!", "Password Doesnt Match", 4000, "b");
    }

}



function DeleteAccount() {
    const data = $('#DeleteAccount').serializeArray();

    $.ajax({
        type: "POST",
        url: `/Settings/DeleteAccountPost`,
        data: data,
        success: function (data) {
            if (data.value == "true") {
                Logout();
                window.location = "/";
            } else {
                DisplayMsg(1, "Error!", "Cannot Delete Your College Account", 4000, "b");
            }
        }
    });
}


function UpdateStudent() {
    const data = $('#UpdateStudent').serializeArray();

    $.ajax({
        type: "POST",
        url: `/Students/UpdateStudent`,
        data: data,
        success: function (data) {
            console.log(data.value)
            if (data.value.includes("Updated")) {
                DisplayMsg(0, "Updated!", "Student Details Updated Successfull", 4000, "b");
            } else {
                DisplayMsg(1, "Error!", "Student Details Cannot Be Updated", 4000, "b");
            }
        }
    });
}

function UpdateExam() {
    const data = $('#UpdateExam').serializeArray();

    if (data[3].value.toString().trim() == "") {
        data[3].value = data[2].value;
    }
    if (data[5].value.toString().trim() == "") {
        data[5].value = data[4].value;
    }

    $.ajax({
        type: "POST",
        url: `/Exam/EditExam`,
        data: data,
        success: function (data) {
            console.log(data.value)
            if (data.value.includes("true")) {
                DisplayMsg(0, "Updated!", "Exam Details Updated Successfull", 4000, "b");
            } else {
                DisplayMsg(1, "Error!", "Exam Details Cannot Be Updated", 4000, "b");
            }
        }
    });
}