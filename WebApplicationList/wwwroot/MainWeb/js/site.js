$(document).ready(function(){

    const buttonMenu = $("#openmenu");
    const imgButton = $(".img-close");

    buttonMenu.on('click',OpenMenu);


    function OpenMenu(){
        $(".menu-header").css({
            "width":"100%",
            "padding":"40px",
            "opacity":"1",
        })
        $('body').css("overflow","hidden");
        imgButton.attr("src","/MainWeb/Images/close.svg");
        buttonMenu.off('click');
        buttonMenu.on('click',CloseMenu)
    }
    
    function CloseMenu(){
        $(".menu-header").css({
            "width":"",
            "padding":"",
            "opacity":"0",
        })
        $('body').css("overflow","");
        imgButton.attr("src","/MainWeb/Images/hamburgermenu.svg")
        buttonMenu.off();
        buttonMenu.on('click',OpenMenu);
    }

    const droplist = $(".profile-listdown_inner");

    const buttonDroplist = $(".profile-button");

    buttonDroplist.on('click',OpenDropList);

    function OpenDropList(){
        droplist.css({
            "visibility":"visible",
            "opacity":"1",
        });
        buttonDroplist.off();
        buttonDroplist.on('click',CloseDropList);
    }

    function CloseDropList(){
        droplist.css({
            "visibility":"",
            "opacity":"",
        });
        buttonDroplist.off();
        buttonDroplist.on('click',OpenDropList);
    }

    $(document).mouseup( function(e){
		if ( !buttonDroplist.is(e.target)
		    && !droplist.is(e.target) && buttonDroplist.has(e.target).length === 0 ) {
			CloseDropList();
		}
	});


});
$(document).ready(function(){

    const OpenModalBut = $(".login-user");
    const CloseModalBut = $(".close-button-modal-login");
    const ModalObj = $(".pop-up-authorized");
    const ModalRegister = $(".pop-up-register__inner");
    
    OpenModalBut.on('click',OpenModalLogin);
    CloseModalBut.on('click',CloseModalLogin);

    $("#login-modal").on('click',OpenRegsiterModal);
    $(".close-button-modal-register").on('click',CloseModalLogin);
    $("#register-modal").on('click',CloseRegisterModal);

    

    function OpenModalLogin(){
        ModalObj.css({
            "visibility":"visible",
            "opacity":"1",
        });
        $("body").css("overflow","hidden");
    }

  

    function CloseModalLogin(){
        ModalObj.css({
            "visibility":"hidden",
            "opacity":"0",
        });
        $("body").css("overflow","");
        CloseRegisterModal();
    }

    function OpenRegsiterModal(){
        ModalRegister.css({
            "transform":"",
            "visibility":"visible",
        })
    }

    function CloseRegisterModal(){
        ModalRegister.css({
            "transform":"translate(200%,-50%)",
            "visibility":"hidden",
        })
    }


});

function StartAnimation(){
    $('.pop-up-load-anim_inner').css({
        "visibility":"visible",
        "opacity":"1",
    });
}

function SuccessAnimation(){
    $("#load-animation-authorize").attr("src","/MainWeb/Images/success.gif");
}

function StopAnimation(){
    $("#load-animation-authorize").attr("src","/MainWeb/Images/spinner.gif");
    $('.pop-up-load-anim_inner').css({
        "visibility":"",
        "opacity":"",
    });
}

function StopAnimationReverse(){
    $("#load-animation-authorize").attr("src","/MainWeb/Images/spinner.gif");
    $('.pop-up-load-anim_inner').css({
        "visibility":"",
        "opacity":"",
    });
    location.reload();
}

function CloseNotification(){
   $(".notification-window").css({
        "visibility":"hidden",
        "opacity":"0"
   });
}
function OpenNotification(){
    $(".notification-window").css({
        "visibility":"visible",
        "opacity":"1"
   });
}

function SuccessMessage(message){
    const MessageResult = $(".message-notification");
    $("#notification-image").attr("src","/MainWeb/Images/success.svg");
    MessageResult.html(message);
    OpenNotification();
    setTimeout(CloseNotification,3000);
}

function ErrorMessage(message){
    const MessageResult = $(".message-notification");
    $("#notification-image").attr("src","/MainWeb/Images/error.svg");
    MessageResult.html(message);
    OpenNotification();
    setTimeout(CloseNotification,3000);
}


