// import "./standardreportKw.scss";
import React, { useState, useContext, useEffect, useRef } from "react";
import { KundeWebContext, MainPageContext } from "../../context/Context";
import loadingImage from "../../assets/images/callbackActivityIndicator.gif";
import api from "../../services/api.js";
import "./lestill_Click_Component-kw.scss";

function LestillClickKw(props) {
  const fylkeChkBtm = useRef();
  const kommuneChkBtm = useRef();
  const teamChkBtm = useRef();
  const postChkBtm = useRef();
  const budRuteChkBtm = useRef();

  const fylkeChkBtmDiv = useRef();
  const kommuneChkBtmDiv = useRef();
  const teamChkBtmDiv = useRef();

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
  return (
    <div
      className="modal fade bd-example-modal-lg"
      data-backdrop="false"
      id={props.id}
      tabIndex="-1"
      role="dialog"
      aria-labelledby="exampleModalCenterTitle"
      aria-hidden="true"
    >
      <div
        className="modal-dialog modal-dialog-centered viewDetail"
        role="document"
      >
        <div className="modal-content" style={{ border: "black 4px solid" }}>
          <div className="Common-modal-header">
            <span className="dialog-kw" id="exampleModalLongTitle">
              LAGRE UTVALGSLISTE{" "}
            </span>
            <button
              type="button"
              className="close"
              data-dismiss="modal"
              aria-label="Close"
              // ref={btnClose}
            >
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div className="View_modal-body pl-2">
            {/* {displayMsg ? (
        <span className=" sok-Alert-text pl-1">{errormsg}</span>
      ) : null} */}
            <table className="lestill">
              <tbody>
                <tr>
                  <td>
                    <span
                      id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxNameLabel"
                      className="SaveUtvaldivLabelText pl-2"
                    >
                      Listenavn
                    </span>
                  </td>
                  <td>
                    <input
                      // ref={FirstInputText}
                      name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxName"
                      type="text"
                      id="utvalgnavn"
                      onChange={""}
                      className="selection-input ml-1"
                      placeholder=""
                    />
                  </td>
                </tr>
                <tr>
                  <td>
                    <span className="SaveUtvaldivLabelText ml-1">
                      De nye utvalgene skal ha navn som ender med:
                    </span>
                  </td>
                </tr>
                <tr>
                  <td ref={fylkeChkBtmDiv}>
                    <input
                      className="mt-1 ml-1"
                      type="radio"
                      onChange={handleChangeBtm}
                      value={0}
                      id="FylkeB"
                      ref={fylkeChkBtm}
                    />
                    <label
                      className="form-check-label reportLabel"
                      htmlFor="FylkeB"
                    >
                      {" "}
                      Dagens dato{" "}
                    </label>
                  </td>
                </tr>
                <tr>
                  <td ref={kommuneChkBtmDiv}>
                    <input
                      className="mt-1 ml-1"
                      type="radio"
                      onChange={handleChangeBtm}
                      value={1}
                      id="KommuneB"
                      ref={kommuneChkBtm}
                    />
                    <label
                      className="form-check-label reportLabel"
                      htmlFor="KommuneB"
                    >
                      {" "}
                      Dagens dato og klokkeslett{" "}
                    </label>
                  </td>
                </tr>
                <tr>
                  <td ref={teamChkBtmDiv}>
                    <input
                      className="mt-1 ml-1"
                      type="radio"
                      onChange={handleChangeBtm}
                      value={2}
                      id="TeamB"
                      ref={teamChkBtm}
                    />
                    <label className="form-check-label reportLabel" htmlFor="TeamB">
                      {" "}
                      Egendefinert tekst{" "}
                    </label>
                  </td>
                </tr>

                <tr>
                  <td>
                    <span
                      id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxLogoLabel"
                      className="SaveUtvaldivLabelText pl-2"
                    >
                      Egendefinert tekst
                    </span>
                  </td>
                  <td>
                    <input
                      // ref={ThirdInputText}
                      className="selection-input ml-1"
                      name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxLogo"
                      type="text"
                      id="uxLogo"
                      onChange={""}
                    />
                  </td>
                </tr>
                <br />
                <tr>
                  <td>
                    <button
                      type="button"
                      className="KSPU_button_Gray"
                      data-dismiss="modal"
                    >
                      Avbryt
                    </button>
                  </td>
                  <td></td>
                  <td>
                    <button type="button" onClick={""} className="KSPU_button-kw">
                      Lagre
                    </button>
                  </td>
                </tr>
                <br />
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
}

export default LestillClickKw;
