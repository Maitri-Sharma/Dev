import React, { useEffect, useState, useRef, useContext } from "react";
import readmore from "../../assets/images/read_more.gif";
import { KundeWebContext } from "../../context/Context";
import { kundeweb_utvalg } from ".././KspuConfig";
import api from "../../services/api";
import loadingImage from "../../assets/images/callbackActivityIndicator.gif";
import { NumberFormat } from "../../common/Functions";

function Simple_save_utvalg() {
  const [loading, setloading] = useState(false);
  const { username_kw, setusername_kw } = useContext(KundeWebContext);
  const [melding, setmelding] = useState(false);
  const { Page, setPage } = useContext(KundeWebContext);
  const { Page_P, setPage_P } = useContext(KundeWebContext);
  const { UtvalgID, setUtvalgID } = useContext(KundeWebContext);
  const { HouseholdSum_Demo, setHouseholdSum_Demo } =
    useContext(KundeWebContext);
  const showwarning = () => {
    setmelding(true);
  };
  const GotoMain = () => {
    setPage("");
  };
  const goback = () => {
    setPage("Demogra_velg_antall_click");
  };
  const LagreClick = async () => {
    setloading(true);
    let saveOldReoler = "false";
    let skipHistory = "false";
    let forceUtvalgListId = 0;
    let name = username_kw;
    let url = `Utvalg/SaveUtvalg?userName=${name}&`;
    url = url + `saveOldReoler=${saveOldReoler}&`;
    url = url + `skipHistory=${skipHistory}&`;
    url = url + `forceUtvalgListId=${forceUtvalgListId}`;
    try {
      let selection = "utvalg";
      let describtion = "";
      let A = kundeweb_utvalg();
      A.name = selection;
      A.kundeNavn = describtion;
      A.totalAntall = HouseholdSum_Demo;
      // A.reoler[0].description = describtion;
      // A.reoler[0].antall.households = HouseholdSum;
      A.criterias[0].criteriaType = 19;
      A.criterias[0].criteria = "Geografipulkkliste";
      // setPage("Simple_save_selection");

      const { data, status } = await api.postdata(url, A);
      if (status === 200) {
        let utvalgID = data.utvalgId;
        setUtvalgID(utvalgID);
        // let msg = `Utvalg ${name} er lagret.`
        // seterrormsg(msg)
        // setdisplayMsg(true)
        // setissave(true);
        setPage("Simple_save_selection");
        setloading(false);
      }
    } catch (e) {
      console.log(e);
    }

    //    setPage("Geogra_distribution_click")

    // setPage("");
  };
  return (
    // <HousholdContext.Consumer>
    <div
      className={loading ? "col-5 p-2 blur" : "col-5 p-2"}
      style={{ height: "100%" }}
    >
      {/* <div className="padding_NoColor_B">
        <span className="title">Navngi og lagre utvalget</span>
      </div> */}
      <div>
        <div style={{ display: "" }}>
          <div className="padding_Color_L_R_T_B">
            <div className="AktivtUtvalg">
              <div className="AktivtUtvalgHeading">
                <span className="">Utvalg</span>
              </div>
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
            ) : null}

            {melding ? (
              <div className="pr-3">
                <div className="error WarningSign">
                  <div className="divErrorHeading">Melding:</div>
                  <span
                    id="uxKjoreAnalyse_uxLblMessage"
                    className="divErrorText_kw"
                  >
                    Utvalgsnavnet må ha minst 3 tegn.
                  </span>
                </div>
              </div>
            ) : null}
            <div className="pt-3">
              {/* <div className=""> */}{" "}
              <label className="form-check-label label-text" htmlFor="Hush">
                {" "}
                Husholdninger{" "}
              </label>
              <span
                id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                className="divValueTextBold div_right pr-3"
              >
                {NumberFormat(HouseholdSum_Demo)}
              </span>
              {/* </div> */}
            </div>
            <div
              style={{
                width: "370px",
                borderTop: "solid 1px black",
                fontWeight: "bold",
                padding: "0px",
              }}
            >
              &nbsp;
              <span
                id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                className="divValueTextBold div_right pl-3"
              >
                {NumberFormat(HouseholdSum_Demo)}
              </span>
            </div>
          </div>
          <div className="div_left">
            <input
              type="submit"
              name="DemografiAnalyse1$uxFooter$uxBtForrige"
              value="Tilbake"
              onClick={goback}
              className="KSPU_button_Gray"
            />
            <div className="padding_NoColor_T">
              <a
                className="KSPU_LinkButton_Url_KW pl-2"
                onClick={GotoMain}
              >
                Avbryt
              </a>
            </div>
          </div>
          <div className="float-right">
            <div>
              <input
                type="submit"
                name="DemografiAnalyse1$uxFooter$uxBtnNeste"
                value="Lagre"
                onClick={LagreClick}
                className="KSPU_button-kw"
              />
            </div>
          </div>
          <br />
          <br />
          <br />
          <br />
          <a
            onClick={showwarning}
            id="uxShowUtvalgListDetails_uxAddMoreToList"
            className="KSPU_LinkButton1_Url margin"
          >
            <img src={readmore} />
            &nbsp;Lagre utvalget
          </a>
        </div>
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
      </div>
    </div>
  );
}
export default Simple_save_utvalg;
