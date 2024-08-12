import React, { useState, useContext, useEffect } from "react";
import { KundeWebContext } from "../context/Context.js";
import "../App.css";
import { NumberFormatKW } from "../common/Functions.js";

function VisDetaljerModal_KW(props, { parentCallback }) {
  const [displayMsg, setDisplayMsg] = useState(false);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  const [visAfterReol, setVisAfterReol] = useState(
    props.ResultAfterCreationObject
  );
  const [newReoler, setNewReoler] = useState(props.ResultAfterRuteCreation);
  const [visRestBeforeReol, setVisRestBeforeReol] = useState(props.oldReoler);

  let EndringAntall =
    (utvalgapiobject.totalAntall / utvalgapiobject.antallBeforeRecreation) *
      100 -
    100;
  EndringAntall = parseFloat(EndringAntall).toFixed(2) + "%";
  EndringAntall = EndringAntall.replace(".", ",");

  let ArealAvvik = parseFloat(utvalgapiobject.arealAvvik).toFixed(2) + "%";
  ArealAvvik = ArealAvvik.replace(".", ",");
  const renderPerson = () => {
    return (
      <>
        {visRestBeforeReol.map((item, i) => (
          <tr key={i}>
            <td className="flykecontent BeforeReolerTable_Normal">
              {item.fylke}
            </td>
            <td className="flykecontent BeforeReolerTable_Normal">
              {item.kommune}
            </td>
            <td className="flykecontent BeforeReolerTable_Normal">
              {item.teamName}
            </td>
            <td className="flykecontent BeforeReolerTable_Normal">
              {item.descriptiveName}
            </td>

            <td className="flykecontent BeforeReolerTable_Normal">
              {item.antall.households}
            </td>

            <td className="flykecontent BeforeReolerTable_Normal">
              {item.antall.businesses}
            </td>

            <td className="flykecontent BeforeReolerTable_Normal">{}</td>
            <td className="flykecontent BeforeReolerTable_Normal">{}</td>
          </tr>
        ))}
      </>
    );
  };
  const renderPersonNew = () => {
    return (
      <>
        {visAfterReol.map((item, i) => (
          <tr key={i}>
            <td className="flykecontent BeforeReolerTable_Normal">
              {item.fylke}
            </td>
            <td className="flykecontent BeforeReolerTable_Normal">
              {item.kommune}
            </td>
            <td className="flykecontent BeforeReolerTable_Normal">
              {item.teamName}
            </td>
            <td className="flykecontent BeforeReolerTable_Normal">
              {item.descriptiveName}
            </td>

            <td className="flykecontent BeforeReolerTable_Normal">{}</td>

            <td className="flykecontent BeforeReolerTable_Normal">{}</td>

            <td className="flykecontent BeforeReolerTable_Normal">
              {item.antall.households}
            </td>
            <td className="flykecontent BeforeReolerTable_Normal">
              {item.antall.businesses}
            </td>
          </tr>
        ))}
      </>
    );
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
          className="modal-dialog modal-dialog-centered modal-dialog-centered-kw resultmaximizer"
          role="document"
        >
          <div className="modal-content">
            <div className="modal-header VisDetailsHeader">
              <h5 className=" visTitle " id="exampleModalLongTitle">
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
            <div className="modal-body flykebody">
              {displayMsg ? (
                <div className="pr-3">
                  <span
                    id="uxKjoreAnalyse_uxLblMessage"
                    className="divErrorText_kw"
                  >
                    Dette utvalget inneholder ingen budruter
                  </span>
                </div>
              ) : (
                <div>
                  <div className="row pb-4 ">
                    <div className="col-6 BeforeReolerTable_Normal">
                      <div>Endring antall :{EndringAntall}</div>
                      <div>Endring areal : {ArealAvvik}</div>
                    </div>
                    <div className="col-2 BeforeReolerTable_Normal">
                      <div>Totalt antall : {}</div>
                    </div>
                    <div className="col-3 ml-4 BeforeReolerTable_Normal">
                      <div className="ml-1">
                        <b>Sist lagret</b> :{" "}
                        {NumberFormatKW(utvalgapiobject.antallBeforeRecreation)}
                      </div>
                      <div>
                        <b className="ml-4">Dagens</b> :{" "}
                        {NumberFormatKW(utvalgapiobject.totalAntall)}
                      </div>
                    </div>
                  </div>
                  <div className="">
                    <table className="tableRow">
                      <tbody>
                        <tr className="flykeHeader">
                          <th className="tabledataRow BeforeReolerTable">
                            Fylke
                          </th>
                          <th className="tabledataRow BeforeReolerTable">
                            Kommune
                          </th>
                          <th className="tabledataRow BeforeReolerTable">
                            Team
                          </th>
                          <th className="tabledataRow BeforeReolerTable">
                            Budrute
                          </th>
                          <th className="tabledataRow BeforeReolerTable">
                            Ant HH
                          </th>
                          <th className="tabledataRow BeforeReolerTable">
                            Ant V
                          </th>
                          <th className="tabledataRow BeforeReolerTable">
                            Ant HH
                          </th>
                          <th className="tabledataRow BeforeReolerTable">
                            Ant V
                          </th>
                        </tr>
                        {renderPersonNew()}
                        {renderPerson()}

                        {newReoler.map((item, i) => (
                          <tr key={i}>
                            <td className="flykecontent BeforeReolerTable_Normal">
                              {item.fylke}
                            </td>
                            <td className="flykecontent BeforeReolerTable_Normal">
                              {item.kommune}
                            </td>
                            <td className="flykecontent BeforeReolerTable_Normal">
                              {item.teamName}
                            </td>
                            <td className="flykecontent BeforeReolerTable_Normal">
                              {item.descriptiveName}
                            </td>

                            <td className="flykecontent BeforeReolerTable_Normal">
                              {item?.householdsOld}
                            </td>

                            <td className="flykecontent BeforeReolerTable_Normal">
                              {item?.businessesOld}
                            </td>

                            <td className="flykecontent BeforeReolerTable_Normal">
                              {item.antall.households}
                            </td>
                            <td className="flykecontent BeforeReolerTable_Normal">
                              {item.antall.businesses}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                  <div className="row flykebody">
                    <div className="col-6 pt-3">
                      <button
                        type="button"
                        className="btn btn-default KSPU_LinkButton float-right"
                        id="maksimer_id"
                        data-dismiss="modal"
                      >
                        Lukk
                      </button>
                    </div>
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

export default VisDetaljerModal_KW;
