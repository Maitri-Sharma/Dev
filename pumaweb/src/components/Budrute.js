import React, { useState, useRef, useEffect, useContext } from "react";
import MottakerComponent from "./Mottakergrupper";
import Geographie_footer from "./Geographie_footer";
import "../App.css";
import api from "../services/api.js";
import { UtvalgContext } from "../context/Context.js";
import { KSPUContext } from "../context/Context.js";
import { MainPageContext } from "../context/Context.js";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";

function Budrute() {
  const searchRute = useRef(null);
  const summaryBox = useRef(null);
  const resultBox = useRef(null);
  const [disableBtn, setBtnDisable] = useState(true);
  const { searchData, setSearchData } = useContext(UtvalgContext);
  const { searchURL, setSearchURL, setActivUtvalg } = useContext(KSPUContext);
  const { layer } = useContext(MainPageContext);
  const { mapView } = useContext(MainPageContext);

  const { highlight, setHighlight } = useContext(MainPageContext);
  useEffect(() => {
    setActivUtvalg({});
  }, []);
  const fetchData = async (url) => {
    try {
      const { data, status } = await api.getdata(url);
      if (status === 200) {
        if (data.length > 0) {
          data.sort((a, b) => {
            return a.reolNumber - b.reolNumber;
          });

          data.map(function (d, i) {
            var newOption = new Option(d.name, d.reolId);
            summaryBox.current.add(newOption, i);
          });
        }
      } else {
        console.error("error : " + status);
      }
    } catch (error) {
      console.error("er : " + error);
    }
  };

  function fetchRouteData() {
    fetchData("Reol/SearchReolByReolName?reolName=" + searchRute.current.value);
  }

  function addRouteData() {
    let newArr = [];
    Array.from(summaryBox.current.options)
      .filter((x) => x.selected)
      .map((d) => {
        if (
          !Array.from(resultBox.current.options)
            .map((opt) => opt.value)
            .includes(d.value)
        ) {
          var newOption = new Option(d.text, d.value);
          resultBox.current.add(newOption, resultBox.current.length);

          //Adding logic for kommune Addition for Search
          let newArray = [...searchData, d.value];
          if (searchData.includes(d.value)) {
            newArray = newArray.filter((j) => j !== d.value);
          }
          newArr.push(newArray[0]);
          setSearchURL("Reol/GetReolsFromReolIDString?ids=");
        }
      });
    setSearchData(newArr);
    setBtnDisable(false);
  }

  const handleRemoveClick = (e) => {
    Array.from(resultBox.current.options)
      .filter((x) => x.selected)
      .map((d) => {
        resultBox.current.remove(d.index);
        //Adding logic for removal of kommune from Search
        let newArray = searchData.filter((j) => j !== d.value);
        setSearchData(newArray);
      });
    if (resultBox.current.length === 0) {
      setBtnDisable(true);
    }
  };

  const handleMap = async () => {
    let routeids = [];
    let searchArrayData = Array.from(resultBox.current.options);
    if (searchArrayData.length > 0) {
      // await Promise.all(
      //   selectedGroups.map(async (item) => {
      //     try {
      //       const { data, status } = await api.getdata(
      //         "Reol/GetReolsInFylke?fylkeId=" + item
      //       );
      //       if (status === 200) {
      //         routeids.push(
      //           data.map((item) => {
      //             return "'" + `${item["reolId"]}` + "'";
      //           })
      //         );
      //       } else {
      //         console.error("error : " + status);
      //       }
      //     } catch (error) {
      //       console.error("er : " + error);
      //     }
      //   })
      // );

      let r = searchArrayData
        .map((element) => "'" + element.value + "'")
        .join(",");
      let reolWhereClause = `reol_id in (${r})`;
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

      queryObject.where = `${reolWhereClause}`;
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
            // let j = mapView.graphics.items.length;
            // var k = 0;
            // k = j;
            // for (var i = j; i > 0; i--) {
            //   if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
            //     mapView.graphics.remove(mapView.graphics.items[i - 1]);
            //     //j++;
            //   }
            // }
            mapView.graphics.removeAll();

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
  return (
    <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <div className="col-10 m-0 p-0 pr-2">
          <i className="fa fa-user-circle-o pl-1"></i>
          <input
            type="text"
            ref={searchRute}
            className="TeamInputText mt-1"
            placeholder=""
          />
        </div>
        <div className="col-2 m-0 p-0 ">
          <input
            type="submit"
            onClick={fetchRouteData}
            className="KSPU_button mt-1"
            value="Søk"
          />
        </div>
      </div>
      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 pt-1">
        <div className="col-10 m-0 p-0 pr-2">
          <select
            size="4"
            ref={summaryBox}
            className="KommunListbox2 buttonHidden ml-1"
            multiple
          ></select>{" "}
        </div>
        <div className="col-2 m-0 p-0 pr-2">
          <input
            type="submit"
            onClick={addRouteData}
            className="KSPU_button "
            value="Velg"
            l
          />
        </div>
      </div>
      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 pt-1">
        <div className="col-10 m-0 p-0 pr-2">
          <span className="label p-1">Valgte budruter </span>
        </div>
        <div className="col-2 m-0 p-0 pr-2"></div>
      </div>
      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 pt-1">
        <div className="col-9 m-0 p-0 pr-2">
          <select
            size="4"
            ref={resultBox}
            className="KommunListbox2 buttonHidden"
            multiple
          ></select>{" "}
        </div>
        <div className="col-3 m-0 p-0 pr-2">
          <div className="col-3 m-0 p-0 pr-2">
            <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
              <input
                type="submit"
                className="KSPU_button"
                disabled={disableBtn}
                value="Vis i kart"
                onClick={handleMap}
              />
            </div>
            <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 pt-2">
              <input
                type="submit"
                onClick={handleRemoveClick}
                disabled={disableBtn}
                className="KSPU_button"
                value="Fjern"
              />
            </div>
          </div>
        </div>
      </div>
      <MottakerComponent page="DTPage" marginTop=".3rem" />
      <Geographie_footer name=" Budrute" />
    </div>
  );
}

export default Budrute;
