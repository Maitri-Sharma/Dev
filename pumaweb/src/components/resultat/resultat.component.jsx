import React, { useContext, useState, useEffect } from "react";
import "../../App.css";
import expand from "../../assets/images/esri/expand.png";
import collapse from "../../assets/images/esri/collapse.png";
import ModelComponent from "../ResultantModel";
import SegmentModel from "../SegmentModel.js";

import { KSPUContext } from "../../context/Context.js";
import { getAntall } from "../KspuConfig";
import TableNew from "../TableNew.js";
import api from "../../services/api.js";

import DemografyModel from "../DemografyModel";
import { NumberFormat, filterCommonReolIds } from "../../common/Functions";

import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";

import Spinner from "../../components/spinner/spinner.component";

import { MainPageContext } from "../../context/Context.js";

function Resultat(props, { parentCallback }) {
  const [togglevalue, settogglevalue] = useState(false);
  const {
    resultData,
    setResultData,
    isWidgetActive,
    setIsWidgetActive,
    rutefoshkerVisited,
    setRutefoshkerVisited,
    rutefoshkerPreviousSelectedRutes,
    setrutefoshkerPreviousSelectedRutes,
    setCheckedList,
    checkedList,
  } = useContext(KSPUContext);
  const { searchURL } = useContext(KSPUContext);
  const [resultdatanew, setresultdatanew] = useState([]);
  const [outputData, setOutputData] = useState([]);
  const [previousKeys, setPreviousKeys] = useState([]);

  const [ModelName, setModelName] = useState(" ");
  const [segmentModelName, setSegmentModelName] = useState(" ");
  const [demografyModelName, setDemografyModelName] = useState("");
  const { showBusiness, setShowBusiness, showHousehold, setShowHousehold } =
    useContext(KSPUContext);
  const { selectedsegment, setselectedsegment } = useContext(KSPUContext);
  const { selecteddemografiecheckbox, setselecteddemografiecheckbox } =
    useContext(KSPUContext);
  const {
    maintainUnsavedRute,
    setMaintainUnsavedRute,
    SelectionUpdate,
    setSelectionUpdate,
  } = useContext(KSPUContext);

  const [test, setTest] = useState(false);
  const [copyresult, setcopyresult] = useState([]);
  const [display, setDisplay] = useState(false);

  const { mapView } = useContext(MainPageContext);
  const [loading, setloading] = useState(false);
  const [selectedRows, setSelectedRows] = useState([]);
  const { showReservedHouseHolds, setShowReservedHouseHolds } =
    useContext(KSPUContext);
  const { activUtvalg, setActivUtvalg } = useContext(KSPUContext);
  const [TotalAntal, setTotalAntall] = useState(props.Totalantallvalue);
  const [households, setHouseholds] = useState([]);
  const [householdsReserved, setHouseholdsReserved] = useState([]);
  const [disableZoom, setDisableZoom] = useState(false);

  const callback = (selectedrecord, selected, selectedRows) => {
    if (selectedrecord[0].children.length === 0) {
      setTest(true);
      setcopyresult(selectedrecord);
      setDisplay(true);
    }
    setTest(true);
    setcopyresult(selectedrecord);
  };

  useEffect(() => {
    let selectedKeys = [];
    outputData.map((item, i) => {
      if (!item.children) {
        selectedKeys.push(item.key);
      }
    });

    if (outputData.length !== 0) {
      setSelectionUpdate(false);
      updateActiveUtvalg(outputData);
    }
  }, [outputData]);

  const updateActiveUtvalg = async (selectedDataSet) => {
    let selectedRoutes = await getSelectedRoutes(selectedDataSet);
    let selectedReolIds = [];
    selectedRoutes.map((item, i) => {
      selectedReolIds.push(item.key.toString());
    });

    let url = "Reol/GetReolsFromReolIDs?";

    try {
      const { data, status } = await api.postdata(url, selectedReolIds);

      if (status === 200) {
        if (data.length > 0) {
          let antall = getAntall(selectedRoutes);
          if (activUtvalg.receivers.length !== 0) {
            activUtvalg.receivers.map((item) => {
              if (showHousehold) {
                if (item.receiverId !== 1) {
                  activUtvalg.receivers.push({ receiverId: 1, selected: true });
                }
              } else {
                let temp = activUtvalg;
                temp = temp.receivers.filter((result) => {
                  return result.receiverId !== 1;
                });
                activUtvalg.receivers = temp;
              }
              if (showBusiness) {
                if (item.receiverId !== 4) {
                  activUtvalg.receivers.push({ receiverId: 4, selected: true });
                }
              } else {
                let temp = activUtvalg;
                temp = temp.receivers.filter((result) => {
                  return result.receiverId !== 4;
                });
                activUtvalg.receivers = temp;
              }
              if (showReservedHouseHolds) {
                if (item.receiverId !== 5) {
                  activUtvalg.hasReservedReceivers = true;
                  activUtvalg.receivers.push({ receiverId: 5, selected: true });
                }
              } else {
                let temp = activUtvalg;
                temp = temp.receivers.filter((result) => {
                  return result.receiverId !== 5;
                });
                activUtvalg.receivers = temp;
              }
            });
          } else {
            if (showHousehold) {
              activUtvalg.receivers.push({ receiverId: 1, selected: true });
            }
            if (showBusiness) {
              activUtvalg.receivers.push({ receiverId: 4, selected: true });
            }
            if (showReservedHouseHolds) {
              activUtvalg.hasReservedReceivers = true;
              activUtvalg.receivers.push({ receiverId: 5, selected: true });
            }
          }

          activUtvalg.reoler = data;
          activUtvalg.totalAntall =
            (showHousehold ? antall[0] : 0) +
            (showBusiness ? antall[1] : 0) +
            (showReservedHouseHolds ? antall[2] : 0);
          activUtvalg.Antall = antall;
          activUtvalg.hush = antall[0];
          setActivUtvalg(activUtvalg);

          setSelectionUpdate(true);
        }
      }
    } catch (error) {
      console.error("er : " + error);
    }
  };

  const callbackReoler = (SelectedRecords, selected, selectedRows) => {
    let houseValue = households;
    if (!selected && houseValue.includes(SelectedRecords.house)) {
      houseValue = houseValue.filter((item) => {
        return item !== SelectedRecords.house;
      });
    } else {
      houseValue.push(SelectedRecords.house);
    }
    setHouseholds(houseValue);
    let reservValue = householdsReserved;
    if (!selected && reservValue.includes(SelectedRecords.householdsReserved)) {
      reservValue = reservValue.filter((item) => {
        return item !== SelectedRecords.householdsReserved;
      });
    } else {
      reservValue.push(SelectedRecords.householdsReserved);
    }
    setHouseholdsReserved(reservValue);
  };

  const getSelectedRoutes = (data) => {
    let selectedArray = [];
    let selectedRoutes = data.reduce((acc, dt) => {
      if (dt.cat === "rute") {
        selectedArray.push(dt.key);
        return acc.concat(dt);
      }
      return acc;
    }, []);
    return selectedRoutes;
  };

  // useEffect(async () => {
  //   await setResultData(
  //     await groupBy(
  //       activUtvalg.reoler,
  //       "",
  //       0,
  //       showHousehold,
  //       showBusiness,
  //       showReservedHouseHolds,
  //       []
  //     )
  //   );
  // }, [showBusiness, showReservedHouseHolds, showHousehold]);

  // useEffect(async () => {
  //   let result = await groupBy(
  //     activUtvalg.reoler,
  //     "",
  //     0,
  //     showHousehold,
  //     showBusiness,
  //     showReservedHouseHolds,
  //     []
  //   );
  //   await setResultData(result); //JSON roeler needs to pass
  //   // let keys = getChildrenRoute(result);
  //   // setSelectedRows(keys);
  //   // setresultdatanew(keys);
  // }, [showReservedHouseHolds]);

  // useEffect(async () => {
  //   let result = await groupBy(
  //     activUtvalg.reoler,
  //     "",
  //     0,
  //     showHousehold,
  //     showBusiness,
  //     showReservedHouseHolds,
  //     []
  //   );
  //   await setResultData(result);
  //   // let keys = getChildrenRoute(result);
  //   // setSelectedRows(keys);
  //   // setresultdatanew(keys);
  // }, [showHousehold]);

  const showModel = () => {
    setModelName("ViewMaximizer");
  };

  const showSegmentModel = () => {
    setSegmentModelName("SegmentForDeling");
  };

  const showDemographiModel = () => {
    setDemografyModelName("DemografikeForDeling");
  };

  useEffect(() => {
    //excute when first time component load
    let keys = getChildrenRoute(resultData);
    setSelectedRows(keys);
    setresultdatanew(keys);
  }, []);

  useEffect(() => {
    //excute when add rute on map from add rute widget
    let keys = getChildrenRoute(resultData);
    setSelectedRows(keys);
  }, [resultData]);

  useEffect(() => {
    // This function fires each time a layer view is created htmlFor a layer in
    // the map of the view

    if (!isWidgetActive) {
      if (mapView.map.allLayers.items.length < 2) {
        mapView.on("layerview-create", (event) => {
          if (event.layer.url !== null) {
            // The LayerView for the desired layer
            handleShowSelectedRute();
          }
        });
      } else {
        handleShowSelectedRute();
      }
    }
  }, [resultdatanew]);

  const getChildrenRoute = (data) => {
    let selectedRoutekeys = data.reduce((acc, dt) => {
      if (dt.children !== undefined && dt.children !== null) {
        return acc.concat(getChildrenRoute(dt.children));
      }
      return acc.concat(dt.key);
    }, []);
    return selectedRoutekeys;
  };

  const showRute = async () => {
    let previousKeysToCompare;
    let selectedKeys = [];
    setDisableZoom(false);
    outputData.map((item, i) => {
      if (!item.children) {
        selectedKeys.push(item.key);
      }
    });

    if (previousKeys.length === 0) {
      previousKeysToCompare = previousKeys;
      previousKeysToCompare = await getChildrenRoute(resultData);
      await setPreviousKeys(previousKeysToCompare);
    } else {
      previousKeysToCompare = previousKeys;
      await setPreviousKeys(selectedKeys);
    }

    let resultantGraphicsDifference = [];

    if (selectedKeys.length !== 0) {
      if (previousKeysToCompare.length > selectedKeys.length) {
        resultantGraphicsDifference = previousKeysToCompare.filter(
          (d) => !selectedKeys.includes(d)
        );

        let j = mapView.graphics.items.length;
        for (let i = j; i > 0; i--) {
          if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
            if (
              resultantGraphicsDifference.includes(
                parseInt(mapView.graphics.items[i - 1].attributes.reol_id)
              )
            ) {
              mapView.graphics.remove(mapView.graphics.items[i - 1]);
            }
          }
        }
      } else {
        let allDisplayedReolID = [];

        mapView.graphics.items.map((graphicElement) => {
          if (
            graphicElement.attributes !== undefined &&
            graphicElement.attributes !== null
          ) {
            if (graphicElement.attributes.reol_id !== undefined) {
              allDisplayedReolID.push(
                parseInt(graphicElement.attributes.reol_id)
              );
            }
          }
        });

        selectedKeys.map((key) => {
          if (!allDisplayedReolID.includes(key)) {
            resultantGraphicsDifference.push(key);
          }
        });
        setDisableZoom(true);
        setresultdatanew(resultantGraphicsDifference);
      }
    }
  };

  const handleShowSelectedRute = async () => {
    if (resultdatanew.length > 0 && resultdatanew[0] !== undefined) {
      let dataItemDiffer = [];
      setloading(true);
      if (rutefoshkerVisited) {
        if (!(parseInt(activUtvalg?.listId) === 0)) {
          let currentSelectedRute = [];
          let dataItems = [];

          if (activUtvalg !== undefined) {
            activUtvalg.reoler.forEach(function (reolItem) {
              currentSelectedRute.push(reolItem.reolId);
            });
          }

          if (
            rutefoshkerPreviousSelectedRutes.length < currentSelectedRute.length
          ) {
            dataItemDiffer = currentSelectedRute.filter(
              (d) => !rutefoshkerPreviousSelectedRutes.includes(d)
            );
            dataItems = resultdatanew;
          } else if (
            rutefoshkerPreviousSelectedRutes.length > currentSelectedRute.length
          ) {
            dataItemDiffer = rutefoshkerPreviousSelectedRutes.filter(
              (d) => !currentSelectedRute.includes(d)
            );

            dataItems = [...resultdatanew];
            dataItems = dataItems.concat(dataItemDiffer);
          } else {
            dataItemDiffer = rutefoshkerPreviousSelectedRutes.filter(
              (d) => !currentSelectedRute.includes(d)
            );
            dataItems = [...resultdatanew];
            dataItems = dataItems.concat(dataItemDiffer);
          }

          rutefoshkerPreviousSelectedRutes.splice(
            0,
            rutefoshkerPreviousSelectedRutes.length
          );

          dataItems.forEach(function (reolItem) {
            let j = mapView.graphics.items.length;
            for (let i = j; i > 0; i--) {
              if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
                if (
                  mapView.graphics.items[i - 1].attributes.reol_id ===
                  reolItem.toString()
                ) {
                  if (
                    Object.entries(
                      mapView.graphics.items[i - 1].symbol.color
                    ).toString() ===
                      Object.entries({
                        r: 237,
                        g: 54,
                        b: 21,
                        a: 0.25,
                      }).toString() ||
                    Object.entries(
                      mapView.graphics.items[i - 1].symbol.color
                    ).toString() ===
                      Object.entries({
                        r: 0,
                        g: 255,
                        b: 0,
                        a: 0.8,
                      }).toString()
                  ) {
                    mapView.graphics.remove(mapView.graphics.items[i - 1]);
                  }
                }
              }
            }
          });
        } else {
          let j = mapView.graphics.items.length;
          for (let i = j; i > 0; i--) {
            if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
              mapView.graphics.remove(mapView.graphics.items[i - 1]);
            }
          }
        }
      }

      let r = resultdatanew.map((element) => "'" + element + "'").join(",");
      let reolsWhereClause = `reol_id in (${r})`;
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

      if (!disableZoom) {
        const queryExtent = new Query();
        queryExtent.where = `${reolsWhereClause}`;
        let resultsExtent = await query.executeForExtent(
          BudruterUrl,
          queryExtent
        );

        mapView.goTo(resultsExtent);
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

        queryResults.outFields = "reol_id";
        queryResults.where = "OBJECTID IN (" + objectsIds.join(",") + ")";
        queryResults.outSpatialReference = mapView.spatialReference;
        queryResults.returnGeometry = true;

        promise[i] = query.executeQueryJSON(BudruterUrl, queryResults);
      }

      Promise.all(promise).then((values) => {
        for (let i = 0; i < values.length; i++) {
          for (let j = 0; j < values[i].features.length; j++) {
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

            let graphic = new Graphic(
              values[i].features[j].geometry,
              selectedSymbol,
              values[i].features[j].attributes
            );
            mapView.graphics.add(graphic);
          }
        }

        setloading(false);
      });

      if (rutefoshkerVisited) {
        // if (maintainUnsavedRute?.length !== 0) {
        let commonSelections = filterCommonReolIds(checkedList);
        let checkedListActiveUtvalReolerItem = [];
        if (activUtvalg !== undefined) {
          activUtvalg.reoler.forEach(function (reolItem) {
            checkedListActiveUtvalReolerItem.push(reolItem.reolId);
          });
        }

        if (commonSelections.filteredCommonItems.length > 0) {
          let selectedDoubleCoverageItems = [];
          commonSelections.filteredCommonItems.map((item) => {
            if (checkedListActiveUtvalReolerItem.includes(item)) {
              selectedDoubleCoverageItems.push(item);
            }
          });

          setloading(true);
          zoomToDoubleCoverage(
            selectedDoubleCoverageItems,
            "rgba(0, 255, 0, 0.80)",
            false
          );
        }
        //}
      }

      setRutefoshkerVisited(false);
    }
  };

  const zoomToDoubleCoverage = async (Reolids, colorcode, enableZoom) => {
    if (Reolids.length > 0) {
      setloading(true);
      let k = Reolids.map((element) => "'" + element + "'").join(",");
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
        for (let i = 0; i < values.length; i++) {
          for (let j = 0; j < values[i].features.length; j++) {
            let selectedSymbol = {};

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
            let graphic = new Graphic(
              values[i].features[j].geometry,
              selectedSymbol,
              values[i].features[j].attributes
            );
            mapView.graphics.add(graphic);
          }
        }
        setloading(false);
      });
    }
  };

  const toggle = () => {
    settogglevalue(!togglevalue);
  };

  const gettitle = (url) => {
    if (url.includes("Fylke")) {
      return "Fylke\\Kommune\\Team\\Rute";
    } else if (url.includes("Kommune")) {
      return "Kommune\\Team\\Rute";
    } else if (url.includes("Team")) {
      return "Team\\Rute";
    } else {
      return "Fylke\\Kommune\\Team\\Rute";
    }
  };

  const columns = [
    {
      title: gettitle(searchURL),
      dataIndex: "name",
      key: "key",
      // sorter: (a, b) => {
      //   return;
      // },
      // sortOrder: "ascend",
      // sortDirections: ["ASC", "DESC"],
    },
    {
      title: "Antall",
      dataIndex: "total",
      key: "total",
      align: "right",
      render: (total) => NumberFormat(total),
    },
  ];

  return (
    <div>
      <div>{loading ? <Spinner /> : null}</div>
      <div className="card Kj-background-color mr-1 mt-1">
        {ModelName === "ViewMaximizer" ? (
          <ModelComponent
            title={"RESULTATDETALJER"}
            id={"maximizer"}
            dataResult={resultData}
          />
        ) : null}
        {demografyModelName === "DemografikeForDeling" ? (
          <DemografyModel
            title={"INDEKSVERDIER"}
            id={"demografyfordeling"}
            dataResult={resultData}
            childrenData={resultData[0].children}
            parentCallback={callback}
          />
        ) : null}
        {segmentModelName === "SegmentForDeling" ? (
          <SegmentModel
            title={"SEGMENTFORDELING"}
            id={"segmentfordeling"}
            dataResult={resultData}
            childrenData={resultData[0].children}
            parentCallback={callback}
          />
        ) : null}
        <div className="row">
          <div className="col-8">
            <p className="avan p-1">RESULTAT</p>
          </div>
          <div className="col-4">
            {!togglevalue ? (
              <img
                className="d-flex float-right pt-1 mr-1"
                src={collapse}
                onClick={toggle}
              />
            ) : (
              <img
                className="d-flex float-right pt-1 mr-1"
                src={expand}
                onClick={toggle}
              />
            )}
          </div>
        </div>

        {!togglevalue ? (
          <div className="Kj-div-background-color pt-2 pb-2 defaultwrap">
            <div className="mb-2 mr-1 ml-1">
              <a
                id="uxShowUtvalgDetails_uxUtvalgReoler_uxPnlTree_uxShowDetails"
                href=""
                className="KSPU_LinkButton float-right mr-1"
                data-toggle="modal"
                data-target="#maximizer"
                onClick={showModel}
              >
                Maksimer
              </a>

              <div>
                {test ? (
                  display ? (
                    <p className="sok-Alert-text">
                      Utvalget inneholder enten ingen budruter eller ingen
                      mottakergrupper.
                    </p>
                  ) : (
                    <TableNew
                      width1={""}
                      columnsArray={columns}
                      data={copyresult}
                      setoutputDataList={setOutputData}
                      defaultSelectedColumn={[]}
                      setSelectedRows={[]}
                      hideselection={1}
                    />
                  )
                ) : (
                  <TableNew
                    width1={""}
                    columnsArray={columns}
                    data={resultData}
                    setoutputDataList={setOutputData}
                    setSelectedRows={setSelectedRows}
                    defaultSelectedColumn={selectedRows}
                    hideselection={0}
                    parentCallback={callbackReoler}
                  />
                )}
              </div>
              {selectedsegment.length > 0 ? (
                <div className=" float-left mr-1">
                  <a
                    id="uxShowUtvalgDetails_uxUtvalgReoler_uxPnlTree_uxShowDetails"
                    href=""
                    className="KSPU_LinkButton float-right mr-1"
                    data-toggle="modal"
                    data-target="#segmentfordeling"
                    onClick={showSegmentModel}
                  >
                    Vis segmentfordeling
                  </a>
                </div>
              ) : null}
              {selecteddemografiecheckbox.length > 0 ? (
                <div className=" float-left mr-1">
                  <a
                    id="uxShowUtvalgDetails_uxUtvalgReoler_uxPnlTree_uxShowDetails"
                    href=""
                    className="KSPU_LinkButton float-right mr-1"
                    data-toggle="modal"
                    data-target="#demografyfordeling"
                    onClick={showDemographiModel}
                  >
                    Vis indeksverdier
                  </a>
                </div>
              ) : null}
              <div className=" float-right mr-1">
                <input
                  type="button"
                  id="uxBtnLagre"
                  className="KSPU_button mr-1"
                  value="Oppdater kart"
                  onClick={showRute}
                />
              </div>
            </div>
          </div>
        ) : null}
      </div>
    </div>
  );
}

export default Resultat;
