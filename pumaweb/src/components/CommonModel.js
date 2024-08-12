import React, { useState, useRef, useContext } from "react";
import { KSPUContext, UtvalgContext } from "../context/Context.js";
import ComponentHistorydata from "./datadef/Historydata";
import api from "../services/api.js";
import { getCriteriaText } from "./KspuConfig";
function History(props) {
  const [btnDisabled, setBtnDisabled] = useState(true);
  const { activUtvalg, setAktivDisplay } = useContext(KSPUContext);
  const [logoName, setLogoName] = useState(
    activUtvalg.logo ? activUtvalg.logo : ""
  );
  const [loading, setloading] = useState(false);
  const FirstInputText = useRef();
  const btnClose = useRef();
  const textInput = (e) => {
    if (FirstInputText.current.value) {
      setLogoName(FirstInputText.current.value);
      setBtnDisabled(false);
    } else {
      setBtnDisabled(true);
      setLogoName(" ");
    }
  };

  const saveListLogo = async () => {
    let url = "";
    setloading(true);
    url = url + `Utvalg/UpdateLogo?userName=Internbruker&`;

    url = url + `utvalgId=${props.data}&logo=${encodeURIComponent(logoName)}`;

    try {
      const { data, status } = await api.putData(url);
      if (data.length === 0) {
        setloading(false);
      } else {
        btnClose.current.click();
        document.getElementById("selectionLogo").innerHTML = logoName;
        activUtvalg.logo = logoName;
        setAktivDisplay(true);
      }
    } catch (error) {
      console.log("errorpage API not working");
      console.error("error : " + error);

      setloading(false);
    }
  };
  return (
    <div>
      <div
        className="modal fade bd-example-modal-lg"
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
          <div className="modal-content">
            <div className="Common-modal-header">
              <span
                className="common-modal-title pt-2 pl-1"
                id="exampleModalLongTitle"
              >
                {props.title}
              </span>
              <button
                type="button"
                className="close"
                data-dismiss="modal"
                ref={btnClose}
                aria-label="Close"
                onClick={props.onClose}
              >
                <span aria-hidden="true">&times;</span>
              </button>
            </div>
            <div className="View_modal-body pl-2">
              {props.id == "visDetails" ? (
                <table className="CriteriaText pl-3">
                  <thead className="CriteriaText_Header">
                    <tr className="CriteriaText_Header ">
                      <th className="col-1 CriteriaText_Header pl-1" scope="">
                        Kriterietype{" "}
                      </th>

                      <th
                        className="col-2 CriteriaText_Header pl-1"
                        scope="col"
                      >
                        Kriterie
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    {props.data.map((item) => {
                      return (
                        <tr>
                          <th className="CriteriaText pl-1">
                            {getCriteriaText(item.criteriaType)}
                          </th>
                          {item?.criteria?.indexOf("Kommuner") === -1 ? (
                            item?.criteria?.indexOf("Geografiplukkliste") ===
                            -1 ? (
                              <th className="CriteriaText viewCriteriaDetails">
                                {item?.criteria}
                              </th>
                            ) : null
                          ) : (
                            <th className="CriteriaText viewCriteriaDetails">
                              {item?.criteria?.substring(
                                0,
                                item?.criteria?.indexOf("Kommuner")
                              )}
                              <br />
                              {item?.criteria?.substring(
                                item?.criteria?.indexOf("Kommuner")
                              )}
                            </th>
                          )}
                        </tr>
                      );
                    })}
                  </tbody>
                </table>
              ) : null}

              {props.id == "visHistory" ? (
                <table className="CriteriaText pl-3">
                  <thead className="CriteriaText_Header">
                    <tr className="CriteriaText_Header ">
                      <th className="col-1 CriteriaText_Header pl-1" scope="">
                        Dato{" "}
                      </th>

                      <th
                        className="col-2 CriteriaText_Header pl-1"
                        scope="col"
                      >
                        Bruker
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    {props.data.map((item) => (
                      <ComponentHistorydata Item={item} />
                    ))}
                  </tbody>
                </table>
              ) : null}
              {props.id == "visEdit" ? (
                <div>
                  <span className="UtvaldivLabelText_1 pl-2">
                    Forhandlerpåtrykk
                  </span>
                  <input
                    ref={FirstInputText}
                    type="text"
                    className="selection-input ml-1"
                    placeholder=""
                    value={logoName}
                    onChange={textInput}
                  />
                </div>
              ) : null}
              <div className="pt-3 pb-2">
                <button
                  type="button"
                  className="btn KSPU_button mr-2"
                  data-dismiss="modal"
                >
                  Lukk
                </button>
                {props.id == "visEdit" ? (
                  <button
                    type="button"
                    className="btn KSPU_button mr-1 float-right"
                    // disabled={btnDisabled}
                    onClick={saveListLogo}
                  >
                    Lagre
                  </button>
                ) : null}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default History;
