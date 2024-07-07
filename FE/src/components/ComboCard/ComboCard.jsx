import { useState } from "react";
import BookingCombo from "../BookingCombo";
import { CircleCheck } from "lucide-react";
import ProtectedRoute from "../ProtectedRoute";

const ComboCard = ({ combo }) => {
  const [isOpen, setIsOpen] = useState(false);
  const [isHover, setIsHover] = useState(false);
  const [selectComboId, setSelectComboId] = useState();
  const handleOpenModal = (comboId) => {
    setIsOpen(true);
    setSelectComboId(comboId);
  };
  const handleHideModal = () => setIsOpen(false);
  const formatPrice = (price) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(price);
  };
  return (
    <div className="col col-lg-3 col-md-6 col-sm-6 cursor-pointer">
      <div
        onClick={() => handleOpenModal(combo.comboId)}
        className={`${
          isHover ? "bg-purple-600 text-white shadow-xl" : ""
        } p-4 h-[350px] border-2 rounded-lg duration-300 ease-in-out`}
        onMouseEnter={() => setIsHover(true)}
        onMouseLeave={() => setIsHover(false)}
      >
        <h3 className={`${isHover ? "text-white" : ""}`}>{combo.comboType}</h3>
        <div
          className={`${
            isHover
              ? "border-purple-600 text-purple-600 bg-white"
              : "bg-yellow-500 text-white"
          } text-xl my-5 rounded-lg border-2 py-2 text-center shadow-xl`}
        >
          <span className="value_text">{formatPrice(combo.price)}</span>
        </div>
        <div className="w-full">
          {combo.services.map((service, idx) => (
            <div key={idx} className="flex w-full items-center gap-x-3 ">
              <CircleCheck size={16} />
              <div>
                {service.included ? (
                  <span>{service.serviceName}</span>
                ) : (
                  <div>
                    <span>{service.serviceName}</span>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      </div>
      {isOpen && selectComboId === combo.comboId && (
        <ProtectedRoute allowedRoles={["Customer"]}>
          <BookingCombo
            isOpen={isOpen}
            handleHideModal={handleHideModal}
            comboId={selectComboId}
          />
        </ProtectedRoute>
      )}
    </div>
  );
};

export default ComboCard;
