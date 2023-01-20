/******************/
/*** JQUERYMAPS ***/
/******************/

	/*** JQM LIBRARY ***/
	if (window.location.href.indexOf('jqmdebug') != -1)
		document.write("<script src='http://localhost/jqm/jquerymaps/libs/jquerymaps/JQueryMaps_debug.js'><\/script>");
	else
		document.write("<script src='jquerymaps/libs/jquerymaps/JQueryMaps.js'><\/script>");
		
	//*** VARIABLES ***
	var jqmMap;
	
	//*** DOCUMENT READY ***
	$(document).ready(function() {
		jqmLoadMap();
	});
	
	//*** JQM - MAP LOAD ***
	function jqmLoadMap() {
	    var theme = "jquerymaps/theme/golf_terr_us.aspx";
//	    var theme = "jquerymaps/theme/us.aspx";
		var params = { mapDivId: "jqmMap", configUrl: "jquerymaps/jqm_config.xml", initialThemeUrl: theme, width: "1000", height: "600"};
		jqmMap = new JQueryMaps.Map(params);
		
		jqmLoadLegend();
    }
	
	function jqmFromMap(obj) {
	    
        var f = obj.clickedFeatures;
        
        //BREADCRUMB
        if (obj.event == "zoomFinished") { 
            jqmBreadCrumb(f); 
            
            if (f.length == 0)
                $("#jqm_info").html("");
            else
                jqmInfo(f[f.length - 1].id);
        }
    }
	
	function jqmLoadLegend() {
		$.ajax({url: "jquerymaps/info/golf_territory_legend.aspx", success: function(data) { $("#jqm_legend").html(data).show("blind");  } });
	}
	
	function jqmLoadState(obj) {
	    jqmMap.clickOnFeature(obj.id.substr(0, 5));
	}
	
	function jqmBreadCrumb(f) {
	    
	    var title = "";
		if (f.length > 0) {
    			title = "<a href='Javascript:jqmMap.displayInitialView();' title='Back to National View' class='bread'>National View</a>";
			for (i = f.length; i > 0; i--) {
				if (i > 1)  
				    title += " &rsaquo; <a href='Javascript:jqmMap.getBackToPreviousLevel();' title='Back to " + f[f.length - i].label + "' class='bread'>" + f[f.length - i].label + "</a>";
				else
					title += " &rsaquo; " + f[f.length - i].label;
			}
		} else {
			title = "National View";
		}
        
		$("#jqm_bread").html(title);
    }
    
    function jqmInfo(id) {
        $.ajax({url: "jquerymaps/info/info.aspx", data: "id=" + id, success: function(data) { $("#jqm_info").html(data);  } });
    }