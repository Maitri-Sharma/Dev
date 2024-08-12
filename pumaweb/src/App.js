import React, { useState, useEffect } from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { Navigate } from "react-router";

import Interweb from "./pages/intern-web/intern-web.component";
import KundeWeb from "./pages/kunde-web/kunde-web.component";
import AbandonPage from "./pages/abandon/abandon.component";
import PageNotFound from "./pages/page-not-found/page-not-found.component";
import UnAuthenticatedUser from "./pages/unauthenticated-user/unauthenticated-user.component";

import { MainPageContext } from "./context/Context";

import "./App.css";

import { UserAgentApplication } from "msal";
import { msalConfig } from "./authProvider";
import { getUserProfile, generateAPIToken } from "./services/MSUtils";

function App() {
  var userName = "";
  const [mapView, setMapView] = useState("");
  const [internuserName, setinternuserName] = useState("");
  const [addressPoints, setAddressPoints] = useState([]);
  const [budruterMapView, setbudruterMapview] = useState([]);
  const [ActiveMapButton, setActiveMapButton] = useState("");
  const [isTokenCreated, setIsTokenCreated] = useState(false);

  var key = "";

  useEffect(async () => {
    // If Kundeweb URL

    if (window.location.href.toLowerCase().includes("pumakundeweb")) {
      try {
        const url = new URL(window.location.href.replaceAll("amp;",""));
        userName = url.searchParams.get("kw_username");

        sessionStorage.setItem("userName", userName);
        var xxcu_qty = url.searchParams.get("xxcu_qty");
        key = url.searchParams.get("kw_key");
        sessionStorage.setItem("key", key);
        var xxcu_section = url.searchParams.get("xxcu_section");
        var xxcu_item = url.searchParams.get("xxcu_item");
        var xxcu_type = url.searchParams.get("xxcu_type");
        var kwUtvalgsType = url.searchParams.get("utvalgstype");
        var kwUtvalgsId = url.searchParams.get("utvalgid");
        var xxcu_channel = url.searchParams.get("xxcu_channel");
        var xxcu_parameters = `xxcu_refpage=ibeCCtpSctDspRte.jsp?section=10021&xxcu_qty=${xxcu_qty}&xxcu_section=${xxcu_section}&xxcu_item=${xxcu_item}&xxcu_channel=${xxcu_channel}&xxcu_type=${xxcu_type}`;
        sessionStorage.setItem("xxcu_parameters", xxcu_parameters);
        if (kwUtvalgsId && kwUtvalgsType) {
          sessionStorage.setItem("kwUtvalgsType", kwUtvalgsType);
          sessionStorage.setItem("kwUtvalgsId", kwUtvalgsId);
        }
        if (userName) {
          setinternuserName(userName);
          await generateAPIToken(userName, key);
          setIsTokenCreated(true);
        } else if (
          `${process.env.REACT_APP_AUTHENTICATION_REQUIRED}` === "false"
        ) {
          // Normally for development envionments
          userName = "Internbruker";
          sessionStorage.setItem("userName", userName);
          setinternuserName(userName);
          await generateAPIToken(userName, key);
          setIsTokenCreated(true);
        } else {
          userName = "Internbruker";
          setinternuserName(userName);
          // await generateAPIToken(userName);

          sessionStorage.clear(); //Delete all from session storage
          window.location.href = `${process.env.REACT_APP_KUNDEWEB_LOGIN_URL}`;
        }
      } catch (err) {
        userName = "";
        setinternuserName(userName);
        sessionStorage.clear();
      }
    } else {
      // If Internweb URL
      try {
        if (`${process.env.REACT_APP_AUTHENTICATION_REQUIRED}` == "false") {
          userName = "Internbruker";
          sessionStorage.setItem("userName", userName);
          setinternuserName(userName);
          await generateAPIToken(userName, key);
        } else if (
          sessionStorage.getItem("userName") == null ||
          sessionStorage.getItem("userName") == ""
        ) {
          const userAgentApplication = new UserAgentApplication({
            auth: {
              clientId: msalConfig.auth.clientId,
              redirectUri: msalConfig.auth.redirectUri,
              authority: msalConfig.auth.authority,
            },
            cache: {
              cacheLocation: msalConfig.cache.cacheLocation,
              storeAuthStateInCookie: msalConfig.cache.storeAuthStateInCookie,
            },
          });

          await userAgentApplication.loginPopup({
            scopes: msalConfig.auth.scopes,
            prompt: "select_account",
          });

          const user = await getUserProfile(
            userAgentApplication,
            msalConfig.auth.scopes
          );

          userName = user.userPrincipalName;

          setinternuserName(userName);
          sessionStorage.setItem("userName", userName);
          await generateAPIToken(userName, key);

          window.location.href = window.location.origin;
        }
      } catch (err) {
        userName = "";
        setinternuserName(userName);
        sessionStorage.clear();
      }
    }
  }, []);

  if (
    sessionStorage.getItem("userName") == null ||
    sessionStorage.getItem("userName") == ""
  ) {
    return <UnAuthenticatedUser />;
  } else {
    return (
      <MainPageContext.Provider
        value={{
          ActiveMapButton,
          setActiveMapButton,
          mapView,
          setMapView,
          addressPoints,
          setAddressPoints,
          budruterMapView,
          setbudruterMapview,
        }}
      >
        <Router>
          <Routes>
            {!window.location.href.toLowerCase().includes("pumakundeweb") ? (
              <Route path="/" element={<Interweb />} />
            ) : (
              <Route path="/" element={ isTokenCreated ? <KundeWeb /> : <div/>} />
            )}
            <Route path="/Abandon" element={<AbandonPage />} />
            <Route
              path="help"
              element={<Navigate replace to={"../help/index.htm"} />}
            />
            <Route path="/*" element={<PageNotFound />} />
          </Routes>
        </Router>
      </MainPageContext.Provider>
    );
  }
}

export default App;
