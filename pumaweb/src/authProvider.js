// authProvider.js
import { MsalAuthProvider } from "react-aad-msal";

// MSAL Configurations
export const msalConfig = {
  auth: {
    clientId: `${process.env.REACT_APP_AAD_CLIENTID}`,
    authority: "https://login.microsoftonline.com/postennorge.onmicrosoft.com",
    redirectUri: window.location.origin,
    validateAuthority: true,
    navigateToLoginRequestUrl: true,
    postLogoutRedirectUri: window.location.origin,
    scopes: ["user.read"],
  },
  cache: {
    cacheLocation: "sessionStorage",
    storeAuthStateInCookie: true,
  },
};

export const authProvider = new MsalAuthProvider(msalConfig);
