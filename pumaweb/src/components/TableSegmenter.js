import { Table } from "antd";
import React, { useEffect, useContext } from "react";
import styled from "styled-components";
import loadingImage from "../assets/images/callbackActivityIndicator.gif";

import { KSPUContext, KundeWebContext } from "../context/Context.js";
const Styles = styled.div``;

export default function TreeData({
  columnsArray,
  data,
  setoutputDataList,
  defaultSelectedColumn = [],
  hideselection = 0,
  page,
  parentCallback,
}) {
  const [checkStrictly, setCheckStrictly] = React.useState(false);
  const [hideSelectAll, setHide] = React.useState(
    page == "segmenter" ? false : true
  );
  const [selectedPKeys, setSelectedPkeys] = React.useState([]);
  const { selectedRowKeys, setSelectedRowKeys } = React.useContext(KSPUContext);
  const [loading, setloading] = React.useState([true]);
  const { selectedrecord_s, setselectedrecord_s } =
    React.useContext(KSPUContext);
  const [selectedvalue, setselectedvalue] = React.useState(
    selectedrecord_s.length > 0 ? selectedrecord_s : []
  );
  const { pagekeys, setpagekeys } = React.useContext(KSPUContext);
  const { pagekeysseg, setpagekeysseg } = React.useContext(KSPUContext);
  const { pagekeysgeo, setpagekeysgeo } = React.useContext(KSPUContext);
  const { defaultSelectedColumn_s, setdefaultSelectedColumn_s } =
    React.useContext(KSPUContext);

  let ResultOutData_Array = [];
  let ResultOutputData = [];

  let rowSelection = {};

  // useEffect(()=>{
  //   setSelectedRowKeys(defaultSelectedColumn);

  // useEffect(()=>{
  // setselectedvalue(selectedvalue.push(selectedrecord_s))

  // },[selectedvalue])

  useEffect(() => {
    if (page == "segmenter") {
      pagekeysseg.splice(-1, 1);
      setSelectedRowKeys(pagekeysseg);
    } else if (page == "Demogra") {
      pagekeys.splice(-1, 1);
      setSelectedRowKeys(pagekeys);
    } else if (page == "Geogra") {
      pagekeysgeo.splice(-1, 1);
      setSelectedRowKeys(pagekeysgeo);
    }

    if (defaultSelectedColumn_s.length > 0) {
      defaultSelectedColumn = [
        ...defaultSelectedColumn,
        ...defaultSelectedColumn_s,
      ];
      // setSelectedRowKeys(selectedRowKeys.push(defaultSelectedColumn_s))
    }
  }, []);
  // },[])

  if (defaultSelectedColumn.length > 0) {
    setdefaultSelectedColumn_s(defaultSelectedColumn);
  }

  if (defaultSelectedColumn.length == 0) {
    let arr = [...selectedRowKeys, ...defaultSelectedColumn];

    rowSelection = {
      selectedRowKeys: arr,
      onChange: (selectedRowKeys, selectedRows) => {
        let arr = [...selectedRowKeys, ...defaultSelectedColumn];
        if (page == "segmenter") {
          //parentCallback(selectedRows);
        }
        if (defaultSelectedColumn.length > 0) {
          setSelectedRowKeys(arr);
        } else {
          setSelectedRowKeys(selectedRowKeys);
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
          selectedRowKeys.push(page);
          setpagekeysgeo(selectedRowKeys);
        }

        setoutputDataList(selectedRows);
        let selectedpkeysTemp = [];
        if (selectedRows.length > 0) {
          selectedRows.forEach(async (record) => {
            if (
              record.pkey != undefined &&
              selectedpkeysTemp.indexOf(record.pkey) === -1
            ) {
              selectedpkeysTemp.push(record.pkey);
            } else if (
              record.pkey == undefined &&
              selectedpkeysTemp.indexOf(record.key) === -1
            ) {
              if (selectedpkeysTemp.indexOf(record.key) === -1) {
                selectedpkeysTemp.push(
                  parseInt(
                    record.key.match(/\d+/)[0].substring(0, 2),
                    10
                  ).toString()
                );
              }
              selectedpkeysTemp.push(record.key);
            }
          });
        }
        setSelectedPkeys(selectedpkeysTemp);
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
      onSelectAll: (record, selected, selectedRows) => {
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
      hideSelectAll: hideSelectAll,
    };
  } else {
    let arr = [...selectedRowKeys, ...defaultSelectedColumn_s];

    rowSelection = {
      selectedRowKeys: arr,
      onChange: (selectedRowKeys, selectedRows) => {
        if (page == "segmenter") {
          //parentCallback(selectedRows);
        }
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
          selectedRowKeys.push(page);
          setpagekeysgeo(selectedRowKeys);
        }
        setoutputDataList(selectedRows);
        let selectedpkeysTemp = [];
        if (selectedRows.length > 0) {
          selectedRows.forEach(async (record) => {
            if (
              record.pkey != undefined &&
              selectedpkeysTemp.indexOf(record.pkey) === -1
            ) {
              selectedpkeysTemp.push(record.pkey);
            } else if (
              record.pkey == undefined &&
              selectedpkeysTemp.indexOf(record.key) === -1
            ) {
              if (selectedpkeysTemp.indexOf(record.key) === -1) {
                selectedpkeysTemp.push(
                  parseInt(
                    record.key.match(/\d+/)[0].substring(0, 2),
                    10
                  ).toString()
                );
              }
              selectedpkeysTemp.push(record.key);
            }
          });
        }
        setSelectedPkeys(selectedpkeysTemp);
      },
      onSelect: (record, selected, selectedRows) => {
        let selectedpkeysTemp = [];
        if (selectedRows.length > 0) {
          // selectedRows.map((record) => {
          //   if (record.key != record.pkey) {
          //     setSelectedPkeys(selectedPKeys.concat(record.pkey));
          //     selectedpkeysTemp.push(record.pkey);
          //   }
          // });
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
          // ResultOutData_Array = ResultOutData_Array.filter(function (obj) {
          //   return obj.key !== record.key;
          // });
          ResultOutData_Array.splice(ResultOutData_Array.indexOf(record), 1);
        }
        // let Temp = RemoveDuplicates(ResultOutData_Array, "ID");

        // setResultOutputData(ResultOutData_Array);

        parentCallback(
          ResultOutData_Array,
          selectedpkeysTemp,
          selectedRows,
          record
        );
      },

      onSelectAll: (selected, selectedRows, changeRows) => {},
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
      if (Object.values(defaultSelectedColumn).includes(record.key)) {
        if (
          record.pkey != null &&
          record.pkey != "undefined" &&
          selectedPKeys != null &&
          selectedPKeys != "undefined" &&
          selectedPKeys.indexOf(record.pkey) === -1
        ) {
          selectedPKeys.push(record.pkey);
        }
      }
    }

    rowclassname = Object.values(selectedPKeys).includes(record.key)
      ? "expand-parent"
      : "";

    return rowclassname;
  };

  if (page == "Geogra") {
    // rowSelection["hideSelectAlll"] = true;
  }
  return (
    <>
      <Styles className="w-100 gridsize">
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
            rowClassName={rowClassName}
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
        <img
          src={loadingImage}
          style={{
            width: "20px",
            height: "20px",
            display: !loading ? "none" : "block",
            position: "absolute",
            top: "170px",
            left: "250px",
            zindex: 100,
          }}
        />
      </Styles>
    </>
  );
}
