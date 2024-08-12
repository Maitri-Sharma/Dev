import axios from "axios";
import Swal from "sweetalert2";

const baseDataLayerEndpoint = `${process.env.REACT_APP_API_URL}`;

const getdata = (url) => {
  return axios.get(baseDataLayerEndpoint + url, {
    headers: {
      Authorization: "Bearer " + sessionStorage.getItem("Token"),
    },
  });
};
const putData = (url, bodyData) => {
  return axios.put(baseDataLayerEndpoint + url, bodyData, {
    headers: {
      Authorization: "Bearer " + sessionStorage.getItem("Token"),
    },
  });
};
const getToken = (url, bodyData) => {
  return axios.post(baseDataLayerEndpoint + url, bodyData);
};
const postdata = (url, bodyData) => {
  return axios.post(baseDataLayerEndpoint + url, bodyData, {
    headers: {
      Authorization: "Bearer " + sessionStorage.getItem("Token"),
    },
  });
};
const deletedata = (url) => {
  return axios.delete(baseDataLayerEndpoint + url, {
    headers: {
      Authorization: "Bearer " + sessionStorage.getItem("Token"),
    },
  });
};

axios.interceptors.response.use(
  (responses) => {
    return responses;
  },
  function (error) {
    //Unauthorized or token expired
    if (
      (error?.response?.status.toString() === "500" &&
        error.response.data.Message.toLowerCase().indexOf("expired") >= 0) ||
      error?.response?.status.toString() === "401"
    ) {
      Swal.fire({
        text: "Økten er avsluttet. Klikk på ok for å logge på igjen.",
        showConfirmButton: true,
        confirmButtonText: "OK",
        confirmButtonColor: "#7bc144",
        allowOutsideClick: false,
        allowEscapeKey: false,
      }).then((result) => {
        if (result.isConfirmed) {
          sessionStorage.setItem("userName", "");
          //If its kunderweb then redirect to OEBS login page
          if (window.location.href.toLowerCase().includes("pumakundeweb"))
            window.location.href = `${process.env.REACT_APP_KUNDEWEB_LOGIN_URL}`;
          //Or else redirect to internweb
          else window.location.href = window.location.origin;
        }
      });
    }
    return Promise.reject(error);
  }
);

/******************Data Dog Initialization START ****************************** */
// datadogLogs.init({
//   clientToken: `${process.env.REACT_APP_DATADOG_CLIENT_TOKEN}`,
//   service: "pumaweb",
//   env: `${process.env.REACT_APP_ENV}`,
//   site: "datadoghq.eu",
//   forwardErrorsToLogs: true,
//   sampleRate: 100,
//   silentMultipleInit: true,
// });

// const logger = //datadogLogs.logger; // datadogLogs.getLogger("Logger");
// logger.setContext({ env: `${process.env.REACT_APP_ENV}` });
// logger.addContext("referrer", document.referrer);
/******************Data Dog Initialization END ****************************** */

export default {
  getdata,
  postdata,
  deletedata,
  // logger,
  getToken,
  putData,
};
