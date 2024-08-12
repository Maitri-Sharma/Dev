import React, { useState, useRef, useContext } from "react";
import { KSPUContext } from "../context/Context.js";

import { GetImageUrl } from "../common/Functions";

import api from "../services/api.js";
function ForhandlerlistModel(props, parentCallback) {
  const [member, setMember] = useState(props.memberUtvalgs);
  const [loading, setloading] = useState(false);
  const { activUtvalglist, setActivUtvalglist, setAktivDisplay } =
    useContext(KSPUContext);
  const [state, setState] = useState([]);
  const [memberLogos, setMemberLogos] = useState([]);
  const [logoName, setLogoName] = useState(activUtvalglist.logo ? activUtvalglist.logo :" ");
  const [isListofList,setIslistofList] = useState(false);
  const FirstInputText = useRef();
  const btnClose = useRef();

  const handleChange = (e, id,isListofList) => {
    let logo = e.target.value;
    let memberLogo;
    let changedActivUtvalList = activUtvalglist;
    if(isListofList){
      setIslistofList(true);
       memberLogo = {
        listId: id,
        logo: logo,
        requestUpdateUtvaglLogos: [],
        memberList:[]
      };
      let objIndex = changedActivUtvalList.memberLists.findIndex(
        (obj) => obj.listId === id
      );
      changedActivUtvalList.memberLists[objIndex].logo = logo;
    

    }
    else{
      setIslistofList(false);
       memberLogo = {
        utvalgId: id,
        logo: logo,
      };
      let objIndex = changedActivUtvalList.memberUtvalgs.findIndex(
        (obj) => obj.utvalgId == id
      );
      changedActivUtvalList.memberUtvalgs[objIndex].logo = logo;

    }
    
    setMemberLogos((memberLogos) => [...memberLogos, memberLogo]);
    setActivUtvalglist({});
    setActivUtvalglist(changedActivUtvalList);

  };
  const parentLogoChange = (e, id) => {
    setLogoName(FirstInputText.current.value);
    let logoName = e.target.value;
    let logo = {
      listId: id,
      logo: logoName,
    };
    setState((state) => [...state, logo]);
  };
  const saveListLogo = async () => {
    let requestQuery;
    let id = 0;
    if (activUtvalglist.listId) {
      id = activUtvalglist.listId;
    }
    if(isListofList){
      requestQuery = {
        listId: id,
        logo: logoName,
        requestUpdateUtvaglLogos: [],
        memberList: memberLogos,
      };
    }
    else{
      requestQuery = {
        listId: id,
        logo: logoName,
        requestUpdateUtvaglLogos: memberLogos,
      };
    }
  

    let url = "";
    setloading(true);
    url = url + `UtvalgList/UpdateListLogo?userName=Internbruker&`;
    try {
      const { data, status } = await api.putData(url, requestQuery);
      if (data.length == 0) {
        setloading(false);
      } else {
        btnClose.current.click();
        activUtvalglist.logo = logoName;
        setAktivDisplay(false);
        setAktivDisplay(true);
        props.parentCallback(true);
      }
    } catch (error) {
      console.log("errorpage API not working");
      console.error("error : " + error);

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

  const renderMemberUtvalg = (args) => {
    return args.map((item, index) => (
      <>
        <div key={index} className="pl-2">
          {renderPerson1(item)}
          <span className="UtvaldivLabelText_1">
            {item.name}
            <input
              type="text"
              defaultValue={item.logo}
              className="selection-input float-right mr-2"
              id={item.utvalgId}
              onChange={(e) => handleChange(e, item.utvalgId,false)}
            />
          </span>
        </div>
        {/* {item?.listId
          ? item?.memberLists
            ? renderPerson(item.memberLists)
            : null
          : null}
        {item?.listId
          ? item.memberUtvalgs
            ? renderPerson(item.memberUtvalgs)
            : null
          : null} */}
      </>
    ));
  };
  const renderMemberList = (args) => {
    return args.map((item, index) => (
      <>
        <div key={index} className="pl-2">
          {renderPerson1(item)}
          <span className="UtvaldivLabelText_1">
            {item.name}
            <input
              type="text"
              defaultValue={item.logo}
              className="selection-input float-right mr-2"
              id={item.listId}
              onChange={(e) => handleChange(e, item.listId,true)}
            />
          </span>
        </div>
        {/* {item?.listId
          ? item?.memberLists
            ? renderPerson(item.memberLists)
            : null
          : null}
        {item?.listId
          ? item.memberUtvalgs
            ? renderPerson(item.memberUtvalgs)
            : null
          : null} */}
      </>
    ));
  };

  return (
    <div>
      <div
        className="modal fade bd-example-modal-lg"
        id={props.id}
        tabIndex="-1"
        role="dialog"
        aria-labelledby="exampleModalCenterTitle"
        aria-hidden="true"
      >
        <div
          className="modal-dialog modal-dialog-centered"
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
                {renderPerson1(activUtvalglist)}
                <span className="UtvaldivLabelText_1 pl-1">
                  {activUtvalglist.name}
                </span>
                <input
                  type="text"
                  className="selection-input float-right mr-2"
                  placeholder=""
                  value={logoName}
                  ref={FirstInputText}
                  onChange={(e) => parentLogoChange(e, activUtvalglist.listId)}
                />
              </div>
              {typeof props.data !== "undefined" &&
              props.data.length <= 0 &&
              activUtvalglist.basedOn > 0 ? (
                <> </>
              ) : (
                <div>
                  {activUtvalglist?.memberLists?.length > 0
                    ? renderMemberList(activUtvalglist?.memberLists)
                    : renderMemberUtvalg(activUtvalglist?.memberUtvalgs)}
                  {/* {(activUtvalglist?.listId)
                    ? activUtvalglist.memberUtvalgs                      
                        ? renderPerson(activUtvalglist.memberUtvalgs)
                      : null
                      : null
                    } */}
                  {/* {renderPerson(activUtvalglist)} */}
                  {/* {member.map((item, index) => (
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
                  ))} */}
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

export default ForhandlerlistModel;
