import React, { useState, useRef, useContext, useEffect } from "react";
import { Utvalg, Utvalglist } from "../components/KspuConfig";
import api from "../services/api.js";

export const CreateUtvalglist = async (selectedDataSet) => {
  var a = Utvalglist();
  a.listId = selectedDataSet.listId;
  a.parentList = selectedDataSet.parentList;
  a.name = selectedDataSet.name;
  a.modifications = selectedDataSet.modifications;
  a.logo = selectedDataSet.logo;
  a.kundeNavn = selectedDataSet.kundeNavn;
  a.ordreReferanse = selectedDataSet.ordreReferanse;
  a.ordreType = selectedDataSet.ordreType;
  a.ordreStatus = selectedDataSet.ordreStatus;
  a.kundeNummer = selectedDataSet.kundeNummer;
  a.innleveringsDato = selectedDataSet.innleveringsDato;
  a.avtalenummer = selectedDataSet.avtalenummer;
  a.sistOppdatert = selectedDataSet.sistOppdatert;
  a.sistEndretAv = selectedDataSet.sistEndretAv;
  a.antallWhenLastSaved = selectedDataSet.antallWhenLastSaved;
  a.weight = selectedDataSet.weight;
  a.distributionType = selectedDataSet.distributionType;
  a.distributionDate = selectedDataSet.distributionDate;
  a.isBasis = selectedDataSet.isBasis;
  a.basedOn = selectedDataSet.basedOn;
  a.basedOnName = selectedDataSet.basedOnName;
  a.wasBasedOn = selectedDataSet.wasBasedOn;
  a.wasBasedOnName = selectedDataSet.wasBasedOnName;
  a.allowDouble = selectedDataSet.allowDouble;
  a.listsBasedOnMe = selectedDataSet.listsBasedOnMe;
  a.memberUtvalgs = selectedDataSet.memberUtvalgs;
  a.memberLists = selectedDataSet.memberLists;
  a.antall = selectedDataSet.antall;
  a.thickness = selectedDataSet.thickness;
  a.antallBeforeRecreation = selectedDataSet.antallBeforeRecreation;
  return a;
};
export const GetImageUrl = (utvalgType, isBasis, inList, orderType) => {
  let orderTypeStr = "";
  if (orderType == "1") orderTypeStr = "-låst";
  if (orderType == "2") orderTypeStr = "-tilbud";
  return (
    "IKON-" +
    utvalgType +
    (isBasis ? "-basis" : "") +
    orderTypeStr +
    (inList ? "-liste" : "") +
    ".gif"
  );
};

export const CreateActiveUtvalg = async (selectedDataSet) => {
  let routes = await GetSelectedRoutes(selectedDataSet.reoler);
  let Antall = GetAntall(routes);
  var a = Utvalg();
  a.Business = Antall[1];
  a.ReservedHouseHolds = Antall[2];
  a.hush = Antall[0];
  a.hasReservedReceivers = selectedDataSet.hasReservedReceivers;
  a.oldReolMapMissing = selectedDataSet.oldReolMapMissing;
  a.reolerBeforeRecreation = selectedDataSet.reolerBeforeRecreation;
  a.reoler = selectedDataSet.reoler;
  a.utvalgId = selectedDataSet.utvalgId;
  a.changed = selectedDataSet.changed;
  a.name = selectedDataSet.name;
  a.kundeNavn = selectedDataSet.selectedDataSet;
  a.logo = selectedDataSet.logo;
  a.reolMapName = selectedDataSet.reolMapName;
  a.oldReolMapName = selectedDataSet.oldReolMapName;
  a.skrivebeskyttet = selectedDataSet.skrivebeskyttet;
  a.weight = selectedDataSet.weight;
  a.distributionType = selectedDataSet.distributionType;
  a.distributionDate = selectedDataSet.distributionDate;
  a.arealAvvik = selectedDataSet.arealAvvik;
  a.isBasis = selectedDataSet.isBasis;
  a.basedOn = selectedDataSet.basedOn;
  a.basedOnName = selectedDataSet.basedOnName;
  a.wasBasedOn = selectedDataSet.wasBasedOn;
  a.wasBasedOnName = selectedDataSet.wasBasedOnName;
  a.listId = selectedDataSet.listId;
  a.listName = selectedDataSet.listName;
  a.antallBeforeRecreation = selectedDataSet.antallBeforeRecreation;
  a.totalAntall = selectedDataSet.totalAntall;
  a.ordreReferanse = selectedDataSet.ordreReferanse;
  a.ordreType = selectedDataSet.ordreType;
  a.ordreStatus = selectedDataSet.ordreStatus;
  a.kundeNummer = selectedDataSet.kundeNummer;
  a.innleveringsDato = selectedDataSet.innleveringsDato;
  a.avtalenummer = selectedDataSet.avtalenummer;
  a.thickness = selectedDataSet.thickness;
  a.antallWhenLastSaved = selectedDataSet.antallWhenLastSaved;
  a.modifications = selectedDataSet.modifications;
  a.kommuner = selectedDataSet.kommuner;
  a.receivers = selectedDataSet.receivers;
  a.districts = selectedDataSet.districts;
  a.postalZones = selectedDataSet.postalZones;
  a.criterias = selectedDataSet.criterias;
  a.utvalgsBasedOnMe = selectedDataSet.utvalgsBasedOnMe;
  a.Antall = Antall;
  return a;
};
const GetSelectedRoutes = (data) => {
  let selectedArray = [];
  let selectedRoutes = data.reduce((acc, dt) => {
    if (!(dt.children === undefined)) {
      return acc.concat(GetSelectedRoutes(dt.children));
    } else {
      selectedArray.push(dt.key);
    }
    return acc.concat(dt);
  }, []);
  //setSelectedKeys(selectedArray);
  return selectedRoutes;
};
const GetAntall = (routes) => {
  let houseAntall = 0;
  let businessAntall = 0;
  let householdsReservedAntall = 0;

  routes.map((item) => {
    houseAntall = houseAntall + item.antall.households;
    businessAntall = businessAntall + item.antall.businesses;
    householdsReservedAntall =
      householdsReservedAntall + item.antall.householdsReserved;
  });
  return [houseAntall, businessAntall, householdsReservedAntall];
};

export const FormatDate = (dateString) => {
  let date = new Date(dateString);
  let dato =
    (date.getDate() < 10 ? "0" : "") +
    date.getDate() +
    "." +
    (date.getMonth() < 9 ? "0" : "") +
    (date.getMonth() + 1) +
    "." +
    date.getFullYear();
  return dato;
};
export const NumberFormat = (item) => {
  return item == null || item == "undefined" || item == "" || isNaN(item)
    ? "0"
    : item.toLocaleString("no-NO");
};
export const NumberFormatKW = (item) => {
  return item == null || item == "undefined" || item == "" || isNaN(item)
    ? "0"
    : item.toLocaleString("no-NO");
};
export const CurrentDate = () => {
  let timeElapsed = Date.now();
  let today = new Date(timeElapsed);
  let date = today.toISOString();
  return date;
};
export const imagePath = (path) => {
  // console.log(path, "path function");
  if (path > 36) {
    let tempPath = path % 36 === 0 ? 36 : path % 36;
    // console.log(tempPath, "tempPathfunction");
    return require(`../assets/images/symboler/symbol${Number(tempPath)}.JPG`);
  } else {
    return require(`../assets/images/symboler/symbol${Number(path)}.JPG`);
  }
};

export const DemografyCategories = {
  ald19_23: "Alder alle 19-23",
  ald24_34: "Alder alle 24-34",
  ald35_44: "Alder alle 35-44",
  ald45_54: "Alder alle 45-54",
  ald55_64: "Alder alle 55-64",
  ald65_74: "Alder alle 65-74",
  ald75_84: "Alder alle 75-84",
  ald85_o: "Alder alle 85-",

  C19_23_MEN: "Alder menn 19-23",
  C24_34_MEN: "Alder menn 24-34",
  C35_44_MEN: "Alder menn 35-44",
  C45_54_MEN: "Alder menn 45-54",
  C55_64_MEN: "Alder menn 55-64",
  C65_74_MEN: "Alder menn 65-74",
  C75_84_MEN: "Alder menn 75-84",
  C85_o_MEN: "Alder menn 85-",

  C19_23_KVI: "Alder kvinne 19-23",
  C24_34_KVI: "Alder kvinne 24-34",
  C35_44_KVI: "Alder kvinne 35-44",
  C45_54_KVI: "Alder kvinne 45-54",
  C55_64_KVI: "Alder kvinne 55-64",
  C65_74_KVI: "Alder kvinne 65-74",
  C75_84_KVI: "Alder kvinne 75-84",
  C85_o_KVI: "Alder kvinne 85-",

  grunnskole: "Grunnskole",
  hogskole_1: "Høgskole/Universitet høyt nivå",
  hogskole_u: "Høgskole/Universitet lavt nivå",
  ingen_uopp: "Ingen oppgitt utdanning",
  videregaen: "Videregående skole",

  annen_bo: "Annen bolig",
  blokk: "Blokk/Bygård/Terrassehus",
  bofelleskap: "Bofellesskap",
  enebolig: "Enebolig",
  rekkehus: "Rekkehus",
  annen_bo: "Annen bolig",
  tomannsbolig: "Tomannsbolig",

  h1_aar: "1 år",
  h2_5_aar: "3-5 år",
  h10_20_aar: "11-20 år",
  h30_40_aar: "31-40 år",
  hover_50: "Over 50 år",
  h2_aar: "2 år",
  h5_10_aar: "6-10 år",
  h20_30_aar: "21-30 år",
  h40_50_aar: "41-50 år",
  h_ukjent: "Ukjent",

  aru_50: "Under 50 m2",
  ar60_79: "60-79 m2",
  ar100_119: "100-119 m2",
  ar140_159: "140-159 m2",
  ar200_249: "200-249 m2",
  ar50_59: "50-59 m2",
  ar80_99: "80-99 m2",
  ar120_139: "120-139 m2",
  ar160_199: "160-199 m2",
  ar250_o: "Over 250 m2",

  int0: "Ingen inntekt",
  int0_100: "0 000kr - 100 000kr",
  int100_200: "100 000kr - 200 000kr",
  int200_300: "200 000kr - 300 000kr",
  int300_400: "300 000kr - 4000 000kr",
  int400_500: "400 000kr - 500 000kr",
  int500_600: "500 000kr - 600 000kr",
  int600_700: "600 000kr - 700 000kr",
  int700_800: "700 000kr - 800 000kr",
  int800_1000: "800 000kr - 1000 000kr",
  int1000_1500: "1000 000kr - 1500 000kr",

  enslig_u_b: "Enslig uten barn",
  par_u_barn: "Par uten barn",
  par: "Par",
  enslig_m_b: "Enslig med barn",
  par_m_barn: "Par med barn",
  flerfamili: "Flerfamilie",

  Audi: "Audi",
  Chevrolet: "Chevrolet",
  Citroen: "Citroen",
  Ford: "Ford",
  Hyundai: "Hyundai",
  Mazda: "Mazda",
  Mitsubishi: "Mitsubishi",
  Opel: "Opel",
  Renault: "Renault",
  Skoda: "Skoda",
  Suzuki: "Suzuki",
  Toyota: "Toyota",
  Volvo: "Volvo",
  BMW: "BMW",
  land_rover: "Land Rover",
  Fiat: "Fiat",
  Honda: "Honda",
  Kia: "Kia",
  MERCEDES_BENZ: "Mercedes Benz",
  Nissan: "Nissan",
  Peugeot: "Peugeot",
  Tesla: "Tesla",
  Subaru: "Subaru",
  Saab: "Saab",
  Volkswagen: "Volkswagen",
  Andre: "Andre",

  bilalder_20_: "Biler bygget for 20- år siden eller mer",
  bilalder_14_15: "14-15 år siden",
  bilalder_10_11: "10-11 år siden",
  bilalder_6_7: "6-7 år siden",
  bilalder_4: "4 år siden",
  bilalder_2: "2 år siden",
  bilalder_0: "under 1 år",
  bilalder_16_19: "16-19 år siden",
  bilalder_12_13: "12-13 år siden",
  bilalder_8_9: "8-9 år siden",
  bilalder_5: "5 år siden",
  bilalder_3: "3 år siden",
  bilalder_1: "1 år siden",
};

export const ColorCodes = () => {
  return [
    [255, 0, 219, 0.35], //change color with 2nd last
    [74, 16, 17, 0.35],
    [109, 97, 7, 0.35],
    [0, 100, 58, 0.35],
    [0, 47, 25, 0.35],
    [123, 193, 68, 0.5],
    [253, 187, 47, 0.35],
    [91, 214, 208, 0.35],
    [46, 46, 237, 0.35],
    [208, 61, 216, 0.35],
    [44, 53, 44, 0.5],
    [5, 193, 175, 0.35],
    [0, 113, 188, 0.35],
    [102, 45, 145, 0.35],
    [147, 39, 143, 0.35],
    [158, 0, 93, 0.35],
    [211, 20, 90, 0.35], // Changed color
    [1, 250, 124, 0.35],
    [186, 160, 49, 0.35],
    [252, 157, 223, 0.35],
    [72, 14, 84, 0.35],
    [148, 129, 193, 0.35],
    [171, 199, 239, 0.35],
    [15, 237, 194, 0.35],
    [43, 135, 118, 0.35],
    [35, 158, 35, 0.35],
    [155, 198, 155, 0.35],
    [192, 237, 55, 0.35],
    [229, 207, 137, 0.35],
    [196, 136, 91, 0.35],
    [33, 31, 30, 0.35],
    [118, 118, 118, 0.35], //incorrect color
    [0, 0, 0, 0.35],
    [96, 57, 22, 0.35],
    [40, 39, 5, 0.5],
    [255, 255, 0, 0.35],
  ];
};

export const CommonColorCodes = () => {
  return ["rgba(0, 255, 0, 0.80)"];
};

export const filterCommonReolIds = (items) => {
  let ResultArr = [];
  let commonItems = [];
  let commonSelectionNames = [];

  for (let i = 0; i < items?.length; i++) {
    items[i]?.reoler?.map((item, index) => {
      if (ResultArr.length === 0) {
        ResultArr.push(item?.reolId);
      } else if (ResultArr?.includes(item?.reolId)) {
        commonItems.push(item?.reolId);
      } else {
        ResultArr.push(item?.reolId);
      }
    });
  }

  for (let i = 0; i < items?.length; i++) {
    let flag = false;
    items[i]?.reoler?.map((item, index) => {
      if (commonItems?.includes(item?.reolId)) {
        flag = true;
      }
    });
    if (flag) {
      commonSelectionNames.push(items[i].name);
    }
  }

  let filteredCommonItems = [...new Set(commonItems)];
  let filteredCommonSelectionNames = [...new Set(commonSelectionNames)];

  return { filteredCommonItems, filteredCommonSelectionNames };
};

export const getListData = async (id) => {
  let url = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${id}`;

  try {
    //api.logger.info("APIURL", url);
    const { data, status } = await api.getdata(url);
    if (data.length == 0) {
      //api.logger.error("Error : No Data is present for mentioned Id" + id);
      return 0;
    } else {
      return data;
    }
  } catch (error) {
    return 0;
  }
};
