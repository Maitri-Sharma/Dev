/* eslint-disable react/jsx-pascal-case */
import React, { useState, useContext, useEffect } from "react";

import "../App.css";
import expand from "../assets/images/esri/expand.png";
import collapse from "../assets/images/esri/collapse.png";
import { KSPUContext } from "../context/Context.js";
import DemografikeOmrade from "./DemografikeOmrade";
import SelectionDetails from "./SelectionDetails";
import Submit_Button from "./Submit_Button";

import { UtvalgContext } from "../context/Context";
import { Utvalg, NewUtvalg, criterias, getAntall } from "./KspuConfig";

function Demografie() {
  const [togglevalue, settogglevalue] = useState(false);
  const [togglevalue1, settogglevalue1] = useState(false);
  const [togglevalue2, settogglevalue2] = useState(false);
  const [togglevalue3, settogglevalue3] = useState(false);
  const [togglevalue4, settogglevalue4] = useState(false);
  const [togglevalue5, settogglevalue5] = useState(false);
  const [togglevalue6, settogglevalue6] = useState(false);
  const [togglevalue7, settogglevalue7] = useState(false);
  const [togglevalue8, settogglevalue8] = useState(false);
  const [togglevalue9, settogglevalue9] = useState(false);
  const [currentStep, setCurrentStep] = useState(1);
  const [searchData, setSearchData] = useState([]);
  const { demoIndexArray, setDemoIndexArray } = useContext(KSPUContext);
  const { activUtvalg, setActivUtvalg } = useContext(KSPUContext);
  const { demografikmsg, setdemografikmsg } = useContext(KSPUContext);
  const { demografikAntalMsg, setDemografikAntalMsg } = useContext(KSPUContext);
  const { demografikagemsg, setdemografikagemsg } = useContext(KSPUContext);

  const [radioUtvalg, setRadioUtvalg] = useState(true);

  const { existingActive, setExistingActive } = useContext(KSPUContext);

  const { selecteddemografiecheckbox, setselecteddemografiecheckbox } =
    useContext(KSPUContext);
  const { selecteddemografiecheckbox_c, setselecteddemografiecheckbox_c } =
    useContext(KSPUContext);
  const [checkboxvalue, setcheckboxvalue] = useState(Array(97).fill(false));
  const [demografyObjArray, setDemografyObjArray] = useState([]);

  let selectedvalueMen = [
    "C19_23_MEN",
    "C24_34_MEN",
    "C35_44_MEN",
    "C45_54_MEN",
    "C55_64_MEN",
    "C65_74_MEN",
    "C75_84_MEN",
    "C85_o_MEN",
  ];
  let selectedvalueWomen = [
    "C19_23_KIV",
    "C24_34_KIV",
    "C35_44_KIV",
    "C45_54_KIV",
    "C55_64_KIV",
    "C65_74_KIV",
    "C75_84_KIV",
    "C85_o_KIV",
  ];

  useEffect(() => {
    setExistingActive(false);
    setdemografikagemsg(false);
    setdemografikmsg(false);
    setselecteddemografiecheckbox_c([]);
    setselecteddemografiecheckbox([]);
    setDemografyObjArray([]);
  }, []);
  let arr = selecteddemografiecheckbox_c;

  let arr_index = selecteddemografiecheckbox_c;
  const foundMen = selecteddemografiecheckbox.some((r) =>
    selectedvalueMen.includes(r)
  );
  const foundWomen = selecteddemografiecheckbox.some((r) =>
    selectedvalueWomen.includes(r)
  );

  const sortWithIndex = (value, index, op) => {
    let objectArray = demografyObjArray;
    let obj = {};
    obj["value"] = value;
    obj["index"] = index;
    if (op === "push") {
      objectArray.push(obj);
    } else {
      objectArray = objectArray?.filter((item) => {
        return item.value !== value;
      });
    }
    objectArray?.sort((a, b) => {
      return a.index - b.index;
    });
    setDemografyObjArray(objectArray);
    let demografyName = objectArray?.map((item) => {
      return item?.value;
    });
    return demografyName;
  };

  const handleCheck = (event, value, index) => {
    let checked = demoIndexArray;
    let _checkboxvalue = [...checkboxvalue];
    _checkboxvalue[index] = event.target.checked;
    setcheckboxvalue(_checkboxvalue);

    if (event.target.checked === false) {
      arr = sortWithIndex(value, index, "pop");
      checked = checked?.filter((i) => {
        return i !== index;
      });
    } else {
      // arr.push(value);
      arr = sortWithIndex(value, index, "push");
      checked?.push(index);
    }
    setDemoIndexArray(checked);
    setselecteddemografiecheckbox_c(arr);
    setselecteddemografiecheckbox(arr);
  };

  const toggle = () => {
    settogglevalue(!togglevalue);
  };
  const toggle1 = () => {
    settogglevalue1(!togglevalue1);
  };
  const toggle2 = () => {
    settogglevalue2(!togglevalue2);
  };
  const toggle3 = () => {
    settogglevalue3(!togglevalue3);
  };
  const toggle4 = () => {
    settogglevalue4(!togglevalue4);
  };
  const toggle5 = () => {
    settogglevalue5(!togglevalue5);
  };
  const toggle6 = () => {
    settogglevalue6(!togglevalue6);
  };
  const toggle7 = () => {
    settogglevalue7(!togglevalue7);
  };
  const toggle8 = () => {
    settogglevalue8(!togglevalue8);
  };
  const toggle9 = () => {
    settogglevalue9(!togglevalue9);
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
            <span className="sok-text1">Demografiseleksjon</span>
          </div>
          <div className="col-4 span-color1">
            <span className="d-flex float-right sok-text1 pt-1">
              Trinn {currentStep} av 3
            </span>
          </div>
        </div>
        <div className="sok-text">
          <p className="pl-1">
            Med utgangspunkt i kjennetegn hos din målgruppe kan du lage utvalg
            basert på alder, kjønn, utdanningsnivå, biltype, boligalder,
            personinntekt etc.{" "}
          </p>
        </div>
        {currentStep == 1 ? (
          <div className="divcolor pl-1 pr-1 pt-1 ">
            <div className="divsubcolor pl-1 pb-1 ">
              <span className="demografi-install-text">
                INNSTILLINGER FOR DEMOGRAFISELEKSJON
              </span>
            </div>
            {demografikmsg ? (
              <div className="pr-2">
                <span
                  id="uxKjoreAnalyse_uxLblMessage"
                  className="divErrorText_kw"
                >
                  Ingen kriterier er valgt.
                </span>
              </div>
            ) : null}
            {demografikagemsg ? (
              <div className="pr-2">
                <span
                  id="uxKjoreAnalyse_uxLblMessage"
                  className="divErrorText_kw"
                >
                  Bare kriterier fra én kategori kan velges med mindre alder og
                  kjønn kombineres. Dersom kjønn er valgt må også alder velges.
                </span>
              </div>
            ) : null}
            {Object.keys(activUtvalg).length === 0 ? null : (
              <div className="analysencard mt-3">
                <p className="analysen pl-2">Velg utgangspunkt for analysen</p>
                <div className="newutvalg">
                  <label className="radio-inline sok-text">
                    <input
                      type="radio"
                      id="newUtvalgName"
                      onChange={newutval}
                      checked={
                        Object.keys(activUtvalg).length === 0
                          ? true
                          : radioUtvalg
                      }
                      //defaultChecked
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

            <div className="card Kj-background-color ml-1 mr-1 mt-2 ">
              <div className="divsubcolor">
                <div className="row">
                  <div className="col-10">
                    <p className="avan1 p-1">Alder</p>
                  </div>
                  <div className="col-2">
                    {togglevalue ? (
                      <img
                        className="d-flex float-right pt-1"
                        src={collapse}
                        onClick={toggle}
                      />
                    ) : (
                      <img
                        className="d-flex float-right pt-1"
                        src={expand}
                        onClick={toggle}
                      />
                    )}
                  </div>
                </div>
              </div>
              {togglevalue ? (
                <div
                  className="container demografic-container center"
                  style={{ display: "flex" }}
                >
                  <div className="row">
                    <div className="col-lg-6 col-md-6 col-sm-12 col-xs-12 ">
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline sok-text">
                          <input
                            type="checkbox"
                            id="19-23"
                            value="19-23"
                            checked={checkboxvalue[0]}
                            onChange={(event) =>
                              handleCheck(event, "ald19_23", 0)
                            }
                          />{" "}
                          &nbsp; 19-23
                        </label>
                      </div>

                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline sok-text">
                          <input
                            type="checkbox"
                            value="35-44"
                            id="35-44"
                            checked={checkboxvalue[2]}
                            onChange={(event) =>
                              handleCheck(event, "ald35_44", 2)
                            }
                          />{" "}
                          &nbsp; 35-44
                        </label>
                      </div>

                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline sok-text">
                          <input
                            type="checkbox"
                            value="55-64"
                            id="55-64"
                            checked={checkboxvalue[4]}
                            onChange={(event) =>
                              handleCheck(event, "ald55_64", 4)
                            }
                          />{" "}
                          &nbsp; 55-64
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline sok-text">
                          <input
                            type="checkbox"
                            value="75-84"
                            id="75-84"
                            checked={checkboxvalue[6]}
                            onChange={(event) =>
                              handleCheck(event, "ald75_84", 6)
                            }
                          />{" "}
                          &nbsp; 75-84
                        </label>
                      </div>
                    </div>
                    <div className="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline sok-text">
                          <input
                            type="checkbox"
                            value="24-34"
                            id="24-34"
                            checked={checkboxvalue[1]}
                            onChange={(event) =>
                              handleCheck(event, "ald24_34", 1)
                            }
                          />{" "}
                          &nbsp; 24-34
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline sok-text">
                          <input
                            type="checkbox"
                            value="45-54"
                            id="45-54"
                            checked={checkboxvalue[3]}
                            onChange={(event) =>
                              handleCheck(event, "ald45_54", 3)
                            }
                          />{" "}
                          &nbsp; 45-54
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline sok-text">
                          <input
                            type="checkbox"
                            value="65-74"
                            id="65-74"
                            checked={checkboxvalue[5]}
                            onChange={(event) =>
                              handleCheck(event, "ald65_74", 5)
                            }
                          />{" "}
                          &nbsp; 65-74
                        </label>
                      </div>

                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline sok-text">
                          <input
                            type="checkbox"
                            value="85-94"
                            id="85-94"
                            checked={checkboxvalue[7]}
                            onChange={(event) =>
                              handleCheck(event, "ald85_o", 7)
                            }
                          />{" "}
                          &nbsp; 85-94
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0"></div>
                    </div>
                  </div>
                </div>
              ) : null}
            </div>

            <div className="card Kj-background-color ml-1 mr-1 mt-1">
              <div className="divsubcolor">
                <div className="row">
                  <div className="col-10">
                    <p className="avan1 p-1">
                      Kjønn (Brukes kun sammen med Alder)
                    </p>
                  </div>
                  <div className="col-2">
                    {togglevalue1 ? (
                      <img
                        className="d-flex float-right pt-1"
                        src={collapse}
                        onClick={toggle1}
                      />
                    ) : (
                      <img
                        className="d-flex float-right pt-1"
                        src={expand}
                        onClick={toggle1}
                      />
                    )}
                  </div>
                </div>
              </div>
              {togglevalue1 ? (
                <div className="container demografic-container center">
                  <div className="row">
                    <div className="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                      <label className="checkbox-inline sok-text">
                        <input
                          type="checkbox"
                          checked={checkboxvalue[95]}
                          onChange={(event) => handleCheck(event, "Menn", 95)}
                          id="Menn"
                          name="Menn"
                          value="Menn"
                        />
                        Menn
                      </label>
                    </div>
                    <div className="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                      <label className="checkbox-inline sok-text">
                        <input
                          type="checkbox"
                          checked={checkboxvalue[96]}
                          onChange={(event) =>
                            handleCheck(event, "Kvinner", 96)
                          }
                          id="Kvinner"
                          name="Kvinner"
                          value="Kvinner"
                        />
                        Kvinner
                      </label>
                    </div>
                  </div>
                </div>
              ) : null}
            </div>

            <div className="card Kj-background-color ml-1 mr-1 mt-1">
              <div className="divsubcolor">
                <div className="row">
                  <div className="col-10">
                    <p className="avan1 p-1">Utdanningsnivå</p>
                  </div>
                  <div className="col-2">
                    {togglevalue2 ? (
                      <img
                        className="d-flex float-right pt-1"
                        src={collapse}
                        onClick={toggle2}
                      />
                    ) : (
                      <img
                        className="d-flex float-right pt-1"
                        src={expand}
                        onClick={toggle2}
                      />
                    )}
                  </div>
                </div>
              </div>
              {togglevalue2 ? (
                <div className="container demografic-container center">
                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline sok-text">
                      <input
                        type="checkbox"
                        checked={checkboxvalue[8]}
                        onChange={(event) =>
                          handleCheck(event, "ingen_uopp", 8)
                        }
                        id="Ingen oppgitt utdanning"
                      />
                      Ingen oppgitt utdanning
                    </label>
                  </div>

                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline sok-text">
                      <input
                        type="checkbox"
                        checked={checkboxvalue[9]}
                        onChange={(event) =>
                          handleCheck(event, "grunnskole", 9)
                        }
                      />
                      Grunnskole
                    </label>
                  </div>
                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline sok-text">
                      <input
                        checked={checkboxvalue[10]}
                        onChange={(event) =>
                          handleCheck(event, "videregaen", 10)
                        }
                        id="DemografiAnalyse1_DemografiKriterier1_uxFPUtdanningsNiva_VIDEREGAEN"
                        type="checkbox"
                        name="DemografiAnalyse1$DemografiKriterier1$uxFPUtdanningsNiva$VIDEREGAEN"
                      />
                      Videregående skole
                    </label>
                  </div>
                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline inline-block sok-text">
                      <input
                        type="checkbox"
                        checked={checkboxvalue[11]}
                        onChange={(event) =>
                          handleCheck(event, "hogskole_u", 11)
                        }
                        name="DemografiAnalyse1$DemografiKriterier1$uxFPUtdanningsNiva$HOGSKOLE_U"
                      />
                      Høgskole/Universitet lavt nivå
                    </label>
                  </div>
                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline sok-text">
                      <input
                        type="checkbox"
                        checked={checkboxvalue[12]}
                        onChange={(event) =>
                          handleCheck(event, "hogskole_1", 12)
                        }
                        name="DemografiAnalyse1$DemografiKriterier1$uxFPUtdanningsNiva$HOGSKOLE_1"
                      />
                      Høgskole/Universitet høyt nivå
                    </label>
                  </div>
                </div>
              ) : null}
            </div>

            <div className="card Kj-background-color ml-1 mr-1 mt-1 ">
              <div className="divsubcolor">
                <div className="row">
                  <div className="col-10">
                    <p className="avan1 p-1">Boligtype</p>
                  </div>
                  <div className="col-2">
                    {togglevalue3 ? (
                      <img
                        className="d-flex float-right pt-1"
                        src={collapse}
                        onClick={toggle3}
                      />
                    ) : (
                      <img
                        className="d-flex float-right pt-1"
                        src={expand}
                        onClick={toggle3}
                      />
                    )}
                  </div>
                </div>
              </div>
              {togglevalue3 ? (
                <div className="container demografic-container center ">
                  <div className="row">
                    <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 ">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[30]}
                            onChange={(event) =>
                              handleCheck(event, "enebolig", 30)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligtype_ENEBOLIG"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligtype$ENEBOLIG"
                          />
                          Enebolig
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 ">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[32]}
                            onChange={(event) =>
                              handleCheck(event, "blokk", 32)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligtype_BLOKK"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligtype$BLOKK"
                          />
                          Blokk/Bygård/Terrassehus
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 ">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[34]}
                            onChange={(event) =>
                              handleCheck(event, "tomannsbolig", 34)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligtype_TOMANNSBOLIG"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligtype$TOMANNSBOLIG"
                          />
                          Tomannsbolig
                        </label>
                      </div>
                    </div>
                    <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                      {/* Comment below option after discussion with Wenke */}
                      {/* <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 ">
                        <label className="checkbox-inline toggletext ">
                          <input
                            checked={checkboxvalue[31]}
                            onChange={(event) =>
                              handleCheck(event, "bofelleskap", 31)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligtype_TOMANNSBOLIG"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligtype$TOMANNSBOLIG"
                          />
                          Bofellesskap
                        </label>
                      </div> */}
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 ">
                        <label className="checkbox-inline toggletext ">
                          <input
                            checked={checkboxvalue[33]}
                            onChange={(event) =>
                              handleCheck(event, "rekkehus", 33)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligtype_REKKEHUS"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligtype$TOMANNSBOLIG"
                          />
                          Rekkehus
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 ">
                        <label className="checkbox-inline toggletext ">
                          <input
                            checked={checkboxvalue[35]}
                            onChange={(event) =>
                              handleCheck(event, "annen_bo", 35)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligtype_ANNENBOLIG"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligtype$TOMANNSBOLIG"
                          />
                          Annen bolig
                        </label>
                      </div>
                    </div>
                  </div>
                </div>
              ) : null}
            </div>

            <div className="card Kj-background-color ml-1 mr-1 mt-1 ">
              <div className="divsubcolor">
                <div className="row">
                  <div className="col-10">
                    <p className="avan1 p-1">Boligalder</p>
                  </div>
                  <div className="col-2">
                    {togglevalue4 ? (
                      <img
                        className="d-flex float-right pt-1"
                        src={collapse}
                        onClick={toggle4}
                      />
                    ) : (
                      <img
                        className="d-flex float-right pt-1"
                        src={expand}
                        onClick={toggle4}
                      />
                    )}
                  </div>
                </div>
              </div>
              {togglevalue4 ? (
                <div className="container demografic-container center">
                  <div className="row">
                    <div className="col-lg-6 col-md-6 col-sm-12 col-xs-12 ">
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[46]}
                            onChange={(event) =>
                              handleCheck(event, "h1_aar", 46)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H1_AAR"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H1_AAR"
                          />
                          1 år
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[48]}
                            onChange={(event) =>
                              handleCheck(event, "h2_5_aar", 48)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H2_5_AAR"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H2_5_AAR"
                          />
                          3-5 år
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[50]}
                            onChange={(event) =>
                              handleCheck(event, "h10_20_aar", 50)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H10_20_AAR"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H10_20_AAR"
                          />
                          11-20 år
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[52]}
                            onChange={(event) =>
                              handleCheck(event, "h30_40_aar", 52)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H30_40_AAR"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H30_40_AAR"
                          />
                          31-40 år
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[54]}
                            onChange={(event) =>
                              handleCheck(event, "hover_50", 54)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_HOVER_50"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$HOVER_50"
                          />
                          Over 50 år
                        </label>
                      </div>
                    </div>
                    <div className="col-lg-6 col-md-6 col-sm-12 col-xs-12 ">
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[47]}
                            onChange={(event) =>
                              handleCheck(event, "h2_aar", 47)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H2_AAR"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H2_AAR"
                          />
                          2 år
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[49]}
                            onChange={(event) =>
                              handleCheck(event, "h5_10_aar", 49)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H5_10_AAR"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H5_10_AAR"
                          />
                          6-10 år
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[51]}
                            onChange={(event) =>
                              handleCheck(event, "h20_30_aar", 51)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H20_30_AAR"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H20_30_AAR"
                          />
                          21-30 år
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[53]}
                            onChange={(event) =>
                              handleCheck(event, "h40_50_aar", 53)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H40_50_AAR"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H40_50_AAR"
                          />
                          41-50 år
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[55]}
                            onChange={(event) =>
                              handleCheck(event, "h_ukjent", 55)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H_UKJENT"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H_UKJENT"
                          />
                          Ukjent
                        </label>
                      </div>
                    </div>
                  </div>
                </div>
              ) : null}
            </div>

            <div className="card Kj-background-color ml-1 mr-1 mt-1 ">
              <div className="divsubcolor">
                <div className="row">
                  <div className="col-10">
                    <p className="avan1 p-1">Boligstørrelse</p>
                  </div>
                  <div className="col-2">
                    {togglevalue5 ? (
                      <img
                        className="d-flex float-right pt-1"
                        src={collapse}
                        onClick={toggle5}
                      />
                    ) : (
                      <img
                        className="d-flex float-right pt-1"
                        src={expand}
                        onClick={toggle5}
                      />
                    )}
                  </div>
                </div>
              </div>
              {togglevalue5 ? (
                <div className="demografic-container">
                  <div className="row nowrap">
                    <div className="col-lg-6 col-md-6 col-sm-12 col-xs-12 ">
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[36]}
                            onChange={(event) =>
                              handleCheck(event, "aru_50", 36)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_ARU_50"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$ARU_50"
                          />
                          Under 50 m2
                        </label>
                      </div>

                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[38]}
                            onChange={(event) =>
                              handleCheck(event, "ar60_79", 38)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR60_79"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR60_79"
                          />
                          60-79 m2
                        </label>
                      </div>

                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[40]}
                            onChange={(event) =>
                              handleCheck(event, "ar100_119", 40)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR100_119"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR100_119"
                          />
                          100-119 m2
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[42]}
                            onChange={(event) =>
                              handleCheck(event, "ar140_159", 42)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR140_159"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR140_159"
                          />
                          140-159 m2
                        </label>
                      </div>

                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[44]}
                            onChange={(event) =>
                              handleCheck(event, "ar200_249", 44)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR200_249"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR200_249"
                          />
                          200-249 m2
                        </label>
                      </div>
                    </div>
                    <div className="col-lg-6 col-md-6 col-sm-12 col-xs-12 ">
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[37]}
                            onChange={(event) =>
                              handleCheck(event, "ar50_59", 37)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR50_59"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR50_59"
                          />
                          50-59 m2
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[39]}
                            onChange={(event) =>
                              handleCheck(event, "ar80_99", 39)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR80_99"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR80_99"
                          />
                          80-99 m2
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[41]}
                            onChange={(event) =>
                              handleCheck(event, "ar120_139", 41)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR120_139"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR120_139"
                          />
                          120-139 m2
                        </label>
                      </div>

                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[43]}
                            onChange={(event) =>
                              handleCheck(event, "ar160_199", 43)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR160_199"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR160_199"
                          />
                          160-199 m2
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[45]}
                            onChange={(event) =>
                              handleCheck(event, "ar250_o", 45)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR250_O"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR250_O"
                          />
                          Over 250 m2
                        </label>
                      </div>
                    </div>
                  </div>
                </div>
              ) : null}
            </div>

            <div className="card Kj-background-color ml-1 mr-1 mt-1">
              <div className="divsubcolor">
                <div className="row">
                  <div className="col-10">
                    <p className="avan1 p-1">Personinntekt</p>
                  </div>
                  <div className="col-2">
                    {togglevalue9 ? (
                      <img
                        className="d-flex float-right pt-1"
                        src={collapse}
                        onClick={toggle9}
                      />
                    ) : (
                      <img
                        className="d-flex float-right pt-1"
                        src={expand}
                        onClick={toggle9}
                      />
                    )}
                  </div>
                </div>
              </div>
              {togglevalue9 ? (
                <div className="container demografic-container center">
                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline toggletext">
                      <input
                        checked={checkboxvalue[13]}
                        onChange={(event) => handleCheck(event, "int0", 13)}
                        id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT0"
                        type="checkbox"
                        name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT0"
                      />
                      Ingen inntekt
                    </label>
                  </div>
                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline toggletext">
                      <input
                        checked={checkboxvalue[14]}
                        onChange={(event) => handleCheck(event, "int0_100", 14)}
                        id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT0_100"
                        type="checkbox"
                        name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT0_100"
                      />
                      0 000kr - 100 000kr
                    </label>
                  </div>
                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline toggletext">
                      <input
                        checked={checkboxvalue[15]}
                        onChange={(event) =>
                          handleCheck(event, "int100_200", 15)
                        }
                        id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT100_200"
                        type="checkbox"
                        name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT100_200"
                      />
                      100 000kr - 200 000kr
                    </label>
                  </div>
                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline toggletext">
                      <input
                        checked={checkboxvalue[16]}
                        onChange={(event) =>
                          handleCheck(event, "int200_300", 16)
                        }
                        id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT200_300"
                        type="checkbox"
                        name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT200_300"
                      />
                      200 000kr - 300 000kr
                    </label>
                  </div>
                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline toggletext">
                      <input
                        checked={checkboxvalue[17]}
                        onChange={(event) =>
                          handleCheck(event, "int300_400", 17)
                        }
                        id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT300_400"
                        type="checkbox"
                        name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT300_400"
                      />
                      300 000kr - 4000 000kr
                    </label>
                  </div>
                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline toggletext">
                      <input
                        checked={checkboxvalue[18]}
                        onChange={(event) =>
                          handleCheck(event, "int400_500", 18)
                        }
                        id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT400_500"
                        type="checkbox"
                        name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT400_500"
                      />
                      400 000kr - 500 000kr
                    </label>
                  </div>
                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline toggletext">
                      <input
                        checked={checkboxvalue[19]}
                        onChange={(event) =>
                          handleCheck(event, "int500_600", 19)
                        }
                        id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT500_600"
                        type="checkbox"
                        name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT500_600"
                      />
                      500 000kr - 600 000kr
                    </label>
                  </div>
                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline toggletext">
                      <input
                        checked={checkboxvalue[20]}
                        onChange={(event) =>
                          handleCheck(event, "int600_700", 20)
                        }
                        id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT600_700"
                        type="checkbox"
                        name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT600_700"
                      />
                      600 000kr - 700 000kr
                    </label>
                  </div>
                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline toggletext">
                      <input
                        checked={checkboxvalue[21]}
                        onChange={(event) =>
                          handleCheck(event, "int700_800", 21)
                        }
                        id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT700_800"
                        type="checkbox"
                        name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT700_800"
                      />
                      700 000kr - 800 000kr
                    </label>
                  </div>
                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline toggletext">
                      <input
                        checked={checkboxvalue[22]}
                        onChange={(event) =>
                          handleCheck(event, "int800_1000", 22)
                        }
                        id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT800_1000"
                        type="checkbox"
                        name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT800_1000"
                      />
                      800 000kr - 1000 000kr
                    </label>
                  </div>
                  <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                    <label className="checkbox-inline toggletext">
                      <input
                        checked={checkboxvalue[23]}
                        onChange={(event) =>
                          handleCheck(event, "int1000_1500", 23)
                        }
                        id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT1000_1500"
                        type="checkbox"
                        name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT1000_1500"
                      />
                      1000 000kr - 1500 000kr
                    </label>
                  </div>
                </div>
              ) : null}
            </div>

            <div className="card Kj-background-color ml-1 mr-1 mt-1">
              <div className="divsubcolor">
                <div className="row">
                  <div className="col-10">
                    <p className="avan1 p-1">Husholdning</p>
                  </div>
                  <div className="col-2">
                    {togglevalue6 ? (
                      <img
                        className="d-flex float-right pt-1"
                        src={collapse}
                        onClick={toggle6}
                      />
                    ) : (
                      <img
                        className="d-flex float-right pt-1"
                        src={expand}
                        onClick={toggle6}
                      />
                    )}
                  </div>
                </div>
              </div>
              {togglevalue6 ? (
                <div className="container demografic-container center">
                  <div className="row">
                    <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[24]}
                            onChange={(event) =>
                              handleCheck(event, "enslig_u_b", 24)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPHusholdning_ENSLIG_U_B"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPHusholdning$ENSLIG_U_B"
                          />
                          Enslig uten barn
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[26]}
                            onChange={(event) =>
                              handleCheck(event, "par_u_barn", 26)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPHusholdning_PAR_U_BARN"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPHusholdning$PAR_U_BARN"
                          />
                          Par uten barn
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[28]}
                            onChange={(event) => handleCheck(event, "par", 28)}
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPHusholdning_PAR"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPHusholdning$PAR"
                          />
                          Par
                        </label>
                      </div>
                    </div>
                    <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[25]}
                            onChange={(event) =>
                              handleCheck(event, "enslig_m_b", 25)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPHusholdning_ENSLIG_M_B"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPHusholdning$ENSLIG_M_B"
                          />
                          Enslig med barn
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[27]}
                            onChange={(event) =>
                              handleCheck(event, "par_m_barn", 27)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPHusholdning_PAR_m_BARN"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPHusholdning$PAR_m_BARN"
                          />
                          Par med barn
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[29]}
                            onChange={(event) =>
                              handleCheck(event, "flerfamili", 29)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPHusholdning_FLERFAMILI"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPHusholdning$FLERFAMILI"
                          />
                          Flerfamilie
                        </label>
                      </div>
                    </div>
                  </div>
                </div>
              ) : null}
            </div>

            <div className="card Kj-background-color ml-1 mr-1 mt-1 ">
              <div className="divsubcolor">
                <div className="row">
                  <div className="col-10">
                    <p className="avan1 p-1">Biltype</p>
                  </div>
                  <div className="col-2">
                    {togglevalue7 ? (
                      <img
                        className="d-flex float-right pt-1"
                        src={collapse}
                        onClick={toggle7}
                      />
                    ) : (
                      <img
                        className="d-flex float-right pt-1"
                        src={expand}
                        onClick={toggle7}
                      />
                    )}
                  </div>
                </div>
              </div>
              {togglevalue7 ? (
                <div className="container demografic-container center">
                  <div className="row ">
                    <div className="col-lg-6 col-md-6 col-sm-12 col-xs-12 m-0 p-0">
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[56]}
                            onChange={(event) => handleCheck(event, "Audi", 56)}
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_AUDI"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$AUDI"
                          />
                          Audi
                        </label>
                      </div>

                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[60]}
                            onChange={(event) =>
                              handleCheck(event, "Citroen", 60)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_CITROEN"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$CITROEN"
                          />
                          Citroen
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[62]}
                            onChange={(event) => handleCheck(event, "Ford", 62)}
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_FORD"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$FORD"
                          />
                          Ford
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[64]}
                            onChange={(event) =>
                              handleCheck(event, "Hyundai", 64)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_HYUNDAI"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$HYUNDAI"
                          />
                          Hyundai
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[58]}
                            onChange={(event) =>
                              handleCheck(event, "land_rover", 58)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_LandRover"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$LandRover"
                          />
                          Land Rover
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[66]}
                            onChange={(event) =>
                              handleCheck(event, "Mazda", 66)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_MAZDA"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$MAZDA"
                          />
                          Mazda
                        </label>
                      </div>

                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[69]}
                            onChange={(event) =>
                              handleCheck(event, "Nissan", 69)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_NISSAN"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$NISSAN"
                          />
                          Nissan
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[71]}
                            onChange={(event) =>
                              handleCheck(event, "Peugeot", 71)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_PEUGEOT"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$PEUGEOT"
                          />
                          Peugeot
                        </label>{" "}
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[75]}
                            onChange={(event) =>
                              handleCheck(event, "Subaru", 75)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_SUBARU"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$SUBARU"
                          />
                          Subaru
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[74]}
                            onChange={(event) =>
                              handleCheck(event, "Skoda", 74)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_SKODA"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$SKODA"
                          />
                          Skoda
                        </label>{" "}
                      </div>

                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[59]}
                            onChange={(event) =>
                              handleCheck(event, "Tesla", 59)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_Tesla"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$Tesla"
                          />
                          Tesla
                        </label>{" "}
                      </div>

                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[80]}
                            onChange={(event) =>
                              handleCheck(event, "Volvo", 80)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_VOLVO"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$VOLVO"
                          />
                          Volvo
                        </label>{" "}
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[81]}
                            onChange={(event) =>
                              handleCheck(event, "andre_merk", 81)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_ANDRE_MERK"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$ANDRE_MERK"
                          />
                          Andre
                        </label>
                      </div>
                    </div>
                    <div className="col-lg-6 col-md-6 col-sm-12 col-xs-12 m-0 p-0">
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[57]}
                            onChange={(event) => handleCheck(event, "BMW", 57)}
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_BMW"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$BMW"
                          />
                          BMW
                        </label>
                      </div>

                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[61]}
                            onChange={(event) => handleCheck(event, "Fiat", 61)}
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_FIAT"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$FIAT"
                          />
                          Fiat
                        </label>
                      </div>

                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[63]}
                            onChange={(event) =>
                              handleCheck(event, "Honda", 63)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_HONDA"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$HONDA"
                          />
                          Honda
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[65]}
                            onChange={(event) => handleCheck(event, "Kia", 65)}
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_KIA"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$KIA"
                          />
                          Kia
                        </label>{" "}
                      </div>

                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[67]}
                            onChange={(event) =>
                              handleCheck(event, "MERCEDES_BENZ", 67)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_MERCEDES_BENZ"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$MERCEDES_BENZ"
                          />
                          Mercedes Benz
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[68]}
                            onChange={(event) =>
                              handleCheck(event, "Mitsubishi", 68)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_MITSUBISHI"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$MITSUBISHI"
                          />
                          Mitsubishi
                        </label>{" "}
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[70]}
                            onChange={(event) => handleCheck(event, "Opel", 70)}
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_OPEL"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$OPEL"
                          />
                          Opel
                        </label>
                      </div>

                      {/* <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[73]}
                            onChange={(event) => handleCheck(event, "Seat", 73)}
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_SEAT"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$SEAT"
                          />
                          Seat
                        </label>
                      </div> */}
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[72]}
                            onChange={(event) =>
                              handleCheck(event, "Renault", 72)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_RENAULT"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$RENAULT"
                          />
                          Renault
                        </label>
                      </div>

                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[77]}
                            onChange={(event) => handleCheck(event, "Saab", 77)}
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_SAAB"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$SAAB"
                          />
                          Saab
                        </label>{" "}
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[76]}
                            onChange={(event) =>
                              handleCheck(event, "Suzuki", 76)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_SUZUKI"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$SUZUKI"
                          />
                          Suzuki
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[78]}
                            onChange={(event) =>
                              handleCheck(event, "Toyota", 78)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_TOYOTA"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$TOYOTA"
                          />
                          Toyota
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[79]}
                            onChange={(event) =>
                              handleCheck(event, "Volkswagen", 79)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_VOLKSWAGEN"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$VOLKSWAGEN"
                          />
                          Volkswagen
                        </label>
                      </div>
                    </div>
                  </div>
                </div>
              ) : null}
            </div>

            <div className="card Kj-background-color ml-1 mr-1 mt-1">
              <div className="divsubcolor">
                <div className="row">
                  <div className="col-10">
                    <p className="avan1 p-1">Bilens alder</p>
                  </div>
                  <div className="col-2">
                    {togglevalue8 ? (
                      <img
                        className="d-flex float-right pt-1"
                        src={collapse}
                        onClick={toggle8}
                      />
                    ) : (
                      <img
                        className="d-flex float-right pt-1"
                        src={expand}
                        onClick={toggle8}
                      />
                    )}
                  </div>
                </div>
              </div>
              {togglevalue8 ? (
                <div className="container demografic-container center">
                  <div className="row">
                    <div className="col-lg-6 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[82]}
                            onChange={(event) =>
                              handleCheck(event, "bilalder_20_", 82)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_20_"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_20_"
                          />
                          Biler bygget for 20- år siden eller mer
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[84]}
                            onChange={(event) =>
                              handleCheck(event, "bilalder_14_15", 84)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_14_15"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_14_15"
                          />
                          14-15 år siden
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext ">
                          <input
                            checked={checkboxvalue[86]}
                            onChange={(event) =>
                              handleCheck(event, "bilalder_10_11", 86)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_10_11"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_10_11"
                          />
                          10-11 år siden
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[88]}
                            onChange={(event) =>
                              handleCheck(event, "bilalder_6_7", 88)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_6_7"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_6_7"
                          />
                          6-7 år siden
                        </label>{" "}
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[90]}
                            onChange={(event) =>
                              handleCheck(event, "bilalder_4", 90)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_4"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_4"
                          />
                          4 år siden
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[92]}
                            onChange={(event) =>
                              handleCheck(event, "bilalder_2", 92)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_2"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_2"
                          />
                          2 år siden
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[94]}
                            onChange={(event) =>
                              handleCheck(event, "bilalder_0", 94)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_0"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_0"
                          />
                          under 1 år
                        </label>{" "}
                      </div>
                    </div>

                    {/* </div> */}

                    <div className="col-lg-6 col-md-6 col-sm-12 col-xs-12 m-0 p-0">
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[83]}
                            onChange={(event) =>
                              handleCheck(event, "bilalder_16_19", 83)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_16_19"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_16_19"
                          />
                          16-19 år siden
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[85]}
                            onChange={(event) =>
                              handleCheck(event, "bilalder_12_13", 85)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_12_13"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_12_13"
                          />
                          12-13 år siden
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[87]}
                            onChange={(event) =>
                              handleCheck(event, "bilalder_8_9", 87)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_8_9"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_8_9"
                          />
                          8-9 år siden
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[89]}
                            onChange={(event) =>
                              handleCheck(event, "bilalder_5", 89)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_5"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_5"
                          />
                          5 år siden
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[91]}
                            onChange={(event) =>
                              handleCheck(event, "bilalder_3", 91)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_3"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_3"
                          />
                          3 år siden
                        </label>
                      </div>
                      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <label className="checkbox-inline toggletext">
                          <input
                            checked={checkboxvalue[93]}
                            onChange={(event) =>
                              handleCheck(event, "bilalder_1", 93)
                            }
                            id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_1"
                            type="checkbox"
                            name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_1"
                          />
                          1 år siden
                        </label>
                      </div>
                    </div>
                  </div>
                </div>
              ) : null}
            </div>
          </div>
        ) : currentStep == 2 ? (
          <DemografikeOmrade />
        ) : currentStep == 3 ? (
          <SelectionDetails />
        ) : null}
        <div className="divcolor pl-1 pr-1 pt-1 ">
          <Submit_Button
            tabvalue={20}
            setDemografikAntalMsg={setDemografikAntalMsg}
            setdemografikmsg={setdemografikmsg}
            demografikeSubmitValue={existingActive}
            currentStep={currentStep}
            setCurrentStep={setCurrentStep}
          />
        </div>
      </div>
    </UtvalgContext.Provider>
  );
}

export default Demografie;
