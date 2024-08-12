import React, { useRef, useEffect, useContext, useState } from "react";
import { MainPageContext } from "../../context/Context";
import "./AdressepunktResultatModelPopup.styles.scss";
import Graphic from "@arcgis/core/Graphic";
import TextSymbol from "@arcgis/core/symbols/TextSymbol";
function AdressepunktResultatModelPopup(props) {
  const [displayMsg, setDisplayMsg] = useState(false);
  const { addressPoints, setAddressPoints } = useContext(MainPageContext);
  const { mapView } = useContext(MainPageContext);
  const { budruterMapView, setbudruterMapview } = useContext(MainPageContext);

  const zoomTil = (graphics) => {
    var geo = mapView.center;
    geo.x = graphics.x;
    geo.y = graphics.y;
    mapView.goTo({
      geometry: geo,
      zoom: 11,
    });
  };

  const Velg = (item) => {
    let val = [];
    val.push(item);
    addressPoints.push(item.feature);
    setAddressPoints(addressPoints);
    setbudruterMapview(item);
    sessionStorage.setItem("addressPoints", JSON.stringify(addressPoints));
    //mapView.graphics.removeAll();
    mapView.graphics.add(
      new Graphic({
        attributes: item.feature.attributes, // Data attributes returned
        geometry: item.feature.geometry, // Point returned,
        symbol: {
          type: "simple-marker",
          color: "blue",
          size: "12px",
          outline: {
            color: "blue",
            width: "2px",
          },
        },
        popupTemplate: {
          title: "{PlaceName}", // Data attribute names
          content: "{Place_addr}",
        },
      })
    );

    let labelAddressPoint = new Graphic({
      geometry: item.feature.geometry,
      symbol: {
        type: "text",
        color: "white",
        haloColor: "black",
        haloSize: "1px",
        text: item.feature.address,
        xoffset: 3,
        yoffset: 3,
        font: {
          size: 12,
          family: "sans-serif",
          weight: "bolder",
        },
      },
    });

    mapView.graphics.add(labelAddressPoint);

    let textSymbol = new TextSymbol({
      color: "white",
      haloColor: "black",
      haloSize: "1px",
      xoffset: 3,
      yoffset: 3,
      font: {
        // autocast as esri/symbols/Font
        size: 12,
        //family: "sans-serif",
        family: "Arial",
        weight: "bolder",
      },
      text: item.feature.attributes.Match_addr,
    });

    // add label to point graphicse
    const labelPoint = new Graphic({
      geometry: item.feature.geometry,
      symbol: textSymbol,
    });
    mapView.graphics.add(labelPoint);
    zoomTil(item.feature.geometry);

    // setAddressPoints(addressPoints.concat(val));

    document.getElementById("addressPunktResultatDiv").style.display = "none";
    document.getElementById("searchResultDiv").style.display = "none";
  };

  const closeModelPopup = () => {
    document.getElementById("addressPunktResultatDiv").style.display = "none";
    document.getElementById("searchResultDiv").style.display = "none";
  };

  return (
    <div>
      <div
        id="addressPunktResultatDiv"
        className="modal modal-content addressDiv"
        role="dialog"
        aria-labelledby="ModalCenterTitle"
        aria-hidden="true"
      >
        <div className="modal-header addresstitle">
          <h5 className="modal-title">SØKERESULTAT </h5>
          <button
            type="button"
            className="close"
            data-dismiss="modal"
            aria-label="Close"
            onClick={closeModelPopup}
          >
            <span aria-hidden="true">&times;</span>
          </button>
        </div>
        <div className="modal-body addressPunktContent">
          {displayMsg ? (
            <div className="addressPunktMsg">
              <span className="addressErrorText">
                Feil ved adressesøk: En feil oppsto ved adressesøk. Kontakt
                superbruker i Posten.
              </span>
            </div>
          ) : (
            <div>
              <table className="addressTable">
                <thead>
                  <tr className="addressHeader">
                    <th className="addressDataheader addressRow">Gate/sted</th>
                    <th className="addressDataheader addressRow">Husnummer</th>
                    <th className="addressDataheader addressRow">Score</th>
                    <th className="addressDataheader addressRow">Type</th>
                    <th className="addressDataheader addressRow">
                      &nbsp;&nbsp;&nbsp;&nbsp;
                    </th>
                  </tr>
                </thead>
                {props.searchResults !== undefined &&
                props.searchResults.length >= 0 ? (
                  <tbody>
                    {props.searchResults[0].results.map((item, i) => (
                      <tr key={i}>
                        <td className="addressContent addressDataheader">
                          {item.target.attributes.Match_addr}
                        </td>
                        <td className="addressContent addressDataheader">
                          {item.target.attributes.Husnummer}
                        </td>
                        <td className="addressContent addressDataheader">
                          {item.target.attributes.Score}
                        </td>
                        <td className="addressContent addressDataheader">
                          {item.target.attributes.Addr_type}
                        </td>
                        <td className="addressContent addressDataheader">
                          <p
                            id=""
                            href="#"
                            className="addressVelg float-right mr-1"
                            onClick={() => {
                              Velg(item);
                            }}
                            data-dismiss="modal"
                          >
                            velg
                          </p>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                ) : null}
              </table>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

export default AdressepunktResultatModelPopup;
