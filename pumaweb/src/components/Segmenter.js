import React, { useState, useEffect, useContext } from "react";
import { segmenter_kriterier } from "./KspuConfig";
import PostreklameDataKSPU from "./datadef/PostreklameDataKSPU";
import VegGeografiskOmrade from "./VegGeografiskOmrade";

import { KSPUContext } from "../context/Context.js";

import { NewUtvalg } from "./KspuConfig";
import { UtvalgContext } from "../context/Context";

import SelectionDetails from "./SelectionDetails";
import Submit_Button from "./Submit_Button";


function Segmenter(props) {
  const segDataList = segmenter_kriterier();
  const [nomessagediv, setnomessagediv] = useState(false);
  const { selectedsegment, setselectedsegment } = useContext(KSPUContext);

  const { activUtvalg, setActivUtvalg } = useContext(KSPUContext);
  const [currentStep, setCurrentStep] = useState(1);

  const { selectedName, setSelectedName } = useContext(KSPUContext);

  const [searchData, setSearchData] = useState([]);
  const [existingActive, setExistingActive] = useState(false);
  const [radioUtvalg, setRadioUtvalg] = useState(true);

  useEffect(() => {
    if (radioUtvalg) {
      setSelectedName([]);
      setselectedsegment([]);
    }
  }, []);

  const nomessage = async () => {
    if (selectedsegment.length == 0) {
      setnomessagediv(true);
    } else {
      setnomessagediv(false);
    }
  };

  const activUtvalgExisting = (e) => {
    if (e.target.checked) {
      setExistingActive(true);
      setRadioUtvalg(false);
    } else {
      setExistingActive(false);
      setRadioUtvalg(true);
    }
  };
  const newutval = (e) => {
    setExistingActive(false);
    setRadioUtvalg(true);
  };

  return (
    <UtvalgContext.Provider
      value={{ searchData, setSearchData, existingActive }}
    >
      <div className="card">
        <div className=" row pl-1 pr-1">
          <div className="col-8 span-color1">
            <span className="sok-text1">Postreklame Segmenter</span>
          </div>
          <div className="col-4 span-color1">
            <span className="d-flex float-right sok-text1 pt-1">
              Trinn {currentStep} av 3
            </span>
          </div>
        </div>
        <div className="sok-text">
          <p>
            Posten har delt Norge inn i 10 ulike segmenter basert på ulike
            interesser og holdninger. Du kan velge mellom et eller flere
            segmenter som inneholder variabler til dine målgrupper.
          </p>
        </div>
        <div className="Kj-div-background-color pl-1 pr-1 pt-1 ">
          {currentStep == 1 ? (
            <div className="Kj-div-background-color pl-1 pr-1 pt-1 ">
              <div className="Kj-background-color pl-1 pb-1  ">
                <span className="install-text">
                  INNSTILLINGER FOR POSTREKLAME SEGMENTER
                </span>
              </div>
              {nomessagediv ? (
                <div className="pr-3">
                  <span
                    id="uxKjoreAnalyse_uxLblMessage"
                    className="divErrorText_kw"
                  >
                    Ingen segmenter er valgt. Velg minst ett segment.
                  </span>
                </div>
              ) : null}

              {Object.keys(activUtvalg).length === 0 ? null : (
                <div className="analysencard mt-3">
                  <p className="analysen pl-2">
                    Velg utgangspunkt for analysen
                  </p>
                  <div className="newutvalg">
                    <label className="radio-inline sok-text">
                      <input
                        type="radio"
                        id="newUtvalgName"
                        onChange={newutval}
                        checked={radioUtvalg}
                      />{" "}
                      {NewUtvalg()}
                    </label>
                    <br />
                    <label className="radio-inline sok-text">
                      <input
                        type="radio"
                        id="activUtvalg"
                        checked={existingActive}
                        onChange={activUtvalgExisting}
                      />{" "}
                      {activUtvalg.name}
                    </label>
                  </div>
                </div>
              )}

              <p className="label p-2 mt-2"> Velg segment</p>
              <div className="ml-0">
                <div className="ml-0">
                  {segDataList.map((data, index) => (
                    <PostreklameDataKSPU data={data} key={index} />
                  ))}
                </div>
              </div>

              <div className="sok-text pl-1">
                <p className="pt-3">
                  Finn ditt segment og mer informasjon om Postreklame Segmenter
                </p>
              </div>
            </div>
          ) : currentStep == 2 ? (
            <VegGeografiskOmrade />
          ) : currentStep == 3 ? (
            <SelectionDetails />
          ) : null}
          <Submit_Button
            tabvalue={30}
            setnomessagediv={setnomessagediv}
            segmenterSubmitValue={existingActive}
            currentStep={currentStep}
            setCurrentStep={setCurrentStep}
          />
        </div>
      </div>
    </UtvalgContext.Provider>
  );
}

export default Segmenter;
