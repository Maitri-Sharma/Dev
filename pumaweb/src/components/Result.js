import React, { useState, useContext, useEffect, useRef } from "react";
import SelectionDetails from "./SelectionDetails";
import { KSPUContext } from "../context/Context.js";

function Result(props) {
  const { activUtvalg, setActivUtvalg } = useContext(KSPUContext);
  return (
    <div className="card Kj-background-color ml-1 mr-1 mt-1">
      <div className="row ">
        <p className="avan pl-4 pt-1 ">RESULTAT</p>
      </div>
      {Object.keys(activUtvalg).length === 0 ? (
        <></>
      ) : (
        <div className="Kj-div-background-color pt-2 pb-2">
          <SelectionDetails />
        </div>
      )}
    </div>
  );
}

export default Result;
