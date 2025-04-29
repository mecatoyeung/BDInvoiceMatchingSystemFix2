"use client"

import { useRouter } from "next/navigation"

import * as signalR from "@microsoft/signalr"

import fetchWrapper from "@/app/helpers/fetchWrapper"
import { useEffect, useState } from "react"

import { AgGridReact } from "ag-grid-react"
import ColumnsToolPanel from "@/app/components/agGrid/columnsToolPanel"

import Button from "@/app/helpers/button"
import Modal from "@/app/components/Modal"
import { ProgressBar } from "react-bootstrap"
import DateTimeRenderer from "@/app/helpers/datetimeRenderer"

export default function Rebates() {
  const router = useRouter()

  const ButtonRenderer = (props) => {
    const handleMatchClick = () => {
      router.push("/priceRebates/" + props.data.id + "/match")
    }

    const handleDownloadExcelClick = () => {
      const URL =
        process.env.NEXT_PUBLIC_API_URL +
        "PriceRebates/" +
        props.data.id +
        "/DownloadExcel"
      if (typeof window !== "undefined") {
        window.location.href = URL
      }
    }

    const handleDeleteClick = () => {
      setDeleteModalForm({
        ...deleteModalForm,
        isOpen: true,
        id: props.data.id,
      })
    }
    return (
      <>
        <Button onClick={handleMatchClick}>Match</Button>
        <Button onClick={handleDownloadExcelClick}>Download</Button>
        <Button onClick={handleDeleteClick} variant="danger">
          Delete
        </Button>
      </>
    )
  }

  const [priceRebates, setPriceRebates] = useState([])
  const [rowData, setRowData] = useState([])
  const [uploadModalForm, setUploadModalForm] = useState({
    isOpen: false,
    selectedFile: null,
    uploadConnectionId: null,
    uploadProgress: 0,
    uploadIntervalId: null,
  })
  const [deleteModalForm, setDeleteModalForm] = useState({
    isOpen: false,
    id: 0,
  })

  const onUploadModalUpload = async () => {
    const formData = new FormData()
    formData.append("file", uploadModalForm.selectedFile)
    formData.append("connectionId", uploadModalForm.connectionId)

    const response = await fetchWrapper
      .post(`PriceRebates/UploadExcel`, formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then(async (resp) => {
        let respJson = await resp.json()

        const uploadIntervalId = setInterval(
          () => checkProgressJob(respJson.id),
          5000
        )
        setUploadModalForm((prev) => ({
          ...prev,
          uploadIntervalId: uploadIntervalId,
        }))
      })
  }

  const checkProgressJob = async (id) => {
    const resp = await fetchWrapper
      .get(`PriceRebates/` + id)
      .then(async (resp2) => {
        let resp2Json = await resp2.json()
        let progress =
          (resp2Json.currentUploadRow / resp2Json.totalUploadRow) * 100
        setUploadModalForm((prev) => ({
          ...prev,
          uploadProgress: progress,
        }))
      })
  }

  useEffect(() => {
    if (uploadModalForm.uploadProgress === 0) return
    console.log(
      uploadModalForm.uploadIntervalId,
      uploadModalForm.uploadProgress
    )
    if (uploadModalForm.uploadProgress === 100) {
      clearInterval(uploadModalForm.uploadIntervalId)
      setUploadModalForm((prev) => ({
        ...prev,
        isOpen: false,
        uploadProgress: 0,
        uploadIntervalId: null,
      }))
      refreshRowData()
    }
  }, [uploadModalForm.uploadProgress])

  const handleUploadBtnClick = () => {
    setUploadModalForm({
      ...uploadModalForm,
      connectionId: null,
      uploadProgress: 0,
      isOpen: true,
    })
  }

  const onUploadModalClose = async () => {
    setUploadModalForm({
      ...uploadModalForm,
      isOpen: false,
    })
  }

  const handleFileUpload = (e) => {
    setUploadModalForm({
      ...uploadModalForm,
      selectedFile: e.target.files[0],
    })
  }

  const confirmDeleteBtnClickHandler = async (e) => {
    const response = await fetchWrapper.delete(
      `PriceRebates/${deleteModalForm.id}`
    )

    if (response.ok) {
      await refreshRowData()
      setDeleteModalForm({
        ...deleteModalForm,
        isOpen: false,
      })
    }
  }

  const deleteModalCloseBtnClickHandler = async (e) => {
    setDeleteModalForm({
      ...deleteModalForm,
      isOpen: false,
    })
  }

  const [columnDefs] = useState([
    { headerName: "ID", field: "id", width: 50 },
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
  ])

  const frameworkComponents = { buttonRenderer: ButtonRenderer }

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
  }

  const refreshRowData = async () => {
    const response = await fetchWrapper.get("PriceRebates")
    const data = await response.json()
    console.log(data)
    setRowData(data)
  }

  useEffect(() => {
    ;(async () => {
      refreshRowData()
    })()
  }, [])

  return (
    <div>
      <h2 className="m-2 font-bold">Price Rebates</h2>
      <div className="p-2">
        <Button
          onClick={() => {
            handleUploadBtnClick()
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
                Delete
              </Button>
              <Button onClick={deleteModalCloseBtnClickHandler}>Close</Button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
