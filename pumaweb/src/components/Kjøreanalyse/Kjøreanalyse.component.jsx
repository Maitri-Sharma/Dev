import React, { useState, useContext, useRef } from "react";
import "../../App.css";
import expand from "../../assets/images/esri/expand.png";
import collapse from "../../assets/images/esri/collapse.png";
import { MapConfig } from "../../config/mapconfig";
import * as locator from "@arcgis/core/rest/locator";
import { MainPageContext, KSPUContext } from "../../context/Context";
import * as geometryEngine from "@arcgis/core/geometry/geometryEngine";

import KjøreanalyseComponentLoading from "../Kjøreanalyse/Kjøreanalyse.component_loading";
import Extent from "@arcgis/core/geometry/Extent";
import Graphic from "@arcgis/core/Graphic";
function KjøreAnalyse(props) {
  const [value, setValue] = useState("");
  const [radio1value, setradio1value] = useState(true);
  const [radio2value, setradio2value] = useState(false);

  const [displayMsg, setDisplayMsg] = useState(false);

  const { addressPoints, setAddressPoints } = useContext(MainPageContext);
  const [searchResult, setSearchResult] = useState([]);

  const [selectedItem, setSelectedItem] = useState("");
  const [placeChangeModalValue, setPlaceChangeModalValue] = useState("");
  const [checkedList, setCheckedList] = useState(addressPoints);
  const [refresh, setRefresh] = useState(false);
  const antallValue = useRef();
  const dist = useRef();
  const checkBoxValue = useRef();
  const [maxAntall, setMaxAntall] = useState("");
  const [antallKm, setAntallKm] = useState(0);
  const [antallMinute, setAntallMinute] = useState(0);
  const [multiSelection, setMultiSelection] = useState(false);
  const [currentStep, setCurrentStep] = useState(1);
  const { mapView } = useContext(MainPageContext);
  const { showBusiness, showHousehold, setKjDisplay, setvalue } =
    useContext(KSPUContext);
  const [addressPointGeometry, setAddressPointGeometry] = useState("");
  const [minAddressError, setMinAddressError] = useState(false);
  const [minAddressErrMsg, setMinAddressErrMsg] = useState("");
  const [antAllError, setAntAllError] = useState(false);
  const [antAllErrMsg, setAntAllErrMsg] = useState("");
  //search result operations
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

      mapView.goTo(addressPointGeometry);
      zoomTil(item?.location ? item.location : item?.geometry);
    });
  };
  //search result operations
  const getAntall = () => {
    setMaxAntall(antallValue.current.value);
  };
  const KjMinute = (e) => {
    setMinAddressError(false);
    // setMinAddressErrMsg("");
    setAntAllError(false);
    setradio1value(true);
    setradio2value(false);
    distance();
  };
  const KjKilometer = (e) => {
    setMinAddressError(false);
    // setMinAddressErrMsg("");
    setAntAllError(false);
    setradio1value(false);
    setradio2value(true);
    distance();
  };
  const multipleSelection = (val) => {
    if (val === true) {
      setMultiSelection(true);
    } else {
      setMultiSelection(false);
    }
  };
  const distance = () => {
    setMinAddressError(false);
    
    setAntAllError(false);
    
    let val = 0;
    let selectionText = document.getElementById("distanceInput").value;
    if (radio2value) {
      if (selectionText !== undefined && selectionText !== "") {
        setAntallKm(selectionText);
      } else {
        setAntallKm(val);
      }

      setAntallMinute(val);
    }
    if (radio1value) {
      if (selectionText !== undefined) {
        setAntallMinute(selectionText);
      } else {
        setAntallMinute(val);
      }

      setAntallKm(val);
    }
    // if (
    //   selectionText.length === 0 ||
    //   selectionText <= 0 ||
    //   Number.isInteger(Number(selectionText)) === false
    // ) {
    //   setAntAllError(true);
    //   setAntAllErrMsg("Enter an Integer");
    // } else {
    //   setAntAllErrMsg("");
    //   setAntAllError(false);
    // }
  };
  const selectAll = () => {
    setRefresh(true);
    setCheckedList(addressPoints);
    setTimeout(() => {
      setRefresh(false);
    }, 10);
  };

  const deSelectAll = () => {
    setRefresh(true);
    setCheckedList([]);
    setTimeout(() => {
      setRefresh(false);
    }, 10);
  };

  const Velg = (item) => {
    let val = [];
    val.push(item);
    setAddressPoints(addressPoints.concat(val));
    setCheckedList(checkedList.concat(item));
    updateMap(val);
  };
  const handleCancel = () => {
    setKjDisplay(false);
    setvalue(true);
  };
  const Fjern = (item) => {
    var arr = addressPoints.filter((data) => data !== item);
    var checkedArray = checkedList?.filter((selectedItem) => selectedItem?.key !== item?.key);
    setAddressPoints(arr);
    setCheckedList(checkedArray);
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

  const changePlaceName = () => {
    addressPoints.map((item) => {
      if (item.key === selectedItem) {
        if (item.attributes.display === undefined) {
          item.attributes.Match_addr = placeChangeModalValue;
          item.address = placeChangeModalValue;
        } else {
          item.attributes.display = placeChangeModalValue;
        }

        deleteFromMap(item?.location ? item.location : item?.geometry);
        updateMap([item]);
        
      }
    });
    setPlaceChangeModalValue("");
    setAddressPoints([...addressPoints]);
    sessionStorage.setItem("addressPoints", JSON.stringify(addressPoints));
  };

  const deleteChecked = () => {
    setRefresh(true);

    if (checkedList.length > 0) {
      var newArray = [];
      for (let i = 0; i < addressPoints.length; i++) {
        for (let j = 0; j < checkedList.length; j++) {
          var f = 0;
          if (i === checkedList[j].key) {
            deleteFromMap(addressPoints[i].location);
            f = 1;
            break;
          }
        }
        if (f === 0) {
          newArray.push(addressPoints[i]);
        }
      }
      setAddressPoints(newArray);
      setCheckedList([]);
    }

    setTimeout(() => {
      setRefresh(false);
    }, 10);
  };

  const deleteAll = () => {
    setAddressPoints([]);
    setCheckedList([]);
    mapView.graphics.removeAll();
  };

  const handleCheckboxChange = (event, item) => {
    if (event.target.checked) {
      checkedList.push(item);
    } else {
      var filteredArr = checkedList.filter((val) => {
        return val.key !== item.key;
      });
      setCheckedList(filteredArr);
    }
  };

  const handleKeypress = (e) => {
    //it triggers by pressing the enter key
    if (e.key === "Enter") {
      searchPlace();
    }
  };

  const searchPlace = async () => {
    window.$("#budruteresult").modal("show");
    setMinAddressError(false);
    setMinAddressErrMsg("");
    const findPlaces = (category, pt) => {
      locator
        .addressToLocations(MapConfig.geoSokUrl, {
          address: {
            OBJECTID: 0,
            SingleLine: value,
          },
          countryCode: "",
          categories: "",
          maxLocations: 25,
          outFields: "*",
        })

        .then(function (results) {
          setSearchResult(results);
        });
    };
    await findPlaces("Populated Place", "viken");
  };

  const InputClick = (e) => {
    setValue(e.target.value);
  };
  const nextClick = async () => {
    // debugger;

    if (checkedList.length === 0) {
      setMinAddressError(true);
      setMinAddressErrMsg("Velg minst ett adressepunkt.");
    } else if (
      radio1value &&
      (antallMinute.length === 0 || antallMinute.length === undefined)
    ) {
      setAntAllError(true);
      setAntAllErrMsg("Angi antall minutter (som et heltall).");
    } else if (
      (antallKm.length === 0 || antallKm.length === undefined) &&
      radio2value
    ) {
      setAntAllError(true);
      setAntAllErrMsg("Angi antall kilometer (som et heltall).");
    } else {
      if (antAllError !== true && checkedList.length !== 0) {
        if (antallKm.length !== 0 || antallMinute.length !== 0) {
          setCurrentStep(currentStep + 1);
        }
      }
    }
  };
  const callback = (step) => {
    setCurrentStep(step - 1);
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

  const zoomAll = () => {
    let geo = [];
    addressPoints.map((item) => {
      geo.push(item?.location ? item.location : item?.geometry);
    });
    let totalGeo = geometryEngine.union(geo);

    var newExtent = new Extent({
      xmin: totalGeo.extent.xmin,
      xmax: totalGeo.extent.xmax,
      ymin: totalGeo.extent.ymin,
      ymax: totalGeo.extent.ymax,
      spatialReference: { wkid: mapView.center.spatialReference.wkid },
    });
    mapView.goTo({ target: newExtent });
  };
  return (
    <div className="card">
      <div>
        <div
          className="modal fade bd-example-modal-lg"
          id="budruteresult"
          tabIndex="-1"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
          aria-hidden="true"
        >
          <div
            className="modal-dialog modal-dialog-centered budrutemax"
            role="document"
          >
            <div className="modal-content">
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
                      Feil ved adressesøk: En feil oppsto ved adressesøk.
                      Kontakt superbruker i Posten.
                    </span>
                  </div>
                ) : (
                  <div>
                    <div className="">
                      <table className="tableRow">
                        <tbody>
                          <tr className="flykeHeader">
                            <th className="tabledataRow budruteRow">Gate/sted</th>
                            <th className="tabledataRow budruteRow">Husnummer</th>
                            <th className="tabledataRow budruteRow">Score</th>
                            <th className="tabledataRow budruteRow">Type</th>
                            <th className="tabledataRow budruteRow">
                              &nbsp;&nbsp;&nbsp;&nbsp;
                            </th>
                          </tr>
                          {searchResult.map((item, i) => (
                            <tr key={i}>
                              <td className="flykecontent tabledataRow">
                                {item.attributes.Match_addr}
                              </td>
                              <td className="flykecontent tabledataRow">
                                {item.attributes.Husnummer}
                              </td>
                              <td className="flykecontent tabledataRow">
                                {item.attributes.Score}
                              </td>
                              <td className="flykecontent tabledataRow">
                                {item.attributes.Addr_type}
                              </td>
                              <td className="flykecontent tabledataRow">
                                <a
                                  id=""
                                  href="#"
                                  className="KSPU_LinkButton float-right mr-1"
                                  onClick={() => {
                                    Velg(item);
                                  }}
                                  data-dismiss="modal"
                                >
                                  velg
                                </a>
                              </td>
                            </tr>
                          ))}
                        </tbody>
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
                  value={placeChangeModalValue}
                  onChange={(e) => setPlaceChangeModalValue(e.target.value)}
                />{" "}
              </div>
              <div className="modal-footer">
                <button
                  type="button"
                  className="btn btn-primary"
                  onClick={changePlaceName}
                  data-dismiss="modal"
                >
                  Ok
                </button>
                <button
                  type="button"
                  className="btn btn-secondary"
                  data-dismiss="modal"
                >
                  Cancel
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className=" row pl-1 pr-1">
        <div className="col-8 span-color1">
          <span className="sok-text1">Kjøreanalyse</span>
        </div>
        <div className="col-4 span-color1">
          <span className="d-flex float-right sok-text1 pt-1">
            Trinn {currentStep} av 3{" "}
          </span>
        </div>
      </div>
      <div className="sok-text">
        <p className="pl-1">
          Dersom du har et utsalgssted og ønsker å nå nye eller eksisterende
          kunder innenfor geografisk nærhet til butikken, er dette et godt
          verktøy. Du kan velge kjøretid i minutter eller kjøreavstand i
          kilometer.
        </p>
      </div>
      {currentStep === 1 ? (
        <div className="Kj-div-background-color  pl-1 pr-1 pt-1 ">
          <div className="Kj-background-color pl-1 pb-1 ">
            <span className="install-text">INNSTILLINGER FOR KJØREANALYSE</span>
          </div>
          {minAddressError ? (
            <span className="sok-text ml-2 red">
              {minAddressErrMsg}
              <br />
            </span>
          ) : null}
          {antAllError ? (
            <span className="sok-text ml-2 red">
              {antAllErrMsg}
              <br />
            </span>
          ) : null}
          <p className="label p-2">
            Velg adressepunktet/adressepunktene som skal være med i analysen
          </p>
          <span className="sok-text ml-2"> Legg til adressepunkt</span>
          <div className="row ml-1 mt-1">
            <div className="input-groupco-4 pr-1">
              <i className="fa fa-user-circle-o pl-1"></i>
              <input
                type="text"
                className="InputValueText mt-1"
                value={value}
                onChange={InputClick}
                placeholder="- Skriv inn adresse her -"
                onKeyPress={handleKeypress}
              />
            </div>
            <button
              id="uxShowUtvalgDetails_uxUtvalgReoler_uxPnlTree_uxShowDetails"
              className="KSPU_button ml-1 mt-1"
              data-toggle="modal"
              data-target="#budruteresult"
              onClick={searchPlace}
              // onClick={FinnClick}
            >
              Finn
            </button>
          </div>
          <div>
            {addressPoints.length > 0 ? (
              <table
                className="budruteTable"
                cellSpacing="0"
                rules="all"
                border="1"
              >
                {refresh ? null : (
                  <tbody>
                    <tr className="GridView_Row">
                      <td>
                        <a
                          href="#"
                          className="KSPU_LinkButton float-right mr-1"
                          onClick={selectAll}
                        >
                          Velg alle
                        </a>
                      </td>
                      <td>
                        <a
                          href="#"
                          className="KSPU_LinkButton float-right mr-1"
                          onClick={deSelectAll}
                        >
                          Velg ingen
                        </a>
                      </td>
                      <td>
                        <p
                          className="KSPU_LinkButton float-right mr-1"
                          onClick={zoomAll}
                        >
                          Zoom til valgte
                        </p>
                      </td>
                      <td>
                        <a
                          href="#"
                          className="KSPU_LinkButton float-right mr-1"
                          onClick={deleteChecked}
                        >
                          Fjern valgte
                        </a>
                      </td>
                      <td>
                        <a
                          href="#"
                          className="KSPU_LinkButton float-right mr-1"
                          onClick={deleteAll}
                        >
                          Fjern alle
                        </a>
                      </td>
                    </tr>
                    {addressPoints.map((item, key) => {
                      let obj;
                      obj = item;
                      obj["key"] = key;
                      return (
                        <tr className="GridView_Row" key={key}>
                          <td>
                            <input
                              type="checkbox"
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
                            {item?.attributes?.display
                              ? item?.attributes?.display
                              : item?.attributes?.Match_addr}
                          </td>
                          <td className="flykecontent">
                            <p
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
                            </p>
                            {/* <a> {buttonValue}</a> */}
                          </td>
                          <td className="flykecontent">
                            <a
                              id="uxShowUtvalgDetails_uxUtvalgReoler_uxPnlTree_uxShowDetails"
                              href="#"
                              className="KSPU_LinkButton float-right mr-1"
                              data-toggle="modal"
                              data-target="#kommunefordeling"
                              onClick={() => {
                                setSelectedItem(item.key);
                              }}
                            >
                              Endre navn
                            </a>
                          </td>

                          <td className="flykecontent">
                            <a
                              href="#"
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
                    
                  </tbody>
                )}
              </table>
            ) : (
              <span className="sok-text ml-2">
                Ingen adressepunkter er lagt til.
              </span>
            )}
          </div>
          <p className="label p-2 mt-3">
            Spesifiser eventuelt maksimumsantall pr adressepunkt
          </p>
          <div className="row m-0 p-0 pl-3 pb-3">
            <div className="pr-1">
              <span className="sok-text">Maks. antall:</span>
            </div>
            <div>
              <input
                type="text"
                ref={antallValue}
                className="InputValueText Kj-input ml-1"
                placeholder=""
                onChange={getAntall}
              />
            </div>
          </div>
          <p className="label p-2">
            Velg kjøretid eller kjøreavstand fra adressepunkt
          </p>
          <div className="sok-text ml-3 pb-2">
            <div className="pb-2">
              <input
                type="radio"
                name="optradio"
                className="sok-text"
                onChange={KjMinute}
                checked={radio1value}
              />{" "}
              Kjøretid fra adressepunktet/ene
            </div>
            <div>
              <input
                type="radio"
                name="optradio"
                onChange={KjKilometer}
                checked={radio2value}
              />{" "}
              Kjøreavstand fra adressepunktet/ene
            </div>
          </div>
          <div className="row pl-3 mt-2">
            <div className="radio">
              {radio1value ? (
                <span className="sok-text">Spesifiser antall minutter</span>
              ) : null}
              {radio2value ? (
                <span className="sok-text">Spesifiser antall kilometer</span>
              ) : null}
            </div>
            <div className="pb-2">
              <input
                type="text"
                className="Kj-input ml-2 w-75"
                id="distanceInput"
                useRef={dist}
                placeholder=""
                onChange={distance}
              />
            </div>
            <div className="sok-text ml-2 mt-2 pb-2">
              <input
                type="checkbox"
                value="1"
                useRef={checkBoxValue}
                id="seprateUtvalg"
                onChange={(e) => {
                  multipleSelection(e.target.checked);
                }}
              />{" "}
              Ønsker et unikt utvalg pr. adressepunkt
            </div>
          </div>

          <div className="row mt-1 mb-1">
            {/* <div className="col-lg-4 col-md-0 col-sm-0 col-xs-0 m-0 p-0"></div> */}
            <div className="col-lg-6 col-md-6 col-sm-6 col-xs-6 p-0 pl-1 _flex-start">
              <input
                type="submit"
                className="KSPU_button ml-3"
                value="Avbryt"
                onClick={handleCancel}
              />
            </div>
            <div className="col-lg-6 col-md-6 col-sm-6 col-xs-6 p-0 pr-1 _flex-end">
              <input
                type="submit"
                className="KSPU_button mr-3"
                value="Neste &gt;&gt;"
                onClick={nextClick}
              />
            </div>
          </div>
        </div>
      ) : currentStep === 2 ? (
        <KjøreanalyseComponentLoading
          maxAntall={maxAntall}
          antallKm={antallKm}
          antallMinute={antallMinute}
          currentStep={currentStep}
          parentCallback={callback}
          multiSelection={multiSelection}
          selectionAddressPoint={checkedList}
        />
      ) : null}
    </div>
  );
}
export default KjøreAnalyse;
