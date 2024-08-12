import React, { useState, useContext, useEffect } from "react";
import { KundeWebContext, KSPUContext } from "../context/Context.js";

function Demografie_details() {
  const { Page, setPage } = useContext(KundeWebContext);

  const GotoMain = () => {
    setPage("");
  };
  const goback = () => {
    setPage("Demogra_velg_antall_click");
  };
  return (
    // <HousholdContext.Consumer>

    <div className="col-5 p-2">
      <div className="padding_NoColor_B">
        <span className="title">Navngi og lagre utvalget</span>
      </div>

      <div className="padding_Color_L_R_T_B">
        <div className="AktivtUtvalg">
          <div className="AktivtUtvalgHeading">
            <span className="">Utvalg</span>
          </div>
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
              {0}
            </span>
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
              className="divValueTextBold div_right"
            >
              {0}
            </span>
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
              name="DemografiAnalyse1$uxFooter$uxBtnNeste"
              value="Lagre"
              className="KSPU_button-kw"
            />
          </div>
        </div>
      </div>
    </div>
  );
}

export default Demografie_details;
