import React, { useState, useContext, useEffect } from "react";
import Geogra_distribution_cart_click from "./Geogra_distribution_cart_click";
import Calender from "./Calendar";
import { KundeWebContext } from "../context/Context.js";

function Segmenter_distribution_click() {
  const [gramalertvisible, setgramalertvisible] = useState(false);
  const [cartclick, setcartclick] = useState(false);
  const [mmalertvisible, setmmalertvisible] = useState(false);
  const [ShowCalenderComp, setShowCalenderComp] = useState(false);
  const [buttonenable, setbuttonenable] = useState(true);
  const [distthickness, setdistthickness] = useState("");
  const [distthicknessmm, setdistthicknessmm] = useState("");
  const [radio1value, setradio1value] = useState(false);
  const [radio2value, setradio2value] = useState(true);
  const { Page, setPage } = useContext(KundeWebContext);
  const { Antallvalue, setAntallvalue } = useContext(KundeWebContext);

  const gramcheck = (e) => {
    let distthinckness = e.target.value;
    setdistthickness(e.target.value);
    if (parseInt(distthinckness) > 200) {
      setgramalertvisible(true);
    } else {
      setgramalertvisible(false);
    }
    if (mmalertvisible) {
      setbuttonenable(true);
    }
    if (gramalertvisible) {
      setbuttonenable(true);
    }

    if (distthickness && distthicknessmm) {
      setbuttonenable(false);
    }
  };
  const ShowCalender = () => {
    setShowCalenderComp(true);
  };
  const cartClick = () => {
    setcartclick(true);
  };
  const goback = () => {
    setPage("LagutvalgClick_segmenter");
  };
  const mmcheck = (e) => {
    let distthincknessmm = e.target.value;
    setdistthicknessmm(e.target.value);

    if (parseInt(distthincknessmm) > 5) {
      setmmalertvisible(true);
    } else {
      setmmalertvisible(false);
    }

    if (mmalertvisible) {
      setbuttonenable(true);
    }
    if (gramalertvisible) {
      setbuttonenable(true);
    } else {
      setbuttonenable(false);
    }
    if (distthickness && distthicknessmm) {
      setbuttonenable(false);
    }
  };
  return (
    <div className="col-5 p-2">
      <div className="padding_NoColor_B" style={{ cursor: "pointer" }}>
        <a id="uxHandlekurvSmall_uxLnkbtnHandlekurv" onClick={cartClick}>
          <div className="handlekurv handlekurvText pl-2">
            Du har 1 utvalg i bestillingen din.
          </div>
        </a>
      </div>

      {!cartclick ? (
        <div>
          {" "}
          <div id="uxTabDistribusjon">
            <div id="uxDistribusjon_uxContents">
              <div id="uxDistribusjon_uxInnerContents">
                <div className="titleWizard padding_NoColor_B">
                  Distribusjonsdetaljer
                </div>

                <div className="padding_Color_L_R_T_B">
                  <table style={{ width: "100%", overflow: "auto" }}>
                    <tbody>
                      <tr>
                        <td style={{ width: "75px" }} align="center">
                          <div className="divLabelText">Antall </div>
                        </td>
                        <td style={{ width: "50px" }} align="left">
                          <div
                            id="uxDistribusjon_uxDistAntallsInfo"
                            className="divValueText pl-4"
                          >
                            {Antallvalue}
                          </div>
                        </td>
                        <td style={{ width: "100px" }}></td>
                      </tr>
                      <tr>
                        <td align="center">
                          <div className="divLabelText">Vekt pr. sending</div>
                        </td>
                        <td>
                          <div>
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <input
                              type="text"
                              id="uxDistThickness"
                              onChange={gramcheck}
                              className="divnumberText DistWeight"
                              maxLength="3"
                              onkeyup="Distr.gui.thickness.validate();Distr.gui.thickness.onEnter(event);"
                              onblur="Distr.gui.thickness.thicknessWarning();"
                            />
                            &nbsp;{" "}
                            {gramalertvisible ? (
                              <span id="uxWeightValErr" className="red">
                                <b>!</b>
                              </span>
                            ) : null}
                          </div>
                        </td>
                        <td align="left">
                          <div className="bold">
                            gram{" "}
                            <span
                              id="uxDistribusjon_dvWeightLimitText"
                              className="gray"
                            >
                              (maks 200g){" "}
                            </span>
                          </div>
                        </td>
                      </tr>
                      <tr>
                        <td></td>
                        <td></td>
                        <td align="left">
                          <a
                            href="http://www.bring.no/radgivning/sende-noe/klargjoring/klargjoring-uadressert-post"
                            id="uxDistribusjon_uxVektInformation"
                            target="_blank"
                            className="KSPU_LinkButton_Url_KW"
                          >
                            Hjelp til vektberegning
                          </a>
                        </td>
                      </tr>

                      <tr>
                        <td align="center">
                          <div className="divLabelText">Tykkelse pr. sending</div>
                        </td>
                        <td>
                          <div>
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <input
                              type="text"
                              id="uxDistThicknessmm"
                              className="divnumberText DistWeight"
                              maxLength="2"
                              onChange={mmcheck}
                            />
                            &nbsp;{" "}
                            {mmalertvisible ? (
                              <span id="uxThicknessValErr" className="red">
                                <b>!</b>
                              </span>
                            ) : null}
                          </div>
                        </td>
                        <td align="left">
                          <div className="bold">
                            mm{" "}
                            <span
                              id="uxDistribusjon_dvThicknessLimitText"
                              className="gray"
                            >
                              (maks 5mm)
                            </span>
                          </div>
                        </td>
                      </tr>
                      <tr>
                        <td></td>
                        <td></td>
                        <td>
                          <input
                            name="uxDistribusjon$uxType"
                            type="hidden"
                            id="uxDistribusjon_uxType"
                            value="U"
                          />
                          <input
                            name="uxDistribusjon$uxDistId"
                            type="hidden"
                            id="uxDistribusjon_uxDistId"
                            value="2359412"
                          />
                          <input
                            name="uxDistribusjon$uxDistInfo"
                            type="hidden"
                            id="uxDistribusjon_uxDistInfo"
                            value="0|Null|01.01.0001|false|0"
                          />
                          <input
                            name="uxDistribusjon$uxDistMK"
                            type="hidden"
                            id="uxDistribusjon_uxDistMK"
                          />
                        </td>
                      </tr>
                    </tbody>
                  </table>

                  <div className="padding_NoColor_T_B">
                    <input
                      type="button"
                      id="uxBtShowDates"
                      className="KSPU_button_Gray"
                      width="210px"
                      style={{ width: "210px" }}
                      value="Vis mulige distribusjonsdatoer"
                      disabled={buttonenable}
                      onClick={ShowCalender}
                    />
                  </div>

                  <div id="uxDistError" className="Hide">
                    <div className="error WarningSign">
                      <div className="divErrorHeading"> Melding: </div>
                      <span
                        id="uxShowDist_uxErrorMsg"
                        className="divErrorText"
                      ></span>
                    </div>
                  </div>
                </div>

                <div id="uxDistKalenderBilde" className="Show">
                  <div className="paddingBig_NoColor_T clearFloat"></div>
                </div>

                {ShowCalenderComp ? (
                  <div id="uxDistKalender">
                    <div className="padding_NoColor_T clearFloat">
                      <div className="padding_Color_L_R_T_B">
                        <span id="uxSubTitle" className="subTitle">
                          Velg distribusjonsperiode
                        </span>

                        <div className="padding_Color_T_B sok-text">
                          &nbsp;&nbsp;
                          <input
                            type="radio"
                            id="uxrbRange"
                            name="disttype"
                            className="sok-text"
                            value="S"
                            checked={radio1value}
                            disabled={true}
                            onChange="Distr.calendar.changeDistType(this.value);"
                          />{" "}
                          &nbsp;&nbsp; tidliguke &nbsp;&nbsp;
                          <input
                            type="radio"
                            id="uxrbDate"
                            name="disttype"
                            className="sok-text"
                            value="B"
                            checked={radio2value}
                            onChange="Distr.calendar.changeDistType(this.value);"
                          />{" "}
                          &nbsp;&nbsp; midtuke
                        </div>

                        <div className="padding_Color_T" style={{ width: "5rem" }}>
                          <table>
                            <Calender page="DTPage" fontSize="11pt" />
                          </table>
                          {/* <select id="uxSelectMnd" className="divValueText" onchange="Distr.gui.calendar.setSelected();">
              </select> */}
                        </div>

                        <div className="padding_Color_B clearFloat">
                          <table width="100%">
                            <tbody>
                              <tr>
                                {/* <td align="left">
                  <a id="uxLnkPrevious" onClick="Distr.gui.wait.show();" className="KSPU_LinkInText_kw prevmnd">Forrige måned</a>
                 </td>
                 <td align="right">
                  <a id="uxLnkNext" onClick="Distr.gui.wait.show();" className="KSPU_LinkInText_kw nextmnd">Neste måned</a>
                 </td> */}
                              </tr>
                            </tbody>
                          </table>
                        </div>
                        <div style={{ fontSize: "13px" }}>
                          Ønsker du andre datoer enn de som er valgbare her, ta
                          kontakt med kundeservice på 04045.
                        </div>
                        <div className="paddingBig_NoColor_T clearFloat">
                          <div id="uxDistInfoStar" className="Hide">
                            <span className="red">*</span>
                            <span id="Span1" className="divValueText">
                              ) Distribusjon på en dag merket med stjerne
                              innebærer at enkelte budruter med for liten
                              kapasitet blir utelatt. Klikk datoen for å se
                              hvilke budruter det gjelder.
                            </span>
                          </div>

                          <div
                            id="uxDistFullBookedError"
                            className="clearFloat paddingBig_NoColor_T Hide"
                          >
                            <div className="error WarningSign">
                              <div
                                id="uxShowDist_uxFullBookedHeading"
                                className="divErrorHeading"
                              >
                                {" "}
                                Fullbooket budruter i denne perioden
                              </div>
                              <span
                                id="uxShowDist_uxFullErrorMsg"
                                className="divErrorText"
                              >
                                Systemet klarte ikke å hentet kapasitetsdata,
                                prøv igjen senere.
                              </span>
                              <span
                                id="uxShowDist_uxFullBookedMsg"
                                className="divValueText"
                              >
                                Disse budrutene er markert med rødt i kartet.
                                Klikk på dem for mer informasjon eller
                                <a
                                  id="uxShowDist_uxDisplayDetails"
                                  className="KSPU_LinkButton_Url"
                                  href="javascript:Distr.ruteInfo.show()"
                                >
                                  se oversikt over rutene...
                                </a>
                              </span>
                              <div className="padding_NoColor_T_B">
                                <span
                                  id="uxShowDist_uxFullBookeRemove"
                                  className="divValueText clearFloat"
                                >
                                  <input
                                    type="checkbox"
                                    name="acceptRemove"
                                    id="acceptRemove"
                                    value="Ja"
                                    onClick="Distr.gui.acceptRemove.clicked();"
                                  />{" "}
                                  Jeg godtar at de fullbookede rutene fjernes
                                  fra bestillingen
                                </span>
                                <span
                                  id="uxShowDist_uxFullBookeBasedOn"
                                  className="diverrorText clearFloat"
                                >
                                  Utvalget kan ikke endres, du må velge en dag
                                  med full kapasitet eller frikoble utvalget.
                                </span>
                              </div>
                              <span
                                id="uxShowDist_uxFullBookedNext"
                                className="divValueText"
                              >
                                <br />
                                Ønsker du ikke å utelate disse budrutene må du
                                endre distribusjonsdato. Neste ledige er{" "}
                              </span>
                            </div>
                          </div>
                        </div>

                        <div
                          className="padding_Color_T_B divValueText"
                          id="uxDistOutofDateRangeMsg"
                        ></div>
                      </div>
                    </div>
                  </div>
                ) : null}

                <div className="paddingBig_NoColor_T clearFloat">
                  <div className="div_left">
                    <input
                      type="submit"
                      name="uxDistribusjon$uxBtnDistBack"
                      value="Tilbake"
                      onClick={goback}
                      id="uxDistribusjon_uxBtnDistBack"
                      className="KSPU_button_Gray"
                    />
                  </div>
                  <div>
                    <table border="0" cellpadding="0" cellspacing="0">
                      <tbody>
                        <tr>
                          <td style={{ verticalAlign: "middle" }}>
                            <input
                              type="button"
                              name="uxDistribusjon$uxBtnUpdateMap$uxButton"
                              value=""
                              onClick='Distr.gui.wait.show();;return KSPU_ClientClick(null, &apos;KSPU_DoCallback(\&apos;uxDistribusjon$uxBtnUpdateMap\&apos;,\&apos;ButtonClicked\&apos;,processCallbackResult,null,KSPUCallbackError,false)&apos;);WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("uxDistribusjon$uxBtnUpdateMap$uxButton", "", true, "", "", false, true))'
                              id="uxDistribusjon_uxBtnUpdateMap_uxButton"
                              className="Hide"
                            />
                          </td>
                          <td
                            style={{
                              paddingLeft: "8px",
                              verticalAlign: "middle",
                            }}
                          ></td>
                        </tr>
                      </tbody>
                    </table>
                  </div>
                  <div className="div_right">
                    <input
                      type="submit"
                      name="uxDistribusjon$uxDistSetDelivery"
                      value="Angi innleveringsdetaljer"
                      id="uxDistribusjon_uxDistSetDelivery"
                      disabled="disabled"
                      className="KSPU_button-kw"
                      style={{ width: "175px" }}
                    />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      ) : (
        <Geogra_distribution_cart_click />
      )}
    </div>
  );
}
export default Segmenter_distribution_click;
