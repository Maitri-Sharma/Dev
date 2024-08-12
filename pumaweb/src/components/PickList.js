import React, { useState, useEffect, useContext } from "react";
import TableNew from "./TableNew.js";
import { KSPUContext } from "../context/Context.js";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";
import { MainPageContext } from "../context/Context.js";
import { GetData, groupBy } from "../Data";
export default function PickList() {
  const { resultData, setResultData } = useContext(KSPUContext);
  const { searchURL } = useContext(KSPUContext);
  const {
    picklistData,
    setPicklistData,
    showHousehold,
    showBusiness,
    showReservedHouseHolds,
  } = useContext(KSPUContext);
  const [selectedRows, setSelectedRows] = useState([]);
  const { mapView } = useContext(MainPageContext);
  const [households, setHouseholds] = useState([]);
  const [householdsReserved, setHouseholdsReserved] = useState([]);
  const [outputData, setOutputData] = useState([]);
  let selectedKeys = [];

  useEffect(() => {
    selectedKeys = [];
    outputData.map((item, i) => {
      if (!item.children) {
        selectedKeys.push(item.key);
      }
    });
    setPicklistData(selectedKeys);
  }, [outputData]);

  useEffect(() => {
    let data = resultData.sort(function (a, b) {
      return Intl.Collator("no", { numeric: true }).compare(a.name, b.name);
    });
    if (data.length > 0) {
      data.map((childObj) => {
        if (childObj?.children?.length > 0) {
          childObj.children = childObj?.children.sort(function (a, b) {
            return Intl.Collator("no", { numeric: true }).compare(
              a.name,
              b.name
            );
          });
        }
      });
    }

    setResultData(data);
    let keys = getChildrenRoute(resultData);
    setSelectedRows(keys);
    setPicklistData(keys);
  }, []);

  const getChildrenRoute = (data) => {
    let selectedRoutekeys = data.reduce((acc, dt) => {
      if (!(dt.children === undefined)) {
        return acc.concat(getChildrenRoute(dt.children));
      }
      return acc.concat(dt.key);
    }, []);
    return selectedRoutekeys;
  };

  const handlemap = async () => {
    if (picklistData.length > 0) {
      let r = picklistData.map((element) => "'" + element + "'").join(",");
      let reolsWhereClause = `reol_id in (${r})`;
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

      queryObject.where = `${reolsWhereClause}`;
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

  const callback = (SelectedRecords, selected, selectedRows) => {
    let houseValue = households;
    if (!selected && houseValue.includes(SelectedRecords.house)) {
      houseValue = houseValue.filter((item) => {
        return item != SelectedRecords.house;
      });
    } else {
      houseValue.push(SelectedRecords.house);
    }
    setHouseholds(houseValue);

    let reservValue = householdsReserved;
    if (!selected && reservValue.includes(SelectedRecords.householdsReserved)) {
      reservValue = reservValue.filter((item) => {
        return item != SelectedRecords.householdsReserved;
      });
    } else {
      reservValue.push(SelectedRecords.householdsReserved);
    }

    setHouseholdsReserved(reservValue);
  };

  const gettitle = (url) => {
    if (url.includes("Fylke")) {
      return "Fylke\\Kommune\\Team\\Rute";
    } else if (url.includes("Kommune")) {
      return "Kommune\\Team\\Rute";
    } else if (url.includes("Team")) {
      return "Team\\Rute";
    } else {
      return "Fylke\\Kommune\\Team\\Rute";
    }
  };

  const columns = [
    {
      title: gettitle(searchURL),
      dataIndex: "name",
      key: "key",
      sorter: (a, b) => {
        return;
      },
      sortOrder: "ascend",
      sortDirections: ["ASC", "DESC"],
    },
    {
      title: "Antall",
      dataIndex: "total",
      key: "total",
      align: "right",
    },
  ];

  return (
    <>
      <div
        className="overflow-auto"
        style={{ height: "450px", overflowY: "scroll" }}
      >
        <TableNew
          width1={""}
          columnsArray={columns}
          data={resultData}
          setoutputDataList={setOutputData}
          setSelectedRows={setSelectedRows}
          defaultSelectedColumn={selectedRows}
          hideselection={0}
          parentCallback={callback}
        />
      </div>

      <input
        type="submit"
        id="uxBtopdatkart"
        className="KSPU_button"
        value="Oppdater kart"
        onClick={handlemap}
        style={{
          float: "right",
          marginBottom: "5%",
        }}
      />
    </>
  );
}
