import React, { useState, useRef, useContext } from "react";
import "../App.css";
import MottakerComponent from "./Mottakergrupper";
import { UtvalgContext } from "../context/Context";

function Geographie_footer(props) {
  const { setIsPickList } = useContext(UtvalgContext);
  const pickList = useRef();
  const HandlePickList = () => {
    // setIsPickList(pickList.current.checked);
    setIsPickList(true);
    
  };
  const HandlePickList_new = () => {
    // setIsPickList(pickList.current.checked);
    setIsPickList(false);
    
  };
  return (
    <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
      
      {props.name === "" ? (
        <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 pt-2">
          <div className="col-10 m-0 p-0 pr-2">
            {" "}
            <span className="label p-2">Velg veien videre</span>
          </div>
          <div className="col-2 m-0 p-0 pr-2"></div>

          <div className="sok-text ml-3">
            <div>
              <input
                type="radio"
                name="optradio"
                className="sok-text"
                // checked
                defaultChecked={true}
                onClick={HandlePickList_new}
              />{" "}
              {props.checkname1}
            </div>
            <div>
              <input
                type="radio"
                ref={pickList}
                name="optradio"
                onClick={HandlePickList}
              />
              {props.checkname2}
            </div>
          </div>
        </div>
      ) : (
        <></>
      )}
    </div>
  );
}

export default Geographie_footer;
