import React from "react"
import moment from "moment"
const DateTimeRenderer = (props) => {
  return moment(props.value).format("YYYY-MM-DD hh:mm:ss a")
}
export default DateTimeRenderer
