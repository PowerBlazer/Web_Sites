$(document).ready(function(){
    $("#settingbutton").click(function(){
        InitSettings();
    });
});


function InitSettings(){
    

    var valuesElement = [$("#description-content"),$("#surname-rename"),
    $("#year-rename"),$("#proffesion-rename"),$("#header-description-input")];

    for(var i = 0;i<valuesElement.length;i++){
        valuesElement[i].blur(GetValuesInput);
    }

    $("#send-newPassword").click(ChangePassword);
    $(".link-set").click(SetInputLinks);

    

}

function GetValuesInput(){
    const DescriptionBlock = $("#description-content");
    const YearBlock = $("#year-rename");
    const ProfessionBlock = $("#proffesion-rename");
    const HeaderDescriptionBlock = $("#header-description-input");

    var profileUserInfo = {
        Surname:"",
        Year:parseInt(YearBlock.val()),
        Profession:ProfessionBlock.val(),
        HeaderDescription:HeaderDescriptionBlock.val(),
        Description:DescriptionBlock.val(),
    }

    SendNewUserInfo(JSON.stringify(profileUserInfo));
}

function ChangePassword(){
    var previousPassword = $("#previous-password-rename").val();
    var newPassword = $("#new-password-rename").val();
    var newPasswordConfirm = $("#confirm-password-rename").val();

    if(previousPassword.length==0||newPassword.length==0||
        newPasswordConfirm.length==0)
    {
        SetErrors(["Все поля должны быть заполнены"]);
        return;
    }

    if(newPassword!==newPasswordConfirm){
        SetErrors(["Пароли должны одинаковы"]);
        return;
    }

    var passwordModel = {
        OldPassword:previousPassword,
        NewPassword:newPassword,
    }

    SendNewPassword(JSON.stringify(passwordModel));

}

function ClearPasswordInputs(){
    $("#previous-password-rename").val("");
    $("#new-password-rename").val("");
    $("#confirm-password-rename").val("");
}

function ClearErrors(){
    $(".password-message-errors").html("");
}

function SetErrors(massiv){
    ClearErrors();
    for(var i=0;i<massiv.length;i++){
        $(".password-message-errors").append("<li>"+massiv[i]+"</li>");
    }
}

function SetInputLinks(){
    const Modal = $(".modal-bind-link_inner");
    const CloseModal = $("#bind-modal-close");
    const SendButton = $("#bind-link");
    const Input = $("#bind-link-value");

    SendButton.val($(this).val());

    Modal.css({
        "opacity":"1",
        "visibility":"visible",
    });

    CloseModal.click(CloseModalFunc);

    SendButton.click(function(){
        if(SendButton.val().length!==0){
            var id = SendButton.val();
            BindLinkUser(parseInt(id),Input.val());
            CloseModalFunc();
        }

    });

    function CloseModalFunc(){
        Input.val("");
        Modal.css({
            "opacity":"",
            "visibility":"",
        });
        CloseModal.off();
        SendButton.off();
    }
    
}




//AJAX
function SendNewUserInfo(data){
    $.ajax({
        type:"POST",
        url:"/Profile/ChangeUserInfo",
        data:{jsonProfileUserInfo:data},
        success:function(result){
            if(result){
               SuccessMessage("Изменено"); 
            }
            else{
                ErrorMessage("Ошибка на сервере");
            }
        },
        error:function(){
            ErrorMessage("Ошибка на сервере");
        }
    })
}

function SendNewPassword(data){
    $.ajax({
        type:"POST",
        url:"/Account/ChangePassword",
        data:{jsonPasswordModel:data},
        success:function(result){
            if(result.success){
                ClearErrors();
                SuccessMessage("Успешно Изменено");
            }
            else{
                ClearPasswordInputs();
                SetErrors(result.errors);
            }
        },
        error:function(){
            ErrorMessage("Ошибка на сервере , попробуйте позже");
        }
    })
}

function BindLinkUser(id,url){
    $.ajax({
        type:"POST",
        url:"/Profile/BindLinkUser",
        data:{
            url:url,
            id:id,
        },
        success:function(result){
            if(result){
                SuccessMessage("Успешно сохранено");
            }
            else{
                ErrorMessage("Не сохранено,проверьте правильность ввода")
            }
        },
        error:function(){
            ErrorMessage("Ошибка на сервере");
        }
    })
}


//----