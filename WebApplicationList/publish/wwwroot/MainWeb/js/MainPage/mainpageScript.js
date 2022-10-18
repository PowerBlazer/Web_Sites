$(document).ready(function(){
    const listDownPanel = $(".list-down-sorting-panel").find(".listdown-block");

    const listDownButton = $(".list-down-sorting-panel").find(".sorting-button");

    listDownButton.click(function(){
        if(listDownPanel.hasClass("disabled-listdown")){
            $(this).find(".sorting-button_inner").find("span").css({
                "transform":"rotate(0deg)",
                "border-bottom":"5px solid var(--button-color)"
            });
            $(this).find("span").css("color","var(--button-color)");
            $(this).css("color","var(--button-color)");
        }
        else{
            closeListDownFilters();
        }
        listDownPanel.toggleClass("disabled-listdown");
    });
    listDownButton.find(".sorting-button_inner").find("div").val("name");
    listDownPanel.find("button").click(function(){
        listDownPanel.addClass("disabled-listdown");
        listDownButton.find(".sorting-button_inner").find("div").html($(this).html());
        listDownButton.find(".sorting-button_inner").find("div").val($(this).val());
        if($(".search-filter-item").find(".active").html()=="Сайты"){
            SearchProjectsEvent();
        }
        if($(".search-filter-item").find(".active").html()=="Люди"){
            SearchUsersEvent();
        }
        
        closeListDownFilters();
    });

    $(".open-project-info").click(OpenModalProject);
    $(".close-modal").click(CloseModalProject);

    $(document).mouseup(function(e){
        if(!$("#project-content").is(e.target) && $("#project-content").has(e.target).length === 0&&
    !$(".pop-up-authorized").is(e.target)&&
    $(".pop-up-authorized").has(e.target).length===0){
            CloseModalProject();
        }
    });

    $(document).keydown(function(e){
        if($(".modal-project-info").css("opacity")==="1"&& e.keyCode==27){
            CloseModalProject();
        }
    })

    function closeListDownFilters(){
        listDownButton.find(".sorting-button_inner").find("span").css({
            "transform":"rotate(180deg)",
            "border-bottom":"5px solid black"
        });
        listDownButton.find("span").css("color","");
        listDownButton.css("color","");
    }

    $(document).mouseup(function(e){
		
		if ( !listDownButton.is(e.target)
		    && listDownButton.has(e.target).length === 0
             && !listDownPanel.is(e.target)&& listDownPanel.has(e.target).length===0) { 
			listDownPanel.addClass("disabled-listdown");
            closeListDownFilters();
		}
	});

    $(".filter-button").click(function(){
        $(".filter-button").removeClass("active");
        $(this).addClass("active");
    });


    //-----------------------

    const filtersButton = $(".list-down-filters-button");
    
    filtersButton.click(function(){
        ClickButtonGroupList($(this));
    });

    function ClickButtonGroupList(button){
        let groupPanel = button.next();
        let buttonArrow = button.find('div');
        if(groupPanel.hasClass("disabled-listdown")){
            button.css({
                "outline":"1px solid var(--button-color)",
            });
            buttonArrow.css({
                "transform":"rotate(0deg)",
                "border-bottom":"4px solid var(--button-color)"
            });
            button.find(".span").css({
                "font-size":"12px",
            })
            button.find(".span").addClass("active-span-list-down");
        }
        else{
            DisabledActive();
        }
        function DisabledActive(){
            if(button.find(".select").html().length===0){
                button.css({
                    "outline":"",
                });
                buttonArrow.css({
                    "border-bottom":"",
                    "transform":"",
                });
                button.find(".span").css({
                    "font-size":"",
                })
                button.find(".span").removeClass("active-span-list-down");
            }
            else{
                button.css({
                    "outline":"",
                });
                buttonArrow.css({
                    "border-bottom":"",
                    "transform":"",
                });
            }
        }

        groupPanel.toggleClass("disabled-listdown");

        groupPanel.find("li").find("button").click(function(){
            let value = $(this).html();
            if(value==="Все"){
                button.find(".select").html("");
                DisabledActive();
                groupPanel.addClass("disabled-listdown");
                groupPanel.find("li").find("button").off();
                SearchProjectsEvent();
                return;
            }
            button.find(".select").html(value);
            button.css({
                "outline":"",
            });
            buttonArrow.css({
                "border-bottom":"",
                "transform":"",
            });
            groupPanel.addClass("disabled-listdown");
            SearchProjectsEvent();
            groupPanel.find("li").find("button").off();
        });

        $(document).mouseup(function(e){
            if( !button.is(e.target)
            && button.has(e.target).length === 0
            && !groupPanel.is(e.target)&& groupPanel.has(e.target).length===0) { 
                groupPanel.addClass("disabled-listdown");
                    DisabledActive();
            }
        })
    }
    //------------------------


    $("#search-input").on("input",function(){
        if($(".search-filter-item").find(".active").html()=="Сайты"){
            SearchProjectsEvent();
            return;
        }
        if($(".search-filter-item").find(".active").html()=="Люди"){
            SearchUsersEvent();
        }
    });

    $(".filter-button").click(function(){
        if($(this).html()=="Сайты"){
            $(".list-down-filters-panel").css({
                "visibility":"",
                "opacity":"",
            });
            SearchProjectsEvent();
        }
        if($(this).html()=="Люди"){
            $(".list-down-filters-panel").css({
                "visibility":"hidden",
                "opacity":"0",
            });
            SearchUsersEvent();
        }
    })


    function SearchProjectsEvent(){
        const searchText = $("#search-input").val();
        const searchSortType = listDownButton.find(".sorting-button_inner").find("div").val();
        const searchProjectType = $(".list-down-filters-button").find(".select").html();

        let pageIndex = 20;

        if(searchProjectType==="Типы сайтов"){
            searchProjectType="";
        }

        let searchOptions = {
            Text:searchText,
            Type:searchProjectType,
            SortType:searchSortType,
            pageIndex:pageIndex
        }

        SearchProjects(JSON.stringify(searchOptions),$(".projects-blocks_inner"));
    }

    function SearchUsersEvent(){
        const searchText = $("#search-input").val();
        const searchSortType = listDownButton.find(".sorting-button_inner").find("div").val();
        let pageIndex = 20;

        let searchOptions = {
            Text:searchText,
            Type:"",
            SortType:searchSortType,
            pageIndex:pageIndex
        }

        SearchUsers(JSON.stringify(searchOptions));
    }


   

});


function SearchUserInit(){
    $(".subscribe-button").click(function(){
        let userName = $(this).val();
        if($(this).hasClass("disabled-button")){
            $(this).removeClass("disabled-button");
            $(this).find('div').html("Подписаться");
            UnSubscribe(userName);
        }
        else{
            $(this).addClass("disabled-button");
            $(this).find('div').html("Подписано");
            Subscribe(userName);
        }
    });
}

function SearchUsers(data,block){
    $.ajax({
        type:"POST",
        url:"/Main/SearchUsers",
        data:{searchOptionsJson:data},
        success:function(result){
            $(".projects-blocks_inner").html(result);
            SearchUserInit();
        },
        error:function(){
            ErrorsMessage("Ошибка на сервере пробуйте позже");
        }
    })
}
