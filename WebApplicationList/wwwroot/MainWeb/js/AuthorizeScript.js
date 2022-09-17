$(document).ready(function(){
    
    $(".post-data-register").on('click',RegisterGetData);
    $(".post-data-login").on('click',LoginGetData);
    $("#logout-but").on('click',function(){
        LogoutUser();
    });

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

    
})

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

function GetAvatar(){
    $.ajax({
        type:"POST",
        url:"/Account/GetAvatar",
        success: function(result){
            if(result!==""){
                $("#user-avatar").attr("src","/"+result);
                //$("#profile-image-list").attr("src","/"+result);
            }
            else{
               $("#profile-image").attr("src","/UserIcons/defaultAvatar.jpg");
            }
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

    