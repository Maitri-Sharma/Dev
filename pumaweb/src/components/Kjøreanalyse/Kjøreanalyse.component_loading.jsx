import React, { useState, useContext, useRef, useEffect } from "react";
import { MainPageContext } from "../../context/Context";
import { KSPUContext } from "../../context/Context.js";
import { groupBy } from "../../Data";

import KjøreanalyseComponentResultant from "../Kjøreanalyse/Kjøreanalyse.component_resultant";

import Geoprocessor from "@arcgis/core/tasks/Geoprocessor";
import SpatialReference from "@arcgis/core/geometry/SpatialReference";
import { submitJob } from "@arcgis/core/rest/geoprocessor";
import JobInfo from "@arcgis/core/rest/support/JobInfo";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";
import { Result } from "antd";
import * as geometryEngine from "@arcgis/core/geometry/geometryEngine";
import * as webMercatorUtils from "@arcgis/core/geometry/support/webMercatorUtils";
import { MapConfig } from "../../config/mapconfig";
import Spinner from "../../components/spinner/spinner.component";
import api from "../../services/api.js";
import spinner from "../../assets/images/kw/spinner.gif";
import SelectionDetails from "../SelectionDetails";
import Swal from "sweetalert2";
import "./Kjøreanalyse.styles.scss";

import {
  Reol,
  getAntallUtvalg,
  criterias,
  Utvalg,
  NewUtvalgName,
} from "../KspuConfig";
import KjøreAnalyse from "./Kjøreanalyse.component";
function KjøreanalyseComponentLoading(props) {
  const { addressPoints, setAddressPoints } = useContext(MainPageContext);
  const [kjAddressPoint, setKjAddressPoint] = useState(
    props.selectionAddressPoint
  );
  const [Budruterresult, setBudruterresult] = useState([]);
  const { mapView } = useContext(MainPageContext);

  const [melding, setmelding] = useState(false);
  const [errormsg, seterrormsg] = useState("");
  const [loadtext, setloadtext] = useState("Initialisere");
  const [loadingdiv, setloadingdiv] = useState(true);
  const [processing, setProcessing] = useState(false);
  const [currentStep, setCurrentStep] = useState(props.currentStep);
  const { resultData, setResultData } = useContext(KSPUContext);
  const { activUtvalg, setActivUtvalg } = useContext(KSPUContext);

  const {
    showBusiness,
    showHousehold,
    setKjDisplay,
    setvalue,
    setshoworklist,
    showorklist,
    setActivUtvalglist,

    setutvalglistcheck,
    setShowReservedHouseHolds,
    setShowBusiness,
  } = useContext(KSPUContext);
  const { showReservedHouseHolds } = useContext(KSPUContext);

  const [createdUtvalg, setCreatedUtvalg] = useState([]);
  const [isLoading, setIsloading] = useState(true);

  function formatData(reolObj) {
    var r = Reol();
    r.name = reolObj.reolnavn;
    r.distance = reolObj.distance;
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
  useEffect(() => {
    setShowReservedHouseHolds(false);
    setShowBusiness(false);
    const fetchdata = async () => {
      sessionStorage.setItem("addressPoints", JSON.stringify(addressPoints));

      setProcessing(true);
      let MAP_URL = MapConfig.driveTimeAnalysisUrl;
      let MAP_URL_VALUE = "";
      let submitjobparams = {};
      let j = mapView.graphics.items.length;

      for (var i = j; i > 0; i--) {
        if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
          mapView.graphics.remove(mapView.graphics.items[i - 1]);
        }
      }
      setActivUtvalglist({});

      setutvalglistcheck(false);

      if (props.multiSelection) {
        var newArray1 = [];
        setIsloading(true);
        var skipDuplicateRoute = [];
        var duplicateRoute = [];
        kjAddressPoint.map((item, i) => {
          let TempFeature = [];
          if (item.location !== undefined) {
            TempFeature.push({
              geometry: {
                x: item.location.x,
                y: item.location.y,
              },
              attributes: {
                OBJECTID: i + 1,
                Name: item.attributes.Match_addr,
              },
            });
          } else {
            TempFeature.push({
              geometry: {
                x: item.geometry.x,
                y: item.geometry.y,
              },
              attributes: {
                OBJECTID: i + 1,
                Name: item.attributes.Match_addr,
              },
            });
          }

          let TempObjectValue = {
            displayFieldName: "",
            geometryType: "esriGeometryPoint",
            spatialReference: {
              wkid: 32633,
              latestWkid: 32633,
            },
            fields: [
              {
                name: "OBJECTID",
                type: "esriFieldTypeOID",
                alias: "OBJECTID",
              },
              {
                name: "Name",
                type: "esriFieldTypeString",
                alias: "Name",
                length: 254,
              },
            ],
            features: TempFeature,

            exceededTransferLimit: false,
          };

          if (props.antallKm !== 0) {
            MAP_URL_VALUE = MapConfig.driveTimeAnalysisUrl + "/DriveDistance";
            let distanceKM = props.antallKm * 1000;
            submitjobparams = {
              Distance: distanceKM,
              StartingPoints: TempObjectValue,
            };
          } else {
            MAP_URL_VALUE = MapConfig.driveTimeAnalysisUrl + "/DriveTime";
            submitjobparams = {
              Minutes: props.antallMinute,
              StartingPoints: TempObjectValue,
            };
          }

          let geoprocessor = new Geoprocessor({
            url: MAP_URL_VALUE,

            outSpatialReference: SpatialReference.WebMercator,
          });

          submitJob(MAP_URL_VALUE, submitjobparams).then(async function (
            jobInfo
          ) {
            let jobid = jobInfo.jobId;
            setloadtext("behandling");

            let ReolID = [];
            await geoprocessor
              .waitForJobCompletion(jobid)
              .then(async function (result) {
                if (result.jobStatus == "job-succeeded") {
                  setloadtext("suksess");
                  setloadingdiv(false);
                  await geoprocessor
                    .getResultData(result.jobId, "result")
                    .then(async function (Result) {
                      Result.value.features.map((item) => {
                        ReolID.push(item.attributes.reol_id);
                        //ReolID.push(item.attributes.REOL_ID);
                      });
                    });
                } else {
                  setmelding(true);
                  seterrormsg("noe gikk galt. Prøv igjen senere.");
                }
              });

            let k = ReolID.map((element) => "'" + element + "'").join(",");
            let reolsWhereClause = `reol_id in (${k})`;
            let BudruterUrl;

            let allLayersAndSublayers = mapView.map.allLayers.flatten(function (
              item
            ) {
              return item.layers || item.sublayers;
            });

            allLayersAndSublayers.items.forEach(function (item) {
              if (item.title === "Budruter") {
                BudruterUrl = item.url;
              }
            });
            const kommuneName = await GetAllBurdruter();
            async function GetAllBurdruter() {
              let queryObject = new Query();

              queryObject.where = `${reolsWhereClause}`;
              queryObject.returnGeometry = true;
              queryObject.outFields = MapConfig.budruterOutField;

              await query
                .executeQueryJSON(BudruterUrl, queryObject)
                .then(function (results) {
                  if (results.features.length > 0) {
                    let featuresGeometry = [];
                    let currentReoler = [];

                    let distances = [];

                    //sort the results
                    for (let i = 0; i < results.features.length; i++) {
                      distances.push({
                        index: i,
                        distance: geometryEngine.distance(
                          item.location ? item.location : item.geometry,
                          results.features[i].geometry,
                          "kilometers"
                        ),
                      });
                    }

                    let sortedDistances = distances.sort(function (a, b) {
                      return a.distance - b.distance;
                    });
                    let sortedResults = [];

                    for (let j = 0; j < sortedDistances.length; j++) {
                      results.features[sortedDistances[j].index].attributes[
                        "distance"
                      ] = sortedDistances[j].distance;
                      sortedResults.push(
                        results.features[sortedDistances[j].index]
                      );
                    }

                    let totalCount = 0;
                    sortedResults.map((item) => {
                      if (props.maxAntall) {
                        if (totalCount <= props.maxAntall) {
                          if (skipDuplicateRoute.length > 0) {
                            if (
                              !skipDuplicateRoute.includes(
                                item.attributes.reol_id
                              )
                            ) {
                              totalCount = totalCount + item.attributes.hh;

                              skipDuplicateRoute.push(item.attributes.reol_id);
                              currentReoler.push(formatData(item.attributes));
                              featuresGeometry.push(item.geometry);
                            } else {
                              let filteredArray = [];
                              for (let l = 0; l < newArray1.length; l++) {
                                let newArrayReolerLength =
                                  newArray1[l].reoler.length;
                                filteredArray = [];
                                for (let m = 0; m < newArrayReolerLength; m++) {
                                  if (
                                    item.attributes.reol_id ===
                                      newArray1[l].reoler[m].reolId &&
                                    item.attributes.distance <
                                      newArray1[l].reoler[m].distance
                                  ) {
                                    totalCount =
                                      totalCount + item.attributes.hh;
                                    currentReoler.push(
                                      formatData(item.attributes)
                                    );
                                    featuresGeometry.push(item.geometry);
                                  } else {
                                    filteredArray.push(newArray1[l].reoler[m]);
                                  }
                                }
                                if (filteredArray.length > 0) {
                                  newArray1[l].reoler = filteredArray;
                                }
                              }
                            }
                          } else {
                            totalCount = totalCount + item.attributes.hh;

                            skipDuplicateRoute.push(item.attributes.reol_id);
                            currentReoler.push(formatData(item.attributes));
                            featuresGeometry.push(item.geometry);
                          }
                        }
                      } else {
                        if (skipDuplicateRoute.length > 0) {
                          if (
                            !skipDuplicateRoute.includes(
                              item.attributes.reol_id
                            )
                          ) {
                            skipDuplicateRoute.push(item.attributes.reol_id);

                            currentReoler.push(formatData(item.attributes));
                            featuresGeometry.push(item.geometry);
                          } else {
                            let filteredArray = [];
                            for (let l = 0; l < newArray1.length; l++) {
                              let newArrayReolerLength =
                                newArray1[l].reoler.length;
                              filteredArray = [];
                              for (let m = 0; m < newArrayReolerLength; m++) {
                                if (
                                  item.attributes.reol_id ===
                                    newArray1[l].reoler[m].reolId &&
                                  item.attributes.distance <
                                    newArray1[l].reoler[m].distance
                                ) {
                                  currentReoler.push(
                                    formatData(item.attributes)
                                  );
                                  featuresGeometry.push(item.geometry);
                                } else {
                                  filteredArray.push(newArray1[l].reoler[m]);
                                }
                              }
                              if (filteredArray.length > 0) {
                                newArray1[l].reoler = filteredArray;
                              }
                            }
                          }
                        } else {
                          // skipDuplicateRoute.push({
                          //   reol_id: item.attributes.reol_id,
                          //   distance: item.attributes.distance,
                          // });
                          skipDuplicateRoute.push(item.attributes.reol_id);
                          currentReoler.push(formatData(item.attributes));
                          featuresGeometry.push(item.geometry);
                        }
                      }
                    });
                    mapView.goTo(featuresGeometry);
                    results.features.forEach(function async(feature) {
                      Budruterresult.push(feature.attributes);
                    });
                    let utvalg = Utvalg();
                    utvalg.reoler = currentReoler;
                    utvalg.hasReservedReceivers = showReservedHouseHolds
                      ? true
                      : false;
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
                    utvalg.Business = Antall[1];
                    utvalg.ReservedHouseHolds = Antall[2];
                    utvalg.hush = Antall[0];
                    // let k = selectedKoummeIDs.map((element) => element).join(",");
                    // let newString = str + "Kommuner:" + k;
                    // Kjøretid 10 min og maksimalt antall 2000
                    let criteraString = "";
                    if (props.antallMinute !== 0) {
                      if (props.maxAntall) {
                        criteraString =
                          "Kjøretid " +
                          props.antallMinute +
                          " min og maksimalt antall " +
                          props.maxAntall +
                          " og unikt utvalg pr. adressepunkt";
                      } else {
                        criteraString =
                          props.antallMinute +
                          " min og unikt utvalg pr. adressepunkt";
                      }
                    }
                    if (props.antallKm !== 0) {
                      if (props.maxAntall) {
                        criteraString =
                          "Kjøreavstand " +
                          props.antallKm +
                          " km og maksimalt antall " +
                          props.maxAntall +
                          " og unikt utvalg pr. adressepunkt";
                      } else {
                        criteraString =
                          props.antallKm +
                          " km og unikt utvalg pr. adressepunkt";
                      }
                    }
                    let kjøreanalyseType = 8;
                    utvalg.criterias.push(
                      criterias(kjøreanalyseType, criteraString)
                    );
                    if (item.attributes.display) {
                      utvalg.name = item.attributes.display;
                    } else if (item.address) {
                      utvalg.name = item.address;
                    } else {
                      utvalg.name = item.attributes.Match_addr;
                    }
                    newArray1.push(JSON.parse(JSON.stringify(utvalg)));
                    if (kjAddressPoint.length === newArray1.length) {
                      setActivUtvalg({});
                      setActivUtvalg(utvalg);
                      setIsloading(false);
                    }

                    setProcessing(false);
                  }

                  setCreatedUtvalg(newArray1);
                  // To save these selection seprately
                  // if (newArray1.length > 0) {
                  //   showSelectionInWorklist(newArray1);
                  // }
                  setBudruterresult(Budruterresult);
                });
            }
          });
        });

        if (!isLoading) {
          nextClick();
        }
      } else {
        setProcessing(true);
        let MAP_URL = MapConfig.driveTimeAnalysisUrl;
        let MAP_URL_VALUE = "";
        let submitjobparams = {};

        let TempObjectValue = {
          displayFieldName: "",
          geometryType: "esriGeometryPoint",
          spatialReference: {
            wkid: 32633,
            latestWkid: 32633,
          },
          fields: [
            {
              name: "OBJECTID",
              type: "esriFieldTypeOID",
              alias: "OBJECTID",
            },
            {
              name: "Name",
              type: "esriFieldTypeString",
              alias: "Name",
              length: 254,
            },
          ],

          exceededTransferLimit: false,
        };

        let TempFeature = [];
        let kjAddressPointLength;
        if (props.maxAntall.length === 0) {
          kjAddressPointLength = kjAddressPoint.length;
        } else {
          kjAddressPointLength = 1;
        }
        for (let i = 0; i < kjAddressPointLength; i++) {
          if (kjAddressPoint[i].location !== undefined) {
            TempFeature.push({
              geometry: {
                x: kjAddressPoint[i].location.x,
                y: kjAddressPoint[i].location.y,
              },
              attributes: {
                OBJECTID: i + 1,
                Name: kjAddressPoint[i].attributes.Match_addr,
              },
            });
          } else {
            TempFeature.push({
              geometry: {
                x: kjAddressPoint[i].geometry.x,
                y: kjAddressPoint[i].geometry.y,
              },
              attributes: {
                OBJECTID: i + 1,
                Name: kjAddressPoint[i].attributes.Match_addr,
              },
            });
          }
        }

        TempObjectValue["features"] = TempFeature;

        if (props.antallKm !== 0) {
          MAP_URL_VALUE = MapConfig.driveTimeAnalysisUrl + "/DriveDistance";
          let distanceKM = props.antallKm * 1000;
          submitjobparams = {
            Distance: distanceKM,
            StartingPoints: TempObjectValue,
          };
        } else {
          MAP_URL_VALUE = MapConfig.driveTimeAnalysisUrl + "/DriveTime";
          submitjobparams = {
            Minutes: props.antallMinute,
            StartingPoints: TempObjectValue,
          };
        }

        let geoprocessor = new Geoprocessor({
          url: MAP_URL_VALUE,

          outSpatialReference: SpatialReference.WebMercator,
        });

        await submitJob(MAP_URL_VALUE, submitjobparams).then(async function (
          jobInfo
        ) {
          let jobid = jobInfo.jobId;
          setloadtext("behandling");

          let ReolID = [];
          await geoprocessor
            .waitForJobCompletion(jobid)
            .then(async function (result) {
              if (result.jobStatus == "job-succeeded") {
                setloadtext("suksess");
                setloadingdiv(false);
                await geoprocessor
                  .getResultData(result.jobId, "result")
                  .then(async function (Result) {
                    Result.value.features.map((item) => {
                      ReolID.push(item.attributes.reol_id);
                      //ReolID.push(item.attributes.REOL_ID);
                    });
                  });
              } else {
                setmelding(true);
                seterrormsg("noe gikk galt. Prøv igjen senere.");
              }
            });

          if (ReolID.length !== 0) {
            let k = ReolID.map((element) => "'" + element + "'").join(",");
            let reolsWhereClause = `reol_id in (${k})`;
            let BudruterUrl;

            let allLayersAndSublayers = mapView.map.allLayers.flatten(function (
              item
            ) {
              return item.layers || item.sublayers;
            });

            allLayersAndSublayers.items.forEach(function (item) {
              if (item.title === "Budruter") {
                BudruterUrl = item.url;
              }
            });

            //Get ObjectIDs
            const queryOIDs = new Query();
            queryOIDs.where = `${reolsWhereClause}`;
            let oids = await query.executeForIds(BudruterUrl, queryOIDs);

            //build query block for more than 2000 ObjectIDs
            let times = null;
            let quotient = Math.floor(oids.length / 2000);
            let remainder = oids.length % 2000;
            let startIndex = 0;
            let endIndex = 2000;
            let promise = [];

            if (quotient < 1) {
              times = 1;
              endIndex = oids.length;
            } else {
              if (remainder === 0) {
                times = quotient;
              } else {
                times = quotient + 1;
              }
            }

            for (let i = 0; i < times; i++) {
              if (i > 0) {
                startIndex = startIndex + 2000;
                endIndex = endIndex + 2000;
              }
              if (i === times - 1) {
                endIndex = oids.length;
              }

              let objectsIds = oids.slice(startIndex, endIndex);

              const queryResults = new Query();
              queryResults.outFields = MapConfig.budruterOutField;
              queryResults.where = "OBJECTID IN (" + objectsIds.join(",") + ")";
              queryResults.outSpatialReference = mapView.spatialReference;
              queryResults.returnGeometry = true;

              promise[i] = query.executeQueryJSON(BudruterUrl, queryResults);
            }

            Promise.all(promise).then((values) => {
              let resultsFeatures = [];
              for (let i = 0; i < values.length; i++) {
                for (let j = 0; j < values[i].features.length; j++) {
                  resultsFeatures.push(values[i].features[j]);
                }
              }

              if (resultsFeatures.length > 0) {
                let featuresGeometry = [];
                let currentReoler = [];
                let distances = [];

                //sort the results
                for (let i = 0; i < resultsFeatures.length; i++) {
                  distances.push({
                    index: i,
                    distance: geometryEngine.distance(
                      kjAddressPoint[0].location
                        ? kjAddressPoint[0].location
                        : kjAddressPoint[0].geometry,
                      resultsFeatures[i].geometry,
                      "kilometers"
                    ),
                  });
                }

                let sortedDistances = distances.sort(function (a, b) {
                  return a.distance - b.distance;
                });
                let sortedResults = [];

                for (let k = 0; k < sortedDistances.length; k++) {
                  sortedResults.push(resultsFeatures[sortedDistances[k].index]);
                }

                let totalCount = 0;
                sortedResults.map((item) => {
                  if (props.maxAntall) {
                    if (totalCount <= props.maxAntall) {
                      featuresGeometry.push(item.geometry);
                      totalCount = totalCount + item.attributes.hh;
                      currentReoler.push(formatData(item.attributes));
                    }
                  } else {
                    featuresGeometry.push(item.geometry);
                    currentReoler.push(formatData(item.attributes));
                  }
                });

                mapView.goTo(featuresGeometry);
                resultsFeatures.forEach(function async(item) {
                  Budruterresult.push(item.attributes);
                });

                let utvalg = Utvalg();
                utvalg.reoler = currentReoler;
                utvalg.hasReservedReceivers = showReservedHouseHolds
                  ? true
                  : false;
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
                utvalg.Business = Antall[1];
                utvalg.ReservedHouseHolds = Antall[2];
                utvalg.hush = Antall[0];
                let criteraString = "";
                if (props.antallMinute !== 0) {
                  if (props.maxAntall) {
                    criteraString =
                      "Kjøretid " +
                      props.antallMinute +
                      " min og maksimalt antall " +
                      props.maxAntall;
                  } else {
                    criteraString = props.antallMinute + " min";
                  }
                }
                if (props.antallKm !== 0) {
                  if (props.maxAntall) {
                    criteraString =
                      "Kjøreavstand " +
                      props.antallKm +
                      " km og maksimalt antall " +
                      props.maxAntall;
                  } else {
                    criteraString = props.antallKm + " km";
                  }
                }
                let kjøreanalyseType = 8;
                utvalg.criterias.push(
                  criterias(kjøreanalyseType, criteraString)
                );

                setActivUtvalg({});
                setActivUtvalg(utvalg);
                setProcessing(false);
                setIsloading(false);
              }

              setBudruterresult(Budruterresult);
            });
            nextClick();
          } else {
            await setCurrentStep(currentStep - 1);
            setProcessing(false);
            setIsloading(false);
            handleprevclick();

            Swal.fire({
              text: "Analysen er for stor. Redusere antall adressepunkter eller kjøretid/kjøreavstand.",
              confirmButtonColor: "#7bc144",
              confirmButtonText: "Lukk",
            });
          }
        });
      }

      if (Budruterresult.length > 0) {
        let total_antall = Budruterresult.map((item) => item.tot_anta).reduce(
          (prev, next) => prev + next
        );
        let household_res = Budruterresult.map((item) => item.hh_res).reduce(
          (prev, next) => prev + next
        );
        let household_value = Budruterresult.map((item) => item.hh).reduce(
          (prev, next) => prev + next
        );
      }
    };
    fetchdata().catch(console.error);
  }, []);

  const nextClick = () => {
    isLoading === false
      ? setCurrentStep(currentStep + 1)
      : setCurrentStep(currentStep);
  };
  const handleCancel = () => {
    setKjDisplay(false);
    setvalue(true);
  };
  const callback = (step) => {
    setCurrentStep(step - 1);
  };
  const handleprevclick = () => {
    props.parentCallback(props.currentStep);
  };
  return (
    <div>
      {currentStep === 2 ? (
        <div className="row col-12 m-0 p-0 mt-2 mb-2">
          {processing || isLoading ? (
            <div className="KjøreanalyseSpinnerDiv">
              <img className="mb-1 KjøreanalyseSpinner" src={spinner} />
            </div>
          ) : (
            nextClick()
          )}
          <div className="col-12">
            <div className="row">
              <div className="col-lg-4 col-md-4 col-sm-4 col-xs-4 p-0 pl-1">
                <input
                  type="submit"
                  id="uxBtForrige"
                  className="KSPU_button mt-1"
                  value="<< Forrige"
                  onClick={handleprevclick}
                  style={{
                    visibility: currentStep > 1 ? "visible" : "hidden",
                    text: "",
                  }}
                />
              </div>
              <div className="col-lg-4 col-md-4 col-sm-4 col-xs-4 p-0 _center">
                <input
                  type="submit"
                  id="uxBtnAvbryt"
                  className="KSPU_button mt-1"
                  value="Avbryt"
                  onClick={handleCancel}
                  style={{ text: "Avbryt" }}
                />
              </div>
              <div className="col-lg-4 col-md-4 col-sm-4 col-xs-4 p-0 pr-1 _flex-end">
                <input
                  type="submit"
                  id="uxBtnNeste"
                  className="KSPU_button mt-1"
                  value="Neste >>"
                  onClick={nextClick}
                  disabled
                  style={{
                    display: currentStep < 3 ? "block" : "none",
                  }}
                />
              </div>
            </div>
          </div>
        </div>
      ) : currentStep === 3 ? (
        <div>
          <KjøreanalyseComponentResultant
            currentStep={currentStep}
            parentCallback={callback}
            multiSelection={props.multiSelection}
            utvalgArray={createdUtvalg}
          />
        </div>
      ) : null}
    </div>
  );
}

export default KjøreanalyseComponentLoading;
