import React, { useState, useRef,useEffect, useContext } from "react";
import MottakerComponent from "./Mottakergrupper";
import "../App.css";
import Geographie_footer from "./Geographie_footer";
import api from "../services/api.js";
import { UtvalgContext } from "../context/Context.js";
import { KSPUContext } from "../context/Context.js";
import { MainPageContext } from "../context/Context.js";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";

function Postnr() {
  const searchPostnr = useRef(null);
  const searchFPostnr = useRef(null);
  const searchTPostnr = useRef(null);
  const summaryBox = useRef(null);
  const [disableBtn, setBtnDisable] = useState(true);
  const { searchData, setSearchData } = useContext(UtvalgContext);
  const { searchURL, setSearchURL,setActivUtvalg,setShowHousehold,
    setShowBusiness,
    setShowReservedHouseHolds } = useContext(KSPUContext);
  const { layer } = useContext(MainPageContext);
  const { mapView } = useContext(MainPageContext);
  const [postnrArray,setPostnrArray] = useState([]);

  const { highlight, setHighlight } = useContext(MainPageContext);
  const handleKeypress = (e) => {
    //it triggers by pressing the enter key
    if (e.key === "Enter") {
      fetchPostnrData();
    }
  };
  useEffect(() => {
    setActivUtvalg({});
    setShowReservedHouseHolds(false);
    setShowBusiness(false);
    setShowHousehold(true);
  }, []);
  const handleKeypressPostnrFromToData = (e) => {
    //it triggers by pressing the enter key
    if (e.key === "Enter") {
      fetchPostnrFromToData();
    }
  };
  const fetchData = async (url) => {
    try {
      const { data, status } = await api.getdata(url);
      if (status === 200) {
        if (data.length > 0) {
          let newArray = postnrArray;
          data.map(function (d, i) {
            if (
              !Array.from(summaryBox.current.options)
                .map((opt) => opt.value)
                .includes(d.post_nr)
            ) {
              var newOption = new Option(d.diplayText, d.post_nr);
              summaryBox.current.add(newOption, i);
              newArray.push(d.post_nr);
            }
          });
          //Adding logic htmlFor Postnr Addition htmlFor Search
          setPostnrArray(newArray);
          setSearchData(newArray);
          setSearchURL("Reol/GetReolsInPostNr?postnummer=");
          setBtnDisable(false);
        }
      } else {
        console.error("error : " + status);
      }
    } catch (error) {
      console.error("er : " + error);
    }
  };

  function fetchPostnrData() {
    fetchData("Postnr/GetPostNr?postnr=" + searchPostnr.current.value);
  }

  function fetchPostnrFromToData() {
    fetchData(
      "Postnr/GetPostNrFromTo?PostnrFrom=" +
        searchFPostnr.current.value +
        "&PostnrTo=" +
        searchTPostnr.current.value
    );
  }
  const handleRemoveClick = (e) => {
    Array.from(summaryBox.current.options)
      .filter((x) => x.selected)
      .map((d) => {
        summaryBox.current.remove(d.index);

        //Adding logic htmlFor removal of Postnr from Search
        let newArray = searchData.filter((j) => j !== d.value);
        setSearchData(newArray);
      });
    if (summaryBox.current.length === 0) {
      setBtnDisable(true);
    }
  };

  const handleMap = async () => {
    let routeids = [];
    let searchArrayData = Array.from(summaryBox.current.options);
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

      let p = searchArrayData
        .map((element) => "'" + element.value + "'")
        .join(",");
      let postWhereClause = `postnr  in (${p})`;
      
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

      queryObject.where = `${postWhereClause}`;
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

  return (
    <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <div className="col-10 m-0 p-0 pr-2">
          <label className="form-check-label label-text" htmlFor="Hush">
            {" "}
            Postnr{" "}
          </label>
        </div>
        <div className="col-2 m-0 p-0 pr-2"></div>
      </div>

      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <div className="col-9 m-0 p-0 pr-2">
          <input
            type="text"
            ref={searchPostnr}
            className="KommunInputText"
            placeholder=""
            onKeyPress={handleKeypress}
          />
        </div>
        <div className="col-3 m-0 p-0 pr-2">
          <input
            type="submit"
            onClick={fetchPostnrData}
            className="KSPU_button"
            value="Legg til"
            l
          />
        </div>
      </div>

      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <div className="col-9 m-0 p-0 pr-2">
          <label className="form-check-label label-text" htmlFor="Hush">
            {" "}
            Postnr fra - til{" "}
          </label>
        </div>
        <div className="col-3 m-0 p-0 pr-2"></div>
      </div>

      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <div className="col-3 m-0 p-0 pr-2">
          <input
            type="text"
            ref={searchFPostnr}
            className="PostNr"
            placeholder=""
            // onKeyPress={handleKeypressPostnrFromToData}
          />
        </div>
        <div className="col-2 m-0 p-0 pr-2">-</div>
        <div className="col-3 m-0 p-0 pr-2">
          <input
            type="text"
            ref={searchTPostnr}
            className="PostNr"
            placeholder=""
            onKeyPress={handleKeypressPostnrFromToData}
          />
        </div>
        <div className="col-4 m-0 p-0 pr-2">
          <input
            type="submit"
            onClick={fetchPostnrFromToData}
            className="KSPU_button ml-3 "
            value="Legg til"
          />
        </div>
      </div>

      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <div className="col-9 m-0 p-0 pr-2">
          <span className="label p-1">Valgte postnr </span>
        </div>
        <div className="col-3 m-0 p-0 pr-2"></div>
      </div>

      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <div className="col-9 m-0 p-0 pr-2">
          <select
            size="5"
            ref={summaryBox}
            className="KommunListbox1 buttonHidden mt-1"
            multiple
          ></select>
        </div>
        <div className="col-3 m-0 p-0 pr-2">
          <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
            <input
              type="submit"
              className="KSPU_button pr-1"
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
      {/* <div>

<span className="label p-1" > 

	    Valgte postnr          </span>

<div className="row ml-1 pt-1">
<select size="4" className="KommunListbox2 buttonHidden ml-3" multiple>
</select>
<div className="col-2 pt-5 mr-2">
<br/>
<input type="submit" className="KSPU_button" value="Vis i kart" /><p></p>
 <input type="submit" className="KSPU_button" value="Fjern" />
</div>

</div>
</div> */}

      <MottakerComponent page="DTPage" marginTop=".3rem"/>
      <Geographie_footer
        name=""
        checkname1=" Alle budruter under valgte postnummer"
        checkname2=" Plukkliste"
      />
    </div>
  );
}

export default Postnr;
