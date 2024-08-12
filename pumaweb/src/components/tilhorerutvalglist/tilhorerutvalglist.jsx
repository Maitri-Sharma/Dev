import React, { useEffect, useState, useContext, useRef } from "react";
import "../../App.css";
import expand from "../../assets/images/esri/expand.png";
import collapse from "../../assets/images/esri/collapse.png";
import Connectlisttolist from "../arbeidsliste-show/connectListOfList";
import { KSPUContext, UtvalgContext } from "../../context/Context.js";
import Swal from "sweetalert2";
import $ from "jquery";
function TilHorerUtvalgList(props) {
  const [togglevalue, settogglevalue] = useState(false);
  const [ModelName, setModelName] = useState("");
  const { activUtvalglist } = useContext(KSPUContext);
  const toggle = () => {
    settogglevalue(!togglevalue);
  };
  const showModel = () => {
    if (activUtvalglist?.ordreType === 1) {
      let msg =
        "Du forsøker nå å koble utvalg til en liste i ordre. Annuller eller lås opp ordre før endring.";
      $(".modal").remove();
      $(".modal-backdrop").remove();
      Swal.fire({
        text: msg,
        confirmButtonColor: "#7bc144",
        confirmButtonText: "Lukk",
      });
    } else {
      setModelName("ShowConnectList");
    }
  };
  return (
    <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 pr-1">
      {ModelName === "ShowConnectList" ? (
        <Connectlisttolist
          id={"ConnectList"}
          name={"LISTETILHØRIGHET"}
          isbasis={activUtvalglist?.isBasis}
          listOfList={activUtvalglist}
        />
      ) : null}

      <div className="card Kj-background-color col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        {/* <Helmet>
          <style>
            {`.form-check-input {
                  position: absolute;
                  // margin-top: .3rem;
                  margin-top: ${props.marginTop};
                  margin-left: -1.25rem;
              }
              `}
          </style>
        </Helmet> */}
        <div className="row">
          <div className="col-6">
            <p className="avan p-1 ">TILHØRER UTVALGSLISTE</p>
          </div>
          <div className="col-6" onClick={toggle}>
            {!togglevalue ? (
              <img className="d-flex float-right pt-1 mr-1" src={collapse} />
            ) : (
              <img className="d-flex float-right pt-1 mr-1" src={expand} />
            )}
          </div>
        </div>
        {!togglevalue ? (
          <div className="Kj-div-background-color col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 ">
            {props.data.memberLists.length > 0 ? (
              <p className="divValueText-list mb-4 p-0">
                Listen inneholder en eller flere lister og kan derfor ikke
                kobles til en ny liste.
              </p>
            ) : (
              <div className="row col-12 p-0 m-0">
                <div className="row col-6 p-0 m-0 pl-1 pr-1">
                  {props.data.parentList !== undefined &&
                  props.data.parentList !== null ? (
                    <p className="divValueText-list mb-4 p-0">
                      {props.data.parentList.name}
                    </p>
                  ) : (
                    <p className="divValueText-list mb-4 p-0">
                      Ingen listetilhørighet
                    </p>
                  )}
                </div>

                <div className="col-6 m-0 p-0 pl-1">
                  <a
                    id="uxShowUtvalgDetails_uxUtvalgReoler_uxPnlTree_uxShowDetails"
                    href=""
                    className="KSPU_LinkButton float-right mr-1"
                    data-toggle="modal"
                    data-target="#ConnectList"
                    onClick={showModel}
                  >
                    Endre listetilhørighet
                  </a>
                </div>
              </div>
            )}
          </div>
        ) : null}
      </div>
    </div>
  );
}

export default TilHorerUtvalgList;
