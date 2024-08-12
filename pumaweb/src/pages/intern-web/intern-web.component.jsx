import React, { useState } from "react";

import SokComponentPage from "../../components/Sok";
import WebMapView from "../../components/webmap/webmapView";
import Denkn from "../../components/Denken";
import Rute from "../../components/Rute";
import Adress from "../../components/Adressepunkt_og_fastantallsanalyse/Adressepunkt_og_fastantallsanalyse.component";
import Demografie from "../../components/Demografie";
import Kj from "../../components/Kjøreanalyse/Kjøreanalyse.component";
import UtvalDetails from "../../components/UtvalDetails";
import { KSPUContext } from "../../context/Context";
import bring from "../../assets/images/logotop_bringNew.gif";
import puma from "../../assets/images/logo_PUMA 1.png";

import tabEnd from "../../assets/images/tabEnd.jpg";
import LastOppAdressepunkte from "../../components/last_opp_adressepunkter/last_opp_adressepunkter.component";
import SegmenterComponent from "../../components/Segmenter";
import GeografiComponents from "../../components/Geografianalyse";

import Arbeidsliste from "../../components/arbeidsliste-show/arbeidsliste";
import "./intern-web.styles.scss";

function InterWeb(props) {
  const [groupData, setGroupData] = useState({});
  const [showPriceCal, setshowPriceCal] = useState(false);
  const [showorklist, setshoworklist] = useState([]);
  const [checkedList, setCheckedList] = useState([]);
  const [activUtvalglist, setActivUtvalglist] = useState({});
  const [utvalglistcheck, setutvalglistcheck] = useState(false);
  const [resultData, setResultData] = useState([]);
  const [reolerData, setreolerData] = useState({});
  const [activUtvalg, setActivUtvalg] = useState({});
  const [LargUtvalg, setLargUtvalg] = useState({});
  const [picklistData, setPicklistData] = useState([]);
  const [searchURL, setSearchURL] = useState("");
  const [showHousehold, setShowHousehold] = useState(false);
  const [showBusiness, setShowBusiness] = useState(false);
  const [showDenking, setShowDenking] = useState(false);
  const [showReserverte, setShowReserverte] = useState(false);
  const [showReservedHouseHolds, setShowReservedHouseHolds] = useState(false);
  const [value, setvalue] = useState(true);
  const [AktivDisplay, setAktivDisplay] = useState(false);
  const [DenknDisplay, setDenknDisplay] = useState(false);
  const [RuteDisplay, setRuteDisplay] = useState(false);
  const [MapDisplay, setMapDisplay] = useState(true);
  const [KjDisplay, setKjDisplay] = useState(false);
  const [AdresDisplay, setAdresDisplay] = useState(false);
  const [DemografieDisplay, setDemografieDisplay] = useState(false);
  const [SegmenterDisplay, setSegmenterDisplay] = useState(false);
  const [AddresslisteDisplay, setAddresslisteDisplay] = useState(false);
  const [GeografiDisplay, setGeografiDisplay] = useState(false);
  const [MarkerDisplay, setMarkerDisplay] = useState(false);
  const [InfoDisplay, setInfoDisplay] = useState(false);
  const [VelgReolDisplay, setVelgReolDisplay] = useState(false);
  const [lagDisplay, setlagDisplay] = useState(false);
  const [PlussDisplay, setPlussDisplay] = useState(false);
  const [KnapMinusDisplay, setKnapMinusDisplay] = useState(false);
  const [PanorerDisplay, setPanorerDisplay] = useState(false);
  const [ForrigeDisplay, setForrigerDisplay] = useState(false);
  const [HeleNorgeDisplay, setHeleNorgeDisplay] = useState(false);
  const [selectedsegment, setselectedsegment] = useState([]);
  const [selectedName, setSelectedName] = useState([]);
  const [selectedKoummeIDs, setselectedKoummeIDs] = useState([]);
  const [HouseholdSum, setHouseholdSum] = useState([]);
  const [selectedDemografike, setSelectedDemografike] = useState([]);
  const [segmenterresultarray, setsegmenterresultarray] = useState([]);
  const [selectedRowKeys, setSelectedRowKeys] = useState([]);
  const [selectedrecord_s, setselectedrecord_s] = useState([]);
  const [pagekeys, setpagekeys] = useState([]);
  const [pagekeysseg, setpagekeysseg] = useState([]);
  const [pagekeysgeo, setpagekeysgeo] = useState([]);
  const [defaultSelectedColumn_s, setdefaultSelectedColumn_s] = useState([]);
  const [selecteddemografiecheckbox, setselecteddemografiecheckbox] = useState(
    []
  );
  const [selecteddemografiecheckbox_c, setselecteddemografiecheckbox_c] =
    useState([]);
  const [Demograresultarray, setDemograresultarray] = useState([]);
  const [errormsg, seterrormsg] = useState("");
  const [issave, setissave] = useState(false);
  const [antallValue, setAntallValue] = useState("");
  const [demografikmsg, setdemografikmsg] = useState(false);
  const [demografikAntalMsg, setDemografikAntalMsg] = useState(false);
  const [geograErrMsg, setGeograErrMsg] = useState(false);
  const [existingActive, setExistingActive] = useState(false);
  const [rutefoshkerVisited, setRutefoshkerVisited] = useState(false);
  const [
    rutefoshkerPreviousSelectedRutes,
    setrutefoshkerPreviousSelectedRutes,
  ] = useState([]);
  const [save, setSave] = useState(false);
  const [showcalendarmodal, setshowcalendarmodal] = useState(false);
  const [Selecteddate, setSelecteddate] = useState("");
  const [internuserName, setinternuserName] = useState("Internbruker");
  const [interntoken, setinterntoken] = useState("");
  const [Details, setDetails] = useState(false);
  const [basisUtvalg, setbasisUtvalg] = useState(false);
  const [isWidgetActive, setIsWidgetActive] = useState(false);
  const [demografikagemsg, setdemografikagemsg] = useState(false);
  const [mapattribute, setmapattribute] = useState([]);
  const [Budruteendringer, setBudruteendringer] = useState(false);
  const [SelectionUpdate, setSelectionUpdate] = useState(false);
  const [expandListId, setExpandListId] = useState([]);
  const [demographyIndex, setDemographyIndex] = useState({});
  const [globalBilTypeIW, setGlobalBilTypeIW] = useState(false);
  const [demoIndexArray, setDemoIndexArray] = useState([]);
  const [maintainUnsavedRute, setMaintainUnsavedRute] = useState([]);

  //useEffect(() => {setvalue("true")});
  const pageAbandon = () => {
    window.location.href = window.location.origin + "/Abandon";
  };
  const HomePage = () => {
    let path = window.location.origin;
    window.location.href = path;
  };
  const showcalendar = (e) => {
    setshowcalendarmodal(true);
  };
  const showModal = () => {
    setshowcalendarmodal(!showcalendarmodal);
  };
  const handleCallback = (childData) => {
    setSelecteddate(childData);
  };
  //useEffect(() => {setvalue("true")});
  const SokComponet = () => {
    setselectedsegment([]);
    setselecteddemografiecheckbox([]);
    setvalue(true);
    setAktivDisplay(false);
    setDenknDisplay(false);
    setMapDisplay(true);
    setRuteDisplay(false);
    setAdresDisplay(false);
    setDemografieDisplay(false);
    setSegmenterDisplay(false);
    setAddresslisteDisplay(false);
    setGeografiDisplay(false);
    setKjDisplay(false);
  };
  const AktivtUtvalgComponent = () => {
    setselecteddemografiecheckbox([]);
    setselectedsegment([]);
    setAktivDisplay(true);
    setMapDisplay(true);
    setDenknDisplay(false);
    setvalue(false);
    setRuteDisplay(false);
    setAdresDisplay(false);
    setDemografieDisplay(false);
    setSegmenterDisplay(false);
    setAddresslisteDisplay(false);
    setGeografiDisplay(false);
    setKjDisplay(false);
  };
  const DenknComponent = () => {
    setAktivDisplay(false);
    setvalue(false);
    setDenknDisplay(true);
    setMapDisplay(true);
    setRuteDisplay(false);
    setAdresDisplay(false);
    setDemografieDisplay(false);
    setSegmenterDisplay(false);
    setAddresslisteDisplay(false);
    setGeografiDisplay(false);
    setKjDisplay(false);
  };
  const ReolComponent = () => {
    setDenknDisplay(false);
    setAktivDisplay(false);
    setvalue(false);
    setRuteDisplay(true);
    setMapDisplay(true);
    setAdresDisplay(false);
    setDemografieDisplay(false);
    setSegmenterDisplay(false);
    setAddresslisteDisplay(false);
    setGeografiDisplay(false);
    setKjDisplay(false);
  };
  const KjoreAnalyse = () => {
    setKjDisplay(true);
    setAktivDisplay(false);
    setDenknDisplay(false);
    setMapDisplay(true);
    setvalue(false);
    setAdresDisplay(false);
    setRuteDisplay(false);
    setDemografieDisplay(false);
    setSegmenterDisplay(false);
    setAddresslisteDisplay(false);
    setGeografiDisplay(false);
    setselectedsegment([]);
    setselecteddemografiecheckbox([]);
  };
  const Segmenter = () => {
    setselectedsegment([]);
    setselecteddemografiecheckbox([]);
    setSegmenterDisplay(true);
    setKjDisplay(false);
    setAktivDisplay(false);
    setDenknDisplay(false);
    setMapDisplay(true);
    setvalue(false);
    setAdresDisplay(false);
    setRuteDisplay(false);
    setDemografieDisplay(false);
    setAddresslisteDisplay(false);
    setGeografiDisplay(false);
  };
  const Adres = () => {
    setselectedsegment([]);
    setselecteddemografiecheckbox([]);
    setAdresDisplay(true);
    setSegmenterDisplay(false);
    setAktivDisplay(false);
    setDenknDisplay(false);
    setvalue(false);
    setKjDisplay(false);
    setMapDisplay(true);
    setRuteDisplay(false);
    setDemografieDisplay(false);
    setAddresslisteDisplay(false);
    setGeografiDisplay(false);
  };
  const Demografiefun = () => {
    setselectedsegment([]);
    setselecteddemografiecheckbox([]);
    setselecteddemografiecheckbox_c([]);
    setDemoIndexArray([]);
    setDemograresultarray([]);
    setsegmenterresultarray([]);
    setpagekeysseg([]);
    setDemografieDisplay(true);
    setAktivDisplay(false);
    setDenknDisplay(false);
    setvalue(false);
    setKjDisplay(false);
    setMapDisplay(true);
    setRuteDisplay(false);
    setAdresDisplay(false);
    setSegmenterDisplay(false);
    setAddresslisteDisplay(false);
    setGeografiDisplay(false);
  };
  const Addressliste = () => {
    setDemografieDisplay(false);
    setAktivDisplay(false);
    setDenknDisplay(false);
    setvalue(false);
    setKjDisplay(false);
    setMapDisplay(true);
    setRuteDisplay(false);
    setAdresDisplay(false);
    setSegmenterDisplay(false);
    setAddresslisteDisplay(true);
    setGeografiDisplay(false);
  };
  const GeografiComponent = () => {
    setselectedsegment([]);
    setPicklistData([]);
    setGeograErrMsg(false);
    setselecteddemografiecheckbox([]);
    setDemografieDisplay(false);
    setAktivDisplay(false);
    setDenknDisplay(false);
    setvalue(false);
    setKjDisplay(false);
    setMapDisplay(true);
    setRuteDisplay(false);
    setAdresDisplay(false);
    setSegmenterDisplay(false);
    setAddresslisteDisplay(false);
    setGeografiDisplay(true);
  };
  const MarkerComponent = () => {
    // setDemografieDisplay(false);
    // setAktivDisplay(false);
    // setDenknDisplay(false);
    // setvalue(false);
    // setKjDisplay(false);
    // setMapDisplay(true);
    // setRuteDisplay(false);
    // setAdresDisplay(false);
    // setSegmenterDisplay(false);
    // setAddresslisteDisplay(false);
    // setGeografiDisplay(false);
    setMarkerDisplay(true);
    setInfoDisplay(false);
    setVelgReolDisplay(false);
    setlagDisplay(false);
    setPlussDisplay(false);
    setKnapMinusDisplay(false);
    setPanorerDisplay(false);
    setForrigerDisplay(false);
    setHeleNorgeDisplay(false);
  };
  const HeleNorges = () => {
    setHeleNorgeDisplay(true);
    setForrigerDisplay(false);
    setPanorerDisplay(false);
    setKnapMinusDisplay(false);
    setPlussDisplay(false);
    setlagDisplay(false);
    setVelgReolDisplay(false);
    setInfoDisplay(false);
    setMarkerDisplay(false);
  };
  const Panorers = () => {
    setPanorerDisplay(true);
    setKnapMinusDisplay(false);
    setPlussDisplay(false);
    setlagDisplay(false);
    setVelgReolDisplay(false);
    setInfoDisplay(false);
    setMarkerDisplay(false);
    setForrigerDisplay(false);
    setHeleNorgeDisplay(false);
  };
  const Forriges = () => {
    setForrigerDisplay(true);
    setPanorerDisplay(false);
    setKnapMinusDisplay(false);
    setPlussDisplay(false);
    setlagDisplay(false);
    setVelgReolDisplay(false);
    setInfoDisplay(false);
    setMarkerDisplay(false);
    setHeleNorgeDisplay(false);
  };

  const InfoRutes = () => {
    setInfoDisplay(true);
    setMarkerDisplay(false);
    setVelgReolDisplay(false);
    setlagDisplay(false);
    setPlussDisplay(false);
    setKnapMinusDisplay(false);
    setPanorerDisplay(false);
    setForrigerDisplay(false);
    setHeleNorgeDisplay(false);
  };
  const VelgReols = () => {
    setVelgReolDisplay(true);
    setInfoDisplay(false);
    setMarkerDisplay(false);
    setlagDisplay(false);
    setPlussDisplay(false);
    setKnapMinusDisplay(false);
    setPanorerDisplay(false);
    setForrigerDisplay(false);
    setHeleNorgeDisplay(false);
  };
  const Lags = () => {
    setlagDisplay(true);
    setVelgReolDisplay(false);
    setInfoDisplay(false);
    setMarkerDisplay(false);
    setPlussDisplay(false);
    setKnapMinusDisplay(false);
    setPanorerDisplay(false);
    setForrigerDisplay(false);
    setHeleNorgeDisplay(false);
  };
  const Plusses = () => {
    setPlussDisplay(true);
    setlagDisplay(false);
    setVelgReolDisplay(false);
    setInfoDisplay(false);
    setMarkerDisplay(false);
    setKnapMinusDisplay(false);
    setPanorerDisplay(false);
    setForrigerDisplay(false);
    setHeleNorgeDisplay(false);
  };

  const KnapMinuses = () => {
    setKnapMinusDisplay(true);
    setPlussDisplay(false);
    setlagDisplay(false);
    setVelgReolDisplay(false);
    setInfoDisplay(false);
    setMarkerDisplay(false);
    setPanorerDisplay(false);
    setForrigerDisplay(false);
    setHeleNorgeDisplay(false);
  };

  return (
    <div className=" container-fluid">
      <KSPUContext.Provider
        value={{
          geograErrMsg,
          setGeograErrMsg,
          demoIndexArray,
          setDemoIndexArray,
          globalBilTypeIW,
          setGlobalBilTypeIW,
          groupData,
          setGroupData,
          expandListId,
          setExpandListId,
          mapattribute,
          setmapattribute,
          demografikagemsg,
          setdemografikagemsg,
          showPriceCal,
          setshowPriceCal,
          selectedName,
          setSelectedName,
          demografikAntalMsg,
          setDemografikAntalMsg,
          demografikmsg,
          setdemografikmsg,
          existingActive,
          setExistingActive,
          antallValue,
          setAntallValue,
          Demograresultarray,
          setDemograresultarray,
          selecteddemografiecheckbox_c,
          setselecteddemografiecheckbox_c,
          selecteddemografiecheckbox,
          setselecteddemografiecheckbox,
          selectedrecord_s,
          setselectedrecord_s,
          pagekeysgeo,
          setpagekeysgeo,
          pagekeysseg,
          setpagekeysseg,
          pagekeys,
          setpagekeys,
          defaultSelectedColumn_s,
          setdefaultSelectedColumn_s,
          HouseholdSum,
          setHouseholdSum,
          selectedKoummeIDs,
          setselectedKoummeIDs,
          errormsg,
          seterrormsg,
          selectedDemografike,
          setSelectedDemografike,
          selectedRowKeys,
          setSelectedRowKeys,
          segmenterresultarray,
          setsegmenterresultarray,
          selectedsegment,
          setselectedsegment,
          resultData,
          setResultData,
          showBusiness,
          setShowBusiness,
          showHousehold,
          setShowHousehold,
          showDenking,
          setShowDenking,
          showReserverte,
          setShowReserverte,
          showReservedHouseHolds,
          setShowReservedHouseHolds,
          searchURL,
          setSearchURL,
          picklistData,
          setPicklistData,
          activUtvalg,
          setActivUtvalg,
          setvalue,
          setAktivDisplay,
          setRuteDisplay,
          setMapDisplay,
          LargUtvalg,
          setLargUtvalg,
          reolerData,
          setreolerData,
          issave,
          setissave,
          activUtvalglist,
          setActivUtvalglist,
          utvalglistcheck,
          setutvalglistcheck,
          save,
          setSave,
          showorklist,
          setshoworklist,
          checkedList,
          setCheckedList,
          interntoken,
          setinterntoken,
          setDemografieDisplay,
          setSegmenterDisplay,
          setAddresslisteDisplay,
          setGeografiDisplay,
          Details,
          setDetails,
          basisUtvalg,
          setbasisUtvalg,
          AddresslisteDisplay,
          isWidgetActive,
          setIsWidgetActive,
          setAdresDisplay,
          setKjDisplay,
          Budruteendringer,
          setBudruteendringer,
          SelectionUpdate,
          setSelectionUpdate,
          demographyIndex,
          setDemographyIndex,
          rutefoshkerVisited,
          setRutefoshkerVisited,
          rutefoshkerPreviousSelectedRutes,
          setrutefoshkerPreviousSelectedRutes,
          maintainUnsavedRute,
          setMaintainUnsavedRute,
        }}
      >
        <div className="row">
          <div className="col-3">
            <img
              id="uxImgBtnHome"
              className="HomePage ml-3"
              alt="Posten Logo"
              title="Gå til startfanen"
              src={puma}
              tabIndex="0"
              commandargument="0"
              bordercolor="Gray"
              borderwidth="2"
              onClick={HomePage}
            />
          </div>
          <div className="col-lg-9 col-md-6 col-sm-12 col-xs-12">
            {/* <img
            src={puma}
            style={{ float: "right" }}
            alt="Posten Logo"
            usemap="#Map"
            className=""
          /> */}
            <img
              src={bring}
              style={{ float: "right" }}
              className="CancelPage"
              alt="Posten Logo"
              onClick={pageAbandon}
            />
          </div>
        </div>
        {/* <div className="row">
        <div className="col-3">
        
          <img
            id="uxImgBtnHome"
            className="HomePage ml-3"
            alt="Home"
            title="Gå til startfanen"           
            src={home}
            tabIndex="0"
            commandargument="0"            
            bordercolor="Gray"
            borderwidth="2"
            onClick={HomePage}
          />
          
        </div>
        <div className="col-9 ">
        
          <input className="KSPU_button float-right mr-2"
            type="button"
            id="logoutbtn"
            value="Avslutt"
             onClick={pageAbandon}
          />
          
        </div>
        
      </div> */}
        <div className="row">
          <div
            className={
              !RuteDisplay
                ? "col-lg-3 col-md-4 col-sm-12 sidebar"
                : "col-lg-12 m-0 p-0"
            }
          >
            <div
              className={
                !RuteDisplay
                  ? "row headericonrow"
                  : "row headericonrow col-lg-3 col-md-4 col-sm-12 m-0 p-0"
              }
              style={{ paddingLeft: "20px", paddingRight: "20px" }}
            >
              {/* <div className="row headericonrow"> */}
              {/* <div className="col-1 mr-1"></div>
              <div className="pl-1"> */}
              <div className="row">
                <div className="col small-padding">
                  {GeografiDisplay ? (
                    <img
                      id="uxImgBtnGeografi"
                      title="Geografianalyse"
                      alt="Geografianalyse"
                      className="Geografianalyse_Aktiv"
                      tabIndex="0"
                      commandargument="0"
                    />
                  ) : (
                    <img
                      id="uxImgBtnGeografi"
                      title="Geografianalyse"
                      alt="Geografianalyse"
                      className="Geografianalyse"
                      tabIndex="0"
                      commandargument="0"
                      onClick={GeografiComponent}
                    />
                  )}
                </div>

                <div className="col small-padding">
                  {DemografieDisplay ? (
                    <img
                      id="uxImgBtnDemografi"
                      title="Demografiseleksjon"
                      alt="Demografiseleksjon"
                      className="Demografiseleksjon_Aktiv"
                      tabIndex="1"
                      commandargument="1"
                    />
                  ) : (
                    <img
                      id="uxImgBtnDemografi"
                      title="Demografiseleksjon"
                      alt="Demografiseleksjon"
                      className="Demografiseleksjon"
                      onClick={Demografiefun}
                      tabIndex="1"
                      commandargument="1"
                    />
                  )}
                </div>
                {/* <div className="col small-padding">
                  {SegmenterDisplay ? (
                    <img
                      id="uxImgBtnSegmenter"
                      title="Postreklame Segmenter"
                      alt="Postreklame Segmenter"
                      className="Segmenter_Aktiv"
                      tabIndex="2"
                      commandargument="2"
                    />
                  ) : (
                    <img
                      id="uxImgBtnSegmenter"
                      title="Postreklame Segmenter"
                      alt="Postreklame Segmenter"
                      className="Segmenterimage"
                      tabIndex="2"
                      commandargument="2"
                      onClick={Segmenter}
                    />
                  )}
                </div> */}
                <div className="col small-padding">
                  {KjDisplay ? (
                    <img
                      id="uxImgBtnKjoreAnalyse"
                      title="Kjøreanalyse"
                      alt="KjoreAnalyse"
                      className="kjimg_Aktiv"
                    />
                  ) : (
                    <img
                      id="uxImgBtnKjoreAnalyse"
                      title="Kjøreanalyse"
                      alt="KjoreAnalyse"
                      className="kjimg"
                      onClick={KjoreAnalyse}
                    />
                  )}
                </div>
                <div className="col small-padding">
                  {AdresDisplay ? (
                    <img
                      id="uxImgBtnFinnReolAdresse"
                      alt="FastAntall"
                      title="Adressepunkt og fastantallsanalyse"
                      className="FastAntall_Aktiv"
                    />
                  ) : (
                    <img
                      id="uxImgBtnFinnReolAdresse"
                      alt="FastAntall"
                      title="Adressepunkt og fastantallsanalyse"
                      className="FastAntall"
                      onClick={Adres}
                    />
                  )}
                </div>
                <div className="col small-padding">
                  {AddresslisteDisplay ? (
                    <img
                      id="uxImgBtnUpload"
                      title="Last opp adressepunkter"
                      alt="AdresseListe"
                      className="AdresseListe_Aktiv"
                      tabIndex="5"
                      commandargument="5"
                    />
                  ) : (
                    <img
                      id="uxImgBtnUpload"
                      title="Last opp adressepunkter"
                      alt="AdresseListe"
                      className="AdresseListe"
                      tabIndex="5"
                      commandargument="5"
                      onClick={Addressliste}
                    />
                  )}
                </div>
                <div className="pl-pointzero5">
                  <img id="Image2" alt="tabEnd" src={tabEnd} />
                </div>
              </div>
            </div>

            <div className={!RuteDisplay ? "row" : "row col-lg-12"}>
              {/* <div className="row"> */}
              {!RuteDisplay ? (
                // <div className='col'>
                <div className="col-1 pb-4 leftmenu">
                  {value ? (
                    <img id="uxBtnSok" alt="Sok" className="Sok_Aktiv" />
                  ) : (
                    <img
                      id="uxBtnSok"
                      alt="Sok"
                      className="Sok"
                      onClick={SokComponet}
                    />
                  )}
                  {/* </div>
          <div className="col-1"> */}
                  {AktivDisplay ? (
                    <img
                      id="uxBtnUtvalgsadministrator"
                      alt="AktivtUtvalg"
                      className="AktivtUtvalgleftMenu_Aktiv"
                    />
                  ) : (
                    <img
                      id="uxBtnUtvalgsadministrator"
                      alt="AktivtUtvalg"
                      className="AktivtUtvalgleftMenu"
                      onClick={AktivtUtvalgComponent}
                    />
                  )}
                  {/* </div>
          <div className="col-1"> */}
                  {RuteDisplay ? (
                    <img
                      id="uxBtnReolUtforsker"
                      alt="ReolUforsker"
                      className="ReolUforsker_Aktiv"
                      tabIndex="8"
                      commandargument="8"
                    />
                  ) : (
                    <img
                      id="uxBtnReolUtforsker"
                      alt="ReolUforsker"
                      className="ReolUforsker"
                      tabIndex="8"
                      commandargument="8"
                      onClick={ReolComponent}
                    />
                  )}
                  {/* </div>
          <div className="col-1"> */}
                  {/* {DenknDisplay ? (
              <img
                id="uxBtnDekning"
                alt="Dekning"
                src={Dekning_aktiv}
                tabIndex="9"
                commandargument="9"
              />
            ) : (
              <img
                id="uxBtnDekning"
                alt="Dekning"
                src={Dekning}
                tabIndex="9"
                commandargument="9"
                onClick={DenknComponent}
              />
            )} */}

                  {/* </div> */}
                </div>
              ) : (
                <div className="col-1 pb-4 leftmenu">
                  {value ? (
                    <img id="uxBtnSok" alt="Sok" className="Sok_Aktiv" />
                  ) : (
                    <img
                      id="uxBtnSok"
                      alt="Sok"
                      className="Sok"
                      onClick={SokComponet}
                    />
                  )}
                  {AktivDisplay ? (
                    <img
                      id="uxBtnUtvalgsadministrator"
                      alt="AktivtUtvalg"
                      className="AktivtUtvalgleftMenu_Aktiv"
                    />
                  ) : (
                    <img
                      id="uxBtnUtvalgsadministrator"
                      alt="AktivtUtvalg"
                      className="AktivtUtvalgleftMenu"
                      onClick={AktivtUtvalgComponent}
                    />
                  )}
                  {RuteDisplay ? (
                    <img
                      id="uxBtnReolUtforsker"
                      alt="ReolUforsker"
                      className="ReolUforsker_Aktiv"
                      tabIndex="8"
                      commandargument="8"
                    />
                  ) : (
                    <img
                      id="uxBtnReolUtforsker"
                      alt="ReolUforsker"
                      className="ReolUforsker"
                      tabIndex="8"
                      commandargument="8"
                      onClick={ReolComponent}
                    />
                  )}
                </div>
              )}

              <div
                className={
                  !RuteDisplay
                    ? "col p-0 m-0 pb-2 pr-1"
                    : "col-lg-11 col-md-11 col-sm-11 col-xs-11 p-0 m-0 pt-1 pl-1"
                }
              >
                {DemografieDisplay ? <Demografie /> : null}
                {KjDisplay ? <Kj /> : null}
                {AdresDisplay ? <Adress /> : null}
                {RuteDisplay ? <Rute /> : null}{" "}
                {value ? <SokComponentPage /> : null}
                {DenknDisplay ? <Denkn /> : null}
                {AktivDisplay ? <UtvalDetails /> : <></>}
                {AddresslisteDisplay ? <LastOppAdressepunkte /> : null}
                {/* {SegmenterDisplay ? <SegmenterComponent /> : null} */}
                {GeografiDisplay ? <GeografiComponents /> : null}
              </div>
              {/* </div> */}
            </div>
          </div>
          <div
            className={
              !RuteDisplay
                ? "col-lg-9 col-md-8 col-sm-12 col-xs-12 p-0 m-0 mapview-custom-width"
                : "col-lg-0 col-md-0 col-sm-0 col-xs-0 p-0 m-0"
            }
          >
            {MapDisplay ? (
              <div
                className="col-lg-12 col-md-12 col-sm-12 col-xs-12 pt-1"
                style={{ display: !RuteDisplay ? "block" : "none" }}
              >
                <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 row MapText mapregion">
                  <WebMapView />
                </div>
                <Arbeidsliste />
                {/* <div className="spanstyle2"></div> */}
              </div>
            ) : null}
          </div>
        </div>
        {/* <div className="spanstyle2"></div> */}
        <div>
          <hr className="half-width" />
        </div>
        <div className="row ml-1">
          <p className="copy pt-1 ">Copyright Posten Norge As - </p>
          {/* </div>
   <div className="col-2">  */}

          <a
            href="http://www.bring.no/privacy+policy"
            className="KSPU_LinkButton_Footer_Url pb-4"
          >
            Privacy Policy
          </a>

          <div className="container text-right">
            <p className="copy">versjon: 2.0.7461.17302 </p>
          </div>
        </div>
      </KSPUContext.Provider>
    </div>
  );
}
export default InterWeb;
