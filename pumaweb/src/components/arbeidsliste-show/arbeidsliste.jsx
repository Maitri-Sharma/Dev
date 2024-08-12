import React, { useState, useRef, useContext, useEffect } from "react";
import { KSPUContext, MainPageContext } from "../../context/Context.js";
import {
  CreateUtvalglist,
  GetImageUrl,
  CreateActiveUtvalg,
  FormatDate,
  NumberFormat,
  imagePath,
  ColorCodes,
  filterCommonReolIds,
  CurrentDate,
} from "../../common/Functions";
import api from "../../services/api.js";
import Swal from "sweetalert2";
import $ from "jquery";
import { groupBy } from "../../Data";
import ConnectSelectiontolist from "./connectselectiontolist.jsx";

import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";
import * as query from "@arcgis/core/rest/query";

import Spinner from "../../components/spinner/spinner.component";

function Arbeidsliste(props) {
  const {
    activUtvalg,
    setActivUtvalg,
    setvalue,
    setAktivDisplay,
    setResultData,
    activUtvalglist,
    setActivUtvalglist,
    utvalglistcheck,
    setutvalglistcheck,
    showorklist,
    setshoworklist,
    setCheckedList,
    checkedList,
    setShowBusiness,
    showHousehold,
    setShowHousehold,
    showReservedHouseHolds,
    setShowReservedHouseHolds,
    showBusiness,
    setIsWidgetActive,
    setDemografieDisplay,
    setSegmenterDisplay,
    setAddresslisteDisplay,
    setGeografiDisplay,
    setKjDisplay,
    setAdresDisplay,
    Details,
    setDetails,
    Budruteendringer,
    setBudruteendringer,
    SelectionUpdate,
    setSelectionUpdate,
    expandListId,
    setExpandListId,
    maintainUnsavedRute,
    setMaintainUnsavedRute,
  } = useContext(KSPUContext);

  const Lukk = useRef(null);
  const Lukkalle = useRef(null);
  const Kobetil = useRef(null);
  const VisKart = useRef(null);
  const Fjern = useRef(null);
  const { mapView, ActiveMapButton, setActiveMapButton } =
    useContext(MainPageContext);
  const [selectedGroups, setSelectedGroups] = useState([]);
  const [btnDisabled, setBtnDisabled] = useState(true);
  const [addSelection, setaddSelection] = useState(" ");
  const { issave, setissave } = useContext(KSPUContext);

  const [refresh, setRefresh] = useState(false);

  const [loading, setloading] = useState(false);
  const [clearMap, setClearMap] = useState(false);
  const [selectedBasicSelection, setSelectedBasicSelection] = useState(false);

  let basicSelectionFlag = false;
  var newText = 0;
  let newList = false;
  let commonSelections = [];
  let resultsExtent = null;
  let dataReolerItem = [];
  const [Modal, setModal] = useState(false);
  const [owner, setOwner] = useState(null);

  const SaveUtvalgButton = async (e) => {
    if (owner === "Lukkalle") {
      setloading(true);

      let allUtvalgsDetails = [];
      maintainUnsavedRute.forEach(function (item) {
        allUtvalgsDetails.push(item.activeUtval);
      });

      let saveOldReoler = "false";
      let skipHistory = "false";
      let forceUtvalgListId = 0;

      let url = `Utvalg/SaveUtvalgs?userName=Internbruker&`;
      url = url + `saveOldReoler=${saveOldReoler}&`;
      url = url + `skipHistory=${skipHistory}&`;
      url = url + `forceUtvalgListId=${forceUtvalgListId}`;

      try {
        allUtvalgsDetails.map((item, index) => {
          item.modifications.push({
            modificationId: Math.floor(100000 + Math.random() * 900000),
            userId: "Internbruker",
            modificationTime: CurrentDate(),
          });
        });
        let queryRequest = {
          utvalgs: allUtvalgsDetails,
        };
        const { data, status } = await api.postdata(url, queryRequest);

        if (status === 200) {
          setOwner(null);
          setModal(false);
          let j = mapView.graphics.items.length;

          for (let i = j; i > 0; i--) {
            if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
              mapView.graphics.remove(mapView.graphics.items[i - 1]);
            }
          }

          setCheckedList([]);
          setshoworklist([]);
          setActivUtvalg({});
          setActivUtvalglist({});
          setSelectionUpdate(false);
          setutvalglistcheck(false);
          setBtnDisabled(true);
          setvalue(true);
          setAktivDisplay(false);

          //clear saved items from maintainUnsavedRute array
          if (maintainUnsavedRute?.length !== 0) {
            let tempMaintainUnsavedRute;

            tempMaintainUnsavedRute = maintainUnsavedRute;
            tempMaintainUnsavedRute.splice(0, tempMaintainUnsavedRute.length);
            setMaintainUnsavedRute(tempMaintainUnsavedRute);
          }

          $(".modal").remove();
          $(".modal-backdrop").remove();
          setloading(false);
        } else {
          let msg = `Oppgitte søkekriterier ga ikke noe resultat.`;
          $(".modal").remove();
          $(".modal-backdrop").remove();
          setloading(false);
          Swal.fire({
            text: msg,
            confirmButtonColor: "#7bc144",
            confirmButtonText: "Lukk",
          });
        }
      } catch (error) {
        console.error("error : " + error);
        let msg = `Oppgitte søkekriterier ga ikke noe resultat.`;
        $(".modal").remove();
        $(".modal-backdrop").remove();
        Swal.fire({
          text: msg,
          confirmButtonColor: "#7bc144",
          confirmButtonText: "Lukk",
        });
        setloading(false);
      }
    } else {
      setloading(true);
      let selectedUtvalgIDs = [];
      let activeSelectionIDs = [];

      if (maintainUnsavedRute?.length !== 0) {
        //get utvalgid from checkedlist and compare with maintainUnsavedRute utvalgid
        if (checkedList.length !== 0 || checkedList.length !== undefined) {
          checkedList.forEach(function (item) {
            selectedUtvalgIDs.push(item.utvalgId);
          });
        }

        maintainUnsavedRute.forEach(function (item) {
          selectedUtvalgIDs.forEach(function (selectedUtvalgItem) {
            if (item.selectionID === selectedUtvalgItem) {
              activeSelectionIDs.push(item.activeUtval);
            }
          });
        });
      }

      let saveOldReoler = "false";
      let skipHistory = "false";
      let forceUtvalgListId = 0;

      let url = `Utvalg/SaveUtvalgs?userName=Internbruker&`;
      url = url + `saveOldReoler=${saveOldReoler}&`;
      url = url + `skipHistory=${skipHistory}&`;
      url = url + `forceUtvalgListId=${forceUtvalgListId}`;

      try {
        activeSelectionIDs.map((item, index) => {
          item.modifications.push({
            modificationId: Math.floor(100000 + Math.random() * 900000),
            userId: "Internbruker",
            modificationTime: CurrentDate(),
          });
        });
        let queryRequest = {
          utvalgs: activeSelectionIDs,
        };
        const { data, status } = await api.postdata(url, queryRequest);

        if (status === 200) {
          setOwner(null);
          setModal(false);

          var sliced = [];

          selectedGroups.map((item) => sliced.push(item.slice(1)));

          var checkList = showorklist.filter((item, i) => {
            if (item.utvalgId === undefined) {
              return !sliced.includes(JSON.stringify(item.listId));
            } else {
              return !sliced.includes(JSON.stringify(item.utvalgId));
            }
          });

          let j = mapView.graphics.items.length;
          for (let i = j; i > 0; i--) {
            if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
              mapView.graphics.remove(mapView.graphics.items[i - 1]);
            }
          }

          if (checkList.length === 0 || checkList.length === undefined) {
            setCheckedList([]);
            setshoworklist([]);
            setBtnDisabled(true);
            setvalue(true);
            setActivUtvalg({});
            setActivUtvalglist({});
            setAktivDisplay(false);
            setDemografieDisplay(false);
            setSegmenterDisplay(false);
            setAddresslisteDisplay(false);
            setGeografiDisplay(false);
            setKjDisplay(false);
          } else {
            showorklist.map((item) => {
              if (Object.keys(activUtvalg).length > 0) {
                if (
                  item?.utvalgId?.toString() ===
                  activUtvalg?.utvalgId?.toString()
                ) {
                  setActivUtvalg({});
                  setvalue(true);
                  setAktivDisplay(false);
                }
              } else {
                if (
                  item?.listId?.toString() ===
                  activUtvalglist?.listId?.toString()
                ) {
                  setActivUtvalglist({});
                  setutvalglistcheck(false);
                  setvalue(true);
                  setAktivDisplay(false);
                }
              }
            });

            //remove selected item from checkedlist from maintainUnsavedRute array
            let selectedUtvalgIDs = [];
            let activeSelectionIDs = [];
            if (checkedList.length !== 0 || checkedList.length !== undefined) {
              checkedList.forEach(function (item) {
                selectedUtvalgIDs.push(item.utvalgId);
              });
            }

            //remove selected item from maintainUnsavedRute array
            if (maintainUnsavedRute?.length !== 0) {
              let tempMaintainUnsavedRute;
              maintainUnsavedRute.forEach(function (item) {
                selectedUtvalgIDs.forEach(function (selectedUtvalgItem) {
                  if (item.selectionID === selectedUtvalgItem) {
                    activeSelectionIDs.push(item.selectionID);
                  }
                });
              });

              tempMaintainUnsavedRute = maintainUnsavedRute;
              tempMaintainUnsavedRute = tempMaintainUnsavedRute.filter(
                (obj) =>
                  !activeSelectionIDs.some(
                    (activeSelectionIDs) =>
                      obj.selectionID === activeSelectionIDs
                  )
              );

              setMaintainUnsavedRute(tempMaintainUnsavedRute);
            }

            setRefresh(true);
            setshoworklist(checkList);
            setCheckedList([]);
            setSelectedGroups([]);
            setTimeout(() => {
              setRefresh(false);
            }, 5);
          }

          setloading(false);
        } else {
          let msg = `Oppgitte søkekriterier ga ikke noe resultat.`;
          $(".modal").remove();
          $(".modal-backdrop").remove();
          setloading(false);
          Swal.fire({
            text: msg,
            confirmButtonColor: "#7bc144",
            confirmButtonText: "Lukk",
          });
        }
      } catch (error) {
        console.error("error : " + error);
        let msg = `Oppgitte søkekriterier ga ikke noe resultat.`;
        $(".modal").remove();
        $(".modal-backdrop").remove();
        Swal.fire({
          text: msg,
          confirmButtonColor: "#7bc144",
          confirmButtonText: "Lukk",
        });
        setloading(false);
      }

      //save only utvalg object from maintainunsavedrute which are utvalgsid are present in checklist
      //todo
      //empty the maintainunsavedrute's id which are saved
      setloading(false);

      setOwner(null);
      setModal(false);
    }
  };

  const showToMap = () => {
    //close mapview popup
    mapView.popup.close();

    commonSelections = filterCommonReolIds(checkedList);
    setClearMap(true);
    resultsExtent = null;

    let j = mapView.graphics.items.length;

    for (let i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
      }
    }

    checkedList.map((item, index, checkedList) => {
      if (item.utvalgId !== undefined && item.utvalgId !== 0) {
        let reolId = [];
        if (item.reoler && item.reoler.length > 0) {
          item.reoler.map(async (reolerItem, index) => {
            reolId.push(reolerItem.reolId);
          });
        }

        if (reolId.length > 0) {
          if (item?.imagePath) {
            let tempPath;
            if (item?.imagePath > 36) {
              tempPath = item?.imagePath % 36 === 0 ? 36 : item?.imagePath % 36;
            } else {
              tempPath = item?.imagePath;
            }

            let imageCode = tempPath - 1;
            zoomToSelection(
              reolId,
              ColorCodes()[imageCode],
              checkedList.length,
              index
            );
          } else {
            let tempPath =
              item?.imagePathFull % 36 === 0 ? 1 : item?.imagePathFull % 36;

            zoomToSelection(
              reolId,
              ColorCodes()[tempPath],
              checkedList.length,
              index
            );
          }
        } else if (commonSelections.filteredCommonItems.length > 0) {
          zoomToDoubleCoverage(
            commonSelections.filteredCommonItems,
            "rgba(0, 255, 0, 0.80)"
          );
        }
      }
    });

    window.scrollTo(0, 0);
  };

  const zoomToSelection = async (
    Reolids,
    colorcode,
    arrayLength,
    arrayIndex
  ) => {
    if (Reolids.length > 0) {
      let k = Reolids.map((element) => "'" + element + "'").join(",");
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
        setloading(true);

        const queryExtent = new Query();
        queryExtent.where = `${sql_geography}`;
        let queryExtentResult = await query.executeForExtent(
          BudruterUrl,
          queryExtent
        );

        if (!resultsExtent) resultsExtent = queryExtentResult.extent;
        else resultsExtent.union(queryExtentResult.extent);

        //Get ObjectIDs
        const queryOIDs = new Query();
        queryOIDs.where = `${sql_geography}`;
        let oids = await query.executeForIds(BudruterUrl, queryOIDs);

        //build query block for more than 2000 ObjectIDs
        let times = null;
        let quotient = Math.floor(oids.length / 2000);
        let remainder = oids.length % 2000;
        let startIndex = 0;
        let endIndex = 2000;
        let promise = [];

        if (quotient < 1) {
          times = 1;
          endIndex = oids.length;
        } else {
          if (remainder === 0) {
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

          queryResults.outFields = ["tot_anta", "hh", "hh_res", "reol_id"];
          queryResults.where = "OBJECTID IN (" + objectsIds.join(",") + ")";
          queryResults.outSpatialReference = mapView.spatialReference;
          queryResults.returnGeometry = true;

          promise[i] = query.executeQueryJSON(BudruterUrl, queryResults);
        }

        const run = async function waitForPromise() {
          //setloading(true);
          // let result = await any Promise, like:
          let values = await Promise.all(promise);

          for (let i = 0; i < values.length; i++) {
            for (let j = 0; j < values[i].features.length; j++) {
              let selectedSymbol = {
                type: "simple-fill", // autocasts as new SimpleFillSymbol()
                color: colorcode,
                style: "solid",
                outline: {
                  // autocasts as new SimpleLineSymbol()
                  color: [237, 54, 21],
                  width: 0.75,
                },
              };

              let graphic = new Graphic(
                values[i].features[j].geometry,
                selectedSymbol,
                values[i].features[j].attributes
              );
              mapView.graphics.add(graphic);
            }
          }

          if (arrayLength === undefined) {
            if (commonSelections.filteredCommonItems.length > 0) {
              // delay(20000);
              setloading(true);
              zoomToDoubleCoverage(
                commonSelections.filteredCommonItems,
                "rgba(0, 255, 0, 0.80)"
              );
            } else {
              setloading(false);
            }
          }

          if (arrayLength - 1 === arrayIndex) {
            if (commonSelections.filteredCommonItems.length > 0) {
              //await delay(20000);
              setloading(true);
              zoomToDoubleCoverage(
                commonSelections.filteredCommonItems,
                "rgba(0, 255, 0, 0.80)",
                true
              );
            } else {
              mapView.goTo(resultsExtent);
              setloading(false);
            }
          }
        };

        run();
      }
    }
  };

  const zoomToDoubleCoverage = async (Reolids, colorcode, enableZoom) => {
    if (Reolids.length > 0) {
      //setloading(true);
      let k = Reolids.map((element) => "'" + element + "'").join(",");
      let reolsWhereClause = `reol_id in (${k})`;
      let BudruterUrl;

      //await delay(15000);

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

      //Get ObjectIDs
      const queryOIDs = new Query();
      queryOIDs.where = `${reolsWhereClause}`;
      let oids = await query.executeForIds(BudruterUrl, queryOIDs);

      //build query block for more than 2000 ObjectIDs
      let times = null;
      let quotient = Math.floor(oids.length / 2000);
      let remainder = oids.length % 2000;
      let startIndex = 0;
      let endIndex = 2000;
      let promise = [];

      const queryExtent = new Query();
      queryExtent.where = `${reolsWhereClause}`;
      let doubleCoveragefeaturesGeometryExtent = await query.executeForExtent(
        BudruterUrl,
        queryExtent
      );

      if (enableZoom) {
        mapView.goTo(doubleCoveragefeaturesGeometryExtent);
      }

      if (quotient < 1) {
        times = 1;
        endIndex = oids.length;
      } else {
        if (remainder === 0) {
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

        queryResults.outFields = ["tot_anta", "hh", "hh_res", "reol_id"];
        queryResults.where = "OBJECTID IN (" + objectsIds.join(",") + ")";
        queryResults.outSpatialReference = mapView.spatialReference;
        queryResults.returnGeometry = true;

        promise[i] = query.executeQueryJSON(BudruterUrl, queryResults);
      }

      Promise.all(promise).then((values) => {
        let doubleCoverageGraphics = [];
        for (let i = 0; i < values.length; i++) {
          for (let j = 0; j < values[i].features.length; j++) {
            let selectedSymbol = {};
            if (dataReolerItem.length !== 0) {
              if (
                dataReolerItem.includes(
                  values[i].features[j].attributes.reol_id
                )
              ) {
                selectedSymbol = {
                  type: "simple-fill", // autocasts as new SimpleFillSymbol()
                  color: colorcode,
                  style: "solid",
                  outline: {
                    // autocasts as new SimpleLineSymbol()
                    color: [237, 54, 21],
                    width: 0.75,
                  },
                };
              } else {
                selectedSymbol = {
                  type: "simple-fill", // autocasts as new SimpleFillSymbol()
                  color: [255, 255, 0, 1], //yellow
                  style: "solid",
                  outline: {
                    // autocasts as new SimpleLineSymbol()
                    color: [237, 54, 21],
                    width: 0.75,
                  },
                };
              }
            } else {
              selectedSymbol = {
                type: "simple-fill", // autocasts as new SimpleFillSymbol()
                color: colorcode,
                style: "solid",
                outline: {
                  // autocasts as new SimpleLineSymbol()
                  color: [237, 54, 21],
                  width: 0.75,
                },
              };
            }

            let graphic = new Graphic(
              values[i].features[j].geometry,
              selectedSymbol,
              values[i].features[j].attributes
            );

            doubleCoverageGraphics.push(graphic);
          }
        }
        setTimeout(function () {
          mapView.graphics.addMany(doubleCoverageGraphics);
          setloading(false);
        }, 15000);
      });

      // mapView.watch("updating", function (evt) {
      //   if (evt) {
      //     setloading(true);
      //   } else {
      //     setloading(false);
      //   }
      // });
    }
  };

  const selectAll = () => {
    setRefresh(true);
    let ar1 = [];
    let ar2 = [];
    showorklist.map((item, key) => {
      {
        item.memberLists &&
          item.memberLists?.map((val) => {
            if (val?.memberUtvalgs) {
              val?.memberUtvalgs?.map((item) => {
                ar1.push(item);
              });
            }
            if (val?.memberLists) {
              val?.memberLists?.map((item) => {
                ar1.push(item);
              });
            }
            ar1.push(val);
          });
      }
      {
        item.memberUtvalgs &&
          item.memberUtvalgs?.map((val) => {
            ar2.push(val);
          });
      }
    });

    setCheckedList(showorklist.concat(ar1).concat(ar2));

    renderPerson(showorklist);
    setTimeout(() => {
      setRefresh(false);
    }, 5);

    let newArray = [];
    newArray = selectedGroups;

    showorklist.map((item) => {
      if (item.utvalgId === undefined || item.utvalgId === 0) {
        let id = "L" + item.listId;
        newArray = [...newArray, id];
        if (selectedGroups.includes(id)) {
          newArray = newArray.filter((day) => day !== id);
        }
      } else {
        let id = "U" + item.utvalgId;
        newArray = [...newArray, id];
        if (selectedGroups.includes(id)) {
          newArray = newArray.filter((day) => day !== id);
        }
      }
      setSelectedGroups(newArray);
    });
    if (newArray.length > 0) {
      setBtnDisabled(false);
    } else {
      setBtnDisabled(true);
    }
    checkedList.map((item, index) => {
      item["imagePathFull"] = index;
    });
  };

  const neiClick = (e) => {
    if (owner === "Lukkalle") {
      setOwner(null);
      setModal(false);
      let j = mapView.graphics.items.length;
      for (let i = j; i > 0; i--) {
        if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
          mapView.graphics.remove(mapView.graphics.items[i - 1]);
        }
      }

      setCheckedList([]);
      setshoworklist([]);
      setActivUtvalg({});
      setActivUtvalglist({});
      setutvalglistcheck(false);
      setSelectionUpdate(false);
      setBtnDisabled(true);
      setvalue(true);
      setAktivDisplay(false);
    } else {
      setOwner(null);
      setModal(false);
      var sliced = [];

      selectedGroups.map((item) => sliced.push(item.slice(1)));

      var checkList = showorklist.filter((item, i) => {
        if (item.utvalgId === undefined) {
          return !sliced.includes(JSON.stringify(item.listId));
        } else {
          return !sliced.includes(JSON.stringify(item.utvalgId));
        }
      });

      let j = mapView.graphics.items.length;
      for (let i = j; i > 0; i--) {
        if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
          mapView.graphics.remove(mapView.graphics.items[i - 1]);
        }
      }

      if (checkList.length === 0 || checkList.length === undefined) {
        setCheckedList([]);
        setshoworklist([]);
        setBtnDisabled(true);
        setvalue(true);
        setActivUtvalg({});
        setActivUtvalglist({});
        setAktivDisplay(false);
        setDemografieDisplay(false);
        setSegmenterDisplay(false);
        setAddresslisteDisplay(false);
        setGeografiDisplay(false);
        setKjDisplay(false);
      } else {
        showorklist.map((item) => {
          if (Object.keys(activUtvalg).length > 0) {
            if (
              item?.utvalgId?.toString() === activUtvalg?.utvalgId?.toString()
            ) {
              setActivUtvalg({});
              setvalue(true);
              setAktivDisplay(false);
            }
          } else {
            if (
              item?.listId?.toString() === activUtvalglist?.listId?.toString()
            ) {
              setActivUtvalglist({});
              setutvalglistcheck(false);
              setvalue(true);
              setAktivDisplay(false);
            }
          }
        });

        //remove selected item from checkedlist from maintainUnsavedRute array
        let selectedUtvalgIDs = [];
        let activeSelectionIDs = [];
        if (checkedList.length !== 0 || checkedList.length !== undefined) {
          checkedList.forEach(function (item) {
            selectedUtvalgIDs.push(item.utvalgId);
          });
        }

        //remove selected item from maintainUnsavedRute array
        if (maintainUnsavedRute?.length !== 0) {
          let tempMaintainUnsavedRute;
          maintainUnsavedRute.forEach(function (item) {
            selectedUtvalgIDs.forEach(function (selectedUtvalgItem) {
              if (item.selectionID === selectedUtvalgItem) {
                activeSelectionIDs.push(item.selectionID);
              }
            });
          });

          tempMaintainUnsavedRute = maintainUnsavedRute;
          tempMaintainUnsavedRute = tempMaintainUnsavedRute.filter(
            (obj) =>
              !activeSelectionIDs.some(
                (activeSelectionIDs) => obj.selectionID === activeSelectionIDs
              )
          );

          setMaintainUnsavedRute(tempMaintainUnsavedRute);
        }

        setRefresh(true);
        setshoworklist(checkList);
        setCheckedList([]);
        setSelectedGroups([]);
        setTimeout(() => {
          setRefresh(false);
        }, 5);
      }
    }
  };

  const deSelectAll = () => {
    let j = mapView.graphics.items.length;
    for (let i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
      }
    }

    setClearMap(false);
    setRefresh(true);
    setCheckedList([]);
    setSelectedGroups([]);
    setBtnDisabled(true);
    renderPerson(showorklist);
    setTimeout(() => {
      setRefresh(false);
    }, 10);
  };

  useEffect(() => {
    setRefresh(true);
    setCheckedList([]);
    setSelectedGroups([]);
    setBtnDisabled(true);
    renderPerson(showorklist);

    setTimeout(() => {
      setRefresh(false);
    }, 10);
  }, []);

  useEffect(() => {
    setRefresh(true);

    renderPerson(showorklist);

    setTimeout(() => {
      setRefresh(false);
    }, 5);
  }, [showorklist]);

  const workListLukk = async (e) => {
    if (maintainUnsavedRute?.length !== 0) {
      //get utvalgid from checkedlist and compare with maintainUnsavedRute utvalgid
      let selectedUtvalgIDs = [];
      let activeSelectionIDs = [];
      if (checkedList.length !== 0 || checkedList.length !== undefined) {
        checkedList.forEach(function (item) {
          selectedUtvalgIDs.push(item.utvalgId);
        });
      }

      maintainUnsavedRute.forEach(function (item) {
        selectedUtvalgIDs.forEach(function (selectedUtvalgItem) {
          if (item.selectionID === selectedUtvalgItem) {
            activeSelectionIDs.push(item.selectionID);
          }
        });
      });

      if (activeSelectionIDs.length !== 0) {
        setOwner("Lukk");
        setModal(true);
      }
    } else {
      setOwner(null);
      setModal(false);
      var sliced = [];

      selectedGroups.map((item) => sliced.push(item.slice(1)));

      var checkList = showorklist.filter((item, i) => {
        if (item.utvalgId === undefined) {
          return !sliced.includes(JSON.stringify(item.listId));
        } else {
          return !sliced.includes(JSON.stringify(item.utvalgId));
        }
      });

      let j = mapView.graphics.items.length;
      for (let i = j; i > 0; i--) {
        if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
          mapView.graphics.remove(mapView.graphics.items[i - 1]);
        }
      }

      if (checkList.length === 0 || checkList.length === undefined) {
        setCheckedList([]);
        setshoworklist([]);
        setBtnDisabled(true);
        setvalue(true);
        setActivUtvalg({});
        setActivUtvalglist({});
        setAktivDisplay(false);
        setDemografieDisplay(false);
        setSegmenterDisplay(false);
        setAddresslisteDisplay(false);
        setGeografiDisplay(false);
        setKjDisplay(false);
      } else {
        showorklist.map((item) => {
          if (Object.keys(activUtvalg).length > 0) {
            if (
              item?.utvalgId?.toString() === activUtvalg?.utvalgId?.toString()
            ) {
              setActivUtvalg({});
              setvalue(true);
              setAktivDisplay(false);
            }
          } else {
            if (
              item?.listId?.toString() === activUtvalglist?.listId?.toString()
            ) {
              setActivUtvalglist({});
              setutvalglistcheck(false);
              setvalue(true);
              setAktivDisplay(false);
            }
          }
        });

        setRefresh(true);
        setshoworklist(checkList);
        setCheckedList([]);
        setSelectedGroups([]);
        setTimeout(() => {
          setRefresh(false);
        }, 10);
      }
    }
  };

  const worklistLukkAll = (e) => {
    if (maintainUnsavedRute?.length !== 0) {
      setOwner("Lukkalle");
      setModal(true);
    } else {
      setOwner(null);
      setModal(false);

      let j = mapView.graphics.items.length;

      for (let i = j; i > 0; i--) {
        if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
          mapView.graphics.remove(mapView.graphics.items[i - 1]);
        }
      }

      setCheckedList([]);
      setshoworklist([]);
      setActivUtvalg({});
      setActivUtvalglist({});
      setSelectionUpdate(false);
      setutvalglistcheck(false);
      setBtnDisabled(true);
      setvalue(true);
      setAktivDisplay(false);
    }
  };

  const handleChange = async (e, item, colorCodeIndex) => {
    // setExpandListId(false);
    setRefresh(true);
    setaddSelection("");
    if (selectedGroups.length) {
      var newArray = selectedGroups;
    } else {
      var newArray = [];
    }
    if (e.target.checked) {
      newArray = [...selectedGroups, e.target.id];
      if (selectedGroups.includes(e.target.id)) {
        newArray = newArray.filter((day) => day !== e.target.id);
      }
    } else {
      if (selectedGroups.includes(e.target.id)) {
        newArray = newArray.filter((day) => day !== e.target.id);
      }
    }
    await setSelectedGroups(newArray);

    if (e.target.checked) {
      // item["imagePath"] = colorCodeIndex;
      if (item.memberUtvalgs?.length) {
        item.memberUtvalgs.map((item) => {
          checkedList.push(item);
        });
      }
      if (item.memberLists?.length) {
        item.memberLists?.map((val) => {
          if (val?.memberUtvalgs) {
            val?.memberUtvalgs?.map((data) => {
              checkedList.push(data);
            });
          }
          if (val?.memberLists) {
            val?.memberLists?.map((data) => {
              checkedList.push(data);
            });
          }
          checkedList.push(val);
        });
      }
      checkedList.push(item);
      setBtnDisabled(false);
    } else {
      let checkedListCopy = checkedList;
      if (
        item.memberUtvalgs?.length &&
        !expandListId.includes(item?.listId.toString())
      ) {
        item.memberUtvalgs?.map((items) => {
          let arr = checkedListCopy.filter((val) => {
            return (
              val.utvalgId !== items?.utvalgId && item.listId !== val?.listId
            );
          });
          checkedListCopy = arr;
        });
        var filteredArr = checkedListCopy;
      } else if (
        item.memberLists?.length &&
        !expandListId.includes(item?.listId.toString())
      ) {
        item.memberLists?.map((items) => {
          items.memberUtvalgs?.map((lastChild) => {
            let arr = checkedListCopy.filter((val) => {
              return (
                lastChild?.utvalgId !== val?.utvalgId &&
                items?.listId !== val?.listId &&
                item?.listId !== val?.listId
              );
            });
            checkedListCopy = arr;
          });
        });
        var filteredArr = checkedListCopy;
      } else {
        var filteredArr = checkedList.filter((val) => {
          if (val.utvalgId === undefined) {
            return val.listId !== item.listId;
          } else if (val.utvalgId !== undefined) {
            return val.utvalgId !== item.utvalgId;
          }
        });
      }

      filteredArr.map((item, index) => {
        item["imagePathFull"] = index;
      });

      setCheckedList(filteredArr);
      if (filteredArr.length > 0) {
        setBtnDisabled(false);
      } else {
        setBtnDisabled(true);
      }
    }

    setRefresh(false);
  };

  const decoupledList = async (e) => {
    let queryRequest = [];
    let listArray = [];
    let updatedListId = "";
    let updatedListName = "";
    let selectionUpdate = false;
    checkedList.map((item) => {
      if (
        item.utvalgId &&
        (item.listId === 0 || item.listId === undefined || item.listId === "0")
      ) {
        selectionUpdate = false;
        let msg =
          "Du har krysset av et eller flere utvalg som ikke ligger i en utvalgsliste. Denne funksjonen kan kun kjøres på utvalg som ligger i en utvalgsliste.";
        $(".modal").remove();
        $(".modal-backdrop").remove();
        Swal.fire({
          text: msg,
          confirmButtonColor: "#7bc144",
          confirmButtonText: "Lukk",
        });
      } else if (!item?.utvalgId) {
        selectionUpdate = false;
        let msg =
          "Du har krysset av et eller flere utvalg som ikke ligger i en utvalgsliste. Denne funksjonen kan kun kjøres på utvalg som ligger i en utvalgsliste.";
        $(".modal").remove();
        $(".modal-backdrop").remove();
        Swal.fire({
          text: msg,
          confirmButtonColor: "#7bc144",
          confirmButtonText: "Lukk",
        });
      } else if (item?.ordreType === 1) {
        selectionUpdate = false;
        let msg =
          "Du forsøker nå å koble fra ett utvalg eller liste til en ordre. Annuller eller lås opp ordre før endring.";
        $(".modal").remove();
        $(".modal-backdrop").remove();
        Swal.fire({
          text: msg,
          confirmButtonColor: "#7bc144",
          confirmButtonText: "Lukk",
        });
      } else {
        updatedListId = item.listId;
        updatedListName = item.listName;
        let queryString = {
          currentlyActive: true,
          expanded: true,
          utvalg: item,
          utvalgListId: item.listId,
          innrykk: 0,
          swatchMapSymbol: null,
        };
        listArray.push(item);
        queryRequest.push(queryString);
        selectionUpdate = true;
        if (item?.utvalgId !== undefined) {
          let checked = checkedList.filter((val) => {
            return val?.utvalgId !== item?.utvalgId;
          });
          // setCheckedList(checked);
        }
      }
    });
    if (selectionUpdate) {
      setRefresh(true);
      // setCheckedList([]);
      // setCheckedList(listArray);
      setloading(true);
      let newObj = {};
      newObj["workingListEntries"] = queryRequest;

      if (queryRequest.length !== 0) {
        let url = `Utvalg/DecoupleUtvalgsFromLists?userName=Internbruker`;

        try {
          //api.logger.info("APIURL", url);
          const { data, status } = await api.postdata(url, newObj);

          if (status === 200) {
            let worklistShowFlag = false;
            listArray.map((listItem) => {
              // if (listItem.listId.toString() === updatedListId.toString()) {
              listItem.listId = 0;
              listItem.listName = "";
              listItem.ordreType = 0;
              listItem.ordreStatus = 0;
              // }
              // listArray.push(listItem);
            });
            showorklist.map((item) => {
              if (item.listId.toString() !== updatedListId.toString()) {
                let flag = false;
                if (item?.memberLists?.length > 0) {
                  item?.memberLists.map((newItemMember) => {
                    if (
                      newItemMember.listId.toString() ===
                      updatedListId.toString()
                    ) {
                      flag = true;
                    }
                  });
                  if (!flag) {
                    listArray.push(item);
                  } else {
                    updatedListId = item.listId;
                    worklistShowFlag = true;
                  }
                } else {
                  if (!listArray.includes(item)) {
                    listArray.push(item);
                  }
                }
              } else {
                if (!item.antall) {
                  item.listId = 0;
                  item.listName = "";
                  listArray.push(item);
                } else {
                  worklistShowFlag = true;
                }
              }
            });

            let flagArray = [];
            if (worklistShowFlag) {
              let newlistUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${updatedListId}`;
              const { data, status } = await api.getdata(newlistUrl);
              if (status === 200 && data !== undefined) {
                if (listArray.length > 0) {
                  await setshoworklist(listArray.concat(data));
                } else {
                  flagArray.push(data);
                  await setshoworklist(flagArray);
                }
                if (
                  activUtvalglist?.listId.toString() === data?.listId.toString()
                ) {
                  // let obj = await CreateUtvalglist(data);
                  setActivUtvalglist(data);
                } else {
                  let apiCallUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${activUtvalglist?.listId}`;
                  const { data, status } = await api.getdata(apiCallUrl);
                  if (status === 200 && data !== undefined) {
                    // let obj = await CreateUtvalglist(data);
                    setActivUtvalglist(data);
                  }
                }
              }
            } else {
              await setshoworklist(listArray);
            }
            renderPerson(showorklist);
            setTimeout(() => {
              setRefresh(false);
            }, 5);
            $(".modal").remove();
            $(".modal-backdrop").remove();
            Swal.fire({
              text: `Utvalg ble koblet fra liste: ${updatedListName}`,
              confirmButtonColor: "#7bc144",
              confirmButtonText: "Lukk",
            });

            setloading(false);
          }
        } catch (error) {
          //api.logger.error("errorpage API not working");
          //api.logger.error("error : " + error);
          setloading(false);
        }
      }
    }
  };

  const openWorklist = async (e) => {
    //disable sketech widget on switching the selection
    if (mapView.activeTool !== null) {
      setActiveMapButton("");
      mapView.activeTool = null;
    }

    //close mapview popup
    mapView.popup.close();

    setBudruteendringer(false);
    if (!clearMap) {
      let j = mapView.graphics.items.length;

      for (let i = j; i > 0; i--) {
        if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
          mapView.graphics.remove(mapView.graphics.items[i - 1]);
        }
      }
    }

    mapView.graphics.items.forEach(function (item) {
      if (item.attributes !== null) {
        if (item.attributes.utvalgid !== undefined) {
          mapView.graphics.remove(item);
        }
      }
    });

    let previousActivUtvalg;
    if (Object.keys(activUtvalg).length > 0) {
      previousActivUtvalg = activUtvalg;
    }

    let url = "";
    let id = e.target.id;
    let checkutvalg = id.substring(0, 1);
    id = id.substring(1);
    setShowReservedHouseHolds(false);
    setActivUtvalglist({});
    setActivUtvalg({});
    // setutvalglistcheck(false);
    setShowBusiness(false);
    setIsWidgetActive(false);
    setShowHousehold(false);
    checkutvalg = checkutvalg.toUpperCase();
    if (checkutvalg === "U") {
      setloading(true);
      setActivUtvalglist({});
      // setActivUtvalg({});
      setutvalglistcheck(false);
      url = url + `Utvalg/GetUtvalg?utvalgId=${id}`;
      try {
        //api.logger.info("APIURL", url);
        let { data, status } = await api.getdata(url);

        if (status !== 200) {
          //api.logger.error("Error : No Data is present for mentioned Id" + id);
        } else {
          commonSelections = filterCommonReolIds(checkedList);
          if (maintainUnsavedRute?.length !== 0) {
            maintainUnsavedRute.forEach(function (item) {
              if (item.selectionID === parseInt(id)) {
                data = {};
                data = JSON.parse(JSON.stringify(item.activeUtval));
              }
            });
          }
          showorklist.map((item, x) => {
            if (item?.utvalgId === data?.utvalgId) {
              item.totalAntall = data?.totalAntall;
            }
          });
          setshoworklist([...showorklist]);
          let selectedUtvalgColorCode;
          checkedList.forEach(function (utvalgItem) {
            if (utvalgItem?.utvalgId.toString() === id) {
              let tempPath;

              if (utvalgItem?.imagePath > 36) {
                tempPath =
                  utvalgItem?.imagePath % 36 === 0
                    ? 36
                    : utvalgItem?.imagePath % 36;
              } else {
                tempPath = utvalgItem?.imagePath;
              }

              let colorcode = ColorCodes()[tempPath - 1];
              if (colorcode !== undefined) {
                selectedUtvalgColorCode = {
                  r: colorcode[0],
                  g: colorcode[1],
                  b: colorcode[2],
                  a: colorcode[3],
                };
              }
            }
          });

          setActivUtvalg({});

          setResultData(
            groupBy(
              data.reoler,
              "",
              0,
              showHousehold,
              showBusiness,
              showReservedHouseHolds,
              []
            )
          );

          if (clearMap) {
            let previousActiveUtvalgImagePath;
            checkedList.forEach(function (utvalgItem) {
              if (previousActivUtvalg !== undefined) {
                if (utvalgItem.utvalgId === previousActivUtvalg.utvalgId) {
                  previousActiveUtvalgImagePath = utvalgItem.imagePath;
                }
              }
            });

            dataReolerItem = [];
            data.reoler.forEach(function (reolItem) {
              dataReolerItem.push(reolItem.reolId.toString());
            });

            let j = mapView.graphics.items.length;
            for (let i = j; i > 0; i--) {
              if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
                if (selectedUtvalgColorCode !== undefined) {
                  if (
                    dataReolerItem.includes(
                      mapView.graphics.items[i - 1].attributes.reol_id
                    )
                  ) {
                    if (
                      Object.entries(
                        mapView.graphics.items[i - 1].symbol.color
                      ).toString() ===
                        Object.entries(selectedUtvalgColorCode).toString() ||
                      Object.entries(
                        mapView.graphics.items[i - 1].symbol.color
                      ).toString() ===
                        Object.entries({
                          r: 0,
                          g: 255,
                          b: 0,
                          a: 0.8,
                        }).toString() ||
                      Object.entries(
                        mapView.graphics.items[i - 1].symbol.color
                      ).toString() ===
                        Object.entries({
                          r: 255,
                          g: 255,
                          b: 0,
                          a: 1,
                        }).toString()
                    ) {
                      mapView.graphics.remove(mapView.graphics.items[i - 1]);
                    }
                  }
                } else {
                  mapView.graphics.remove(mapView.graphics.items[i - 1]);
                }
              }
            }

            let checkedListUtvalId = [];
            checkedList.forEach(function (utvalgItem) {
              checkedListUtvalId.push(utvalgItem.utvalgId);
            });

            if (previousActivUtvalg !== undefined) {
              if (checkedListUtvalId.includes(previousActivUtvalg.utvalgId)) {
                let previousActivUtvalgReolerItem = [];
                previousActivUtvalg.reoler.forEach(function (reolItem) {
                  previousActivUtvalgReolerItem.push(
                    reolItem.reolId.toString()
                  );
                });

                let m = mapView.graphics.items.length;
                for (let n = m; n > 0; n--) {
                  if (
                    mapView.graphics.items[n - 1].geometry.type === "polygon"
                  ) {
                    if (
                      previousActivUtvalgReolerItem.includes(
                        mapView.graphics.items[n - 1].attributes.reol_id
                      )
                    ) {
                      mapView.graphics.remove(mapView.graphics.items[n - 1]);
                    }
                  }
                }
              }
            }

            let previousActivUtvalgReolIds = [];
            if (previousActivUtvalg !== undefined) {
              previousActivUtvalg.reoler.map(async (reolerItem, index) => {
                previousActivUtvalgReolIds.push(reolerItem.reolId);
              });
            }

            let obj = await CreateActiveUtvalg(data);

            await setActivUtvalg(obj);

            setutvalglistcheck(false);

            setissave(true);

            setvalue(false);
            setDetails(false);
            // setAktivDisplay(false);

            setDemografieDisplay(false);
            setSegmenterDisplay(false);
            setAddresslisteDisplay(false);
            setGeografiDisplay(false);

            setAdresDisplay(false);

            setKjDisplay(false);
            setAktivDisplay(true);

            if (previousActiveUtvalgImagePath !== undefined) {
              if (previousActivUtvalg?.utvalgId.toString() !== id) {
                let tempPath;
                if (previousActiveUtvalgImagePath > 36) {
                  tempPath =
                    previousActiveUtvalgImagePath % 36 === 0
                      ? 36
                      : previousActiveUtvalgImagePath % 36;
                } else {
                  tempPath = previousActiveUtvalgImagePath;
                }

                let preImageCode = tempPath - 1;
                zoomToSelection(
                  previousActivUtvalgReolIds,
                  ColorCodes()[preImageCode]
                );
              } else {
                //await delay(5000);
                if (commonSelections.filteredCommonItems.length > 0) {
                  setloading(true);
                  zoomToDoubleCoverage(
                    commonSelections.filteredCommonItems,
                    "rgba(0, 255, 0, 0.80)",
                    false
                  );
                } else {
                  setloading(false);
                }
              }
            } else {
              //await delay(5000);
              if (commonSelections.filteredCommonItems.length > 0) {
                setloading(true);
                zoomToDoubleCoverage(
                  commonSelections.filteredCommonItems,
                  "rgba(0, 255, 0, 0.80)",
                  false
                );
              } else {
                setloading(false);
              }
            }
          } else {
            let obj = await CreateActiveUtvalg(data);

            await setActivUtvalg(obj);

            setutvalglistcheck(false);

            setissave(true);

            setvalue(false);
            setDetails(false);
            // setAktivDisplay(false);

            setDemografieDisplay(false);
            setSegmenterDisplay(false);
            setAddresslisteDisplay(false);
            setGeografiDisplay(false);

            setAdresDisplay(false);

            setKjDisplay(false);
            setAktivDisplay(true);

            setloading(false);
          }
        }
      } catch (error) {
        //api.logger.error("errorpage API not working");
        //api.logger.error("error : " + error);
      }
    } else {
      setloading(true);
      setutvalglistcheck(true);
      //setActivUtvalg({});
      url =
        url + `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${id}`;
      // `UtvalgList/GetUtvalgList?listId=${id}&getParentList=${true}&getMemberUtvalg=${true}`;
      try {
        //api.logger.info("APIURL", url);
        const { data, status } = await api.getdata(url);
        if (data.length === 0) {
          //api.logger.error("Error : No Data is present for mentioned Id" + id);
        } else {
          await setActivUtvalglist({});
          let obj = await CreateUtvalglist(data);
          await setActivUtvalglist(obj);
          setutvalglistcheck(true);
          setvalue(false);

          setDetails(false);
          // setAktivDisplay(false);

          setDemografieDisplay(false);
          setSegmenterDisplay(false);
          setAddresslisteDisplay(false);
          setGeografiDisplay(false);
          setKjDisplay(false);
          setAdresDisplay(false);
          setissave(true);
          setAktivDisplay(true);
          setloading(false);
        }
      } catch (error) {
        //api.logger.error("errorpage API not working");
        //api.logger.error("error : " + error);
      }
    }

    window.scrollTo(0, 0);
  };

  const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

  const addSelectionsToList = async () => {
    let flag = false;
    basicSelectionFlag = false;
    newList = false;
    let isBasedOnFlag = false;
    let orderStatusFlag = false;
    checkedList.map((item) => {
      if (
        item.listId !== 0 &&
        item.listId !== undefined &&
        item.listId !== "" &&
        item.listId !== "0"
      ) {
        flag = true;
      } else if (item?.ordreType === 1) {
        orderStatusFlag = true;
      } else if (item?.basedOn) {
        isBasedOnFlag = true;
      }
      if (checkedList[0].isBasis) {
        newList = true;
      } else {
        newList = false;
      }

      setSelectedBasicSelection(newList);
      if (checkedList.length > 1) {
        if (newList !== item.isBasis && basicSelectionFlag === false) {
          basicSelectionFlag = true;
        }
      }
    });

    if (flag) {
      let msg =
        "Kan ikke koble valgte utvalg til liste fordi minst ett av utvalgene allerede er tilknyttet en utvalgsliste.";
      $(".modal").remove();
      $(".modal-backdrop").remove();
      Swal.fire({
        text: msg,
        confirmButtonColor: "#7bc144",
        confirmButtonText: "Lukk",
      });
    } else if (orderStatusFlag) {
      let msg =
        "Du forsøker nå å koble utvalg til en liste i ordre. Annuller eller lås opp ordre før endring.";
      $(".modal").remove();
      $(".modal-backdrop").remove();
      Swal.fire({
        text: msg,
        confirmButtonColor: "#7bc144",
        confirmButtonText: "Lukk",
      });
    } else if (basicSelectionFlag) {
      let msg =
        "Det er ikke lov å legge basisutvalg og andre utvalg til i samme utvalgsliste.";
      $(".modal").remove();
      $(".modal-backdrop").remove();
      Swal.fire({
        text: msg,
        confirmButtonColor: "#7bc144",
        confirmButtonText: "Lukk",
      });
    } else if (isBasedOnFlag) {
      let msg = "Kampanje kan ikke kobles til utvalg eller liste.";
      $(".modal").remove();
      $(".modal-backdrop").remove();
      Swal.fire({
        text: msg,
        confirmButtonColor: "#7bc144",
        confirmButtonText: "Lukk",
      });
    } else {
      setaddSelection("newSelectionAdd");
    }
  };
  const imgLoader = (path) => {
    return require("../../assets/images/Icons/" + path);
  };

  const handleExpandCollapse = (e) => {
    if (!expandListId.includes(e.target.id)) {
      setExpandListId([...expandListId, e.target.id]);
      document.getElementById(e.target.id).classList.remove("collapseworklist");
      document.getElementById(e.target.id).classList.add("expandworklist");
    } else {
      let temp = expandListId.filter((x) => x !== e.target.id);
      setExpandListId(temp);
      document.getElementById(e.target.id).classList.remove("expandworklist");
      document.getElementById(e.target.id).classList.add("collapseworklist");
    }
  };

  const colorImageRenderFunction = (item) => {
    if (item?.utvalgId) {
      newText = newText + 1;
      item["imagePath"] = newText;
    }

    return (
      <div className="col-2 UtvalgslisterText">
        {item?.utvalgId ? (
          <div className="AktivtUtvalg ml-4">
            <img
              id="uxShowUtvalgListDetails_uxHandlekurv_uxGrdMemberUtv_ctl02_uxKartSymbol"
              src={imagePath(newText)}
              className="imgstyle"
            />
            {/* <span className=""></span> */}
          </div>
        ) : null}
      </div>
    );
  };

  const onAddtolist = (values) => {
    let checkedListCopy = JSON.parse(JSON.stringify(checkedList));
    if (values?.length) {
      values?.map((items) => {
        let arr = checkedListCopy.filter((val) => {
          return items?.utvalgId !== val?.utvalgId;
        });
        checkedListCopy = arr;
      });
      setCheckedList(checkedListCopy);
    }
  };

  const newRenderFunction = (item) => {
    let Image = "";
    if (!item.utvalgId) {
      if (item.basedOn > 0) {
        Image = GetImageUrl(
          "kampanjeliste",
          item.isBasis,
          false,
          item.ordreType
        );
      } else {
        Image = GetImageUrl(
          "utvalgsliste",
          item.isBasis,
          false,
          item.ordreType
        );
      }
    } else {
      let list =
        item.listId !== 0 &&
        item.listId !== undefined &&
        item.listId !== "" &&
        item.listId !== "0"
          ? true
          : false;
      // let list = !item.listId ? false : true;
      if (item.basedOn > 0) {
        Image = GetImageUrl("kampanje", item.isBasis, list, item.ordreType);
      } else {
        Image = GetImageUrl("utvalg", item.isBasis, list, item.ordreType);
      }
    }

    return <img className="mb-1" src={imgLoader(Image)} />;
  };

  const UncheckSelections = (Item) => {
    let checked = checkedList;
    if (Item.memberLists?.length > 0) {
      Item.memberLists?.map((childList) => {
        checked = checked?.filter((ch) => {
          return ch?.listId !== childList?.listId;
        });
        childList?.memberUtvalgs?.map((utvalgs) => {
          checked = checked?.filter((ch) => {
            return ch?.utvalgId !== utvalgs?.utvalgId;
          });
        });
      });
    }
    if (Item.memberUtvalgs?.length > 0) {
      Item?.memberUtvalgs?.map((utvalgs) => {
        checked = checked?.filter((ch) => {
          return ch?.utvalgId !== utvalgs?.utvalgId;
        });
      });
    }
    setCheckedList(checked);
  };

  const renderPerson = (args) => {
    return args.map((item, index) => (
      <div key={index}>
        {refresh ? null : (
          <div
            key={index}
            className={
              (Object.keys(activUtvalg).length !== 0 &&
                item.utvalgId === activUtvalg.utvalgId) ||
              (Object.keys(activUtvalglist).length !== 0 &&
                item.listId === activUtvalglist.listId)
                ? "row  avan greenBackGround"
                : "row  avan"
            }
          >
            <div className="col-2 UtvalgslisterText">
              {item.modifications.length > 0
                ? FormatDate(item.modifications[0].modificationTime)
                : null}
            </div>
            <div className="col-6 UtvalgslisterText" key={index}>
              <input
                className=""
                type="checkbox"
                // value=""
                defaultChecked={
                  checkedList.filter((i) => {
                    if (i.utvalgId === undefined || i.utvalgId === 0) {
                      return i.listId === item.listId;
                    } else {
                      return i.utvalgId === item.utvalgId;
                    }
                  }).length
                    ? true
                    : false

                  // checkedList.filter((i) => i.utvalgId === item.utvalgId).length
                  //   ? true
                  //   : false
                }
                id={!item.utvalgId ? "L" + item.listId : "U" + item.utvalgId}
                onChange={(e) => {
                  handleChange(e, item, index + 1);
                }}
              />
              {!item.utvalgId && item.basedOn < 1 ? (
                <button
                  id={item.listId}
                  className={
                    // item?.listId?.toString() ===
                    //   activUtvalg?.listId?.toString() ||
                    expandListId.includes(item?.listId.toString())
                      ? "mb-1 pl-1 pr-1 expandworklist" //minus icon
                      : "mb-1 pl-1 pr-1 collapseworklist" //plus icon
                  }
                  onClick={(e) => {
                    handleExpandCollapse(e);
                    if (expandListId.includes(item?.listId.toString())) {
                      UncheckSelections(item);
                    }
                  }}
                />
              ) : null}

              {newRenderFunction(item)}
              <span
                id={!item.utvalgId ? "L" + item.listId : "U" + item.utvalgId}
                onClick={openWorklist}
                className="pl-1 cursorpointer"
              >
                {item.name}
              </span>
            </div>
            <div className="col-1 UtvalgslisterText text-right">
              {item.antallBeforeRecreation
                ? NumberFormat(item.antallBeforeRecreation)
                : " "}
            </div>
            <div className="col-1 UtvalgslisterText text-right">
              {NumberFormat(item.totalAntall || item.antall)}
            </div>

            {colorImageRenderFunction(item)}
          </div>
        )}
        {expandListId.includes(`${item.listId}`)
          ? item.memberLists
            ? item.basedOn > 0
              ? null
              : renderPerson(item.memberLists)
            : null
          : null}
        {expandListId.includes(`${item.listId}`)
          ? item.memberUtvalgs
            ? item.basedOn > 0
              ? null
              : renderPerson(item.memberUtvalgs)
            : null
          : null}
      </div>
    ));
  };

  return (
    <div>
      <div>{loading ? <Spinner /> : null}</div>
      {Modal ? (
        <div
          className="modal fade bd-example-modal-lg"
          id="uxBtnLukkAndLukkalle"
          tabIndex="-1"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
          aria-hidden="true"
        >
          <div className="modal-dialog" role="document">
            <div className="modal-content">
              <div className="modal-header segFord">
                <h5 className="modal-title " id="exampleModalLongTitle">
                  Advarsel
                </h5>
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
                          &nbsp; Skal endringer lagres før utvalg/utvalglister
                          lukkes?
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
                            Avbryt
                          </button>
                          <button
                            type="button"
                            className="modalMessage_button ml-5"
                            data-dismiss="modal"
                            onClick={neiClick}
                          >
                            Nei
                          </button>
                          <button
                            type="button"
                            onClick={SaveUtvalgButton}
                            className="modalMessage_button ml-5"
                            data-dismiss="modal"
                            data-target="#kvittering"
                          >
                            Ja
                          </button>
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      ) : null}

      <div>
        {addSelection === "newSelectionAdd" ? (
          <ConnectSelectiontolist
            id={"newKobletil"}
            name={"KOBLE UTVALG TIL LISTE"}
            list={checkedList}
            basicListFlag={selectedBasicSelection}
            parentCallback={onAddtolist}
          />
        ) : null}
        <div className="row">
          <div className="col-12">
            <p className="avan pt-1">Arbeidsliste</p>
          </div>
        </div>
        <div className="row">
          <div className="col">
            <input
              type="submit"
              className="KSPU_button mr-1"
              ref={VisKart}
              value="Vis i kart"
              onClick={showToMap}
              disabled={
                checkedList.filter((item) => {
                  return item?.utvalgId === undefined || item?.utvalgId === 0;
                }).length > 0 || checkedList?.length === 0
                  ? true
                  : false
              }
            />
            <input
              type="submit"
              className="KSPU_button"
              ref={Lukk}
              value="Lukk"
              data-toggle="modal"
              data-target="#uxBtnLukkAndLukkalle"
              onClick={workListLukk}
              disabled={btnDisabled}
            />
          </div>
          <div className="col-lg-6 col-md-12 _flex-end">
            <input
              type="submit"
              className="KSPU_button mr-1"
              value="Koble til utvalgsliste"
              data-toggle="modal"
              data-target="#newKobletil"
              disabled={btnDisabled}
              onClick={addSelectionsToList}
            />

            <input
              type="submit"
              className="KSPU_button mr-1"
              ref={Fjern}
              value="Fjern fra utvalgsliste"
              onClick={decoupledList}
              disabled={btnDisabled}
            />
            <input
              type="submit"
              className="KSPU_button mr-1"
              ref={Lukkalle}
              value="Lukk alle"
              data-toggle="modal"
              data-target="#uxBtnLukkAndLukkalle"
              onClick={worklistLukkAll}
              //disabled={btnDisabled}
            />
          </div>
        </div>

        <div className="row pt-2">
          <div className="col">
            <p className="KSPU_LinkButton float-left mr-3" onClick={selectAll}>
              Velg alle
            </p>
            <p
              className="KSPU_LinkButton float-left mr-3"
              onClick={deSelectAll}
            >
              Velg ingen
            </p>
          </div>

          <div className="col-12">
            <p className="KSPU_LinkButton float-right mr-1"> Fjern fra kart</p>
          </div>
        </div>
        {showorklist.length !== 0 ? (
          <div>
            <div className="row  avan pt-1 pb-1">
              <div className="col-2">Endret</div>
              <div className="col-6">Utvalg / Utvalgslister</div>
              <div className="col-1 text-right pr-3">Før</div>
              <div className="col-1 text-right pr-3">Nå</div>
              <div className="col-2">I kart</div>
            </div>
            {renderPerson(showorklist)}
          </div>
        ) : (
          <div className="Sok-header">
            Ingen åpne utvalg eller utvalgslister
          </div>
        )}
      </div>
    </div>
  );
}

export default Arbeidsliste;
