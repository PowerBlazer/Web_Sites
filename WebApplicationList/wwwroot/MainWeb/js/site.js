
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
            "visibility":"hidden",
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
            "visibility":"hidden",
            "opacity":"",
        })
    }

    $(document).keydown(function(e){
        if(e.keyCode==27){
            CloseModalAuthorized();
            CloseLoginPanel();
            CloseRegisterPanel();
            RemoveFocusElements([LoginButton,RegisterButton]);
        }
    });

    $(document).keydown(function(e){
        if(e.keyCode==13&&CheckOpenBlock(RegisterPanel)){
            RegisterGetData();
        }
        
        if(e.keyCode==13&&CheckOpenBlock(LoginPanel)){
            LoginGetData();
        }
    });

    //---------

    const openMenuButton = $(".menu-open-icon");
    const closeMenuButton= $("#menu-close-icon");

    const MenuPanel = $("header").next();
    const BrightnessPanel = $(".brightness");

    openMenuButton.click(function(){
        MenuPanel.css({
            "width":"260px",
        });
        BrightnessPanel.css({
            "opacity":"1",
            "visibility":"visible",
        });
        $('body').css("overflow","hidden");
        $(".user-add-project").css("display","flex");
    });
    closeMenuButton.click(function(){
        $(".user-add-project").css("display","none");
        MenuPanel.css({
            "width":"0",
        });
        BrightnessPanel.css({
            "opacity":"",
            "visibility":"",
        });
        $('body').css("overflow","");
    });

});

function CheckOpenBlock(element){
    let blockStyle = element.css("visibility");
    if(blockStyle == "visible"){
        return true;
    }
    else{
        return false;
    }
    
}
function RemoveFocusElements(massiv){
    for(let i=0;i<massiv.length;i++){
        massiv[i].blur();
    }
}

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

function InitModalProjectInfo(){
    $(".close-modal").click(CloseModalProject);
    $(document).mouseup(function(e){
        if(!$("#project-content").is(e.target) && $("#project-content").has(e.target).length === 0&&
    !$(".pop-up-authorized").is(e.target)&&
    $(".pop-up-authorized").has(e.target).length===0){
        CloseModalProject();
    }});
}

function OpenModalProject(){

    const modalWindow = $(".modal-project-info");

    let value = $(this).val();

    modalWindow.css({
        "visibility":"visible",
        "opacity":"1",
    });

    $(".load-animation").css({
        "visibility":"visible",
    })

    $('body').css("overflow","hidden")

    GetProject(value);
}

function CloseModalProject(){

    if($(".modal-project-info").css("visibility")==="visible"){
        $(".modal-project-info").css({
            "visibility":"",
            "opacity":"",
        });
        $("#project-content").html("");
        $('body').css("overflow","");
    }
}

function InitModalProject(){
    const modalAuthorized = $(".pop-up-authorized");
    const modalRegister = $(".pop-up-register__inner");
    const avatar = $("#user-avatar");

    let linkAvatar = $(".user-avatar_img").attr("src");

    avatar.attr("src",linkAvatar);
    
    $("#project-register-user").click(function(){
        modalAuthorized.css({
            "visibility":"visible",
            "opacity":"1",
        });
        modalRegister.css({
            "visibility":"visible",
            "opacity":"1",
        });
    });

    $("#send-comment").click(function(){
        let idProject = $(this).val();
        let text = $("#comment").val();

        AddComment(text,idProject);
    });

    $(".subscribe-user").click(function(){
        let userName = $(this).val();
        if($(this).hasClass("disabled-button")){
            $(this).removeClass("disabled-button");
            $(this).html("Подписаться");
            UnSubscribe(userName);
        }
        else{
            $(this).addClass("disabled-button");
            $(this).html("Подписано");
            Subscribe(userName);
        }
    });

    
    InitLikePanel();
}

function InitLikePanel(){

    const likeButton = $("#like-button");
    const dislikeButton = $("#delete-like");

    let projectId = $("#like-button-panel").attr("data-id");


    likeButton.click(function(){
        Like(projectId);
        
        let likes =  $("#count-like").html();
        $("#count-like").html(Number(likes)+1)
    });
    dislikeButton.click(function(){
        Dislike(projectId);

        let likes =  $("#count-like").html();
        $("#count-like").html(Number(likes)-1)
    });


}



//AJAX//
function SearchProjects(data,block){
    $.ajax({
        type:"POST",
        url:"/Main/SearchProjects",
        data:{searchOptionsJson:data},
        success:function(result){
            $(".projects-blocks_inner").html(result);
            $(".open-project-info").click(OpenModalProject);
        },
        error:function(){
            ErrorsMessage("Ошибка на сервере пробуйте позже");
        }
    })
}

function GetProject(projectName){
    $.ajax({
        type:"POST",
        url:"/Main/GetProjectInfo",
        data:{projectName:projectName},
        success:function(result){
            $(".load-animation").css({"visibility":"hidden"});
            $("#project-content").html(result);
            InitModalProject();
        },
        error:function(error){
            if(error.status===404){
                ErrorMessage(error.responseText);
            }

            CloseModalProject();
            ErrorsMessage("Ошибка на сервере , пробуйте позже");
        }
    })
}
function AddComment(text,idProject){
    $.ajax({
        type:"POST",
        url:"/UserProject/AddComment",
        data:{
            text:text,
            projectId:idProject,
        },
        success:function(result){
            if(result){
                SuccessMessage("Успешно");
                UpdateComments(idProject);
            }
            else{
                ErrorMessage("Больше 5 комментариев нельзя");
            }
        },
        error:function(error){
            if(error.status===400){
                ErrorMessage(error.responseText);
                return;
            }

            ErrorMessage("Ошибка на сервере,пробуйте позже");
        }
    })
}
function UpdateComments(projectId){
    $.ajax({
        type:"POST",
        url:"/Main/GetCommentsProject",
        data:{
            count:20,
            projectId:projectId,
        },
        success:function(result){
            $("#comments-panel").html(result);
        },
        error:function(){
            ErrorMessage("Ошибка на сервере,пробуйте позже");
        }
    })
}
function Like(projectId){
    $.ajax({
        type:"POST",
        url:"/UserProject/PutLike",
        data:{projectId:projectId},
        success:function(result){
            if(result){
                let button = "<button type=\"button\" class=\"disabled\" id=\"delete-like\">Убрать лайк</button>"
                $("#like-button-panel").html(button);
                InitLikePanel();
            }
            else{
               ErrorMessage("Уже лайкнуто");
            }
        },
        errror:function(){
            ErrorMessage("Ошибка на сервере пробуйте позже");
        }
    })
}
function Dislike(projectId){
    $.ajax({
        type:"POST",
        url:"/UserProject/DeleteLike",
        data:{projectId:projectId},
        success:function(result){
            if(result){
                let button = "<button type=\"button\"  id=\"like-button\">Оценить</button>"
                $("#like-button-panel").html(button);
                InitLikePanel();
            }
            else{
                ErrorMessage("Ошибка на сервере пробуйте позже");
            }
        },
        errror:function(){
            ErrorMessage("Ошибка на сервере пробуйте позже");
        }
    })
}
function Subscribe(userName){
    $.ajax({
        type:"POST",
        url:"/Profile/SetSubscribe",
        data:{userName:userName},
        success:function(result){
            if(result == null){
                ErrorMessage("На самого себя нельзя");
                return;
            }
            if(result){
                SuccessMessage("Подписано");
            }
            else{
                ErrorMessage("Ошибка на сервере пробуйте позже")
            }
        },
        error:function(){
            ErrorMessage("Ошибка на сервере пробуйте позже");
        }
    })
}
function UnSubscribe(userName){
    $.ajax({
        type:"POST",
        url:"/Profile/DeleteSubscribe",
        data:{userName:userName},
        success:function(result){
            if(result == null){
                ErrorMessage("На самого себя нельзя");
                return;
            }
            if(result){
                SuccessMessage("Отписано");
            }
            else{
                ErrorMessage("Ошибка на сервере пробуйте позже")
            }
        },
        error:function(){
            ErrorMessage("Ошибка на сервере пробуйте позже");
        }
    })
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


