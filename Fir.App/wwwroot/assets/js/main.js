$(document).ready(function () {

    AOS.init();

    $(".nav-left a").click(function (e) {
        e.preventDefault();
        window.open("index.html", "_self")
    })

    $(".home li:nth-child(1) a").click(function (e) {
        e.preventDefault();
        window.open("index.html", "_self")
    })

    $(".producttypes li:nth-child(2) a").click(function (e) {
        e.preventDefault();
        window.open("orangeamarylls.html", "_self")
    })

    $(".classic li:nth-child(2) a").click(function (e) {
        e.preventDefault();
        window.open("tabs.html", "_self")
    })

    $(".classic li:nth-child(1) a").click(function (e) {
        e.preventDefault();
        window.open("accordions.html", "_self")
    })



    $(".infographic li:nth-child(5) a").click(function (e) {
        e.preventDefault();
        window.open("progressbar.html", "_self")
    })





    let stickynav = $(".stickyNav")
    let goUpBtn = $(".goHeadButton")
    $(window).scroll(function () {

        var yPos = $(window).scrollTop()

        if (yPos > 500) {
            $(stickynav).addClass("animate")
            $(goUpBtn).fadeIn()
        }
        else {
            $(stickynav).removeClass("animate")
            $(goUpBtn).fadeOut()
        }

        $(goUpBtn).click(function () {
            $(window).scrollTop(0)
        })


    })


    $(".search-shopping .shopping").hover(function () {

        

            $(".search-shopping .basketList").css("height", "220px")
    },


        function () {
            $(".basketAlert").css("opacity", "0%")

            $(".search-shopping .basketList").css("height", "0%")
        }
    )

    $(".search-shopping .basketList").hover(function () {
        $(this).css("height", "220px")
    }, function () {
        $(this).css("height", "0%")
    })


    $(".menu-icon i").click(function () {
        $(".menu").addClass("active")

    })
    $(".myClose").click(function () {
        $(".menu").removeClass("active")
    })


    $(".classic li:nth-child(1) a").click(function () {
        console.log("aue");
    })

    $(".menu .menu-nav-links li.1 a").click(function (e) {


        e.preventDefault();
        let sublink = $(this).parent().children(".sublinks")

        let icon = $(this).children().last()
        console.log("aue");

        $(icon).toggleClass("customAnimate")
        $(sublink).slideToggle()
    })

    $(".menu .menu-nav-links li .sublinks li").click(function (e) {
        e.stopPropagation()

        e.preventDefault()
        let subsublink = $(this).children(".sub-subLinks")
        $(subsublink).slideToggle()



        // $(".menu .menu-nav-links li.1").children(".sublinks").slideUp()

    })

    $("#menuHome").click(function () {
        window.open("index.html", "_self")
    })

    $("#standartproduct").click(function () {
        window.open("orangeamarylls.html", "_self")
    })

    $("#accordionMob").click(function () {
        window.open("accordions.html", "_self")
    })


    $("#progressMob").click(function () {
        window.open("progressbar.html", "_self")
    })


    $("#tabsMob").click(function () {
        window.open("tabs.html", "_self")
    })





    $(".search-icon i").click(function (e) {
        e.stopPropagation()
        $(".search-icon input").slideToggle()
    })
    $(window).click(function () {

        $(".search-icon input").slideUp()
    })



    // $(".product .productPrice").children().first().click(function(){


    // })

}
)
