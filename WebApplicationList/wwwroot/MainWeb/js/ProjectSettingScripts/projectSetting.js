function LoadExplorer(){

    const ResultPanel = $(".explorer-items_inner");
    const ResultFileContent = $("#result-text-file");
    const ModalFileWindow = $(".modal-check-file");
    const FolderOpenButton = $(".explorer-item-name-folder");
    const FileOpenButton = $(".explorer-item-name-file");
    const SettingsOpen = $("#setting-project-title");
    const FileManagerOpen = $("#file-mamager-title");
    const ReturnFirstExplorerButton = $("#return-first-page-explorer");
    const ReturnFolder = $("#return-path-button");

    FolderOpenButton.on('click',FolderOpen);
    ReturnFolder.on('click',FolderOpen);
   
    FileOpenButton.on('click',function(){
        var path = $(this).val();
        GetContentFile(path);
    })
    FileManagerOpen.on('click',function(){
        ClearPaintButtonsTitle();
        GetExplorerItem($(this).val());
        $(this).css({
            "color":"var(--button-color)",
        });
        $(".permision-next").html("");
    });
    SettingsOpen.on('click',function(){
        ClearPaintButtonsTitle();
        $(this).css({
            "color":"var(--button-color)",
        });
        var buttonView = "<button class=\"next-setting-project\">Сохранить</button>";

        $(".permision-next").html(buttonView);
        GetSettings();
    });

    function FolderOpen(){
        var path = $(this).val();
        GetExplorerItem(path);
    }
    function ClearPaintButtonsTitle(){
        [SettingsOpen,FileManagerOpen].forEach(element=>{
            element.css({
                "color":""
            });
        });
    }
    
    ReturnFirstExplorerButton.on('click',function(){
        $("#add-project").click();
    });

    

    function InitSettings(){
        var _byteImage="";
        var selectedPageValue ="";

        const ProjectNameInput = $("#project-name-input");
        const ProjectDescriptionImput = $("#project-description-input");
        const ProjectTypesInput = $("#project-types-input");

        const ListdownButton = $(".listdown-button-page");
        const ListDownPanel = $(".list-down-select-page");
        const ListDownItemButton = $(".list-down-page-item");
        const ListDownItemButtonFormat = $(".list-down-page-item-format");
        const SaveProjectButton = $(".next-setting-project");

        ListdownButton.on('click',OpenListDown);
        $("#close-modal-filecontent").on('click',CloseModalFileContent);
        SaveProjectButton.on('click',SaveProject);


        ProjectNameInput.blur(function(){
            var labelElem = $(this).prev();
            labelElem.css("visibility","hidden");
            $(".generate-url-block").css("display","none");
            if($(this).val()==""){
                labelElem.css("visibility","");
                $("#error-message-name").html("Это поле не может быть пустым");
                return;
            }
            CheckValidationProjectName($(this).val(),$("#error-message-name"));
        });
        ProjectNameInput.focus(function(){
            var labelElem = $(this).prev();
            labelElem.css("visibility","hidden");
        });
        ProjectDescriptionImput.blur(function(){
            var labelElem = $(this).prev();
            labelElem.css("visibility","hidden");

            if($(this).val()==""){
                labelElem.css("visibility","");
            }
        });
        ProjectDescriptionImput.focus(function(){
            var labelElem = $(this).prev();
            labelElem.css("visibility","hidden");
        });
        ProjectTypesInput.blur(function(){
            var labelElem = $(this).prev();
            labelElem.css("visibility","hidden");
            if($(this).val()==""){
                labelElem.css("visibility","");
            }
        });
        ProjectTypesInput.focus(function(){
            var labelElem = $(this).prev();
            labelElem.css("visibility","hidden");
        });

        function OpenListDown(){
            ListdownButton.off('click');
            $(this).next().css({
                "opacity":"1",
                "visibility":"visible",
                "padding":"10px 15px",
            })
            ListdownButton.on('click',CloseListDown);
        }
        function CloseListDown(){
            ListdownButton.off('click');
            ListDownPanel.css({
                "opacity":"",
                "visibility":"",
                "padding":"",
            })
            ListdownButton.on('click',OpenListDown);
        }
        
        ListDownItemButton.on('click',SelectDownItem);
        ListDownItemButtonFormat.on('click',SelectDownItemFormat);

        function SelectDownItem(){
            $("#error-message-page").html("");
            if(ProjectNameInput.val()==="" || $("#error-message-name").html()!=="")
            {
                $("#error-message-page").html("Отсутсвует название сайта");
                return;
            }

            $(this).parent().prev().html("Select:"+$(this).html());
            var page = $(this).find("span").html();
            selectedPageValue = page;
            GenerateSiteUrl(page);
        }

        function SelectDownItemFormat(){
            if(ProjectNameInput.val()==="" || $("#error-message-name").html()!==""){
                $("#error-message-format").html("Отсутсвует название сайта");
                return;
            }
            $("#error-message-format").html("");
            var header = $(this).parent().prev();
            header.html($(this).html());
            var button = "<button value=\""+$(this).val()+"\">Форматировать</button>";
            $("#format-button-active").html(button);
            $("#format-button-active").find("button").on('click',function(){
                var projectName = ProjectNameInput.val();
                var path = $(this).val();
                FormattingFile(path,projectName);
            })
        }

        function GenerateSiteUrl(selectName){
            var url = GetUrl();
            url = url+"/UserProject?Site="+ProjectNameInput.val()+
            "&page="+selectName;
            $(".generate-url-block").css("display","");
            $("#generate-url").html(url);
        }
        
        $(document).mouseup( function(e){
            if ( !ListdownButton.is(e.target)
                && ListdownButton.has(e.target).length === 0 && !ListDownPanel.is(e.target)&&
                ListdownButton.has(e.target).length===0) {
                CloseListDown();
            }
        });

        document.querySelector('#load-image-project').addEventListener('change',function () {
            var reader = new FileReader();
            var selectedFile = this.files[0];
            reader.onload = function () {
                var comma = this.result.indexOf(',');
                var base64 = this.result.substr(comma + 1);
                SetImage(this.result);
                SendProjectImage(base64,selectedFile);
            }
            reader.readAsDataURL(selectedFile);
        }, false);

        function SetImage(resultImage){
            $("#result-load-image").css("display","");
            $("#result-load-image").attr("src",resultImage);
        }

        function SendProjectImage(byteImage,input){
            var parts = input.name.split('.');
            var exts = "null";
            var size = input.size;
    
            if(parts.length>1){
                exts = parts.pop();
            }
            
            if(exts==="png"||exts=="jpg"){
                if(size<=20971520){
                    _byteImage = byteImage;   
                }
                else{
                    ErrorMessage("Файл превышает размер");
                }
            }
            else{
                ErrorMessage("Файл имеет не подходящий формат");
            }
        }

        function SaveProject(){
            var projectName = ProjectNameInput.val();
            var description = ProjectDescriptionImput.val();
            var Types = ProjectTypesInput.val();

            if(projectName===""||projectName.length==0)
            {
                ErrorMessage("Не указано название проекта");
                return;
            }

            if(_byteImage==="")
            {
                ErrorMessage("Не загружено изображение");
                return;
            }

            if(selectedPageValue==="")
            {
                ErrorMessage("Не указана главная страница сайта");
                return;
            }

            var ProjectSettingViewModel = {
                Name:projectName,
                Description:description,
                Types:Types,
                ImageByte:_byteImage,
                SelectedPage:selectedPageValue,
            }

            SaveProjectAjax(JSON.stringify(ProjectSettingViewModel));

        }
    }


   

    //AJAX
    function GetExplorerItem(path){
        $.ajax({
            type:"POST",
            url:"/UserProject/GetExplorerItems",
            data:{path:path},
            success:function(result){
                ResultPanel.html(result);
                LoadExplorer();
            },
            error:function(){
                ErrorMessage("Ошибка на сервере ,пробуйте позже еще раз");
            }
        })
    }
    
    
    function GetSettings(){
        $.ajax({
            type:"POST",
            url:"/UserProject/GetSettingsProject",
            success:function(result){
                ResultPanel.html(result);
                InitSettings();
            },
            error:function(){
                ErrorMessage("Оишбка на сервере, пробуйте позже");
            }
        })
    }
    
    function SaveProjectAjax(projectSettingVM){
        StartAnimation();
        $.ajax({
            type:"POST",
            url:"/UserProject/SaveProject",
            data:{jsonProjectSettingVM:projectSettingVM},
            success:function(result){
                if(!result){
                    StopAnimation();
                    ErrorMessage("Ошибка,пробуйте позже");
                }
                StopAnimation();
                SuccessMessage("Успешно сохранено");
                window.location.href = location.origin+'/Profile?page=projects';
            },
            error:function(){
                StopAnimation();
                ErrorMessage("Ошибка на сервере, попробуйте еще раз позже");
            }
        })
    }

}
function FormattingFile(path,projectName){
    StartAnimation();
    $.ajax({
        type:"POST",
        url:"/UserProject/FormattingFile",
        data:{
            path:path,
            projectName:projectName,
        },
        success:function(result){
            StopAnimation();
            SuccessMessage("Успешно отформатировано")
            $("#result-text-file").html(result);
            OpenModalFileContent();
            $("#close-modal-filecontent").on('click',CloseModalFileContent);
        },
        error:function(){
            StopAnimation();
            ErrorMessage("Ошибка на сервере ,пробуйте позже еще раз");
        }

    })
}



function ChangeContentFile(path,content){
    $.ajax({
        type:"POST",
        url:"/UserProject/ChangeContentFile",
        data:{
            content:content,
            path:path,
        },
        success:function(result){
            if(result){
                CloseModalFileContent();
                SuccessMessage("Успешно сохранено");
            }
            else{
                ErrorMessage("Ошибка на сервере ,пробуйте позже еще раз");
            }
        },
    });
}
function CheckValidationProjectName(projectName,element){
    $.ajax({
        type:"POST",
        url:"/UserProject/GetValidationNameProject",
        data:{projectName:projectName},
        success:function(result){
            if(!result){
                element.html("Это имя занято,придумайте другое");
            }
            else{
                element.html("");
            }
        },
    })
}

function GetContentFile(path){
    $.ajax({
        type:"POST",
        url:"/UserProject/GetFileContent",
        data:{path:path},
        success:function(result){
            $("#result-text-file").html(result);
            OpenModalFileContent();
            InitModalFile();
        },
        error:function(){
            ErrorMessage("Ошибка на сервере ,пробуйте позже еще раз");
        }
    })
}

function OpenModalFileContent(){
    $(".modal-check-file").css({
        "opacity":"1",
        "visibility":"visible",
    });
    $("body").css("overflow","hidden");
    
}
function CloseModalFileContent(){
    if($(".modal-check-file").css("visibility")==="visible"){
        $(".modal-check-file").css({
            "opacity":"",
            "visibility":"",
        });
        if($("#project-setting").css("visibility")!=="visible"){
            $("body").css("overflow","");
        }
       
    }
}

function InitModalFile(){

    const CloseModalFile = $("#close-modal-filecontent");
    const SenfFileContentButton = $("#send-file-content");
    const FileContentTextArea = $("#file-content-input");

    document.getElementById('file-content-input').addEventListener('keydown', function(e) {
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

    $(document).keyup(function(e){
        if(e.keyCode===27){
            CloseModalFileContent();
        }
    })

    $(document).mouseup( function(e){
        if ( !$(".modal-check-window").is(e.target)
            && $(".modal-check-window").has(e.target).length === 0 ) {
            CloseModalFileContent();
        }
    });

    function SendContentFile(){
        var path = $(this).val();
        var fileContent = FileContentTextArea.val();
        ChangeContentFile(path,fileContent)
    }

    CloseModalFile.on('click',CloseModalFileContent);
    SenfFileContentButton.on('click',SendContentFile);
}

