import React, { useEffect, useState, useContext } from "react";
import { KundeWebContext } from "../../context/Context";
import "./lestill_Click_Component-kw.scss";
import symbol1 from "../../assets/images/symboler/symbol1.JPG";
import white from "../../assets/images/white.gif";
import readmore from "../../assets/images/read_more.gif";
import api from "../../services/api.js";
import { kundeweb_utvalg } from "../KspuConfig";
//import { datadogLogs } from "@datadog/browser-logs";
import LestillClickKw from "./lestillClickKw";
import { StandardreportKw } from "../apne_Button_Click-kw/standardreportKw";
import { NumberFormat } from "../../common/Functions";

function Lestill_Click_Component() {
  const { Page_P, setPage_P } = useContext(KundeWebContext);
  const { Page, setPage } = useContext(KundeWebContext);
  const { warninputvalue, setwarninputvalue } = useContext(KundeWebContext);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  const { UtvalgID, setUtvalgID } = useContext(KundeWebContext);
  const { utvalgname, setutvalgname } = useContext(KundeWebContext);
  const { Antallvalue, setAntallvalue } = useContext(KundeWebContext);
  const [warninputvalue_1, setwarninputvalue_1] = useState("");
  const [desinput, setdesinput] = useState("");
  const [melding, setmelding] = useState(false);
  const [errormsg, seterrormsg] = useState("");
  const { newhome, setnewhome } = useContext(KundeWebContext);
  const [lestillUtvalgName, setLestillUtvalgName] = useState("");
  const [skrivUtvalgetName, setSkrivUtvalgetName] = useState("");
  const { username_kw, setusername_kw } = useContext(KundeWebContext);

  useEffect(() => {
    setUtvalgID(
      utvalgapiobject.antall === undefined ||
        typeof utvalgapiobject.antall == undefined
        ? utvalgapiobject.utvalgId
        : utvalgapiobject.listId
    );
    setAntallvalue(
      utvalgapiobject.antall === undefined ||
        typeof utvalgapiobject.antall == undefined
        ? utvalgapiobject.totalAntall
        : utvalgapiobject.antall
    );
  }, []);

  const AngiClick = () => {
    setPage_P("Lestill_Click_Component");
    setPage("Geogra_distribution_click");
  };
  const EndreClick = () => {
    setPage("Apne_Button_Click");
  };
  const GotoMain = () => {
    setnewhome(false);
    setPage("");
  };
  const lestillUtvalg = () => {
    setLestillUtvalgName("lestillUtvalg");
  };
  const skrivUtvalget = () => {
    setSkrivUtvalgetName("skrivUtvalget");
  };
  const LagreClick = async () => {
    // setnomessagediv(false);
    //setPage("Apne_Button_Click")
    const { data, status } = await api.getdata(
      `Utvalg/UtvalgNameExists?utvalgNavn=${warninputvalue_1}`
    );
    if (status === 200) {
      if (data == true) {
        //datadogLogs.logger.info("UtvalgnameExistsResult", data);
        setmelding(true);
        let msg = `Utvalget ${warninputvalue_1} eksisterer allerede. Velg et annet utvalgsnavn.`;
        seterrormsg(msg);
      } else {
        let saveOldReoler = "false";
        let skipHistory = "false";
        let forceUtvalgListId = 0;
        let name = username_kw;
        let url = `Utvalg/SaveUtvalg?userName=${name}&`;
        url = url + `saveOldReoler=${saveOldReoler}&`;
        url = url + `skipHistory=${skipHistory}&`;
        url = url + `forceUtvalgListId=${forceUtvalgListId}`;
        try {
          let A = kundeweb_utvalg();
          A.name = warninputvalue_1;
          A.kundeNavn = desinput;
          A.totalAntall = Antallvalue;
          // A.reoler[0].description = describtion;
          // A.reoler[0].antall.households = HouseholdSum;
          A.criterias[0].criteriaType = 19;
          A.criterias[0].criteria = "Geografipulkkliste";
          const { data, status } = await api.postdata(url, A);
          if (status === 200) {
            let utvalgID = data.utvalgId;
            setPage("Apne_Button_Click");
          }
        } catch (e) {
          //datadogLogs.logger.info("saveutvalgError", e);
          console.log(e);
        }
        //    setPage("Geogra_distribution_click")
      }
    }
  };
  const warninput = () => {
    setmelding(false);
    let textinput = document.getElementById("warntext").value;
    setwarninputvalue_1(textinput);
  };
  const desinputonchange = () => {
    let desctextvalue = document.getElementById("desctext").value;
    setdesinput(desctextvalue);
  };
  const cartClick = () => {
    setPage("CartClick");
  };

  return (
    <div className="col-5 p-2">
      <div className="padding_NoColor_B cursor">
        <a onClick={cartClick}>
          <div className="handlekurv handlekurvText pl-2">
            Du har 1 utvalg i bestillingen din.
          </div>
        </a>
      </div>
      <div id="uxShowUtvalgListDetails_uxContents">
        <div id="uxShowUtvalgListDetails_uxInnerContents">
          <div className="padding_Color_L_R_T_B clearFloat">
            <div className="titleWizard padding_NoColor_B">
              "
              <span
                id="uxShowUtvalgListDetails_uxLblListName"
                className="green"
              >
                {utvalgapiobject.name}
              </span>
              " har{" "}
              <span
                id="uxShowUtvalgListDetails_uxLblListCountUtvalg"
                className="green"
              >
                1
              </span>{" "}
              utvalg
            </div>

            {/* ---modals  starting */}
            <div
              className="modal fade bd-example-modal-lg"
              id="exampleModal"
              tabIndex="-1"
              role="dialog"
              aria-labelledby="exampleModalCenterTitle"
              aria-hidden="true"
            >
              <div className="modal-dialog " role="document">
                <div className="modal-content">
                  <div className="Common-modal-header">
                    <div className="divDockedPanel">
                      <div className=" divDockedPanelTop">
                        <span className="dialog-kw" id="exampleModalLabel">
                          KVITTERING
                        </span>
                        <button
                          type="button"
                          className="close"
                          data-dismiss="modal"
                          aria-label="Close"
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
                                  &nbsp; Lagring utført.
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
                                    Lukk
                                  </button>{" "}
                                </div>
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
            </div>
            {/* -----savemodal ending---- */}
            {/* ----------selection modal starting----- */}
            <div
              className="modal show"
              id="exampleModal-1"
              tabIndex="-1"
              role="dialog"
              aria-labelledby="exampleModalCenterTitle"
              aria-hidden="true"
              data-backdrop="false"
            >
              <div className="modal-dialog" role="document">
                <div className="modal-content">
                  <div className="">
                    {/* <div className=""> */}
                    <div className=" divDockedPanelTop">
                      <span className="dialog-kw" id="exampleModalLabel">
                        Angi navn på nytt utvalg{" "}
                      </span>
                      <button
                        type="button"
                        className="close pr-2"
                        data-dismiss="modal"
                        aria-label="Close"
                      >
                        <span aria-hidden="true">&times;</span>
                      </button>
                    </div>
                    <div className="View_modal-body-appneet pl-2">
                      <p></p>
                      {melding ? (
                        <span className=" sok-Alert-text pl-1">{errormsg}</span>
                      ) : null}
                      {melding ? <p></p> : null}
                      <label className="divValueText">Utvalgsnavn</label>
                      <input
                        type="text"
                        maxLength="50"
                        value={warninputvalue_1}
                        onChange={warninput}
                        id="warntext"
                        className="inputwidth"
                      />
                      <br />
                      <label className="divValueText">Forhandlerpåtrykk </label>
                      <input
                        type="text"
                        maxLength="50"
                        value={desinput}
                        onChange={desinputonchange}
                        id="desctext"
                        className="inputwidth"
                      />

                      <p></p>

                      <div className="div_left">
                        <input
                          type="submit"
                          name="DemografiAnalyse1$uxFooter$uxBtForrige"
                          value="Avbryt"
                          data-dismiss="modal"
                          className="KSPU_button_Gray"
                        />
                      </div>
                      <div className="div-right">
                        <input
                          type="submit"
                          name="uxDistribusjon$uxDistSetDelivery"
                          value="Lagre"
                          onClick={LagreClick}
                          id="uxDistribusjon_uxDistSetDelivery"
                          className="KSPU_button-kw float-right"
                        />
                      </div>

                      <br />
                      <br />
                    </div>
                  </div>
                </div>
              </div>
            </div>
            {/* ---------selection modal ending --------- */}

            <div id="uxShowUtvalgListDetails_uxHandlekurv_uxContents">
              <div className="padding_NoColor_B clearFloat">
                <table className="tablestyle">
                  <tbody>
                    <tr>
                      <td>
                        <div id="uxShowUtvalgListDetails_uxHandlekurv_uxDivGridContainer">
                          <div>
                            <table
                              cellSpacing="0"
                              rules="rows"
                              border="0"
                              id="uxShowUtvalgListDetails_uxHandlekurv_uxGrdMemberUtv"
                              className="cellcollapse"
                            >
                              <tbody>
                                <tr className="GridView_Row_kw">
                                  <td align="center">
                                    <img
                                      id="uxShowUtvalgListDetails_uxHandlekurv_uxGrdMemberUtv_ctl02_uxKartSymbol"
                                      src={symbol1}
                                      className="imgstyle"
                                    />
                                  </td>
                                  <td>
                                    <a className="sok-text">{utvalgname}</a>
                                  </td>
                                  <td align="right">
                                    <a
                                      className="KSPU_LinkButton_Url sok-text"
                                      onClick={EndreClick}
                                    >
                                      Endre
                                    </a>
                                  </td>
                                  <td align="center">
                                    <img
                                      id="uxShowUtvalgListDetails_uxHandlekurv_uxGrdMemberUtv_ctl02_uxDobbeldekningSymbol"
                                      src={white}
                                      className="imgstyle1"
                                    />
                                  </td>
                                  <td align="right" className="tdwidth">
                                    {NumberFormat(Antallvalue)}
                                  </td>
                                </tr>
                              </tbody>
                            </table>
                          </div>
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <table className="tdwidth1">
                  <tbody>
                    <tr className="bold">
                      <td className="sok-text-1">
                        Totalt, alle utvalg i bestillingen
                      </td>

                      <td className="tdwidth2">
                        <span id="uxShowUtvalgListDetails_uxHandlekurv_uxLblTotAnt">
                          {NumberFormat(Antallvalue)}
                        </span>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>

            <div>
              <div>
                <div className="gray">
                  ID:{" "}
                  <span
                    id="uxShowUtvalgListDetails_uxLblRefNr"
                    className="gray"
                  >
                    {UtvalgID}
                  </span>
                </div>
              </div>

              <div>
                <a
                  id="uxShowUtvalgListDetails_uxEditForhandler"
                  className="KSPU_LinkButton_Url sok-text"
                  href="#"
                >
                  Endre beskrivelse
                </a>
              </div>
            </div>
          </div>

          <div className="padding_NoColor_T paddingBig_NoColor_B clearFloat">
            <div className="div_left">
              <a
                id="uxShowUtvalgListDetails_uxBtnAvbryt"
                className="KSPU_LinkButton_Url_KW"
                onClick={GotoMain}
              >
                Avbryt
              </a>
            </div>

            <div className="div_right">
              <input
                type="submit"
                name="uxDistribusjon$uxDistSetDelivery"
                value="Angi distribusjonsdetaljer"
                id="uxDistribusjon_uxDistSetDelivery"
                onClick={AngiClick}
                className="KSPU_button-kw"
                style={{ width: "175px" }}
              />
            </div>
          </div>
          <div>
            {lestillUtvalgName === "lestillUtvalg" ? (
              <LestillClickKw
                title={"lestillUtvalg"}
                id={"lestillUtvalg"}
                // type=""
                // isList={utvalglistcheck}
              />
            ) : null}
            {skrivUtvalgetName === "skrivUtvalget" ? (
              <StandardreportKw
                title={"skrivUtvalget"}
                id={"skrivUtvalget"}
                // type=""
                // isList={utvalglistcheck}
              />
            ) : null}
          </div>

          <table className="wizUnfilled paddingBig_NoColor_T clearFloat">
            <tbody>
              <tr>
                <td className="bold">Du kan også..</td>
              </tr>

              <tr>
                <td>
                  <a
                    onClick="showWaitAnimation();"
                    id="uxShowUtvalgListDetails_uxAddMoreToList"
                    className="KSPU_LinkButton1_Url margin"
                  >
                    <img src={readmore} />
                    &nbsp;Legg flere utvalg til bestillingen
                  </a>
                </td>
              </tr>

              <tr>
                <td>
                  <a
                    id="uxShowUtvalgListDetails_uxAddMoreToList"
                    className="KSPU_LinkButton1_Url margin"
                    data-toggle="modal"
                    data-target="#lestillUtvalg"
                    onClick={lestillUtvalg}
                  >
                    <img src={readmore} />
                    &nbsp; Kopiere bestillingen
                  </a>
                </td>
              </tr>

              <tr>
                <td>
                  <a
                    id="uxShowUtvalgListDetails_uxAddMoreToList"
                    className="KSPU_LinkButton1_Url margin"
                    data-toggle="modal"
                    data-target="#skrivUtvalget"
                    onClick={skrivUtvalget}
                  >
                    <img src={readmore} />
                    &nbsp; Skriv ut bestillingen
                  </a>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

export default Lestill_Click_Component;
