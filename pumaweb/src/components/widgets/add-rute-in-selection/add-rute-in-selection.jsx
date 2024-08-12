import React, { useContext, useState, useEffect } from "react";

import SketchViewModel from "@arcgis/core/widgets/Sketch/SketchViewModel";
import * as geometryEngineAsync from "@arcgis/core/geometry/geometryEngineAsync";
import Graphic from "@arcgis/core/Graphic";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";

import {
  Reol,
  getAntallUtvalg,
  NewUtvalgName,
  Utvalg,
  criterias,
  criterias_KW,
} from "../../KspuConfig";
import { KSPUContext, UtvalgContext } from "../../../context/Context.js";
import { groupBy } from "../../../Data";
import { KundeWebContext } from "../../../context/Context.js";
import { MainPageContext } from "../../../context/Context.js";

import { MapConfig } from "../../../config/mapconfig";
import Spinner from "../../../components/spinner/spinner.component";
import { CurrentDate } from "../../../common/Functions";
function formatData(reolObj) {
  var r = Reol();
  r.name = reolObj.reolnavn;
  r.reolNumber = reolObj.reolnr;
  r.description = reolObj.beskrivelse;
  r.comment = reolObj.kommentar;
  r.descriptiveName = reolObj.beskrivelse + " (" + reolObj.reol_id + ")";
  r.reolId = parseInt(reolObj.reol_id);
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

export function AddRuteInSelection(props) {
  const { mapView, setMapView } = useContext(MainPageContext);
  const { resultData, setResultData, isWidgetActive, setIsWidgetActive } =
    useContext(KSPUContext);
  const { activUtvalg, setActivUtvalg } = useContext(KSPUContext);
  const [loading, setloading] = useState(false);
  const {
    showBusiness,
    setShowBusiness,
    showHousehold,
    setShowHousehold,
    setDemografieDisplay,
    setSegmenterDisplay,
    setAddresslisteDisplay,
    setGeografiDisplay,
    setKjDisplay,
    setAdresDisplay,
    setvalue,
    setutvalglistcheck,
    SelectionUpdate,
    setSelectionUpdate,
    checkedList,
    setCheckedList,
    maintainUnsavedRute,
    setMaintainUnsavedRute,
  } = useContext(KSPUContext);
  const {
    showReservedHouseHolds,
    setAktivDisplay,
    setissave,
    setRuteDisplay,
    setSave,
  } = useContext(KSPUContext);
  const { searchURL } = useContext(KSPUContext);
  const { ActiveMapButton, setActiveMapButton } = useContext(MainPageContext);

  let Antall = [];
  // setSelectionUpdate(true);
  const createUtvalgObject = (
    selectedDataSet,
    key,
    criteriaType,
    FromDemografie
  ) => {
    var a = Utvalg();
    setSelectionUpdate(true);
    a.reoler = selectedDataSet;
    Antall = getAntallUtvalg(selectedDataSet);

    a.hasReservedReceivers = showReservedHouseHolds ? true : false;
    a.name = NewUtvalgName();
    a.totalAntall =
      Antall[0] +
      (showBusiness ? Antall[1] : 0) +
      (showReservedHouseHolds ? Antall[2] : 0);
    a.receivers = [{ ReceiverId: 1, selected: true }];
    if (showBusiness) a.receivers.push({ ReceiverId: 4, selected: true });
    if (showReservedHouseHolds)
      a.receivers.push({ ReceiverId: 5, selected: true });
    a.modifications = [];

    a.Business = Antall[1];
    a.ReservedHouseHolds = Antall[2];
    a.hush = Antall[0];
    a.criterias.push(criterias(criteriaType, key));
    a.Antall = Antall;
    setActivUtvalg(a);
  };

  useEffect(() => {
    if (Object.keys(activUtvalg).length === 0) {
      setShowHousehold(true);
    }
  }, []);

  const gettitle = (url) => {
    if (url.includes("Fylke")) {
      return 0;
    } else if (url.includes("Kommune")) {
      return 1;
    } else if (url.includes("Team")) {
      return 2;
    } else {
      return 0;
    }
  };

  const selectFeatures = async (geometry, view) => {
    let BudruterUrl;
    let toBeSavedRuteDetails = {};

    let allLayersAndSublayers = view.map.allLayers.flatten(function (item) {
      return item.layers || item.sublayers;
    });

    allLayersAndSublayers.items.forEach(function (item) {
      if (item.title === "Budruter") {
        BudruterUrl = item.url;
      }
    });

    // create a query and set its geometry parameter to the
    // rectangle that was drawn on the view
    let queryObject = new Query({
      geometry: geometry,
      outFields: MapConfig.budruterOutField,
      spatialRelationship: "intersects", // Relationship operation to apply
      returnGeometry: true,
      outSpatialReference: { wkid: 25833 },
    });

    // query graphics from the csv layer view. Geometry set for the query
    // can be polygon for point features and only intersecting geometries are returned

    await query
      .executeQueryJSON(BudruterUrl, queryObject)
      .then(async (results) => {
        let currentReoler = activUtvalg.reoler;
        setIsWidgetActive(false);
        results.features.map((item) => {
          var geometry = item.geometry;
          if (geometry) {
            if (geometry.type === "polygon") {
              let symbol = {
                type: "simple-fill", // autocasts as new SimpleFillSymbol()
                color: [237, 54, 21, 0.25],
                style: "solid",
                outline: {
                  // autocasts as new SimpleLineSymbol()
                  color: [237, 54, 21],
                  width: 0.75,
                },
              };

              let reoldIDsTransparentRed = [];
              let reolIDsDoubleCovColor = [];
              let allDisplayedReolID = [];

              mapView.graphics.items.map((graphicElement) => {
                if (
                  graphicElement.attributes !== undefined &&
                  graphicElement.attributes !== null
                ) {
                  if (graphicElement.attributes.reol_id !== undefined) {
                    allDisplayedReolID.push(graphicElement.attributes.reol_id);
                    if (
                      Object.entries(graphicElement.symbol.color).toString() ===
                      Object.entries({
                        r: 237,
                        g: 54,
                        b: 21,
                        a: 0.25,
                      }).toString()
                    ) {
                      reoldIDsTransparentRed.push(
                        graphicElement.attributes.reol_id
                      );
                    } else if (
                      Object.entries(graphicElement.symbol.color).toString() ===
                        Object.entries({
                          r: 0,
                          g: 255,
                          b: 0,
                          a: 0.8,
                        }).toString() ||
                      Object.entries(graphicElement.symbol.color).toString() ===
                        Object.entries({
                          r: 255,
                          g: 255,
                          b: 0,
                          a: 1,
                        }).toString()
                    ) {
                      reolIDsDoubleCovColor.push(
                        graphicElement.attributes.reol_id
                      );
                    } else {
                      // do nothing
                    }
                  }
                }
              });

              if (!reoldIDsTransparentRed.includes(item.attributes.reol_id)) {
                if (!reolIDsDoubleCovColor.includes(item.attributes.reol_id)) {
                  if (allDisplayedReolID.includes(item.attributes.reol_id)) {
                    symbol = {
                      type: "simple-fill", // autocasts as new SimpleFillSymbol()
                      color: [0, 255, 0, 0.8],
                      style: "solid",
                      outline: {
                        // autocasts as new SimpleLineSymbol()
                        color: [237, 54, 21],
                        width: 0.75,
                      },
                    };
                  }

                  currentReoler.push(formatData(item.attributes));

                  var graphic = new Graphic(geometry, symbol, item.attributes);
                  // make sure to remmove previous highlighted feature
                  mapView.graphics.add(graphic);
                }
              }
            }
          }
        });

        setloading(false);

        let utvalg = activUtvalg;
        // setSelectionUpdate(true);
        utvalg.reoler = currentReoler;
        utvalg.hasReservedReceivers = showReservedHouseHolds ? true : false;
        // utvalg.criterias.push(criterias(500, "Valgt enkeltvis"));
        //utvalg.name = NewUtvalgName();
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
        // utvalg.hush = Antall[0];
        // utvalg.criterias.push(criterias(1, ""));
        let fastantallsanalyseType = 10;
        let criteraString = "Valgt enkeltvis";
        utvalg.criterias.push(criterias(fastantallsanalyseType, criteraString));
        // setSelectionUpdate(true);
        // setActivUtvalg({});
        // await setActivUtvalg(utvalg);
        await setActivUtvalg((oldState) => ({
          ...oldState,
          Antall: utvalg.Antall,
          reoler: currentReoler,
          totalAntall: utvalg.totalAntall,
          receivers: utvalg.receivers,
          Business: utvalg.Business,
          ReservedHouseHolds: utvalg.ReservedHouseHolds,
          hush: utvalg.hush,
          criterias: utvalg.criterias,
        }));

        setSelectionUpdate(true);
        setMapView(mapView);
        //setActivUtvalg({});
        setActivUtvalg(utvalg);

        if (checkedList?.length > 0) {
          checkedList.map((item, x) => {
            if (
              JSON.parse(item?.utvalgId) === JSON.parse(activUtvalg?.utvalgId)
            ) {
              checkedList[x].reoler = activUtvalg.reoler;
              checkedList[x].totalAntall = activUtvalg.totalAntall;
              checkedList[x].antallBeforeRecreation =
                activUtvalg.antallBeforeRecreation;
            }
          });
          setCheckedList([...checkedList]);
        }

        let toBeSavedRute;
        if (maintainUnsavedRute?.length === 0) {
          toBeSavedRute = maintainUnsavedRute;
          toBeSavedRuteDetails = {
            selectionID: activUtvalg.utvalgId,
            activeUtval: utvalg,
          };
          toBeSavedRute.push(toBeSavedRuteDetails);
          setMaintainUnsavedRute(toBeSavedRute);
        } else {
          let selectionIDs = [];
          maintainUnsavedRute.forEach(function (item) {
            selectionIDs.push(item.selectionID);
          });

          if (selectionIDs.includes(activUtvalg.utvalgId)) {
            maintainUnsavedRute.forEach(function (item) {
              if (item.selectionID === activUtvalg.utvalgId) {
                item.activeUtval.reoler = activUtvalg.reoler;
              }
            });
          } else {
            toBeSavedRute = maintainUnsavedRute;
            toBeSavedRuteDetails = {
              selectionID: activUtvalg.utvalgId,
              activeUtval: utvalg,
            };
            toBeSavedRute.push(toBeSavedRuteDetails);
            setMaintainUnsavedRute(toBeSavedRute);
          }
        }

        mapView.popup.close();
        handleAddRuteInSelection();
      })
      .catch((error) => {
        console.log("error occured while calling feature layer", error);
      });
  };
  const handleAddRuteInSelection = () => {
    setActiveMapButton("addRouteInSelection");
    const view = mapView;
    const graphicslayer = props.graphicLayer;

    const sketchViewModel = new SketchViewModel({
      view: view,
      layer: graphicslayer,
    });
    view.popup.close();

    sketchViewModel.create("rectangle", { mode: "freehand" });

    sketchViewModel.on("create", async (event) => {
      if (event.state === "complete") {
        // this polygon will be used to query features that intersect it
        const geometries = graphicslayer.graphics.map(function (graphic) {
          return graphic.geometry;
        });
        const queryGeometry = await geometryEngineAsync.union(
          geometries.toArray()
        );
        graphicslayer.removeAll();
        setloading(true);
        await selectFeatures(queryGeometry, view);
      }
    });
  };

  return (
    <div>
      <div>{loading ? <Spinner /> : null}</div>
      <div id="addRuteInSelectionDiv">
        {ActiveMapButton === "addRouteInSelection" ? (
          <button
            className="esri-widget--button esri-interactive esri-widget add-rute-in-selection-icon focus"
            id="addRuteInSelectionButton"
            type="button"
            title="Legg til rute i utvalg"
            disabled={Object.keys(activUtvalg).length > 0 ? false : true}
            onClick={handleAddRuteInSelection}
          ></button>
        ) : (
          <button
            className="esri-widget--button esri-interactive esri-widget add-rute-in-selection-icon"
            id="addRuteInSelectionButton"
            type="button"
            title="Legg til rute i utvalg"
            disabled={Object.keys(activUtvalg).length > 0 ? false : true}
            onClick={handleAddRuteInSelection}
          ></button>
        )}
      </div>
    </div>
  );
}
// ================add rute in map for kundeweb started============///
export function AddRuteInSelectionKw(props) {
  const { mapView, setMapView } = useContext(MainPageContext);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  const [loading, setloading] = useState(false);
  const { householdcheckbox, sethouseholdcheckbox } =
    useContext(KundeWebContext);
  const { Page, setPage } = useContext(KundeWebContext);
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);
  const [utvalgObject, setutvalgObject] = useState({});
  const { custNos, setcustNos } = useContext(KundeWebContext);
  const { avtaleData, setavtaleData } = useContext(KundeWebContext);
  const { username_kw, setusername_kw, showReservedHouseHolds, showBusiness } =
    useContext(KundeWebContext);
  const { ActiveMapButton, setActiveMapButton } = useContext(MainPageContext);
  const { selectedrecord_s, setselectedrecord_s } = useContext(KundeWebContext);
  const { selectedRowKeys, setSelectedRowKeys } =
    React.useContext(KundeWebContext);
  const { routeUpdateEnabled, setRouteUpdateEnabled } =
    useContext(KundeWebContext);
  const { BusinessSum, setBusinessSum } = useContext(KundeWebContext);
  const { HouseholdSum, setHouseholdSum } = useContext(KundeWebContext);
  const { selectionUpdateKW, setSelectionUpdateKW } =
    useContext(KundeWebContext);

  let Antall = [];

  const selectFeatures = async (geometry, view) => {
    let BudruterUrl;

    let allLayersAndSublayers = view.map.allLayers.flatten(function (item) {
      return item.layers || item.sublayers;
    });

    allLayersAndSublayers.items.forEach(function (item) {
      if (item.title === "Budruter") {
        BudruterUrl = item.url;
      }
    });

    // create a query and set its geometry parameter to the
    // rectangle that was drawn on the view
    let queryObject = new Query({
      geometry: geometry,
      outFields: MapConfig.budruterOutField,
      spatialRelationship: "intersects", // Relationship operation to apply
      returnGeometry: true,
      outSpatialReference: { wkid: 25833 },
    });

    // query graphics from the csv layer view. Geometry set for the query
    // can be polygon for point features and only intersecting geometries are returned

    await query
      .executeQueryJSON(BudruterUrl, queryObject)
      .then(async (results) => {
        let currentReoler = [];
        if (
          Object.keys(utvalgapiobject).length !== 0 &&
          utvalgapiobject !== undefined &&
          utvalgapiobject.reoler !== undefined
        ) {
          currentReoler = utvalgapiobject.reoler;
        }

        results.features.map((item) => {
          var geometry = item.geometry;
          if (geometry) {
            if (geometry.type === "polygon") {
              let symbol = {
                type: "simple-fill", // autocasts as new SimpleFillSymbol()
                color: [237, 54, 21, 0.25],
                style: "solid",
                outline: {
                  // autocasts as new SimpleLineSymbol()
                  color: [237, 54, 21],
                  width: 0.75,
                },
              };
              let reoldIDsTransparentRed = [];
              let reolIDsDoubleCovColor = [];
              let allDisplayedReolID = [];
              mapView.graphics.items.map((graphicElement) => {
                if (
                  graphicElement.attributes !== undefined &&
                  graphicElement.attributes !== null
                ) {
                  if (graphicElement.attributes.reol_id !== undefined) {
                    allDisplayedReolID.push(graphicElement.attributes.reol_id);
                    if (
                      Object.entries(graphicElement.symbol.color).toString() ===
                      Object.entries({
                        r: 237,
                        g: 54,
                        b: 21,
                        a: 0.25,
                      }).toString()
                    ) {
                      reoldIDsTransparentRed.push(
                        graphicElement.attributes.reol_id
                      );
                    } else if (
                      Object.entries(graphicElement.symbol.color).toString() ===
                        Object.entries({
                          r: 0,
                          g: 255,
                          b: 0,
                          a: 0.8,
                        }).toString() ||
                      Object.entries(graphicElement.symbol.color).toString() ===
                        Object.entries({
                          r: 255,
                          g: 255,
                          b: 0,
                          a: 1,
                        }).toString()
                    ) {
                      reolIDsDoubleCovColor.push(
                        graphicElement.attributes.reol_id
                      );
                    } else {
                      // do nothing
                    }
                  }
                }
              });
              if (!reoldIDsTransparentRed.includes(item.attributes.reol_id)) {
                if (reolIDsDoubleCovColor.includes(item.attributes.reol_id)) {
                  if (allDisplayedReolID.includes(item.attributes.reol_id)) {
                    symbol = {
                      type: "simple-fill", // autocasts as new SimpleFillSymbol()
                      color: [0, 255, 0, 0.8],
                      style: "solid",
                      outline: {
                        // autocasts as new SimpleLineSymbol()
                        color: [237, 54, 21],
                        width: 0.75,
                      },
                    };
                  }
                }
                currentReoler.push(formatData(item.attributes));
                var graphic = new Graphic(geometry, symbol, item.attributes);
                // make sure to remmove previous highlighted feature
                mapView.graphics.add(graphic);
              }
            }
          }
        });

        //updating tablenew component
        let reols = currentReoler.map((o) => o.reolId.toString());
        setSelectedRowKeys([...reols]);

        let utvalg = Utvalg();
        utvalg.reoler = currentReoler;
        utvalg.hasReservedReceivers = false;
        utvalg.name = NewUtvalgName();
        let Antall = getAntallUtvalg(currentReoler);
        setHouseholdSum(Antall[0]);
        setBusinessSum(Antall[1]);
        utvalg.Antall = Antall;
        utvalg.totalAntall = Antall[0] + (businesscheckbox ? Antall[1] : 0);
        utvalg.Business = Antall[1];
        utvalg.ReservedHouseHolds = Antall[2];
        utvalg.receivers = [{ receiverId: 1, selected: true }];
        if (businesscheckbox)
          utvalg.receivers.push({ receiverId: 4, selected: true });
        utvalg.modifications = [];
        utvalg.modifications.push({
          modificationId: Math.floor(100000 + Math.random() * 900000),
          userId: "Internbruker",
          modificationTime: CurrentDate(),
          listId: 0,
        });
        let fastantallsanalyseType = 10;
        let criteraString = "Valgt enkeltvis";
        utvalg.criterias.push(criterias(fastantallsanalyseType, criteraString));
        // setutvalgapiobject(utvalgapiobject);
        setutvalgapiobject((oldState) => ({
          ...oldState,
          Antall: utvalg.Antall,
          reoler: currentReoler,
          totalAntall: utvalg.totalAntall,
          receivers: utvalg.receivers,
          Business: utvalg.Business,
          ReservedHouseHolds: utvalg.ReservedHouseHolds,
          criterias: utvalg.criterias,
          modifications: utvalg.modifications,
        }));
        setSelectionUpdateKW(true);
        // setutvalgapiobject(() => utvalg);
        // setPage("LagutvalgClick");

        setMapView(view);
        setutvalgObject(utvalg);
        setloading(false);
        handleAddRuteInSelectionKW();
      })
      .catch((err) => {
        console.log("error occured while querying", err);
      });
  };

  const handleAddRuteInSelectionKW = () => {
    setActiveMapButton("addRouteInSelection");
    const view = props.view;
    const graphicslayer = props.graphicLayer;

    const sketchViewModel = new SketchViewModel({
      view: view,
      layer: graphicslayer,
    });
    view.popup.close();
    sketchViewModel.create("rectangle", { mode: "freehand" });

    sketchViewModel.on("create", async (event) => {
      if (event.state === "complete") {
        // this polygon will be used to query features that intersect it
        const geometries = graphicslayer.graphics.map(function (graphic) {
          return graphic.geometry;
        });
        const queryGeometry = await geometryEngineAsync.union(
          geometries.toArray()
        );
        graphicslayer.removeAll();
        setloading(true);
        await selectFeatures(queryGeometry, view);
      }
    });
  };

  return (
    <div>
      <div>{loading ? <Spinner /> : null}</div>
      <div id="addRuteInSelectionDiv">
        {ActiveMapButton === "addRouteInSelection" ? (
          <button
            className="esri-widget--button esri-interactive esri-widget add-rute-in-selection-icon focus"
            id="addRuteInSelectionButton"
            type="button"
            title="Legg til rute i utvalg"
            disabled={
              Object.keys(utvalgapiobject).length > 0 || routeUpdateEnabled
                ? false
                : true
            }
            onClick={() => {
              handleAddRuteInSelectionKW();
            }}
          ></button>
        ) : (
          <button
            className="esri-widget--button esri-interactive esri-widget add-rute-in-selection-icon"
            id="addRuteInSelectionButton"
            type="button"
            title="Legg til rute i utvalg"
            disabled={
              Object.keys(utvalgapiobject).length > 0 || routeUpdateEnabled
                ? false
                : true
            }
            onClick={handleAddRuteInSelectionKW}
          ></button>
        )}
      </div>
    </div>
  );
}
