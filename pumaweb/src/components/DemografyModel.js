import React, { useState, useContext, useEffect } from "react";
import { KSPUContext } from "../context/Context.js";

import { segmenter_kriterier } from "./KspuConfig";
import { groupBy } from "../Data";
import { CreateActiveUtvalg, DemografyCategories } from "../common/Functions";
function DemografyModel(props, { parentCallback }) {
  const segDataList = segmenter_kriterier();
  const { resultData, setResultData, activUtvalg, setActivUtvalg } =
    useContext(KSPUContext);
  const { selecteddemografiecheckbox, setselecteddemografiecheckbox } =
    useContext(KSPUContext);
  const [agder, setAgder] = useState([]);
  const [resultData1, setResultData1] = useState([]);
  const [childrenCheckbox, setChildren] = useState(props.childrenData);
  const [ChangeCheck, setChangeCheck] = useState(Array(10).fill(false));
  const [displayMsg, setDisplayMsg] = useState(false);
  const [checkedList, setCheckedList] = useState([]);
  const [loading, setLoading] = useState(false);
  const { mapattribute, setmapattribute } = useContext(KSPUContext);
  const [upperModelobj, setupperModelobj] = useState([]);
  const [indexObj, setIndexObj] = useState({});
  const [refresh, setRefresh] = useState(false);
  const { showBusiness, showReservedHouseHolds, showHousehold } =
    useContext(KSPUContext);
  const min = (item, mapattribute) => {
    var res = Math.min.apply(
      Math,
      mapattribute.map(function (o) {
        return o[item];
      })
    );
    return res;
  };

  const max = (item, mapattribute) => {
    var res = Math.max.apply(
      Math,
      mapattribute.map(function (o) {
        return o[item];
      })
    );
    return res;
  };

  useEffect(() => {
    let Obj = {};
    let newArray = [];
    selecteddemografiecheckbox.map((item, index) => {
      newArray[index] = {
        name: item,
        min: min(item, mapattribute),
        max: max(item, mapattribute),
      };
      Obj[item] = [];
    });
    calc(activUtvalg);

    setupperModelobj(newArray);
  }, []);

  const calc = (object) => {
    let Obj = {};
    selecteddemografiecheckbox.map((item, index) => {
      Obj[item] = [];
    });
    object?.reoler.map((item) => {
      for (let i = 0; i < selecteddemografiecheckbox.length; i++) {
        Obj[selecteddemografiecheckbox[i]].push(
          item?.indexData[Object.keys(item?.indexData)[i]]
        );
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
    calc(activObj);
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

  return (
    <div>
      <div
        class="modal fade bd-example-modal-lg"
        id={props.id}
        tabIndex="-1"
        role="dialog"
        aria-labelledby="exampleModalCenterTitle"
        aria-hidden="true"
      >
        <div
          class="modal-dialog modal-dialog-centered resultmaximizer"
          role="document"
        >
          <div class="modal-content">
            <div class="modal-header segFord">
              <h5 class="modal-title " id="exampleModalLongTitle">
                {props.title}
              </h5>
              <button
                type="button"
                class="close"
                data-dismiss="modal"
                aria-label="Close"
              >
                <span aria-hidden="true">&times;</span>
              </button>
            </div>

            <div class="modal-body flykebody">
              {displayMsg ? (
                <div className="pr-3">
                  <span
                    id="uxKjoreAnalyse_uxLblMessage"
                    class="divErrorText_kw"
                  >
                    Dette utvalget inneholder ingen budruter
                  </span>
                </div>
              ) : (
                <div>
                  <div class="scrollbarmodalNew">
                    <table class="tableRow">
                      <tbody class="pr-4">
                        <tr class="flykeHeader">
                          <th class="tabledataRow segmentModel ">Variabel</th>
                          <th class="tabledataRow  ">Minimum</th>
                          <th class="tabledataRow segmentAntal">Maksimum</th>
                        </tr>

                        {Object.keys(indexObj).map((key, index) => (
                          <tr>
                            <td class="tabledataRow">
                              {" "}
                              {/* {key.charAt(0).toUpperCase() + key.slice(1)} */}
                              {DemografyCategories[key]}
                            </td>
                            <td class="tabledataRow">
                              {Math.min.apply(Math, indexObj[key])}
                            </td>
                            <td class="tabledataRow">
                              {Math.max.apply(Math, indexObj[key])}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                  <div class="scrollbarmodal">
                    {loading ? (
                      "loading"
                    ) : (
                      <table class="tableRow">
                        {refresh ? null : (
                          <tbody>
                            <tr class="flykeHeader">
                              <th class="tabledataRow">Fylke</th>
                              <th class="tabledataRow">Kommune</th>
                              <th class="tabledataRow">Team</th>
                              <th class="tabledataRow">Budrute</th>
                              <th class="tabledataRow">Antall</th>
                              <th class="tabledataRow">Fjern</th>
                              {Object.keys(selecteddemografiecheckbox).map(
                                (key) => (
                                  // return <option value={key}>{item?.indexData[key]}</option>
                                  <>
                                    <th class="tabledataRow">
                                      {" "}
                                      {selecteddemografiecheckbox[key]
                                        .charAt(0)
                                        .toUpperCase() +
                                        selecteddemografiecheckbox[key].slice(
                                          1
                                        )}
                                    </th>
                                  </>
                                )
                              )}
                            </tr>

                            {activUtvalg?.reoler
                              .sort((a, b) => (a.fylke > b.fylke ? 1 : -1))
                              .map((item, index) => (
                                <tr>
                                  <td class="tabledataRow"> {item.fylke}</td>
                                  <td class="tabledataRow"> {item.kommune}</td>
                                  <td class="tabledataRow"> {item.teamName}</td>
                                  <td class="tabledataRow"> {item.name}</td>
                                  <td class="tabledataRow">
                                    {" "}
                                    {!showReservedHouseHolds
                                      ? item?.antall?.households
                                      : item?.antall?.households +
                                        item?.antall?.householdsReserved}
                                  </td>
                                  <td class="flykecontent tabledataRow">
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

                                      // onClick={(event) =>
                                      //   deleteFlyke(budruter, index, event)
                                      // }
                                    />
                                  </td>

                                  {Object.keys(item?.indexData).map((key) => (
                                    // return <option value={key}>{item?.indexData[key]}</option>
                                    <>
                                      <td class="tabledataRow">
                                        {" "}
                                        {item?.indexData[key]}
                                      </td>
                                    </>
                                  ))}
                                </tr>
                              ))}
                          </tbody>
                        )}
                      </table>
                    )}
                  </div>
                  <div class="row flykebody">
                    <div class=" col-2 modal-footer ">
                      <button
                        type="button"
                        class="btn btn-default"
                        onClick={() => deleteRow()}
                      >
                        Fjern valgte
                      </button>
                    </div>
                    <div class="col-9 pt-3">
                      <button
                        type="button"
                        class="btn btn-default SegmenterCancel_Button float-right "
                        id="maksimer_id"
                        data-dismiss="modal"
                      >
                        Lukk
                      </button>
                      {/* <button onClick={test}>test</button> */}
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

export default DemografyModel;
