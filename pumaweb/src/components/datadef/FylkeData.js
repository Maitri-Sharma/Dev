import React, { useState, useEffect } from "react";

const FylkeData = ({ data, setSelectedGroups, selectedGroups }) => {
  const handleChange = (e) => {
    let newArray = [...selectedGroups, e.target.id];
    if (selectedGroups.includes(e.target.id)) {
      newArray = newArray.filter((day) => day !== e.target.id);
    }
    setSelectedGroups(newArray);
  };

  return (
    <div>
      <input
        className="form-check-input mt-1 "
        type="checkbox"
        onChange={handleChange}
        value={data.fylkeID}
        id={data.fylkeID}
      />
      <label className="form-check-label" htmlFor={data.fylkeID}>
        {" "}
        {data.fylkeName}{" "}
      </label>
    </div>
  );
};

export default FylkeData;
