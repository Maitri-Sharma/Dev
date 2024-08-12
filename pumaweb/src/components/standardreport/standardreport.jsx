﻿import React, { useState, useContext, useEffect, useRef } from "react";
import { genReportUtvalg, getImageMap } from "../../services/reports/reports";
import { KSPUContext, MainPageContext } from "../../context/Context";
import loadingImage from "../../assets/images/callbackActivityIndicator.gif";
import api from "../../services/api.js";

import "../standardreport/standardreport.styles.scss";

const getPRSDateDetails = async (date) => {
  const { data, status } = await api.getdata(
    "GetPrsCalendarAdminDetails/GetPRSAdminCalendarDayDetail?FindDate=" +
      date.split(".").reverse().join("-")
  );
  if (status === 200) {
    if (data.frequencyType === null) {
      return "";
    }
    switch (data.frequencyType.trim()) {
      case "A":
        if (data.isEarlyWeekFirstDay || data.isEarlyWeekSecondDay) {
          return "A-uke, tidliguke";
        } else if (data.isMidWeekFirstDay || data.isMidWeekSecondDay) {
          return "A-uke, midtuke";
        }
        break;
      case "B":
        if (data.isEarlyWeekFirstDay || data.isEarlyWeekSecondDay) {
          return "B-uke, tidliguke";
        } else if (data.isMidWeekFirstDay || data.isMidWeekSecondDay) {
          return "B-uke, midtuke";
        }
        break;
    }
    //Call DataAccessAPI for Distribution Date
    //GetPRSAdminCalendarDayDetail (date)
    return "";
  } else {
    console.error("error : " + status);
  }
};

export function Standardreport(props) {
  const btnReport = useRef();
  const btnClose = useRef();
  const reportType = useRef();
  const chkAddresPoint = useRef();
  const chkAddresPointDiv = useRef();
  const distrDateRef = useRef();
  const emailRef = useRef();
  const fylkeChkBtm = useRef();
  const kommuneChkBtm = useRef();
  const teamChkBtm = useRef();
  const postChkBtm = useRef();
  const budRuteChkBtm = useRef();

  const fylkeChkBtmDiv = useRef();
  const kommuneChkBtmDiv = useRef();
  const teamChkBtmDiv = useRef();
  const postChkBtmDiv = useRef();

  const fylkeChk = useRef();
  const kommuneChk = useRef();
  const teamChk = useRef();
  const postChk = useRef();
  const budrtruteChk = useRef();
  const { showBusiness, setShowBusiness, showHousehold, setShowHousehold } =
    useContext(KSPUContext);
  const { showReservedHouseHolds, setShowReservedHouseHolds } =
    useContext(KSPUContext);
  const { activUtvalg, activUtvalglist } = useContext(KSPUContext);
  const [loading, setLoading] = useState(false);
  const [strDayDetailsState, setStrDayDetailsState] = useState("");
  const cancelReport = () => {
    setLoading(false);
    // btnReport.current.disabled = false;
    // if (props.type == "distr") {
    //   btnReport.current.disabled = true;
    // }
    btnClose.current.click();
  };
  const CreateReport = async () => {
    let level = 0;
    let uptolevel = 0;
    if (props.isList) {
      if (
        emailRef.current.value === "" ||
        emailRef.current.value === undefined
      ) {
        alert("Vennligst fyll inn e-post adresse");
        return;
      }
    }
    if (fylkeChk.current.checked) {
      level = 0;
    } else if (kommuneChk.current.checked) {
      level = 1;
    } else if (teamChk.current.checked) {
      level = 2;
    } else if (postChk.current.checked) {
      level = 3;
    } else if (budrtruteChk.current.checked) {
      level = 4;
    } else {
      level = 0;
    }

    if (fylkeChkBtm.current.checked) {
      uptolevel = 0;
    } else if (kommuneChkBtm.current.checked) {
      uptolevel = 1;
    } else if (teamChkBtm.current.checked) {
      uptolevel = 2;
    } else if (postChkBtm.current.checked) {
      uptolevel = 3;
    } else if (budRuteChkBtm.current.checked) {
      uptolevel = 4;
    } else {
      uptolevel = 0;
    }

    let distrDate = props.type === "distr" ? distrDateRef.current.value : "";

    //Check for distrDate By calling API TODO and act accordingly

    //TODO : Check for Addresspoints in Session , if present any the implement the logic to pass those to reports

    setLoading(true);
    btnReport.current.disabled = true;
    var includeAddressPoint = false;
    if (chkAddresPoint.current.checked === true) {
      includeAddressPoint = true;
    }

    if (props.isList) {
      await genReportUtvalg(
        activUtvalglist,
        distrDate,
        strDayDetailsState,
        false,
        showHousehold,
        showBusiness,
        showReservedHouseHolds,
        level,
        reportType.current.value,
        uptolevel,
        props.isList,
        postChk.current.checked,
        emailRef.current.value,
        includeAddressPoint
      ); // getImageMap();
    } else {
      await genReportUtvalg(
        activUtvalg,
        distrDate,
        strDayDetailsState,
        false,
        showHousehold,
        showBusiness,
        showReservedHouseHolds,
        level,
        reportType.current.value,
        uptolevel,
        props.isList,
        postChk.current.checked,
        "",
        includeAddressPoint
      ); // getImageMap();
    }
    btnReport.current.disabled = false;
    setLoading(false);
    btnClose.current.click();
  };
  const handleEmailChange = async () => {};
  const handleDateChange = async () => {
    let distrDate = props.type === "distr" ? distrDateRef.current.value : "";
    var date_regex = /^(0[1-9]|[1-2][0-9]|3[0-1]).(0[1-9]|1[0-2]).[0-9]{4}$/;
    let strDayDetails = "";
    if (distrDate !== "" && date_regex.test(distrDate) === true) {
      strDayDetails = await getPRSDateDetails(distrDate);

      if (strDayDetails === "") {
        alert("Frekvensdetaljer er ikke til stede i databasen");
      } else {
        btnReport.current.disabled = false;
        setStrDayDetailsState(strDayDetails);
        
      }
    } else {
      btnReport.current.disabled = true;
    }
  };

  const handleChange = (e) => {
    fylkeChk.current.checked = false;
    kommuneChk.current.checked = false;
    teamChk.current.checked = false;
    postChk.current.checked = false;
    budrtruteChk.current.checked = false;

    switch (e.target.id) {
      case "Fylke":
        fylkeChk.current.checked = true;
        postChkBtmDiv.current.style.display = "none";
        fylkeChkBtmDiv.current.style.display = "block";
        kommuneChkBtmDiv.current.style.display = "block";
        teamChkBtmDiv.current.style.display = "block";
        break;
      case "Kommune":
        kommuneChk.current.checked = "true";
        fylkeChkBtmDiv.current.style.display = "none";
        postChkBtmDiv.current.style.display = "none";
        kommuneChkBtmDiv.current.style.display = "block";
        teamChkBtmDiv.current.style.display = "block";
        break;
      case "Team":
        teamChk.current.checked = true;
        fylkeChkBtmDiv.current.style.display = "none";
        kommuneChkBtmDiv.current.style.display = "none";
        postChkBtmDiv.current.style.display = "none";
        teamChkBtmDiv.current.style.display = "block";
        break;
      case "Postnummer":
        postChk.current.checked = true;
        fylkeChkBtmDiv.current.style.display = "none";
        kommuneChkBtmDiv.current.style.display = "none";
        teamChkBtmDiv.current.style.display = "none";
        postChkBtmDiv.current.style.display = "block";
        break;
      case "Budrute":
        budrtruteChk.current.checked = true;
        postChkBtmDiv.current.style.display = "none";
        fylkeChkBtmDiv.current.style.display = "none";
        kommuneChkBtmDiv.current.style.display = "none";
        teamChkBtmDiv.current.style.display = "none";
        break;
    }
  };

  const handleChangeBtm = (e) => {
    fylkeChkBtm.current.checked = false;
    kommuneChkBtm.current.checked = false;
    teamChkBtm.current.checked = false;
    postChkBtm.current.checked = false;
    budRuteChkBtm.current.checked = false;

    switch (e.target.id) {
      case "FylkeB":
        fylkeChkBtm.current.checked = true;
        break;
      case "KommuneB":
        kommuneChkBtm.current.checked = "true";
        break;
      case "TeamB":
        teamChkBtm.current.checked = true;
        break;
      case "PostnummerB":
        postChkBtm.current.checked = true;
        break;
      case "BudruteB":
        budRuteChkBtm.current.checked = true;
        break;
    }
  };

  useEffect(() => {
    if (
      sessionStorage.getItem("addressPoints") === undefined ||
      sessionStorage.getItem("addressPoints") === null ||
      sessionStorage.getItem("addressPoints") === ""
    ) {
      chkAddresPointDiv.current.style.display = "none";
    }
    fylkeChk.current.checked = true;
    kommuneChk.current.checked = false;
    teamChk.current.checked = false;
    postChk.current.checked = false;
    budrtruteChk.current.checked = false;
    fylkeChkBtm.current.checked = false;
    kommuneChkBtm.current.checked = false;
    teamChkBtm.current.checked = false;
    postChkBtm.current.checked = false;
    budRuteChkBtm.current.checked = true;
    postChkBtmDiv.current.style.display = "none";
    if (props.type === "distr") {
      btnReport.current.disabled = true;
      chkAddresPointDiv.current.style.display = "none";
    }
  }, []);

  return (
    <div
      className="modal fade bd-example-modal-lg"
      id={props.id}
      role="dialog"
      aria-labelledby="exampleModalCenterTitle"
      aria-hidden="true"
    >
      <div
        className="modal-dialog modal-dialog-centered viewDetail"
        role="document"
      >
        <div className="modal-content">
          <div className="Common-modal-header">
            <span
              className="common-modal-title pt-2 pl-2"
              id="exampleModalLongTitle"
            >
              RAPPORTUTSKRIFT
            </span>
            <button
              type="button"
              className="close"
              data-dismiss="modal"
              aria-label="Close"
              ref={btnClose}
            >
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div className="View_modal-body pl-2">
            {props.type === "distr" ? (
              <p>
                <label className="mr-2 boldText">
                  Første dag i intervall :{" "}
                </label>
                <input
                  type="textbox"
                  id="distrDate"
                  className="distrDate"
                  onChange={handleDateChange}
                  placeholder="dd.mm.åååå"
                  ref={distrDateRef}
                ></input>
              </p>
            ) : null}
            <p className=" boldText">Velg høyeste nivå rapporten skal vise</p>
            <div className="filterContent">
              <div>
                <input
                  className="mt-1"
                  type="radio"
                  onChange={handleChange}
                  value={0}
                  id="Fylke"
                  ref={fylkeChk}
                />
                <label
                  className="form-check-label reportLabel ml-2"
                  htmlFor="Fylke"
                >
                  {" "}
                  Fylke{" "}
                </label>
              </div>
              <div>
                <input
                  className="mt-1"
                  type="radio"
                  onChange={handleChange}
                  value={1}
                  id="Kommune"
                  ref={kommuneChk}
                />
                <label
                  className="form-check-label reportLabel ml-2"
                  htmlFor="Kommune"
                >
                  {" "}
                  Kommune{" "}
                </label>
              </div>
              <div>
                <input
                  className="mt-1"
                  type="radio"
                  onChange={handleChange}
                  value={2}
                  id="Team"
                  ref={teamChk}
                />
                <label
                  className="form-check-label reportLabel ml-2"
                  htmlFor="Team"
                >
                  {" "}
                  Team{" "}
                </label>
              </div>
              <div>
                <input
                  className="mt-1"
                  type="radio"
                  onChange={handleChange}
                  value={3}
                  id="Postnummer"
                  ref={postChk}
                />
                <label
                  className="form-check-label reportLabel ml-2"
                  htmlFor="Postnummer"
                >
                  {" "}
                  Postnummer{" "}
                </label>
              </div>
              <div>
                <input
                  className="mt-1"
                  type="radio"
                  onChange={handleChange}
                  value={4}
                  id="Budrute"
                  ref={budrtruteChk}
                />
                <label
                  className="form-check-label reportLabel ml-2"
                  htmlFor="Budrute"
                >
                  {" "}
                  Budrute{" "}
                </label>
              </div>
            </div>
            <hr></hr>
            <p className=" boldText">Velg laveste nivå rapporten skal vise</p>
            <div className="filterContent">
              <div ref={fylkeChkBtmDiv}>
                <input
                  className="mt-1"
                  type="radio"
                  onChange={handleChangeBtm}
                  value={0}
                  id="FylkeB"
                  ref={fylkeChkBtm}
                />
                <label
                  className="form-check-label reportLabel ml-2"
                  htmlFor="FylkeB"
                >
                  {" "}
                  Fylke{" "}
                </label>
              </div>
              <div ref={kommuneChkBtmDiv}>
                <input
                  className="mt-1"
                  type="radio"
                  onChange={handleChangeBtm}
                  value={1}
                  id="KommuneB"
                  ref={kommuneChkBtm}
                />
                <label
                  className="form-check-label reportLabel ml-2"
                  htmlFor="KommuneB"
                >
                  {" "}
                  Kommune{" "}
                </label>
              </div>
              <div ref={teamChkBtmDiv}>
                <input
                  className="mt-1"
                  type="radio"
                  onChange={handleChangeBtm}
                  value={2}
                  id="TeamB"
                  ref={teamChkBtm}
                />
                <label
                  className="form-check-label reportLabel ml-2"
                  htmlFor="TeamB"
                >
                  {" "}
                  Team{" "}
                </label>
              </div>
              <div ref={postChkBtmDiv}>
                <input
                  className="mt-1"
                  type="radio"
                  onChange={handleChangeBtm}
                  value={3}
                  id="PostnummerB"
                  ref={postChkBtm}
                />
                <label
                  className="form-check-label reportLabel ml-2"
                  htmlFor="PostnummerB"
                >
                  {" "}
                  Postnummer{" "}
                </label>
              </div>
              <div>
                <input
                  className="mt-1"
                  type="radio"
                  onChange={handleChangeBtm}
                  value={4}
                  id="BudruteB"
                  ref={budRuteChkBtm}
                />
                <label
                  className="form-check-label reportLabel ml-2"
                  htmlFor="BudruteB"
                >
                  {" "}
                  Budrute{" "}
                </label>
              </div>
              <div ref={chkAddresPointDiv}>
                <input
                  className="mt-1"
                  type="checkbox"
                  value={4}
                  id="Adressepunkt"
                  ref={chkAddresPoint}
                />
                <label
                  className="form-check-label reportLabel ml-2"
                  htmlFor="Adressepunkt"
                >
                  {" "}
                  Adressepunkt{" "}
                </label>
              </div>
            </div>
            <br></br>
            <hr></hr>
            <label className="reportLabel ml-2"> Rapportformat </label>

            {props.isList ? (
              <select name="reportType" ref={reportType} className="reportType">
                <option key="pdf" value="pdf">
                  PDF
                </option>
                <option key="excel" value="excel">
                  Excel
                </option>
                <option key="excelDataOnly" value="excelDataOnly">
                  Excel Data Only
                </option>
              </select>
            ) : (
              <select name="reportType" ref={reportType} className="reportType">
                <option key="pdf" value="pdf">
                  PDF
                </option>
                <option key="excel" value="excel">
                  Excel
                </option>
                <option key="excelDataOnly" value="excelDataOnly">
                  Excel Data Only
                </option>
              </select>
            )}

            {props.isList ? (
              <p>
                <label className="reportLabel ml-2 mr-3">Epost </label>
                <input
                  type="textbox"
                  id="email"
                  className="distrDate ml-5"
                  onChange={handleEmailChange}
                  ref={emailRef}
                ></input>
              </p>
            ) : null}
            <br></br>
            <label
              className="progressText"
              style={{ display: !loading ? "none" : "block" }}
            >
              Takk for forespørselen. Rapporten vil bli lastet ned snart.
            </label>
            <br></br>
            <button
              type="button"
              className="KSPU_button "
              onClick={cancelReport}
            >
              Avbryt
            </button>
            <img
              className="loadingImage"
              src={loadingImage}
              style={{ display: !loading ? "none" : "block" }}
            />
            <button
              type="button"
              className="KSPU_button lagrepport"
              ref={btnReport}
              onClick={CreateReport}
            >
              Lag rapport
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}