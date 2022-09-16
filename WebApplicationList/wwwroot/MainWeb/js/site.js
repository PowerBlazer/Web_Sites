$(document).ready(function(){

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


