

function InitProject(userName){
    const ProjectsBlock = $("#projects-block");
    GetProjects(userName,ProjectsBlock);

}


//Ajax

function GetProjects(userName,element){
    $.ajax({
        type:"POST",
        url:"/Profile/GetUserProjects",
        data:{login:userName},
        success:function(result){
            element.html(result);
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

//------