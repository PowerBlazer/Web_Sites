
$(document).ready(function(){
    document.querySelector('#input-file-avatar').addEventListener('change',function () {
        var reader = new FileReader();
        var selectedFile = this.files[0];
        reader.onload = function () {
            var comma = this.result.indexOf(',');
            var base64 = this.result.substr(comma + 1);
            SendAvatar(base64,selectedFile);
        }
        reader.readAsDataURL(selectedFile);
    }, false);
    
    function SendAvatar(byteImage,input){
        var elementError = $(".error-message-download-image");
        var parts = input.name.split('.');
        var exts = "null";
        var size = input.size;

        if(parts.length>1){
            exts = parts.pop();
        }
        
        if(exts==="png"||exts=="jpg"){
            if(size<=20971520){
                SendUserAvatar(byteImage); 
            }
            else{
                elementError.html("Файл превышает размер");
            }
        }
        else{
            elementError.html("Файл имеет не подходящий формат");
        }
    }

});
$(document).ready(function(){

    const buttonOpenModalDescription = $("#open-modal-description");
    const buttonCloseModalDescription = $("#close-modal-description");
    const buttinSendDescriptionInfo = $("#send-description");
    const elementModalDescription = $(".add-description-modal");
    const elementTextAreaDescription = $("#description-info");
    const buttonClearModalDesciption = $("#clear-textarea-description");
    const buttonChngeModalDescription = $("#change-description");

    buttonOpenModalDescription.on('click',OpenModalDescription);
    buttonCloseModalDescription.on('click',CloseModalDescription);
    buttonClearModalDesciption.on('click',ClearTextAreaDescription);
    buttinSendDescriptionInfo.on('click',SendDescriprionValue);
    buttonChngeModalDescription.on('click',ChangeOpenModalAndGiveText);

    function OpenModalDescription(){
        elementModalDescription.css({
            "visibility":"visible",
            "opacity":"1"
        });
    }

    function CloseModalDescription(){
        elementModalDescription.css({
            "visibility":"",
            "opacity":""
        });
    }

    function ClearTextAreaDescription(){
        elementTextAreaDescription.val('');
    }

    $(document).mouseup( function(e){
		if ( !$(".add-description-modal_inner").is(e.target)
		    && $(".add-description-modal_inner").has(e.target).length === 0 ) {
			CloseModalDescription();
		}
	});

    function SendDescriprionValue(){
        var text = elementTextAreaDescription.val();
        SendUserDescription(text);
    }

    function ChangeOpenModalAndGiveText(){
        OpenModalDescription();    
    }

    document.getElementById('description-info').addEventListener('keydown', function(e) {
        if (e.key == 'Tab') {
          e.preventDefault();
          var start = this.selectionStart;
          var end = this.selectionEnd;
          this.value = this.value.substring(0, start) +
            "\t" + this.value.substring(end);
          this.selectionStart =
            this.selectionEnd = start + 1;
        }
    });

    InitIframe();

    function InitIframe(){
        var linksIframe = '<meta charset=\'utf-8\'>\n'+
        '<meta name=\'viewport\' content=\'width=device-width, initial-scale=1\'>\n'+
        '<link rel=\'stylesheet\' type=\'text/css\' href=\'IframeInit.css\'>';
        $(".gener-description").contents().find('head').html(linksIframe);
        
    }
});
$(document).ready(function(){

    const profilePanel = $(".profile-info-block_inner");
    const projectPanel = $(".projects-block-profile_inner");
    const favoritesPanel = $(".favorites-block-profile_inner");
    const settingsPanel = $(".settings-block-profile_inner");
    const addProjectPanel = $(".add-project-block-profile_inner");

    const elementsPanel = [profilePanel,projectPanel,
    favoritesPanel,settingsPanel,addProjectPanel];

    const buttonProfile = $("#profilebutton");
    const buttonProject = $("#projectbutton");
    const buttonFavorites = $("#favoritesbutton");
    const buttonSetting = $("#settingbutton");
    const buttonAddProject = $("#add-project");

    buttonProfile.on('click',function(){
        OpenElement(profilePanel);
        PaintButton($(this));
    });
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
    })

    function ClosePanel(){
        elementsPanel.forEach(element => {
            element.css({
                "position":"absolute",
                "transform":"translateX(-100%)",
                "opacity":"0",
            });
        });
    }

    function OpenElement(element){
        ClosePanel();
        element.css({
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

});
$(document).ready(function(){

    const AddProjectPage = $("#add-project");
    const HeaderStage = $(".stages-header-project");
    const ResultBlock = $(".stage_inner-result");

    AddProjectPage.on('click',FirstStage);

    //Ajax
    function FirstStage(){
        if(ResultBlock.html()!==""){
            $.ajax({
                type:"GET",
                url:"/UserProject/GetExplorerStart",
                success: function(result){
                    ResultBlock.html(result);
                    HeaderStage.html("Шаг 1/3 - Загрузка архива");
                    InitForm();
                },
                error:function(){
                    ErrorMessage("Ошибка на сервере ,пробуйте позже еще раз");
                }
            })
        }
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
               ResultBlock.html(result);
               HeaderStage.html("Шаг 2/3 - Настройки проекта");
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

    



    //----  

    

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
});

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

