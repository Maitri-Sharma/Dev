import React, { useEffect, useState, useContext, useRef } from "react";
import { KundeWebContext, MainPageContext } from "../../context/Context";
import "../../components/cartClick_Component_Kw/cartClick_kw.styles.scss";
import Swal from "sweetalert2";
import readmore from "../../assets/images/read_more.gif";
import api from "../../services/api.js";
import { StandardreportKw } from "../apne_Button_Click-kw/standardreportKw";
import spinner from "../../assets/images/kw/spinner.gif";
import {
  NumberFormat,
  imagePath,
  ColorCodes,
  CommonColorCodes,
  CurrentDate,
  filterCommonReolIds,
} from "../../common/Functions";
import "../../components/lestill_Click-kw/lestill_Click_Component-kw.scss";
import moment from "moment";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";
import * as query from "@arcgis/core/rest/query";
import ForHandlerListModel_KW from "../ForHandlerListModel_KW";
import warningImg from "../../assets/images/varsels_ikon_stort.png";
import $ from "jquery";
import SaveUtvalgListKW from "../SaveUtvalgList-kw";

function CartClick_Component_Kw() {
  const { Page_P, setPage_P } = useContext(KundeWebContext);
  const { Page, setPage } = useContext(KundeWebContext);
  const { warninputvalue, setwarninputvalue } = useContext(KundeWebContext);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  const { UtvalgID, setUtvalgID } = useContext(KundeWebContext);
  const { utvalgname, setutvalgname } = useContext(KundeWebContext);
  const { Antallvalue, setAntallvalue } = useContext(KundeWebContext);
  const [warninputvalue_1, setwarninputvalue_1] = useState("");
  const [desinput, setdesinput] = useState("");
  const [melding, setmelding] = useState(false);
  const [errormsg, seterrormsg] = useState("");
  const { newhome, setnewhome } = useContext(KundeWebContext);
  const [lestillUtvalgName, setLestillUtvalgName] = useState("");
  const [skrivUtvalgetName, setSkrivUtvalgetName] = useState("");
  const { username_kw, setusername_kw } = useContext(KundeWebContext);
  const { utvalglistapiobject, setutvalglistapiobject } =
    useContext(KundeWebContext);
  const { ActiveUtvalgObject, setActiveUtvalgObject } =
    useContext(KundeWebContext);
  const { LeggTilCheckedItems, setLeggTilCheckedItems } =
    useContext(KundeWebContext);
  const { LeggTilCheckedItems1, setLeggTilCheckedItems1 } =
    useContext(KundeWebContext);
  const [melding1, setmelding1] = useState(false);
  const [errormsg1, seterrormsg1] = useState("");
  const btnclose = useRef();
  const [utvalglistcheck, setutvalglistcheck] = useState(true);
  const [DisableEgendefinert, setDisableEgendefinert] = useState(true);
  const [melding_1, setmelding_1] = useState(false);
  const [errormsg_1, seterrormsg_1] = useState("");
  const [AddDate, setAddDate] = useState(true);
  const [AddTime, setAddTime] = useState(false);
  const [AddCustomText, setAddCustomText] = useState(false);
  const [Total, setTotal] = useState(0);
  const [listTotal, setListTotal] = useState(0);
  const [antallBeforeRecreationSum, setAntallBeforeRecreationSum] = useState(0);
  const [antallWhenSavedSum, setAntallWhenSavedSum] = useState(0);
  const [CustomText, setCutomText] = useState("");
  const [utvalgmemberdisplay, setutvalgmemberdisplay] = useState(false);
  const { CartItems, setCartItems } = useContext(KundeWebContext);
  const { custNos, setcustNos } = useContext(KundeWebContext);
  const btnclose_1 = useRef();
  const btnclose_Avbryt = useRef();
  const { mapView } = useContext(MainPageContext);
  const [ModalOpenID, setModalOpenID] = useState("");
  const [dateAdded, setDateAdded] = useState(false);
  const { Endrelistapiobject, setEndrelistapiobject } =
    useContext(KundeWebContext);
  const { Endreapiobject, setEndreapiobject } = useContext(KundeWebContext);
  const { cartClickModalHide, setCartClickModalHide } =
    useContext(KundeWebContext);
  const { KopierCheckedItems, setKopierCheckedItems } =
    useContext(KundeWebContext);

  const [duplicate, setDuplicate] = useState(false);
  const [duplicateArr, setDuplicateArr] = useState([]);
  const [utvalgitemdisplay, setutvalgitemdisplay] = useState(false);
  const [modalLoading, setModalLoading] = useState(false);
  const { leggtiltrue, setleggtiltrue } = useContext(KundeWebContext);
  const [contentLoading, setContentLoading] = useState(true);
  const [routeUpdate, setRoutepdate] = useState(false);
  const [routeUpdateCount, setRoutepdateCount] = useState(0);
  const [LagreList, setLagreList] = useState("");

  const MapRender = async (Reolids, colorcode) => {
    let rendered = false;
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
    await GetAllBurdruter();
    async function GetAllBurdruter() {
      let queryObject = new Query();
      let featuresGeometry = [];
      // queryObject.where = "1=1";

      queryObject.where = `${sql_geography}`;
      queryObject.returnGeometry = true;
      queryObject.outFields = ["tot_anta", "hh", "hh_res"];

      await query
        .executeQueryJSON(BudruterUrl, queryObject)
        .then(function (results) {
          if (results.features.length > 0) {
            let selectedSymbol = {
              type: "simple-fill",
              color: colorcode,
              style: "solid",
              outline: {
                color: [237, 54, 21],
                width: 0.75,
              },
            };

            results.features.map((item) => {
              // featuresGeometry.push(item.geometry);
              let graphic = new Graphic(
                item.geometry,
                selectedSymbol,
                item.attributes
              );
              mapView.graphics.add(graphic);
            });
            rendered = true;
          }
        });
      // mapView.graphics.items.forEach(function (item) {
      //   if (item.geometry.type === "polygon") {
      //     featuresGeometry.push(item.geometry);
      //   }
      // });
      // mapView.goTo(featuresGeometry);
    }
    return rendered;
  };

  const EndreBeskrivelseClick = () => {
    setModalOpenID("ForHandlerListModal");
  };

  useEffect(() => {
    if (CartItems?.length > 0) {
      let sum = 0;
      CartItems?.map((item) => {
        sum += item.totalAntall;
      });
      setTotal(sum);
    }
  }, [CartItems]);

  useEffect(() => {
    LoadList(utvalglistapiobject);
    setutvalgapiobject({});
  }, []);

  const mapRenderDoubleCoverage = async (commonSelections) => {
    if (commonSelections.filteredCommonItems?.length > 0) {
      await MapRender(
        commonSelections.filteredCommonItems,
        CommonColorCodes()[0]
      );
    }
  };

  const LoadList = async (utvalgList) => {
    if (!leggtiltrue) {
      let KartItems = [];
      if (utvalgList?.memberUtvalgs?.length > 0) {
        let listUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${utvalgList.listId}`;
        const { data, status } = await api.getdata(listUrl);
        if (status === 200 && data !== undefined) {
          data?.memberUtvalgs?.map((i) => {
            KartItems?.push(i);
          });
          setutvalglistapiobject(data);
        }
      }
      let cartItems = JSON.parse(JSON.stringify(KartItems));

      setCartItems(cartItems);

      let commonSelections = filterCommonReolIds(cartItems);
      setDuplicateArr(commonSelections?.filteredCommonSelectionNames);
      if (commonSelections?.filteredCommonSelectionNames?.length > 0) {
        setDuplicate(true);
      }

      let j = mapView.graphics.items.length;
      for (var i = j; i > 0; i--) {
        if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
          mapView.graphics.remove(mapView.graphics.items[i - 1]);
          //j++;
        }
      }

      let renderedCount = 0; //index can't be used.
      cartItems.map(async (items, index1) => {
        let Reolids = [];
        if (items.reoler && items.reoler.length > 0) {
          items.reoler.map((item) => {
            if (!commonSelections.filteredCommonItems.includes(item.reolId))
              Reolids.push(item.reolId);
          });
        }
        if (Reolids?.length) {
          await MapRender(Reolids, ColorCodes()[index1]);
        }
        renderedCount = renderedCount + 1;
        if (renderedCount === cartItems?.length) {
          await mapRenderDoubleCoverage(commonSelections);
          ZoomToAll();
          renderedCount = 0;
        }
      });

      // if (commonSelections.filteredCommonItems?.length > 0) {
      //   await MapRender(commonSelections.filteredCommonItems, CommonColorCodes()[0]);
      //   ZoomToAll();
      // }

      if (KopierCheckedItems?.length > 0 && cartClickModalHide === false) {
        window.$("#lestillUtvalg").modal("show");
      }
      setContentLoading(false);
    }
    if (leggtiltrue) {
      setleggtiltrue(false);

      window.scroll(0, 0);

      var ItemsTobeDisplayedInCart = [];
      let resultObject = [];
      if (LeggTilCheckedItems.length > 0) {
        LeggTilCheckedItems.map((item) => {
          var utvalgID = item.utvalgId.toString();
          if (utvalgID.charAt(0) === "U") {
            utvalgID = JSON.parse(utvalgID.slice(1));
          } else {
            utvalgID = utvalgID;
          }
          item.utvalgId = utvalgID;

          resultObject.push(item);
        });
        let listUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${utvalgList.listId}`;
        const { data, status } = await api.getdata(listUrl);
        if (data !== undefined && status === 200) {
          setutvalglistapiobject(data);
          data?.memberUtvalgs?.map((i) => {
            ItemsTobeDisplayedInCart.push(i);
          });
          let customername = "";
          if (username_kw) {
            customername = username_kw;
          } else {
            customername = "Internbruker";
          }
          if (status === 200 && data !== undefined) {
            let url = `UtvalgList/AddUtvalgsToExistingList?userName=${customername}`;
            resultObject.map((item) => {
              data.memberUtvalgs.push(item);
            });

            var listData = data;
            listData.kundeNummer = custNos;
            try {
              var a = {
                utvalgList: listData,
                utvalgs: resultObject,
              };

              await api.postdata(url, a).then((res) => {
                if (res?.data !== undefined && res?.status === 200) {
                  let listUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${utvalgList.listId}`;
                  api.getdata(listUrl).then((res1) => {
                    if (res1?.data !== undefined && res1?.status === 200) {
                      ItemsTobeDisplayedInCart = [];
                      setutvalglistapiobject(res1.data);
                      res1?.data?.memberUtvalgs?.map((i) => {
                        ItemsTobeDisplayedInCart.push(i);
                      });

                      ItemsTobeDisplayedInCart = JSON.parse(
                        JSON.stringify(ItemsTobeDisplayedInCart)
                      );

                      let commonSelections = filterCommonReolIds(
                        ItemsTobeDisplayedInCart
                      );
                      setDuplicateArr(
                        commonSelections.filteredCommonSelectionNames
                      );
                      if (
                        commonSelections?.filteredCommonSelectionNames?.length >
                        0
                      ) {
                        setDuplicate(true);
                      }

                      let j = mapView.graphics.items.length;
                      for (var i = j; i > 0; i--) {
                        if (
                          mapView.graphics.items[i - 1].geometry.type ===
                          "polygon"
                        ) {
                          mapView.graphics.remove(
                            mapView.graphics.items[i - 1]
                          );
                          //j++;
                        }
                      }
                      let renderedCount = 0; //index can't be used.
                      ItemsTobeDisplayedInCart.map(async (items, index1) => {
                        let Reolids = [];
                        if (items.reoler && items.reoler.length > 0) {
                          items.reoler.map(async (item, index) => {
                            if (
                              !commonSelections.filteredCommonItems.includes(
                                item.reolId
                              )
                            )
                              Reolids.push(item.reolId);
                          });
                          if (Reolids?.length) {
                            await MapRender(Reolids, ColorCodes()[index1]);
                          }
                          renderedCount = renderedCount + 1;
                          if (
                            renderedCount === ItemsTobeDisplayedInCart?.length
                          ) {
                            await mapRenderDoubleCoverage(commonSelections);
                            ZoomToAll();
                            renderedCount = 0;
                          }
                        }
                      });

                      setCartItems(ItemsTobeDisplayedInCart);
                    }

                    setLeggTilCheckedItems([]);
                    setContentLoading(false);
                    if (KopierCheckedItems?.length > 0) {
                      window.$("#lestillUtvalg").modal("show");
                    }
                  });
                }
              });
            } catch (error) {
              console.log(error);
            }
          }
        }
      }
    }

    // let sum = 0;
    let totalBeforeCreation = 0;
    let totalWhenSaved = 0;
    let flag = 0;
    let count = 0;
    utvalgList?.memberUtvalgs?.map((item) => {
      if (item.antallBeforeRecreation > 0) {
        flag = 1;
        count += 1;
      }
      totalBeforeCreation = totalBeforeCreation + item?.antallBeforeRecreation;
      totalWhenSaved = totalWhenSaved + item?.antallWhenLastSaved;
      // sum = sum + item?.totalAntall;
    });
    if (flag) {
      let msg = `Budruteendringer har påvirket utvalgene`;
      Swal.fire({
        text: msg,
        confirmButtonColor: "#7bc144",
        confirmButtonText: "Lukk",
        position: "top",
      });
      setRoutepdate(true);
    }
    // setListTotal(sum);
    setAntallBeforeRecreationSum(totalBeforeCreation);
    setAntallWhenSavedSum(totalWhenSaved);
    setRoutepdateCount(count);
  };

  const CustomTextfun = (e) => {
    setCutomText(e.target.value);
  };
  // modal related functions
  const uxBtnLagreSom = useRef();
  const fylkeChkBtm = useRef();
  const kommuneChkBtm = useRef();
  const teamChkBtm = useRef();
  const postChkBtm = useRef();
  const budRuteChkBtm = useRef();

  const fylkeChkBtmDiv = useRef();
  const kommuneChkBtmDiv = useRef();
  const teamChkBtmDiv = useRef();

  const showLagreList = async (e) => {
    setLagreList("Save_Large");
  };

  const handleChangeBtm = (e) => {
    fylkeChkBtm.current.checked = false;
    kommuneChkBtm.current.checked = false;
    teamChkBtm.current.checked = false;

    switch (e.target.id) {
      case "FylkeB":
        e.target.checked = true;

        setmelding_1(false);

        setAddDate(true);
        setAddTime(false);
        setAddCustomText(false);
        setDisableEgendefinert(true);

        break;
      case "KommuneB":
        e.target.checked = true;
        setmelding_1(false);
        setAddTime(true);
        setAddDate(false);
        setAddCustomText(false);
        setDisableEgendefinert(true);
        break;
      case "TeamB":
        e.target.checked = true;
        setmelding_1(false);
        e.target.checked = true;
        setmelding_1(false);
        setAddTime(false);
        setAddDate(false);
        setAddCustomText(true);
        setDisableEgendefinert(false);

        break;
    }
  };
  const LagreClickFun = async () => {
    setModalLoading(true);
    var resultObject = [];

    setCartClickModalHide(true);

    let ItemsTobeChanged = [];
    KopierCheckedItems.map((item) => {
      if (item.memberUtvalgs && item.memberUtvalgs.length > 0) {
        item.memberUtvalgs.map((item) => {
          ItemsTobeChanged.push(item);
        });
      } else {
        ItemsTobeChanged.push(item);
      }
    });

    let ChangedObject = ItemsTobeChanged.map((item) => {
      item.name = warninputvalue_1;
      item.logo = desinput;
      item.listId = 0;
      item.utvalgId = 0;
      return {
        ...item,
      };
    });
    let Reolids = [];

    if (CartItems[0].reoler && CartItems[0].reoler.length > 0) {
      let TemVar = CartItems[0].reoler.map(async (item, index) => {
        Reolids.push(item.reolId);
      });
    }
    let TemItem = LeggTilCheckedItems;
    TemItem = ChangedObject;
    setLeggTilCheckedItems([]);
    setLeggTilCheckedItems(ChangedObject);
    let customername = "";
    if (username_kw) {
      customername = username_kw;
    } else {
      customername = "Internbruker";
    }
    let promiseAllResult = ChangedObject.map(async (item) => {
      if (item.antall) {
        let url = `UtvalgList/SaveUtvalgList?userName=${customername}`;

        try {
          let A = item;
          A.listId = 0;
          A.utvalgId = 0;
          let todaydate = new Date();
          let IsoFormat = todaydate.toISOString();
          A.distributionDate = IsoFormat;
          A.innleveringsDato = IsoFormat;

          const { data, status } = await api.postdata(url, A);

          if (status === 200) {
            resultObject.push(data);
          }
        } catch (e) {
          console.log("error", e);
        }
      } else {
        let saveOldReoler = "false";
        let skipHistory = "false";
        let forceUtvalgListId = 0;
        let url = `Utvalg/SaveUtvalg?userName=${customername}&`;
        url = url + `saveOldReoler=${saveOldReoler}&`;
        url = url + `skipHistory=${skipHistory}&`;

        url = url + `forceUtvalgListId=${forceUtvalgListId}`;

        item.modifications.push({
          modificationId: Math.floor(100000 + Math.random() * 900000),
          userId: customername,
          modificationTime: CurrentDate(),
          listId: 0,
        });
        item.kundeNummer = custNos;
        item.utvalgId = 0;
        item.listId = 0;

        const { data, status } = await api.postdata(url, item);

        if (status === 200) {
          resultObject.push(data);
        }
      }
    });

    return Promise.all(promiseAllResult).then(async () => {
      let listUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${utvalglistapiobject.listId}`;
      const { data, status } = await api.getdata(listUrl);

      let customername = "";
      if (username_kw) {
        customername = username_kw;
      } else {
        customername = "Internbruker";
      }
      if (status === 200 && data !== undefined) {
        let url = `UtvalgList/AddUtvalgsToExistingList?userName=${customername}`;
        // utvalgapiobject.listId = Number(`${data.listId}`);

        var listData = data;
        listData.kundeNummer = custNos;
        try {
          var a = {
            utvalgList: listData,
            utvalgs: resultObject,
          };
          const { data, status } = await api.postdata(url, a);
          if (status === 200) {
            window.$("#exampleModal-1").modal("hide");

            let sum = 0;

            let TempCartItems = CartItems;

            let Cartitem = [];
            CartItems.map((item) => {
              Cartitem.push(item);
            });
            resultObject.map((item) => {
              Cartitem.push(item);
            });

            for (let i = 0; i < Cartitem.length; i++) {
              if (Cartitem[i].antall) {
                sum = sum + Cartitem[i].antall;
              } else {
                sum = sum + Cartitem[i].totalAntall;
              }
            }

            resultObject.map((item) => {
              listData.memberUtvalgs.push(item);
            });
            listData.antall = sum;

            setCartItems(Cartitem);
            let commonSelections = filterCommonReolIds(Cartitem);
            setDuplicateArr(commonSelections.filteredCommonSelectionNames);
            if (commonSelections?.filteredCommonSelectionNames?.length > 0) {
              setDuplicate(true);
            }

            let Reolids = [];

            // let j = mapView.graphics.items.length;
            // var k = 0;
            // k = j;
            // for (var i = j; i > 0; i--) {
            //   if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
            //     mapView.graphics.remove(mapView.graphics.items[i - 1]);
            //     //j++;
            //   }
            // }
            let j = mapView.graphics.items.length;
            for (var i = j; i > 0; i--) {
              if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
                mapView.graphics.remove(mapView.graphics.items[i - 1]);
                //j++;
              }
            }
            let renderedCount = 0;
            Cartitem.map(async (items, index1) => {
              if (items.reoler && items.reoler.length > 0) {
                items.reoler.map((item) => {
                  if (
                    !commonSelections.filteredCommonItems.includes(item.reolId)
                  )
                    Reolids.push(item.reolId);
                });
                if (Reolids?.length) {
                  await MapRender(Reolids, ColorCodes()[index1]);
                }
                renderedCount = renderedCount + 1;
                if (renderedCount === Cartitem?.length) {
                  await mapRenderDoubleCoverage(commonSelections);
                  ZoomToAll();
                  renderedCount = 0;
                }
              }
            });

            setDateAdded(true);
            // setTotal(sum);

            setutvalgitemdisplay(true);
            setutvalglistapiobject(listData);
            setCartClickModalHide(true);
            window.$("#exampleModal-1").modal("hide");
            setTimeout(() => {
              setModalLoading(false);
            }, 10);

            btnclose_1.current.click();
          }
        } catch (e) {
          console.log("error", e);
        }
      }
    });
  };
  const AngiClick = () => {
    setPage_P("cartClick_Component_kw");
    setutvalgapiobject({});
    setPage("Geogra_distribution_click");
  };
  const EndreClick = (Endrevalue) => {
    if (Endrevalue.listId && Endrevalue.antall) {
      setEndreapiobject({});
      setEndrelistapiobject(Endrevalue);
    } else {
      setEndrelistapiobject({});
      setEndreapiobject(JSON.parse(JSON.stringify(Endrevalue)));
      setutvalgapiobject(JSON.parse(JSON.stringify(Endrevalue)));
    }
    setPage("EndreClick_kw");
  };

  const RemoveRouteUpdateWarnings = () => {
    setRoutepdate(false);
    setRoutepdateCount(0);
  };

  const AcceptAllChanges = async () => {
    setContentLoading(true);
    let NewListId = utvalglistapiobject.listId;
    let url = `Utvalg/AcceptAllChangesForList?userName=Internbruker&ListId=${NewListId}`;
    try {
      const { data, status } = await api.postdata(url);
      if (status === 200) {
        let updatedList = NewListId;

        let newlistUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${updatedList}`;
        const { data, status } = await api.getdata(newlistUrl);
        if (status === 200 && data !== undefined) {
          setutvalglistapiobject(data);
          setRoutepdate(false);
          setRoutepdateCount(0);
          LoadList(data);
        }

        // $(".modal").remove();
        // $(".modal-backdrop").remove();

        Swal.fire({
          text: `Budruteendringer er nå godkjent for alle utvalgene i lista.`,
          confirmButtonColor: "#7bc144",
          confirmButtonText: "Lukk",
          position: "top",
        });
        setContentLoading(false);
      } else {
        // $(".modal").remove();
        // $(".modal-backdrop").remove();
        setContentLoading(false);

        Swal.fire({
          text: `Oppgitte søkekriterier ga ikke noe resultat.`,
          confirmButtonColor: "#7bc144",
          confirmButtonText: "Lukk",
          position: "top",
        });
      }
    } catch (error) {
      console.error("error : " + error);

      // $(".modal").remove();
      // $(".modal-backdrop").remove();
      Swal.fire({
        text: `Oppgitte søkekriterier ga ikke noe resultat.`,
        confirmButtonColor: "#7bc144",
        confirmButtonText: "Lukk",
        position: "top",
      });

      LoadList(utvalglistapiobject);
    }
  };
  const GotoMain = () => {
    let j = mapView.graphics.items.length;
    for (var i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
        //j++;
      }
    }
    setCartItems([]);
    setutvalgapiobject({});
    setutvalglistapiobject([]);
    setnewhome(false);
    setPage("");
  };
  const lestillUtvalg = () => {
    window.$("#lestillUtvalg").modal("show");
  };

  const skrivUtvalget = () => {
    setSkrivUtvalgetName("skrivUtvalget");
  };

  const KopierClickfun = async () => {
    var resultObject = [];
    setModalLoading(true);
    setContentLoading(true);

    setCartClickModalHide(true);

    let ItemsTobeChanged = [];
    KopierCheckedItems.map((item) => {
      if (item.memberUtvalgs && item.memberUtvalgs.length > 0) {
        item.memberUtvalgs.map((item) => {
          ItemsTobeChanged.push(item);
        });
      } else if (item.memberUtvalgs === undefined) {
        ItemsTobeChanged.push(item);
      }
    });
    let ItemsToBeChangedOrg = [];
    let ReolPromises = ItemsTobeChanged.map(async (item) => {
      let itemUrl = `Utvalg/GetUtvalg?utvalgId=${item.utvalgId}`;
      const { data, status } = await api.getdata(itemUrl, item);
      if (status === 200) {
        ItemsToBeChangedOrg.push(data);
      }
    });
    Promise.all(ReolPromises).then(async () => {
      let TempDate = new Date();
      var TempAddingValue;
      if (AddDate) {
        TempAddingValue = moment(TempDate).format("DD-MM-YYYY");
      }
      if (AddTime) {
        TempAddingValue = moment(TempDate).format("DD-MM-YYYY HH:MMM:SS");
      }
      if (AddCustomText) {
        TempAddingValue = CustomText;
      }
      let ChangedObject = ItemsToBeChangedOrg.map((item) => {
        item.name = item.name + "-" + TempAddingValue;
        item.utvalgId = 0;
        item.listId = 0;
        item.listName = "";
        item.logo = item.logo ? item.logo : "";
        // if (item.utvalgId) {
        //   item.utvalgId =
        //     item.utvalgId?.toString().charAt(0) === "U"
        //       ? Number(item.utvalgId?.toString().slice(1))
        //       : Number(item?.utvalgId);
        // }
        // if (item.listId) {
        //   item.listId =
        //     item.listId?.toString().charAt(0) === "L"
        //       ? Number(item.listId?.toString().slice(1))
        //       : Number(item?.listId);
        // }

        // if (item.kundeNummer === null && item.kundeNummer === undefined) {
        //   item.kundeNummer = custNos;
        // }
        item.kundeNummer = custNos;
        return { ...item };
      });

      let customername = "";
      if (username_kw) {
        customername = username_kw;
      } else {
        customername = "Internbruker";
      }

      // let promiseAllResult = ChangedObject.map(async (item) => {
      //   console.log(ChangedObject,"ChangedObject")
      //   if (item.antall) {
      //     let url = `UtvalgList/SaveUtvalgList?userName=${customername}`;
      //     try {
      //       let A = item;
      //       A.listId = 0;
      //       A.utvalgId = 0;
      //       let todaydate = new Date();
      //       let IsoFormat = todaydate.toISOString();
      //       A.distributionDate = IsoFormat;
      //       A.innleveringsDato = IsoFormat;

      //       const { data, status } = await api.postdata(url, A);

      //       if (status === 200) {
      //         resultObject.push(data);
      //       }
      //     } catch (e) {
      //       console.log("error", e);
      //     }
      //   }
      // });

      // return Promise.all(promiseAllResult).then(async () => {
      let listUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${utvalglistapiobject.listId}`;

      const { data, status } = await api.getdata(listUrl);

      customername = "";
      if (username_kw) {
        customername = username_kw;
      } else {
        customername = "Internbruker";
      }
      if (status === 200 && data !== undefined) {
        let url = `UtvalgList/AddUtvalgsToExistingList?userName=${customername}`;
        // utvalgapiobject.listId = Number(`${data.listId}`);

        var listData = data;

        ChangedObject?.map(item=>{
          listData?.memberUtvalgs?.push(item);
        })

        // if (
        //   listData.kundeNummer === null &&
        //   listData.kundeNummer === undefined
        // ) {
        //   listData.kundeNummer = custNos;
        // }
        listData.kundeNummer = custNos;
        try {
          var a = {
            utvalgList: listData,
            utvalgs: ChangedObject,
          };
          const { data, status } = await api.postdata(url, a);
          if (status === 200) {
            window.$("#exampleModal-1").modal("hide");
            let sum = 0;

            let listUrl1 = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${utvalglistapiobject.listId}`;

            const { data, status } = await api.getdata(listUrl1);
            let Cartitem = [];

            if (status === 200) {
              data?.memberUtvalgs.map((item) => {
                Cartitem.push(item);
              });

              // CartItems.map((item) => {
              //   Cartitem.push(item);
              // });
              // ChangedObject.map((item) => {
              //   Cartitem.push(item);
              // });

              // for (let i = 0; i < Cartitem.length; i++) {
              //   if (Cartitem[i].antall) {
              //     sum = sum + Cartitem[i].antall;
              //   } else if (Cartitem[i].totalAntall) {
              //     sum = sum + Cartitem[i].totalAntall;
              //   }
              // }
              // ChangedObject.map((item) => {
              //   listData.memberUtvalgs.push(item);
              // });
              // listData.antall = sum;

              setCartItems(Cartitem);
              let commonSelections = filterCommonReolIds(Cartitem);
              setDuplicateArr(commonSelections.filteredCommonSelectionNames);
              if (commonSelections?.filteredCommonSelectionNames?.length > 0) {
                setDuplicate(true);
              }
              let Reolids = [];

              let j = mapView.graphics.items.length;
              for (var i = j; i > 0; i--) {
                if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
                  mapView.graphics.remove(mapView.graphics.items[i - 1]);
                  //j++;
                }
              }

              let renderedCount = 0;
              Cartitem.map(async (items, index1) => {
                if (items.reoler && items.reoler.length > 0) {
                  items.reoler.map((item) => {
                    if (
                      !commonSelections?.filteredCommonItems?.includes(
                        item.reolId
                      )
                    )
                      Reolids.push(item.reolId);
                  });
                  if (Reolids?.length) {
                    await MapRender(Reolids, ColorCodes()[index1]);
                  }
                  renderedCount += 1;
                  if (renderedCount === Cartitem?.length) {
                    await mapRenderDoubleCoverage(commonSelections);
                    ZoomToAll();
                    renderedCount = 0;
                  }
                }
              });

              setDateAdded(true);

              setutvalgitemdisplay(true);
              setCartClickModalHide(true);
              setutvalglistapiobject(data);
              btnclose_1.current.click();
              window.$("#lestillUtvalg").modal("hide");
              setModalLoading(false);
              setContentLoading(false);
            }
            else{
              console.log("Something went wrong");
            }
          }
        } catch (e) {
          console.log("error", e);
        }
      }
      // });
    });
  };

  const warninput = () => {
    setmelding(false);
    let textinput = document.getElementById("warntext").value;
    setwarninputvalue_1(textinput);
  };
  const desinputonchange = () => {
    let desctextvalue = document.getElementById("desctext").value;
    setdesinput(desctextvalue);
  };
  const cartClick = () => {
    setPage("CartClick");
  };
  const LeggFlereClick = () => {
    setnewhome(true);
    setPage("");
  };

  const ZoomToAll = () => {
    let featuresGeometry = [];
    mapView.graphics.items.map((item) => {
      if (item.geometry.type === "polygon") {
        featuresGeometry.push(item.geometry);
      }
    });
    mapView.goTo(featuresGeometry);
  };

  return (
    <div className="col-5 p-2">
      <div className="padding_NoColor_B cursor">
        <a>
          <div className="handlekurv handlekurvText pl-2">
            Du har {CartItems.length} utvalg i bestillingen din.
          </div>
        </a>
      </div>
      {ModalOpenID === "ForHandlerListModal" ? (
        <ForHandlerListModel_KW
          title={"Endre forhandlerpåtrykk"}
          id={"visEditKW"}
          data={utvalglistapiobject.memberLists}
          memberUtvalgs={utvalglistapiobject.memberUtvalgs}
        />
      ) : null}
      {LagreList == "Save_Large" ? (
        <SaveUtvalgListKW
          id={"uxBtnLagreSomList"}
          loadList={LoadList}
          RemoveRouteUpdateWarnings={RemoveRouteUpdateWarnings}
          // AcceptAllChanges={AcceptAllChaanges}
        />
      ) : null}
      <div
        className="modal fade bd-example-modal-lg"
        data-backdrop="false"
        id="lestillUtvalg"
        tabIndex="-1"
        role="dialog"
        aria-labelledby="exampleModalCenterTitle"
        aria-hidden="true"
      >
        <div className="modal-dialog viewDetail" role="document">
          <div className="modal-content" style={{ border: "black 4px solid" }}>
            <div className="Common-modal-header">
              <span className="dialog-kw" id="exampleModalLongTitle">
                LAGRE UTVALGSLISTE{" "}
              </span>
              <button
                type="button"
                className="close"
                data-dismiss="modal"
                aria-label="Close"
              >
                <span aria-hidden="true">&times;</span>
              </button>
            </div>
            <div className="View_modal-body pl-2">
              <table className="lestill">
                <tbody>
                  <tr>
                    <td>
                      <span className="SaveUtvaldivLabelText ml-1">
                        De nye utvalgene skal ha navn som ender med:
                      </span>
                    </td>
                  </tr>
                  <td>
                    {melding_1 ? (
                      <span className=" sok-Alert-text pl-1">{errormsg_1}</span>
                    ) : null}
                    {melding_1 ? <td></td> : null}
                  </td>
                  <tr>
                    <td ref={fylkeChkBtmDiv}>
                      <input
                        className="mt-1 ml-1"
                        type="radio"
                        onChange={handleChangeBtm}
                        value={0}
                        id="FylkeB"
                        ref={fylkeChkBtm}
                      />
                      <label
                        className="form-check-label reportLabel"
                        htmlFor="FylkeB"
                      >
                        {" "}
                        Dagens dato{" "}
                      </label>
                    </td>
                  </tr>
                  <tr>
                    <td ref={kommuneChkBtmDiv}>
                      <input
                        className="mt-1 ml-1"
                        type="radio"
                        onChange={handleChangeBtm}
                        value={1}
                        id="KommuneB"
                        ref={kommuneChkBtm}
                      />
                      <label
                        className="form-check-label reportLabel"
                        htmlFor="KommuneB"
                      >
                        {" "}
                        Dagens dato og klokkeslett{" "}
                      </label>
                    </td>
                  </tr>
                  <tr>
                    <td ref={teamChkBtmDiv}>
                      <input
                        className="mt-1 ml-1"
                        type="radio"
                        onChange={handleChangeBtm}
                        value={2}
                        id="TeamB"
                        ref={teamChkBtm}
                      />
                      <label
                        className="form-check-label reportLabel"
                        htmlFor="TeamB"
                      >
                        {" "}
                        Egendefinert tekst{" "}
                      </label>
                    </td>
                  </tr>

                  <tr>
                    <td>
                      <span
                        id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxLogoLabel"
                        className="SaveUtvaldivLabelText pl-2"
                      >
                        Egendefinert tekst
                      </span>
                      &nbsp; &nbsp; &nbsp;
                      <input
                        className=""
                        name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxLogo"
                        type="text"
                        id="uxLogo"
                        value={CustomText}
                        onChange={CustomTextfun}
                        disabled={DisableEgendefinert}
                      />
                    </td>
                  </tr>
                  <br />
                  <tr>
                    <td>
                      <button
                        type="button"
                        className="KSPU_button_Gray"
                        data-dismiss="modal"
                        ref={btnclose_1}
                      >
                        Avbryt
                      </button>
                    </td>
                    <td></td>
                    <td>
                      <button
                        type="button"
                        onClick={KopierClickfun}
                        className="KSPU_button-kw"
                        disabled={modalLoading}
                      >
                        Lagre
                      </button>
                    </td>
                  </tr>
                  <br />
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>

      <div id="uxShowUtvalgListDetails_uxContents">
        <div id="uxShowUtvalgListDetails_uxInnerContents">
          <div className="padding_Color_L_R_T_B clearFloat">
            <div className="titleWizard padding_NoColor_B">
              "
              <span
                id="uxShowUtvalgListDetails_uxLblListName"
                className="green wordbreak"
              >
                {utvalglistapiobject.name}
              </span>
              " har{" "}
              <span
                id="uxShowUtvalgListDetails_uxLblListCountUtvalg"
                className="green"
              >
                {CartItems.length}
              </span>{" "}
              utvalg
            </div>
            {duplicate ? (
              <div
                className="cartClick-error WarningSign"
                style={{ margin: "10px 0px 10px 0px" }}
              >
                <div className="divErrorHeading flex-center">
                  Noen av utvalgene dine overlapper:
                </div>

                <p id="uxKjoreAnalyse_uxLblMessage" className="divErrorText_kw">
                  Budruter som det finnes i mer enn ett utvalg er markert grønt
                  i kartet
                  <span className="green-marker">
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                  </span>
                  <br />
                  <br />
                  Du kan fjerne budruterfra utvalg ved å klikke 'Endre' i listen
                  under
                </p>
              </div>
            ) : null}
            {routeUpdate ? (
              <div
                className="cartClick-error WarningSign"
                style={{ margin: "10px 0px 10px 0px" }}
              >
                <div className="divErrorHeading">
                  {routeUpdateCount} av utvalgene har blitt endret
                </div>

                <p id="uxKjoreAnalyse_uxLblMessage" className="divErrorText_kw">
                  Dette har skjedd på grunn av endringer i budrutene eller for
                  ajourføre antall mottakere ved tilflyttning og fraflytting.
                  <br />
                  <br />
                  Du kan se hva som er endret i listen nedenfor. Klikk på et
                  utvalg for å se endringer per utvalg. Her kan du også få en
                  detaljert oversikt over endringene på budrute.
                </p>
              </div>
            ) : null}
            {utvalglistapiobject.ordreType === 1 ? (
              <div className="pr-3">
                <div className="error WarningSign">
                  <div className="divErrorHeading">Melding:</div>
                  <p
                    id="uxKjoreAnalyse_uxLblMessage"
                    className="divErrorText_kw"
                  >
                    Valgt bestilling har allerede et ordrenummer. Du kan kopiere
                    bestillingen.
                  </p>
                </div>
              </div>
            ) : null}
            {/* ---modals  starting */}
            <div
              className="modal fade bd-example-modal-lg"
              id="exampleModal"
              tabIndex="-1"
              role="dialog"
              aria-labelledby="exampleModalCenterTitle"
              aria-hidden="true"
            >
              <div
                className="modal-dialog modal-dialog-centered "
                role="document"
              >
                <div className="modal-content">
                  <div className="Common-modal-header">
                    <div className="divDockedPanel">
                      <div className=" divDockedPanelTop">
                        <span className="dialog-kw" id="exampleModalLabel">
                          KVITTERING
                        </span>
                        <button
                          type="button"
                          className="close"
                          data-dismiss="modal"
                          aria-label="Close"
                        >
                          <span aria-hidden="true">&times;</span>
                        </button>
                      </div>
                      <div className="View_modal-body pl-2">
                        <table>
                          <tbody>
                            <tr>
                              <td>
                                <p className="p-slett">
                                  &nbsp; Lagring utført.
                                </p>{" "}
                              </td>
                              <td></td>
                            </tr>

                            <tr>
                              <td>
                                <div className="ml-4">
                                  <button
                                    type="button"
                                    className="modalMessage_button"
                                    data-dismiss="modal"
                                  >
                                    Lukk
                                  </button>{" "}
                                </div>
                              </td>
                            </tr>
                            <br />
                          </tbody>
                        </table>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            {/* -----savemodal ending---- */}
            {/* ----------selection legg till modal modal starting----- */}
            <div
              className="modal show"
              id="exampleModal-1"
              tabIndex="-1"
              role="dialog"
              aria-labelledby="exampleModalCenterTitle"
              aria-hidden="true"
              data-backdrop="false"
            >
              <div
                className="modal-dialog modal-dialog-centered "
                role="document"
              >
                <div className="modal-content">
                  <div className="">
                    {/* <div className =""> */}
                    <div className=" divDockedPanelTop">
                      <span className="dialog-kw" id="exampleModalLabel">
                        Angi navn på nytt utvalg{" "}
                      </span>
                      <button
                        type="button"
                        className="close pr-2"
                        data-dismiss="modal"
                        aria-label="Close"
                      >
                        <span aria-hidden="true">&times;</span>
                      </button>
                    </div>
                    <div className="View_modal-body-appneet pl-2">
                      <p></p>
                      {melding ? (
                        <span className=" sok-Alert-text pl-1">{errormsg}</span>
                      ) : null}
                      {melding ? <p></p> : null}
                      <label className="divValueText">Utvalgsnavn</label>
                      <input
                        type="text"
                        maxLength="50"
                        value={warninputvalue_1}
                        onChange={warninput}
                        id="warntext"
                        className="inputwidth"
                      />
                      <br />

                      <label className="divValueText">Forhandlerpåtrykk </label>
                      <input
                        type="text"
                        maxLength="50"
                        value={desinput}
                        onChange={desinputonchange}
                        id="desctext"
                        className="inputwidth"
                      />

                      <p></p>

                      <div className="div_left">
                        <input
                          type="submit"
                          name="DemografiAnalyse1$uxFooter$uxBtForrige"
                          value="Avbryt"
                          data-dismiss="modal"
                          className="KSPU_button_Gray"
                          ref={btnclose_Avbryt}
                        />
                      </div>
                      <div className="div-right">
                        <input
                          type="submit"
                          name="uxDistribusjon$uxDistSetDelivery"
                          value="Lagre"
                          onClick={LagreClickFun}
                          id="uxDistribusjon_uxDistSetDelivery"
                          className="KSPU_button-kw float-right"
                          disabled={modalLoading ? true : false}
                        />
                      </div>

                      <br />
                      <br />
                    </div>
                  </div>
                </div>
              </div>
            </div>
            {/* ---------selection modal ending --------- */}
            <div
              className="modal fade  bd-example-modal-lg"
              id="exampleModal-1"
              tabIndex="-1"
              data-backdrop="false"
              role="dialog"
              aria-labelledby="exampleModalCenterTitle"
              aria-hidden="true"
            >
              <div
                className="modal-dialog modal-dialog-centered  "
                role="document"
              >
                <div className="modal-content">
                  <div className="">
                    <div className=" divDockedPanelTop">
                      <span className="dialog-kw" id="exampleModalLabel">
                        Endre beskrivelse
                      </span>
                      <button
                        type="button"
                        className="close pr-2"
                        data-dismiss="modal"
                        aria-label="Close"
                        ref={btnclose}
                      >
                        <span aria-hidden="true">&times;</span>
                      </button>
                    </div>
                    <div className="View_modal-body-appneet pl-2">
                      {!melding1 ? <p></p> : null}
                      {melding1 ? (
                        <div className="pr-3">
                          <div className="error WarningSign">
                            <div className="divErrorHeading">Melding:</div>
                            <p
                              id="uxKjoreAnalyse_uxLblMessage"
                              className="divErrorText_kw"
                            >
                              {errormsg1}
                            </p>
                          </div>
                        </div>
                      ) : null}
                      {melding1 ? (
                        <div>
                          <p></p>
                        </div>
                      ) : null}

                      <input
                        type="text"
                        maxLength="50"
                        placeholder="Navn på bestil"
                        value={warninputvalue_1}
                        onChange={warninput}
                        id="warntext"
                        className="inputwidth"
                      />

                      <p></p>
                      <div className="">
                        <div className="div_left">
                          <input
                            type="submit"
                            name="DemografiAnalyse1$uxFooter$uxBtForrige"
                            value="Avbryt"
                            data-dismiss="modal"
                            className="KSPU_button_Gray"
                          />
                          &nbsp;&nbsp;
                          <input
                            type="submit"
                            name="uxDistribusjon$uxDistSetDelivery"
                            value="Lagre"
                            // onClick={""}
                            id="uxDistribusjon_uxDistSetDelivery"
                            className="KSPU_button-kw"
                          />
                        </div>
                        <div className="padding_NoColor_T clearFloat sok-text">
                          Du kan senere finne igjen bestillingen ved å klikke{" "}
                          <br />
                          <a className="read-more_ref" href="javscript:">
                            ÅPNE ET LAGRET UTVALG{" "}
                          </a>{" "}
                          på forsiden
                        </div>
                      </div>
                      <br />
                    </div>
                  </div>
                </div>
              </div>
            </div>{" "}
            {/* endre click modal ends------ */}
            <div id="uxShowUtvalgListDetails_uxHandlekurv_uxContents">
              {routeUpdate ? (
                <div className="padding_NoColor_B clearFloat">
                  <table className="tablestyle">
                    <tbody>
                      <tr>
                        <td>
                          <div id="uxShowUtvalgListDetails_uxHandlekurv_uxDivGridContainer">
                            {contentLoading ? (
                              <div className="Adressepunkt_og_fastantallsanalyseSpinnerDiv">
                                <img
                                  className="mb-1"
                                  src={spinner}
                                  style={{ height: 30, width: 30 }}
                                />
                              </div>
                            ) : (
                              <div className="table-container">
                                <table
                                  cellSpacing="0"
                                  rules="rows"
                                  border="0"
                                  id="uxShowUtvalgListDetails_uxHandlekurv_uxGrdMemberUtv"
                                  className="cellcollapse"
                                >
                                  <tbody>
                                    <tr className="bgGrey">
                                      <th></th>
                                      <th></th>
                                      <th></th>
                                      <th></th>
                                      <th
                                        style={{
                                          textAlign: "end",
                                          width: "15%",
                                        }}
                                        className="yellowcoding pr-1"
                                      >
                                        Før
                                      </th>
                                      <th
                                        style={{
                                          textAlign: "end",
                                          width: "15%",
                                        }}
                                        className="coding white-text pr-1"
                                      >
                                        Nå
                                      </th>
                                    </tr>

                                    {CartItems.length > 0
                                      ? CartItems.map((item, index) => (
                                          <tr
                                            className="GridView_Row_kw cart-table-tr"
                                            key={index}
                                          >
                                            <td align="center">
                                              <img
                                                id="uxShowUtvalgListDetails_uxHandlekurv_uxGrdMemberUtv_ctl02_uxKartSymbol"
                                                src={imagePath(index + 1)}
                                                className="imgstyle"
                                              />
                                            </td>
                                            <td className="padding-horizontal table-text wordbreak">
                                              <a
                                                // style={{
                                                //   wordBreak: "normal",
                                                //   cursor: "pointer",
                                                // }}
                                                onClick={() => EndreClick(item)}
                                              >
                                                {item.name}
                                              </a>
                                            </td>
                                            <td className="padding-horizontal">
                                              <a
                                                className="KSPU_LinkButton_Url sok-text custom_link"
                                                onClick={() => EndreClick(item)}
                                              >
                                                <div>Endre</div>
                                              </a>
                                            </td>
                                            <td>
                                              <img
                                                className={
                                                  item?.antallBeforeRecreation >
                                                  0
                                                    ? "float-right pt-1 mr-1 warningLogoSmall warning-sign"
                                                    : "hidden"
                                                }
                                                src={warningImg}
                                              />
                                            </td>
                                            <td
                                              align="right"
                                              className={
                                                item.antallBeforeRecreation > 0
                                                  ? "yellowcoding tdwidth small-text"
                                                  : "tdwidth small-text"
                                              }
                                            >
                                              {item.antallBeforeRecreation > 0
                                                ? NumberFormat(
                                                    item.antallBeforeRecreation
                                                  )
                                                : null}
                                            </td>
                                            <td
                                              align="right"
                                              className="tdwidth small-text"
                                            >
                                              {NumberFormat(item.totalAntall)}
                                            </td>
                                          </tr>
                                        ))
                                      : null}
                                  </tbody>
                                </table>
                              </div>
                            )}
                          </div>
                        </td>
                      </tr>
                    </tbody>
                  </table>

                  <table className="tdwidth1">
                    <tbody>
                      <tr className="bold">
                        <td className="sok-text-1">
                          Totalt, alle utvalg i bestillingen
                        </td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td
                          style={{ textAlign: "end", width: "15%" }}
                          className="yellowcoding"
                        >
                          {NumberFormat(antallBeforeRecreationSum)}
                        </td>
                        <td
                          style={{ textAlign: "end", width: "15%" }}
                          className="coding"
                        >
                          {NumberFormat(Total)}
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              ) : (
                <div>
                  <div className="table-container">
                    <table className="tablestyle">
                      <tbody>
                        <tr>
                          <td>
                            <div id="uxShowUtvalgListDetails_uxHandlekurv_uxDivGridContainer">
                              {contentLoading ? (
                                <div className="Adressepunkt_og_fastantallsanalyseSpinnerDiv">
                                  <img
                                    className="mb-1"
                                    src={spinner}
                                    style={{ height: 30, width: 30 }}
                                  />
                                </div>
                              ) : (
                                <div>
                                  <table
                                    cellSpacing="0"
                                    rules="rows"
                                    border="0"
                                    id="uxShowUtvalgListDetails_uxHandlekurv_uxGrdMemberUtv"
                                    className="cellcollapse"
                                  >
                                    <tbody>
                                      {CartItems.length > 0
                                        ? CartItems.map((item, index) => (
                                            <tr
                                              className="GridView_Row_kw cart-table-tr"
                                              key={index}
                                            >
                                              <td align="center">
                                                <img
                                                  id="uxShowUtvalgListDetails_uxHandlekurv_uxGrdMemberUtv_ctl02_uxKartSymbol"
                                                  src={imagePath(index + 1)}
                                                  className="imgstyle"
                                                />
                                              </td>
                                              <td>
                                                <a
                                                  className="table-text wordbreak"
                                                  onClick={() =>
                                                    EndreClick(item)
                                                  }
                                                >
                                                  {item.name}
                                                </a>
                                              </td>
                                              <td className="padding-horizontal">
                                                <a
                                                  className="KSPU_LinkButton_Url sok-text custom_link"
                                                  onClick={() =>
                                                    EndreClick(item)
                                                  }
                                                >
                                                  <div>Endre</div>
                                                </a>
                                              </td>
                                              <td>
                                                <img
                                                  className={
                                                    duplicateArr.includes(
                                                      item?.name
                                                    )
                                                      ? "float-right pt-1 mr-1 warningLogoSmall warning-sign"
                                                      : "hidden"
                                                  }
                                                  src={warningImg}
                                                />
                                              </td>

                                              {item.antall ? (
                                                <td
                                                  align="right"
                                                  className="tdwidth small-text"
                                                >
                                                  {NumberFormat(item.antall)}
                                                </td>
                                              ) : (
                                                <td
                                                  align="right"
                                                  className="tdwidth small-text"
                                                >
                                                  {NumberFormat(
                                                    item.totalAntall
                                                  )}
                                                </td>
                                              )}
                                            </tr>
                                          ))
                                        : null}
                                    </tbody>
                                  </table>
                                </div>
                              )}
                            </div>
                          </td>
                        </tr>
                      </tbody>
                    </table>
                  </div>
                  <table className="tdwidth1">
                    <tbody>
                      <tr className="bold">
                        <td className="sok-text-1">
                          Totalt, alle utvalg i bestillingen
                        </td>

                        <td className="tdwidth2">
                          <span id="uxShowUtvalgListDetails_uxHandlekurv_uxLblTotAnt">
                            {NumberFormat(Total)}
                          </span>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              )}
            </div>
            <div>
              <div>
                <div className="gray">
                  ID:{" "}
                  <span
                    id="uxShowUtvalgListDetails_uxLblRefNr"
                    className="gray"
                  >
                    {"L" + utvalglistapiobject.listId}
                  </span>
                </div>
              </div>

              <div>
                <a
                  id="uxShowUtvalgListDetails_uxEditForhandler"
                  className="KSPU_LinkButton_Url sok-text"
                  onClick={EndreBeskrivelseClick}
                  data-toggle="modal"
                  data-target="#visEditKW"
                >
                  Endre beskrivelse
                </a>
              </div>
            </div>
          </div>

          <div className="padding_NoColor_T paddingBig_NoColor_B clearFloat">
            <div className="div_left">
              <a
                id="uxShowUtvalgListDetails_uxBtnAvbryt"
                className="KSPU_LinkButton_Url_KW"
                onClick={GotoMain}
              >
                Avbryt
              </a>
            </div>

            <div className="div_right">
              {utvalglistapiobject?.ordreType === 1 ? (
                <input
                  type="submit"
                  name="saveUtvalgLagreEndringer"
                  value="Kopier liste"
                  id="uxBtnLagreSom"
                  className="KSPU_button mr-1"
                  data-toggle="modal"
                  data-target="#uxBtnLagreSomList"
                  onClick={showLagreList}
                  ref={uxBtnLagreSom}
                  class="KSPU_button-kw"
                />
              ) : routeUpdate ? (
                <input
                  type="submit"
                  name="uxDistribusjon$uxDistSetDelivery"
                  value="Aksepter alle endringer"
                  id="uxDistribusjon_uxDistSetDelivery"
                  onClick={AcceptAllChanges}
                  className="KSPU_button-kw"
                  style={{ width: "175px" }}
                />
              ) : (
                <input
                  type="submit"
                  name="uxDistribusjon$uxDistSetDelivery"
                  value="Angi distribusjonsdetaljer"
                  id="uxDistribusjon_uxDistSetDelivery"
                  onClick={AngiClick}
                  className="KSPU_button-kw"
                  style={{ width: "175px" }}
                />
              )}
            </div>
          </div>
          <div>
            {skrivUtvalgetName === "skrivUtvalget" ? (
              <StandardreportKw
                title={"skrivUtvalget"}
                id={"skrivUtvalget"}
                modalshow={true}
                isList={utvalglistcheck}
              />
            ) : null}
          </div>

          <table className="wizUnfilled paddingBig_NoColor_T clearFloat">
            <tbody>
              <tr>
                <td className="bold">Du kan også..</td>
              </tr>

              <tr>
                <td>
                  <a
                    onClick={LeggFlereClick}
                    id="uxShowUtvalgListDetails_uxAddMoreToList"
                    className={
                      utvalglistapiobject.ordreType === 1
                        ? "invisible"
                        : "KSPU_LinkButton1_Url margin"
                    }
                  >
                    <img src={readmore} />
                    &nbsp;Legg flere utvalg til bestillingen
                  </a>
                </td>
              </tr>

              <tr>
                <td>
                  <a
                    // id="uxShowUtvalgListDetails_uxAddMoreToList"
                    id="uxBtnLagreSom"
                    className={
                      utvalglistapiobject.ordreType === 1
                        ? "invisible"
                        : "KSPU_LinkButton1_Url margin"
                    }
                    data-toggle="modal"
                    data-target="#uxBtnLagreSomList"
                    onClick={showLagreList}
                    // data-target="#lestillUtvalg"
                    // onClick={lestillUtvalg}
                  >
                    <img src={readmore} />
                    &nbsp; Kopiere bestillingen
                  </a>
                </td>
              </tr>

              <tr>
                <td>
                  <a
                    id="uxShowUtvalgListDetails_uxAddMoreToList"
                    className="KSPU_LinkButton1_Url margin"
                    data-toggle="modal"
                    data-target="#skrivUtvalget"
                    onClick={skrivUtvalget}
                  >
                    <img src={readmore} />
                    &nbsp; Skriv ut bestillingen
                  </a>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

export default CartClick_Component_Kw;
