import React, { useEffect, useState, useContext, useRef } from "react";
import "../../App.css";
import expand from "../../assets/images/esri/expand.png";
import collapse from "../../assets/images/esri/collapse.png";
import { groupBy } from "../../Data";
import api from "../../services/api.js";
import {
  GetImageUrl,
  CreateUtvalglist,
  FormatDate,
  CreateActiveUtvalg,
} from "../../common/Functions";
import { KSPUContext, MainPageContext } from "../../context/Context.js";
import Spinner from "../../components/spinner/spinner.component";

function Campaign(props) {
  const {
    setActivUtvalg,
    setvalue,
    setAktivDisplay,
    setResultData,
    setActivUtvalglist,
    setutvalglistcheck,
    setshoworklist,
    showorklist,
  } = useContext(KSPUContext);
  const [togglevalue, settogglevalue] = useState(true);
  const [header, setHeader] = useState("");
  const [ModelName, setModelName] = useState("");
  const [campaignTilbudData, setcampaignTilbudData] = useState([]);
  const [campaignFinsihedData, setcampaignFinsihedData] = useState([]);
  const [loading, setloading] = useState(false);
  const { mapView } = useContext(MainPageContext);
  const toggle = () => {
    settogglevalue(!togglevalue);
  };
  const showModel = () => {
    setModelName("ShowConnectList");
  };

  const imgLoader = (path) => {
    return require("../../assets/images/Icons/" + path);
  };

  const fetchCampaigns = async () => {
    let url;

    if (props.data.utvalgId) {
      url = `Utvalg/GetUtvalgCampaigns?utvalgId=${props.data.utvalgId}`;
      setHeader("KAMPANJER BASERT PÅ BASISUTVALGET");
    } else {
      url = `UtvalgList/GetUtvalgListCampaigns?listId=${props.data.listId}`;
      setHeader("KAMPANJER BASERT PÅ BASISLISTA");
    }

    const { data, status } = await api.getdata(url);
    if (status === 200) {
      let tilbud = [];
      let finished = [];

      data.map((item) => {
        if (item.ordreType === 1) {
          finished.push(item);
        } else {
          tilbud.push(item);
        }
      });
      tilbud = tilbud.sort(function (a, b) {
        var c = new Date(a.distributionDate);

        var d = new Date(b.distributionDate);

        return c - d;
      });
      finished = finished.sort(function (a, b) {
        var c = new Date(a.distributionDate);

        var d = new Date(b.distributionDate);

        return c - d;
      });

      setcampaignTilbudData(tilbud);
      setcampaignFinsihedData(finished);
    }
  };

  const openCampaign = async (e) => {
    let url = "";
    let id = e.target.id;
    let newWorklist = false;
    let addNewList = false;
    if (props.data.utvalgId) {
      setloading(true);
      url = url + `Utvalg/GetUtvalg?utvalgId=${id}`;
      try {
        //api.logger.info("APIURL", url);
        const { data, status } = await api.getdata(url);
        if (data.length === 0) {
          //api.logger.error("Error : No Data is present for mentioned Id" + id);
        } else {
          setActivUtvalg({});
          let j = mapView.graphics.items.length;

          for (let i = j; i > 0; i--) {
            if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
              mapView.graphics.remove(mapView.graphics.items[i - 1]);
            }
          }
          setResultData(groupBy(data.reoler, "", 0, 0, 0, 0, []));
          let obj = await CreateActiveUtvalg(data);
          setActivUtvalg(obj);
          setvalue(false);
          setutvalglistcheck(false);
          // setAktivDisplay(true);
          showorklist.map((val) => {
            if (val.name === data.name) {
              newWorklist = true;
            }
          });
          if (!newWorklist) {
            showorklist.push(data);
          }
          setshoworklist(showorklist);
          setloading(false);
        }
      } catch (error) {
        //api.logger.error("errorpage API not working");
        //api.logger.error("error : " + error);
      }
    } else {
      setloading(true);
      url =
        url + `UtvalgList/GetUtvalgListwithAllReferences?UtvalglistId=${id}`;
      try {
        //api.logger.info("APIURL", url);
        const { data, status } = await api.getdata(url);
        if (data.length === 0) {
          //api.logger.error("Error : No Data is present for mentioned Id" + id);
        } else {
          let obj = await CreateUtvalglist(data);
          setActivUtvalglist(obj);
          setvalue(false);
          setutvalglistcheck(true);
          setAktivDisplay(true);
          showorklist.map((val) => {
            if (val.name === data.name) {
              addNewList = true;
            }
          });
          if (!addNewList) {
            setshoworklist((showorklist) => [...showorklist, data]);
          }
          setloading(false);
        }
      } catch (error) {
        //api.logger.error("errorpage API not working");
        //api.logger.error("error : " + error);
      }
    }
    window.scrollTo(0, 0);
  };

  useEffect(() => {
    fetchCampaigns();
  }, []);

  return (
    <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 pr-1">
      <div>{loading ? <Spinner /> : null}</div>
      <div className="card Kj-background-color col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <div className="row">
          <div className="col-8">
            <p className="avan p-1 " style={{ fontSize: "0.90vw" }}>
              {header}
            </p>
          </div>
          <div className="col-4 _flex-end" onClick={toggle}>
            {!togglevalue ? (
              <img className="d-flex float-right pt-1 mr-1" src={collapse} />
            ) : (
              <img className="d-flex float-right pt-1 mr-1" src={expand} />
            )}
          </div>
        </div>
        {!togglevalue ? (
          <div
            style={{ overflowY: "auto", height: "100px" }}
            className="Kj-div-background-color col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 "
          >
            {campaignTilbudData.length < 1 &&
            campaignFinsihedData.length < 1 ? (
              <p className="divValueText-list mb-4 p-0"></p>
            ) : (
              <div className="finClick_css1 borderless">
                {campaignTilbudData.length > 0 ? (
                  <div className="row col-12 p-0 m-0 mb-3">
                    <div className="row col-12 p-0 m-0 pl-1 pr-1">
                      <div className="col-1 selectionTable_center m-0 p-0 pl-1">
                        <span></span>
                      </div>
                      <div className="col-7 selectionTable_center m-0 p-0 pl-1">
                        <span>Tilbud</span>
                      </div>
                      <div className="col-1 selectionTable_center m-0 p-0 pl-1">
                        <span></span>
                      </div>
                      <div className="col-3 selectionTable_center m-0 p-0 pr-1">
                        <span>Dist.dato</span>
                      </div>
                    </div>
                    {campaignTilbudData.map((item) => (
                      <div className="row col-12 p-0 m-0 pl-1 pr-1">
                        <div className="col-1 selectionTable_center m-0 p-0 pl-1">
                          <img
                            className="mb-1"
                            src={imgLoader(
                              GetImageUrl(
                                "utvalg",
                                item.IsBasis,
                                item.utvalgId ? true : false,
                                item.ordreType
                              )
                            )}
                          />
                        </div>
                        <div className="col-7 m-0 p-0 selectionTable_left pl-1">
                          <span id={item.id} onClick={openCampaign}>
                            {item.name}
                          </span>
                        </div>
                        <div className="col-1 selectionTable_center m-0 p-0 pl-1">
                          <img
                            className="mb-1"
                            src={
                              item.isDisconnected
                                ? require("../../assets/images/disconnected.png")
                                : require("../../assets/images/disconnect.png")
                            }
                          />
                        </div>
                        <div className="col-3 selectionTable_center m-0 p-0 selectionTable_right pr-1">
                          <span>
                            {item.distributionDate === "0001-01-01T00:00:00"
                              ? ""
                              : FormatDate(item.distributionDate)}
                          </span>
                        </div>
                      </div>
                    ))}
                  </div>
                ) : null}

                {campaignFinsihedData.length > 0 ? (
                  <div className="row col-12 p-0 m-0 mb-2">
                    <div className="row col-12 p-0 m-0 pl-1 pr-1">
                      <div className="col-1 selectionTable_center m-0 p-0 pl-1">
                        <p></p>
                      </div>
                      <div className="col-7 selectionTable_center m-0 p-0 pl-1">
                        <span>Ferdige kampanjer</span>
                      </div>
                      <div className="col-1 selectionTable_center m-0 p-0 pl-1">
                        <span></span>
                      </div>
                      <div className="col-3 selectionTable_center m-0 p-0 pr-1">
                        <span>Dist.dato</span>
                      </div>
                    </div>
                    {campaignFinsihedData.map((item) => (
                      <div className="row col-12 p-0 m-0 pl-1 pr-1">
                        <div className="col-1 selectionTable_center m-0 p-0 pl-1">
                          <img
                            className="mb-1"
                            src={imgLoader(
                              GetImageUrl(
                                "utvalg",
                                item.IsBasis,
                                item.utvalgId ? true : false,
                                item.ordreType
                              )
                            )}
                          />
                        </div>
                        <div className="col-7 m-0 p-0 selectionTable_left pl-1">
                          <span id={item.id} onClick={openCampaign}>
                            {item.name}
                          </span>
                        </div>
                        <div className="col-1 selectionTable_center m-0 p-0 pl-1">
                          <img
                            className="mb-1"
                            src={
                              item.isDisconnected
                                ? require("../../assets/images/disconnected.png")
                                : require("../../assets/images/disconnect.png")
                            }
                          />
                        </div>
                        <div className="col-3 selectionTable_center m-0 p-0 selectionTable_right pr-1">
                          <span>{FormatDate(item.distributionDate)}</span>
                        </div>
                      </div>
                    ))}
                  </div>
                ) : null}
              </div>
            )}
          </div>
        ) : null}
      </div>
    </div>
  );
}

export default Campaign;
