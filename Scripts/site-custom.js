function isIE() {
    ua = navigator.userAgent;
    var is_ie = ua.indexOf("MSIE ") > -1 || ua.indexOf("Trident/") > -1;
    return is_ie;
}

$(window).on("load resize", function () {
    $("img.bg-img").hide();

    var Uurl = $("img.bg-img").attr("src"),
        Fimg = $('.full-background'),
        siteimg = $(".site-background"),
        win = $(window),
        winH = $(window).height(),
        nav = $('nav.navbar'),
        navbarH = $("nav.navbar").outerHeight();

    $(Fimg).css('backgroundImage', 'url(' + Uurl + ')');
    $(siteimg).css('backgroundImage', 'url(' + Uurl + ')');

    $(".splash-logo").css({
        margin: ($(window).height() - $(".splash-logo").outerHeight()) / 2 + "px auto 0",
    });


    $('input[type="text"], input[type="password"]').focus(function () {
        $(this).data('placeholder', $(this).attr('placeholder')).attr('placeholder', '');
    }).blur(function () {
        $(this).attr('placeholder', $(this).data('placeholder'));
    });

    $(".loginfrm").css({
        marginTop: ($(window).height() - $(".loginfrm").outerHeight()) / 2 + "px"
    });

    //if (window.matchMedia("(max-width: 767px)").matches) {
    //    // The viewport is less than 768 pixels wide 
    //    $(".loginfrm").css({ margin: "0 auto" });
    //} else {
    //    // The viewport is at least 768 pixels wide
    //    $(".loginfrm").css({
    //        marginTop: ($(window).height() - $(".loginfrm").outerHeight()) / 2 + "px"
    //        //'marginLeft': ($(window).width() - $('.loginfrm').outerWidth()) * 3 / 4 + "px"
    //    });
    //}

    //if (isIE()) {
    //    //alert('It is InternetExplorer');
    //    $("nav.navbar").css("display", "block"), $("img.logo").css("margin-top", "8px");
    //} else {
    //    //alert('It is NOT InternetExplorer');
    //    $("nav.navbar").css("display", "flex");
    //}

    /*-------------- Start Navbar Auto Change  ---------------*/
    win.scroll(function () {
        if (win.scrollTop() > 45) {
            //nav.fadeOut(10); 
            nav.addClass('header-bg');
        } else {
            //nav.fadeIn(100);
            nav.removeClass('header-bg');
        }
    });

    $(".main-content").css({
        "min-height": winH - (navbarH + 20),
        "margin-top": navbarH
    });

    var drpdown = $("#dropdown"),
        mbtn = $(".menu-button");

    mbtn.click(function () {
        drpdown.toggle();

        if (window.matchMedia("(max-width: 767px)").matches) {
            drpdown.css({
                width: win.width(),
                marginLeft: "-15px"
            });
        } else {
            drpdown.css({
                width: "250px"
            });
        }
    });
    $(this).mouseup(function (e) {
        if (!($(e.target).parent(".menu-button").length > 0)) {
            drpdown.hide();
        }
    });
});
