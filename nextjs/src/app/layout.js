"use client"

import { useState, useEffect } from "react"

import { useRouter } from "next/navigation"
import Link from "next/link"

import localFont from "next/font/local"

import { isBrowser } from "./helpers/isBrowser"

import "./globals.css"

const geistSans = localFont({
  src: "./fonts/GeistVF.woff",
  variable: "--font-geist-sans",
  weight: "100 900",
})
const geistMono = localFont({
  src: "./fonts/GeistMonoVF.woff",
  variable: "--font-geist-mono",
  weight: "100 900",
})

export default function RootLayout({ children }) {
  const router = useRouter()

  const [username, setUsername] = useState("")

  const logoutBtnClickHandler = () => {
    if (isBrowser()) {
      localStorage.removeItem("Username")
      localStorage.removeItem("X-Access-Token")
      setUsername("")
      router.push("/account/login")
      router.refresh()
    }
  }

  const loginBtnClickHandler = () => {
    if (isBrowser()) {
      router.push("/account/login")
    }
  }

  useEffect(() => {
    setUsername(localStorage.getItem("Username") ?? "")
  }, [router.isReady])

  return (
    <html lang="en" suppressHydrationWarning>
      <head>
        <title>BD Invoice Matching System</title>
        <link rel="preconnect" href="https://fonts.googleapis.com" />
        <link
          rel="preconnect"
          href="https://fonts.gstatic.com"
          crossOrigin="true"
        />
        <link
          href="https://fonts.googleapis.com/css2?family=Roboto:ital,wght@0,100;0,300;0,400;0,500;0,700;0,900;1,100;1,300;1,400;1,500;1,700;1,900&display=swap"
          rel="stylesheet"
        />
        <link
          href="https://cdn.jsdelivr.net/npm/ag-grid-community/dist/styles/ag-grid.css"
          rel="stylesheet"
        />
        <link
          href="https://cdn.jsdelivr.net/npm/ag-grid-community/dist/styles/ag-theme-alpine.css"
          rel="stylesheet"
        />
      </head>
      <body
        className={`${geistSans.variable} ${geistMono.variable} antialiased`}
      >
        <div className="min-h-screen flex flex-col">
          <header className="bg-gray-800 text-white py-2 p-2">
            <nav className="bg-gray-800">
              <div className="container mx-auto flex items-center justify-between">
                <div className="text-white text-xl font-bold">
                  BD Invoice Matching System
                </div>
                <ul className="flex space-x-4"></ul>
                <ul className="menu lg:menu-horizontal bg-gray-800 rounded-box">
                  <li>
                    <Link href="/" className="text-white hover:text-gray-400">
                      Home
                    </Link>
                  </li>
                  <li>
                    <Link
                      href="/priceRebates"
                      className="text-white hover:text-gray-400"
                    >
                      Price Rebates
                    </Link>
                  </li>
                  <li>
                    <Link
                      href="/documentsFromCashew"
                      className="text-white hover:text-gray-400"
                    >
                      Documents from Cashew
                    </Link>
                  </li>
                  <li>
                    <Link
                      href="/fileSources"
                      className="text-white hover:text-gray-400"
                    >
                      File Sources
                    </Link>
                  </li>
                  <li>
                    <Link
                      href="/settings"
                      className="text-white hover:text-gray-400"
                    >
                      Settings
                    </Link>
                  </li>
                  <li>
                    <Link href="#" className="text-white hover:text-gray-400">
                      About
                    </Link>
                  </li>
                  <li>
                    <div className="dropdown dropdown-hover">
                      {username && (
                        <>
                          <div tabIndex="0" role="button" className="btn m-1">
                            Welcome, {username}
                          </div>
                          <ul
                            tabIndex="0"
                            className="bg-gray-800 dropdown-content menu rounded-box z-[1] p-2 shadow"
                            style={{
                              lineHeight: "64px",
                              textAlign: "center",
                              width: "100%",
                            }}
                          >
                            <li
                              className="cursor-pointer"
                              onClick={logoutBtnClickHandler}
                            >
                              Logout
                            </li>
                          </ul>
                        </>
                      )}
                      {!username && (
                        <>
                          <div
                            tabIndex="0"
                            role="button"
                            className="btn m-1 cursor-pointer"
                            onClick={loginBtnClickHandler}
                          >
                            Login
                          </div>
                        </>
                      )}
                    </div>
                  </li>
                </ul>
              </div>
            </nav>
          </header>

          <main className="flex-grow mx-auto py-4" style={{ width: "100%" }}>
            {children}
          </main>

          <footer className="bg-gray-800 text-white py-4 p-4">
            <div className="container mx-auto">
              <p>Copyright 2024 @ Sonik Global Limited</p>
            </div>
          </footer>
        </div>
      </body>
    </html>
  )
}
