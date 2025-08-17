"use client";

import { useParams, useRouter } from "next/navigation";

import fetchWrapper from "@/app/helpers/fetchWrapper";
import { useEffect, useState } from "react";

import { AgGridReact } from "ag-grid-react";

import Button from "@/app/helpers/button";
import Modal from "@/app/components/Modal";
import DateTimeRenderer from "@/app/helpers/datetimeRenderer";

import withAuth from "../components/withAuth";

function Settings() {
  const params = useParams();

  const router = useRouter();

  const [priceRebateSetting, setPriceRebateSetting] = useState(null);
  const [responseMessage, setResponseMessage] = useState({
    type: "Success",
    message: "",
  });

  const getPriceRebateSetting = async () => {
    const response = await fetchWrapper.get(`PriceRebateSetting`);
    const data = await response.json();
    setPriceRebateSetting(data);
  };

  const saveBtnClickHandler = async () => {
    setResponseMessage(null);
    const response = await fetchWrapper.put(`PriceRebateSetting`, {
      ...priceRebateSetting,
    });
    if (response.ok) {
      setResponseMessage({
        type: "success",
        message: "Price Rebate Setting is updated successfully!",
      });
    }
  };

  useEffect(() => {
    (async () => {
      getPriceRebateSetting();
    })();
  }, []);

  return (
    <div>
      <div className="flex items-center justify-center flex-col">
        <div className="p-2 container">
          <h2 className="m-2 font-bold">Price Rebate Setting</h2>
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
      {priceRebateSetting && (
        <div className="flex items-center justify-center flex-col">
          <div className="p-2 container">
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Document No Header Name
              </label>
              <input
                type="text"
                id="documentNoHeaderName"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={priceRebateSetting.documentNoHeaderName}
                onChange={(e) => {
                  setPriceRebateSetting({
                    ...priceRebateSetting,
                    documentNoHeaderName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Stock Code Header Name
              </label>
              <input
                type="text"
                id="stockCodeHeaderName"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={priceRebateSetting.stockCodeHeaderName}
                onChange={(e) => {
                  setPriceRebateSetting({
                    ...priceRebateSetting,
                    stockCodeHeaderName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Description Header Name
              </label>
              <input
                type="text"
                id="descriptionHeaderName"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={priceRebateSetting.descriptionHeaderName}
                onChange={(e) => {
                  setPriceRebateSetting({
                    ...priceRebateSetting,
                    descriptionHeaderName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Quantity Header Name
              </label>
              <input
                type="text"
                id="folderPath"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={priceRebateSetting.quantityHeaderName}
                onChange={(e) => {
                  setPriceRebateSetting({
                    ...priceRebateSetting,
                    quantityHeaderName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Unit Price Header Name
              </label>
              <input
                type="text"
                id="unitPriceHeaderName"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={priceRebateSetting.unitPriceHeaderName}
                onChange={(e) => {
                  setPriceRebateSetting({
                    ...priceRebateSetting,
                    unitPriceHeaderName: e.target.value,
                  });
                }}
              />
            </div>
            <div className="mb-4">
              <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Subtotal in USD Header Name
              </label>
              <input
                type="text"
                id="subtotalInUSDHeaderName"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                value={priceRebateSetting.subtotalInUSDHeaderName}
                onChange={(e) => {
                  setPriceRebateSetting({
                    ...priceRebateSetting,
                    subtotalInUSDHeaderName: e.target.value,
                  });
                }}
              />
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default withAuth(Settings);
