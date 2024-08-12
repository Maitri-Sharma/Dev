import React, { useState, useContext, useEffect, useRef } from "react";
import Mottakergrupper from "./Mottakergrupper";
import Resultat from "./resultat/resultat.component";
import eye from "../assets/images/eye.png";
import ModelComponent from "./CommonModel";
import { KSPUContext } from "../context/Context.js";
import { getCriteriaText } from "./KspuConfig";
import Campaign from "../components/campaign/campaign.jsx";
import Swal from "sweetalert2";

import $ from "jquery";
import {
  NumberFormat,
  CreateUtvalglist,
  CreateActiveUtvalg,
  CurrentDate,
  FormatDate,
} from "../common/Functions";
import api from "../services/api.js";
import disConnect from "../assets/images/disconnect.png";
import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import { groupBy } from "../Data";
import VisDetaljerModal from "./VisDetaljerModal";
import { MainPageContext } from "../context/Context.js";

import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";

import { MapConfig } from "../config/mapconfig";
import Spinner from "./spinner/spinner.component";

function SelectionDetails(props) {
  const [ModelNameDT, setModelNameDT] = useState(" ");
  const [ModelNameHY, setModelNameHY] = useState(" ");
  const [ModelNameED, setModelNameED] = useState(" ");
  const [loading, setloading] = useState(false);
  const { mapView } = useContext(MainPageContext);
  const [commonReoler, setCommonReoler] = useState();
  const [ResultBeforeCreationObject, setResultBeforeCreationObject] =
    useState();
  const [resultRestReoler, setResultRestReoler] = useState();
  const [ResultAfterRuteCreation, setResultAfterRuteCreation] = useState();
  const [ResultAfterCreationObject, setResultAfterCreationObject] = useState();
  const [Modal, setModal] = useState(false);
  const btnClose = useRef();
  const {
    activUtvalg,
    setActivUtvalg,
    setvalue,
    setAktivDisplay,
    setSelectionUpdate,
    setActivUtvalglist,

    setutvalglistcheck,

    setDemografieDisplay,
    setSegmenterDisplay,
    setAddresslisteDisplay,
    setGeografiDisplay,
    setKjDisplay,
    setAdresDisplay,
    showReservedHouseHolds,
    showHousehold,
    showBusiness,
    basisUtvalg,
    setbasisUtvalg,
    setResultData,
    Budruteendringer,
    setBudruteendringer,
    showorklist,
    setshoworklist,
  } = useContext(KSPUContext);
  const { issave, setissave } = useContext(KSPUContext);
  const [Totalantall, setTotalAntall] = useState(activUtvalg.hush);
  const [visDetaljerModalName, setVisDetaljerModalName] = useState(" ");

  const inputChk = useRef();
  const openKampUtvalg = async (e) => {
    let addNewList = false;
    setloading(true);
    let j = mapView.graphics.items.length;
    for (var i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
      }
    }
    let url = "";
    let id = e.target.id;

    url = url + `Utvalg/GetUtvalg?utvalgId=${id}`;
    try {
      const { data, status } = await api.getdata(url);

      if (data.length === 0) {
        //api.logger.error("Error : No Data is present for mentioned Id" + id);
      } else {
        await setActivUtvalg({});
        await setResultData(await groupBy(data.reoler, "", 0, 0, 0, 0, []));
        let obj = await CreateActiveUtvalg(data);
        await setActivUtvalg(obj);
        showorklist.map((val) => {
          if (val.name === data.name) {
            addNewList = true;
          }
        });
        if (!addNewList) {
          setshoworklist((showorklist) => [...showorklist, obj]);
        }
        props.parentCallback(false);
      }
    } catch (error) {
      //api.logger.error("errorpage API not working");
      //api.logger.error("error : " + error);
    }
  };
  const openUtvalgList = async (e) => {
    setloading(true);
    let id = e.target.id;
    id = id.substring(1);

    let url = `UtvalgList/GetUtvalgList?listId=${id}&getParentList=${true}&getMemberUtvalg=${true}`;
    try {
      const { data, status } = await api.getdata(url);
      if (data.length == 0) {
        setloading(false);
      } else {
        let listArray = [];
        showorklist.map((item) => {
          if (item.listId.toString() !== id.toString()) {
            listArray.push(item);
          }
        });
        listArray.push(data);
        setshoworklist(listArray);
        await setActivUtvalglist({});

        let obj = await CreateUtvalglist(data);
        await setActivUtvalglist(obj);
        setvalue(false);
        setissave(true);
        setutvalglistcheck(true);
        setAktivDisplay(true);
        await setActivUtvalg({});
        setDemografieDisplay(false);
        setSegmenterDisplay(false);
        setAddresslisteDisplay(false);
        setGeografiDisplay(false);
        setKjDisplay(false);
        setAdresDisplay(false);
        setloading(false);
      }
    } catch (error) {
      //api.logger.error("errorpage API not working");
      //api.logger.error("error : " + error);
      setloading(false);
    }
  };
  useEffect(() => {
    if (showBusiness) {
      setTotalAntall(activUtvalg.Antall[1]);
      activUtvalg.totalAntall = activUtvalg.Antall[1];
    } else if (showReservedHouseHolds) {
      setTotalAntall(activUtvalg.Antall[2]);
      activUtvalg.totalAntall = activUtvalg.Antall[2];
    } else if (showHousehold) {
      setTotalAntall(activUtvalg.Antall[0]);
      activUtvalg.totalAntall = activUtvalg.Antall[0];
    }
    if (showBusiness && showReservedHouseHolds) {
      setTotalAntall(activUtvalg.Antall[1] + activUtvalg.Antall[2]);
      activUtvalg.totalAntall = activUtvalg.Antall[1] + activUtvalg.Antall[2];
    }
    if (showHousehold && showReservedHouseHolds) {
      setTotalAntall(activUtvalg.Antall[0] + activUtvalg.Antall[2]);
      activUtvalg.totalAntall = activUtvalg.Antall[0] + activUtvalg.Antall[2];
    }
    if (showBusiness && showHousehold) {
      setTotalAntall(activUtvalg.Antall[0] + activUtvalg.Antall[1]);
      activUtvalg.totalAntall = activUtvalg.Antall[0] + activUtvalg.Antall[1];
    }
    if (showBusiness && showReservedHouseHolds && showHousehold) {
      setTotalAntall(
        activUtvalg.Antall[0] + activUtvalg.Antall[1] + activUtvalg.Antall[2]
      );
      activUtvalg.totalAntall =
        activUtvalg.Antall[0] + activUtvalg.Antall[1] + activUtvalg.Antall[2];
    }
    activUtvalg.hasReservedReceivers = showReservedHouseHolds;
  }, [showHousehold, showBusiness, showReservedHouseHolds]);

  useEffect(async () => {
    if (activUtvalg.antallBeforeRecreation > 0 && Budruteendringer) {
      setBudruteendringer(false);
      let msg = `Budruteendringer har påvirket utvalgene`;

      Swal.fire({
        text: msg,
        confirmButtonColor: "#7bc144",
        confirmButtonText: "Lukk",
      });
    }
    if (activUtvalg.isBasis) {
      setbasisUtvalg(true);
    } else {
      setbasisUtvalg(false);
    }
  }, []);
  let dateString1 = " ";
  if (activUtvalg !== "undefined" && activUtvalg.length <= 0) {
    if (activUtvalg?.modifications?.length > 0) {
      dateString1 =
        activUtvalg.modifications[activUtvalg?.modifications?.length - 1]
          .modificationTime;
    }
  }
  const disconnectUtvalg = async (e) => {
    setModal(true);
  };
  const Jaclick = async (e) => {
    if (
      activUtvalg.basedOn !== 0 &&
      activUtvalg.basedOn !== undefined &&
      activUtvalg.basedOn !== ""
    ) {
      setloading(true);
      activUtvalg.WasBasedOn = activUtvalg.basedOn;
      activUtvalg.basedOn = 0;
      activUtvalg.basedOnName = "";
      let saveOldReoler = "false";
      let skipHistory = "false";
      let forceUtvalgListId = 0;
      let url = `Utvalg/SaveUtvalg?userName=Internbruker&`;
      url = url + `saveOldReoler=${saveOldReoler}&`;
      url = url + `skipHistory=${skipHistory}&`;

      url = url + `forceUtvalgListId=${forceUtvalgListId}`;
      try {
        activUtvalg.modifications.push({
          modificationId: Math.floor(100000 + Math.random() * 900000),
          userId: "Internbruker",
          modificationTime: CurrentDate(),
          listId: 0,
        });

        const { data, status } = await api.postdata(url, activUtvalg);
        if (status === 200) {
          let disconnectUrl = `Utvalg/UpdateReolMapnameForDisconnectedUtvalg?userName=Internbruker`;

          const { data, status } = await api.postdata(
            disconnectUrl,
            activUtvalg
          );

          if (status === 200) {
            showorklist.map((item, x) => {
              if (item?.utvalgId === activUtvalg?.utvalgId) {
                item.basedOn = 0;
                item.basedOnName = "";
                item.modifications[0].modificationTime = CurrentDate();
              }
            });
            setshoworklist([...showorklist]);
            let msg = `Kampanje er koblet fra basis utvalg/liste.`;
            $(".modal").remove();
            $(".modal-backdrop").remove();
            Swal.fire({
              text: msg,
              confirmButtonColor: "#7bc144",
              confirmButtonText: "Lukk",
            });
            setloading(false);
          } else {
            let msg = `Oppgitte søkekriterier ga ikke noe resultat.`;
            $(".modal").remove();
            $(".modal-backdrop").remove();
            Swal.fire({
              text: msg,
              confirmButtonColor: "#7bc144",
              confirmButtonText: "Lukk",
            });
            setloading(false);
          }
        } else {
          let msg = `Oppgitte søkekriterier ga ikke noe resultat.`;
          $(".modal").remove();
          $(".modal-backdrop").remove();
          Swal.fire({
            text: msg,
            confirmButtonColor: "#7bc144",
            confirmButtonText: "Lukk",
          });
          setloading(false);
        }
      } catch (error) {
        console.error("error : " + error);
        let msg = `Oppgitte søkekriterier ga ikke noe resultat.`;
        $(".modal").remove();
        $(".modal-backdrop").remove();
        Swal.fire({
          text: msg,
          confirmButtonColor: "#7bc144",
          confirmButtonText: "Lukk",
        });
        setloading(false);
      }
    }
  };
  const utvalgBasis = () => {
    setbasisUtvalg(inputChk.current.checked);
    setSelectionUpdate(true);
  };

  const showModelDetail = () => {
    setModelNameDT("eyeIcon_ViewDetails");
  };
  const showModelHistory = () => {
    setModelNameHY("eyeIcon_ViewHistory");
  };
  const showModelEdit = () => {
    setModelNameED("eyeIcon_ViewEdit");
  };

  const showVisDetaljer = () => {
    let reoler = activUtvalg.reoler;
    let reolerBeforeRecreation = activUtvalg.reolerBeforeRecreation;

    let reolids = [];
    let result = reoler.filter((o1) =>
      reolerBeforeRecreation.map((o2) => {
        if (o1.reolId === o2.reolId) {
          o1["businessesOld"] = o2.antall.businesses;
          o1["householdsOld"] = o2.antall.households;
          reolids.push(o1.reolId);
        }
      })
    );

    let restReoler = [];
    let resultBeforeCreationObject = reolerBeforeRecreation.filter((item) => {
      if (reolids.includes(item.reolId)) {
        return item;
      } else {
        restReoler.push(item);
      }
    });
    setResultBeforeCreationObject(resultBeforeCreationObject);
    setResultRestReoler(restReoler);
    let restCurrentReoler = [];
    let resultAfterCreationObject = reoler.filter((item) => {
      if (reolids.includes(item.reolId)) {
        return item;
      } else {
        restCurrentReoler.push(item);
      }
    });
    setResultAfterRuteCreation(resultAfterCreationObject);

    setResultAfterCreationObject(restCurrentReoler);
    setCommonReoler(result);
    setVisDetaljerModalName("VisDetaljer");
  };

  const showVisKart = async () => {
    setloading(true);
    let previousUtvalg = false;
    // remove previous old utvalg feature
    mapView.graphics.items.forEach(function (item) {
      if (item.attributes !== null) {
        if (item.attributes.utvalgid !== undefined) {
          previousUtvalg = true;
          mapView.graphics.remove(item);
        }
      }
    });
    setloading(false);

    if (!previousUtvalg) {
      setloading(true);
      let utvalgId = activUtvalg.utvalgId;

      let queryObject = new Query();

      queryObject.where = `utvalgid =` + utvalgId;
      queryObject.returnGeometry = true;
      queryObject.outFields = ["utvalgid"];

      await query
        .executeQueryJSON(MapConfig.oldUtvalgGeometryUrl, queryObject)
        .then(function (results) {
          if (results.features.length > 0) {
            let selectedSymbol = {
              type: "simple-fill", // autocasts as new SimpleFillSymbol()
              color: [51, 51, 51, 0.75],
              style: "backward-diagonal",
              outline: {
                // autocasts as new SimpleLineSymbol()
                color: [18, 12, 12],
                width: 0.75,
              },
            };

            results.features.map((item) => {
              let graphic = new Graphic(
                item.geometry,
                selectedSymbol,
                item.attributes
              );
              mapView.graphics.add(graphic);
            });
          } else {
            let msg = `Dette utvalget er uendret siden forrige oppdatering - identisk antall og område i kartet. Det er derfor ikke laget et "før"-kart for sammenligning. Klikk OK for å fortsette.`;
            $(".modal").remove();
            $(".modal-backdrop").remove();
            Swal.fire({
              text: msg,
              confirmButtonColor: "#7bc144",
              confirmButtonText: "Lukk",
            });
            setloading(false);
          }

          mapView.watch("updating", function (evt) {
            setloading(true);
            setloading(false);
          });
          setloading(false);
        });
    }
  };

  const onClose = () => {
    setModelNameHY("");
    setModelNameDT("");
  };

  return (
    <div>
      <div>{loading ? <Spinner /> : null}</div>
      <div className="pl-1 pt-1 nowrap p-0 m-0">
        {ModelNameDT == "eyeIcon_ViewDetails" ? (
          <ModelComponent
            title={"UTVALGSKRITERIER"}
            id={"visDetails"}
            data={activUtvalg.criterias}
            onClose={onClose}
          />
        ) : null}
        {ModelNameHY == "eyeIcon_ViewHistory" ? (
          <ModelComponent
            title={"HISTORIKK"}
            id={"visHistory"}
            data={activUtvalg.modifications}
            onClose={onClose}
          />
        ) : null}
        {ModelNameED == "eyeIcon_ViewEdit" ? (
          <ModelComponent
            title={"Endre forhandlerpåtrykk"}
            id={"visEdit"}
            data={activUtvalg.utvalgId}
          />
        ) : null}
        {visDetaljerModalName == "VisDetaljer" ? (
          <VisDetaljerModal
            title={"BUDRUTEENDRINGER"}
            id={"visdetaljer"}
            ResultBeforeCreationObject={ResultBeforeCreationObject}
            ResultAfterRuteCreation={ResultAfterRuteCreation}
            ResultAfterCreationObject={ResultAfterCreationObject}
            oldReoler={resultRestReoler}
          />
        ) : null}

        {Modal ? (
          <div
            className="modal fade bd-example-modal-lg"
            id="uxBtnDisconnectSelection"
            tabIndex="-1"
            role="dialog"
            aria-labelledby="exampleModalCenterTitle"
            aria-hidden="true"
          >
            <div className="modal-dialog" role="document">
              <div className="modal-content">
                <div className="modal-header segFord">
                  <h5 className="modal-title " id="exampleModalLongTitle">
                    Advarsel
                  </h5>
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
                  <table>
                    <tbody>
                      <tr>
                        <td>
                          <p className="p-slett">
                            Utvalget "{activUtvalg.name}" er knyttet til
                            basisutvalget <br /> "{activUtvalg.basedOnName}", og
                            endres automatisk når dette <br />
                            basisutvalget endres. <br />
                            <br /> Fristiller du utvalget, kan du endre utvalget
                            direkte,
                            <br />
                            men det vil ikke lenger oppdateres automatisk <br />
                            når basisutvalget oppdateres. Denne operasjonen kan
                            ikke angres. <br /> <br /> Ønsker du å bryte
                            knytningen til basisutvalget?
                          </p>{" "}
                        </td>
                        <td></td>
                      </tr>

                      <tr>
                        <td>
                          <div className="ml-4">
                            <button
                              type="button"
                              className="modalMessage_button"
                              data-dismiss="modal"
                            >
                              Avbryt
                            </button>
                            <button
                              type="button"
                              className="modalMessage_button ml-5"
                              data-dismiss="modal"
                            >
                              Nei
                            </button>
                            <button
                              type="button"
                              onClick={Jaclick}
                              className="modalMessage_button ml-5"
                              data-dismiss="modal"
                              data-target="#disconnectListFromSelection"
                            >
                              Ja
                            </button>
                          </div>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>
        ) : null}

        <div className="row">
          <div className="col-5 _flex-start">
            <div>
              <label id="UtvalgNavn" className="divDetailsName">
                Utvalg{" "}
              </label>
            </div>
          </div>
          <div className="col-7 no-width">
            <span id="" className="divDetailsName">
              {" "}
              {activUtvalg.name}
            </span>
          </div>
        </div>
        <div className="row">
          <div className="col-5">
            <span id="TotalAntall" className="UtvaldivLabelText">
              Totalt antall
            </span>
          </div>
          <div className="col" style={{ textAlign: "left" }}>
            <span id="" className="divValueText">
              {NumberFormat(activUtvalg.totalAntall)}
            </span>
          </div>
        </div>
        <div className="row">
          <div className="col-5">
            <span id="Mottakergrupper" className="UtvaldivLabelText">
              Mottakergrupper
            </span>
          </div>
          <div className="col" style={{ textAlign: "left" }}>
            <span id="" className="divValueText">
              {showHousehold
                ? `Hush. : ${NumberFormat(activUtvalg?.Antall[0])} `
                : ""}{" "}
              {showBusiness ? <br /> : ""}
              {showBusiness
                ? `Virk. : ${NumberFormat(activUtvalg?.Antall[1])} `
                : ""}{" "}
              {showReservedHouseHolds ? <br /> : ""}
              {showReservedHouseHolds
                ? ` Res.hush. : ${NumberFormat(activUtvalg?.Antall[2])} `
                : ""}
            </span>
          </div>
        </div>
        <div className="row">
          <div className="col-5">
            <span id="Utvalgskriterier" className="UtvaldivLabelText">
              Utvalgskriterier{" "}
            </span>
          </div>
          <div className="col-5">
            <span id="" className="divValueText_budruter  break-text">
              {activUtvalg?.criterias?.length > 0
                ? getCriteriaText(activUtvalg?.criterias[0]?.criteriaType)
                : null}
            </span>
          </div>
          <div className="col-2 no-padding">
            {/* <a  id="uxShowCriteria" href="" 
                                     className="KSPU_LinkButton" >Vis detaljer
                                         </a> */}
            <img
              id="uxShowCriteria"
              className="KSPU_LinkButton "
              data-toggle="modal"
              data-target="#visDetails"
              onClick={showModelDetail}
              alt="Vis detaljer"
              title="Vis detaljer"
              src={eye}
            />
          </div>
        </div>
        <div className="row">
          <div className="col-5">
            <span id="Strukturendringer" className="UtvaldivLabelText">
              Budruteendringer{" "}
            </span>
          </div>
          {activUtvalg.antallBeforeRecreation > 0 ? (
            <div className="col">
              <span
                href=""
                className="selectionUtvalgList"
                data-toggle="modal"
                data-target="#visdetaljer"
                onClick={showVisDetaljer}
              >
                Vis detaljer
              </span>
              <span
                // type="button"
                // Visible="False"
                // id="uxShowStructuralChangesInMap"
                value="Vis/Fjern i kart"
                className="selectionUtvalgList ml-2"
                onClick={showVisKart}
                // LinkButtonclass="KSPU_LinkButton"
              >
                Vis/Fjern i kart
              </span>
            </div>
          ) : (
            <div className="col">
              <span id="" className="divValueText">
                {" "}
                Nei
              </span>
            </div>
          )}
        </div>

        <div className="row">
          <div className="col-5">
            <span id="Referansenr" className="UtvaldivLabelText">
              UtvalgsID{" "}
            </span>
          </div>
          <div className="col-7" style={{ textAlign: "left" }}>
            <span id="" className="divValueText">
              {activUtvalg.utvalgId === 0 || activUtvalg.utvalgId === undefined
                ? " - "
                : "U" + activUtvalg.utvalgId}
            </span>
          </div>
        </div>

        <div className="row">
          <div className="col-5">
            <span id="KundeNr" className="UtvaldivLabelText">
              Kundenr{" "}
            </span>
          </div>
          <div className="col-7" style={{ textAlign: "left" }}>
            <span id="" className="divValueText">
              {activUtvalg.kundeNummer === 0 ||
              activUtvalg.kundeNummer === undefined ||
              activUtvalg.kundeNummer === "0"
                ? ""
                : activUtvalg.kundeNummer}
            </span>
          </div>
        </div>

        <div className="row">
          <div className="col-5">
            <span id="Forhandlerpaatrykk" className="UtvaldivLabelText">
              Forhandlerpåtryk
            </span>
          </div>
          <div className="col-5">
            <span id="selectionLogo" className="divValueText">
              {activUtvalg.logo}
            </span>
          </div>
          {props.Utvalcheck == "True" ? (
            <div className="col-2 no-padding">
              <a
                id="uxEditForhandler"
                className="KSPU_LinkButton"
                onClick={showModelEdit}
                data-toggle="modal"
                data-target="#visEdit"
                alt="Rediger"
                title="Rediger"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="16"
                  height="16"
                  fill="currentColor"
                  className="bi bi-pencil-square"
                  data-toggle="modal"
                  data-target="#visEdit"
                  viewBox="0 0 16 16"
                >
                  <path d="M15.502 1.94a.5.5 0 0 1 0 .706L14.459 3.69l-2-2L13.502.646a.5.5 0 0 1 .707 0l1.293 1.293zm-1.75 2.456-2-2L4.939 9.21a.5.5 0 0 0-.121.196l-.805 2.414a.25.25 0 0 0 .316.316l2.414-.805a.5.5 0 0 0 .196-.12l6.813-6.814z" />
                  <path
                    fillRule="evenodd"
                    d="M1 13.5A1.5 1.5 0 0 0 2.5 15h11a1.5 1.5 0 0 0 1.5-1.5v-6a.5.5 0 0 0-1 0v6a.5.5 0 0 1-.5.5h-11a.5.5 0 0 1-.5-.5v-11a.5.5 0 0 1 .5-.5H9a.5.5 0 0 0 0-1H2.5A1.5 1.5 0 0 0 1 2.5v11z"
                  />
                </svg>
              </a>
              {/* <img id="uxEditForhandler" className="KSPU_LinkButton" alt="Rediger" title="Rediger" src={edit} /> */}
            </div>
          ) : null}
        </div>

        <div className="row">
          <div className="col-5">
            <span id="LagretDato" className="UtvaldivLabelText">
              Sist lagret{" "}
            </span>
          </div>
          <div className="col-5">
            <span id="modificationTime" className="divValueText">
              {activUtvalg?.modifications?.length > 0
                ? FormatDate(activUtvalg?.modifications[0].modificationTime)
                : "Ikke lagret"}
            </span>
          </div>
          {props.Utvalcheck == "True" ? (
            <div className="col-2 no-padding">
              <img
                id="uxShowHistory"
                className="KSPU_LinkButton"
                data-toggle="modal"
                data-target="#visHistory"
                onClick={showModelHistory}
                alt="Vis historikk"
                title="Vis historikk"
                src={eye}
              />
            </div>
          ) : null}
        </div>
        <div className="row">
          <div className="col-5">
            <span id="LagretNavn" className="UtvaldivLabelText">
              Lagret av{" "}
            </span>
          </div>
          <div className="col-7" style={{ textAlign: "left" }}>
            <span id="" className="divValueText">
              {activUtvalg?.modifications?.length > 0
                ? activUtvalg.modifications[
                    activUtvalg?.modifications?.length - 1
                  ].userId
                : ""}
            </span>
          </div>
        </div>

        <div className="row">
          <div className="col-5">
            <span id="uxWriteProtectLabel" className="UtvaldivLabelText">
              Skrivebeskyttet
            </span>
          </div>
          <div className="col-7"></div>
        </div>

        <div className="row mb-2">
          <div className="col-5">
            <span id="uxIsBasisLabel" className="UtvaldivLabelText">
              Basisutvalg
            </span>
          </div>
          {activUtvalg.basedOn !== 0 &&
          activUtvalg.basedOn !== undefined &&
          activUtvalg.basedOn !== "" ? (
            // <div className="row">
            <div className="col-5  divValueText mb-3">
              Utvalget er knyttet til
              <span
                id={activUtvalg.basedOn}
                className="selectionUtvalgList "
                onClick={openKampUtvalg}
              >
                {" "}
                {activUtvalg.basedOnName}
              </span>
              {loading ? (
                <img
                  src={loadingImage}
                  style={{
                    width: "20px",
                    height: "20px",
                    position: "absolute",
                    left: "122px",
                    zindex: 100,
                  }}
                />
              ) : null}
              {/* </div>
            <div className="col-2"> */}
            </div>
          ) : (
            // </div>
            <div className="col-7  defaultwrap">
              {activUtvalg.listId !== 0 &&
              activUtvalg.listId !== undefined &&
              activUtvalg.listId !== "" &&
              activUtvalg.listId !== "0" ? (
                <input
                  type="checkbox"
                  id="uxIsBasisChkBox"
                  value=""
                  ref={inputChk}
                  className="UtvaldivLabelText align-middle"
                  checked={basisUtvalg}
                  onChange={utvalgBasis}
                  disabled={true}
                />
              ) : (
                <input
                  type="checkbox"
                  id="uxIsBasisChkBox"
                  value=""
                  ref={inputChk}
                  className="UtvaldivLabelText align-middle"
                  checked={basisUtvalg}
                  onChange={utvalgBasis}
                />
              )}
              <span id="basisutvalg" className="form-check-label label-text">
                Utvalget er et basisutvalg{" "}
              </span>
            </div>
          )}
          {activUtvalg.basedOn !== 0 &&
          activUtvalg.basedOn !== undefined &&
          activUtvalg.basedOn !== "" ? (
            <div className="col-2">
              <img
                id="disconnect"
                className="KSPU_LinkButton"
                onClick={disconnectUtvalg}
                data-toggle="modal"
                data-target="#uxBtnDisconnectSelection"
                alt="Frikoble denne listen fra basislisten"
                title="Frikoble denne listen fra basislisten"
                src={disConnect}
              />
            </div>
          ) : null}
        </div>
        {activUtvalg.listId !== 0 &&
        activUtvalg.listId !== undefined &&
        activUtvalg.listId !== "" &&
        activUtvalg.listId !== "0" ? (
          <div className="row">
            <div className="col-5">
              <span id="uxWriteProtectLabel" className="UtvaldivLabelText">
                Utvalg ligger i
              </span>
            </div>
            <div className="col-7">
              <span
                id={"L" + activUtvalg.listId}
                className="selectionUtvalgList defaultwrap UtvaldivLabelValue"
                onClick={openUtvalgList}
              >
                {activUtvalg?.listName ? activUtvalg?.listName : ""}
              </span>
              {loading ? (
                <img
                  src={loadingImage}
                  style={{
                    width: "20px",
                    height: "20px",
                    position: "absolute",
                    left: "122px",
                    zindex: 100,
                  }}
                />
              ) : null}
            </div>
          </div>
        ) : null}

        <div>
          <Mottakergrupper page="DTPage" marginTop=".3rem" />
        </div>
        <div className="mt-2">
          <Resultat Totalantallvalue={Totalantall} />
        </div>

        {activUtvalg.isBasis ? (
          <div className="mt-2">
            <Campaign data={activUtvalg} />
          </div>
        ) : null}
      </div>
    </div>
  );
}

export default SelectionDetails;
