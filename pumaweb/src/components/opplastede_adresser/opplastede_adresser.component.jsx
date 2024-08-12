import React, { useState, useEffect, useContext, useRef } from "react";
import "../opplastede_adresser/opplastede_adresser.styles.scss";
import LastOppAdressepunkterResultat from "../last_opp_adressepunkter_resultat/last_opp_adressepunkter_resultat.component";
import { Table } from "antd";
import { KSPUContext } from "../../context/Context";
function OpplastedeAdresser(props, { parentCallback }) {
  const [result, setresult] = useState([]);
  const [headervalues, setheadervalues] = useState([]);
  const [currentStep, setCurrentStep] = useState(props.currentStep);
  const [showerrordiv, setshowerrordiv] = useState(false);
  const [errormsg, seterrormsg] = useState("");
  const [cityArray, setCityArray] = useState([]);
  const [butikkArray, setButikkArray] = useState([]);
  const [addressArray, setAddressArray] = useState([]);
  const [postArray, setPostArray] = useState([]);

  const [data, setData] = useState([]);
  const { setAddresslisteDisplay, setvalue } = useContext(KSPUContext);
  const [columns, setColumn] = useState([]);
  useEffect(() => {
    let t = [];
    var lengths;

    if (props.Type === "tab" || "comma" || "semicolon") {
      lengths = props.Data.map((a) => a.length);
    }
    lengths = Math.max(...lengths);

    for (let i = 0; i < lengths; i++) {
      t.push(`kolumne${i}`);
    }

    let s = [];
    let resultarray = props.Data.map((item) => {
      if (props.Type === "tab" || "comma" || "semicolon") {
        s.push(item);
      }
    });
    if (!props.header) {
      s.unshift(t);
    }

    setheadervalues(s[0]);

    setresult(s);
    formatJSON(s);
  }, []);

  const nextclick = async () => {
    let head = [];
    if (props.header) {
      head = result[0];
    } else {
      head = result[1];
    }

    var postenitem;
    var addresse;
    var city;
    var butikk;
    for (let i = 0; i < head.length; i++) {
      if (
        head[i].toLowerCase() === "postnr" ||
        head[i].toLowerCase() === "kolumne3"
      ) {
        postenitem = head[i];
      } else if (
        head[i].toLowerCase() === "besøksadresse" ||
        head[i].toLowerCase() === "kolumne2"
      ) {
        addresse = head[i];
      } else if (
        head[i].toLowerCase() === "poststed" ||
        head[i].toLowerCase() === "kolumne4"
      ) {
        city = head[i];
      } else if (
        head[i].toLowerCase() === "butikk" ||
        head[i].toLowerCase() === "kolumne1"
      ) {
        butikk = head[i];
      }
    }

    let postIndex = head.indexOf(postenitem);
    let addressIndex = head.indexOf(addresse);
    let cityIndex = head.indexOf(city);
    let butikkIndex = head.indexOf(butikk);

    let cityArray = [];
    if (cityIndex !== -1) {
      cityArray = result.map((item, i) => {
        return item[cityIndex];
      });
    }
    cityArray.shift();
    cityArray = cityArray.filter((b) => {
      return b !== undefined;
    });
    setCityArray(cityArray);
    let butikkArray = [];
    if (butikkIndex !== -1) {
      butikkArray = result.map((item, i) => {
        return item[butikkIndex];
      });
    }
    butikkArray.shift();
    butikkArray = butikkArray.filter((b) => {
      return b !== undefined;
    });
    setButikkArray(butikkArray);
    let addressArray = [];
    if (addressIndex !== -1) {
      addressArray = result.map((item, i) => {
        return item[addressIndex];
      });
    }
    addressArray.shift();
    addressArray = addressArray.filter((b) => {
      return b !== undefined;
    });
    setAddressArray(addressArray);
    let postArray = [];
    if (postIndex !== -1) {
      postArray = result.map((item, i) => {
        return item[postIndex];
      });
    }

    postArray.shift();
    postArray = postArray.filter((b) => {
      return b !== undefined;
    });
    setPostArray(postArray);
    let alllengh = postArray.map((item) => {
      if (item !== undefined) {
        return item.length;
      }
    });

    setCurrentStep(currentStep + 1);
  };

  const handleCancel = () => {
    setAddresslisteDisplay(false);
    setvalue(true);
  };
  const callback = (step) => {
    setCurrentStep(step - 1);
  };
  const handleprevclick = () => {
    props.parentCallback(props.currentStep);
  };

  var dat = [];
  var cols = [];

  const formatJSON = (res) => {
    res[0].map((i) => {
      cols.push({
        title: () => {
          return <div style={{ padding: 0, fontSize: "10px" }}>{i}</div>;
        },
        dataIndex: i,
        key: i,
        render: (text) => (
          <div style={{ padding: 4, fontSize: "11px" }}>{text}</div>
        ),
      });
    });
    for (let i = 0; i < res?.length; i++) {
      dat.push({ key: i });
    }
    res.map((i, index) => {
      if (index !== 0) {
        res[0].map((j, index2) => {
          if (i.length > 1) {
            dat[index][j] = i[index2];
          }
          return null;
        });
      }
      return null;
    });
    setData(dat);
    setColumn(cols);
  };

  return (
    <div className="card">
      {showerrordiv ? (
        <div className="sok-text red">
          <p className="pl-1">{errormsg}</p>
        </div>
      ) : null}
      {currentStep === 2 ? (
        <>
          {data?.length && columns?.length ? (
            <Table
              columns={columns}
              dataSource={data}
              pagination={false}
              style={{ height: "300px", marginBottom: 60, padding: 1 }}
              scroll={{ y: 300 }}
            />
          ) : null}
          <div className="row no-margin">
            <div className="row col-12 m-0 p-0 mt-2 mb-2">
              <div className="row pl-1 pr-1">
                <div className="col-4">
                  <input
                    type="submit"
                    id="uxBtForrige"
                    className="KSPU_button"
                    value="<< Forrige"
                    onClick={handleprevclick}
                    style={{
                      visibility: currentStep > 1 ? "visible" : "hidden",
                      text: "",
                      float: "left",
                    }}
                  />
                </div>
                <div className="col-4">
                  <input
                    type="submit"
                    id="uxBtnAvbryt"
                    className="KSPU_button"
                    value="Avbryt"
                    onClick={handleCancel}
                    style={{
                      text: "Avbryt",
                      marginLeft: "auto",
                      float: "left",
                    }}
                  />
                </div>
                <div className="col-4">
                  <input
                    type="submit"
                    id="uxBtnNeste"
                    className="KSPU_button float-right"
                    value="Neste >>"
                    onClick={nextclick}
                    style={{
                      display: currentStep < 3 ? "block" : "none",
                      float: "right",
                      marginLeft: "auto",
                    }}
                  />
                </div>
              </div>
            </div>
          </div>
        </>
      ) : currentStep === 3 ? (
        <LastOppAdressepunkterResultat
          currentStep={currentStep}
          parentCallback={callback}
          postNrArray={postArray}
          cityArray={cityArray}
          addressArray={addressArray}
          butikkArray={butikkArray}
        />
      ) : null}
    </div>
  );
}

export default OpplastedeAdresser;
