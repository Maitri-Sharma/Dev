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
import { FetchData } from "../Data";
import swal from "sweetalert";
import $ from "jquery";
import { Buffer } from "buffer";
import ShowCustomer from "./showCustomer.js";
import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import { CurrentDate } from "../common/Functions";

function SaveCampaign(props) {
  const { errormsg, seterrormsg } = useContext(KSPUContext);
  const { issave, setissave } = useContext(KSPUContext);
  const { resultData, setResultData } = useContext(KSPUContext);
  const { mapView } = useContext(MainPageContext);
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
  const [kunde_name, setkunde_name] = useState(
    activUtvalg.utvalgId ? activUtvalg.kundeNummer : null
  );
  const [kunde_number, setkunde_number] = useState(
    activUtvalg.utvalgId ? activUtvalg.kundeNummer : null
  );
  const [logo, set_logo] = useState("");
  const [test, settest] = useState(false);
  const [showmodel, setshowmodel] = useState(false);
  const btnClose = useRef();
  const { LargUtvalg, setLargUtvalg } = useContext(KSPUContext);
  const { reolerData, setreolerData } = useContext(KSPUContext);
  const [loading, setloading] = useState(false);
  const {
    showReservedHouseHolds,
    setShowReservedHouseHolds,
    setshoworklist,
    showorklist,
  } = useContext(KSPUContext);
  const FirstInputText = useRef();
  const SecondInputText = useRef();
  const ThirdInputText = useRef();

  const EnterName = () => {
    let name_utvalg = document.getElementById("utvalgnavn19").value;
    setname(name_utvalg);
    setdisplayMsg(false);
  };
  const EnterKundeNumber = () => {
    let kundeNumber = document.getElementById("uxKunde19").value;
    setkunde_name(kundeNumber);
    setkunde_number(kundeNumber);
  };
  const Enterlogo = () => {
    let logo = document.getElementById("uxLogo19").value;
    set_logo(logo);
  };

  const velgCustomer = (e) => {
    setkunde_number(Number(e.target.attributes["customerid"].value));
    setkunde_name(e.target.attributes["customername"].value);
  };

  const FinSaveUtvalg = async () => {
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
    document.getElementById("btnSaveKampUtvalg").disabled = true;
    let j = mapView.graphics.items.length;
    for (var i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
      }
    }

    if (name == "" || name.trim().length < 3) {
      document.getElementById("btnSaveKampUtvalg").disabled = false;
      setdisplayMsg(true);
      seterrormsg("Beskrivelse av utvalget må ha minst 3 tegn.");
    } else if (name.indexOf(">") > -1 || name.indexOf("<") > -1) {
      document.getElementById("btnSaveKampUtvalg").disabled = false;
      setdisplayMsg(true);
      seterrormsg("Should not conatain special character");
    } else if (activUtvalg.totalAntall == 0) {
      document.getElementById("btnSaveKampUtvalg").disabled = false;
      setdisplayMsg(true);
      seterrormsg(
        "Utvalget har ingen mottakere og kan derfor ikke lagres. Kontroller at utvalget inneholder budruter og at minst en mottakergruppe er valgt."
      );
    } else {
      setdisplayMsg(false);
      document.getElementById("btnSaveKampUtvalg").disabled = true;
      const { data, status } = await api.getdata(
        `Utvalg/UtvalgNameExists?utvalgNavn=${name}`
      );
      if (status === 200) {
        if (data == true) {
          let msg = `Utvalget ${name} eksisterer allerede. Velg et annet utvalgsnavn.`;
          seterrormsg(msg);
          setdisplayMsg(true);
          document.getElementById("btnSaveKampUtvalg").disabled = false;
        } else {
          // call to save fnctionality
          //Utvalg/SaveUtvalg?userName=tcs&saveOldReoler=false&skipHistory=false&forceUtvalgListId=0
          let saveOldReoler = "false";
          let skipHistory = "false";
          let forceUtvalgListId = 0;
          let url = `Utvalg/SaveUtvalg?userName=Internbruker&`;
          url = url + `saveOldReoler=${saveOldReoler}&`;
          url = url + `skipHistory=${skipHistory}&`;

          url = url + `forceUtvalgListId=${forceUtvalgListId}`;

          try {
            setloading(true);

            var a = saveUtvalg();
            a.totalAntall = activUtvalg.totalAntall;
            a.modifications.push({
              modificationId: Math.floor(100000 + Math.random() * 900000),
              userId: "Internbruker",
              modificationTime: CurrentDate(),
              listId: 0,
            });
            a.name = name;
            a.criterias = activUtvalg.criterias;
            a.reoler = activUtvalg.reoler;
            a.Antall = activUtvalg.Antall;
            a.kundeNummer = activUtvalg.kundeNummer;
            a.logo = logo;
            a.listName = "";
            a.listId = 0;
            // a.isBasis = activUtvalg.isBasis;
            a.receivers = activUtvalg.receivers;
            a.hasReservedReceivers = showReservedHouseHolds;
            a.basedOn = activUtvalg.utvalgId;
            a.basedOnName = activUtvalg.name;
            a.isBasis = false;

            const { data, status } = await api.postdata(url, a);
            //let status = 200;
            if (status === 200) {
              document.getElementById("btnSaveKampUtvalg").disabled = false;
              btnClose.current.click();
              let utvalgID = data.utvalgId;
              if (activUtvalg.utvalgId) {
                await setActivUtvalg({});
              }
              a.utvalgId = utvalgID;
              let activ = showorklist;
              activ.push(a);
              setshoworklist(activ);

              await setActivUtvalg(a);

              setloading(false);
              setissave(true);
              settest(true);
              setAktivDisplay(true);
              setDemografieDisplay(false);
              setSegmenterDisplay(false);
              setAddresslisteDisplay(false);
              setGeografiDisplay(false);
              setKjDisplay(false);
              setAdresDisplay(false);
            } else {
              seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
              document.getElementById("btnSaveKampUtvalg").disabled = false;
              setdisplayMsg(true);
              setissave(false);
              setloading(false);
            }
          } catch (error) {
            console.error("error : " + error);
            seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
            document.getElementById("btnSaveKampUtvalg").disabled = false;
            setdisplayMsg(true);
            setissave(false);
            setloading(false);
          }
        }
      } else {
        console.error("error : " + status);
        seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
        document.getElementById("btnSaveKampUtvalg").disabled = false;
        setdisplayMsg(true);
      }
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
      {/* <!-- Modal --> */}
      {/* {showmodel === true ? (
        <ShowCustomer id={"showCustomer"} result={eConnectResult} />
      ) : null} */}

      <div>
        <div
          className="modal fade bd-example-modal-lg"
          data-backdrop="false"
          id="showCustomer19"
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
              className="modal-content"
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
          <div className="modal-content" style={{ border: "black 3px solid" }}>
            <div className="Common-modal-header">
              <span
                className="common-modal-title pt-1 pl-2"
                id="exampleModalLongTitle"
              >
                OPPRETT KAMPANJE
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
                        id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxNameLabel"
                        className="SaveUtvaldivLabelText pl-2"
                      >
                        Utvalgsnavn
                      </span>
                    </td>
                    <td>
                      <input
                        ref={FirstInputText}
                        name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxName"
                        type="text"
                        id="utvalgnavn19"
                        onChange={EnterName}
                        className="selection-input ml-1"
                        placeholder=""
                        maxLength="50"
                      />
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span
                        id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxKundeLabel"
                        className="SaveUtvaldivLabelText pl-2"
                      >
                        Kundenr/navn
                      </span>
                    </td>
                    <td>
                      <input
                        ref={SecondInputText}
                        value={
                          activUtvalg.utvalgId ? activUtvalg.kundeNummer : null
                        }
                        name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxKunde"
                        type="text"
                        id="uxKunde19"
                        className="selection-input ml-1"
                        //onChange={EnterKundeNumber}
                        disabled={true}
                      />
                    </td>
                    {/* <td className="pl-2">
                      <input
                        type="submit"
                        className="KSPU_button"
                        name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxFindKunde"
                        value="Finn"
                        data-toggle="modal"
                        data-target="#showCustomer19"
                        onClick={FinSaveUtvalg}
                        id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxFindKunde1"
                      />
                    </td> */}
                  </tr>
                  <tr>
                    <td>
                      <span
                        id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxLogoLabel"
                        className="SaveUtvaldivLabelText pl-2"
                      >
                        Forhandlerpåtrykk
                      </span>
                    </td>
                    <td>
                      <input
                        ref={ThirdInputText}
                        className="selection-input ml-1"
                        name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxLogo"
                        type="text"
                        id="uxLogo19"
                        onChange={Enterlogo}
                        placeholder=""
                      />
                    </td>
                  </tr>
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
                        id="btnSaveKampUtvalg"
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

export default SaveCampaign;
