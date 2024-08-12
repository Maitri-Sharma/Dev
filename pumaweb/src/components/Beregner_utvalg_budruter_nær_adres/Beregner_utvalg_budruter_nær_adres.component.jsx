import React, { useEffect, useState, useRef, useContext } from "react";
import "./Beregner_utvalg_budruter_nær_adres.styles.scss";
import { KundeWebContext, MainPageContext } from "../../context/Context";

import Geoprocessor from "@arcgis/core/tasks/Geoprocessor";
import SpatialReference from "@arcgis/core/geometry/SpatialReference";
import { submitJob } from "@arcgis/core/rest/geoprocessor";
import JobInfo from "@arcgis/core/rest/support/JobInfo";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";
import * as geometryEngine from "@arcgis/core/geometry/geometryEngine";
import Extent from "@arcgis/core/geometry/Extent";
import FeatureLayer from "@arcgis/core/layers/FeatureLayer";

import { Result } from "antd";

import loadingImage from "../../assets/images/callbackActivityIndicator.gif";

import {
  Utvalg,
  NewUtvalgName,
  criterias_KW,
  getAntall_KW,
  formatData,
  getAntallUtvalg,
} from "../KspuConfig";
import { MapConfig } from "../../config/mapconfig";

function Burdruter_velg_KW() {
  const [SelectedDataSet, setSelectedDataSet] = useState([]);
  const { householdcheckbox, sethouseholdcheckbox } =
    useContext(KundeWebContext);
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);
  const { HouseholdSum, setHouseholdSum } = useContext(KundeWebContext);
  const { BusinessSum, setBusinessSum } = useContext(KundeWebContext);
  const { SelectedItemCheckBox_Budruter, setSelectedItemCheckBox_Budruter } =
    useContext(KundeWebContext);
  const { HouseholdSum_budruter, setHouseholdSum_budruter } =
    useContext(KundeWebContext);
  const [Budruterresult, setBudruterresult] = useState([]);
  const { mapView } = useContext(MainPageContext);
  const { Page, setPage } = useContext(KundeWebContext);
  const { Page_P, setPage_P } = useContext(KundeWebContext);
  const [melding, setmelding] = useState(false);
  const [errormsg, seterrormsg] = useState("");
  const [loadtext, setloadtext] = useState("Initialiserer");
  const [loadingdiv, setloadingdiv] = useState(true);
  const [loading, setloading] = useState(true);
  const { BudruterTimeSelection, setBudruterTimeSelection } =
    useContext(KundeWebContext);
  const { BudruterDistanceSelection, setBudruterDistanceSelection } =
    useContext(KundeWebContext);
  const { BudruterAntallSelection, setBudruterAntallSelection } =
    useContext(KundeWebContext);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  const { showBusiness, setShowBusiness } = useContext(KundeWebContext);
  const { showReservedHouseHolds, setShowReservedHouseHolds } =
    useContext(KundeWebContext);
  const { custNos, setcustNos } = useContext(KundeWebContext);
  const { avtaleData, setavtaleData } = useContext(KundeWebContext);
  const { username_kw, setusername_kw } = useContext(KundeWebContext);
  const { BudruterSelectedName, setBudruterSelectedName } =
    useContext(KundeWebContext);

  let currentReoler = [];
  let Antall = [];

  useEffect(() => {
    const fetchdata = async () => {
      if (BudruterDistanceSelection !== "" || BudruterTimeSelection != "") {
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
        for (let i = 0; i < SelectedItemCheckBox_Budruter.length; i++) {
          TempFeature.push({
            geometry: {
              x: SelectedItemCheckBox_Budruter[i].location
                ? SelectedItemCheckBox_Budruter[i].location.x
                : SelectedItemCheckBox_Budruter[i].geometry.x,
              y: SelectedItemCheckBox_Budruter[i].location
                ? SelectedItemCheckBox_Budruter[i].location.y
                : SelectedItemCheckBox_Budruter[i].geometry.y,
            },
            attributes: {
              OBJECTID: i + 1,
              Name: SelectedItemCheckBox_Budruter[i].attributes
                ? SelectedItemCheckBox_Budruter[i].attributes.Match_addr
                : SelectedItemCheckBox_Budruter[i].feature.attributes
                  .Match_addr,
            },
          });
        }

        TempObjectValue["features"] = TempFeature;

        if (BudruterDistanceSelection !== "") {
          MAP_URL_VALUE = MapConfig.driveTimeAnalysisUrl + "/DriveDistance";
          submitjobparams = {
            Distance: BudruterDistanceSelection * 1000,
            StartingPoints: TempObjectValue,
          };
        } else if (BudruterTimeSelection !== "") {
          MAP_URL_VALUE = MapConfig.driveTimeAnalysisUrl + "/DriveTime";
          submitjobparams = {
            Minutes: BudruterTimeSelection,
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
          setloadtext("Prosesserer adressepunktet");

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
          let sql_geography = `reol_id in (${k})`;
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

            queryObject.where = `${sql_geography}`;
            queryObject.returnGeometry = true;
            queryObject.outFields = MapConfig.budruterOutField;

            await query
              .executeQueryJSON(BudruterUrl, queryObject)
              .then(function (results) {
                if (results.features.length > 0) {
                  let featuresGeometry = [];
                  let selectedSymbol = {
                    type: "simple-fill", // autocasts as new SimpleFillSymbol()
                    color: [237, 54, 21, 0.25],
                    style: "solid",
                    outline: {
                      // autocasts as new SimpleLineSymbol()
                      color: [237, 54, 21],
                      width: 0.75,
                    },
                  };
                  let totalCount = 0;
                  //mapView.graphics.removeAll();
                  results.features.map((item) => {
                    featuresGeometry.push(item.geometry);
                    totalCount = totalCount + item.attributes.hh;
                    if (BudruterAntallSelection !== "") {
                      if (totalCount <= BudruterAntallSelection) {
                        Budruterresult.push(item.attributes);
                        currentReoler.push(formatData(item.attributes));
                      }
                    } else {
                      Budruterresult.push(item.attributes);
                      currentReoler.push(formatData(item.attributes));
                    }
                    let graphic = new Graphic(
                      item.geometry,
                      selectedSymbol,
                      item.attributes
                    );
                    mapView.graphics.add(graphic);
                  });

                  mapView.goTo(featuresGeometry);
                }
                setSelectedDataSet(currentReoler);
                setBudruterresult(Budruterresult);
              });
          }
        });
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
          await createUtvalgObject(currentReoler, "Budruter: ", 200);
          setHouseholdSum_budruter(household_value);
          setPage("LagutvalgClick");
          setPage_P("Burdruter_velg_KW");
        }
      }

      if (BudruterAntallSelection !== "") {
        let antAll = BudruterAntallSelection;
        //mapView.graphics.removeAll();
        // let distance = BudruterDistanceSelection;
        runAnalysis(antAll);
      }
    };

    fetchdata().catch(console.error);
  }, []);

  const goback = () => {
    setPage("Budrutervelg");
  };
  const GotoMain = () => {
    setPage("");
  };

  const runAnalysis = (antall) => {
    let distance = 60;
    let addresPointGeometry = [];
    SelectedItemCheckBox_Budruter.map((item) => {
      addresPointGeometry.push(
        item.location ? item.location : item.geometry
      );
    });

    const buffer = geometryEngine.buffer(
      addresPointGeometry,
      distance,
      "kilometers",
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

    let unionBufferGeometry = geometryEngine.union(buffer);

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
          geometry: unionBufferGeometry,
          outFields: ["*"],
          spatialRelationship: "intersects",
          returnGeometry: true,
          outSpatialReference: mapView.SpatialReference,
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

        if (results.length > 0) {
          let inititalCount = 0;
          let falg = false;
          if (results.length > 0) {
            let featuresGeometry = [];
            let currentReoler = [];
            var graphic;
            let geo = [];
            let uniqueReols = [];

            //sort the results
            for (let i = 0; i < addresPointGeometry?.length; i++) {
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
              let graphicItems = [];
              sortedResults.map((item, index) => {
                if (totalCount <= antall) {
                  totalCount = totalCount + item.attributes.hh;
                  featuresGeometry.push(item.geometry);
                  Budruterresult.push(item.attributes);
                  currentReoler.push(formatData(item.attributes));
                  falg = true;
                  if (!uniqueReols?.includes(item?.attributes?.reol_id)) {
                    graphicItems.push(item);
                    uniqueReols.push(item.attributes.reol_id);
                  }
                } else {
                  if (inititalCount <= antall && !falg) {
                    if (
                      inititalCount <= Number(antall) &&
                      falg == false &&
                      item.attributes.hh <= Number(antall)
                    ) {
                      featuresGeometry.push(item.geometry);
                      inititalCount += item.attributes.hh;
                      Budruterresult.push(item.attributes);
                      currentReoler.push(formatData(item.attributes));
                      if (!uniqueReols?.includes(item?.attributes?.reol_id)) {
                        graphicItems.push(item);
                        uniqueReols.push(item.attributes.reol_id);
                      }
                    }
                  }
                }
              });

              graphicItems?.map((item) => {
                var geometry = item.geometry;
                if (geometry) {
                  if (geometry.type === "polygon") {
                    var geometryvalue = item.geometry;
                    if (geometryvalue) {
                      if (geometryvalue.type === "polygon") {
                        var symbol = {
                          type: "simple-fill",
                          color: [237, 54, 21, 0.25],
                          style: "solid",
                          outline: {
                            color: [237, 54, 21],
                            width: 0.75,
                          },
                        };
                      }
                      graphic = new Graphic(
                        geometryvalue,
                        symbol,
                        item.attributes
                      );
                    }
                  }
                }
                mapView.graphics.add(graphic);
              });
            }

            featuresGeometry.map((item) => {
              geo.push(item);
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
            setBudruterresult(Budruterresult);
            let reolIds = currentReoler.map((o) => o.reolId);
            let filteredcurrentReoler = currentReoler.filter(
              ({ reolId }, index) => !reolIds.includes(reolId, index + 1)
            );
            createUtvalgObject(filteredcurrentReoler, "Budruter: ", 200);
          }

          setPage_P("Burdruter_velg_KW");

          setPage("LagutvalgClick");
        }
      });
    });

    featureLayer.load();
  };

  const createUtvalgObject = async (selectedDataSet, criteriaType, key) => {
    Antall = getAntallUtvalg(selectedDataSet);
    var A = Utvalg();

    let receivers = [];

    if (businesscheckbox) {
      if (A.receivers?.length) {
        receivers = A.receivers.filter((i) => {
          return i.receiverId !== 4;
        });
        A.receivers = receivers;
      }
      A.receivers.push({ receiverId: 4, selected: true });
    } else {
      receivers = A.receivers.filter((i) => {
        return i.receiverId !== 4;
      });
      A.receivers = receivers;
    }

    if (householdcheckbox) {
      if (A.receivers?.length) {
        receivers = A.receivers.filter((i) => {
          return i.receiverId !== 1;
        });
        A.receivers = receivers;
      }
      A.receivers.push({ receiverId: 1, selected: true });
    } else {
      receivers = A.receivers.filter((i) => {
        return i.receiverId !== 1;
      });
      A.receivers = receivers;
    }


    A.hasReservedReceivers = false;
    A.name = NewUtvalgName();
    if (householdcheckbox && !businesscheckbox) {
      A.totalAntall = Antall[0];
    }
    if (businesscheckbox && !householdcheckbox) {
      A.totalAntall = Antall[1];
    }
    if (householdcheckbox && businesscheckbox) {
      A.totalAntall = Antall[1] + Antall[0];
    }

    // A.receivers = [{ ReceiverId: 1, selected: true }];
    // if (showBusiness) A.receivers.push({ ReceiverId: 4, selected: true });
    // if (showReservedHouseHolds)
    //   A.receivers.push({ ReceiverId: 5, selected: true });
    A.modifications = [];
    A.reoler = selectedDataSet;
    A.Business = Antall[1];
    A.ReservedHouseHolds = Antall[2];
    A.hush = Antall[0];
    A.criterias.push(criterias_KW(criteriaType, key));
    A.Antall = Antall;
    A.ordreReferanse = "";
    A.oldReolMapName = "";
    A.kundeNavn = username_kw;
    A.kundeNummer = custNos;
    A.avtalenummer = avtaleData;

    await setutvalgapiobject(A);
  };

  return (
    <div className={"col-5 pt-2 pt-2 "}>
      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <span className="title">Velg budruter nær adresse</span>
      </div>
      <img
        src={loadingImage}
        style={{
          width: "20px",
          height: "20px",
          display: !loading ? "none" : "block",
          position: "absolute",
          top: "170px",
          left: "250px",
          zindex: 100,
        }}
      />

      {melding ? <br /> : null}
      {melding ? (
        <span className=" sok-Alert-text pl-1">{errormsg}</span>
      ) : null}
      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <p
          id="uxGeografiAnalyse_uxHeader_lblDesc"
          className="lblAnalysisHeaderDesc"
        >
          {" "}
          Velg budruter ved å oppgi en eller flere adresser og begrense etter
          kjøreavstand, kjøretid, antall mottakere eller at adressene ligger på
          ruten. Beregner utvalg... <p></p>
          <br />
          {loadtext == "suksess" ? (
            <div className="flex">
              <span className="mytext"> {loadtext} </span>&nbsp;&nbsp;&nbsp;
            </div>
          ) : (
            <div className="flex">
              <span className="mytext"> {loadtext} </span>&nbsp;
              <span className="mytext">{BudruterSelectedName} </span>
              &nbsp;&nbsp;&nbsp;
              {loadingdiv ? <div className="dot-elastic"></div> : null}
            </div>
          )}
          {/* <div className="row">
            <div className="dot-elastic" style={{ marginLeft: "25%" }}></div>
            <div style={{ marginRight: "20%" }}>Initialisere &nbsp;&nbsp; </div>
          </div> */}
          <p></p>
        </p>
      </div>
      <br />
      <br />

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
            onClick={""}
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

export default Burdruter_velg_KW;
