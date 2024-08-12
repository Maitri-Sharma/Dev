import React, { useState, useContext } from "react";
import { KundeWebContext } from "../../context/Context";
import { MainPageContext } from "../../context/Context.js";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";

export const MapRenderFun = async (Reolids, colorcode) => {
  const { mapView } = useContext(MainPageContext);

  if (Reolids.length > 0) {
    let k = Reolids.map((element) => "'" + element + "'").join(",");
    let sql_geography = `reol_id in (${k})`;
    let BudruterUrl;

    let allLayersAndSublayers = mapView.map.allLayers.flatten(function (item) {
      return item.layers || item.sublayers;
    });

    allLayersAndSublayers.items.forEach(function (item) {
      if (item.title === "Budruter") {
        BudruterUrl = item.url;
      }
    });
    const kommuneName = await GetAllBurdruter();
    async function GetAllBurdruter() {
      let queryObject = new Query();
      queryObject.where = `${sql_geography}`;
      queryObject.returnGeometry = true;
      queryObject.outFields = ["tot_anta", "hh", "hh_res", "reol_id"];

      await query
        .executeQueryJSON(BudruterUrl, queryObject)
        .then(function (results) {
          if (results.features.length > 0) {
            let featuresGeometry = [];
            let selectedSymbol = {
              type: "simple-fill", // autocasts as new SimpleFillSymbol()
              // color: [237, 54, 21, 0.25],
              color: colorcode,
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
            // mapView.graphics.removeAll();
            for (var i = j; i > 0; i--) {
              if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
                mapView.graphics.remove(mapView.graphics.items[i - 1]);
                //j++;
              }
            }

            results.features.map((item) => {
              featuresGeometry.push(item.geometry);
              let graphic = new Graphic(item.geometry, selectedSymbol);
              mapView.graphics.add(graphic);
            });

            mapView.goTo(featuresGeometry);
          }
        });
    }
  }
};
