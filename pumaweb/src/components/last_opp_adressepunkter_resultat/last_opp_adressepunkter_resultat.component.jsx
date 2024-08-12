import React, { useState, useEffect, useContext } from "react";
import LastOppAdressepunkter from "../last_opp_adressepunkter/last_opp_adressepunkter.component";

import * as locator from "@arcgis/core/rest/locator";
import Graphic from "@arcgis/core/Graphic";
import { MapConfig } from "../../config/mapconfig";
import { geoKodingObj } from "../KspuConfig";
import Spinner from "../../components/spinner/spinner.component";

import { MainPageContext, KSPUContext } from "../../context/Context";

import loadingImage from "../../assets/images/kw/spinner.gif";
import "./last_opp_adressepunkter_resultat.styles.scss";

function LastOppAdressepunkterResultat(props, { parentCallback }) {
  const [currentStep, setCurrentStep] = useState(props.currentStep);
  const [totalCount, setTotalCount] = useState(0);
  const [updateInMap, setUpdateInMap] = useState(0);
  const { mapView } = useContext(MainPageContext);
  const { AddresslisteDisplay, setAddresslisteDisplay } =
    useContext(KSPUContext);
  const { addressPoints, setAddressPoints } = useContext(MainPageContext);
  const [loading, setLoading] = useState(false);
  const [showMessage, setShowMessage] = useState(false);
  const [message, setNewMessage] = useState("notHappy");

  let resultsArray = [];
  let wrongAddressArray = [];
  let countMap = 0;

  let addressCount = 0;
  const findPlaces = (addressObj, butikkDisplayName) => {
    setLoading(true);
    addressCount = addressCount + 1;
    let options = {
      addresses: addressObj,
    };

    locator
      .addressesToLocations(MapConfig.geoKodingUrl, options)
      .then(function (results) {
        results.map((item) => {
          resultsArray.push(item);
          if (item.score === 0) {
            wrongAddressArray.push(butikkDisplayName);
          }
        });

        setAddressPoints(addressPoints.concat(resultsArray));
        sessionStorage.setItem(
          "addressPoints",
          JSON.stringify(addressPoints.concat(resultsArray))
        );

        mapView.popup.close();

        let addressPointGeometry = [];

        results.forEach(function (result) {
          result.attributes["display"] = butikkDisplayName;
        });

        results.forEach(function (result) {
          if (
            result.location.x !== "NaN" &&
            result.location.x !== 0 &&
            result.location.y !== "NaN" &&
            result.location.y !== 0
          ) {
            countMap = countMap + 1;
            addressPointGeometry.push(result.location);

            mapView.graphics.add(
              new Graphic({
                attributes: result.attributes, // Data attributes returned
                geometry: result.location, // Point returned
                symbol: {
                  type: "simple-marker",
                  color: "blue",
                  size: "10px",
                  outline: {
                    color: "blue",
                    width: "1px",
                  },
                },

                popupTemplate: {
                  title: "{PlaceName}", // Data attribute names
                  content: "{Place_addr}",
                },
              })
            );
            let displayName = result.address;
            if (result?.attributes?.display) {
              displayName = result.attributes.display;
            }
            let labelAddressPoint = new Graphic({
              geometry: result.location,
              symbol: {
                type: "text",
                color: "white",
                haloColor: "black",
                haloSize: "1px",
                text: displayName,
                xoffset: 5,
                yoffset: 5,
                font: {
                  // autocast as esri/symbols/Font
                  size: 10,
                  family: "sans-serif",
                  weight: "bolder",
                },
              },
            });

            mapView.graphics.add(labelAddressPoint);
          }
        });
        if (
          props.cityArray?.length === addressCount &&
          wrongAddressArray.length > 0
        ) {
          setShowMessage(true);
          let msg = `${wrongAddressArray?.map((item) => {
            return " " + item;
          })} `;

          setNewMessage(msg);
        }
        mapView.goTo(addressPointGeometry);
        setTotalCount(props.cityArray?.length);
        setUpdateInMap(countMap);
        setLoading(false);
      });
  };
  const arrayDataItems = (wrongAddressArray, index) => {
    return wrongAddressArray.map((item) => <span>{item.Adresse}</span>);
  };

  useEffect(async () => {
    for (let i = 0; i < props.cityArray.length; i++) {
      let addressObj = geoKodingObj();
      addressObj.OBJECTID = i;
      addressObj.Postnummer = props.postNrArray[i];
      addressObj.Poststed = props.cityArray[i];
      addressObj.Adresse = props.addressArray[i];
      await findPlaces(addressObj, props.butikkArray[i]);
    }
  }, []);

  const nextClick = async () => {
    await setAddresslisteDisplay(false);
    await setAddresslisteDisplay(true);
  };

  const callback = (step) => {
    setCurrentStep(step - 1);
  };
  const handlePrevClick = () => {
    let j = mapView.graphics.items.length;

    for (var i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
        //j++;
      }
    }
    //mapView.graphics.removeAll();
    props.parentCallback(props.currentStep);
  };

  return (
    <div>
      <div>{loading ? <Spinner /> : null}</div>
      <div className="card">
        {currentStep === 3 ? (
          <div className="row">
            {loading ? (
              <img
                src={loadingImage}
                alt="loading"
                style={{
                  width: "50px",
                  height: "50px",
                  position: "absolute",
                  left: "100px",
                  zIndex: 100,
                }}
              />
            ) : null}
            {updateInMap ? (
              <div className="addressPoint">
                <span>
                  Stedfestet {updateInMap} av {totalCount} adresser. Adressene
                  er lagt til i vinduet "Mine adressepunkter" og kan brukes
                  videre i kjøreanalyse eller til å finne budruter rundt
                  adressepunkter.
                </span>
              </div>
            ) : null}

            {showMessage ? (
              <div className="wrongAddressPoint">
                <span>Find below wrong butikkName in addressFile: </span>
                <br />
                <span>{message}</span>
              </div>
            ) : null}
            <div className="row col-12 m-0 p-0 mt-5 mb-2">
              <div className="col-12 ml-1">
                <input
                  type="submit"
                  id="uxBtForrige"
                  className="KSPU_button"
                  value="<< Forrige"
                  onClick={handlePrevClick}
                  style={{
                    visibility: currentStep > 1 ? "visible" : "hidden",
                    text: "",
                  }}
                />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input
                  type="submit"
                  id="uxBtnNeste"
                  className="KSPU_button float-right mr-2"
                  value="Last opp flere adresser"
                  onClick={nextClick}
                  style={{
                    display: currentStep < 4 ? "block" : "none",
                  }}
                />
              </div>
            </div>
          </div>
        ) : currentStep === 1 ? (
          <LastOppAdressepunkter
            currentStep={currentStep}
            parentCallback={callback}
          />
        ) : null}
      </div>
    </div>
  );
}

export default LastOppAdressepunkterResultat;
