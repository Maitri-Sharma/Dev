import React, { useContext, useState, useEffect } from "react";

import SketchViewModel from "@arcgis/core/widgets/Sketch/SketchViewModel";
import * as geometryEngineAsync from "@arcgis/core/geometry/geometryEngineAsync";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";

import { KSPUContext } from "../../../context/Context.js";
import { groupBy } from "../../../Data";
import {
  getAntallUtvalg,
  Utvalg,
  NewUtvalgName,
  criterias,
  Reol,
} from "../../KspuConfig";
import { KundeWebContext } from "../../../context/Context.js";
import { MainPageContext } from "../../../context/Context.js";
import { MapConfig } from "../../../config/mapconfig";
import Spinner from "../../../components/spinner/spinner.component";

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

export function SelectNewRute(props) {
  const { mapView, setMapView } = useContext(MainPageContext);
  const { resultData, setResultData } = useContext(KSPUContext);
  const {
    activUtvalg,
    setActivUtvalg,
    setActivUtvalglist,
    setAktivDisplay,
    setvalue,
    setDemografieDisplay,
    setSegmenterDisplay,
    setAddresslisteDisplay,
    setGeografiDisplay,
    setRuteDisplay,
    setKjDisplay,
    setAdresDisplay,
    isWidgetActive,
    setIsWidgetActive,
    setutvalglistcheck,
    showReservedHouseHolds,
    setShowReservedHouseHolds,
  } = useContext(KSPUContext);
  const { showBusiness, setShowBusiness, showHousehold, setShowHousehold } =
    useContext(KSPUContext);

  const [ButtonActiveColor, setButtonActiveColor] = useState(false);
  const [loading, setloading] = useState(false);
  const { ActiveMapButton, setActiveMapButton } = useContext(MainPageContext);

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

  const handleSingleSelect = async (querygeometry) => {
    let BudruterUrl;

    let allLayersAndSublayers = mapView.map.allLayers.flatten(function (item) {
      return item.layers || item.sublayers;
    });

    allLayersAndSublayers.items.forEach(function (item) {
      if (item.title === "Budruter") {
        BudruterUrl = item.url;
      }
    });

    mapView.popup.close();

    let queryObject = new Query({
      geometry: querygeometry,
      outFields: MapConfig.budruterOutField,
      spatialRelationship: "intersects", // Relationship operation to apply
      returnGeometry: true,
      outSpatialReference: { wkid: 25833 },
    });

    await query
      .executeQueryJSON(BudruterUrl, queryObject)
      .then(async (results) => {
        let currentReoler = [];
        setIsWidgetActive(false);
        results.features.map((item) => {
          var geometry = item.geometry;
          if (geometry) {
            if (geometry.type === "polygon") {
              var symbol = {
                type: "simple-fill", // autocasts as new SimpleFillSymbol()
                color: [237, 54, 21, 0.25],
                style: "solid",
                outline: {
                  // autocasts as new SimpleLineSymbol()
                  color: [237, 54, 21],
                  width: 0.75,
                },
              };
              currentReoler.push(formatData(item.attributes));
              var graphic = new Graphic(geometry, symbol, item.attributes);
              // make sure to remmove previous highlighted feature
              //mapView.graphics.add(graphic);

              // mapView.graphics.add(graphic);
            }
          }
        });

        setloading(false);
        let utvalg = Utvalg();
        utvalg.reoler = currentReoler;
        setShowHousehold(true);
        setShowBusiness(false);
        setShowReservedHouseHolds(false);
        utvalg.hasReservedReceivers = showReservedHouseHolds ? true : false;
        // utvalg.criterias.push(criterias(500, "Valgt enkeltvis"));
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
        let fastantallsanalyseType = 10;
        let criteraString = "Valgt enkeltvis";
        utvalg.criterias.push(criterias(fastantallsanalyseType, criteraString));

        setActivUtvalg({});
        await setActivUtvalg(utvalg);
        setutvalglistcheck(false);
        setActivUtvalglist({});
        setMapView(mapView);
        // setAktivDisplay(false);
        setAktivDisplay(true);
        setvalue(false);
        setRuteDisplay(false);
        setAdresDisplay(false);
        setDemografieDisplay(false);
        setSegmenterDisplay(false);
        setAddresslisteDisplay(false);
        setGeografiDisplay(false);
        setKjDisplay(false);
        mapView.popup.close();
      })
      .catch((error) => {
        console.log("error occured while calling feature layer", error);
      });
  };

  const handleNewRuteInSelection = () => {
    setActiveMapButton("selectNewRoute");
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
        let j = mapView.graphics.items.length;
        for (var i = j; i > 0; i--) {
          if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
            mapView.graphics.remove(mapView.graphics.items[i - 1]);
            //j++;
          }
        }
        //mapView.graphics.removeAll();
        graphicslayer.removeAll();
        setloading(true);
        handleSingleSelect(queryGeometry);
      }
    });
  };

  return mapView ? (
    <div>
      <div>{loading ? <Spinner /> : null}</div>
      <div id="selectNewRuteDiv">
        {ActiveMapButton === "selectNewRoute" ? (
          <button
            className={
              "esri-widget--button esri-interactive select-new-rute-icon esri-widget focus"
            }
            id="selectNewRuteButton"
            type="button"
            title="Nytt utvalg i kart"
            onClick={handleNewRuteInSelection}
          ></button>
        ) : (
          <button
            className={
              "esri-widget--button esri-interactive select-new-rute-icon esri-widget"
            }
            id="selectNewRuteButton"
            type="button"
            title="Nytt utvalg i kart"
            onClick={handleNewRuteInSelection}
          ></button>
        )}
      </div>
    </div>
  ) : null;
}

export function SelectNewRuteKw(props) {
  const { mapView, setMapView } = useContext(MainPageContext);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  const { householdcheckbox, sethouseholdcheckbox } =
    useContext(KundeWebContext);
  const { Page, setPage } = useContext(KundeWebContext);
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);
  const [utvalgObjectvalue, setutvalgObjectvalue] = useState(utvalgapiobject);

  useEffect(async () => {
    await setPage(Page);
  }, [utvalgObjectvalue]);

  const handleSingleSelect = async (querygeometry) => {
    let BudruterUrl;

    let allLayersAndSublayers = mapView.map.allLayers.flatten(function (item) {
      return item.layers || item.sublayers;
    });

    allLayersAndSublayers.items.forEach(function (item) {
      if (item.title === "Budruter") {
        BudruterUrl = item.url;
      }
    });

    mapView.popup.close();

    let queryObject = new Query({
      geometry: querygeometry,
      outFields: MapConfig.budruterOutField,
      spatialRelationship: "intersects", // Relationship operation to apply
      returnGeometry: true,
      outSpatialReference: { wkid: 25833 },
    });

    await query
      .executeQueryJSON(BudruterUrl, queryObject)
      .then(async (results) => {
        let currentReoler = [];
        results.features.map((item) => {
          var geometry = item.geometry;
          if (geometry) {
            if (geometry.type === "polygon") {
              var symbol = {
                type: "simple-fill", // autocasts as new SimpleFillSymbol()
                color: [237, 54, 21, 0.25],
                style: "solid",
                outline: {
                  // autocasts as new SimpleLineSymbol()
                  color: [237, 54, 21],
                  width: 0.75,
                },
              };
              currentReoler.push(formatData(item.attributes));
              var graphic = new Graphic(geometry, symbol, item.attributes);
              // make sure to remmove previous highlighted feature
              //mapView.graphics.add(graphic);
            }
          }
        });
        let utvalg = Utvalg();
        utvalg.reoler = currentReoler;
        utvalg.hasReservedReceivers = false;
        utvalg.name = NewUtvalgName();
        let Antall = getAntallUtvalg(currentReoler);
        utvalg.Antall = Antall;
        utvalg.totalAntall = Antall[0] + (businesscheckbox ? Antall[1] : 0);
        utvalg.receivers = [{ receiverId: 1, selected: true }];
        if (businesscheckbox)
          utvalg.receivers.push({ receiverId: 4, selected: true });
        utvalg.modifications = [];
        let fastantallsanalyseType = 10;
        let criteraString = "Valgt enkeltvis";
        utvalg.criterias.push(criterias(fastantallsanalyseType, criteraString));
        await setutvalgapiobject(utvalg);
        setMapView(mapView);
        setutvalgObjectvalue(utvalg);
      })
      .catch((error) => {
        console.log("error occured while calling feature layer", error);
      });
  };

  const handleNewRuteInSelection = () => {
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
        // mapView.graphics.removeAll();
        let j = mapView.graphics.items.length;
        for (var i = j; i > 0; i--) {
          if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
            mapView.graphics.remove(mapView.graphics.items[i - 1]);
            //j++;
          }
        }
        graphicslayer.removeAll();
        handleSingleSelect(queryGeometry);
      }
    });
  };

  return mapView ? (
    <div id="selectNewRuteDiv">
      <button
        className="esri-widget--button esri-interactive select-new-rute-icon esri-widget"
        id="selectNewRuteButton"
        type="button"
        title="Nytt utvalg i kart"
        onClick={handleNewRuteInSelection}
      ></button>
    </div>
  ) : null;
}
