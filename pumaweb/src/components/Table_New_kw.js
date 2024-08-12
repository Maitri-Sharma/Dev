import { Table } from "antd";
import React, { useEffect, useContext } from "react";
import styled from "styled-components";

import { KundeWebContext } from "../context/Context.js";

let StyleswithHeading = styled.div`
  padding-left: 5px;
`;

let StylesNoHeading = styled.div`
  padding-left: 5px;
  th.ant-table-cell.ant-table-column-sort.ant-table-column-has-sorters:after {
    content: "";
  }
`;

let Styles = StyleswithHeading;

export default function TreeData({
  columnsArray,
  data,
  setoutputDataList,
  defaultSelectedColumn = [],
  hideselection = 0,
  page,
  parentCallback,
  comp,
}) {
  const [checkStrictly, setCheckStrictly] = React.useState(false);
  const [hideSelectAll, setHide] = React.useState(
    page === "segmenter" || page === "Demogra" ? false : true
  );
  const [selectedPKeys, setSelectedPkeys] = React.useState([]);
  const { selectedRowKeys, setSelectedRowKeys } =
    React.useContext(KundeWebContext);
  const [loading, setloading] = React.useState([true]);
  const { selectedrecord_s, setselectedrecord_s } = useContext(KundeWebContext);
  const [selectedvalue, setselectedvalue] = React.useState(
    selectedrecord_s.length > 0 ? selectedrecord_s : []
  );
  const { pagekeys, setpagekeys } = React.useContext(KundeWebContext);
  const { pagekeysseg, setpagekeysseg } = React.useContext(KundeWebContext);
  const { pagekeysgeo, setpagekeysgeo } = React.useContext(KundeWebContext);
  const { defaultSelectedColumn_s, setdefaultSelectedColumn_s } =
    React.useContext(KundeWebContext);
  const { ResultOutputData, setResultOutputData } =
    React.useContext(KundeWebContext);
  const { criteriaObject, setCriteriaObject } = useContext(KundeWebContext);

  let rowSelection = {};
  let ResultOutData_Array = [];

  useEffect(() => {
    setselectedrecord_s([]);
    setdefaultSelectedColumn_s([]);
    defaultSelectedColumn = [];
    setSelectedPkeys([]);
    setSelectedRowKeys([]);
  }, []);

  if (defaultSelectedColumn.length > 0) {
    setdefaultSelectedColumn_s(defaultSelectedColumn);
  }

  if (defaultSelectedColumn.length == 0) {
    let arr = [...selectedRowKeys, ...defaultSelectedColumn];
    arr = [...arr, ...selectedrecord_s];
    rowSelection = {
      selectedRowKeys: arr,
      onChange: (selectedRowKeys, selectedRows, selected) => {
        let selectedpkeysTemp = [];
        if (selectedRows.length > 0) {
          selectedRows.forEach(async (record) => {
            if (
              record.pkey != undefined &&
              selectedpkeysTemp.indexOf(record.pkey.toString()) === -1
            ) {
              selectedpkeysTemp.push(
                parseInt(
                  record.pkey.match(/\d+/)[0].substring(0, 2),
                  10
                ).toString()
              );
              selectedpkeysTemp.push(record.pkey.toString());
            } else if (
              record.pkey == undefined &&
              selectedpkeysTemp.indexOf(record.key.toString()) === -1
            ) {
              if (selectedpkeysTemp.indexOf(record.key.toString()) === -1) {
                selectedpkeysTemp.push(
                  parseInt(
                    record.key.match(/\d+/)[0].substring(0, 2),
                    10
                  ).toString()
                );
              }
              selectedpkeysTemp.push(record.key.toString());
            }
          });
        }
        setSelectedPkeys(selectedpkeysTemp);

        let arr = [...selectedRowKeys, ...defaultSelectedColumn];

        if (defaultSelectedColumn.length > 0) {
          setSelectedRowKeys(arr);
        } else {
          setSelectedRowKeys(selectedRowKeys);
        }

        setoutputDataList(selectedRows);
      },
      onSelect: (record, selected, selectedRows) => {
        if (criteriaObject?.enum === "19") {
          if (selectedRows?.length && selectedRows[0]?.children === undefined) {
            console.log("Route");
          } else if (
            selectedRows?.length &&
            selectedRows[0]?.children.length &&
            selectedRows[0]?.children[0]?.children === undefined
          ) {
            console.log("Team");
          } else if (
            selectedRows?.length &&
            selectedRows[0]?.children.length &&
            selectedRows[0]?.children[0]?.children?.length &&
            selectedRows[0]?.children[0]?.children[0]?.children === undefined
          ) {
            console.log("Kommune");
          } else if (
            selectedRows?.length &&
            selectedRows[0]?.children.length &&
            selectedRows[0]?.children[0]?.children?.length &&
            selectedRows[0]?.children[0]?.children[0]?.children?.length
          ) {
            console.log("flyk");
          }
        }

        let str = "";
        selectedRows.map((item, index) => {
          if (item?.ID !== undefined) {
            selectedRows?.length === index + 1
              ? (str = str + item?.ID)
              : (str = str + item?.ID + ",");
          }
        });

        setCriteriaObject({
          ...criteriaObject,
          KommuneIds: str,
        });
        let selectedpkeysTemp = [];

        if (selectedRows.length > 0) {
          selectedRows.map((T1) => {
            if (T1.children) {
              T1.children.map((item) => {
                if (item.children) {
                  item.children.map((result) => {
                    selectedpkeysTemp.push(result.key);
                  });
                } else {
                  selectedpkeysTemp.push(item.key);
                }
              });
            }
            selectedpkeysTemp.push(T1.key);
          });
        }
        ResultOutData_Array = ResultOutputData;
        if (selected) {
          ResultOutData_Array.push(record);
        }
        if (selected == false) {
          ResultOutData_Array.splice(ResultOutData_Array.indexOf(record), 1);
        }
        parentCallback(
          ResultOutData_Array,
          selectedpkeysTemp,
          selectedRows,
          record
        );
      },

      onSelectAll: (selected, record, selectedRows, changeRows) => {
        let selectedpkeysTemp = [];

        if (selectedRows.length > 0) {
          selectedRows.map((T1) => {
            if (T1.children) {
              T1.children.map((item) => {
                if (item.children) {
                  item.children.map((result) => {
                    selectedpkeysTemp.push(result.key);
                  });
                } else {
                  selectedpkeysTemp.push(item.key);
                }
              });
            }
            selectedpkeysTemp.push(T1.key);
          });
        }
        ResultOutData_Array = ResultOutputData;
        if (selected) {
          ResultOutData_Array.push(record);
        }
        if (selected == false) {
          ResultOutData_Array.splice(ResultOutData_Array.indexOf(record), 1);
        }
        parentCallback(ResultOutData_Array, selectedpkeysTemp, selectedRows);
      },
      hideSelectAll: hideSelectAll,
    };
  } else {
    let arr = [...selectedRowKeys, ...selectedrecord_s];

    rowSelection = {
      selectedRowKeys: arr,
      onChange: (selectedRowKeys, selectedRows, selected) => {
        let selectedpkeysTemp = [];
        if (selectedRows.length > 0) {
          selectedRows.forEach(async (record) => {
            if (
              record.pkey != undefined &&
              selectedpkeysTemp.indexOf(record.pkey.toString()) === -1
            ) {
              selectedpkeysTemp.push(
                parseInt(
                  record.pkey.match(/\d+/)[0].substring(0, 2),
                  10
                ).toString()
              );
              selectedpkeysTemp.push(record.pkey.toString());
            } else if (
              record.pkey == undefined &&
              selectedpkeysTemp.indexOf(record.key.toString()) === -1
            ) {
              if (selectedpkeysTemp.indexOf(record.key.toString()) === -1) {
                selectedpkeysTemp.push(
                  parseInt(
                    record.key.match(/\d+/)[0].substring(0, 2),
                    10
                  ).toString()
                );
              }
              selectedpkeysTemp.push(record.key.toString());
            }
          });
        }
        setSelectedPkeys(selectedpkeysTemp);

        if (defaultSelectedColumn.length > 0) {
          setSelectedRowKeys(selectedRowKeys);
        } else {
          setSelectedRowKeys(arr);
        }

        if (page == "segmenter") {
          selectedRowKeys.push(page);
          setpagekeysseg(selectedRowKeys);
        }
        if (page == "Demogra") {
          selectedRowKeys.push(page);
          setpagekeys(selectedRowKeys);
        }
        if (page == "Geogra") {
          setpagekeysgeo(selectedRowKeys);
        }
        setoutputDataList(selectedRows);
      },
      onSelect: (record, selected, selectedRows) => {
        let selectedpkeysTemp = [];
        if (selectedRows.length > 0) {
          selectedRows.map((T1) => {
            if (T1.children) {
              T1.children.map((item) => {
                if (item.children) {
                  item.children.map((result) => {
                    selectedpkeysTemp.push(result.key);
                  });
                } else {
                  selectedpkeysTemp.push(item.key);
                }
              });
            }
            selectedpkeysTemp.push(T1.key);
          });
        }
        ResultOutData_Array = ResultOutputData;
        if (selected) {
          ResultOutData_Array.push(record);
        }
        if (selected == false) {
          ResultOutData_Array.splice(ResultOutData_Array.indexOf(record), 1);
        }

        parentCallback(
          ResultOutData_Array,
          selectedpkeysTemp,
          selectedRows,
          record
        );
      },

      onSelectAll: (selected, record, selectedRows, changeRows) => {
        let selectedpkeysTemp = [];

        if (selectedRows.length > 0) {
          selectedRows.map((T1) => {
            if (T1.children) {
              T1.children.map((item) => {
                if (item.children) {
                  item.children.map((result) => {
                    selectedpkeysTemp.push(result.key);
                  });
                } else {
                  selectedpkeysTemp.push(item.key);
                }
              });
            }
            selectedpkeysTemp.push(T1.key);
          });
        }
        ResultOutData_Array = ResultOutputData;
        if (selected) {
          ResultOutData_Array.push(record);
        }
        if (selected == false) {
          ResultOutData_Array.splice(ResultOutData_Array.indexOf(record), 1);
        }
        parentCallback(ResultOutData_Array, selectedpkeysTemp, selectedRows);
      },
      hideSelectAll: hideSelectAll,
    };
  }
  const rowClassName = (record, index) => {
    let rowclassname;
    if (
      defaultSelectedColumn != "undefined" &&
      defaultSelectedColumn != null &&
      defaultSelectedColumn.length > 0
    ) {
      if (
        Object.values(defaultSelectedColumn).includes(record.key.toString())
      ) {
        if (
          record.pkey != null &&
          record.pkey != "undefined" &&
          selectedPKeys != null &&
          selectedPKeys != "undefined" &&
          selectedPKeys.indexOf(record.pkey.toString()) === -1
        ) {
          selectedPKeys.push(record.pkey.toString());
        }
      }
    }

    rowclassname = Object.values(selectedPKeys).includes(record.key.toString())
      ? "expand-parent"
      : "";

    return rowclassname;
  };

  if (page == "Geogra") {
    Styles = StylesNoHeading;
  } else if (page == "segmenter" || page == "Demogra") {
    Styles = StyleswithHeading;
  }
  return (
    <>
      <Styles>
        <div style={{ display: loading ? "none" : "block", zindex: -1 }}>
          <Table
            className="TreeView_MultiColumn"
            showSorterTooltip={false}
            columns={columnsArray}
            pagination={false}
            hideSelectAll={hideSelectAll}
            rowSelection={
              hideselection === 1 ? null : { ...rowSelection, checkStrictly }
            }
            dataSource={data}
            //rowClassName={rowClassName}
            onRow={(record, rowIndex) => {
              setloading(false);
              if (
                Object.values(selectedPKeys).includes(record.key) &&
                record.key != record.pkey &&
                !Object.values(selectedPKeys).includes(record.pkey)
              ) {
                setSelectedPkeys(selectedPKeys.concat(record.pkey));
              }
            }}
          />
        </div>
      </Styles>
    </>
  );
}
