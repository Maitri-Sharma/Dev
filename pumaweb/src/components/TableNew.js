import {Table} from "antd";
import React from "react";
import styled from "styled-components";
import Spinner from "../components/spinner/spinner.component";

const Styles = styled.div`
  padding-left: 5px;
  th.ant-table-cell.ant-table-column-sort.ant-table-column-has-sorters:after {
    content: "";
  }
`;

export default function TreeData({
                                     width1,
                                     columnsArray,
                                     data,
                                     setoutputDataList,
                                     defaultSelectedColumn = [],
                                     setSelectedRows,
                                     hideselection = 0,
                                     parentCallback,
                                 }) {

    const [checkStrictly, setCheckStrictly] = React.useState(false);

    const [hideSelectAll, setHide] = React.useState(true);

    const [selectedPKeys, setSelectedPkeys] = React.useState([]);

    const [loading, setloading] = React.useState([true]);

    const rowSelection = {
        selectedRowKeys: defaultSelectedColumn,
        onChange: (selectedRowKeys, selectedRows) => {
            setSelectedRows(selectedRowKeys);
            let selectedparentkeys = [];
            defaultSelectedColumn.length = 0;

            if (selectedRows.length > 0) {
                selectedRows.forEach(async (row) => {
                    if (selectedparentkeys?.indexOf(row.pkey.toString()) === -1) {
                        if (row.pkey !== "null") {
                            selectedparentkeys?.push(
                                parseInt(
                                    row.pkey
                                        .toString()
                                        .match(/\d+/)[0]
                                        .substring(0, row.pkey.toString().length === 3 ? 1 : 2),
                                    10
                                ).toString()
                            );
                            selectedparentkeys.push(row.pkey.toString());
                        }
                    }
                    if (defaultSelectedColumn.indexOf(row.key) === -1) {
                        defaultSelectedColumn.push(row.key);
                    }
                });
            }
            setSelectedPkeys(selectedparentkeys);
            setoutputDataList(selectedRows);
        },
      hideSelectAll: true,
    };

    const findChildNodeSelected = (currentNode, Keytocheck) => {
        let index, currentChild, result;

        if (Keytocheck === currentNode.key) {
            if (
                currentNode.pkey != null &&
                currentNode.pkey !== "undefined" &&
                selectedPKeys != null &&
                selectedPKeys !== "undefined" &&
                selectedPKeys.indexOf(currentNode.pkey.toString()) === -1
            ) {
                selectedPKeys.push(currentNode.pkey.toString());

                let thenumber = parseInt(currentNode.pkey.match(/\d+/)[0], 10);
                if (
                    thenumber != null &&
                    thenumber !== "undefined" && selectedPKeys !== "undefined" &&
                    selectedPKeys.indexOf(thenumber.toString()) === -1
                ) {
                    selectedPKeys.push(thenumber.toString());
                    selectedPKeys.push(
                        parseInt(
                            currentNode.pkey.match(/\d+/)[0].substring(0, 2),
                            10
                        ).toString()
                    );
                }
            }
            return currentNode;
        } else {
            for (
                index = 0;
                currentNode.children !== undefined &&
                index < currentNode.children.length;
                index += 1
            ) {
                currentChild = currentNode.children[index];
                result = findChildNodeSelected(currentChild, Keytocheck);
                if (result !== false) {
                    return result;
                }
            }
            return false;
        }
    };

    return <Styles className="w-100">
        <div style={{display: loading ? "none" : "block", zindex: -1}}>
            <Table
                pagination={false}
                className="TreeView_MultiColumn"
                showSorterTooltip={false}
                columns={columnsArray}
                hideSelectAll={hideSelectAll}
                rowSelection={
                    hideselection === 1 ? null : {...rowSelection, checkStrictly}
                }
                dataSource={data}
                onRow={(record) => {
                    if (
                        Object.values(selectedPKeys).includes(record.key) &&
                        record.key !== record.pkey &&
                        !Object.values(selectedPKeys).includes(record.pkey)
                    ) {
                        setSelectedPkeys(selectedPKeys.concat(record.pkey));
                    }
                    setloading(false);
                }}
            />
        </div>
        <div>{loading ? <Spinner/> : null}</div>
    </Styles>
}
