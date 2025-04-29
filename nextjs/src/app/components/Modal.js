import React from "react"

const Modal = ({
  isOpen,
  onClose,
  content,
  footer = (
    <>
      <button onClick={onClose} className="btn">
        Close
      </button>
    </>
  ),
}) => {
  return (
    <>
      {isOpen && (
        <div className="modal modal-open">
          <div className="modal-box">
            {content}
            <div className="modal-action">{footer}</div>
          </div>
        </div>
      )}
    </>
  )
}

export default Modal
