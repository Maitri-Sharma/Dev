import React, { useContext, useEffect, useRef, useState } from "react";
import "../App.css";
import MottakerComponent from "./Mottakergrupper";
import Information from "./Information";
import TableNew from "./TableNew.js";
import api from "../services/api.js";
import { GetData, groupBy } from "../Data";
import { KSPUContext, MainPageContext } from "../context/Context.js";
import { criterias, getAntall, NewUtvalgName, Utvalg } from "./KspuConfig";
import Spinner from "../components/spinner/spinner.component";
import { File } from "better-xlsx";
import { saveAs } from "file-saver";
import {
  CurrentDate,
  filterCommonReolIds,
  NumberFormat,
} from "../common/Functions";

function Rute() {
  const [vistogglevalue, setvistogglevalue] = useState(false);
  const [initialListValue, setInitialValue] = useState(
    "Det er ingen aktive utvalg"
  );
  const [TotalValue, settotalValue] = useState(0);
  const [HusstandValue, setHusstandValue] = useState();
  const [detailText, setDetailText] = useState("");
  const [datalist, setData] = useState([]);
  const [outputData, setOutputData] = useState([]);
  const btnSaveSom = useRef(null);
  const btnSave = useRef(null);
  const [loading, setloading] = useState(false);
  const {
    showHousehold,
    setShowHousehold,
    showBusiness,
    setShowBusiness,
    setshoworklist,
    showorklist,
    setCheckedList,
    checkedList,
    rutefoshkerVisited,
    setRutefoshkerVisited,
    rutefoshkerPreviousSelectedRutes,
    setrutefoshkerPreviousSelectedRutes,
    maintainUnsavedRute,
    setMaintainUnsavedRute,
  } = useContext(KSPUContext);
  const { showDenking, setShowDenking } = useContext(KSPUContext);
  const { showReserverte, setShowReserverte } = useContext(KSPUContext);
  const { showReservedHouseHolds, setShowReservedHouseHolds } =
    useContext(KSPUContext);
  const { resultData, setResultData } = useContext(KSPUContext);
  const {
    activUtvalg,
    setActivUtvalg,
    setAktivDisplay,
    setRuteDisplay,
    setissave,
    setSave,
    setMapDisplay,
    setActivUtvalglist,
    utvalglistcheck,
    setutvalglistcheck,
    setSelectionUpdate,
    setDetails,
  } = useContext(KSPUContext);
  const [selectedKeys, setSelectedKeys] = useState([]);
  const [selectedRows, setSelectedRows] = useState([]);
  const [showmsg, setshowmsg] = useState(false);
  const [displaymsg, setdisplayMsg] = useState("");
  const [households, setHouseholds] = useState([]);
  const [householdsReserved, setHouseholdsReserved] = useState([]);
  const { mapView } = useContext(MainPageContext);
  const [routesData, setroutesData] = useState([]);
  const [reolIDsUpdate, setReolIDsUpdate] = useState(false);
  const routes = (data) => {
    setroutesData(data);
  };

  const ExportExcel = (column, dataSource, fileName) => {
    const file = new File();
    let sheet = file.addSheet("sheet-test");
    let depth = getDepth(column);
    let columnNum = getColumns(column);
    let rowArr = [];
    for (let k = 0; k < depth; k++) {
      rowArr.push(sheet.addRow());
    }
    rowArr.map((ele) => {
      for (let j = 0; j < columnNum; j++) {
        let cell = ele.addCell();
        cell.value = j;
      }
    });
    //  initializes the header
    init(column, 0, 0);
    //  unfold the columns in order
    let columnLineArr = [];
    columnLine(column);
    //  according to the column, the dataSource the data inside is sorted and converted into a two-dimensional array
    let dataSourceArr = [];
    dataSource.map((ele) => {
      let dataTemp = [];
      columnLineArr.map((item) => {
        dataTemp.push({
          [item.dataIndex]: ele[item.dataIndex],
          value: NumberFormat(ele[item.dataIndex]),
        });
      });
      dataSourceArr.push(dataTemp);
    });

    //  drawing table data
    dataSourceArr.forEach((item, index) => {
      // according to the data, create the corresponding number of rows
      let row = sheet.addRow();
      row.setHeightCM(0.8);
      // creates a cell for that number
      item.map((ele) => {
        let cell = row.addCell();
        if (ele.hasOwnProperty("num")) {
          cell.value = index + 1;
        } else {
          cell.value = ele.value;
        }
        cell.style.align.v = "left";
        cell.style.align.h = "right";
      });
    });
    // set the width of each column
    for (var i = 0; i < 4; i++) {
      sheet.col(i).width = 20;
    }
    file.saveAs("blob").then(function (content) {
      saveAs(content, fileName + ".xlsx");
    });

    //  unfold the columns in order
    function columnLine(column) {
      column.map((ele) => {
        if (ele.children === undefined || ele.children.length === 0) {
          columnLineArr.push(ele);
        } else {
          columnLine(ele.children);
        }
      });
    }
    //  initializes the header
    function init(column, rowIndex, columnIndex) {
      column.map((item, index) => {
        let hCell = sheet.cell(rowIndex, columnIndex);
        //  if there are no child elements,   all the columns
        if (item.title === " operation ") {
          hCell.value = "";
        } else if (item.children === undefined || item.children.length === 0) {
          //  add a cell to the first row
          hCell.value = item.title;
          hCell.vMerge = depth - rowIndex - 1;
          hCell.style.align.h = "right";
          //hCell.style.font.color = "fffff8df";
          hCell.style.align.v = "right";
          columnIndex++;
          // rowIndex++
        } else {
          let childrenNum = 0;
          function getColumns(arr) {
            arr.map((ele) => {
              if (ele.children) {
                getColumns(ele.children);
              } else {
                childrenNum++;
              }
            });
          }
          getColumns(item.children);
          hCell.hMerge = childrenNum - 1;
          hCell.value = item.title;
          hCell.style.align.h = "right";
          hCell.style.align.v = "right";
          //hCell.style.font.color = "fffff8df";
          let rowCopy = rowIndex;
          rowCopy++;
          init(item.children, rowCopy, columnIndex);
          //  next cell start
          columnIndex = columnIndex + childrenNum;
        }
      });
    }
    //  gets table head rows
    function getDepth(arr) {
      const eleDepths = [];
      arr.forEach((ele) => {
        let depth = 0;
        if (Array.isArray(ele.children)) {
          depth = getDepth(ele.children);
        }
        eleDepths.push(depth);
      });
      return 1 + max(eleDepths);
    }

    function max(arr) {
      return arr.reduce((accu, curr) => {
        if (curr > accu) return curr;
        return accu;
      });
    }
    //  calculates the number of header columns
    function getColumns(arr) {
      let columnNum = 0;
      arr.map((ele) => {
        if (ele.children) {
          getColumns(ele.children);
        } else {
          columnNum++;
        }
      });
      return columnNum;
    }
  };

  const newSelectionCreation = () => {
    let j = mapView.graphics.items.length;
    for (var i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
      }
    }
    setOutputData([]);
    setSelectedRows([]);
    setActivUtvalg({});
    setutvalglistcheck(false);
    setActivUtvalglist({});
    settotalValue(0);
    setHusstandValue(0);
    setShowReservedHouseHolds(false);
    setShowBusiness(false);
    setShowHousehold(true);
    setInitialValue("Det er ingen aktive utvalg");
    setDetailText(`Hush.: ${0},  Sone 0: ${0},  Sone 1: ${0},  Sone 2: ${0}`);
    setvistogglevalue(false);
    btnSaveSom.current.disabled = true;
    btnSave.current.disabled = true;
  };

  useEffect(() => {
    let data = groupBy(
      routesData,
      "",
      0,
      showHousehold,
      showBusiness,
      showReservedHouseHolds,
      [],
      ""
    );

    setData(data);

    createUtvalgObject();

    if (!showHousehold) {
      columns = columns.filter(function (element) {
        return element.dataIndex !== "house";
      });
    }
    if (!showBusiness) {
      columns = columns.filter(function (element) {
        return element.dataIndex !== "businesses";
      });
    }
    if (!showReservedHouseHolds) {
      columns = columns.filter(function (element) {
        return element.dataIndex !== "householdsReserved";
      });
    }
  }, [
    showReservedHouseHolds,
    showReserverte,
    showDenking,
    showBusiness,
    showHousehold,
  ]);
  const createUtvalgObject = () => {
    let sum = 0;
    let sum0 = 0;
    let sum1 = 0;
    let sum2 = 0;
    outputData.map((item) => {
      if (!item.children) {
        let zone0 = 0;
        let zone1 = 0;
        let zone2 = 0;
        switch (item.prisZone) {
          case 0:
            zone0 =
              (showHousehold ? item.house : 0) +
              (showBusiness ? item.businesses : 0) +
              (showReservedHouseHolds ? item.householdsReserved : 0);
            break;
          case 1:
            zone1 =
              (showHousehold ? item.house : 0) +
              (showBusiness ? item.businesses : 0) +
              (showReservedHouseHolds ? item.householdsReserved : 0);
            break;
          case 2:
            zone2 =
              (showHousehold ? item.house : 0) +
              (showBusiness ? item.businesses : 0) +
              (showReservedHouseHolds ? item.householdsReserved : 0);
            break;
        }
        sum0 = sum0 + zone0;
        sum1 = sum1 + zone1;
        sum2 = sum2 + zone2;
      }
    });
    sum = sum0 + sum1 + sum2;
    settotalValue(sum);
    let totalhouseholds = 0;
    let totalhouseholdsReserved = 0;
    if (Object.keys(activUtvalg).length !== 0) {
      totalhouseholds = activUtvalg.Antall[0];
      totalhouseholdsReserved = activUtvalg.Antall[2];
    } else {
      totalhouseholds = households.reduce((partialSum, a) => partialSum + a, 0);
      totalhouseholdsReserved = householdsReserved.reduce(
        (partialSum, a) => partialSum + a,
        0
      );
    }

    let TotalHouseholdsHouseReserve = totalhouseholds + totalhouseholdsReserved;
    let HusstandPercent =
      Math.round(
        (totalhouseholds / TotalHouseholdsHouseReserve) * 100
      ).toFixed() + "%";
    setHusstandValue(HusstandPercent);
    if (showReservedHouseHolds && showReserverte) {
      setDetailText(
        `Hush.: ${sum}, Res.hush.:${totalhouseholdsReserved},  Sone 0: ${sum0},  Sone 1: ${sum1},  Sone 2: ${sum2}`
      );
    } else {
      setDetailText(
        `Hush.: ${sum},  Sone 0: ${sum0},  Sone 1: ${sum1},  Sone 2: ${sum2}`
      );
    }
  };

  const callback = (SelectedRecords, selected) => {
    let houseValue = households;
    if (!selected && houseValue.includes(SelectedRecords.house)) {
      houseValue = houseValue.filter((item) => {
        return item != SelectedRecords.house;
      });
    } else {
      houseValue.push(SelectedRecords.house);
    }
    setHouseholds(houseValue);

    let reservValue = householdsReserved;
    if (!selected && reservValue.includes(SelectedRecords.householdsReserved)) {
      reservValue = reservValue.filter((item) => {
        return item != SelectedRecords.householdsReserved;
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
    setSelectedKeys(selectedArray);
    return selectedRoutes;
  };
  const handleOpen = async (e) => {
    await createActiveUtvalg(outputData, "Fylke: 30", 10);
    setutvalglistcheck(false);
    setActivUtvalglist({});
    setSave(true);
    setissave(false);
    setRuteDisplay(false);
    setAktivDisplay(true);
    setMapDisplay(true);
  };

  const saveSelection = async (e) => {
    setloading(true);
    setSelectionUpdate(false);
    activUtvalg.reoler = await NewFun(outputData);

    let saveOldReoler = "false";
    let skipHistory = "false";
    let forceUtvalgListId = 0;
    let url = `Utvalg/SaveUtvalg?userName=Internbruker&`;
    url = url + `saveOldReoler=${saveOldReoler}&`;
    url = url + `skipHistory=${skipHistory}&`;

    url = url + `forceUtvalgListId=${forceUtvalgListId}`;
    try {
      activUtvalg.modifications.push({
        modificationId: Math.floor(100000 + Math.random() * 900000),
        userId: "Internbruker",
        modificationTime: CurrentDate(),
        listId: 0,
      });
      const { data, status } = await api.postdata(url, activUtvalg);
      if (status === 200) {
        let flag = 0;
        let NewListId = 0;

        var newListData;
        var doubleCoverageItems;
        let selectedArr = { list: [], index1: 0, index2: 0 };
        if (checkedList?.length > 0) {
          checkedList.map((item, x) => {
            if (
              JSON.parse(item?.utvalgId) === JSON.parse(activUtvalg?.utvalgId)
            ) {
              checkedList[x] = data;
            }
          });
          setCheckedList([...checkedList]);
        }
        showorklist.map((item, x) => {
          if (
            item?.memberLists?.length !== 0 &&
            item?.memberLists !== undefined
          ) {
            item?.memberLists.map((it, y) => {
              if (
                it?.memberUtvalgs?.length !== 0 &&
                it?.memberUtvalgs !== undefined
              ) {
                it?.memberUtvalgs.map((i, index) => {
                  if (i?.utvalgId === activUtvalg?.utvalgId) {
                    flag = 1;
                    NewListId = item.listId;
                    item.memberLists[y].antall -= i.totalAntall;
                    item.memberLists[y].antall += data.totalAntall;
                    item.antall -= i.totalAntall;
                    item.antall += data.totalAntall;
                    it.memberUtvalgs[index] = data;

                    selectedArr.list = it.memberUtvalgs;
                    selectedArr.index1 = x;
                    selectedArr.index2 = y;
                    newListData = item;
                  }
                });
              }
            });
          } else if (
            item?.memberLists?.length === 0 &&
            item?.memberUtvalgs?.length > 0
          ) {
            item?.memberUtvalgs.map((i, index) => {
              if (i?.utvalgId === activUtvalg?.utvalgId) {
                flag = 2;
                NewListId = item.listId;

                item.antall -= i.totalAntall;
                item.antall += data.totalAntall;
                item.memberUtvalgs[index] = data;
                selectedArr.list = item.memberUtvalgs;
                selectedArr.index1 = x;
                newListData = item;
              }
            });
          } else if (item?.utvalgId === activUtvalg?.utvalgId) {
            item.totalAntall = data.totalAntall;
            item.modifications[0].modificationTime =
              data?.modifications[0]?.modificationTime;
          }
        });
        setshoworklist([...showorklist]);

        if (
          newListData?.listId &&
          JSON.parse(newListData?.listId) === JSON.parse(activUtvalg?.listId)
        ) {
          if (newListData?.memberUtvalgs) {
            doubleCoverageItems = filterCommonReolIds(
              newListData?.memberUtvalgs
            );
          }
        } else {
          if (newListData?.memberLists) {
            newListData?.memberLists.map((item) => {
              if (
                JSON.parse(item?.listId) === JSON.parse(activUtvalg?.listId)
              ) {
                doubleCoverageItems = filterCommonReolIds(item?.memberUtvalgs);
              }
            });
          } else {
            if (activUtvalg?.listId && JSON.parse(activUtvalg?.listId)) {
              newListData = await getListDetails(activUtvalg?.listId);
              if (newListData?.memberUtvalgs) {
                doubleCoverageItems = filterCommonReolIds(
                  newListData?.memberUtvalgs
                );
              }
            }
          }
        }

        let msg = "";
        if (doubleCoverageItems?.filteredCommonSelectionNames?.length > 1) {
          let commonRuteCount = 0;
          if (doubleCoverageItems?.filteredCommonItems?.length > 0) {
            doubleCoverageItems?.filteredCommonItems?.map((item) => {
              commonRuteCount = commonRuteCount + 1;
            });
          }

          msg = `Utvalg  "${
            activUtvalg.name
          }" er lagret. Det er dobbeltdekning på ${commonRuteCount} ruter på denne utvalgslisten. "${doubleCoverageItems?.filteredCommonSelectionNames?.map(
            (item) => {
              return " " + item;
            }
          )}"`;
        } else {
          msg = `Utvalg  "${activUtvalg.name}" er lagret.`;
        }

        //clear saved item from maintainUnsavedRute array
        if (maintainUnsavedRute?.length !== 0) {
          let activeSelectionID;
          let tempMaintainUnsavedRute;
          maintainUnsavedRute.forEach(function (item) {
            if (item.selectionID === activUtvalg.utvalgId) {
              item.activeUtval.reoler = activUtvalg.reoler;
              activeSelectionID = item.selectionID;
            }
          });

          tempMaintainUnsavedRute = maintainUnsavedRute;
          tempMaintainUnsavedRute = tempMaintainUnsavedRute.filter(
            (Item) => Item.selectionID != activeSelectionID
          );

          setMaintainUnsavedRute(tempMaintainUnsavedRute);
        }

        setloading(false);
        setdisplayMsg(msg);
        setshowmsg(true);
      } else {
        setloading(false);
        setdisplayMsg("Oppgitte søkekriterier ga ikke noe resultat.");
        setshowmsg(true);
      }
    } catch (error) {
      console.error("error : " + error);

      setloading(false);
      setdisplayMsg("Oppgitte søkekriterier ga ikke noe resultat.");
      setshowmsg(true);
    }
  };
  const NewFun = async (outputData) => {
    let routes123 = await getSelectedRoutes(outputData);
    let reolerArray = [];

    routes123.map((item) => {
      reolerArray.push(routesData.filter((x) => x.reolId === item.key)[0]);
    });
    let Antall = getAntall(routes123);
    activUtvalg.totalAntall =
      (showHousehold ? Antall[0] : 0) +
      (showBusiness ? Antall[1] : 0) +
      (showReservedHouseHolds ? Antall[2] : 0);
    activUtvalg.Antall = Antall;
    activUtvalg.hush = Antall[0];
    return reolerArray;
    //filter rutes data using selected key and assign it to new variable and assign it to reoler
    //let Antall = getAntall(routesArray);
  };
  const getListDetails = async (id) => {
    let url = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${id}`;

    try {
      //api.logger.info("APIURL", url);
      const { data } = await api.getdata(url);
      if (data.length === 0) {
        //api.logger.error("Error : No Data is present for mentioned Id" + id);
      } else {
        return data;
      }
    } catch (error) {
      //api.logger.error("errorpage API not working");
      //api.logger.error("error : " + error);
    }
  };
  const updateActiveUtvalg = async (selectedDataSet) => {
    let selectedRoutes = await getSelectedRoutes(selectedDataSet);
    let reolerArray = [];
    let toBeSavedRuteDetails = {};
    selectedRoutes.map((item) => {
      reolerArray.push(routesData.filter((x) => x.reolId === item.key)[0]);
    });

    let previousStateSelectedRutes = [];
    if (rutefoshkerPreviousSelectedRutes.length === 0) {
      previousStateSelectedRutes = rutefoshkerPreviousSelectedRutes;
      reolerArray.map((item) => {
        previousStateSelectedRutes.push(item.reolId);
      });
      setrutefoshkerPreviousSelectedRutes(previousStateSelectedRutes);
    }

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
    activUtvalg.reoler = reolerArray;
    activUtvalg.Antall = antall;
    setActivUtvalg(activUtvalg);

    if (checkedList?.length > 0) {
      checkedList.map((item, x) => {
        if (JSON.parse(item?.utvalgId) === JSON.parse(activUtvalg?.utvalgId)) {
          checkedList[x].reoler = activUtvalg.reoler;
          checkedList[x].totalAntall = activUtvalg.totalAntall;
          checkedList[x].antallBeforeRecreation =
            activUtvalg.antallBeforeRecreation;
        }
      });
      setCheckedList([...checkedList]);
    }
    if (!reolIDsUpdate) {
      let toBeSavedRute;
      if (maintainUnsavedRute?.length === 0) {
        toBeSavedRute = maintainUnsavedRute;
        toBeSavedRuteDetails = {
          selectionID: activUtvalg.utvalgId,
          activeUtval: activUtvalg,
        };
        toBeSavedRute.push(toBeSavedRuteDetails);
        setMaintainUnsavedRute(toBeSavedRute);
      } else {
        let selectionIDs = [];
        maintainUnsavedRute.forEach(function (item) {
          selectionIDs.push(item.selectionID);
        });

        if (selectionIDs.includes(activUtvalg.utvalgId)) {
          maintainUnsavedRute.forEach(function (item) {
            if (item.selectionID === activUtvalg.utvalgId) {
              item.activeUtval.reoler = activUtvalg.reoler;
            }
          });
        } else {
          toBeSavedRute = maintainUnsavedRute;
          toBeSavedRuteDetails = {
            selectionID: activUtvalg.utvalgId,
            activeUtval: activUtvalg,
          };
          toBeSavedRute.push(toBeSavedRuteDetails);
          setMaintainUnsavedRute(toBeSavedRute);
        }
      }

      setReolIDsUpdate(false);
    }
  };

  const createActiveUtvalg = async (selectedDataSet, key, criteriaType) => {
    let routes123 = await getSelectedRoutes(selectedDataSet);
    let reolerArray = [];
    routes123.map((item) => {
      reolerArray.push(routesData.filter((x) => x.reolId == item.key)[0]);
    });
    let Antall = getAntall(routes123);
    var a = Utvalg();
    a.hasReservedReceivers = !!showReservedHouseHolds;
    a.name = NewUtvalgName();

    a.totalAntall =
      (showHousehold ? Antall[0] : 0) +
      (showBusiness ? Antall[1] : 0) +
      (showReservedHouseHolds ? Antall[2] : 0);
    if (showHousehold) a.receivers.push({ receiverId: 1, selected: true });
    //a.receivers = [{ receiverId: 1, selected: true }];
    if (showBusiness) a.receivers.push({ receiverId: 4, selected: true });
    if (showReservedHouseHolds)
      a.receivers.push({ receiverId: 5, selected: true });

    a.reoler = reolerArray;
    a.criterias.push(criterias(criteriaType, key));
    a.Antall = Antall;
    setActivUtvalg(a);
  };

  useEffect(async () => {
    if (selectedKeys.length > 0) {
      await Promise.resolve(
        setResultData(
          await groupBy(
            routesData,
            "",
            0,
            showHousehold,
            showBusiness,
            showReservedHouseHolds,
            selectedKeys,
            ""
          )
        )
      );
      setReolIDsUpdate(false);
    }
  }, [selectedKeys]);

  useEffect(() => {
    if (outputData.length > 0) {
      createUtvalgObject();
      if (activUtvalg.name && activUtvalg.name !== "Påbegynt utvalg") {
        setInitialValue(activUtvalg.name);

        updateActiveUtvalg(outputData);
        btnSaveSom.current.disabled = false;
        if (
          activUtvalg.basedOn !== 0 &&
          activUtvalg.basedOn !== undefined &&
          activUtvalg.basedOn !== ""
        ) {
          btnSave.current.disabled = true;
        } else {
          btnSave.current.disabled = false;
        }
      } else {
        setInitialValue("Påbegynt utvalg");
        setutvalglistcheck(false);
        setActivUtvalglist({});
        btnSaveSom.current.disabled = false;
        btnSave.current.disabled = true;
        createActiveUtvalg(outputData, "Fylke: 30", 10);
      }
      setvistogglevalue(true);
    } else {
      settotalValue(0);

      setDetailText(`Hush.: ${0},  Sone 0: ${0},  Sone 1: ${0},  Sone 2: ${0}`);
      setvistogglevalue(false);
      btnSaveSom.current.disabled = true;
      btnSave.current.disabled = true;
    }
  }, [outputData]);

  useEffect(async () => {
    setRutefoshkerVisited(true);
    setDetails(false);

    await fetchData().then(async () => {
      if (activUtvalg.reoler !== null && activUtvalg.reoler !== undefined) {
        let selectedRoutesData = selectedRows;

        activUtvalg.reoler.map((item) => {
          selectedRoutesData.push(parseInt(item.reolId));
        });

        setSelectedRows(selectedRoutesData);

        let data = groupBy(
          activUtvalg.reoler,
          "",
          0,
          showHousehold,
          showBusiness,
          showReservedHouseHolds,
          []
        );

        let routesDataUpdate = await getChildrenRoute(data);

        btnSaveSom.current.disabled = true;
        setReolIDsUpdate(true);
        setOutputData(routesDataUpdate);
      }
    });
    if (Object.keys(activUtvalg).length === 0) {
      setShowHousehold(true);
    }
  }, []);

  const getChildrenRoute = (data) => {
    return data.reduce((acc, dt) => {
      if (!(dt.children === undefined)) {
        return acc.concat(getChildrenRoute(dt.children));
      }
      return acc.concat(dt);
    }, []);
  };
  const fetchData = async () => {
    try {
      setData(
        await GetData(
          "Reol/GetAllReols",
          "fylke",
          0,
          showHousehold,
          showBusiness,
          showReservedHouseHolds,
          [],
          routes
        )
      );
    } catch (error) {
      console.error("er : " + error);
    }
  };
  let columns = [
    {
      title: "Fylke\\Kommun\\Team\\Rute",
      dataIndex: "name",
      key: "key",
      sorter: (a, b) => {
        return;
      },
      sortOrder: "ascend",
      sortDirections: ["ASC", "DESC"],
      shouldCellUpdate: () => false,
    },

    {
      title: "Sone 0",
      dataIndex: "zone0",
      key: "zone0",
      align: "right",
      render: (zone0) => NumberFormat(zone0),
      shouldCellUpdate: () => false,
    },
    {
      title: "Sone 1",
      dataIndex: "zone1",
      key: "zone1",
      align: "right",
      render: (zone1) => NumberFormat(zone1),
      shouldCellUpdate: () => false,
    },
    {
      title: "Sone 2",
      dataIndex: "zone2",
      key: "zone2",
      align: "right",
      render: (zone2) => NumberFormat(zone2),
      shouldCellUpdate: () => false,
    },
    {
      title: "Total",
      dataIndex: "total",
      key: "total",
      align: "right",
      render: (total) => NumberFormat(total),
      shouldCellUpdate: () => true,
    },
  ];

  const showColumn = () => {
    if (showHousehold) {
      columns.splice(1, 0, {
        title: "Hush.",
        dataIndex: "house",
        key: "house",
        align: "right",
        render: (house) => NumberFormat(house),
        shouldCellUpdate: () => false,
      });
    }
    if (showBusiness) {
      columns.splice(2, 0, {
        title: "Virk.",
        dataIndex: "businesses",
        key: "businesses",
        align: "right",
        render: (businesses) => NumberFormat(businesses),
        shouldCellUpdate: () => false,
      });
    }

    if (showReservedHouseHolds && showReserverte) {
      columns.splice(2, 0, {
        title: "Hush resv",
        dataIndex: "householdsReserved",
        key: "householdsReserved",
        align: "right",
        render: (householdsReserved) => NumberFormat(householdsReserved),
        shouldCellUpdate: () => false,
      });
    }
    return columns;
  };

  return (
    <div className="card divcolor rutePage">
      <div className="Kj-background-color pl-1 pb-1 ">
        <span className="Rute-text">RUTEUTFORSKER</span>
      </div>
      <div style={{ lineHeight: ".5rem" }}>&nbsp;</div>
      <>
        {showmsg ? (
          <div className="UtvaldivLabelText ml-2">{displaymsg}</div>
        ) : null}
        <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 p-0 m-0">
          <div className="row col-lg-6 col-md-12 col-sm-12 col-xs-12 p-0 m-0">
            <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 p-0 ml-2">
              <div className="col-lg-2 col-md-2 col-sm-3 col-xs-3 p-0 m-0">
                {" "}
                <input
                  type="submit"
                  className="KSPU_button "
                  value="Nytt utvalg"
                  onClick={newSelectionCreation}
                />
              </div>
              <div className="col-lg-2 col-md-2 col-sm-3 col-xs-3 p-0 m-0">
                {" "}
                <input
                  type="submit"
                  className="KSPU_button"
                  ref={btnSave}
                  //disabled={true}
                  value="Lagre"
                  onClick={saveSelection}
                />
              </div>
              <div>{loading ? <Spinner /> : null}</div>
              <div className="col-lg-2 col-md-2 col-sm-3 col-xs-3 p-0 m-0">
                {" "}
                <input
                  type="submit"
                  ref={btnSaveSom}
                  onClick={handleOpen}
                  className="KSPU_button"
                  data-toggle="modal"
                  data-target="#uxBtnLagre123"
                  value="Lagre som"
                />
              </div>
              <div className="col-lg-2 col-md-2 col-sm-3 col-xs-3 p-0 m-0">
                <input
                  type="submit"
                  className="KSPU_button"
                  value="Eksporter"
                  onClick={() =>
                    ExportExcel(showColumn(), outputData, "pumaexcel")
                  }
                />
              </div>
            </div>
            <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 p-0 ml-2 mt-2 m-0">
              <div className="p-0 m-0">
                <label className="rute_label">Utvalg: </label>
                <label className="ruteSelectionName d-inline ml-1">
                  {initialListValue}
                </label>
              </div>
              {/* <div className="col-lg-3 col-md-3 col-sm-6 col-xs-6 p-0 m-0">
                {" "}
                <label className="ruteSelectionName d-inline">
                  {initialListValue}
                </label>
              </div> */}
            </div>
            <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 p-0 ml-2 m-0">
              <div className="p-0 m-0">
                <label className="rute_label">Totalt: </label>
                <label className="divValueText d-inline ml-1">
                  {TotalValue}
                </label>
              </div>
              {/* <div className="col-lg-2 col-md-3 col-sm-6 col-xs-6 p-0 m-0">
                <label className="divValueText d-inline">{TotalValue}</label>
              </div> */}
            </div>
            {showDenking ? (
              <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 p-0 ml-2 m-0">
                <div className="col-lg-1 col-md-2 col-sm-3 col-xs-4 p-0 m-0">
                  <label className="rute_label">Husstandsdekning: </label>
                </div>
                <div className="col-lg-11 col-md-10 col-sm-9 col-xs-8 p-0 m-0 pl-1">
                  <label className="dekningValue d-inline">
                    {HusstandValue}
                  </label>
                </div>
              </div>
            ) : null}
            <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 p-0 ml-2 m-0">
              <div className="p-0 m-0">
                <label
                  className="rute_label"
                  style={{
                    visibility: detailText.length > 0 ? "visible" : "hidden",
                  }}
                >
                  Detaljer:{" "}
                </label>
                <label className="divValueText d-inline ml-1">
                  {detailText}
                </label>
              </div>
              {/* <div className="col-lg-10 col-md-10 col-sm-9 col-xs-8 p-0 m-0 pl-1">
                <label className="divValueText d-inline">{detailText}</label>
              </div> */}
            </div>
          </div>
          <div className="col-lg-6 col-md-12">
            <div className="row">
              <div className="col-lg-6 col-md-6 p-0 m-0 pr-2">
                {vistogglevalue ? (
                  <div>
                    <MottakerComponent marginTop="0px" />
                  </div>
                ) : null}
              </div>
              <div className="col-lg-6 col-md-6 p-0 m-0 pr-2">
                {vistogglevalue ? (
                  <div>
                    <Information />
                  </div>
                ) : null}
              </div>
            </div>
          </div>
        </div>
        <TableNew
          width1={"100%"}
          columnsArray={showColumn()}
          data={datalist}
          setoutputDataList={setOutputData}
          defaultSelectedColumn={selectedRows}
          setSelectedRows={setSelectedRows}
          hideselection={0}
          parentCallback={callback}
        />
      </>
    </div>
  );
}
export default Rute;
