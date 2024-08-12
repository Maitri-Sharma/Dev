import React, { useContext, useState, useEffect } from "react";

import { MainPageContext, KSPUContext } from "../../../context/Context.js";

export function DisableSketch(props) {
  const { mapView, setMapView } = useContext(MainPageContext);
  const { ActiveMapButton, setActiveMapButton } = useContext(MainPageContext);

  const handleDisableSketch = () => {
    setActiveMapButton("");
    mapView.activeTool = null;
  };

  return (
    <div id="disableSketchDiv">
      <button
        className="esri-widget--button esri-interactive esri-widget disable-sketch-icon"
        id="disableSketchButton"
        type="button"
        title="Start panorering"
        onClick={handleDisableSketch}
      ></button>
    </div>
  );
}

export default DisableSketch;
