import React, { useState, useContext, useEffect, useRef } from "react";
import eye from "../assets/images/eye.png";
import edit from "../assets/images/edit.png";
import ForhandlerlistModel from "./ForhandlerlistModel";
import { KSPUContext } from "../context/Context.js";
import { getCriteriaText } from "./KspuConfig";
import UtvalDetails from "./UtvalDetails";
import Utvalselection from "./utvalselection";
import Utvalglistselection from "./utvalglistselection";
import TilHorerUtvalgList from "../components/tilhorerutvalglist/tilhorerutvalglist.jsx";
import Campaign from "../components/campaign/campaign.jsx";
import api from "../services/api.js";
import Swal from "sweetalert2";
import $ from "jquery";
import disConnect from "../assets/images/disconnect.png";
function Selectionlist(props) {
  const {
    activUtvalglist,
    setActivUtvalglist,
    setAktivDisplay,
    showorklist,
    setshoworklist,
  } = useContext(KSPUContext);
  const [ModelNameED, setModelNameED] = useState(" ");
  const [flag, setFlag] = useState(false);
  const [flagNew, setFlagNew] = useState(true);
  const [loading, setloading] = useState(false);
  const [Modal, setModal] = useState(false);
  const basisChecked = useRef(null);
  const btnClose = useRef();
  const showModelEdit = () => {
    setModelNameED("eyeIcon_ViewEdit");
  };
  const openKampList = async (e) => {
    setloading(true);
    let id = e.target.id;

    let url = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${id}`;
    try {
      //api.logger.info("APIURL", url);
      const { data, status } = await api.getdata(url);
      if (data.length == 0) {
        //api.logger.error("Error : No Data is present for mentioned Id" + id);
        setloading(false);
      } else {
        let newWorklistArray = [];
        await setActivUtvalglist({});

        showorklist.map((item) => {
          if (item.listId?.toString() !== data.listId?.toString()) {
            newWorklistArray.push(item);
          }
        });
        newWorklistArray.push(data);
        await setActivUtvalglist(data);
        setshoworklist(newWorklistArray);

        setAktivDisplay(false);
        setAktivDisplay(true);

        setloading(false);
      }
    } catch (error) {
      //api.logger.error("errorpage API not working");
      //api.logger.error("error : " + error);
      setloading(false);
    }
  };
  const openPreviousKampList = async (e) => {
    setloading(true);
    let id = e.target.id;

    let url = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${id}`;
    try {
      //api.logger.info("APIURL", url);
      const { data, status } = await api.getdata(url);
      if (data.length == 0) {
        //api.logger.error("Error : No Data is present for mentioned Id" + id);
        setloading(false);
      } else {
        let newWorklistArray = [];
        await setActivUtvalglist({});

        showorklist.map((item) => {
          if (item.listId?.toString() !== data.listId?.toString()) {
            newWorklistArray.push(item);
          }
        });
        newWorklistArray.push(data);
        await setActivUtvalglist(data);
        setshoworklist(newWorklistArray);

        setAktivDisplay(false);
        setAktivDisplay(true);

        setloading(false);
      }
    } catch (error) {
      //api.logger.error("errorpage API not working");

      setloading(false);
    }
  };
  const disconnectList = () => {
    setModal(true);
  };

  const Jaclick = async () => {
    if (
      activUtvalglist.basedOn !== 0 &&
      activUtvalglist.basedOn !== undefined &&
      activUtvalglist.basedOn !== ""
    ) {
      setloading(true);
      activUtvalglist.WasBasedOn = activUtvalglist.basedOn;
      activUtvalglist.basedOn = 0;
      activUtvalglist.basedOnName = "";
      let disconnectUrl = `UtvalgList/DisconnectList?userName=Internbruker`;
      let reqObj = {
        listId: activUtvalglist?.listId,
      };
      try {
        const { data, status } = await api.putData(disconnectUrl, reqObj);
        if (status === 200) {
          await setActivUtvalglist({});
          // let msg = "updated list";
          let activ = showorklist;
          var newActiv = [];
          newActiv = activ.filter((item) => {
            return item.listId !== activUtvalglist.listId;
          });
          newActiv.push(data);
          await setshoworklist(newActiv);
          await setActivUtvalglist(data);
          activUtvalglist.WasBasedOn = activUtvalglist.basedOn;
          activUtvalglist.basedOn = 0;
          activUtvalglist.basedOnName = "";
          let msg = `Kampanje er koblet fra basis utvalg/liste.`;
          $(".modal").remove();
          $(".modal-backdrop").remove();
          Swal.fire({
            text: msg,
            confirmButtonColor: "#7bc144",
            confirmButtonText: "Lukk",
          });
          //activUtvalglist.logo = logoName;
          setAktivDisplay(false);
          setAktivDisplay(true);
          setloading(false);
          btnClose.current.click();
        }
      } catch (error) {
        console.log("errorpage API not working");
        console.error("error : " + error);

        setloading(false);
      }
    }
  };
  const listUpdateBasis = async (e) => {
    // alert("Hi");
    // debugger;
    let listBasis = false;
    if (e.target.checked) {
      listBasis = true;
    } else {
      listBasis = false;
    }
    let listId = activUtvalglist.listId;
    let isBasedOn = activUtvalglist.basedOn;
    if (isBasedOn > 0) {
      $(".modal").remove();
      $(".modal-backdrop").remove();
      Swal.fire({
        text: `Lista er basert på en annen liste og kan ikke være basisliste.`,
        confirmButtonColor: "#7bc144",
        confirmButtonText: "Lukk",
      });
    } else {
      let url = "";
      let listArray = [];
      let flagArray = [];
      setloading(true);
      //url = url + `UtvalgList/UpdateListLogo?userName=Internbruker&`;
      url =
        url +
        `UtvalgList/UpdateIsBasis?listId=${listId}&isBasis=${listBasis}&basedOn=${isBasedOn}`;
      try {
        const { data, status } = await api.putData(url);
        if (status === 200) {
          setloading(false);
          activUtvalglist.isBasis = listBasis;
          let msg = "";
          if (listBasis) {
            msg = "Utvalglisten er lagret som en basisliste";
          } else {
            msg = "Utvalglisten er ikke lenger lagret som en basisliste.";
          }
          showorklist.map((item) => {
            if (item.listId.toString() !== listId.toString()) {
              listArray.push(item);
            }
          });

          let newlistUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${listId}`;
          const { data, status } = await api.getdata(newlistUrl);
          if (status === 200 && data !== undefined) {
            if (listArray.length > 0) {
              await setshoworklist(listArray.concat(data));
            } else {
              flagArray.push(data);
              await setshoworklist(flagArray);
            }
          }

          $(".modal").remove();
          $(".modal-backdrop").remove();
          Swal.fire({
            text: msg,
            confirmButtonColor: "#7bc144",
            confirmButtonText: "Lukk",
          });

          setAktivDisplay(false);
          setAktivDisplay(true);
        }
      } catch (error) {
        console.log("errorpage API not working");
        console.error("error : " + error);

        setloading(false);
      }
    }
  };
  const listAllowDouble = async (e) => {
    // alert("Hi");
    // debugger;
    let listDouble = false;
    if (e.target.checked) {
      listDouble = true;
    } else {
      listDouble = false;
    }
    let listId = activUtvalglist.listId;
    setloading(true);
    let url = "";

    url =
      url +
      `UtvalgList/UpdateAllowDouble?listId=${listId}&allowDouble=${listDouble}`;
    try {
      const { data, status } = await api.putData(url);
      if (status === 200) {
        setloading(false);
        activUtvalglist.allowDouble = listDouble;
        let msg = "";
        if (data === true) {
          msg =
            "Utvalglisten er oppdatert .Utvalglisten sjekkes ikke for dobbeldekning ved gjenskapning.";
        } else {
          msg =
            "Utvalglisten er oppdatert.Utvalglisten sjekkes for dobbeldekning ved gjenskapning.";
        }

        $(".modal").remove();
        $(".modal-backdrop").remove();
        Swal.fire({
          text: msg,
          confirmButtonColor: "#7bc144",
          confirmButtonText: "Lukk",
        });

        //activUtvalglist.logo = logoName;
        setAktivDisplay(false);
        setAktivDisplay(true);
      }
    } catch (error) {
      console.log("errorpage API not working");
      console.error("error : " + error);

      setloading(false);
    }
    //}
  };
  const callback = (param) => {
    setFlag(param);
    setFlagNew(false);
  };
  return (
    <div className="pl-1 pt-1 nowrap p-0 m-0">
      {ModelNameED == "eyeIcon_ViewEdit" ? (
        <ForhandlerlistModel
          title={"Endre forhandlerpåtrykk"}
          id={"visEdit"}
          data={activUtvalglist.memberLists}
          memberUtvalgs={activUtvalglist.memberUtvalgs}
          parentCallback={callback}
        />
      ) : null}
      {Modal ? (
        <div
          className="modal fade bd-example-modal-lg"
          id="uxBtnDisconnect"
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
                          Listen "{activUtvalglist.name}" er knyttet til
                          basislisten <br /> "{activUtvalglist.basedOnName}", og
                          endres automatisk når denne <br /> basislisten endres.{" "}
                          <br />
                          <br /> Fristiller du listen, kan du endre listen
                          direkte, men det vil <br /> ikke lenger oppdateres
                          automatisk når basislisten oppdateres. <br />
                          Denne operasjonen kan ikke angres. <br /> <br />{" "}
                          Ønsker du å bryte knytningen til basislisten?
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
                            data-target="#disconnectListFromList"
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
        <div className="col-5">
          <label id="UtvalglistNavn" className="divDetailsName">
            Utvalgsliste{" "}
          </label>
        </div>
        <div className="col-7">
          <span id="" className="divDetailsName">
            {activUtvalglist.name}{" "}
          </span>
        </div>
      </div>
      <div className="row">
        <div className="col-5">
          <span id="listAntall" className="UtvaldivLabelText">
            Totalt antall
          </span>
        </div>
        <div className="col-7" style={{ textAlign: "left" }}>
          <span id="" className="divValueText_SelectionDetails">
            {activUtvalglist.antall}
          </span>
        </div>
      </div>

      <div className="row">
        <div className="col-5">
          <span id="Referansenr" className="UtvaldivLabelText">
            ListeID{" "}
          </span>
        </div>
        <div className="col-7" style={{ textAlign: "left" }}>
          <span id="" className="divValueText_SelectionDetails">
            {activUtvalglist.listId === 0 ||
            activUtvalglist.listId === undefined
              ? " - "
              : "L" + activUtvalglist.listId}
          </span>
        </div>
      </div>

      <div className="row">
        <div className="col-5">
          <span id="listKundeNr" className="UtvaldivLabelText">
            Kundenr{" "}
          </span>
        </div>
        <div className="col-7" style={{ textAlign: "left" }}>
          <span id="" className="divValueText_SelectionDetails">
            {activUtvalglist.kundeNummer === 0 ||
            activUtvalglist.kundeNummer === "0" ||
            activUtvalglist.kundeNummer === undefined
              ? ""
              : activUtvalglist.kundeNummer}
          </span>
        </div>
      </div>

      <div className="row">
        <div className="col-5">
          <span id="listForhandlerpaatrykk" className="UtvaldivLabelText">
            Forhandlerpåtryk
          </span>
        </div>
        <div className="col-5">
          <span id="" className="divValueText_SelectionDetails">
            {activUtvalglist.logo}
          </span>
        </div>

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
        </div>
      </div>
      <div className="row mb-1">
        <div className="col-5">
          <span id="uxIsBasislistLabel" className="UtvaldivLabelText">
            Basisliste
          </span>
        </div>
        {activUtvalglist.basedOn !== 0 &&
        activUtvalglist.basedOn !== undefined &&
        activUtvalglist.basedOn !== "" ? (
          // <div className="row">
          <div className="col-lg-6 col-md-12 divValueText">
            Utvalget er knyttet til
            <span
              id={activUtvalglist.basedOn}
              className="selectionUtvalgList"
              onClick={openKampList}
            >
              {" "}
              {activUtvalglist.basedOnName}
            </span>
            {/* </div>
            <div className="col-2"> */}
          </div>
        ) : (
          <div className="col-lg-6 col-md-12">
            <input
              type="checkbox"
              id="uxIsBasislistChkBox"
              value=""
              useRef={basisChecked}
              className="UtvaldivLabelText align-middle"
              checked={activUtvalglist.isBasis}
              onChange={(e) => listUpdateBasis(e)}
            />

            <span
              id="basislistutvalg"
              className="form-check-label label-text divValueText"
            >
              Lista er en basisliste{" "}
            </span>
            <span
              id={activUtvalglist.wasBasedOn}
              className="selectionUtvalgList"
              onClick={openPreviousKampList}
            >
              {activUtvalglist.wasBasedOnName}
            </span>
          </div>
        )}
        {activUtvalglist.basedOn !== 0 &&
        activUtvalglist.basedOn !== undefined &&
        activUtvalglist.basedOn !== "" ? (
          <div className="col-1 m-0 p-0">
            <img
              id="disconnect"
              className="KSPU_LinkButton"
              onClick={disconnectList}
              data-toggle="modal"
              data-target="#uxBtnDisconnect"
              alt="Frikoble denne listen fra basislisten"
              title="Frikoble denne listen fra basislisten"
              src={disConnect}
            />
          </div>
        ) : (
          <div className="col-1"></div>
        )}
      </div>
      <div className="row mb-2">
        <div className="col-5">
          <span id="uxIsDobbeldekning" className="UtvaldivLabelText">
            Dobbeldekning
          </span>
        </div>
        <div className="col-7">
          <input
            type="checkbox"
            id="uxIsDobbeldekningChkBox"
            value=""
            className="UtvaldivLabelText align-middle"
            checked={activUtvalglist.allowDouble}
            onChange={(e) => listAllowDouble(e)}
          />
          <span
            id="Dobbeldekning"
            className="form-check-label label-text divValueText"
          >
            Behold dobbeltdekning{" "}
          </span>
        </div>
      </div>
      {flag || flagNew ? (
        <div className="">
          <Utvalselection marginTop=".3rem" data={activUtvalglist} />
        </div>
      ) : null}
      <div className="mt-2">
        <TilHorerUtvalgList data={activUtvalglist} />
      </div>

      {activUtvalglist.isBasis ? (
        <div className="mt-2">
          <Campaign data={activUtvalglist} />
        </div>
      ) : null}
    </div>
  );
}

export default Selectionlist;
