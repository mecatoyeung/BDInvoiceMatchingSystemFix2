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

import withAuth from "@/app/components/withAuth"

function RebateMatch() {
  const router = useRouter()
  const params = useParams()

  const { id } = params

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

  const [priceRebateCounts, setPriceRebateCount] = useState({
    matchedCount: null,
    unmatchedCount: null,
  })

  const [gettingPriceRebates, setGettingPriceRebates] = useState(false)

  const [
    deleteDocumentFromCashewItemModal,
    setDeleteDocumentFromCashewItemModal,
  ] = useState({
    isOpen: false,
    id: 0,
  })

  const [priceRebateItemsColumnsModal, setPriceRebateItemsColumnsModal] =
    useState({
      isOpen: false,
    })

  const [
    documentFromCashewItemsColumnsModal,
    setDocumentFromCashewItemsColumnsModal,
  ] = useState({
    isOpen: false,
  })

  const [autoMatchProgress, setAutoMatchProgress] = useState({
    status: "READY",
    priceRebateItemsCount: 0,
    matchedPriceRebateItemsCount: 0,
    autoMatchProgress: 0.0,
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

    const handleModifyClick = () => {
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
        <Button onClick={handleModifyClick}>Modify</Button>
        <Button onClick={handleDeleteClick} variant="danger">
          Delete
        </Button>
      </>
    )
  }

  const documentFromCashewItemModalSaveBtnClickHandler = async () => {
    await fetchWrapper.put(
      `DocumentFromCashewItems/${modifyDocumentFromCashewItemModal.id}`,
      {
        ...modifyDocumentFromCashewItemModal,
      }
    )

    await onRowSelectedPriceRebateItems()

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

      await onRowSelectedPriceRebateItems()

      setDeleteDocumentFromCashewItemModal({
        ...deleteDocumentFromCashewItemModal,
        isOpen: false,
      })
    }

  const deleteDocumentFromCashewItemModalCloseBtnClickHandler = () => {
    setDeleteDocumentFromCashewItemModal({
      ...deleteDocumentFromCashewItemModal,
      isOpen: false,
    })
  }

  const priceRebateItemsGridRef = useRef(null)

  const priceRebateItemsRowSelection = useMemo(() => {
    return {
      mode: "multiRow",
      isRowSelectable: (rowNode) => {
        return (
          selectedPriceRebateItemsDocumentNos.length == 0 ||
          selectedPriceRebateItemsDocumentNos.includes(rowNode.data.documentNo)
        )
      },
    }
  }, [])

  const priceRebateItemsIsRowSelctable = (rowNode) => {
    return true
    return (
      selectedPriceRebateItemsDocumentNos.length == 0 ||
      selectedPriceRebateItemsDocumentNos.includes(rowNode.data.documentNo)
    )
  }

  const onPriceRebateSelectionChanged = (e) => {
    const selectedNodes = e.api.getSelectedNodes()

    selectedNodes.forEach((node) => {
      if (
        selectedPriceRebateItemsDocumentNos.length == 0 ||
        selectedPriceRebateItemsDocumentNos.includes(node.data.documentNo)
      ) {
      } else {
        node.setSelected(false)
      }
    })
  }

  const onRowSelectedPriceRebateItems = async () => {
    const priceRebateItemsSelectedRows =
      priceRebateItemsGridRef.current.api.getSelectedRows()
    let documentNos = priceRebateItemsSelectedRows.map((r) => r.documentNo)
    documentNos = documentNos.filter(
      (value, index, array) => array.indexOf(value) === index
    )
    setSelectedPriceRebateItemsDocumentNos(documentNos)

    let ids = priceRebateItemsSelectedRows.map((r) => r.id)
    setSelectedPriceRebateItemsIDs(ids)

    await getDocumentFromCashewItemsByDocumentNos(documentNos)

    let matchingIDs = priceRebateItemsSelectedRows.map((r) =>
      r.matchingID == null ? 0 : r.matchingID
    )
    setMatchings(matchingIDs)
  }

  const documentFromCashewItemsGridRef = useRef(null)

  const documentFromCashewItemsRowSelection = useMemo(() => {
    return {
      mode: "multiRow",
    }
  }, [])

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
    console.log(documentFromCashewItemsSelectedRows)
  }

  const [priceRebateItemsGridOptions, setPriceRebateItemsGridOptions] =
    useState({
      animateRows: false,
    })

  const [priceRebateItemsColumnDefs, setPriceRebateItemsColumnDefs] = useState([
    {
      headerName: "",
      field: "select",
      checkboxSelection: true,
      headerCheckboxSelection: false,
      width: 50,
    },
    { headerName: "ID", field: "id", width: 80 },
    {
      headerName: "Document No",
      field: "documentNo",
      filter: "agTextColumnFilter",
      width: 150,
    },
    {
      headerName: "Stock Code",
      field: "stockCode",
      filter: "agTextColumnFilter",
      width: 150,
    },
    {
      headerName: "SKU",
      field: "sku",
      filter: "agTextColumnFilter",
      width: 150,
    },
    {
      headerName: "Description",
      field: "description",
      filter: "agTextColumnFilter",
      width: 350,
    },
    {
      headerName: "Quantity",
      field: "quantity",
      filter: "agNumberColumnFilter",
      width: 150,
    },
    {
      headerName: "Unit Price",
      field: "unitPrice",
      filter: "agNumberColumnFilter",
      width: 150,
    },
    {
      headerName: "Subtotal (USD)",
      field: "subtotalInUSD",
      filter: "agNumberColumnFilter",
      width: 150,
    },
    {
      headerName: "Subtotal (HKD)",
      field: "subtotalInHKD",
      filter: "agNumberColumnFilter",
      width: 150,
    },
    {
      headerName: "Matched?",
      field: "matched",
      width: 150,
      filter: "agSetColumnFilter",
    },
    {
      headerName: "Matching ID",
      field: "matchingID",
      width: 150,
      filter: "agSetColumnFilter",
    },
    {
      headerName: "No Data but Matched?",
      field: "noDataButMatched",
      width: 150,
      filter: "agSetColumnFilter",
    },
  ])

  const initialItems = [
    { id: 0, text: "One", color: "#616AFF" },
    { id: 1, text: "Two", color: "#2DBAE7" },
    { id: 2, text: "Three", color: "#fd4e4e" },
    { id: 3, text: "Four", color: "#FFBF00" },
    { id: 4, text: "Five", color: "#e66139" },
    { id: 5, text: "Six", color: "#3577ef" },
    { id: 6, text: "Seven", color: "#ababab" },
    { id: 7, text: "Eight", color: "#21C8B7" },
    { id: 8, text: "Nine", color: "#FED67D" },
    { id: 9, text: "Ten", color: "#013540" },
  ]

  const getDocumentFromCashewItemsByDocumentNos = async (documentNos) => {
    const url = new URL(
      "https://api.example.com/DocumentFromCashewItems/ByDocumentNos"
    )
    const urlParams = new URLSearchParams()
    if (documentNos.length == 0) documentNos = []
    documentNos = [documentNos[0]]
    documentNos.forEach((documentNo) => {
      urlParams.append("documentNos", documentNo)
    })
    url.search = urlParams.toString()
    const completeUrl = url.toString().replace("https://api.example.com/", "")

    const documentFromCashewItemsResponse = await fetchWrapper.get(completeUrl)
    const documentFromCashewItemsData =
      await documentFromCashewItemsResponse.json()
    setDocumentFromCashewItemsRowData(documentFromCashewItemsData)
  }

  const matchBtnClickHandler = async () => {
    if (selectedDocumentFromCashewItemsIDs.length == 0) {
      setModalForm({
        ...modalForm,
        isOpen: true,
        content: (
          <>
            <h1>Warning</h1>
            <p>No data found on the invoice</p>
          </>
        ),
        footer: (
          <>
            <Button
              variant="warning"
              onClick={async () => {
                await fetchWrapper
                  .post("Matching/Match", {
                    priceRebateItems: selectedPriceRebateItemsIDs,
                    documentFromCashewItems: selectedDocumentFromCashewItemsIDs,
                    noDataButMatch: true,
                  })
                  .then(async (response) => {
                    let data = await response.json()
                    if (response.status == 200) {
                      await getPriceRebates()
                      setModalForm({
                        ...modalForm,
                        isOpen: true,
                        content: (
                          <>
                            <h1>Success</h1>
                            <p>{data.message}</p>
                          </>
                        ),
                      })
                    } else {
                      setModalForm({
                        ...modalForm,
                        isOpen: true,
                        content: (
                          <>
                            <h1>Error</h1>
                            <p>{data.message}</p>
                          </>
                        ),
                      })
                    }
                  })
              }}
            >
              Continue
            </Button>
            <Button
              onClick={() =>
                setModalForm({
                  isOpen: false,
                })
              }
            >
              Cancel
            </Button>
          </>
        ),
      })
      return
    }
    try {
      let response = await fetchWrapper.post("Matching/Match", {
        priceRebateItems: selectedPriceRebateItemsIDs,
        documentFromCashewItems: selectedDocumentFromCashewItemsIDs,
      })
      let data = await response.json()
      setModalForm({
        ...modalForm,
        isOpen: true,
        content: (
          <>
            <h1>Success</h1>
            <p>{data.message}</p>
          </>
        ),
      })
    } catch (e) {
      setModalForm({
        ...modalForm,
        isOpen: true,
        content: (
          <>
            <h1>Error</h1>
            <p>{e.message}</p>
          </>
        ),
      })
    }
    const priceRebateItemsSelectedRows =
      priceRebateItemsGridRef.current.api.getSelectedRows()
    let documentNos = priceRebateItemsSelectedRows.map((r) => r.documentNo)
    documentNos = documentNos.filter(
      (value, index, array) => array.indexOf(value) === index
    )
    setSelectedPriceRebateItemsDocumentNos(documentNos)

    await getPriceRebates()
  }

  const unmatchBtnClickHandler = async () => {
    try {
      var response = await fetchWrapper.post("Matching/Unmatch", {
        priceRebateItems: selectedPriceRebateItemsIDs,
      })
      setModalForm({
        ...modalForm,
        isOpen: true,
        content: (
          <>
            <h1>Success</h1>
            <p>{response.message}</p>
          </>
        ),
      })
    } catch (e) {
      console.error(e.message)
      setModalForm({
        ...modalForm,
        isOpen: true,
        content: (
          <>
            <h1>Error</h1>
            <p>{e.message}</p>
          </>
        ),
      })
    }
    await getPriceRebates()
  }

  let automatchProgressIntervalRef = useRef(null)

  const autoMatchBtnClickHandler = async () => {
    try {
      let response = fetchWrapper.post(`Matching/Automatch`, {
        priceRebateId: id,
      })
      let progressResponse = await fetchWrapper.post(
        `Matching/AutomatchProgress`,
        {
          priceRebateId: id,
        }
      )
      let progressResponseData = await progressResponse.json()

      setAutoMatchProgress({
        status: "PROCESSING",
        priceRebateItemsCount: 0,
        matchedPriceRebateItemsCount: 0,
        autoMatchProgress: 0.0,
      })

      automatchProgressIntervalRef = setInterval(async () => {
        var runningProgressResponse = await fetchWrapper.post(
          `Matching/AutomatchProgress`,
          {
            priceRebateId: id,
          }
        )
        let runningProgressResponseData = await runningProgressResponse.json()
        if (
          runningProgressResponseData.matchedPriceRebateItemsCount ==
          progressResponseData.matchedPriceRebateItemsCount
        ) {
          clearInterval(automatchProgressIntervalRef.current)
          console.log(runningProgressResponseData)
        } else {
          console.log(runningProgressResponseData)
        }
      }, 5000)
      //await getPriceRebates();
    } catch (e) {
      console.error(e.message)
    }
  }

  const priceRebateItemsColumnsBtnClickHandler = () => {
    setPriceRebateItemsColumnsModal({
      ...priceRebateItemsColumnsModal,
      isOpen: true,
    })
  }

  const documentFromCashewItemsColumnsBtnClickHandler = () => {
    setDocumentFromCashewItemsColumnsModal({
      ...documentFromCashewItemsColumnsModal,
      isOpen: true,
    })
  }

  const getPriceRebates = async () => {
    setGettingPriceRebates(true)
    const priceRebateResponse = await fetchWrapper.get(
      `PriceRebates/${params.id}`
    )

    const priceRebateData = await priceRebateResponse.json()
    setPriceRebate(priceRebateData.priceRebate)

    setPriceRebateCount({
      matchedCount: priceRebateData.matchedCount,
      unmatchedCount: priceRebateData.unmatchedCount,
    })

    const priceRebateItemsResponse = await fetchWrapper.get(
      `PriceRebates/${params.id}/Items`
    )
    const priceRebateItemsData = await priceRebateItemsResponse.json()

    console.log(priceRebateItemsData)

    setPriceRebateItemsRowData(priceRebateItemsData)

    setGettingPriceRebates(false)
  }

  const getAutoMatchProgress = async () => {
    const autoMatchProgressResponse = await fetchWrapper.post(
      `Matching/AutoMatchProgress`,
      {
        priceRebateId: id,
      }
    )

    const autoMatchProgressResponseData = await autoMatchProgressResponse.json()

    setAutoMatchProgress(autoMatchProgressResponseData)
  }

  const [
    documentFromCashewItemsColumnDefs,
    setDocumentFromCashewItemsColumnDefs,
  ] = useState([
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
      filter: "agNumberColumnFilter",
      hide: false,
      width: 150,
    },
    {
      headerName: "Unit Of Measure",
      field: "unitOfMeasure",
      filter: "agTextColumnFilter",
      hide: false,
      width: 350,
    },
    {
      headerName: "Unit Price",
      field: "unitPrice",
      cellRenderer: DecimalRenderer,
      filter: "agNumberColumnFilter",
      hide: false,
      width: 150,
    },
    {
      headerName: "Subtotal",
      field: "subtotal",
      cellRenderer: DecimalRenderer,
      filter: "agNumberColumnFilter",
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
      width: 350,
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

  const savePriceRebateItemsColumnDefs = (priceRebateItemsColumnDefs) => {
    localStorage.setItem(
      "priceRebateItemsColumnDefs",
      JSON.stringify(priceRebateItemsColumnDefs)
    )
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
    const mPriceRebateItemsColumnDefs = localStorage.getItem(
      "priceRebateItemsColumnDefs"
    )
    if (mPriceRebateItemsColumnDefs != null) {
      let columnDefs = JSON.parse(mPriceRebateItemsColumnDefs)
      let order = columnDefs.map((d) => d.headerName)
      let updatedPriceRebateItemsColumnDefs = [...priceRebateItemsColumnDefs]
      for (let i = 0; i < updatedPriceRebateItemsColumnDefs.length; i++) {
        let updatedPriceRebateItemsColumnDef =
          updatedPriceRebateItemsColumnDefs[i]
        let columnDef = columnDefs.find(
          (d) => d.field == updatedPriceRebateItemsColumnDef.field
        )
        updatedPriceRebateItemsColumnDef.hide = columnDef.hide
      }
      updatedPriceRebateItemsColumnDefs.sort(
        (a, b) => order.indexOf(a.headerName) - order.indexOf(b.headerName)
      )
      setPriceRebateItemsColumnDefs(updatedPriceRebateItemsColumnDefs)
    }

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
        updatedDocumentFromCashewItemsColumnDef.hide = columnDef.hide
      }
      updatedDocumentFromCashewItemsColumnDefs.sort(
        (a, b) => order.indexOf(a.headerName) - order.indexOf(b.headerName)
      )
      setDocumentFromCashewItemsColumnDefs(
        updatedDocumentFromCashewItemsColumnDefs
      )
    }
  }

  const [priceRebate, setPriceRebate] = useState(null)
  const [priceRebateItemsRowData, setPriceRebateItemsRowData] = useState([])
  const [documentFromCashewForm, setDocumentFromCashewForm] = useState({
    showAllUnmatched: false,
  })
  const [documentFromCashewItemsRowData, setDocumentFromCashewItemsRowData] =
    useState([])
  const [selectedMatchings, setSelectedMatchings] = useState([])

  const [
    selectedPriceRebateItemsDocumentNos,
    setSelectedPriceRebateItemsDocumentNos,
  ] = useState([])
  const [selectedPriceRebateItemsIDs, setSelectedPriceRebateItemsIDs] =
    useState([])

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

  const [autoMatchModalForm, setAutoMatchModalForm] = useState({
    isOpen: false,
  })

  const refreshPriceRebate = async () => {
    await getPriceRebates()
  }

  const deselectAllBtnClickHandler = () => {
    priceRebateItemsGridRef.current.api.deselectAll()
  }

  useEffect(() => {
    let getAutoMatchProgressIntervalId
    ;(async () => {
      await getPriceRebates()
      await loadDefaultColumnDefs()

      getAutoMatchProgressIntervalId = setInterval(async () => {
        await getAutoMatchProgress()
      }, 5000)
    })()

    return () => {
      if (getAutoMatchProgressIntervalId) {
        clearInterval(getAutoMatchProgressIntervalId)
      }
    }
  }, [])

  useEffect(() => {
    let timeoutId = null
    const controller = new AbortController()

    const runUpdateAutoMatchProgress = async () => {
      try {
        // Pass signal if getAutoMatchProgress uses fetch/axios and supports AbortSignal
        await getAutoMatchProgress({ signal: controller.signal })
      } catch (err) {
        if (err.name === "AbortError") return // intentionally aborted
        console.error(err)
      }

      // schedule next run and store id so we can clear it
      timeoutId = setTimeout(runUpdateAutoMatchProgress, 5000)
    }

    runUpdateAutoMatchProgress()

    return () => {
      // cancel any in-flight request
      controller.abort()
      // clear any pending timeout so the callback doesn't run after unmount
      if (timeoutId) clearTimeout(timeoutId)
    }
  }, [])

  return (
    <div className="flex flex-col">
      <h1 className="m-2 font-bold">Price Rebate Matching</h1>
      <div className="p-2">
        <Button onClick={() => refreshPriceRebate()}>
          {gettingPriceRebates && (
            <svg
              style={{
                width: 24,
                height: 24,
              }}
              aria-hidden="true"
              className="inline w-8 h-8 scale-50 text-gray-200 animate-spin dark:text-gray-600 fill-gray-600 dark:fill-gray-300"
              viewBox="0 0 100 101"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z"
                fill="currentColor"
              />
              <path
                d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z"
                fill="currentFill"
              />
            </svg>
          )}
          {!gettingPriceRebates && <>Refresh</>}
        </Button>
        <Button onClick={() => router.push("/priceRebates")}>Back</Button>
      </div>
      <main className="flex flex-col">
        <div className="bg-gray-100 px-3 pt-3">
          {priceRebate && (
            <>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <div className="mb-4">
                    <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                      Excel Filename
                    </label>
                    <input
                      type="text"
                      id="excelFilename"
                      className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                      value={priceRebate.excelFilename}
                      required
                      readOnly
                    />
                  </div>
                  <div className="mb-4">
                    <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                      Uploaded Date Time
                    </label>
                    <input
                      type="text"
                      id="uploadedDateTime"
                      className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                      value={DateTimeRenderer(priceRebate.uploadedDateTime)}
                      required
                      readOnly
                    />
                  </div>
                  <div className="mb-4">
                    <label className="block text-gray-700 text-sm font-bold mb-2">
                      <input
                        className="mr-2 leading-tight"
                        type="checkbox"
                        name="interests"
                        checked={priceRebate.allItemsAreMatched}
                        readOnly
                      />
                      <span className="text-sm">All Matched!</span>
                    </label>
                  </div>
                  <div className="mb-4">
                    <p>
                      <span>
                        Matched: {priceRebateCounts.matchedCount}
                        {", "}
                        Unmatched: {priceRebateCounts.unmatchedCount}
                      </span>
                    </p>
                  </div>
                  <div className="mb-4">
                    <p>
                      <span>
                        Auto Match Status: {autoMatchProgress.status}, Progress:{" "}
                        {autoMatchProgress.autoMatchProgress.toFixed(2)}%
                      </span>
                    </p>
                  </div>
                </div>
                <div>
                  <div className="mb-4">
                    <Button
                      onClick={() => priceRebateItemsColumnsBtnClickHandler()}
                    >
                      Left Columns
                    </Button>
                    <Button
                      onClick={() =>
                        documentFromCashewItemsColumnsBtnClickHandler()
                      }
                    >
                      Right Columns
                    </Button>
                  </div>
                </div>
              </div>
              <Button onClick={() => deselectAllBtnClickHandler()}>
                Deselect All
              </Button>
              <Button onClick={() => matchBtnClickHandler()}>Match!</Button>
              <Button onClick={() => unmatchBtnClickHandler()}>Unmatch!</Button>
              <Button onClick={() => autoMatchBtnClickHandler()}>
                Automatch
              </Button>
            </>
          )}
        </div>
        <div className="flex-grow flex flex-row bg-gray-100 p-2">
          <div
            className="ag-theme-alpine p-2"
            style={{ height: 600, width: "50%", position: "relative" }}
          >
            <AgGridReact
              ref={priceRebateItemsGridRef}
              columnDefs={priceRebateItemsColumnDefs}
              rowData={priceRebateItemsRowData}
              pagination={true}
              rowSelection="multiple"
              isRowSelectable={priceRebateItemsIsRowSelctable}
              onSelectionChanged={onPriceRebateSelectionChanged}
              onRowSelected={onRowSelectedPriceRebateItems}
              onRowDataUpdated={async () => {
                let documentNos = []
                const gridApi = priceRebateItemsGridRef.current.api
                gridApi.forEachNode((node) => {
                  if (selectedPriceRebateItemsIDs.includes(node.data.id)) {
                    node.setSelected(true)
                    documentNos.push(node.data.documentNo)
                  }
                })

                documentNos = documentNos.filter(
                  (value, index, array) => array.indexOf(value) === index
                )
                await getDocumentFromCashewItemsByDocumentNos(documentNos)
              }}
            ></AgGridReact>
          </div>
          <div
            className="ag-theme-alpine p-2"
            style={{ height: 600, width: "50%" }}
          >
            <AgGridReact
              ref={documentFromCashewItemsGridRef}
              columnDefs={documentFromCashewItemsColumnDefs}
              rowData={documentFromCashewItemsRowData}
              getRowStyle={getDocumentFromCashewItemsRowStyle}
              frameworkComponents={documentFromCashewItemsFrameworkComponents}
              pagination={true}
              rowSelection={documentFromCashewItemsRowSelection}
              onRowSelected={onRowSelectedDocumentFromCashewItems}
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
        footer={modalForm.footer}
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
      {priceRebateItemsColumnsModal.isOpen && (
        <div className="modal modal-open">
          <div className="modal-box">
            <div>
              <ul>
                {priceRebateItemsColumnDefs &&
                  priceRebateItemsColumnDefs.map((item, itemIndex) => (
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
                                let updatedPriceRebateItemsColumnDefs = [
                                  ...priceRebateItemsColumnDefs,
                                ]
                                updatedPriceRebateItemsColumnDefs[
                                  itemIndex
                                ].hide = false
                                setPriceRebateItemsColumnDefs(
                                  updatedPriceRebateItemsColumnDefs
                                )
                              }}
                            >
                              Show
                            </Button>
                          )}
                          {!item.hide && (
                            <Button
                              onClick={() => {
                                let updatedPriceRebateItemsColumnDefs = [
                                  ...priceRebateItemsColumnDefs,
                                ]
                                updatedPriceRebateItemsColumnDefs[
                                  itemIndex
                                ].hide = true
                                setPriceRebateItemsColumnDefs(
                                  updatedPriceRebateItemsColumnDefs
                                )
                              }}
                            >
                              Hide
                            </Button>
                          )}
                          <Button
                            onClick={() => {
                              if (itemIndex == 0) return
                              let updatedPriceRebateItemsColumnDefs = [
                                ...priceRebateItemsColumnDefs,
                              ]
                              let tmpColumnDef = {
                                ...updatedPriceRebateItemsColumnDefs[
                                  itemIndex - 1
                                ],
                              }
                              updatedPriceRebateItemsColumnDefs[itemIndex - 1] =
                                updatedPriceRebateItemsColumnDefs[itemIndex]
                              updatedPriceRebateItemsColumnDefs[itemIndex] =
                                tmpColumnDef
                              setPriceRebateItemsColumnDefs(
                                updatedPriceRebateItemsColumnDefs
                              )
                            }}
                          >
                            &uarr;
                          </Button>
                          <Button
                            onClick={() => {
                              if (
                                itemIndex >=
                                priceRebateItemsColumnDefs.length - 1
                              )
                                return
                              let updatedPriceRebateItemsColumnDefs = [
                                ...priceRebateItemsColumnDefs,
                              ]
                              let tmpColumnDef = {
                                ...updatedPriceRebateItemsColumnDefs[itemIndex],
                              }
                              updatedPriceRebateItemsColumnDefs[itemIndex] =
                                updatedPriceRebateItemsColumnDefs[itemIndex + 1]
                              updatedPriceRebateItemsColumnDefs[itemIndex + 1] =
                                tmpColumnDef
                              setPriceRebateItemsColumnDefs(
                                updatedPriceRebateItemsColumnDefs
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
                  savePriceRebateItemsColumnDefs(priceRebateItemsColumnDefs)
                }
              >
                Save as Default
              </Button>
              <Button
                onClick={() =>
                  setPriceRebateItemsColumnsModal({
                    ...priceRebateItemsColumnsModal,
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
      {autoMatchModalForm.isOpen && (
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
          </div>
        </div>
      )}
    </div>
  )
}

export default withAuth(RebateMatch)
