import React, { useState, useContext, useEffect } from "react";
import "../App.css";
// import { KundeWebContext } from "../context/Context.js";

function AdressepunktModel(props) {
  const [displayMsg, setDisplayMsg] = useState(false);
  const [selectedvalue, setselectedvalue] = useState("");
  //   const [gateValue, setGateValue] = useState(KundeWebContext);

  const dropdownselection = (e) => {
    setselectedvalue(e.target.value);
  };
  const buderuteVelg = () => {};

  return (
    <div>
      {/* <!-- Modal --> */}
      <div
        className="modal fade bd-example-modal-lg"
        id={props.id}
        tabIndex="-1"
        role="dialog"
        aria-labelledby="exampleModalCenterTitle"
        aria-hidden="true"
      >
        <div
          className="modal-dialog modal-dialog-centered budrutemax"
          role="document"
        >
          <div className="modal-content">
            <div className="modal-header budrutetitle">
              <h5 className="modal-titlekw " id="exampleModalLongTitle">
                {props.title}
              </h5>
              <button
                type="button"
                className="close"
                data-dismiss="modal"
                aria-label="Close"
              >
                <span aria-hidden="true">&times;</span>
              </button>
            </div>
            <div className="modal-body budrutebody">
              {displayMsg ? (
                <div className="budruteMax">
                  <span
                    id="uxKjoreAnalyse_uxLblMessage"
                    className="divErrorText_kw"
                  >
                    Feil ved adressesøk: En feil oppsto ved adressesøk. Kontakt
                    superbruker i Posten.
                  </span>
                </div>
              ) : (
                <div>
                  <div className="">
                    <table className="tableRow">
                      <tbody>
                        <tr className="flykeHeader">
                          <th className="tabledataRow budruteRow">Gate/sted</th>
                          <th className="tabledataRow budruteRow">Husnummer</th>
                          <th className="tabledataRow budruteRow">Score</th>
                          <th className="tabledataRow budruteRow">Type</th>
                          <th className="tabledataRow budruteRow">
                            &nbsp;&nbsp;&nbsp;&nbsp;
                          </th>
                        </tr>
                        <th className="tabledataRow">
                          {" "}
                          <tr>
                            <td className="flykecontent">agder</td>
                          </tr>
                        </th>
                        <th className="tabledataRow">
                          {" "}
                          <tr>
                            <td className="flykecontent">
                              <select
                                id="uxDropDownListUtvalg"
                                style={{ height: "1.5rem" }}
                                className=" p-text"
                                onClick={dropdownselection}
                                title="Begrens søket med"
                              >
                                <option value="1" Selected="True">
                                  1
                                </option>
                                <option value="2">2</option>
                                <option value="3">3</option>
                                <option value="4">4</option>
                                <option value="5">5</option>
                              </select>
                            </td>
                          </tr>
                        </th>
                        <th className="tabledataRow">
                          {" "}
                          <tr>
                            <td className="flykecontent">50</td>
                          </tr>
                        </th>
                        <th className="tabledataRow">
                          {" "}
                          <tr>
                            <td className="flykecontent">Adresse</td>
                          </tr>
                        </th>
                        <th className="tabledataRow">
                          {" "}
                          <tr>
                            <td className="flykecontent">
                              <a
                                id=""
                                href=""
                                className="KSPU_LinkButton float-right mr-1"
                                onClick={buderuteVelg}
                              >
                                velg
                              </a>
                            </td>
                          </tr>
                        </th>
                      </tbody>
                    </table>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default AdressepunktModel;
