export const AdressepunktOgFastantallsanalyseConfig = {
  defaultBuffer: 10000,
  defaultDistance: 150,
};
export const NewUtvalgName = () => "Påbegynt utvalg";
export const NewUtvalg = () => "Nytt utvalg";
export const geoKodingObj = () => ({
  OBJECTID: 0,
  Postnummer: "",
  Adresse: "",
  Poststed: "",
});
export const Reol = () => ({
  name: "",
  reolNumber: "",
  description: "",
  comment: null,
  descriptiveName: "",
  reolId: 0,
  kommuneId: "",
  kommune: "",
  kommuneFullDistribusjon: null,
  fylkeId: "",
  fylke: "",
  teamNumber: "",
  teamName: "",
  postalZone: "",
  postalArea: "",
  segmentId: null,
  antall: {
    households: 0,
    householdsReserved: 0,
    farmers: 0,
    farmersReserved: 0,
    houses: 0,
    housesReserved: 0,
    includeHousesReserved: 0,
    businesses: 0,
    totalReserved: 0,
    priorityHouseholdsReserved: 0,
    nonPriorityHouseholdsReserved: 0,
    priorityBusinessReserved: 0,
    nonPriorityBusinessReserved: 0,
  },
  avisDeknings: null,
  prisSone: 0,
  ruteType: "",
  postkontorNavn: "",
  prsEnhetsId: "",
  prsName: "",
  prsDescription: "",
  frequency: "",
  sondagFlag: "",
});

export const criterias = (type, text) => {
  return { criteriaType: type, criteria: text };
};
export const criterias_KW = (type, text) => {
  return { criteriaType: type, criteria: getCriteriaText(text) };
};

export const getCriteriaText = (key) => {
  switch (key) {
    case 3:
      return "Geografi fra Fylke";
    case 4:
      return "Geografi fra kommune";
    case 5:
      return "Geografi fra team";
    case 21:
      return "Geografi fra budrute";
    case 6:
      return "Geografi fra postsone";
    case 19:
      return "Geografiplukkliste";
    case 12:
      return "Demografi";
    case 2:
      return "Segment";
    case 8:
      return "Kjøreanalyse";
    case 9:
      return "Fra adressepunkt";
    case 100:
      return "Geografi";
    case 11:
      return "Fastantall rundt adressepunkt";
    case 200:
      return "Budruter";
    case 10:
      return "Valgt enkeltvis";
  }
};

export const getAntallUtvalg = (routes) => {
  let houseAntall = 0;
  let businessAntall = 0;
  let householdsReservedAntall = 0;

  routes.map((item) => {
    if (item?.antall) {
      houseAntall = houseAntall + item.antall.households;
      businessAntall = businessAntall + item.antall.businesses;
      householdsReservedAntall =
        householdsReservedAntall + item.antall.householdsReserved;
    } else {
      houseAntall = houseAntall + item.house;
      businessAntall = businessAntall + item.businesses;
      householdsReservedAntall =
        householdsReservedAntall + item.householdsReserved;
    }
  });
  return [houseAntall, businessAntall, householdsReservedAntall];
};

export const getAntall = (routes) => {
  let houseAntall = 0;
  let businessAntall = 0;
  let householdsReservedAntall = 0;

  routes.map((item) => {
    houseAntall = houseAntall + item.house;
    businessAntall = businessAntall + item.businesses;
    householdsReservedAntall =
      householdsReservedAntall + item.householdsReserved;
  });
  return [houseAntall, businessAntall, householdsReservedAntall];
};
export const getAntall_KW = (routes) => {
  let houseAntall = 0;
  let businessAntall = 0;
  let householdsReservedAntall = 0;

  routes.map((item) => {
    houseAntall = houseAntall + item.antall.houses;
    businessAntall = businessAntall + item.antall.businesses;
    householdsReservedAntall =
      householdsReservedAntall + item.antall.householdsReserved;
  });
  return [houseAntall, businessAntall, householdsReservedAntall];
};

export const kundeweb_utvalg = () => ({
  hasReservedReceivers: false,
  oldReolMapMissing: true,
  reolerBeforeRecreation: [],
  reoler: [],
  utvalgId: 0,
  changed: false,
  name: "",
  kundeNavn: "",
  logo: "",
  reolMapName: "",
  oldReolMapName: "",
  skrivebeskyttet: true,
  weight: 0,
  distributionType: 1,
  distributionDate: "0001-01-01T00:00:00",
  arealAvvik: 0,
  isBasis: false,
  basedOn: 0,
  basedOnName: "",
  wasBasedOn: 0,
  wasBasedOnName: "",
  listId: 0,
  listName: "",
  antallBeforeRecreation: 0,
  totalAntall: 0,
  ordreReferanse: "",
  ordreType: 0,
  ordreStatus: 0,
  kundeNummer: 0,
  innleveringsDato: "0001-01-01T00:00:00",
  avtalenummer: 0,
  thickness: 0,
  antallWhenLastSaved: 0,
  modifications: [],
  kommuner: [],
  receivers: [
    {
      receiverId: 1,
      selected: true,
    },
  ],
  districts: [],
  postalZones: [],
  Antall: [],
  criterias: [],
  utvalgsBasedOnMe: [],
});

export const saveUtvalg = () => ({
  hasReservedReceivers: false,
  oldReolMapMissing: true,
  reolerBeforeRecreation: [],
  reoler: [],
  utvalgId: 0,
  changed: false,
  name: "abc_test",
  kundeNavn: "test",
  logo: "",
  reolMapName: "",
  oldReolMapName: "",
  skrivebeskyttet: true,
  weight: 0,
  distributionType: 0,
  distributionDate: "0001-01-01T00:00:00",
  arealAvvik: 0,
  isBasis: false,
  basedOn: 0,
  basedOnName: "",
  wasBasedOn: 0,
  wasBasedOnName: "",
  listId: 0,
  listName: "",
  antallBeforeRecreation: 0,
  totalAntall: 0,
  ordreReferanse: "",
  ordreType: 0,
  ordreStatus: 0,
  kundeNummer: 0,
  innleveringsDato: "0001-01-01T00:00:00",
  avtalenummer: 0,
  thickness: 0,
  antallWhenLastSaved: 0,
  modifications: [],
  kommuner: [],
  receivers: [],
  districts: [],
  postalZones: [],
  Antall: [],
  criterias: [],
  utvalgsBasedOnMe: [],
});

export const Utvalglist = () => ({
  listId: 0,
  parentList: {
    listId: 0,
    parentList: "",
    modifications: [],
    logo: "",
    name: "",
    kundeNavn: "",
    ordreReferanse: "",
    ordreType: 0,
    ordreStatus: 0,
    kundeNummer: "",
    innleveringsDato: "0001-01-01T00:00:00",
    avtalenummer: 0,
    sistOppdatert: "",
    sistEndretAv: "",
    antallWhenLastSaved: 0,
    weight: 0,
    distributionType: 0,
    distributionDate: "0001-01-01T00:00:00",
    isBasis: false,
    basedOn: 0,
    basedOnName: "",
    wasBasedOn: 0,
    wasBasedOnName: "",
    allowDouble: true,
    listsBasedOnMe: [],
    memberUtvalgs: [],
    memberLists: [],
    antall: 0,
    thickness: 0,
  },
  modifications: [],
  logo: " ",
  name: " ",
  kundeNavn: " ",
  ordreReferanse: "",
  antallBeforeRecreation: 0,
  ordreType: 0,
  ordreStatus: 0,
  kundeNummer: " ",
  innleveringsDato: "0001-01-01T00:00:00",
  avtalenummer: 0,
  sistOppdatert: "",
  sistEndretAv: " ",
  antallWhenLastSaved: 0,
  weight: 0,
  distributionType: 0,
  distributionDate: "0001-01-01T00:00:00",
  isBasis: false,
  basedOn: 0,
  basedOnName: "",
  wasBasedOn: 0,
  wasBasedOnName: " ",
  allowDouble: true,
  listsBasedOnMe: [],
  memberUtvalgs: [],
  memberLists: [],
  antall: 0,
  thickness: 0,
});
export const Utvalg = () => ({
  hasReservedReceivers: false,
  oldReolMapMissing: false,
  reolerBeforeRecreation: null,
  reoler: [],
  utvalgId: 0,
  changed: false,
  name: "",
  kundeNavn: "",
  logo: "",
  reolMapName: "",
  oldReolMapName: "",
  skrivebeskyttet: false,
  weight: 0,
  distributionType: 0,
  distributionDate: "0001-01-01T00:00:00",
  arealAvvik: 0,
  isBasis: false,
  basedOn: 0,
  basedOnName: "",
  wasBasedOn: 0,
  wasBasedOnName: "",
  listId: 0,
  listName: "",
  antallBeforeRecreation: 0,
  totalAntall: 0,
  ordreReferanse: "",
  ordreType: 0,
  ordreStatus: 0,
  kundeNummer: "",
  innleveringsDato: "0001-01-01T00:00:00",
  avtalenummer: 0,
  thickness: 0,
  antallWhenLastSaved: 0,
  modifications: [],
  kommuner: [],
  receivers: [],
  districts: [],
  postalZones: [],
  criterias: [],
  utvalgsBasedOnMe: null,
  Antall: [],
});

export function segmenter_kriterier() {
  const segmenter = [
    {
      id: "uxSegmenterAnalyse_SegmenterKriterier1_A",
      name: "Senior Ordinær",
    },
    {
      id: "uxSegmenterAnalyse_SegmenterKriterier1_B",
      name: "Senior Aktiv",
    },
    {
      id: "uxSegmenterAnalyse_SegmenterKriterier1_C1",
      name: "Urban Ung",
    },
    {
      id: "uxSegmenterAnalyse_SegmenterKriterier1_C2",
      name: "Urban Moden",
    },
    {
      id: "uxSegmenterAnalyse_SegmenterKriterier1_D",
      name: "Ola og Kari Tradisjonell",
    },
    {
      id: "uxSegmenterAnalyse_SegmenterKriterier1_E",
      name: "Ola og Kari Individualist",
    },
    {
      id: "uxSegmenterAnalyse_SegmenterKriterier1_F",
      name: "Barnefamilie Velstand og Kultur",
    },
    {
      id: "uxSegmenterAnalyse_SegmenterKriterier1_G",
      name: "Barnefamilie Barnerik",
    },
    {
      id: "uxSegmenterAnalyse_SegmenterKriterier1_H",
      name: "Barnefamilie Prisbevisst",
    },
    {
      id: "uxSegmenterAnalyse_SegmenterKriterier1_I",
      name: "Barnefamilie Moderne Aktiv",
    },
  ];

  const demografi_alder = [
    {
      id: "DemografiAnalyse1_DemografiKriterier1_uxFPAlder_19_23",
      name: "19_23",
    },
    {
      id: "DemografiAnalyse1_DemografiKriterier1_uxFPAlder_24_34",
      name: "24-34",
    },
    {
      id: "DemografiAnalyse1_DemografiKriterier1_uxFPAlder_35_44",
      name: "35-44",
    },
    {
      id: "DemografiAnalyse1_DemografiKriterier1_uxFPAlder_45_54",
      name: "45-54",
    },
    {
      id: "DemografiAnalyse1_DemografiKriterier1_uxFPAlder_65_74",
      name: "65-74",
    },
    {
      id: "DemografiAnalyse1_DemografiKriterier1_uxFPAlder_75_84",
      name: "75-84",
    },
    {
      id: "DemografiAnalyse1_DemografiKriterier1_uxFPAlder_85_94",
      name: "85-94",
    },
  ];

  return segmenter;
}

export function test(namevalue, agevalue) {
  const testobj = {
    name: namevalue,
  };
  return testobj;
}

export function formatData(reolObj) {
  var r = Reol();
  r.name = reolObj.reolnavn;
  r.reolNumber = reolObj.reolnr;
  r.description = reolObj.beskrivelse;
  r.comment = reolObj.kommentar;
  r.descriptiveName = reolObj.beskrivelse + " (" + reolObj.reol_id + ")";
  r.reolId = reolObj.reol_id;
  r.kommuneId = reolObj.kommuneid;
  r.kommune = reolObj.kommune;
  r.kommuneFullDistribusjon = null;
  r.fylkeId = reolObj.fylkeid;
  r.fylke = reolObj.fylke;
  r.teamNumber = reolObj.teamnr;
  r.teamName = reolObj.teamnavn;
  r.postalZone = reolObj.postnr;
  r.postalArea = reolObj.poststed;
  r.segmentId = reolObj.segment;
  r.antall = {
    households: reolObj.hh,
    householdsReserved: reolObj.hh_res,
    farmers: reolObj.gb,
    farmersReserved: reolObj.gb_res,
    houses: reolObj.er,
    housesReserved: reolObj.er_res,
    includeHousesReserved: 0,
    businesses: reolObj.vh,
    totalReserved: 0,
    priorityHouseholdsReserved: reolObj.p_hh_u_res,
    nonPriorityHouseholdsReserved: reolObj.np_hh_u_res,
    priorityBusinessReserved: reolObj.p_vh_u_res,
    nonPriorityBusinessReserved: reolObj.np_vh_u_res,
  };
  r.avisDeknings = null;
  r.prisSone = reolObj.prissone;
  r.ruteType = reolObj.reoltype;
  r.postkontorNavn = reolObj.pbkontnavn;
  r.prsEnhetsId = reolObj.prsnr;
  r.prsName = reolObj.prsnavn;
  r.prsDescription = reolObj.prsbeskrivelse;
  r.frequency = reolObj.rutedistfreq;
  r.sondagFlag = reolObj.sondagflag;
  if (r.description === undefined || r.description === "")
    r.description = reolObj.pbkontnavn;
  if (r.description === undefined || r.description === "")
    r.description = reolObj.prsbeskrivelse;

  // 08.08.2006 - Reolnavn skal brukes dersom den har verdi, ellers får den
  // beskrivelse verdien
  if (r.name === undefined || r.name === "" || r.name === null)
    r.name = r.description;
  return r;
}

export default {
  segmenter_kriterier: segmenter_kriterier,
  getCriteriaText: getCriteriaText,
};
