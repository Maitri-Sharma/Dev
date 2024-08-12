import React, { useState, useContext } from "react";
import { KSPUContext } from "../../context/Context";

const PostreklameDataKSPU = ({ data, parentCallback }) => {
  const { selectedsegment, setselectedsegment } = useContext(KSPUContext);
  const { selectedName, setSelectedName } = useContext(KSPUContext);

  let arr = selectedsegment;
  const select = (e) => {
    let name = selectedName;
    let nameValue = e.target.value;
    let last_name = nameValue.slice(nameValue.lastIndexOf("_") + 1);
    if (e.target.checked === false) {
      let k = selectedsegment;
      let last_char_value = e.target.id.slice(e.target.id.lastIndexOf("_") + 1);
      const index = k.indexOf(last_char_value);
      if (index > -1) {
        k.splice(index, 1);
      }
      setselectedsegment(k);
      name = name.filter(i=>{return i !== last_name.toString()});
      setSelectedName(name);
    } else {
      name.push(last_name.toString());
      setSelectedName(name);
      let value = e.target.id;
      let char_no = value.lastIndexOf("_");
      let last_char = value.slice(value.lastIndexOf("_") + 1);
      arr.push(last_char.toString());

      let d = arr.join(",");
      setselectedsegment(arr);
    }
  };
  return (
    <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 ml-5 p-0">
      <input
        className="form-check-input"
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

export default PostreklameDataKSPU;
