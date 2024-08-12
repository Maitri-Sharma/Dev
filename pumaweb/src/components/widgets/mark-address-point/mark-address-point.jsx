import React, { useContext, useState, useEffect } from "react";
import SketchViewModel from "@arcgis/core/widgets/Sketch/SketchViewModel";
import Graphic from "@arcgis/core/Graphic";
import GraphicsLayer from "@arcgis/core/layers/GraphicsLayer";
import TextSymbol from "@arcgis/core/symbols/TextSymbol";

import { KSPUContext, MainPageContext } from "../../../context/Context.js";

export function MarkAddressPoint(props) {
  const { mapView, setMapView } = useContext(MainPageContext);
  const { addressPoints, setAddressPoints } = useContext(MainPageContext);
  const { ActiveMapButton, setActiveMapButton } = useContext(MainPageContext);

  const handleMarkAddressPoint = () => {
    setActiveMapButton("markAddress");
    const graphicslayer = new GraphicsLayer();

    const sketchViewModel = new SketchViewModel({
      view: mapView,
      layer: graphicslayer,
    });
    mapView.popup.close();
    sketchViewModel.create("point");

    sketchViewModel.on("create", async (event) => {
      if (event.state === "complete") {
        addGraphic(event);
      }
    });

    let textSymbol = new TextSymbol({
      color: "white",
      haloColor: "black",
      haloSize: "1px",
      xoffset: 3,
      yoffset: 3,
      font: {
        // autocast as esri/symbols/Font
        size: 12,
        //family: "sans-serif",
        family: "Arial",
        weight: "bolder",
      },
    });

    function addGraphic(event) {
      // Create a new graphic and set its geometry to
      textSymbol.text = prompt(
        "Legg inn et navn for merking av adressepunktet:",
        ""
      );

      const pointGraphic = new Graphic({
        geometry: event.graphic.geometry,
        symbol: {
          type: "simple-marker",
          color: "blue",
          size: "12px",
          outline: {
            color: "blue",
            width: "2px",
          },
        },
        attributes: {
          Match_addr: textSymbol.text,
        },
      });

      if (textSymbol.text !== null && textSymbol.text.length > 0 ) {
        mapView.graphics.add(pointGraphic);

        // add label to point graphics
        const labelPoint = new Graphic({
          geometry: event.graphic.geometry,
          symbol: textSymbol,
        });
        mapView.graphics.add(labelPoint);
        //addressPoints.push(pointGraphic);
        setAddressPoints(addressPoints.concat(pointGraphic));
        sessionStorage.setItem(
          "addressPoints",
          JSON.stringify(addressPoints.concat(pointGraphic))
        );
        setActiveMapButton("");
      }
      else{
        setActiveMapButton("");
      }
    }
  };

  return (
    <div id="markAddressPointDiv">
      {ActiveMapButton === "markAddress" ? (
        <button
          className="esri-widget--button esri-interactive esri-icon-map-pin esri-widget focus"
          id="markAddressPointButton"
          type="button"
          title="Merk adressepunkt i kart"
          onClick={handleMarkAddressPoint}
        ></button>
      ) : (
        <button
          className="esri-widget--button esri-interactive esri-icon-map-pin esri-widget"
          id="markAddressPointButton"
          type="button"
          title="Merk adressepunkt i kart"
          onClick={handleMarkAddressPoint}
        ></button>
      )}
    </div>
  );
}

export default MarkAddressPoint;
