import api from "./services/api";
export const GetData = async (
  url,
  key,
  level,
  showHouse = false,
  show = false,
  showReserved = false,
  picklistData = [],
  routes,
  picklistFlag,
  postAPI = false,
  apiRequest
) => {
  return await fetchData(
    url,
    key,
    level,
    showHouse,
    show,
    showReserved,
    picklistData,
    routes,
    picklistFlag,
    postAPI,
    apiRequest
  );
};

const distributionDetails = (dayDetails, showHousehold, showBusiness, data) => {
  let VHD1 = 0;
  let VHD2 = 0;
  let HHD1 = 0;
  let HHD2 = 0;

  switch (dayDetails) {
    case "A-uke, tidliguke":
      if (data.frequency == "A") {
        if (showBusiness) {
          VHD1 = 0;
          VHD2 = 0;
        }
        if (showHousehold) {
          HHD1 =
            data.antall.priorityHouseholdsReserved +
            data.antall.nonPriorityHouseholdsReserved;
          HHD2 = 0;
        }
      } else if (data.frequency == "B") {
        if (showBusiness) {
          VHD1 = 0;
          VHD2 = 0;
        }
        if (showHousehold) {
          HHD1 = data.antall.priorityHouseholdsReserved;
          HHD2 = data.antall.nonPriorityHouseholdsReserved;
        }
      } else if (data.frequency == "0") {
        if (showBusiness) {
          VHD1 = 0;
          VHD2 = 0;
        }
        if (showHousehold) {
          HHD1 =
            data.antall.priorityHouseholdsReserved +
            data.antall.nonPriorityHouseholdsReserved;
          HHD2 = 0;
        }
      }
    case "A-uke, midtuke":
      if (data.frequency == "A") {
        if (showBusiness) {
          VHD1 =
            data.antall.priorityBusinessReserved +
            data.antall.nonPriorityBusinessReserved;
          VHD2 = 0;
        }
        if (showHousehold) {
          HHD1 =
            data.antall.priorityHouseholdsReserved +
            data.antall.nonPriorityHouseholdsReserved;
          HHD2 = 0;
        }
      } else if (data.frequency == "B") {
        if (showBusiness) {
          VHD1 = 0;
          VHD2 =
            data.antall.priorityBusinessReserved +
            data.antall.nonPriorityBusinessReserved;
        }
        if (showHousehold) {
          HHD1 = 0;
          HHD2 =
            data.antall.priorityHouseholdsReserved +
            data.antall.nonPriorityHouseholdsReserved;
        }
      } else if (data.frequency == "0") {
        if (showBusiness) {
          VHD1 =
            data.antall.priorityBusinessReserved +
            data.antall.nonPriorityBusinessReserved;
          VHD2 = 0;
        }
        if (showHousehold) {
          HHD1 =
            data.antall.priorityHouseholdsReserved +
            data.antall.nonPriorityHouseholdsReserved;
          HHD2 = 0;
        }
      }
    case "B-uke, tidliguke":
      if (data.frequency == "A") {
        if (showBusiness) {
          VHD1 = 0;
          VHD2 = 0;
        }
        if (showHousehold) {
          HHD1 = data.antall.priorityHouseholdsReserved;
          HHD2 = data.antall.nonPriorityHouseholdsReserved;
        }
      } else if (data.frequency == "B") {
        if (showBusiness) {
          VHD1 = 0;
          VHD2 = 0;
        }
        if (showHousehold) {
          HHD1 =
            data.antall.priorityHouseholdsReserved +
            data.antall.nonPriorityHouseholdsReserved;
          HHD2 = 0;
        }
      } else if (data.frequency == "0") {
        if (showBusiness) {
          VHD1 = 0;
          VHD2 = 0;
        }
        if (showHousehold) {
          HHD1 =
            data.antall.priorityHouseholdsReserved +
            data.antall.nonPriorityHouseholdsReserved;
          HHD2 = 0;
        }
      }
    case "B-uke, midtuke":
      if (data.frequency == "A") {
        if (showBusiness) {
          VHD1 = 0;
          VHD2 =
            data.antall.priorityBusinessReserved +
            data.antall.nonPriorityBusinessReserved;
        }
        if (showHousehold) {
          HHD1 = 0;
          HHD2 =
            data.antall.priorityHouseholdsReserved +
            data.antall.nonPriorityHouseholdsReserved;
        }
      } else if (data.frequency == "B") {
        if (showBusiness) {
          VHD1 =
            data.antall.priorityBusinessReserved +
            data.antall.nonPriorityBusinessReserved;
          VHD2 = 0;
        }
        if (showHousehold) {
          HHD1 =
            data.antall.priorityHouseholdsReserved +
            data.antall.nonPriorityHouseholdsReserved;
          HHD2 = 0;
        }
      } else if (data.frequency == "0") {
        if (showBusiness) {
          VHD1 =
            data.antall.priorityBusinessReserved +
            data.antall.nonPriorityBusinessReserved;
          VHD2 = 0;
        }
        if (showHousehold) {
          HHD1 =
            data.antall.priorityHouseholdsReserved +
            data.antall.nonPriorityHouseholdsReserved;
          HHD2 = 0;
        }
      }
  }

  return { VHD1, VHD2, HHD1, HHD2 };
};

const flykefn = (
  data,
  showHousehold,
  showBusiness,
  showReserved,
  level,
  dayDetails
) => {
  let zone0 = 0;
  let zone1 = 0;
  let zone2 = 0;
  let VH1 = 0;
  let VH2 = 0;
  let HH1 = 0;
  let HH2 = 0;
  let H0 = 0;
  let H1 = 0;
  let H2 = 0;
  let V0 = 0;
  let V1 = 0;
  let V2 = 0;

  if (dayDetails != "") {
    let { VHD1, VHD2, HHD1, HHD2 } = distributionDetails(
      dayDetails,
      showHousehold,
      showBusiness,
      data
    );
    VH1 = VHD1;
    VH2 = VHD2;
    HH1 = HHD1;
    HH2 = HHD2;
  }

  switch (data.prisSone) {
    case 0:
      zone0 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H0 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V0 = showBusiness ? data.antall.businesses : 0;
      break;
    case 1:
      zone1 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H1 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V1 = showBusiness ? data.antall.businesses : 0;
      break;
    case 2:
      zone2 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H2 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V2 = showBusiness ? data.antall.businesses : 0;
      break;
  }
  return {
    name: data.fylke,
    cat: "fylke",
    prisZone: data.prisSone,
    key: parseInt(data.fylkeId),
    house: data.antall.households,
    householdsReserved: data.antall.householdsReserved,
    businesses: data.antall.businesses,
    HHD1: HH1,
    HHD2: HH2,
    VHD1: VH1,
    VHD2: VH2,
    zone0: zone0,
    zone1: zone1,
    zone2: zone2,
    pkey: "null",
    H0: H0,
    H1: H1,
    H2: H2,
    V0: V0,
    V1: V1,
    V2: V2,
    total:
      (showHousehold ? data.antall.households : 0) +
      (showBusiness ? data.antall.businesses : 0) +
      (showReserved ? data.antall.householdsReserved : 0),
    children: [],
  };
};

const kommunefn = (
  data,
  showHousehold,
  showBusiness,
  showReserved,
  level,
  dayDetails
) => {
  let zone0 = 0;
  let zone1 = 0;
  let zone2 = 0;
  let VH1 = 0;
  let VH2 = 0;
  let HH1 = 0;
  let HH2 = 0;
  let H0 = 0;
  let H1 = 0;
  let H2 = 0;
  let V0 = 0;
  let V1 = 0;
  let V2 = 0;

  if (dayDetails != "") {
    let { VHD1, VHD2, HHD1, HHD2 } = distributionDetails(
      dayDetails,
      showHousehold,
      showBusiness,
      data
    );
    VH1 = VHD1;
    VH2 = VHD2;
    HH1 = HHD1;
    HH2 = HHD2;
  }
  switch (data.prisSone) {
    case 0:
      zone0 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H0 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V0 = showBusiness ? data.antall.businesses : 0;
      break;
    case 1:
      zone1 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H1 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V1 = showBusiness ? data.antall.businesses : 0;
      break;
    case 2:
      zone2 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H2 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V2 = showBusiness ? data.antall.businesses : 0;
      break;
  }

  return {
    name: data.kommune,
    cat: "kommune",
    prisZone: data.prisSone,
    key: parseInt(data.kommuneId),
    house: data.antall.households,
    householdsReserved: data.antall.householdsReserved,
    businesses: data.antall.businesses,
    HHD1: HH1,
    HHD2: HH2,
    VHD1: VH1,
    VHD2: VH2,
    zone0: zone0,
    zone1: zone1,
    zone2: zone2,
    pkey: level === 1 ? "null" : parseInt(data.fylkeId),
    H0: H0,
    H1: H1,
    H2: H2,
    V0: V0,
    V1: V1,
    V2: V2,
    total:
      (showHousehold ? data.antall.households : 0) +
      (showBusiness ? data.antall.businesses : 0) +
      (showReserved ? data.antall.householdsReserved : 0),
    children: [],
  };
};

const teamfn = (
  data,
  showHousehold,
  showBusiness,
  showReserved,
  level,
  dayDetails,
  picklistFlag
) => {
  let zone0 = 0;
  let zone1 = 0;
  let zone2 = 0;
  let VH1 = 0;
  let VH2 = 0;
  let HH1 = 0;
  let HH2 = 0;
  let H0 = 0;
  let H1 = 0;
  let H2 = 0;
  let V0 = 0;
  let V1 = 0;
  let V2 = 0;

  if (dayDetails != "") {
    let { VHD1, VHD2, HHD1, HHD2 } = distributionDetails(
      dayDetails,
      showHousehold,
      showBusiness,
      data
    );
    VH1 = VHD1;
    VH2 = VHD2;
    HH1 = HHD1;
    HH2 = HHD2;
  }
  switch (data.prisSone) {
    case 0:
      zone0 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H0 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V0 = showBusiness ? data.antall.businesses : 0;
      break;
    case 1:
      zone1 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H1 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V1 = showBusiness ? data.antall.businesses : 0;
      break;
    case 2:
      zone2 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H2 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V2 = showBusiness ? data.antall.businesses : 0;
      break;
  }
  return {
    name:
      picklistFlag === 2
        ? data.postalZone + "-" + data.postalArea
        : data.teamName,
    cat: "team",
    prisZone: data.prisSone,
    key:
      picklistFlag === 2
        ? data.kommuneId + data.teamName + data.postalArea + data.postalZone
        : picklistFlag === 1
        ? data.teamName
        : data.kommuneId + data.teamName,
    house: data.antall.households,
    householdsReserved: data.antall.householdsReserved,
    businesses: data.antall.businesses,
    HHD1: HH1,
    HHD2: HH2,
    VHD1: VH1,
    VHD2: VH2,
    zone0: zone0,
    zone1: zone1,
    zone2: zone2,
    pkey: level === 2 ? "null" : parseInt(data.kommuneId),
    H0: H0,
    H1: H1,
    H2: H2,
    V0: V0,
    V1: V1,
    V2: V2,
    total:
      (showHousehold ? data.antall.households : 0) +
      (showBusiness ? data.antall.businesses : 0) +
      (showReserved ? data.antall.householdsReserved : 0),
    children: [],
  };
};

const routefn = (
  data,
  showHousehold,
  showBusiness,
  showReserved,
  level,
  dayDetails,
  picklistFlag
) => {
  let zone0 = 0;
  let zone1 = 0;
  let zone2 = 0;
  let VH1 = 0;
  let VH2 = 0;
  let HH1 = 0;
  let HH2 = 0;
  let H0 = 0;
  let H1 = 0;
  let H2 = 0;
  let V0 = 0;
  let V1 = 0;
  let V2 = 0;

  if (dayDetails != "") {
    let { VHD1, VHD2, HHD1, HHD2 } = distributionDetails(
      dayDetails,
      showHousehold,
      showBusiness,
      data
    );
    VH1 = VHD1;
    VH2 = VHD2;
    HH1 = HHD1;
    HH2 = HHD2;
  }
  switch (data.prisSone) {
    case 0:
      zone0 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H0 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V0 = showBusiness ? data.antall.businesses : 0;
      break;
    case 1:
      zone1 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H1 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V1 = showBusiness ? data.antall.businesses : 0;
      break;
    case 2:
      zone2 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H2 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V2 = showBusiness ? data.antall.businesses : 0;
      break;
  }

  return {
    name: data.name,
    cat: "rute",
    prisZone: data.prisSone,
    key: parseInt(data.reolId),
    house: data.antall.households,
    householdsReserved: data.antall.householdsReserved,
    businesses: data.antall.businesses,
    HHD1: HH1,
    HHD2: HH2,
    VHD1: VH1,
    VHD2: VH2,
    zone0: zone0,
    zone1: zone1,
    zone2: zone2,
    pkey:
      level === 3
        ? "null"
        : picklistFlag === 2
        ? data.kommuneId + data.teamName + data.postalArea + data.postalZone
        : picklistFlag === 1
        ? data.teamName
        : data.kommuneId + data.teamName,
    H0: H0,
    H1: H1,
    H2: H2,
    V0: V0,
    V1: V1,
    V2: V2,
    indexData: data.indexData,
    segmentId: data.segmentId,
    total:
      (showHousehold ? data.antall.households : 0) +
      (showBusiness ? data.antall.businesses : 0) +
      (showReserved ? data.antall.householdsReserved : 0),
  };
};

export const groupBy = (
  data,
  key,
  level,
  showHouse,
  show,
  showReserved,
  picklistData,
  dayDetails = "",
  picklistFlag
) => {
  if (data?.length !== undefined && data?.length > 0) {
    let formattedData = data.reduce((acc, item) => {
      if (item === null || item === undefined) {
        return acc;
      }
      if (picklistData.length > 0) {
        if (picklistData.includes(item.reolId)) {
          if (level === 0)
            acc.push(
              flykefn(item, showHouse, show, showReserved, level, dayDetails)
            );
          if (level <= 1)
            acc.push(
              kommunefn(item, showHouse, show, showReserved, level, dayDetails)
            );
          if (level <= 2)
            acc.push(
              teamfn(
                item,
                showHouse,
                show,
                showReserved,
                level,
                dayDetails,
                picklistFlag
              )
            );
          if (level <= 3)
            acc.push(
              routefn(
                item,
                showHouse,
                show,
                showReserved,
                level,
                dayDetails,
                picklistFlag
              )
            );
        }
      } else {
        if (level === 0)
          acc.push(
            flykefn(item, showHouse, show, showReserved, level, dayDetails)
          );
        if (level <= 1)
          acc.push(
            kommunefn(item, showHouse, show, showReserved, level, dayDetails)
          );
        if (level <= 2)
          acc.push(
            teamfn(
              item,
              showHouse,
              show,
              showReserved,
              level,
              dayDetails,
              picklistFlag
            )
          );
        if (level <= 3)
          acc.push(
            routefn(
              item,
              showHouse,
              show,
              showReserved,
              level,
              dayDetails,
              picklistFlag
            )
          );
      }

      return acc;
    }, []);

    let root = [];

    formattedData.map((el) => {
      if (el.pkey === "null") {
        let index = root.findIndex((element) => element.key === el.key);

        if (index === -1) {
          root.push(el);
        } else {
          root[index].house = root[index].house + el.house;
          root[index].householdsReserved =
            root[index].householdsReserved + el.householdsReserved;
          root[index].businesses = root[index].businesses + el.businesses;
          root[index].zone0 = root[index].zone0 + el.zone0;
          root[index].zone1 = root[index].zone1 + el.zone1;
          root[index].zone2 = root[index].zone2 + el.zone2;
          root[index].total = root[index].total + el.total;
          root[index].HHD1 = root[index].HHD1 + el.HHD1;
          root[index].HHD2 = root[index].HHD2 + el.HHD2;
          root[index].VHD1 = root[index].VHD1 + el.VHD1;
          root[index].VHD2 = root[index].VHD2 + el.VHD2;
          root[index].H0 = root[index].H0 + el.H0;
          root[index].H1 = root[index].H1 + el.H1;
          root[index].H2 = root[index].H2 + el.H2;
          root[index].V0 = root[index].V0 + el.V0;
          root[index].V1 = root[index].V1 + el.V1;
          root[index].V2 = root[index].V2 + el.V2;
        }
      } else {
        let parentIndex = formattedData.findIndex(
          (element) => element.key === el.pkey
        );
        let childIndex = formattedData[parentIndex].children.findIndex(
          (element) => element.key == el.key
        );

        if (childIndex === -1) {
          formattedData[parentIndex].children.push(el);
        } else {
          formattedData[parentIndex].children[childIndex].house =
            formattedData[parentIndex].children[childIndex].house + el.house;
          formattedData[parentIndex].children[childIndex].householdsReserved =
            formattedData[parentIndex].children[childIndex].householdsReserved +
            el.householdsReserved;
          formattedData[parentIndex].children[childIndex].businesses =
            formattedData[parentIndex].children[childIndex].businesses +
            el.businesses;
          formattedData[parentIndex].children[childIndex].zone0 =
            formattedData[parentIndex].children[childIndex].zone0 + el.zone0;
          formattedData[parentIndex].children[childIndex].zone1 =
            formattedData[parentIndex].children[childIndex].zone1 + el.zone1;
          formattedData[parentIndex].children[childIndex].zone2 =
            formattedData[parentIndex].children[childIndex].zone2 + el.zone2;
          formattedData[parentIndex].children[childIndex].total =
            formattedData[parentIndex].children[childIndex].total + el.total;
          formattedData[parentIndex].children[childIndex].HHD1 =
            formattedData[parentIndex].children[childIndex].HHD1 + el.HHD1;
          formattedData[parentIndex].children[childIndex].HHD2 =
            formattedData[parentIndex].children[childIndex].HHD2 + el.HHD2;
          formattedData[parentIndex].children[childIndex].VHD1 =
            formattedData[parentIndex].children[childIndex].VHD1 + el.VHD1;
          formattedData[parentIndex].children[childIndex].VHD2 =
            formattedData[parentIndex].children[childIndex].VHD2 + el.VHD2;
          formattedData[parentIndex].children[childIndex].H0 =
            formattedData[parentIndex].children[childIndex].H0 + el.H0;
          formattedData[parentIndex].children[childIndex].H1 =
            formattedData[parentIndex].children[childIndex].H1 + el.H1;
          formattedData[parentIndex].children[childIndex].H2 =
            formattedData[parentIndex].children[childIndex].H2 + el.H2;
          formattedData[parentIndex].children[childIndex].V0 =
            formattedData[parentIndex].children[childIndex].V0 + el.V0;
          formattedData[parentIndex].children[childIndex].V1 =
            formattedData[parentIndex].children[childIndex].V1 + el.V1;
          formattedData[parentIndex].children[childIndex].V2 =
            formattedData[parentIndex].children[childIndex].V2 + el.V2;
        }
      }
    });
    return root;
  }
};

const format = (data) => {
  return data.reduce((acc, dt) => {
    if (!(dt.children === undefined)) {
      dt.children = format(dt.children);
    }
    return acc.concat(dt);
  }, []);
};

const fetchData = async (
  url,
  key,
  level,
  showHouse,
  show,
  showReserved,
  picklistData,
  routes,
  picklistFlag,
  postAPI,
  apiRequest
) => {
  if (postAPI) {
    try {
      const { data, status } = await api.postdata(url, apiRequest);
      if (status === 200) {
        routes(data);
        return groupBy(
          data,
          key,
          level,
          showHouse,
          show,
          showReserved,
          picklistData,
          routes,
          picklistFlag
        );
      } else {
        console.error("error : " + status);
      }
    } catch (error) {
      console.error("error : " + error);
    }
  } else {
    try {
      const { data, status } = await api.getdata(url);
      if (status === 200) {
        routes(data);
        return groupBy(
          data,
          key,
          level,
          showHouse,
          show,
          showReserved,
          picklistData,
          routes,
          picklistFlag
        );
      } else {
        console.error("error : " + status);
      }
    } catch (error) {
      console.error("error : " + error);
    }
  }
};

const Postroutefn = (
  data,
  showHousehold,
  showBusiness,
  showReserved,
  level,
  dayDetails
) => {
  let zone0 = 0;
  let zone1 = 0;
  let zone2 = 0;
  let VH1 = 0;
  let VH2 = 0;
  let HH1 = 0;
  let HH2 = 0;
  let H0 = 0;
  let H1 = 0;
  let H2 = 0;
  let V0 = 0;
  let V1 = 0;
  let V2 = 0;

  if (dayDetails != "") {
    let { VHD1, VHD2, HHD1, HHD2 } = distributionDetails(
      dayDetails,
      showHousehold,
      showBusiness,
      data
    );
    VH1 = VHD1;
    VH2 = VHD2;
    HH1 = HHD1;
    HH2 = HHD2;
  }

  switch (data.prisSone) {
    case 0:
      zone0 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H0 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V0 = showBusiness ? data.antall.businesses : 0;
      break;
    case 1:
      zone1 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H1 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V1 = showBusiness ? data.antall.businesses : 0;
      break;
    case 2:
      zone2 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H2 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V2 = showBusiness ? data.antall.businesses : 0;
      break;
  }

  return {
    name: data.name,
    cat: "rute",
    prisZone: data.prisSone,
    key: parseInt(data.reolId),
    house: data.antall.households,
    householdsReserved: data.antall.householdsReserved,
    businesses: data.antall.businesses,
    HHD1: HH1,
    HHD2: HH2,
    VHD1: VH1,
    VHD2: VH2,
    zone0: zone0,
    zone1: zone1,
    zone2: zone2,
    pkey: parseInt(data.postalZone),
    H0: H0,
    H1: H1,
    H2: H2,
    V0: V0,
    V1: V1,
    V2: V2,
    total:
      (showHousehold ? data.antall.households : 0) +
      (showBusiness ? data.antall.businesses : 0) +
      (showReserved ? data.antall.householdsReserved : 0),
  };
};

const PostNrfn = (
  data,
  showHousehold,
  showBusiness,
  showReserved,
  level,
  dayDetails
) => {
  let zone0 = 0;
  let zone1 = 0;
  let zone2 = 0;
  let VH1 = 0;
  let VH2 = 0;
  let HH1 = 0;
  let HH2 = 0;
  let H0 = 0;
  let H1 = 0;
  let H2 = 0;
  let V0 = 0;
  let V1 = 0;
  let V2 = 0;

  if (dayDetails != "") {
    let { VHD1, VHD2, HHD1, HHD2 } = distributionDetails(
      dayDetails,
      showHousehold,
      showBusiness,
      data
    );
    VH1 = VHD1;
    VH2 = VHD2;
    HH1 = HHD1;
    HH2 = HHD2;
  }
  switch (data.prisSone) {
    case 0:
      zone0 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H0 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V0 = showBusiness ? data.antall.businesses : 0;
      break;
    case 1:
      zone1 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H1 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V1 = showBusiness ? data.antall.businesses : 0;
      break;
    case 2:
      zone2 =
        (showHousehold ? data.antall.households : 0) +
        (showBusiness ? data.antall.businesses : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      H2 =
        (showHousehold ? data.antall.households : 0) +
        (showReserved ? data.antall.householdsReserved : 0);
      V2 = showBusiness ? data.antall.businesses : 0;
      break;
  }
  return {
    name: data.postalZone,
    cat: "postnr",
    prisZone: data.prisSone,
    key: parseInt(data.postalZone),
    house: data.antall.households,
    householdsReserved: data.antall.householdsReserved,
    businesses: data.antall.businesses,
    HHD1: HH1,
    HHD2: HH2,
    VHD1: VH1,
    VHD2: VH2,
    zone0: zone0,
    zone1: zone1,
    zone2: zone2,
    pkey: "null",
    H0: H0,
    H1: H1,
    H2: H2,
    V0: V0,
    V1: V1,
    V2: V2,
    total:
      (showHousehold ? data.antall.households : 0) +
      (showBusiness ? data.antall.businesses : 0) +
      (showReserved ? data.antall.householdsReserved : 0),
    children: [],
  };
};

export const groupByPostNr = (
  data,
  key,
  level,
  showHouse,
  show,
  showReserved,
  picklistData,
  dayDetails = ""
) => {
  let formattedData = data.reduce((acc, item) => {
    if (item == null) {
      return acc;
    }
    acc.push(PostNrfn(item, showHouse, show, showReserved, level, dayDetails));
    acc.push(
      Postroutefn(item, showHouse, show, showReserved, level, dayDetails)
    );
    return acc;
  }, []);

  let root = [];

  formattedData.map((el) => {
    if (el.pkey === "null") {
      let index = root.findIndex((element) => element.key === el.key);

      if (index === -1) {
        root.push(el);
      } else {
        root[index].house = root[index].house + el.house;
        root[index].householdsReserved =
          root[index].householdsReserved + el.householdsReserved;
        root[index].businesses = root[index].businesses + el.businesses;
        root[index].zone0 = root[index].zone0 + el.zone0;
        root[index].zone1 = root[index].zone1 + el.zone1;
        root[index].zone2 = root[index].zone2 + el.zone2;
        root[index].total = root[index].total + el.total;
        root[index].HHD1 = root[index].HHD1 + el.HHD1;
        root[index].HHD2 = root[index].HHD2 + el.HHD2;
        root[index].VHD1 = root[index].VHD1 + el.VHD1;
        root[index].VHD2 = root[index].VHD2 + el.VHD2;
        root[index].H0 = root[index].H0 + el.H0;
        root[index].H1 = root[index].H1 + el.H1;
        root[index].H2 = root[index].H2 + el.H2;
        root[index].V0 = root[index].V0 + el.V0;
        root[index].V1 = root[index].V1 + el.V1;
        root[index].V2 = root[index].V2 + el.V2;
      }
    } else {
      let parentIndex = formattedData.findIndex(
        (element) => element.key === el.pkey
      );
      let childIndex = formattedData[parentIndex].children.findIndex(
        (element) => element.key == el.key
      );

      if (childIndex === -1) {
        formattedData[parentIndex].children.push(el);
      } else {
        formattedData[parentIndex].children[childIndex].house =
          formattedData[parentIndex].children[childIndex].house + el.house;
        formattedData[parentIndex].children[childIndex].householdsReserved =
          formattedData[parentIndex].children[childIndex].householdsReserved +
          el.householdsReserved;
        formattedData[parentIndex].children[childIndex].businesses =
          formattedData[parentIndex].children[childIndex].businesses +
          el.businesses;
        formattedData[parentIndex].children[childIndex].zone0 =
          formattedData[parentIndex].children[childIndex].zone0 + el.zone0;
        formattedData[parentIndex].children[childIndex].zone1 =
          formattedData[parentIndex].children[childIndex].zone1 + el.zone1;
        formattedData[parentIndex].children[childIndex].zone2 =
          formattedData[parentIndex].children[childIndex].zone2 + el.zone2;
        formattedData[parentIndex].children[childIndex].total =
          formattedData[parentIndex].children[childIndex].total + el.total;
        formattedData[parentIndex].children[childIndex].HHD1 =
          formattedData[parentIndex].children[childIndex].HHD1 + el.HHD1;
        formattedData[parentIndex].children[childIndex].HHD2 =
          formattedData[parentIndex].children[childIndex].HHD2 + el.HHD2;
        formattedData[parentIndex].children[childIndex].VHD1 =
          formattedData[parentIndex].children[childIndex].VHD1 + el.VHD1;
        formattedData[parentIndex].children[childIndex].VHD2 =
          formattedData[parentIndex].children[childIndex].VHD2 + el.VHD2;
        formattedData[parentIndex].children[childIndex].H0 =
          formattedData[parentIndex].children[childIndex].H0 + el.H0;
        formattedData[parentIndex].children[childIndex].H1 =
          formattedData[parentIndex].children[childIndex].H1 + el.H1;
        formattedData[parentIndex].children[childIndex].H2 =
          formattedData[parentIndex].children[childIndex].H2 + el.H2;
        formattedData[parentIndex].children[childIndex].V0 =
          formattedData[parentIndex].children[childIndex].V0 + el.V0;
        formattedData[parentIndex].children[childIndex].V1 =
          formattedData[parentIndex].children[childIndex].V1 + el.V1;
        formattedData[parentIndex].children[childIndex].V2 =
          formattedData[parentIndex].children[childIndex].V2 + el.V2;
      }
    }
  });
  return root;
};
