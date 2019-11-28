/* ---------------------- field input mask ----------------------------------------------- */

/* 
-------------------------------------- sample -------------------------

onkeypress="inputMask(this, event, 'numberDotOnly')" onbeforepaste="inputMaskOnBeforePaste(this, event, 'numberDotOnly')" oncontextmenu="inputMaskOnContextMenu(event)" 
nonAlphanumericOnly
floatOnly
numberOnly
alphanumericOnly
alphabeticalOnly
MaskType1

-------------------------------------- sample -------------------------
*/

var regExpression = '';

function specifiedFormat(sObj, e, sMT) {
    if (!isNaN(sMT)) {
        newValue = sObj.value + String.fromCharCode(e.keyCode);
        sMTNumOfDigit = sMT.split('.')[0].length;
        sMTNumOfDecimal = sMT.split('.')[1].length;
        if (newValue.indexOf('.') != -1) {
            sObjNumOfDigit = newValue.split('.')[0].length;
            sObjNumOfDecimal = newValue.split('.')[1].length;
        } else {
            sObjNumOfDigit = newValue.length;
            sObjNumOfDecimal = 0;
        }

        if (!isNaN(newValue) && (sMTNumOfDigit >= sObjNumOfDigit) && (sMTNumOfDecimal >= sObjNumOfDecimal)) {
            regExpression = /[0-9.]/;
        } else {
            regExpression = /[?]/;
        }
    }
}

function regExpn(obj, e, MT) {
    switch (MT) {
        case 'nonAlphanumericOnly':
            regExpression = /\W/;
            //special char only
            break;
        case 'numberOnly':
            regExpression = /[0-9]/;
            break;
        case 'aOnly':
            //regExpression = /[A-Za-z0-9~` \]\!@#$%^&* ]/;	 			 		
            //all keyboard key, a-z, A-F, 0-9, 
            regExpression = /[\x00-\x7F]/;
            //keyboard only;
            break;
        case 'alphanumericOnly':
            //regExpression = /\w/; //a-z,A-Z,0-9,_ char only >>> Equivalent to '[A-Za-z0-9_]'
            regExpression = /[A-Za-z0-9]/;
            break;
        case 'alphabeticalOnly':
            regExpression = /[a-zA-Z]/;
            break;
        case 'MaskType1':
            regExpression = /[^'#"><]/;
            break;
        case 'dateFormat1':
            regExpression = /[0-9 ^'#"><]/;
            break;
        case 'numberDotOnly':
            regExpression = /[0-9.><]/;
            break;
        case 'NCD':			// number, comma, dot
            regExpression = /[0-9.><,]/;
            break;
        case 'ND':			// number, dot				
            regExpression = (obj.value.indexOf('.') != -1) ? /[0-9]/ : /[0-9.]/;
            break;
        case 'N':			// number, dot
            regExpression = /[0-9]/;
            break;
        case 'currencyFormat1':
            regExpression = /[0-9.><]/;
            break;
        default:
            specifiedFormat(obj, e, MT);
            break;
    }
}

/*
function inputMask(obj, e, MT){
	regExpn(obj, e, MT);
	if (navigator.appName == 'Netscape') {
		if(e.charCode > 0){ //a-z, 0-9
			keyDownChar = String.fromCharCode(e.charCode);			
			if(keyDownChar != ''){
				if(regExpression.exec(keyDownChar) == null ){
					e.preventDefault();
				}
			}			
		}
	}else if (navigator.appName == 'Microsoft Internet Explorer'){
		keyDownChar = String.fromCharCode(event.keyCode);
		if(event.keyCode != 13){
			if(regExpression.exec(keyDownChar) == null ){
				window.event.returnValue = false;
			}
		}
	}
}
*/

function getClipboard() {
    if (window.clipboardData) {	// the IE
        return (window.clipboardData.getData('Text'));
    } else if (window.netscape) {
        netscape.security.PrivilegeManager.enablePrivilege('UniversalXPConnect');
        var clip = Components.classes['@mozilla.org/widget/clipboard;1'].createInstance(Components.interfaces.nsIClipboard);
        if (!clip) return;
        var trans = Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces.nsITransferable);
        if (!trans) return;
        trans.addDataFlavor('text/unicode');
        clip.getData(trans, clip.kGlobalClipboard);
        var str = new Object();
        var len = new Object();
        try {
            trans.getTransferData('text/unicode', str, len);
        } catch (error) {
            return;
        }
        if (str) {
            if (Components.interfaces.nsISupportsWString) str = str.value.QueryInterface(Components.interfaces.nsISupportsWString);
            else if (Components.interfaces.nsISupportsString) str = str.value.QueryInterface(Components.interfaces.nsISupportsString);
            else str = null;
        }
        if (str) return (str.data.substring(0, len.value / 2));
    }
    return;
}

function copy_clip(meintext) {
    if (window.clipboardData) { // the IE   
        window.clipboardData.setData("Text", meintext);
    } else if (window.netscape) {
        netscape.security.PrivilegeManager.enablePrivilege('UniversalXPConnect');
        var clip = Components.classes['@mozilla.org/widget/clipboard;1'].createInstance(Components.interfaces.nsIClipboard);
        if (!clip) return;
        var trans = Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces.nsITransferable);
        if (!trans) return;
        trans.addDataFlavor('text/unicode');
        var str = new Object();
        var len = new Object();
        var str = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString);
        var copytext = meintext;
        str.data = copytext;
        trans.setTransferData("text/unicode", str, copytext.length * 2);
        var clipid = Components.interfaces.nsIClipboard;
        if (!clip) return false;
        clip.setData(trans, null, clipid.kGlobalClipboard);
    }
    //alert("Following info was copied to your clipboard:\n\n" + meintext);
    return false;
}

function inputMaskOnFocus(obj, e, MT) {
    obj.onkeypress = function inputMask(e) {
        regExpn(obj, e, MT);
        if (navigator.appName == 'Netscape') {
            if (e.charCode > 0) { //a-z, 0-9			
                keyDownChar = String.fromCharCode(e.charCode);
                if (keyDownChar != '') {
                    if (regExpression.exec(keyDownChar) == null) {
                        e.preventDefault();
                    }
                }
            }
        } else if (navigator.appName == 'Microsoft Internet Explorer') {
            keyDownChar = String.fromCharCode(event.keyCode);
            if (event.keyCode != 13) {
                if (regExpression.exec(keyDownChar) == null) {
                    window.event.returnValue = false;
                }
            }
        }
    }

    obj.onbeforepaste = function inputMaskOnBeforePaste(e) {
		/*
		if(e != undefined && MT != undefined){		
			regExpn(MT);
			clipboardTxt = getClipboard().toString();
			tmpClipboardTxt = '';
			for(i=0; i<clipboardTxt.length; i++){
				if(regExpression.exec(clipboardTxt.substr(i,1)) == null ){
					//if(alert('Invalid character! (' + MT + ')' )){
						//copy_clip('');
					//}				
					//break;
				}else{
					tmpClipboardTxt = tmpClipboardTxt + clipboardTxt.substr(i,1);
				}
			}
			copy_clip(tmpClipboardTxt);
		}
		*/
    }

    obj.oncontextmenu = function inputMaskOnContextMenu(e) {
        if (navigator.appName == 'Netscape') {
            e.preventDefault();
        } else if (navigator.appName == 'Microsoft Internet Explorer') {
            window.event.returnValue = false;
        }
    }
}
