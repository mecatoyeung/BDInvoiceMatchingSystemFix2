"use client"

import { useParams, useRouter } from "next/navigation"

import fetchWrapper from "@/app/helpers/fetchWrapper"
import { useEffect, useState } from "react"

import Button from "@/app/helpers/button"

export default function Login() {
  const params = useParams()

  const router = useRouter()

  const [loginForm, setLoginForm] = useState({
    email: "",
    password: "",
    status: "initial",
    message: "",
  })

  const loginBtnClickHandler = async () => {
    const response = await fetchWrapper.post("Account/Authenticate", {
      ...loginForm,
    })

    if (response.ok) {
      const data = await response.json()
      localStorage.setItem("Username", data.username)
      localStorage.setItem("X-Access-Token", data.token)
      window.location.href = "/"
    } else {
      const data = await response.json()
      setLoginForm({
        ...loginForm,
        status: "error",
        message: data.message,
      })
    }
  }

  const registerBtnClickHandler = async () => {
    router.push("/account/register")
  }

  useEffect(() => {
    ;(async () => {})()
  }, [])

  return (
    <div className="flex flex-col">
      <h1 className="m-2 font-bold text-center">Account Login</h1>
      <main className="flex flex-col" style={{ alignItems: "center" }}>
        <div
          className="flex-grow flex flex-col bg-gray-800 p-2"
          style={{ width: 480 }}
        >
          <label className="input input-bordered flex items-center gap-2 m-2">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 16 16"
              fill="currentColor"
              className="h-4 w-4 opacity-70"
            >
              <path d="M2.5 3A1.5 1.5 0 0 0 1 4.5v.793c.026.009.051.02.076.032L7.674 8.51c.206.1.446.1.652 0l6.598-3.185A.755.755 0 0 1 15 5.293V4.5A1.5 1.5 0 0 0 13.5 3h-11Z" />
              <path d="M15 6.954 8.978 9.86a2.25 2.25 0 0 1-1.956 0L1 6.954V11.5A1.5 1.5 0 0 0 2.5 13h11a1.5 1.5 0 0 0 1.5-1.5V6.954Z" />
            </svg>
            <input
              type="text"
              className="grow"
              placeholder="Email"
              value={loginForm.email}
              onChange={(e) => {
                setLoginForm({
                  ...loginForm,
                  email: e.target.value,
                })
              }}
            />
          </label>
          <label className="input input-bordered flex items-center gap-2 m-2">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 16 16"
              fill="currentColor"
              className="h-4 w-4 opacity-70"
            >
              <path
                fillRule="evenodd"
                d="M14 6a4 4 0 0 1-4.899 3.899l-1.955 1.955a.5.5 0 0 1-.353.146H5v1.5a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1-.5-.5v-2.293a.5.5 0 0 1 .146-.353l3.955-3.955A4 4 0 1 1 14 6Zm-4-2a.75.75 0 0 0 0 1.5.5.5 0 0 1 .5.5.75.75 0 0 0 1.5 0 2 2 0 0 0-2-2Z"
                clipRule="evenodd"
              />
            </svg>
            <input
              type="password"
              className="grow"
              placeholder="Password"
              value={loginForm.password}
              onChange={(e) => {
                setLoginForm({
                  ...loginForm,
                  password: e.target.value,
                })
              }}
            />
          </label>
          {loginForm.status == "error" && (
            <p className="bg-gray-200 text-error m-2 p-2">
              {loginForm.message}
            </p>
          )}
          {loginForm.status == "success" && (
            <p className="bg-gray-200 text-success m-2 p-2">
              {loginForm.message}
            </p>
          )}
          <button
            className="btn btn-primary m-2"
            onClick={loginBtnClickHandler}
          >
            Sign In
          </button>
          <button className="btn m-2" onClick={registerBtnClickHandler}>
            Register
          </button>
        </div>
      </main>
    </div>
  )
}
