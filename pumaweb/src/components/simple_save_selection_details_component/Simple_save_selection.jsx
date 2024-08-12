import React, { useEffect, useState, useRef, useContext } from "react";
import readmore from "../../assets/images/read_more.gif";
import { KundeWebContext, MainPageContext } from "../../context/Context";
import { kundeweb_utvalg } from ".././KspuConfig";
import api from "../../services/api";
import $ from "jquery";
function Simple_save_selection() {
  const { UtvalgID, setUtvalgID } = useContext(KundeWebContext);
  const { mapView } = useContext(MainPageContext);
  const { HouseholdSum_Demo, setHouseholdSum_Demo } =
    useContext(KundeWebContext);
  const { Page, setPage } = useContext(KundeWebContext);
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
  const GotoMain = () => {
    setPage("");
  };
  const Lagnyttclick = () => {
    // remove previous highlighted feature
    // let j = mapView.graphics.items.length;
    //     var k = 0;
    //     k = j;
    //     for (var i = j; i > 0; i--) {
    //       if (mapView.graphics.items[i-1].geometry.type === "polygon") {
    //         mapView.graphics.remove(mapView.graphics.items[i-1]);
    //         //j++;
    //       }
    //     }
    // mapView.graphics.removeAll();
    let j = mapView.graphics.items.length;
    for (var i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
        //j++;
      }
    }
    //move to initial extent
    mapView.goTo(mapView.initialExtent);
    setPage("");
  };

  return (
    // <HousholdContext.Consumer>
    <div className="col-5 p-2" style={{ height: "100%" }}>
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

            
            <div className="pt-3">
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
                  {HouseholdSum_Demo}
                </span>
              </div>
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
                {HouseholdSum_Demo}
              </span>
            </div>

            {/* /*delete utvalg modal box*/}

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

            {/* /*modal box ends*/}

            <div className="padding_NoColor_B clearFloat">
              <div className="bold green">Ønsker du å sende dette utvalget?</div>
              <div className="bold">
                Ring Bring kundeservice på 04045.
                <br />
                Oppgi referansenummer U
                <span id="DemografiAnalyse1_ShowUtvalgDetails1_uxLblUtvalgIdUpaaloggetinfo">
                  2360670
                </span>
              </div>
              <br />
              <div className="padding_NoColor_L_T sok-text">
                Du må være logget inn for å kunne fullføre bestillingen på nett.
                <br />
                Kundeservice kan gi deg brukernavn og passord.
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
            </div>
          </div>
        </div>
        <div className="padding_NoColor_T">
          <a
            className="KSPU_LinkButton_Url_KW pl-2"
            onClick={GotoMain}
          >
            Avbryt
          </a>
        </div>
        <div className="float-right">
          <div>
            <input
              type="submit"
              name="DemografiAnalyse1$uxFooter$uxBtnNeste"
              value="Lag nytt utvalg"
              onClick={Lagnyttclick}
              className="KSPU_button-kw"
            />
          </div>
        </div>
        <br />
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
export default Simple_save_selection;
