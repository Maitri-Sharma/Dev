import React from "react";

class HelpGuide extends React.Component {
  constructor() {
    super();
    this.handleHelpGuide = this.handleHelpGuide.bind(this);
  }

  handleHelpGuide() {
    window.open("/help/index.htm", "_blank");
    this.helpReload();
  }

  helpReload() {
    window.location.reload();
  }

  render() {
    const view = this.props.view;
    return view ? (
      <div id="helpGuideDiv">
        <button
          className="esri-widget--button esri-interactive esri-icon-question esri-widget"
          id="helpGuideButton"
          type="button"
          title="Hjelp"
          onClick={this.handleHelpGuide}
        ></button>
      </div>
    ) : null;
  }
}

export default HelpGuide;
