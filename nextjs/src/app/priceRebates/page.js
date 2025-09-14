"use client";

import { useRouter } from "next/navigation";

import * as signalR from "@microsoft/signalr";

import fetchWrapper from "@/app/helpers/fetchWrapper";
import { useEffect, useState } from "react";

import { AgGridReact } from "ag-grid-react";
import ColumnsToolPanel from "@/app/components/agGrid/columnsToolPanel";

import Button from "@/app/helpers/button";
import Modal from "@/app/components/Modal";
import { ProgressBar } from "react-bootstrap";
import DateTimeRenderer from "@/app/helpers/datetimeRenderer";

import withAuth from "../components/withAuth";

function Rebates() {
  const router = useRouter();

  const ButtonRenderer = (props) => {
    const handleMatchClick = () => {
      router.push("/priceRebates/" + props.data.id + "/match");
    };

    const handleDownloadExcelClick = () => {
      const URL =
        process.env.NEXT_PUBLIC_API_URL +
        "PriceRebates/" +
        props.data.id +
        "/DownloadExcel";
      if (typeof window !== "undefined") {
        window.location.href = URL;
      }
    };

    const handleDeleteClick = () => {
      setDeleteModalForm({
        ...deleteModalForm,
        isOpen: true,
        id: props.data.id,
        isDeleting: false
      });
    };
    return (
      <>
        <Button onClick={handleMatchClick}>Match</Button>
        <Button onClick={handleDownloadExcelClick}>Download</Button>
        <Button onClick={handleDeleteClick} variant="danger">
          Delete
        </Button>
      </>
    );
  };

  const [gettingPriceRebates, setGettingPriceRebates] = useState(false);
  const [rowData, setRowData] = useState([]);
  const [uploadModalForm, setUploadModalForm] = useState({
    isOpen: false,
    selectedFile: null,
    uploadConnectionId: null,
    uploadProgress: 0,
    uploadIntervalId: null,
  });
  const [deleteModalForm, setDeleteModalForm] = useState({
    isOpen: false,
    isDeleting: false,
    id: 0,
  });

  const onUploadModalUpload = async () => {
    const formData = new FormData();
    formData.append("file", uploadModalForm.selectedFile);
    formData.append("connectionId", uploadModalForm.connectionId);

    const response = await fetchWrapper
      .post(`PriceRebates/UploadExcel`, formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then(async (resp) => {
        let respJson = await resp.json();

        setUploadModalForm({
          ...uploadModalForm,
          isOpen: false
        })

        /*const uploadIntervalId = setInterval(
          () => checkProgressJob(respJson.id),
          5000
        );
        setUploadModalForm((prev) => ({
          ...prev,
          uploadIntervalId: uploadIntervalId,
        }));*/
      });
  };

  /*const checkProgressJob = async (id) => {
    const resp = await fetchWrapper
      .get(`PriceRebates/` + id)
      .then(async (resp2) => {
        let resp2Json = await resp2.json();
        let progress =
          (resp2Json.currentUploadRow / resp2Json.totalUploadRow) * 100;
        setUploadModalForm((prev) => ({
          ...prev,
          uploadProgress: progress,
        }));
      });
  };*/

  /*useEffect(() => {
    if (uploadModalForm.uploadProgress === 0) return;
    console.log(
      uploadModalForm.uploadIntervalId,
      uploadModalForm.uploadProgress
    );
    if (uploadModalForm.uploadProgress === 100) {
      clearInterval(uploadModalForm.uploadIntervalId);
      setUploadModalForm((prev) => ({
        ...prev,
        isOpen: false,
        uploadProgress: 0,
        uploadIntervalId: null,
      }));
      refreshRowData();
    }
  }, [uploadModalForm.uploadProgress]);*/

  const handleUploadBtnClick = () => {
    setUploadModalForm({
      ...uploadModalForm,
      connectionId: null,
      uploadProgress: 0,
      isOpen: true,
    });
  };

  const onUploadModalClose = async () => {
    setUploadModalForm({
      ...uploadModalForm,
      isOpen: false,
    });
  };

  const handleFileUpload = (e) => {
    setUploadModalForm({
      ...uploadModalForm,
      selectedFile: e.target.files[0],
    });
  };

  const confirmDeleteBtnClickHandler = async (e) => {
    setDeleteModalForm({
      ...deleteModalForm,
      isDeleting: true,
    });
    const response = await fetchWrapper.delete(
      `PriceRebates/${deleteModalForm.id}`
    );

    await refreshRowData();
    setDeleteModalForm({
      ...deleteModalForm,
      isOpen: false,
      isDeleting: false
    });
  };

  const handleRefreshBtnClick = async () => {
    await refreshRowData()
  }

  const deleteModalCloseBtnClickHandler = async (e) => {
    setDeleteModalForm({
      ...deleteModalForm,
      isOpen: false,
    });
  };

  const [columnDefs] = useState([
    { headerName: "ID", field: "id", width: 50 },
    { headerName: "Status", field: "status", width: 100 },
    { headerName: "Excel Filename", field: "excelFilename", width: 250 },
    { headerName: "Filename", field: "filename", width: 250 },
    { headerName: "Current Upload Row", field: "currentUploadRow", width: 250 },
    { headerName: "Total Upload Row", field: "totalUploadRow", width: 250 },
    { headerName: "Upload Error", field: "uploadError", width: 250 },
    { headerName: "All Matched", field: "allItemsAreMatched", width: 150 },
    {
      headerName: "Uploaded Date Time",
      field: "uploadedDateTime",
      cellRenderer: DateTimeRenderer,
      width: 250,
    },
    {
      headerName: "Actions",
      field: "actions",
      cellRenderer: ButtonRenderer,
      width: 350,
    },
  ]);

  const frameworkComponents = { buttonRenderer: ButtonRenderer };

  const sideBar = {
    toolPanels: [
      {
        id: "columnsToolPanel",
        labelDefault: "Columns Tool Panel",
        labelKey: "columnsToolPanel",
        iconKey: "columns-tool-panel",
        toolPanel: ColumnsToolPanel,
        toolPanelParams: {},
      },
    ],
  };

  const refreshRowData = async () => {
    setGettingPriceRebates(true);
    const response = await fetchWrapper.get("PriceRebates");
    const data = await response.json();
    console.log(data);
    setRowData(data);
    setGettingPriceRebates(false)
  };

  useEffect(() => {
    (async () => {
      refreshRowData();
    })();
  }, []);

  return (
    <div>
      <h2 className="m-2 font-bold">Price Rebates</h2>
      <div className="p-2">
        <Button
          onClick={() => {
            handleRefreshBtnClick();
          }}
        >
          {gettingPriceRebates && (
            <svg style={{
              width: 24,
              height: 24
            }} aria-hidden="true" className="inline w-8 h-8 scale-50 text-gray-200 animate-spin dark:text-gray-600 fill-gray-600 dark:fill-gray-300" viewBox="0 0 100 101" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z" fill="currentColor"/>
              <path d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z" fill="currentFill"/>
          </svg>
          )}
          {!gettingPriceRebates && (
            <>Refresh</>
          )}
        </Button>
        <Button
          onClick={() => {
            handleUploadBtnClick();
          }}
        >
          Upload
        </Button>
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
      {uploadModalForm.isOpen && (
        <div className="modal modal-open">
          <div className="modal-box">
            <input
              type="file"
              onChange={handleFileUpload}
              className="file-input w-full max-w-xs"
            />
            <ProgressBar
              className="mt-2"
              now={uploadModalForm.uploadProgress}
              label={`${uploadModalForm.uploadProgress.toFixed(0)}%`}
              style={{ height: "1.5rem" }}
            />
            <div className="modal-action">
              <button onClick={onUploadModalUpload} className="btn btn-primary">
                Upload
              </button>
              <button onClick={onUploadModalClose} className="btn">
                Close
              </button>
            </div>
          </div>
        </div>
      )}
      {deleteModalForm.isOpen && (
        <div className="modal modal-open">
          <div className="modal-box">
            <p>
              This operation is not reversible. Besdies, the matching to this
              item will also be deleted. Are you sure you want to delete?
            </p>
            <div className="modal-action">
              <Button onClick={confirmDeleteBtnClickHandler} variant="danger">
                {deleteModalForm.isDeleting && (
                  <>
                    Deleting...
                  </>
                )}
                {!deleteModalForm.isDeleting && (
                  <>
                    Delete
                  </>
                )}
              </Button>
              <Button onClick={deleteModalCloseBtnClickHandler}>Close</Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default withAuth(Rebates);
