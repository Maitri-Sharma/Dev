import React, { useState, useContext, useEffect, useRef } from "react";

import DistributionDetails from "./DistributionDetails";
import SelectionDetails from "./SelectionDetails";
import SelectionList from "./Selection_list";
import { KSPUContext, MainPageContext } from "../context/Context.js";
import SaveUtvalg from "./SaveUtvalg";
import SaveUtvalgList from "./SaveUtvalgList";
import SaveCampaign from "./SaveCampaign";
import SaveCampaignList from "./SaveCampaignList";
import { Standardreport } from "./standardreport/standardreport";
import Swal from "sweetalert2";
import $ from "jquery";
import api from "../services/api.js";
import { CurrentDate, filterCommonReolIds } from "../common/Functions";

import Spinner from "../components/spinner/spinner.component";
function UtvalDetails(props) {
  const { mapView, setActiveMapButton } = useContext(MainPageContext);
  const {
    showReservedHouseHolds,
    setResultData,
    setvalue,
    setAktivDisplay,
    save,
    setSave,
    activUtvalglist,
    setActivUtvalglist,
    utvalglistcheck,
    Details,
    setDetails,
    basisUtvalg,
    setshoworklist,
    showorklist,
    setCheckedList,
    checkedList,
    setshowPriceCal,
    Budruteendringer,
    setBudruteendringer,
    setSelectionUpdate,
    showBusiness,
    showHousehold,
    activUtvalg,
    issave,
    setActivUtvalg,
    maintainUnsavedRute,
    setMaintainUnsavedRute,
  } = useContext(KSPUContext);

  const [loading, setloading] = useState(false);
  const [Large, setLarge] = useState("1");
  const [LagreList, setLagreList] = useState("1");
  const [openKamp, setOpenKamp] = useState("1");
  const [btnDisabled, setBtnDisabled] = useState(false);
  const [showCap, setShowCap] = useState(false);
  const [showReportPopUp, setShowReportPopup] = useState(false);
  const [Modal, setModal] = useState(false);
  const [kvitterModal, setKvitterModal] = useState(false);

  const uxBtnLagreSom = useRef();

  useEffect(() => {
    if (
      Object.keys(activUtvalglist).length > 0 &&
      activUtvalglist?.antallBeforeRecreation > 0 &&
      Budruteendringer
    ) {
      setBudruteendringer(false);
      let msg = `Budruteendringer har påvirket utvalgene`;

      Swal.fire({
        text: msg,
        confirmButtonColor: "#7bc144",
        confirmButtonText: "Lukk",
      });
    }
    if (save) {
      setShowCap(!showCap);
      setSave(false);
    }

    if (Object.keys(activUtvalg).length !== 0) {
      if (
        activUtvalg.utvalgId === 0 ||
        activUtvalg.utvalgId === undefined ||
        activUtvalg.utvalgId === ""
      ) {
        setBtnDisabled(true);
      } else if (
        activUtvalg.basedOn !== 0 &&
        activUtvalg.basedOn !== undefined &&
        activUtvalg.basedOn !== ""
      ) {
        setBtnDisabled(true);
      } else {
        setBtnDisabled(false);
      }
    }
    if (Object.keys(activUtvalglist).length !== 0) {
      if (
        activUtvalglist.basedOn !== 0 &&
        activUtvalglist.basedOn !== undefined &&
        activUtvalglist.basedOn !== ""
      ) {
        setBtnDisabled(true);
      } else {
        setBtnDisabled(false);
      }
    }
  }, []);
  useEffect(() => {
    if (Object.keys(activUtvalg).length !== 0) {
      uxBtnLagreSom.current.click();
    }
  }, [showCap]);

  const disDetails = (e) => {
    e.preventDefault();
    setshowPriceCal(false);
    setDetails(true);
  };
  const disDetails_linkClick = (e) => {
    e.preventDefault();
    setDetails(true);
    setshowPriceCal(true);
  };
  const toggleShowReport = () => {
    setShowReportPopup(true);
  };
  const showLarge = async (e) => {
    setSelectionUpdate(false);
    setLarge("Save_Large");
  };

  const showLagreList = async (e) => {
    setSelectionUpdate(false);
    await setLagreList("Save_Large");
  };

  const GodkjenAlle = () => {
    setModal(true);
  };

  const addKampanje = async (e) => {
    if (utvalglistcheck) {
      await setOpenKamp("OpenCampaignList");
    } else {
      await setOpenKamp("OpenCampaign");
    }
  };

  const slettClick = async () => {
    let url = "";
    let id = "";
    let SelectedItem = {};
    let checkutvalg = "";
    if (activUtvalg) {
      id = activUtvalg?.utvalgId;
      checkutvalg = "U";
      SelectedItem = activUtvalg;
    }
    if (activUtvalglist?.listId) {
      id = activUtvalglist?.listId;
      checkutvalg = "L";
      SelectedItem = activUtvalglist;
    }

    let utvalgType = "";

    if (checkutvalg === "U") {
      Swal.fire({
        title: "Advarsel",
        text: "Skal utvalget slettes?",
        icon: "warning",
        cancelButtonText: "Nei",
        confirmButtonColor: "#7bc144",
        cancelButtonColor: "#7bc144",
        confirmButtonText: "Ja",
        showCancelButton: true,
      }).then(async (willDelete) => {
        if (willDelete.isConfirmed) {
          url = "Utvalg/DeleteUtvalg?utvalgId=";
          const { data, status } = await api.deletedata(url + id);
          if (status === 200) {
            if (data === true) {
              activUtvalg?.reoler.map((reols) => {
                let graphRemove = [];

                mapView.graphics.items.map((graphElement) => {
                  if (
                    graphElement.attributes !== undefined &&
                    graphElement.attributes !== null
                  ) {
                    if (graphElement.attributes.reol_id !== undefined) {
                      if (
                        graphElement.attributes.reol_id.toString() ===
                        reols.reolId.toString()
                      ) {
                        graphRemove.push(graphElement);
                      }
                    }
                  }
                });
                graphRemove.map((itemrmv) => {
                  if (
                    Object.entries(itemrmv.symbol.color).toString() ===
                      Object.entries({
                        r: 237,
                        g: 54,
                        b: 21,
                        a: 0.25,
                      }).toString() ||
                    Object.entries(itemrmv.symbol.color).toString() ===
                      Object.entries({ r: 0, g: 255, b: 0, a: 0.8 }).toString()
                  ) {
                    mapView.graphics.remove(itemrmv);
                  }
                });
              });

              let worklist = showorklist;
              if (checkedList?.length > 0) {
                let filteredArr = checkedList.filter((item) => {
                  return item?.utvalgId !== activUtvalg?.utvalgId;
                });
                setCheckedList(filteredArr);
              }

              worklist.map((item) => {
                if (item?.memberUtvalgs?.length) {
                  let flag = false;
                  //confirms its a list
                  let filteredArray = [];
                  item?.memberUtvalgs.map((i) => {
                    if (i.utvalgId === activUtvalg.utvalgId) {
                      flag = true;
                      filteredArray = item?.memberUtvalgs.filter((utvals) => {
                        return utvals?.utvalgId !== activUtvalg.utvalgId;
                      });
                      item.memberUtvalgs = filteredArray;
                      item.antall -= i.totalAntall;
                    }
                  });
                  if (flag) {
                    setshoworklist(worklist);
                  }
                } else if (item?.listId && item?.memberLists?.length > 0) {
                  let flag = false;
                  //its a list of list
                  let filteredArray = [];
                  item?.memberLists.map((j) => {
                    if (j.memberUtvalgs?.length > 0) {
                      j.memberUtvalgs.map((k) => {
                        if (k.utvalgId === activUtvalg.utvalgId) {
                          flag = true;
                          filteredArray = j.memberUtvalgs.filter((utvals) => {
                            return utvals?.utvalgId !== activUtvalg.utvalgId;
                          });
                          j.memberUtvalgs = filteredArray;
                          item.antall -= k.totalAntall;
                          j.antall -= k.totalAntall;
                        }
                      });
                    }
                  });
                  if (flag) {
                    setshoworklist(worklist);
                  }
                } else {
                  //its an utvalg
                  if (item.utvalgId === activUtvalg.utvalgId) {
                    let filteredArray = worklist;
                    filteredArray = filteredArray.filter((utvals) => {
                      return utvals?.utvalgId !== activUtvalg.utvalgId;
                    });
                    setshoworklist(filteredArray);
                  }
                }
              });

              utvalgType = "utvalgID";
              setAktivDisplay(false);
              setvalue(true);

              $(".modal").remove();
              $(".modal-backdrop").remove();
              Swal.fire({
                text: "Utvalg slettet",
                confirmButtonColor: "#7bc144",
                confirmButtonText: "Lukk",
              });
            }
          }
        } else {
          let msg = "API not working";
          $(".modal").remove();
          $(".modal-backdrop").remove();
          Swal.fire({
            text: msg,
            confirmButtonColor: "#7bc144",
            confirmButtonText: "Lukk",
          });
        }
      });
    }
    if (checkutvalg === "L") {
      let removeFromList = false;
      let workListArray = [];
      let newListId = 0;
      Swal.fire({
        title: "Advarsel",
        text: "Skal liste slettes?",
        icon: "warning",
        cancelButtonText: "Nei",
        cancelButtonColor: "#7bc144",
        confirmButtonColor: "#7bc144",
        confirmButtonText: "Ja",
        showCancelButton: true,
      }).then(async (willDelete) => {
        if (willDelete.isConfirmed) {
          if (SelectedItem?.basedOn > 0) {
            try {
              url = `UtvalgList/DeleteCampaignList?ListId=${SelectedItem.listId}&BasedOn=${SelectedItem.basedOn}`;
              const { data, status } = await api.deletedata(url);

              if (status === 200) {
                let ResultAfterDelete = showorklist.filter(
                  (value) => value.listId != SelectedItem?.listId
                );
                setshoworklist(ResultAfterDelete);
                setAktivDisplay(false);
                setvalue(true);
                $(".modal").remove();
                $(".modal-backdrop").remove();
                Swal.fire({
                  text: `Utvalg slettet`,
                  confirmButtonColor: "#7bc144",
                  position: "top",
                  icon: "success",
                });
              } else {
                console.error("error : " + status);
              }
            } catch (error) {
              console.error("er : " + error);
            }
          } else {
            url = "UtvalgList/DeleteUtvalgList?UtvalgListId=";
            const { data, status } = await api.deletedata(url + id);
            if (status === 200) {
              if (data === true) {
                utvalgType = "ListID";
                let worklist = showorklist;
                worklist.map((item) => {
                  if (item?.listId && item?.memberLists?.length > 0) {
                    if (
                      item?.listId &&
                      item?.listId === activUtvalglist?.listId
                    ) {
                      newListId = 0;
                      removeFromList = true;
                    } else {
                      item?.memberLists.map((j) => {
                        if (j?.listId === activUtvalglist?.listId) {
                          newListId = item?.listId;
                          removeFromList = true;
                        }
                      });
                    }
                  } else if (
                    item?.listId &&
                    item?.listId === activUtvalglist?.listId
                  ) {
                    newListId = 0;
                    removeFromList = true;
                  }

                  if (removeFromList) {
                    removeFromList = false;
                  } else {
                    workListArray.push(item);
                  }
                });
                if (newListId) {
                  let newlistUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${newListId}`;
                  const { data, status } = await api.getdata(newlistUrl);
                  if (status === 200 && data !== undefined) {
                    workListArray.push(data);
                  }
                }

                setshoworklist(workListArray);
                setAktivDisplay(false);
                setvalue(true);
                let msg = "liste slettet";
                $(".modal").remove();
                $(".modal-backdrop").remove();
                Swal.fire({
                  text: msg,
                  confirmButtonColor: "#7bc144",
                  confirmButtonText: "Lukk",
                });
              }
            }
          }
        } else {
          let msg = "API not working";
          $(".modal").remove();
          $(".modal-backdrop").remove();
          Swal.fire({
            text: msg,
            confirmButtonColor: "#7bc144",
            confirmButtonText: "Lukk",
          });
        }
      });
      if (SelectedItem?.basedOn > 0) {
        try {
          url = `UtvalgList/DeleteCampaignList?ListId=${SelectedItem.listId}&BasedOn=${SelectedItem.basedOn}`;
          const { data, status } = await api.deletedata(url);

          if (status === 200) {
            $(".modal").remove();
            $(".modal-backdrop").remove();
          } else {
            console.error("error : " + status);
          }
        } catch (error) {
          console.error("er : " + error);
        }
      }
    }
  };

  const handleCancel = (e) => {
    setResultData([]);
    window.scrollTo(0, 0);
    setActivUtvalg({});
    setActivUtvalglist({});
    setvalue(true);
    setAktivDisplay(false);
  };
  const Jaclick = async () => {
    setloading(true);

    let listArray = [];
    let flagArray = [];
    let NewListId = activUtvalglist.listId;
    let url = `Utvalg/AcceptAllChangesForList?userName=Internbruker&ListId=${NewListId}`;
    try {
      const { data, status } = await api.postdata(url);
      if (status === 200) {
        let updatedList = NewListId;
        let Flag = false;
        showorklist.map((item) => {
          if (item?.memberLists?.length > 0) {
            item?.memberLists.map((memberItem) => {
              if (memberItem.listId.toString() !== NewListId.toString()) {
                Flag = false;
              } else {
                Flag = true;
                updatedList = item?.listId;
              }
            });
            if (!true) {
              listArray.push(item);
            }
          } else if (item.listId.toString() !== NewListId.toString()) {
            listArray.push(item);
          }
        });
        let newlistUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${updatedList}`;
        const { data, status } = await api.getdata(newlistUrl);
        if (status === 200 && data !== undefined) {
          activUtvalglist.antallBeforeRecreation = 0;
          if (listArray.length > 0) {
            await setshoworklist(listArray.concat(data));
          } else {
            flagArray.push(data);
            await setshoworklist(flagArray);
          }
        }

        $(".modal").remove();
        $(".modal-backdrop").remove();

        Swal.fire({
          text: `Budruteendringer er nå godkjent for alle utvalgene i lista.`,
          confirmButtonColor: "#7bc144",
          confirmButtonText: "Lukk",
        });
        setloading(false);
        //swal(msg);
      } else {
        $(".modal").remove();
        $(".modal-backdrop").remove();
        setloading(false);

        Swal.fire({
          text: `Oppgitte søkekriterier ga ikke noe resultat.`,
          confirmButtonColor: "#7bc144",
          confirmButtonText: "Lukk",
        });
      }
    } catch (error) {
      console.error("error : " + error);

      $(".modal").remove();
      $(".modal-backdrop").remove();
      Swal.fire({
        text: `Oppgitte søkekriterier ga ikke noe resultat.`,
        confirmButtonColor: "#7bc144",
        confirmButtonText: "Lukk",
      });

      setloading(false);
    }
    //setKvitterModal(true);
  };
  const SaveUtvalgButton = async (e) => {
    setLarge("");
    //disable sketech widget on switching the selection
    if (mapView.activeTool !== null) {
      setActiveMapButton("");
      mapView.activeTool = null;
    }

    if (activUtvalg?.name !== "Påbegynt utvalg") {
      setloading(true);
      setSelectionUpdate(false);
      let saveOldReoler = "false";
      let skipHistory = "false";
      let forceUtvalgListId = 0;
      let url = `Utvalg/SaveUtvalg?userName=Internbruker&`;
      url = url + `saveOldReoler=${saveOldReoler}&`;
      url = url + `skipHistory=${skipHistory}&`;

      url = url + `forceUtvalgListId=${forceUtvalgListId}`;
      try {
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
          });
        } else {
          if (showHousehold) {
            activUtvalg.receivers.push({ receiverId: 1, selected: true });
          }
        }

        if (activUtvalg.receivers.length !== 0) {
          activUtvalg.receivers.map((item) => {
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
          });
        } else {
          if (showBusiness) {
            activUtvalg.receivers.push({ receiverId: 4, selected: true });
          }
        }
        if (activUtvalg.receivers.length !== 0) {
          activUtvalg.receivers.map((item) => {
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
          if (showReservedHouseHolds) {
            activUtvalg.hasReservedReceivers = true;
            activUtvalg.receivers.push({ receiverId: 5, selected: true });
          }
        }

        activUtvalg.isBasis = basisUtvalg;
        activUtvalg.antallBeforeRecreation = 0;
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

          if (checkedList?.length > 0) {
            checkedList.map((item, x) => {
              if (
                JSON.parse(item?.utvalgId) === JSON.parse(activUtvalg?.utvalgId)
              ) {
                data["imagePath"] = item.imagePath;
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
                      let countList = 0;
                      if (i?.antallWhenLastSaved > data?.totalAntall) {
                        countList = i.antallWhenLastSaved - data.totalAntall;
                        item.memberLists[y].antall -= countList;
                      } else if (i?.antallWhenLastSaved < data?.totalAntall) {
                        countList = data.totalAntall - i.antallWhenLastSaved;
                        item.memberLists[y].antall += countList;
                      }
                      let count = 0;
                      if (i?.antallWhenLastSaved > data?.totalAntall) {
                        count = i.antallWhenLastSaved - data.totalAntall;
                        item.antall -= count;
                      } else if (i?.antallWhenLastSaved < data?.totalAntall) {
                        count = data.totalAntall - i.antallWhenLastSaved;
                        item.antall += count;
                      }
                      // item.memberLists[y].antall -= i.totalAntall;
                      // item.memberLists[y].antall = data.totalAntall;
                      // item.antall -= i.totalAntall;
                      // item.antall = data.totalAntall;
                      it.memberUtvalgs[index] = data;
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
                  let count = 0;
                  if (i?.antallWhenLastSaved > data?.totalAntall) {
                    count = i.antallWhenLastSaved - data.totalAntall;
                    item.antall -= count;
                  } else if (i?.antallWhenLastSaved < data?.totalAntall) {
                    count = data.totalAntall - i.antallWhenLastSaved;
                    item.antall += count;
                  }

                  // // item.antall -= i.totalAntall;
                  // item.antall += data.totalAntall;
                  item.memberUtvalgs[index] = data;
                  newListData = item;
                }
              });
            } else if (
              JSON.parse(item?.utvalgId) === JSON.parse(activUtvalg?.utvalgId)
            ) {
              item.totalAntall = data.totalAntall;
              item.isBasis = basisUtvalg;
              item.modifications[0].modificationTime =
                data?.modifications[0]?.modificationTime;
              item.antallBeforeRecreation = data?.antallBeforeRecreation;
              item.reoler = data?.reoler;
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
                  doubleCoverageItems = filterCommonReolIds(
                    item?.memberUtvalgs
                  );
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
            msg = `Utvalg  "${activUtvalg.name}" er lagret.
          Det er dobbeltdekning på ${commonRuteCount} ruter på denne utvalgslisten. "${doubleCoverageItems?.filteredCommonSelectionNames?.map(
              (item) => {
                return " " + item;
              }
            )}" `;
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
          Swal.fire({
            text: msg,
            confirmButtonColor: "#7bc144",
            confirmButtonText: "Lukk",
          });
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
    }
  };
  const handleCallback = (buttonFlag) => {
    setBtnDisabled(buttonFlag);
  };
  const getListDetails = async (id) => {
    let url = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${id}`;

    try {
      //api.logger.info("APIURL", url);
      const { data, status } = await api.getdata(url);
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

  const closeSaveAsPopUp = () => {
    setLarge("");
  };
  const selectionSaveAsPopUp = () => {
    setBtnDisabled(false);
  };

  return (
    <>
      {Object.keys(activUtvalg).length === 0 &&
      Object.keys(activUtvalglist).length === 0 ? (
        <></>
      ) : (
        <div>
          {!Details ? (
            <div className="card pt-2 pb-2">
              {utvalglistcheck ? (
                <SelectionList />
              ) : (
                <SelectionDetails
                  Utvalcheck={"True"}
                  parentCallback={handleCallback}
                />
              )}
              {Large == "Save_Large" ? (
                <SaveUtvalg
                  id={"uxBtnLagreSom12"}
                  onClose={closeSaveAsPopUp}
                  onSave={selectionSaveAsPopUp}
                  utvalgDetails={true}
                />
              ) : null}
              {LagreList == "Save_Large" ? (
                <SaveUtvalgList id={"uxBtnLagreSomList"} />
              ) : null}
              {openKamp === "OpenCampaign" ? (
                <SaveCampaign id={"uxKampaign"} />
              ) : openKamp === "OpenCampaignList" ? (
                <SaveCampaignList id={"uxKampaign"} />
              ) : null}

              {Modal ? (
                <div
                  className="modal fade bd-example-modal-lg"
                  id="uxBtngodkjenalle"
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
                                  &nbsp; Er du sikker på at du vil godkjenne
                                  budruteendringer for alle utvalgene i lista?
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
                                  >
                                    Nei
                                  </button>
                                  <button
                                    type="button"
                                    onClick={Jaclick}
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

              {kvitterModal ? (
                <div
                  className="modal fade bd-example-modal-lg"
                  id="kvittering"
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
                      <div className="modal-header segFord">
                        <h5 className="modal-title " id="exampleModalLongTitle">
                          KVITTERING
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
              ) : null}

              <div className="pl-1 mt-4">
                <div className="row">
                  <div className="col-lg-2">
                    <input
                      type="button"
                      id="uxBtnSlett"
                      className="KSPU_button mr-1"
                      value="Slett"
                      onClick={slettClick}
                    />
                  </div>
                  <div className="col-lg-10 col-md-12 _flex-row _flex-end">
                    {Object.keys(activUtvalglist).length > 0 &&
                    activUtvalglist.antallBeforeRecreation > 0 ? (
                      <input
                        type="button"
                        className="KSPU_button mr-1"
                        data-toggle="modal"
                        data-target="#uxBtngodkjenalle"
                        onClick={GodkjenAlle}
                        value="Godkjen Alle"
                      />
                    ) : null}
                    <input
                      type="button"
                      id="uxBtnLukk"
                      className="KSPU_button mr-1"
                      onClick={handleCancel}
                      value="Lukk"
                    />

                    {!utvalglistcheck ? (
                      <input
                        type="button"
                        id="uxBtnLagreSom"
                        className="KSPU_button mr-1"
                        data-toggle="modal"
                        data-target="#uxBtnLagreSom12"
                        onClick={showLarge}
                        value="Lagre som"
                        ref={uxBtnLagreSom}
                      />
                    ) : (
                      <input
                        type="button"
                        id="uxBtnLagreSom"
                        className="KSPU_button mr-1"
                        data-toggle="modal"
                        data-target="#uxBtnLagreSomList"
                        onClick={showLagreList}
                        value="Lagre som"
                        ref={uxBtnLagreSom}
                      />
                    )}
                    {!utvalglistcheck ? (
                      <>
                        <input
                          type="button"
                          id="uxBtnLagre"
                          className="KSPU_button mr-1"
                          value="Lagre"
                          disabled={
                            btnDisabled ||
                            activUtvalg?.name === "Påbegynt utvalg"
                              ? true
                              : btnDisabled
                          }
                          onClick={SaveUtvalgButton}
                        />

                        <div>{loading ? <Spinner /> : null}</div>
                      </>
                    ) : null}
                  </div>
                </div>
                {issave && !activUtvalg.isBasis && !activUtvalglist.isBasis ? (
                  <div className="row mt-4">
                    {utvalglistcheck ? (
                      <div className="col-5"></div>
                    ) : (
                      <div className="col-5"></div>
                    )}

                    {activUtvalg?.ordreType != 1 &&
                    activUtvalglist?.ordreType != 1 &&
                    activUtvalg?.name !== "Påbegynt utvalg" ? (
                      <div className="col-5 ml-5">
                        <input
                          type="button"
                          id="uxBtnCheckCapacity"
                          className="Utval_KSPU_button_Green float-right mr-2"
                          onClick={disDetails}
                          value="Sjekk kapasitet"
                        />
                      </div>
                    ) : null}
                  </div>
                ) : issave ? (
                  <div className="col mt-2 _flex-end no-padding ">
                    <input
                      type="button"
                      id="uxLagKampanje"
                      className="Utval_KSPU_button_Green mr-1"
                      data-toggle="modal"
                      data-target="#uxKampaign"
                      value="Lag kampanje"
                      onClick={addKampanje}
                      display="block"
                    />
                  </div>
                ) : null}

                <div className="mt-4">
                  <span id="uxLblMoreFunc" className="UtvaldivLabelText">
                    Du kan også..{" "}
                  </span>
                  {issave &&
                  !activUtvalg?.isBasis &&
                  !activUtvalglist?.isBasis ? (
                    activUtvalg?.ordreType != 1 &&
                    activUtvalglist?.ordreType != 1 &&
                    activUtvalg?.name !== "Påbegynt utvalg" ? (
                      <a
                        id="uxShowUtvalgDetails_uxABeregnPris"
                        href=""
                        className="Utval_read-more "
                        onClick={disDetails_linkClick}
                      >
                        Gå rett til prisberegning
                      </a>
                    ) : null
                  ) : null}
                  {/* <label id="uxShowUtvalgDetails_uxABeregnPris" className="Utval_read-more" onClick={disDetails}>Gå rett til prisberegning</label> */}
                  {showReportPopUp === true ? (
                    <Standardreport
                      id="showReport"
                      type=""
                      isList={utvalglistcheck}
                    />
                  ) : null}
                  <label
                    id="uxShowUtvalgDetails_uxLnkLagRapport"
                    data-toggle="modal"
                    data-target="#showReport"
                    onClick={toggleShowReport}
                    className="Utval_read-more-lbl"
                  >
                    Skriv ut utvalg
                  </label>

                  {/* <label id="uxShowUtvalgDetails_uxLnkLagRapport"  className="Utval_read-more">Skriv ut utvalg</label> */}
                  {showReportPopUp === true ? (
                    <Standardreport
                      id="showDistrReport"
                      type="distr"
                      isList={utvalglistcheck}
                    />
                  ) : null}
                  <label
                    id="uxShowUtvalgDetails_uxLnkLagRapportDistr"
                    data-toggle="modal"
                    data-target="#showDistrReport"
                    onClick={toggleShowReport}
                    className="Utval_read-more-lbl"
                  >
                    SKRIV UT UTVALG DISTRIBUSJONSSEKVENS
                  </label>
                  {/* <label id="uxShowUtvalgDetails_uxLnkLagRapportDistr"  className="Utval_read-more">SKRIV UT UTVALG DISTRIBUSJONSSEKVENS</label> */}
                </div>
              </div>
              {save ? <SaveUtvalg id={"uxBtnLagreSom1245"} /> : null}
            </div>
          ) : (
            <DistributionDetails />
          )}
        </div>
      )}
    </>
  );
}

export default UtvalDetails;
