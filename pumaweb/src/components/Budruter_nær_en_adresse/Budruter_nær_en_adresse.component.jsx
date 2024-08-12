import { Alert } from "antd";
import React, { useState, useContext, useRef, useEffect } from "react";
import { KundeWebContext, MainPageContext } from "../../context/Context";
// import Budrutekommune from "./BudruteKommune.js";
import * as locator from "@arcgis/core/rest/locator";
import Graphic from "@arcgis/core/Graphic";
import { MapConfig } from "../../config/mapconfig";
import Point from "@arcgis/core/geometry/Point";
import TextSymbol from "@arcgis/core/symbols/TextSymbol";
import "./Budruter_nær_en_adresse.styles.scss";
import * as geoprocessor from "@arcgis/core/rest/geoprocessor";
import { submitJob } from "@arcgis/core/rest/geoprocessor";
import Extent from "@arcgis/core/geometry/Extent";
import * as geometryEngine from "@arcgis/core/geometry/geometryEngine";

function BudruterKW() {
  const { budruterMapView, setbudruterMapview } = useContext(MainPageContext);

  const { householdcheckbox, sethouseholdcheckbox } =
    useContext(KundeWebContext);
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);
  const [kjoretidCheck, setkjoretidCheck] = useState(true);
  const [KjoreavCheck, setkjoreavCheck] = useState(false);
  const [Antallcheck, setAntallcheck] = useState(false);
  const [nomessagediv, setnomessagediv] = useState(false);
  const { Page, setPage } = useContext(KundeWebContext);
  const { mapView } = useContext(MainPageContext);
  const [selection, setselection] = useState("");
  const [Avstand_input, setAvstand_input] = useState("");
  // const [budruteModelName, setBudruteModelName] = useState(" ");
  const [addressEnter, setAddressEnter] = useState("");
  const [gateValue, setGateValue] = useState("");
  const [UpdateValue, setUpdateValue] = useState("");
  const [buttonValue, setButtonValue] = useState("");
  const [kommuneName, setkommuneName] = useState(" ");
  const [displayMsg, setDisplayMsg] = useState(false);
  const [selectedvalue, setselectedvalue] = useState("");
  const [Result, setResult] = useState([]);
  const [scrollbar, setscrollbar] = useState(false);
  const [SelectedItem, setSelectedItem] = useState([]);
  const [Show, setShow] = useState(false);
  const [ValueToBeDeleted, setValueToBeDeleted] = useState("");
  const [ArrayValue, setArrayValue] = useState([]);
  const btnclose = useRef();
  const [EnteredAntallValue, setEnteredAntallValue] = useState(0);
  const { SelectedItemCheckBox_Budruter, setSelectedItemCheckBox_Budruter } =
    useContext(KundeWebContext);
  const { BudruterTimeSelection, setBudruterTimeSelection } =
    useContext(KundeWebContext);
  const { BudruterDistanceSelection, setBudruterDistanceSelection } =
    useContext(KundeWebContext);
  const { BudruterAntallSelection, setBudruterAntallSelection } =
    useContext(KundeWebContext);
  const [melding1, setmelding1] = useState(false);
  const [errormsg1, seterrormsg1] = useState("");
  const [ErrorMsg, setErrorMsg] = useState("");
  const { BudruterSelectedName, setBudruterSelectedName } =
    useContext(KundeWebContext);
  const { addressPoints, setAddressPoints } = useContext(MainPageContext);
  const { criteriaObject, setCriteriaObject } = useContext(KundeWebContext);
  const [checkedList, setCheckedList] = useState(addressPoints);
  const [selectedName, setSelectedName] = useState("");
  const [placeChangeModalValue, setPlaceChangeModalValue] = useState("");

  useEffect(async () => {
    sethouseholdcheckbox(true);
    // setSelectedItemCheckBox_Budruter(addressPoints);
    // setSelectedItemCheckBox_Budruter(
    //   SelectedItemCheckBox_Budruter.concat(addressPoints)
    // );
    // setSelectedItem(SelectedItem.concat(addressPoints));
    // SelectedVelg(SelectedItem.concat(addressPoints))

    // mapView.graphics.removeAll();
  }, []);
  useEffect(() => {
    setCheckedList(addressPoints);
    setSelectedItemCheckBox_Budruter(addressPoints);
  }, [addressPoints]);

  const handleCheckboxChange = (event, item) => {
    if (event.target.checked) {
      checkedList.push(item);
      setSelectedItemCheckBox_Budruter(checkedList);
    } else {
      var filteredArr = checkedList.filter((val) => {
        return val.key !== item.key;
      });
      setCheckedList(filteredArr);
      setSelectedItemCheckBox_Budruter(filteredArr);
    }
  };

  const Avstand = () => {
    let Avstand_value = document.getElementById("Avstand_value").value;
    setselection("");
    setBudruterTimeSelection("");
    setBudruterAntallSelection("");
    setEnteredAntallValue("");
    setAvstand_input(Avstand_value);
    setBudruterDistanceSelection(Avstand_value);
  };

  const EnterAntallValuefun = (e) => {
    setAvstand_input("");
    setselection("");
    setBudruterTimeSelection("");
    setBudruterDistanceSelection("");
    setBudruterAntallSelection(e.target.value);
    setEnteredAntallValue(e.target.value);
  };
  const kjoretidClick = () => {
    setkjoretidCheck(true);
    setkjoreavCheck(false);
    setAntallcheck(false);
  };
  const KjoreavClick = () => {
    setkjoretidCheck(false);
    setkjoreavCheck(true);
    setAntallcheck(false);
  };
  const Enterselection = (e) => {
    // let selectionText = document.getElementById("selection").value;
    setAvstand_input("");
    setBudruterDistanceSelection("");
    setBudruterAntallSelection("");
    setEnteredAntallValue("");
    setselection(e.target.value);
    setBudruterTimeSelection(e.target.value);
  };
  const AntallClick = () => {
    setkjoretidCheck(false);
    setkjoreavCheck(false);
    setAntallcheck(true);
  };
  const goback = () => {
    setPage("");
    mapView.extent = new Extent(MapConfig.kundewebMapExtent);
    mapView.goTo(mapView.extent);
  };
  const GotoMain = () => {
    setPage("");
  };
  const LagUtvalgClick = async () => {
    let j = mapView.graphics.items.length;
      for (var i = j; i > 0; i--) {
        if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
          mapView.graphics.remove(mapView.graphics.items[i - 1]);
          //j++;
        }
      }
    if(addressPoints && SelectedItemCheckBox_Budruter.length == 0){
      setSelectedItemCheckBox_Budruter(addressPoints);
    }
    if (
      (selection == "" && Avstand_input == "" && EnteredAntallValue == "") ||
      addressPoints.length == 0
    ) {
      setmelding1(true);
      seterrormsg1("Velg en av alternativknappene og oppgi verdien.");
    } else if (Number(selection) > 120) {
      setmelding1(true);
      seterrormsg1(
        "Øverste grense 120 for antall minutter i kjøreanalysen er overskredet"
      );
    } else if (Number(Avstand_input) > 500) {
      setmelding1(true);
      seterrormsg1(
        "Øverste grense 500 for antall kilometer i kjøreanalysen er overskredet."
      );
    } else if (Number(EnteredAntallValue) > 999999) {
      setmelding1(true);
      seterrormsg1(
        "Øverste grense 999999 for antall kilometer i kjøreanalysen er overskredet."
      );
    } else {
      let str = "";
      if (kjoretidCheck) {
        str = str + "Antall minutter: " + BudruterTimeSelection + " minutter";
      } else if (KjoreavCheck) {
        str = str + "Avstand km: " + BudruterDistanceSelection + " km";
      } else {
        str = str + BudruterAntallSelection + " ant" + " og 60 km";
      }
      setCriteriaObject({
        ...criteriaObject,
        BudruterFeature: str,
      });
      setPage("Burdruter_velg_KW");
    }
  };
  const zoomTil = (graphics) => {
    var geo = mapView.center;
    geo.x = graphics.x;
    geo.y = graphics.y;
    mapView.goTo({
      geometry: geo,
      zoom: 11,
    });
  };

  const handleKeypress = (e) => {
    //it triggers by pressing the enter key
    if (e.key === "Enter") {
      budruterModel();
    }
  };
  const budruterModel = async () => {
    setDisplayMsg(false);
    if (addressEnter === "") {
      setDisplayMsg(true);
      setErrorMsg("Fant ingen matchende adresser.");
    } else {
      const findPlaces = () => {
        locator
          .addressToLocations(MapConfig.geoSokUrl, {
            address: {
              OBJECTID: 0,
              SingleLine: addressEnter,
            },
            countryCode: "",
            categories: "",
            maxLocations: 25,
            outFields: "*",
          })

          .then(function (results) {
            if (results.length == 0) {
              setDisplayMsg(true);
              setErrorMsg(
                "Feil ved adressesøk: Sjekk at du har skrevet inn en gyldig adresse. Hvis det fortsatt feiler, kontakt kundeservice  på telefon 04045. "
              );
              window.$("#budruteresult").modal("show");
            } else {
              window.$("#budruteresult").modal("show");

              setResult(results);
            }
          });
      };
      await findPlaces();
      // setGateValue(gateValue);
      // setBudruteModelName("BudruteForDeling");
    }
  };
  const dropdownselection = (e) => {
    setselectedvalue(e.target.value);
  };

  const HushholdRadioCheckfun = (e) => {
    if (e.target.checked) {
      sethouseholdcheckbox(true);
    } else {
      sethouseholdcheckbox(false);
    }
  };

  const BusinessRadioCheckfun = (e) => {
    if (e.target.checked) {
      setbusinesscheckbox(true);
    } else {
      setbusinesscheckbox(false);
    }
  };

  // const Velg = (item) => {
  //   let TempValue = SelectedItemCheckBox_Budruter;
  //   TempValue.push(item);
  //   setSelectedItemCheckBox_Budruter(TempValue);
  //   setBudruterSelectedName(
  //     item.attributes
  //       ? item.attributes.Match_addr
  //       : item.feature.attributes.Match_addr
  //   );
  //   let Temp = [];
  //   Temp.push(item);

  //   let Locations = [];

  //   Array.prototype.push.apply(ArrayValue, Temp);
  //   setArrayValue(Temp);
  //   ArrayValue.map((item) => {
  //     Locations.push(item.location);
  //   });

  //   // ArrayValue.forEach(function (result) {
  //   mapView.graphics.add(
  //     new Graphic({
  //       attributes: item.attributes ? item.attributes : item.feature.attributes, // Data attributes returned
  //       geometry: item.location, // Point returned
  //       symbol: {
  //         type: "simple-marker",
  //         color: "blue",
  //         size: "12px",
  //         text: item.attributes.Match_addr,
  //         outline: {
  //           color: "red",
  //           width: "2px",
  //         },
  //       },

  //       popupTemplate: {
  //         title: "{PlaceName}", // Data attribute names
  //         content: "{Place_addr}",
  //       },
  //     })
  //   );
  //   let labelAddressPoint = new Graphic({
  //     geometry: item.location,
  //     symbol: {
  //       type: "text",
  //       color: "white",
  //       haloColor: "black",
  //       haloSize: "1px",
  //       text: item.attributes
  //         ? item.attributes.address
  //         : item.feature.attributes.address,
  //       xoffset: 3,
  //       yoffset: 3,
  //       font: {
  //         size: 12,
  //         family: "sans-serif",
  //         weight: "bolder",
  //       },
  //     },
  //   });

  //   // mapView.graphics.add(labelAddressPoint);
  //   // let labelAddressPoint = new Graphic({
  //   //   geometry: item.feature ? item.feature.geometry : "",
  //   //   symbol: {
  //   //     type: "text",
  //   //     color: "white",
  //   //     haloColor: "black",
  //   //     haloSize: "1px",
  //   //     text: item.feature ? item.feature.address : item.address,
  //   //     xoffset: 3,
  //   //     yoffset: 3,
  //   //     font: {
  //   //       size: 12,
  //   //       family: "sans-serif",
  //   //       weight: "bolder",
  //   //     },
  //   //   },
  //   // });

  //   // mapView.graphics.add(labelAddressPoint);

  //   let textSymbol = new TextSymbol({
  //     color: "white",
  //     haloColor: "black",
  //     haloSize: "1px",
  //     xoffset: 3,
  //     yoffset: 3,
  //     font: {
  //       // autocast as esri/symbols/Font
  //       size: 12,
  //       //family: "sans-serif",
  //       family: "Arial",
  //       weight: "bolder",
  //     },
  //     text: item.attributes.Match_addr,
  //   });

  //   // // add label to point graphics
  //   const labelPoint = new Graphic({
  //     geometry: item.location,
  //     symbol: textSymbol,
  //   });
  //   mapView.graphics.add(labelPoint);
  //   //zoomTil(item.location);

  //   //});
  //   //   view.popup.close();
  //   //   view.graphics.removeAll();
  //   //mapView.goTo(item.location);

  //   let SelectedArray = [];
  //   SelectedArray.push(item);
  //   Array.prototype.push.apply(SelectedItem, SelectedArray);
  //   SelectedItem.concat(budruterMapView);
  //   setSelectedItem(SelectedItem);
  //   setShow(true);
  // };

  const Velg = (item) => {
    let val = [];
    val.push(item);
    setAddressPoints(addressPoints.concat(val));
    setCheckedList(checkedList.concat(item));
    updateMap(val);
  };

  const Fjern = (item) => {
    var arr = addressPoints.filter((data) => data !== item);
    setAddressPoints(arr);
    deleteFromMap(item?.location ? item.location : item?.geometry);
  };

  const deleteFromMap = (geometry) => {
    const mapArr = [];
    mapView.graphics.map((ite) => {
      const isEqual = geometryEngine.equals(geometry, ite.geometry);
      if (isEqual === true) {
        mapArr.push(ite);
      }
    });
    mapArr.map((it) => {
      mapView.graphics.remove(it);
    });
  };

  const updateMap = (address) => {
    address.map((item) => {
      mapView.graphics.add(
        new Graphic({
          attributes: item.attributes, // Data attributes returned
          geometry: item?.location ? item.location : item?.geometry, // Point returned
          symbol: {
            type: "simple-marker",
            color: "blue",
            size: "12px",
            outline: {
              color: "blue",
              width: "2px",
            },
          },
          popupTemplate: {
            title: "{PlaceName}", // Data attribute names
            content: "{Place_addr}",
          },
        })
      );
      let displayName = item.address;
      if (item?.attributes?.display) {
        displayName = item.attributes.display;
      }
      let labelAddressPoint = new Graphic({
        geometry: item?.location ? item.location : item?.geometry,
        symbol: {
          type: "text",
          color: "white",
          haloColor: "black",
          haloSize: "1px",
          text: displayName,
          xoffset: 5,
          yoffset: 5,
          font: {
            size: 10,
            family: "sans-serif",
            weight: "bolder",
          },
        },
      });

      mapView.graphics.add(labelAddressPoint);
      zoomTil(item?.location ? item.location : item?.geometry);
    });
  };

  // const DeleteKommune = (param) => {
  //   let Temp = SelectedItem;
  //   let ItemToRemove = "";
  //   if (param.attributes) {
  //     ItemToRemove = param.attributes.Postnummer;
  //   } else {
  //     ItemToRemove = param.feature.attributes.Postnummer;
  //   }
  //   const FilteredArray = Temp.filter((item) => {
  //     if (item.attributes) return item.attributes.Postnummer !== ItemToRemove;
  //     else return item.feature.attributes.Postnummer !== ItemToRemove;
  //   });
  //   if (FilteredArray.length == 0) {
  //     // let j = mapView.graphics.items.length;
  //     //   var k = 0;
  //     //   k = j;
  //     //   for (var i = j; i > 0; i--) {
  //     //     if (mapView.graphics.items[i-1].geometry.type === "polygon") {
  //     //       mapView.graphics.remove(mapView.graphics.items[i-1]);
  //     //       //j++;
  //     //     }
  //     //   }
  //     mapView.graphics.removeAll();
  //     setSelectedItem([]);
  //   } else {
  //     // let j = mapView.graphics.items.length;
  //     //   var k = 0;
  //     //   k = j;
  //     //   for (var i = j; i > 0; i--) {
  //     //     if (mapView.graphics.items[i-1].geometry.type === "polygon") {
  //     //       mapView.graphics.remove(mapView.graphics.items[i-1]);
  //     //       //j++;
  //     //     }
  //     //   }
  //     mapView.graphics.removeAll();
  //     FilteredArray.forEach(function (result) {
  //       mapView.graphics.add(
  //         new Graphic({
  //           attributes: result.attributes, // Data attributes returned
  //           geometry: result.location, // Point returned
  //           symbol: {
  //             type: "simple-marker",
  //             color: "blue",
  //             size: "12px",
  //             text: result.attributes
  //               ? result.attributes.Match_addr
  //               : result.feature.attributes.Match_addr,
  //             outline: {
  //               color: "red",
  //               width: "2px",
  //             },
  //           },

  //           popupTemplate: {
  //             title: "{PlaceName}", // Data attribute names
  //             content: "{Place_addr}",
  //           },
  //         })
  //       );
  //     });
  //   }

  //   setSelectedItem(FilteredArray);
  // };
  // const InputClick = (e) => {
  //   // let objIndex = SelectedItem.findIndex(
  //   //   (obj) => obj.Stedsnavn == ValueToBeDeleted
  //   // );
  //   setUpdateValue(e.target.value);
  //   // SelectedItem[objIndex].name = UpdateValue;
  // };
  const okButton = async () => {
    // let objIndex = SelectedItem.findIndex((obj) =>
    //   obj.attributes
    //     ? obj.attributes.Match_addr == ValueToBeDeleted
    //     : obj.feature.attributes.Match_addr == ValueToBeDeleted
    // );
    // //   setUpdateValue(e.target.value);
    // if (SelectedItem[objIndex].attributes)
    //   SelectedItem[objIndex].attributes.Match_addr = UpdateValue;
    // else SelectedItem[objIndex].feature.attributes.Match_addr = UpdateValue;

    // // setSelectedItem(
    // //   ...SelectedItem,
    // // );
    // setUpdateValue(ValueToBeDeleted);

    // btnclose.current.click();

    addressPoints.map((item) => {
      if (item.key == selectedName) {
        item.attributes.Match_addr = placeChangeModalValue;
        item.address = placeChangeModalValue;
        deleteFromMap(item?.location ? item.location : item?.geometry);
        updateMap([item]);
      }
    });
    setAddressPoints([...addressPoints]);
  };

  const AddressEnter = () => {
    let addressText = document.getElementById("addressEnter").value;
    setAddressEnter(addressText);
  };
  // const kommuneChange = (item) => {
  //   let item1 = "";
  //   if (item.attributes) {
  //     item1 = item.attributes.Match_addr;
  //   } else {
  //     item1 = item.feature.attributes.Match_addr;
  //   }
  //   setValueToBeDeleted(item1);
  // };
  // const ZoomTillClick = (item) => {
  //   // let TempLon = item.location.x;
  //   // let TempLon = item.location.y;
  //   let pt = new Point({
  //     latitude: item.location.y,
  //     longitude: item.location.x,
  //     spatialReference: mapView.spatialReference,
  //   });
  //   const TempLocation = item.location;

  //   mapView.goTo({
  //     center: [item.location.x, item.location.y],
  //     zoom: 5,
  //   });
  // };
  // const SelectedCheckBoxfun = (item, e) => {
  //   let TempArray = SelectedItemCheckBox_Budruter;
  //   if (e.target.checked) {
  //     TempArray.push(item);
  //   }
  //   if (!e.target.checked) {
  //     // ResultOutData_Array = ResultOutData_Array.filter(function (obj) {
  //     //   return obj.key !== record.key;
  //     // });
  //     TempArray.splice(TempArray.indexOf(item), 1);
  //   }

  //   setSelectedItemCheckBox_Budruter(TempArray);
  // };

  // const SelectedVelg = () => {

  //    addressPoints.map((item, i) =>

  //   )

  //     //   </div>
  //   );
  // };
  const renderPerson = (result, index) => {
    return Result.map((item, i) => (
      <tr key={i}>
        <th className="tabledataRow">
          {" "}
          <tr>
            <td className="flykecontent">
              {/* <tr>
                {item.attributes.Gatenavn !== "" ? (
                  c
                ) : (
                  <td className="flykecontent">{item.attributes.Stedsnavn}</td>
                )}
              </tr> */}
              <td className="flykecontent">{item.attributes.Match_addr}</td>
            </td>
          </tr>
        </th>

        <th className="tabledataRow">
          {" "}
          <tr>
            <td className="flykecontent">
              <tr>
                <td className="flykecontent">{item.attributes.Husnummer}</td>
              </tr>
            </td>
          </tr>
        </th>

        <th className="tabledataRow">
          {" "}
          <tr>
            <td className="flykecontent">
              <tr>
                <td className="flykecontent">{item.attributes.Score}</td>
              </tr>
            </td>
          </tr>
        </th>
        <th className="tabledataRow">
          {" "}
          <tr>
            <td className="flykecontent">
              <tr>
                <td className="flykecontent">{"Stedsnavn"}</td>
              </tr>
            </td>
          </tr>
        </th>

        <th className="tabledataRow">
          {" "}
          <tr>
            <td className="flykecontent">
              <p
                data-dismiss="modal"
                className="KSPU_LinkButton float-right mr-1"
                onClick={() => Velg(item)}
              >
                velg
              </p>
            </td>
          </tr>
        </th>
      </tr>
    ));
  };

  return (
    <div className="col-5 p-2">
      <div>
        {/* <!-- Modal --> */}
        <div
          className="modal fade bd-example-modal-lg "
          id="budruteresult"
          data-backdrop="static"
          tabIndex="-1"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
          aria-hidden="true"
        >
          <div
            className="modal-dialog modal-dialog budrutemax  "
            role="document"
          >
            <div className="modal-content ">
              <div className="modal-header budrutetitle">
                <h5 className="modal-titlekw " id="exampleModalLongTitle">
                  SØKERESULTAT{" "}
                </h5>
                <button
                  type="button"
                  className="close"
                  data-dismiss="modal"
                  aria-label="Close"
                >
                  <span aria-hidden="true">&times;</span>
                </button>
              </div>
              <div className="modal-body budrutebody">
                {displayMsg ? (
                  <div className="budruteMax">
                    <span
                      id="uxKjoreAnalyse_uxLblMessage"
                      className="divErrorText_kw"
                    >
                      {ErrorMsg}
                      {/* Feil ved adressesøk: En feil oppsto ved adressesøk.
                      Kontakt superbruker i Posten. */}
                    </span>
                    <p></p>
                    <br />
                    <br />
                    <br />
                    <br />
                  </div>
                ) : (
                  <div>
                    {/* <div className={scrollbar ? "scrollbardiv" : ""}> */}
                    <div className="scrollit">
                      <table className="tableRow">
                        <thead>
                          <tr className="flykeHeader">
                            <th className="tabledataRow budruteRow">
                              Gate/sted
                            </th>
                            <th className="tabledataRow budruteRow">
                              Husnummer
                            </th>
                            <th className="tabledataRow budruteRow">Score</th>
                            <th className="tabledataRow budruteRow">Type</th>
                            <th className="tabledataRow budruteRow">
                              &nbsp;&nbsp;&nbsp;&nbsp;
                            </th>
                          </tr>
                        </thead>
                        <tbody>{renderPerson()}</tbody>
                      </table>
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
      <div>
        {/* <!-- Modal --> */}
        <div
          className="modal fade bd-example-modal-lg"
          id="kommunefordeling"
          tabIndex="-1"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
          aria-hidden="true"
        >
          <div
            className="modal-dialog modal-dialog-centered kommuneModel "
            role="document"
          >
            <div className="modal-content">
              <div className="modal-body">
                <label>Legg inn et navn for merking av adressepunktet:</label>
                <input
                  type="text"
                  className="form-control kommuneModel"
                  id="text"
                  aria-describedby="emailHelp"
                  placeholder=""
                  onChange={(e) => setPlaceChangeModalValue(e.target.value)}
                />{" "}
              </div>
              <div className="modal-footer">
                <button
                  type="button"
                  className="btn btn-primary"
                  onClick={okButton}
                  data-dismiss="modal"
                >
                  Ok
                </button>
                <button
                  type="button"
                  className="btn btn-secondary"
                  data-dismiss="modal"
                  ref={btnclose}
                >
                  Cancel
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
      {/* {budruteModelName == "BudruteForDeling" ? (
        <BudruteModelkw
          title={"SØKERESULTAT"}
          id={"budruteresult"}
          dataResult={resultData}
          childrenData={resultData[0].children}
          parentCallback={callback}
        />
      ) : null} */}
      {/* {kommuneName == "KommuneForDeling" ? (
        <Budrutekommune
          title={"SEGMENTFORDELING"}
          id={"kommunefordeling"}
          dataResult={resultData}
          childrenData={resultData[0].children}
          parentCallback={callback}
        />
      ) : null} */}
      <div className="">
        <span className=" title ">Velg budruter nær adresse</span>
      </div>
      <br />
      <div className="p-text">
        Vi kan hjelpe deg å finne kunder i et spesifikt område.<p></p>
        Angi adressen du ønsker å ta utgangspunkt i og velg de kriteriene som
        passer dine ønsker.
        <p></p>
        Vi plukker de rutene som passer best til de kriteriene du har valgt.
      </div>
      <br />
      {melding1 ? (
        <div className="pr-3">
          <div className="error WarningSign">
            <div className="divErrorHeading">Melding:</div>
            <span id="uxKjoreAnalyse_uxLblMessage" className="divErrorText_kw">
              {errormsg1}
            </span>
          </div>
        </div>
      ) : null}

      {nomessagediv ? (
        <div className="pr-3">
          <div className="error WarningSign">
            <div className="divErrorHeading">Melding:</div>
            <span id="uxKjoreAnalyse_uxLblMessage" className="divErrorText_kw">
              Ingen adresser tilgjengelig
            </span>
          </div>
        </div>
      ) : null}
      <p></p>

      <table className="divPanelBody_kw" cellPadding="2" cellSpacing="2">
        <tbody>
          <tr>
            <td colSpan="2" style={{ top: "auto" }}></td>
          </tr>
          <tr>
            <td>
              <span
                id="uxKjoreAnalyse_lblWizSteepDescription"
                className="divHeaderText_kw"
              ></span>

              <div className="wizFilled">
                <div className="padding_NoColor_L_R_T">
                  <table border="0" cellPadding="0" cellSpacing="0">
                    <tbody>
                      <tr>
                        <td colSpan="2" className="bold">
                          Angi adresse
                        </td>
                      </tr>

                      <tr>
                        <td>
                          <input
                            name="uxKjoreAnalyse$AddressFinder1$uxAddress"
                            type="text"
                            placeholder="- Skriv inn adresse her -"
                            value={addressEnter}
                            onChange={AddressEnter}
                            id="addressEnter"
                            title="Søk etter adresse"
                            className="divValueText_adr"
                            // onfocus="this.value = '';"
                            // onkeydown="if (event.keyCode == 13) { document.forms[0].uxKjoreAnalyse_AddressFinder1_uxFindAddress_uxButton.click(); return false;}"
                            // onkeyup=";return gdEnableOrDisableButton('uxKjoreAnalyse_AddressFinder1_uxFindAddress_uxButton',null,'uxKjoreAnalyse_AddressFinder1_uxFindAddress', 1);"
                            style={{ width: "200px" }}
                            onKeyPress={handleKeypress}
                          />
                        </td>

                        <td style={{ paddingLeft: "10px" }}>
                          <table border="0" cellPadding="0" cellSpacing="0">
                            <tbody>
                              <tr>
                                <td style={{ Align: "middle" }}>
                                  <button
                                    id="uxShowUtvalgDetails_uxUtvalgReoler_uxPnlTree_uxShowDetails"
                                    className="budruteButton float-right mr-1"
                                    // data-toggle="modal"
                                    // data-target="#budruteresult"
                                    onClick={budruterModel}
                                  >
                                    Finn i kartet
                                  </button>
                                </td>
                                <td
                                  style={{
                                    paddingLeft: "8px",
                                    vertical: "middle",
                                  }}
                                >
                                  <table
                                    border="0"
                                    cellPadding="0"
                                    cellSpacing="0"
                                  >
                                    <tbody>
                                      <tr>
                                        <td
                                          style={{
                                            width: "16px",
                                            height: "16px",
                                          }}
                                        >
                                          <img
                                            id="uxKjoreAnalyse_AddressFinder1_uxFindAddress_uxIndicator_uxIndicator"
                                            src="images/callbackActivityIndicator.gif"
                                            style={{
                                              borderWidth: "0px",
                                              visibility: "hidden",
                                            }}
                                          />
                                        </td>
                                      </tr>
                                    </tbody>
                                  </table>
                                </td>
                              </tr>
                            </tbody>
                          </table>
                        </td>
                      </tr>
                    </tbody>
                  </table>

                  <table
                    className="budruteTable"
                    cellSpacing="3"
                    cellPadding="3"
                    rules="all"
                    border="1"
                  >
                    <thead></thead>
                    {/* <tbody> */}
                    <tbody>
                      {addressPoints.map((item, key) => {
                        let obj;
                        obj = item;
                        obj["key"] = key;
                        return (
                          <tr className="GridView_Row">
                            <td>
                              <input
                                type="checkbox"
                                // defaultChecked={
                                //   SelectedItemCheckBox_Budruter.filter((i) => i.key === item.key)
                                //     .length
                                //     ? true
                                //     : false
                                // }
                                // onChange={(e) => SelectedCheckBoxfun(item, e)}

                                defaultChecked={
                                  checkedList.filter((i) => i.key === item.key)
                                    .length
                                    ? true
                                    : false
                                }
                                onChange={(e) => {
                                  handleCheckboxChange(e, item);
                                }}
                              />
                            </td>
                            <td className="flykecontent">
                              <tr>
                                {item.attributes ? (
                                  <td className="flykecontent">
                                    {item.attributes.Match_addr}
                                  </td>
                                ) : (
                                  <td className="flykecontent">
                                    {item.feature.attributes.Match_addr}
                                  </td>
                                )}
                              </tr>
                            </td>
                            <td></td>
                            <td className="flykecontent">
                              <a
                                className="KSPU_LinkButton float-right mr-1"
                                onClick={() => {
                                  zoomTil(
                                    item?.location
                                      ? item.location
                                      : item?.geometry
                                  );
                                }}
                              >
                                {" "}
                                Zoom til
                              </a>
                              {/* <a> {buttonValue}</a> */}
                            </td>

                            <td className="flykecontent">
                              <a
                                id="uxShowUtvalgDetails_uxUtvalgReoler_uxPnlTree_uxShowDetails"
                                className="KSPU_LinkButton float-right mr-1"
                                data-toggle="modal"
                                data-target="#kommunefordeling"
                                onClick={() => {
                                  setSelectedName(item.key);
                                }}
                              >
                                Endre navn
                              </a>
                            </td>

                            <td className="flykecontent">
                              <a
                                className="KSPU_LinkButton float-right mr-1"
                                onClick={() => {
                                  Fjern(item);
                                }}
                              >
                                Fjern
                              </a>
                            </td>
                          </tr>
                        );
                      })}
                    </tbody>{" "}
                    {/* </tbody> */}
                  </table>

                  <div className="padding_NoColor_T_B">
                    <div id="uxKjoreAnalyse_uxAddressGrid_uxContents">
                      {/* <!--<span id="uxKjoreAnalyse_uxAddressGrid_uxNoPoints" className="divValueText">Legg til adresser ved å søke etter dem, ved å markere punkter i kartet, eller ved å laste opp en adressefil.</span>--> */}
                      {SelectedItem.length > 0 ? (
                        <div
                          style={{ width: "100%", border: "solid black 1px" }}
                        >
                          <div></div>
                        </div>
                      ) : null}

                      <input
                        type="hidden"
                        name="uxKjoreAnalyse$uxAddressGrid$uxNewNameText"
                        id="uxKjoreAnalyse_uxAddressGrid_uxNewNameText"
                      />
                      <input
                        type="hidden"
                        name="uxKjoreAnalyse$uxAddressGrid$uxNewNameIndex"
                        id="uxKjoreAnalyse_uxAddressGrid_uxNewNameIndex"
                      />
                    </div>
                  </div>
                  <div style={{ visibility: "collapse" }}></div>
                </div>

                <div className="divLabelText padding_NoColor_L_R_T">
                  Velg budruter etter...
                </div>

                <div className="padding_NoColor_L_R_T">
                  <div className="divDockedBorder">
                    <div className="divDockedHeader">
                      <span name="rbAdresseKjoretid">
                        <input
                          id="uxKjoreAnalyse_rbAdresseKjoretid"
                          type="radio"
                          onChange={kjoretidClick}
                          name="uxKjoreAnalyse$xxx"
                          value="rbAdresseKjoretid"
                          checked={kjoretidCheck}
                        />
                      </span>
                      &nbsp;&nbsp; Kjøretid fra adresse
                    </div>
                    {kjoretidCheck ? (
                      <div
                        id="divAdresseKjoretid"
                        className="padding_NoColor_L_R_T_B"
                      >
                        <table>
                          <tbody>
                            <tr>
                              <td>
                                <span id="uxKjoreAnalyse_lblAntallMinutter">
                                  Antall minutter:{" "}
                                </span>
                              </td>
                              <td>
                                <input
                                  name="uxKjoreAnalyse$txtbAdresseKjoretid"
                                  type="text"
                                  id="selection"
                                  onChange={(e) => Enterselection(e)}
                                  value={selection}
                                />
                              </td>
                            </tr>
                            <tr>
                              <td></td>
                              <td align="left">
                                <span id="uxKjoreAnalyse_lblInfoAntallMinutter">
                                  Maks 120 minutter
                                </span>
                              </td>
                            </tr>
                          </tbody>
                        </table>
                      </div>
                    ) : null}
                  </div>
                </div>

                <div className="padding_NoColor_L_R_T">
                  <div className="divDockedBorder">
                    <div className="divDockedHeader">
                      <span name="rbAdresseKjoreAvstand">
                        <input
                          id="uxKjoreAnalyse_rbAdresseKjoreAvstand"
                          type="radio"
                          onClick={KjoreavClick}
                          name="uxKjoreAnalyse$xxx"
                          value="rbAdresseKjoreAvstand"
                        />
                      </span>
                      &nbsp;&nbsp; Kjøreavstand fra adresse
                    </div>
                    {KjoreavCheck ? (
                      <div
                        id="divAdresseKjoreAvstand"
                        className="padding_NoColor_L_R_T_B"
                      >
                        <table>
                          <tbody>
                            <tr>
                              <td>
                                <span id="uxKjoreAnalyse_lblAvstandMeter">
                                  Avstand km:{" "}
                                </span>
                              </td>
                              <td>
                                <input
                                  name="uxKjoreAnalyse$txtbAdresseKjoreAvstand"
                                  type="text"
                                  value={Avstand_input}
                                  id="Avstand_value"
                                  onChange={Avstand}
                                />
                              </td>
                            </tr>
                            <tr>
                              <td></td>
                              <td align="left">
                                <span id="uxKjoreAnalyse_lblInfoAvstandMeter">
                                  Maks 500 km
                                </span>
                              </td>
                            </tr>
                          </tbody>
                        </table>
                      </div>
                    ) : null}
                  </div>
                </div>

                <div className="padding_NoColor_L_R_T_B">
                  <div className="divDockedBorder">
                    <div className="divDockedHeader">
                      <span name="rbAdresseAntallMottakere">
                        <input
                          id="uxKjoreAnalyse_rbAdresseAntallMottakere"
                          type="radio"
                          onClick={AntallClick}
                          name="uxKjoreAnalyse$xxx"
                          value="rbAdresseAntallMottakere"
                        />
                      </span>
                      &nbsp;&nbsp; Antall mottakere fra adresse
                    </div>
                    {Antallcheck ? (
                      <div
                        id="divAdresseAntallMottakere"
                        className="padding_NoColor_L_R_T_B"
                      >
                        <table>
                          <tbody>
                            <tr>
                              <td>
                                <span id="uxKjoreAnalyse_lblAntallMottakere">
                                  Antall:{" "}
                                </span>
                              </td>
                              <td>
                                <input
                                  name="uxKjoreAnalyse$txtbAdresseAntallMottakere"
                                  type="text"
                                  value={EnteredAntallValue}
                                  id="uxKjoreAnalyse_txtbAdresseAntallMottakere"
                                  onChange={EnterAntallValuefun}
                                />
                              </td>
                            </tr>
                            <tr>
                              <td></td>
                              <td align="left">
                                <span id="uxKjoreAnalyse_lblInfoAntallMottakere">
                                  Maks 999999
                                </span>
                              </td>
                            </tr>
                          </tbody>
                        </table>
                      </div>
                    ) : null}
                  </div>
                </div>
                <div className="padding_NoColor_L_R_T_B">
                  <div className="divDockedBorder">
                    <table width="75%">
                      <tbody>
                        <tr>
                          <td colSpan="3" className="divDetailsName">
                            <p className="motta-heading p-2">MOTTAKERGRUPPE</p>
                          </td>
                        </tr>
                        <tr>
                          <td colSpan="2">
                            <div style={{ paddingLeft: "18px", float: "left" }}>
                              <span className="KSPU_CheckBox ">
                                <input
                                  id="uxKjoreAnalyse_uxHouseholds_uxCheckbox"
                                  type="checkbox"
                                  name="uxKjoreAnalyse$uxHouseholds$uxCheckbox"
                                  checked={householdcheckbox}
                                  onChange={(e) => HushholdRadioCheckfun(e)}
                                />
                                <label htmlFor="uxKjoreAnalyse_uxHouseholds_uxCheckbox">
                                  &nbsp;&nbsp;Husholdninger
                                </label>
                              </span>
                              &nbsp;&nbsp;&nbsp;&nbsp;
                              <span className="KSPU_CheckBox">
                                <input
                                  id="uxKjoreAnalyse_uxBusinesses_uxCheckbox"
                                  type="checkbox"
                                  name="uxKjoreAnalyse$uxBusinesses$uxCheckbox"
                                  checked={businesscheckbox}
                                  onClick={(e) => BusinessRadioCheckfun(e)}
                                />
                                <label htmlFor="uxKjoreAnalyse_uxBusinesses_uxCheckbox">
                                  &nbsp;&nbsp;&nbsp;Virksomheter
                                </label>
                              </span>
                            </div>
                          </td>
                          <td></td>
                        </tr>
                        <tr>
                          <td style={{ paddingLeft: "21px" }}></td>
                          <td style={{ paddingLeft: "8px" }}>
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                          </td>
                          <td style={{ paddingLeft: "8px" }}></td>
                        </tr>
                      </tbody>
                    </table>
                  </div>
                </div>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
      {/* <div className=" pr-5 pt-4">
    <input type="button"  value="Lag utvalg"   className="KSPU_button-kw float-right  p-text"/>
        </div>
        </div> */}
      <div className="pt-3">
        <div className="pl-3">
          <input
            type="button"
            value="Tilbake"
            onClick={goback}
            className="KSPU_button_Gray float-left"
          />
        </div>
        <div className="pr-4">
          <input
            type="button"
            value="Lag utvalg"
            onClick={LagUtvalgClick}
            className="KSPU_button-kw float-right"
          />
        </div>
      </div>

      <div className="paddingBig_NoColor_T">
        <a
          className="KSPU_LinkButton_Url_KW pl-3"
          onClick={GotoMain}
        >
          Avbryt
        </a>
      </div>
      <br />
    </div>
  );
}

export default BudruterKW;