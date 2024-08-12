import React, { useEffect, useState, useContext, useRef } from "react";
import "../App.css";
import expand from "../assets/images/esri/expand.png";
import collapse from "../assets/images/esri/collapse.png";
import Helmet from "react-helmet";
import { KSPUContext } from "../context/Context.js";
import { groupBy } from "../Data";

function Mottakergrupper(props) {
  const [togglevalue, settogglevalue] = useState(false);
  const {
    showBusiness,
    setShowBusiness,
    showHousehold,
    setShowHousehold,
    activUtvalg,
    setSelectionUpdate,
    setResultData,
    groupData,
    setGroupData,
  } = useContext(KSPUContext);
  const { showReservedHouseHolds, setShowReservedHouseHolds } =
    useContext(KSPUContext);
  const { selecteddemografiecheckbox, setselecteddemografiecheckbox } =
    useContext(KSPUContext);
  const { selectedsegment, setselectedsegment } = useContext(KSPUContext);
  const inputChk = useRef();
  const inputChk1 = useRef();
  const notshowReserver = useRef();
  const showReserved = useRef();

  const toggle = () => {
    settogglevalue(!togglevalue);
  };

  useEffect(async () => {
    if (groupData.length > 0) {
      setResultData(groupData);
      setGroupData("");
    } else {
      if (Object.keys(activUtvalg).length !== 0) {
        let data = await groupBy(
          activUtvalg.reoler,
          "",
          0,
          showHousehold,
          showBusiness,
          showReservedHouseHolds,
          []
        );
        data = data.sort(function (a, b) {
          return Intl.Collator("no", { numeric: true }).compare(a.name, b.name);
        });
        data?.map((fylk) => {
          if (fylk?.children?.length > 0) {
            fylk.children = fylk?.children.sort(function (a, b) {
              return Intl.Collator("no", { numeric: true }).compare(
                a.name,
                b.name
              );
            });
            fylk?.children?.map((kommune) => {
              if (kommune?.children?.length > 0) {
                kommune.children = kommune?.children.sort(function (a, b) {
                  return Intl.Collator("no", { numeric: true }).compare(
                    a.name,
                    b.name
                  );
                });
                kommune?.children?.map((team) => {
                  if (team?.children?.length > 0) {
                    team.children = team?.children.sort(function (a, b) {
                      return Intl.Collator("no", { numeric: true }).compare(
                        a.name,
                        b.name
                      );
                    });
                  }
                });
              }
            });
          }
        });
        await setResultData(data);
      }
    }
  }, [showBusiness, showReservedHouseHolds, showHousehold]);

  const handleHouseholdClick = async (e) => {
    setShowHousehold(inputChk1.current.checked);
    if (!inputChk1.current.checked) {
      if (
        Object.keys(activUtvalg).length !== 0 &&
        activUtvalg?.receivers?.length > 0
      ) {
        if (showReservedHouseHolds) {
          setShowReservedHouseHolds(false);
          showReserved.current.checked = false;
          activUtvalg.receivers.map((item) => {
            if (item.receiverId === 1 && item.selected === true) {
              item.selected = false;
            }
            if (item.receiverId === 5 && item.selected === true) {
              item.selected = false;
            }
          });
        } else {
          activUtvalg.receivers.map((item) => {
            if (item.receiverId === 1 && item.selected === true) {
              item.selected = false;
            }
          });
        }
      }
    }
    setSelectionUpdate(true);
  };
  const handleBusinessClick = async (e) => {
    setShowBusiness(inputChk.current.checked);
    if (!inputChk.current.checked) {
      if (
        Object.keys(activUtvalg).length !== 0 &&
        activUtvalg?.receivers?.length > 0
      ) {
        activUtvalg.receivers.map((item) => {
          if (item.receiverId === 4 && item.selected === true) {
            item.selected = false;
          }
        });
      }
    }
    setSelectionUpdate(true);
  };

  const handleShowReseverd = async (e) => {
    setShowReservedHouseHolds(true);
    if (
      Object.keys(activUtvalg).length !== 0 &&
      activUtvalg?.receivers?.length > 0
    ) {
      activUtvalg.receivers.map((item) => {
        if (item.receiverId === 5 && item.selected === false) {
          item.selected = true;
        }
      });
    }

    setSelectionUpdate(true);
    notshowReserver.current.checked = false;
  };
  const handleNotShowReseverd = () => {
    setShowReservedHouseHolds(false);

    if (
      Object.keys(activUtvalg).length !== 0 &&
      activUtvalg?.receivers?.length > 0
    ) {
      activUtvalg.receivers.map((item) => {
        if (item.receiverId === 5 && item.selected === true) {
          item.selected = false;
        }
      });
    }

    setSelectionUpdate(true);
    showReserved.current.checked = false;
  };

  useEffect(() => {
    if (props.page === "DTPage") settogglevalue(false);
    else settogglevalue(true);

    if (
      Object.keys(activUtvalg).length !== 0 &&
      activUtvalg.receivers.length > 0
    ) {
      activUtvalg.receivers.map((item) => {
        if (activUtvalg.name === "Påbegynt utvalg" && !showBusiness) {
          setShowBusiness(false);
          let temp = activUtvalg;
          temp = temp.receivers.filter((result) => {
            return result.receiverId !== 4;
          });
          activUtvalg.receivers = temp;
        } else {
          if (item.receiverId === 4 && item.selected === true) {
            setShowBusiness(true);
          } else if (item.receiverId !== 4 && showBusiness) {
            activUtvalg.receivers.push({ receiverId: 4, selected: true });
          }
        }
        if (activUtvalg.name === "Påbegynt utvalg" && !showHousehold) {
          setShowHousehold(false);
          let temp = activUtvalg;
          temp = temp.receivers.filter((result) => {
            return result.receiverId !== 1;
          });
          activUtvalg.receivers = temp;
        } else {
          if (item.receiverId === 1 && item.selected === true) {
            setShowHousehold(true);
          } else if (item.receiverId !== 1 && showHousehold) {
            activUtvalg.receivers.push({ receiverId: 1, selected: true });
          }
        }
        if (activUtvalg.name === "Påbegynt utvalg" && !showReservedHouseHolds) {
          setShowReservedHouseHolds(false);
          notshowReserver.current.checked = true;
          let temp = activUtvalg;
          temp = temp.receivers.filter((result) => {
            return result.receiverId !== 5;
          });
          activUtvalg.receivers = temp;
        } else {
          if (item.receiverId === 5 && item.selected === true) {
            setShowReservedHouseHolds(true);
          } else if (item.receiverId !== 5 && showReservedHouseHolds) {
            activUtvalg.receivers.push({ receiverId: 5, selected: true });
          }
        }
      });
    } else {
      notshowReserver.current.checked = true;
      setShowReservedHouseHolds(false);
      setShowBusiness(false);
      setShowHousehold(true);
    }
    notshowReserver.current.checked = true;
  }, []);
  return (
    <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
      <div className="card Kj-background-color col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <Helmet>
          <style>
            {`.form-check-input {
                  position: absolute;
                  // margin-top: .3rem;
                  margin-top: ${props.marginTop};
                  margin-left: -1.25rem;
              }
              `}
          </style>
        </Helmet>
        <div className="row">
          <div className="col-8">
            <p className="avan p-1 ">MOTTAKERGRUPPER</p>
          </div>
          <div className="col-4" onClick={toggle}>
            {!togglevalue ? (
              <img className="d-flex float-right pt-1 mr-1" src={collapse} />
            ) : (
              <img className="d-flex float-right pt-1 mr-1" src={expand} />
            )}
          </div>
        </div>

        {!togglevalue ? (
          <div className="Kj-div-background-color pt-2 pb-2 col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
            <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
              <div className="col-lg-6 col-md-6 col-sm-6 col-xs-6 m-0 p-0 pl-4 ">
                {/* <div > */}
                <label className="form-check-label" htmlFor="Hush">
                  {" "}
                  <input
                    className="form-check-input "
                    type="checkbox"
                    value=""
                    id="Hush"
                    ref={inputChk1}
                    checked={showHousehold}
                    onChange={handleHouseholdClick}
                  />
                  Husholdninger{" "}
                </label>
                {/* </div>             */}
              </div>
              {selecteddemografiecheckbox.length > 0 ||
              selectedsegment.length > 0 ? null : (
                <div className="col-lg-6 col-md-6 col-sm-6 col-xs-6 m-0 p-0 pl-4 ">
                  <label className="form-check-label label-text" htmlFor="Virk">
                    {" "}
                    <input
                      className="form-check-input  "
                      ref={inputChk}
                      type="checkbox"
                      value=""
                      id="Virk"
                      checked={showBusiness}
                      onChange={handleBusinessClick}
                    />
                    Virksomheter{" "}
                  </label>
                </div>
              )}
            </div>
            <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 pt-1 sok-text">
              <div className="col-lg-6 col-md-6 col-sm-6 col-xs-6 m-0 p-0 pl-1">
                <span className="label">Inkludert reserverte?</span>
              </div>
              <div className="col-lg-6 col-md-6 col-sm-6 col-xs-6 m-0 p-0 pl-1">
                <div className="_flex-end mr-1">
                  <label className="radio-inline sok-text">
                    <input
                      type="radio"
                      ref={notshowReserver}
                      onChange={handleNotShowReseverd}
                      value=""
                      checked={!showReservedHouseHolds}
                    />
                    Nei
                  </label>
                  <label className="radio-inline sok-text pl-1">
                    <input
                      id="RadioPlukkliste"
                      ref={showReserved}
                      onChange={handleShowReseverd}
                      type="radio"
                      value=""
                      checked={showReservedHouseHolds}
                      disabled={
                        (showHousehold && showBusiness) || showHousehold
                          ? false
                          : true
                      }
                    />
                    Ja
                  </label>
                </div>
              </div>
            </div>
          </div>
        ) : null}
      </div>
    </div>
  );
}

export default Mottakergrupper;
