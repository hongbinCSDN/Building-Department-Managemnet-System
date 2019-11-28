function searcher(p) {
    var _self = this;
    var able = true;
    this.setLoading = function () {
        if (!able) return;
        resultTable.style.height = resultTable.offsetHeight + "px";
        resultFooter.style.height = resultFooter.offsetHeight + "px";
        attr(resultFooter, "addClass", "masking");
        if (searchTable != null) addClass(searchTable, "loading");
        addClass(resultPanel, "masking");
    };
    this.clearLoading = function () {
        if (!able) return;
        resultTable.style.height = "";
        resultFooter.style.height = "";
        attr(resultFooter, "dropClass", "masking");
        if (searchTable != null) dropClass(searchTable, "loading");
        dropClass(resultPanel, "masking");
    };
    var buildPostData = function (p) {
        if (!able) return;
        var f = parseForm(searchTable);
        dom_(f, "input", [{ "type": "hidden" }, { "name": "Sort" }, { "value": _self.Sort == null ? null : _self.Sort }]);
        dom_(f, "input", [{ "type": "hidden" }, { "name": "SortType" }, { "value": _self.SortType == null ? 0 : _self.SortType }]);
        dom_(f, "input", [{ "type": "hidden" }, { "name": "Page" }, { "value": _self.Page == null ? null : _self.Page }]);
        dom_(f, "input", [{ "type": "hidden" }, { "name": "Total" }, { "value": _self.Total == null ? 0 : _self.Total }]);
        dom_(f, "input", [{ "type": "hidden" }, { "name": "Rpp" }, { "value": _self.Rpp == null ? 0 : _self.Rpp }]);
        if (Columns != null) for (var i = 0; i < Columns.length; i++) {
            if (Columns[i].displayName != null) dom_(f, "input", [{ "type": "hidden" }, { "name": "Columns[" + i + "][displayName]" }, { "value": Columns[i].displayName }]);
            if (Columns[i].columnName != null) dom_(f, "input", [{ "type": "hidden" }, { "name": "Columns[" + i + "][columnName]" }, { "value": Columns[i].columnName }]);
            if (Columns[i].format != null) dom_(f, "input", [{ "type": "hidden" }, { "name": "Columns[" + i + "][format]" }, { "value": Columns[i].format }]);
        }
        if (p != null) for (var i = 0; i < p.length; i++) dom_(f, "input", [{ "type": "hidden" }, { "name": p[i].name }, { "value": p[i].value }]);
        return f;
    };

    var fillServerData = function (r) {
        if (_self.cacheOption["isEnable"]) {
            sessionStorage.setItem(_self.cacheOption["cacheName"], JSON.stringify(r));
        }
        if (r != null && r.Result != null && r.Result == "FAILURE") {
            showErrorMessage(r.ErrorMessages);
        } else if (isArray(r)) {
            showErrorMessage(null);
            _self.Data = r;
            _self.Sort = null;
            _self.SortType = null;
            _self.Page = null;
            _self.Total = null;
            _self.Rpp = null;
            _self.Const = null;
        } else {
            showErrorMessage(null);
            _self.Data = r.Data;
            _self.Sort = r.Sort;
            _self.SortType = r.SortType;
            _self.Page = r.Page;
            _self.Total = r.Total;
            _self.Rpp = r.Rpp;
            _self.Const = r.Const;
            if (!ClientColumn) {
                if (r.Columns != null) {
                    Columns = [];
                    for (var i = 0; i < r.Columns.length; i++) {
                        Columns.push({
                            displayName: r.Columns[i].displayName
                            , columnName: r.Columns[i].columnName
                            , format: r.Columns[i].format
                        });
                    }
                }
            } else {
                if (r.Columns != null) {
                    for (var i = 0; i < r.Columns.length; i++) {
                        for (var j = 0; j < Columns.length; j++) {
                            if (r.Columns[i].displayName == Columns[j].displayName && r.Columns[i].format != null) {
                                Columns[j].format = r.Columns[i].format;
                                break;
                            }
                        }
                    }
                }
            }
            _self.TotalPage = _self.Total == null || _self.Total == 0 ? 1 : Math.floor((_self.Total - 1) / _self.Rpp) + 1;
        }
    };

    var getServerData = function (form, path, callback) { $.post(path + "?t=" + Date.now(), $(form).serialize()).done(function (d) { callback(d); }); };
    this.add = function (row, callback) {
        if (Columns != null) {
            if (row == null) {
                row = {};
            }
            if (newItemFields != null) {
                for (var i = 0; i < newItemFields.length; i++)
                    if (row[newItemFields[i]] == null) row[newItemFields[i]] = "";

            }
            _self.Data.push(row);
            renderRow(row, _self.Data.length - 1);
        }
        if (callback != null) callback();
    };
    this.remove = function (idx, callback) {
        
        _self.Data = _self.Data.splice(idx, 1);
        removeDom(resultTable.querySelectorAll("tbody > tr")[idx]);
        if (callback != null) callback();
    };


    var renderRow = function (row, idx) {
        //console.log(row);
        //return;
        //if (tbody == null) renderStructure();
        var tr = dom_(tbody, "tr");
        if (_self.rowStyle != null) {
            var o = _self.rowStyle(row, idx);
            if (o != null) for (var k in o) {
                tr.style[k] = o[k];
            }
        }
        if (ColumnClick != null) {
            attr(tr, "onclick", { parameters: { d: row, c: ColumnClick }, callback: function (d, p, e) { p.c(p.d); } });
        }
        for (var i = 0; i < Columns.length; i++) {
            var td = dom_(tr, "td");
            if (Columns[i].columnName != null) {
                if (Columns[i].format == "System.DateTime") {
                    attr(td, "html", row[Columns[i].columnName + _self.Const.DISPLAY_DATE_COLUMN_SUFFIX]);
                } else if (Columns[i].format == "date") {
                    if (_self.Const != null && _self.Const.DISPLAY_DATE_COLUMN_SUFFIX != null && row[Columns[i].columnName + _self.Const.DISPLAY_DATE_COLUMN_SUFFIX] != null) {
                        attr(td, "html", row[Columns[i].columnName + _self.Const.DISPLAY_DATE_COLUMN_SUFFIX]);
                    } else if (row[Columns[i].columnName] != null) {
                        attr(td, "html", date2String(new Date(row[Columns[i].columnName])));
                    }
                } else if (Columns[i].format == "time") {
                    attr(td, "html", time2String(new Date(row[Columns[i].columnName])));
                    
                } else attr(td, "html", row[Columns[i].columnName] == null ? "&nbsp;" : row[Columns[i].columnName] + "");
            } else if (Columns[i].formater != null) {
                var o = Columns[i].formater(row, idx, _self);
                if (o != null) td.appendChild(o);
            }
            if (Columns[i].click != null) {
                attr(td, "onclick", { parameters: { c: Columns[i], d: row }, callback: function (d, p, e) { if (p.d != null) p.c.click(p.d); } });
                attr(td, "addClass", "link");
                attr(td, "cursor", "pointer");
                attr(td, "tabindex", "0");
                //add = function (d, k, f, c) {

                KE.add(td, KE.KEY_ENTER, KeyboardEvent.PRESS, { element: td }, function (d, k, e, p) { trigger(p.element, "onclick"); });

            }
            if (Columns[i].onAfterRender != null)
                Columns[i].onAfterRender(td, row, idx);

            //var inputDates = tr.querySelectorAll(".inputDate");
            //for (var i = 0; i < inputDates.length; i++) datepickize(inputDates[i]);
        }

    };
    var renderStructure = function () {
        domClear(resultTable);
        domClear(resultFooter);
        if (this.Header != null) {
            resultTable.appendChild(this.Header());
        } else {
            var thead_tr = dom_(dom_(resultTable, "thead"), "tr");
            for (var i = 0; i < Columns.length; i++) {

                //
                var th;
                if (Columns[i].displayName != null) {
                    th = dom_(thead_tr, "th", { html: Columns[i].displayName, tabindex: "0" });
                    if (_self.Sortable) {
                        attr(th, "cursor", "pointer");
                        attr(th, "onclick", {
                            parameters: Columns[i].columnName, callback: function (d, p, e) {
                                _self.SortType = (_self.Sort == p && _self.SortType == 0) ? 1 : 0;
                                _self.Sort = p;
                                _self.search();
                            }
                        });
                        KE.add(th, KE.KEY_ENTER, KeyboardEvent.PRESS, { element: th }, function (d, k, e, p) { trigger(p.element, "onclick"); });
                    }} else if (Columns[i].headerFormater != null) {
                    th = dom_(thead_tr, "th", { tabindex: "0" });
                    th.appendChild(Columns[i].headerFormater(_self.Data, i));
                }
                if (Columns[i].css != null) {
                    for (var j in Columns[i].css) {
                        th.style[j] = Columns[i].css[j];
                    }
                }

            }
        }
        tbody = dom_(resultTable, "tbody");
       if (_self.Footer) attr(resultFooter, "dropClass", "hide");
        dom_(resultFooter, "span", { "addClass": "w3-left", "html": _self.Total == null ? "" : ((_self.Total <= 1 ? "No. of &nbsp;Record :" : "No. of &nbsp;Records : ") + "<b>" + _self.Total + "</b>") });
        // dom_(resultFooter, "span", {"addClass":"w3-left", "html": _self.Total == null ? "" : ("<b>"+_self.Total +"</b>"+ (_self.Total <= 1 ? "&nbsp;record" : "&nbsp;records") +" found.") });
        if (_self.Page != 1) dom_(resultFooter, "button", { "type": "button", "html": "First ", "addClass": "pagingButton", "onclick": { callback: function () { _self.goToPage(1); } } });
        if (_self.Page != 1) dom_(resultFooter, "button", { "type": "button", "html": "Prev ", "addClass": "pagingButton", "onclick": { parameters: _self.Page, callback: function (d, p, e) { _self.goToPage(p - 1); } } });
        for (var i = Math.max(1, _self.Page - 3); i <= Math.min(_self.TotalPage, _self.Page + 3); i++) {
            var numButton = dom_(resultFooter, "button", { "type": "button", "html": i, "addClass": "pagingButton", "onclick": { parameters: i, callback: function (d, p, e) { _self.goToPage(p); } } });
            if (_self.Page == i) attr(numButton, "addClass", "selected");
        }
        if (_self.Page != _self.TotalPage) dom_(resultFooter, "button", { "type": "button", "html": "Next ", "addClass": "pagingButton", "onclick": { parameters: _self.Page, callback: function (d, p, e) { _self.goToPage(p + 1); } } });
        if (_self.Page != _self.TotalPage) dom_(resultFooter, "button", { "type": "button", "html": "Last ", "addClass": "pagingButton", "onclick": { parameters: _self.TotalPage, callback: function (d, p, e) { _self.goToPage(p); } } });
        var backTopButton = createButton("Back to Top", "fa-arrow-up", [{ "onclick": { parameters: {}, callback: function () { document.body.scrollTop = 0; document.documentElement.scrollTop = 0; } } }]);
        backTopButton.style.float = "right";
       // resultFooter.appendChild(backTopButton);
        if (exportPath != null) {
            var exportButton = createButton("Export", "fa fa-print", [{
                "onclick": {
                    parameters: {}, callback: _self.export
                }
            }]
            );
            exportButton.style.float = "right";
            resultFooter.appendChild(exportButton);
        }
        if (_self.resultFootButtons != null) {
            for (var i = 0; i < _self.resultFootButtons.length; i++) {
                _self.resultFootButtons[i].style.float = "right";
                resultFooter.appendChild(_self.resultFootButtons[i]);
            }
        }
    }
    this.export = function () {
        var exportingDom = resultTable;
        addClass(exportingDom, "exporting");
        getServerData(buildPostData(), exportPath, function (d) {
            if (d != null && d.key != null) {
                if (window.dlFrame == null) dlFrame = dom_(document.body, "iframe", { "display": "none", "name": "dlFrame" });
                if (window.dlForm == null) dlForm = dom_(document.body, "form");
                dlForm.method = "post";
                dlForm.target = "dlFrame";
                dlForm.action = exportPath;
                dom_(dlForm, "input", { "type": "hidden", "name": "key", "value": d.key });
                dlForm.submit();
                attr(dlForm, "html", "");
                dropClass(exportingDom, "exporting");
            }
        });
    };
    this.render = function (callback) {
        renderStructure();
        if (able && this.Data != null)
            for (var j = 0; j < this.Data.length; j++) {
                //console.log(j);
                renderRow(this.Data[j], j);
            }
        if (callback != null) callback();
    };
    this.goToPage = function (page) {
        if (!able) return; _self.Page = page; _self.search();
    };
    this.search = function (p, callback) {


        if (!!window.chrome && (!!window.chrome.webstore || !!window.chrome.runtime)) {
            if (window.location.href.substring(window.location.href.lastIndexOf("/") + 1).toLowerCase() == "index")
            history.pushState(null, null, "#SEARCH");
        }
        else {
            if (window.location.href.substring(window.location.href.lastIndexOf("/") + 1).toLowerCase() == "index")
            dom_(document.body, "a", [{ "href": "#SEARCH" }]).click();
        }


      
        if (!able) return;
        if (_self.onBeforeSearch != null) {

            var r = _self.onBeforeSearch();
            if (r == false) return false;
        }
        _self.setLoading();
        getServerData(buildPostData(p), searchPath, function (r) {
            // console.log(r);
            fillServerData(r);
            _self.render(function () { _self.clearLoading(); if (_self.onAfterSearch != null) _self.onAfterSearch(r); if (callback != null) callback(); });
        });
        return _self;
    };

    this.loadCache = function (r) {

        fillServerData(r);
        _self.render(function () { _self.clearLoading(); if (_self.onAfterSearch != null) _self.onAfterSearch(r); });

        return _self;
    };

    this.rowStyle = p.rowStyle;
    searcher.all.push(this);
    this.onBeforeSearch = p.onBeforeSearch;
    this.onAfterSearch = p.onAfterSearch;
    this.Sortable = p.Sortable == null ? true : p.Sortable;;
    this.Sort = p.Sort;
    this.SortType = p.SortType;
    this.Page;
    this.Total;
    this.Rpp;
    this.TotalPage = null;
    this.resultFootButtons = p.resultFootButtons;
    this.Footer = p.Footer == null ? true : p.Footer;
    this.Header = p.Header;
    var Columns = p.Columns;
    var ColumnClick = p.ColumnClick;
    var ClientColumn = Columns != null && Columns.length > 0;
    var searchPath = p.searchPath;
    var newItemFields = p.newItemFields;
    this.Data = p.Data;
    var exportPath = p.exportPath;
    if (this.resultFootButtons != null && this.resultFootButtons.length > 0) this.Footer = true;
    if (this.exportPath != null) this.Footer = true;
    var searchTable = domId(p.searchTable); //if (searchTable == null) return;
    var tbody;
    this.Const;
    if (searchTable != null) {
        attr(searchTable, "addClass", "w3-border");
        attr(searchTable, "addClass", "displayForm");
        var searchButton = searchTable.getElementsByClassName("searchButton");
        searchButton = searchButton.length == 0 ? null : searchButton[0];
        var resetButton = searchTable.getElementsByClassName("resetButton");
        resetButton = resetButton.length == 0 ? null : resetButton[0];
        var searchFooter = searchTable.getElementsByClassName("footer");
        searchFooter = searchFooter.length == 0 ? null : searchFooter[0];
        if (searchFooter != null) alwayBottom(searchFooter);
    }

    


    var resultPanel = domId(p.resultPanel); if (resultPanel == null) { able = false; return; }
    resultPanel.style.overflowX = "auto";
    var resultTable = dom_(resultPanel, "table");
    var resultFooter = dom("div");
    attr(resultFooter, "addClass", "hide");
    insertAfter(resultPanel, resultFooter);
    attr(resultTable, "addClass", "w3-table-all");
    attr(resultTable, "addClass", "w3-hoverable");
    attr(resultTable, "addClass", "resultTable");
    attr(resultFooter, "addClass", "resultFooter");
    attr(searchButton, "onclick", { callback: function () {   _self.Page = 1; _self.search(false); } });
    attr(resetButton, "onclick", {
        callback: function () {
            document.body.scrollTop = 0;
            document.documentElement.scrollTop = 0;
            _self.setLoading();
           // window.reload();
            location.href = location.pathname;
           
        }
    });

    //cache
    this.cacheOption = { isEnable: p.cacheOption ? p.cacheOption["isEnable"] : false, cacheName: p.cacheOption ? p.cacheOption["cacheName"] : "" };
    if (this.cacheOption["isEnable"]) {
        //Get cache
        var serverDataStr = sessionStorage.getItem(this.cacheOption["cacheName"]);
        var serverData = serverDataStr ? JSON.parse(serverDataStr) : null;
        var loadCache = this.loadCache;
        if (serverData != null || serverData != undefined) {
            domReady(function () {
                loadCache(serverData);
            });
        }

    }




    return _self;
}

searcher.all = [];

//Add by Chester 2019/06/17
//function complexSearcher(p) {
//    var _self = this;
//    var able = true;

//    this.setLoading = function () {
//        if (!able) return;
//        resultTable.style.height = resultTable.offsetHeight + "px";
//        resultFooter.style.height = resultFooter.offsetHeight + "px";
//        attr(resultFooter, "addClass", "masking");
//        if (searchTable != null) addClass(searchTable, "loading");
//        addClass(resultPanel, "masking");
//    };
//    this.clearLoading = function () {
//        if (!able) return;
//        resultTable.style.height = "";
//        resultFooter.style.height = "";
//        attr(resultFooter, "dropClass", "masking");
//        if (searchTable != null) dropClass(searchTable, "loading");
//        dropClass(resultPanel, "masking");
//    };
//    var buildPostData = function (p) {
//        if (!able) return;
//        var f = parseForm(searchTable);
//        dom_(f, "input", [{ "type": "hidden" }, { "name": "Sort" }, { "value": _self.Sort == null ? null : _self.Sort }]);
//        dom_(f, "input", [{ "type": "hidden" }, { "name": "SortType" }, { "value": _self.SortType == null ? 0 : _self.SortType }]);
//        dom_(f, "input", [{ "type": "hidden" }, { "name": "Page" }, { "value": _self.Page == null ? null : _self.Page }]);
//        dom_(f, "input", [{ "type": "hidden" }, { "name": "Total" }, { "value": _self.Total == null ? 0 : _self.Total }]);
//        dom_(f, "input", [{ "type": "hidden" }, { "name": "Rpp" }, { "value": _self.Rpp == null ? 0 : _self.Rpp }]);
//        if (Columns != null) for (var i = 0; i < Columns.length; i++) {
//            if (Columns[i].displayName != null) dom_(f, "input", [{ "type": "hidden" }, { "name": "Columns[" + i + "][displayName]" }, { "value": Columns[i].displayName }]);
//            if (Columns[i].columnName != null) dom_(f, "input", [{ "type": "hidden" }, { "name": "Columns[" + i + "][columnName]" }, { "value": Columns[i].columnName }]);
//            if (Columns[i].format != null) dom_(f, "input", [{ "type": "hidden" }, { "name": "Columns[" + i + "][format]" }, { "value": Columns[i].format }]);
//        }
//        if (p != null) for (var i = 0; i < p.length; i++) dom_(f, "input", [{ "type": "hidden" }, { "name": p[i].name }, { "value": p[i].value }]);
//        return f;
//    };
//    var isNullOrUndefied = function (obj) {
//        return obj == "" || obj == null || obj == undefined;
//    }
//    var isDateFormat = function (obj) {
//        return isNaN(obj) && !isNaN(Date.parse(obj));
//    }
//    var getServerData = function (form, path, callback) { $.post(path + "?t=" + Date.now(), $(form).serialize()).done(function (d) { callback(d); }); };
//    this.render = function (callback) {
//        resultTable.innerHTML = "";
//        resultFooter.innerHTML = "";
//        if (able && _self.Data != null) {
//            if (this.Header != null) {
//                resultTable.appendChild(this.Header());
//            } else {
//                var thead_tr = dom_(dom_(resultTable, "thead"), "tr");
//                for (var i = 0; i < Columns.length; i++) {

//                    //
//                    if (Columns[i].displayName != null) {
//                        var th = dom_(thead_tr, "th", { html: Columns[i].displayName, tabindex: "0" });
//                        attr(th, "cursor", "pointer");
//                        attr(th, "onclick", {
//                            parameters: Columns[i].columnName, callback: function (d, p, e) {
//                                _self.SortType = (_self.Sort == p && _self.SortType == 0) ? 1 : 0;
//                                _self.Sort = p;
//                                _self.search();
//                            }
//                        });
//                    } else if (Columns[i].headerFormater != null) {
//                        var th = dom_(thead_tr, "th", { tabindex: "0" });
//                        th.appendChild(Columns[i].headerFormater(_self.Data, i));
//                    }
//                }
//            }

//            //console.log(_self.Data);

//            var renderTemplate = "";
//            for (var row = 0; row < _self.Data.length; row++) {
//                var tmp = p.renderTemplate;
//                var renderSubTemplate = "";
//                var subColumns = [];
//                for (var col = 0; col < Columns.length; col++) {
//                    var arr = Columns[col].columName.split('.');
//                    if (arr.length > 1) {
//                        if ($.isArray(_self.Data[row][arr[0]]) && _self.Data[row][arr[0]].length > 0) {
//                            subColumns.push(Columns[col].columName);
//                            //for (var subRow = 0; subRow < _self.Data[row][arr[0]].length; subRow++) {
//                            //    var subTemp = p.renderSubTemplate;
//                            //    subTemp = subTemp.replace("{" + arr[1] + "}", _self.Data[row][arr[0]][subRow][arr[1]]);
//                            //    renderSubTemplate += subTemp;
//                            //}
//                        } else if ($.isPlainObject(_self.Data[row][arr[0]])) {
//                            tmp = tmp.replace(RegExp("{" + arr[1] + "}", "g"), isNullOrUndefied(_self.Data[row][arr[0]][arr[1]]) ? "" : (isDateFormat(_self.Data[row][arr[0]][arr[1]]) ? date2String(new Date(_self.Data[row][arr[0]][arr[1]])) : _self.Data[row][arr[0]][arr[1]]));
//                            $(tmp)
//                        } else {
//                            tmp = tmp.replace(RegExp("{" + arr[1] + "}", "g"), "");
//                        }
//                    } else {
//                        tmp = tmp.replace(RegExp("{" + arr[0] + "}", "g"), isNullOrUndefied(_self.Data[row][arr[0]]) ? "" : _self.Data[row][arr[0]]);
//                    }
//                    //if (Columns[col].click != null) {
//                    //    //attr(td, "onclick", { parameters: { c: Columns[col], d: _self.Data[j] }, callback: function (d, p, e) { if (p.d != null) p.c.click(p.d); } });
//                    //    //attr(td, "addClass", "link");
//                    //    //attr(td, "cursor", "pointer");
//                    //    console.log("click: " + Columns[col] + ":" + _self.Data[row]);
//                    //}
//                }

//                if (subColumns.length > 0) {
//                    var subField = subColumns[0].split('.');
//                    for (var subRow = 0; subRow < _self.Data[row][subField[0]].length; subRow++) {
//                        var subTemp = p.renderSubTemplate;
//                        for (var subCol = 0; subCol < subColumns.length; subCol++) {
//                            subField = subColumns[subCol].split('.');

//                            subTemp = subTemp.replace(RegExp("{" + subField[1] + "}", "g"), isNullOrUndefied(_self.Data[row][subField[0]][subRow][subField[1]]) ? "" : (isDateFormat(_self.Data[row][subField[0]][subRow][subField[1]]) ? date2String(new Date(_self.Data[row][subField[0]][subRow][subField[1]])) : _self.Data[row][subField[0]][subRow][subField[1]]));
//                        }
//                        renderSubTemplate += subTemp;
//                    }
//                }

//                renderTemplate += tmp.replace("{renderSubTemplate}", renderSubTemplate);
//            }

//            var tbody = dom_(resultTable, "tbody");
//            tbody.innerHTML = renderTemplate;


//            if (_self.Footer) attr(resultFooter, "dropClass", "hide");
//            if (_self.Page != 1) dom_(resultFooter, "button", { "type": "button", "html": "First ", "addClass": "pagingButton", "onclick": { callback: function () { _self.goToPage(1); } } });
//            if (_self.Page != 1) dom_(resultFooter, "button", { "type": "button", "html": "Prev ", "addClass": "pagingButton", "onclick": { parameters: _self.Page, callback: function (d, p, e) { _self.goToPage(p - 1); } } });
//            for (var i = Math.max(1, _self.Page - 3); i <= Math.min(_self.TotalPage, _self.Page + 3); i++) {
//                var numButton = dom_(resultFooter, "button", { "type": "button", "html": i, "addClass": "pagingButton", "onclick": { parameters: i, callback: function (d, p, e) { _self.goToPage(p); } } });
//                if (_self.Page == i) attr(numButton, "addClass", "selected");
//            }
//            if (_self.Page != _self.TotalPage) dom_(resultFooter, "button", { "type": "button", "html": "Next ", "addClass": "pagingButton", "onclick": { parameters: _self.Page, callback: function (d, p, e) { _self.goToPage(p + 1); } } });
//            if (_self.Page != _self.TotalPage) dom_(resultFooter, "button", { "type": "button", "html": "Last ", "addClass": "pagingButton", "onclick": { parameters: _self.TotalPage, callback: function (d, p, e) { _self.goToPage(p); } } });
//            var backTopButton = createButton("Back to Top", "fa-arrow-up", [{ "onclick": { parameters: {}, callback: function () { document.body.scrollTop = 0; document.documentElement.scrollTop = 0; } } }]);
//            backTopButton.style.float = "right";
//            resultFooter.appendChild(backTopButton);

//            if (exportPath != null) {
//                var exportButton = createButton("Export", "fa-save", [{
//                    "onclick": {
//                        parameters: {}, callback: function () {
//                            var exportingDom = resultTable;
//                            addClass(exportingDom, "exporting");
//                            getServerData(buildPostData(), exportPath, function (d) {
//                                if (d != null && d.key != null) {
//                                    if (window.dlFrame == null) dlFrame = dom_(document.body, "iframe", { "display": "none", "name": "dlFrame" });
//                                    if (window.dlForm == null) dlForm = dom_(document.body, "form");
//                                    dlForm.method = "post";
//                                    dlForm.target = "dlFrame";
//                                    dlForm.action = exportPath;
//                                    dom_(dlForm, "input", { "type": "hidden", "name": "key", "value": d.key });
//                                    dlForm.submit();
//                                    attr(dlForm, "html", "");
//                                    dropClass(exportingDom, "exporting");
//                                }
//                            });
//                        }
//                    }
//                }]
//                );
//                exportButton.style.float = "right";
//                resultFooter.appendChild(exportButton);
//            }
//        }
//        if (_self.resultFootButtons != null) {
//            for (var i = 0; i < _self.resultFootButtons.length; i++) {
//                _self.resultFootButtons[i].style.float = "right";
//                resultFooter.appendChild(_self.resultFootButtons[i]);
//            }
//        }



//        var inputDates = resultPanel.querySelectorAll(".inputDate");
//        for (var i = 0; i < inputDates.length; i++) {
//            datepickize(inputDates[i]);
//        }

//        if (callback != null) callback();
//    };
//    this.goToPage = function (page) {
//        if (!able) return; _self.Page = page; _self.search();
//    }
//    this.search = function (p, callback) {
//        if (!able) return;
//        _self.setLoading();
//        if (_self.onBeforeSearch != null) _self.onBeforeSearch();
//        getServerData(buildPostData(p), searchPath, function (r) {
//            if (r != null && r.Result != null && r.Result == "FAILURE") {
//                showErrorMessage(r.ErrorMessages);
//            } else if (isArray(r) && r.length <= 0) {
//                showErrorMessage(r.ErrorMessages);
//                _self.Data = [];
//                //_self.Sort = null;
//                //_self.SortType = 0;
//                //_self.Page = 1;
//                _self.Total = 0;
//                _self.Rpp = 10;
//                _self.Const = null;
//                _self.TotalPage = _self.Total == null || _self.Total == 0 ? 1 : (_self.Total - (_self.Total % _self.Rpp)) / _self.Rpp;
//            } else if (isArray(r) && r.length > 0) {
//                showErrorMessage(null);
//                _self.Data = r;
//                _self.Sort = _self.Data[0].Sort;
//                _self.SortType = _self.Data[0].SortType;
//                _self.Page = _self.Data[0].Page;
//                //_self.Total = null;
//                _self.Total = _self.Data[0].Total;
//                _self.Rpp = _self.Data[0].Rpp;
//                _self.Const = null;
//                _self.TotalPage = _self.Total == null || _self.Total == 0 ? 1 : (_self.Total - (_self.Total % _self.Rpp)) / _self.Rpp;
//            } else {
//                showErrorMessage(null);
//                _self.Data = r.Data;
//                _self.Sort = r.Sort;
//                _self.SortType = r.SortType;
//                _self.Page = r.Page;
//                _self.Total = r.Total;
//                _self.Rpp = r.Rpp;
//                _self.Const = r.Const;
//                if (!ClientColumn) {
//                    if (r.Columns != null) {
//                        Columns = [];
//                        for (var i = 0; i < r.Columns.length; i++) {
//                            Columns.push({
//                                displayName: r.Columns[i].displayName
//                                , columnName: r.Columns[i].columnName
//                                , format: r.Columns[i].format
//                            });
//                        }
//                    }
//                } else {
//                    if (r.Columns != null) {
//                        for (var i = 0; i < r.Columns.length; i++) {
//                            for (var j = 0; j < Columns.length; j++) {
//                                if (r.Columns[i].displayName == Columns[j].displayName && r.Columns[i].format != null) {
//                                    Columns[j].format = r.Columns[i].format;
//                                    break;
//                                }
//                            }
//                        }
//                    }
//                }
//                _self.TotalPage = _self.Total == null || _self.Total == 0 ? 1 : (_self.Total - (_self.Total % _self.Rpp)) / _self.Rpp;
//            }
//            _self.render(function () { _self.clearLoading(); if (_self.onAfterSearch != null) _self.onAfterSearch(); if (callback != null) callback(); });
//        });
//        return _self;
//    };

//    this.onBeforeSearch = p.onBeforeSearch;
//    this.onAfterSearch = p.onAfterSearch;
//    this.Sort = p.Sort;
//    this.SortType = p.SortType;
//    this.Page;
//    this.Total;
//    this.Rpp;
//    this.TotalPage = null;
//    this.resultFootButtons = p.resultFootButtons;
//    this.Footer = p.Footer == null ? true : p.Footer;
//    this.Header = p.Header;
//    var Columns = p.Columns;
//    var ClientColumn = Columns != null && Columns.length > 0;
//    var searchPath = p.searchPath;
//    this.Data = p.Data;
//    var exportPath = p.exportPath;
//    if (this.resultFootButtons != null && this.resultFootButtons.length > 0) this.Footer = true;
//    if (this.exportPath != null) this.Footer = true;
//    var searchTable = domId(p.searchTable); //if (searchTable == null) return;
//    this.Const;
//    if (searchTable != null) {
//        attr(searchTable, "addClass", "w3-border");
//        attr(searchTable, "addClass", "displayForm");
//        var searchButton = searchTable.getElementsByClassName("searchButton");
//        searchButton = searchButton.length == 0 ? null : searchButton[0];
//        var resetButton = searchTable.getElementsByClassName("resetButton");
//        resetButton = resetButton.length == 0 ? null : resetButton[0];
//        var searchFooter = searchTable.getElementsByClassName("footer");
//        searchFooter = searchFooter.length == 0 ? null : searchFooter[0];
//        if (searchFooter != null) alwayBottom(searchFooter);
//    }




//    var resultPanel = domId(p.resultPanel); if (resultPanel == null) { able = false; return; }
//    resultPanel.style.overflowX = "auto";
//    var resultTable = dom_(resultPanel, "table");
//    var resultFooter = dom("div");
//    attr(resultFooter, "addClass", "hide");
//    insertAfter(resultPanel, resultFooter);
//    //attr(resultTable, "addClass", "w3-table-all");
//    attr(resultTable, "addClass", "w3-hoverable");
//    attr(resultTable, "addClass", "resultTable");
//    attr(resultFooter, "addClass", "resultFooter");
//    attr(searchButton, "onclick", { callback: function () { _self.Page = 1; _self.search(false); } });
//    attr(resetButton, "onclick", {
//        callback: function () {
//            document.body.scrollTop = 0;
//            document.documentElement.scrollTop = 0;
//            _self.setLoading();
//            window.location = window.location;
//            //window.reload(true);
//        }
//    });

//}

function formatDate(fmt, date) {
    if (date === null || date === undefined || date === "") {
        return ""
    }
    var o = {
        "M+": date.getMonth() + 1,                   //�·�
        "d+": date.getDate(),                        //��
        "h+": date.getHours(),                       //Сʱ
        "m+": date.getMinutes(),                     //��
        "s+": date.getSeconds(),                     //��
        "q+": Math.floor((date.getMonth() + 3) / 3), //����
        "S": date.getMilliseconds()                  //����
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
};

function generateHTMLByTemp(templateElem, data, resultElem) {
    var tempSource = $(templateElem).html();
    Handlebars.registerHelper('formatDate', function (date, options) {
        if (date === null || date === undefined || date === "") {
            return ""
        }
        date = new Date(date);
        return formatDate("dd/MM/yyyy", date);
    });
    var template = Handlebars.compile(tempSource);
    //$(resultElem).html(template(data));
    if (resultElem != null && resultElem != undefined && resultElem != "") {
        $(resultElem).html(template(data));
        return;
    } return template(data);
};

function complexResultSearcher(p) {
    var _self = this;
    var able = true;
    this.setLoading = function () {
        if (!able) return;
        resultTable.style.height = resultTable.offsetHeight + "px";
        resultFooter.style.height = resultFooter.offsetHeight + "px";
        attr(resultFooter, "addClass", "masking");
        if (searchTable != null) addClass(searchTable, "loading");
        addClass(resultPanel, "masking");
    };
    this.clearLoading = function () {
        if (!able) return;
        resultTable.style.height = "";
        resultFooter.style.height = "";
        attr(resultFooter, "dropClass", "masking");
        if (searchTable != null) dropClass(searchTable, "loading");
        dropClass(resultPanel, "masking");
    };
    var buildPostData = function (p) {
        console.log("p:" + p);
        if (!able) return;
        var f = parseForm(searchTable);
        dom_(f, "input", [{ "type": "hidden" }, { "name": "Sort" }, { "value": _self.Sort == null ? null : _self.Sort }]);
        dom_(f, "input", [{ "type": "hidden" }, { "name": "SortType" }, { "value": _self.SortType == null ? 0 : _self.SortType }]);
        dom_(f, "input", [{ "type": "hidden" }, { "name": "Page" }, { "value": _self.Page == null ? null : _self.Page }]);
        dom_(f, "input", [{ "type": "hidden" }, { "name": "Total" }, { "value": _self.Total == null ? 0 : _self.Total }]);
        dom_(f, "input", [{ "type": "hidden" }, { "name": "Rpp" }, { "value": _self.Rpp == null ? 0 : _self.Rpp }]);
        if (Columns != null) for (var i = 0; i < Columns.length; i++) {
            if (Columns[i].displayName != null) dom_(f, "input", [{ "type": "hidden" }, { "name": "Columns[" + i + "][displayName]" }, { "value": Columns[i].displayName }]);
            if (Columns[i].columnName != null) dom_(f, "input", [{ "type": "hidden" }, { "name": "Columns[" + i + "][columnName]" }, { "value": Columns[i].columnName }]);
            if (Columns[i].format != null) dom_(f, "input", [{ "type": "hidden" }, { "name": "Columns[" + i + "][format]" }, { "value": Columns[i].format }]);
        }
        if (p != null) for (var i = 0; i < p.length; i++) dom_(f, "input", [{ "type": "hidden" }, { "name": p[i].name }, { "value": p[i].value }]);
        return f;
    };
    var getServerData = function (form, path, callback) { $.post(path + "?t=" + Date.now(), $(form).serialize()).done(function (d) { callback(d); }); };

    this.render = function (callback) {
        resultTable.innerHTML = "";
        resultFooter.innerHTML = "";
        if (able && _self.Data != null) {
            //Header
            if (this.Header != null) {
                resultTable.appendChild(this.Header());
            } else {
                var thead_tr = dom_(dom_(resultTable, "thead"), "tr");
                for (var i = 0; i < Columns.length; i++) {

                    //
                    if (Columns[i].displayName != null) {
                        var th = dom_(thead_tr, "th", { html: Columns[i].displayName, tabindex: "0" });
                        attr(th, "cursor", "pointer");
                        attr(th, "onclick", {
                            parameters: Columns[i].columnName, callback: function (d, p, e) {
                                _self.SortType = (_self.Sort == p && _self.SortType == 0) ? 1 : 0;
                                _self.Sort = p;
                                _self.search();
                            }
                        });
                        KE.add(th, KE.KEY_ENTER, KeyboardEvent.PRESS, { element: th }, function (d, k, e, p) { trigger(p.element, "onclick"); });
                    } else if (Columns[i].headerFormater != null) {
                        var th = dom_(thead_tr, "th", { tabindex: "0" });
                        th.appendChild(Columns[i].headerFormater(_self.Data, i));
                    }


                }
            }

            var tbody = dom_(resultTable, "tbody");
            //tbody.innerHTML = ($("#resultTableTemp").html());
            tbody.innerHTML = generateHTMLByTemp("#resultTableTemp", _self, null);

            // Footer
            if (_self.Footer) attr(resultFooter, "dropClass", "hide");
            dom_(resultFooter, "span", { "addClass": "w3-left", "html": _self.Total == null ? "" : ((_self.Total <= 1 ? "No. of &nbsp;Record :" : "No. of &nbsp;Records : ") + "<b>" + _self.Total + "</b>") });
            // dom_(resultFooter, "span", {"addClass":"w3-left", "html": _self.Total == null ? "" : ("<b>"+_self.Total +"</b>"+ (_self.Total <= 1 ? "&nbsp;record" : "&nbsp;records") +" found.") });
            if (_self.Page != 1) dom_(resultFooter, "button", { "type": "button", "html": "First ", "addClass": "pagingButton", "onclick": { callback: function () { _self.goToPage(1); } } });
            if (_self.Page != 1) dom_(resultFooter, "button", { "type": "button", "html": "Prev ", "addClass": "pagingButton", "onclick": { parameters: _self.Page, callback: function (d, p, e) { _self.goToPage(p - 1); } } });
            for (var i = Math.max(1, _self.Page - 3); i <= Math.min(_self.TotalPage, _self.Page + 3); i++) {
                var numButton = dom_(resultFooter, "button", { "type": "button", "html": i, "addClass": "pagingButton", "onclick": { parameters: i, callback: function (d, p, e) { _self.goToPage(p); } } });
                if (_self.Page == i) attr(numButton, "addClass", "selected");
            }
            if (_self.Page != _self.TotalPage) dom_(resultFooter, "button", { "type": "button", "html": "Next ", "addClass": "pagingButton", "onclick": { parameters: _self.Page, callback: function (d, p, e) { _self.goToPage(p + 1); } } });
            if (_self.Page != _self.TotalPage) dom_(resultFooter, "button", { "type": "button", "html": "Last ", "addClass": "pagingButton", "onclick": { parameters: _self.TotalPage, callback: function (d, p, e) { _self.goToPage(p); } } });

            var backTopButton = createButton("Back to Top", "fa-arrow-up", [{ "onclick": { parameters: {}, callback: function () { document.body.scrollTop = 0; document.documentElement.scrollTop = 0; } } }]);
            backTopButton.style.float = "right";
            resultFooter.appendChild(backTopButton);

            if (exportPath != null) {
                var exportButton = createButton("Export", "fa fa-print", [{
                    "onclick": {
                        parameters: {}, callback: function () {
                            var exportingDom = resultTable;
                            addClass(exportingDom, "exporting");
                            getServerData(buildPostData(), exportPath, function (d) {
                                if (d != null && d.key != null) {
                                    if (window.dlFrame == null) dlFrame = dom_(document.body, "iframe", { "display": "none", "name": "dlFrame" });
                                    if (window.dlForm == null) dlForm = dom_(document.body, "form");
                                    dlForm.method = "post";
                                    dlForm.target = "dlFrame";
                                    dlForm.action = exportPath;
                                    dom_(dlForm, "input", { "type": "hidden", "name": "key", "value": d.key });
                                    dlForm.submit();
                                    attr(dlForm, "html", "");
                                    dropClass(exportingDom, "exporting");
                                }
                            });
                        }
                    }
                }]
                );
                exportButton.style.float = "right";
                resultFooter.appendChild(exportButton);
            }
        }

        if (_self.resultFootButtons != null) {
            for (var i = 0; i < _self.resultFootButtons.length; i++) {
                _self.resultFootButtons[i].style.float = "right";
                resultFooter.appendChild(_self.resultFootButtons[i]);
            }
        }



        var inputDates = resultPanel.querySelectorAll(".inputDate");
        for (var i = 0; i < inputDates.length; i++) {
            datepickize(inputDates[i]);
        }

        if (callback != null) callback();
    };

    this.goToPage = function (page) {
        if (!able) return; _self.Page = page; _self.search();
    }
    this.search = function (p, callback) {
        if (!able) return;
        _self.setLoading();
        if (_self.onBeforeSearch != null) _self.onBeforeSearch();
        getServerData(buildPostData(p), searchPath, function (r) {
            if (r != null && r.Result != null && r.Result == "FAILURE") {
                showErrorMessage(r.ErrorMessages);
            } else if (isArray(r)) {
                showErrorMessage(null);
                _self.Data = r;
                _self.Sort = null;
                _self.SortType = null;
                _self.Page = null;
                _self.Total = null;
                _self.Rpp = null;
                _self.Const = null;
            } else {
                showErrorMessage(null);
                _self.Data = r.Data;
                _self.Sort = r.Sort;
                _self.SortType = r.SortType;
                _self.Page = r.Page;
                _self.Total = r.Total;
                _self.Rpp = r.Rpp;
                _self.Const = r.Const;
                if (!ClientColumn) {
                    if (r.Columns != null) {
                        Columns = [];
                        for (var i = 0; i < r.Columns.length; i++) {
                            Columns.push({
                                displayName: r.Columns[i].displayName
                                , columnName: r.Columns[i].columnName
                                , format: r.Columns[i].format
                            });
                        }
                    }
                } else {
                    if (r.Columns != null) {
                        for (var i = 0; i < r.Columns.length; i++) {
                            for (var j = 0; j < Columns.length; j++) {
                                if (r.Columns[i].displayName == Columns[j].displayName && r.Columns[i].format != null) {
                                    Columns[j].format = r.Columns[i].format;
                                    break;
                                }
                            }
                        }
                    }
                }
                _self.TotalPage = _self.Total == null || _self.Total == 0 ? 1 : (_self.Total - (_self.Total % _self.Rpp)) / _self.Rpp + 1;
            }
            _self.render(function () { _self.clearLoading(); if (_self.onAfterSearch != null) _self.onAfterSearch(r); if (callback != null) callback(); });
        });
        return _self;
    };

    this.onBeforeSearch = p.onBeforeSearch;
    this.onAfterSearch = p.onAfterSearch;
    this.Sort = p.Sort;
    this.SortType = p.SortType;
    this.Page;
    this.Total;
    this.Rpp;
    this.TotalPage = null;
    this.resultFootButtons = p.resultFootButtons;
    this.Footer = p.Footer == null ? true : p.Footer;
    this.Header = p.Header;
    var Columns = p.Columns;
    var ColumnClick = p.ColumnClick;
    var ClientColumn = Columns != null && Columns.length > 0;
    var searchPath = p.searchPath;
    this.Data = p.Data;
    var exportPath = p.exportPath;
    if (this.resultFootButtons != null && this.resultFootButtons.length > 0) this.Footer = true;
    if (this.exportPath != null) this.Footer = true;
    var searchTable = domId(p.searchTable); //if (searchTable == null) return;
    this.Const;
    if (searchTable != null) {
        attr(searchTable, "addClass", "w3-border");
        attr(searchTable, "addClass", "displayForm");
        var searchButton = searchTable.getElementsByClassName("searchButton");
        searchButton = searchButton.length == 0 ? null : searchButton[0];
        var resetButton = searchTable.getElementsByClassName("resetButton");
        resetButton = resetButton.length == 0 ? null : resetButton[0];
        var searchFooter = searchTable.getElementsByClassName("footer");
        searchFooter = searchFooter.length == 0 ? null : searchFooter[0];
        if (searchFooter != null) alwayBottom(searchFooter);
    }

    var resultPanel = domId(p.resultPanel); if (resultPanel == null) { able = false; return; }
    resultPanel.style.overflowX = "auto";
    var resultTable = dom_(resultPanel, "table");
    var resultFooter = dom("div");
    attr(resultFooter, "addClass", "hide");
    insertAfter(resultPanel, resultFooter);
    if (p.tableClass !== null && p.tableClass !== undefined && p.tableClass !== "") {
        for (let i = 0; i < p.tableClass.length; i++) {
            attr(resultTable, "addClass", p.tableClass[i]);
        }
    }
    //attr(resultTable, "addClass", "w3-table-all");
    attr(resultTable, "addClass", "w3-hoverable");
    attr(resultTable, "addClass", "resultTable");
    attr(resultFooter, "addClass", "resultFooter");
    attr(searchButton, "onclick", { callback: function () { _self.Page = 1; _self.search(false); } });
    attr(resetButton, "onclick", {
        callback: function () {
            document.body.scrollTop = 0;
            document.documentElement.scrollTop = 0;
            _self.setLoading();
            window.reload();
        }
    });


    return _self;
}
domReadyLast(function () {
    if (window.searcher.all !=null)
    if ("#SEARCH" == window.location.hash && window.searcher.all.length > 0) window.searcher.all[0].search();
});
