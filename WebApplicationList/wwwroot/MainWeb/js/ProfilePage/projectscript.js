$(document).ready(function(){
    $("#projectbutton").click(function(){
        InitProject();
    });
});

function InitProject(){
    const ProjectsBlock = $("#projects-block");
    
    GetProjects("",ProjectsBlock);
}


//Ajax

function GetProjects(userName,element){
    $.ajax({
        type:"POST",
        url:"/Profile/GetUserProjects",
        data:{login:userName},
        success:function(result){
            element.html(result);
        },
        error:function(){
            ErrorMessage("Ошибка на сервере");
        }
    });
}

//------