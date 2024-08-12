import React from "react";

import "./spinner.styles.scss";

class Spinner extends React.Component {
  constructor(props) {
    super(props);
  }

  render() {
    return (
      <div className="loader-container">
        <div className="loader"></div>
      </div>
    );
  }
}

export default Spinner;
