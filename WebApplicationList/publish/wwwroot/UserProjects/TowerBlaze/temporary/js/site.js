var scrollPos=0;
$(window).scroll(function(){
    var st = $(this).scrollTop();
    if (st > scrollPos)
        document.getElementById('headerset').style.height = '0px';
    else
        document.getElementById('headerset').style.height = '104px';

    scrollPos = st;
    
    if(st>185)
        $('header').css("background-color","rgba(255, 255, 255, 0.9)")
    else
        $('header').css("background-color","")
 });

 $(document).ready(function(){
    $("#input-text").on('click', function(){
        $(".subscribe-input-item").focus();
    });

    $(".subscribe-input-item").blur(function(){
        if($(this).val()=="")
            $("#input-text").css("visibility","");
        else
            $("#input-text").css("visibility","hidden");
    });

    $(".subscribe-input-item").on('focus',function(){
        $("#input-text").css("visibility","hidden");
    });

    $("#close").on('click',OpenMenu)

    function OpenMenu(){
        $(".list-menu").css({
            "width":"100%",
            "padding":"50px 50px",
        });
        $("#menu-img").attr("src","Images/close.svg");
        $("#close").off("click");
        $("#close").on('click',CloseMenu);
    }
    function CloseMenu(){
        $(".list-menu").css({
            "width":"",
            "padding":"",
        });
        $("#menu-img").attr("src","Images/hamburgermenu.svg")
        $("#close").off("click");
        $("#close").on('click',OpenMenu);
    }
    


 })