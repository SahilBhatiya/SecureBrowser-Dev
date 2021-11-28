const video = document.getElementById("crrVideo")
let model
let canvas = document.getElementById("crrCanvas")
let ctx = canvas.getContext("2d")
let videoData
const videoWidth = 350
const videoHeight = parseInt(videoWidth / 1.5)

const setupCamera = () => {
    navigator.mediaDevices.getUserMedia({
        video: { width: videoWidth, height: videoHeight },
        audio: false,
    }).then(stream => {
        video.srcObject = stream;
    })

    canvas.height = videoHeight
    canvas.width = videoWidth
}

const detectFaces = async () => {
    const predection = await model.estimateFaces(video, false)
    if (predection.length != 1) {
        ctx.drawImage(video, 0, 0, videoWidth, videoHeight)
        videoData = canvas.toDataURL("image/jpeg");
        const sEmail = GetURLParameter("studentEmail")
        const examId = GetURLParameter("examId")
        const clgEmail = GetURLParameter("clgEmail")
        $.ajax({
            type: "POST",
            url: `/Home/SendCopyData`,
            data: {
                "StudentEmail": sEmail,
                "Image": videoData,
                "examId": examId,
                "clgEmail": clgEmail,
            },
            success: function (data1) {
                if (!data1.value.toString().includes("t")) {
                } else {
                    DisplayMsg(1, "Caught!", "Your Video Is Being Sent To Your College", 4000, "b")
                }
            }
        });
    }
}

setupCamera()

video.addEventListener("loadeddata", async () => {
    model = await blazeface.load()
    setInterval(detectFaces, 200)
})
