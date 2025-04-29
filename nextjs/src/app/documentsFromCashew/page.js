"use client"

import { useMemo, useRef } from "react"
import { useParams, useRouter } from "next/navigation"

import fetchWrapper from "@helpers/fetchWrapper"
import { useEffect, useState } from "react"

import { AgGridReact } from "ag-grid-react"

import Modal from "@/app/components/Modal"
import CustomToolPanel from "@/app/components/agGrid/columnsToolPanel"

import Button from "@helpers/button"
import DateRenderer from "@helpers/dateRenderer"
import DateTimeRenderer from "@helpers/datetimeRenderer"
import DecimalRenderer from "@helpers/decimalRenderer"
import moment from "moment"

export default function DocumentsFromCashew() {
  const router = useRouter()

  const [gridApi, setGridApi] = useState(null)
  const [paginationPageSize, setPaginationPageSize] = useState(1000)
  const [currentPage, setCurrentPage] = useState(1)
  const [sortModel, setSortModel] = useState([
    {
      colId: "ID",
      sort: "desc",
    },
  ])
  const [filterModel, setFilterModel] = useState([])

  const [
    modifyDocumentFromCashewItemModal,
    setModifyDocumentFromCashewItemModal,
  ] = useState({
    isOpen: false,
    id: 0,
    documentFromCashewID: 0,
    customerCode: "",
    customerName: "",
    customerAddress: "",
    deliveryDate: "",
    documentDate: "",
    documentNo: "",
    stockCode: "",
    description: "",
    lotNo: "",
    unitOfMeasure: "",
    quantity: 0,
    pdfFilename: "",
    csvFilename: "",
  })

  const DocumentFromCashewItemsButtonRenderer = (props) => {
    const handleDownloadPdfClick = () => {
      const URL =
        process.env.NEXT_PUBLIC_API_URL +
        "DocumentsFromCashew/" +
        props.data.documentFromCashewID +
        "/DownloadPdf"
      if (typeof window !== "undefined") {
        window.location.href = URL
      }
    }

    const handleDownloadCsvClick = () => {
      const URL =
        process.env.NEXT_PUBLIC_API_URL +
        "DocumentsFromCashew/" +
        props.data.documentFromCashewID +
        "/DownloadCsv"
      if (typeof window !== "undefined") {
        window.location.href = URL
      }
    }

    const handleModifyClick = () => {
      console.log(props.node.data)
      setModifyDocumentFromCashewItemModal({
        ...modifyDocumentFromCashewItemModal,
        isOpen: true,
        ...props.node.data,
      })
    }

    const handleDeleteClick = () => {
      setDeleteDocumentFromCashewItemModal({
        ...deleteDocumentFromCashewItemModal,
        isOpen: true,
        id: props.node.data.id,
      })
    }

    return (
      <>
        <Button onClick={handleDownloadPdfClick}>Download PDF</Button>
        <Button onClick={handleDownloadCsvClick}>Download CSV</Button>
        <Button onClick={handleModifyClick}>Modify</Button>
        <Button onClick={handleDeleteClick} variant="danger">
          Delete
        </Button>
      </>
    )
  }

  const [
    deleteDocumentFromCashewItemModal,
    setDeleteDocumentFromCashewItemModal,
  ] = useState({
    isOpen: false,
    id: 0,
  })

  const [
    documentFromCashewItemsColumnsModal,
    setDocumentFromCashewItemsColumnsModal,
  ] = useState({
    isOpen: false,
  })

  const documentFromCashewItemModalSaveBtnClickHandler = async () => {
    await fetchWrapper.put(
      `DocumentFromCashewItems/${modifyDocumentFromCashewItemModal.id}`,
      {
        ...modifyDocumentFromCashewItemModal,
      }
    )

    await getDocumentFromCashewItemsPagination(filterModel)

    setModifyDocumentFromCashewItemModal({
      ...modifyDocumentFromCashewItemModal,
      isOpen: false,
    })
  }

  const documentFromCashewItemModalCloseBtnClickHandler = () => {
    setModifyDocumentFromCashewItemModal({
      ...modifyDocumentFromCashewItemModal,
      isOpen: false,
    })
  }

  const deleteDocumentFromCashewItemModalConfirmDeleteBtnClickHandler =
    async () => {
      await fetchWrapper.delete(
        `DocumentFromCashewItems/${deleteDocumentFromCashewItemModal.id}`
      )

      setDeleteDocumentFromCashewItemModal({
        ...deleteDocumentFromCashewItemModal,
        isOpen: false,
      })

      await getDocumentFromCashewItemsPagination(filterModel)
    }

  const deleteDocumentFromCashewItemModalCloseBtnClickHandler = () => {
    setDeleteDocumentFromCashewItemModal({
      ...deleteDocumentFromCashewItemModal,
      isOpen: false,
    })
  }

  const documentFromCashewItemsGridRef = useRef(null)

  const documentFromCashewItemsRowSelection = useMemo(() => {
    return {
      mode: "multiRow",
    }
  }, [])

  const onGridReady = (params) => {
    setGridApi(params.api)
  }

  const onRowSelectedDocumentFromCashewItems = () => {
    const documentFromCashewItemsSelectedRows =
      documentFromCashewItemsGridRef.current.api.getSelectedRows()
    let documentNos = documentFromCashewItemsSelectedRows.map(
      (r) => r.documentNo
    )
    documentNos = documentNos.filter(
      (value, index, array) => array.indexOf(value) === index
    )
    setSelectedDocumentFromCashewItemsDocumentNos(documentNos)

    let ids = documentFromCashewItemsSelectedRows.map((r) => r.id)
    setSelectedDocumentFromCashewItemsIDs(ids)
  }

  const getDocumentFromCashewItemsPagination = async (mFilterModel) => {
    let queryStringInArray = []
    for (let i = 0; i < mFilterModel.length; i++) {
      queryStringInArray.push("fieldName=" + mFilterModel[i].fieldName)
      queryStringInArray.push("fieldType=" + mFilterModel[i].fieldType)
      queryStringInArray.push("filterType=" + mFilterModel[i].filterType)
      queryStringInArray.push("filterValue=" + mFilterModel[i].filterValue)
    }
    //if (queryStringInArray.length == 0) return
    let queryString = queryStringInArray.join("&")
    const documentFromCashewItemsResponse = await fetchWrapper.get(
      `DocumentFromCashewItems/Pagination?${queryString}`
    )
    let documentFromCashewItemsData =
      await documentFromCashewItemsResponse.json()

    for (let i = 0; i < documentFromCashewItemsData.pageData.length; i++) {
      try {
        documentFromCashewItemsData.pageData[
          i
        ].documentFromCashew.documentDate = moment(
          documentFromCashewItemsData.pageData[i].documentFromCashew
            .documentDate
        ).format("YYYY-MM-DD")
      } catch {
        documentFromCashewItemsData.pageData[
          i
        ].documentFromCashew.documentDate = null
      }

      try {
        documentFromCashewItemsData.pageData[
          i
        ].documentFromCashew.deliveryDate = moment(
          documentFromCashewItemsData.pageData[i].documentFromCashew
            .deliveryDate
        ).format("YYYY-MM-DD")
      } catch {
        documentFromCashewItemsData.pageData[
          i
        ].documentFromCashew.deliveryDate = null
      }
    }

    setDocumentFromCashewItemsRowData(documentFromCashewItemsData.pageData)
    return documentFromCashewItemsData
  }

  const [
    documentFromCashewItemsColumnDefs,
    setDocumentFromCashewItemsColumnDefs,
  ] = useState([
    {
      headerName: "",
      field: "select",
      checkboxSelection: true,
      headerCheckboxSelection: false,
      width: 50,
    },
    { headerName: "ID", field: "id", width: 50 },
    {
      headerName: "PDF Filename",
      field: "documentFromCashew.pdfFilename",
      filter: "agTextColumnFilter",
      hide: false,
      width: 200,
    },
    {
      headerName: "CSV Filename",
      field: "documentFromCashew.csvFilename",
      filter: "agTextColumnFilter",
      hide: false,
      width: 200,
    },
    {
      headerName: "Customer Code",
      field: "documentFromCashew.customerCode",
      filter: "agTextColumnFilter",
      hide: false,
      width: 150,
    },
    {
      headerName: "Customer Name",
      field: "documentFromCashew.customerName",
      filter: "agTextColumnFilter",
      hide: false,
      width: 150,
    },
    {
      headerName: "Customer Address",
      field: "documentFromCashew.customerAddress",
      filter: "agTextColumnFilter",
      hide: false,
      width: 200,
    },
    {
      headerName: "Document Date",
      field: "documentFromCashew.documentDate",
      cellRenderer: DateTimeRenderer,
      hide: false,
      width: 150,
    },
    {
      headerName: "Delivery Date",
      field: "documentFromCashew.deliveryDate",
      cellRenderer: DateTimeRenderer,
      hide: false,
      width: 150,
    },
    {
      headerName: "Document No",
      field: "documentFromCashew.documentNo",
      filter: "agTextColumnFilter",
      hide: false,
      width: 150,
    },
    {
      headerName: "Stock Code",
      field: "stockCode",
      filter: "agTextColumnFilter",
      hide: false,
      width: 150,
    },
    {
      headerName: "Description",
      field: "description",
      filter: "agTextColumnFilter",
      hide: false,
      width: 350,
    },
    {
      headerName: "Lot No",
      field: "lotNo",
      filter: "agTextColumnFilter",
      hide: false,
      width: 150,
    },
    {
      headerName: "Quantity",
      field: "quantity",
      hide: false,
      width: 150,
    },
    {
      headerName: "Unit Of Measure",
      field: "unitOfMeasure",
      hide: false,
      width: 350,
    },
    {
      headerName: "Unit Price",
      field: "unitPrice",
      cellRenderer: DecimalRenderer,
      hide: false,
      width: 150,
    },
    {
      headerName: "Subtotal",
      field: "subtotal",
      cellRenderer: DecimalRenderer,
      hide: false,
      width: 150,
    },
    {
      headerName: "Uploaded Date Time",
      field: "uploadedDateTime",
      cellRenderer: DateTimeRenderer,
      hide: false,
      width: 200,
    },
    {
      headerName: "Matched?",
      field: "matched",
      hide: false,
      width: 150,
    },
    {
      headerName: "Actions",
      field: "actions",
      cellRenderer: DocumentFromCashewItemsButtonRenderer,
      hide: false,
      width: 550,
    },
  ])

  const documentFromCashewItemsFrameworkComponents = {
    buttonRenderer: DocumentFromCashewItemsButtonRenderer,
  }

  const getDocumentFromCashewItemsRowStyle = (params) => {
    if (matchings.includes(params.data.matchingID)) {
      return {
        backgroundColor: "lightgreen",
      }
    }
  }

  const onPaginationChanged = async (params) => {}

  const onSortChanged = async (params) => {}

  const onFilterChanged = async (params) => {
    let mFilterModel = params.api.getFilterModel()
    let mFilterModelInArray = []
    for (const [key, value] of Object.entries(mFilterModel)) {
      mFilterModelInArray.push({
        fieldName: key,
        fieldType: value.filterType,
        filterType: value.type,
        filterValue: value.filter,
      })
    }
    setFilterModel(mFilterModelInArray)
    getDocumentFromCashewItemsPagination(mFilterModelInArray)
  }

  const saveDocumentFromCashewItemsColumnDefs = (
    documentFromCashewItemsColumnDefs
  ) => {
    localStorage.setItem(
      "documentFromCashewItemsColumnDefs",
      JSON.stringify(documentFromCashewItemsColumnDefs)
    )
  }

  const loadDefaultColumnDefs = async () => {
    const mDocumentFromCashewItemsColumnDefs = localStorage.getItem(
      "documentFromCashewItemsColumnDefs"
    )
    if (mDocumentFromCashewItemsColumnDefs != null) {
      let columnDefs = JSON.parse(mDocumentFromCashewItemsColumnDefs)
      let order = columnDefs.map((d) => d.headerName)
      let updatedDocumentFromCashewItemsColumnDefs = [
        ...documentFromCashewItemsColumnDefs,
      ]
      for (
        let i = 0;
        i < updatedDocumentFromCashewItemsColumnDefs.length;
        i++
      ) {
        let updatedDocumentFromCashewItemsColumnDef =
          updatedDocumentFromCashewItemsColumnDefs[i]
        let columnDef = columnDefs.find(
          (d) => d.field == updatedDocumentFromCashewItemsColumnDef.field
        )
        try {
          updatedDocumentFromCashewItemsColumnDefs.hide = columnDef.hide
        } catch {
          updatedDocumentFromCashewItemsColumnDefs.hide = false
        }
      }
      updatedDocumentFromCashewItemsColumnDefs.sort(
        (a, b) => order.indexOf(a.headerName) - order.indexOf(b.headerName)
      )
      setDocumentFromCashewItemsColumnDefs(
        updatedDocumentFromCashewItemsColumnDefs
      )
    }
  }

  const [documentFromCashewForm, setDocumentFromCashewForm] = useState({
    showAllUnmatched: false,
  })
  const [documentFromCashewItemsRowData, setDocumentFromCashewItemsRowData] =
    useState([])
  const [selectedMatchings, setSelectedMatchings] = useState([])

  const [
    selectedDocumentFromCashewItemsDocumentNos,
    setSelectedDocumentFromCashewItemsDocumentNos,
  ] = useState([])
  const [
    selectedDocumentFromCashewItemsIDs,
    setSelectedDocumentFromCashewItemsIDs,
  ] = useState([])
  const [matchings, setMatchings] = useState([])
  const [modalForm, setModalForm] = useState({
    isOpen: false,
    content: <></>,
  })

  useEffect(() => {
    ;(async () => {
      await getDocumentFromCashewItemsPagination(filterModel)
      console.log("useEffect1")
    })()
  }, [router.isReady])

  useEffect(() => {
    ;(async () => {
      await loadDefaultColumnDefs()
      console.log("useEffect2")
    })()
  }, [])

  return (
    <div className="flex flex-col">
      <h1 className="m-2 font-bold">Documents from Cashew</h1>
      <div className="p-2">
        <Button onClick={() => router.push("/priceRebates")}>Back</Button>
      </div>
      <main className="flex flex-col">
        <div className="flex-grow flex flex-row bg-gray-100 p-2">
          <div
            className="ag-theme-alpine p-2"
            style={{ height: 600, width: "100%" }}
          >
            {console.log(documentFromCashewItemsRowData)}
            <AgGridReact
              ref={documentFromCashewItemsGridRef}
              onGridReady={onGridReady}
              columnDefs={documentFromCashewItemsColumnDefs}
              rowData={documentFromCashewItemsRowData}
              getRowStyle={getDocumentFromCashewItemsRowStyle}
              frameworkComponents={documentFromCashewItemsFrameworkComponents}
              pagination={true}
              rowSelection="multiple"
              onRowSelected={onRowSelectedDocumentFromCashewItems}
              paginationPageSize={paginationPageSize}
              paginationPageSizeSelector={[100, 200, 500, 1000]}
              onPaginationChanged={onPaginationChanged}
              onSortChanged={onSortChanged}
              onFilterChanged={onFilterChanged}
              onRowDataUpdated={() => {
                const gridApi = documentFromCashewItemsGridRef.current.api
                gridApi.forEachNode((node) => {
                  if (
                    selectedDocumentFromCashewItemsIDs.includes(node.data.id)
                  ) {
                    node.setSelected(true)
                  }
                })
              }}
            ></AgGridReact>
          </div>
        </div>
      </main>
      <Modal
        isOpen={modalForm.isOpen}
        onClose={() =>
          setModalForm({
            ...modalForm,
            isOpen: false,
            content: <></>,
          })
        }
        content={modalForm.content}
      />

      {modifyDocumentFromCashewItemModal.isOpen && (
        <div className="modal modal-open">
          <div className="modal-box">
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                PDF Filename (Read only)
              </label>
              <input
                type="text"
                id="pdfFilename"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={
                  modifyDocumentFromCashewItemModal.documentFromCashew
                    .pdfFilename
                }
                readOnly
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                CSV Filename (Read only)
              </label>
              <input
                type="text"
                id="csvFilename"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={
                  modifyDocumentFromCashewItemModal.documentFromCashew
                    .csvFilename
                }
                readOnly
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Customer Code
              </label>
              <input
                type="text"
                id="customerCode"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={
                  modifyDocumentFromCashewItemModal.documentFromCashew
                    .customerCode
                }
                onChange={(e) => {
                  setModifyDocumentFromCashewItemModal({
                    ...modifyDocumentFromCashewItemModal,
                    documentFromCashew: {
                      ...modifyDocumentFromCashewItemModal.documentFromCashew,
                      customerCode: e.target.value,
                    },
                  })
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Customer Name (Read only)
              </label>
              <input
                type="text"
                id="customerName"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={
                  modifyDocumentFromCashewItemModal.documentFromCashew
                    .customerName
                }
                readOnly
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Customer Address (Read only)
              </label>
              <input
                type="text"
                id="customerAddress"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={
                  modifyDocumentFromCashewItemModal.documentFromCashew
                    .customerAddress
                }
                readOnly
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Document Date (Read only)
              </label>
              <input
                type="date"
                id="documentDate"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={
                  modifyDocumentFromCashewItemModal.documentFromCashew
                    .documentDate
                }
                onChange={(e) => {
                  setModifyDocumentFromCashewItemModal({
                    ...modifyDocumentFromCashewItemModal,
                    documentFromCashew: {
                      ...modifyDocumentFromCashewItemModal.documentFromCashew,
                      documentDate: e.target.value,
                    },
                  })
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Delivery Date
              </label>
              <input
                type="date"
                id="deliveryDate"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={
                  modifyDocumentFromCashewItemModal.documentFromCashew
                    .deliveryDate
                }
                onChange={(e) => {
                  setModifyDocumentFromCashewItemModal({
                    ...modifyDocumentFromCashewItemModal,
                    documentFromCashew: {
                      ...modifyDocumentFromCashewItemModal.documentFromCashew,
                      deliveryDate: e.target.value,
                    },
                  })
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Document No
              </label>
              <input
                type="text"
                id="documentNo"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={
                  modifyDocumentFromCashewItemModal.documentFromCashew
                    .documentNo
                }
                onChange={(e) => {
                  setModifyDocumentFromCashewItemModal({
                    ...modifyDocumentFromCashewItemModal,
                    documentFromCashew: {
                      ...modifyDocumentFromCashewItemModal.documentFromCashew,
                      documentNo: e.target.value,
                    },
                  })
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Stock Code
              </label>
              <input
                type="text"
                id="stockCode"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={modifyDocumentFromCashewItemModal.stockCode}
                onChange={(e) => {
                  setModifyDocumentFromCashewItemModal({
                    ...modifyDocumentFromCashewItemModal,
                    stockCode: e.target.value,
                  })
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Description
              </label>
              <input
                type="text"
                id="description"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={modifyDocumentFromCashewItemModal.description}
                onChange={(e) => {
                  setModifyDocumentFromCashewItemModal({
                    ...modifyDocumentFromCashewItemModal,
                    description: e.target.value,
                  })
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Lot No
              </label>
              <input
                type="text"
                id="stockCode"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={modifyDocumentFromCashewItemModal.lotNo}
                onChange={(e) => {
                  setModifyDocumentFromCashewItemModal({
                    ...modifyDocumentFromCashewItemModal,
                    lotNo: e.target.value,
                  })
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Unit of Measure
              </label>
              <input
                type="text"
                id="unitOfMeasure"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={modifyDocumentFromCashewItemModal.unitOfMeasure}
                onChange={(e) => {
                  setModifyDocumentFromCashewItemModal({
                    ...modifyDocumentFromCashewItemModal,
                    unitOfMeasure: e.target.value,
                  })
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Quantity
              </label>
              <input
                type="number"
                id="stockCode"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={modifyDocumentFromCashewItemModal.quantity}
                onChange={(e) => {
                  setModifyDocumentFromCashewItemModal({
                    ...modifyDocumentFromCashewItemModal,
                    quantity: e.target.value,
                  })
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Uploaded Date Time (Read only)
              </label>
              <input
                type="text"
                id="uploadedDateTime"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={
                  modifyDocumentFromCashewItemModal.documentFromCashew
                    .uploadedDateTime
                }
                required
                readOnly
              />
            </div>
            <div className="modal-action">
              <Button onClick={documentFromCashewItemModalSaveBtnClickHandler}>
                Save
              </Button>
              <Button onClick={documentFromCashewItemModalCloseBtnClickHandler}>
                Close
              </Button>
            </div>
          </div>
        </div>
      )}

      {deleteDocumentFromCashewItemModal.isOpen && (
        <div className="modal modal-open">
          <div className="modal-box">
            <p>
              This operation is not reversible. Besdies, the matching to this
              item will also be deleted. Are you sure you want to delete?
            </p>
            <div className="modal-action">
              <Button
                onClick={
                  deleteDocumentFromCashewItemModalConfirmDeleteBtnClickHandler
                }
                variant="danger"
              >
                Delete
              </Button>
              <Button
                onClick={deleteDocumentFromCashewItemModalCloseBtnClickHandler}
              >
                Close
              </Button>
            </div>
          </div>
        </div>
      )}

      {documentFromCashewItemsColumnsModal.isOpen && (
        <div className="modal modal-open">
          <div className="modal-box">
            <div>
              <ul>
                {documentFromCashewItemsColumnDefs &&
                  documentFromCashewItemsColumnDefs.map((item, itemIndex) => (
                    <li key={item.id} className="m-2">
                      <div className="flex flex-row">
                        <div className="columnHandle">
                          <svg
                            xmlns="http://www.w3.org/2000/svg"
                            className="h-5 w-5"
                            fill="none"
                            viewBox="0 0 24 24"
                            stroke="currentColor"
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              strokeWidth="2"
                              d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"
                            />
                          </svg>
                        </div>
                        <div className="columnName flex-grow">
                          {item.headerName}
                        </div>
                        <div className="columnActions">
                          {item.hide && (
                            <Button
                              onClick={() => {
                                let updatedDocumentFromCashewItemsColumnDefs = [
                                  ...documentFromCashewItemsColumnDefs,
                                ]
                                updatedDocumentFromCashewItemsColumnDefs[
                                  itemIndex
                                ].hide = false
                                setDocumentFromCashewItemsColumnDefs(
                                  updatedDocumentFromCashewItemsColumnDefs
                                )
                              }}
                            >
                              Show
                            </Button>
                          )}
                          {!item.hide && (
                            <Button
                              onClick={() => {
                                let updatedDocumentFromCashewItemsColumnDefs = [
                                  ...documentFromCashewItemsColumnDefs,
                                ]
                                updatedDocumentFromCashewItemsColumnDefs[
                                  itemIndex
                                ].hide = true
                                setDocumentFromCashewItemsColumnDefs(
                                  updatedDocumentFromCashewItemsColumnDefs
                                )
                              }}
                            >
                              Hide
                            </Button>
                          )}
                          <Button
                            onClick={() => {
                              if (itemIndex == 0) return
                              let updatedDocumentFromCashewItemsColumnDefs = [
                                ...documentFromCashewItemsColumnDefs,
                              ]
                              let tmpColumnDef = {
                                ...updatedDocumentFromCashewItemsColumnDefs[
                                  itemIndex - 1
                                ],
                              }
                              updatedDocumentFromCashewItemsColumnDefs[
                                itemIndex - 1
                              ] =
                                updatedDocumentFromCashewItemsColumnDefs[
                                  itemIndex
                                ]
                              updatedDocumentFromCashewItemsColumnDefs[
                                itemIndex
                              ] = tmpColumnDef
                              setDocumentFromCashewItemsColumnDefs(
                                updatedDocumentFromCashewItemsColumnDefs
                              )
                            }}
                          >
                            &uarr;
                          </Button>
                          <Button
                            onClick={() => {
                              if (
                                itemIndex >=
                                documentFromCashewItemsColumnDefs.length - 1
                              )
                                return
                              let updatedDocumentFromCashewItemsColumnDefs = [
                                ...documentFromCashewItemsColumnDefs,
                              ]
                              let tmpColumnDef = {
                                ...updatedDocumentFromCashewItemsColumnDefs[
                                  itemIndex
                                ],
                              }
                              updatedDocumentFromCashewItemsColumnDefs[
                                itemIndex
                              ] =
                                updatedDocumentFromCashewItemsColumnDefs[
                                  itemIndex + 1
                                ]
                              updatedDocumentFromCashewItemsColumnDefs[
                                itemIndex + 1
                              ] = tmpColumnDef
                              setDocumentFromCashewItemsColumnDefs(
                                updatedDocumentFromCashewItemsColumnDefs
                              )
                            }}
                          >
                            &darr;
                          </Button>
                        </div>
                      </div>
                    </li>
                  ))}
              </ul>
            </div>
            <div className="modal-action">
              <Button
                onClick={() =>
                  saveDocumentFromCashewItemsColumnDefs(
                    documentFromCashewItemsColumnDefs
                  )
                }
              >
                Save as Default
              </Button>
              <Button
                onClick={() =>
                  setDocumentFromCashewItemsColumnsModal({
                    ...documentFromCashewItemsColumnsModal,
                    isOpen: false,
                  })
                }
              >
                Close
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
