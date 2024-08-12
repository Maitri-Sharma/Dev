import React, { useContext, useState, useEffect } from "react";

import SketchViewModel from "@arcgis/core/widgets/Sketch/SketchViewModel";
import * as geometryEngineAsync from "@arcgis/core/geometry/geometryEngineAsync";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";

import { KSPUContext } from "../../../context/Context.js";
import { groupBy } from "../../../Data";
import { getAntallUtvalg } from "../../KspuConfig";
import { KundeWebContext } from "../../../context/Context.js";
import { MainPageContext } from "../../../context/Context.js";
import { MapConfig } from "../../../config/mapconfig";
import Spinner from "../../../components/spinner/spinner.component";

export function RemoveRuteInSelection(props) {
  const { mapView, setMapView } = useContext(MainPageContext);
  const [loading, setloading] = useState(false);
  const {
    resultData,
    setResultData,
    isWidgetActive,
    setIsWidgetActive,
    setAktivDisplay,
    setSelectionUpdate,
    checkedList,
    setCheckedList,
    maintainUnsavedRute,
    setMaintainUnsavedRute,
  } = useContext(KSPUContext);
  const { activUtvalg, setActivUtvalg } = useContext(KSPUContext);
  const { showBusiness, showHousehold, setShowHousehold } =
    useContext(KSPUContext);
  const { showReservedHouseHolds } = useContext(KSPUContext);
  const { searchURL } = useContext(KSPUContext);
  const { ActiveMapButton, setActiveMapButton } = useContext(MainPageContext);

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

  const handleRemoveRuteInSelection = () => {
    setActiveMapButton("removeRouteInSelection");
    const view = mapView;
    const graphicslayer = props.graphicLayer;

    const desketchViewModel = new SketchViewModel({
      view: view,
      layer: graphicslayer,
    });

    view.popup.close();
    desketchViewModel.create("rectangle", { mode: "freehand" });

    desketchViewModel.on("create", async (event) => {
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
        deselectFeatures(queryGeometry, view);
      }
    });
  };

  const deselectFeatures = async (geometry, view) => {
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

    let queryObject = new Query({
      geometry: geometry,
      outFields: MapConfig.budruterOutField,
      spatialRelationship: "intersects", // Relationship operation to apply
      returnGeometry: false,
      outSpatialReference: { wkid: 25833 },
    });

    await query
      .executeQueryJSON(BudruterUrl, queryObject)
      .then(async (results) => {
        let currentReoler = activUtvalg.reoler;
        results.features.map((item) => {
          let graphRemove = [];

          mapView.graphics.items.map((graphElement) => {
            if (
              graphElement.attributes !== undefined &&
              graphElement.attributes !== null
            ) {
              if (graphElement.attributes.reol_id !== undefined) {
                if (
                  graphElement.attributes.reol_id === item.attributes.reol_id
                ) {
                  currentReoler.map((reoler) => {
                    if (reoler.reolId.toString() === item.attributes.reol_id) {
                      graphRemove.push(graphElement);
                    }
                  });
                }
              }
            }
          });
          graphRemove.map((itemrmv) => {
            if (
              Object.entries(itemrmv.symbol.color).toString() ===
                Object.entries({ r: 237, g: 54, b: 21, a: 0.25 }).toString() ||
              Object.entries(itemrmv.symbol.color).toString() ===
                Object.entries({ r: 0, g: 255, b: 0, a: 0.8 }).toString()
            ) {
              view.graphics.remove(itemrmv);
              currentReoler = currentReoler.filter(
                (x) =>
                  parseInt(x.reolId) !== parseInt(itemrmv.attributes.reol_id)
              );
            }
          });
        });

        setloading(false);
        let utvalg = activUtvalg;
        utvalg.reoler = currentReoler;
        let data = groupBy(
          currentReoler,
          "",
          gettitle(searchURL),
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
        setSelectionUpdate(true);
        //setActivUtvalg({});
        //setActivUtvalg(utvalg);
        await setActivUtvalg((oldState) => ({
          ...oldState,
          Antall: utvalg.Antall,
          receivers: utvalg.receivers,
          Business: utvalg.Business,
          hush: utvalg.hush,
          ReservedHouseHolds: utvalg.ReservedHouseHolds,
          reoler: currentReoler,
          totalAntall: utvalg.totalAntall,
        }));
        // setAktivDisplay(true);

        if (checkedList?.length > 0) {
          checkedList.map((item, x) => {
            if (JSON.parse(item?.utvalgId) === JSON.parse(utvalg?.utvalgId)) {
              checkedList[x].reoler = utvalg.reoler;
              checkedList[x].totalAntall = utvalg.totalAntall;
              checkedList[x].antallBeforeRecreation =
                utvalg.antallBeforeRecreation;
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

        view.popup.close();

        setMapView(view);
        handleRemoveRuteInSelection();
      })
      .catch((error) => {
        console.log("error occured while calling feature layer", error);
      });
  };

  return (
    <div>
      <div>{loading ? <Spinner /> : null}</div>
      <div id="removeRuteInSelectionDiv">
        {ActiveMapButton === "removeRouteInSelection" ? (
          <button
            className="esri-widget--button esri-interactive esri-widget remove-rute-in-selection-icon focus"
            id="removeRuteInSelectionButton"
            type="button"
            title="Fjern rute i utvalg"
            disabled={Object.keys(activUtvalg).length > 0 ? false : true}
            onClick={handleRemoveRuteInSelection}
          ></button>
        ) : (
          <button
            className="esri-widget--button esri-interactive esri-widget remove-rute-in-selection-icon"
            id="removeRuteInSelectionButton"
            type="button"
            title="Fjern rute i utvalg"
            disabled={Object.keys(activUtvalg).length > 0 ? false : true}
            onClick={handleRemoveRuteInSelection}
          ></button>
        )}
      </div>
    </div>
  );
}

export function RemoveRuteInSelectionKw(props) {
  const { mapView, setMapView } = useContext(MainPageContext);
  const [loading, setloading] = useState(false);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  const { householdcheckbox, sethouseholdcheckbox } =
    useContext(KundeWebContext);
  const { Page, setPage } = useContext(KundeWebContext);
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);
  const [utvalgObject, setutvalgObject] = useState({});
  const { ActiveMapButton, setActiveMapButton } = useContext(MainPageContext);
  const { selectedRowKeys, setSelectedRowKeys } =
    React.useContext(KundeWebContext);
  const { selectedrecord_s, setselectedrecord_s } = useContext(KundeWebContext);
  const { routeUpdateEnabled, setRouteUpdateEnabled } =
    useContext(KundeWebContext);
  const { BusinessSum, setBusinessSum } = useContext(KundeWebContext);
  const { HouseholdSum, setHouseholdSum } = useContext(KundeWebContext);
  const { selectionUpdateKW, setSelectionUpdateKW } =
    useContext(KundeWebContext);

  useEffect(() => {
    setPage(Page);
  }, [utvalgObject]);

  const handleRemoveRuteInSelection = () => {
    setActiveMapButton("removeRouteInSelection");
    const view = mapView;
    const graphicslayer = props.graphicLayer;

    const desketchViewModel = new SketchViewModel({
      view: view,
      layer: graphicslayer,
    });

    view.popup.close();
    desketchViewModel.create("rectangle", { mode: "freehand" });

    desketchViewModel.on("create", async (event) => {
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
        await deselectFeatures(queryGeometry, view);
      }
    });
  };

  const deselectFeatures = async (geometry, view) => {
    let BudruterUrl;

    let allLayersAndSublayers = view.map.allLayers.flatten(function (item) {
      return item.layers || item.sublayers;
    });

    allLayersAndSublayers.items.forEach(function (item) {
      if (item.title === "Budruter") {
        BudruterUrl = item.url;
      }
    });

    let queryObject = new Query({
      geometry: geometry,
      outFields: MapConfig.budruterOutField,
      spatialRelationship: "intersects", // Relationship operation to apply
      returnGeometry: false,
      outSpatialReference: { wkid: 25833 },
    });

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
          let graphRemove = [];

          mapView.graphics.items.map((graphElement) => {
            if (
              graphElement.attributes !== undefined &&
              graphElement.attributes !== null
            ) {
              if (graphElement.attributes.reol_id !== undefined) {
                if (
                  graphElement.attributes.reol_id === item.attributes.reol_id
                ) {
                  currentReoler.map((reoler) => {
                    if (reoler.reolId.toString() === item.attributes.reol_id) {
                      graphRemove.push(graphElement);
                    }
                  });
                }
              }
            }
          });
          graphRemove.map((itemrmv) => {
            if (
              Object.entries(itemrmv.symbol.color).toString() ===
                Object.entries({ r: 237, g: 54, b: 21, a: 0.25 }).toString() ||
              Object.entries(itemrmv.symbol.color).toString() ===
                Object.entries({ r: 0, g: 255, b: 0, a: 0.8 }).toString()
            ) {
              view.graphics.remove(itemrmv);
              currentReoler = currentReoler.filter(
                (x) =>
                  parseInt(x.reolId) !== parseInt(itemrmv.attributes.reol_id)
              );
            }
          });
        });

        let reols = currentReoler.map((o) => o.reolId.toString());
        setSelectedRowKeys([...reols]);

        let utvalg = utvalgapiobject;
        utvalg.reoler = currentReoler;
        let Antall = getAntallUtvalg(currentReoler);
        setHouseholdSum(Antall[0]);
        setBusinessSum(Antall[1]);
        utvalg.Antall = Antall;
        utvalg.totalAntall = Antall[0] + (businesscheckbox ? Antall[1] : 0);

        setutvalgapiobject((oldState) => ({
          ...oldState,
          Antall: utvalg.Antall,
          reoler: currentReoler,
          totalAntall: utvalg.totalAntall,
        }));
        setSelectionUpdateKW(true);
        // await setutvalgapiobject(utvalg);
        view.popup.close();
        setMapView(view);
        setutvalgObject((oldState) => ({
          ...oldState,
          utvalgObject: utvalg,
        }));
        setloading(false);
        handleRemoveRuteInSelection();
      })
      .catch((error) => {
        console.log("error occured while calling feature layer", error);
      });
  };

  return (
    <div>
      <div>{loading ? <Spinner /> : null}</div>
      <div id="removeRuteInSelectionDiv">
        {ActiveMapButton === "removeRouteInSelection" ? (
          <button
            className="esri-widget--button esri-interactive esri-widget remove-rute-in-selection-icon focus"
            id="removeRuteInSelectionButton"
            type="button"
            title="Fjern rute i utvalg"
            disabled={
              Object.keys(utvalgapiobject).length > 0 || routeUpdateEnabled
                ? false
                : true
            }
            onClick={handleRemoveRuteInSelection}
          ></button>
        ) : (
          <button
            className="esri-widget--button esri-interactive esri-widget remove-rute-in-selection-icon"
            id="removeRuteInSelectionButton"
            type="button"
            title="Fjern rute i utvalg"
            disabled={
              Object.keys(utvalgapiobject).length > 0 || routeUpdateEnabled
                ? false
                : true
            }
            onClick={handleRemoveRuteInSelection}
          ></button>
        )}
      </div>
    </div>
  );
}
