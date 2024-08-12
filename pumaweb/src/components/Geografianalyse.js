import React, { useState, useContext } from "react";
import "../App.css";
import expand from "../assets/images/esri/expand.png";
import collapse from "../assets/images/esri/collapse.png";
import Mottakergrupper from "./Mottakergrupper";
import KommuneComponent from "./Kommun";
import FylkeComponent from "./Fylke";
import Postbokscomponent from "./Postboks";
import TeamComponent from "./Team";
import BudruteComponent from "./Budrute";
import PostnrComponent from "./Postnr";
import Submit_Button from "./Submit_Button";
import useCurrentStep from "../common/useCurrentStep.js";

import {
  KSPUContext,
  MainPageContext,
  UtvalgContext,
} from "../context/Context";
import Result from "./Result";
import PickList from "./PickList";

function test(value) {
  Geografianalyse(value);
}

function Geografianalyse(props) {
  const [togglevalue, settogglevalue] = useState(false);
  const [isPickList, setIsPickList] = useState(false);
  const [velgvalue, setVelgvalue] = useState("0");
  const [searchData, setSearchData] = useState([]);
  const [currentStep, setCurrentStep] = useState(1);
  const {
    resultData,
    setShowHousehold,
    setShowBusiness,
    setShowReservedHouseHolds,
    geograErrMsg
  } = useContext(KSPUContext);
  const { mapView } = useContext(MainPageContext);
  const toggle = () => {
    settogglevalue(!togglevalue);
  };
  const Velg = (e) => {
    // mapView.graphics.removeAll();
    let j = mapView.graphics.items.length;
    for (var i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
        //j++;
      }
    }
    setVelgvalue(e.target.value);
    setShowReservedHouseHolds(false);
    setShowBusiness(false);
    setShowHousehold(true);
    if (e.target.value > 0) setCurrentStep(1);
  };
  return (
    <UtvalgContext.Provider
      value={{ searchData, setSearchData, setIsPickList, isPickList }}
    >
      <div className="card">
        <div className=" row pl-1 pr-1">
          <div className="col-8 span-color1">
            <span className="sok-text1">Geografianalyse</span>
          </div>
          <div className="col-4 span-color1">
            <span className="d-flex float-right sok-text1 pt-1">
              Trinn {currentStep} av {velgvalue === "4" ? "2" : "3"}
            </span>
          </div>
        </div>
        <div className="sok-text">
          <p className="pl-1">
            Når du vil treffe husstander i utvalgte geografiske områder kan du
            med dette verktøyet velge områder i forhold til geografiske
            kriterier.
          </p>
        </div>
        <div className="Kj-div-background-color pl-1 pr-1 pt-1">
          {parseInt(velgvalue) <= 6 ? (
            <>
              <div className="Kj-background-color pl-1 pb-1 ">
                <span className="install-text">VELG GEOGRAFISK OMRÅDE</span>
              </div>
              {geograErrMsg ? (
                <span className="sok-text ml-2 red">
                  Det må velges minst en kommune
                  <br/>
                </span>
          ) : null}
              <div className="mt-2 pl-1">
                <select
                  id="uxDropDownListGeografi"
                  className="form-select btn-work form-select-size_1 mb-1"
                  aria-label=".form-select-sm example"
                  value={velgvalue}
                  onChange={Velg}
                >
                  <option value="0">- Velg området -</option>
                  <option value="1">Fylke</option>
                  <option value="2">Kommune</option>
                  <option value="3">Team</option>
                  <option value="4">Budrute</option>
                  <option value="5">Postnr</option>
                  {/* <option value="6">Postboks</option> */}
                </select>
              </div>

              {velgvalue === "1" ? <FylkeComponent /> : null}
              {velgvalue === "3" ? <TeamComponent /> : null}
              {velgvalue === "4" ? <BudruteComponent /> : null}
              {velgvalue === "5" ? <PostnrComponent /> : null}
              {velgvalue === "2" ? <KommuneComponent /> : null}
              {velgvalue === "6" ? <Postbokscomponent /> : null}
            </>
          ) : parseInt(velgvalue) === 9 ? (
            <PickList />
          ) : parseInt(velgvalue) === 10 ? (
            <Result />
          ) : null}

          {parseInt(velgvalue) === 0 ? null : (
            <Submit_Button
              tabvalue={velgvalue}
              setVelgvalue={setVelgvalue}
              currentStep={currentStep}
              setCurrentStep={setCurrentStep}
            />
          )}
        </div>
      </div>
    </UtvalgContext.Provider>
  );
}

export default Geografianalyse;
