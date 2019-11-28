  
//Gobel function........
//--"string".trim
if(typeof String.prototype.trim!=='function'){String.prototype.trim=function(){return this.replace(/^\s+|\s+$/g,'');};}

//--"array".indexof
if(!Array.prototype.indexOf) {Array.prototype.indexOf = function(needle){for(var i=0;i<this.length;i++)if(this[i]===needle)return i;return -1;};}

if (typeof String.prototype.trim !== 'function') { String.prototype.trim = function () { return this.replace(/^\s+|\s+$/g, ''); }; }
function upper(v) {return v==null?"":(typeof v==="string")?v.toUpperCase():v;}
function lower(v) {return v==null?"":(typeof v==="string")?v.toLowerCase():v;}

function display(v) { return v == null ? "" : (typeof v === "string") ? v : v; }

function clone(o, attach) {
    if (null == o || "object" != typeof o)
        return null;
    var c;
    if (isHTMLOptionElement(o))
        return dom("option", [{ value: o.value, html: o.innerHTML }]);
    else {
        c = o.constructor();
        for (var k in o)
            if (o.hasOwnProperty(k)) c[k] = o[k];
        for (var k in attach)
            if (attach.hasOwnProperty(k)) c[k] = attach[k];
        return c;
    }
}

if (!document.getElementsByClassName) {
	document.getElementsByClassName = function(search, container) {
		var d = container||document, elements, i, results = [];
		if (d.querySelectorAll) return d.querySelectorAll("." + search); // IE8
		if (d.evaluate) { // IE6, IE7
			elements=d.evaluate(".//*[contains(concat(' ', @class, ' '), ' "+search+" ')]",d,null,0,null);
			while ((i = elements.iterateNext())) results.push(i);
			return results;
		}
		elements = d.getElementsByTagName("*");
		for(i=0;i<elements.length;i++)if(new RegExp("(^|\\s)"+search+"(\\s|$)").test(elements[i].className))results.push(elements[i]);
		return results;
	};
}
/*
function keywordStyle(text, keyword) {
	if (text==null) return "";//By joven
	var find="";for(var i=0,len=keyword==null?0:keyword.length;i<len;i++){find+=keyword[i]=="\\"?"\\\\":"["+keyword[i]+"]";}
	return text.replace(new RegExp(find, 'gi'), function(str) {return '<span class="atcpKeyword">'+str+'</span>'});
}*/





(function(w) {
	
	
	
	w.x1 = {};
	
    w.x1.trigger = function (d, k) {
        var d = domId(d); if (d == null) return;
        if (w["LISTENING_EVENT"] == null) return;
        if (w["LISTENING_EVENT"][k] == null) return;
        for (var i = 0; i < w["LISTENING_EVENT"][k].length; i++) {
            if (w["LISTENING_EVENT"][k][i].d == d) {
                window["LISTENING_EVENT"][k][i].v.callback(d, window["LISTENING_EVENT"][k][i].v.parameters,k);
            }
        }
    };

    w.x1.appendEvent = function (d, k, v) {
		var evtk = (function(o){w[o]=!w[o]?{}:w[o];return o;})("LISTENING_EVENT");
		if(w[evtk][k] == null) w[evtk][k] = [];
        w[evtk][k].push({ "d": d, "v": v });
		if(!d[k])d[k]=(function(e){
			return function(e){
				for(var i=0;i<w[evtk][k].length; i++) {
					if(w[evtk][k][i].d===d) {
						if(false===w[evtk][k][i].v.callback(w[evtk][k][i].d,w[evtk][k][i].v.parameters,e)) {
							return false;
						}
					}
				}
			};
		})();
    }
    w.x1.insertAfter    = function (p, k) { var pp = p.parentNode; if (pp.lastChild == p) pp.appendChild(k); else pp.insertBefore(k, p.nextSibling); };	
	w.x1.trim		= function(v) {return v.replace(/^\s+|\s+$/g,'');};
	w.x1.upper		= function(v) {return v==null?"":(typeof v==="string")?v.toUpperCase():v;};
	w.x1.lower		= function(v) {return v==null?"":(typeof v==="string")?v.toLowerCase():v;};
	w.x1.replaceAll	= function(s,k,v) {return s.replace(new RegExp(k, 'g'), v);};

    w.x1.isArray = function (v) { return Object.prototype.toString.call(v) === "[object Array]"; }
    w.x1.isHTMLCollection = function (v) { return Object.prototype.toString.call(v) === "[object HTMLCollection]"; }
    w.x1.isHTMLOptionElement = function (v) { return Object.prototype.toString.call(v) === "[object HTMLOptionElement]"; }
	w.x1.isString			= function(v) {return typeof v==="string";}
	w.x1.isDom			= function(v) {return v != null && v instanceof Element;}

    w.x1.isNodeList = function (v) { return Object.prototype.toString.call(v) ==="[object NodeList]";}


	w.x1.domId			= function(k, p){return k==null?null:isString(k)?document.getElementById(k):k;};
	w.x1.domTag			= function(k, p){p=domId(p);return (p==null?document:p).getElementsByTagName(k);};
	w.x1.domName			= function(k, p){p=domId(p);return (p==null?document:p).getElementsByName(k);};
	w.x1.domClass		= function(k, p){p=domId(p);return (p==null?document:p).getElementsByClassName(k);};
	w.x1.dom				= function(k,kv){return attrs(document.createElement(k),kv);}
	w.x1._dom			= function(p,k,kv) {p.appendChild(dom(k,kv));return p;}
	w.x1.dom_			= function(p,k,kv){var c=dom(k,kv);p.appendChild(c);return c;}
	w.x1.attrs			= function(d,kv) {d=isString(d)?domId(d):d;for(var k in kv)if(isArray(kv))attrs(d,kv[k]);else if(isArray(kv[k])) for(m in kv[k])attr(d,k,kv[k][m]);else attr(d,k,kv[k]);return d;}
	w.x1.moveDom			= function(d,v,kv){if(v!=null&&d!=null)if(!kv)d.appendChild(v);else if(kv.before)d.insertBefore(v, kv.before);return v;}

    w.x1.getMate = function (d, k) { if (k == null) { k = d.tagName; } return domId(d).parentNode.getElementsByTagName(k); };
    w.x1.domClassSeq = function (d, k) {
        var sameClassDom = document.querySelectorAll("." + k);
        for (var i = 0; i < sameClassDom.length; i++)if (sameClassDom[i] === d) return i;
    };

    w.x1.domClear = function (d) { if (d != null) while (d.childNodes.length > 0) d.removeChild(d.childNodes[0]); };
	w.x1.removeDom		= function(d){
							d=typeof d==="string"?domId(d):d;
							if(isArray(d)) {for(var i=0;i<d.length;i++) removeDom(d[i]); return; }
                             if (isHTMLCollection(d)) { removeDom(Array.prototype.slice.call(d)); return; }
							if(d.parentNode!=null)d.parentNode.removeChild(d);
						};
	w.x1.includeClass	= function(d,cls){var d=domId(d);return d.className == null?false:(" "+d.className+" ").indexOf(trim(" "+cls+" "))>=0;};
	w.x1.dropClass		= function(d,cls){
		var d=domId(d);
		if(d.className != null) 
			d.className=trim(replaceAll(" "+d.className+" ", " "+trim(cls)+" ", " "));
		};
	w.x1.addClass		= function(d,cls){
		var d=domId(d);
		if(!includeClass(d,cls)) d.className = d.className + " " + cls;
		};
	
	w.x1.attr			= function(d,k,v) {
							if(d==null)return;
							if(typeof d==="string"){d=domId(d);if(d==null)return;}
                            else if (isArray(d)) { for (var i = 0; i < d.length; i++) attr(d[i], k, v); return; }
                            else if (isNodeList(d)) { for (var i = 0; i < d.length; i++) attr(d[i], k, v); return; }
                            else if (isHTMLCollection(d)) {
                                var doms = Array.prototype.slice.call(d);
                                for (var i = 0; i < doms.length; i++)
                                    attr(doms[i], k, v); return;
                            }
							if(k=="child")d.appendChild(v);
							else if(k=="addClass")addClass(d,v);
							else if(k=="dropClass")dropClass(d,v);
							else if(k=="changeClass") if(includeClass(d,v))dropClass(d,v);else addClass(d,v);
							else if(k=="class")d.className=v;
							else if(k=="html")if(!v)d.innerHTML="";else if(v&&v.nodeType=== 1)d.appendChild(v);else d.innerHTML=v;
							else if(k=="width")d.style.width=v;
							else if(k=="height")d.style.height=v;
							else if(k=="cursor")d.style.cursor=v;
							else if(k=="float")d.style.float=v;
							else if(k=="checked")d.checked=v;
							else if(k=="selected")d.selected=v;
							else if(k=="appendChild")d.appendChild(v);
							else if(k=="display")d.style.display=v;
							else if(k=="visibility")d.style.visibility=v;
							else if(k=="overflowX")d.style.overflowX=v;
							else if(k=="borderWidth")d.style.borderWidth=v;
							else if(k=="borderStyle")d.style.borderStyle=v;
							else if(k=="borderColor")d.style.borderColor=v;
							else if(k=="backgroundColor")d.style.backgroundColor=v;
							else if(k=="lineHeight")d.style.lineHeight=v;
							else if(k=="textAlign")d.style.textAlign=v;
                            else if (k == "margin") d.style.margin = v;
                            else if (k == "position") d.style.position = v;
                            else if (k == "top") d.style.top = v;
                            else if (k == "left") d.style.left = v;
                            else if (k == "src") d.src = v;

                            else if (k == "value") d.value = v == null ? "" : v;
							else if(k=="disabled"){if(v)d.disabled=true;else d.removeAttribute("disabled");}
							else if(k=="readonly"){
								if(v)d.readOnly=true;else d.removeAttribute("readonly");
								if(upper(d.type)=="TEXT")
									attr(d,"onclick",{"callback": function(d,p,e) {if(d.readOnly)return false}});
								else if(upper(d.type)=="CHECKBOX")
									attr(d,"onclick",{"callback": function(d,p,e) {if(d.readOnly)return false}});
								else if(upper(d.tagName)=="SELECT") {
									var options = domTag("option", d);
									var notSelected = [];
									for(var i = 0 ;i < options.length;i++) if(!options[i].selected) notSelected.push(options[i]);
									removeDom(notSelected);
								}
							}
							else if(k=="onclick")		appendEvent(d,k,v);
							else if(k=="onchange")		appendEvent(d,k,v);
							else if(k=="onblur")		appendEvent(d,k,v);
							else if(k=="onselect")		appendEvent(d,k,v);
							else if(k=="onfocus")		appendEvent(d,k,v);
							else if(k=="onkeydown")		appendEvent(d,k,v);
							else if(k=="onscroll")		appendEvent(d,k,v);
							else if(k=="onmouseover")	appendEvent(d,k,v);
							else if(k=="onmousedown")	appendEvent(d,k,v);
							else if(k=="onmouseup")		appendEvent(d,k,v);
							
							else if(k=="ontouchstart")	appendEvent(d,k,v);
							else if(k=="ontouchmove")	appendEvent(d,k,v);
							else if(k=="ontouchend")	appendEvent(d,k,v);
							
							else if(k=="onmousemove")	appendEvent(d,k,v);
							else if(k=="onkeyup")		appendEvent(d,k,v);
							else if(k=="onload")		appendEvent(d,k,v);
							else if(k=="onwheel")		appendEvent(d,k,v);
							else if(k=="onmouseleave")	appendEvent(d,k,v);
							else if(k=="onmousewheel")	appendEvent(d,k,v);
							else if(k=="onsubmit")		appendEvent(d,k,v);
							else if(k=="onkeypress")	appendEvent(d,k,v);
							else if(k=="onresize")		appendEvent(d,k,v);
								

							else d.setAttribute(k,v);
							return d;
	};
	w.x1.domPosition = function(d) {
		d=domId(d);
		var t = 0, l = 0;
		do {
			t += d.offsetTop  || 0;
			l += d.offsetLeft || 0;
			d = d.offsetParent;
		} while(d);
		return {top: t, left: l};
    };
    w.x1.snapshot = function (d) {
        var obj = {};
        var elements = d.querySelectorAll("input, select, textarea");
        for (var i = 0; i < elements.length; ++i) {
            var element = elements[i];
            var type = element.type;
            var name = element.name;
            var value = element.value;
            if (name) {
                if (obj[name]) {
                    if (!isArray(obj[name])) obj[name] = [];
                    obj[name].push(value);
                }
                else obj[name] = value;
            }
        }
        return obj;
    };

    w.x1.parseForm = function (d) {
        d = domId(d);
        var f = dom("form");
        attr(f, "enctype", "multipart/form-data");
        if (d != null) {
            var tmp;
            tmp = d.tagName == "INPUT" ? [d] : d.querySelectorAll("input");
            console.log("tmp: " + tmp.length);
            for (var i = 0; i < tmp.length; i++)
            // f.appendChild(tmp[i].cloneNode(true));
            {
                //f.appendChild(tmp[i].cloneNode(true));
                if (tmp[i].type == "checkbox") {
                    f.appendChild(tmp[i].cloneNode(true));
                }
                else if (tmp[i].type == "radio") {
                    f.appendChild(tmp[i].cloneNode(true));
                }
                else {
                    var inp = dom_(f, "input", [{ "type": "hidden" }, { "name": tmp[i].name }, { "value": tmp[i].value }]);
                    tmp[i].value = inp.value == null ? "" : inp.value;

                }
                // console.log("name: " + inp.name);
                //console.log("name: " + inp.value);
            }
            

            tmp = d.tagName == "TEXTAREA" ? [d] :d.querySelectorAll("textarea");
            for (var i = 0; i < tmp.length; i++)// f.appendChild(tmp[i].cloneNode(true));
            {
                f.appendChild(tmp[i].cloneNode(true));
                //var inp =  dom_(f, "input", [{ "type": "hidden" }, { "name": tmp[i].name }, { "html": tmp[i].innerHTML }]);

                //tmp[i].innerHTML = inp.innerHTML == null ? "" : inp.innerHTML;
            } 
            tmp = d.tagName == "SELECT" ? [d] :d.querySelectorAll("select");
            for (var i = 0; i < tmp.length; i++)
                if (tmp[i].selectedIndex != null)
                {
                   
                    dom_(f, "input", [{ "type": "hidden" }, { "name": tmp[i].name }, { "value": tmp[i].querySelectorAll("option")[tmp[i].selectedIndex].value }]);

                }
                  }
        return f;
    };
    /*
    w.x1.req = function (url, dataArea, callback) {

        var fd = new FormData();
        var r = [];
        if (isArray(dataArea)) {
            for (var i = 0; i < dataArea.length; i++) {
                fd.append(dataArea[i].name, dataArea[i].value);
            }
        } else {
            d = domId(dataArea);
            if (d != null) {
                var tmp;
                tmp = d.tagName == "INPUT" ? [d] : d.querySelectorAll("input");
                for (var i = 0; i < tmp.length; i++) {
                    if (tmp[i].type == "file" && tmp[i].files != null && tmp[i].files.length > 0)
                        fd.append(tmp[i].name, tmp[i].files[0]);
                    else if (tmp[i].type == "checkbox") {
                        if (tmp[i].checked) fd.append(tmp[i].name, tmp[i].value);
                    } else fd.append(tmp[i].name, tmp[i].value);
                }
                tmp = d.tagName == "TEXTAREA" ? [d] : d.querySelectorAll("textarea");
                for (var i = 0; i < tmp.length; i++) fd.append(tmp[i].name, tmp[i].value);
                tmp = d.tagName == "SELECT" ? [d] : d.querySelectorAll("select");
                for (var i = 0; i < tmp.length; i++) fd.append(tmp[i].name, tmp[i].value);
            }
        }
       

        $.ajax({
            url: url,
            data: fd,
            cache: false,
            processData: false,
            contentType: false,
            type: 'POST',
            success: function (data) { if (callback != null) callback(data); }
        });
    }*/

    w.x1.jData = function (d) { return isString(d) ? JSON.parse(d) : d; };
    w.x1.addFormItem = function (f, k, v) {
        if (k != null && trim(k) != "")
            f.append(k, v); return f;
    };
    w.x1.toForm = function (d, fd) {
        if (fd == null) fd = new FormData();
        if (d == null) return fd;
        if (isArray(d) || isNodeList(d)) {
            for (var i = 0; i < d.length; i++) toForm(d[i], fd);
        } else if (isString(d)) {
            toForm(domId(d), fd);
        } else if (isDom(d)) {
            if (upper(d.tagName) == "SELECT") addFormItem(fd, d.name, d.value);
            else if (upper(d.tagName) == "TEXTAREA") addFormItem(fd, d.name, d.value);
            else if (upper(d.tagName) == "INPUT") {
                if (d.type == "file" && d.files != null && d.files.length > 0) addFormItem(fd, d.name, d.files[0]);
                else if (d.type == "checkbox" || d.type == "radio") { if (d.checked) addFormItem(fd, d.name, d.value); }
                else addFormItem(fd, d.name, d.value);
            }
            else {
                toForm(d.querySelectorAll("select"), fd);
                toForm(d.querySelectorAll("textarea"), fd);
                toForm(d.querySelectorAll("input"), fd);
            }
        } else {
            for (var k in d) { addFormItem(fd, k, d[k]); }
        }
        return fd;
    };
    w.x1.req = function (url, dataArea, callback) {
        var xhttp = new XMLHttpRequest();
        xhttp.onreadystatechange = function () {
            if (this.readyState == 4 && this.status == 200)
                callback(jData(this.responseText));
        };
        xhttp.open("POST", url + ((/\?/).test(url) ? "&" : "?") + (new Date()).getTime(), true);
        xhttp.send(toForm(dataArea));
        return xhttp;
    }












    w.x1.createButton = function (text, icon, attrList) {
        var xlsButton = dom("button");
        xlsButton.className = "btn btn-default";
        xlsButton.type = "button";
        if(icon != null)
        dom_(xlsButton, "li", { "addClass": "fa " + icon });
        if (text != "") dom_(xlsButton, "span", { "html": "&nbsp;" + text });
        attrs(xlsButton, attrList);
        return xlsButton;
    };

    w.x1.clearForm = function (d) {
        d = domId(d);
        if (d == null) return;
      
        tmp = d.querySelectorAll("input");
        for (var i = 0; i < tmp.length; i++) tmp[i].value = "";

        tmp = d.querySelectorAll("textarea");
        for (var i = 0; i < tmp.length; i++) {
            tmp[i].innerHTML = "";
            tmp[i].value = "";
        }
        tmp = d.querySelectorAll("select");
        for (var i = 0; i < tmp.length; i++) {
            var options = tmp[i].querySelectorAll("option");
            if (options.length > 0) {
                for (var j = 0; j < options.length; j++) {
                    options[j].removeAttribute("selected");
                }
                tmp[i].value = options[0].value;
                tmp[i].selectedIndex = 0;
            }
        }
    };

    w.x1.renderForm = function (form, data, modelPrefix) {
        form = domId(form);
        if (form == null) return;






        tmp = d.querySelectorAll("input");
        for (var i = 0; i < tmp.length; i++) {
            if (tmp[i].id != "" && tmp[i].id.indexOf(modelPrefix) == 0) {
              //  console.log(tmp[i].id(modelPrefix));
            }
            //tmp[i].value = "";
        }
        /*
        tmp = d.querySelectorAll("textarea");
        for (var i = 0; i < tmp.length; i++) {
            tmp[i].innerHTML = "";
            tmp[i].value = "";
        }
        tmp = d.querySelectorAll("select");
        for (var i = 0; i < tmp.length; i++) {
            var options = tmp[i].querySelectorAll("option");
            if (options.length > 0) {
                for (var j = 0; j < options.length; j++) {
                    options[j].removeAttribute("selected");
                }
                tmp[i].value = options[0].value;
                tmp[i].selectedIndex = 0;
            }
        }*/
    };


    w.x1.datepickize = function (dom) {
        $(dom)
            .attr("placeholder", "DD/MM/YYYY")
            .flatpickr({
                dateFormat: "d/m/Y"
                , allowInput: true
            });
    };

    w.x1.numberBox = function (dom) {
        $(dom)
            //.attr("type", "number")
            .on("input", function (event) { event.currentTarget.value = event.currentTarget.value.replace(/[^\d]/g, ''); })
            .on("blur", function (event) { event.currentTarget.value = event.currentTarget.value.replace(/[^\d]/g, ''); });
    };

    w.x1.upperCaseBox = function (dom) {
        $(dom)
            //.attr("type", "number")
            .on("input", function (event) { event.currentTarget.value = event.currentTarget.value.toUpperCase(); })
            .on("blur", function (event) { event.currentTarget.value = event.currentTarget.value.toUpperCase(); });
    };

    w.x1.date2String = function (date) {


        if (isNaN(date.getTime())) {
            return "";
        }

        var dd = date.getDate();
        var mm = date.getMonth() + 1;
        var yyyy = date.getFullYear();
        if (dd < 10) dd = '0' + dd;
        if (mm < 10) mm = '0' + mm;
        return dd + '/' + mm + '/' + yyyy;
    };
    w.x1.time2String = function (date) {


        if (isNaN(date.getTime())) {
            return "";
        }

        var dd = date.getDate();
      //  dd.setHours(0, 0, 0, 0);
        var mm = date.getMonth() + 1;
        var yyyy = date.getFullYear();
        var HH = date.getHours();
        var MM = date.getMinutes();
        var ss = date.getSeconds();
        if (HH < 10) HH = '0' + HH;
        if (MM < 10) MM = '0' + MM;
        if (ss < 10) ss = '0' + ss;

        if (dd < 10) dd = '0' + dd;
        if (mm < 10) mm = '0' + mm;
        var result = dd + '/' + mm + '/' + yyyy + " "+ HH + ":" + MM + ":" + ss;
      
        return result ;
    };

    w.x1.toErrorMessage = function (str) {
        //  alert(str);
        str = str.replace(/_/g, " ");

        return str.replace(
            /\w\S*/g,
            function (txt) {
                return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
            }
        );

    };

    w.x1.showErrorMessage = function (errorMessages) {
        var errorFields = domClass("errorField");
        var errorTexts = domClass("errorText");
        attr(errorFields, "dropClass", "errorField");
        removeDom(errorTexts);
        if (errorMessages == null) return;
        console.log("Controller Validation Error!");
        console.log(errorMessages);
        for (var key in errorMessages) {
            if ("ALERT" == key) {
                alert(errorMessages[key]); continue;
            }
            if (!errorMessages.hasOwnProperty(key)) continue;
            var errEle = domName(key + "_atcp");
            if (errEle.length == 0) errEle = domName(key);
            if (errEle.length > 0) {
                errEle = errEle[0];
                insertAfter(errEle, dom("div", [{ "addClass": "errorText" }, { "html": toErrorMessage(errorMessages[key].toString()) }]));
                attr(errEle, "addClass", "errorField");
            }
        }
        trigger(window, "onscroll");
    };
	for(var k in w.x1) {if(typeof w[k] !=='function') w[k] = w.x1[k];};
})(window);













function KeyboardEvent(){}
KeyboardEvent.UP = "UP";
KeyboardEvent.DOWN = "DOWN";
KeyboardEvent.PRESS = "PRESS";
KeyboardEvent.KEY_ALL=0;
KeyboardEvent.KEY_ESC=27;
KeyboardEvent.KEY_ENTER=13;
KeyboardEvent.KEY_F5=116;
KeyboardEvent.KEY_UP=38;
KeyboardEvent.KEY_DOWN=40;

KeyboardEvent.KEY_TAB=9;
KeyboardEvent.KEY_SHIFT=16;
KeyboardEvent.KEY_CTRL=17;
KeyboardEvent.KEY_ALT=18;

KeyboardEvent.KEY_0=48;
KeyboardEvent.KEY_1=49;
KeyboardEvent.KEY_2=50;
KeyboardEvent.KEY_3=51;
KeyboardEvent.KEY_4=52;
KeyboardEvent.KEY_5=53;
KeyboardEvent.KEY_6=54;
KeyboardEvent.KEY_7=55;
KeyboardEvent.KEY_8=56;
KeyboardEvent.KEY_9=57;
KeyboardEvent.KEY_F12=123;

KeyboardEvent.k=new Array();
KeyboardEvent.add=function(d,k,f,p,c){
	if(isArray(d))for(var i=0;i<d.length;i=i+1)this.add(d[i],k,f,p,c);
	var q=false,w=false;
	for(var i=0;i<this.k.length;i=i+1){q=this.k[i].d==d&&this.k[i].f==f;if(q)break;}
	for(var i=0;i<this.k.length;i=i+1){w=this.k[i].d==d&&this.k[i].k==k&&this.k[i].f==f;if(w)break;}
	if(!w)this.k.push({"d":d,"k":k,"f":f,"c":c,"p":p});
    if (!q) {
        var s = this; var r = (function (e) {
            return function (e, d) {
                e = e || window.event; for (var i = 0; i < s.k.length; i = i + 1){
                    var o = s.k[i]; if (o.d == this && (o.k == e.keyCode || o.k == 0))
                    {
                        if (!s.k[i].c(s.k[i].d, s.k[i].k, e.keyCode, s.k[i].p)) {
                            if (e.preventDefault) e.preventDefault(); else e.returnValue = false; return false;
                        }
                    }
                }
            };
        })();
	if(KeyboardEvent.UP==f)d.onkeyup=r;else if(KeyboardEvent.DOWN==f)d.onkeydown=r;else if(KeyboardEvent.PRESS==f)d.onkeypress=r;}
};
window.KE = KeyboardEvent;



var domReadyFs = [];
var domReadyFs2 = [];
function domReady(f) { domReadyFs.push(f); }
function domReadyLast(f) { domReadyFs2.push(f); }
window.onload = function () {
    for (var i = 0; i < domReadyFs.length; i++)domReadyFs[i]();
    for (var j = domReadyFs2.length-1; j >= 0; j--)domReadyFs2[j]();
};


var displayFormLabels;
function autoDisplayFormLabels() {
    /*if (displayFormLabels == null) return;
    for (var i = 0; i < displayFormLabels.length; i++) {
        displayFormLabels[i].style.height = "0px";
        var mates = getMate(displayFormLabels[i]), maxH = 0;
        for (var j = 0; j < mates.length; j++) if (mates[j].offsetTop == displayFormLabels[i].offsetTop) maxH = Math.max(maxH, mates[j].offsetHeight);
        displayFormLabels[i].style.height = maxH + "px";
    }*/
}
domReady(function () {
    var backButton = document.getElementsByClassName("backButton");
    backButton = backButton.length == 0 ? null : backButton[0];
    attr(backButton, "onclick", {
        callback: function () {
            addClass(document.body, "bodyLoading");
            history.go(-1);
            return false;
            //history.back(); 
        }
    });

    displayFormLabels = document.querySelectorAll(".displayForm > .w3-row > div > div:not(.data):not(.w3-center)");
    attr(window, "onscroll", { callback: autoDisplayFormLabels });
    attr(window, "onresize", { callback: autoDisplayFormLabels });

  
});

//function goTo(path, datas, popup) {
//    if (trim(path) == "") return;
//    popup = popup != null && popup;
//    //var resultTables = domClass("resultTable");
//    if (!popup) addClass(document.body, "bodyLoading");
//    var f = dom_(document.body, "form", { "action": path, "method": "get" });
//    if (datas != null) for (var i = 0; i < datas.length; i++)
//        if (datas[i].value != null)
//            dom_(f, "input", { "type": "hidden", "name": datas[i].name, "value": datas[i].value });


//    if (popup) {
//        var win = popupWinds('about:blank', '_form', 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, copyhistory=no, width=860, height=400');
//        f.target = '_form';


//    }

//    f.submit();

//}

function goTo(path, datas, popup) {
    if (trim(path) == "") return;
    popup = popup != null && popup;
    //var resultTables = domClass("resultTable");
    if (!popup) addClass(document.body, "bodyLoading");
    var f = dom_(document.body, "form", { "action": path, "method": "post" });
    if (datas != null) for (var i = 0; i < datas.length; i++)
        if (datas[i].value !=null)
            dom_(f, "input", { "type": "hidden", "name": datas[i].name, "value": datas[i].value });


    if (popup) {
        var win = popupWinds('about:blank', '_form', 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, copyhistory=no, width=1060, height=600');
        f.target = '_form';
   
 
    }

    f.submit();
        
}

function openNewTab(path, datas) {
    if (trim(path) == "") return;
    var f = dom_(document.body, "form", { "action": path, "method": "post" });
    if (datas != null) for (var i = 0; i < datas.length; i++)
        if (datas[i].value != null)
            dom_(f, "input", { "type": "hidden", "name": datas[i].name, "value": datas[i].value });

    f.target = '_blank';
    f.submit();

}

function downloadFile(path, datas) {


    var rootPath = document.getElementById('rootPath').value;

    if (path.indexOf(rootPath) == -1) {
        path = rootPath + path;
    }

    //var resultTables = domClass("resultTable");;
    if (isString(datas))
    {
        console.log("isstring");
        datas = domId(datas);
        datas.action = path;
        datas.submit();
    }
    else if (datas != null) {
        var f = dom_(document.body, "form", { "action": path, "method": "post" });
        for (var key in datas)
        {

        
            dom_(f, "input", { "type": "hidden", "name":key, "value": datas[key] });
        }

        f.submit();
    }

       // var win = popupWinds('about:blank', '_self', 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, copyhistory=no, width=860, height=400');
      //  f.target = '_self';
   // f.submit();
}

function printBarcode(number) {
    window.location = ("MWMS2PrinterAgent:" + number);
    //var win = popupWinds("http://google.com/print?number=" +number, 'POP_REPORT', 'toolbar=no, location=yes, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, copyhistory=no, width=' + 1000+ ', height=' + 700);
    //f.target = 'bd_reporting';
    //f.submit();
}


function popupWind(url) {
    window.open((url), "bd_wind", "width=800,height=600,status=0,toolbar=0,location=0,menubar=0,resizable=1,scrollbars=1");
}

function popupWinds(url, name, param) {
    //alert(url); fixURL(url))
   return window.open(fixURL(url), name, param);
}	


var fixURL = function (url) {
    return url.replace(/&/g, '&amp;');
};

var checkURL = function () {
    var list = [ //106
        '&Aacute',
        '&aacute',
        '&Acirc',
        '&acirc',
        '&acute',
        '&AElig',
        '&aelig',
        '&Agrave',
        '&agrave',
        '&AMP',
        '&amp',
        '&Aring',
        '&aring',
        '&Atilde',
        '&atilde',
        '&Auml',
        '&auml',
        '&brvbar',
        '&Ccedil',
        '&ccedil',
        '&cedil',
        '&cent',
        '&COPY',
        '&copy',
        '&curren',
        '&deg',
        '&divide',
        '&Eacute',
        '&eacute',
        '&Ecirc',
        '&ecirc',
        '&Egrave',
        '&egrave',
        '&ETH',
        '&eth',
        '&Euml',
        '&euml',
        '&frac12',
        '&frac14',
        '&frac34',
        '&GT',
        '&gt',
        '&Iacute',
        '&iacute',
        '&Icirc',
        '&icirc',
        '&iexcl',
        '&Igrave',
        '&igrave',
        '&iquest',
        '&Iuml',
        '&iuml',
        '&laquo',
        '&LT',
        '&lt',
        '&macr',
        '&micro',
        '&middot',
        '&nbsp',
        '&not',
        '&Ntilde',
        '&ntilde',
        '&Oacute',
        '&oacute',
        '&Ocirc',
        '&ocirc',
        '&Ograve',
        '&ograve',
        '&ordf',
        '&ordm',
        '&Oslash',
        '&oslash',
        '&Otilde',
        '&otilde',
        '&Ouml',
        '&ouml',
        '&para',
        '&plusmn',
        '&pound',
        '&QUOT',
        '&quot',
        '&raquo',
        '&REG',
        '&reg',
        '&sect',
        '&shy',
        '&sup1',
        '&sup2',
        '&sup3',
        '&szlig',
        '&THORN',
        '&thorn',
        '&times',
        '&Uacute',
        '&uacute',
        '&Ucirc',
        '&ucirc',
        '&Ugrave',
        '&ugrave',
        '&uml',
        '&Uuml',
        '&uuml',
        '&Yacute',
        '&yacute',
        '&yen',
        '&yuml'
    ];

    return function (url) {
        var l = list;
        var i = l.length;
        var matchIndex;
        var current;
        var nextchar;
        var errors = [];
        for (; i--;) {
            matchIndex = url.indexOf(l[i]);
            current = l[i];
            if (matchIndex > -1) {
                if ((current === '&amp' || current === '&AMP') && url.charAt(matchIndex + 4) === ';') {
                    //如果是 &amp; 或 &AMP; 我们就认为是故意要输出 & ,比如是一个调用fixURL方法修正过的URL.里面的& 会被我们替换为 amp;
                    //所以,我们要跳过它,去检查后面.
                    continue;
                }
                nextchar = url.charAt(matchIndex + current.length);
                if (!/[a-zA-Z0-9]/.test(nextchar)) {
                    //此处我们只要发现任意一个 ,如 &reg后面紧随字符不在 a-z,A-Z,0-9范围内.就算有问题.
                    //这样处理实际和标准的细节以及浏览器实现有细微差异. 但是本着任何浏览器来跑case,都能发现潜在威胁的原则.和实现复杂度的考虑.
                    // 我们姑且粗暴的这样处理了. 似乎还不错.

                    errors.push(current + nextchar);
                }
            }
        }
        if (errors.length) {
            throw Error('contains : \n' + errors.join('\n'));
        }
    };
}();



//wilson
function disableDom(d, p) {
	p=p==null?{}:p;
	var afterAllDisable = p.afterAllDisable;
	d = domId(d);
	if(d == null) return;
	var inputs = domTag("input", d);
	attr(inputs, "readonly", true);
	var selects = domTag("select", d);
	attr(selects, "readonly", true);
	var textareas = domTag("textarea", d);
	attr(textareas, "readonly", true);
}
var cacheUrlParameters = null;

function getUrlParameters() {
    var vars = {};
    var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function(m,key,value) {
        vars[key] = value;
    });
    return vars;
}



// Change style of top container on scroll
function myFunction() {
    if (document.body.scrollTop > 80 || document.documentElement.scrollTop > 80) {
        document.getElementById("myTop").classList.add("w3-card-4", "w3-animate-opacity");
        document.getElementById("myIntro").classList.add("w3-show-inline-block");
    } else {
        document.getElementById("myIntro").classList.remove("w3-show-inline-block");
        document.getElementById("myTop").classList.remove("w3-card-4", "w3-animate-opacity");
    }
}

function alwayBottom(d) {
    d = domId(d); if (d == null) return;
    var span = dom("div", [{ "height": d.offsetHeight, "html": "&nbsp;" }]);
    if (d.nextSibling != null) d.parentNode.insertBefore(span, d.nextSibling);
    else d.parentNode.appendChild(span); var es = eState(); render();
    function render() { if (Math.max(window.scrollY, document.documentElement.scrollTop) + window.innerHeight < es.topHeight) startFloat(); else stopFloat(); }
    function eState() { return { topHeight: d.offsetTop + d.offsetHeight, left: d.offsetLeft, width: d.offsetWidth }; }
    function startFloat() { attr(d, "addClass", "alwayBottom"); d.style.left = es.left; d.style.width = es.width + "px"; }
    function stopFloat() { attr(d, "dropClass", "alwayBottom"); d.style.left = ""; d.style.width = ""; }
    attr(window, "onscroll", { callback: render });
    attr(window, "onresize", { callback: function () { stopFloat(); es = eState(); render(); } });
}






function Trim(str) {
    var tmp_value = str;
    if (tmp_value == null) return "";
    var tmp_index;
    tmp_index_1 = "";
    while (tmp_index_1 != -1) {
        tmp_value = tmp_value.replace(" ", "");
        tmp_index_1 = tmp_value.substr(0, 1).indexOf(" ");
    }
    tmp_index_1 = "";
    while (tmp_index_1 != -1) {
        tmp_index_1 = tmp_value.substr(tmp_value.length - 1, 1).indexOf(" ");
        if (tmp_index_1 != -1) {
            tmp_value = tmp_value.substr(0, tmp_value.length - 1);
        }
    }
    return (tmp_value);
}

//new add
function checkDateFormat(inputDate) {

    var regExp = new RegExp("[0-2][0-9][/][0][1-9][/][1-2][0-9][0-9][0-9]" || "[3][0-1][/][0][1-9][/][1-2][0-9][0-9][0-9]" || "[0-2][0-9][/][1][0-2][/][1-2][0-9][0-9][0-9]");

    if (!regExp.test(inputDate)) {
        return false;
    }
}

function checkBRNo(BRNo, flag) {

    var regExp = new RegExp("[0-9]{8}");

    if (!regExp.test(BRNo)) {
        alert(BRNo);
        alert(regExp.test(BRNo));
        flag = false;

        return false;
    }
    else {
        alert("yes");
        flag = true
        return true;
    }
}
function toAscii(ip_char_inputValue) {
    var symbols = " !\"#$%&'()*+'-./0123456789:;<=>?@";
    var loAZ = "abcdefghijklmnopqrstuvwxyz";

    symbols += loAZ.toUpperCase();
    symbols += "[\\]^_`";
    symbols += loAZ;
    symbols += "{|}~";
    var loc;
    loc = symbols.indexOf(ip_char_inputValue);

    if (loc > -1) {
        Ascii_Decimal = 32 + loc;
        return (32 + loc);
    }
    return (0);  // If not in range 32-126 return ZERO   
}   	
function checkHkid(attName) {
    var mult = new Array(3, 8, 7, 6, 5, 4, 3, 2);
    var x = 11;
    var checkdigit;
    var ch;
    var endValue;
    var z = 0;
    var idlength;
    var i;
    var idx;
    var alpha_count;
    var errorMsg = '';

    var hkid = Trim(attName);
    hkid = hkid.toUpperCase();

    var idre = new RegExp("[A-Z]{1,2}[0-9]{6}[(][0-9ABC][)]", "g");

    if (!idre.test(hkid)) {
        return false;
    }

    hkid = hkid.replace(/[()]/g, '');
    idlength = hkid.toString().length;


    if (idlength != 0) {
        if (idlength == 8) {
            alpha_count = 1;
            idx = 1;
        }
        else if (idlength == 9) {
            alpha_count = 2;
            idx = 0;
        }
        else {
            return false;
        }

        for (i = 0; i < alpha_count; i++) {
            ch = hkid.charAt(i);
            z = z + (toAscii(ch) - toAscii('A') + 1) * mult[idx];
            idx = idx + 1;
        }

        for (i = alpha_count; i < idlength - 1; i++) {
            ch = hkid.charAt(i);
            z = z + ch * mult[idx];

            idx = idx + 1;
        }

        endValue = z % x;
        if (endValue == 0) {
            checkdigit = 0;
        }
        else {
            if (x - endValue < 10) {
                checkdigit = x - endValue;
            }
            else if (x - endValue == 10) {
                checkdigit = "A";
            }
            else if (x - endValue == 11) {
                checkdigit = "B";
            }
            else if (x - endValue == 12) {
                checkdigit = "C";
            }
            else {
                checkdigit = x - endValue + toAscii('A') - 10;
            }
        }
    }
    if (checkdigit == hkid.charAt(idlength - 1)) {
        return true;
    }
    else {

        return false;
    }

    return true;
}   


function checkAller(checkboxAllClass, checkboxClass) {
    all = domClass(checkboxAllClass); if (all.length == 0) return;
    each = domClass(checkboxClass); if (each.length == 0) return;
    for (var i = 0; i < all.length; i++) {
        attr(all[i], "onclick", {
            parameters: { all: all, each: each }
            , callback: function (d, p, e) {
                for (var i = 0; i < p.all.length; i++) if (p.all[i] != d) p.all[i].checked = d.checked;
                for (var i = 0; i < p.each.length; i++) p.each[i].checked = d.checked;
            }
        });
    }
    for (var i = 0; i < each.length; i++) {
        attr(each[i], "onclick", {
            parameters: { all: all, each: each }
            , callback: function (d, p, e) {
                if (d.checked == false) for (var i = 0; i < p.all.length; i++) if (p.all[i] != d) p.all[i].checked = false;
            }
        });
    }
}

//Begin Add by Chester 2019-09-10
/**
 * 根据参数名，获取url后面的参数的值
 * */
function getQueryVariable(variable) {
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        if (pair[0] == variable) { return pair[1]; }
    }
    return null;
}
//End Add by Chester 2019-09-10
domReadyLast(function () {
    var tds = domTag("td");
    for (var i = 0; i < tds.length; i++) {
        if (tds[i].onclick == null) {
            attr(tds[i], "onclick", {
                callback: function (d, p, e) {
                    var rad = d.querySelectorAll('input[type="radio"]');
                    if (rad.length == 1 && !rad[0].disabled) rad[0].checked = true;
                }
            });
        }
    }
});