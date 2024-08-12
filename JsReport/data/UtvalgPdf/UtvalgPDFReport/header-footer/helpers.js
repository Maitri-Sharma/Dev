

function checkDistrDate(date) {
    console.log("checkDistrDate = " + date);
    if (date != undefined && date != '')
        return true;
    else return false;
}

function getPageNumber (pageIndex) {
    if (pageIndex == null) {
        return ''
    }

    const pageNumber = pageIndex + 1

    return pageNumber
}
function isChild(pageIndex){
    return pageIndex >0 ? true:false;
}
function getTotalPages (pages) {
    if (!pages) {
        return ''
    }

    return pages.length
}
