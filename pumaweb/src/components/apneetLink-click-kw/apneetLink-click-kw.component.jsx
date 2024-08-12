import React, { useState, useContext } from "react";
import { KundeWebContext } from "../../context/Context";
import minus from "../../assets/images/minus2.gif";
import plus from "../../assets/images/plus2.gif";
import api from "../../services/api.js";
import "./apneetLink-click-kw.styles.scss";
import loadingImage from "../../assets/images/callbackActivityIndicator.gif";
import { MainPageContext } from "../../context/Context.js";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";
import Swal from "sweetalert2";
import FeatureLayer from "@arcgis/core/layers/FeatureLayer";
import Spinner from "../spinner/spinner.component";
import { CreateUtvalglist } from "../../common/Functions";
import ShowRecreateKW from "../ShowReCreateService-kw";

function ApneetLinkClick() {
  // const { custNos, setcustNos } = useContext(MainPageContext);
  const { custNos } = useContext(KundeWebContext);
  const { avtaleData, setavtaleData } = useContext(MainPageContext);
  const { mapView } = useContext(MainPageContext);
  const { Page, setPage } = useContext(KundeWebContext);
  const [nomessagediv, setnomessagediv] = useState(false);
  const [plusicon, setplusicon] = useState(false);
  const [Apneetinput, setApneetinput] = useState("");
  const [sokresult, setsokresult] = useState([]);
  const [showresult, setshowresult] = useState(false);
  const [plusicon_1, setplusicon_1] = useState(false);
  const [plusicon_2, setplusicon_2] = useState(false);
  const [sokcompletedorders, setsokcompletedorders] = useState([]);
  const [sortdatecheck, setsortdatecheck] = useState(true);
  const [sortnamecheck, setsortnamecheck] = useState(false);
  const [listresult, setlistresult] = useState([]);
  const [scrollbar, setscrollbar] = useState(false);
  const [scrollbar1, setscrollbar1] = useState(false);
  const [UtvalgID, setUtvalgID] = useState("");
  const [Modal, setModal] = useState(false);
  const [disable, setdisable] = useState(true);
  const [loading, setloading] = React.useState([true]);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  const [Item, setItem] = useState({});
  const [Type, setType] = useState("");
  const { newhome, setnewhome } = useContext(KundeWebContext);
  const { CartItems, setCartItems } = useContext(KundeWebContext);
  const { utvalglistapiobject, setutvalglistapiobject } =
    useContext(KundeWebContext);
  const [buttondisplay, setbuttondisplay] = useState(false);
  const [recreateType, setrecreateType] = useState("");
  const [recreateId, setrecreateId] = useState(0);
  const [melding, setmelding] = useState(false);
  const [errormsg, seterrormsg] = useState("");

  const GotoMain = () => {
    setPage("");
  };
  const datesorting = () => {
    setsortdatecheck(true);
    setsortnamecheck(false);
    let Temp = sokcompletedorders;
    let Date_Sort = Temp.sort(function (a, b) {
      // if (a.modifications.length !== 0) {
      //   var c = new Date(
      //     a.modifications[
      //       a.modifications.length - 1
      //     ].modificationTime.substring(0, 10)
      //   );
      var c = new Date(a.modificationDate.substring(0, 10));
      //}
      // if (b.modifications.length !== 0) {
      //   var d = new Date(
      //     b.modifications[
      //       b.modifications.length - 1
      //     ].modificationTime.substring(0, 10)
      //   );
      // }
      var d = new Date(b.modificationDate.substring(0, 10));
      return d - c;
    });
    setsokcompletedorders(Date_Sort);
    let TempListresult = listresult;
    let Date_Sortlist = TempListresult.sort(function (a, b) {
      var c = new Date(a.modificationDate.substring(0, 10));
      var d = new Date(b.modificationDate.substring(0, 10));
      return d - c;
      // return c - d;
    });
    setlistresult(Date_Sortlist);

    let Temp1 = sokresult;
    let Date_Sort1 = Temp1.sort(function (a, b) {
      // if (a.modifications.length !== 0) {
      //   var c = new Date(
      //     a.modifications[
      //       a.modifications.length - 1
      //     ].modificationTime.substring(0, 10)
      //   );
      // }
      var c = new Date(a.modificationDate.substring(0, 10));
      // if (b.modifications.length !== 0) {
      //   var d = new Date(
      //     b.modifications[
      //       b.modifications.length - 1
      //     ].modificationTime.substring(0, 10)
      //   );
      // }
      var d = new Date(b.modificationDate.substring(0, 10));
      return d - c;
      // return c - d;
    });
    setsokresult(Date_Sort1);
  };
  const namesorting = () => {
    setsortdatecheck(false);
    setsortnamecheck(true);
    let Temp = sokcompletedorders;
    let Name_Sort = Temp.sort(function (a, b) {
      return Intl.Collator("no", { numeric: true }).compare(a.name, b.name);
    });
    setsokcompletedorders(Name_Sort);
    let Temp1 = sokresult;
    let Name_Sort1 = Temp1.sort(function (a, b) {
      return Intl.Collator("no", { numeric: true }).compare(a.name, b.name);
    });
    setsokresult(Name_Sort1);
    let Temp2 = listresult;
    let Name_Sort2 = Temp2.sort(function (a, b) {
      return Intl.Collator("no", { numeric: true }).compare(a.name, b.name);
    });
    setlistresult(Name_Sort2);
  };
  const LeggClick = () => {
    setPage("Lestill_Click_Component");
  };
  const Jaclick = async () => {
    try {
      let ID = 0;
      let url = "";
      let utvalgType = "";
      if (Item.listId && Item.antall !== undefined) {
        ID = Item.listId.substring(1);
        url = "UtvalgList/DeleteUtvalgList?UtvalgListId=";
        utvalgType = "UtvalgList";
      }
      if (Item.utvalgId) {
        ID = Item.utvalgId.substring(1);
        url = "Utvalg/DeleteUtvalg?utvalgId=";
        utvalgType = "Utvalg";
      }
      const { data, status } = await api.deletedata(url + ID);
      if (status === 200) {
        if (Type === "progress") {
          let ResultAfterDelete = sokresult.filter(
            (value) =>
              value.utvalgId !== Item.utvalgId || value.listId !== Item.listId
          );
          if (data === true && utvalgType === "Utvalg") {
            Swal.fire({
              text: "Utvalg slettet",
              confirmButtonColor: "#7bc144",
              position: "top",
              icon: "success",
            });
          }
          if (data === true && utvalgType === "UtvalgList") {
            Swal.fire({
              text: "liste slettet",
              confirmButtonColor: "#7bc144",
              position: "top",
              icon: "success",
            });
          }
          setsokresult(ResultAfterDelete);
        }
        if (Type === "lister") {
          let ResultAfterDelete = listresult.filter(
            (value) => value.listId !== Item.listId
          );
          setlistresult(ResultAfterDelete);
        }
      } else {
        console.error("error : " + status);
      }
    } catch (error) {
      console.error("er : " + error);
    }
  };
  // const checkboxcheck = (item) => {
  //   console.log(item);
  // };
  const OpenModal = (item, type) => {
    setUtvalgID(item.utvalgId);
    setItem(item);
    setModal(true);
    setType(type);
  };

  const Fetchresult1 = async (param) => {
    try {
      const { data, status } = await api.getdata(
        `Utvalg/SearchUtvalgByUtvalgName?utvalgNavn=${encodeURIComponent(
          param
        )}&customerNos=${custNos}&onlyBasisUtvalg=${0}&extendedInfo=false`
      );

      if (data.length === 0) {
        return [];
      }
      if (status == 200) {
        if (data.length > 3) {
          setscrollbar(true);
        }
        return data;
      }
    } catch (error) {
      console.error("er : " + error);
    }
  };
  const Fetchresult = async (param) => {
    try {
      const { data, status } = await api.getdata(
        `UtvalgList/SearchUtvalgListSimpleByIDAndCustomerNo?utvalglistname=${encodeURIComponent(
          param
        )}&customerNos=${custNos}&forceCustomerAndAgreementCheck=${false}&extendedInfo=${true}&onlyBasisLists=${0}&includeChildrenUtvalg=${false}`
      );
      if (status == 200) {
        if (data.length > 3) {
          setscrollbar(true);
        }
        return data;
      }
      if (status == 204) {
        return [];
      }
    } catch (error) {
      console.error("er : " + error);
    }
  };
  const handleKeypress = (e) => {
    //it triggers by pressing the enter key
    if (e.key === "Enter") {
      SokClick();
    }
  };
  const SokClick = async (event) => {
    let s = sokresult;
    while (s.length > 0) {
      s.splice(0, 1);
    }
    setsokresult(s);
    let t = sokcompletedorders;
    while (t.length > 0) {
      t.splice(0, 1);
    }
    setsokcompletedorders(t);
    let r = listresult;
    while (r.length > 0) {
      r.splice(0, 1);
    }
    setlistresult(r);
    if (Apneetinput === "") {
      setnomessagediv(true);
      setshowresult(false);
    } else {
      setnomessagediv(false);
      setloading(false);

      //search by utvalg or list Id
      //----------------------------

      // utvalgId=2315091&includeReols=true
      //url= url+`SearchUtvalgByUtvalgIdAndCustmerNo?`
      //url= url+`SearchUtvalgListSimpleByIdAndCustNoAgreeNo?`
      let checkutvalg = "";
      let url = "";
      let url1 = "";
      let isDataFound = false;
      let result = [];
      let temp1 = [];
      let listresultvalue = [];

      if (Apneetinput && custNos) {
        setmelding(false);
        let name = Apneetinput;
        name = name.trim();
        checkutvalg = name.substring(0, 1);
        checkutvalg = checkutvalg.toUpperCase();

        let numberPart = 0;
        if (checkutvalg === "U" || checkutvalg === "L") {
          name = name.substring(1);
          try {
            numberPart = JSON.parse(name);
          } catch {}
        } else {
          try {
            numberPart = JSON.parse(Apneetinput);
          } catch {}
        }

        if (numberPart !== 0 && checkutvalg === "U") {
          url = `Utvalg/SearchUtvalgByUtvalgId?utvalgId=${name}&includeReols=${false}&customerNos=${custNos}`;
        } else if (numberPart !== 0 && checkutvalg === "L") {
          url = `UtvalgList/SearchUtvalgListSimpleById?utvalglistid=${name}&customerNos=${custNos}`;
        } else if (
          numberPart !== 0 &&
          checkutvalg !== "U" &&
          checkutvalg !== "L"
        ) {
          url = `Utvalg/SearchUtvalgByUtvalgId?utvalgId=${name}&includeReols=${false}&customerNos=${custNos}`;
          url1 = `UtvalgList/SearchUtvalgListSimpleById?utvalglistid=${name}&customerNos=${custNos}`;
        } else {
          listresultvalue = await Fetchresult(Apneetinput);
          listresultvalue = [...listresultvalue];
          listresultvalue = listresultvalue.map((item) => {
            return { ...item, listId: "L" + item.listId };
          });
          if (listresultvalue.length > 3) {
            setscrollbar(true);
          }
          result = await Fetchresult1(Apneetinput);
          result = result.map((item) => {
            return { ...item, utvalgId: "U" + item.utvalgId };
          });
        }

        if (numberPart !== 0) {
          if (checkutvalg === "U" || checkutvalg === "L") {
            try {
              result = await getResult(url, checkutvalg);
              isDataFound = true;
            } catch (error) {
              console.error("error : " + error);
            }
          } else if (checkutvalg !== "U" && checkutvalg !== "L") {
            try {
              result = await getResult(url, checkutvalg);
              if (result.length > 0) {
                isDataFound = true;
              } else {
                result = await getResult(url1, checkutvalg);
              }
            } catch (error) {
              console.error("error : " + error);
            }
          }
        }
      } else {
        setmelding(true);
        seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
      }

      // temp1 = new Set(result);

      temp1 = [...result, ...listresultvalue];
      temp1 = temp1.filter((i) => {
        if (i.hasMemberList) {
          return i.hasMemberList === false;
        } else {
          return i;
        }
      });

      // temp1 = temp1.sort(function (a, b) {
      //   // if (a.modifications.length !== 0) {
      //   var c = new Date(a.modificationDate.substring(0, 10));

      //   var d = new Date(b.modificationDate.substring(0, 10));

      //   // return d - c;
      // });
      let sokResult = temp1.filter((item) => {
        return item.ordreType === 0 && item.isBasis === false;
        //{
        // return sokresult.push(item);
        // }
      });
      sokResult = sokResult.sort(function (a, b) {
        // if (a.modifications.length !== 0) {
        var c = new Date(a.modificationDate.substring(0, 10));

        var d = new Date(b.modificationDate.substring(0, 10));

        return d - c;
      });
      setsokresult(sokResult);

      let thirdcase = temp1.filter((item) => {
        return item.ordreType === 1 && item.isBasis === false;
      });
      thirdcase = thirdcase.sort(function (a, b) {
        // if (a.modifications.length !== 0) {
        var c = new Date(a.modificationDate.substring(0, 10));

        var d = new Date(b.modificationDate.substring(0, 10));

        return d - c;
      });
      setsokcompletedorders(thirdcase);
      let secondcase = temp1.filter((item) => {
        return item.ordreType === 0 && item.isBasis === true;
        // return listresult.push(item);
      });
      // secondcase = secondcase.sort(function (a, b) {
      //   // if (a.modifications.length !== 0) {
      //   var c = new Date(a.modificationDate.substring(0, 10));

      //   var d = new Date(b.modificationDate.substring(0, 10));

      //   return d - c;
      // });

      setlistresult(secondcase);
      setloading(true);
      setshowresult(true);
      if (temp1.length > 2 || sokcompletedorders.length > 2) {
        setscrollbar(true);
      }
      setbuttondisplay(true);
    }
  };

  const getResult = async (url, checkutvalg) => {
    let result = [];
    try {
      const { data, status } = await api.getdata(url);
      if (data.length > 0) {
        result = data.map((item) => {
          if (checkutvalg === "U") {
            return { ...item, utvalgId: "U" + item.utvalgId };
          } else {
            return { ...item, listId: "L" + item.listId };
          }
        });
      }
      return result;
    } catch (error) {
      console.error("error : " + error);
    }
  };

  const sendjob = () => {};
  const warninput = () => {
    setdisable(false);
  };
  const showplusicon = () => {
    setplusicon(true);
  };
  const showminusicon = () => {
    setplusicon(false);
  };
  const showplusicon_1 = () => {
    setplusicon_1(true);
  };
  const showminusicon_1 = () => {
    setplusicon_1(false);
  };
  const showplusicon_2 = () => {
    setplusicon_2(true);
  };
  const showminusicon_2 = () => {
    setplusicon_2(false);
  };
  const textboxclick = () => {
    setmelding(false);
    setnomessagediv(false);
    let textinput = document.getElementById("Apneetinput").value;
    setApneetinput(textinput);
  };
  const ApneButtonClick = async (utvalgname, ID, index, item) => {
    let newFlag = true;
    if (item?.isRecreated === false) {
      // setIsRecreate(false);
      if (item?.utvalgId) {
        setrecreateId(item?.utvalgId.substring(1));
        setrecreateType("U");
      } else {
        setrecreateId(item?.listId.substring(1));
        setrecreateType("L");
      }
      newFlag = false;
    }
    if (!newFlag) {
      setModal("openRecreatePopup");
    } else {
      setModal("");

      try {
        setCartItems([]);
        setloading(false);
        let _Id;
        try {
          _Id =
            ID.substring(0, 1) === "U" || ID.substring(0, 1) === "L"
              ? ID.substring(1)
              : ID;
        } catch {
          _Id = ID;
        }

        let url = `Utvalg/GetUtvalgReolIDs?utvalgID=${_Id}`;
        const { data, status } = await api.getdata(url);
        if (status === 200) {
          if (data.length > 0) {
            let k = data.map((element) => "'" + element + "'").join(",");
            let reolsWhereClause = `reol_id in (${k})`;
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

            const getAllFeatures = (featureLayer) => {
              const queryOIDs = new Query();
              queryOIDs.where = `${reolsWhereClause}`;

              return featureLayer
                .queryObjectIds(queryOIDs)
                .then(function (oids) {
                  let times = null;
                  let quotient = Math.floor(oids.length / 2000);
                  let remainder = oids.length % 2000;
                  let startIndex = 0;
                  let endIndex = 2000;
                  let promise = [];

                  const queryExtent = new Query();
                  queryExtent.where = `${reolsWhereClause}`;
                  promise[0] = featureLayer.queryExtent(queryExtent);

                  if (quotient < 1) {
                    times = 1;
                    endIndex = oids.length;
                  } else {
                    if (remainder == 0) {
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
                    queryResults.returnGeometry = true;
                    //queryResults.outFields = MapConfig.budruterOutField;
                    queryResults.outFields = "reol_id";
                    queryResults.where =
                      "OBJECTID IN (" + objectsIds.join(",") + ")";
                    queryResults.outSpatialReference = mapView.SpatialReference;

                    promise[i + 1] = featureLayer.queryFeatures(queryResults);
                  }

                  return Promise.all(promise).then((values) => {
                    let results = [];
                    let resultsExtent;
                    for (let i = 0; i < values.length; i++) {
                      if (values[i].count === undefined) {
                        for (let j = 0; j < values[i].features.length; j++) {
                          results.push(values[i].features[j]);
                        }
                      } else {
                        resultsExtent = values[i].extent;
                        mapView.goTo(resultsExtent);
                      }
                    }
                    return results;
                  });
                });
            };

            const featureLayer = new FeatureLayer({
              url: BudruterUrl,
            });

            featureLayer.when(() => {
              getAllFeatures(featureLayer).then((results) => {
                setloading(true);
                if (results.length > 0) {
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

                  results.map((item) => {
                    featuresGeometry.push(item.geometry);

                    let graphic = new Graphic(
                      item.geometry,
                      selectedSymbol,
                      item.attributes
                    );
                    mapView.graphics.add(graphic);
                  });

                  // mapView
                  //   .goTo(featuresGeometry, { animate: false })
                  //   .then(function (response) {
                  //     var zoomView = {};
                  //     zoomView = mapView.extent.expand;
                  //     mapView.goTo(zoomView);
                  //   });

                  setloading(true);
                }
              });
            });
            featureLayer.load();
          }
        }

        let listresultvalue = await Fetchresult(utvalgname);
        listresultvalue = [...listresultvalue];
        let temp = await Fetchresult1(utvalgname);
        let temp1 = [...listresultvalue, ...temp];

        if (temp1.length == 0) {
          setloading(true);
          // window.$("#exampleModal-1").modal("show");
        } else {
          setloading(true);
          if (temp1.length > 0) {
            temp1 = temp1.filter((item) => {
              if (
                item.listId == 0 ||
                item.listId == "" ||
                item.listId == null
              ) {
                return item.utvalgId == _Id;
              } else {
                return item.listId == _Id;
              }
            });
          }

          if (item.utvalgId && item.utvalgId !== "") {
            let _uid;
            try {
              _uid =
                item.utvalgId.substring(0, 1) === "U"
                  ? item.utvalgId.substring(1)
                  : item.utvalgId;
            } catch {
              _uid = item.utvalgId;
            }
            item.utvalgId = _uid;
          } else {
            let _lid;
            try {
              _lid =
                item.listId.substring(0, 1) === "L"
                  ? item.listId.substring(1)
                  : item.listId;
            } catch {
              _lid = item.listId;
            }
            item.listId = _lid;
          }

          setutvalgapiobject(item);

          if (item?.utvalgId) {
            setPage("Apne_Button_Click");
          } else {
            let url = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${item.listId}`;

            try {
              const { data, status } = await api.getdata(url);
              if (data.length == 0) {
              } else {
                let obj = await CreateUtvalglist(data);
                setutvalglistapiobject(obj);
                if (obj.memberUtvalgs?.length > 0) {
                  setPage("cartClick_Component_kw");
                }
              }
            } catch {}
          }
        }

        // //basics
        // if (index === 2) {
        //   const { data, status } = await api.getdata(
        //     `Utvalg/SearchUtvalgSimple?utvalgNavn=${utvalgname}&searchMethod=1`
        //   );
        //   if (status == 200) {
        //     let filteredData = data.filter(
        //       (item) =>
        //         (item.utvalgId || item.listId) == ID &&
        //         (item.utvalgName || item.name) == utvalgname
        //     );
        //     filteredData.forEach(
        //       (item, i) => (item.utvalgId = "L" + item.utvalgId)
        //     );
        //     setutvalgapiobject(filteredData);
        //     // setPage("Apne_Button_Click");
        //     setloading(true);
        //   }
        // }
        // //active or order
        // if (index === 1 || index === 3) {
        //   const { data, status } = await api.getdata(
        //     `Utvalg/SearchUtvalg?utvalgNavn=${utvalgname}&searchMethod=1`
        //   );
        //   if (status == 200) {
        //     let filteredData = data.filter(
        //       (item) =>
        //         (item.utvalgId || item.listId) == ID &&
        //         (item.utvalgName || item.name) == utvalgname
        //     );
        //     filteredData.forEach(
        //       (item, i) => (item.utvalgId = "U" + item.utvalgId)
        //     );
        //     setutvalgapiobject(filteredData);
        //     if (index === 1) {
        //       // setPage("Apne_Button_Click");
        //       setloading(true);
        //     }
        //     if (index === 3) {
        //       // setPage("Apne_Button_Completedorders");
        //       setloading(true);
        //     }
        //}
        // }
      } catch (error) {
        console.error("er : " + error);
      }
    }
  };
  const cartClick = () => {
    setPage("cartClick_Component_kw");
  };

  return (
    <div className="col-5 p-2">
      {Modal === "openRecreatePopup" ? (
        <ShowRecreateKW
          id={"uxBtnOpen"}
          recreateId={recreateId}
          recreateType={recreateType}
        />
      ) : null}
      {newhome ? (
        <div className="padding_NoColor_B" style={{ cursor: "pointer" }}>
          <a id="uxHandlekurvSmall_uxLnkbtnHandlekurv" onClick={cartClick}>
            <div className="handlekurv handlekurvText pl-2">
              Du har 1 utvalg i bestillingen din.
            </div>
          </a>
        </div>
      ) : null}
      {newhome ? <br /> : null}
      <div className="divPanelBody_kw height-95">
        <div className="padding_Color_L_T_B pr-4 height-100">
          <div className="bold">
            Søk etter utvalg eller bestillinger med navn eller ID:
          </div>
          <div className="padding_NoColor_T">
            <table className="t-width">
              <tbody>
                <tr>
                  <td className="p-0">
                    <div className="divValueText">
                      <input
                        type="text"
                        id="Apneetinput"
                        value={Apneetinput}
                        onChange={textboxclick}
                        className="Apneet-inputbox"
                        onKeyPress={handleKeypress}
                      />
                    </div>
                  </td>
                  <td className="Apneet-table2">
                    <table border="0" cellPadding="0" cellSpacing="0">
                      <tbody>
                        <tr>
                          <td className="green1">
                            <input
                              type="button"
                              value="Søk"
                              onClick={SokClick}
                              className=" KSPU_button_Green sok-text"
                            />
                          </td>
                        </tr>
                      </tbody>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td colSpan="2" className="">
                    <div className="">
                      <input
                        id="uxSokEtterUtvalg_uxSortByDate_uxRadioButton"
                        type="radio"
                        className="mt-1"
                        value="uxRadioButton"
                        onChange={datesorting}
                        checked={sortdatecheck}
                      />
                      &nbsp;
                      <label className="sok-text pb-2">Sortert på dato</label>
                      &nbsp;
                      <input
                        id="uxSokEtterUtvalg_uxSortByName_uxRadioButton"
                        type="radio"
                        className="mt-1"
                        checked={sortnamecheck}
                        onChange={namesorting}
                        value="uxRadioButton"
                      />
                      <label className="sok-text">Sortert på navn</label>
                    </div>
                  </td>
                </tr>
                <tr>
                  {melding ? (
                    <span className=" sok-Alert-text pl-1">{errormsg}</span>
                  ) : null}
                </tr>
              </tbody>
            </table>
          </div>
          <div>{!loading ? <Spinner /> : null}</div>

          {nomessagediv ? (
            <div className="padding_Color_L_T_B pr-3">
              <div className="error WarningSign">
                <div className="divErrorHeading">Melding:</div>
                <p id="uxKjoreAnalyse_uxLblMessage" className="divErrorText_kw">
                  Søket ga ikke noe resultat. Prøv på nytt. Hvis det fortsatt
                  feiler, kontakt superbruker i Posten.
                </p>
              </div>
            </div>
          ) : null}

          <div
            className="modal fade bd-example-modal-lg"
            id="AdverselModal"
            tabIndex="-1"
            role="dialog"
            aria-labelledby="exampleModalCenterTitle"
            aria-hidden="true"
          >
            <div className="modal-dialog" role="document">
              <div className="modal-content">
                <div className="Common-modal-header">
                  <div className="divDockedPanel">
                    <div className=" divDockedPanelTop">
                      <span className="dialog-kw" id="exampleModalLabel">
                        Advarsel
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
                                &nbsp; Skal utvalget slettes?
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
                                  Nei
                                </button>
                                {/* </td>
        <td></td>
        <td> */}{" "}
                                &nbsp;&nbsp;
                                <button
                                  type="button"
                                  onClick={Jaclick}
                                  className="modalMessage_button"
                                  data-dismiss="modal"
                                >
                                  Ja
                                </button>
                              </div>
                            </td>
                          </tr>
                          <tr>
                            <td>&nbsp;</td>
                          </tr>
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div
            className="modal fade"
            id="exampleModal-1"
            tabIndex="-1"
            role="dialog"
            aria-labelledby="exampleModalCenterTitle"
            aria-hidden="true"
          >
            <div className="modal-dialog" role="document">
              <div className="modal-content">
                <div className="">
                  {/* <div className=""> */}
                  <div className=" divDockedPanelTop">
                    <span className="dialog-kw" id="exampleModalLabel">
                      Advarsel
                    </span>
                  </div>
                  <div className="View_modal-body-appneet pl-2">
                    <table>
                      <tbody>
                        <tr>
                          <td valign="top">
                            <p
                              className="sok-text"
                              id="uxSokEtterUtvalg_uxUtvalgList_uxOnTheFlyConfirm_uxWarning_uxTxtMessage"
                            >
                              Datagrunnlaget må oppdateres i disse
                              utvalgene/utvalgslistene. Det vil bli bestilt en
                              gjenskapningsjobb som du får respons på via
                              e-post. Sende jobb?
                            </p>
                          </td>
                        </tr>
                        <tr>
                          <td colSpan="3" valign="top">
                            <p className="sok-text">
                              Skriv inn e-postadresse du skal varsles på når
                              <br /> jobben er klar:
                            </p>
                          </td>
                        </tr>
                        <tr>
                          <td colSpan="3" valign="top">
                            <input
                              type="text"
                              maxLength="255"
                              onChange={warninput}
                              className="inputwidth"
                            />
                          </td>
                        </tr>

                        <tr>
                          <td>
                            <div className="pt-3">
                              <button
                                type="button"
                                className="modalMessage_button float-left"
                                data-dismiss="modal"
                              >
                                Nei
                              </button>
                              {/* </td>
        <td></td>
        <td> */}{" "}
                              &nbsp;&nbsp;
                              <button
                                type="button"
                                onClick={sendjob}
                                disabled={disable}
                                className="modalMessage_button float-right"
                              >
                                send job
                              </button>
                            </div>
                          </td>
                        </tr>
                        <tr>
                          <td>&nbsp;</td>
                        </tr>
                      </tbody>
                    </table>
                  </div>
                </div>
              </div>
            </div>
            {/* </div> */}
          </div>

          <div>
            <div className={scrollbar ? "scrollbardiv" : ""}>
              {showresult ? (
                <div>
                  {" "}
                  <div className="bold  green row  ">
                    <div className="pl-2 ">
                      {plusicon ? (
                        <img
                          id="dlAUimg"
                          className="cursor"
                          alt="Skjul liste"
                          src={plus}
                          onClick={showminusicon}
                        />
                      ) : (
                        <img
                          className="cursor"
                          id="dlAUimg"
                          alt="Skjul liste"
                          src={minus}
                          onClick={showplusicon}
                        />
                      )}
                    </div>
                    &nbsp;Bestillinger under arbeid
                  </div>
                  {!plusicon ? (
                    <div>
                      {sokresult.map((item, index) => {
                        return (
                          <div className="padding_NoColor_T" key={index}>
                            <div className="clearFloat UpperLine padding_NoColor_T_B">
                              <div>
                                <div className="div_left">
                                  <div className="bold namefont wordbreak">
                                    {item.name}
                                  </div>
                                  <div className="namefont">
                                    Sist lagret{" "}
                                    {item.modifications.length &&
                                    item.modifications[0].modificationTime &&
                                    item.modifications[0].modificationTime !==
                                      ""
                                      ? item.modifications[0].modificationTime.substring(
                                          0,
                                          10
                                        )
                                      : item?.modificationDate &&
                                        item?.modificationDate !== ""
                                      ? item.modificationDate.substring(0, 10)
                                      : null}
                                    . Id:{" "}
                                    {typeof item.antall === "undefined"
                                      ? item.utvalgId
                                      : item.listId}
                                    <br />{" "}
                                    {typeof item.antall === "undefined"
                                      ? item.totalAntall
                                      : item.antall}
                                    &nbsp; mottakere.
                                  </div>
                                </div>
                                {newhome ? <br /> : null}
                                {newhome ? (
                                  <div>
                                    <div className="row float-right pr-3">
                                      <span className="namefont">Legg til</span>
                                      &nbsp;&nbsp;
                                      <input type="checkbox" />
                                    </div>
                                  </div>
                                ) : (
                                  <div className="div_right submit">
                                    <input
                                      type="submit"
                                      name="uxSokEtterUtvalg$uxUtvalgList$uxDLUnderArbeid$ctl00$uxBtnOpen"
                                      value="Åpne"
                                      // data-toggle="modal" data-target="#exampleModal-1"
                                      id="uxSokEtterUtvalg_uxUtvalgList_uxDLUnderArbeid_ctl00_uxBtnOpen"
                                      className="KSPU_button-kw"
                                      data-toggle="modal"
                                      data-target="#uxBtnOpen"
                                      onClick={() =>
                                        ApneButtonClick(
                                          item.name,
                                          typeof item.antall == "undefined"
                                            ? item.utvalgId
                                            : item.listId,
                                          1,
                                          item
                                        )
                                      }
                                    />

                                    <br />
                                    <a
                                      className="KSPU_LinkButton_Url_KW pl-2"
                                      data-toggle="modal"
                                      data-target="#AdverselModal"
                                      onClick={() =>
                                        OpenModal(item, "progress")
                                      }
                                    >
                                      Slett
                                    </a>
                                  </div>
                                )}
                              </div>
                            </div>
                          </div>
                        );
                      })}
                    </div>
                  ) : null}
                  {!plusicon && sokresult.length > 0 ? (
                    <div>
                      <br />
                      <br />
                    </div>
                  ) : null}
                  <div className="bold  green row  ">
                    <div className="pl-2 ">
                      {plusicon_1 ? (
                        <img
                          id="dlAUimg"
                          className="cursor"
                          alt="Skjul liste"
                          src={plus}
                          onClick={showminusicon_1}
                        />
                      ) : (
                        <img
                          className="cursor"
                          id="dlAUimg"
                          alt="Skjul liste"
                          src={minus}
                          onClick={showplusicon_1}
                        />
                      )}
                    </div>
                    &nbsp;Basislister / malutvalg
                  </div>
                  {!plusicon_1 ? (
                    <div className="">
                      {listresult.map((item, index) => {
                        return (
                          <div className="padding_NoColor_T" key={index}>
                            <div className="clearFloat UpperLine padding_NoColor_T_B">
                              <div>
                                <div className="div_left">
                                  <div className="bold namefont wordbreak">
                                    {item.name.trim()}
                                  </div>

                                  <div className="namefont">
                                    Sist lagret{" "}
                                    {item.modifications.length &&
                                    item.modifications[0].modificationTime &&
                                    item.modifications[0].modificationTime !==
                                      ""
                                      ? item.modifications[0].modificationTime.substring(
                                          0,
                                          10
                                        )
                                      : item?.modificationDate &&
                                        item?.modificationDate !== ""
                                      ? item.modificationDate.substring(0, 10)
                                      : null}
                                    Id:{" "}
                                    {typeof item.antall == "undefined"
                                      ? item.utvalgId
                                      : item.listId}
                                    &nbsp;&nbsp;
                                    {typeof item.totalAntall == "undefined"
                                      ? item.antall
                                      : item.totalAntall}
                                    &nbsp; mottakere.
                                  </div>
                                </div>
                                {newhome ? <br /> : null}
                                {newhome ? (
                                  <div>
                                    <div className="row float-right pr-3">
                                      <span className="namefont">Legg til</span>
                                      &nbsp;&nbsp;
                                      <input type="checkbox" />
                                    </div>
                                  </div>
                                ) : (
                                  <div className="div_right submit">
                                    <input
                                      type="submit"
                                      name="uxSokEtterUtvalg$uxUtvalgList$uxDLUnderArbeid$ctl00$uxBtnOpen"
                                      value="Åpne"
                                      id="uxSokEtterUtvalg_uxUtvalgList_uxDLUnderArbeid_ctl00_uxBtnOpen"
                                      className="KSPU_button-kw"
                                      data-toggle="modal"
                                      data-target="#uxBtnOpen"
                                      onClick={() =>
                                        ApneButtonClick(
                                          item.name,
                                          typeof item.totalAntall == "undefined"
                                            ? item.listId
                                            : item.utvalgId,
                                          2,
                                          item
                                        )
                                      }
                                    />

                                    <br />
                                    <a
                                      className="KSPU_LinkButton_Url_KW pl-2"
                                      data-toggle="modal"
                                      data-target="#AdverselModal"
                                      onClick={() => OpenModal(item, "lister")}
                                    >
                                      Slett
                                    </a>
                                  </div>
                                )}
                              </div>
                            </div>
                          </div>
                        );
                      })}
                    </div>
                  ) : null}
                  {!plusicon_1 && listresult.length > 0 ? (
                    <div>
                      <br />
                      <br />
                    </div>
                  ) : null}
                  <div className="bold  green row  ">
                    <div className="pl-2 ">
                      {plusicon_2 ? (
                        <img
                          id="dlAUimg"
                          className="cursor"
                          alt="Skjul liste"
                          src={plus}
                          onClick={showminusicon_2}
                        />
                      ) : (
                        <img
                          className="cursor"
                          id="dlAUimg"
                          alt="Skjul liste"
                          src={minus}
                          onClick={showplusicon_2}
                        />
                      )}
                    </div>
                    &nbsp; Fullførte bestillinger
                  </div>
                  {!plusicon_2 ? (
                    <div>
                      {sokcompletedorders.map((item, index) => {
                        return (
                          <div className="padding_NoColor_T" key={index}>
                            <div className="clearFloat UpperLine padding_NoColor_T_B">
                              <div>
                                <div className="div_left">
                                  <div className="bold namefont wordbreak">
                                    {item.name.trim()}
                                  </div>

                                  <div className="namefont">
                                    Sist lagret{" "}
                                    {item.modifications.length &&
                                    item.modifications[0].modificationTime &&
                                    item.modifications[0].modificationTime !==
                                      ""
                                      ? item.modifications[0].modificationTime.substring(
                                          0,
                                          10
                                        )
                                      : item?.modificationDate &&
                                        item?.modificationDate !== ""
                                      ? item.modificationDate.substring(0, 10)
                                      : null}
                                    . Id:{" "}
                                    {typeof item.antall == "undefined"
                                      ? item.utvalgId
                                      : item.listId}
                                    {/* Sist lagret {item.innleveringsDato.substring(0,10)}. Id: {item.listId} */}
                                    <br />{" "}
                                    {typeof item.antall == "undefined"
                                      ? item.totalAntall
                                      : item.antall}
                                    &nbsp; mottakere.
                                  </div>
                                </div>
                                {newhome ? <br /> : null}
                                {newhome ? (
                                  <div>
                                    <div className="row float-right pr-3">
                                      <span className="namefont">Kopier</span>
                                      &nbsp;&nbsp;
                                      <input
                                        type="checkbox"
                                        // onClick={checkboxcheck(item)}
                                      />
                                    </div>
                                  </div>
                                ) : (
                                  <div className="div_right submit">
                                    <input
                                      type="submit"
                                      name="uxSokEtterUtvalg$uxUtvalgList$uxDLUnderArbeid$ctl00$uxBtnOpen"
                                      value="Åpne"
                                      id="uxSokEtterUtvalg_uxUtvalgList_uxDLUnderArbeid_ctl00_uxBtnOpen"
                                      className="KSPU_button-kw"
                                      data-toggle="modal"
                                      data-target="#uxBtnOpen"
                                      onClick={() =>
                                        ApneButtonClick(
                                          item.name,
                                          typeof item.antall == "undefined"
                                            ? item.utvalgId
                                            : item.listId,
                                          3,
                                          item
                                        )
                                      }
                                    />

                                    <br />
                                  </div>
                                )}
                              </div>
                            </div>
                          </div>
                        );
                      })}
                    </div>
                  ) : null}
                </div>
              ) : null}
              {}
            </div>
          </div>
          {newhome ? <br /> : null}
          {newhome && buttondisplay ? (
            <div className="padding_NoColor_T pb-4">
              <input
                type="submit"
                className="KSPU_button_Gray float-right pr-3"
                onClick={LeggClick}
                value="Legg til valgte"
              />
            </div>
          ) : null}
        </div>
      </div>
      <div className="">
        <a className="KSPU_LinkButton_Url_KW pl-2" onClick={GotoMain}>
          Avbryt
        </a>
      </div>
    </div>
  );
}

export default ApneetLinkClick;
