import React, { useEffect, useState, useContext } from "react";
import { KundeWebContext } from "../../context/Context";
import "./Apne_Button_Completedorders.scss";
import swal from "sweetalert";
import $ from "jquery";
import api from "../../services/api.js";
import { kundeweb_utvalg } from "../KspuConfig";

function Apne_Button_Completedorders() {
  const { username_kw, setusername_kw } = useContext(KundeWebContext);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  const { UtvalgID, setUtvalgID } = useContext(KundeWebContext);
  const [utvalgname, setutvalgname] = useState("");
  const { Antallvalue, setAntallvalue } = useContext(KundeWebContext);
  const [apnetext, setapnetext] = useState("");
  const { Page, setPage } = useContext(KundeWebContext);
  const [warninputvalue_1, setwarninputvalue_1] = useState(
    utvalgapiobject[0].name ? utvalgapiobject[0].name : ""
  );
  const [nomessagediv, setnomessagediv] = useState(true);
  const [desinput, setdesinput] = useState("");
  const [melding, setmelding] = useState(false);
  const [errormsg, seterrormsg] = useState("");

  // const [apnetext,setapnetext] = useState(utvalgapiobject[0].name ? utvalgapiobject[0].name+new Date() :utvalgapiobject[0].utvalgName+ (new Date().toUTCString().replace("GMT","")))

  useEffect(() => {
    if (utvalgapiobject[0].utvalgId) {
      setUtvalgID(utvalgapiobject[0].utvalgId);
      setutvalgname(
        utvalgapiobject[0].name
          ? utvalgapiobject[0].name
          : utvalgapiobject[0].utvalgName
      );
      setAntallvalue(
        utvalgapiobject[0].totalAntall
          ? utvalgapiobject[0].totalAntall
          : utvalgapiobject[0].antall
      );
    }
  }, []);
  const desinputonchange = () => {
    let desctextvalue = document.getElementById("desctext").value;
    setdesinput(desctextvalue);
  };
  const GotoMain = () => {
    setPage("");
  };
  const warninput = () => {
    setmelding(false);
    let textinput = document.getElementById("warntext").value;
    setwarninputvalue_1(textinput);
  };
  const LagreClick = async () => {
    setnomessagediv(false);
    //setPage("Apne_Button_Click")
    const { data, status } = await api.getdata(
      `Utvalg/UtvalgNameExists?utvalgNavn=${warninputvalue_1}`
    );
    if (status === 200) {
      if (data == true) {
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
          console.log(e);
        }
        //    setPage("Geogra_distribution_click")
      }
    }
  };
  const Apnetextbox = () => {
    let textboxvalue = document.getElementById(
      "uxShowUtvalgDetails_uxForhandlerpaatrykk"
    ).value;
    setapnetext(textboxvalue);
  };
  const LargeKopiClick = () => {
    setwarninputvalue_1(utvalgapiobject[0].name ? utvalgapiobject[0].name : "");
  };

  return (
    <div className="col-5 p-2">
      <div className="padding_NoColor_B cursor">
        <a id="uxHandlekurvSmall_uxLnkbtnHandlekurv">
          <div className="handlekurv handlekurvText pl-2">
            Du har 1 utvalg i bestillingen din.
          </div>
        </a>
      </div>

      <div className="padding_Color_L_R_T_B">
        <div className="AktivtUtvalg">
          <div className="AktivtUtvalgHeading">
            <span id="uxShowUtvalgDetails_uxLblSavedUtvalgName">
              {utvalgname}
            </span>

            {/* <span id="uxShowUtvalgDetails_uxLblSavedUtvalgName">{  (utvalgapiobject[0].name)+new Date().toUTCString().replace("GMT","") ||  (utvalgapiobject[0].utvalgName)+new Date().toUTCString().replace("GMT","")}</span> */}
          </div>

          <div>
            <div className="gray">
              ID:
              <span id="uxShowUtvalgDetails_uxRefNr" className="gray">
                {utvalgapiobject[0].utvalgId || utvalgapiobject[0].listId}
              </span>
            </div>
          </div>
        </div>

        <div>
          {" "}
          <label className="form-check-label label-text" htmlFor="Hush">
            {" "}
            Husholdninger{" "}
          </label>
          <span className="divValueTextBold div_right">{Antallvalue}</span>
          <div className="underline_kw"></div>
          <span className="divValueTextBold div_right">{Antallvalue}</span>
        </div>
        <p></p>
        {nomessagediv ? (
          <div className="pr-3">
            <div className="error WarningSign">
              <div className="divErrorHeading">Melding:</div>
              <p id="uxKjoreAnalyse_uxLblMessage" className="divErrorText_kw">
                Valgt bestilling har allerede et ordrenummer. Du kan kopiere
                bestillingen.
              </p>
            </div>
          </div>
        ) : null}

        <div
          className="modal show"
          id="exampleModal-1"
          tabIndex="-1"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
          aria-hidden="true"
          data-backdrop="false"
        >
          <div className="modal-dialog modal-dialog-centered " role="document">
            <div className="modal-content">
              <div className="">
                {/* <div className=""> */}
                <div className=" divDockedPanelTop">
                  <span className="dialog-kw" id="exampleModalLabel">
                    LAGRE UTVALG{" "}
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
                  <label className="divValueText_kw">Utvalgsnavn</label>
                  <input
                    type="text"
                    maxLength="50"
                    value={warninputvalue_1}
                    onChange={warninput}
                    id="warntext"
                    className="inputwidth"
                  />
                  <br />
                  <label className="divValueText_kw">Beskrivelse</label>
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

        <div
          className="modal fade bd-example-modal-lg"
          id="exampleModal"
          tabIndex="-1"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
          aria-hidden="true"
        >
          <div className="modal-dialog modal-dialog-centered " role="document">
            <div className="modal-content">
              <div className="Common-modal-header">
                <div className="divDockedPanel">
                  <div className=" divDockedPanelTop">
                    <span className="dialog-kw" id="exampleModalLabel">
                      Advarsel
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
                              &nbsp; Skal utvalget slettes?
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
                                Nei
                              </button>
                              {/* </td>
        <td></td>
        <td> */}{" "}
                              &nbsp;&nbsp;
                              <button
                                type="button"
                                onClick={""}
                                className="modalMessage_button"
                                data-dismiss="modal"
                              >
                                Ja
                              </button>
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

        <div className="padding_NoColor_T clearFloat">
          <div className="bold">Beskrivelse av utvalget</div>
          <div>
            <input
              type="text"
              value={apnetext}
              onChange={Apnetextbox}
              maxLength="50"
              id="uxShowUtvalgDetails_uxForhandlerpaatrykk"
              className="divValueText_kw tablewidth"
            />
          </div>
          <div className="gray">
            Gi en beskrivelse som gjør det lett å identifisere korrekt sending;
            skriv inn avsender og evt kjennetegn ved sendingen. Denne
            beskrivelsen vil du også finne igjen når pakningsmateriell skal
            produseres.
          </div>
        </div>
        <br />
      </div>
      <br />
      <div className="div_right">
        <input
          type="submit"
          name="uxDistribusjon$uxDistSetDelivery"
          value="Lagre Kopi"
          id="uxDistribusjon_uxDistSetDelivery"
          className="KSPU_button-kw"
          data-toggle="modal"
          data-target="#exampleModal-1"
          onClick={LargeKopiClick}
        />
      </div>
      <div className="div_left">
        <a
          className="KSPU_LinkButton_Url_KW pl-2"
          onClick={GotoMain}
        >
          Avbryt
        </a>
      </div>
      <br />
      <br />

      <div className="div_left">
        <span className="bold">Du kan også..</span>

        <br />
      </div>
      <br />

      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <a
          id="uxBtnAddUtvalg"
          className="KSPU_LinkButton1_Url prevmnd"
          href={process.env.REACT_APP_APNE_CLICK_SOK_PAGE_LINK_URL}
        >
          <b>Skriv ut utvalget</b>
        </a>
      </div>
    </div>
  );
}
export default Apne_Button_Completedorders;
