(function ($) {
    $.fn.multicolselect = function (options) {

        var obj = $(this);

        var defaults = {
            buttonImage: "",  // Image to be used for the button  //selectbutton.gif
            valueCol: 2,                   // The column to be used to display value for the textbox
            hideCol: 0,
            uuidCol: 0,
            value: '--Please Select--'
        };

        var options = $.extend(defaults, options);
        console.log(options);
        return this.each(function () {
            obj.hide();

            if ($.support.msie) {
                obj.before("<div><input type='text' id='mltsel' value='' readonly='readonly'/></div>");
                obj.css("background", "#fff");
                obj.css("position", "absolute");
                obj.css("z-index", "2000");

            } else {
                obj.before("<div><input type='text' class='mltsel' value='' readonly='readonly'/></div>");
                obj.css("background", "#fff");
                obj.css("position", "absolute");
                obj.css("z-index", "2000");

            }

            obj.css("border", "1px solid");
            //obj.prev().find("input[type='text']").val(obj.find("tr[title='def']").find("td:eq("+options.valueCol+")").text());
            obj.prev().find("input[type='text']").val(options.value);

            if (obj.find('table').width() < obj.prev().find("input[type='text']").width()) {
                obj.width(obj.prev().find("input[type='text']").width());
            }

            obj.find("tr:has(td):not(:has(th))").hover(function () {
                $(this).css("background-color", "#0055aa");
                $(this).css("color", "#fff");
            }, function () {

                if (($(this).index() + 1) % 2 == 0) {
                    $(this).css("background-color", "");
                    $(this).css("color", "");
                    $(this).removeClass("grid-alt-item");
                    $(this).addClass("grid-alt-item");
                } else {
                    $(this).css("background-color", "");
                    $(this).css("color", "");
                }


                $(this).css("color", "#000");
            });


            obj.find("tr").each(function () {
                $(this).find("td:eq(" + options.hideCol + ")").css("display", "none");
                $(this).find("th:eq(" + options.hideCol + ")").css("display", "none");
            });


            obj.find("td").each(function () {
                $(this).css("padding-right", "10px");
            });
            obj.find("th").each(function () {
                $(this).css("text-align", "left");
                $(this).css("padding-right", "10px");
            });
            obj.find("tr:has(th):not(:has(td))").click(function () {
                obj.prev().find("input[type='text']").val("--Please Select--");
                $("#selectedLetterUuid").val($(this).find("td:eq(" + options.uuidCol + ")").text().trim());
                //alert("12");
                //alert(options.uuidCol);
                alert($(this).find("td:eq(" + options.uuidCol + ")").text());
                //alert("34");
                selectForm();
                obj.hide();
            });
            obj.find('tr:has(td):not(:has(th))').click(function () {
                obj.prev().find("input[type='text']").val($(this).find("td:eq(" + options.valueCol + ")").text().trim());
                $("#selectedLetterUuid").val($(this).find("td:eq(" + options.uuidCol + ")").text().trim());
                //alert($(this).find("td:eq(" + options.uuidCol + ")").text());
                selectForm();
                obj.hide();

            });

            obj.prev().find("input[type='text']").click(function () {
                if (obj.is(":visible")) {
                    obj.hide();
                } else {
                    obj.show();
                }
            });

            obj.prev().find("input[type='image']").click(function () {
                if (obj.is(":visible")) {
                    obj.hide();
                } else {
                    obj.show();
                }
                return false;
            });

        });

    };


})(jQuery);