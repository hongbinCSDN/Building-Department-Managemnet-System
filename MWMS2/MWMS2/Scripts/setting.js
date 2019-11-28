
window.AppName = "App 1 v0.24";




function openReport(form) {
    var rootPath = "http://localhost:65107" + document.getElementById("rootPath").value;
    var win = popupWinds(rootPath +"/Report/GenerateReport?" + ($("#" + form).serialize()), 'POP_REPORT', 'toolbar=no, location=yes, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, copyhistory=no, width=' + 1000 + ', height=' + 700);

}