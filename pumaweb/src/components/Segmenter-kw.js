import React, { useState, useContext, useEffect } from "react";
import { KundeWebContext, KSPUContext } from "../context/Context.js";
import { segmenter_kriterier } from "./KspuConfig";
import PostreklameData from "./datadef/PostreklameData.js";
import VeglGeografiskOmrade_kw from "./VelgGeografiskOmrade_kw.js";
import api from "../services/api.js";
import { MainPageContext } from "../context/Context.js";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import { MapConfig } from "../config/mapconfig";
import Extent from "@arcgis/core/geometry/Extent";

function Segmenter_KW({ parentCallback }) {
  const [loading, setloading] = useState(false);
  const segDataList = segmenter_kriterier();
  const { Page, setPage } = useContext(KundeWebContext);
  const [nomessagediv, setnomessagediv] = useState(false);
  const { selectedsegment, setselectedsegment } = useContext(KundeWebContext);
  const { segmenterresultarray, setsegmenterresultarray } =
    useContext(KundeWebContext);
  const [Kommuneresult, setKommuneresult] = useState([]);
  const [Fylkeresult, setFylkeresult] = useState([]);
  const { mapView } = useContext(MainPageContext);

  const goback = () => {
    setPage("");
    mapView.extent = new Extent(MapConfig.kundewebMapExtent);
    mapView.goTo(mapView.extent);
  };
  const GotoMain = () => {
    setPage("");
  };
  useEffect(() => {
    setselectedsegment([]);
  }, []);
  const nomessage = async () => {
    setloading(true);
    if (selectedsegment.length == 0) {
      setnomessagediv(true);
      setloading(false);
    } else {
      setnomessagediv(false);

      await fetchData();
      setPage("VeglGeografiskOmrade_kw");
      setloading(false);
    }
  };

  const fetchData = async () => {
    try {
      // let kommuneUrl;
      // let fylkeUrl;

      // let Finalresult = [];

      // let allLayersAndSublayers = mapView.map.allLayers.flatten(function (
      //   item
      // ) {
      //   return item.layers || item.sublayers;
      // });

      // allLayersAndSublayers.items.forEach(function (item) {
      //   if (item.title === "Kommune") {
      //     kommuneUrl = item.url;
      //   }
      //   if (item.title === "Fylke") {
      //     fylkeUrl = item.url;
      //   }
      // });

      // const kommuneName = await GetAllKommunes();
      // const fylkesName = await getAllFylkes();

      // async function getAllFylkes() {
      //   let queryObject = new Query();
      //   queryObject.where = "1=1";
      //   queryObject.returnGeometry = false;
      //   queryObject.outFields = ["fylke", "fylke_id"];

      //   await query
      //     .executeQueryJSON(fylkeUrl, queryObject)
      //     .then(async function (results) {
      //       await results.features.forEach(function (feature) {
      //         Fylkeresult.push(feature.attributes);

      //         });
      //     });
      //   setFylkeresult(Fylkeresult);
      //  }

      // async function GetAllKommunes() {
      //   let queryObject = new Query();
      //   queryObject.where = "1=1";
      //   queryObject.returnGeometry = false;
      //   queryObject.outFields = ["kommune", "komm_id", "fylke_id"];

      //   await query
      //     .executeQueryJSON(kommuneUrl, queryObject)
      //     .then(async function (results) {
      //       await results.features.forEach(function (feature) {
      //         Kommuneresult.push(feature.attributes);
      //         // });
      //     });
      //   setKommuneresult(Kommuneresult);
      // }
      // 
      // // Finalresult = Kommuneresult.map((item, i) =>
      // //   Object.assign({}, item, Fylkeresult[i])
      // // );
      // if (Fylkeresult.length !== 0 && Kommuneresult.length !== 0) {
      //   Finalresult = Kommuneresult.map((t1) => ({
      //     ...t1,
      //     ...Fylkeresult.find((t2) => t2.fylke_id === t1.fylke_id),
      //   }));
      // }

      // let result = [];
      // let s = [];
      // Finalresult.map((u) => {
      //   if (!s.includes(u.fylke_id)) {
      //     result.push({
      //       ID: u.fylke_id,
      //       name: u.fylke,
      //       key: u.fylke_id,
      //       children: [
      //         {
      //           ID: u.kommune,
      //           key: u.komm_id,
      //           name: u.kommune,
      //         },
      //       ],
      //     });

      //     s.push(u.fylke_id);
      //   } else {
      //     let index = result.findIndex((element) => element.ID === u.fylke_id);
      //     result[index].children.push({
      //       ID: u.komm_id,
      //       key: u.komm_id,
      //       name: u.kommune,
      //     });
      //   }
      // });
      // setsegmenterresultarray(result);

      const { data, status } = await api.getdata("Kommune/GetAllKommunes");
      if (status === 200) {
        let s = [];
        let result = [];

        data.map((u) => {
          if (!s.includes(u.fylkeID)) {
            result.push({
              ID: u.fylkeID,
              name: u.fylkeName,
              key: u.fylkeID,
              children: [
                {
                  ID: u.kommuneID,
                  key: u.kommuneID,
                  name: u.kommuneName,
                },
              ],
            });

            s.push(u.fylkeID);
          } else {
            let index = result.findIndex((element) => element.ID === u.fylkeID);
            result[index].children.push({
              ID: u.kommuneID,
              key: u.kommuneID,
              name: u.kommuneName,
            });

          }
        });
        setsegmenterresultarray(result);

        s = data.map((item) => {
          return {
            ID: item.fylkeID,
            name: item.fylkeName,
            children: [
              {
                ID: item.kommuneID,
                name: item.kommuneName,
              },
            ],
          };
        });
      } else {
        console.error("error : " + status);
      }
    } catch (error) {
      console.error("er : " + error);
    }
  };
  return (
    <div className={loading ? "col-5  blur" : "col-5"}>
      <div className="">
        <span className="title">Lag utvalg basert på segmenter</span>
      </div>
      <div className="padding_NoColor_T">
        <p
          id="DemografiAnalyse1_uxHeader_lblDesc"
          className="lblAnalysisHeaderDesc"
        >
          Enkelte områder har overvekt av noen typer mennesker med ulike behov.
          Vi har gjort det enklere for deg å finne områdene der disse bor.
          <br />
          Vi har delt inn husholdningene i Norge i 10 grupper etter noen/basert
          på visse kriterier.
        </p>
      </div>

      <div className=" specialfont">
        <span
          id="uxSegmenterAnalyse_uxHeader_lblStep1"
          className="lblAnalysisHeaderStepBold"
        >
          1. Velg ett eller flere{" "}
          <a
            href="http://www.bring.no/radgivning/kundedialog/dm-i-postkasse"
            target="_blank"
            runat="server"
            className="KSPU_LinkInText_kw"
          >
            segmenter
          </a>{" "}
          nedenfor
        </span>
        {loading ? (
          <img
            src={loadingImage}
            style={{
              width: "20px",
              height: "20px",
              position: "absolute",
              left: "210px",
              zindex: 100,
            }}
          />
        ) : null}

        <div>
          <span
            id="DemografiAnalyse1_uxHeader_lblStep2"
            className="lblAnalysisHeaderStep"
          >
            2. Velg geografisk område
          </span>
        </div>
        <br />
        <p>
          Resultatet er et sett med budruter der det bor en overvekt av de
          personer med de interessene du har valgt.
        </p>
      </div>

      {nomessagediv ? <br /> : null}
      {nomessagediv ? (
        <div className="pr-3">
          <div className="error WarningSign">
            <div className="divErrorHeading">Melding:</div>
            <span id="uxKjoreAnalyse_uxLblMessage" className="divErrorText_kw">
              Ingen segmenter er valgt. Velg minst ett segment.
            </span>
          </div>
        </div>
      ) : null}
      <br />
      {/* { nomessage ? <VeglGeografiskOmrade_kw/> :  */}
      <div style={{ backgroundColor: "#E6E6E6" }}>
        <div className="ml-4">
          <div className="ml-2 ">
            {segDataList.map((data, index) => (
              <PostreklameData data={data} key={index}/>
            ))}
          </div>
        </div>
      </div>
      {/* } */}
      <p></p>

      {/* <div className="padding_Color_L_R_T_B sok-text">
            <table className="padding_NoColor_L_R_T_B wizFilled">           
      <tbody><tr>
        <td>
           <table border="0" width="100%" className="wizFilled">
		<tbody><tr>
			<td><input id="uxSegmenterAnalyse_SegmenterKriterier1_A" type="checkbox" name="uxSegmenterAnalyse$SegmenterKriterier1$A"/><label className="pl-1">Senior Ordinær</label></td>
		</tr>
		<tr>
			<td><input id="uxSegmenterAnalyse_SegmenterKriterier1_B" type="checkbox" name="uxSegmenterAnalyse$SegmenterKriterier1$B"/><label className="pl-1">Senior Aktiv</label></td>
		</tr>
		<tr>
			<td><input id="uxSegmenterAnalyse_SegmenterKriterier1_C1" type="checkbox" name="uxSegmenterAnalyse$SegmenterKriterier1$C1"/><label className="pl-1">Urban Ung</label></td>
		</tr>
		<tr>
			<td><input id="uxSegmenterAnalyse_SegmenterKriterier1_C2" type="checkbox" name="uxSegmenterAnalyse$SegmenterKriterier1$C2"/><label className="pl-1">Urban Moden</label></td>
		</tr>
		<tr>
			<td><input id="uxSegmenterAnalyse_SegmenterKriterier1_D" type="checkbox" name="uxSegmenterAnalyse$SegmenterKriterier1$D"/><label className="pl-1">Ola og Kari Tradisjonell</label></td>
		</tr>
		<tr>
			<td><input id="uxSegmenterAnalyse_SegmenterKriterier1_E" type="checkbox" name="uxSegmenterAnalyse$SegmenterKriterier1$E"/><label className="pl-1">Ola og Kari Individualist</label></td>
		</tr>
		<tr>
			<td><input id="uxSegmenterAnalyse_SegmenterKriterier1_F" type="checkbox" name="uxSegmenterAnalyse$SegmenterKriterier1$F"/><label className="pl-1">Barnefamilie Velstand og Kultur</label></td>
		</tr>
		<tr>
			<td><input id="uxSegmenterAnalyse_SegmenterKriterier1_G" type="checkbox" name="uxSegmenterAnalyse$SegmenterKriterier1$G"/><label className="pl-1">Barnefamilie Barnerik</label></td>
		</tr>
		<tr>
			<td><input id="uxSegmenterAnalyse_SegmenterKriterier1_H" type="checkbox" name="uxSegmenterAnalyse$SegmenterKriterier1$H"/><label className="pl-1">Barnefamilie Prisbevisst</label></td>
		</tr>
		<tr>
			<td><input id="uxSegmenterAnalyse_SegmenterKriterier1_I" type="checkbox" name="uxSegmenterAnalyse$SegmenterKriterier1$I"/><label className="pl-1">Barnefamilie Moderne Aktiv</label></td>
		</tr>
	</tbody></table>
	
        </td>
      </tr>
</tbody></table>
            </div> */}
      <br />

      <div className="div_left">
        <input
          type="submit"
          name="DemografiAnalyse1$uxFooter$uxBtForrige"
          value="Tilbake"
          onClick={goback}
          className="KSPU_button_Gray"
        />
        <div className="padding_NoColor_T">
          <a
            className="KSPU_LinkButton_Url_KW pl-2"
            onClick={GotoMain}
          >
            Avbryt
          </a>
        </div>
      </div>

      <div className="float-right">
        <div>
          <input
            type="submit"
            name="DemografiAnalyse1$uxFooter$uxBtnNeste"
            value="Velg geografisk område "
            onClick={nomessage}
            className="KSPU_button-kw"
          />
        </div>
      </div>
    </div>
  );
}

export default Segmenter_KW;
