import React, { useEffect, useState, useContext } from "react";
import { KundeWebContext } from "../../context/Context";
import minus from "../../assets/images/minus2.gif";
import plus from "../../assets/images/plus2.gif";
import api from "../../services/api.js";
import swal from "sweetalert";
import "../apne_Button_Click-kw/Apne_Button_Click-kw.scss";
import $ from "jquery";
import loadingImage from "../../assets/images/callbackActivityIndicator.gif";
import { MainPageContext } from "../../context/Context.js";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";

function Sok_Component_kw() {
  const { LeggTilCheckedItems, setLeggTilCheckedItems } =
    useContext(KundeWebContext);
  const [plusicon_member, setplusicon_member] = useState(false);
  const [plusicon_member_1, setplusicon_member_1] = useState(false);
  const [plusicon_member_2, setplusicon_member_2] = useState(false);
  const [minusicon_member_1, setminusicon_member_1] = useState(false);
  const [minusicon_member, setminusicon_member] = useState(false);
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
  const [buttondisplay, setbuttondisplay] = useState(false);
  const { KopierModal, setKopierModal } = useContext(KundeWebContext);
  const { CartItems, setCartItems } = useContext(KundeWebContext);
  const [collapseArr, setCollapseArr] = useState([]);
  const [kopierArr, setKopierArr] = useState([]);
  const [leggTilArr, setLeggTilArr] = useState([]);
  const { cartClickModalHide, setCartClickModalHide } =
    useContext(KundeWebContext);
  const { KopierCheckedItems, setKopierCheckedItems } =
    useContext(KundeWebContext);
  const { leggtiltrue, setleggtiltrue } = useContext(KundeWebContext);
  const [activityLoading, setActivityLoading] = useState(false);
  const { utvalglistapiobject, setutvalglistapiobject } =
    useContext(KundeWebContext);
  const { custNos } = useContext(KundeWebContext);
  const [melding, setmelding] = useState(false);
  const [errormsg, seterrormsg] = useState("");

  const GotoMain = () => {
    setPage("");
  };
  useEffect(() => {
    setLeggTilCheckedItems([]);
    setKopierCheckedItems([]);
    // setCartItems([]);
  }, []);
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
      return c - d;
    });
    setsokcompletedorders(Date_Sort);
    let TempListresult = listresult;
    let Date_Sortlist = TempListresult.sort(function (a, b) {
      var c = new Date(a.modificationDate.substring(0, 10));
      var d = new Date(b.modificationDate.substring(0, 10));
      return c - d;
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
      return c - d;
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
    setActivityLoading(true);
    setCartClickModalHide(false);
    if (kopierArr?.length) {
      setKopierModal(true);
    }
    setPage("cartClick_Component_kw");
    setActivityLoading(false);
  };
  const Jaclick = async () => {
    try {
      const { data, status } = await api.deletedata(
        "Utvalg/DeleteUtvalg?utvalgId=" + UtvalgID
      );
      if (status === 200) {
        if (Type === "progress") {
          let ResultAfterDelete = sokresult.filter(
            (value) => value.utvalgId !== Item.utvalgId
          );
          setsokresult(ResultAfterDelete);
        }
        if (Type === "lister") {
          let ResultAfterDelete = listresult.filter(
            (value) => value.utvalgId !== Item.utvalgId
          );
          setlistresult(ResultAfterDelete);
        }

        //     $('.modal').remove();
        //     $('.modal-backdrop').remove();
        //    swal("UtvalgId is deleted Successfully")
      } else {
        console.error("error : " + status);
      }
    } catch (error) {
      // swal("oops! Something Went Wrong!");

      console.error("er : " + error);
    }
  };
  const OpenModal = (item, type) => {
    setUtvalgID(item.utvalgId);
    setItem(item);
    setModal(true);
    setType(type);
  };

  const Fetchresult1 = async (param) => {
    try {
      const { data, status } = await api.getdata(
        `Utvalg/SearchUtvalgByUtvalgName?utvalgNavn=${param}&customerNos=${custNos}&onlyBasisUtvalg=0&extendedInfo=true`
        // `Utvalg/SearchUtvalgByUtvalgName?forceCustomerAndAgreementCheck
        // =${false}&extendedInfo=${false}&onlyBasisUtvalg=${1}&utvalgNavn=${param}`
      );
      if (data.length === 0) {
        return [];
      }
      if (status == 200) {
        if (data.length > 3) {
          setscrollbar(true);
        }
        const newArr1 = data.map((v) => ({ ...v, memberUtvalgs: [] }));

        return newArr1;
      }
    } catch (error) {
      console.error("er : " + error);
    }
  };
  const Fetchresult = async (param) => {
    try {
      // const { data, status } = await api.getdata(
      //   `Utvalg/SearchUtvalg?utvalgNavn=${param}&searchMethod=1`
      // );
      // const { data, status } = await api.getdata(
      //   `UtvalgList/SearchUtvalgListSimpleByIDAndCustomerNo?utvalglistname=${param}&customerNos=${custNos}&agreementNos=${avtaleData}&forceCustomerAndAgreementCheck=${false}&extendedInfo=${false}&onlyBasisLists=${1}&includeChildrenUtvalg=${false}`
      // );
      const { data, status } = await api.getdata(
        `UtvalgList/SearchUtvalgListSimpleByIDAndCustomerNo?utvalglistname=${param}&customerNos=${custNos}&forceCustomerAndAgreementCheck=${false}&extendedInfo=${true}&onlyBasisLists=${0}&includeChildrenUtvalg=${true}`
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

  const SokClick = async () => {
    setbuttondisplay(false);
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

      let checkutvalg = "";
      let temp = [];
      let listresultvalue = [];
      let url = "";
      let url1 = "";

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
          temp = await Fetchresult1(Apneetinput);
          temp = temp.map((item) => {
            return { ...item, utvalgId: "U" + item.utvalgId };
          });
        }

        if (numberPart !== 0) {
          if (checkutvalg === "U") {
            try {
              temp = await getResult(url, checkutvalg, false);
              console.log(temp, "temp");
            } catch (error) {
              console.error("error : " + error);
            }
          } else if (checkutvalg === "L") {
            try {
              temp = await getResult(url, checkutvalg, true);
              console.log(temp, "temp");
            } catch (error) {
              console.error("error : " + error);
            }
          } else if (checkutvalg !== "U" && checkutvalg !== "L") {
            try {
              temp = await getResult(url, checkutvalg, false);
              if (temp.length > 0) {
              } else {
                temp = await getResult(url1, checkutvalg, true);
              }
            } catch (error) {
              console.error("error : " + error);
            }
          }
        }

        let temp1 = [...listresultvalue, ...temp];
        temp1 = temp1.filter((i) => {
          if (i.hasMemberList) {
            return i.hasMemberList === false;
          } else {
            return i;
          }
        });

        // temp1 = new Set(temp1);
        // temp1 = [...temp1];

        temp1 = temp1.sort(function (a, b) {
          var c = new Date(a.modificationDate.substring(0, 10));
          var d = new Date(b.modificationDate.substring(0, 10));

          return c - d;
        });
        let sokResult = temp1.filter((item) => {
          return item.ordreType === 0 && item.isBasis === false;
        });
        setsokresult(sokResult);

        let thirdcase = temp1.filter((item) => {
          return item.ordreType === 1 && item.isBasis === false;
        });
        setsokcompletedorders(thirdcase);
        let secondcase = temp1.filter((item) => {
          return item.ordreType === 0 && item.isBasis === true;
          // return listresult.push(item);
        });

        setlistresult(secondcase);
        setloading(true);
        setshowresult(true);
        if (temp1.length > 2 || sokcompletedorders.length > 2) {
          setscrollbar(true);
        }
      } else {
        setloading(true);
        setmelding(true);
        seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
      }
    }
  };

  const getResult = async (url, checkutvalg, isList) => {
    let result = [];
    if (isList) {
      let listID = "";
      try {
        const { data, status } = await api.getdata(url);
        if (data?.length > 0) {
          listID = data[0]?.listId;
          let listUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${listID}`;
          try {
            const { data, status } = await api.getdata(listUrl);
            if (Object.keys(data).length > 0) {
              result.push({ ...data, listId: "L" + data.listId });
            }
            return result;
          } catch (error) {
            console.error("error : " + error);
          }
        }
        return result;
      } catch (error) {
        console.error("error : " + error);
      }
    } else {
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
    }
  };
  const LeggtilClick = (item) => {};
  const sendjob = () => {};
  const warninput = () => {
    setdisable(false);
  };
  const showmminusicon_member = () => {
    setplusicon_member(true);
  };
  const showplusicon_member = () => {
    setplusicon_member(false);
  };
  const showmminusicon_member_1 = () => {
    setplusicon_member_1(true);
  };
  const showplusicon_member_1 = () => {
    setplusicon_member_1(false);
  };
  const showmminusicon_member_2 = () => {
    setplusicon_member_2(true);
  };
  const showplusicon_member_2 = () => {
    setplusicon_member_2(false);
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
    setnomessagediv(false);
    let textinput = document.getElementById("Apneetinput").value;
    setApneetinput(textinput);
  };
  const ApneButtonClick = async (utvalgname, ID, index) => {
    try {
      setloading(false);
      let url = `Utvalg/GetUtvalgReolIDs?utvalgID=${ID.substring(1)}`;
      const { data, status } = await api.getdata(url);
      if (status === 200) {
        if (data.length > 0) {
          let k = data.map((element) => "'" + element + "'").join(",");
          let sql_geography = `reol_id in (${k})`;
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
                    color: [237, 54, 21, 0.25],
                    style: "solid",
                    outline: {
                      // autocasts as new SimpleLineSymbol()
                      color: [237, 54, 21],
                      width: 0.75,
                    },
                  };

                  let j = mapView.graphics.items.length;
                  for (var i = j; i > 0; i--) {
                    if (
                      mapView.graphics.items[i - 1].geometry.type === "polygon"
                    ) {
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
      }

      let listresultvalue = await Fetchresult(utvalgname);
      listresultvalue = [...listresultvalue];

      let temp = await Fetchresult1(utvalgname);

      let temp1 = [...listresultvalue, ...temp];

      // let filteredData = temp1.filter((item) => {
      //   return (
      //     (item.utvalgId in temp1 || item.listId in temp1) == ID.substring(1) &&
      //     (item.utvalgName || item.name) == utvalgname
      //   );
      // });

      // temp1.length = 0;
      if (temp1.length == 0) {
        setloading(true);
        window.$("#exampleModal-1").modal("show");
      } else {
        setutvalgapiobject(temp1[0]);
        //need to check
        setPage("Apne_Button_Click");
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
  };

  const LeggTilCheckBoxClick = (param, e) => {
    let TempStorageOfCheckedItems = LeggTilCheckedItems;
    if (e.target.checked) {
      if (param.memberUtvalgs?.length > 0) {
        param.memberUtvalgs.map((item) => {
          TempStorageOfCheckedItems.push(item);
          leggTilArr.push(item);
        });
      } else {
        TempStorageOfCheckedItems.push(param);
        leggTilArr.push(param);
      }
      setbuttondisplay(true);
    } else {
      if (param.memberUtvalgs?.length > 0) {
        let newArr = [];
        for (let i = 0; i < TempStorageOfCheckedItems?.length; i++) {
          for (let j = 0; j < param.memberUtvalgs?.length; j++) {
            var f = 0;
            if (
              TempStorageOfCheckedItems[i].name === param.memberUtvalgs[j].name
            ) {
              f = 1;
              break;
            }
          }
          if (f === 0) {
            newArr.push(TempStorageOfCheckedItems[i]);
          }
        }
        param.memberUtvalgs.map((item) => {
          TempStorageOfCheckedItems.push(item);
          leggTilArr.push(item);
        });
        TempStorageOfCheckedItems = newArr;
        setLeggTilArr([...newArr]);
      } else {
        let newArr = TempStorageOfCheckedItems.filter((x) => x != param);
        TempStorageOfCheckedItems = newArr;
        setLeggTilArr([...newArr]);
      }
    }
    if (typeof param.antall === "undefined" || param.antall === "") {
      setKopierModal(false);
    } else {
      setKopierModal(true);
    }

    setLeggTilCheckedItems(TempStorageOfCheckedItems.flat(1));
  };

  const LeggTillCheck = (param, e) => {
    let TempStorageOfCheckedItems = LeggTilCheckedItems;
    if (e.target.checked) {
      TempStorageOfCheckedItems.push(param);
      leggTilArr.push(param);
      setbuttondisplay(true);
    } else {
      let newArr = TempStorageOfCheckedItems.filter((x) => x != param);
      TempStorageOfCheckedItems = newArr;
      setLeggTilArr([...newArr]);
    }
    if (TempStorageOfCheckedItems.flat(1).length > 0) {
      setleggtiltrue(true);
    }
    setLeggTilCheckedItems(TempStorageOfCheckedItems.flat(1));
  };

  const KopierCheckBoxClick = (param, e) => {
    let TempStorageOfCheckedItems = KopierCheckedItems;
    if (e.target.checked) {
      if (param.memberUtvalgs?.length > 0) {
        param.memberUtvalgs.map((item) => {
          TempStorageOfCheckedItems.push(item);
          kopierArr.push(item);
        });
      } else {
        TempStorageOfCheckedItems.push(param);
        kopierArr.push(param);
      }
      setbuttondisplay(true);
    } else {
      if (param.memberUtvalgs?.length > 0) {
        let newArr = [];
        for (let i = 0; i < TempStorageOfCheckedItems?.length; i++) {
          for (let j = 0; j < param.memberUtvalgs?.length; j++) {
            var f = 0;
            if (
              TempStorageOfCheckedItems[i].name === param.memberUtvalgs[j].name
            ) {
              f = 1;
              break;
            }
          }
          if (f === 0) {
            newArr.push(TempStorageOfCheckedItems[i]);
          }
        }
        param.memberUtvalgs.map((item) => {
          TempStorageOfCheckedItems.push(item);
          kopierArr.push(item);
        });
        TempStorageOfCheckedItems = newArr;
        setKopierArr([...newArr]);
      } else {
        let newArr = TempStorageOfCheckedItems.filter((x) => x != param);
        TempStorageOfCheckedItems = newArr;
        setKopierArr([...newArr]);
      }
    }
    if (typeof param.antall === "undefined" || param.antall === "") {
      setKopierModal(false);
    } else {
      setKopierModal(true);
    }

    setKopierCheckedItems(TempStorageOfCheckedItems.flat(1));
  };

  // const LeggtilCheckBoxClick = (param, e) => {
  //   let TempStorageOfCheckedItems = LeggTilCheckedItems1;
  //   if (e.target.checked) {
  //     if (param.memberUtvalgs.length > 0) {
  //       let Tem_value = param.memberUtvalgs.map((item) => {
  //         TempStorageOfCheckedItems.push(item);
  //       });
  //     } else {
  //       TempStorageOfCheckedItems.push(param);
  //     }
  //     setbuttondisplay(true);
  //   } else {
  //     let newArr = TempStorageOfCheckedItems.filter((x) => x != param);
  //     TempStorageOfCheckedItems = newArr;
  //   }
  //   if (typeof param.antall === "undefined" || param.antall === "") {
  //     setKopierModal(false);
  //   } else {
  //     setKopierModal(true);
  //   }
  //   let flag = 0;
  //   TempStorageOfCheckedItems.map((item)=>{
  //     if(item.antall === undefined){
  //       flag=1;
  //     }
  //   })
  //   if(flag===1){
  //     set
  //   }

  //   setLeggTilCheckedItems1(TempStorageOfCheckedItems.flat(1));
  // };

  const handleExpandCollapse = (e) => {
    if (document.getElementById(e.target.id).src === minus) {
      document.getElementById(e.target.id).src = plus;
      setCollapseArr(
        collapseArr.filter((item) => {
          return item !== e.target.id;
        })
      );
    } else {
      document.getElementById(e.target.id).src = minus;
      setCollapseArr([...collapseArr, e.target.id]);
    }
  };

  const renderItem = (item) => {
    return (
      <>
        {item !== null ? (
          <div className="div">
            {item?.memberUtvalgs?.map((item) => (
              <div className="">
                <div className="bold namefont">{item.name}</div>
                <div className="row float-right pr-3">
                  <span className="namefont"> Kopier </span>
                  &nbsp;&nbsp;
                  <input
                    type="checkbox"
                    onChange={(e) => KopierCheckBoxClick(item, e)}
                  />
                </div>

                <div className="namefont">
                  Sist lagret{" "}
                  {item.modifications.length !== 0
                    ? item.modifications[
                        item.modifications.length - 1
                      ].modificationTime.substring(0, 10)
                    : null}
                  . Id: {"U" + item.utvalgId}
                  <br /> {item.totalAntall}
                  &nbsp; mottakere.
                </div>
              </div>
            ))}
          </div>
        ) : null}
      </>
    );
  };

  return (
    <div className="col-5 p-2">
      {/* <button onClick={test}>test</button> */}
      {newhome ? (
        <div
          className="padding_NoColor_B"
          style={{ cursor: "pointer" }}
          onClick={() => {
            setPage("cartClick_Component_kw");
          }}
        >
          <a id="uxHandlekurvSmall_uxLnkbtnHandlekurv" onClick={""}>
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
      <div className="divPanelBody_kw">
        <div className="padding_Color_L_T_B pr-4">
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
                      <label className="sok-text pb-0">Sortert på dato</label>
                      &nbsp;
                      <input
                        id="uxSokEtterUtvalg_uxSortByName_uxRadioButton"
                        type="radio"
                        className="mt-1"
                        checked={sortnamecheck}
                        onChange={namesorting}
                        value="uxRadioButton"
                      />
                      <label className="sok-text pb-0">Sortert på navn</label>
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
          <img
            src={loadingImage}
            style={{
              width: "20px",
              height: "20px",
              display: loading ? "none" : "block",
              position: "absolute",
              top: "170px",
              left: "250px",
              zindex: 100,
            }}
          />
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
                      {sokresult.map((item) => {
                        return (
                          <div className="padding_NoColor_T">
                            <div className="clearFloat UpperLine padding_NoColor_T_B">
                              <div>
                                <div className="div">
                                  <div className="bold namefont">
                                    {item.name}
                                  </div>
                                  <div className="row float-right pr-3">
                                    {typeof item.antall === "undefined" ||
                                    item.antall === "" ? (
                                      <span className="namefont">
                                        {" "}
                                        Legg til{" "}
                                      </span>
                                    ) : (
                                      <span className="namefont"> Kopier </span>
                                    )}
                                    &nbsp;&nbsp;
                                    <input
                                      type="checkbox"
                                      onChange={(e) =>
                                        typeof item.antall === "undefined" ||
                                        item.antall === ""
                                          ? LeggTillCheck(item, e)
                                          : KopierCheckBoxClick(item, e)
                                      }
                                      // onChange={(e) => {
                                      //   KopierCheckBoxClick(item, e);
                                      // }}
                                    />
                                  </div>

                                  <div className="namefont">
                                    Sist lagret{" "}
                                    {item.modifications.length &&
                                    item.modifications[0].modificationTime &&
                                    item.modifications[0].modificationTime !==
                                      ""
                                      ? item.modifications[0]?.modificationTime?.substring(
                                          0,
                                          10
                                        )
                                      : item?.modificationDate &&
                                        item?.modificationDate !== ""
                                      ? item?.modificationDate?.substring(0, 10)
                                      : null}
                                    . Id:{" "}
                                    {typeof item.antall === "undefined" ||
                                    item.antall === ""
                                      ? item?.utvalgId
                                      : item?.listId}
                                    <br />
                                    {typeof item.antall === "undefined"
                                      ? item?.totalAntall
                                      : item?.antall}
                                    &nbsp; mottakere.
                                    {item?.memberUtvalgs?.length &&
                                    item?.memberUtvalgs?.length > 0 ? (
                                      <div className="pl-2 ">
                                        <img
                                          id={item.listId}
                                          className="cursor"
                                          alt="Skjul liste"
                                          src={plus}
                                          onClick={handleExpandCollapse}
                                        />
                                        &nbsp;vis utvalg
                                        {collapseArr?.includes(item.listId)
                                          ? renderItem(item)
                                          : renderItem(null)}
                                      </div>
                                    ) : null}
                                  </div>
                                </div>
                                <br />
                              </div>
                            </div>
                          </div>
                        );
                      })}
                    </div>
                  ) : null}
                  <div className="bold  green row   ">
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
                  {/* second list of items */}
                  {!plusicon_1 ? (
                    <div>
                      {listresult.map((item) => {
                        return (
                          <div className="padding_NoColor_T">
                            <div className="clearFloat UpperLine padding_NoColor_T_B">
                              <div>
                                <div className="div">
                                  <div className="bold namefont">
                                    {item.name}
                                  </div>
                                  <div className="row float-right pr-3">
                                    <span className="namefont"> Kopier </span>
                                    {/* )} */}
                                    &nbsp;&nbsp;
                                    <input
                                      type="checkbox"
                                      onChange={(e) =>
                                        KopierCheckBoxClick(item, e)
                                      }
                                    />
                                  </div>

                                  <div className="namefont">
                                    Sist lagret{" "}
                                    {item.modifications !== undefined
                                      ? item.modifications.length !== 0
                                        ? item.modifications[
                                            item.modifications.length - 1
                                          ].modificationTime.substring(0, 10)
                                        : null
                                      : null}
                                    . Id:{" "}
                                    {typeof item.antall === "undefined" ||
                                    item.antall === ""
                                      ? item.utvalgId
                                      : item.listId}
                                    <br />
                                    {typeof item.antall === "undefined"
                                      ? item.totalAntall
                                      : item.antall}
                                    &nbsp; mottakere.
                                    {item.memberUtvalgs !== undefined ||
                                    typeof item.memberUtvalgs !== undefined ? (
                                      item.memberUtvalgs.length > 0 ? (
                                        <div className="pl-2 ">
                                          {!plusicon_member_1 ? (
                                            <img
                                              id="dlAUimg"
                                              className="cursor"
                                              alt="Skjul liste"
                                              src={plus}
                                              onClick={showmminusicon_member_1}
                                            />
                                          ) : (
                                            <img
                                              className="cursor"
                                              id="dlAUimg"
                                              alt="Skjul liste"
                                              src={minus}
                                              onClick={showplusicon_member_1}
                                            />
                                          )}{" "}
                                          &nbsp;vis utvalg
                                          {/* -------member id show----- */}
                                          {plusicon_member_1 ? (
                                            <div className="div">
                                              {item.memberUtvalgs.map(
                                                (item) => (
                                                  <div className="">
                                                    <div className="bold namefont">
                                                      {item.name}
                                                    </div>
                                                    <div className="row float-right pr-3">
                                                      <span className="namefont">
                                                        {" "}
                                                        Kopier{" "}
                                                      </span>
                                                      &nbsp;&nbsp;
                                                      <input
                                                        type="checkbox"
                                                        onChange={(e) =>
                                                          KopierCheckBoxClick(
                                                            item,
                                                            e
                                                          )
                                                        }
                                                      />
                                                    </div>

                                                    <div className="namefont">
                                                      Sist lagret{" "}
                                                      {item.modifications
                                                        .length !== 0
                                                        ? item.modifications[
                                                            item.modifications
                                                              .length - 1
                                                          ].modificationTime.substring(
                                                            0,
                                                            10
                                                          )
                                                        : null}
                                                      . Id:{"U" + item.utvalgId}
                                                      <br /> {item.totalAntall}
                                                      &nbsp; mottakere.
                                                    </div>
                                                  </div>
                                                )
                                              )}
                                            </div>
                                          ) : null}
                                          {/* -----member utval item ends ---- */}
                                        </div>
                                      ) : null
                                    ) : null}
                                  </div>
                                </div>
                                <br />
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
                  {/* thrid case started */}
                  {!plusicon_2 ? (
                    <div>
                      {sokcompletedorders.map((item) => {
                        return (
                          <div className="padding_NoColor_T">
                            <div className="clearFloat UpperLine padding_NoColor_T_B">
                              <div>
                                <div className="div">
                                  <div className="bold namefont">
                                    {item.name}
                                  </div>
                                  <div className="row float-right pr-3">
                                    <span className="namefont"> Kopier </span>
                                    {/* )} */}
                                    &nbsp;&nbsp;
                                    <input
                                      type="checkbox"
                                      onChange={(e) =>
                                        KopierCheckBoxClick(item, e)
                                      }
                                    />
                                  </div>

                                  <div className="namefont">
                                    Sist lagret{" "}
                                    {item.modifications !== undefined
                                      ? item.modifications.length !== 0
                                        ? item.modifications[
                                            item.modifications.length - 1
                                          ].modificationTime.substring(0, 10)
                                        : null
                                      : null}
                                    . Id:{" "}
                                    {typeof item.antall === "undefined" ||
                                    item.antall === ""
                                      ? item.utvalgId
                                      : item.listId}
                                    <br />
                                    {typeof item.antall === "undefined"
                                      ? item.totalAntall
                                      : item.antall}
                                    &nbsp; mottakere.
                                    {item.memberUtvalgs !== undefined ||
                                    typeof item.memberUtvalgs !== undefined ? (
                                      item.memberUtvalgs.length > 0 ? (
                                        <div className="pl-2 ">
                                          {!plusicon_member_2 ? (
                                            <img
                                              id="dlAUimg"
                                              className="cursor"
                                              alt="Skjul liste"
                                              src={plus}
                                              onClick={showmminusicon_member_2}
                                            />
                                          ) : (
                                            <img
                                              className="cursor"
                                              id="dlAUimg"
                                              alt="Skjul liste"
                                              src={minus}
                                              onClick={showplusicon_member_2}
                                            />
                                          )}{" "}
                                          &nbsp;vis utvalg
                                          {/* -------member id show----- */}
                                          {plusicon_member_2 ? (
                                            <div className="div">
                                              {item.memberUtvalgs.map(
                                                (item) => (
                                                  <div className="">
                                                    <div className="bold namefont">
                                                      {item.name}
                                                    </div>
                                                    <div className="row float-right pr-3">
                                                      <span className="namefont">
                                                        {" "}
                                                        Kopier{" "}
                                                      </span>
                                                      &nbsp;&nbsp;
                                                      <input
                                                        type="checkbox"
                                                        onChange={(e) =>
                                                          KopierCheckBoxClick(
                                                            item,
                                                            e
                                                          )
                                                        }
                                                      />
                                                    </div>

                                                    <div className="namefont">
                                                      Sist lagret{" "}
                                                      {item.modifications
                                                        .length !== 0
                                                        ? item.modifications[
                                                            item.modifications
                                                              .length - 1
                                                          ].modificationTime.substring(
                                                            0,
                                                            10
                                                          )
                                                        : null}
                                                      . Id:{" "}
                                                      {"U" + item.utvalgId}
                                                      <br /> {item.totalAntall}
                                                      &nbsp; mottakere.
                                                    </div>
                                                  </div>
                                                )
                                              )}
                                            </div>
                                          ) : null}
                                          {/* -----member utval item ends ---- */}
                                        </div>
                                      ) : null
                                    ) : null}
                                  </div>
                                </div>
                                <br />
                              </div>
                            </div>
                          </div>
                        );
                      })}
                    </div>
                  ) : null}
                  <br />
                </div>
              ) : null}
            </div>{" "}
          </div>
          {buttondisplay ? (
            <div className="padding_NoColor_T pb-4">
              <input
                type="submit"
                className={
                  LeggTilCheckedItems.length == 0 &&
                  KopierCheckedItems?.length == 0
                    ? "KSPU_button_Gray float-right pr-3"
                    : "KSPU_button-kw float-right pr-3"
                }
                onClick={LeggClick}
                value={"Legg til valgte"}
                disabled={
                  LeggTilCheckedItems.length > 0 ||
                  KopierCheckedItems?.length > 0 ||
                  activityLoading === true
                    ? false
                    : true
                }
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

export default Sok_Component_kw;
