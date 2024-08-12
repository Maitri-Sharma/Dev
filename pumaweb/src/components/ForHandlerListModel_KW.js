import React, { useState, useRef, useContext } from "react";
import { KundeWebContext } from "../context/Context.js";

import { GetImageUrl } from "../common/Functions";

import api from "../services/api.js";
import Swal from "sweetalert2";
function ForHandlerListModel_KW(props, parentCallback) {
  const { username_kw, setusername_kw } = useContext(KundeWebContext);
  const [member, setMember] = useState(props.memberUtvalgs);
  const [loading, setloading] = useState(false);
  const { utvalglistapiobject, setutvalglistapiobject, setPage } =
    useContext(KundeWebContext);
  const [state, setState] = useState([]);
  const [memberLogos, setMemberLogos] = useState([]);
  const [logoName, setLogoName] = useState(utvalglistapiobject.logo);
  const FirstInputText = useRef();
  const btnClose = useRef();
  const handleChange = (e, id) => {
    let logo = e.target.value;
    let memberLogo = {
      utvalgId: id,
      logo: logo,
    };
    let changedActivUtvalList = utvalglistapiobject;
    let objIndex = changedActivUtvalList.memberUtvalgs.findIndex(
      (obj) => obj.utvalgId == id
    );
    changedActivUtvalList.memberUtvalgs[objIndex].logo = logo;
    setutvalglistapiobject({});
    setutvalglistapiobject(changedActivUtvalList);

    setMemberLogos((memberLogos) => [...memberLogos, memberLogo]);
  };
  const handleChange1 = (e, id) => {
    setLogoName(FirstInputText.current.value);
    let logoName = e.target.value;
    let logo = {
      listId: id,
      logo: logoName,
    };
    setState((state) => [...state, logo]);
  };
  const saveListLogo = async () => {
    let id = 0;
    if (utvalglistapiobject.listId) {
      id = utvalglistapiobject.listId;
    }
    let requestQuery = {
      listId: id,
      logo: logoName,
      requestUpdateUtvaglLogos: memberLogos,
    };

    let url = "";
    setloading(true);
    let customername = "";
    if (username_kw) {
      customername = username_kw;
    } else {
      customername = "Internbruker";
    }
    url = url + `UtvalgList/UpdateListLogo?userName=${customername}&`;
    try {
      const { data, status } = await api.putData(url, requestQuery);
      if (data.length == 0) {
        setloading(false);
      } else {
        utvalglistapiobject.logo = logoName;
        // setPage("Geogra_distribution_click");

        //props.parentCallback(true);
        Swal.fire({
          title: "<strong>KVITTERING </strong>",
          text: `Lagring utført.`,
          confirmButtonColor: "#7bc144",
          confirmButtonText: "Lukk",
          position: "top",
          icon: "success",
        });
        btnClose.current.click();
      }
    } catch (error) {
      console.log("errorpage API not working");
      console.error("error : " + error);
      btnClose.current.click();
      setloading(false);
    }
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
      let list = !item.listId ? false : true;
      if (item.BasedOn > 0) {
        Image = GetImageUrl("kampanje", item.isBasis, list, item.ordreType);
      } else {
        Image = GetImageUrl("utvalg", item.isBasis, list, item.ordreType);
      }
    }

    return <img className="mb-1" src={imgLoader(Image)} />;
  };
  const imgLoader = (path) => {
    return require("../assets/images/Icons/" + path);
  };
  return (
    <div>
      <div
        className="modal fade bd-example-modal-lg"
        id={props.id}
        data-backdrop="false"
        tabIndex="-1"
        role="dialog"
        aria-labelledby="exampleModalCenterTitle"
        aria-hidden="true"
      >
        <div
          className="modal-dialog"
          role="document"
        >
          <div className="modal-content">
            <div className="Common-modal-header">
              <span
                className="common-modal-title pt-1 pl-2"
                id="exampleModalLongTitle"
              >
                {props.title}
              </span>
              <button
                type="button"
                className="close"
                data-dismiss="modal"
                aria-label="Close"
                ref={btnClose}
              >
                <span aria-hidden="true">&times;</span>
              </button>
            </div>
            <div className="View_modal-body pl-2">
              <div>
                {renderPerson1(utvalglistapiobject)}
                <span className="UtvaldivLabelText_1 pl-1">
                  {utvalglistapiobject.name}
                </span>
                <input
                  type="text"
                  className="selection-input float-right mr-2"
                  placeholder=""
                  value={logoName}
                  ref={FirstInputText}
                  onChange={(e) => handleChange1(e, utvalglistapiobject.listId)}
                />
              </div>
              {typeof props.data !== "undefined" &&
              props.data.length <= 0 &&
              utvalglistapiobject.basedOn > 0 ? (
                <> </>
              ) : (
                <div>
                  {member.map((item, index) => (
                    <div key={index} className="pl-2">
                      {renderPerson1(item)}
                      <span className="UtvaldivLabelText_1">
                        {item.name}
                        <input
                          type="text"
                          defaultValue={item.logo}
                          className="selection-input float-right mr-2"
                          id={item.utvalgId}
                          onChange={(e) => handleChange(e, item.utvalgId)}
                        />
                      </span>
                    </div>
                  ))}
                </div>
              )}
              <div className="pt-3 pb-2 ">
                <button
                  type="button"
                  className="btn KSPU_button mr-2"
                  data-dismiss="modal"
                >
                  Lukk
                </button>

                <button
                  type="button"
                  onClick={saveListLogo}
                  className="btn KSPU_button mr-1 float-right"
                >
                  Lagre
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default ForHandlerListModel_KW;
