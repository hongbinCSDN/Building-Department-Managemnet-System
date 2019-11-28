function atcp(p) {
    var _self = this;
    this.focusData = function (idx) {
        //console.log("focusData");
        if (downDiv == null || includeClass(downDiv)) return;
        selectedIndex = idx;
        if (downTableTbody != null && downTableTbody.childNodes.length > idx) {
            attr(downTableTbody.childNodes, "dropClass", "selected");
            attr(downTableTbody.childNodes[idx], "addClass", "selected");
        }
    }
    this.selectData = function () {
        if (downDiv == null || includeClass(downDiv)) return;
        if (selectedIndex != null) {
            if (selectedIndex >= data.length || _self.loading()) {
                _self.stopLoading();
                if (mustMatch == null || mustMatch == true) {
                    _self.setValue("", "");
                } else {
                    _self.setValue(element.value, element.value);
                }
            } else {
                var domObject = { display: element, value: hideElement };
                var beforeValue = { display: element.value, value: hideElement.value };
                var afterValue = { display: data[selectedIndex][displayKey], value: data[selectedIndex][valueKey] };

                _self.setValue(data[selectedIndex][displayKey], data[selectedIndex][valueKey]);
               
                //if (data != null && (oldDisplay != data[selectedIndex][displayKey] || oldValue != data[selectedIndex][valueKey])) {



                    if (onSelect) onSelect(data[selectedIndex], selectedIndex, domObject, beforeValue, afterValue);
                //}
            }
        }
        _self.hide();
    };

    this.getValue = function () {
        return { display: element.value, value: hideElement.value, data: data == null ? null : data[selectedIndex == null ? 0 : selectedIndex] };
    };
    this.setValue = function (displayValue, hideValue) {
        if (displayValue != null) element.value = displayValue;
        if (hideValue != null) hideElement.value = hideValue;
    };
    this.showing = function () {
        atcp.hideAll();
        return downDiv != null && !includeClass(downDiv, "hide");
    }
    this.hide = function () { if (downDiv != null) attr(downDiv, "addClass", "hide"); };



    this.onloading = function () { attr(element, "placeholder", "Loading.."); };
    this.onloaded = function () { attr(element, "placeholder", ""); };



    this.loading = function () { return xhttp != null; }
    this.stopLoading = function () {
        if (xhttp != null) xhttp.abort(); xhttp = null;
        _self.onloaded();
    }
    this.show = function () {
        selectedIndex = null;
        if (_self.loading()) { _self.stopLoading(); }
        var para = { dataSource: dataSource, term: element.value };
        if (passElement != null) {
            for (var i = 0; i < passElement.length; i++) {
                para["atcpParameters." + passElement[i].name] = domId(passElement[i].id).value;
            }
        }
        //atcpParameters
        //passElement
        xhttp = null;
        _self.onloading();
        xhttp = req(dataUrl, para, function (rsp) {
            _self.onloaded();

            data = jData(rsp);
            if (downDiv == null) {
                downDiv = dom_(element.parentNode, "div", [{ "addClass": "latcpDropDown latcpElement" }]);
                attr(downDiv, "onmouseover", { callback: function (sender, p) { mouseOnAtcp = true; } });
                attr(downDiv, "onmouseleave", { callback: function (sender, p) { mouseOnAtcp = false; } });
            }
            attrs(downDiv, [{
                "addClass": "hide"
            }]);

            if (data != null && data.length > 0 ) {
                var position = domPosition(element);
            attrs(downDiv, [{
                "top": (position.top + + element.offsetHeight) + "px"
                , "left": position.left + "px"
                , "width": element.offsetWidth + "px"
                // , "height": "200px"
                , "dropClass": "hide"
            }]);
            if (minWidth != null) downDiv.style.minWidth = minWidth + "px";
            /*
            if (mustMatch == null || mustMatch == false) {
                if (element.value != "" && data != null && data.length > 1) {
                    var firstItem = {};
                    for (var i = 0; i < columns.length; i++) firstItem[columns[i].key] = "";
                    firstItem[valueKey] = element.value;
                    firstItem[displayKey] = element.value;
                    if (data != null) data.unshift(firstItem);
                }
            }
            */




            if (mustMatch == null || mustMatch == true) {
                var blankItem = {};
                for (var i = 0; i < columns.length; i++) blankItem[columns[i].key] = "";
                blankItem[valueKey] = "";
                blankItem[displayKey] = "";
                if (data != null) data.unshift(blankItem);
            }

            if (downTable == null) {
                downTable = dom_(downDiv, "table", [{ "addClass": "latcpElement" }]);
                if (showHeader) {
                    var tr = dom_(dom_(downTable, "thead", [{ "addClass": "latcpElement" }]), "tr", [{ "addClass": "latcpElement" }]);
                    for (var i = 0; i < columns.length; i++) { var th = dom_(tr, "th", [{ "html": columns[i].name }, { "addClass": "latcpElement" }]); }
                }
                downTableTbody = dom_(downTable, "tbody", [{ "addClass": "latcpElement" }]);
            } else {
                downTableTbody.innerHTML = "";
            }
                for (var i = 0; i < data.length; i++) {
                    var tr = dom_(downTableTbody, "tr", [{ "addClass": "latcpElement" }]);
                    attr(tr, "onclick", {
                        parameters: { idx: i }, callback: function (d, p, e) {
                            _self.focusData(p.idx);
                            _self.selectData();
                        }
                    });
                    attr(tr, "onmouseover", { parameters: { selectedIndex: i }, callback: function (sender, p) { _self.focusData(p.selectedIndex) } });
                    for (var j = 0; j < columns.length; j++) {
                        var th = dom_(tr, "td", [{ "html": data[i][columns[j].key] }, { "addClass": "latcpElement latcpTd" }]);
                    }
                }
                if (data.length > 0) { _self.focusData(0); }
            }
        });
    };


    var id = p.id;
    var dataSource = p.dataSource;
    var valueKey = p.valueKey;
    var displayKey = p.displayKey;
    var columns = p.columns;
    var onSelect = p.onSelect;
    var rel = p.rel;
    var passElement = p.passElement;
    var init = p.init;
    var showHeader = p.showHeader == null ? true : p.showHeader;
    var dataUrl = domId("rootPath").value + "/Atcp/Req";

    //    var dataUrl = "http://" + window.location.host + "/Atcp/Req";
    var minWidth = p.minWidth;
    var mustMatch = p.mustMatch;
    var element = domId(id);
    if (element == null || element.readonly != null) return;
    var hideElement = dom_(element.parentNode, "input", { type: "hidden" });
    attr(element, "autocomplete", "off");
    attr(element, "addClass", "atcpDisplay");
    attr(hideElement, "name", element.name);
    attr(element, "name", element.name + "_atcp");
    attr(element, "addClass", "latcpElement");
    attr(hideElement, "addClass", "latcpElement");
    var elementValue = element.value; element.value = "";
    if (init != null && !init) {
        _self.setValue(elementValue, elementValue);
    } else if (elementValue != null && elementValue != "") {


        var para = { dataSource: dataSource, id: elementValue };
        if (passElement != null) {
            for (var i = 0; i < passElement.length; i++) {
                para["atcpParameters." + passElement[i].name] = domId(passElement[i].id).value;
            }
        }
        //atcpParameters
        //passElement
        xhttp = null;
        _self.onloading();
        xhttp = req(dataUrl, para, function (rsp) {
            //console.log(rsp);
            data = jData(rsp);
            if (data != null && data.length > 0) _self.setValue(data[0][displayKey], elementValue);
            else _self.setValue("", elementValue);
        });
    } else {
        _self.setValue("", elementValue);
    }


    var downDiv;
    var downTable;
    var downTableTbody;
    var data;
    var selectedIndex;
    var mouseOnAtcp = false;
    var oldDisplay, oldValue;
    var xhttp = null;
    atcp.Map[id] = _self;

    attr(element, "onblur", {
        parameters: {}, callback: function (d, p, e) {
            debugMsg("onblur");
            _self.stopLoading();
            if (!mouseOnAtcp) { _self.hide(); hideElement.value = element.value; return; }
        }
    });

    attr(element, "onkeyup", {
        parameters: {}, callback: function (d, p, e) {
            debugMsg("onkeyup");
            if (e.keyCode == KE.KEY_UP) {
            } else if (e.keyCode == KE.KEY_DOWN) {
            } else if (e.keyCode == KE.KEY_ENTER || e.keyCode == KE.KEY_TAB) {
            } else { _self.show(); }
        }
    });
    attr(element, "onfocus", {
        parameters: {}, callback: function (d, p, e) {
            debugMsg("onfocus");
            _self.stopLoading();
            oldDisplay = element.value;
            oldValue = hideElement.value;
            _self.show();
        }
    });
    attr(element, "onkeydown", {
        parameters: {}, callback: function (d, p, e) {
            debugMsg("onkeydown");
            _self.stopLoading();
            if (e.keyCode == KE.KEY_UP) {
                if (data != null && selectedIndex > 0) _self.focusData(selectedIndex - 1);
            } else if (e.keyCode == KE.KEY_DOWN) {
                if (data != null && selectedIndex < data.length) _self.focusData(selectedIndex + 1);
            } else if (e.keyCode == KE.KEY_ENTER || e.keyCode == KE.KEY_TAB) {
                _self.focusData(selectedIndex);
                _self.selectData();
                _self.hide();
            } else {
            }
        }
    });
}
var debugging = true;
function debugMsg(v) {
    if (debugging) console.log(v);
}
atcp.Map = {};
atcp.hideAll = function (e) {
    if (e == null || (e.srcElement != null && includeClass(e.srcElement, "atcpElement"))) return;
    for (var k in atcp.Map) atcp.Map[k].hide();
};
atcp.clear = function (key) { if (atcp.Map[key] != null) atcp.Map[key].setValue("", ""); };
atcp.setValue = function (key, displayValue, hideValue) { if (atcp.Map[key] != null) atcp.Map[key].setValue(displayValue, hideValue); };
atcp.getValue = function (key) { if (atcp.Map[key] != null) return atcp.Map[key].getValue(); else return null; };
attr(window, "onwheel", { "callback": function (sender, p, e) { atcp.hideAll(e); } });
attr(window, "onmousewheel", { "callback": function (sender, p, e) { atcp.hideAll(e); } });
attr(window, "onscroll", { "callback": function (sender, p, e) { atcp.hideAll(); } });
attr(window, "onresize", { "callback": function (sender, p, e) { atcp.hideAll(); } });

function atcpcertification_no(p) {
    var certification_no = p.certification_no;

    var certification_noExist = domId(certification_no) != null;
    if (certification_noExist) {
        new atcp({
            id: certification_no, dataSource: "CERTIFICATION_NO", valueKey: "CERTIFICATION_NO", displayKey: "CERTIFICATION_NO", init: false, mustMatch: false
            , columns: [{ "name": "CERTIFICATION_NO", "key": "CERTIFICATION_NO" }, { "name": "CHINESE_NAME", "key": "CHINESE_NAME" }, { "name": "FULLNAME", "key": "FULLNAME" }]
            , onSelect: function (row) {
                //console.log("**");
                //console.log(p);
                //console.log("**");
                attr(domId(p.chineseName), "value", row.CHINESE_NAME == null ? "" : row.CHINESE_NAME);
                attr(domId(p.englishName), "value", row.FULLNAME == null ? "" : row.FULLNAME);
                attr(domId(p.contactNo), "value", row.CONTACT_NO == null ? "" : row.CONTACT_NO);
                attr(domId(p.faxNo), "value", row.FAX_NO == null ? "" : row.FAX_NO);
                //console.log(row.EXPIRY_DATE);
                attr(domId(p.expiryDate), "value", row.EXPIRY_DATE == null ? "" : date2String(new Date(row.EXPIRY_DATE)));

                attr(domId(p.asEngName), "value", row.AS_CHINESE_NAME == null ? "" : row.AS_CHINESE_NAME);
                attr(domId(p.asChnName), "value", row.ASFULLNAME == null ? "" : row.ASFULLNAME);


            }
        });
    }

}

function atcpAddress(p) {
    var streetName = p.streetName;
    var streetNo = p.streetNo;
    var building = p.building;
    var floor = p.floor;
    var unit = p.unit;
    var bcisBlock = p.bcisBlock;
    var bcisDistrict = p.bcisDistrict;
    var blkNo = p.blkNo;
    var region = p.region;
    var blkId = p.blkId;
    var unitId = p.unitId;

    var streetNameExist = domId(streetName) != null;
    var streetNoExist = domId(streetNo) != null;
    var buildingExist = domId(building) != null;
    var floorExist = domId(floor) != null;
    var unitExist = domId(unit) != null;
    var bcisBlockExist = domId(bcisBlock) != null;
    var bcisDistrictExist = domId(bcisDistrict) != null;
    var blkNoExist = domId(blkNo) != null;
    var blkIdExist = domId(blkId) != null;
    var unitIdExist = domId(unitId) != null;
    var regions = domName(region);

    if (streetNameExist) new atcp({
        id: streetName, dataSource: "AddressStreetName", valueKey: "code", displayKey: "code", init: false, mustMatch: false
        , columns: [{ "name": "Street Name", "key": "code" }]
        , onSelect: function (row) {
            if (streetNoExist) atcp.clear(streetNo);
            if (buildingExist) atcp.clear(building);
            if (floorExist) atcp.clear(floor);
            if (unitExist) atcp.clear(unit);
            if (blkIdExist) attr(domId(blkId), "value", "");
            if (unitIdExist) attr(domId(unitId), "value", "");
        }
    });

    if (streetNoExist) new atcp({
        id: streetNo, dataSource: "AddressStreetNumber", valueKey: "code", displayKey: "code", init: false, mustMatch: false
        , passElement: [{ name: "ST_NAME", id: streetName }]
        , columns: [{ "name": "Street No.", "key": "code" }, { "name": "Building", "key": "building" }]
        , onSelect: function (row) {
            if (buildingExist) atcp.setValue(building, row.building, row.building);
            if (floorExist) atcp.clear(floor);
            if (unitExist) atcp.clear(unit);
            if (blkIdExist) attr(domId(blkId), "value", "");
            if (unitIdExist) attr(domId(unitId), "value", "");
            if (blkNoExist) attr(blkNo, "value", row.BLK_NO == null ? "" : row.BLK_NO);
            if (regions.length > 0 && row.REGION_CODE != null) for (var i = 0; i < regions.length; i++) {
                if (regions[i].value == row.REGION_CODE) regions[i].checked = true;
            }
        }
    });
    if (buildingExist) new atcp({
        id: building
        , dataSource: "AddressBuilding", init: false, mustMatch: false
        , passElement: [
            { name: "ST_NAME", id: streetName }
            , { name: "STREETNO", id: streetNo }
        ]
        , valueKey: "code"
        , displayKey: "code"
        , columns: [{ "name": "Street Name", "key": "ST_NAME" }, { "name": "Street No.", "key": "STREETNO" }, { "name": "Building", "key": "code" }]
        , onSelect: function (row) {
            if (row.code != "") {
                atcp.setValue(streetName, row.ST_NAME, row.ST_NAME);
                atcp.setValue(streetNo, row.STREETNO, row.STREETNO);
            }
            if (floorExist) atcp.clear(floor);
            if (unitExist) atcp.clear(unit);
            if (bcisBlockExist) attr(bcisBlock, "value", row.ADR_BLK_ID == null ? "" : row.ADR_BLK_ID);
            if (bcisDistrictExist) domId(bcisDistrict).value = row.DISTRICT_CODE;
            if (blkIdExist) attr(domId(blkId), "value", "");
            if (unitIdExist) attr(domId(unitId), "value", "");
            if (blkNoExist) attr(blkNo, "value", row.BLK_NO == null ? "" : row.BLK_NO);
            if (regions.length > 0 && row.REGION_CODE != null) for (var i = 0; i < regions.length; i++) {
                if (regions[i].value == row.REGION_CODE) regions[i].checked = true;
            }
        }
    });

    if (floorExist) new atcp({
        id: floor
        , dataSource: "AddressFloor", init: false, mustMatch: false
        , passElement: [
            { name: "ST_NAME", id: streetName }
            , { name: "STREETNO", id: streetNo }
            , { name: "BUILDING", id: building }
        ]
        , valueKey: "code"
        , displayKey: "code"
        , columns: [{ "name": "Floor", "key": "code" }]
        , onSelect: function (row) {
            if (unitExist) atcp.clear(unit);
            if (blkIdExist) attr(domId(blkId), "value", "");
            if (unitIdExist) attr(domId(unitId), "value", "");

        }
    });

    if (unitExist) new atcp({
        id: unit
        , dataSource: "AddressUnit", init: false, mustMatch: false
        , passElement: [
            { name: "ST_NAME", id: streetName }
            , { name: "STREETNO", id: streetNo }
            , { name: "BUILDING", id: building }
            , { name: "FLOOR", id: floor }
        ]
        , valueKey: "code"
        , displayKey: "code"
        , columns: [{ "name": "Unit", "key": "code" }]
        , onSelect: function (row) {
            console.log(row);
            if (blkIdExist) attr(domId(blkId), "value", row.ADR_BLK_ID == null ? "" : row.ADR_BLK_ID);
            if (unitIdExist) attr(domId(unitId), "value", row.ADR_UNIT_ID == null ? "" : row.ADR_UNIT_ID);
        }
    });
}

function atcpSMMContact(p) {
    var chi = p.chineseName;
    var eng = p.englishName;
    var idn = p.idNumber;


    new atcp({
        id: chi, dataSource: "SMMPersonContact", valueKey: "NAME_CHINESE", displayKey: "NAME_CHINESE", init: false, mustMatch: false
        , columns: [{ "name": "Chinese Name", "key": "NAME_CHINESE" }, { "name": "English Name", "key": "NAME_ENGLISH" }, { "name": "Number", "key": "ID_NUMBER" }]
        , passElement: [
            { name: "NAME_CHINESE", id: chi }
            , { name: "NAME_ENGLISH", id: eng }
            , { name: "ID_NUMBER", id: idn }
        ]
        , onSelect: function (row) {
            atcp.setValue(eng, row.NAME_ENGLISH, row.NAME_ENGLISH);
            atcp.setValue(idn, row.ID_NUMBER, row.ID_NUMBER);
        }
    });
    new atcp({
        id: eng, dataSource: "SMMPersonContact", valueKey: "NAME_ENGLISH", displayKey: "NAME_ENGLISH", init: false, mustMatch: false
        , columns: [{ "name": "Chinese Name", "key": "NAME_CHINESE" }, { "name": "English Name", "key": "NAME_ENGLISH" }, { "name": "Number", "key": "ID_NUMBER" }]
        , passElement: [
            { name: "NAME_CHINESE", id: chi }
            , { name: "NAME_ENGLISH", id: eng }
            , { name: "ID_NUMBER", id: idn }
        ]
        , onSelect: function (row) {
            atcp.setValue(chi, row.NAME_CHINESE, row.NAME_CHINESE);
            atcp.setValue(idn, row.ID_NUMBER, row.ID_NUMBER);
        }
    });

    new atcp({
        id: idn, dataSource: "SMMPersonContact", valueKey: "ID_NUMBER", displayKey: "ID_NUMBER", init: false, mustMatch: false
        , columns: [{ "name": "Chinese Name", "key": "NAME_CHINESE" }, { "name": "English Name", "key": "NAME_ENGLISH" }, { "name": "Number", "key": "ID_NUMBER" }]
        , passElement: [
            { name: "NAME_CHINESE", id: chi }
            , { name: "NAME_ENGLISH", id: eng }
            , { name: "ID_NUMBER", id: idn }
        ]
        , onSelect: function (row) {
            atcp.setValue(eng, row.NAME_ENGLISH, row.NAME_ENGLISH);
            atcp.setValue(chi, row.NAME_CHINESE, row.NAME_CHINESE);
        }
    });


}

function atcpAddress2(p) {
    var fullAddress = p.fullAddress;
    var streetName = p.streetName;
    var streetNo = p.streetNo;
    var building = p.building;
    var floor = p.floor;
    var unit = p.unit;
    var bcisBlock = p.bcisBlock;
    var bcisDistrict = p.bcisDistrict;
    var blkNo = p.blkNo;
    var region = p.region;
    var blkId = p.blkId;
    var unitId = p.unitId;
    var FILEREF_FOUR = p.FILEREF_FOUR;
    var FILEREF_TWO = p.FILEREF_TWO;
    var FILEREF_FOURExist = domId(FILEREF_FOUR);
    var FILEREF_TWOExist = domId(FILEREF_TWO);

    var fullAddressExist = domId(fullAddress) != null;
    var streetNameExist = domId(streetName) != null;
    var streetNoExist = domId(streetNo) != null;
    var buildingExist = domId(building) != null;
    var floorExist = domId(floor) != null;
    var unitExist = domId(unit) != null;
    var bcisBlockExist = domId(bcisBlock) != null;
    var bcisDistrictExist = domId(bcisDistrict) != null;
    var blkNoExist = domId(blkNo) != null;
    var blkIdExist = domId(blkId) != null;
    var unitIdExist = domId(unitId) != null;
    var regions = domName(region);
    if (blkIdExist && unitIdExist) {
        attr(unitId, "onblur", {
            callback: function (d, p, e) {
                if (d.value == null) return;
                if (d.value == "") return;
                req(domId("rootPath").value + "/Atcp/BlockId", d, function (data) {
                    if (data != null) {
                        if (data != null && data.BLKID != null) {
                            domId(blkId).value = data.BLKID;
                        }
                    }
                });
            }
        });
    }
    /*
    if (fullAddressExist) {
        //attr(fullAddress, "onblur", { callback: function (d, p, e) { domId(blkId).value = ""; } });
        new atcp({
            id: fullAddress, dataSource: "AddressFullAddress", valueKey: "KEYWORD", displayKey: "KEYWORD", init: false, mustMatch: false, minWidth: 500
            , columns: [{ "name": "Address", "key": "KEYWORD" }]
            , passElement: [
                { name: "FULL", id: fullAddress }
                , { name: "STREETNAME", id: streetName }
                , { name: "STREETNO", id: streetNo }
                , { name: "BUILDING", id: building }
                , { name: "BLK_ID", id: blkId }
            ]
            , onSelect: function (row) {
                if (streetNameExist) atcp.setValue(streetName, row.STREETNAME == null ? "" : row.STREETNAME, row.STREETNAME == null ? "" : row.STREETNAME);
                if (streetNoExist) atcp.setValue(streetNo, row.STREETNO == null ? "" : row.STREETNO, row.STREETNO == null ? "" : row.STREETNO);
                if (buildingExist) atcp.setValue(building, row.BUILDINGNAME == null ? "" : row.BUILDINGNAME, row.BUILDINGNAME == null ? "" : row.BUILDINGNAME);
                if (blkIdExist) domId(blkId).value = row.BLK_ID == null ? "" : row.BLK_ID;
            }
        });
    }
    */
    if (streetNameExist) {
        //attr(streetName, "onblur", { callback: function (d, p, e) { domId(blkId).value = ""; } });
        new atcp({
           
            showHeader: false,id: streetName, dataSource: "AddressFullAddress", valueKey: "STREETNAME", displayKey: "STREETNAME", init: false, mustMatch: false, minWidth: 500
            , columns: [
                { "name": "Street Name", "key": "STREETNAME" }
                //, { "name": "Street No.", "key": "STREETNO" }
                //, { "name": "Building Name.", "key": "BUILDING" }
                //, { "name": "Block ID", "key": "BLK_ID" }
            ]
            , passElement: [
                //{ name: "FULL", id: fullAddress }
                { name: "STREETNAME", id: streetName }
                //, { name: "STREETNO", id: streetNo }
                //, { name: "BUILDING", id: building }
                //, { name: "BLK_ID", id: blkId }
            ]
            , onSelect: function (row) {
                //if (fullAddressExist) atcp.setValue(fullAddress, row.KEYWORD == null ? "" : row.KEYWORD, row.KEYWORD == null ? "" : row.KEYWORD);
                if (streetNoExist) atcp.setValue(streetNo, row.STREETNO == null ? "" : row.STREETNO, row.STREETNO == null ? "" : row.STREETNO);
                if (buildingExist) atcp.setValue(building, row.BUILDINGNAME == null ? "" : row.BUILDINGNAME, row.BUILDINGNAME == null ? "" : row.BUILDINGNAME);
                if (blkIdExist) domId(blkId).value = row.BLK_ID == null ? "" : row.BLK_ID;
                /*
                if (FILEREF_FOURExist) domId(FILEREF_FOUR).value = row.BLK_ID == null ? "" : row.BLK_ID;

                var FILEREF_FOURExist = domId(FILEREF_FOUR);
                var FILEREF_TWOExist = domId(FILEREF_TWO);
                FREF_SEQ, FREF_YR
                */

            }
        });
    }


    if (streetNoExist) {
        //attr(streetNo, "onblur", { callback: function (d, p, e) { domId(blkId).value = ""; } });
        new atcp({
            showHeader: false,id: streetNo, dataSource: "AddressFullAddress", valueKey: "STREETNO", displayKey: "STREETNO", init: false, mustMatch: false
            , columns: [
                { "name": "Street No.", "key": "STREETNO" }
                
            ]
            , passElement: [
                //{ name: "FULL", id: fullAddress }
                { name: "STREETNAME", id: streetName }
                , { name: "STREETNO", id: streetNo }
                //, { name: "BUILDING", id: building }
                //, { name: "BLK_ID", id: blkId }
            ]
            , onSelect: function (row) {
                //if (fullAddressExist) atcp.setValue(fullAddress, row.KEYWORD == null ? "" : row.KEYWORD, row.KEYWORD == null ? "" : row.KEYWORD);
                //if (streetNameExist) atcp.setValue(streetName, row.STREETNAME == null ? "" : row.STREETNAME, row.STREETNAME == null ? "" : row.STREETNAME);
                if (buildingExist) atcp.setValue(building, row.BUILDINGNAME == null ? "" : row.BUILDINGNAME, row.BUILDINGNAME == null ? "" : row.BUILDINGNAME);
                if (blkIdExist) domId(blkId).value = row.BLK_ID == null ? "" : row.BLK_ID;
             

                if (FILEREF_FOURExist) domId("FILEREF_FOUR").value = row.FREF_SEQ == null ? "" : row.FREF_SEQ;
                if (FILEREF_TWOExist) domId("FILEREF_TWO").value = row.FREF_YR == null ? "" : row.FREF_YR;
            }
        });
    }


    if (buildingExist) {
        //attr(building, "onblur", { callback: function (d, p, e) { domId(blkId).value = ""; } });
        new atcp({
            id: building, dataSource: "AddressFullAddress", valueKey: "BUILDINGNAME", displayKey: "BUILDINGNAME", init: false, mustMatch: false, minWidth: 500
            , columns: [
                { "name": "Street No.", "key": "STREETNO" },{ "name": "Building", "key": "BUILDINGNAME" }, { "name": "Block ID", "key": "BLK_ID" }]
            , passElement: [
                //{ name: "FULL", id: fullAddress }
                 { name: "STREETNAME", id: streetName }
                , { name: "STREETNO", id: streetNo }
                , { name: "BUILDING", id: building }
                //, { name: "BLK_ID", id: blkId }
            ]
            , onSelect: function (row) {
                if (fullAddressExist) atcp.setValue(fullAddress, row.KEYWORD == null ? "" : row.KEYWORD, row.KEYWORD == null ? "" : row.KEYWORD);
                if (streetNameExist) atcp.setValue(streetName, row.STREETNAME == null ? "" : row.STREETNAME, row.STREETNAME == null ? "" : row.STREETNAME);
                if (streetNoExist) atcp.setValue(streetNo, row.STREETNO == null ? "" : row.STREETNO, row.STREETNO == null ? "" : row.STREETNO);
                if (blkIdExist) domId(blkId).value = row.BLK_ID == null ? "" : row.BLK_ID;

                if (FILEREF_FOURExist) domId("FILEREF_FOUR").value = row.FREF_SEQ == null ? "" : row.FREF_SEQ;
                if (FILEREF_TWOExist) domId("FILEREF_TWO").value = row.FREF_YR == null ? "" : row.FREF_YR;
            }
        });
    }

    if (floorExist) {
        //attr(floor, "onblur", { callback: function (d, p, e) { domId(unitId).value = ""; } });
        new atcp({
            id: floor, dataSource: "AddressFullUnit", valueKey: "FLOOR", displayKey: "FLOOR", init: false, mustMatch: false, minWidth: 500
            , columns: [{ "name": "Floor", "key": "FLOOR" }]
            , passElement: [
                //{ name: "FULL", id: fullAddress }
                // { name: "STREETNAME", id: streetName }
                //, { name: "STREETNO", id: streetNo }
                //, { name: "BUILDING", id: building }
                 { name: "BLK_ID", id: blkId }
                , { name: "FLOOR", id: floor }
               // , { name: "UNIT", id: unit }FILEREF_FOUR
            ]
            , onSelect: function (row) {
                //if (unitExist) atcp.setValue(unit, row.UNIT == null ? "" : row.UNIT, row.UNIT == null ? "" : row.UNIT);
               // if (unitIdExist) domId(unitId).value = row.ADR_UNIT_ID == null ? "" : row.ADR_UNIT_ID;


            }
        });
    }
    if (unitExist) {
        //attr(unit, "onblur", { callback: function (d, p, e) { domId(unitId).value = ""; } });
        new atcp({
            id: unit, dataSource: "AddressFullUnit", valueKey: "UNIT", displayKey: "UNIT", init: false, mustMatch: false, minWidth: 500
            , columns: [{ "name": "Unit", "key": "UNIT" }, { "name": "Unit ID", "key": "ADR_UNIT_ID" }]
            , passElement: [
                //{ name: "FULL", id: fullAddress }
                //{ name: "STREETNAME", id: streetName }
                //, { name: "STREETNO", id: streetNo }
                //, { name: "BUILDING", id: building }
                 { name: "BLK_ID", id: blkId }
                , { name: "FLOOR", id: floor }
                , { name: "UNIT", id: unit }
            ]
            , onSelect: function (row) {
                //if (floorExist) atcp.setValue(floor, row.FLOOR == null ? "" : row.FLOOR, row.FLOOR == null ? "" : row.FLOOR);
                if (unitIdExist) domId(unitId).value = row.ADR_UNIT_ID == null ? "" : row.ADR_UNIT_ID;
            }
        });
    }


    if (FILEREF_FOURExist) {
        //attr(unit, "onblur", { callback: function (d, p, e) { domId(unitId).value = ""; } });
        new atcp({
            id: FILEREF_FOUR, dataSource: "Address42", valueKey: "FILEREF_FOUR", displayKey: "FILEREF_FOUR", init: false, mustMatch: false, minWidth: 500
            , columns: [{ "name": "FILEREF_FOUR", "key": "FILEREF_FOUR" }, { "name": "FILEREF_TWO", "key": "FILEREF_TWO" }]
            , passElement: [
                //{ name: "FULL", id: fullAddress }
                //{ name: "STREETNAME", id: streetName }
                //, { name: "STREETNO", id: streetNo }
                //, { name: "BUILDING", id: building }
                { name: "BLK_ID", id: blkId }
                , { name: "FILEREF_FOUR", id: FILEREF_FOUR }
                , { name: "FILEREF_TWO", id: FILEREF_TWO }
            ]
            , onSelect: function (row) {
                if (FILEREF_TWOExist) atcp.setValue(FILEREF_TWO, row.FILEREF_TWO == null ? "" : row.FILEREF_TWO, row.FILEREF_TWO == null ? "" : row.FILEREF_TWO);
                //if (unitIdExist) domId(unitId).value = row.ADR_UNIT_ID == null ? "" : row.ADR_UNIT_ID;
            }
        });
    }

    if (FILEREF_TWOExist) {
        //attr(unit, "onblur", { callback: function (d, p, e) { domId(unitId).value = ""; } });
        new atcp({
            id: FILEREF_TWO, dataSource: "Address42", valueKey: "FILEREF_TWO", displayKey: "FILEREF_TWO", init: false, mustMatch: false, minWidth: 500
            , columns: [{ "name": "FILEREF_FOUR", "key": "FILEREF_FOUR" }, { "name": "FILEREF_TWO", "key": "FILEREF_TWO" }]
            , passElement: [
                //{ name: "FULL", id: fullAddress }
                //{ name: "STREETNAME", id: streetName }
                //, { name: "STREETNO", id: streetNo }
                //, { name: "BUILDING", id: building }
                { name: "BLK_ID", id: blkId }
                , { name: "FILEREF_FOUR", id: FILEREF_FOUR }
                , { name: "FILEREF_TWO", id: FILEREF_TWO }
            ]
            , onSelect: function (row) {
                if (FILEREF_FOURExist) atcp.setValue(FILEREF_FOUR, row.FILEREF_FOUR == null ? "" : row.FILEREF_FOUR, row.FILEREF_FOUR == null ? "" : row.FILEREF_FOUR);

                //if (floorExist) atcp.setValue(floor, row.FLOOR == null ? "" : row.FLOOR, row.FLOOR == null ? "" : row.FLOOR);
                //if (unitIdExist) domId(unitId).value = row.ADR_UNIT_ID == null ? "" : row.ADR_UNIT_ID;
            }
        });
    }
    //var FILEREF_FOURExist = domId(FILEREF_FOUR);
    //var FILEREF_TWOExist = domId(FILEREF_TWO);
}










