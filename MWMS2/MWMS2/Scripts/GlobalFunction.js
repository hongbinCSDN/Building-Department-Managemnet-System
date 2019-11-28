


function copyEngAddrToQp() {
    for (var i = 1; i <= 5; i++) {
        document.getElementById('C_IND_CERTIFICATE_QP_ADDRESS_E' + i).value =
            document.getElementById('HOME_ADDRESS_ENG_ADDRESS_LINE' + i).value;
    }
}
function copyChnAddrToQp() {
    for (var i = 1; i <= 5; i++) {
        document.getElementById('C_IND_CERTIFICATE_QP_ADDRESS_C' + i).value =
            document.getElementById('HOME_ADDRESS_CHI_ADDRESS_LINE' + i).value;
    }
}

function copyEngOfficeAddrToHome() {
    for (var i = 1; i <= 5; i++) {
        document.getElementById('HOME_ADDRESS_ENG_ADDRESS_LINE' + i).value =
            document.getElementById('OFFICE_ADDRESS_ENG_ADDRESS_LINE' + i).value;
    }
} 
function copyEngHomeAddrToOffice() {
    for (var i = 1; i <= 5; i++) {
        document.getElementById('OFFICE_ADDRESS_ENG_ADDRESS_LINE' + i).value =
            document.getElementById('HOME_ADDRESS_ENG_ADDRESS_LINE' + i).value;
    }
}
function copyChnHomeAddrToOffice() {
    for (var i = 1; i <= 5; i++) {
        document.getElementById('OFFICE_ADDRESS_CHI_ADDRESS_LINE' + i).value =
            document.getElementById('HOME_ADDRESS_CHI_ADDRESS_LINE' + i).value;
    }
}

function copyChnOfficeAddrToHome() {
    for (var i = 1; i <= 5; i++) {
        document.getElementById('HOME_ADDRESS_CHI_ADDRESS_LINE' + i).value =
            document.getElementById('OFFICE_ADDRESS_CHI_ADDRESS_LINE' + i).value;
    }
}
function copyEngAddrToBs() {
    for (var i = 1; i <= 5; i++) {
        document.getElementById('BS_ADDRESS_ENG_ADDRESS_LINE' + i).value =
            document.getElementById('HOME_ADDRESS_ENG_ADDRESS_LINE' + i).value;
    }
}
function copyChnAddrToBs() {
    for (var i = 1; i <= 5; i++) {
        document.getElementById('BS_ADDRESS_CHI_ADDRESS_LINE' + i).value =
            document.getElementById('HOME_ADDRESS_CHI_ADDRESS_LINE' + i).value;
    }
} 
function copyFaxNoToBs() {
  
        document.getElementById('C_IND_APPLICATION_BS_FAX_NO1').value =
            document.getElementById('C_IND_APPLICATION_FAX_NO1').value;
    
} 
function copyTelNoToBs() {

    document.getElementById('C_IND_APPLICATION_BS_TELEPHONE_NO1').value =
        document.getElementById('C_IND_APPLICATION_TELEPHONE_NO1').value;

} function copyEmailToBs() {

    document.getElementById('C_IND_APPLICATION_BS_EMAIL').value =
        document.getElementById('C_IND_APPLICATION_EMAIL').value;

} 


domReady(function () {

   

    $(".checkAll").click(function () {
        //th>> nearest table 
//        find 第幾個TH
 //      同一個COL 就TICK 哂佢

        //
        //
        $('input:checkbox').not(this).prop('checked', this.checked);
    });
});