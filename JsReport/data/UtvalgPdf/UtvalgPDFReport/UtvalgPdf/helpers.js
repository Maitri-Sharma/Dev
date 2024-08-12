function totalSum(x1,x2,x3){
    return parseInt(x1)+parseInt(x2)+parseInt(x3);
}
function sum(val1, val2) {
    return val1 + val2   
}


function checkDistrDate(date) {
    if (date != undefined && date != '')
        return true;
    else return false;
}

function checkFlykeLevel(level){
    return level > 0 ? true:false;
}

function checkKommuneLevel(level){
    return level > 1 ? true:false;
}
function checkTeamLevel(level){
    return level > 2 ? true:false;
}
function checkRuteLevel(level,distdate){
    return (level===3 || distdate != "")? true:false;
}
function numberFormat(item){
  return item.toLocaleString("no-NO");
}

function checkIndex(index) {
    if (index != undefined && index == 0)
        return true;
    else return false;
}

function checkPostalCode(className)
{
     if (className != undefined && className == 'postalrow')
        return true;
    else return false; 
}

function CheckListName(reportName,listName)
{
     return reportName != listName;
}