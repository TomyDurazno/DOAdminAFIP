$(() => {
    var dropzone = new Dropzone(document.querySelector("#dropzone-component"));

    dropzone.on("success", obj => {
        var ret = JSON.parse(obj.xhr.response);

        window.open("/download?key=" + ret.key, "_blank");
    });
})