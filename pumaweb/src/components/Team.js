import React, { useState, useRef, useEffect, useContext } from "react";
import MottakerComponent from "./Mottakergrupper";
import "../App.css";
import Geographie_footer from "./Geographie_footer";
import { UtvalgContext } from "../context/Context.js";
import api from "../services/api.js";
import { KSPUContext } from "../context/Context.js";
import { MainPageContext } from "../context/Context.js";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";
import Spinner from "./spinner/spinner.component";
function Team() {
  const inputElment = useRef(null);
  const dataBox = useRef(null);
  const summaryBox = useRef(null);
  const [disableBtn, setBtnDisable] = useState(true);
  const { searchData, setSearchData } = useContext(UtvalgContext);
  const [loading, setloading] = useState(false);
  const {
    setSearchURL,
    setActivUtvalg,
    setShowHousehold,
    setShowBusiness,
    setShowReservedHouseHolds,
  } = useContext(KSPUContext);
  const { mapView } = useContext(MainPageContext);
  const [teamArray, setTeamArray] = useState([]);
  useEffect(() => {
    setActivUtvalg({});
    setShowReservedHouseHolds(false);
    setShowBusiness(false);
    setShowHousehold(true);
  }, []);
  const fetchData = async () => {
    try {
      const { data, status } = await api.getdata(
        `Team/SearchTeam?teamNavn=${inputElment.current.value}`
      );
      if (status === 200) {
        if (data.length > 0) {
          // summaryBox.current.options
          if (dataBox.current.options.length > 0) {
            Array.from(dataBox.current.options).map((d) => {
              dataBox.current.remove(d.index);
            });
          }
          data.map(function (d, i) {
            var newOption = new Option(d.teamName, d.teamName);
            dataBox.current.add(newOption, i);
          });
        }
      } else {
        console.error("error : " + status);
      }
    } catch (error) {
      console.error("er : " + error);
    }
  };

  function addData() {
    const optionLabels = Array.from(summaryBox.current.options).map(
      (opt) => opt.value
    );
    let newArray = teamArray;
    for (var i = 0; i < dataBox.current.selectedOptions.length; i++) {
      if (!optionLabels.includes(dataBox.current.selectedOptions[i].value)) {
        var newOption = new Option(
          dataBox.current.selectedOptions[i].label,
          dataBox.current.selectedOptions[i].value
        );
        summaryBox.current.add(newOption, i);

        //Adding logic for Teams Addition for Search

        if (searchData.includes(dataBox.current.selectedOptions[i].value)) {
          newArray = newArray.filter(
            (j) => j !== dataBox.current.selectedOptions[i].value
          );
        }
        newArray.push(dataBox.current.selectedOptions[i].value);
      }
    }

    setTeamArray(newArray);
    setSearchData(newArray);
    setSearchURL("Reol/GetReolsInTeam");
    setBtnDisable(false);
  }

  const handleRemoveClick = (e) => {
    Array.from(summaryBox.current.options)
      .filter((x) => x.selected)
      .map((d) => {
        summaryBox.current.remove(d.index);

        //Adding logic for removal of Postnr from Search
        let newArray = searchData.filter((j) => j !== d.value);
        setSearchData(newArray);
      });
    if (summaryBox.current.length === 0) {
      setBtnDisable(true);
    }
  };

  const handleMap = async () => {
    setloading(true);
    let searchArrayData = Array.from(summaryBox.current.options);
    if (searchArrayData.length > 0) {
      let t = searchArrayData
        .map((element) => "'" + element.value + "'")
        .join(",");
      let teamWhereClause = `teamnavn in (${t})`;

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

      //Get ObjectIDs
      const queryOIDs = new Query();
      queryOIDs.where = `${teamWhereClause}`;
      let oids = await query.executeForIds(BudruterUrl, queryOIDs);

      //build query block for more than 2000 ObjectIDs
      let times = null;
      let quotient = Math.floor(oids.length / 2000);
      let remainder = oids.length % 2000;
      let startIndex = 0;
      let endIndex = 2000;
      let promise = [];

      if (quotient < 1) {
        times = 1;
        endIndex = oids.length;
      } else {
        if (remainder === 0) {
          times = quotient;
        } else {
          times = quotient + 1;
        }
      }

      for (let i = 0; i < times; i++) {
        if (i > 0) {
          startIndex = startIndex + 2000;
          endIndex = endIndex + 2000;
        }
        if (i === times - 1) {
          endIndex = oids.length;
        }

        let objectsIds = oids.slice(startIndex, endIndex);

        const queryResults = new Query();

        queryResults.outFields = "reol_id";
        queryResults.where = "OBJECTID IN (" + objectsIds.join(",") + ")";
        queryResults.outSpatialReference = mapView.spatialReference;
        queryResults.returnGeometry = true;

        promise[i] = query.executeQueryJSON(BudruterUrl, queryResults);
      }

      Promise.all(promise).then((values) => {
        for (let i = 0; i < values.length; i++) {
          for (let j = 0; j < values[i].features.length; j++) {
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

            let graphic = new Graphic(
              values[i].features[j].geometry,
              selectedSymbol,
              values[i].features[j].attributes
            );
            mapView.graphics.add(graphic);
          }
        }

        setloading(false);
      });
    }
  };
  const handleKeypress = (e) => {
    //it triggers by pressing the enter key
    if (e.key === "Enter") {
      fetchData();
    }
  };

  return (
    <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
      <div>{loading ? <Spinner /> : null}</div>
      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <div className="col-10 m-0 p-0 pr-2">
          <i className="fa fa-user-circle-o pl-1"></i>
          <input
            type="text"
            ref={inputElment}
            className="TeamInputText mt-1"
            placeholder=""
            onKeyPress={handleKeypress}
          />
        </div>
        <div className="col-2 m-0 p-0 ">
          {" "}
          <input
            type="submit"
            onClick={fetchData}
            className="KSPU_button mt-1"
            value="Søk"
          />
        </div>
      </div>
      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 pt-1">
        <div className="col-10 m-0 p-0 pr-2">
          <select
            size="5"
            ref={dataBox}
            className="KommunListbox2 buttonHidden ml-1"
            multiple
          ></select>
        </div>
        <div className="col-2 m-0 p-0 pr-2">
          <input
            type="submit"
            onClick={addData}
            className="KSPU_button"
            value="Velg"
            l
          />
        </div>
      </div>
      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 pt-1">
        <div className="col-10 m-0 p-0 pr-2">
          <span className="label p-1">Valgte team </span>
        </div>
        <div className="col-2 m-0 p-0 pr-2"></div>
      </div>
      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 pt-1">
        <div className="col-9 m-0 p-0 pr-2">
          <select
            size="5"
            ref={summaryBox}
            className="KommunListbox2 buttonHidden"
            multiple
          ></select>
        </div>
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

      <MottakerComponent page="DTPage" marginTop=".3rem" />
      <Geographie_footer
        checkname1=" Hele team(ene)"
        checkname2=" Plukkliste"
        name=""
      />
    </div>
  );
}

export default Team;
