import React, { useState, useRef, useContext, useEffect } from "react";
import Calender from "./Calendar";
import { KSPUContext, MainPageContext } from "../context/Context.js";
import api from "../services/api.js";
import moment from "moment";
import { NumberFormat, CreateActiveUtvalg } from "../common/Functions";

import loadingImage from "../assets/images/callbackActivityIndicator.gif";

function DistributionDetails(props) {
  const [Selecteddate, setSelecteddate] = useState("");
  const [enable, setenable] = useState(true);
  const [weightFlag, setWeightFlag] = useState(false);
  const {
    setAktivDisplay,
    activUtvalglist,
    setActivUtvalglist,
    utvalglistcheck,
    setDetails,
    showPriceCal,
    setshowPriceCal,
    activUtvalg,
    internuserName,
    setinternuserName,
    setshoworklist,
    showorklist,
    setActivUtvalg,
  } = useContext(KSPUContext);
  const [thicknessFlag, setThicknessFlag] = useState(false);
  const [showCalenderComp, setShowCalenderComp] = useState(false);

  const { mapView } = useContext(MainPageContext);
  const [visDistributionDate, setVisDistributionDate] = useState(true);
  const [loading, setloading] = useState(false);
  const [distWeight, setDistWeight] = useState(
    activUtvalg.weight !== "" && activUtvalg.weight > 0
      ? activUtvalg.weight
      : ""
  );
  const [distThickness, setDistThickness] = useState(
    activUtvalg.thickness !== "" && activUtvalg.thickness > 0
      ? activUtvalg.thickness
      : ""
  );
  const [tempDistWeight, setTempDistWeight] = useState("");
  const [tempDistThickness, setTempDistThickness] = useState("");

  const [weekType, setWeekType] = useState("S");
  const [selectionId, setSelectionId] = useState("");
  const [selectionType, setSelectionType] = useState("");
  const [removeReolId, setRemoveReolId] = useState([]);
  const [selectedReolId, setSelectedReolId] = useState([]);
  const [distributionTypeValue, setDistributionTypeValue] = useState(false);

  const [defultSelectedDate, setDefaultSelectedDate] = useState("");

  const ConfiguratorCall = () => {
    var kundenumber;
    setenable(false);
    if (utvalglistcheck) {
      kundenumber = activUtvalglist.kundeNummer;
    } else {
      kundenumber = activUtvalg.kundeNummer;
    }
    setloading(false);
    window.open(
      `${process.env.REACT_APP_CONFIGURATOR_INTERN_URL}?utvalgstype=${selectionType}&utvalgid=${selectionId}&kundenummer=${kundenumber}&xxcu_thicknessVal=${distThickness}`
    );
  };

  const SaveSelectionwithDistributionDate = async () => {
    setloading(true);
    let saveOldReoler = "false";
    let skipHistory = "false";
    let forceUtvalgListId = 0;
    let url = "";
    if (!utvalglistcheck) {
      url = `Utvalg/SaveUtvalg?userName=${"Internbruker"}&`;
      url = url + `saveOldReoler=${saveOldReoler}&`;
      url = url + `skipHistory=${skipHistory}&`;
      url = url + `forceUtvalgListId=${forceUtvalgListId}`;
      var A = activUtvalg;
      A.weight = parseInt(distWeight);
      A.thickness = Number(distThickness);
      A.distributionDate = Selecteddate;
      if (weekType == "B") {
        A.distributionType = 2;
      } else if (weekType == "S") {
        A.distributionType = 1;
      } else {
        A.distributionType = 0;
      }
    } else {
      let listDistributionType = 0;
      if (weekType == "B") {
        listDistributionType = 2;
      } else if (weekType == "S") {
        listDistributionType = 1;
      } else {
        listDistributionType = 0;
      }
      url = `UtvalgList/UpdateUtvalgListDistributionData?userName=${"Internbruker"}`;
      var objRequest = {
        listId: activUtvalglist?.listId,
        weight: parseInt(distWeight),
        distributionType: listDistributionType,
        distributionDate: Selecteddate,
        thickness: Number(distThickness),
        ruteId: removeReolId,
      };
    }
    if (!utvalglistcheck) {
      try {
        const { data, status } = await api.postdata(url, A);

        if (status === 200) {
          let j = mapView.graphics.items.length;
          for (var i = j; i > 0; i--) {
            if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
              mapView.graphics.remove(mapView.graphics.items[i - 1]);
            }
          }
          let obj = await CreateActiveUtvalg(data);
          await setActivUtvalg(obj);
          setDetails(false);

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
                      item.memberLists[y].antall -= i.totalAntall;
                      item.memberLists[y].antall += data.totalAntall;
                      item.antall -= i.totalAntall;
                      item.antall += data.totalAntall;
                      it.memberUtvalgs[index] = data;
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
                  item.antall -= i.totalAntall;
                  item.antall += data.totalAntall;
                  item.memberUtvalgs[index] = data;
                }
              });
            } else if (
              JSON.parse(item?.utvalgId) === JSON.parse(activUtvalg?.utvalgId)
            ) {
              item.totalAntall = data.totalAntall;
              item.modifications[0].modificationTime =
                data?.modifications[0]?.modificationTime;
              item.antallBeforeRecreation = data?.antallBeforeRecreation;
              item.reoler = data?.reoler;
            }
          });
          setshoworklist([...showorklist]);

          // activUtvalg.weight = parseInt(distWeight);
          // activUtvalg.thickness = Number(distThickness);
          // activUtvalg.distributionDate = Selecteddate;
          ConfiguratorCall();
        }
      } catch (e) {
        console.log(e);
      }
    } else {
      try {
        const { data, status } = await api.putData(url, objRequest);
        if (status === 200) {
          setDetails(false);
          let url = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${activUtvalglist?.listId}`;
          // `UtvalgList/GetUtvalgList?listId=${id}&getParentList=${true}&getMemberUtvalg=${true}`;
          try {
            //api.logger.info("APIURL", url);
            const { data, status } = await api.getdata(url);
            if (data.length === 0) {
              //api.logger.error("Error : No Data is present for mentioned Id" + id);
            } else {
              let activ = showorklist;
              var newActiv = [];
              newActiv = activ.filter((item) => {
                return item.listId !== activUtvalglist?.listId;
              });
              newActiv.push(data);
              await setshoworklist(newActiv);
              await setActivUtvalglist({});
              // let obj = await CreateUtvalglist(data);
              await setActivUtvalglist(data);
              setloading(false);
            }
          } catch (error) {
            //api.logger.error("errorpage API not working");
            //api.logger.error("error : " + error);
          }
          // activUtvalglist.weight = parseInt(distWeight);
          // activUtvalglist.thickness = Number(distThickness);
          // activUtvalglist.distributionDate = Selecteddate;
          ConfiguratorCall();
        }
      } catch (e) {
        console.log(e);
      }
    }
  };

  const handleCallback = (
    childData,
    buttonFlag,
    removedRoutes,
    bookedReolID
  ) => {
    let deleteReolId = [];
    if (removedRoutes && utvalglistcheck) {
      if (removedRoutes?.ruteInfo) {
        removedRoutes?.ruteInfo.map((item) => {
          deleteReolId.push(item.ruteId);
        });
      }

      setRemoveReolId(deleteReolId);
      // console.log("removedRoutes", removedRoutes);
      // console.log("deleteReolId", deleteReolId);
    }
    if (bookedReolID) {
      setSelectedReolId(bookedReolID);
    }

    if (buttonFlag) {
      deleteReolId = [];
      setenable(true);
    } else {
      setenable(false);
    }

    let parsedDate = moment(childData, "DD.MM.YYYY H:mm:ss");

    let distributiondate = parsedDate.toLocaleString().slice(0, -5);

    setSelecteddate(distributiondate);
  };
  const distributionWeight = (e) => {
    setShowCalenderComp(false);
    setenable(true);
    let distWeightValue = e.target.value.toString().replace(",", ".");

    setDistWeight(Number(distWeightValue));
    setTempDistWeight(e.target.value);
    if (
      Number(distWeightValue) > 200 ||
      Number(distWeightValue).toString() === "NaN" ||
      Number(distWeightValue) % 1 !== 0 ||
      Number(distWeightValue) <= 3.5 ||
      distWeightValue.toString().includes(",") ||
      distWeightValue.toString().includes(".")
    ) {
      setWeightFlag(true);
    } else {
      setWeightFlag(false);
    }
    if (thicknessFlag) {
      setVisDistributionDate(true);
    }
    if (weightFlag) {
      setVisDistributionDate(true);
    }

    if (distWeight && distThickness) {
      setVisDistributionDate(false);
    }
  };
  useEffect(() => {
    if (utvalglistcheck) {
      setSelectionId(activUtvalglist.listId);
      setSelectionType("L");
      if (activUtvalglist?.distributionDate !== "") {
        let formatchange = new Date(activUtvalglist?.distributionDate);
        formatchange = new Date(formatchange);
        setDefaultSelectedDate(formatchange);
      }
      if (activUtvalglist?.weight) {
        setTempDistWeight(activUtvalglist?.weight);
        setDistWeight(activUtvalglist?.weight);
      }
      if (activUtvalglist?.thickness) {
        setTempDistThickness(activUtvalglist?.thickness);
        setDistThickness(activUtvalglist?.thickness);
      }
      if (activUtvalglist?.thickness > 0 && activUtvalglist?.weight > 0) {
        setVisDistributionDate(false);
        setShowCalenderComp(true);
      } else {
        setVisDistributionDate(true);
      }
      if (
        activUtvalglist?.distributionType !== "" &&
        activUtvalglist?.distributionType !== "undefined"
      ) {
        if (activUtvalglist?.distributionType == "2") {
          setWeekType("B");
          setDistributionTypeValue(true);
        } else {
          setWeekType("S");
          setDistributionTypeValue(false);
        }
      } else {
        setWeekType("S");
        setDistributionTypeValue(false);
      }
    } else {
      setSelectionId(activUtvalg.utvalgId);
      setSelectionType("U");
      if (activUtvalg?.distributionDate !== "") {
        let formatchange = new Date(activUtvalg?.distributionDate);
        formatchange = new Date(formatchange);
        setDefaultSelectedDate(formatchange);
      }
      if (activUtvalg?.weight) {
        setTempDistWeight(activUtvalg?.weight);
        setDistWeight(activUtvalg?.weight);
      }
      if (activUtvalg?.thickness) {
        setTempDistThickness(activUtvalg?.thickness);
      }
      if (activUtvalg?.thickness > 0 && activUtvalg?.weight > 0) {
        // setbuttonenable(true);
        setVisDistributionDate(false);
        setShowCalenderComp(true);
      } else {
        setVisDistributionDate(true);
      }
      if (
        activUtvalg?.distributionType !== "" &&
        activUtvalg?.distributionType !== "undefined"
      ) {
        if (activUtvalg?.distributionType == "2") {
          setWeekType("B");
          setDistributionTypeValue(true);
        } else {
          setWeekType("S");
          setDistributionTypeValue(false);
        }
      } else {
        setWeekType("S");
        setDistributionTypeValue(false);
      }
    }
    // if (   parseInt(distthickness) > 0 ||   (parseInt(distthicknessmm) > 0 &&
    // parseInt(distthicknessmm) > 0) ) {   setbuttonenable(false);
    // setShowCalenderComp(true); }
  }, []);
  const ShowCalender = async () => {
    setShowCalenderComp(true);
    setenable(true);
  };
  const distributionThickness = (e) => {
    setShowCalenderComp(false);
    setenable(true);
    let distthincknessmm = e.target.value;
    let Temp = distthincknessmm.toString().replace(",", ".");
    setDistThickness(Number(Temp));
    setTempDistThickness(e.target.value);
    if (!distthincknessmm) {
      setVisDistributionDate(true);
    }
    if (
      Number(Temp) > 5 ||
      Number(Temp) <= 0 ||
      Number(Temp).toString() === "NaN"
    ) {
      setThicknessFlag(true);
    } else {
      setThicknessFlag(false);
    }

    if (thicknessFlag) {
      setVisDistributionDate(true);
    }
    if (weightFlag) {
      setVisDistributionDate(true);
    } else {
      setVisDistributionDate(false);
    }
    if (distWeight && distThickness) {
      setVisDistributionDate(false);
    }
  };
  const tillBack = (e) => {
    let j = mapView.graphics.items.length;
    for (var i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
      }
    }
    setDetails(false);
    setshowPriceCal(false);
    setAktivDisplay(true);
  };
  const SelectRadio = (value, e) => {
    if (value == "B" && e.target.value == "on") {
      setWeekType("B");
      setDistributionTypeValue(true);
    } else {
      setWeekType("S");
      setDistributionTypeValue(false);
    }
  };

  return (
    <div className="card row col-lg-12 col-md-12 col-sm-12 col-xs-12 p-0 m-0">
      <span className="sok-text1 ">Distribusjonsdetaljer</span>
      <div className="Distribution_Div row col-lg-12 col-md-12 col-sm-12 col-xs-12 p-0 m-0">
        <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 p-0 m-0">
          <div className="col-5 Distribution_label m-0 p-0 pl-1 mt-2 mb-1">
            <span>Antall</span>
          </div>
          {!utvalglistcheck ? (
            <div className="col-7 divnumberText m-0 p-0 pl-3 mt-2 mb-1">
              <span>{NumberFormat(activUtvalg.totalAntall)}</span>
            </div>
          ) : (
            <div className="col-7 divnumberText m-0 p-0 pl-3 mt-2 mb-1">
              <span>{NumberFormat(activUtvalglist.antall)}</span>
            </div>
          )}
        </div>
        <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 p-0 m-0">
          <div className="col-5 Distribution_label m-0 p-0 pl-1">
            <span>Vekt pr. sending</span>
          </div>
          <div className="col-3">
            <input
              type="text"
              id="uxDist_Thickness"
              value={tempDistWeight.toString().replace(".", ",")}
              onChange={distributionWeight}
              className="divnumberText DistWeight_Intrn"
              maxLength="4"
              // onkeyup="Distr.gui.thickness.validate();Distr.gui.thickness.onEnter(event);"
              // onBlur="Distr.gui.thickness.thicknessWarning();"
            />
            &nbsp;{" "}
            {(weightFlag && !showPriceCal) ||
            (showPriceCal &&
              (tempDistWeight.toString().includes(",") ||
                tempDistWeight.toString().includes(".") ||
                Number(distWeight).toString() === "NaN")) ? (
              <span id="uxWeightValErr" className="red">
                <b>!</b>
              </span>
            ) : null}
          </div>
          <div className="col-4 Distribution_label">
            {" "}
            <span>gram (maks 200g)</span>
          </div>
        </div>
        <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 p-0 m-0">
          <div className="col-12">
            <a
              href="http://www.bring.no/tjenester/klargjoring#uadressert-post"
              className="KSPU_LinkButton_Url Distribution_Text float-right"
              target="_blank"
              id="uxDistribusjon_vektInformationIW"
            >
              Hjelp til vektberegning
            </a>
          </div>
        </div>
        <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 p-0 m-0">
          <div className="col-5 Distribution_label m-0 p-0 pl-1">
            <span>Tykkelse pr. sending</span>
          </div>
          <div className="col-3">
            <input
              type="text"
              id="uxDistThicknessmm"
              className="divnumberText DistWeight_Intrn"
              maxLength="4"
              value={tempDistThickness.toString().replace(".", ",")}
              onChange={distributionThickness}
            />
            &nbsp;{" "}
            {(thicknessFlag && !showPriceCal) ||
            (showPriceCal && Number(distThickness).toString() === "NaN") ? (
              <span id="uxThicknessValErr" className="red">
                <b>!</b>
              </span>
            ) : null}
          </div>
          <div className="col-4 Distribution_label">
            {" "}
            <span>mm (maks 5mm)</span>
          </div>
        </div>
        <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 p-0 m-0 Distribution_Text pt-2 pb-2">
          <div className="col-12 m-0 p-0 pl-1">
            <input
              type="button"
              value="Vis mulige distribusjonsdatoer"
              disabled={
                showPriceCal
                  ? Number(distThickness) <= 0 ||
                    distWeight === "" ||
                    distThickness === "" ||
                    Number(distWeight) <= 0 ||
                    Number(distWeight) % 1 !== 0 ||
                    tempDistWeight.toString().includes(",") ||
                    tempDistWeight.toString().includes(".") ||
                    Number(distThickness).toString() === "NaN" ||
                    Number(distWeight).toString() === "NaN"
                  : Number(distWeight) > 200 ||
                    Number(distWeight) <= 3.5 ||
                    Number(distThickness) > 5 ||
                    Number(distThickness) <= 0 ||
                    Number(distWeight) % 1 !== 0 ||
                    tempDistWeight.toString().includes(",") ||
                    tempDistWeight.toString().includes(".") ||
                    distWeight === "" ||
                    distThickness === "" ||
                    thicknessFlag ||
                    weightFlag
              }
              onClick={ShowCalender}
            />
          </div>
        </div>
      </div>
      {showCalenderComp ? (
        <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 p-0 m-0 Distribution_Div mt-2">
          <div className="col-12 m-0 p-0 pl-1">
            <span className="Distribution_Text1 pt-2">
              Velg distribusjonsperiode
            </span>
          </div>
          <div className="col-12 m-0 p-0 pl-1">
            <div className="row Distribution_Text pt-2 mb-2 m-0 p-0 pl-1">
              <div className="col-6 m-0 p-0">
                <input
                  type="radio"
                  name="optradio"
                  onChange={(e) => SelectRadio("S", e)}
                  checked={!distributionTypeValue}
                />{" "}
                tidliguke
              </div>
              <div className="col-6 m-0 p-0">
                <input
                  type="radio"
                  name="optradio"
                  onChange={(e) => SelectRadio("B", e)}
                  checked={distributionTypeValue}
                />{" "}
                midtuke
              </div>
            </div>
          </div>
          <div
            className="col-12 no-padding"
            style={{
              backgroundColor: "#fff",
            }}
          >
            <Calender
              page="DTPage_2"
              fontSize="11pt"
              parentCallback={handleCallback}
              weight={distWeight}
              thickness={distThickness}
              type={selectionType}
              selection={weekType}
              UtvalgID={selectionId}
              defaultDate={defultSelectedDate}
              newselectedReolId={selectedReolId}
              Calendar={showPriceCal ? "PriceCalculation" : "normalCalendar"}
            />
          </div>
        </div>
      ) : null}

      <div className="row p-1">
        <div className="col-lg-4 col-md-12">
          <input
            type="submit"
            name="uxDistribusjon$uxBtnDistBack"
            value="Tilbake"
            id="uxDistribusjon_uxBtnDistBack"
            className="Distribution_Text"
            onClick={tillBack}
          ></input>
        </div>
        <div className="col-lg-8 col-md-12 _flex-end">
          <input
            type="submit"
            name="uxDistribusjon$uxDistSetDelivery"
            value="Angi innleveringsdetaljer"
            id="uxDistribusjon_uxDistSetDelivery"
            disabled={enable}
            className={enable ? "KSPU_button_Gray" : "KSPU_button-kw"}
            onClick={(e) => {
              SaveSelectionwithDistributionDate();
            }}
          />
        </div>
        {loading ? (
          <img
            src={loadingImage}
            style={{
              width: "20px",
              height: "20px",
              position: "absolute",
              left: "75px",
              zindex: 100,
            }}
          />
        ) : null}
      </div>
    </div>
  );
}

export default DistributionDetails;
