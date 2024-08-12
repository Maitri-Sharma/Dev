var currentRowIndex = 0;
var mergedCellContent = '';


function checkDistrDate(date) {
    if (date != undefined && date != '')
        return true;
    else return false;
}

function totalSum(x1, x2, x3) {
    return parseInt(x1) + parseInt(x2) + parseInt(x3);
}

function sum(val1, val2) {
    return val1 + val2
}

const axios = require('axios')

function getBase64(url) {
    return axios
        .get(url, {
            responseType: 'arraybuffer'
        })
        .then(response => Buffer.from(response.data, 'binary').toString('base64'))
}

async function ImgProcess(url) {
    return await getBase64(url);
}


function checkFlykeLevel(level) {
    return level > 0 ? true : false;
}

function checkKommuneLevel(level) {
    return level > 1 ? true : false;
}

function checkTeamLevel(level) {
    return level > 2 ? true : false;
}

function checkRuteLevel(level,distdate){
    return (level===3 || distdate != "")? true:false;
}

function numberFormat(item) {
    return item.toLocaleString("no-NO");
}

function getRowIndex() {
    console.log("getRowIndex = " + currentRowIndex);
    return parseInt(currentRowIndex);
}

function getMapLastRowIndex() {
    currentRowIndex = parseInt(currentRowIndex) + 20
    console.log("getMapLastRowIndex = " + currentRowIndex);
    return parseInt(currentRowIndex);
}
function getChildClass(cssClass)
{
    if(cssClass == "5")
         return 16;
    else if(cssClass == "6")
         return 17;
    else if(cssClass == "7")
         return 18;
    else if(cssClass == "8")
         return 19;   
}

function getMapID() {
    return "map" + currentRowIndex;
}

function rowCounter(addRows) {
    currentRowIndex = currentRowIndex + 1;

    if (addRows != undefined && !isNaN(addRows))
        currentRowIndex = currentRowIndex + parseInt(addRows);

    console.log("currentRowIndex = " + currentRowIndex);

}


function mergeCells(date) {

    console.log("currentRowIndex = " + currentRowIndex);

    if (checkDistrDate(date)) {
        if (mergedCellContent === '')
            mergedCellContent = '<mergeCell ref="C2:D2"/><mergeCell ref="G1:H3"/>';

        mergedCellContent = mergedCellContent + `<mergeCell ref="A${currentRowIndex}:H${currentRowIndex}"/>`;
    } else {
        if (mergedCellContent === '')
            mergedCellContent = '<mergeCell ref="G1:H3"/>';

        mergedCellContent = mergedCellContent + `<mergeCell ref="A${currentRowIndex}:F${currentRowIndex}"/>`;
    }

    console.log("mergedCellContent = " + mergedCellContent);
    return mergedCellContent;

}

function checkIndex(index) {
    if (index != undefined && index == 0)
        return true;
    else return false;
}

function CheckListName(reportName,listName)
{
     return reportName != listName;
}