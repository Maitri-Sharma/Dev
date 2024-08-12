import React, { useState, useContext, useEffect } from "react";
import { KSPUContext } from "../context/Context.js";
import { getCriteriaText } from "./KspuConfig";
import { segmenter_kriterier } from "./KspuConfig";
import PostreklameDataKSPU from "./datadef/PostreklameDataKSPU";
import { groupBy } from "../Data";
import { CreateActiveUtvalg } from "../common/Functions";

function SegmentModel(props, { parentCallback }) {
  const segDataList = segmenter_kriterier();
  const { resultData, setResultData, activUtvalg, setActivUtvalg } =
    useContext(KSPUContext);
  const { selectedsegment, setselectedsegment } = useContext(KSPUContext);
  const { selectedName, setSelectedName } = useContext(KSPUContext);
  const [agder, setAgder] = useState([]);
  const [resultData1, setResultData1] = useState([]);
  const [childrenCheckbox, setChildren] = useState(props.childrenData);
  const [ChangeCheck, setChangeCheck] = useState(Array(10).fill(false));
  const [displayMsg, setDisplayMsg] = useState(false);
  const [checkedList, setCheckedList] = useState([]);
  const [refresh, setRefresh] = useState(false);
  const { showBusiness, showReservedHouseHolds, showHousehold } =
    useContext(KSPUContext);
  const [indexObj, setIndexObj] = useState({});
  useEffect(() => {
    calc(activUtvalg);
  }, []);

  const handleCheckboxChange = (event, reol) => {
    if (event.target.checked) {
      checkedList.push(reol);
    } else {
      var filteredArr = checkedList.filter((val) => {
        return val !== reol;
      });
      setCheckedList(filteredArr);
    }
  };

  const deleteFlyke = (item, i, event) => {
    const _ChangeCheck = [...ChangeCheck];
    _ChangeCheck[i] = event.target.checked;
    setChangeCheck(_ChangeCheck);

    // setChangeCheck(true)
    let agder1 = agder;
    agder1.push(item);
    setAgder(agder1);
    // setResultData(resultData1);
  };

  const getDifference = (a, b) =>
    Object.fromEntries(
      Object.entries(b).filter(([key, val]) => key in a && a[key] !== val)
    );

  const calc = (object) => {
    let Obj = {};
    selectedsegment.map((item, index) => {
      Obj[item] = 0;
    });
    object?.reoler.map((item) => {
      for (let i = 0; i < selectedsegment.length; i++) {
        if (selectedsegment[i] === item.segmentId) {
          if (showReservedHouseHolds) {
            Obj[selectedsegment[i]] =
              Obj[selectedsegment[i]] +
              item?.antall?.households +
              item?.antall?.householdsReserved;
          } else {
            Obj[selectedsegment[i]] =
              Obj[selectedsegment[i]] + item?.antall?.households;
          }
        }
      }
    });
    setIndexObj(Obj);
  };

  const deleteRow = async () => {
    setRefresh(true);
    let newReolArr = [];
    let activObj = JSON.parse(JSON.stringify(activUtvalg));
    activObj?.reoler.map((item) => {
      if (!checkedList?.includes(item.reolId)) {
        newReolArr.push(item);
      }
    });
    activObj.reoler = newReolArr;
    let newUtvagObj = await CreateActiveUtvalg(activObj);
    newUtvagObj.totalAntall =
      (showHousehold ? newUtvagObj?.hush : 0) +
      (showBusiness ? newUtvagObj?.Business : 0) +
      (showReservedHouseHolds ? newUtvagObj?.ReservedHouseHolds : 0);
    calc(activObj);
    // if()
    setActivUtvalg(newUtvagObj);
    let data = groupBy(
      newReolArr,
      "",
      0,
      showHousehold,
      showBusiness,
      showReservedHouseHolds,
      [],
      ""
    );
    setResultData(data);
    setCheckedList([]);
    setTimeout(() => {
      setRefresh(false);
    }, 10);
  };
  return (
    <div>
      {/* <!-- Modal --> */}
      <div
        className="modal fade bd-example-modal-lg"
        id={props.id}
        tabIndex="-1"
        role="dialog"
        aria-labelledby="exampleModalCenterTitle"
        aria-hidden="true"
      >
        <div
          className="modal-dialog modal-dialog-centered resultmaximizer"
          role="document"
        >
          <div className="modal-content">
            <div className="modal-header segFord">
              <h5 className="modal-title " id="exampleModalLongTitle">
                {props.title}
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
            <div className="modal-body flykebody">
              {displayMsg ? (
                <div className="pr-3">
                  <span
                    id="uxKjoreAnalyse_uxLblMessage"
                    className="divErrorText_kw"
                  >
                    Dette utvalget inneholder ingen budruter
                  </span>
                </div>
              ) : (
                <div>
                  <div className="scrollbarmodalNew">
                    <table className="tableRow">
                      <tbody className="pr-4">
                        <tr className="flykeHeader">
                          <th className="tabledataRow segmentModel ">
                            Segment
                          </th>
                          <th className="tabledataRow segmentAntal">Antall</th>
                        </tr>
                        <tr>
                          <td className="tabledataRow">
                            <tr>
                              <td>
                                {selectedName?.map((item) => {
                                  return (
                                    <tr className="flykecontent">
                                      <td>{item}</td>
                                    </tr>
                                  );
                                })}
                              </td>
                              <td>
                                {selectedsegment?.map((item) => {
                                  return (
                                    <tr>
                                      <td className="flykecontent">
                                        - segmentkode{" " + item}
                                      </td>
                                    </tr>
                                  );
                                })}
                              </td>
                            </tr>
                          </td>
                          <td className="tabledataRow">
                            {" "}
                            {Object.keys(indexObj)?.map((key) => {
                              return (
                                <tr>
                                  <td className="flykecontent">
                                    {indexObj[key]}
                                  </td>
                                </tr>
                              );
                            })}
                          </td>
                        </tr>
                      </tbody>
                    </table>
                  </div>
                  <div className="scrollbarmodal">
                    <table className="tableRow">
                      {refresh ? null : (
                        <tbody>
                          <tr className="flykeHeader">
                            <th className="tabledataRow">Fylke</th>
                            <th className="tabledataRow">Kommune</th>
                            <th className="tabledataRow">Team</th>
                            <th className="tabledataRow">Budrute</th>
                            <th className="tabledataRow">Segmentkode</th>
                            <th className="tabledataRow">Antall</th>
                            <th className="tabledataRow">
                              &nbsp;&nbsp;&nbsp;&nbsp;
                            </th>
                          </tr>
                          {activUtvalg?.reoler
                            .sort((a, b) => (a.fylke > b.fylke ? 1 : -1))
                            .map((item, index) => (
                              <tr>
                                <td className="tabledataRow"> {item.fylke}</td>

                                <td className="tabledataRow">
                                  {" "}
                                  {item.kommune}
                                </td>

                                <td className="tabledataRow">
                                  {" "}
                                  {item.teamName}
                                </td>

                                <td className="tabledataRow"> {item.name}</td>

                                <td className="tabledataRow">
                                  {" "}
                                  {item.segmentId}
                                </td>

                                <td className="tabledataRow">
                                  {" "}
                                  {!showReservedHouseHolds
                                    ? item?.antall?.households
                                    : item?.antall?.households +
                                      item?.antall?.householdsReserved}
                                </td>

                                <td className="flykecontent tabledataRow">
                                  <input
                                    type="checkbox"
                                    defaultChecked={
                                      checkedList.filter(
                                        (i) => i === item.reolId
                                      ).length
                                        ? true
                                        : false
                                    }
                                    onChange={(e) => {
                                      handleCheckboxChange(e, item?.reolId);
                                    }}
                                  />
                                </td>
                              </tr>
                            ))}
                        </tbody>
                      )}
                    </table>
                  </div>
                  <div className="row flykebody">
                    <div className=" col-2 modal-footer ">
                      <button
                        type="button"
                        className="btn btn-default"
                        onClick={() => deleteRow()}
                      >
                        Fjern valgte
                      </button>
                    </div>

                    <div className="col-9 pt-3">
                      <button
                        type="button"
                        className="btn btn-default SegmenterCancel_Button float-right "
                        id="maksimer_id"
                        data-dismiss="modal"
                      >
                        Lukk
                      </button>
                    </div>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
export default SegmentModel;
