import React, { useState, useRef, useContext } from "react";
import { KSPUContext, MainPageContext } from "../context/Context.js";
import api from "../services/api.js";
import { v4 as uuidv4 } from "uuid";
import { saveUtvalg } from "./KspuConfig";

import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import { CurrentDate } from "../common/Functions";
function SaveUtvalg(props) {
  const { errormsg, seterrormsg } = useContext(KSPUContext);
  const { issave, setissave } = useContext(KSPUContext);

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
    showBusiness,
    showHousehold,
    setSelectionUpdate,
    basisUtvalg,
    setutvalglistcheck,
    setActivUtvalglist,
  } = useContext(KSPUContext);
  const { selecteddemografiecheckbox, setselecteddemografiecheckbox } =
    useContext(KSPUContext);
  const { selectedsegment, setselectedsegment } = useContext(KSPUContext);

  const [displayMsg, setdisplayMsg] = useState(false);
  const [eConnectResult, seteConnectResult] = useState([]);
  const [name, setname] = useState("");
  const [kunde_name, setkunde_name] = useState(activUtvalg?.kundeNummer);

  const [kunde_number, setkunde_number] = useState(activUtvalg?.kundeNummer);
  const [logo, set_logo] = useState("");
  const [test, settest] = useState(false);
  const [showmodel, setshowmodel] = useState(false);
  const btnClose = useRef();

  const [loading, setloading] = useState(false);
  const { mapView, setActiveMapButton } = useContext(MainPageContext);

  const { showReservedHouseHolds, setshoworklist, showorklist } =
    useContext(KSPUContext);
  const FirstInputText = useRef();
  const SecondInputText = useRef();
  const ThirdInputText = useRef();

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
  const Enterlogo = () => {
    let logo = document.getElementById("uxLogo").value;
    set_logo(logo);
  };

  const velgCustomer = (e) => {

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
    window.$("#showCustomer").modal("show");
    let uniqueId = uuidv4();
    let eConnectUrl = `ECPuma/FindCustomer380`;
    setloading(true);

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
          //seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.")
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
    setSelectionUpdate(false);

    //disable sketech widget on switching the selection
    setActiveMapButton("");
    mapView.activeTool = null;

    if (!(props.utvalgDetails && activUtvalg.name === "Påbegynt utvalg")) {
      let j = mapView.graphics.items.length;
      for (var i = j; i > 0; i--) {
        if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
          mapView.graphics.remove(mapView.graphics.items[i - 1]);
        }
      }
    }

    if (name == "" || name.trim().length < 3) {
      setdisplayMsg(true);
      seterrormsg("Beskrivelse av utvalget må ha minst 3 tegn.");
    } else if (name.indexOf(">") > -1 || name.indexOf("<") > -1) {
      setdisplayMsg(true);
      seterrormsg("Should not conatain special character");
    } else if (activUtvalg.totalAntall == 0) {
      setdisplayMsg(true);
      seterrormsg(
        "Utvalget har ingen mottakere og kan derfor ikke lagres. Kontroller at utvalget inneholder budruter og at minst en mottakergruppe er valgt."
      );
    } else {
      setdisplayMsg(false);
      document.getElementById("btnSaveUtvalg").disabled = true;
      const { data, status } = await api.getdata(
        `Utvalg/UtvalgNameExists?utvalgNavn=${name}`
      );
      if (status === 200) {
        if (data == true) {
          let msg = `Utvalget ${name} eksisterer allerede. Velg et annet utvalgsnavn.`;
          seterrormsg(msg);
          setdisplayMsg(true);
          document.getElementById("btnSaveUtvalg").disabled = false;
        } else {
          // call to save fnctionality

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
            a.kundeNummer = kunde_number;
            a.isBasis = basisUtvalg;
            a.logo = logo;

            if (activUtvalg.receivers.length !== 0) {
              activUtvalg.receivers.map((item) => {
                if (showHousehold) {
                  if (item.receiverId !== 1) {
                    activUtvalg.receivers.push({
                      receiverId: 1,
                      selected: true,
                    });
                  }
                } else {
                  let temp = activUtvalg;
                  temp = temp.receivers.filter((result) => {
                    return result.receiverId !== 1;
                  });
                  activUtvalg.receivers = temp;
                }
              });
            } else {
              if (showHousehold) {
                activUtvalg.receivers.push({ receiverId: 1, selected: true });
              }
            }

            if (activUtvalg.receivers.length !== 0) {
              activUtvalg.receivers.map((item) => {
                if (showBusiness) {
                  if (item.receiverId !== 4) {
                    activUtvalg.receivers.push({
                      receiverId: 4,
                      selected: true,
                    });
                  }
                } else {
                  let temp = activUtvalg;
                  temp = temp.receivers.filter((result) => {
                    return result.receiverId !== 4;
                  });
                  activUtvalg.receivers = temp;
                }
              });
            } else {
              if (showBusiness) {
                activUtvalg.receivers.push({ receiverId: 4, selected: true });
              }
            }
            if (activUtvalg.receivers.length !== 0) {
              activUtvalg.receivers.map((item) => {
                if (showReservedHouseHolds) {
                  if (item.receiverId !== 5) {
                    activUtvalg.hasReservedReceivers = true;
                    activUtvalg.receivers.push({
                      receiverId: 5,
                      selected: true,
                    });
                  }
                } else {
                  let temp = activUtvalg;
                  temp = temp.receivers.filter((result) => {
                    return result.receiverId !== 5;
                  });
                  activUtvalg.receivers = temp;
                }
              });
            } else {
              if (showReservedHouseHolds) {
                activUtvalg.hasReservedReceivers = true;
                activUtvalg.receivers.push({ receiverId: 5, selected: true });
              }
            }
            if (activUtvalg?.name === "Påbegynt utvalg") {
              a.isBasis = basisUtvalg;
            }
            a.receivers = activUtvalg.receivers;
            a.hasReservedReceivers = showReservedHouseHolds;

            const { data, status } = await api.postdata(url, a);

            if (status === 200) {
              if (props.utvalgDetails) {
                props.onSave();
              }

              document.getElementById("btnSaveUtvalg").disabled = false;
              btnClose.current.click();
              let utvalgID = data.utvalgId;
              if (activUtvalg.utvalgId) {
                await setActivUtvalg({});
              }
              setutvalglistcheck(false);
              setActivUtvalglist({});
              a.utvalgId = utvalgID;
              let activ = showorklist;
              activ.push(a);
              setshoworklist(activ);
              setselectedsegment([]);
              setselecteddemografiecheckbox([]);

              setissave(true);
              settest(true);

              await setActivUtvalg(a);

              setloading(false);

              setAktivDisplay(true);
              setDemografieDisplay(false);
              setSegmenterDisplay(false);
              setAddresslisteDisplay(false);
              setGeografiDisplay(false);
              setKjDisplay(false);
              setAdresDisplay(false);
            } else {
              seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
              document.getElementById("btnSaveUtvalg").disabled = false;
              setdisplayMsg(true);
              setissave(false);
              setloading(false);
            }
          } catch (error) {
            console.error("error : " + error);
            seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
            document.getElementById("btnSaveUtvalg").disabled = false;
            setdisplayMsg(true);
            setissave(false);
            setloading(false);
          }
        }
      } else {
        console.error("error : " + status);
        seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
        setdisplayMsg(true);
        document.getElementById("btnSaveUtvalg").disabled = false;
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
      <div>
        <div
          className="modal fade bd-example-modal-lg"
          data-backdrop="false"
          id="showCustomer"
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
                LAGRE UTVALG
              </span>
              <button
                type="button"
                className="close"
                data-dismiss="modal"
                aria-label="Close"
                ref={btnClose}
                onClick={props.onClose}
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
                        id="utvalgnavn"
                        onChange={EnterName}
                        className="selection-input ml-1"
                        maxLength="50"
                        placeholder=""
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
                        value={kunde_number ? kunde_number : null}
                        name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxKunde"
                        type="text"
                        id="uxKunde"
                        className="selection-input ml-1"
                        onChange={EnterKundeNumber}
                        placeholder=""
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
                        data-target="#showCustomer"
                        onClick={FinSaveUtvalg}
                        id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxFindKunde"
                      />
                    </td>
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
                        id="uxLogo"
                        onChange={Enterlogo}
                        placeholder=""
                      />
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <button
                        type="button"
                        className="btn KSPU_button"
                        data-dismiss="modal"
                        onClick={props.onClose}
                      >
                        Avbryt
                      </button>
                    </td>
                    <td></td>
                    <td>
                      <button
                        type="button"
                        id="btnSaveUtvalg"
                        onClick={uxSaveUtvalg}
                        className="btn KSPU_button"
                        disabled={Number(kunde_number).toString() === "NaN"}
                      >
                        Lagre
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default SaveUtvalg;
