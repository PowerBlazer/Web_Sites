
$(document).ready(function(){
    const CloseModal = $(".close-button-modal-login");
    const ModalPanel = $(".pop-up-authorized");
    const RegisterPanel = $(".pop-up-register__inner");
    const LoginPanel = $(".pop-up-login__inner");

    const LoginButton = $(".sign-in");
    const RegisterButton = $(".register");

    const OverLoginButton = $("#register-modal");
    const OverRegisterButton = $("#login-modal");
    
    CloseModal.click(function(){
        CloseLoginPanel();
        CloseRegisterPanel();
        CloseModalAuthorized();
    });

    LoginButton.click(function(){
        CloseRegisterPanel();
        OpeModalAuthorized();
        OpenLoginPanel();
    });

    RegisterButton.click(function(){
        CloseLoginPanel();
        OpeModalAuthorized();
        OpenRegisterPanel();
    });

    OverLoginButton.click(function(){
        CloseRegisterPanel();
        OpenLoginPanel();
    })

    OverRegisterButton.click(function(){
        CloseLoginPanel();
        OpenRegisterPanel();
    })

    function OpeModalAuthorized(){
        ModalPanel.css({
            "visibility":"visible",
            "opacity":"1",
        })
    }

    function CloseModalAuthorized(){
        ModalPanel.css({
            "visibility":"",
            "opacity":"",
        })
    }

    function OpenLoginPanel(){
        LoginPanel.css({
            "visibility":"visible",
            "opacity":"1",
        })
    }

    function CloseLoginPanel(){
        LoginPanel.css({
            "visibility":"",
            "opacity":"",
        }) 
    }

    function OpenRegisterPanel(){
        RegisterPanel.css({
            "visibility":"visible",
            "opacity":"1",
        })
    }

    function CloseRegisterPanel(){
        RegisterPanel.css({
            "visibility":"",
            "opacity":"",
        })
    }

    
});

function InitAuthorizeScript(){
    const MenuUserPanel = $(".main-avatar-menu_inner");
    const MenuUser = $(".avatar-menu");
    const MenuUserAvatarLink = $(".user-avatar");

    MenuUserAvatarLink.click(function(){
        MenuUserPanel.toggleClass("active-menu");
    });

    $(document).mouseup(function(e){
        if ( !MenuUser.is(e.target)
            && MenuUser.has(e.target).length === 0&&
        !MenuUserAvatarLink.is(e.target)&&MenuUserAvatarLink.has(e.target).length===0 ) {
            MenuUserPanel.removeClass("active-menu");
        }
    });
}

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


