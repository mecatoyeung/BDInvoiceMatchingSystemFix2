import React from "react"
import moment from "moment"
const DateRenderer = (props) => {
  return moment(props.value).format("YYYY-MM-DD")
}
export default DateRenderer
