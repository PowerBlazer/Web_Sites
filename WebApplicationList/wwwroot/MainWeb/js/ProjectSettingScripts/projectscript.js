
function ProjectElementsForSettingsPanel(){

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