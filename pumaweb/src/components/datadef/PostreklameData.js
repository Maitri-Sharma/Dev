import React, { useState, useContext } from "react";
import { KundeWebContext } from "../../context/Context";

const PostreklameData = ({ data, parentCallback }) => {
  const { selectedsegment, setselectedsegment } = useContext(KundeWebContext);

  let arr = selectedsegment;
  const select = (e) => {
    if (e.target.checked === false) {
      let k = selectedsegment;
      let last_char_value = e.target.id.slice(e.target.id.lastIndexOf("_") + 1);
      const index = k.indexOf(last_char_value);
      if (index > -1) {
        k.splice(index, 1);
      }
      setselectedsegment(k);
    } else {
      let value = e.target.id;
      let char_no = value.lastIndexOf("_");
      let last_char = value.slice(value.lastIndexOf("_") + 1);
      arr.push(last_char.toString());

      let d = arr.join(",");
      setselectedsegment(arr);
    }
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

export default PostreklameData;
