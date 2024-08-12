import React from "react";

function PostreklameData1({ data }) {
  return (
    <div>
      <input
        className="form-check-input  "
        type="checkbox"
        value={data.name}
        id={data.name}
      />
      <label className="form-check-label" htmlFor={data.name}>
        {" "}
        {data.name}{" "}
      </label>
    </div>
  );
}

export default PostreklameData1;
