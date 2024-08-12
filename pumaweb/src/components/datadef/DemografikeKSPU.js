import React, { useState, useContext } from "react";
import { KSPUContext } from "../../context/Context";

const DemografikeKSPU = ({ data, parentCallback }) => {
  const { selectedDemografike, setSelectedDemografike } =
    useContext(KSPUContext);

  let arr = selectedDemografike;
  const select = (e) => {
    if (e.target.checked == false) {
      let k = selectedDemografike;
      const index = k.indexOf(e.target.id);
      if (index > -1) {
        k.splice(index, 1);
      }
      setSelectedDemografike(k);
    } else {
      //  if(arr)
      let value = e.target.id;
      arr.push(value.toString());

      let d = arr.join(",");
      setSelectedDemografike(arr);
    }
    // parentCallback(value)
  };
  return (
    <div>
      <input
        className="form-check-input  "
        type="checkbox"
        onClick={select}
        value={data.name}
        id={data.id}
      />
      <label className="form-check-label" htmlFor={data.name}>
        {" "}
        {data.name}{" "}
      </label>
    </div>
  );
};

export default DemografikeKSPU;
