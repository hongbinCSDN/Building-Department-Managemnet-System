
function popuper(p) {
    var _self = this;
    this.title = p.title;
    this.message = p.message;
    this.buttons = p.buttons;
    var div;
    this.render = function () {
        div = dom("div", [{ "addClass": "w3-modal" }]);
        var div2 = dom_(div, "div", [{ "addClass": "w3-modal-content w3-card-4" }, { width: "600px" }]);
        var header = dom_(div2, "header", [{ "addClass": "w3-container w3-red" }]);
        var span = dom_(header, "span", [
            { "addClass": "w3-button w3-red w3-xlarge w3-display-topright" }
            , { title: "Close" }
            , { html: "&times;" }
            , { onclick: { parameters: {}, callback: function (d, p, e) { _self.close() } } }
        ]);
        var titleElement = dom_(header, "div");
        titleElement.style.lineHeight = "55px";
        if (this.title != null) {
            if (isString(this.title)) attr(titleElement, "html", this.title);
            else titleElement.appendChild(this.title);
        }
        var messageElement = dom_(div2, "div", [{ "addClass": "w3-container" }]);
        messageElement.style.lineHeight = "55px";
        if (this.message != null) {
            if (isString(this.message)) attr(messageElement, "html", this.message);
            else messageElement.appendChild(this.message);
        }
        var footer = dom_(div2, "div", [{ "addClass": "w3-container w3-light-grey w3-padding" }]);
        footer.style.textAlign = "center";
        if (this.buttons != null) for (var i = 0; i < this.buttons.length; i++) {
            attr(this.buttons[i], "onclick", { callback: _self.close });
            footer.appendChild(_self.buttons[i]);
        }

    };
    this.open = function () { _self.render(); document.body.appendChild(div); div.style.display = "block"; return _self;};
    this.close = function () { if (div != null && div.parentNode != null) div.parentNode.removeChild(div); return _self; };
    return _self;
};
