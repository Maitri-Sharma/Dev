import React, { useEffect, useState, useContext, useRef } from "react";
import "../App.css";
import expand from "../assets/images/esri/expand.png";
import collapse from "../assets/images/esri/collapse.png";
import Utvalgselectiondata from "./datadef/Utvalgselectiondata";

function Utvalselection(props) {
  const [togglevalue, settogglevalue] = useState(false);
  const toggle = () => {
    settogglevalue(!togglevalue);
  };
  return (
    <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 pr-1">
      <div className="card Kj-background-color col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <div className="row">
          <div className="col-8">
            <p className="avan p-1 ">UTVALG</p>
          </div>
          <div className="col-4" onClick={toggle}>
            {togglevalue ? (
              <img className="d-flex float-right pt-1 mr-1" src={collapse} />
            ) : (
              <img className="d-flex float-right pt-1 mr-1" src={expand} />
            )}
          </div>
        </div>
        {togglevalue ? (
          <div className="Kj-div-background-color col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 ">
            {props.data.memberLists.length > 0 ? (
              <p className="divValueText-list mb-4 p-0">
                Listen inneholder en eller flere lister med ytterligere utvalg.
                Se arbeidsliste for detaljer.
              </p>
            ) : typeof props.data.memberUtvalgs !== "undefined" &&
              props.data.memberUtvalgs.length <= 0 ? (
              <span>Ingen utvalg tilgjengelig </span>
            ) : (
              <div className="row col-12 p-0 m-0">
                <div className="row col-12 p-0 m-0 pl-1 pr-1">
                  <div className="col-9 selectionTable_center m-0 p-0 pl-1">
                    <span>Utvalg</span>
                  </div>
                  <div className="col-3 selectionTable_center m-0 p-0 pr-1">
                    <span>Antall</span>
                  </div>
                </div>
                {props.data.memberUtvalgs.map((item) => (
                  <Utvalgselectiondata Item={item} />
                ))}
              </div>
            )}
          </div>
        ) : null}
      </div>
    </div>
  );
}

export default Utvalselection;
