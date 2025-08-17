"use client";

import { useRouter } from "next/navigation";

import fetchWrapper from "@/app/helpers/fetchWrapper";
import { useEffect, useState } from "react";

import { AgGridReact } from "ag-grid-react";

import Button from "@/app/helpers/button";
import Modal from "@/app/components/Modal";
import DateTimeRenderer from "@/app/helpers/datetimeRenderer";

import withAuth from "../components/withAuth";

function FileSources() {
  const router = useRouter();

  const ButtonRenderer = (props) => {
    const handleModifyClick = () => {
      router.push("/fileSources/" + props.data.id);
    };
    const handleDeleteClick = () => {
      setDeleteModalForm({
        ...deleteModalForm,
        isOpen: true,
        id: props.data.id,
      });
    };
    return (
      <>
        <Button onClick={handleModifyClick}>Modify</Button>
        <Button onClick={handleDeleteClick} variant="danger">
          Delete
        </Button>
      </>
    );
  };

  const [rowData, setRowData] = useState([]);
  const [deleteModalForm, setDeleteModalForm] = useState({
    isOpen: false,
    id: 0,
  });

  const confirmDeleteBtnClickHandler = async (e) => {
    const response = await fetchWrapper.delete(
      `FileSources/${deleteModalForm.id}`
    );

    if (response.ok) {
      await refreshRowData();
      setDeleteModalForm({
        ...deleteModalForm,
        isOpen: false,
      });
    }
  };

  const deleteModalCloseBtnClickHandler = async (e) => {
    setDeleteModalForm({
      ...deleteModalForm,
      isOpen: false,
    });
  };

  const [columnDefs] = useState([
    { headerName: "ID", field: "id", width: 50 },
    { headerName: "Folder Path", field: "folderPath", width: 250 },
    {
      headerName: "Actions",
      field: "actions",
      cellRenderer: ButtonRenderer,
      width: 250,
    },
  ]);

  const frameworkComponents = { buttonRenderer: ButtonRenderer };

  const refreshRowData = async () => {
    const response = await fetchWrapper.get("FileSources");
    const data = await response.json();
    setRowData(data);
  };

  useEffect(() => {
    (async () => {
      refreshRowData();
    })();
  }, []);

  return (
    <div>
      <h2 className="m-2 font-bold">File Sources</h2>
      <div className="p-2">
        <Button onClick={() => {}}>New</Button>
      </div>
      <div
        className="ag-theme-alpine p-2"
        style={{ height: 600, width: "100%" }}
      >
        <AgGridReact
          columnDefs={columnDefs}
          rowData={rowData}
          frameworkComponents={frameworkComponents}
        ></AgGridReact>
      </div>
      {deleteModalForm.isOpen && (
        <div className="modal modal-open">
          <div className="modal-box">
            <p>
              This operation is not reversible. Besdies, the matching to this
              item will also be deleted. Are you sure you want to delete?
            </p>
            <div className="modal-action">
              <Button onClick={confirmDeleteBtnClickHandler} variant="danger">
                Delete
              </Button>
              <Button onClick={deleteModalCloseBtnClickHandler}>Close</Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default withAuth(FileSources);
