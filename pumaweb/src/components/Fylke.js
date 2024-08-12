import React, { useState, useEffect, useContext, useRef } from "react";
import "../App.css";

import Geographie_footer from "./Geographie_footer";
import api from "../services/api.js";
import FylkeData from "./datadef/FylkeData.js";
import { UtvalgContext } from "../context/Context.js";
import { KSPUContext } from "../context/Context.js";
import { MainPageContext } from "../context/Context.js";
import MottakerComponent from "./Mottakergrupper";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";

function Fylke() {
  const { setIsPickList } = useContext(UtvalgContext);
  const [pickListCheckbox, setPickListCheckbox] = useState(true);
  const [datalist, setData] = useState([]);
  const [selectedGroups, setSelectedGroups] = useState([]);
  const { searchData, setSearchData } = useContext(UtvalgContext);
  const {
    searchURL,
    setSearchURL,
    setShowHousehold,
    setShowBusiness,
    setShowReservedHouseHolds,
    setActivUtvalg,
  } = useContext(KSPUContext);
  const [alertbox, setAlert] = useState(false);
  const { layer } = useContext(MainPageContext);
  const { view } = useContext(MainPageContext);
  const { highlight, setHighlight } = useContext(MainPageContext);
  const { mapView } = useContext(MainPageContext);
  const pickList = useRef();
  useEffect(() => {
    fetchData("Fylke/Getallfylkes");
    setActivUtvalg({});
    setShowReservedHouseHolds(false);
    setShowBusiness(false);
    setShowHousehold(true);
  }, []);

  useEffect(() => {
    setSearchData(selectedGroups);
    setSearchURL("Reol/GetReolsInFylke?fylkeId=");
  }, [selectedGroups]);

  const fetchData = async (url) => {
    try {
      const { data, status } = await api.getdata(url);
      if (status === 200) {
        setData(data);
      } else {
        console.error("error : " + status);
      }
    } catch (error) {
      console.error("er : " + error);
    }
  };

  const handleMap = async () => {
    let routeids = [];
    if (selectedGroups.length > 0) {
      let f = selectedGroups.map((element) => "'" + element + "'").join(",");
      let fylkeWhereClause = `fylkeid in (${f})`;
      
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

      let queryObject = new Query();

      queryObject.where = `${fylkeWhereClause}`;
      queryObject.returnGeometry = true;
      queryObject.outFields = ["tot_anta", "hh", "reol_id"];

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

            // remove previous highlighted feature
            let j = mapView.graphics.items.length;
            var k = 0;
            k = j;
            for (var i = j; i > 0; i--) {
              if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
                mapView.graphics.remove(mapView.graphics.items[i - 1]);
                //j++;
              }
            }
            // mapView.graphics.removeAll();

            results.features.map((item) => {
              featuresGeometry.push(item.geometry);
              let graphic = new Graphic(item.geometry, selectedSymbol);
              mapView.graphics.add(graphic);
            });

            mapView.goTo(featuresGeometry);
          }
        });
    }
  };

  const HandlePickList = async (e) => {
    setIsPickList(pickList.current.checked);
    setPickListCheckbox(false);
    // setIsPickList(true);
    
  };
  const HandlePickList_new = async (e) => {
    // setIsPickList(pickList.current.checked);
    setPickListCheckbox(true);
    setIsPickList(false);
    
  };

  return (
    <div>
      {alertbox ? (
        <span className="sok-Alert-text pl-2">Det må velges minst et fylke</span>
      ) : null}
      <div>
        <p className="label p-2">Merk fylker du ønsker å ha med i utvalget</p>
        <div className="row col-12 col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 mb-2">
          <div className="col-lg-8 col-md-8 col-sm-12 col-xs-12  Fylke pl-4">
            <div className="ml-2">
              {datalist.map((data) => (
                <FylkeData
                  data={data}
                  setSelectedGroups={setSelectedGroups}
                  selectedGroups={selectedGroups}
                />
              ))}
            </div>
          </div>

          <div className="col-lg-4 col-md-4 col-sm-12 col-xs-12 pt-4">
            <div className="col m-0 p-0">
              <div className="col-lg-12 col-md-12 col-sm-0 col-xs-0 mt-5"></div>
              <div className="col-lg-12 col-md-12 col-sm-0 col-xs-0 mt-5"></div>
              <div className="col-lg-12 col-md-12 col-sm-0 col-xs-0 mt-5"></div>
              <div className="col-lg-12 col-md-12 col-sm-0 col-xs-0 mt-5"></div>
              <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 mt-4 p-0 m-0">
                <input
                  type="submit"
                  className="KSPU_button m-0 p-0"
                  onClick={handleMap}
                  value="Vis i Kart"
                />
              </div>
            </div>
          </div>
        </div>
      </div>
      <MottakerComponent page="DTPage" marginTop=".3rem" />
      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 pt-2">
        <div className="col-10 m-0 p-0 pr-2">
          {" "}
          <span className="label p-2">Velg veien videre</span>
        </div>
        <div className="col-2 m-0 p-0 pr-2"></div>

        <div className="sok-text ml-3">
          <div>
            <input
              type="radio"
              name="optradio"
              className="sok-text"
              checked={pickListCheckbox}
              onChange={HandlePickList_new}
            />{" "}
            Hele fylket
            {/* {props.checkname1} */}
          </div>
          <div>
            <input
              type="radio"
              ref={pickList}
              name="optradio"
              onClick={HandlePickList}
            />
            Plukkliste
            {/* {props.checkname2} */}
          </div>
        </div>
      </div>
      {/* <Geographie_footer
        checkname1=" Hele fylket"
        checkname2=" Plukkliste"
        name=""
      /> */}
    </div>
  );
}

export default Fylke;
