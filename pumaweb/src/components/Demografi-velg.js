import React, { useState, useContext, useEffect } from "react";
import { KundeWebContext } from "../context/Context.js";
import Item from "antd/lib/list/Item";
import { MapConfig } from "../config/mapconfig";
import Extent from "@arcgis/core/geometry/Extent";
import { MainPageContext } from "../context/Context.js";

function DemografiVelg({ parentCallback }) {
  const [AlderCheck, setAlderCheck] = useState(true);
  const [UtdannCheck, setUtdannCheck] = useState(false);
  const [PersonCheck, setPersonCheck] = useState(false);
  const [HushCheck, setHushCheck] = useState(false);
  const [BoligTCheck, setBoligTCheck] = useState(false);
  const [BoligsCheck, setBoligsCheck] = useState(false);
  const [BoligalderCheck, setBoligalderCheck] = useState(false);
  const [BiltypeCheck, setBiltypeCheck] = useState(false);
  const [BilensCheck, setBilensCheck] = useState(false);
  const { Page, setPage } = useContext(KundeWebContext);
  const { Page_P, setPage_P } = useContext(KundeWebContext);
  const [nomessagediv, setnomessagediv] = useState(false);
  const [noaddress, setnoaddress] = useState(false);
  const { selecteddemografiecheckbox, setselecteddemografiecheckbox } =
    useContext(KundeWebContext);
  const { selecteddemografiecheckbox_c, setselecteddemografiecheckbox_c } =
    useContext(KundeWebContext);
  const { globalBilType, setGlobalBilType } = useContext(KundeWebContext);
  const [checkboxvalue, setcheckboxvalue] = useState(Array(95).fill(false));
  const { criteriaObject, setCriteriaObject } = useContext(KundeWebContext);
  const [selectedCheckboxes, setSelectedCheckboxes] = useState([]);
  const [refresh, setRefresh] = useState(false);
  const { mapView } = useContext(MainPageContext);

  let arr = selecteddemografiecheckbox;
  let arr_index = selecteddemografiecheckbox_c;
  const handleCheck = (event, value, index) => {
    if (event.target.checked === true) {
      selectedCheckboxes.push(value);
      setCriteriaObject({
        ...criteriaObject,
        demograCheckedItems: selectedCheckboxes,
      });
    } else {
      var filteredArr = selectedCheckboxes.filter((val) => {
        return val !== value;
      });
      setSelectedCheckboxes(filteredArr);
      setCriteriaObject({
        ...criteriaObject,
        demograCheckedItems: filteredArr,
      });
    }

    let _checkboxvalue = [...checkboxvalue];
    _checkboxvalue[index] = event.target.checked;
    setcheckboxvalue(_checkboxvalue);

    if (event.target.checked == false) {
      let k = selecteddemografiecheckbox;
      k.push(value);
      setselecteddemografiecheckbox(k);
      let s = selecteddemografiecheckbox_c;
      const indexOfIndex = s.indexOf(index);
      setselecteddemografiecheckbox_c(s);
    } else {
      // arr.push(value);
      if (value.includes("-")) {
        arr.push("ALD" + value);
      } else {
        arr.push(value);
      }
      arr_index.push(index);
      setselecteddemografiecheckbox_c(arr_index);
      let d = arr.join(",");
      setselecteddemografiecheckbox(arr);
    }
  };

  useEffect(() => {
    setselecteddemografiecheckbox([]);
    setCriteriaObject({
      ...criteriaObject,
      demograFeature: "Alder alle",
    });
  }, []);

  const goback = () => {
    setPage("");
    mapView.extent = new Extent(MapConfig.kundewebMapExtent);
    mapView.goTo(mapView.extent);
  };
  const GotoMain = () => {
    setPage("");
  };
  const nomessage = () => {
    noaddress ? setnomessagediv(true) : setnomessagediv(false);
    setPage("Demogra_Velg_Click");
    setPage_P("Demografivelg");
  };

  const UtdannClick = () => {
    setSelectedCheckboxes([]);
    setcheckboxvalue(Array(95).fill(false));
    setAlderCheck(false);
    setUtdannCheck(true);
    setHushCheck(false);
    setPersonCheck(false);
    setBoligTCheck(false);
    setBoligsCheck(false);
    setBoligalderCheck(false);
    setBiltypeCheck(false);
    setBilensCheck(false);
    setCriteriaObject({
      ...criteriaObject,
      demograFeature: "Utdanningsnivå",
      demograCheckedItems: [],
    });
    // selecteddemografiecheckbox.map(item=> {return
    // document.getElementById(item).checked = true})
  };

  const AlderClick = async () => {
    setSelectedCheckboxes([]);
    setcheckboxvalue(Array(95).fill(false));
    setAlderCheck(true);
    setUtdannCheck(false);
    setHushCheck(false);
    setPersonCheck(false);
    setBoligTCheck(false);
    setBoligsCheck(false);
    setBoligalderCheck(false);
    setBiltypeCheck(false);
    setGlobalBilType(false);
    setBilensCheck(false);
    setCriteriaObject({
      ...criteriaObject,
      demograFeature: "Alder alle",
      demograCheckedItems: [],
    });
    // selecteddemografiecheckbox.map(item=>     {if(collection.includes(item))
    //    return document.getElementById(item).checked = true}         )
  };
  const PersonClick = () => {
    setSelectedCheckboxes([]);
    setcheckboxvalue(Array(95).fill(false));
    setAlderCheck(false);
    setUtdannCheck(false);
    setHushCheck(false);
    setPersonCheck(true);
    setBoligTCheck(false);
    setBoligsCheck(false);
    setBoligalderCheck(false);
    setBiltypeCheck(false);
    setGlobalBilType(false);
    setBilensCheck(false);
    setCriteriaObject({
      ...criteriaObject,
      demograFeature: "Personinntekt",
      demograCheckedItems: [],
    });
  };
  const HushClick = () => {
    setSelectedCheckboxes([]);
    setcheckboxvalue(Array(95).fill(false));
    setAlderCheck(false);
    setUtdannCheck(false);
    setHushCheck(true);
    setPersonCheck(false);
    setBoligTCheck(false);
    setBoligsCheck(false);
    setBoligalderCheck(false);
    setBiltypeCheck(false);
    setGlobalBilType(false);
    setBilensCheck(false);
    setCriteriaObject({
      ...criteriaObject,
      demograFeature: "Husholdning",
      demograCheckedItems: [],
    });
  };
  const BoligTClick = () => {
    setSelectedCheckboxes([]);
    setcheckboxvalue(Array(95).fill(false));
    setAlderCheck(false);
    setUtdannCheck(false);
    setHushCheck(false);
    setPersonCheck(false);
    setBoligTCheck(true);
    setBoligsCheck(false);
    setBoligalderCheck(false);
    setBiltypeCheck(false);
    setGlobalBilType(false);
    setBilensCheck(false);
    setCriteriaObject({
      ...criteriaObject,
      demograFeature: "Boligtype",
      demograCheckedItems: [],
    });
  };
  const BoligsClick = () => {
    setSelectedCheckboxes([]);
    setcheckboxvalue(Array(95).fill(false));
    setAlderCheck(false);
    setUtdannCheck(false);
    setHushCheck(false);
    setPersonCheck(false);
    setBoligTCheck(false);
    setBoligsCheck(true);
    setBoligalderCheck(false);
    setBiltypeCheck(false);
    setGlobalBilType(false);
    setBilensCheck(false);
    setCriteriaObject({
      ...criteriaObject,
      demograFeature: "Boligstørrelse",
      demograCheckedItems: [],
    });
  };
  const BoligalderClick = () => {
    setSelectedCheckboxes([]);
    setcheckboxvalue(Array(95).fill(false));
    setAlderCheck(false);
    setUtdannCheck(false);
    setHushCheck(false);
    setPersonCheck(false);
    setBoligTCheck(false);
    setBoligsCheck(false);
    setBoligalderCheck(true);
    setBiltypeCheck(false);
    setGlobalBilType(false);
    setBilensCheck(false);
    setCriteriaObject({
      ...criteriaObject,
      demograFeature: "Boligalder",
      demograCheckedItems: [],
    });
  };

  const BiltypeClick = () => {
    setSelectedCheckboxes([]);
    setcheckboxvalue(Array(95).fill(false));
    setAlderCheck(false);
    setUtdannCheck(false);
    setHushCheck(false);
    setPersonCheck(false);
    setBoligTCheck(false);
    setBoligsCheck(false);
    setBoligalderCheck(false);
    setBiltypeCheck(true);
    setGlobalBilType(true);
    setBilensCheck(false);
    setCriteriaObject({
      ...criteriaObject,
      demograFeature: "Biltype",
      demograCheckedItems: [],
    });
  };
  const BilensClick = () => {
    setSelectedCheckboxes([]);
    setcheckboxvalue(Array(95).fill(false));
    setAlderCheck(false);
    setUtdannCheck(false);
    setHushCheck(false);
    setPersonCheck(false);
    setBoligTCheck(false);
    setBoligsCheck(false);
    setBoligalderCheck(false);
    setBiltypeCheck(false);
    setGlobalBilType(false);
    setBilensCheck(true);
    setCriteriaObject({
      ...criteriaObject,
      demograFeature: "Bilens alder",
      demograCheckedItems: [],
    });
  };

  return (
    <div className="col-5 p-2">
      <div className="paddingBig_NoColor_B">
        <span className=" title ">Lag utvalg basert på demografi</span>

        <div className="padding_NoColor_T lblAnalysisHeaderDesc">
          <p
            id="DemografiAnalyse1_uxHeader_lblDesc"
            className="lblAnalysisHeaderDesc"
          >
            Vi vet hvor personer med forskjellige demografiske variabler bor.
          </p>
          <p>
            Du kan velge ut budruter der det bor mennesker med de beskrivelsene
            du ønsker.
          </p>
        </div>
        <div className="">
          <p className="lblAnalysisHeaderStep ">
            {" "}
            <b>1. Velg kjennetegn for målgruppen din</b>
            <br />
            <p className="pl-3">
              Du kan kun velge en variabel (Flere variabler kan kombineres i en
              mer utfyllende analyse – ta i tilfelle kontakt med kundeservice på
              04045)
            </p>
            2. Velg geografisk område
            <br />
            3. Velg antallet mottakere du ønsker å sende ut til
          </p>
        </div>

        {nomessagediv ? (
          <div className="pr-3">
            <div className="error WarningSign">
              <div className="divErrorHeading">Melding:</div>
              <span
                id="uxKjoreAnalyse_uxLblMessage"
                className="divErrorText_kw"
              >
                Ingen adresser tilgjengelig
              </span>
            </div>
          </div>
        ) : null}
        <p></p>

        <table className="padding_NoColor_B width100 pb-2">
          <tbody>
            <tr>
              <td>
                {/* <span id="DemografiAnalyse1_uxLblATitle" className="divHeaderText_kw">Velg kun én av variablene nedenfor. De ulike variablene kan ikke kombineres (med unntak av alder og kjønn). <br/> Det er  fullt mulig å lage utvalg med flere kombinerte variabler, ta i tilfelle kontakt med Bring Kundeservice 04045</span> */}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div className="padding_Color_L_R_T_B">
        <div className="card Kj-background-color-kw ml-1 mr-1 mt-2 ">
          <table border="0" width="100%" bgcolor="#919195">
            <tbody>
              <tr>
                <td className="pt-2 ">
                  <span className="rbtnDockedPanel_demografi">
                    &nbsp;&nbsp;
                    <input
                      id="DemografiAnalyse1_DemografiKriterier1_rbuxFPAlder"
                      type="radio"
                      name="DemografiAnalyse1$DemografiKriterier1$aa"
                      value="rbuxFPAlder"
                      checked={AlderCheck}
                      onChange={AlderClick}
                    />
                    <label className="pl-2 Demografie_text"> Alder</label>
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
          {AlderCheck ? (
            <div className="container demografic-container-kw center">
              <div className="row">
                <div className="col-4">
                  <label className="checkbox-inline sok-text pl-2">
                    <input
                      type="checkbox"
                      id="19-23"
                      value="ald19_23"
                      checked={checkboxvalue[0]}
                      onChange={(event) => handleCheck(event, "ald19_23", 0)}
                    />{" "}
                    &nbsp; 19-23
                  </label>
                  <label className="checkbox-inline sok-text pl-2">
                    <input
                      type="checkbox"
                      value="ald45_54"
                      id="45-54"
                      checked={checkboxvalue[1]}
                      onChange={(event) => handleCheck(event, "ald45_54", 1)}
                    />{" "}
                    &nbsp; 45-54
                  </label>
                  <label className="checkbox-inline sok-text pl-2">
                    <input
                      type="checkbox"
                      value="ald75_84"
                      id="75-84"
                      checked={checkboxvalue[2]}
                      onChange={(event) => handleCheck(event, "ald75_84", 2)}
                    />{" "}
                    &nbsp; 75-84
                  </label>
                </div>
                <div className="col-4">
                  <label className="checkbox-inline sok-text">
                    <input
                      type="checkbox"
                      value="ald24_34"
                      id="24-34"
                      checked={checkboxvalue[3]}
                      onChange={(event) => handleCheck(event, "ald24_34", 3)}
                    />{" "}
                    &nbsp; 24-34
                  </label>
                  <label className="checkbox-inline sok-text">
                    <input
                      type="checkbox"
                      value="ald55_64"
                      id="55-64"
                      checked={checkboxvalue[4]}
                      onChange={(event) => handleCheck(event, "ald55_64", 4)}
                    />{" "}
                    &nbsp; 55-64
                  </label>
                  <label className="checkbox-inline sok-text">
                    <input
                      type="checkbox"
                      value="ald85_o"
                      id="85-94"
                      checked={checkboxvalue[5]}
                      onChange={(event) => handleCheck(event, "ald85_o", 5)}
                    />{" "}
                    &nbsp; 85-94
                  </label>
                </div>
                <div className="col-4">
                  <label className="checkbox-inline sok-text">
                    <input
                      type="checkbox"
                      value="ald35_44"
                      id="35-44"
                      checked={checkboxvalue[6]}
                      onChange={(event) => handleCheck(event, "ald35_44", 6)}
                    />{" "}
                    &nbsp; 35-44
                  </label>
                  <label className="checkbox-inline sok-text">
                    <input
                      type="checkbox"
                      value="ald65_74"
                      id="65-74"
                      checked={checkboxvalue[7]}
                      onChange={(event) => handleCheck(event, "ald65_74", 7)}
                    />{" "}
                    &nbsp; 65-74
                  </label>
                </div>
              </div>
            </div>
          ) : null}
        </div>

        <div className="card Kj-background-color-kw ml-1 mr-1 mt-2 ">
          <table border="0" width="100%" bgcolor="#919195">
            <tbody>
              <tr>
                <td className="pt-2">
                  <span className="rbtnDockedPanel_demografi">
                    &nbsp;&nbsp;
                    <input
                      id="DemografiAnalyse1_DemografiKriterier1_rbuxFPAlder"
                      type="radio"
                      name="DemografiAnalyse1$DemografiKriterier1$aa"
                      value="rbuxFPAlder"
                      onChange={UtdannClick}
                    />
                    <label className="pl-2 Demografie_text">
                      {" "}
                      Utdanningsnivå
                    </label>
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
          {UtdannCheck ? (
            <div className="container demografic-container-kw center pt-2">
              <div className="row">
                <div className="col-6">
                  <span className="divCheckText_demografi">
                    <input
                      type="checkbox"
                      checked={checkboxvalue[8]}
                      onChange={(event) => handleCheck(event, "ingen_uopp", 8)}
                      id="Ingen oppgitt utdanning"
                      className="mb-2"
                    />{" "}
                    Ingen oppgitt utdanning
                  </span>
                </div>
                <div className="col-6">
                  <span className="divCheckText_demografi">
                    <input
                      type="checkbox"
                      checked={checkboxvalue[9]}
                      onChange={(event) => handleCheck(event, "grunnskole", 9)}
                      className="mb-2"
                    />{" "}
                    Grunnskole
                  </span>
                </div>
              </div>
              <div className="row">
                <div className="col-6">
                  <span className="divCheckText_demografi">
                    <input
                      checked={checkboxvalue[10]}
                      onChange={(event) => handleCheck(event, "videregaen", 10)}
                      id="DemografiAnalyse1_DemografiKriterier1_uxFPUtdanningsNiva_VIDEREGAEN"
                      type="checkbox"
                      className="mb-2"
                      name="DemografiAnalyse1$DemografiKriterier1$uxFPUtdanningsNiva$VIDEREGAEN"
                    />{" "}
                    Videregående skole
                  </span>
                </div>
                <div className="col-6">
                  <span className="divCheckText_demografi">
                    <input
                      checked={checkboxvalue[11]}
                      onChange={(event) => handleCheck(event, "hogskole_u", 11)}
                      className="mb-3"
                      type="checkbox"
                      name="DemografiAnalyse1$DemografiKriterier1$uxFPUtdanningsNiva$HOGSKOLE_U"
                    />{" "}
                    Høgskole/Universitet lavt nivå
                  </span>
                </div>
              </div>
              <div className="row mb-2">
                <div className="col-7">
                  <span className="divCheckText_demografi">
                    <input
                      checked={checkboxvalue[12]}
                      onChange={(event) => handleCheck(event, "hogskole_1", 12)}
                      type="checkbox"
                      name="DemografiAnalyse1$DemografiKriterier1$uxFPUtdanningsNiva$HOGSKOLE_1"
                      className="mb-3"
                    />{" "}
                    Høgskole/Universitet høyt nivå
                  </span>
                </div>
              </div>
            </div>
          ) : null}
        </div>

        <div className="card Kj-background-color-kw ml-1 mr-1 mt-2 ">
          <table border="0" width="100%" bgcolor="#919195">
            <tbody>
              <tr>
                <td className="pt-2">
                  <span className="rbtnDockedPanel_demografi">
                    &nbsp;&nbsp;
                    <input
                      id="DemografiAnalyse1_DemografiKriterier1_rbuxFPAlder"
                      type="radio"
                      name="DemografiAnalyse1$DemografiKriterier1$aa"
                      onChange={PersonClick}
                      value="rbuxFPAlder"
                    />
                    <label className="pl-2 Demografie_text">
                      Personinntekt
                    </label>
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
          {PersonCheck ? (
            <div className="container demografic-container-kw center pt-2">
              <table border="0" width="100%">
                <tbody>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[13]}
                          onChange={(event) => handleCheck(event, "int0", 13)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT0"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT0"
                          className="mb-2"
                        />
                        <label className="pl-1">Ingen inntekt</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[14]}
                          onChange={(event) =>
                            handleCheck(event, "int0_100", 14)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT0_100"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT0_100"
                          className="mb-2"
                        />
                        <label className="pl-1">0 000kr - 100 000kr</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[15]}
                          onChange={(event) =>
                            handleCheck(event, "int100_200", 15)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT100_200"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT100_200"
                          className="mb-2"
                        />
                        <label className="pl-1">100 000kr - 200 000kr</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[16]}
                          onChange={(event) =>
                            handleCheck(event, "int200_300", 16)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT200_300"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT200_300"
                          className="mb-2"
                        />
                        <label className="pl-1">200 000kr - 300 000kr</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[17]}
                          onChange={(event) =>
                            handleCheck(event, "int300_400", 17)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT300_400"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT300_400"
                          className="mb-2"
                        />
                        <label className="pl-1">300 000kr - 4000 000kr</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[18]}
                          onChange={(event) =>
                            handleCheck(event, "int400_500", 18)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT400_500"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT400_500"
                          className="mb-2"
                        />
                        <label className="pl-1">400 000kr - 500 000kr</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[19]}
                          onChange={(event) =>
                            handleCheck(event, "int500_600", 19)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT500_600"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT500_600"
                          className="mb-2"
                        />
                        <label className="pl-1">500 000kr - 600 000kr</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[20]}
                          onChange={(event) =>
                            handleCheck(event, "int600_700", 20)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT600_700"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT600_700"
                          className="mb-2"
                        />
                        <label className="pl-1">600 000kr - 700 000kr</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[21]}
                          onChange={(event) =>
                            handleCheck(event, "int700_800", 21)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT700_800"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT700_800"
                          className="mb-2"
                        />
                        <label className="pl-1">700 000kr - 800 000kr</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[22]}
                          onChange={(event) =>
                            handleCheck(event, "int800_1000", 22)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT800_1000"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT800_1000"
                          className="mb-4"
                        />
                        <label className="pl-1">800 000kr - 1000 000kr</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[23]}
                          onChange={(event) =>
                            handleCheck(event, "int1000_1500", 23)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPPersoninntekt_INT1000_1500"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPPersoninntekt$INT1000_1500"
                          className="mb-4"
                        />
                        <label className="pl-1">1000 000kr - 1500 000kr</label>
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          ) : null}
        </div>

        <div className="card Kj-background-color-kw ml-1 mr-1 mt-2 ">
          <table border="0" width="100%" bgcolor="#919195">
            <tbody>
              <tr>
                <td className="pt-2">
                  <span className="rbtnDockedPanel_demografi">
                    &nbsp;&nbsp;
                    <input
                      id="DemografiAnalyse1_DemografiKriterier1_rbuxFPAlder"
                      type="radio"
                      name="DemografiAnalyse1$DemografiKriterier1$aa"
                      value="rbuxFPAlder"
                      onChange={HushClick}
                    />
                    <label className="pl-2 Demografie_text">Husholdning</label>
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
          {HushCheck ? (
            <div className="container demografic-container-kw center pt-2">
              <table border="0" width="100%">
                <tbody>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[24]}
                          onChange={(event) =>
                            handleCheck(event, "enslig_u_b", 24)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPHusholdning_ENSLIG_U_B"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPHusholdning$ENSLIG_U_B"
                          className="mb-2"
                        />
                        <label className="pl-1">Enslig uten barn</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[25]}
                          onChange={(event) =>
                            handleCheck(event, "enslig_m_b", 25)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPHusholdning_ENSLIG_M_B"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPHusholdning$ENSLIG_M_B"
                          className="mb-2"
                        />
                        <label className="pl-1">Enslig med barn</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[26]}
                          onChange={(event) =>
                            handleCheck(event, "par_u_barn", 26)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPHusholdning_PAR_U_BARN"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPHusholdning$PAR_U_BARN"
                          className="mb-2"
                        />
                        <label className="pl-1">Par uten barn</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[27]}
                          onChange={(event) =>
                            handleCheck(event, "par_m_barn", 27)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPHusholdning_PAR_m_BARN"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPHusholdning$PAR_m_BARN"
                          className="mb-2"
                        />
                        <label className="pl-1">Par med barn</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[28]}
                          onChange={(event) => handleCheck(event, "par", 28)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPHusholdning_PAR"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPHusholdning$PAR"
                          className="mb-2"
                        />
                        <label className="pl-1">Par</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[29]}
                          onChange={(event) =>
                            handleCheck(event, "flerfamili", 29)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPHusholdning_FLERFAMILI"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPHusholdning$FLERFAMILI"
                          className="mb-2"
                        />
                        <label className="pl-1">Flerfamilie</label>
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          ) : null}
        </div>

        <div className="card Kj-background-color-kw ml-1 mr-1 mt-2 ">
          <table border="0" width="100%" bgcolor="#919195">
            <tbody>
              <tr>
                <td className="pt-2">
                  <span className="rbtnDockedPanel_demografi">
                    &nbsp;&nbsp;
                    <input
                      id="DemografiAnalyse1_DemografiKriterier1_rbuxFPAlder"
                      type="radio"
                      name="DemografiAnalyse1$DemografiKriterier1$aa"
                      value="rbuxFPAlder"
                      onChange={BoligTClick}
                    />
                    <label className="pl-2 Demografie_text">Boligtype</label>
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
          {BoligTCheck ? (
            <div className="container demografic-container-kw center pt-2">
              <table border="0" width="100%">
                <tbody>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[30]}
                          onChange={(event) =>
                            handleCheck(event, "enebolig", 30)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligtype_ENEBOLIG"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligtype$ENEBOLIG"
                          className="mb-2"
                        />
                        <label className="pl-1">Enebolig</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[35]}
                          onChange={(event) =>
                            handleCheck(event, "annen_bo", 35)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligtype_ANNEN_BO"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligtype$ANNEN_BO"
                          className="mb-2"
                        />
                        <label className="pl-1">Annen bolig</label>
                      </span>
                    </td>

                    {/* Comment below option after discussion with Wenke */}
                    {/* <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[31]}
                          onChange={(event) =>
                            handleCheck(event, "bofelleskap", 31)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligtype_BOFELLESKAP"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligtype$BOFELLESKAP"
                          className="mb-2"
                        />
                        <label className="pl-1">Bofellesskap</label>
                      </span>
                    </td> */}
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[32]}
                          onChange={(event) => handleCheck(event, "blokk", 32)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligtype_BLOKK"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligtype$BLOKK"
                          className="mb-2"
                        />
                        <label className="pl-1">Blokk/Bygård/Terrassehus</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[33]}
                          onChange={(event) =>
                            handleCheck(event, "rekkehus", 33)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligtype_REKKEHUS"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligtype$REKKEHUS"
                          className="mb-2"
                        />
                        <label className="pl-1">Rekkehus</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[34]}
                          onChange={(event) =>
                            handleCheck(event, "tomannsbolig", 34)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligtype_TOMANNSBOLIG"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligtype$TOMANNSBOLIG"
                          className="mb-2"
                        />
                        <label className="pl-1">Tomannsbolig</label>
                      </span>
                    </td>
                    
                  </tr>
                </tbody>
              </table>
            </div>
          ) : null}
        </div>

        <div className="card Kj-background-color-kw ml-1 mr-1 mt-2 ">
          <table border="0" width="100%" bgcolor="#919195">
            <tbody>
              <tr>
                <td className="pt-2">
                  <span className="rbtnDockedPanel_demografi">
                    &nbsp;&nbsp;
                    <input
                      id="DemografiAnalyse1_DemografiKriterier1_rbuxFPAlder"
                      type="radio"
                      name="DemografiAnalyse1$DemografiKriterier1$aa"
                      value="rbuxFPAlder"
                      onChange={BoligsClick}
                    />
                    <label className="pl-2 Demografie_text">
                      Boligstørrelse
                    </label>
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
          {BoligsCheck ? (
            <div className="container demografic-container-kw center pt-2">
              <table border="0" width="100%">
                <tbody>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[36]}
                          onChange={(event) => handleCheck(event, "aru_50", 36)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_ARU_50"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$ARU_50"
                          className="mb-2"
                        />
                        <label className="pl-1">Under 50 m2</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[37]}
                          onChange={(event) =>
                            handleCheck(event, "ar50_59", 37)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR50_59"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR50_59"
                          className="mb-2"
                        />
                        <label className="pl-1">50-59 m2</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[38]}
                          onChange={(event) =>
                            handleCheck(event, "ar60_79", 38)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR60_79"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR60_79"
                          className="mb-2"
                        />
                        <label className="pl-1">60-79 m2</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[39]}
                          onChange={(event) =>
                            handleCheck(event, "ar80_99", 39)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR80_99"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR80_99"
                          className="mb-2"
                        />
                        <label className="pl-1">80-99 m2</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[40]}
                          onChange={(event) =>
                            handleCheck(event, "ar100_119", 40)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR100_119"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR100_119"
                          className="mb-2"
                        />
                        <label className="pl-1">100-119 m2</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[41]}
                          onChange={(event) =>
                            handleCheck(event, "ar120_139", 41)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR120_139"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR120_139"
                          className="mb-2"
                        />
                        <label className="pl-1">120-139 m2</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[42]}
                          onChange={(event) =>
                            handleCheck(event, "ar140_159", 42)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR140_159"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR140_159"
                          className="mb-2"
                        />
                        <label className="pl-1">140-159 m2</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[43]}
                          onChange={(event) =>
                            handleCheck(event, "ar160_199", 43)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR160_199"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR160_199"
                          className="mb-2"
                        />
                        <label className="pl-1">160-199 m2</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[44]}
                          onChange={(event) =>
                            handleCheck(event, "ar200_249", 44)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR200_249"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR200_249"
                          className="mb-2"
                        />
                        <label className="pl-1">200-249 m2</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[45]}
                          onChange={(event) =>
                            handleCheck(event, "ar250_o", 45)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligstorrelse_AR250_O"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligstorrelse$AR250_O"
                          className="mb-2"
                        />
                        <label className="pl-1">Over 250 m2</label>
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          ) : null}
        </div>

        <div className="card Kj-background-color-kw ml-1 mr-1 mt-2 ">
          <table border="0" width="100%" bgcolor="#919195">
            <tbody>
              <tr>
                <td className="pt-2">
                  <span className="rbtnDockedPanel_demografi">
                    &nbsp;&nbsp;
                    <input
                      id="DemografiAnalyse1_DemografiKriterier1_rbuxFPAlder"
                      type="radio"
                      name="DemografiAnalyse1$DemografiKriterier1$aa"
                      value="rbuxFPAlder"
                      onChange={BoligalderClick}
                    />
                    <label className="pl-2 Demografie_text">Boligalder</label>
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
          {BoligalderCheck ? (
            <div className="container demografic-container-kw center pt-2">
              <table border="0" width="100%">
                <tbody>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[46]}
                          onChange={(event) => handleCheck(event, "h1_aar", 46)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H1_AAR"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H1_AAR"
                          className="mb-2"
                        />
                        <label className="pl-1">1 år</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[47]}
                          onChange={(event) => handleCheck(event, "h2_aar", 47)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H2_AAR"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H2_AAR"
                          className="mb-2"
                        />
                        <label className="pl-1">2 år</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[48]}
                          onChange={(event) =>
                            handleCheck(event, "h2_5_aar", 48)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H2_5_AAR"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H2_5_AAR"
                          className="mb-2"
                        />
                        <label className="pl-1">3-5 år</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[49]}
                          onChange={(event) =>
                            handleCheck(event, "h5_10_aar", 49)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H5_10_AAR"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H5_10_AAR"
                          className="mb-2"
                        />
                        <label className="pl-1">6-10 år</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[50]}
                          onChange={(event) =>
                            handleCheck(event, "h10_20_aar", 50)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H10_20_AAR"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H10_20_AAR"
                          className="mb-2"
                        />
                        <label className="pl-1">11-20 år</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[51]}
                          onChange={(event) =>
                            handleCheck(event, "h20_30_aar", 51)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H20_30_AAR"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H20_30_AAR"
                          className="mb-2"
                        />
                        <label className="pl-1">21-30 år</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[52]}
                          onChange={(event) =>
                            handleCheck(event, "h30_40_aar", 52)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H30_40_AAR"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H30_40_AAR"
                          className="mb-2"
                        />
                        <label className="pl-1">31-40 år</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[53]}
                          onChange={(event) =>
                            handleCheck(event, "h40_50_aar", 53)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H40_50_AAR"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H40_50_AAR"
                          className="mb-2"
                        />
                        <label className="pl-1">41-50 år</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[54]}
                          onChange={(event) =>
                            handleCheck(event, "hover_50", 54)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_HOVER_50"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$HOVER_50"
                          className="mb-2"
                        />
                        <label className="pl-1">Over 50 år</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[55]}
                          onChange={(event) =>
                            handleCheck(event, "h_ukjent", 55)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBoligalder_H_UKJENT"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBoligalder$H_UKJENT"
                          className="mb-2"
                        />
                        <label className="pl-1">Ukjent</label>
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          ) : null}
        </div>

        <div className="card Kj-background-color-kw ml-1 mr-1 mt-2 ">
          <table border="0" width="100%" bgcolor="#919195">
            <tbody>
              <tr>
                <td className="pt-2">
                  <span className="rbtnDockedPanel_demografi">
                    &nbsp;&nbsp;
                    <input
                      id="DemografiAnalyse1_DemografiKriterier1_rbuxFPAlder"
                      type="radio"
                      name="DemografiAnalyse1$DemografiKriterier1$aa"
                      value="rbuxFPAlder"
                      onChange={BiltypeClick}
                    />
                    <label className="pl-2 Demografie_text">Biltype</label>
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
          {BiltypeCheck ? (
            <div className="container demografic-container-kw center pt-2">
              <table border="0" width="100%">
                <tbody>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[56]}
                          onChange={(event) => handleCheck(event, "Audi", 56)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_AUDI"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$AUDI"
                          className="mb-2"
                        />
                        <label className="pl-1">Audi</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[57]}
                          onChange={(event) => handleCheck(event, "BMW", 57)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_BMW"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$BMW"
                          className="mb-2"
                        />
                        <label className="pl-1">BMW</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[60]}
                          onChange={(event) =>
                            handleCheck(event, "Citroen", 60)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_CITROEN"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$CITROEN"
                          className="mb-2"
                        />
                        <label className="pl-1">Citroen</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[61]}
                          onChange={(event) => handleCheck(event, "Fiat", 61)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_FIAT"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$FIAT"
                          className="mb-2"
                        />
                        <label className="pl-1">Fiat</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[62]}
                          onChange={(event) => handleCheck(event, "Ford", 62)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_FORD"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$FORD"
                          className="mb-2"
                        />
                        <label className="pl-1">Ford</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[63]}
                          onChange={(event) => handleCheck(event, "Honda", 63)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_HONDA"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$HONDA"
                          className="mb-2"
                        />
                        <label className="pl-1">Honda</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[64]}
                          onChange={(event) =>
                            handleCheck(event, "Hyundai", 64)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_HYUNDAI"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$HYUNDAI"
                          className="mb-2"
                        />
                        <label className="pl-1">Hyundai</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[65]}
                          onChange={(event) => handleCheck(event, "Kia", 65)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_KIA"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$KIA"
                          className="mb-2"
                        />
                        <label className="pl-1">Kia</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[59]}
                          onChange={(event) =>
                            handleCheck(event, "land_rover", 59)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_land_rover"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$land_rover"
                          className="mb-2"
                        />
                        <label className="pl-1">Land Rover</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[66]}
                          onChange={(event) => handleCheck(event, "Mazda", 66)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_MAZDA"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$MAZDA"
                          className="mb-2"
                        />
                        <label className="pl-1">Mazda</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[67]}
                          onChange={(event) =>
                            handleCheck(event, "Mercedes_benz", 67)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_MERCEDES_BENZ"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$MERCEDES_BENZ"
                          className="mb-2"
                        />
                        <label className="pl-1">Mercedes Benz</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[68]}
                          onChange={(event) =>
                            handleCheck(event, "Mitsubishi", 68)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_MITSUBISHI"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$MITSUBISHI"
                          className="mb-2"
                        />
                        <label className="pl-1">Mitsubishi</label>
                      </span>
                    </td>
                    {/* <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[58]}
                          onChange={(event) =>
                            handleCheck(event, "Chevrolet", 58)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_CHEVROLET"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$CHEVROLET"
                          className="mb-2"
                        />
                        <label className="pl-1">Chevrolet</label>
                      </span>
                    </td> */}
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[69]}
                          onChange={(event) => handleCheck(event, "Nissan", 69)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_NISSAN"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$NISSAN"
                          className="mb-2"
                        />
                        <label className="pl-1">Nissan</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[70]}
                          onChange={(event) => handleCheck(event, "Opel", 70)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_OPEL"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$OPEL"
                          className="mb-2"
                        />
                        <label className="pl-1">Opel</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[71]}
                          onChange={(event) =>
                            handleCheck(event, "Peugeot", 71)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_PEUGEOT"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$PEUGEOT"
                          className="mb-2"
                        />
                        <label className="pl-1">Peugeot</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[72]}
                          onChange={(event) =>
                            handleCheck(event, "Renault", 72)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_RENAULT"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$RENAULT"
                          className="mb-2"
                        />
                        <label className="pl-1">Renault</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[74]}
                          onChange={(event) => handleCheck(event, "Skoda", 74)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_SKODA"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$SKODA"
                          className="mb-2"
                        />
                        <label className="pl-1">Skoda</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[75]}
                          onChange={(event) => handleCheck(event, "Subaru", 75)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_SUBARU"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$SUBARU"
                          className="mb-2"
                        />
                        <label className="pl-1">Subaru</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[76]}
                          onChange={(event) => handleCheck(event, "Suzuki", 76)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_SUZUKI"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$SUZUKI"
                          className="mb-2"
                        />
                        <label className="pl-1">Suzuki</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[77]}
                          onChange={(event) => handleCheck(event, "Saab", 77)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_SAAB"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$SAAB"
                          className="mb-2"
                        />
                        <label className="pl-1">Saab</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[73]}
                          onChange={(event) => handleCheck(event, "Tesla", 73)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_Tesla"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$Tesla"
                          className="mb-2"
                        />
                        <label className="pl-1">Tesla</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[78]}
                          onChange={(event) => handleCheck(event, "Toyota", 78)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_TOYOTA"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$TOYOTA"
                          className="mb-2"
                        />
                        <label className="pl-1">Toyota</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[79]}
                          onChange={(event) =>
                            handleCheck(event, "Volkswagen", 79)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_VOLKSWAGEN"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$VOLKSWAGEN"
                          className="mb-2"
                        />
                        <label className="pl-1">Volkswagen</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[80]}
                          onChange={(event) => handleCheck(event, "Volvo", 80)}
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_VOLVO"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$VOLVO"
                          className="mb-2"
                        />
                        <label className="pl-1">Volvo</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[81]}
                          onChange={(event) =>
                            handleCheck(event, "Andre_merk", 81)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBiltype_ANDRE_MERK"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBiltype$ANDRE_MERK"
                          className="mb-2"
                        />
                        <label className="pl-1">Andre</label>
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          ) : null}
        </div>

        <div className="card Kj-background-color-kw ml-1 mr-1 mt-2 ">
          <table border="0" width="100%" bgcolor="#919195">
            <tbody>
              <tr>
                <td className="pt-2">
                  <span className="rbtnDockedPanel_demografi">
                    &nbsp;&nbsp;
                    <input
                      id="DemografiAnalyse1_DemografiKriterier1_rbuxFPAlder"
                      type="radio"
                      name="DemografiAnalyse1$DemografiKriterier1$aa"
                      value="rbuxFPAlder"
                      onChange={BilensClick}
                    />
                    <label className="pl-2 Demografie_text">
                      {" "}
                      Bilens alder
                    </label>
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
          {BilensCheck ? (
            <div className="container demografic-container-kw center pt-2">
              <table border="0" width="100%">
                <tbody>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[82]}
                          onChange={(event) =>
                            handleCheck(event, "bilalder_20_", 82)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_20_"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_20_"
                          className="mb-4"
                        />
                        <label className="pl-1">
                          Biler bygget for 20- år siden eller mer
                        </label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[83]}
                          onChange={(event) =>
                            handleCheck(event, "bilalder_16_19", 83)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_16_19"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_16_19"
                          className="mb-4"
                        />
                        <label className="pl-1">16-19 år siden</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[84]}
                          onChange={(event) =>
                            handleCheck(event, "bilalder_14_15", 84)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_14_15"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_14_15"
                          className="mb-2"
                        />
                        <label className="pl-1">14-15 år siden</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[85]}
                          onChange={(event) =>
                            handleCheck(event, "bilalder_12_13", 85)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_12_13"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_12_13"
                          className="mb-4"
                        />
                        <label className="pl-1">12-13 år siden</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[86]}
                          onChange={(event) =>
                            handleCheck(event, "bilalder_10_11", 86)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_10_11"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_10_11"
                          className="mb-2"
                        />
                        <label className="pl-1">10-11 år siden</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[87]}
                          onChange={(event) =>
                            handleCheck(event, "bilalder_8_9", 87)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_8_9"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_8_9"
                          className="mb-2"
                        />
                        <label className="pl-1">8-9 år siden</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[88]}
                          onChange={(event) =>
                            handleCheck(event, "bilalder_6_7", 88)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_6_7"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_6_7"
                          className="mb-2"
                        />
                        <label className="pl-1">6-7 år siden</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[89]}
                          onChange={(event) =>
                            handleCheck(event, "bilalder_5", 89)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_5"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_5"
                          className="mb-2"
                        />
                        <label className="pl-1">5 år siden</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[90]}
                          onChange={(event) =>
                            handleCheck(event, "bilalder_4", 90)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_4"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_4"
                          className="mb-2"
                        />
                        <label className="pl-1">4 år siden</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[91]}
                          onChange={(event) =>
                            handleCheck(event, "bilalder_3", 91)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_3"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_3"
                          className="mb-2"
                        />
                        <label className="pl-1">3 år siden</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[92]}
                          onChange={(event) =>
                            handleCheck(event, "bilalder_2", 92)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_2"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_2"
                          className="mb-2"
                        />
                        <label className="pl-1">2 år siden</label>
                      </span>
                    </td>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[93]}
                          onChange={(event) =>
                            handleCheck(event, "bilalder_1", 93)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_1"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_1"
                          className="mb-2"
                        />
                        <label className="pl-1">1 år siden</label>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <span className="divCheckText_demografi">
                        <input
                          checked={checkboxvalue[94]}
                          onChange={(event) =>
                            handleCheck(event, "bilalder_0", 94)
                          }
                          id="DemografiAnalyse1_DemografiKriterier1_uxFPBilensAlder_BILALDER_0"
                          type="checkbox"
                          name="DemografiAnalyse1$DemografiKriterier1$uxFPBilensAlder$BILALDER_0"
                          className="mb-2"
                        />
                        <label className="pl-1">under 1 år</label>
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          ) : null}
        </div>
      </div>
      <br />
      <div className="div_left">
        <input
          type="submit"
          name="DemografiAnalyse1$uxFooter$uxBtForrige"
          value="Tilbake"
          onClick={goback}
          className="KSPU_button_Gray"
        />
        <div className="padding_NoColor_T">
          <a className="KSPU_LinkButton_Url_KW pl-2" onClick={GotoMain}>
            Avbryt
          </a>
        </div>
      </div>

      <div className="float-right">
        <div>
          <input
            type="submit"
            name="DemografiAnalyse1$uxFooter$uxBtnNeste"
            value="Velg geografisk område "
            onClick={nomessage}
            className="KSPU_button-kw"
            disabled={selectedCheckboxes?.length ? false : true}
          />
        </div>
      </div>
    </div>
  );
}

export default DemografiVelg;
