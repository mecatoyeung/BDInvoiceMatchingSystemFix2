import React from "react"

class ColumnsToolPanel extends React.Component {
  constructor(props) {
    super(props)
    this.toggleColumn = this.toggleColumn.bind(this)
  }

  toggleColumn() {
    const { api } = this.props
    const column = api
      .getColumnApi()
      .getColumnState()
      .find((col) => col.colId === "yourColumnName")
    if (column) {
      api.getColumnApi().setColumnVisible(column.colId, !column.hide)
    }
  }

  render() {
    return (
      <div>
        <button onClick={this.toggleColumn}>Toggle Column Visibility</button>
      </div>
    )
  }
}

export default ColumnsToolPanel
