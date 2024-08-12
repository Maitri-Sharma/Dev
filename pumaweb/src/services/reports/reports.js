import axios from "axios";
import Swal from "sweetalert2";
//import { datadogLogs } from "@datadog/browser-logs";
// import {  groupBy, groupByPostNr } from "../../Data";
import { ExportMapImage } from "./mapExport";
// import { Buffer } from "buffer";

// const exportSvcUrL = `${process.env.REACT_APP_MapPrint_API_URL}`;
const DataAccessUrl = `${process.env.REACT_APP_API_URL}`;

// const exportUrl = exportSvcUrL + "/Export%20Web%20Map%20Task/execute";

// async function getBase64(url) {
//   let pdfUrl = "";
//   await axios
//     .post(exportUrl, url, {
//       headers: {
//         "Content-Type": "application/x-www-form-urlencoded",
//       },
//     })
//     .then(async (response) => {
//       pdfUrl = response.data.results[0].value.url.toString();
//     });

//   if (pdfUrl === "") {
//     return "";
//   }
//   pdfUrl = pdfUrl.replace("http://", "https://");

//   return await axios
//     .get(pdfUrl, {
//       responseType: "arraybuffer",
//     })
//     .then((response) =>
//       Buffer.from(response.data, "binary").toString("base64")
//     );
// }

export async function getImageMap() {
  let url = await ExportMapImage([
    { reolId: 1017930007 },
    { reolId: 1017930008 },
  ]);
  return url;
}

// const groupbyData = (
//   strDayDetails,
//   data,
//   showReserved,
//   showHousehold,
//   showBusiness,
//   level
// ) => {
//   //Call groupBy with Data and distribution criteria
//   return groupBy(
//     data,
//     "",
//     level,
//     showHousehold,
//     showBusiness,
//     showReserved,
//     [],
//     strDayDetails
//   );
// };

// const groupByPostNrData = (
//   strDayDetails,
//   data,
//   showReserved,
//   showHousehold,
//   showBusiness,
//   level
// ) => {
//   //Call groupBy with Data and distribution criteria
//   return groupByPostNr(
//     data,
//     "",
//     level,
//     showHousehold,
//     showBusiness,
//     showReserved,
//     [],
//     strDayDetails
//   );
// };

// const callReportGenerator = async (dataLoad, exportType, filename, isList) => {
//   var data = {
//     template: {
//       name: exportType === "pdf" ? "UtvalgPdf" : "UtvalgToExcel",
//     },
//     data: dataLoad,
//   };

//   await axios
//     .post(ReportUrl, data, {
//       responseType: "blob",
//     })
//     .then((response) => {
//       if (!isList) {
//         if (exportType === "pdf") {
//           const url = window.URL.createObjectURL(
//             new Blob([response.data], { type: "application/pdf" })
//           );
//           const pdfWindow = window.open();
//           pdfWindow.location.href = url;
//         } else {
//           const url = window.URL.createObjectURL(
//             new Blob([response.data], {
//               type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//             })
//           );
//           const pdfWindow = window.open();
//           pdfWindow.location.href = url;
//         }
//       }
//       // const link = document.createElement('a');
//       // link.href = url;
//       // let fname = exportType === "pdf" ? '.pdf' : ".xlsx"
//       // link.setAttribute('download', filename+fname); //or any other extension
//       // document.body.appendChild(link);
//       // link.click();
//     })
//     .catch((err) =>
//       console.log("error Occured while generating report from server")
//     );
// };

// const processUtvalg = async (
//   activeUtvalg,
//   strDayDetails,
//   isCustomerWeb,
//   showHousehold,
//   showBusiness,
//   showReserved,
//   level,
//   index,
//   type,
//   isPostnrList,
//   includeAddressPoint
// ) => {
//   //call group by with distribution criteria
//   let itemsArray = [];
//   let reoler = activeUtvalg.reoler;
//   let name = activeUtvalg.name;
//   let logo = activeUtvalg.logo;

//   if (isPostnrList) {
//     itemsArray = groupByPostNrData(
//       strDayDetails,
//       reoler,
//       showReserved,
//       showHousehold,
//       showBusiness,
//       level
//     );
//   } else {
//     itemsArray = groupbyData(
//       strDayDetails,
//       reoler,
//       showReserved,
//       showHousehold,
//       showBusiness,
//       level
//     );
//   }

//   let cnt = 0;
//   itemsArray.map((item) => {
//     cnt = cnt + 1;
//     if (item.children) {
//       item.children.map((x) => {
//         cnt = cnt + 1;
//         if (x.children) {
//           x.children.map((y) => {
//             cnt = cnt + 1;
//             if (y.children) {
//               y.children.map((z) => {
//                 cnt = cnt + 1;
//               });
//             }
//           });
//         }
//       });
//     }
//   });

//   let imgUrl;
//   let imageData;

//   if (type == "excelDataOnly") {
//     imgUrl = "";
//   } else
//     await ExportMapImage(
//       reoler,
//       isCustomerWeb,
//       strDayDetails,
//       includeAddressPoint
//     ).then((result) => (imgUrl = result));

//   if (type == "excelDataOnly") {
//     imageData = "";
//   } else await getBase64(imgUrl).then((result) => (imageData = result));

//   let totalCount = 0;
//   let zone0Count = 0;
//   let zone1Count = 0;
//   let zone2Count = 0;
//   let dag2Count = 0;
//   let dag1Count = 0;
//   let houseHolds = 0;
//   let business = 0;

//   itemsArray.map((item) => {
//     totalCount = totalCount + item.total;
//     zone0Count = zone0Count + item.zone0;
//     zone1Count = zone1Count + item.zone1;
//     zone2Count = zone2Count + item.zone2;
//     dag1Count = dag1Count + item.HHD1 + item.VHD1;
//     dag2Count = dag2Count + item.HHD2 + item.VHD2;
//     houseHolds =
//       houseHolds +
//       (showHousehold ? item.house : 0) +
//       (showReserved ? item.householdsReserved : 0);
//     business = business + (showBusiness ? item.businesses : 0);
//   });

//   return {
//     id: index,
//     itemsCount: cnt,
//     showReserverd: showReserved,
//     showHousehold: showHousehold,
//     showBusiness: showBusiness,
//     name: name,
//     HouseHolds: houseHolds,
//     Business: business,
//     Reoler: itemsArray,
//     imageurl: imgUrl,
//     forhandlerpartyk: logo,
//     zone0: zone0Count,
//     zone1: zone1Count,
//     zone2: zone2Count,
//     total: totalCount,
//     dag1: dag1Count,
//     dag2: dag2Count,
//     imagedata: imageData,
//   };
// };

export const genReportUtvalg = async (
  activeUtvalg,
  distrDate,
  strDayDetails,
  isCustomerWeb,
  showHousehold,
  showBusiness,
  showReserved,
  level,
  type,
  uptolevel,
  isList,
  isPostNrList,
  emailAddr,
  includeAddressPoint
) => {
  //call dataaccessapi
  // let utvalgArray = [];
  // let ReportName = isList ? activeUtvalg.name : "";
  isCustomerWeb = isCustomerWeb === undefined ? false : isCustomerWeb;

  var selectedAddressPoint = JSON.parse(JSON.stringify(sessionStorage.getItem("addressPoints"))) ;
  if (!includeAddressPoint)
    selectedAddressPoint = "";
  if (isList) {
    await axios
      .post(
        DataAccessUrl +
        "/Reports/GenerateReport?listId=" +
        activeUtvalg.listId +
        "&distrDate=" +
        distrDate.toString() +
        "&strDayDetails=" +
        strDayDetails +
        "&reportType=" +
        type +
        "&emailTo=" +
        emailAddr +
        "&level=" +
        level +
        "&uptolevel=" +
        uptolevel +
        "&showBusiness=" +
        showBusiness +
        "&showHouseholds=" +
        showHousehold +
        "&showHouseholdsReserved=" +
        showReserved +
        "&isCustomerWeb=" +
        isCustomerWeb,
        {
          selectedAddress: selectedAddressPoint
        }
      )
      .then((respose) => {
        // alert("Forespørselen er sendt. Du vil motta rapporten i en e-post");
        let msg = "Forespørselen er sendt. Du vil motta rapporten i en e-post";
        Swal.fire({
          text: msg,
          confirmButtonColor: "#7bc144",
          confirmButtonText: "Lukk",
          position: "top"
        });
      })
      .catch(
        console.log(
          "Issue with Calling DataAccessAPi GenerateReport Controller"
        )
      );
    return "";
  } else {
    // utvalgArray.push(
    //   await processUtvalg(
    //     activeUtvalg,
    //     strDayDetails,
    //     isCustomerWeb,
    //     showHousehold,
    //     showBusiness,
    //     showReserved,
    //     level,
    //     10,
    //     type,
    //     isPostNrList,
    //     includeAddressPoint
    //   )
    // );

    // //Prepare the Object for Report Service
    // const reportPayLoad = {
    //   reportName: ReportName,
    //   level: level,
    //   emailTo: emailAddr,
    //   isList: isList,
    //   isEmail: isList ? true : false,
    //   uptoLevel: uptolevel,
    //   isMapwithData: type == "excelDataOnly" ? 0 : 1,
    //   distrDate: distrDate,
    //   items: utvalgArray,
    // };

    // //Call Report Service URL
    // await callReportGenerator(reportPayLoad, type, activeUtvalg.name, isList);

    // return "";

    await axios
      .post(
        DataAccessUrl +
        "Reports/GenerateSelectionReport?" +
        "&distrDate=" +
        distrDate.toString() +
        "&strDayDetails=" +
        strDayDetails +
        "&reportType=" +
        type +
        "&level=" +
        level +
        "&uptolevel=" +
        uptolevel +
        "&showBusiness=" +
        showBusiness +
        "&showHouseholds=" +
        showHousehold +
        "&showHouseholdsReserved=" +
        showReserved +
        "&isCustomerWeb=" +
        isCustomerWeb,
        {
          selectedAddress: selectedAddressPoint,
          utvalgData: activeUtvalg
        },
        {
          "Content-Type": "application/json",
          responseType: "blob",
        }
      )
      .then((response) => {
        if (type === "pdf") {
          const url = window.URL.createObjectURL(
            new Blob([response.data], { type: "application/pdf" })
          );
          const pdfWindow = window.open();
          pdfWindow.location.href = url;
        } else {
          const url = window.URL.createObjectURL(
            new Blob([response.data], {
              type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            })
          );
          //const pdfWindow = window.open();
          window.location.href = url;
        }
        // alert("Forespørselen er sendt. Du vil motta rapporten i en e-post");
      })
      .catch(
        console.log(
          "Issue with Calling DataAccessAPi GenerateReport Controller"
        )
      );
    return "";
  }
};
