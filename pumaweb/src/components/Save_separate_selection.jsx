import React, { useState, useRef, useContext, useEffect } from "react";
import {
  KSPUContext,
  UtvalgContext,
  MainPageContext,
} from "../context/Context.js";
import api from "../services/api.js";
import { v4 as uuidv4 } from "uuid";
import {
  saveUtvalg,
  Utvalg,
  NewUtvalgName,
  criterias,
  getAntall,
} from "./KspuConfig";
import { groupBy } from "../Data";

import swal from "sweetalert";
import $ from "jquery";
import { Buffer } from "buffer";
import ShowCustomer from "./showCustomer.js";
import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import { CreateActiveUtvalg, CurrentDate } from "../common/Functions";

function SaveSeprateUtvalg(props) {
  const { errormsg, seterrormsg } = useContext(KSPUContext);
  const { issave, setissave } = useContext(KSPUContext);
  const { resultData, setResultData } = useContext(KSPUContext);
  const {
    activUtvalg,
    setActivUtvalg,
    setAktivDisplay,
    setDemografieDisplay,
    setSegmenterDisplay,
    setAddresslisteDisplay,
    setGeografiDisplay,
    setKjDisplay,
    setAdresDisplay,
  } = useContext(KSPUContext);

  const [Totalantall, setTotalAntall] = useState(activUtvalg.hush);
  const [displayMsg, setdisplayMsg] = useState(false);
  const [eConnectResult, seteConnectResult] = useState([]);
  const [name, setname] = useState("");
  const [kunde_name, setkunde_name] = useState("");
  // const [kunde_name, setkunde_name] = useState(
  //   "POSTEN OG BRING TESTKUNDE KONSERNINTERN PPOST"
  // );
  const [kunde_number, setkunde_number] = useState("");
  const [logo, set_logo] = useState("");
  const [test, settest] = useState(false);
  const [showmodel, setshowmodel] = useState(false);
  const btnClose = useRef();
  const selectionEndText = useRef();
  const { LargUtvalg, setLargUtvalg } = useContext(KSPUContext);
  const { reolerData, setreolerData } = useContext(KSPUContext);
  const [loading, setloading] = useState(false);

  // const {showBusiness, setShowBusiness} = useContext(KSPUContext);
  const {
    showReservedHouseHolds,
    setShowReservedHouseHolds,
    showHousehold,
    showBusiness,
    setshoworklist,
    showorklist,
  } = useContext(KSPUContext);
  const { addressPoints, setAddressPoints } = useContext(MainPageContext);
  const [locationName, setLocationName] = useState("");
  const [locationNameEnd, setLocationNameEnd] = useState("");
  const [saveResult, setSaveResult] = useState();
  const [finalUtvalg, setFinalUtvalg] = useState(
    JSON.parse(JSON.stringify(props.utvalgArray))
  );

  const EnterName = () => {
    let name_utvalg = document.getElementById("utvalgnavn").value;
    setname(name_utvalg);
    setdisplayMsg(false);
  };
  const EnterKundeNumber = () => {
    let kundeNumber = document.getElementById("uxKunde").value;
    setkunde_name(kundeNumber);
    setkunde_number(kundeNumber);
  };
  const enterLocation = (key, value) => {
    setdisplayMsg(false);
    finalUtvalg.map((item, index) => {
      if (key === index) {
        item.name = value;
      }
    });
    setFinalUtvalg([...finalUtvalg]);
  };
  const Enterlogo = (key, value) => {
    finalUtvalg.map((item, index) => {
      if (key === index) {
        item.logo = value;
      }
    });
    setFinalUtvalg([...finalUtvalg]);
  };
  const EnterNameEnd = () => {
    setdisplayMsg(false);
    let locEnd = selectionEndText.current.value;
    // let locEnd = document.getElementById("lcend").value;
    setLocationNameEnd(locEnd);
  };

  const velgCustomer = (e) => {
    // let value = e.target.id;
    // const answer_array = value.split(",");

    // setkunde_number(answer_array[1]);
    // setkunde_name(answer_array[0]);
    setkunde_number(Number(e.target.attributes["customerid"].value));
    setkunde_name(e.target.attributes["customername"].value);
  };
  const handleKeypress = (e) => {
    //it triggers by pressing the enter key
    if (e.key === "Enter") {
      FinSaveUtvalg();
    }
  };
  const FinSaveUtvalg = async () => {
    window.$("#showCustomer123").modal("show");
    let uniqueId = uuidv4();
    let eConnectUrl = `ECPuma/FindCustomer380`;
    setloading(true);
    //setshowmodel(true);
    if (kunde_name) {
      let eConnectHeader = {
        Header: {
          SystemCode: "Analytiker",
          MessageId: uniqueId,
          SecurityToken: null,
          UserName: null,
          Version: null,
          Timestamp: null,
        },
        Aktornummer: null,
        Kundenummer: null,
        Navn: null,
        MaksRader: "100",
      };
      if (!isNaN(+kunde_name)) {
        eConnectHeader = {
          Header: {
            SystemCode: "Analytiker",
            MessageId: uniqueId,
            SecurityToken: null,
            UserName: null,
            Version: null,
            Timestamp: null,
          },
          Aktornummer: null,
          Kundenummer: kunde_name,
          Navn: null,
          MaksRader: "100",
        };
      } else {
        eConnectHeader = {
          Header: {
            SystemCode: "Analytiker",
            MessageId: uniqueId,
            SecurityToken: null,
            UserName: null,
            Version: null,
            Timestamp: null,
          },
          Aktornummer: null,
          Kundenummer: null,
          Navn: kunde_name,
          MaksRader: "100",
        };
      }

      try {
        const { data, status } = await api.postdata(
          eConnectUrl,
          eConnectHeader
        );
        if (data.length == 0) {
          seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
          setdisplayMsg(true);
          setloading(false);
        } else {
          setdisplayMsg(false);

          await seteConnectResult(data);

          setshowmodel(true);
          setloading(false);
        }
      } catch (error) {
        console.error("error : " + error);
        seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
        setdisplayMsg(true);
        setloading(false);
      }
    }
  };

  const uxSaveUtvalg = async (event) => {
    setdisplayMsg(false);
    //event.preventDefault();
    let flag = 0;
    let saveUtvalgFlag = 0;
    let queryRequest = [];
    finalUtvalg.map((item) => {
      if (item.name == "" || item.name.trim().length < 3) {
        setdisplayMsg(true);
        seterrormsg(
          "Alle navn må ha mer enn 3 tegn. Følgende utvalgsnavn må endres:"
        );
        flag = 1;
      } else if (item.name.trim().length > 50) {
        setdisplayMsg(true);
        let msg = `Beskrivelse av utvalget ${item.name} må ha mindre enn 50 tegn.`;
        seterrormsg(msg);
        flag = 1;
      } else if (item?.totalAntall === 0) {
        setdisplayMsg(true);
        let msg = `Utvalget ${item.name} har ingen mottakere og kan derfor ikke lagres. Kontroller at utvalget inneholder budruter og at minst en mottakergruppe er valgt.`;
        seterrormsg(msg);
        flag = 1;
      } else {
        queryRequest.push(item.name + locationNameEnd);
      }
    });
    if (flag === 0) {
      try {
        document.getElementById("btnSaveSeparateUtvalg").disabled = true;
        let url = `Utvalg/UtvalgNamesExists?utvalgNames`;
        const { data, status } = await api.postdata(url, queryRequest);
        if (status === 200) {
          if (data === true) {
            saveUtvalgFlag = 1;
            let msg = `Utvalget name eksisterer allerede. Velg et annet utvalgsnavn.`;
            seterrormsg(msg);
            setdisplayMsg(true);
            document.getElementById("btnSaveSeparateUtvalg").disabled = false;
          } else {
            setdisplayMsg(false);
          }
        } else {
          saveUtvalgFlag = 1;
          let msg = `Utvalget name eksisterer allerede. Velg et annet utvalgsnavn.`;
          seterrormsg(msg);
          setdisplayMsg(true);
          document.getElementById("btnSaveSeparateUtvalg").disabled = false;
        }
      } catch (error) {
        saveUtvalgFlag = 1;
        console.error("error : " + error);
        seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
        setdisplayMsg(true);
        document.getElementById("btnSaveSeparateUtvalg").disabled = false;
      }
    }
    if (saveUtvalgFlag === 0) {
      saveUtvalgArray();
    }
  };

  const saveUtvalgArray = async (event) => {
    let saveOldReoler = "false";
    let skipHistory = "false";
    let forceUtvalgListId = 0;
    let url = `Utvalg/SaveUtvalgs?userName=Internbruker&`;
    url = url + `saveOldReoler=${saveOldReoler}&`;
    url = url + `skipHistory=${skipHistory}&`;

    url = url + `forceUtvalgListId=${forceUtvalgListId}`;

    try {
      setloading(true);

      finalUtvalg.map((item, index) => {
        item.modifications.push({
          modificationId: Math.floor(100000 + Math.random() * 900000),
          userId: "Internbruker",
          modificationTime: CurrentDate(),
          listId: 0,
        });

        item.kundeNummer = kunde_number;
        item.name = item.name + locationNameEnd;
      });
      let queryRequest = {
        utvalgs: finalUtvalg,
      };

      const { data, status } = await api.postdata(url, queryRequest);

      if (status === 200) {
        var _flag = 0;
        // for (let i = 0; i < finalUtvalg.length; i++) {
        //   for (let j = 0; j < data.length; j++) {
        //     if (data[j].utvalg != null) {
        //       if (finalUtvalg[i].name === data[j].utvalg.name) {
        //         finalUtvalg[i].utvalgId = data[j].utvalg.utvalgId;
        //         finalUtvalg[i].logo = data[j].utvalg.logo;
        //         break;
        //       }
        //     } else {
        //       _flag = 1;
        //       seterrormsg("Something went wrong");
        //       setdisplayMsg(true);
        //     }
        //   }
        // }

        if (_flag === 0) {
          btnClose.current.click();
          if (showorklist.length > 0) {
            for (let j = 0; j < data.length; j++) {
              if (data[j].utvalg != null) {
                showorklist.push(data[j].utvalg);
              }
            }
            setshoworklist([...showorklist]);
          } else {
            let newArrayForWorkList = [];
            for (let j = 0; j < data.length; j++) {
              if (data[j].utvalg != null) {
                newArrayForWorkList.push(data[j].utvalg);
              }
            }
            setshoworklist(newArrayForWorkList);
          }

          //showorklist.push(a);
          //await setshoworklist(showorklist);
          // activUtvalg.name = name;
          // activUtvalg.utvalgId = utvalgID;

          // activUtvalg.logo = logo;
          // activUtvalg.kundeNummer = kunde_number;
          setissave(true);
          settest(true);
          //await setActivUtvalg({});
          if (data[0].utvalg !== null) {
            let obj = await CreateActiveUtvalg(data[0].utvalg);
            await setResultData(
              await groupBy(
                data[0].utvalg.reoler,
                "",
                0,
                showHousehold,
                showBusiness,
                showReservedHouseHolds,
                []
              )
            );
            await setActivUtvalg(obj);
          }

          document.getElementById("btnSaveSeparateUtvalg").disabled = false;
          btnClose.current.click();

          setloading(false);
          setAktivDisplay(true);
          setDemografieDisplay(false);
          setSegmenterDisplay(false);
          setAddresslisteDisplay(false);
          setGeografiDisplay(false);
          setKjDisplay(false);
          setAdresDisplay(false);
        }
      } else {
        seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
        setdisplayMsg(true);
        setissave(false);
        setloading(false);
        document.getElementById("btnSaveSeparateUtvalg").disabled = false;
      }
    } catch (error) {
      console.error("error : " + error);
      seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
      setdisplayMsg(true);
      setissave(false);
      setloading(false);
      document.getElementById("btnSaveSeparateUtvalg").disabled = false;
    }
  };
  const renderPerson = (result, index) => {
    return eConnectResult.kundedata.map((item) => (
      <tr key={index}>
        <th className="tabledataRow">
          {" "}
          <tr>
            <td className="flykecontent">
              <tr>
                <td className="flykecontent">{item.juridisknavn}</td>
              </tr>
            </td>
          </tr>
        </th>

        <th className="tabledataRow">
          {" "}
          <tr>
            <td className="flykecontent">
              <tr>
                <td className="flykecontent">{item.kundenummer}</td>
              </tr>
            </td>
          </tr>
        </th>

        <th className="tabledataRow">
          {" "}
          <tr>
            <td className="flykecontent">
              <p
                id={item.juridisknavn + "," + item.kundenummer}
                customername={item.juridisknavn}
                customerid={item.kundenummer}
                data-dismiss="modal"
                className="KSPU_LinkButton float-right mr-1"
                onClick={velgCustomer}
              >
                velg
              </p>
            </td>
          </tr>
        </th>
      </tr>
    ));
  };

  return (
    <div>
      <div>
        <div
          className="modal fade bd-example-modal-lg"
          data-backdrop="false"
          id="showCustomer123"
          tabIndex="-1"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
          style={{ zIndex: "1051" }}
          aria-hidden="true"
        >
          <div
            className="modal-dialog modal-dialog-centered viewDetail"
            role="document"
          >
            <div
              className="modal-content scrollvertical"
              style={{ border: "black 3px solid" }}
            >
              <div className="Common-modal-header">
                <span
                  className="common-modal-title pt-1 pl-2"
                  id="exampleModalLongTitle"
                >
                  SØKERESULTAT
                </span>
                <button
                  type="button"
                  className="close"
                  data-dismiss="modal"
                  aria-label="Close"
                  //ref={btnClose}
                >
                  <span aria-hidden="true">&times;</span>
                </button>
              </div>
              {loading ? (
                <img
                  src={loadingImage}
                  style={{
                    width: "20px",
                    height: "20px",
                    position: "absolute",
                    left: "210px",
                    zindex: 100,
                  }}
                />
              ) : (
                <div className="View_modal-body budrutebody">
                  {displayMsg ? (
                    <span className=" sok-Alert-text pl-1">{errormsg}</span>
                  ) : null}
                  <table className="tableRow">
                    <thead>
                      <tr className="flykeHeader">
                        <th className="tabledataRow budruteRow">Kundenavn</th>
                        <th className="tabledataRow budruteRow">Kundenummer</th>
                        <th className="tabledataRow budruteRow">
                          &nbsp;&nbsp;&nbsp;&nbsp;
                        </th>
                      </tr>
                    </thead>
                    <tbody>{showmodel ? renderPerson() : null}</tbody>
                  </table>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
      {/* starts here */}
      <div
        className="modal fade bd-example-modal-lg"
        data-backdrop="false"
        id={props.id}
        tabIndex="-1"
        role="dialog"
        aria-labelledby="exampleModalCenterTitle"
        aria-hidden="true"
        style={{ width: "2200px" }}
      >
        <div
          className="modal-dialog modal-dialog-centered viewDetail"
          role="document"
          style={{ margin: "0px 20px 0px 340px" }}
        >
          <div
            className="modal-content scrollvertical"
            style={{ border: "black 3px solid", width: "2200px" }}
          >
            <div className="Common-modal-header">
              <span
                className="common-modal-title pt-1 pl-2"
                id="exampleModalLongTitle"
              >
                LAGRE UTVALG
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
              {displayMsg ? (
                <span className=" sok-Alert-text pl-1">{errormsg}</span>
              ) : null}
              <table>
                <tbody>
                  <tr>
                    <td>
                      <span
                        id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxKundeLabel"
                        className="SaveUtvaldivLabelText pl-2"
                      >
                        Kundenummer/-navn:
                      </span>
                    </td>
                    <td>
                      <input
                        // ref={SecondInputText}
                        value={showmodel ? kunde_number : null}
                        name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxKunde"
                        type="text"
                        id="uxKunde"
                        className="selection-input ml-1"
                        onChange={EnterKundeNumber}
                        placeholder={kunde_number}
                        onKeyPress={handleKeypress}
                      />
                    </td>
                    <td className="pl-2">
                      <input
                        type="submit"
                        className="KSPU_button"
                        name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxFindKunde"
                        value="Finn"
                        data-toggle="modal"
                        data-target="#showCustomer123"
                        onClick={FinSaveUtvalg}
                        id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxFindKunde"
                        disabled={kunde_number.length < 3 ? true : false}
                      />
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span
                        id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxNameLabel"
                        className="SaveUtvaldivLabelText pl-2"
                      >
                        Hvert utvalgsnavn ender med:
                      </span>
                    </td>
                    <td>
                      <input
                        ref={selectionEndText}
                        name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxName"
                        type="text"
                        id="lcend"
                        onChange={EnterNameEnd}
                        className="selection-input ml-1"
                        placeholder=""
                      />
                    </td>
                  </tr>

                  <tr>
                    <th colSpan={2}>
                      <span
                        id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxKundeLabel"
                        className="SaveUtvaldivLabelText pl-2"
                      >
                        Navn på adressepunkt Utvalgsnavn
                      </span>
                    </th>
                    <th>
                      <span
                        id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxKundeLabel"
                        className="SaveUtvaldivLabelText pl-2"
                      >
                        Forhandlerpåtrykk
                      </span>
                    </th>
                  </tr>

                  {finalUtvalg.map((it, index) => {
                    return (
                      <tr>
                        <td>
                          <label className="form-check-label pl-2">
                            {index + 1}. {it.name}
                          </label>
                        </td>
                        <td>
                          <input
                            name=""
                            type="text"
                            id="addrpoint"
                            className="selection-input ml-1"
                            placeholder=""
                            maxLength="50"
                            defaultValue={it.name}
                            onChange={(e) => {
                              enterLocation(index, e.target.value);
                            }}
                          />
                        </td>
                        <td>
                          <input
                            name=""
                            type="text"
                            id=""
                            defaultValue={it.logo}
                            className="selection-input ml-1"
                            placeholder=""
                            onChange={(e) => {
                              Enterlogo(index, e.target.value);
                            }}
                          />
                        </td>
                      </tr>
                    );
                  })}

                  <br />
                  <tr>
                    <td>
                      <button
                        type="button"
                        className="btn KSPU_button"
                        data-dismiss="modal"
                      >
                        Avbryt
                      </button>
                    </td>

                    <td></td>
                    <td>
                      <button
                        type="button"
                        id="btnSaveSeparateUtvalg"
                        onClick={uxSaveUtvalg}
                        className="btn KSPU_button"
                      >
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
    </div>
  );
}

export default SaveSeprateUtvalg;
