import { navigate } from "./navigate"

const fetchWrapper = {
  get: async (url) => {
    let xAccessToken = ""
    try {
      xAccessToken = localStorage.getItem("X-Access-Token")
    } catch {
      xAccessToken = ""
    }

    const response = await fetch(process.env.NEXT_PUBLIC_API_URL + url, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
        "X-Access-Token": xAccessToken,
      },
    })

    if (response.status == 401) {
      navigate("/account/login")
    }

    return response
  },
  post: async (url, body) => {
    let xAccessToken = ""
    try {
      xAccessToken = localStorage.getItem("X-Access-Token")
    } catch {
      xAccessToken = ""
    }
    let response
    if (body instanceof FormData) {
      console.log(process.env.NEXT_PUBLIC_API_URL + url)
      response = await fetch(process.env.NEXT_PUBLIC_API_URL + url, {
        method: "POST",
        headers: {
          "Access-Control-Allow-Origin": "*",
          "X-Access-Token": xAccessToken,
        },
        body: body,
      })
    } else {
      response = await fetch(process.env.NEXT_PUBLIC_API_URL + url, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Access-Control-Allow-Origin": "*",
          "X-Access-Token": xAccessToken,
        },
        body: JSON.stringify(body),
      })
    }

    if (response.status == 401) {
      navigate("/account/login")
    }

    return response
  },
  put: async (url, body) => {
    let xAccessToken = ""
    try {
      xAccessToken = localStorage.getItem("X-Access-Token")
    } catch {
      xAccessToken = ""
    }
    const response = await fetch(process.env.NEXT_PUBLIC_API_URL + url, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
        "X-Access-Token": xAccessToken,
      },
      body: JSON.stringify(body),
    })

    if (response.status == 401) {
      navigate("/account/login")
    }

    return response
  },
  delete: async (url, body) => {
    let xAccessToken = ""
    console.log(body)
    try {
      xAccessToken = localStorage.getItem("X-Access-Token")
    } catch {
      xAccessToken = ""
    }
    await fetch(process.env.NEXT_PUBLIC_API_URL + url, {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
        "X-Access-Token": xAccessToken,
      },
      body: JSON.stringify(body),
    }).then(response => {
      if (response.status == 401) {
        navigate("/account/login")
      }
      return response
    }).catch(error => {
      console.error(error)
    })

    
  },
}
export default fetchWrapper
