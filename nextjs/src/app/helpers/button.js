import React from "react"

const Button = ({ children, onClick, variant = "primary" }) => {
  const baseStyles = "mr-2 px-4 py-2 font-semibold text-white rounded"
  const variantStyles = {
    primary: "bg-blue-500 hover:bg-blue-700",
    secondary: "bg-gray-500 hover:bg-gray-700",
    danger: "bg-red-500 hover:bg-red-700",
    warning: "bg-yellow-500 hover:bg-yellow-600 text-white font-semibold py-2 px-4 rounded shadow"
  }
  return (
    <button
      onClick={onClick}
      className={`${baseStyles} ${variantStyles[variant]}`}
    >
      {" "}
      {children}{" "}
    </button>
  )
}
export default Button
