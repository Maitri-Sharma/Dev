import React, { useRef, useEffect, useContext, useState } from "react";

import Spinner from "../../components/spinner/spinner.component";
import { MainPageContext } from "../../context/Context.js";

import ArcGISMap from "@arcgis/core/Map";
import MapView from "@arcgis/core/views/MapView";
import Home from "@arcgis/core/widgets/Home";
import ScaleBar from "@arcgis/core/widgets/ScaleBar";
import Fullscreen from "@arcgis/core/widgets/Fullscreen";
import Expand from "@arcgis/core/widgets/Expand";
import Search from "@arcgis/core/widgets/Search";
import Legend from "@arcgis/core/widgets/Legend";
import LayerList from "@arcgis/core/widgets/LayerList";
// import DatePicker from "@arcgis/core/widgets/support/DatePicker";
import BaseMap from "@arcgis/core/Basemap";
import Extent from "@arcgis/core/geometry/Extent";
import VectorTileLayer from "@arcgis/core/layers/VectorTileLayer";
import MapImageLayer from "@arcgis/core/layers/MapImageLayer";
import GraphicsLayer from "@arcgis/core/layers/GraphicsLayer";
import Graphic from "@arcgis/core/Graphic";
import * as watchUtils from "@arcgis/core/core/watchUtils";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";

import AdressepunktResultatModelPopup from "../AdressepunktResultatModelPopup/AdressepunktResultatModelPopup.component";

//Custom widgets
import {
  SelectNewRute,
  SelectNewRuteKw,
} from "../widgets/select-new-rute/select-new-rute.jsx";
import {
  AddRuteInSelection,
  AddRuteInSelectionKw,
} from "../widgets/add-rute-in-selection/add-rute-in-selection.jsx";
import {
  RemoveRuteInSelection,
  RemoveRuteInSelectionKw,
} from "../widgets/remove-rute-in-selection/remove-rute-in-selection.jsx";
import MarkAddressPoint from "../widgets/mark-address-point/mark-address-point.jsx";
// import HelpGuide from "../widgets/help-guide/help-guide.jsx";
import DisableSketch from "../widgets/disable-sketch/disable-sketch.jsx";

import { MapConfig } from "../../config/mapconfig";

import api from "../../services/api";

import "./webmapView.styles.scss";

import EsriConfig from "@arcgis/core/config.js";
EsriConfig.assetsPath = "./assets";

function WebMapView() {
  const mapviewDiv = useRef(null);
  var [rutegraphicsLayer, setRuteGraphicsLayer] = useState({});

  const [loading, setloading] = useState(true);
  var { mapView, setMapView } = useContext(MainPageContext);
  const [searchResults, setSearchResults] = useState();
  let params;

  useEffect(() => {
    if (mapviewDiv.current) {
      //vector tile layer
      const vectortileLayer = new VectorTileLayer({
        url: MapConfig.vectorTileLayerUrl,
        title: "Basemap",
      });

      //set geodata basemap
      const geodataBasemap = new BaseMap({
        baseLayers: [vectortileLayer],
        title: "basemap",
        id: "basemap",
      });

      //create map
      setloading(true);
      const map = new ArcGISMap({
        //basemap: "topo-vector",
        basemap: geodataBasemap,
      });

      const activeMapView = new MapView({
        map: map,
        container: mapviewDiv.current,
        //center: MapConfig.center,
        zoom: MapConfig.zoom,
        popup: {
          dockEnabled: true,
          dockOptions: {
            // Disables the dock button from the popup
            buttonEnabled: true,
            // Ignore the default sizes that trigger responsive docking
            breakpoint: false,
          },
        },
      });

      // add home widget
      const homeWidget = new Home({ view: activeMapView });

      //add scalebar widget
      const scaleBarWidget = new ScaleBar({
        view: activeMapView,
        unit: "metric",
      });

      //add search widget
      const searchWidget = new Search({
        view: activeMapView,
        sources: [
          {
            url: MapConfig.geoKodingUrl,
            singleLineFieldName: "SingleLine",
            outFields: ["*"],
            name: "Puma GeoKoding Service",
            placeholder: "Adresse",
            popupEnabled: false,
            // resultSymbol: {
            //   type: "simple-marker",
            //   color: "blue",
            //   size: "12px",
            //   outline: {
            //     color: "blue",
            //     width: "2px",
            //   },
            // },
          },
        ],

        includeDefaultSources: false,
      });

      searchWidget.viewModel.on("search-complete", function (event) {
        document.getElementById("searchResultDiv").style.display = "block";
        setSearchResults(event.results);
      });

      const searchWidgetExpand = new Expand({
        activeMapView,
        content: searchWidget,
        expanded: false,
        expandTooltip: "Søk Adresse",
      });

      const fullscreenWidget = new Fullscreen({
        view: activeMapView,
      });

      // add legend widget
      const legendWidget = new Legend({
        view: activeMapView,
      });

      const legendExpand = new Expand({
        activeMapView,
        content: legendWidget,
        expanded: false,
        expandTooltip: "Kartsymboler",
      });

      // add layerlist widget
      const layerListWidget = new LayerList({
        view: activeMapView,
      });

      const layerListExpand = new Expand({
        activeMapView,
        content: layerListWidget,
        expanded: false,
        expandTooltip: "Endre Kartlag",
      });

      //add datepicker widget
      // const calenderWidget = new DatePicker({
      //   // value: "2022-02-08",
      // });

      // const calenderWidgetExpand = new Expand({
      //   content: calenderWidget,
      //   expanded: false,
      //   expandTooltip: "Kalender",
      // });

      const selectNewRuteInMapWidget = document.getElementById(
        "selectNewRuteWidgetDiv"
      );

      const addRuteInSelectionWidget = document.getElementById(
        "addRuteInSelectionWidgetDiv"
      );

      const removeRuteInSelectionWidget = document.getElementById(
        "removeRuteInSelectionWidgetDiv"
      );

      const markAddressPointWidget = document.getElementById(
        "markAddressPointWidgetDiv"
      );

      const disableSketchWidget = document.getElementById(
        "disableSketchWidgetDiv"
      );

      // const helpGuideWidget = document.getElementById("helpGuideWidgetDiv");

      //add KSPU layers
      const kspuMapImageLayer = new MapImageLayer({
        url: MapConfig.kspuLayerUrl,
      });

      //add kundeweb layers
      const kundewebMapImageLayer = new MapImageLayer({
        url: MapConfig.kundewebLayerUrl,
      });

      mapView = activeMapView;
      setMapView(activeMapView);

      activeMapView.when(() => {
        //api.logger.info("map view is ready");

        // adds the widgets to the top left corner of the MapView
        activeMapView.ui.add(homeWidget, "top-left");

        // Add widget to the bottom left corner of the view
        activeMapView.ui.add(scaleBarWidget, {
          position: "bottom-left",
        });

        //add kspu or kundeweb  layer
        if (!window.location.href.toLowerCase().includes("pumakundeweb")) {
          map.add(kspuMapImageLayer);

          activeMapView.whenLayerView(kspuMapImageLayer).then((layerView) => {
            watchUtils.whenFalse(layerView, "updating", () => {
              setloading(false);
            });
          });
          activeMapView.constraints = { minScale: 10240000, maxScale: 0 };

          // executeIdentify() is called each time the view is clicked
          activeMapView.on("click", executeIdentifyKSPULayer.bind(this));
        } else {
          map.add(kundewebMapImageLayer);

          activeMapView
            .whenLayerView(kundewebMapImageLayer)
            .then((layerView) => {
              watchUtils.whenFalse(layerView, "updating", () => {
                setloading(false);
              });
            });

          activeMapView.constraints = { minScale: 10240000, maxScale: 0 };

          // executeIdentify() is called each time the view is clicked
          activeMapView.on("click", executeIdentifyKundewebLayer.bind(this));
        }

        let graphicsLayer = new GraphicsLayer({
          listMode: "hide",
        });
        map.add(graphicsLayer);

        setRuteGraphicsLayer(graphicsLayer);

        // Add the widget to the top right corner of the view
        if (!window.location.href.toLowerCase().includes("pumakundeweb")) {
          // Set the extent on the view
          activeMapView.extent = new Extent(MapConfig.inernwebMapExtent);

          activeMapView.ui.add(
            [
              searchWidgetExpand,
              markAddressPointWidget,
              legendExpand,
              layerListExpand,
              fullscreenWidget,
              selectNewRuteInMapWidget,
              addRuteInSelectionWidget,
              removeRuteInSelectionWidget,
              disableSketchWidget,
              //calenderWidgetExpand,
              // helpGuideWidget,
            ],
            "top-right"
          );
        } else {
          // Set the extent on the view
          activeMapView.extent = new Extent(MapConfig.kundewebMapExtent);
          activeMapView.ui.add(
            [
              searchWidgetExpand,
              legendExpand,
              layerListExpand,
              fullscreenWidget,
              //selectNewRuteInMapWidget,
              addRuteInSelectionWidget,
              removeRuteInSelectionWidget,
              disableSketchWidget,
              // helpGuideWidget,
            ],
            "top-right"
          );
        }
      });

      function executeIdentifyKSPULayer(evt) {
        //setloading(true);
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

        let queryObject = new Query({
          geometry: evt.mapPoint,
          outFields: MapConfig.budruterOutField,
          spatialRelationship: "intersects",
          returnGeometry: true,
          outSpatialReference: mapView.SpatialReference,
        });

        query.executeQueryJSON(BudruterUrl, queryObject).then((results) => {
          if (results.features.length > 0) {
            results.features.map((item) => {
              let geometry = item.geometry;
              if (geometry) {
                if (geometry.type === "polygon") {
                  let symbol = {
                    type: "simple-fill", // autocasts as new SimpleFillSymbol()
                    color: [0, 84, 227, 0.25],
                    style: "solid",
                    outline: {
                      // autocasts as new SimpleLineSymbol()
                      color: [0, 84, 227],
                      width: 0.75,
                    },
                  };
                  let graphic = new Graphic(geometry, symbol);

                  activeMapView.graphics.add(graphic);

                  watchUtils.whenTrue(
                    activeMapView.popup,
                    "visible",
                    function () {
                      watchUtils.whenFalseOnce(
                        activeMapView.popup,
                        "visible",
                        function () {
                          activeMapView.graphics.items.forEach(function (item) {
                            if (item.attributes === undefined) {
                              activeMapView.graphics.remove(item);
                            }
                          });
                        }
                      );
                    }
                  );
                }
              }

              let popupContent =
                "<table class='popupTemplateTable'>" +
                "<tbody>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Teamnavn</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.teamnavn +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Rutenummer</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.reolnr +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Rutenavn</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.beskrivelse +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Antall HH</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.hh +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Antall VH</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.vh +
                "</td>" +
                "</tr>" +
                " <td class='popupTemplateDataColumn'>Kommune</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.kommune +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Fylke</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.fylke +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Postnummer</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.postnr +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>PRS</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.prsnavn +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Rute ID</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.reol_id +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Rutetype</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.reoltype +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Frekvens</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.rutedistfreq +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Tilleggsinfo distribusjon</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.sondagflag +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Teamnummer</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.teamnr +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Poststed</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.poststed +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Fylke ID</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.fylkeid +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Kommune ID</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.kommuneid +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Antall reserverte</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.hh_res +
                "</td>" +
                "</tr>" +
                "<tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>HH ikke prio</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.np_hh_u_res +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>HH prio</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.p_hh_u_res +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Prissone</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.prissone +
                "</td>" +
                "</tr>" +
                // "<tr>" +
                // " <td class='popupTemplateDataColumn'>Segment ID</td>" +
                // " <td class='popupTemplateData'>" +
                // (!item.attributes.segment ? "" : item.attributes.segment) +
                // "</td>" +
                // "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Postreklamesenter enhetsid</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.prsnr +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Postreklamesenter beskrivelse</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.prsbeskrivelse +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                " </table>";

              // show popup on click on map
              let popup = activeMapView.popup;
              popup.title = "Budruter";
              popup.content = popupContent;
              popup.location = evt.mapPoint;
              popup.open();
              // setloading(false);
            });
          }
        });
      }

      function executeIdentifyKundewebLayer(evt) {
        //  setloading(true);
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

        let queryObject = new Query({
          geometry: evt.mapPoint,
          outFields: MapConfig.budruterOutField,
          spatialRelationship: "intersects",
          returnGeometry: true,
          outSpatialReference: mapView.SpatialReference,
        });

        query.executeQueryJSON(BudruterUrl, queryObject).then((results) => {
          if (results.features.length > 0) {
            results.features.map((item) => {
              let geometry = item.geometry;
              if (geometry) {
                if (geometry.type === "polygon") {
                  let symbol = {
                    type: "simple-fill", // autocasts as new SimpleFillSymbol()
                    color: [0, 84, 227, 0.25],
                    style: "solid",
                    outline: {
                      // autocasts as new SimpleLineSymbol()
                      color: [0, 84, 227],
                      width: 0.75,
                    },
                  };
                  let graphic = new Graphic(geometry, symbol);

                  activeMapView.graphics.add(graphic);

                  watchUtils.whenTrue(
                    activeMapView.popup,
                    "visible",
                    function () {
                      watchUtils.whenFalseOnce(
                        activeMapView.popup,
                        "visible",
                        function () {
                          activeMapView.graphics.items.forEach(function (item) {
                            if (item.attributes === undefined) {
                              activeMapView.graphics.remove(item);
                            }
                          });
                        }
                      );
                    }
                  );
                }
              }

              let popupContent =
                "<table class='popupTemplateTable'>" +
                "<tbody>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Teamnavn</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.teamnavn +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Rutenummer</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.reolnr +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Rutenavn</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.beskrivelse +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Antall HH</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.hh +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Antall VH</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.vh +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Kommune</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.kommune +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Fylke</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.fylke +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Postnummer</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.postnr +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>PRS</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.prsnavn +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Rute ID</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.reol_id +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Rutetype</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.reoltype +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Frekvens</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.rutedistfreq +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Tilleggsinfo distribusjon</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.sondagflag +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Teamnummer</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.teamnr +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Poststed</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.poststed +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Fylke ID</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.fylkeid +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Kommune ID</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.kommuneid +
                "</td>" +
                "</tr>" +
                // "<tr>" +
                // " <td class='popupTemplateDataColumn'>Antall reserverte</td>" +
                // " <td class='popupTemplateData'>" +
                // item.attributes.hh_res +
                // "</td>" +
                // "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>HH ikke prio</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.np_hh_u_res +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>HH prio</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.p_hh_u_res +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Prissone</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.prissone +
                "</td>" +
                "</tr>" +
                // "<tr>" +
                // " <td class='popupTemplateDataColumn'>Segment ID</td>" +
                // " <td class='popupTemplateData'>" +
                // (!item.attributes.segment ? "" : item.attributes.segment) +
                // "</td>" +
                // "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Postreklamesenter enhetsid</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.prsnr +
                "</td>" +
                "</tr>" +
                "<tr>" +
                " <td class='popupTemplateDataColumn'>Postreklamesenter beskrivelse</td>" +
                " <td class='popupTemplateData'>" +
                item.attributes.prsbeskrivelse +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                " </table>";

              // show popup on click on map
              let popup = activeMapView.popup;
              popup.title = "Budruter";
              popup.content = popupContent;
              popup.location = evt.mapPoint;
              popup.open();
              //setloading(false);
            });
          }
        });
      }
    }
  }, []);

  return (
    <div>
      <div>{loading ? <Spinner /> : null}</div>
      <div className="mapviewDiv" ref={mapviewDiv}>
        {!window.location.href.toLowerCase().includes("pumakundeweb") && (
          <div
            id="markAddressPointWidgetDiv"
            style={{ display: loading ? "none" : "block" }}
          >
            <MarkAddressPoint view={mapView} />
          </div>
        )}
        {/* {!window.location.href.toLowerCase().includes("pumakundeweb") && ( */}
        <div
          id="selectNewRuteWidgetDiv"
          style={{ display: loading ? "none" : "block" }}
        >
          {!window.location.href.toLowerCase().includes("pumakundeweb") ? (
            <SelectNewRute view={mapView} graphicLayer={rutegraphicsLayer} />
          ) : null}
        </div>
        <div
          id="addRuteInSelectionWidgetDiv"
          style={{ display: loading ? "none" : "block" }}
        >
          {!window.location.href.toLowerCase().includes("pumakundeweb") ? (
            <AddRuteInSelection
              view={mapView}
              graphicLayer={rutegraphicsLayer}
            />
          ) : (
            <AddRuteInSelectionKw
              view={mapView}
              graphicLayer={rutegraphicsLayer}
            />
          )}
        </div>
        <div
          id="removeRuteInSelectionWidgetDiv"
          style={{ display: loading ? "none" : "block" }}
        >
          {!window.location.href.toLowerCase().includes("pumakundeweb") ? (
            <RemoveRuteInSelection
              view={mapView}
              graphicLayer={rutegraphicsLayer}
            />
          ) : (
            <RemoveRuteInSelectionKw
              view={mapView}
              graphicLayer={rutegraphicsLayer}
            />
          )}
        </div>
        {/* {!window.location.href.toLowerCase().includes("pumakundeweb") && (
          <div
            id="helpGuideWidgetDiv"
            style={{ display: loading ? "none" : "block" }}
          >
            <HelpGuide view={mapView} />
          </div>
        )} */}
        <div
          id="disableSketchWidgetDiv"
          style={{ display: loading ? "none" : "block" }}
        >
          <DisableSketch view={mapView} graphicLayer={rutegraphicsLayer} />
        </div>

        <div id="searchResultDiv" className="searchResult">
          <AdressepunktResultatModelPopup searchResults={searchResults} />
        </div>
      </div>
    </div>
  );
}

export default WebMapView;
