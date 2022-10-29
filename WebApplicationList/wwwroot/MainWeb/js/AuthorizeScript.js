$(document).ready(function(){
    $(".post-data-register").on('click',RegisterGetData);
    $(".post-data-login").on('click',LoginGetData);
    $("#logout").on('click',function(){
        LogoutUser();
    });   
})

function LoginGetData(){

    const Login = $("#login");

    const Password = $("#password");

    const RememberMe = $("#remember");

    var rememberVal;

    if(RememberMe.is(':checked')){
        rememberVal = true;
    }
    else{
        rememberVal=false;
    }

    
        var stringJson = JSON.stringify({
            Login:Login.val(),
            Password:Password.val(),
            RememberMe:rememberVal,
        });
    
        LoginAuthorize(stringJson);
    
}


function RegisterGetData(){

    const Login = $("#login-reg");

    const Email = $("#email");

    const Password = $("#password-reg");

    const RememberMe = $("#remember-reg");

    var rememberVal;

    if(RememberMe.is(':checked')){
        rememberVal = true;
    }
    else{
        rememberVal=false;
    }

    
        var stringJson =  JSON.stringify({
            Login:Login.val(),
            Email:Email.val(),
            Password:Password.val(),
            RememberMe:rememberVal,
        });
    
        registerAuthorize(stringJson);
    
}

function LogoutUser(){
    StartAnimation();
    $.ajax({
        type: "POST",
        url: "/Account/Logout",
        success: function(result) {
            if(result==true){
                SuccessAnimation();
                setTimeout(StopAnimationReverse,2000);
                $(location).attr('href',location.origin+'/Main'); 
            }
            else{
                StopAnimation();  
            }                      
        },
        error: function(jqxhr, status, errorMsg){
            StopAnimation(); 
            ErrorMessage("Ошибка на сервере"); 
        } 
    });
}
function LoginAuthorize(jsonstring){
    StartAnimation();
    $.ajax({
        type: "POST",
        url: "/Account/Login",
        data: { loginModelJs:jsonstring },
        success: function(result) {
            if(result.success==true)
            {
                SuccessAnimation();
                setTimeout(StopAnimationReverse,2000);                         
            }
            else
            {
                StopAnimation();
                SetErrorsMessage(result.errors,$("#login-error"));
            }   
        },
        error: function(jqxhr, status, errorMsg){
            StopAnimation();
            SetErrorsMessage(["Ошибка на сервере, попробуйте позже"],$("#login-error"));
        } 
    });
}

function registerAuthorize(jsonstring){
    StartAnimation();
    $.ajax({
        type: "POST",
        url: "/Account/Register",
        data: { registerModelJs:jsonstring },
        success: function(result) {
            if(result.success==true)
            {
                SuccessAnimation();
                setTimeout(StopAnimationReverse,2000);                         
            }
            else
            {
                StopAnimation();
                SetErrorsMessage(result.errors,$("#register-errors"));
            }   
        },
        error: function(jqxhr, status, errorMsg){
            StopAnimation();
            SetErrorsMessage(["Ошибка на сервере, попробуйте позже"],$("#register-errors"));
        }
         
    });
    
}

function GetUserInfo(){
    $.ajax({
        type:"POST",
        url:"/Profile/GetUserInfo",
        success: function(result){
            $(".user-avatar_img").attr("src","/"+result.linkAvatar);
            $("#set-email").html(result.email);
        },
    })
}

function CheckAuthorize(){
    $.ajax({
        type:"POST",
        url:"/Account/CheckCookie",
        success:function(result){
            if(!result){
                location.reload();
            }
        }
    })
}

function SetErrorsMessage(massiv,element){
    element.html("");
    if(massiv!==null)
    {
        massiv.forEach((item)=>{
            element.append('<span>'+item+'</span>');
        });
    }
}   

    