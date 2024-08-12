import React, { useState, useRef, useContext, useEffect } from "react";
import "../App.css";
import { KSPUContext, MainPageContext } from "../context/Context.js";
import { NumberFormat } from "../common/Functions";
import SaveUtvalg from "./SaveUtvalg";
import {
  Utvalg,
  Utvalglist,
  NewUtvalgName,
  criterias,
  getAntall,
} from "./KspuConfig";
import { groupBy } from "../Data";
import {
  CreateUtvalglist,
  GetImageUrl,
  CreateActiveUtvalg,
} from "../common/Functions";
import { assertBlock } from "@babel/types";
import api from "../services/api.js";
import Swal from "sweetalert2";
import $ from "jquery";
// import ShowReCreateService from "./ShowReCreateService";
// import SaveCampaign from "./SaveCampaign";
import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import ShowRecreate from "./ShowReCreateService";
function FinnClick(props) {
  const { result } = props;
  const [btnDisabled, setBtnDisabled] = useState(true);
  const [selectedGroups, setSelectedGroups] = useState([]);
  const [ApiResult, setApiResult] = useState(props.result);
  const { showReserverte, setShowReserverte } = useContext(KSPUContext);
  const { mapView } = useContext(MainPageContext);
  const [Modal, setModal] = useState("");
  const [selectedRecreateGroups, setSelectedRecreateGroups] = useState([]);
  //const [utvalgObject, setUtvalgObject] = useState({});
  const {
    groupData,
    setGroupData,
    activUtvalg,
    setActivUtvalg,
    setvalue,
    setAktivDisplay,
    resultData,
    setResultData,
    activUtvalglist,
    setActivUtvalglist,
    utvalglistcheck,
    setutvalglistcheck,
    showorklist,
    setshoworklist,
    setShowHousehold,
    setShowBusiness,
    setShowReservedHouseHolds,
    setIsWidgetActive,
    Budruteendringer,
    setBudruteendringer,
    expandListId,
    setDetails,
    setExpandListId,
    showBusiness,
    showReservedHouseHolds,
    showHousehold,
    setCheckedList,
  } = useContext(KSPUContext);
  const { issave, setissave } = useContext(KSPUContext);
  const [selectedKeys, setSelectedKeys] = useState([]);
  const [loading, setloading] = useState(false);
  const [SelectedItem, setSelectedItem] = useState({});
  const [recreateType, setrecreateType] = useState("");
  const [recreateId, setrecreateId] = useState(0);
  const [refresh, setRefresh] = useState(false);
  var worklistArray = showorklist;
  useEffect(() => {
    setRefresh(true);
    setSelectedItem({});
    setSelectedGroups([]);
    setSelectedRecreateGroups([]);

    setApiResult(props.result);
    setTimeout(() => {
      setRefresh(false);
    }, 10);
  }, [props.result]);

  const slettClick = async () => {
    let url = "";
    let id = selectedGroups[0];
    let checkutvalg = id.substring(0, 1);

    checkutvalg = checkutvalg.toUpperCase();
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
          try {
            url = "Utvalg/DeleteUtvalg?utvalgId=";
            const { data, status } = await api.deletedata(
              url + id.substring(1)
            );
            if (status === 200) {
              if (data === true) {
                let ResultAfterDelete = result.filter(
                  (value) => value.utvalgId != id.substring(1)
                );

                await renderPerson(ResultAfterDelete);
                setApiResult(ResultAfterDelete);

                // const item = result.find(({ utvalgId }) => utvalgId === id);
                $(".modal").remove();
                $(".modal-backdrop").remove();

                Swal.fire({
                  text: `Utvalg slettet`,
                  confirmButtonColor: "#7bc144",
                  position: "top",
                  icon: "success",
                });
              }
            }
          } catch {
            Swal.fire({
              text: `Utvalget er knyttet til en ordre og kan ikke slettes`,
              confirmButtonColor: "#7bc144",
              position: "top",
              icon: "info",
            });
          }
        } else {
        }
      });
    }
    if (checkutvalg === "L") {
      Swal.fire({
        title: "Advarsel",
        text: "Skal liste slettes?",
        icon: "warning",
        cancelButtonColor: "#7bc144",
        cancelButtonText: "Nei",
        confirmButtonText: "Ja",
        confirmButtonColor: "#7bc144",
        showCancelButton: true,
      }).then(async (willDelete) => {
        if (willDelete.isConfirmed) {
          if (SelectedItem.basedOn > 0) {
            try {
              url = `UtvalgList/DeleteCampaignList?ListId=${SelectedItem.listId}&BasedOn=${SelectedItem.basedOn}`;
              const { data, status } = await api.deletedata(url);
              //"Utvalg/DeleteUtvalg?utvalgId=" + utvalgId

              if (status === 200) {
                let ResultAfterDelete = result.filter(
                  (value) => value.listId != SelectedItem?.listId
                );
                await renderPerson(ResultAfterDelete);
                setApiResult(ResultAfterDelete);
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
            try {
              url = "UtvalgList/DeleteUtvalgList?UtvalgListId=";
              const { data, status } = await api.deletedata(
                url + id.substring(1)
              );
              if (status === 200) {
                if (data === true) {
                  Swal.fire({
                    text: `liste slettet`,
                    confirmButtonColor: "#7bc144",
                    position: "top",
                    icon: "success",
                  });
                  let ResultAfterDelete = result.filter(
                    (value) => value.listId != id.substring(1)
                  );
                  await renderPerson(ResultAfterDelete);
                  setApiResult(ResultAfterDelete);
                  // const item = result.find(({ utvalgId }) => utvalgId === id);
                  $(".modal").remove();
                  $(".modal-backdrop").remove();
                }
              }
            } catch {
              Swal.fire({
                text: `Utvalget er knyttet til en ordre og kan ikke slettes`,
                confirmButtonColor: "#7bc144",
                position: "top",
                icon: "info",
              });
            }
          }
        } else {
        }
      });
    }
  };

  const handleChange = (item) => {
    let itemid = "";
    if (!item.utvalgId) {
      itemid = "L" + item.listId;
    } else {
      itemid = "U" + item.utvalgId;
    }

    let newCreateArray = [...selectedRecreateGroups, item];
    if (selectedRecreateGroups.includes(item)) {
      newCreateArray = newCreateArray.filter((day) => day !== item);
    }
    let newArray = [...selectedGroups, itemid];
    if (selectedGroups.includes(itemid)) {
      newArray = newArray.filter((day) => day !== itemid);
    }
    if (newArray.length > 0) setBtnDisabled(false);
    else setBtnDisabled(true);
    setSelectedGroups(newArray);
    setSelectedRecreateGroups(newCreateArray);
    setSelectedItem(item);
  };

  const getAntall = (routes) => {
    let houseAntall = 0;
    let businessAntall = 0;
    let householdsReservedAntall = 0;

    routes.map((item) => {
      houseAntall = houseAntall + item.antall.households;
      businessAntall = businessAntall + item.antall.businesses;
      householdsReservedAntall =
        householdsReservedAntall + item.antall.householdsReserved;
    });
    return [houseAntall, businessAntall, householdsReservedAntall];
  };
  const createActiveUtvalg = (selectedDataSet) => {
    // let routes = await getSelectedRoutes(selectedDataSet.reoler);
    let Antall = getAntall(selectedDataSet.reoler);

    let utvalgObj = Utvalg();

    utvalgObj.name = selectedDataSet.name;
    utvalgObj.reoler = selectedDataSet.reoler;
    utvalgObj.Antall = Antall;
    utvalgObj.Business = Antall[1];
    utvalgObj.ReservedHouseHolds = Antall[2];
    utvalgObj.hush = Antall[0];

    utvalgObj.hasReservedReceivers = selectedDataSet.hasReservedReceivers;
    utvalgObj.oldReolMapMissng = selectedDataSet.oldReolMapMissing;
    utvalgObj.reolerBeforeRecreation = selectedDataSet.reolerBeforeRecreation;
    utvalgObj.utvalgId = selectedDataSet.utvalgId;
    utvalgObj.changed = selectedDataSet.changed;
    utvalgObj.kundeNavn = selectedDataSet.selectedDataSet;
    utvalgObj.logo = selectedDataSet.logo;
    utvalgObj.reolMapName = selectedDataSet.reolMapName;
    utvalgObj.oldReolMapName = selectedDataSet.oldReolMapName;
    utvalgObj.skrivebeskyttet = selectedDataSet.skrivebeskyttet;
    utvalgObj.weight = selectedDataSet.weight;
    utvalgObj.distributionType = selectedDataSet.distributionType;
    utvalgObj.distributionDate = selectedDataSet.distributionDate;
    utvalgObj.arealAvvik = selectedDataSet.arealAvvik;
    utvalgObj.isBasis = selectedDataSet.isBasis;
    utvalgObj.basedOn = selectedDataSet.basedOn;
    utvalgObj.basedOnName = selectedDataSet.basedOnName;
    utvalgObj.wasBasedOn = selectedDataSet.wasBasedOn;
    utvalgObj.wasBasedOnName = selectedDataSet.wasBasedOnName;
    utvalgObj.listId = selectedDataSet.listId;
    utvalgObj.listName = selectedDataSet.listName;
    utvalgObj.antallBeforeRecreation = selectedDataSet.antallBeforeRecreation;
    utvalgObj.totalAntall = selectedDataSet.totalAntall;
    utvalgObj.ordreReferanse = selectedDataSet.ordreReferanse;
    utvalgObj.ordreType = selectedDataSet.ordreType;
    utvalgObj.ordreStatus = selectedDataSet.ordreStatus;
    utvalgObj.kundeNummer = selectedDataSet.kundeNummer;
    utvalgObj.innleveringsDato = selectedDataSet.innleveringsDato;
    utvalgObj.avtalenummer = selectedDataSet.avtalenummer;
    utvalgObj.thickness = selectedDataSet.thickness;
    utvalgObj.antallWhenLastSaved = selectedDataSet.antallWhenLastSaved;
    utvalgObj.modifications = selectedDataSet.modifications;
    utvalgObj.kommuner = selectedDataSet.kommuner;
    utvalgObj.receivers = selectedDataSet.receivers;
    utvalgObj.districts = selectedDataSet.districts;
    utvalgObj.postalZones = selectedDataSet.postalZones;
    utvalgObj.criterias = selectedDataSet.criterias;
    utvalgObj.utvalgsBasedOnMe = selectedDataSet.utvalgsBasedOnMe;
    setActivUtvalg(utvalgObj);
    //setUtvalgObject({id : 1});
    var flag1 = false;
    var flag2 = false;
    var flag3 = false;
    showorklist.map((item) => {
      if (item?.utvalgId === undefined || item?.utvalgId === 0) {
        //confirms its not an utvalg
        item?.memberUtvalgs.map((i) => {
          if (i.utvalgId === utvalgObj.utvalgId) {
            flag2 = true;
          }
        });
      } else {
        //its an utvalg
        if (item?.utvalgId === utvalgObj.utvalgId) {
          flag1 = true;
        }
      }

      if (item?.listId && item?.memberLists?.length > 0) {
        //its a list of list
        item?.memberLists.map((j) => {
          if (j.memberUtvalgs?.length > 0) {
            j.memberUtvalgs.map((k) => {
              if (k.utvalgId === utvalgObj.utvalgId) {
                flag3 = true;
              }
            });
          }
        });
      }
      if (item?.memberUtvalgs?.length > 0) {
        item?.memberUtvalgs?.map((i) => {
          if (i?.utvalgId === utvalgObj?.utvalgId) {
            let arr = [];
            arr?.push(item?.listId?.toString());
            setExpandListId(arr);
          }
        });
      }
    });

    if (!flag1 && !flag2 && !flag3) {
      setshoworklist((showorklist) => [...showorklist, utvalgObj]);
    }
  };

  const getSelectedRoutes = (data) => {
    let selectedArray = [];
    let selectedRoutes = data.reduce((acc, dt) => {
      if (!(dt.children === undefined)) {
        return acc.concat(getSelectedRoutes(dt.children));
      } else {
        selectedArray.push(dt.key);
      }
      return acc.concat(dt);
    }, []);
    setSelectedKeys(selectedArray);
    return selectedRoutes;
  };
  const openSelection = async (id) => {
    setActivUtvalglist({});

    setutvalglistcheck(false);
    let url = `Utvalg/GetUtvalg?utvalgId=${id}`;
    try {
      //api.logger.info("APIURL", url);
      const { data, status } = await api.getdata(url);
      if (data.length === 0) {
        //api.logger.error("Error : No Data is present for mentioned Id" + id);
        setloading(false);
      } else {
        // await setActivUtvalg({});

        createActiveUtvalg(data);
        // setResultData(groupBy(data.reoler, "", 0, 0, 0, 0, []));
        let result = groupBy(
          data.reoler,
          "",
          0,
          showHousehold,
          showBusiness,
          showReservedHouseHolds,
          []
        );
        setResultData(result);
        //setvalue(false);
        //setissave(true);
        //setutvalglistcheck(false);
        //setAktivDisplay(true);
        setloading(false);
        showorklist?.map((item) => {
          if (item?.memberUtvalgs?.length > 0) {
            item?.memberUtvalgs?.map((i) => {
              if (i?.utvalgId === data?.utvalgId) {
                let arr = [];
                arr?.push(item?.listId?.toString());
                setExpandListId(arr);
              }
            });
          }
        });
      }
    } catch (error) {
      //api.logger.error("errorpage API not working");
      //api.logger.error("error : " + error);
      setloading(false);
    }
  };

  useEffect(async () => {
    setGroupData(resultData);
  }, [resultData]);

  const openList = async (id) => {
    setActivUtvalg({});
    setCheckedList([]);

    let url = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${id}`;

    try {
      //api.logger.info("APIURL", url);
      const { data, status } = await api.getdata(url);
      if (data.length === 0) {
        //api.logger.error("Error : No Data is present for mentioned Id" + id);
        setloading(false);
      } else {
        let obj = await CreateUtvalglist(data);
        setActivUtvalglist(obj);

        // showorklist.map((item) => {
        //   if (item?.listId?.toString() !== obj?.listId?.toString()) {
        //     worklistArray.push(item);
        //   }
        // });
        worklistArray = worklistArray.filter((item) => {
          return item?.listId?.toString() !== obj?.listId?.toString();
        });
        worklistArray.push(obj);
        // const duplicatelistId = worklistArray.map((o) => o.listId);
        // const filteredArray = worklistArray.filter(
        //   ({ listId }, index) => !duplicatelistId.includes(listId, index + 1)
        // );
        let commonUtvalgs = [];
        let commonLists = [];
        //Deep filtering after basic outer filteration
        showorklist.map((val) => {
          //case where redundant utvalgs present in showorklist and inside newly added list of list
          if (val?.utvalgId !== undefined && val?.utvalgId !== 0) {
            if (obj?.listId && obj?.memberLists?.length > 0) {
              obj.memberLists?.map((i) => {
                if (i.memberUtvalgs.length) {
                  i.memberUtvalgs.map((j) => {
                    if (j.utvalgId === val.utvalgId) {
                      commonUtvalgs.push(val.utvalgId);
                    }
                  });
                }
              });
            }
          }
          //case where redundant lists present in list of list of showorklist and inside newly added list
          if (val?.listId !== undefined && val?.memberLists?.length > 0) {
            if (obj?.listId && obj?.memberUtvalgs?.length > 0) {
              val.memberLists?.map((i) => {
                if (i.listId === obj.listId) {
                  commonLists.push(obj.listId);
                }
              });
            }
          }
          //case where redundant lists present in list of showorklist and inside newly added list of list
          if (val?.listId !== undefined && val?.memberUtvalgs?.length > 0) {
            if (obj?.listId && obj?.memberLists?.length > 0) {
              obj.memberLists?.map((i) => {
                if (i.listId === val.listId) {
                  commonLists.push(val.listId);
                }
              });
            }
          }
        });

        const filteredUtvalgArray = worklistArray.filter((item) => {
          return !commonUtvalgs.includes(item.utvalgId);
        });
        const filteredListArray = filteredUtvalgArray.filter((item) => {
          return !commonLists.includes(item.listId);
        });

        setshoworklist(filteredListArray);

        // let arr = [];
        // arr.push(obj?.listId?.toString());
        setExpandListId([]);
        setvalue(false);
        setissave(true);
        setutvalglistcheck(true);
        setAktivDisplay(true);
        setloading(false);
      }
    } catch (error) {
      console.log(error);
      //api.logger.error("errorpage API not working");
      //api.logger.error("error : " + error);
      setloading(false);
    }
  };
  const handleOpen = async (e) => {
    let newFlag = true;
    setDetails(false);
    setCheckedList([]);
    selectedRecreateGroups.map((item) => {
      if (item?.isRecreated === false) {
        // setIsRecreate(false);
        if (item?.utvalgId) {
          setrecreateId(item?.utvalgId);
          setrecreateType("U");
        } else {
          setrecreateId(item?.listId);
          setrecreateType("L");
        }
        newFlag = false;
      }
    });
    setloading(true);
    setBudruteendringer(true);
    setIsWidgetActive(false);
    // debugger;

    if (!newFlag) {
      await setModal("openRecreatePopup");

      setloading(false);
    } else {
      setModal("");
      //First check if there is any data selected
      if (selectedGroups.length > 0) {
        let j = mapView.graphics.items.length;
        //Clear Map
        for (var i = j; i > 0; i--) {
          if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
            mapView.graphics.remove(mapView.graphics.items[i - 1]);
            //j++;
          }
        }

        //mapView.graphics.removeAll();
        setShowHousehold(false);
        setShowReservedHouseHolds(false);
        setShowBusiness(false);
        //Check if its selection or selection List
        if (selectedGroups.filter((x) => x.indexOf("U") >= 0).length > 0) {
          try {
            openSeletions(selectedGroups);
          } catch (error) {}
        } else if (
          selectedGroups.filter((x) => x.indexOf("L") >= 0).length > 0
        ) {
          selectedGroups.map((item) => {
            let id = item;
            id = id.substring(1);
            // setShowReserverte(false);
            openList(id);
          });
        }
      }
    }
  };

  const openSeletions = async (ids) => {
    const promises = ids.map(async (selectionId) => {
      await openSelection(selectionId.substring(1));
    });

    await Promise.all(promises);
    setvalue(false);
    setissave(true);
    setutvalglistcheck(false);
    setAktivDisplay(true);
    //setActivUtvalg(utvalgObject);
  };

  const imgLoader = (path) => {
    return require("../assets/images/Icons/" + path);
  };

  const renderPerson1 = (item) => {
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
      //let list = !item.listId ? false : true;
      if (item.basedOn > 0) {
        Image = GetImageUrl("kampanje", item.isBasis, list, item.ordreType);
      } else {
        Image = GetImageUrl("utvalg", item.isBasis, list, item.ordreType);
      }
    }

    return <img className="mb-1" src={imgLoader(Image)} />;
  };

  const renderPerson = (resultvalue, index) => {
    return resultvalue.map((item) => (
      <tr key={!item.utvalgId ? item.listId : item.utvalgId}>
        <th className="minwidth25 b-0 p-0 pl-1" scope="row ">
          <input
            className="mt-1"
            type="checkbox"
            name="selectionlistname"
            value=""
            id={!item.utvalgId ? "L" + item.listId : "U" + item.utvalgId}
            onChange={() => handleChange(item)}
          />
          {renderPerson1(item)}
        </th>
        <td className="tr-font1 finClick_css2 borderless">{item.name} </td>
        <td className="tr-font1 borderless text-right p-1 nowrap">
          {item.kundeNummer === 0 ||
          item.kundeNummer === undefined ||
          item.kundeNummer === "0"
            ? ""
            : item.kundeNummer}
        </td>
        <td className="tr-font1 borderless text-right nowrap">
          {item.totalAntall != null
            ? NumberFormat(Number(item.totalAntall))
            : NumberFormat(item.antall)}
        </td>
      </tr>
    ));
  };

  return (
    <div>
      {Modal == "openRecreatePopup" ? (
        <ShowRecreate
          id={"uxBtnOpen"}
          recreateId={recreateId}
          recreateType={recreateType}
        />
      ) : null}
      <div className="row col-12 m-0 p-0">
        <div className="row col-12 m-0 p-0 Sok_Maxwidth">
          <div
            className={
              props.result.length == 1
                ? "bg-color-finn"
                : " finClick_css bg-color-finn"
            }
          >
            <table
              className={
                props.result.length == 1
                  ? " borderless "
                  : " finClick_css1 borderless"
              }
            >
              <thead>
                <tr className="tr-font borderless ">
                  <th
                    className="col-1 minwidth45 b-0 p-0 pl-1 responsive-text-FinClick"
                    scope=""
                  >
                    Velg{" "}
                  </th>
                  {/* <th className="col-2" scope=""></th> */}
                  <th
                    className="col-6 borderless responsive-text-FinClick"
                    scope="col"
                  >
                    Navn
                  </th>
                  <th
                    className="col-2 borderless text-right responsive-text-FinClick"
                    scope="col"
                  >
                    Kunde
                  </th>
                  <th
                    className="col-2 borderless text-right responsive-text-FinClick"
                    scope="col"
                  >
                    Antall
                  </th>
                </tr>
              </thead>
              <tbody>{refresh ? null : renderPerson(ApiResult)}</tbody>
            </table>
          </div>
        </div>
        <div className="row col-12 m-0 p-0 p-2">
          <div className="col-6 p-0 _flex-start">
            <input
              type="submit"
              className="KSPU_button"
              value="Slett"
              onClick={slettClick}
            />
          </div>
          <div className="col-6 p-0 _flex-end">
            <input
              type="button"
              className="KSPU_button"
              disabled={btnDisabled}
              onClick={handleOpen}
              value="Åpne"
              data-toggle="modal"
              data-target="#uxBtnOpen"
            />
            {loading ? (
              <img
                src={loadingImage}
                style={{
                  width: "20px",
                  height: "20px",
                  position: "absolute",
                  left: "122px",
                  zindex: 100,
                }}
              />
            ) : null}
          </div>
        </div>
      </div>
    </div>
  );
}
export default FinnClick;
