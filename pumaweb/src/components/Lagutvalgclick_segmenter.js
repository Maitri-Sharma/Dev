import React, { useEffect, useState, useRef, useContext } from "react";
import "../App.css";
import { KundeWebContext } from "../context/Context";

function LagutvalgClick_segmenter(props, { parentCallback }) {
  const { householdcheckbox, sethouseholdcheckbox } =
    useContext(KundeWebContext);
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);
  const { HouseholdSum, setHouseholdSum } = useContext(KundeWebContext);
  const { BusinessSum, setBusinessSum } = useContext(KundeWebContext);
  const { Antallvalue, setAntallvalue } = useContext(KundeWebContext);
  const { Page, setPage } = useContext(KundeWebContext);
  const [melding, setmelding] = useState(false);
  const [describtion, setdescribtion] = useState("");
  const [selection, setselection] = useState("Påbegynt utvalg");

  const Enterdescribtion = () => {
    let desc = document.getElementById("describtion").value;
    setdescribtion(desc);
    setmelding(false);
  };
  const GotoMain = () => {
    setPage("");
  };
  const distribution_click = () => {
    if (describtion == "" || selection == "") {
      setmelding(true);
    } else {
      setPage("Geogra_distribution_click");
    }
  };
  //  useEffect(() => {

  // businesscheckbox ?  setAntallvalue(parseInt(HouseholdSum)+parseInt(BusinessSum)):
  // setAntallvalue(parseInt(HouseholdSum))
  //     }, []);

  const goback = () => {
    setPage("VeglGeografiskOmrade_kw");
  };

  const CheckInput = () => {
    let value = document.getElementById("describtion").value;
    if (value == "") {
      setmelding(true);
    }
  };
  const Enterselection = () => {
    let selectionText = document.getElementById("selection").value;
    setselection(selectionText);
  };
  return (
    // <HousholdContext.Consumer>
    <div className="col-5 p-2">
      <div className="padding_NoColor_B">
        <span className="title">Navngi og lagre utvalget</span>
      </div>
      <div>
        <div style={{ display: "" }}>
          <div className="padding_Color_L_R_T_B">
            <div className="AktivtUtvalg">
              <div className="AktivtUtvalgHeading">
                <span className="">Utvalg</span>
              </div>
            </div>

            {melding ? (
              <div className="pr-3">
                <div className="error WarningSign">
                  <div className="divErrorHeading">Melding:</div>
                  {(describtion == "" && selection == "") || selection == "" ? (
                    <span
                      id="uxKjoreAnalyse_uxLblMessage"
                      className="divErrorText_kw"
                    >
                      {" "}
                      Utvalget tilfredstiller ikke kriterier for å kunne lagres.
                    </span>
                  ) : null}

                  {describtion == "" ? (
                    <span
                      id="uxKjoreAnalyse_uxLblMessage"
                      className="divErrorText_kw"
                    >
                      Beskrivelse av utvalget må ha minst 3 tegn.
                    </span>
                  ) : null}
                </div>
              </div>
            ) : null}

            <div className="pt-3">
              {householdcheckbox ? (
                <div>
                  {" "}
                  <label className="form-check-label label-text" htmlFor="Hush">
                    {" "}
                    Husholdninger{" "}
                  </label>
                  <span
                    id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                    className="divValueTextBold div_right"
                  >
                    {HouseholdSum}
                  </span>
                </div>
              ) : null}
              {businesscheckbox ? (
                <div>
                  <label className="form-check-label label-text" htmlFor="Virk">
                    {" "}
                    Virksomheter{" "}
                  </label>
                  <span
                    id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                    className="divValueTextBold div_right"
                  >
                    {BusinessSum}
                  </span>
                </div>
              ) : null}
            </div>
            <div
              style={{
                width: "370px",
                borderTop: "solid 1px black",
                fontWeight: "bold",
                padding: "0px"
              }}
            >
              &nbsp;
              <span
                id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblAntallSumText"
                className="divValueTextBold  div_left"
              >
                Totalt for utvalget
              </span>
              {businesscheckbox && householdcheckbox ? (
                <span
                  id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                  className="divValueTextBold div_right"
                >
                  {parseInt(HouseholdSum) + parseInt(BusinessSum)}
                </span>
              ) : businesscheckbox ? (
                <span
                  id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                  className="divValueTextBold div_right"
                >
                  {parseInt(BusinessSum)}
                </span>
              ) : (
                <span
                  id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                  className="divValueTextBold div_right"
                >
                  {parseInt(HouseholdSum)}
                </span>
              )}
            </div>

            {/* <div id="uxGeografiAnalyse_ShowUtvalgDetails1_uxAntallsInfo" className="clearFloat" cssclass="divValueText"><table style={{width: "100px"}}><tbody><tr><td style={{width: "294px",padding:"0px"}} className="form-check-label label-text">Husholdninger</td>
                <span id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum" className="divValueTextBold div_right">0</span>} </tr><tr style={{fontWeight:"bold"}}><td style={{width:"294px",borderTop: "solid 1px black",fontWeight:"bold", padding: "0px"}}>&nbsp;
                </td>
                <td style={{width:"75px",borderTop: "solid 1px black",fontWeight:"bold", padding: "0px",textAlign:"right"}}>>6 179</td></tr></tbody></table></div> */}

            <div className="padding_NoColor_T clearFloat">
              <div className="bold">Gi utvalget et navn</div>
              <div>
                <input
                  style={{ width: "22rem" }}
                  type="text"
                  value={selection}
                  maxLength="50"
                  id="selection"
                  onChange={Enterselection}
                  className="divValueText"
                />
              </div>
              <div className="gray">
                Navnet gjør at du kan søke opp og benytte utvalget senere.
              </div>
            </div>

            <div className="padding_NoColor_T clearFloat">
              <div className="bold">Beskrivelse av utvalget</div>
              <div>
                <input
                  type="text"
                  style={{ width: "22rem" }}
                  maxLength="50"
                  id="describtion"
                  className="divValueText"
                  onChange={Enterdescribtion}
                />
              </div>
              <div className="gray">
                Gi en beskrivelse som gjør det lett å identifisere korrekt
                sending; skriv inn avsender og evt kjennetegn ved sendingen.
                Denne beskrivelsen vil du også finne igjen når pakningsmateriell
                skal produseres.
              </div>
            </div>

            <div className="paddingBig_NoColor_T clearFloat">
              <div>
                <table style={{ width: "100px" }}>
                  <tbody>
                    <tr>
                      <td style={{ width: "80px" }}></td>
                      <td style={{ width: "160px" }}>
                        <div className="div_right"></div>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <div className="padding_NoColor_T paddingBig_NoColor_B clearFloat">
            <div className="div_left">
              <input
                type="submit"
                value="Tilbake"
                onClick={goback}
                className="KSPU_button_Gray"
              />
              <br />

              <div className="padding_NoColor_T">
                <a
                  className="KSPU_LinkButton_Url_KW pl-2"
                  onClick={GotoMain}
                >
                  Avbryt
                </a>
              </div>
            </div>
            <div className="div_right">
              <div>
                <input
                  type="submit"
                  value="Angi distribusjonsdetaljer"
                  onClick={distribution_click}
                  id="uxGeografiAnalyse_ShowUtvalgDetails1_uxLagreDistribusjon"
                  className="KSPU_button-kw div_right"
                  style={{ width: "175px" }}
                />

                <input
                  type="submit"
                  name="uxGeografiAnalyse$ShowUtvalgDetails1$uxLagreKopiDialog"
                  value="Lagre kopi"
                  onClick="hideFloatingPanel('uxDialogs_uxConnectList_uxListekoblingWindow', false, null);hideFloatingPanel('uxDialogs_uxListReportCriteria_uxRapportKriterierListe', false, null);hideFloatingPanel('uxDialogs_uxReportCriteria_uxRapportKriterier', false, null);hideFloatingPanel('uxDialogs_uxSaveListLogo_uxSaveLogoWindow', false, null);hideFloatingPanel('uxDialogs_uxSaveLogo_uxSaveLogoWindow', false, null);hideFloatingPanel('uxDialogs_uxSaveUtvalgAnonymously_uxSaveWindow', false, null);hideFloatingPanel('uxDialogs_uxCustomerSearchResult_uxCustomerResultWindow', false, null);hideFloatingPanel('uxDialogs_uxLagrePanelMultiple_uxSaveWindow', false, null);hideFloatingPanel('uxDialogs_uxConnectUtvalgListToUtvalgList_uxDialogWindow', false, null);hideFloatingPanel('uxDialogs_uxListSearchResult_uxFindListResultsWindow', false, null);hideFloatingPanel('uxDialogs_uxLagreListe_uxLagreListe', false, null);hideFloatingPanel('uxDialogs_uxCopyList_uxLagreListeSom', false, null);hideFloatingPanel('uxDialogs_uxBasedOn_uxCampaignsPanel', false, null);hideFloatingPanel('uxDialogs_uxCreateCampaignList_uxCreateCampaignPanel', false, null);hideFloatingPanel('uxDialogs_uxLagrePanelCampaign_uxSaveWindowCampaign', false, null);hideFloatingPanel('uxDialogs_uxListReportCriteriaDistr_uxRapportKriterierListeDistr', false, null);showFloatingPanel('uxDialogs_uxSaveUtvalg_uxSaveWindow', false, null);;return false;WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;uxGeografiAnalyse$ShowUtvalgDetails1$uxLagreKopiDialog&quot;, &quot;&quot;, true, &quot;&quot;, &quot;&quot;, false, false))"
                  id="uxGeografiAnalyse_ShowUtvalgDetails1_uxLagreKopiDialog"
                  className="KSPU_button"
                  style={{ width: "150px", display: "none" }}
                />

                <span
                  id="uxGeografiAnalyse_ShowUtvalgDetails1_lblhdnCopyUtvalg"
                  className="divValueTextBold div_right"
                  style={{ display: "none" }}
                >
                  0
                </span>
              </div>
              <div>
                <input
                  type="submit"
                  name="uxGeografiAnalyse$ShowUtvalgDetails1$uxAddMoreToList"
                  value="Legg til flere utvalg"
                  onClick='showWaitAnimation();WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("uxGeografiAnalyse$ShowUtvalgDetails1$uxAddMoreToList", "", true, "", "", false, false))'
                  id="uxGeografiAnalyse_ShowUtvalgDetails1_uxAddMoreToList"
                  className="KSPU_button_Gray div_right clearFloat"
                  style={{ width: "150px", marginTop: "10px" }}
                />
              </div>
            </div>
          </div>
          <table className="wizUnfilled paddingBig_NoColor_T clearFloat">
            <tbody>
              <tr>
                <td className="bold">
                  <span id="uxGeografiAnalyse_ShowUtvalgDetails1_uxLblMoreFunc">
                    Du kan også..
                  </span>
                </td>
              </tr>
              <tr>
                <td>
                  <a
                    onClick={CheckInput}
                    id="uxGeografiAnalyse_ShowUtvalgDetails1_uxLagreUtvalg"
                    className="read-more"
                    href='javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("uxGeografiAnalyse$ShowUtvalgDetails1$uxLagreUtvalg", "", true, "", "", false, true))'
                    style={{ fontSize: "12px" }}
                  >
                    Lagre utvalget
                  </a>
                </td>
              </tr>
              <tr>
                <td></td>
              </tr>

              <tr>
                <td>
                  {/* <a onClick="showWaitAnimation();" id="uxGeografiAnalyse_ShowUtvalgDetails1_uxAddMoreToList_ORG" className="read-more" href="javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;uxGeografiAnalyse$ShowUtvalgDetails1$uxAddMoreToList_ORG&quot;, &quot;&quot;, true, &quot;&quot;, &quot;&quot;, false, true))">Legg andre utvalg til bestillingen</a> */}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    //   </HousholdContext.Consumer>
  );
}

export default LagutvalgClick_segmenter;
