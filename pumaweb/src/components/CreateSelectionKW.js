import React, { useEffect, useState, useContext } from "react";
import "../App.css";
import Globe from "../assets/images/icons_kw/geografisk.png";
import Car from "../assets/images/icons_kw/adresse.png";
import Segment from "../assets/images/icons_kw/segment.png";
import Demografie from "../assets/images/icons_kw/demografi.png";
import velgPlus from "../assets/images/icons_kw/KNAPP DISABLE-velg-plus.gif";
import norgeskart from "../assets/images/icons_kw/KNAPP-norgeskart.gif";
import KNAPPRED from "../assets/images/icons_kw/KNAPP RED-pluss.gif";
import KNAPPminus from "../assets/images/icons_kw/KNAPP-minus.gif";
import KNAPPpanorer from "../assets/images/icons_kw/KNAPP-panorer.gif";
import { KundeWebContext, MainPageContext } from "../context/Context.js";
import readmore from "../assets/images/read_more.gif";

function CreateSelectionKW({ parentCallback }) {
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);
  const [velg, setvelg] = useState("");
  const { Page, setPage } = useContext(KundeWebContext);
  const { mapView } = useContext(MainPageContext);
  const { newhome, setnewhome } = useContext(KundeWebContext);
  const { CartItems, setCartItems } = useContext(KundeWebContext);
  const { SelectedItemCheckBox_Budruter, setSelectedItemCheckBox_Budruter } =
    useContext(KundeWebContext);
  const { utvalglistapiobject, setutvalglistapiobject } =
    useContext(KundeWebContext);
  const { criteriaObject, setCriteriaObject } = useContext(KundeWebContext);

  useEffect(() => {
    window.scroll(0, 0);
    setbusinesscheckbox(false);
    setSelectedItemCheckBox_Budruter([]);
    if (mapView) {
      // remove previous highlighted feature
      let j = mapView.graphics.items.length;
      for (var i = j; i > 0; i--) {
        if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
          mapView.graphics.remove(mapView.graphics.items[i - 1]);
        }
      }
    }
  }, []);
  const Geogravelg = () => {
    setPage("Geovelg");
    setCriteriaObject({
      ...criteriaObject,
      enum: "19",
    });
  };

  const DemgrafiClick = () => {
    setPage("Demografivelg");
    setCriteriaObject({
      ...criteriaObject,
      enum: "12",
    });
  };
  const SegmenterClick = () => {
    setPage("Segmentervelg");
    setCriteriaObject({
      ...criteriaObject,
      enum: "2",
    });
  };
  const BudruterClick = () => {
    setPage("Budrutervelg");
    setCriteriaObject({
      ...criteriaObject,
      enum: "11",
    });
  };
  const ApneetClick = () => {
    setPage("ApneetlinkClick");
  };
  const LeggClick = () => {
    setPage("Sok_Component_kw");
    // setPage("ApneetlinkClick");
  };
  const cartClick = () => {
    setPage("cartClick_Component_kw");
  };

  return (
    <div className="col-5 pt-2 pt-2">
      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        {newhome ? (
          <div className="padding_NoColor_B" style={{ cursor: "pointer" }}>
            <a id="uxHandlekurvSmall_uxLnkbtnHandlekurv" onClick={cartClick}>
              <div className="handlekurv handlekurvText pl-2">
                Du har{" "}
                {CartItems.length > 0
                  ? CartItems.length
                  : utvalglistapiobject.memberUtvalgs?.length}{" "}
                utvalg i bestillingen din.
              </div>
            </a>
          </div>
        ) : null}
        {newhome ? <br /> : null}

        <span id="uxWelcomeHeading" className="title">
          Treff dine kunder mest effektivt!
        </span>
      </div>
      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        {newhome ? (
          <p className="p-text">
            {" "}
            Du har allerede laget 1 utvalg, men kan legge til så mange du ønsker
            før du sender bestillingen.
            <br />
            <b>Velg hvordan du vil lage neste utvalg:</b>
          </p>
        ) : (
          <p className="p-text">
            Her kan du finne den kundetypen du ønsker å nå ut til og bestille
            din uadresserte reklameutsendelse.
          </p>
        )}
      </div>
      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <hr className="MenuUpperLine mt-1" />
      </div>

      <div className="row">
        <div className="col-lg-1 col-md-2 col-sm-2 col-xs-2 m-0 p-0 pr-1">
          <img
            src={Globe}
            alt="Geografianalyse"
            className="cursor"
            title="Geografianalyse"
            onClick={Geogravelg}
          />
        </div>
        <div className="col-lg-8 col-md-7 col-sm-7 col-xs-7 m-0 p-0 pl-3 mt-2">
          <span className="titleWizard cursor" onClick={Geogravelg}>
            {" "}
            Geografisk område
          </span>
        </div>
        <div className="col-lg-3 col-md-3 col-sm-3 col-xs-3 m-0 p-0 pr-3 mt-2 text-right">
          {/* <input
            type="button"
            className="KSPU_button_right"
            onClick={Geogravelg}
            value="Velg"
          /> */}
        </div>
      </div>
      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 p-text">
        <span>
          Velg hvor i landet du vil sende reklamen din. Du kan velge hele fylker
          og kommuner, eller budruter fra liste. Du kan også velge et område for
          utsendelse ved å markere direkte i kartet. Du kan velge ut fra fylke,
          kommune, budrute eller rett i kartet.
        </span>
      </div>
      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <hr className="MenuUpperLine mt-1" />
      </div>
      <div className="row ">
        <div className="col-lg-1 col-md-2 col-sm-2 col-xs-2 m-0 p-0 pr-1">
          <img
            src={Car}
            alt="Velg budruter nær en adresse"
            title="Geografianalyse"
            className="cursor"
            onClick={BudruterClick}
          />
        </div>
        <div className="col-lg-8 col-md-7 col-sm-7 col-xs-7 m-0 p-0 pl-3 mt-2">
          <span className="titleWizard pl-2 cursor" onClick={BudruterClick}>
            Budruter nær en adresse
          </span>
        </div>
        <div className="col-lg-3 col-md-3 col-sm-3 col-xs-3 m-0 p-0 pr-3 mt-2 text-right">
          {/* <input
            type="button"
            className="KSPU_button_right"
            onClick={BudruterClick}
            value="Velg"
          /> */}
        </div>
      </div>

      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 mt-1 p-0 p-text">
        <span>
          Angi en adresse og finn de mest aktuelle budrutene for ditt bruk. Du
          kan velge utsendelsen basert på både kjøretid i minutter, kjøreavstand
          (km) eller antall mottakere.
        </span>
      </div>
      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <hr className="MenuUpperLine mt-1" />
      </div>

      {/* <div className="row">
        <div className="col-lg-1 col-md-2 col-sm-2 col-xs-2 m-0 p-0 pr-1">
          <img
            src={Segment}
            alt="Velg budruter nær en adresse"
            title="Geografianalyse"
            onClick={SegmenterClick}
            className="cursor"
          />
        </div>
        <div className="col-lg-8 col-md-7 col-sm-7 col-xs-7 m-0 p-0 pl-3 mt-2">
          <span className="titleWizard cursor " onClick={SegmenterClick}>
            Segmenter
          </span>
        </div>
        <div className="col-lg-3 col-md-3 col-sm-3 col-xs-3 m-0 p-0 pr-3 mt-2 text-right">
          {/* <input
            type="button"
            className="KSPU_button_right"
            onClick={SegmenterClick}
            value="Velg"
          /> */}
      {/* </div>
      </div>  */}

      <div className="row">
        <div className="col-lg-1 col-md-2 col-sm-2 col-xs-2 m-0 p-0 pr-1">
          <img
            src={Demografie}
            alt="Demografianalyse"
            title="Geografianalyse"
            onClick={DemgrafiClick}
            className="cursor"
          />
        </div>
        <div className="col-lg-8 col-md-7 col-sm-7 col-xs-7 m-0 p-0 pl-3 mt-2">
          <span className="titleWizard cursor" onClick={DemgrafiClick}>
            Demografi
          </span>
        </div>
        <div className="col-lg-3 col-md-3 col-sm-3 col-xs-3 m-0 p-0 pr-3 mt-2 text-right">
          {/* <input
            type="button"
            className="KSPU_button_right"
            onClick={DemgrafiClick}
            value="Velg"
          /> */}
        </div>
      </div>
      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 mt-1 p-0 p-text">
        <span>
          Du kan finne kunder ut fra demografiske beskrivelser som kjønn og
          alder, utdanningsnivå, boligtype, inntekt eller bilmerke
        </span>
      </div>
      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <hr className="MenuUpperLine mt-1" />
      </div>

      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        {newhome ? <br /> : null}
        <img src={readmore} />
        &nbsp;
        {newhome ? (
          <a
            className="KSPU_LinkButton1_Url"
            style={{ margin: "0px" }}
            onClick={LeggClick}
          >
            LEGG TIL ET LAGRET utvalg
          </a>
        ) : (
          <a className="KSPU_LinkButton1_Url pl-1" onClick={ApneetClick}>
            <b>ÅPNE ET LAGRET UTVALG</b>
          </a>
        )}
      </div>
      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <a
          id="uxBtnAddUtvalg"
          className="KSPU_LinkButton1_Url prevmnd"
          target="_parent"
          href={process.env.REACT_APP_CREATE_SELECTION_PAGE_LINK_URL}
        >
          <b>Gå til Butikkforsiden</b>
        </a>
      </div>
    </div>
  );
}
export default CreateSelectionKW;
