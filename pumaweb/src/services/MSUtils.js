import { getUserDetails } from "./GraphService";
import api from "./api";

export function normalizeError(error) {
  var normalizedError = {};
  if (typeof error === "string") {
    var errParts = error.split("|");
    normalizedError =
      errParts.length > 1
        ? { message: errParts[1], debug: errParts[0] }
        : { message: error };
  } else {
    normalizedError = {
      message: error.message,
      debug: JSON.stringify(error),
    };
  }
  return normalizedError;
}

export async function getUserProfile(userAgentApplication, scopes) {
  try {
    var accessToken = await getAccessToken(userAgentApplication, scopes);
    if (accessToken) {
      return await getUserDetails(accessToken);
    }

    return null;
  } catch (err) {
    throw err;
  }
}

async function getAccessToken(userAgentApplication, scopes) {
  try {
    var silentResult = await userAgentApplication.acquireTokenPopup({
      scopes: scopes,
    });

    return silentResult.accessToken;
  } catch (err) {
    if (isInteractionRequired(err)) {
      var interactiveResult = await userAgentApplication.acquireTokenPopup({
        scopes: scopes,
      });

      return interactiveResult.accessToken;
    } else {
      throw err;
    }
  }
}

export async function generateAPIToken(userName,key) {
  try {
    var isCustomerWeb = `${process.env.REACT_APP_ISCUSTOMERWEB}`;
let queryString={
  "userName": userName,
  "token": key,
  "isFromKundeWeb": isCustomerWeb
}
    var tokenUrl = `Login/gettoken`;

    const { data, status } = await api.getToken(tokenUrl,queryString);

    if (data.length !== 0) {
      sessionStorage.setItem("Token", data);
    } else {
      //api.logger.info("Token not able to generates");
    }

    return data;
  } catch (err) {
    return null;
  }
}

function isInteractionRequired(error) {
  if (!error.message || error.message.length <= 0) {
    return false;
  }

  return (
    error.message.indexOf("consent_required") > -1 ||
    error.message.indexOf("interaction_required") > -1 ||
    error.message.indexOf("login_required") > -1 ||
    error.message.indexOf("no_account_in_silent_request") > -1
  );
}
