
$(document).ready(function(){
    try{
        document.querySelector('#change-avatar-input').addEventListener('change',function (e) {
            if(this.files.length>0){
                if (window.FormData !== undefined){
                    var data = new FormData();
                    for (var x = 0; x < 1; x++) {
                        data.append("file" + x, this.files[x]);
                    }
                    if(ValidationAvatar(this.files[0])){
                        SendAvatar(data);
                    }
                }
                else{
                    ErrorMessage("Браузер не поддерживает загрузку файлов HTML5!");
                }
            }
        });
    }
    catch{
        
    }
    
});

$(document).ready(function(){
    const projectPanel = $(".projects-block-profile_inner");
    const favoritesPanel = $(".favorites-block-profile_inner");
    const settingsPanel = $(".settings-block-profile_inner");
    const addProjectPanel = $(".add-project-block-profile_inner");
    const settingProjectPanel = $(".setting-project-block_inner");

    const elementsPanel = [projectPanel,
    favoritesPanel,settingsPanel,addProjectPanel,settingProjectPanel];

    const buttonProject = $("#projectbutton");
    const buttonFavorites = $("#favoritesbutton");
    const buttonSetting = $("#settingbutton");
    const buttonAddProject = $("#add-project");
    const buttonProjectSetting = $("#settingProjectbutton")

    buttonProject.on('click',function(){
        OpenElement(projectPanel);
        PaintButton($(this));
    });
    buttonFavorites.on('click',function(){
        OpenElement(favoritesPanel);
        PaintButton($(this));
    });
    buttonSetting.on('click',function(){
        OpenElement(settingsPanel);
        PaintButton($(this));
    })
    buttonAddProject.on('click',function(){
        OpenElement(addProjectPanel);
        PaintButton($(this));
    });

    buttonProjectSetting.on('click',function(){
        OpenElement(settingProjectPanel);
        PaintButton($(this));
    })

    function ClosePanel(){
        elementsPanel.forEach(element => {
            element.css({
                "visibility":"hidden",
                "position":"absolute",
                "transform":"translateX(-100%)",
                "opacity":"0",
            });
        });
    }

    function OpenElement(element){
        ClosePanel();
        element.css({
            "visibility":"",
            "transform":"translateX(0%)",
            "position":"relative",
            "opacity":"1",
        });
    }

    function PaintButton(element){
        $(".menu-profile-item_inner").removeClass('active-profile-item');
        var parent = element.parent();
        parent.addClass('active-profile-item');
    }

    $("#add-project").on('click',FirstStage);

    $(".subscribe-button").click(function(){
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

});

function FirstStage(){
    if($(".stage_inner-result").html()!==""){
        $.ajax({
            type:"GET",
            url:"/UserProject/GetExplorerStart",
            success: function(result){
                $(".stage_inner-result").html(result);
                $(".stages-header-project").html("Шаг 1/3 - Загрузка архива");
                InitForm();
            },
            error:function(){
                ErrorMessage("Ошибка на сервере ,пробуйте позже еще раз");
            }
        })
    }
}
function InitForm(){
    document.querySelector('#zipProjectVal').addEventListener('change',function (e) {
        if(this.files.length>0){
            if (window.FormData !== undefined){
                var data = new FormData();
                for (var x = 0; x < this.files.length; x++) {
                    data.append("file" + x, this.files[x]);
                }
                ValidationZip(data,this.files[0]);
            }
            else{
                ErrorMessage("Браузер не поддерживает загрузку файлов HTML5!");
            }
        }
    });
}
function ValidationZip(data,element){
    var parts = element.name.split('.');
    var size = element.size;

    if(parts.length>1){
        exts = parts.pop();
    }
    
    if(exts==="zip"){
        if(size<52428800){
            SendZip(data);
        }   
        else{
            ErrorMessage("Файл превышает допустимый размер");
        } 
    }
    else{
        ErrorMessage("Файл имеет не подходящий формат");
    }
}

function SendUserAvatar(jsonObject){
    StartAnimation();
    $.ajax({
        type: "POST",
        url: "/Profile/ChangeAvatar",
        data: {byteImage:jsonObject},
        success: function(result){
            if(result){
                SuccessAnimation();
                setTimeout(StopAnimationReverse,2000);     
            }
            else{
                StopAnimation();
                $(".error-message-download-image").html("Файл имеет неправильный формат или превышает размер");
            }
        },
        error: function(jqxhr, status, errorMsg){
            StopAnimation();
            $(".error-message-download-image").html("Ошибка на сервере , попробуйте позже");
        }           
    });
}
function SendZip(data){
    StartAnimation();
    $.ajax({
        type:"POST",
        contentType: false,
        processData: false,
        url:"/UserProject/GetExplorer",
        data:data,
        success: function(result){
           $(".stage_inner-result").html(result);
           $(".stages-header-project").html("Шаг 2/3 - Настройки проекта");
           LoadExplorer();
           StopAnimation(),
           SuccessMessage("Успешно загружено");
        },
        error:function(){
            StopAnimation();
            ErrorMessage("Ошибка на сервере ,пробуйте позже еще раз");
        }
    })
}

function SendUserDescription(descriptionText){
    StartAnimation();
    $.ajax({
        type: "POST",
        url: "/Profile/ChangeDescription",
        data: {description:descriptionText},
        success: function(result){
            if(result){
                SuccessAnimation();
                setTimeout(StopAnimationReverse,2000);     
            }
            else{
                StopAnimation();
                $(".error-message-download-desciption").html("Ошибка на сервере");
            }
        },
        error: function(jqxhr, status, errorMsg){
            StopAnimation();
            $(".error-message-download-description").html("Ошибка на сервере , пробуйте позже");
        }           
    });
}

function SendAvatar(data){
    StartAnimation();
    $.ajax({
        type:"POST",
        url:"/Profile/ChangeAvatar",
        contentType: false,
        processData: false,
        data:data,
        success:function(result){
            if(result){
                SuccessAnimation();
                setTimeout(StopAnimationReverse(),1000);
            }
            else{
                StopAnimation();
                ErrorMessage("Ошибка на сервере , пробуйте позже");
            }
        }

    })
}

function ValidationAvatar(file){
    var parts = file.name.split('.');
    var size = file.size;
    if(parts.length>1){
        exts = parts.pop();
    }

    if(exts ==="png"||exts==="jpg"){
        if(size<20971520){
            return true;
        }
        else{
            ErrorMessage("Файл превышает допустимый размер");
        }
    }
    else{
        ErrorMessage("Формат изображения не подходит для загрузки")
    }

    return false;

}

