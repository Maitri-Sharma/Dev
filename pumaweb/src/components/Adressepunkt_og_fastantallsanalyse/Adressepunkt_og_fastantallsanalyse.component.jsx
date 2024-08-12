import React, { useState, useContext } from "react";
import "../../App.css";
import expand from "../../assets/images/esri/expand.png";
import collapse from "../../assets/images/esri/collapse.png";
import { MapConfig } from "../../config/mapconfig";
import { CurrentDate } from "../../common/Functions";
import { MainPageContext, KSPUContext } from "../../context/Context";
import { AdressepunktOgFastantallsanalyseConfig } from "../KspuConfig";
import Graphic from "@arcgis/core/Graphic";
import FeatureLayer from "@arcgis/core/layers/FeatureLayer";
import {
  Utvalg,
  NewUtvalgName,
  criterias,
  getAntall,
  getAntallUtvalg,
  Reol,
  saveUtvalg,
} from "../KspuConfig";
import { groupBy } from "../../Data";

import * as locator from "@arcgis/core/rest/locator";
import * as geometryEngine from "@arcgis/core/geometry/geometryEngine";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Point from "@arcgis/core/geometry/Point";

import SelectionDetails from "../SelectionDetails";
import spinner from "../../assets/images/kw/spinner.gif";
import "./Adressepunkt_og_fastantallsanalyse.styles.scss";
import SaveUtvalg from "../SaveUtvalg";

import Extent from "@arcgis/core/geometry/Extent";

function AdressepunktOgFastantallsanalyse(props) {
  const [value, setKjValue] = useState("");
  const [radio1value, setradio1value] = useState(true);
  const [radio2value, setradio2value] = useState(false);
  const [togglevalue, settogglevalue] = useState(false);
  const [addressEnter, setAddressEnter] = useState(" ");
  const [updateValue, setUpdateValue] = useState("");
  const [buttonValue, setButtonValue] = useState("");
  const [displayMsg, setDisplayMsg] = useState(false);
  const [kommuneName, setkommuneName] = useState(" ");
  const [selectedvalue, setselectedvalue] = useState("");
  const { addressPoints, setAddressPoints } = useContext(MainPageContext);
  const { mapView } = useContext(MainPageContext);

  const [searchResult, setSearchResult] = useState([]);
  const [selectedItem, setSelectedItem] = useState("");
  const [placeChangeModalValue, setPlaceChangeModalValue] = useState("");
  const [checkedList, setCheckedList] = useState(addressPoints);
  const [refresh, setRefresh] = useState(false);

  const [uxAntAll, setUxAntAll] = useState(
    AdressepunktOgFastantallsanalyseConfig.defaultBuffer
  );
  const [uxDistance, setUxDistance] = useState(
    AdressepunktOgFastantallsanalyseConfig.defaultDistance
  );
  const [antAllError, setAntAllError] = useState(false);
  const [distanceError, setDistanceError] = useState(false);
  const [antAllErrMsg, setAntAllErrMsg] = useState("");
  const [distanceErrMsg, setDistanceErrMsg] = useState("");
  const [minAddressError, setMinAddressError] = useState(false);
  const [minAddressErrMsg, setMinAddressErrMsg] = useState("");
  const [currentStep, setCurrentStep] = useState(1);

  const [distanceChecked, setDistanceChecked] = useState(true);

  const {
    showReservedHouseHolds,
    setShowReservedHouseHolds,
    setKjDisplay,
    setvalue,
    setAdresDisplay,
  } = useContext(KSPUContext);
  const { showBusiness, setShowBusiness, showHousehold, setShowHousehold } =
    useContext(KSPUContext);
  const { activUtvalg, setActivUtvalg } = useContext(KSPUContext);
  const { resultData, setResultData } = useContext(KSPUContext);
  const [Large, setLarge] = useState(" ");

  const [addressPointGeometry, setAddressPointGeometry] = useState("");
  const [processing, setProcessing] = useState(false);

  let routesData = [];
  let Antall = [];

  const handleKeypress = (e) => {
    //it triggers by pressing the enter key
    if (e.key === "Enter") {
      searchPlace();
    }
  };

  const getSelectedRoutes = (data) => {
    return data.reduce((acc, dt) => {
      if (!(dt.children === undefined)) {
        return acc.concat(getSelectedRoutes(dt.children));
      }
      return acc.concat(dt);
    }, []);
  };

  const handleError = (value, name) => {
    if (name == "uxAntall") {
      if (
        value.length === 0 ||
        value <= 0 ||
        Number.isInteger(Number(value)) === false
      ) {
        setAntAllError(true);
        setAntAllErrMsg("Enter an Integer for number of recipients");
      } else {
        setAntAllErrMsg("");
        setAntAllError(false);
        setUxAntAll(value);
      }
    }

    if (name == "uxDistanceLimit") {
      if (
        value.length === 0 ||
        value <= 0 ||
        Number.isInteger(Number(value)) === false
      ) {
        setDistanceError(true);
        setDistanceErrMsg("Maximum distance kilometers must be an integer.");
      } else {
        setDistanceErrMsg("");
        setDistanceError(false);
        setUxDistance(value);
      }
    }
  };

  function formatData(reolObj) {
    var r = Reol();
    r.name = reolObj.reolnavn;
    r.reolNumber = reolObj.reolnr;
    r.description = reolObj.beskrivelse;
    r.comment = reolObj.kommentar;
    r.descriptiveName = reolObj.beskrivelse + " (" + reolObj.reol_id + ")";
    r.reolId = reolObj.reol_id;
    r.kommuneId = reolObj.kommuneid;
    r.kommune = reolObj.kommune;
    r.kommuneFullDistribusjon = null;
    r.fylkeId = reolObj.fylkeid;
    r.fylke = reolObj.fylke;
    r.teamNumber = reolObj.teamnr;
    r.teamName = reolObj.teamnavn;
    r.postalZone = reolObj.postnr;
    r.postalArea = reolObj.poststed;
    r.segmentId = reolObj.segment;
    r.antall = {
      households: reolObj.hh,
      householdsReserved: reolObj.hh_res,
      farmers: reolObj.gb,
      farmersReserved: reolObj.gb_res,
      houses: reolObj.er,
      housesReserved: reolObj.er_res,
      includeHousesReserved: 0,
      businesses: reolObj.vh,
      totalReserved: 0,
      priorityHouseholdsReserved: reolObj.p_hh_u_res,
      nonPriorityHouseholdsReserved: reolObj.np_hh_u_res,
      priorityBusinessReserved: reolObj.p_vh_u_res,
      nonPriorityBusinessReserved: reolObj.np_vh_u_res,
    };
    r.avisDeknings = null;
    r.prisSone = reolObj.prissone;
    r.ruteType = reolObj.reoltype;
    r.postkontorNavn = reolObj.pbkontnavn;
    r.prsEnhetsId = reolObj.prsnr;
    r.prsName = reolObj.prsnavn;
    r.prsDescription = reolObj.prsbeskrivelse;
    r.frequency = reolObj.rutedistfreq;
    r.sondagFlag = reolObj.sondagflag;
    if (r.description === undefined || r.description === "")
      r.description = reolObj.pbkontnavn;
    if (r.description === undefined || r.description === "")
      r.description = reolObj.prsbeskrivelse;
    // 08.08.2006 - Reolnavn skal brukes dersom den har verdi, ellers får den beskrivelse verdien
    if (r.name === undefined || r.name === "" || r.name === null)
      r.name = r.description;
    return r;
  }

  const runAnalysis = (antAll, distance) => {
    let addresPointGeometry = [];
    if (radio1value) {
      distance = 0;
      setUxDistance(distance);
      antAll = 0;
      setUxAntAll(antAll);
    } else {
      distance = distance * 1000;
    }
    checkedList.map((item) => {
      addresPointGeometry.push(item?.location ? item.location : item?.geometry);
    });

    setProcessing(true);
    const buffer = geometryEngine.buffer(
      addresPointGeometry,
      distance,
      "meters",
      false
    );

    //intersection code
    let BudruterUrl;

    let allLayersAndSublayers = mapView.map.allLayers.flatten(function (item) {
      return item.layers || item.sublayers;
    });

    allLayersAndSublayers.items.forEach(function (item) {
      if (item.title === "Budruter") {
        BudruterUrl = item.url;
      }
    });

    let unionBufferGeometry = null;
    if (distance !== 0) {
      unionBufferGeometry = geometryEngine.union(buffer);
    } else {
      unionBufferGeometry = geometryEngine.union(addresPointGeometry);
    }

    
    // var bufferGraphic = new Graphic(unionBufferGeometry, bufferSymbol);
    //   make sure to remmove previous highlighted feature
    // mapView.graphics.add(bufferGraphic);

    // The "public" function that can be called by passing a reference to the
    // layer. Only provided so the "user" of this module (you) does not have to rememeber
    // to pass [] as the second parameter to the recursive function.
    const getAllFeatures = (layer) => {
      return _getAllFeaturesRecursive(layer, []);
    };

    // Recursive function - Handles calling the service multiple times if necessary.
    const _getAllFeaturesRecursive = (layer, featuresSoFar) => {
      return layer
        .queryFeatures({
          start: featuresSoFar.length,
          num: layer.capabilities.query.maxRecordCount,
          //returnGeometry: true,
          // where: `${reolsWhereClause}`,
          outFields: MapConfig.budruterOutField,
          geometry: unionBufferGeometry,
          //outFields: ["*"],
          spatialRelationship: "intersects",
          //returnDistinctValues: true,
          returnGeometry: true,
          outSpatialReference: mapView.spatialReference,
        })
        .then((results) => {
          // If "exceededTransferLimit" is true, then make another request (call
          //  this same function) with a new "start" position. If not, we're at the end
          // and we should just concatenate the results and return what we have.
          if (
            results.exceededTransferLimit &&
            results.exceededTransferLimit === true
          ) {
            return _getAllFeaturesRecursive(layer, [
              ...featuresSoFar,
              ...results.features,
            ]);
          } else {
            return Promise.resolve([...featuresSoFar, ...results.features]);
          }
        });
    };

    const featureLayer = new FeatureLayer({
      url: BudruterUrl,
    });

    featureLayer.when(() => {
      getAllFeatures(featureLayer).then((results) => {
        let unique = {};

        let distinctFeatures = results.filter(function (result) {
          let oid = result.attributes.objectid;

          if (!unique[oid]) {
            unique[oid] = oid;

            return true;
          }

          return false;
        });

        results = distinctFeatures;

        // setloading(true);
        if (results.length > 0) {
          let filteredcurrentReoler = [];

          //sort the results
          for (let i = 0; i < addresPointGeometry.length; i++) {
            let distances = [];
            for (let j = 0; j < results.length; j++) {
              distances.push({
                index: j,
                distance: geometryEngine.distance(
                  addresPointGeometry[i],
                  results[j].geometry,
                  "kilometers"
                ),
              });
            }

            let sortedDistances = [];
            sortedDistances = distances.sort(function (a, b) {
              return a.distance - b.distance;
            });
            let sortedResults = [];

            for (let k = 0; k < sortedDistances.length; k++) {
              sortedResults.push(results[sortedDistances[k].index]);
            }

            let totalCount = 0;
            sortedResults.map((item) => {
              if (antAll) {
                if (totalCount <= antAll) {
                  totalCount = totalCount + item.attributes.hh;

                  filteredcurrentReoler.push(formatData(item.attributes));
                }
              } else {
                filteredcurrentReoler.push(formatData(item.attributes));
              }
            });
          }

          //s  let currentReoler = [...new Set(filteredcurrentReoler)];
          const reolIds = filteredcurrentReoler.map((o) => o.reolId);
          const currentReoler = filteredcurrentReoler.filter(
            ({ reolId }, index) => !reolIds.includes(reolId, index + 1)
          );

          let utvalg = saveUtvalg();
          setShowReservedHouseHolds(false);
          setShowBusiness(false);
          utvalg.reoler = currentReoler;
          utvalg.hasReservedReceivers = showReservedHouseHolds ? true : false;
          utvalg.name = NewUtvalgName();
          let data = groupBy(
            currentReoler,
            "",
            0,
            showHousehold,
            showBusiness,
            showReservedHouseHolds,
            [],
            ""
          );
          setResultData(data);
          let Antall = getAntallUtvalg(currentReoler);
          utvalg.Antall = Antall;
          utvalg.totalAntall =
            (showHousehold ? Antall[0] : 0) +
            (showBusiness ? Antall[1] : 0) +
            (showReservedHouseHolds ? Antall[2] : 0);
          utvalg.hush = Antall[0];
          utvalg.Business = Antall[1];
          utvalg.ReservedHouseHolds = Antall[2];
          if (showHousehold)
            utvalg.receivers.push({ receiverId: 1, selected: true });
          if (showBusiness)
            utvalg.receivers.push({ receiverId: 4, selected: true });
          if (showReservedHouseHolds)
            utvalg.receivers.push({ receiverId: 5, selected: true });
          utvalg.modifications = [];
          utvalg.modifications.push({
            modificationId: Math.floor(100000 + Math.random() * 900000),
            userId: "Internbruker",
            modificationTime: CurrentDate(),
            listId: 0,
          });
          let criteraString = "";
          if (antAll) {
            criteraString =
              "antall " +
              antAll +
              " ant og Avansert kriterie: Inntil " +
              distance / 1000 +
              " km";
          } else {
            criteraString = " ";
          }
          let fastantallsanalyseType = 9;
          utvalg.criterias.push(
            criterias(fastantallsanalyseType, criteraString)
          );
          //utvalg.criterias.push(criterias("Fra adressepunkt", ""));

          setActivUtvalg({});
          setActivUtvalg(utvalg);
          setCurrentStep(currentStep + 1);
          setProcessing(false);
        }
      });
    });
    featureLayer.load();
  };

  const _Next = () => {
    if (currentStep === 1 && processing != true) {
      if (checkedList.length === 0) {
        setMinAddressError(true);
        setMinAddressErrMsg("Select atleast one address point");
      } else {
        setMinAddressError(false);
        setMinAddressErrMsg("");
      }

      if (
        antAllError !== true &&
        distanceError !== true &&
        checkedList.length !== 0
      ) {
        
        let j = mapView.graphics.items.length;

        for (var i = j; i > 0; i--) {
          if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
            mapView.graphics.remove(mapView.graphics.items[i - 1]);
            
          }
        }
        
        runAnalysis(uxAntAll, uxDistance);
        sessionStorage.setItem("addressPoints", JSON.stringify(addressPoints));
        updateMap(addressPoints);
      }
    }
  };

  const _Back = () => {
    if (currentStep === 2 && processing != true) {
      setCurrentStep(currentStep - 1);
      setUxAntAll(AdressepunktOgFastantallsanalyseConfig.defaultBuffer);
      setUxDistance(AdressepunktOgFastantallsanalyseConfig.defaultDistance);
    }
  };

  const _Cancel = () => {
    setKjDisplay(false);
    setAdresDisplay(false);
    setvalue(true);
    
  };

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
      zoomTil(item?.location ? item.location : item?.geometry);
    });
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

  const Fjern = (item) => {
    var arr = addressPoints.filter((data) => data !== item);
    var checkedArray = checkedList?.filter(
      (selectedItem) => selectedItem?.key !== item?.key
    );
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
    //Once selection is created and after that when we update address name address name was not updated in session storage
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

  const searchPlace = async () => {
    window.$("#budruteresult").modal("show");
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
    setKjValue(e.target.value);
  };

  const toggle = () => {
    settogglevalue(!togglevalue);
  };
  const AdresOne = (e) => {
    setradio1value(true);
    setradio2value(false);
    setAntAllError(false);
  };
  const AdresAntall = (e) => {
    setradio1value(false);
    setradio2value(true);
    handleError(uxAntAll, "uxAntall");
  };

  const showLarge = (e) => {
    setLarge("Save_Large");
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
    // <div >
    <div className="card addressContainer">
      {Large == "Save_Large" ? <SaveUtvalg id={"uxBtnLagre12"} /> : null}
      <div>
        {/* <!-- Modal --> */}
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
          <span className="adress-text">
            Adressepunkt og fastantallsanalyse
          </span>
        </div>
        <div className="col-4 span-color1">
          <span className="d-flex float-right adress-text pt-1">
            Trinn {currentStep} av 2
          </span>
        </div>
      </div>
      <div className="sok-text">
        <p className="pl-1">
          Dersom du har et bestemt antall sendinger som du ønsker å distribuere
          og ønsker å nå nye eller eksisterende kunder i definerte områder er
          denne målgruppeanalysen godt egnet.
        </p>
      </div>
      {processing ? (
        // <Spinner/>
        <div className="Adressepunkt_og_fastantallsanalyseSpinnerDiv">
          <img
            className="mb-1"
            src={spinner}
            style={{ height: 30, width: 30 }}
          />
        </div>
      ) : // 'loading'
      currentStep === 1 ? (
        <div className="Kj-div-background-color pl-1 pr-1 pt-1 ">
          <div className="Kj-background-color pl-1 pb-1 ">
            <span className="install-text">
              INNSTILLINGER FOR ADRESSEPUNKT OG FASTANTALLSANALYSE
            </span>
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
          {distanceError ? (
            <span className="sok-text ml-2 red">{distanceErrMsg}</span>
          ) : null}

          <p className="label pl-1 mt-1">
            Velg adressepunktet/adressepunktene som skal være med i analysen
          </p>
          <span className="label pl-1 mt-1"> Legg til adressepunkt</span>
          <div className="row ml-1 mt-1">
            <div className="input-groupco-4 pr-1">
              <i className="fa fa-user-circle-o pl-1"></i>
              <input
                type="text"
                className="InputValueText mt-1"
                value={value}
                placeholder="- Skriv inn adresse her -"
                onChange={InputClick}
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
                            <a
                              href="#"
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
          <p className="spanstyle1"></p>
          <p className="label p-2">Velg type analyse</p>
          <div className="sok-text ml-3">
            <div className="mt-2">
              <input
                type="radio"
                name="optradio"
                onChange={AdresOne}
                checked={radio1value}
              />{" "}
              Finn budruten adressepunktet/ene tilhører
            </div>
            <div className="mt-2">
              <input
                type="radio"
                name="optradio"
                onChange={AdresAntall}
                checked={radio2value}
              />{" "}
              Finn budrutene rundt adressepunktet/ene inntil ønsket antall
              mottakere er nådd
            </div>
          </div>

          <div className="mt-2 mb-2">
            <span className="sok-text">
              Ønsket antall mottakere rundt hvert adressepunkt
              <input
                onChange={(e) => {
                  setUxAntAll(e.target.value);
                  handleError(e.target.value, e.target.name);
                }}
                type="text"
                name="uxAntall"
                className="form-adress  ml-2"
                defaultValue={
                  AdressepunktOgFastantallsanalyseConfig.defaultBuffer
                }
                placeholder=""
                disabled={radio1value}
                maxLength={8}
              />
            </span>
          </div>
          <div className="card Kj-background-color">
            <div className="row mr-1 ">
              <div className="col-6">
                <p className="avan p-1">AVANSERT</p>
              </div>
              <div className="col-6">
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

            {togglevalue ? (
              <div className="Kj-div-background-color p-1 pt-2 pb-2">
                <input
                  className="form-check-input ml-1 "
                  type="checkbox"
                  value=""
                  id="defaultCheck1"
                  defaultChecked={distanceChecked}
                  onChange={() => {
                    setDistanceChecked(!distanceChecked);
                  }}
                />
                <label className="sok-text ml-3 pl-1" htmlFor="defaultCheck1">
                  Begrens avstand til
                  <input
                    type="text"
                    name="uxDistanceLimit"
                    maxLength={20}
                    defaultValue={
                      AdressepunktOgFastantallsanalyseConfig.defaultDistance
                    }
                    className="Kj-input ml-1 mr-1"
                    placeholder=""
                    onChange={(e) => {
                      handleError(e.target.value, e.target.name);
                      setUxDistance(e.target.value);
                    }}
                  />
                  km.
                </label>
              </div>
            ) : null}
          </div>
        </div>
      ) : (
        <div>
          <SelectionDetails
            maxDistance={distanceChecked ? uxDistance * 1000 : 0}
            uxAntAll={radio1value ? 0 : uxAntAll}
            analysisType={radio1value ? "Finn rolen" : "Fast antall"}
          />
        </div>
      )}
      <div className="row mt-1 mb-1 zero-margin">
        <div className="col-lg-4 col-md-4 col-sm-4 col-xs-4 p-0 pl-1">
          {currentStep === 2 ? (
            <input
              type="submit"
              className="KSPU_button"
              value="&lt;&lt; Forrige"
              onClick={_Back}
            />
          ) : null}
        </div>
        <div className="col-lg-4 col-md-4 col-sm-4 col-xs-4 p-0 _flex-end">
          {currentStep === 2 ? (
            <input
              type="submit"
              className="KSPU_button"
              value="Avbryt"
              onClick={_Cancel}
            />
          ) : null}
        </div>
        {currentStep === 1 ? (
          <div className="col-lg-4 col-md-4 col-sm-4 col-xs-4 p-0 pr-1 _flex-end">
            <input
              type="submit"
              className="KSPU_button "
              value="Neste &gt;&gt;"
              onClick={_Next}
            />
          </div>
        ) : (
          <div className="col-lg-4 col-md-4 col-sm-4 col-xs-4 p-0 pr-1">
            <input
              type="submit"
              id="uxBtnLagre"
              className="KSPU_button float-right"
              value="Lagre"
              data-toggle="modal"
              data-target="#uxBtnLagre12"
              onClick={showLarge}
              style={{
                text: "Lagre",
                marginLeft: "auto",
                float: "right",
              }}
            />
          </div>
        )}
      </div>
    </div>
  );
}

export default AdressepunktOgFastantallsanalyse;
