"use client";

import { useParams, useRouter } from "next/navigation";

import fetchWrapper from "@/app/helpers/fetchWrapper";
import { useEffect, useState } from "react";

import { AgGridReact } from "ag-grid-react";

import Button from "@/app/helpers/button";
import Modal from "@/app/components/Modal";
import DateTimeRenderer from "@/app/helpers/datetimeRenderer";

import withAuth from "@/app/components/withAuth";

const documentClasses = [
  {
    value: 1,
    label: "Invoice",
  },
  {
    value: 2,
    label: "Delivery Note",
  },
];

function FileSource() {
  const params = useParams();

  const router = useRouter();

  const [fileSource, setFileSource] = useState(null);
  const [responseMessage, setResponseMessage] = useState({
    type: "Success",
    message: "",
  });

  const getFileSource = async () => {
    const response = await fetchWrapper.get(`FileSources/${params.id}`);
    const data = await response.json();
    setFileSource(data);
  };

  const saveBtnClickHandler = async () => {
    setResponseMessage(null);
    const response = await fetchWrapper.put(`FileSources/${params.id}`, {
      ...fileSource,
    });
    if (response.ok) {
      setResponseMessage({
        type: "success",
        message: "File Source is updated successfully!",
      });
    }
  };

  useEffect(() => {
    (async () => {
      getFileSource();
    })();
  }, []);

  return (
    <div>
      <div className="flex items-center justify-center flex-col">
        <div className="p-2 container">
          <h2 className="m-2 font-bold">File Sources</h2>
          <div className="p-2">
            <Button onClick={saveBtnClickHandler}>Save</Button>
          </div>
          {responseMessage && responseMessage.type == "success" && (
            <div className="alert alert-info p-2">
              {responseMessage.message}
            </div>
          )}
        </div>
      </div>
      {fileSource && (
        <div className="flex items-center justify-center flex-col">
          <div className="p-2 container">
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Folder Path
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.folderPath}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    folderPath: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Document Class
              </label>
              <select
                type="text"
                id="documentClass"
                className="form-control bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={documentClasses.find(
                  (c) => c.value == fileSource.documentClass
                )}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    documentClass: e.value,
                  });
                }}
                readOnly
              >
                <option value="1">Invoice</option>
                <option value="2">Delivery Note</option>
              </select>
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Document No Col Name
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.documentNoColName}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    documentNoColName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Document Date Col Name
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.documentDateColName}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    documentDateColName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Delivery Date Col Name
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.deliveryDateColName}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    deliveryDateColName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Customer Code Col Name
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.customerCodeColName}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    customerCodeColName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Customer Name Col Name
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.customerNameColName}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    customerNameColName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Customer Address Col Name
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.customerAddressColName}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    customerAddressColName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Stock Code Col Name
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.stockCodeColName}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    stockCodeColName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Description Col Name
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.descriptionColName}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    descriptionColName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Lot No Col Name
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.lotNoColName}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    lotNoColName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Quantity Col Name
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.quantityColName}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    quantityColName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Unit of Measure Col Name
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.unitOfMeasureColName}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    unitOfMeasureColName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Unit Price Col Name
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.unitPriceColName}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    unitPriceColName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Subtotal Col Name
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.subtotalColName}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    subtotalColName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Filename Col Name
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.filenameColName}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    filenameColName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block text-gray-700 text-sm font-bold mb-2">
                <input
                  className="mr-2 leading-tight"
                  type="checkbox"
                  name="firstRowIsHeader"
                  checked={fileSource.firstRowIsHeader}
                  onChange={(e) => {
                    setFileSource({
                      ...fileSource,
                      firstRowIsHeader: !fileSource.firstRowIsHeader,
                    });
                  }}
                />
                <span className="text-sm">First Row is Header</span>
              </label>
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Interval in seconds
              </label>
              <input
                type="number"
                id="intervalInSeconds"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={fileSource.intervalInSeconds}
                onChange={(e) => {
                  setFileSource({
                    ...fileSource,
                    intervalInSeconds: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Next Capture Date Time
              </label>
              <input
                type="text"
                id="nextCaptureDateTime"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={DateTimeRenderer(fileSource.nextCaptureDateTime)}
                required
                readOnly
              />
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default withAuth(FileSource);
