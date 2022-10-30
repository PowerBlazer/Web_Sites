
function ProjectElementsForSettingsPanel(){

    const settingProjectButton = $(".setting-project-bt");
    const folderManagerButton = $(".folder-manager-project-bt");
    const deleteProjectButton = $(".delete-project-bt");

    const modalProject = $("#project-setting");

    deleteProjectButton.click(DeleteProject);
    folderManagerButton.click(FolderManager);
    settingProjectButton.click(SettingProject);

    function SettingProject(){
        OpenModal();
        GetSettingsProject($(this).val());
    }
    
    function DeleteProject(){
        OpenModal();
        modalProject.css("align-items","center");
        GetDeleteProjectConfirmPanel($(this).val());
    }

    function FolderManager(){
        OpenModal();
        let projectName = $(this).val();
        GetFolderManagerProject(projectName);
    }

    function StartDownloadAnimation(){
        modalProject.html("");
        let element = "<div class=\"load-animation\"></div>";
        $(".load-animation").css("visibility","visible");
        modalProject.html(element);
    }

    
    function OpenModal(){
        modalProject.css({
            "visibility":"visible",
            "opacity":"1",
            "align-items":"flex-start",
        });
        $('body').css("overflow","hidden");
        StartDownloadAnimation();
    }

    function CloseModal(){
        modalProject.html("");
        modalProject.css({
            "visibility":"",
            "opacity":"",
            "align-items":"",
        })
        $('body').css("overflow","");
    }

    function FolderManagerInit(){

        const closeFileMangerButton = $("#close-folder-manager");
        const fileItem = $(".explorer-item-name-file");
        const folderItem = $(".explorer-item-name-folder");
        const returnFolderButton = $("#return-path-button");

        folderItem.click(function(){
            let path = $(this).val();
            GetFolderItems(path);
        });

        returnFolderButton.click(function(){
            let path = $(this).val();
            GetFolderItems(path);
        })
        
        closeFileMangerButton.click(function(){
            CloseModal();
        });

        fileItem.click(function(){
            let path = $(this).val();
            GetContentFile(path);
        });

    }

    function ProjectSettingsInit(){
        $("#close-setting-project").click(CloseModal);

        const projectNameInput = $("#project-name-setting");
        const projectDescriptionInput = $("#description-project-setting");
        const projectTypesInput = $("#types-project-setting");

        const listDownLoadPage = $("#main-load-page-select");
        const listDownFormattingPage = $("#formatting-page-project");


        const inputImage = $("#change-image-project");

        const formattinButton = $("#formatting-button");
        const saveProjectButton  = $("#save-project-setting");

        projectNameInput.blur(function(){
            let projectName = $(this).val();
            let errorBlock = $(this).parent().find(".error-message-setting");
            errorBlock.html("");
            if($(this).data("currentname")!==projectName){
                CheckValidationProjectName(projectName,errorBlock);
            }
        });

        inputImage.change(function(){
            var reader = new FileReader();

            reader.onload = function(){
                $("#project-image-setting").attr('src',this.result);
            }

            reader.readAsDataURL(this.files[0]);
        });

        saveProjectButton.click(function(){

            let data = new FormData();

            if(inputImage[0].files.length !== 0){
                let format = inputImage[0].files[0].name.split('.');
                let size =  inputImage[0].files[0].size;

                if((format[1]==="jpg"||format[1]==="png")&&size<15728640){
                    data.append("file",inputImage[0].files[0]);
                }
                else{
                    ErrorMessage("Файл превышает допустимый размер или имеет не похоящий формат");
                    return;
                }
            }

            if(projectNameInput.parent()
            .find('.error-message-setting').html().length!==0){
                ErrorMessage("Такое имя уже существует");
                return;
            }

            if(projectNameInput.val().lenght===0){
                ErrorMessage("Название не может быть пустым");
                return;
            }

            let projectOptions = {
                Id:saveProjectButton.val(),
                Name:projectNameInput.val(),
                SelectedPage:$("#main-load-page-select option:selected").text(),
                Description:projectDescriptionInput.val(),
                Types:projectTypesInput.val(),
            }  

            data.append("projectOptions",JSON.stringify(projectOptions));

            UpdateProject(data);
        });

        listDownLoadPage.change(function(){
            let errorBlock = $(this).parent().find(".error-message-setting");

            errorBlock.html("");

            if(projectNameInput.val().length === 0){
                errorBlock.html("Укажите название проекта")
                return;
            }


        });

        formattinButton.click(function(){
            let errorBlock = $(this).parent().find(".error-message-setting");

            errorBlock.html("");
            if(projectNameInput.val().length === 0){
                errorBlock.html("Укажите название проекта")
                return;
            }

        
            let selectedPage = $("#formatting-page-project option:selected").val();

            FormattingFile(selectedPage,projectNameInput.val());
        });

    }

   function UpdateProject(data){
    StartAnimation();
    $.ajax({
        type:"POST",
        contentType: false,
        processData: false,
        url:"/UserProject/UpdateProjectOptions",
        data:data,
        success: function(result){
            SuccessMessage(result);
            StopAnimation();
            CloseModal();
            setTimeout(location.reload(),2000);
        },
        error:function(error){
            if(error.status===400){
                ErrorMessage(error.responseText);
            }

            if(error.status === 500){
                ErrorMessage("Ошибка на сервере попробуйте позже");
            }

            StopAnimation();
           
        }
    })
   }

    function GetDeleteProjectConfirmPanel(projectName){
        $.ajax({
            type:"GET",
            url:"Profile/GetDeleteProjectConfirmPanel",           
            success:function(result){
                modalProject.html(result);

                $("#confirm-delete-bt").click(function(){
                    DeleteProjectAjax(projectName);
                });

                $("#cancel-bt").click(function(){
                    CloseModal();
                });
            }
        })
    }

    


    function DeleteProjectAjax(projectName){
        StartDownloadAnimation();
        $.ajax({
            type:"POST",
            url:"UserProject/DeleteProject",
            data:{projectName:projectName},
            success:function(result){
                if(result){
                    CloseModal();
                    SuccessMessage("Успешно удалено");

                    location.reload();
                }
                else{
                    CloseModal();
                    ErrorMessage("Удаление не прошло , повторите попытку еще раз");
                }
            }
        })
    }

    function GetFolderManagerProject(projectName){
        $.ajax({
            type:"GET",
            url:"Profile/GetFolderManagerProject",
            data:{projectName:projectName},
            success:function(result){
                modalProject.html(result);
                FolderManagerInit();
            },
            error:function(){
                CloseModal();
                ErrorMessage("Попробуйте позже");
            }
        })
    }

    function GetFolderItems(path){
        $.ajax({
            type:"GET",
            url:"Profile/GetFolderItems",
            data:{path:path},
            success:function(result){
                modalProject.html(result);
                FolderManagerInit();
            },
            error:function(){
                ErrorMessage("Ошибка на сервере,пробуйте позже");
            }
        })
    }

    function GetSettingsProject(projectName){
        $.ajax({
            type:"GET",
            url:"Profile/GetSettingPageProject",
            data:{projectName:projectName},
            success:function(result){
                modalProject.html(result);
                ProjectSettingsInit();
            },
            error:function(jqXHR){
                CloseModal();
                if(jqXHR.status === 404){
                    ErrorMessage("Проект не найден");
                    return;
                }

                ErrorMessage("Ошибка на сервере , попробуйте позже");
            }

        })
    }




}


$("#settingProjectbutton").click(function(){
    if(!$(this).hasClass('disable')){
        GetProjectsForSetting();
        $(this).addClass('disable');
    }
})



//Ajax
function GetProjects(userName){
    $.ajax({
        type:"POST",
        url:"/Profile/GetUserProjects",
        data:{login:userName},
        success:function(result){
            $("#projects-block").html(result);
            InitModalProjectInfo();
            $("#projects-block").find(".open-project-info").click(OpenModalProject);
        },
        error:function(){
            ErrorMessage("Ошибка на сервере");
        }
    });
}

function GetFavorites(userName){
    $.ajax({
        type:"POST",
        url:"/Profile/GetUserFavorites",
        data:{userName:userName},
        success:function(result){
            $("#favorites-block").html(result);
            InitModalProjectInfo();
            $("#favorites-block").find(".open-project-info").click(OpenModalProject);
        },
        error:function(){
            ErrorMessage("Ошибка на сервере");
        }
    });
}

function GetProjectsForSetting(){
    $.ajax({
        type:"GET",
        url:"/Profile/GetProjectElementsForSettings",
        success:function(result){
            $("#project-list-setting").html(result);
            ProjectElementsForSettingsPanel();
        },
    })
}

//------