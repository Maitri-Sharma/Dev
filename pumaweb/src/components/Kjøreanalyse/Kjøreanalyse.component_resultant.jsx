import React, { useState, useContext, useRef, useEffect } from "react";
import { MainPageContext } from "../../context/Context";
import { KSPUContext } from "../../context/Context.js";
import SelectionDetails from "../SelectionDetails";
import SaveUtvalg from "../SaveUtvalg";
import SaveSeprateUtvalg from "../Save_separate_selection";

function KjøreanalyseComponentResultant(props) {
  const { activUtvalg, setActivUtvalg, setKjDisplay, setvalue } =
    useContext(KSPUContext);
  const [currentStep, setCurrentStep] = useState(props.currentStep);
  const [Large, setLarge] = useState(" ");

  const handleCancel = async () => {
    await setKjDisplay(false);
    await setvalue(true);
    // await setKjDisplay(true);
    // await setvalue(false);
  };
  const callback = (step) => {
    setCurrentStep(step - 1);
  };
  const handleprevclick = async () => {
    // props.parentCallback(props.currentStep);
    await setKjDisplay(false);
    await setvalue(true);
    await setKjDisplay(true);
    await setvalue(false);
  };
  const showLarge = (e) => {
    props.multiSelection ? setLarge("Save_Large_sep") : setLarge("Save_Large");
  };
  return (
    <div>
      {Large == "Save_Large" ? <SaveUtvalg id={"uxBtnLagre12"} /> : null}
      {Large == "Save_Large_sep" ? (
        <SaveSeprateUtvalg
          id={"uxBtnLagre1234"}
          utvalgArray={props.utvalgArray}
        />
      ) : null}

      <SelectionDetails />
      <div className="col-12 mt-2 mb-1">
        <input
          type="submit"
          id="uxBtForrige"
          className="KSPU_button"
          value="<< Forrige"
          onClick={handleprevclick}
          style={{
            visibility: currentStep > 1 ? "visible" : "hidden",
            text: "",
            float: "left",
          }}
        />
        &nbsp;&nbsp;&nbsp;
        <input
          type="submit"
          id="uxBtnAvbryt"
          className="KSPU_button"
          value="Avbryt"
          onClick={handleCancel}
          style={{ text: "Avbryt", marginLeft: "auto" }}
        />
        {props.multiSelection ? (
          <input
            type="submit"
            id="uxBtnLagre"
            className="KSPU_button float-right"
            value="Lagre alle utvalgene"
            data-toggle="modal"
            data-target="#uxBtnLagre1234"
            onClick={showLarge}
            style={{
              display: currentStep === 3 ? "block" : "none",
              text: "Lagre alle utvalgene",
              marginLeft: "auto",
            }}
          />
        ) : (
          <input
            type="submit"
            id="uxBtnLagre"
            className="KSPU_button float-right"
            value="Lagre"
            data-toggle="modal"
            data-target="#uxBtnLagre12"
            onClick={showLarge}
            style={{
              display: currentStep === 3 ? "block" : "none",
              text: "Lagre",
              marginLeft: "auto",
              float: "right",
            }}
          />
        )}
      </div>
    </div>
  );
}

export default KjøreanalyseComponentResultant;
