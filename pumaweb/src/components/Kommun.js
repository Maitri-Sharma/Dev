import React, { useState, useEffect, useRef, useContext } from "react";
import MottakerComponent from "./Mottakergrupper";
import "../App.css";
import Submit_Button from "./Submit_Button";
import Geographie_footer from "./Geographie_footer";
import api from "../services/api.js";
import { UtvalgContext } from "../context/Context.js";
import { KSPUContext } from "../context/Context.js";
import { MainPageContext } from "../context/Context.js";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";

function Kommun() {
  const [datalist, setData] = useState([]);

  const [selectedData, setselectedData] = useState([]);
  const [disableBtn, setBtnDisable] = useState(true);
  const { searchData, setSearchData } = useContext(UtvalgContext);
  const {
    searchURL,
    setSearchURL,
    setActivUtvalg,
    setShowReservedHouseHolds,
    setShowBusiness,
    setShowHousehold,
  } = useContext(KSPUContext);
  const inputEl = useRef();
  const resultBox = useRef();
  const selectedBox = useRef();
  const { mapView } = useContext(MainPageContext);

  const { highlight, setHighlight } = useContext(MainPageContext);

  useEffect(() => {
    fetchData();
    setShowReservedHouseHolds(false);
    setShowBusiness(false);
    setShowHousehold(true);
    setActivUtvalg({});
  }, []);

  const fetchData = async () => {
    try {
      const { data, status } = await api.getdata("Kommune/GetAllKommunes");
      if (status === 200) {
        setData(data);
      } else {
        console.error("error : " + status);
      }
    } catch (error) {
      console.error("er : " + error);
    }
  };

  const handlenextchange = (e) => {
    //debugger;
    var resultSelect = resultBox.current;
    resultSelect.options.length = 0;
    resultSelect.style.display = "block";
    if (e.target.value == "") {
      if (resultSelect.options.length > 0)
        resultSelect.options.splice(resultSelect.options.length - 1, 0);
      return;
    }
    for (var i = 0; i < datalist.length; i++) {
      var txt = datalist[i].kommuneName.toUpperCase();

      if (txt.toLowerCase().startsWith(e.target.value.toLowerCase())) {
        var newOption = new Option(txt, datalist[i].kommuneID);
        resultSelect.add(newOption, resultSelect.length);
      }
    }
  };

  const handleBoxClick = (e) => {
    inputEl.current.value = e.target.text;
    resultBox.current.style.display = "none";
  };
  const handleSubmit = (e) => {
    if (
      !Array.from(selectedBox.current.options)
        .map((opt) => opt.text)
        .includes(inputEl.current.value)
    ) {
      var res = datalist.filter(
        (item) => item.kommuneName === inputEl.current.value
      );

      var newOption = new Option(res[0].kommuneName, res[0].kommuneID);
      selectedBox.current.add(newOption, resultBox.current.length);

      //Adding logic for kommune Addition for Search
      let newArray = [...searchData, res[0].kommuneID];
      if (searchData.includes(res[0].kommuneID)) {
        newArray = newArray.filter((j) => j !== res[0].kommuneID);
      }
      setSearchData(newArray);
      setSearchURL("Reol/GetReolsInKommune?kommuneId=");
    }
    setBtnDisable(false);
  };
  const handleRemoveClick = (e) => {
    Array.from(selectedBox.current.options)
      .filter((x) => x.selected)
      .map((d) => {
        selectedBox.current.remove(d.index);

        //Adding logic for removal of kommune from Search
        let newArray = searchData.filter((j) => j !== d.value);
        setSearchData(newArray);
      });

    if (selectedBox.current.length === 0) {
      setBtnDisable(true);
    }
  };

  const handleMap = async () => {
    let routeids = [];
    let searchArrayData = Array.from(selectedBox.current.options);
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

      let k = searchArrayData
        .map((element) => "'" + element.value + "'")
        .join(",");
      let kommuneWhereClause = `kommuneId in (${k})`;
      
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

      queryObject.where = `${kommuneWhereClause}`;
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
            //mapView.graphics.removeAll();

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
      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 ">
        <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
          <div className="col-10 m-0 p-0 pr-2">
            <i className="fa fa-user-circle-o pl-1"></i>

            <input
              ref={inputEl}
              id="inputBOx"
              type="text"
              className="KommunInputText mt-1"
              onChange={handlenextchange}
              placeholder=""
            />
          </div>
          <div className="col-2 m-0 p-0 ">
            <input
              type="submit"
              className="KSPU_button mt-1"
              onClick={handleSubmit}
              value="Velg"
            />
          </div>
        </div>
        <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 ">
          <div className="col-10 m-0 p-0 pr-2">
            <select
              ref={resultBox}
              id="resultBox"
              size="3"
              onClick={handleBoxClick}
              className="KommunListbox buttonHidden mt-1"
              multiple
            ></select>
          </div>
          <div className="col-2 m-0 p-0 pr-2"></div>
        </div>
        <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 ">
          <div className="col-10 m-0 p-0 pr-2">
            <span className="label p-1">Valgte kommuner </span>
          </div>
          <div className="col-2 m-0 p-0 pr-2"></div>
        </div>
        <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 ">
          <div className="col-9 m-0 p-0 pr-2">
            {" "}
            <select
              ref={selectedBox}
              size="4"
              className="KommunListbox1 buttonHidden mt-1"
              multiple
            ></select>
          </div>
          <div className="col-3 m-0 p-0 pr-2">
            <div className="col-12 m-0 p-0 pt-1">
              {" "}
              <input
                type="submit"
                className="KSPU_button pl-1 mr-2"
                value="Vis i kart"
                disabled={disableBtn}
                onClick={handleMap}
              />
            </div>
            <div className="col-12 m-0 p-0 pt-2">
              <input
                type="submit"
                className="KSPU_button"
                value="Fjern"
                onClick={handleRemoveClick}
                disabled={disableBtn}
              />
            </div>
          </div>
        </div>
      </div>

      <MottakerComponent page="DTPage" marginTop=".3rem" />
      <Geographie_footer
        checkname1=" Hele kommunen(e)"
        checkname2=" Plukkliste"
        name=""
      />
    </div>
  );
}

export default Kommun;
