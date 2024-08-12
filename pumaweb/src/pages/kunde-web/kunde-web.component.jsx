import React, { useEffect, useState } from "react";

import WebMapView from "../../components/webmap/webmapView";
import CreateSelectionKW from "../../components/CreateSelectionKW";
import GeograVelg from "../../components/Geogra-velg";
import Demografivelg from "../../components/Demografi-velg";
import Segmentervelg from "../../components/Segmenter-kw";
import VegGeografiskOmrade from "../../components/VegGeografiskOmrade";
import VeglGeografiskOmrade_kw from "../../components/VelgGeografiskOmrade_kw";
import BudruterKW from "../../components/Budruter_nær_en_adresse/Budruter_nær_en_adresse.component";
import ApneetLinkClick from "../../components/apneetLink-click-kw/apneetLink-click-kw.component";
import LagutvalgClick from "../../components/Lagutvalgclick";
import Demogra_Velg_Click from "../../components/Demogra_velg_Click";
import Demogra_Velg_Antall_Click from "../../components/Demogra_velg_antall_click";
import Geogra_distribution_click from "../../components/Geogra_distribution_click";
import LagutvalgClick_segmenter from "../../components/Lagutvalgclick_segmenter";
import Segmenter_distribution_click from "../../components/Segmenter_distribution_click";
import Demogra_details from "../../components/Demografie_household_show";
import Geogra_distribution_cart_click from "../../components/Geogra_distribution_cart_click";
import Apne_Button_Click from "../../components/apne_Button_Click-kw/Apne_Button_Click-kw";
import Apne_Button_Completedorders from "../../components/apne_Button_Completedorders/Apne_Button_Completedorders";
import Lestill_Click_Component from "../../components/lestill_Click-kw/lestill_Click_Component-kw";
import Simple_save_utvalg from "../../components/simple_save_utvalg-kw/Simple_save_utvalg-kw";
import Simple_save_selection from "../../components/simple_save_selection_details_component/Simple_save_selection";
import { KundeWebContext } from "../../context/Context";
import Geogra_distribution_tilbake_click from "../../components/Geogra_distribution_tilbake";
import LestillClickKw from "../../components/lestill_Click-kw/lestillClickKw";
import CartClick_Component_kw from "../../components/cartClick_Component_Kw/cartClick_Component_kw";
import Sok_Component_kw from "../../components/Sok_Component_KW/sok_Component_kw";
import Burdruter_velg_KW from "../../components/Beregner_utvalg_budruter_nær_adres/Beregner_utvalg_budruter_nær_adres.component";
import EndreClick_kw from "../../components/EndreClick_KW/EndreClick_kw";
import norgeskart from "../../assets/images/icons_kw/KNAPP-norgeskart.gif";
import plus from "../../assets/images/icons_kw/KNAPP-velg-plus.gif";
import minus from "../../assets/images/icons_kw/KNAPP-velg-minus.gif";
import info from "../../assets/images/icons_kw/KNAPP-info-rute.gif";
import plusgif from "../../assets/images/icons_kw/KNAPP-pluss.gif";
import minusgif from "../../assets/images/icons_kw/KNAPP-minus.gif";
import panorer from "../../assets/images/icons_kw/KNAPP-panorer.gif";
import { v4 as uuidv4 } from "uuid";
import "../../App.css";
import api from "../../services/api";

function KundeWeb(props) {
  const [showReservedHouseHolds, setShowReservedHouseHolds] = useState(false);
  const [HouseholdSum, setHouseholdSum] = useState(0);
  const [householdcheckbox, sethouseholdcheckbox] = useState(true);
  const [businesscheckbox, setbusinesscheckbox] = useState(false);
  const [BusinessSum, setBusinessSum] = useState(0);
  const [Total, setTotal] = useState(0);
  const [Page, setPage] = useState("");
  const [Antallvalue, setAntallvalue] = useState(0);
  const [selectedRowKeys, setSelectedRowKeys] = useState([]);
  const [selectedsegment, setselectedsegment] = useState([]);
  const [segmenterresultarray, setsegmenterresultarray] = useState([]);
  const [selectedarrayofrecords, setselectedarrayofrecords] = useState([]);
  const [selecteddemografiecheckbox, setselecteddemografiecheckbox] = useState(
    []
  );
  const [Page_P, setPage_P] = useState("");
  const [selectedrecord_s, setselectedrecord_s] = useState([]);
  const [selecteddemografiecheckbox_c, setselecteddemografiecheckbox_c] =
    useState([]);
  const [Demograresultarray, setDemograresultarray] = useState([]);
  const [pagekeys, setpagekeys] = useState([]);
  const [pagekeysseg, setpagekeysseg] = useState([]);
  const [pagekeysgeo, setpagekeysgeo] = useState([]);
  const [defaultSelectedColumn_s, setdefaultSelectedColumn_s] = useState([]);
  const [describtion, setdescribtion] = useState("");
  const [selection, setselection] = useState("");
  const [UtvalgID, setUtvalgID] = useState(0);
  const [HouseholdSum_seg, setHouseholdSum_seg] = useState(0);
  const [HouseholdSum_Demo, setHouseholdSum_Demo] = useState(0);
  const [selectedKoummeIDs, setselectedKoummeIDs] = useState([]);
  const [utvalgapiobject, setutvalgapiobject] = useState({});
  const [newhome, setnewhome] = useState(false);
  const [warninputvalue, setwarninputvalue] = useState("");
  const [utvalgname, setutvalgname] = useState("");
  const [gateValue, setGateValue] = useState("");
  const [username_kw, setusername_kw] = useState("");
  const [key_kw, setkey_kw] = useState("");
  const [avtaleData, setavtaleData] = useState("");
  const [custNos, setcustNos] = useState("");

  const [preselection, setpreselection] = useState("");
  const [predesc, setpredesc] = useState("");
  const [utvalglistcheck, setutvalglistcheck] = useState(false);
  const [listmodal, setlistmodal] = useState(false);
  const [utvalglistapiobject, setutvalglistapiobject] = useState({});
  const [showBusiness, setShowBusiness] = useState(businesscheckbox);
  const [resultData, setResultData] = useState([]);
  const [ActiveUtvalgObject, setActiveUtvalgObject] = useState({});
  const [ResultOutputData, setResultOutputData] = useState([]);
  const [SavedUtvalg, setSavedUtvalg] = useState({});
  const [LeggTilCheckedItems, setLeggTilCheckedItems] = useState([]);
  const [KopierModal, setKopierModal] = useState(false);
  const [SelectedItemCheckBox_Budruter, setSelectedItemCheckBox_Budruter] =
    useState([]);
  const [BudruterTimeSelection, setBudruterTimeSelection] = useState(false);
  const [BudruterDistanceSelection, setBudruterDistanceSelection] =
    useState(false);
  const [HouseholdSum_budruter, setHouseholdSum_budruter] = useState(0);
  const [CartItems, setCartItems] = useState([]);
  const [BudruterAntallSelection, setBudruterAntallSelection] = useState("");
  const [rendering, setrendering] = useState(false);
  const [BudruterSelectedName, setBudruterSelectedName] = useState("");
  const [Endreapiobject, setEndreapiobject] = useState({});
  const [Endrelistapiobject, setEndrelistapiobject] = useState({});
  const [cartClickModalHide, setCartClickModalHide] = useState(false);
  const [KopierCheckedItems, setKopierCheckedItems] = useState([]);
  const [leggtiltrue, setleggtiltrue] = useState(false);
  const [criteriaObject, setCriteriaObject] = useState({});
  const [globalBilType, setGlobalBilType] = useState(false);
  const [routeUpdateEnabled, setRouteUpdateEnabled] = useState(false);
  const [selectionUpdateKW, setSelectionUpdateKW] = useState(false);

  const GetAgreementNumber = async (userName, Key) => {
    let uniqueId = uuidv4();
    let eConnectUrl = `ECPuma/AgreementLookup389`;

    let eConnectbody = {
      Header: {
        SystemCode: "",
        MessageId: uniqueId,
        SecurityToken: null,
        UserName: null,
        Version: null,
        Timestamp: null,
      },

      BrukerNavn: userName,
      Key: Key,

      UtvalgsID: null,
    };

    try {
      const { data, status } = await api.postdata(eConnectUrl, eConnectbody);
      if (data.avtaleData === null && data.kundeNr === null) {
        return 0;
      } else {
        setavtaleData(data.avtaleData);
        setcustNos(data.kundeNr);
        //AvtaleData
        return 1;
      }
    } catch (error) {
      console.error("error : " + error);
      return 0;
    }
  };
  useEffect(async () => {
    const url = new URL(window.location.href);
    let kundeUser = sessionStorage.getItem("userName");
    let kundeKey = (url.searchParams.get("kw_key")!== null || url.searchParams.get("kw_key")!== "") ?
    url.searchParams.get("kw_key") : sessionStorage.getItem("key");
    
    let utvalgType = (url.searchParams.get("utvalgstype") !== null || url.searchParams.get("utvalgstype") !== "") ?
    url.searchParams.get("utvalgstype") : sessionStorage.getItem("kwUtvalgsType");
    let selectionId = (url.searchParams.get("utvalgid") !== null || url.searchParams.get("utvalgid") !== "") ? 
    url.searchParams.get("utvalgid") : sessionStorage.getItem("kwUtvalgsId");

    if (utvalgType && selectionId) {
      let url = "";
      if (utvalgType === "U") {
        url = `Utvalg/SearchUtvalgByUtvalgId?utvalgId=${selectionId}&includeReols=${true}`;
      } else {
        url = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${selectionId}`;
      }
      try {
        const { data, status } = await api.getdata(url);
        if (status === 200) {
          if (data?.memberUtvalgs && data?.memberUtvalgs?.length > 0) {
            setutvalglistapiobject(data);
            let cartItems = [];
            data?.memberUtvalgs?.map(item=>{
              cartItems?.push(item);
            })
            setCartItems(cartItems);
            setPage_P("cartClick_Component_kw");
            setPage("Geogra_distribution_click");
          } else if(data?.length > 0) {
            setutvalgapiobject(data[0]);
            setPage_P("Apne_Button_Click");
            setPage("Geogra_distribution_click");
          }
        }
      } catch (error) {
        console.log(error);
      }
    }

    const agreement = await GetAgreementNumber(kundeUser, kundeKey);
  }, []);

  return (
    <KundeWebContext.Provider
      value={{
        selectionUpdateKW,
        setSelectionUpdateKW,
        routeUpdateEnabled,
        setRouteUpdateEnabled,
        globalBilType,
        setGlobalBilType,
        criteriaObject,
        setCriteriaObject,
        leggtiltrue,
        setleggtiltrue,
        KopierCheckedItems,
        setKopierCheckedItems,
        cartClickModalHide,
        setCartClickModalHide,
        Endrelistapiobject,
        setEndrelistapiobject,
        Endreapiobject,
        setEndreapiobject,
        BudruterSelectedName,
        setBudruterSelectedName,
        rendering,
        setrendering,
        BudruterAntallSelection,
        setBudruterAntallSelection,
        CartItems,
        setCartItems,
        HouseholdSum_budruter,
        setHouseholdSum_budruter,
        BudruterTimeSelection,
        setBudruterTimeSelection,
        BudruterDistanceSelection,
        setBudruterDistanceSelection,
        SelectedItemCheckBox_Budruter,
        setSelectedItemCheckBox_Budruter,
        KopierModal,
        setKopierModal,
        LeggTilCheckedItems,
        setLeggTilCheckedItems,
        SavedUtvalg,
        setSavedUtvalg,
        ResultOutputData,
        setResultOutputData,
        ActiveUtvalgObject,
        setActiveUtvalgObject,
        resultData,
        setResultData,
        showReservedHouseHolds,
        setShowReservedHouseHolds,
        showBusiness,
        setShowBusiness,
        utvalglistapiobject,
        setutvalglistapiobject,
        listmodal,
        setlistmodal,
        utvalglistcheck,
        setutvalglistcheck,
        gateValue,
        setGateValue,
        utvalgname,
        setutvalgname,
        warninputvalue,
        setwarninputvalue,
        newhome,
        setnewhome,
        utvalgapiobject,
        setutvalgapiobject,
        selectedKoummeIDs,
        setselectedKoummeIDs,
        HouseholdSum_Demo,
        setHouseholdSum_Demo,
        HouseholdSum_seg,
        setHouseholdSum_seg,
        UtvalgID,
        setUtvalgID,
        selection,
        setselection,
        describtion,
        setdescribtion,
        defaultSelectedColumn_s,
        setdefaultSelectedColumn_s,
        pagekeysgeo,
        setpagekeysgeo,
        pagekeysseg,
        setpagekeysseg,
        pagekeys,
        setpagekeys,
        Demograresultarray,
        setDemograresultarray,
        selecteddemografiecheckbox_c,
        setselecteddemografiecheckbox_c,
        selectedrecord_s,
        setselectedrecord_s,
        Page_P,
        setPage_P,
        selecteddemografiecheckbox,
        setselecteddemografiecheckbox,
        selectedarrayofrecords,
        setselectedarrayofrecords,
        segmenterresultarray,
        setsegmenterresultarray,
        selectedsegment,
        setselectedsegment,
        selectedRowKeys,
        setSelectedRowKeys,
        householdcheckbox,
        sethouseholdcheckbox,
        businesscheckbox,
        setbusinesscheckbox,
        HouseholdSum,
        setHouseholdSum,
        BusinessSum,
        setBusinessSum,
        Total,
        setTotal,
        Page,
        setPage,
        Antallvalue,
        setAntallvalue,
        username_kw,
        setusername_kw,
        key_kw,
        setkey_kw,
        avtaleData,
        setavtaleData,
        custNos,
        setcustNos,
        preselection,
        setpreselection,
        predesc,
        setpredesc,
      }}
    >
      <div className="container-fluid">
        <div className="Framework ">
          <div className="container-kw bodyKW  p-3">
            <div
              id="KW"
              className=" pl-3"
              scrolling="auto"
              frameBorder="0"
              marginWidth="0"
              marginHeight="0"
              style={{ backgroundColor: "#FFFFFF" }}
            >
              <div className="row  ">
                {Page === "" ? <CreateSelectionKW /> : null}

                {Page === "Geovelg" ? <GeograVelg /> : null}

                {Page === "Demografivelg" ? <Demografivelg /> : null}

                {Page === "Segmentervelg" ? <Segmentervelg /> : null}
                {Page === "VegGeografiskOmrade" ? (
                  <VegGeografiskOmrade />
                ) : null}
                {Page === "VeglGeografiskOmrade_kw" ? (
                  <VeglGeografiskOmrade_kw />
                ) : null}

                {Page === "Budrutervelg" ? <BudruterKW /> : null}
                {Page === "ApneetlinkClick" ? <ApneetLinkClick /> : null}

                {Page === "LagutvalgClick" ? (
                  <LagutvalgClick hushold={HouseholdSum} />
                ) : null}

                {Page === "Demogra_Velg_Click" ? <Demogra_Velg_Click /> : null}

                {Page === "Demogra_velg_antall_click" ? (
                  <Demogra_Velg_Antall_Click />
                ) : null}

                {Page === "Geogra_distribution_click" ? (
                  <Geogra_distribution_click />
                ) : null}
                {Page === "LagutvalgClick_segmenter" ? (
                  <LagutvalgClick_segmenter />
                ) : null}
                {Page === "Segmenter_distribution_click" ? (
                  <Segmenter_distribution_click />
                ) : null}
                {Page === "Demogra_details" ? <Demogra_details /> : null}
                {Page === "Geogra_distribution_cart_click" ? (
                  <Geogra_distribution_cart_click />
                ) : null}
                {Page === "Apne_Button_Click" ? <Apne_Button_Click /> : null}
                {Page === "Apne_Button_Completedorders" ? (
                  <Apne_Button_Completedorders />
                ) : null}
                {Page === "Lestill_Click_Component" ? (
                  <Lestill_Click_Component />
                ) : null}
                {Page === "Simple_save_utvalg" ? <Simple_save_utvalg /> : null}

                {Page === "Simple_save_selection" ? (
                  <Simple_save_selection />
                ) : null}

                {Page === "Geogra_distribution_tilbake_click" ? (
                  <Geogra_distribution_tilbake_click />
                ) : null}

                {Page === "LestillClickKw" ? <LestillClickKw /> : null}

                {Page === "cartClick_Component_kw" ? (
                  <CartClick_Component_kw />
                ) : null}

                {Page === "Sok_Component_kw" ? <Sok_Component_kw /> : null}
                {Page === "Burdruter_velg_KW" ? <Burdruter_velg_KW /> : null}

                {Page === "EndreClick_kw" ? <EndreClick_kw /> : null}

                <div
                  className="modal fade bd-example-modal-sm"
                  id="exampleModal"
                  tabIndex="-1"
                  role="dialog"
                  aria-labelledby="exampleModalCenterTitle"
                >
                  <div
                    className="modal-dialog  modal-dialog-centered "
                    role="document"
                  >
                    <div className="">
                      <div className="modalstyle">
                        <div className="modal-content">
                          <div className=" divDockedPanelTop">
                            <span className="dialog-kw" id="exampleModalLabel">
                              Velg budruter
                            </span>
                          </div>
                          <div className="View_modal-body pl-2">
                            <div id="" className="sok-text">
                              <b>
                                <u> Legg til eller fjern budruter i kartet:</u>
                              </b>
                              <br />
                              <br />
                              Slik gjørdu:
                              <br />
                              <br />
                              1a.
                              <img align="bottom" src={plus} />
                              Klikk på ikonet for å legge til budruter
                              <br />
                              1b.
                              <img align="bottom" src={minus} />
                              Klikk på ikonet for å fjerne budruter
                              <br />
                              2. Marker i kartet hvor du ønsker å legge til
                              eller fjerne budruter
                              <br />
                              <br />
                              <br />
                              <b>
                                <u>Mer informasjon om budrutene:</u>
                              </b>
                              <br />
                              <br />
                              Slik gjør du:
                              <br />
                              <br />
                              1. <img align="bottom" src={info} /> Klikk på
                              ikonet
                              <br />
                              2. Klikk i kartet for mer detaljert informasjon om
                              valgte budrute
                              <br />
                              <br />
                            </div>
                            <div
                              id="11.208939868046542_closeDiv"
                              className="modalAlertClose"
                            >
                              <input
                                value="OK"
                                type="button"
                                id="11.208939868046542_closeButton"
                                data-dismiss="modal"
                                aria-label="Close"
                              />
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <div
                  className="modal fade"
                  id="exampleModal1"
                  tabIndex="-1"
                  role="dialog"
                  aria-labelledby="exampleModalCenterTitle"
                >
                  <div
                    className="modal-dialog  modal-dialog-centered "
                    role="document"
                  >
                    <div className="">
                      <div className="modalstyle">
                        <div className="modal-content">
                          <div className=" divDockedPanelTop">
                            <span className="dialog-kw" id="exampleModalLabel1">
                              Symbolforklaring
                            </span>
                          </div>
                          <div className="View_modal-body pl-2">
                            <div id="" className="sok-text">
                              <div id="27.813225899133975_message">
                                <br />
                                <img align="bottom" src={norgeskart} />
                                &nbsp; Klikk her for å se hele Norgeskartet
                                <br />
                                <br />
                                <img align="bottom" src={plusgif} />
                                &nbsp; Klikk på ikonet og marker i kartet for å
                                zoome inn
                                <br />
                                <br />
                                <img align="bottom" src={minusgif} />
                                &nbsp; Klikk på ikonet og marker i kartet for å
                                zoome ut
                                <br />
                                <br />
                                <img align="bottom" src={panorer} />
                                &nbsp; Klikk på ikonet og deretter i kartet for
                                å flytte kartutstnittet
                                <br />
                              </div>
                            </div>
                            <div
                              id="11.208939868046542_closeDiv"
                              className="modalAlertClose"
                            >
                              <input
                                value="OK"
                                type="button"
                                id="11.208939868046542_closeButton"
                                data-dismiss="modal"
                                aria-label="Close"
                              />
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <div className="col-7 Kw-map">
                  <div className="Kw-map pr-3">
                    <WebMapView />
                  </div>
                </div>
              </div>
            </div>

            <p></p>

            <div className="containerFoot">
              <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 padding_Color_L_R_T_B">
                <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                  <a
                    href="http://www.bring.no/sende/post/like-formater/velg-malgruppe-og-send-uadressert-reklame"
                    id="uxAProductInformation"
                    target=" blank"
                    className="KSPU_LinkButton_Footer_Url"
                  >
                    Les mer om uadressert reklame
                  </a>
                </div>
                <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                  <a
                    href="http://www.bring.no/sende/post/like-formater/velg-malgruppe-og-send-uadressert-reklame"
                    id="uxAPriceInformation"
                    target="_blank"
                    className="KSPU_LinkButton_Footer_Url"
                  >
                    Prisliste
                  </a>
                </div>
                <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
                  <a
                    href={process.env.REACT_APP_KUNDEWEB_COMPONENT_LINK_URL}
                    id="uxAContactUs"
                    target="_blank"
                    className="KSPU_LinkButton_Footer_Url"
                  >
                    Ring oss på 04045 eller fyll ut kontakt skjema her
                  </a>
                </div>
                <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 text-right KSPUVersion">
                  versjon:&nbsp;
                  <span id="uxVersionNumber">2.0.7461.17302</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </KundeWebContext.Provider>
  );
}

export default KundeWeb;
