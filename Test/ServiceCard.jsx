/* eslint-disable react/prop-types */

import { Card } from "@/components/ui/card";
import { Button } from "./ui/button";
import serviceImg from "../assets/images/main-img.jpg";

const ServiceCard = ({
  petId,
  petName,
  petHeight,
  petWeight,
  petBirthday,
  onUpdateClick,
  onDeleteClick,
  image,
  petType,
}) => {
  const imgStyles = {
    width: "500px", // Set the desired width
    height: "200px", // Set the desired height
    objectFit: "cover", // Ensure the image covers the area without distortion
    borderRadius: "8px", // Add border-radius if needed
  };

  return (
    <Card className="montserrat-regular bg-gray-200 md:w-75 lg:w-75">
      <div>
        <img src={image ? image : serviceImg} alt={petName} style={imgStyles} />
      </div>

      <div className="h-auto flex flex-col py-4 px-4 gap-y-2">
        <div className="text-xs montserrat-sb">
          <span className="font-normal">Name:</span>{" "}
          <span className="text-base">{petName}</span>
        </div>

        <div className="text-xs montserrat-sb">
          <span className="font-normal">Height-Weight:</span>{" "}
          <span className="text-base">
            {petHeight}mm-{petWeight}kg
          </span>
        </div>
        <div className="text-xs montserrat-sb">
          <span className="font-normal">Birthday:</span>{" "}
          <span className="text-base">{petBirthday}</span>
        </div>
        <div className="text-xs montserrat-sb">
          <span className="font-normal">Type:</span>{" "}
          <span className="text-base">{petType}</span>
        </div>
        <div className="flex gap-x-2">
          <Button
            onClick={() => onUpdateClick(petId)}
            className="w-1/2 bg-white border-[0.5px] border-black text-black rounded-full text-xs"
          >
            Update
          </Button>
          <Button
            onClick={() => onDeleteClick(petId)}
            className="w-1/2 bg-white border-[0.5px] border-black text-black rounded-full text-xs"
          >
            Delete
          </Button>
        </div>
      </div>
    </Card>
  );
};

export default ServiceCard;
