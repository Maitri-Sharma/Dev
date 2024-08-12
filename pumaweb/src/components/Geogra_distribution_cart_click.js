import React, { useState, useContext, useEffect } from "react";
import { KundeWebContext } from "../context/Context.js";
import api from "../services/api.js";
import swal from "sweetalert";
import $ from "jquery";

function Geogra_distribution_cart_click() {
  const { Page, setPage } = useContext(KundeWebContext);
  const { UtvalgID, setUtvalgID } = useContext(KundeWebContext);
  const [depend, setdepend] = useState(false);
  const { HouseholdSum, setHouseholdSum } = useContext(KundeWebContext);
  const [bool, setbool] = useState(false);
  const { Page_P, setPage_P } = useContext(KundeWebContext);
  const { HouseholdSum_seg, setHouseholdSum_seg } = useContext(KundeWebContext);
  const { HouseholdSum_Demo, setHouseholdSum_Demo } =
    useContext(KundeWebContext);
  const [finalhousehold, setfinalhousehold] = useState(
    Page_P == "VeglGeografiskOmrade_kw"
      ? HouseholdSum_seg
      : Page_P === "Demogra_velg_antall_click"
      ? HouseholdSum_Demo
      : HouseholdSum
  );

  const GotoMain = () => {
    setPage("");
  };

  const Lag_Utvalg_Click = () => {
    setPage("");
  };
  // const Neiclick =() =>{
  //     setPage(Page)
  // }
  const Jaclick = async () => {
    
    try {
      const { data, status } = await api.deletedata(
        "Utvalg/DeleteUtvalg?utvalgId=" + UtvalgID
      );
      if (status === 200) {
        $(".modal").remove();
        $(".modal-backdrop").remove();

        setPage("");
      } else {
        console.error("error : " + status);
      }
    } catch (error) {
      // swal("oops! Something Went Wrong!");

      console.error("er : " + error);
    }
  };
  return (
    <div className="col-5">
      <div className="padding_Color_L_R_T_B">
        <div className="AktivtUtvalg">
          <div className="AktivtUtvalgHeading">
            <span id="uxShowUtvalgDetails_uxLblSavedUtvalgName">
              {UtvalgID}
            </span>
          </div>

          <div>
            <div className="gray">
              ID:
              <span id="uxShowUtvalgDetails_uxRefNr" className="gray">
                {UtvalgID}
              </span>
            </div>
          </div>
        </div>

        <div
          id="uxShowUtvalgDetails_uxAntallsInfo"
          className=" clearFloat divValueText"
        >
          <table style={{ width: "321%" }}>
            <tbody>
              <tr>
                <td style={{ width: "294px", padding: "0px" }}>
                  Husholdninger
                </td>
                <td
                  style={{ width: "75px", padding: "0px", textAlign: "right" }}
                >
                  {finalhousehold}
                </td>
              </tr>
              <tr style={{ fontWeight: "bold" }}>
                <td
                  style={{
                    width: "294px",
                    borderTop: "solid 1px black",
                    fontWeight: "bold",
                    padding: "0px",
                  }}
                >
                  &nbsp;
                </td>
                <td
                  style={{
                    width: "75px",
                    borderTop: "solid 1px black",
                    fontWeight: "bold",
                    padding: "0px",
                    textAlign: "right",
                  }}
                >
                  {finalhousehold}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <br />

        <div className="padding_NoColor_B clearFloat">
          <div className="bold green">Ønsker du å sende dette utvalget?</div>
          <div className="bold">
            Ring Bring kundeservice på 04045.
            <br />
            Oppgi referansenummer U
            <span id="uxSegmenterAnalyse_ShowUtvalgDetails1_uxLblUtvalgIdUpaaloggetinfo">
              2359788
            </span>
          </div>
          <div className="padding_NoColor_L_T distribution_small_text">
            Du må være logget inn for å kunne fullføre bestillingen på nett.
            <br />
            Kundeservice kan gi deg brukernavn og passord.
          </div>
        </div>

        <div className="paddingBig_NoColor_T clearFloat">
          <div>
            <table width="100%">
              <tbody>
                <tr>
                  <td style={{ width: "80px" }}>
                    <a
                      id="uxShowUtvalgDetails_uxLinkBtnDelete"
                      className="KSPU_LinkButton_Url_KW"
                      href=""
                      data-toggle="modal"
                      data-target="#exampleModal"
                    >
                      Slett utvalget
                    </a>
                  </td>
                  <td style={{ width: "160px" }}>
                    <div className="div_right"></div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
        {/* <div className="modal fade"  id="exampleModal" tabIndex="-1" role="dialog" aria-labelledby="exampleModalLabel"  aria-hidden="true">
  <div className="modal-dialog modal-dialog-centered" role="document">
    <div className="divDockedPanel">
      <div className=" divDockedPanelTop">
        <span className="dialog-kw"  id="exampleModalLabel">Advarsel</span>
        <button type="button" className="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
      </div>
<div className="">
        <p style={{fontSize:"12px" }}>&nbsp; Skal utvalget slettes?
</p>
      </div>
    
      <div className="View_modal-body pl-2">
      <table>
  <tbody>
    <tr>
    
          
          <div className="ml-4">  &nbsp;&nbsp;  &nbsp;&nbsp;
      <input type="button" name="uxDialogs$uxModalMessage$uxWarning$uxBtnLeftClose"  value="Nei" data-target="modal" className="modalMessage_button"/>
        </div>
        <div className="mr-4">  &nbsp;&nbsp;
        <button  className="modalMessage_button"  onClick={Jaclick}>ja</button>
        </div>
        <tr>
      <td>
      <button type="button" className="btn KSPU_button" data-dismiss="modal">Avbryt</button>
        </td>
        <td></td>
        <td>
        <button type="button"  className="modalMessage_button"  onClick={Jaclick}>ja</button>
        </td>
    </tr>
      

     
 
    </tr>
    </tbody>
    </table>
       
</div> */}

        {/* </div>
      <p></p>
    </div> */}
        {/* </div> */}

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
                            <p style={{ fontSize: "12px" }}>
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
                                onClick={Jaclick}
                                className="modalMessage_button"
                                data-dismiss={bool ? "modal" : ""}
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
      </div>
      <div className="div_left">
        <div className="padding_NoColor_T">
          <a
            className="KSPU_LinkButton_Url_KW pl-2"
            onClick={GotoMain}
          >
            Avbryt
          </a>
        </div>
      </div>
      <p></p>
      <div className="float-right">
        <div>
          <input
            type="submit"
            value="Lag nytt utvalg"
            onClick={Lag_Utvalg_Click}
            className="KSPU_button-kw"
          />
        </div>
      </div>
    </div>
  );
}

export default Geogra_distribution_cart_click;
