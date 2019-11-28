function tabber(p) {
    this.currentIndex = function () { return selectedIdx; };


    this.addTab = function (title, parent) {
        if (tab == null) return null; parent = domId(parent);
        parent = parent != null ? parent : _self.area != null && _self.area.length > 0 ? _self.area[0].parentNode : document.body;
        var newArea = dom_(parent, "div", { addClass: "tabberArea hide" });
        _self.area.push(newArea);
        var newTabItem = dom_(tab, "div");
        attr(newTabItem, "addClass", "w3-bar-item");
        if (title != null && isDom(title)) newTabItem.appendChild(title);
        else attr(newTabItem, "html", title);
        attr(newTabItem, "onclick", {
            parameters: _self.tabItems.length, callback: function (d, p, e) {
                _self.select(p);
            }
        });
        _self.tabItems.push(newTabItem);
        return ({ tab: newTabItem, area: newArea});
    };



    this.select = function (idx) {
        if (selectedIdx == idx || idx < 0  ) return;
        //if (includeClass(_self.tabItems[idx], "selected")) return; || idx == _self.tabItems.length - 1 
        selectedIdx = idx;
        if (_self.area != null && _self.area.length > idx ) {
            attr(_self.area, "addClass", "hide");
            if (_self.area.length > idx) attr(_self.area[idx], "dropClass", "hide");
        }
        attr(_self.tabItems, "dropClass", "selected");
        if (_self.tabItems.length > idx  ) attr(_self.tabItems[idx], "addClass", "selected");
        if (ontab != null) ontab(idx,_self.area[idx]);
    };




    var _self = this;
    if (p == null) return null;
    if (p["tab"] == null) return null;
    if (p["area"] == null) return null;
    var tab = domId(p["tab"]);
    if (tab == null) return null;
    this.area = []; var area0 = domName(p["area"]); if (area0 != null) for (var i = 0; i < area0.length; i++)  _self.area.push(area0[i]);
    if (_self.area != null) attr(_self.area, "addClass", "tabberArea");
    var ontab = p["ontab"];
    this.tabItems = []; var tabItems0 = tab.querySelectorAll("div:not(.header)"); if (tabItems0 != null) for (var i = 0; i < tabItems0.length; i++)  _self.tabItems.push(tabItems0[i]); 
    var headerDiv = tab.querySelectorAll("div.header");
    attr(headerDiv, "addClass", "w3-bar-item");
    var selectedIdx = null;
    //attr(_self.area, "addClass", "w3-animate-opacity");
    attr(tab, "addClass", "tabber");
    attr(tab, "addClass", "w3-bar");
    attr(_self.tabItems, "addClass", "w3-bar-item");
    attr(tab, "dropClass", "hide");
    for (var i = 0; i < _self.tabItems.length; i++) attr(_self.tabItems[i], "onclick", { parameters: i, callback: function (d, p, e) { _self.select(p); } });
   
    if (_self.tabItems.length > 0) _self.select(0);
    return _self;
}